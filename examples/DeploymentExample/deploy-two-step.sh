#!/bin/bash

# Deploy and test contracts using Neo Express with two-step deployment

echo "=== Neo Contract Deployment Example (Two-Step) ==="
echo
echo "This script uses a two-step deployment pattern to avoid Neo Express type issues"
echo

# Ensure Neo Express is running
echo "1. Checking Neo Express status..."
neoxp show balance deployer || exit 1

# Get deployer script hash
DEPLOYER_HASH=$(neoxp wallet show deployer --json | jq -r '.accounts[0].scriptHash')
echo "   Deployer: $DEPLOYER_HASH"

# Deploy Token Contract (without parameters)
echo -e "\n2. Deploying Token Contract..."
TOKEN_TX=$(neoxp contract deploy compiled-contracts/DeploymentExample.TokenContract.nef deployer)
echo "   Transaction: $TOKEN_TX"

# Wait for deployment
sleep 3

# Get deployed contract hash
TOKEN_HASH=$(neoxp show contract --express | grep TokenContract | head -1 | awk '{print $1}')
echo "   Token Contract: $TOKEN_HASH"

# Initialize Token Contract
echo -e "\n3. Initializing Token Contract..."
neoxp contract invoke $TOKEN_HASH initialize $DEPLOYER_HASH deployer

# Deploy NFT Contract (without parameters)
echo -e "\n4. Deploying NFT Contract..."
NFT_TX=$(neoxp contract deploy compiled-contracts/DeploymentExample.NFTContract.nef deployer)
echo "   Transaction: $NFT_TX"

sleep 3
NFT_HASH=$(neoxp show contract --express | grep NFTContract | head -1 | awk '{print $1}')
echo "   NFT Contract: $NFT_HASH"

# Initialize NFT Contract
echo -e "\n5. Initializing NFT Contract..."
neoxp contract invoke $NFT_HASH initialize $DEPLOYER_HASH $TOKEN_HASH 1000000000 deployer

# Deploy Governance Contract (without parameters)
echo -e "\n6. Deploying Governance Contract..."
GOV_TX=$(neoxp contract deploy compiled-contracts/DeploymentExample.GovernanceContract.nef deployer)
echo "   Transaction: $GOV_TX"

sleep 3
GOV_HASH=$(neoxp show contract --express | grep GovernanceContract | head -1 | awk '{print $1}')
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
echo "   Checking if deployer is owner..."
neoxp contract invoke $GOV_HASH getOwner

# Additional tests
echo -e "\n11. Testing contract interactions..."
echo "   Approving tokens for NFT minting..."
neoxp contract invoke $TOKEN_HASH transfer $DEPLOYER_HASH $NFT_HASH 10000000000 "" deployer

echo "   Setting governance on Token contract..."
neoxp contract invoke $TOKEN_HASH setGovernance $GOV_HASH deployer

echo -e "\n✅ Deployment and testing complete!"
echo
echo "Contract Addresses:"
echo "  Token:      $TOKEN_HASH"
echo "  NFT:        $NFT_HASH"
echo "  Governance: $GOV_HASH"
echo
echo "Next steps:"
echo "1. Mint NFTs: neoxp contract invoke $NFT_HASH mint <tokenURI> <properties> deployer"
echo "2. Create proposal: neoxp contract invoke $GOV_HASH createProposal <type> <data> <description> deployer"
echo
echo "=== Contract Update Instructions ==="
echo "To update any of the deployed contracts:"
echo "1. Modify the contract source code"
echo "2. Rebuild: ./build-contracts.sh"
echo "3. Update the contract:"
echo "   - Token: neoxp contract invoke $TOKEN_HASH update \\"
echo "       compiled-contracts/DeploymentExample.TokenContract.nef \\"
echo "       compiled-contracts/DeploymentExample.TokenContract.manifest.json \\"
echo "       null deployer"
echo "   - NFT: neoxp contract invoke $NFT_HASH update \\"
echo "       compiled-contracts/DeploymentExample.NFTContract.nef \\"
echo "       compiled-contracts/DeploymentExample.NFTContract.manifest.json \\"
echo "       null deployer"
echo "   - Governance: neoxp contract invoke $GOV_HASH update \\"
echo "       compiled-contracts/DeploymentExample.GovernanceContract.nef \\"
echo "       compiled-contracts/DeploymentExample.GovernanceContract.manifest.json \\"
echo "       null deployer"