#!/bin/bash

# Build script for multiple contract projects

echo "Building contract projects..."

# Build TokenContract
echo "Building TokenContract..."
dotnet build src/TokenContract/TokenContract.csproj -c Release
if [ $? -ne 0 ]; then
    echo "TokenContract build failed"
    exit 1
fi

# Build NFTContract
echo "Building NFTContract..."
dotnet build src/NFTContract/NFTContract.csproj -c Release
if [ $? -ne 0 ]; then
    echo "NFTContract build failed"
    exit 1
fi

# Build GovernanceContract
echo "Building GovernanceContract..."
dotnet build src/GovernanceContract/GovernanceContract.csproj -c Release
if [ $? -ne 0 ]; then
    echo "GovernanceContract build failed"
    exit 1
fi

# Build legacy contract for compatibility
echo "Building DeploymentExample.Contract..."
dotnet build src/DeploymentExample.Contract/DeploymentExample.Contract.csproj -c Release
if [ $? -ne 0 ]; then
    echo "DeploymentExample.Contract build failed"
    exit 1
fi

echo "All contracts built successfully!"

# Create output directory for compiled contracts
mkdir -p compiled-contracts

echo "Contract build complete. Use nccs to compile the contracts to NEF format."