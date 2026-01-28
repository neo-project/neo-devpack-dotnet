#!/bin/bash
#
# Ordered release script for neo-devpack-dotnet
# This script releases packages in the correct dependency order to avoid
# "dependency not found" errors during the release process.
#
# Usage: ./scripts/release-ordered.sh [--dry-run] [--source <nuget-source>] [--api-key <key>]
#

set -e

# Default values
DRY_RUN=false
NUGET_SOURCE="https://api.nuget.org/v3/index.json"
API_KEY=""
CONFIG="Release"
OUTPUT_DIR="./pub"

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --dry-run)
            DRY_RUN=true
            shift
            ;;
        --source)
            NUGET_SOURCE="$2"
            shift 2
            ;;
        --api-key)
            API_KEY="$2"
            shift 2
            ;;
        --config)
            CONFIG="$2"
            shift 2
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Check API key
if [[ -z "$API_KEY" ]] && [[ "$DRY_RUN" == "false" ]]; then
    if [[ -n "$NUGET_TOKEN" ]]; then
        API_KEY="$NUGET_TOKEN"
    else
        echo "Error: API key is required for publishing. Use --api-key or set NUGET_TOKEN environment variable."
        exit 1
    fi
fi

echo "================================================="
echo "Neo DevPack Ordered Release Script"
echo "================================================="
echo "Configuration: $CONFIG"
echo "Output Directory: $OUTPUT_DIR"
echo "NuGet Source: $NUGET_SOURCE"
echo "Dry Run: $DRY_RUN"
echo "================================================="

# Create output directory
mkdir -p "$OUTPUT_DIR"

# Function to pack and push a project
pack_and_push() {
    local project_path=$1
    local project_name=$(basename "$project_path" .csproj)
    
    echo ""
    echo "Processing $project_name..."
    echo "-------------------------------------------------"
    
    # Pack the project
    echo "Packing $project_name..."
    dotnet pack "$project_path" \
        --configuration "$CONFIG" \
        --output "$OUTPUT_DIR" \
        --no-restore
    
    if [[ "$DRY_RUN" == "false" ]]; then
        # Push the package
        echo "Publishing $project_name to $NUGET_SOURCE..."
        dotnet nuget push "$OUTPUT_DIR/$project_name.*.nupkg" \
            --source "$NUGET_SOURCE" \
            --api-key "$API_KEY" \
            --skip-duplicate \
            --no-service-endpoint
    else
        echo "[DRY RUN] Would publish $project_name to $NUGET_SOURCE"
    fi
    
    echo "✓ $project_name completed"
}

# Function to wait for package availability with fallback
wait_for_package() {
    local package_name=$1
    local version=$2
    local max_attempts=30
    local wait_time=10
    
    if [[ "$DRY_RUN" == "true" ]]; then
        return 0
    fi
    
    echo "Waiting for $package_name version $version to be available..."
    
    # Note: Checking NuGet.org package availability via API is unreliable due to caching
    # We use a simple delay as a more reliable approach
    for i in $(seq 1 $max_attempts); do
        # Try to query the package - suppress errors as the command may fail
        if dotnet package search "$package_name" --source "$NUGET_SOURCE" --exact-match 2>/dev/null | grep -q "$version"; then
            echo "✓ Package $package_name $version is now available"
            return 0
        fi
        echo "Attempt $i/$max_attempts: Package not yet available, waiting ${wait_time}s..."
        sleep $wait_time
    done
    
    echo "Warning: Package $package_name $version may not be available yet, continuing anyway..."
    return 0
}

# Get version from Directory.Build.props
VERSION=$(sed -n 's/.*<VersionPrefix>\(.*\)<\/VersionPrefix>.*/\1/p' ./src/Directory.Build.props | head -1)
if [[ -z "$VERSION" ]]; then
    echo "Error: Could not determine version from src/Directory.Build.props"
    exit 1
fi
echo "Version to release: $VERSION"

# Restore all packages first
echo ""
echo "Restoring packages..."
echo "-------------------------------------------------"
dotnet restore ./neo-devpack-dotnet.sln

# Release packages in dependency order
echo ""
echo "Starting ordered release..."
echo "================================================="

# 1. Neo.SmartContract.Framework (no dependencies)
pack_and_push "./src/Neo.SmartContract.Framework/Neo.SmartContract.Framework.csproj"

# 2. Neo.SmartContract.Analyzer (depends on Framework)
pack_and_push "./src/Neo.SmartContract.Analyzer/Neo.SmartContract.Analyzer.csproj"

# 3. Neo.Disassembler.CSharp (depends on external Neo libs only)
pack_and_push "./src/Neo.Disassembler.CSharp/Neo.Disassembler.CSharp.csproj"

# 4. Neo.SmartContract.Testing (depends on Disassembler and Neo package)
# Note: Now uses NuGet package references for Neo, no special handling needed
pack_and_push "./src/Neo.SmartContract.Testing/Neo.SmartContract.Testing.csproj"

# 5. Neo.Compiler.CSharp (depends on Framework and Testing)
pack_and_push "./src/Neo.Compiler.CSharp/Neo.Compiler.CSharp.csproj"

# 6. Neo.SmartContract.Template (template package, special handling)
echo ""
echo "Processing Neo.SmartContract.Template..."
echo "-------------------------------------------------"
dotnet pack "./src/Neo.SmartContract.Template/Neo.SmartContract.Template.csproj" \
    --configuration "$CONFIG" \
    --output "$OUTPUT_DIR"

if [[ "$DRY_RUN" == "false" ]]; then
    dotnet nuget push "$OUTPUT_DIR/Neo.SmartContract.Template.*.nupkg" \
        --source "$NUGET_SOURCE" \
        --api-key "$API_KEY" \
        --skip-duplicate \
        --no-service-endpoint
else
    echo "[DRY RUN] Would publish Neo.SmartContract.Template to $NUGET_SOURCE"
fi
echo "✓ Neo.SmartContract.Template completed"

echo ""
echo "================================================="
echo "Release completed successfully!"
echo "Released version: $VERSION"
echo "Packages published to: $NUGET_SOURCE"
echo "================================================="
