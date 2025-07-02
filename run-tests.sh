#!/bin/bash

echo "Running Neo Smart Contract Deploy Tests"
echo "======================================="
echo ""

# Run unit tests (excluding integration tests)
echo "Running Unit Tests..."
dotnet test tests/Neo.SmartContract.Deploy.UnitTests \
  --filter "FullyQualifiedName!~Integration" \
  --verbosity quiet

# Check if Neo Express is running
echo ""
echo "Checking for Neo Express..."
if nc -z localhost 50012 2>/dev/null; then
    echo "Neo Express is running on port 50012"
    echo "Running Integration Tests..."
    dotnet test tests/Neo.SmartContract.Deploy.UnitTests \
      --filter "FullyQualifiedName~Integration" \
      --verbosity quiet
else
    echo "Neo Express is not running on port 50012"
    echo "Integration tests require Neo Express to be running."
    echo "To start Neo Express, run: neo-express run -s 1"
fi