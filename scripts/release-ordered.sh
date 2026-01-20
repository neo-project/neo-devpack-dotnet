#!/bin/bash
#
# Ordered release script for neo-devpack-dotnet
# This script releases packages in the correct dependency order to avoid
# "dependency not found" errors during the release process.
#
# Usage: ./scripts/release-ordered.sh [--dry-run] [--source <nuget-source>] [--api-key <key>]
#

set -euo pipefail

# Default values
DRY_RUN=false
NUGET_SOURCE="https://api.nuget.org/v3/index.json"
API_KEY=""
CONFIG="Release"
OUTPUT_DIR="./pub"
PYTHON_BIN="python3"
PACKAGE_BASE=""
WAIT_FOR_PACKAGES=true

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
rm -rf "$OUTPUT_DIR"
mkdir -p "$OUTPUT_DIR"

# Resolve Python interpreter for JSON parsing
if ! command -v "$PYTHON_BIN" >/dev/null 2>&1; then
    PYTHON_BIN="python"
fi
if ! command -v "$PYTHON_BIN" >/dev/null 2>&1; then
    echo "Error: python3 (or python) is required to parse NuGet service index."
    exit 1
fi

# Resolve PackageBaseAddress from NuGet service index
resolve_package_base() {
    "$PYTHON_BIN" - "$NUGET_SOURCE" <<'PY'
import json
import sys
import urllib.request

url = sys.argv[1]
try:
    with urllib.request.urlopen(url) as response:
        data = json.load(response)
except Exception:
    sys.exit(1)

resources = data.get("resources", [])
for resource in resources:
    types = resource.get("@type", [])
    if isinstance(types, str):
        types = [types]
    if any(t.startswith("PackageBaseAddress") for t in types):
        print(resource.get("@id", "").rstrip("/"))
        sys.exit(0)
sys.exit(2)
PY
}

PACKAGE_BASE=$(resolve_package_base || true)
if [[ -z "$PACKAGE_BASE" ]]; then
    WAIT_FOR_PACKAGES=false
    echo "Warning: Could not resolve PackageBaseAddress from $NUGET_SOURCE; skipping package availability checks."
else
    echo "Package base address: $PACKAGE_BASE"
fi

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

# Function to wait for package availability
wait_for_package() {
    local package_name=$1
    local version=$2
    local max_attempts=30
    local wait_time=10
    
    if [[ "$DRY_RUN" == "true" ]]; then
        return 0
    fi
    if [[ "$WAIT_FOR_PACKAGES" == "false" ]]; then
        return 0
    fi
    
    echo "Waiting for $package_name version $version to be available..."
    
    for i in $(seq 1 $max_attempts); do
        local package_id
        package_id=$(echo "$package_name" | tr '[:upper:]' '[:lower:]')
        if "$PYTHON_BIN" - "$PACKAGE_BASE" "$package_id" "$version" <<'PY'
import json
import sys
import urllib.request

base, package_id, version = sys.argv[1], sys.argv[2], sys.argv[3]
url = f"{base}/{package_id}/index.json"
try:
    with urllib.request.urlopen(url) as response:
        data = json.load(response)
except Exception:
    sys.exit(1)

versions = data.get("versions", [])
target = version.lower()
if any(v.lower() == target for v in versions):
    sys.exit(0)
sys.exit(1)
PY
        then
            echo "✓ Package $package_name $version is now available"
            return 0
        fi
        echo "Attempt $i/$max_attempts: Package not yet available, waiting ${wait_time}s..."
        sleep $wait_time
    done
    
    echo "Error: Package $package_name $version did not become available after $((max_attempts * wait_time)) seconds"
    return 1
}

# Get version from Directory.Build.props
VERSION=$(sed -n 's/.*<VersionPrefix>\(.*\)<\/VersionPrefix>.*/\1/p' ./src/Directory.Build.props | head -1)
echo "Version to release: $VERSION"

