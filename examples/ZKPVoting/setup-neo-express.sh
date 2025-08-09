#!/bin/bash

# Setup script for Neo Express environment
echo "Setting up Neo Express for Privacy-Preserving Voting..."

# Install Neo Express if not already installed
if ! command -v neoxp &> /dev/null; then
    echo "Installing Neo Express..."
    dotnet tool install -g Neo.Express
fi

# Create Neo Express configuration
echo "Creating Neo Express blockchain..."
neoxp create -f

# Create test wallets
echo "Creating test wallets..."
neoxp wallet create admin
neoxp wallet create voter1
neoxp wallet create voter2
neoxp wallet create voter3

# Fund wallets with GAS
echo "Funding wallets..."
neoxp transfer 1000 GAS genesis admin
neoxp transfer 100 GAS genesis voter1
neoxp transfer 100 GAS genesis voter2
neoxp transfer 100 GAS genesis voter3

# Start Neo Express
echo "Starting Neo Express..."
neoxp run --seconds-per-block 1 &
NEO_PID=$!

# Wait for blockchain to start
sleep 5

echo "Neo Express is running with PID: $NEO_PID"
echo "Setup complete!"
echo ""
echo "To deploy the contract, run:"
echo "  dotnet build"
echo "  ./deploy-contract.sh"