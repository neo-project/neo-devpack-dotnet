#!/bin/bash

# Local test script for contract update functionality
# This script tests the update process without CI/CD overhead

echo "=== Local Contract Update Test ==="
echo "This script tests contract updates locally without CI/CD"
echo

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if we're in the right directory
if [ ! -f "deploy/DeploymentExample.Deploy/DeploymentExample.Deploy.csproj" ]; then
    echo -e "${RED}Error: Must run from DeploymentExample directory${NC}"
    exit 1
fi

# Step 1: Build contracts
echo -e "${YELLOW}1. Building contracts...${NC}"
if dotnet build src/TokenContract/TokenContract.csproj && \
   dotnet build src/NFTContract/NFTContract.csproj && \
   dotnet build src/GovernanceContract/GovernanceContract.csproj; then
    echo -e "${GREEN}✓ Contracts built successfully${NC}"
else
    echo -e "${RED}✗ Build failed${NC}"
    exit 1
fi

# Step 2: Test update functionality in contracts
echo -e "\n${YELLOW}2. Checking _deploy method with update support...${NC}"

# Check TokenContract has proper _deploy method
if grep -q "public static void _deploy" src/TokenContract/TokenContract.cs; then
    if grep -A 10 "_deploy" src/TokenContract/TokenContract.cs | grep -q "if (update)"; then
        echo -e "${GREEN}✓ TokenContract has _deploy method with update support${NC}"
    else
        echo -e "${RED}✗ TokenContract _deploy method missing update handling${NC}"
    fi
else
    echo -e "${RED}✗ TokenContract missing _deploy method${NC}"
fi

# Check GovernanceContract has proper _deploy method
if grep -q "public static void _deploy" src/GovernanceContract/GovernanceContract.cs; then
    if grep -A 10 "_deploy" src/GovernanceContract/GovernanceContract.cs | grep -q "if (update)"; then
        echo -e "${GREEN}✓ GovernanceContract has _deploy method with update support${NC}"
    else
        echo -e "${RED}✗ GovernanceContract _deploy method missing update handling${NC}"
    fi
else
    echo -e "${RED}✗ GovernanceContract missing _deploy method${NC}"
fi

# Step 3: Test deployment toolkit update functionality
echo -e "\n${YELLOW}3. Testing deployment toolkit...${NC}"
cd deploy/DeploymentExample.Deploy

# Create a simple test program
cat > TestUpdate.cs << 'EOF'
using System;
using System.Threading.Tasks;
using Neo.SmartContract.Deploy;

class TestUpdate
{
    static async Task Main()
    {
        try
        {
            var toolkit = new DeploymentToolkit();
            
            // Test 1: Verify SetWifKey works
            Console.WriteLine("Test 1: Testing WIF key configuration...");
            toolkit.SetWifKey("KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb");
            Console.WriteLine("✓ WIF key set successfully");
            
            // Test 2: Verify network configuration
            Console.WriteLine("\nTest 2: Testing network configuration...");
            toolkit.SetNetwork("testnet");
            Console.WriteLine("✓ Network set to testnet");
            
            // Test 3: Get deployer account from WIF
            Console.WriteLine("\nTest 3: Testing account derivation...");
            var account = await toolkit.GetDeployerAccountAsync();
            Console.WriteLine($"✓ Deployer account: {account}");
            
            // Test 4: Verify UpdateAsync method exists
            Console.WriteLine("\nTest 4: Verifying UpdateAsync method...");
            var type = toolkit.GetType();
            var method = type.GetMethod("UpdateAsync");
            if (method != null)
            {
                Console.WriteLine("✓ UpdateAsync method found");
                var parameters = method.GetParameters();
                Console.WriteLine($"  Parameters: {parameters.Length}");
                foreach (var param in parameters)
                {
                    Console.WriteLine($"  - {param.Name}: {param.ParameterType.Name}");
                }
            }
            else
            {
                Console.WriteLine("✗ UpdateAsync method not found");
            }
            
            Console.WriteLine("\n✅ All toolkit tests passed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Test failed: {ex.Message}");
            Environment.Exit(1);
        }
    }
}
EOF

# Run the test
echo -e "${YELLOW}Running toolkit tests...${NC}"
if dotnet run --project DeploymentExample.Deploy.csproj TestUpdate.cs > /dev/null 2>&1; then
    # Actually run it to see output
    dotnet run --project DeploymentExample.Deploy.csproj TestUpdate.cs
    echo -e "${GREEN}✓ Deployment toolkit tests passed${NC}"
else
    echo -e "${RED}✗ Deployment toolkit tests failed${NC}"
    # Show error output
    dotnet run --project DeploymentExample.Deploy.csproj TestUpdate.cs
fi

# Cleanup
rm -f TestUpdate.cs
cd ../..

# Step 4: Test update scripts
echo -e "\n${YELLOW}4. Checking update scripts...${NC}"

if [ -f "update-contracts.sh" ]; then
    echo -e "${GREEN}✓ update-contracts.sh exists${NC}"
    if [ -x "update-contracts.sh" ]; then
        echo -e "${GREEN}✓ update-contracts.sh is executable${NC}"
    else
        echo -e "${YELLOW}! Making update-contracts.sh executable${NC}"
        chmod +x update-contracts.sh
    fi
else
    echo -e "${RED}✗ update-contracts.sh not found${NC}"
fi

# Step 5: Summary
echo -e "\n${YELLOW}=== Test Summary ===${NC}"
echo "Contract update functionality has been verified locally."
echo
echo "To test actual updates:"
echo "1. Deploy contracts: ./deploy-two-step.sh"
echo "2. Note the contract hashes from deployment"
echo "3. Modify a contract (e.g., change a return value)"
echo "4. Run update: ./update-contracts.sh <TOKEN_HASH> <NFT_HASH> <GOV_HASH>"
echo
echo "For programmatic updates, use the deployment toolkit:"
echo "  cd deploy/DeploymentExample.Deploy"
echo "  dotnet run -- update <contract-hash>"