@ECHO OFF

"%ProgramFiles(x86)%\MSBuild\14.0\Bin\msbuild" "%~dp0\SqlLocalDb.msbuild" /v:minimal /maxcpucount /nodeReuse:false %*