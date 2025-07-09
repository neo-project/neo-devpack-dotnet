#!/bin/bash

# Validate Neo Smart Contract Templates
# This script validates all templates and ensures they work correctly

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
TEMP_DIR="/tmp/neo-template-validation-$$"

# Create temp directory
mkdir -p "$TEMP_DIR"

# Cleanup on exit
trap "rm -rf $TEMP_DIR" EXIT

echo -e "${BLUE}Neo Smart Contract Template Validation${NC}"
echo -e "${BLUE}======================================${NC}"
echo ""

# Function to validate a template
validate_template() {
    local template_name=$1
    local template_path=$2
    local test_name=$3
    local extra_params=$4
    
    echo -e "${YELLOW}Testing template: $template_name${NC}"
    
    # Create test project
    local test_dir="$TEMP_DIR/$test_name"
    mkdir -p "$test_dir"
    
    # Copy template files
    cp -r "$template_path"/* "$test_dir/" 2>/dev/null || true
    
    # Apply basic transformations
    find "$test_dir" -type f -name "*.cs" -o -name "*.csproj" -o -name "*.json" | while read -r file; do
        if [[ -f "$file" ]]; then
            # Replace template placeholders
            sed -i "s/MyContract/$test_name/g" "$file" 2>/dev/null || true
            sed -i "s/NeoContractSolution/$test_name/g" "$file" 2>/dev/null || true
        fi
    done
    
    # Rename directories and files
    find "$test_dir" -depth -name "*MyContract*" | while read -r path; do
        newpath="${path//MyContract/$test_name}"
        if [[ "$path" != "$newpath" ]]; then
            mv "$path" "$newpath" 2>/dev/null || true
        fi
    done
    
    # Try to build
    echo -n "  Building... "
    cd "$test_dir"
    if dotnet build --nologo --verbosity quiet > /dev/null 2>&1; then
        echo -e "${GREEN}✓${NC}"
    else
        echo -e "${RED}✗${NC}"
        echo -e "${RED}  Build failed for $template_name${NC}"
        return 1
    fi
    
    # Try to run tests if they exist
    if [[ -d "tests" ]]; then
        echo -n "  Running tests... "
        if dotnet test --no-build --nologo --verbosity quiet > /dev/null 2>&1; then
            echo -e "${GREEN}✓${NC}"
        else
            echo -e "${RED}✗${NC}"
            echo -e "${RED}  Tests failed for $template_name${NC}"
            return 1
        fi
    fi
    
    echo -e "${GREEN}  Template $template_name validated successfully!${NC}"
    return 0
}

# Function to check template structure
check_template_structure() {
    local template_path=$1
    local template_name=$(basename "$template_path")
    
    echo -e "${YELLOW}Checking structure: $template_name${NC}"
    
    # Check for required files
    local required_files=(
        ".template.config/template.json"
    )
    
    for file in "${required_files[@]}"; do
        if [[ ! -f "$template_path/$file" ]]; then
            echo -e "${RED}  Missing required file: $file${NC}"
            return 1
        fi
    done
    
    # Validate template.json
    local template_json="$template_path/.template.config/template.json"
    if ! jq empty "$template_json" 2>/dev/null; then
        echo -e "${RED}  Invalid JSON in template.json${NC}"
        return 1
    fi
    
    # Check required fields in template.json
    local required_fields=("identity" "name" "shortName")
    for field in "${required_fields[@]}"; do
        if ! jq -e ".$field" "$template_json" > /dev/null 2>&1; then
            echo -e "${RED}  Missing required field in template.json: $field${NC}"
            return 1
        fi
    done
    
    echo -e "${GREEN}  Structure OK${NC}"
    return 0
}

# Main validation
echo "1. Checking template structures..."
echo ""

TEMPLATES_DIR="$PROJECT_ROOT/src/Neo.SmartContract.Template/templates"
FAILED_CHECKS=0

for template_dir in "$TEMPLATES_DIR"/*; do
    if [[ -d "$template_dir" ]]; then
        if ! check_template_structure "$template_dir"; then
            ((FAILED_CHECKS++))
        fi
    fi
done

echo ""
echo "2. Validating template functionality..."
echo ""

# Test neocontractsolution template with different contract types
if [[ -d "$TEMPLATES_DIR/neocontractsolution" ]]; then
    # Basic contract
    if ! validate_template "neocontractsolution-basic" "$TEMPLATES_DIR/neocontractsolution" "TestBasic" "--contractType Basic"; then
        ((FAILED_CHECKS++))
    fi
    
    # NEP-17 Token
    if ! validate_template "neocontractsolution-token" "$TEMPLATES_DIR/neocontractsolution" "TestToken" "--contractType NEP17"; then
        ((FAILED_CHECKS++))
    fi
    
    # NEP-11 NFT
    if ! validate_template "neocontractsolution-nft" "$TEMPLATES_DIR/neocontractsolution" "TestNFT" "--contractType NEP11"; then
        ((FAILED_CHECKS++))
    fi
    
    # Governance
    if ! validate_template "neocontractsolution-governance" "$TEMPLATES_DIR/neocontractsolution" "TestGov" "--contractType Governance"; then
        ((FAILED_CHECKS++))
    fi
fi

# Test other templates
for template_dir in "$TEMPLATES_DIR"/*; do
    if [[ -d "$template_dir" ]] && [[ "$(basename "$template_dir")" != "neocontractsolution" ]]; then
        template_name=$(basename "$template_dir")
        if ! validate_template "$template_name" "$template_dir" "Test${template_name^}" ""; then
            ((FAILED_CHECKS++))
        fi
    fi
done

echo ""
echo "3. Security checks..."
echo ""

# Check for hardcoded private keys
echo -n "  Checking for hardcoded private keys... "
if grep -r "L[1-9A-HJ-NP-Za-km-z]\{51\}" "$TEMPLATES_DIR" --include="*.cs" --include="*.json" > /dev/null 2>&1; then
    echo -e "${RED}✗ Found potential private keys!${NC}"
    ((FAILED_CHECKS++))
else
    echo -e "${GREEN}✓${NC}"
fi

# Check for placeholder values that should be replaced
echo -n "  Checking for placeholder values... "
PLACEHOLDERS=("YOUR_WIF_KEY" "YOUR_CONTRACT_HASH" "YOUR_ADDRESS" "TODO" "FIXME")
FOUND_PLACEHOLDERS=0
for placeholder in "${PLACEHOLDERS[@]}"; do
    if grep -r "$placeholder" "$TEMPLATES_DIR" --include="*.cs" --include="*.json" > /dev/null 2>&1; then
        echo -e "${YELLOW}Warning: Found placeholder '$placeholder'${NC}"
        ((FOUND_PLACEHOLDERS++))
    fi
done
if [[ $FOUND_PLACEHOLDERS -eq 0 ]]; then
    echo -e "${GREEN}✓${NC}"
fi

echo ""
echo "4. Template packaging test..."
echo ""

# Try to pack the template project
echo -n "  Packing template project... "
cd "$PROJECT_ROOT/src/Neo.SmartContract.Template"
if dotnet pack --nologo --verbosity quiet > /dev/null 2>&1; then
    echo -e "${GREEN}✓${NC}"
    
    # Check if package was created
    if ls bin/Debug/*.nupkg > /dev/null 2>&1; then
        echo -e "${GREEN}  Package created successfully${NC}"
    else
        echo -e "${RED}  Package file not found${NC}"
        ((FAILED_CHECKS++))
    fi
else
    echo -e "${RED}✗ Packing failed${NC}"
    ((FAILED_CHECKS++))
fi

# Summary
echo ""
echo -e "${BLUE}Validation Summary${NC}"
echo -e "${BLUE}==================${NC}"

if [[ $FAILED_CHECKS -eq 0 ]]; then
    echo -e "${GREEN}All template validations passed!${NC}"
    exit 0
else
    echo -e "${RED}$FAILED_CHECKS validation(s) failed${NC}"
    exit 1
fi