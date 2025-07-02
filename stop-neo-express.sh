#!/bin/bash

# Script to stop Neo Express cleanly

echo "Stopping Neo Express..."
echo "======================"

# Check if neo-express is installed (can be neo-express or neoxp)
if command -v neoxp &> /dev/null; then
    NEO_EXPRESS_CMD="neoxp"
elif command -v neo-express &> /dev/null; then
    NEO_EXPRESS_CMD="neo-express"
else
    echo "Warning: neo-express is not installed or not in PATH."
    echo "Attempting to kill processes manually..."
    NEO_EXPRESS_CMD=""
fi

# Stop Neo Express using the official command
if [ ! -z "$NEO_EXPRESS_CMD" ]; then
    echo "Stopping Neo Express via official command ($NEO_EXPRESS_CMD)..."
    $NEO_EXPRESS_CMD stop 2>/dev/null || echo "neo-express stop command failed"
fi

# Also kill any remaining processes (backup method)
echo "Checking for remaining Neo Express processes..."
pkill -f "neo-express" 2>/dev/null || echo "No neo-express processes found"
pkill -f "Neo.ConsensusNode" 2>/dev/null || echo "No Neo.ConsensusNode processes found"

# Wait a moment for processes to terminate
sleep 2

# Check if port 50012 is still in use
if nc -z localhost 50012 2>/dev/null; then
    echo "Warning: Port 50012 is still in use. Attempting force kill..."
    # Find and kill process using port 50012
    PORT_PID=$(lsof -ti:50012 2>/dev/null)
    if [ ! -z "$PORT_PID" ]; then
        kill -9 $PORT_PID 2>/dev/null
        echo "Force killed process $PORT_PID using port 50012"
        sleep 1
    fi
fi

# Final check
if nc -z localhost 50012 2>/dev/null; then
    echo "❌ Failed to stop Neo Express completely. Port 50012 is still in use."
    echo "You may need to manually kill the process or restart your system."
    exit 1
else
    echo "✅ Neo Express stopped successfully!"
    echo "Port 50012 is now available."
fi