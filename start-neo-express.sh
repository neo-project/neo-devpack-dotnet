#!/bin/bash

# Script to start Neo Express for integration testing

echo "Starting Neo Express for integration tests..."
echo "==========================================="

# Check if neo-express is installed (can be neo-express or neoxp)
if command -v neoxp &> /dev/null; then
    NEO_EXPRESS_CMD="neoxp"
elif command -v neo-express &> /dev/null; then
    NEO_EXPRESS_CMD="neo-express"
else
    echo "Error: neo-express is not installed."
    echo ""
    echo "To install neo-express, run:"
    echo "  dotnet tool install -g Neo.Express"
    echo ""
    echo "Or if you prefer to install it locally:"
    echo "  dotnet tool install Neo.Express"
    echo ""
    exit 1
fi

echo "Using Neo Express command: $NEO_EXPRESS_CMD"

# Check if the configuration file exists
if [ ! -f "default.neo-express" ]; then
    echo "Error: default.neo-express configuration file not found."
    echo "Please ensure you're running this script from the project root directory."
    exit 1
fi

# Stop any existing Neo Express instance
echo "Stopping any existing Neo Express instances..."
$NEO_EXPRESS_CMD stop || true

# Start Neo Express with our configuration
echo "Starting Neo Express on port 50012..."
$NEO_EXPRESS_CMD run -i default.neo-express -s 1 > neo-express.log 2>&1 &
NEO_EXPRESS_PID=$!

# Wait for it to be ready with timeout
echo "Waiting for Neo Express to be ready..."
TIMEOUT=30
COUNTER=0

while [ $COUNTER -lt $TIMEOUT ]; do
    if nc -z localhost 50012 2>/dev/null; then
        echo ""
        echo "✅ Neo Express is running successfully!"
        echo ""
        echo "RPC endpoint: http://localhost:50012"
        echo "Process ID: $NEO_EXPRESS_PID"
        echo "Log file: neo-express.log"
        echo ""
        echo "To stop Neo Express, run: ./stop-neo-express.sh"
        echo "To run integration tests: ./run-integration-workflow.sh"
        echo "To run all tests: ./run-tests.sh"
        exit 0
    fi
    sleep 1
    COUNTER=$((COUNTER + 1))
    printf "."
done

echo ""
echo "❌ Failed to start Neo Express within $TIMEOUT seconds."
echo "Check neo-express.log for error messages:"
tail -10 neo-express.log 2>/dev/null || echo "No log file found"
exit 1