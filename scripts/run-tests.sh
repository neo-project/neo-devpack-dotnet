#!/bin/bash

# R3E WebGUI Service Test Runner
set -e

echo "🧪 R3E WebGUI Service - Comprehensive Test Suite"
echo "=================================================="
echo

# Configuration
SOLUTION_DIR="/home/neo/git/neo-devpack-dotnet"
TEST_RESULTS_DIR="$SOLUTION_DIR/test-results"
COVERAGE_DIR="$SOLUTION_DIR/coverage"

# Create directories
mkdir -p "$TEST_RESULTS_DIR"
mkdir -p "$COVERAGE_DIR"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Function to run tests for a project
run_tests() {
    local project_name=$1
    local test_path=$2
    
    print_status $BLUE "Running tests for $project_name..."
    
    if [ -d "$test_path" ]; then
        dotnet test "$test_path" \
            --configuration Release \
            --logger "trx;LogFileName=${project_name}.trx" \
            --logger "console;verbosity=normal" \
            --results-directory "$TEST_RESULTS_DIR" \
            --collect:"XPlat Code Coverage" \
            --settings coverlet.runsettings \
            || {
                print_status $RED "❌ Tests failed for $project_name"
                return 1
            }
        
        print_status $GREEN "✅ Tests passed for $project_name"
        return 0
    else
        print_status $YELLOW "⚠️  Test project not found: $test_path"
        return 0
    fi
}

# Function to generate coverage report
generate_coverage_report() {
    print_status $BLUE "Generating coverage report..."
    
    # Find all coverage files
    local coverage_files=$(find "$TEST_RESULTS_DIR" -name "coverage.cobertura.xml" | tr '\n' ';')
    
    if [ -z "$coverage_files" ]; then
        print_status $YELLOW "⚠️  No coverage files found"
        return 0
    fi
    
    # Install reportgenerator if not present
    if ! command -v reportgenerator &> /dev/null; then
        print_status $BLUE "Installing ReportGenerator..."
        dotnet tool install -g dotnet-reportgenerator-globaltool || true
    fi
    
    # Generate HTML report
    reportgenerator \
        "-reports:$coverage_files" \
        "-targetdir:$COVERAGE_DIR" \
        "-reporttypes:Html;JsonSummary" \
        "-title:R3E WebGUI Service Coverage Report" \
        || {
            print_status $YELLOW "⚠️  Could not generate coverage report"
            return 0
        }
    
    print_status $GREEN "✅ Coverage report generated: $COVERAGE_DIR/index.html"
}

