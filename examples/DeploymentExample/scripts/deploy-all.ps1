# Deploy all contracts using the deployment toolkit
# Usage: .\deploy-all.ps1 [-Network testnet] [-WifKey YOUR_KEY]

param(
    [string]$Network = "testnet",
    [string]$WifKey = $env:NEO_WIF_KEY
)

# Check if WIF key is provided
if ([string]::IsNullOrEmpty($WifKey)) {
    Write-Host "Error: WIF key not provided. Set NEO_WIF_KEY environment variable or pass -WifKey parameter." -ForegroundColor Red
    exit 1
}

Write-Host "Neo Smart Contract Deployment Example" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host "Network: $Network" -ForegroundColor Yellow
Write-Host ""

# Change to project root
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location "$scriptPath\.."

# Build all contracts
Write-Host "Building all contracts..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Run tests
Write-Host "Running tests..." -ForegroundColor Yellow
dotnet test --no-build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Tests failed!" -ForegroundColor Red
    exit 1
}

# Deploy using manifest
Write-Host "Deploying contracts to $Network..." -ForegroundColor Yellow
Set-Location "deploy\DeploymentExample.Deploy"

dotnet run -- deploy-manifest `
    -n $Network `
    -w $WifKey `
    -m ..\..\deployment-manifest.json `
    -o deployment-results.json `
    -v

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Deployment completed successfully!" -ForegroundColor Green
    
    # Display results
    if (Test-Path "deployment-results.json") {
        Write-Host "Deployment Results:" -ForegroundColor Yellow
        Get-Content "deployment-results.json" | ConvertFrom-Json | ConvertTo-Json -Depth 10
    }
} else {
    Write-Host "❌ Deployment failed!" -ForegroundColor Red
    exit 1
}