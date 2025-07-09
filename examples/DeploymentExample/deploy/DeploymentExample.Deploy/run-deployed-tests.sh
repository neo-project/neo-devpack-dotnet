#!/bin/bash

# Script to test already deployed contracts on testnet
# Contract addresses:
# - TokenContract: 0xe5af8922400736cfac7337955dcd0e8d98f608cd
# - NFTContract: 0xea414034cc25ddcc681ef6841a178ad4a0bc37e2
# - GovernanceContract: 0x5d773713bcfb164d7f7f8e6877269341f9f6c2b1

echo "Testing deployed contracts on testnet..."
echo ""

# Run the testdeployed command
dotnet run testdeployed