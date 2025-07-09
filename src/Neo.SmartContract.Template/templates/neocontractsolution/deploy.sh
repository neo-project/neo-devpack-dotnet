#!/bin/bash

# Default values
ENVIRONMENT="Development"
WALLET_PATH="wallet.json"
SKIP_BUILD=false
SKIP_TESTS=false

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
        --skip-build)
            SKIP_BUILD=true
            shift
            ;;
        --skip-tests)
            SKIP_TESTS=true
            shift
            ;;
        -h|--help)
            echo "Usage: $0 [-e ENVIRONMENT] [-w WALLET_PATH] [--skip-build] [--skip-tests]"
            echo ""
            echo "Options:"
            echo "  -e, --environment  Deployment environment (default: Development)"
            echo "  -w, --wallet       Path to wallet file (default: wallet.json)"
            echo "  --skip-build       Skip building the solution"
            echo "  --skip-tests       Skip running tests"
            echo "  -h, --help         Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

echo -e "\033[36mNeo N3 Smart Contract Deployment Script\033[0m"
echo -e "\033[36m=======================================\033[0m"
echo ""

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

# Run tests
if [ "$SKIP_TESTS" = false ]; then
    echo ""
    echo -e "\033[33mRunning tests...\033[0m"
    if ! dotnet test --configuration Release --no-build; then
        echo -e "\033[31mTests failed\033[0m"
        exit 1
    fi
    echo -e "\033[32mAll tests passed\033[0m"
else
    echo -e "\033[90mSkipping tests (--skip-tests specified)\033[0m"
fi

# Check wallet configuration
echo ""
echo -e "\033[33mChecking wallet configuration...\033[0m"
echo -e "\033[36mWallet configuration is now per-network in appsettings files\033[0m"
echo -e "\033[36mEnvironment: $ENVIRONMENT will use the corresponding appsettings file\033[0m"

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
    echo -e "\033[33mDeployment will use wallet path from appsettings configuration\033[0m"
fi

# Deploy
echo ""
echo -e "\033[33mStarting deployment...\033[0m"
echo -e "\033[36mEnvironment: $ENVIRONMENT\033[0m"

ORIGINAL_DIR=$(pwd)
cd deploy/Deploy || exit 1

# Note: Wallet configuration is now handled via appsettings files
# No need to copy wallet files - they're referenced directly from wallets/ directory

# Run deployment
export ENVIRONMENT
if ! dotnet run; then
    cd "$ORIGINAL_DIR"
    echo -e "\033[31mDeployment failed\033[0m"
    exit 1
fi

cd "$ORIGINAL_DIR"

echo ""
echo -e "\033[32mDeployment completed successfully!\033[0m"
echo ""
echo -e "\033[36mTo view transaction details, check your Neo blockchain explorer\033[0m"
echo ""
echo -e "\033[36m=== Contract Update Instructions ===\033[0m"
echo "To update deployed contracts:"
echo "1. Modify your contract source code"
echo "2. Run: ./update.sh [-e ENVIRONMENT] [-w WALLET_PATH]"
echo ""
echo "Or manually update using the deployment toolkit:"
echo "cd deploy/Deploy && dotnet run -- update <contract-hash>"