param(
    [string]$Environment = "Development",
    [string]$WalletPath = "wallet.json",
    [switch]$SkipBuild,
    [switch]$SkipTests
)

$ErrorActionPreference = "Stop"

Write-Host "Neo N3 Smart Contract Deployment Script" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan
Write-Host ""

# Check if we're in the solution root
if (-not (Test-Path "*.sln")) {
    Write-Error "No solution file found. Please run this script from the solution root directory."
    exit 1
}

# Build solution
if (-not $SkipBuild) {
    Write-Host "Building solution..." -ForegroundColor Yellow
    dotnet build --configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed"
        exit 1
    }
    Write-Host "Build completed successfully" -ForegroundColor Green
} else {
    Write-Host "Skipping build (--SkipBuild specified)" -ForegroundColor Gray
}

# Run tests
if (-not $SkipTests) {
    Write-Host ""
    Write-Host "Running tests..." -ForegroundColor Yellow
    dotnet test --configuration Release --no-build
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Tests failed"
        exit 1
    }
    Write-Host "All tests passed" -ForegroundColor Green
} else {
    Write-Host "Skipping tests (--SkipTests specified)" -ForegroundColor Gray
}

# Check wallet
Write-Host ""
Write-Host "Checking wallet..." -ForegroundColor Yellow
if (-not (Test-Path $WalletPath)) {
    $deployWalletPath = Join-Path "deploy" "Deploy" $WalletPath
    if (Test-Path $deployWalletPath) {
        $WalletPath = $deployWalletPath
        Write-Host "Found wallet at: $WalletPath" -ForegroundColor Green
    } else {
        Write-Error "Wallet file not found: $WalletPath"
        Write-Host "Please create a wallet first using neo-cli or neo-gui"
        exit 1
    }
} else {
    Write-Host "Found wallet at: $WalletPath" -ForegroundColor Green
}

# Check for wallet password
if (-not $env:WALLET_PASSWORD) {
    Write-Host ""
    Write-Host "Wallet password not found in environment variable WALLET_PASSWORD" -ForegroundColor Yellow
    $securePassword = Read-Host "Enter wallet password" -AsSecureString
    $BSTR = [System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $env:WALLET_PASSWORD = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto($BSTR)
}

# Deploy
Write-Host ""
Write-Host "Starting deployment..." -ForegroundColor Yellow
Write-Host "Environment: $Environment" -ForegroundColor Cyan

$originalLocation = Get-Location
try {
    Set-Location "deploy/Deploy"
    
    # Note: Wallet configuration is now handled via appsettings files
    # No need to copy wallet files - they're referenced directly from wallets/ directory
    
    # Run deployment
    $env:ENVIRONMENT = $Environment
    dotnet run
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Deployment failed"
        exit 1
    }
} finally {
    Set-Location $originalLocation
}

Write-Host ""
Write-Host "Deployment completed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "To view transaction details, check your Neo blockchain explorer" -ForegroundColor Cyan
Write-Host ""
Write-Host "=== Contract Update Instructions ===" -ForegroundColor Cyan
Write-Host "To update deployed contracts:"
Write-Host "1. Modify your contract source code"
Write-Host "2. Run: .\update.ps1 -ContractHash <hash> [-Environment $Environment]"
Write-Host ""
Write-Host "Or manually update using the deployment toolkit:"
Write-Host "cd deploy\Deploy; dotnet run -- update <contract-hash>"