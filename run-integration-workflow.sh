#!/bin/bash

# Complete Neo Express Integration Test Workflow
# This script:
# 1. Starts Neo Express
# 2. Waits for it to be ready
# 3. Runs integration tests
# 4. Stops Neo Express
# 5. Reports results

set -e  # Exit on any error

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Track overall workflow status
WORKFLOW_SUCCESS=0
NEO_EXPRESS_STARTED=0
TEST_RESULTS=""

# Function to print colored output
print_status() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Function to cleanup on exit
cleanup() {
    if [ $NEO_EXPRESS_STARTED -eq 1 ]; then
        print_status $YELLOW "🧹 Cleaning up Neo Express..."
        ./stop-neo-express.sh
    fi
}

# Set trap to ensure cleanup happens even if script is interrupted
trap cleanup EXIT INT TERM

echo "======================================================================"
print_status $BLUE "🚀 Neo Smart Contract Deploy - Integration Test Workflow"
echo "======================================================================"
echo ""

# Step 1: Check prerequisites
print_status $BLUE "📋 Step 1: Checking prerequisites..."
echo "----------------------------------------------------------------------"

# Check if scripts exist
if [ ! -f "start-neo-express.sh" ]; then
    print_status $RED "❌ start-neo-express.sh not found"
    exit 1
fi

if [ ! -f "stop-neo-express.sh" ]; then
    print_status $RED "❌ stop-neo-express.sh not found"
    exit 1
fi

# Check if neo-express is installed (can be neo-express or neoxp)
if command -v neoxp &> /dev/null; then
    NEO_EXPRESS_CMD="neoxp"
    print_status $GREEN "✅ Found Neo Express command: neoxp"
elif command -v neo-express &> /dev/null; then
    NEO_EXPRESS_CMD="neo-express"
    print_status $GREEN "✅ Found Neo Express command: neo-express"
else
    print_status $RED "❌ neo-express is not installed"
    echo ""
    echo "To install neo-express, run:"
    echo "  dotnet tool install -g Neo.Express"
    exit 1
fi

# Check if configuration exists
if [ ! -f "default.neo-express" ]; then
    print_status $RED "❌ default.neo-express configuration not found"
    exit 1
fi

print_status $GREEN "✅ All prerequisites met"
echo ""

# Step 2: Stop any existing Neo Express instances
print_status $BLUE "🛑 Step 2: Stopping any existing Neo Express instances..."
echo "----------------------------------------------------------------------"
./stop-neo-express.sh
echo ""

# Step 3: Start Neo Express
print_status $BLUE "🏁 Step 3: Starting Neo Express..."
echo "----------------------------------------------------------------------"

# Start Neo Express in background
echo "Starting Neo Express..."
$NEO_EXPRESS_CMD run -i default.neo-express -s 1 > neo-express.log 2>&1 &
NEO_EXPRESS_PID=$!
NEO_EXPRESS_STARTED=1

# Wait for Neo Express to be ready (with timeout)
echo "Waiting for Neo Express to be ready..."
TIMEOUT=30
COUNTER=0

while [ $COUNTER -lt $TIMEOUT ]; do
    if nc -z localhost 50012 2>/dev/null; then
        print_status $GREEN "✅ Neo Express is ready on port 50012"
        break
    fi
    sleep 1
    COUNTER=$((COUNTER + 1))
    printf "."
done

echo ""

if [ $COUNTER -eq $TIMEOUT ]; then
    print_status $RED "❌ Neo Express failed to start within $TIMEOUT seconds"
    echo "Check neo-express.log for details:"
    tail -20 neo-express.log
    exit 1
fi

# Give it an extra moment to fully initialize
sleep 2
echo ""

# Step 4: Run integration tests
print_status $BLUE "🧪 Step 4: Running integration tests..."
echo "----------------------------------------------------------------------"

# Run only integration tests
echo "Running integration tests..."
if dotnet test tests/Neo.SmartContract.Deploy.UnitTests --filter "FullyQualifiedName~Integration" --verbosity normal --logger "console;verbosity=detailed"; then
    TEST_RESULTS="PASSED"
    print_status $GREEN "✅ Integration tests completed successfully"
else
    TEST_RESULTS="FAILED"
    WORKFLOW_SUCCESS=1
    print_status $RED "❌ Integration tests failed"
fi

echo ""

# Step 5: Run a quick test summary
print_status $BLUE "📊 Step 5: Generating test summary..."
echo "----------------------------------------------------------------------"

# Get detailed test results
INTEGRATION_RESULT=$(dotnet test tests/Neo.SmartContract.Deploy.UnitTests --filter "FullyQualifiedName~Integration" --verbosity quiet 2>&1)
INTEGRATION_PASSED=$(echo "$INTEGRATION_RESULT" | grep -o "Passed:[[:space:]]*[0-9]*" | grep -o "[0-9]*" || echo "0")
INTEGRATION_FAILED=$(echo "$INTEGRATION_RESULT" | grep -o "Failed:[[:space:]]*[0-9]*" | grep -o "[0-9]*" || echo "0")
INTEGRATION_TOTAL=$(echo "$INTEGRATION_RESULT" | grep -o "Total:[[:space:]]*[0-9]*" | grep -o "[0-9]*" || echo "0")

echo "Integration Test Results:"
echo "------------------------"
echo "Passed: $INTEGRATION_PASSED"
echo "Failed: $INTEGRATION_FAILED"
echo "Total:  $INTEGRATION_TOTAL"

if [ "$INTEGRATION_TOTAL" -gt 0 ]; then
    PASS_RATE=$(echo "scale=1; $INTEGRATION_PASSED * 100 / $INTEGRATION_TOTAL" | bc -l 2>/dev/null || echo "0")
    echo "Pass Rate: ${PASS_RATE}%"
fi

echo ""

# Step 6: Cleanup will happen automatically via trap
print_status $BLUE "🧹 Step 6: Cleanup (automatic)..."
echo "----------------------------------------------------------------------"
echo "Neo Express will be stopped automatically..."
echo ""

# Final summary
echo "======================================================================"
print_status $BLUE "📋 Workflow Summary"
echo "======================================================================"

if [ $WORKFLOW_SUCCESS -eq 0 ]; then
    print_status $GREEN "🎉 WORKFLOW COMPLETED SUCCESSFULLY!"
    echo ""
    echo "✅ Neo Express started and stopped cleanly"
    echo "✅ Integration tests executed"
    echo "✅ Results: $INTEGRATION_PASSED/$INTEGRATION_TOTAL integration tests passed"
else
    print_status $RED "❌ WORKFLOW COMPLETED WITH ISSUES"
    echo ""
    echo "⚠️  Some integration tests failed"
    echo "📝 Check the test output above for details"
    echo "📋 Results: $INTEGRATION_PASSED/$INTEGRATION_TOTAL integration tests passed"
fi

echo ""
echo "Log files:"
echo "- Neo Express log: neo-express.log"
echo ""

exit $WORKFLOW_SUCCESS