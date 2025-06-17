@echo off
echo Neo Smart Contract Fuzzer for Neo N3
echo =============================

REM Get the script directory
SET SCRIPT_DIR=%~dp0

REM Build the project
echo Building project...
dotnet build "%SCRIPT_DIR%Neo.SmartContract.Fuzzer.csproj"

REM Run the fuzzer
echo Running fuzzer...
dotnet run --project "%SCRIPT_DIR%Neo.SmartContract.Fuzzer.csproj" -- %*

echo Fuzzing completed.
pause