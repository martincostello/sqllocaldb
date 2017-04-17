param(
    [Parameter(Mandatory=$false)][bool]   $RestorePackages  = $false,
    [Parameter(Mandatory=$false)][string] $Configuration    = "Release",
    [Parameter(Mandatory=$false)][string] $VersionSuffix    = "",
    [Parameter(Mandatory=$false)][string] $OutputPath       = "",
    [Parameter(Mandatory=$false)][bool]   $PatchVersion     = $false,
    [Parameter(Mandatory=$false)][bool]   $RunTests         = $true,
    [Parameter(Mandatory=$false)][bool]   $CreatePackages   = $true
)

$ErrorActionPreference = "Stop"

$solutionPath  = Split-Path $MyInvocation.MyCommand.Definition
$solutionFile  = Join-Path $solutionPath "SqlLocalDb.sln"
$dotnetVersion = "1.0.1"

if ($OutputPath -eq "") {
    $OutputPath = Join-Path "$(Convert-Path "$PSScriptRoot")" "artifacts"
}

$env:DOTNET_INSTALL_DIR = "$(Convert-Path "$PSScriptRoot")\.dotnetcli"

if ($env:CI -ne $null) {

    $RestorePackages = $true
    $PatchVersion = $true

    if (($VersionSuffix -eq "" -and $env:APPVEYOR_REPO_TAG -eq "false" -and $env:APPVEYOR_BUILD_NUMBER -ne "") -eq $true) {

        $LastVersionBuild = (Get-Content ".\releases.txt" | Select-Object -Last 1)
        $LastVersion = New-Object -TypeName System.Version -ArgumentList $LastVersionBuild
        $ThisVersion = $env:APPVEYOR_BUILD_NUMBER -as [int]

        if ($LastVersion.Revision -eq 0) {
            $ThisBuildNumber = $ThisVersion - $LastVersion.Build
        } else {
            $ThisBuildNumber = $ThisVersion - $LastVersion.Revision
        }

        $VersionSuffix = "beta" + $ThisBuildNumber.ToString("0000")
    }
}

$installDotNetSdk = $false;

if (((Get-Command "dotnet.exe" -ErrorAction SilentlyContinue) -eq $null) -and ((Get-Command "dotnet" -ErrorAction SilentlyContinue) -eq $null))  {
    Write-Host "The .NET Core SDK is not installed."
    $installDotNetSdk = $true
}
else {
    $installedDotNetVersion = (dotnet --version | Out-String).Trim()
    if ($installedDotNetVersion -ne $dotnetVersion) {
        Write-Host "The required version of the .NET Core SDK is not installed. Expected $dotnetVersion but $installedDotNetVersion was found."
        $installDotNetSdk = $true
    }
}

if ($installDotNetSdk -eq $true) {
    $env:DOTNET_INSTALL_DIR = Join-Path "$(Convert-Path "$PSScriptRoot")" ".dotnetcli"

    if (!(Test-Path $env:DOTNET_INSTALL_DIR)) {
        mkdir $env:DOTNET_INSTALL_DIR | Out-Null
        $installScript = Join-Path $env:DOTNET_INSTALL_DIR "install.ps1"
        Invoke-WebRequest "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/dotnet-install.ps1" -OutFile $installScript
        & $installScript -Version "$dotnetVersion" -InstallDir "$env:DOTNET_INSTALL_DIR" -NoPath
    }

    $env:PATH = "$env:DOTNET_INSTALL_DIR;$env:PATH"
    $dotnet   = Join-Path "$env:DOTNET_INSTALL_DIR" "dotnet"
} else {
    $dotnet   = "dotnet"
}

function DotNetRestore {
    param([string]$Project)
    & $dotnet restore $Project --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet restore failed with exit code $LASTEXITCODE"
    }
}

function DotNetBuild {
    param([string]$Project, [string]$Configuration, [string]$VersionSuffix)
    if ($VersionSuffix) {
        & $dotnet build $Project --output $OutputPath --configuration $Configuration --version-suffix "$VersionSuffix"
    } else {
        & $dotnet build $Project --output $OutputPath --configuration $Configuration
    }
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed with exit code $LASTEXITCODE"
    }
}

function DotNetTest {
    param([string]$Project)
    & $dotnet test $Project
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet test failed with exit code $LASTEXITCODE"
    }
}

function DotNetPack {
    param([string]$Project, [string]$Configuration, [string]$VersionSuffix)
    if ($VersionSuffix) {
        & $dotnet pack $Project --output $OutputPath --configuration $Configuration --version-suffix "$VersionSuffix" --include-symbols --include-source
    } else {
        & $dotnet pack $Project --output $OutputPath --configuration $Configuration --include-symbols --include-source
    }
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet pack failed with exit code $LASTEXITCODE"
    }
}

if ($PatchVersion -eq $true) {

    $gitRevision = (git rev-parse HEAD | Out-String).Trim()
    $gitBranch   = (git rev-parse --abbrev-ref HEAD | Out-String).Trim()
    $timestamp   = [DateTime]::UtcNow.ToString("yyyy-MM-ddTHH:mm:ssK")

    $assemblyVersion = Get-Content ".\AssemblyVersion.cs" -Raw
    $assemblyVersionWithMetadata = "{0}[assembly: AssemblyMetadata(""CommitHash"", ""{1}"")]`r`n[assembly: AssemblyMetadata(""CommitBranch"", ""{2}"")]`r`n[assembly: AssemblyMetadata(""BuildTimestamp"", ""{3}"")]" -f $assemblyVersion, $gitRevision, $gitBranch, $timestamp

    Set-Content ".\AssemblyVersion.cs" $assemblyVersionWithMetadata -Encoding utf8
}

$testProjects = @(
    (Join-Path $solutionPath "tests\SqlLocalDb.UnitTests\System.Data.SqlLocalDb.UnitTests.csproj")
)

$packageProjects = @(
    (Join-Path $solutionPath "src\SqlLocalDb\System.Data.SqlLocalDb.csproj")
)

if ($RestorePackages -eq $true) {
    Write-Host "Restoring NuGet packages for solution..." -ForegroundColor Green
    DotNetRestore $solutionFile
}

Write-Host "Building solution..." -ForegroundColor Green
DotNetBuild $solutionFile $Configuration $VersionSuffix

if ($RunTests -eq $true) {
    Write-Host "Testing $($testProjects.Count) project(s)..." -ForegroundColor Green
    ForEach ($project in $testProjects) {
        DotNetTest $project
    }
}

if ($CreatePackages -eq $true) {
    Write-Host "Creating $($packageProjects.Count) package(s)..." -ForegroundColor Green
    ForEach ($project in $packageProjects) {
        DotNetPack $project $Configuration $VersionSuffix
    }
}

if ($PatchVersion -eq $true) {
    Set-Content ".\AssemblyVersion.cs" $assemblyVersion.Trim() -Encoding utf8
}
