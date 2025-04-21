#!/bin/bash

echo "Neo Smart Contract Fuzzer for Neo N3"
echo "============================="

# Get the script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Build the project
echo "Building project..."
dotnet build "$SCRIPT_DIR/Neo.SmartContract.Fuzzer.csproj"

# Run the fuzzer
echo "Running fuzzer..."
dotnet run --project "$SCRIPT_DIR/Neo.SmartContract.Fuzzer.csproj" -- "$@"

echo "Fuzzing completed."