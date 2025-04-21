#!/bin/bash

echo "Dynamic Contract Fuzzer for Neo N3"
echo "==================================="

# Get the repository root directory
REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"

# Function to print help
print_help() {
    echo "Usage: ./run-compiler-fuzzer.sh [options]"
    echo ""
    echo "Options:"
    echo "  --iterations N       Number of contracts to generate (default: 5)"
    echo "  --features N         Number of features per contract (default: 3)"
    echo "  --output DIR         Output directory for generated contracts (default: GeneratedContracts)"
    echo "  --no-execution       Skip execution testing of generated contracts"
    echo "  --log-level LEVEL    Set log level (Debug, Info, Warning, Error)"
    echo "  --duration TIME      Run for specified duration (minutes, or use 'Xh' for hours, 'Xd' for days, 'Xw' for weeks, or 'indefinite')"
    echo "  --checkpoint-interval N  Minutes between checkpoints in long-running mode (default: 30)"
    echo "  --help               Display this help message"
    echo ""
    echo "Examples:"
    echo "  ./run-compiler-fuzzer.sh --iterations 10 --features 3"
    echo "  ./run-compiler-fuzzer.sh --output ./MyContracts --no-execution"
    echo "  ./run-compiler-fuzzer.sh --log-level Debug"
    echo "  ./run-compiler-fuzzer.sh --duration 24h --features 3"
    echo "  ./run-compiler-fuzzer.sh --duration 7d --checkpoint-interval 60"
    echo "  ./run-compiler-fuzzer.sh --duration indefinite --log-level Debug"
}

# Check if help is requested
if [[ " $* " == *" --help "* ]]; then
    print_help
    exit 0
fi

# Build the project
echo "Building project..."
dotnet build "$REPO_ROOT/src/Neo.Compiler.Fuzzer/Neo.Compiler.Fuzzer.csproj"

# Run the fuzzer with dynamic mode
echo "Running fuzzer in dynamic mode..."

# Check if duration is specified
DURATION=""
CHECKPOINT_INTERVAL=30
for arg in "$@"; do
    if [[ "$prev_arg" == "--duration" ]]; then
        DURATION="$arg"
    fi
    if [[ "$prev_arg" == "--checkpoint-interval" ]]; then
        CHECKPOINT_INTERVAL="$arg"
    fi
    prev_arg="$arg"
done

# Print duration information if specified
if [[ -n "$DURATION" ]]; then
    echo "Running for duration: $DURATION with checkpoint interval: $CHECKPOINT_INTERVAL minutes"
    echo "Press Ctrl+C to stop the fuzzer gracefully"
fi

# Run the fuzzer
dotnet run --project "$REPO_ROOT/src/Neo.Compiler.Fuzzer/Neo.Compiler.Fuzzer.csproj" -- dynamic "$@"

echo "Fuzzing completed."
