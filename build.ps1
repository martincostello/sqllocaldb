#! /usr/bin/env pwsh

#Requires -PSEdition Core
#Requires -Version 7

param(
    [Parameter(Mandatory = $false)][string] $Configuration = "Release",
    [Parameter(Mandatory = $false)][string] $OutputPath = ""
)

$env:DOTNET_MULTILEVEL_LOOKUP = "0"
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = "true"
$env:NUGET_XMLDOC_MODE = "skip"

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

$solutionPath = Split-Path $MyInvocation.MyCommand.Definition
$sdkFile = (Join-Path $solutionPath "global.json")

$packageProjects = @(
    (Join-Path $solutionPath "src" "SqlLocalDb" "MartinCostello.SqlLocalDb.csproj")
)

$testProjects = @(
    (Join-Path $solutionPath "tests" "SqlLocalDb.Tests" "MartinCostello.SqlLocalDb.Tests.csproj"),
    (Join-Path $solutionPath "samples" "TodoApp.Tests" "TodoApp.Tests.csproj")
)

$dotnetVersion = (Get-Content $sdkFile | Out-String | ConvertFrom-Json).sdk.version

if ($OutputPath -eq "") {
    $OutputPath = (Join-Path $solutionPath "artifacts")
}

$installDotNetSdk = $false;

if (($null -eq (Get-Command "dotnet" -ErrorAction SilentlyContinue)) -and ($null -eq (Get-Command "dotnet.exe" -ErrorAction SilentlyContinue))) {
    Write-Host "The .NET SDK is not installed."
    $installDotNetSdk = $true
}
else {
    Try {
        $installedDotNetVersion = (dotnet --version 2>&1 | Out-String).Trim()
    }
    Catch {
        $installedDotNetVersion = "?"
    }

    if ($installedDotNetVersion -ne $dotnetVersion) {
        Write-Host "The required version of the .NET SDK is not installed. Expected $dotnetVersion."
        $installDotNetSdk = $true
    }
}

if ($installDotNetSdk -eq $true) {

    $env:DOTNET_INSTALL_DIR = (Join-Path $solutionPath ".dotnetcli")
    $sdkPath = (Join-Path $env:DOTNET_INSTALL_DIR "sdk" $dotnetVersion)

    if (!(Test-Path $sdkPath)) {
        if (!(Test-Path $env:DOTNET_INSTALL_DIR)) {
            mkdir $env:DOTNET_INSTALL_DIR | Out-Null
        }
        [Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor "Tls12"

        if (($PSVersionTable.PSVersion.Major -ge 6) -And !$IsWindows) {
            $installScript = (Join-Path $env:DOTNET_INSTALL_DIR "install.sh")
            Invoke-WebRequest "https://dot.net/v1/dotnet-install.sh" -OutFile $installScript -UseBasicParsing
            chmod +x $installScript
            & $installScript --version "$dotnetVersion" --install-dir "$env:DOTNET_INSTALL_DIR" --no-path
        }
        else {
            $installScript = (Join-Path $env:DOTNET_INSTALL_DIR "install.ps1")
            Invoke-WebRequest "https://dot.net/v1/dotnet-install.ps1" -OutFile $installScript -UseBasicParsing
            & $installScript -Version "$dotnetVersion" -InstallDir "$env:DOTNET_INSTALL_DIR" -NoPath
        }
    }
}
else {
    $env:DOTNET_INSTALL_DIR = Split-Path -Path (Get-Command dotnet).Path
}

$dotnet = (Join-Path "$env:DOTNET_INSTALL_DIR" "dotnet")

if ($installDotNetSdk -eq $true) {
    $env:PATH = "$env:DOTNET_INSTALL_DIR;$env:PATH"
}

function DotNetPack {
    param([string]$Project)

    $PackageOutputPath = (Join-Path $OutputPath "packages")

    & $dotnet pack `
        $Project `
        --output $PackageOutputPath `
        --configuration $Configuration `
        --include-source `
        --include-symbols

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet pack failed with exit code $LASTEXITCODE"
    }
}

function DotNetTest {
    param([string]$Project)

    $packagesRoot = $env:USERPROFILE

    if ($null -eq $packagesRoot) {
        $packagesRoot = "~"
    }

    $nugetPath = (Join-Path $packagesRoot ".nuget" "packages")
    $propsFile = (Join-Path $solutionPath "Directory.Packages.props")
    $reportGeneratorVersion = (Select-Xml -Path $propsFile -XPath "//PackageVersion[@Include='ReportGenerator']/@Version").Node.'#text'
    $reportGeneratorPath = (Join-Path $nugetPath "reportgenerator" $reportGeneratorVersion "tools" "net6.0" "ReportGenerator.dll")

    $coverageOutput = (Join-Path $OutputPath "coverage.*.cobertura.xml")
    $reportOutput = (Join-Path $OutputPath "coverage")

    $additionalArgs = @()

    if (![string]::IsNullOrEmpty($env:GITHUB_SHA)) {
        $additionalArgs += "--logger"
        $additionalArgs += "GitHubActions;report-warnings=false"
    }

    & $dotnet test `
        $Project `
        --output $OutputPath `
        --configuration $Configuration `
        $additionalArgs

    $dotNetTestExitCode = $LASTEXITCODE

    if (Test-Path $coverageOutput) {
        & $dotnet `
            $reportGeneratorPath `
            `"-reports:$coverageOutput`" `
            `"-targetdir:$reportOutput`" `
            -reporttypes:HTML `
            -verbosity:Warning
    }

    if ($dotNetTestExitCode -ne 0) {
        throw "dotnet test failed with exit code $dotNetTestExitCode"
    }
}

Write-Host "Building $($packageProjects.Count) NuGet package(s)..." -ForegroundColor Green
ForEach ($project in $packageProjects) {
    DotNetPack $project
}

Write-Host "Testing $($testProjects.Count) project(s)..." -ForegroundColor Green
Remove-Item -Path (Join-Path $OutputPath "coverage.*.json") -Force -ErrorAction SilentlyContinue | Out-Null
ForEach ($project in $testProjects) {
    DotNetTest $project
}
