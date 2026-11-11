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
    $dllPath16 = "Resources\1.6\Assemblies\RandomPlus.dll"
    $dllPath17 = "Resources\1.7\Assemblies\RandomPlus.dll"
    if (Test-Path $dllPath16) {
        $dllInfo16 = Get-Item $dllPath16
        Write-Host "`n[SUCCESS] Build completed successfully!" -ForegroundColor Green
        Write-Host "Output 1.6 binary: $($dllInfo16.FullName) ($($dllInfo16.Length) bytes)" -ForegroundColor Green
        if (Test-Path $dllPath17) {
            $dllInfo17 = Get-Item $dllPath17
            Write-Host "Output 1.7 binary: $($dllInfo17.FullName) ($($dllInfo17.Length) bytes)" -ForegroundColor Green
        }

        if ($DeployToRimWorld) {
            $destModDir = "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RandomPlusOptimized"
            New-Item -ItemType Directory -Force -Path $destModDir | Out-Null
            Copy-Item -Path "Resources\*" -Destination $destModDir -Recurse -Force
            Write-Host "[DEPLOY] Deployed complete mod to RimWorld: $destModDir" -ForegroundColor Yellow
        }
    }
} else {
    Write-Error "Build failed with exit code $LASTEXITCODE"
}
