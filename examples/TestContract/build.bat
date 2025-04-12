@echo off
setlocal enabledelayedexpansion

echo Building TestContract...

:: Get the root directory of the neo-devpack-dotnet project
set "ROOT_DIR=%~dp0..\..\"
cd "%ROOT_DIR%"

:: Build the contract and generate the interface
dotnet run --project src\Neo.Compiler.CSharp\Neo.Compiler.CSharp.csproj -- ^
    examples\TestContract\TestContract.csproj ^
    --generate-interface

:: Check if the build was successful
if %ERRORLEVEL% NEQ 0 (
    echo Build failed.
    exit /b 1
)

echo.
echo Build successful!
echo Contract files:
dir /b examples\TestContract\bin\sc\

:: Check if the interface file was generated
if exist "examples\TestContract\bin\sc\IHelloWorldContract.cs" (
    echo.
    echo Interface file generated successfully.
    echo Interface file: examples\TestContract\bin\sc\IHelloWorldContract.cs
) else (
    echo.
    echo Interface file was not generated.
    exit /b 1
)

echo.
echo Done!