# Remove Neo core projects from solution to avoid building them
echo ""
echo "Preparing solution..."
echo "-------------------------------------------------"
cp neo-devpack-dotnet.sln neo-devpack-dotnet.release.sln
dotnet sln neo-devpack-dotnet.release.sln remove ./neo/src/Neo/Neo.csproj || true
dotnet sln neo-devpack-dotnet.release.sln remove ./neo/src/Neo.Cryptography.BLS12_381/Neo.Cryptography.BLS12_381.csproj || true
dotnet sln neo-devpack-dotnet.release.sln remove ./neo/src/Neo.Extensions/Neo.Extensions.csproj || true
dotnet sln neo-devpack-dotnet.release.sln remove ./neo/src/Neo.IO/Neo.IO.csproj || true
dotnet sln neo-devpack-dotnet.release.sln remove ./neo/src/Neo.Json/Neo.Json.csproj || true
dotnet sln neo-devpack-dotnet.release.sln remove ./neo/src/Neo.VM/Neo.VM.csproj || true

# Restore all packages first
echo ""
echo "Restoring packages..."
echo "-------------------------------------------------"
dotnet restore neo-devpack-dotnet.release.sln

# Release packages in dependency order
echo ""
echo "Starting ordered release..."
echo "================================================="

# 1. Neo.SmartContract.Framework (no dependencies)
pack_and_push "./src/Neo.SmartContract.Framework/Neo.SmartContract.Framework.csproj"
wait_for_package "Neo.SmartContract.Framework" "$VERSION"

# 2. Neo.SmartContract.Analyzer (depends on Framework)
pack_and_push "./src/Neo.SmartContract.Analyzer/Neo.SmartContract.Analyzer.csproj"

# 3. Neo.Disassembler.CSharp (depends on external Neo libs only)
pack_and_push "./src/Neo.Disassembler.CSharp/Neo.Disassembler.CSharp.csproj"

# 4. Neo.SmartContract.Testing (depends on Disassembler)
# Note: This requires special handling as it references Neo core directly
echo ""
echo "Special handling for Neo.SmartContract.Testing..."
echo "-------------------------------------------------"

# Temporarily replace Neo project reference with package reference
if [[ -f "./src/Neo.SmartContract.Testing/Neo.SmartContract.Testing.csproj.backup" ]]; then
    rm "./src/Neo.SmartContract.Testing/Neo.SmartContract.Testing.csproj.backup"
fi
cp "./src/Neo.SmartContract.Testing/Neo.SmartContract.Testing.csproj" \
   "./src/Neo.SmartContract.Testing/Neo.SmartContract.Testing.csproj.backup"

# Get Neo package version
NEO_VERSION=$(sed -n 's/.*<VersionPrefix>\(.*\)<\/VersionPrefix>.*/\1/p' ./neo/src/Directory.Build.props | head -1)
echo "Neo package version: $NEO_VERSION"

# Replace project reference with package reference
dotnet remove ./src/Neo.SmartContract.Testing/Neo.SmartContract.Testing.csproj reference '..\..\neo\src\Neo\Neo.csproj' || true
dotnet add ./src/Neo.SmartContract.Testing/Neo.SmartContract.Testing.csproj package 'Neo' --version "$NEO_VERSION"

# Fix RpcStore.cs for package reference compatibility
echo "Fixing RpcStore.cs for package reference compatibility..."
sed -i.bak 's/public event IStore\.OnNewSnapshotDelegate/public event Action<IStore, IStoreSnapshot>/g' \
    "./src/Neo.SmartContract.Testing/Storage/Rpc/RpcStore.cs"

dotnet restore ./src/Neo.SmartContract.Testing/Neo.SmartContract.Testing.csproj

pack_and_push "./src/Neo.SmartContract.Testing/Neo.SmartContract.Testing.csproj"
wait_for_package "Neo.SmartContract.Testing" "$VERSION"

# Restore original project file and RpcStore.cs
mv "./src/Neo.SmartContract.Testing/Neo.SmartContract.Testing.csproj.backup" \
   "./src/Neo.SmartContract.Testing/Neo.SmartContract.Testing.csproj"
mv "./src/Neo.SmartContract.Testing/Storage/Rpc/RpcStore.cs.bak" \
   "./src/Neo.SmartContract.Testing/Storage/Rpc/RpcStore.cs"

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

# Clean up
rm -f neo-devpack-dotnet.release.sln

echo ""
echo "================================================="
echo "Release completed successfully!"
echo "Released version: $VERSION"
echo "Packages published to: $NUGET_SOURCE"
echo "================================================="
