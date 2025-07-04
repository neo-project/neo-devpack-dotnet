#!/bin/bash

# Deploy and test contracts using Neo Express

echo "=== Neo Contract Deployment Example ==="
echo

# Ensure Neo Express is running
echo "1. Checking Neo Express status..."
neoxp show balance deployer || exit 1

# Deploy Token Contract
echo -e "\n2. Deploying Token Contract..."
TOKEN_TX=$(neoxp contract deploy compiled-contracts/DeploymentExample.TokenContract.nef deployer --json | jq -r '.transaction')
echo "   Transaction: $TOKEN_TX"

# Get deployed contract hash
sleep 3
TOKEN_HASH=$(neoxp show contract DeploymentExample.TokenContract --json | jq -r '.hash')
echo "   Token Contract: $TOKEN_HASH"

# Initialize Token Contract
echo -e "\n3. Initializing Token Contract..."
DEPLOYER_HASH=$(neoxp wallet show deployer --json | jq -r '.accounts[0].scriptHash')
neoxp contract invoke $TOKEN_HASH initialize $DEPLOYER_HASH deployer

# Deploy NFT Contract
echo -e "\n4. Deploying NFT Contract..."
NFT_TX=$(neoxp contract deploy compiled-contracts/DeploymentExample.NFTContract.nef deployer --json | jq -r '.transaction')
echo "   Transaction: $NFT_TX"

sleep 3
NFT_HASH=$(neoxp show contract DeploymentExample.NFTContract --json | jq -r '.hash')
echo "   NFT Contract: $NFT_HASH"

# Initialize NFT Contract
echo -e "\n5. Initializing NFT Contract..."
neoxp contract invoke $NFT_HASH initialize $DEPLOYER_HASH $TOKEN_HASH 1000000000 deployer

# Deploy Governance Contract
echo -e "\n6. Deploying Governance Contract..."
GOV_TX=$(neoxp contract deploy compiled-contracts/DeploymentExample.GovernanceContract.nef deployer --json | jq -r '.transaction')
echo "   Transaction: $GOV_TX"

sleep 3
GOV_HASH=$(neoxp show contract DeploymentExample.GovernanceContract --json | jq -r '.hash')
echo "   Governance Contract: $GOV_HASH"

# Initialize Governance Contract
echo -e "\n7. Initializing Governance Contract..."
neoxp contract invoke $GOV_HASH initialize $DEPLOYER_HASH $TOKEN_HASH deployer

# Test Token Contract
echo -e "\n8. Testing Token Contract..."
echo "   Getting symbol..."
neoxp contract invoke $TOKEN_HASH symbol
echo "   Getting total supply..."
neoxp contract invoke $TOKEN_HASH totalSupply
echo "   Getting deployer balance..."
neoxp contract invoke $TOKEN_HASH balanceOf $DEPLOYER_HASH

# Test NFT Contract
echo -e "\n9. Testing NFT Contract..."
echo "   Getting NFT symbol..."
neoxp contract invoke $NFT_HASH symbol
echo "   Getting NFT total supply..."
neoxp contract invoke $NFT_HASH totalSupply

# Test Governance Contract
echo -e "\n10. Testing Governance Contract..."
echo "   Checking if deployer is council member..."
neoxp contract invoke $GOV_HASH isCouncilMember $DEPLOYER_HASH

echo -e "\n✅ Deployment and testing complete!"
echo
echo "Contract Addresses:"
echo "  Token:      $TOKEN_HASH"
echo "  NFT:        $NFT_HASH"
echo "  Governance: $GOV_HASH"