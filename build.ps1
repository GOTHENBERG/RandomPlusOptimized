param(
    [string]$Configuration = "Release",
    [switch]$DeployToRimWorld
)

$ErrorActionPreference = "Stop"
Write-Host "=== Building RandomPlus ($Configuration) ===" -ForegroundColor Cyan

$msbuildCandidates = @(
    "C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe",
    "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
)

$msbuild = $msbuildCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $msbuild) {
    $where = (Get-Command msbuild -ErrorAction SilentlyContinue)
    if ($where) { $msbuild = $where.Source }
}

if (-not $msbuild) {
    Write-Error "MSBuild.exe was not found!"
    exit 1
}

Write-Host "[INFO] Using MSBuild: $msbuild" -ForegroundColor Gray
& $msbuild RandomPlus.sln /p:Configuration=$Configuration /v:minimal /nologo

if ($LASTEXITCODE -eq 0) {
    $dllPath = "RandomPlus\bin\$Configuration\RandomPlus.dll"
    if (Test-Path $dllPath) {
        $dllInfo = Get-Item $dllPath
        Write-Host "`n[SUCCESS] Build completed successfully!" -ForegroundColor Green
        Write-Host "Output binary: $($dllInfo.FullName) ($($dllInfo.Length) bytes)" -ForegroundColor Green

        if ($DeployToRimWorld) {
            $modsDir = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RandomPlus\Assemblies"
            New-Item -ItemType Directory -Force -Path $modsDir | Out-Null
            Copy-Item -Path $dllPath -Destination $modsDir -Force
            Write-Host "[DEPLOY] Deployed to RimWorld Mods: $modsDir" -ForegroundColor Yellow
        }
    }
} else {
    Write-Error "Build failed with exit code $LASTEXITCODE"
}
