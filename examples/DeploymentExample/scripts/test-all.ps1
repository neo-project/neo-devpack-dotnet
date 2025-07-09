# Run all tests for the deployment example
# Usage: .\test-all.ps1 [-Category All|Integration|Security]

param(
    [string]$Category = "All"
)

Write-Host "Neo Smart Contract Tests" -ForegroundColor Green
Write-Host "========================" -ForegroundColor Green
Write-Host ""

# Change to project root
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location "$scriptPath\.."

# Build contracts
Write-Host "Building contracts..." -ForegroundColor Yellow
dotnet build --configuration Debug
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Run tests based on category
switch ($Category) {
    "Integration" {
        Write-Host "Running integration tests..." -ForegroundColor Yellow
        dotnet test --no-build --filter "Category=Integration"
    }
    "Security" {
        Write-Host "Running security tests..." -ForegroundColor Yellow
        dotnet test --no-build --filter "Category=Security"
    }
    default {
        Write-Host "Running all tests with coverage..." -ForegroundColor Yellow
        dotnet test `
            --no-build `
            --configuration Debug `
            --logger "console;verbosity=detailed" `
            --collect:"XPlat Code Coverage" `
            --results-directory ./test-results `
            -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
    }
}

# Check if tests passed
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ All tests passed!" -ForegroundColor Green
    
    # Generate coverage report if ReportGenerator is available
    $reportGeneratorPath = Get-Command reportgenerator -ErrorAction SilentlyContinue
    if ($reportGeneratorPath -and $Category -eq "All") {
        Write-Host "Generating coverage report..." -ForegroundColor Yellow
        reportgenerator `
            -reports:"./test-results/*/coverage.opencover.xml" `
            -targetdir:"./test-results/coverage-report" `
            -reporttypes:"Html;Badges"
        Write-Host "Coverage report generated at: ./test-results/coverage-report/index.html" -ForegroundColor Green
    }
} else {
    Write-Host "❌ Some tests failed!" -ForegroundColor Red
    exit 1
}