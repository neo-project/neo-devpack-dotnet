#!/bin/bash

# Default values
ENVIRONMENT="Development"
WALLET_PATH="wallet.json"
CONTRACT_HASH=""
SKIP_BUILD=false

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -e|--environment)
            ENVIRONMENT="$2"
            shift 2
            ;;
        -w|--wallet)
            WALLET_PATH="$2"
            shift 2
            ;;
        -c|--contract)
            CONTRACT_HASH="$2"
            shift 2
            ;;
        --skip-build)
            SKIP_BUILD=true
            shift
            ;;
        -h|--help)
            echo "Usage: $0 [-e ENVIRONMENT] [-w WALLET_PATH] [-c CONTRACT_HASH] [--skip-build]"
            echo ""
            echo "Options:"
            echo "  -e, --environment  Deployment environment (default: Development)"
            echo "  -w, --wallet       Path to wallet file (default: wallet.json)"
            echo "  -c, --contract     Contract hash to update (required)"
            echo "  --skip-build       Skip building the solution"
            echo "  -h, --help         Show this help message"
            echo ""
            echo "Example:"
            echo "  $0 -c 0x1234567890abcdef1234567890abcdef12345678 -e TestNet"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

echo -e "\033[36mNeo N3 Smart Contract Update Script\033[0m"
echo -e "\033[36m===================================\033[0m"
echo ""

# Check if contract hash is provided
if [ -z "$CONTRACT_HASH" ]; then
    echo -e "\033[31mError: Contract hash is required. Use -c or --contract to specify.\033[0m"
    echo "Run '$0 --help' for usage information."
    exit 1
fi

# Check if we're in the solution root
if ! ls *.sln >/dev/null 2>&1; then
    echo -e "\033[31mError: No solution file found. Please run this script from the solution root directory.\033[0m"
    exit 1
fi

# Build solution
if [ "$SKIP_BUILD" = false ]; then
    echo -e "\033[33mBuilding solution...\033[0m"
    if ! dotnet build --configuration Release; then
        echo -e "\033[31mBuild failed\033[0m"
        exit 1
    fi
    echo -e "\033[32mBuild completed successfully\033[0m"
else
    echo -e "\033[90mSkipping build (--skip-build specified)\033[0m"
fi

# Check wallet configuration
echo ""
echo -e "\033[33mChecking wallet configuration...\033[0m"
echo -e "\033[36mEnvironment: $ENVIRONMENT\033[0m"

# Check if wallet files exist based on environment
case $ENVIRONMENT in
    "Development")
        EXPECTED_WALLET="deploy/Deploy/wallets/development.json"
        ;;
    "TestNet")
        EXPECTED_WALLET="deploy/Deploy/wallets/testnet.json"
        ;;
    "MainNet")
        EXPECTED_WALLET="deploy/Deploy/wallets/mainnet.json"
        ;;
    *)
        EXPECTED_WALLET="deploy/Deploy/wallets/development.json"
        ;;
esac

if [ -f "$EXPECTED_WALLET" ]; then
    echo -e "\033[32mFound wallet for $ENVIRONMENT: $EXPECTED_WALLET\033[0m"
else
    echo -e "\033[31mWarning: Expected wallet not found: $EXPECTED_WALLET\033[0m"
    echo -e "\033[33mUpdate will use wallet path from appsettings configuration\033[0m"
fi

# Update contract
echo ""
echo -e "\033[33mStarting contract update...\033[0m"
echo -e "\033[36mContract: $CONTRACT_HASH\033[0m"
echo -e "\033[36mEnvironment: $ENVIRONMENT\033[0m"

ORIGINAL_DIR=$(pwd)
cd deploy/Deploy || exit 1

# Run update using deployment toolkit
export ENVIRONMENT
export CONTRACT_HASH

# Create a temporary update script
cat > update_contract.cs << 'EOF'
using System;
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
        
        // Load configuration from appsettings
        // The toolkit will automatically load the correct environment configuration
        
        try
        {
            // Find the contract source based on the solution structure
            // This assumes a standard structure where contracts are in src/ directory
            var contractPath = FindContractSource();
            
            Console.WriteLine($"Updating contract {contractHash}...");
            Console.WriteLine($"Source: {contractPath}");
            
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
                .Where(f => !f.Contains("/obj/") && !f.Contains("/bin/"))
                .ToList();
                
            if (contractFiles.Count == 1)
            {
                return contractFiles[0];
            }
            else if (contractFiles.Count > 1)
            {
                Console.WriteLine("Multiple contracts found. Please specify which one to update:");
                for (int i = 0; i < contractFiles.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(contractFiles[i])}");
                }
                Console.Write("Select contract (1-{0}): ", contractFiles.Count);
                if (int.TryParse(Console.ReadLine(), out int selection) && 
                    selection > 0 && selection <= contractFiles.Count)
                {
                    return contractFiles[selection - 1];
                }
            }
        }
        
        throw new Exception("No contract source file found. Please ensure your contract is in the src/ directory.");
    }
}
EOF

# Run the update
if ! dotnet run --project Deploy.csproj -- update "$CONTRACT_HASH"; then
    rm -f update_contract.cs
    cd "$ORIGINAL_DIR"
    echo -e "\033[31mUpdate failed\033[0m"
    exit 1
fi

rm -f update_contract.cs
cd "$ORIGINAL_DIR"

echo ""
echo -e "\033[32mContract update completed successfully!\033[0m"
echo ""
echo -e "\033[36mImportant notes:\033[0m"
echo "- The contract must have an 'update' method that calls ContractManagement.Update"
echo "- You must be authorized (typically the contract owner) to perform updates"
echo "- The new contract code must be compatible with existing storage"
echo ""
echo -e "\033[36mTo verify the update, check your Neo blockchain explorer\033[0m"