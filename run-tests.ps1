# Run all tests for the Neo Smart Contract Fuzzer

Write-Host "Running unit tests..." -ForegroundColor Green
dotnet test tests/Neo.SmartContract.Fuzzer.Tests

Write-Host "Running integration tests..." -ForegroundColor Green
dotnet test tests/Neo.SmartContract.Fuzzer.IntegrationTests

Write-Host "All tests completed." -ForegroundColor Green
