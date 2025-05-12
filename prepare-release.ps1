# Prepare release for the Neo Smart Contract Fuzzer

Write-Host "Preparing release for the Neo Smart Contract Fuzzer..." -ForegroundColor Green

# Check code quality
Write-Host "Checking code quality..." -ForegroundColor Yellow
./check-code-quality.ps1

# Run all tests
Write-Host "Running all tests..." -ForegroundColor Yellow
./run-tests.ps1

# Optimize performance
Write-Host "Optimizing performance..." -ForegroundColor Yellow
./optimize-performance.ps1

# Build release package
Write-Host "Building release package..." -ForegroundColor Yellow
./build-release.ps1

Write-Host "Release preparation completed." -ForegroundColor Green
Write-Host "Please review the release package in the 'release' directory." -ForegroundColor Green
