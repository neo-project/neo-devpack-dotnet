#!/bin/bash

# Build script for multi-contract deployment example
# This script compiles all contracts and prepares them for deployment

set -e

echo "=== Building Multi-Contract Deployment Example ==="
echo

# Navigate to project root
cd "$(dirname "$0")"

# Clean previous builds
echo "Cleaning previous builds..."
dotnet clean
rm -rf src/DeploymentExample.Contract/bin/
rm -rf src/DeploymentExample.Contract/obj/

# Build contracts
echo "Building contracts..."
dotnet build src/DeploymentExample.Contract/DeploymentExample.Contract.csproj -c Release

# Build deployment application
echo "Building deployment application..."
dotnet build deploy/DeploymentExample.Deploy/DeploymentExample.Deploy.csproj -c Release

# Build tests
echo "Building tests..."
dotnet build tests/DeploymentExample.Tests/DeploymentExample.Tests.csproj -c Release

echo
echo "=== Build Complete ==="
echo "Contract binaries: src/DeploymentExample.Contract/bin/Release/net9.0/"
echo "Deployment app: deploy/DeploymentExample.Deploy/bin/Release/net9.0/"
echo "Tests: tests/DeploymentExample.Tests/bin/Release/net9.0/"
echo
echo "To deploy contracts:"
echo "  cd deploy/DeploymentExample.Deploy"
echo "  dotnet run multi local"
echo
echo "To run tests:"
echo "  dotnet test"