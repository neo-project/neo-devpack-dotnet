param(
    [Parameter(Mandatory=$true)]
    [string]$ContractHash,
    [string]$Environment = "Development",
    [string]$WalletPath = "wallet.json",
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"

Write-Host "Neo N3 Smart Contract Update Script" -ForegroundColor Cyan
Write-Host "===================================" -ForegroundColor Cyan
Write-Host ""

# Validate contract hash format
if ($ContractHash -notmatch "^0x[a-fA-F0-9]{40}$") {
    Write-Error "Invalid contract hash format. Expected: 0x followed by 40 hex characters"
    Write-Host "Example: 0x1234567890abcdef1234567890abcdef12345678"
    exit 1
}

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
    Write-Host "Skipping build (-SkipBuild specified)" -ForegroundColor Gray
}

# Check wallet configuration
Write-Host ""
Write-Host "Checking wallet configuration..." -ForegroundColor Yellow
Write-Host "Environment: $Environment" -ForegroundColor Cyan

# Check if wallet files exist based on environment
$expectedWallet = switch ($Environment) {
    "Development" { "deploy/Deploy/wallets/development.json" }
    "TestNet" { "deploy/Deploy/wallets/testnet.json" }
    "MainNet" { "deploy/Deploy/wallets/mainnet.json" }
    default { "deploy/Deploy/wallets/development.json" }
}

if (Test-Path $expectedWallet) {
    Write-Host "Found wallet for $Environment`: $expectedWallet" -ForegroundColor Green
} else {
    Write-Warning "Expected wallet not found: $expectedWallet"
    Write-Host "Update will use wallet path from appsettings configuration" -ForegroundColor Yellow
}

# Update contract
Write-Host ""
Write-Host "Starting contract update..." -ForegroundColor Yellow
Write-Host "Contract: $ContractHash" -ForegroundColor Cyan
Write-Host "Environment: $Environment" -ForegroundColor Cyan

$originalLocation = Get-Location
try {
    Set-Location "deploy/Deploy"
    
    # Set environment variables
    $env:ENVIRONMENT = $Environment
    $env:CONTRACT_HASH = $ContractHash
    
    # Create update program
    $updateProgram = @'
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Neo.SmartContract.Deploy;

class UpdateContract
{
    static async Task Main(string[] args)
    {
        var contractHash = Environment.GetEnvironmentVariable("CONTRACT_HASH");
        if (string.IsNullOrEmpty(contractHash))
        {
            Console.WriteLine("Error: CONTRACT_HASH environment variable not set");
            Environment.Exit(1);
        }

        var toolkit = new DeploymentToolkit();
        
        try
        {
            // Find the contract source
            var contractPath = FindContractSource();
            
            Console.WriteLine($"Updating contract {contractHash}...");
            Console.WriteLine($"Source: {contractPath}");
            
            // Load wallet if needed (toolkit will use configuration)
            var result = await toolkit.UpdateAsync(contractHash, contractPath);
            
            if (result.Success)
            {
                Console.WriteLine($"✅ Contract updated successfully!");
                Console.WriteLine($"Transaction: {result.TransactionHash}");
                Console.WriteLine($"Gas consumed: {result.GasConsumed / 100_000_000m} GAS");
            }
            else
            {
                Console.WriteLine($"❌ Update failed: {result.ErrorMessage}");
                Environment.Exit(1);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
            Environment.Exit(1);
        }
    }
    
    static string FindContractSource()
    {
        // Look for contract source files
        var srcDir = Path.Combine("..", "..", "src");
        if (Directory.Exists(srcDir))
        {
            var contractFiles = Directory.GetFiles(srcDir, "*Contract.cs", SearchOption.AllDirectories)
                .Where(f => !f.Contains("/obj/") && !f.Contains("/bin/") && !f.Contains("\\obj\\") && !f.Contains("\\bin\\"))
                .ToList();
                
            if (contractFiles.Count == 1)
            {
                return contractFiles[0];
            }
            else if (contractFiles.Count > 1)
            {
                Console.WriteLine("Multiple contracts found:");
                for (int i = 0; i < contractFiles.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(contractFiles[i])}");
                }
                Console.Write($"Select contract (1-{contractFiles.Count}): ");
                if (int.TryParse(Console.ReadLine(), out int selection) && 
                    selection > 0 && selection <= contractFiles.Count)
                {
                    return contractFiles[selection - 1];
                }
                throw new Exception("Invalid selection");
            }
        }
        
        throw new Exception("No contract source file found. Please ensure your contract is in the src/ directory.");
    }
}
'@
    
    # Save and compile the update program
    $updateProgramPath = "UpdateContractTemp.cs"
    $updateProgram | Out-File -FilePath $updateProgramPath -Encoding UTF8
    
    try {
        # Run the update
        dotnet run --project Deploy.csproj -- update $ContractHash
        
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Update failed"
            exit 1
        }
    }
    finally {
        # Clean up temporary file
        if (Test-Path $updateProgramPath) {
            Remove-Item $updateProgramPath -Force
        }
    }
}
finally {
    Set-Location $originalLocation
}

Write-Host ""
Write-Host "Contract update completed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Important notes:" -ForegroundColor Cyan
Write-Host "- The contract must have an 'update' method that calls ContractManagement.Update"
Write-Host "- You must be authorized (typically the contract owner) to perform updates"
Write-Host "- The new contract code must be compatible with existing storage"
Write-Host ""
Write-Host "To verify the update, check your Neo blockchain explorer" -ForegroundColor Cyan