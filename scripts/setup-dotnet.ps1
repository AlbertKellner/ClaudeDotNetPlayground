param(
    [string]$DotnetVersion = "10.0.100",
    [string]$InstallDir = "$HOME\.dotnet"
)

$ErrorActionPreference = "Stop"

$env:DOTNET_ROOT = $InstallDir
$env:PATH = "$InstallDir;$InstallDir\tools;$env:PATH"

function Test-RequiredSdk {
    try {
        $sdks = dotnet --list-sdks 2>$null
        return ($null -ne ($sdks | Select-String '^10\.'))
    } catch {
        return $false
    }
}

if (Test-RequiredSdk) {
    Write-Host ".NET 10 já está disponível."
} else {
    Write-Host "Instalando .NET SDK $DotnetVersion em: $InstallDir"
    New-Item -ItemType Directory -Force -Path $InstallDir | Out-Null

    $scriptPath = "$env:TEMP\dotnet-install.ps1"
    Invoke-WebRequest -Uri "https://dot.net/v1/dotnet-install.ps1" -OutFile $scriptPath
    & $scriptPath -Version $DotnetVersion -InstallDir $InstallDir

    $env:DOTNET_ROOT = $InstallDir
    $env:PATH = "$InstallDir;$InstallDir\tools;$env:PATH"
}

Write-Host ""
Write-Host "SDKs instalados:"
dotnet --list-sdks

if (-not (Test-RequiredSdk)) {
    throw "Falha: .NET 10 continua indisponível após instalação."
}

$repoRoot = Split-Path $PSScriptRoot -Parent
$globalJsonPath = Join-Path $repoRoot "global.json"

Write-Host ""
Write-Host "Criando/atualizando global.json em $repoRoot"
@"
{
  "sdk": {
    "version": "$DotnetVersion",
    "rollForward": "latestFeature",
    "errorMessage": "O SDK do .NET 10 não foi encontrado. Execute .\\scripts\\setup-dotnet.ps1"
  }
}
"@ | Set-Content -Path $globalJsonPath -Encoding UTF8

Write-Host ""
Write-Host "Ambiente configurado com sucesso."
dotnet --info
