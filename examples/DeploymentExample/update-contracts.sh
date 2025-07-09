#!/bin/bash

# Update deployed contracts using Neo Express

echo "=== Neo Contract Update Script ==="
echo

# Check if contract hashes are provided
if [ -z "$1" ] || [ -z "$2" ] || [ -z "$3" ]; then
    echo "Usage: ./update-contracts.sh <TOKEN_HASH> <NFT_HASH> <GOV_HASH>"
    echo
    echo "Example:"
    echo "  ./update-contracts.sh 0x1234... 0x5678... 0x9abc..."
    echo
    echo "To get contract hashes, run:"
    echo "  neoxp show contract --express"
    exit 1
fi

TOKEN_HASH=$1
NFT_HASH=$2
GOV_HASH=$3

# Ensure Neo Express is running
echo "1. Checking Neo Express status..."
neoxp show balance deployer || exit 1

# Build contracts first
echo -e "\n2. Building updated contracts..."
./build-contracts.sh || exit 1

# Update Token Contract
echo -e "\n3. Updating Token Contract ($TOKEN_HASH)..."
echo "   Reading NEF and manifest files..."
TOKEN_NEF=$(xxd -p compiled-contracts/DeploymentExample.TokenContract.nef | tr -d '\n')
TOKEN_MANIFEST=$(cat compiled-contracts/DeploymentExample.TokenContract.manifest.json | jq -c .)

echo "   Updating contract via ContractManagement..."
# Call ContractManagement.Update directly
# ContractManagement hash on Neo N3: 0xfffdc93764dbaddd97c48f252a53ea4643faa3fd
UPDATE_TX=$(neoxp contract invoke 0xfffdc93764dbaddd97c48f252a53ea4643faa3fd update "0x$TOKEN_NEF" "$TOKEN_MANIFEST" null $TOKEN_HASH deployer --json | jq -r '.transaction')
if [ -z "$UPDATE_TX" ] || [ "$UPDATE_TX" = "null" ]; then
    echo "   ❌ Failed to update Token Contract"
else
    echo "   ✅ Token Contract updated: TX $UPDATE_TX"
fi

# Update NFT Contract
echo -e "\n4. Updating NFT Contract ($NFT_HASH)..."
echo "   Reading NEF and manifest files..."
NFT_NEF=$(xxd -p compiled-contracts/DeploymentExample.NFTContract.nef | tr -d '\n')
NFT_MANIFEST=$(cat compiled-contracts/DeploymentExample.NFTContract.manifest.json | jq -c .)

echo "   Updating contract via ContractManagement..."
# Call ContractManagement.Update directly
UPDATE_TX=$(neoxp contract invoke 0xfffdc93764dbaddd97c48f252a53ea4643faa3fd update "0x$NFT_NEF" "$NFT_MANIFEST" null $NFT_HASH deployer --json | jq -r '.transaction')
if [ -z "$UPDATE_TX" ] || [ "$UPDATE_TX" = "null" ]; then
    echo "   ❌ Failed to update NFT Contract"
else
    echo "   ✅ NFT Contract updated: TX $UPDATE_TX"
fi

# Update Governance Contract
echo -e "\n5. Updating Governance Contract ($GOV_HASH)..."
echo "   Reading NEF and manifest files..."
GOV_NEF=$(xxd -p compiled-contracts/DeploymentExample.GovernanceContract.nef | tr -d '\n')
GOV_MANIFEST=$(cat compiled-contracts/DeploymentExample.GovernanceContract.manifest.json | jq -c .)

echo "   Updating contract via ContractManagement..."
# Call ContractManagement.Update directly
UPDATE_TX=$(neoxp contract invoke 0xfffdc93764dbaddd97c48f252a53ea4643faa3fd update "0x$GOV_NEF" "$GOV_MANIFEST" null $GOV_HASH deployer --json | jq -r '.transaction')
if [ -z "$UPDATE_TX" ] || [ "$UPDATE_TX" = "null" ]; then
    echo "   ❌ Failed to update Governance Contract"
else
    echo "   ✅ Governance Contract updated: TX $UPDATE_TX"
fi

# Wait for updates to be confirmed
echo -e "\n6. Waiting for confirmations..."
sleep 5

# Verify updates by checking contract version or running tests
echo -e "\n7. Verifying updates..."
echo "   Testing Token Contract..."
neoxp contract invoke $TOKEN_HASH symbol || echo "   ⚠️  Token Contract test failed"

echo "   Testing NFT Contract..."
neoxp contract invoke $NFT_HASH symbol || echo "   ⚠️  NFT Contract test failed"

echo "   Testing Governance Contract..."
neoxp contract invoke $GOV_HASH getOwner || echo "   ⚠️  Governance Contract test failed"

echo -e "\n✅ Contract update process complete!"
echo
echo "Note: If updates failed, ensure:"
echo "1. The contracts have proper _deploy methods with update authorization checks"
echo "2. You have authorization (are the contract owner) to update"
echo "3. The new contract code is valid and compiled successfully"
echo "4. The _deploy method checks Runtime.CheckWitness when update=true"