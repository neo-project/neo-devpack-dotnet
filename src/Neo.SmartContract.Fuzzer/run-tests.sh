#!/bin/bash

# Navigate to the project directory
cd "$(dirname "$0")"

# Build the solution
echo "Building solution..."
dotnet build

# Run all tests
echo "Running all tests..."
dotnet test --logger "console;verbosity=detailed"

# Check if coverage.html exists
if [ -f "tests/Neo.SmartContract.Fuzzer.Tests/bin/Debug/net9.0/coverage.html" ]; then
    echo "Coverage report generated: tests/Neo.SmartContract.Fuzzer.Tests/bin/Debug/net9.0/coverage.html"
    
    # Open the coverage report in the default browser (if available)
    if command -v open &> /dev/null; then
        open "tests/Neo.SmartContract.Fuzzer.Tests/bin/Debug/net9.0/coverage.html"
    elif command -v xdg-open &> /dev/null; then
        xdg-open "tests/Neo.SmartContract.Fuzzer.Tests/bin/Debug/net9.0/coverage.html"
    elif command -v start &> /dev/null; then
        start "tests/Neo.SmartContract.Fuzzer.Tests/bin/Debug/net9.0/coverage.html"
    else
        echo "Please open the coverage report manually."
    fi
fi

echo "Tests completed."