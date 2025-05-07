# Optimize performance for the Neo Smart Contract Fuzzer

Write-Host "Optimizing performance..." -ForegroundColor Green

# Build in Release mode with optimizations
Write-Host "Building with optimizations..." -ForegroundColor Yellow
dotnet build src/Neo.SmartContract.Fuzzer -c Release /p:Optimize=true

# Run performance benchmarks
Write-Host "Running performance benchmarks..." -ForegroundColor Yellow
dotnet test tests/Neo.SmartContract.Fuzzer.IntegrationTests --filter "FullyQualifiedName~PerformanceBenchmarkTests"

Write-Host "Performance optimization completed." -ForegroundColor Green
