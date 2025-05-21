@echo off
echo Building solution...
dotnet build

echo Running all tests...
dotnet test --logger "console;verbosity=detailed"

if exist "tests\Neo.SmartContract.Fuzzer.Tests\bin\Debug\net9.0\coverage.html" (
    echo Coverage report generated: tests\Neo.SmartContract.Fuzzer.Tests\bin\Debug\net9.0\coverage.html
    start "" "tests\Neo.SmartContract.Fuzzer.Tests\bin\Debug\net9.0\coverage.html"
) else (
    echo Coverage report not found.
)

echo Tests completed.
pause