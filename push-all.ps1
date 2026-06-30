#Requires -Version 5.1
<#
.SYNOPSIS
    Build and push the EasyPathology.External.LibDataChannel packages to BaGet.
#>
param(
    [string]$BaGetUrl = "",
    [string]$ApiKey = "",
    [switch]$SkipBuild,
    [switch]$SkipPush
)

$ErrorActionPreference = "Stop"
$repoRoot = $PSScriptRoot

if (-not $BaGetUrl) {
    $urlFile = Join-Path $repoRoot "Key/nupkg.url"
    if (-not (Test-Path $urlFile)) {
        $urlFile = Join-Path (Split-Path -Parent $repoRoot) "Key/nupkg.url"
    }
    if (-not (Test-Path $urlFile)) {
        $urlFile = Join-Path (Split-Path -Parent (Split-Path -Parent $repoRoot)) "Key/nupkg.url"
    }
    if (Test-Path $urlFile) {
        $BaGetUrl = (Get-Content $urlFile -Raw).Trim()
    } else {
        $BaGetUrl = "http://localhost:5555/v3/index.json"
    }
}

if (-not $ApiKey) {
    $keyFile = Join-Path $repoRoot "Key/nupkg.sk"
    if (-not (Test-Path $keyFile)) {
        $keyFile = Join-Path (Split-Path -Parent $repoRoot) "Key/nupkg.sk"
    }
    if (-not (Test-Path $keyFile)) {
        $keyFile = Join-Path (Split-Path -Parent (Split-Path -Parent $repoRoot)) "Key/nupkg.sk"
    }
    if (Test-Path $keyFile) {
        $ApiKey = (Get-Content $keyFile -Raw).Trim()
    } else {
        $ApiKey = "NupkgSecretKey-ChangeMe"
    }
}

$projects = @(
    "LibDataChannel.Native",
    "LibDataChannel"
)

if (-not $SkipBuild) {
    Write-Host "Building solution..." -ForegroundColor Cyan
    dotnet build (Join-Path $repoRoot "LibDataChannel.Net.slnx") -c Release
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }
}

if ($SkipPush) {
    Write-Host "[SkipPush] Push skipped." -ForegroundColor Yellow
    return
}

Write-Host "Pushing packages to $BaGetUrl ..." -ForegroundColor Cyan

# Generate a temporary NuGet.Config to allow HTTP push
$configFile = Join-Path $repoRoot "artifacts/nuget.config"
New-Item -ItemType Directory -Path (Split-Path -Parent $configFile) -Force | Out-Null
@"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="baget" value="$BaGetUrl" allowInsecureConnections="true" />
  </packageSources>
</configuration>
"@ | Out-File -FilePath $configFile -Encoding utf8

foreach ($project in $projects) {
    $pkgDir = Join-Path $repoRoot "artifacts"
    $pkg = Get-ChildItem -Path $pkgDir -Filter "EasyPathology.External.$project.*.nupkg" |
        Where-Object { $_.Name -notmatch "\.symbols\.nupkg$" } |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1

    if (-not $pkg) {
        throw "Package not found for $project in $pkgDir"
    }

    Write-Host "Pushing $($pkg.Name) ..." -ForegroundColor Yellow
    dotnet nuget push $pkg.FullName --configfile $configFile -s $BaGetUrl -k $ApiKey --skip-duplicate
    if ($LASTEXITCODE -ne 0) { throw "Push failed for $project" }
}

Write-Host "All packages pushed." -ForegroundColor Green
