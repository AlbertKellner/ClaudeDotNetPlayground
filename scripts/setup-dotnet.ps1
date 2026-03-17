param(
    [string]$DotnetVersion = "10.0.100",
    [string]$InstallDir = "$HOME\.dotnet"
)

$ErrorActionPreference = "Stop"

New-Item -ItemType Directory -Force -Path $InstallDir | Out-Null

Write-Host "Instalando .NET SDK $DotnetVersion em: $InstallDir"

$scriptPath = "$env:TEMP\dotnet-install.ps1"
Invoke-WebRequest -Uri "https://dot.net/v1/dotnet-install.ps1" -OutFile $scriptPath
& $scriptPath -Version $DotnetVersion -InstallDir $InstallDir

$env:DOTNET_ROOT = $InstallDir
$env:PATH = "$InstallDir;$InstallDir\tools;$env:PATH"

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet não encontrado no PATH após instalação."
}

Write-Host ""
Write-Host "SDKs instalados:"
dotnet --list-sdks

$hasDotnet10 = dotnet --list-sdks | Select-String '^10\.'
if (-not $hasDotnet10) {
    throw "Nenhum SDK .NET 10 encontrado após instalação."
}

$repoRoot = Split-Path $PSScriptRoot -Parent
$globalJsonPath = Join-Path $repoRoot "global.json"

@"
{
  "sdk": {
    "version": "$DotnetVersion",
    "rollForward": "latestFeature",
    "errorMessage": "O SDK do .NET 10 não foi encontrado. Execute .\scripts\setup-dotnet.ps1"
  }
}
"@ | Set-Content -Path $globalJsonPath -Encoding UTF8

Write-Host ""
Write-Host "Ambiente configurado com sucesso."
dotnet --info
