#!/bin/bash
# Script to interact with deployed contract

echo "Building and running contract interaction..."
dotnet build
dotnet run --project . -- interact
