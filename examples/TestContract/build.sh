#!/bin/bash

# Exit on error
set -e

# Get the root directory of the neo-devpack-dotnet project
ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo "Building TestContract..."

# Build the contract and generate the interface
cd "$ROOT_DIR"
dotnet run --project src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj -- \
    examples/TestContract/TestContract.csproj \
    --generate-interface

# Check if the build was successful
if [ $? -eq 0 ]; then
    echo -e "${GREEN}Build successful!${NC}"
    echo -e "${GREEN}Contract files:${NC}"
    ls -la examples/TestContract/bin/sc/
else
    echo -e "${RED}Build failed.${NC}"
    exit 1
fi

# Check if the interface file was generated
if [ -f "examples/TestContract/bin/sc/IHelloWorldContract.cs" ]; then
    echo -e "${GREEN}Interface file generated successfully.${NC}"
    echo -e "Interface file: examples/TestContract/bin/sc/IHelloWorldContract.cs"
else
    echo -e "${RED}Interface file was not generated.${NC}"
    exit 1
fi

echo "Done!"
