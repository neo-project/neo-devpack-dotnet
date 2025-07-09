#!/bin/bash

# Deploy all contracts using the deployment toolkit
# Usage: ./deploy-all.sh [network] [wif-key]

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Default values
NETWORK=${1:-testnet}
WIF_KEY=${2:-$NEO_WIF_KEY}

# Check if WIF key is provided
if [ -z "$WIF_KEY" ]; then
    echo -e "${RED}Error: WIF key not provided. Set NEO_WIF_KEY environment variable or pass as argument.${NC}"
    exit 1
fi

echo -e "${GREEN}Neo Smart Contract Deployment Example${NC}"
echo -e "${GREEN}=====================================${NC}"
echo -e "Network: ${YELLOW}$NETWORK${NC}"
echo ""

# Change to project root
cd "$(dirname "$0")/.."

# Build all contracts
echo -e "${YELLOW}Building all contracts...${NC}"
dotnet build

# Run tests
echo -e "${YELLOW}Running tests...${NC}"
dotnet test --no-build

# Deploy using manifest
echo -e "${YELLOW}Deploying contracts to $NETWORK...${NC}"
cd deploy/DeploymentExample.Deploy

dotnet run -- deploy-manifest \
    -n "$NETWORK" \
    -w "$WIF_KEY" \
    -m ../../deployment-manifest.json \
    -o deployment-results.json \
    -v

echo -e "${GREEN}✅ Deployment completed successfully!${NC}"

# Display results
if [ -f "deployment-results.json" ]; then
    echo -e "${YELLOW}Deployment Results:${NC}"
    cat deployment-results.json | jq '.'
fi