#!/bin/bash

# Deploy the voting contract to Neo Express
echo "Deploying Privacy-Preserving Voting Contract..."

# Build the contract
echo "Building contract..."
dotnet build

# Deploy using Neo Express
echo "Deploying to Neo Express..."
neoxp contract deploy ./bin/Debug/net9.0/PrivateVotingContract.nef admin

# Get contract hash
CONTRACT_HASH=$(neoxp contract get PrivateVotingContract | grep "Script Hash" | awk '{print $3}')
echo "Contract deployed at: $CONTRACT_HASH"

# Save contract hash for later use
echo $CONTRACT_HASH > contract-hash.txt

echo "Deployment complete!"