@echo off
setlocal enabledelayedexpansion

echo ========================================================
echo           Building RandomPlus for RimWorld
echo ========================================================

set "MSBUILD="
if exist "C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
) else if exist "C:\Program Files\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe" (
    set "MSBUILD=C:\Program Files\Microsoft Visual Studio\18\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
) else (
    for /f "usebackq tokens=*" %%i in (`where msbuild 2^>nul`) do (
        set "MSBUILD=%%i"
    )
)

if "%MSBUILD%"=="" (
    echo [ERROR] MSBuild was not found on your system!
    exit /b 1
)

echo [INFO] Using MSBuild: %MSBUILD%
echo [INFO] Compiling RandomPlus (Release)...

"%MSBUILD%" RandomPlus.sln /p:Configuration=Release /v:minimal /nologo

if %ERRORLEVEL% equ 0 (
    echo.
    echo ========================================================
    echo  [SUCCESS] RandomPlus built successfully!
    echo  Output: Resources\1.6\Assemblies\RandomPlus.dll
    echo ========================================================
) else (
    echo.
    echo [ERROR] Build failed with error code %ERRORLEVEL%!
    exit /b %ERRORLEVEL%
)

endlocal
