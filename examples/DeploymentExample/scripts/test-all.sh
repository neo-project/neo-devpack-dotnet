#!/bin/bash

# Run all tests for the deployment example
# Usage: ./test-all.sh

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}Neo Smart Contract Tests${NC}"
echo -e "${GREEN}========================${NC}"
echo ""

# Change to project root
cd "$(dirname "$0")/.."

# Build contracts
echo -e "${YELLOW}Building contracts...${NC}"
dotnet build --configuration Debug

# Run all tests with coverage
echo -e "${YELLOW}Running all tests with coverage...${NC}"
dotnet test \
    --no-build \
    --configuration Debug \
    --logger "console;verbosity=detailed" \
    --collect:"XPlat Code Coverage" \
    --results-directory ./test-results \
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover

# Check if tests passed
if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ All tests passed!${NC}"
    
    # Generate coverage report if ReportGenerator is installed
    if command -v reportgenerator &> /dev/null; then
        echo -e "${YELLOW}Generating coverage report...${NC}"
        reportgenerator \
            -reports:"./test-results/*/coverage.opencover.xml" \
            -targetdir:"./test-results/coverage-report" \
            -reporttypes:"Html;Badges"
        echo -e "${GREEN}Coverage report generated at: ./test-results/coverage-report/index.html${NC}"
    fi
else
    echo -e "${RED}❌ Some tests failed!${NC}"
    exit 1
fi

# Run specific test categories if requested
if [ "$1" = "--integration" ]; then
    echo -e "${YELLOW}Running integration tests...${NC}"
    dotnet test --no-build --filter "Category=Integration"
fi

if [ "$1" = "--security" ]; then
    echo -e "${YELLOW}Running security tests...${NC}"
    dotnet test --no-build --filter "Category=Security"
fi