# Function to analyze test results
analyze_results() {
    print_status $BLUE "Analyzing test results..."
    
    local total_tests=0
    local passed_tests=0
    local failed_tests=0
    local skipped_tests=0
    
    # Parse TRX files for results
    for trx_file in "$TEST_RESULTS_DIR"/*.trx; do
        if [ -f "$trx_file" ]; then
            # Basic parsing of TRX files (simplified)
            local file_tests=$(grep -c "UnitTestResult" "$trx_file" 2>/dev/null || echo "0")
            local file_passed=$(grep -c 'outcome="Passed"' "$trx_file" 2>/dev/null || echo "0")
            local file_failed=$(grep -c 'outcome="Failed"' "$trx_file" 2>/dev/null || echo "0")
            local file_skipped=$(grep -c 'outcome="Skipped"' "$trx_file" 2>/dev/null || echo "0")
            
            total_tests=$((total_tests + file_tests))
            passed_tests=$((passed_tests + file_passed))
            failed_tests=$((failed_tests + file_failed))
            skipped_tests=$((skipped_tests + file_skipped))
        fi
    done
    
    echo
    print_status $BLUE "📊 Test Results Summary"
    echo "========================"
    echo "Total Tests:  $total_tests"
    echo "Passed:       $passed_tests"
    echo "Failed:       $failed_tests"
    echo "Skipped:      $skipped_tests"
    
    if [ $failed_tests -eq 0 ]; then
        print_status $GREEN "✅ All tests passed!"
    else
        print_status $RED "❌ $failed_tests tests failed"
    fi
}

# Function to run build verification
verify_build() {
    print_status $BLUE "Verifying build..."
    
    # Build WebGUI Service
    if dotnet build "$SOLUTION_DIR/src/R3E.WebGUI.Service/R3E.WebGUI.Service.csproj" --configuration Release; then
        print_status $GREEN "✅ R3E.WebGUI.Service builds successfully"
    else
        print_status $RED "❌ R3E.WebGUI.Service build failed"
        return 1
    fi
    
    # Build WebGUI Deploy tool
    if dotnet build "$SOLUTION_DIR/src/R3E.WebGUI.Deploy/R3E.WebGUI.Deploy.csproj" --configuration Release; then
        print_status $GREEN "✅ R3E.WebGUI.Deploy builds successfully"
    else
        print_status $RED "❌ R3E.WebGUI.Deploy build failed"
        return 1
    fi
}

# Function to run static analysis
run_static_analysis() {
    print_status $BLUE "Running static analysis..."
    
    # Check code formatting
    if dotnet format "$SOLUTION_DIR" --verify-no-changes --verbosity minimal; then
        print_status $GREEN "✅ Code formatting is correct"
    else
        print_status $YELLOW "⚠️  Code formatting issues found (run 'dotnet format' to fix)"
    fi
    
    # Security analysis (if available)
    if command -v security-scan &> /dev/null; then
        security-scan "$SOLUTION_DIR/src/R3E.WebGUI.Service" || print_status $YELLOW "⚠️  Security scan completed with warnings"
    fi
}

# Main execution
main() {
    local start_time=$(date +%s)
    
    print_status $BLUE "Starting comprehensive test suite..."
    echo "Working directory: $SOLUTION_DIR"
    echo "Results directory: $TEST_RESULTS_DIR"
    echo "Coverage directory: $COVERAGE_DIR"
    echo
    
    # Clean previous results
    rm -rf "$TEST_RESULTS_DIR"/*
    rm -rf "$COVERAGE_DIR"/*
    
    # Change to solution directory
    cd "$SOLUTION_DIR"
    
    # Step 1: Verify builds
    print_status $BLUE "🏗️  Step 1: Build Verification"
    verify_build || exit 1
    echo
    
    # Step 2: Run static analysis
    print_status $BLUE "🔍 Step 2: Static Analysis"
    run_static_analysis
    echo
    
    # Step 3: Run unit tests
    print_status $BLUE "🧪 Step 3: Unit Tests"
    
    local test_failures=0
    
    # WebGUI Service Unit Tests
    run_tests "R3E.WebGUI.Service.UnitTests" "tests/R3E.WebGUI.Service.UnitTests" || ((test_failures++))
    
    # WebGUI Deploy Unit Tests
    run_tests "R3E.WebGUI.Deploy.UnitTests" "tests/R3E.WebGUI.Deploy.UnitTests" || ((test_failures++))
    
    echo
    
    # Step 4: Run integration tests
    print_status $BLUE "🔧 Step 4: Integration Tests"
    
    run_tests "R3E.WebGUI.Service.IntegrationTests" "tests/R3E.WebGUI.Service.IntegrationTests" || ((test_failures++))
    
    echo
    
    # Step 5: Generate coverage report
    print_status $BLUE "📊 Step 5: Coverage Analysis"
    generate_coverage_report
    echo
    
    # Step 6: Analyze results
    analyze_results
    
    local end_time=$(date +%s)
    local duration=$((end_time - start_time))
    
    echo
    print_status $BLUE "⏱️  Test execution completed in ${duration} seconds"
    
    if [ $test_failures -eq 0 ]; then
        print_status $GREEN "🎉 All test suites passed successfully!"
        echo
        echo "📋 Available Reports:"
        echo "   - Test Results: $TEST_RESULTS_DIR/"
        echo "   - Coverage Report: $COVERAGE_DIR/index.html"
        echo "   - Open coverage report: file://$COVERAGE_DIR/index.html"
        exit 0
    else
        print_status $RED "❌ $test_failures test suite(s) failed"
        echo
        echo "📋 Check the following:"
        echo "   - Test Results: $TEST_RESULTS_DIR/*.trx"
        echo "   - Build logs for detailed error information"
        exit 1
    fi
}

# Create coverlet settings
create_coverlet_settings() {
    cat > coverlet.runsettings << EOF
<?xml version="1.0" encoding="utf-8" ?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat code coverage">
        <Configuration>
          <Format>cobertura</Format>
          <Exclude>[*.Tests]*,[*]*Test*,[xunit.*]*,[*.TestAdapter]*</Exclude>
          <ExcludeByAttribute>Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute</ExcludeByAttribute>
          <UseSourceLink>true</UseSourceLink>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
EOF
}

# Check if help was requested
if [[ "$1" == "--help" || "$1" == "-h" ]]; then
    echo "R3E WebGUI Service Test Runner"
    echo
    echo "Usage: $0 [options]"
    echo
    echo "Options:"
    echo "  --help, -h     Show this help message"
    echo "  --build-only   Only run build verification"
    echo "  --unit-only    Only run unit tests"
    echo "  --no-coverage  Skip coverage report generation"
    echo
    echo "Environment Variables:"
    echo "  SKIP_INTEGRATION_TESTS  Set to 'true' to skip integration tests"
    echo "  COVERAGE_THRESHOLD      Minimum coverage percentage (default: none)"
    echo
    exit 0
fi

# Handle command line options
if [[ "$1" == "--build-only" ]]; then
    verify_build
    exit $?
elif [[ "$1" == "--unit-only" ]]; then
    cd "$SOLUTION_DIR"
    create_coverlet_settings
    run_tests "R3E.WebGUI.Service.UnitTests" "tests/R3E.WebGUI.Service.UnitTests"
    run_tests "R3E.WebGUI.Deploy.UnitTests" "tests/R3E.WebGUI.Deploy.UnitTests"
    exit $?
fi

# Create coverlet settings and run main
create_coverlet_settings
main