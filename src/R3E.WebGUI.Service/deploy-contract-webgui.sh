#!/bin/bash

# Comprehensive deployment script for Neo contracts with WebGUI and plugin
# This script handles the complete workflow from compilation to deployment

set -e

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
WEBGUI_SERVICE_URL="${WEBGUI_SERVICE_URL:-http://localhost:8888}"
NETWORK="${NETWORK:-testnet}"
NEO_COMPILER="${NEO_COMPILER:-neo-express}"

# Functions
print_header() {
    echo -e "${BLUE}===================================================${NC}"
    echo -e "${BLUE}$1${NC}"
    echo -e "${BLUE}===================================================${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_info() {
    echo -e "${YELLOW}ℹ️  $1${NC}"
}

usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  -p, --project PATH          Path to .csproj file (required)"
    echo "  -a, --address ADDRESS       Contract address (required for deployment)"
    echo "  -n, --name NAME            Contract name (optional, uses project name if not provided)"
    echo "  -d, --deployer ADDRESS     Deployer Neo address (required)"
    echo "  -w, --network NETWORK      Network: testnet or mainnet (default: testnet)"
    echo "  -s, --service URL          WebGUI service URL (default: http://localhost:8888)"
    echo "  -g, --generate-plugin      Generate Neo plugin"
    echo "  -u, --upload-plugin PATH   Upload plugin ZIP file"
    echo "  -e, --description TEXT     Contract description"
    echo "  -h, --help                 Show this help message"
    echo ""
    echo "Example:"
    echo "  $0 -p MyContract.csproj -a 0x123... -d NPvK... -g"
    exit 1
}

# Parse command line arguments
PROJECT_PATH=""
CONTRACT_ADDRESS=""
CONTRACT_NAME=""
DEPLOYER_ADDRESS=""
DESCRIPTION=""
GENERATE_PLUGIN=false
PLUGIN_PATH=""

while [[ $# -gt 0 ]]; do
    case $1 in
        -p|--project)
            PROJECT_PATH="$2"
            shift 2
            ;;
        -a|--address)
            CONTRACT_ADDRESS="$2"
            shift 2
            ;;
        -n|--name)
            CONTRACT_NAME="$2"
            shift 2
            ;;
        -d|--deployer)
            DEPLOYER_ADDRESS="$2"
            shift 2
            ;;
        -w|--network)
            NETWORK="$2"
            shift 2
            ;;
        -s|--service)
            WEBGUI_SERVICE_URL="$2"
            shift 2
            ;;
        -g|--generate-plugin)
            GENERATE_PLUGIN=true
            shift
            ;;
        -u|--upload-plugin)
            PLUGIN_PATH="$2"
            shift 2
            ;;
        -e|--description)
            DESCRIPTION="$2"
            shift 2
            ;;
        -h|--help)
            usage
            ;;
        *)
            echo "Unknown option: $1"
            usage
            ;;
    esac
done

# Validate required arguments
if [ -z "$PROJECT_PATH" ] || [ -z "$CONTRACT_ADDRESS" ] || [ -z "$DEPLOYER_ADDRESS" ]; then
    print_error "Missing required arguments"
    usage
fi

# Extract contract name from project if not provided
if [ -z "$CONTRACT_NAME" ]; then
    CONTRACT_NAME=$(basename "$PROJECT_PATH" .csproj)
fi

# Main workflow
print_header "Neo Contract WebGUI Deployment"
echo "Contract: $CONTRACT_NAME"
echo "Address: $CONTRACT_ADDRESS"
echo "Network: $NETWORK"
echo "Deployer: $DEPLOYER_ADDRESS"
echo ""

# Step 1: Compile the contract
print_header "Step 1: Compiling Contract"

COMPILE_ARGS="$PROJECT_PATH --output ./output"

if [ "$GENERATE_PLUGIN" = true ]; then
    COMPILE_ARGS="$COMPILE_ARGS --generate-plugin"
    print_info "Plugin generation enabled"
fi

print_info "Running: neo-express compile $COMPILE_ARGS"

if neo-express compile $COMPILE_ARGS; then
    print_success "Contract compiled successfully"
    
    # List generated files
    echo "Generated files:"
    ls -la ./output/
else
    print_error "Contract compilation failed"
    exit 1
fi

# Step 2: Deploy WebGUI from manifest
print_header "Step 2: Deploying WebGUI"

# Generate timestamp
TIMESTAMP=$(date +%s)

# Create message to sign
MESSAGE="Deploy WebGUI for contract $CONTRACT_ADDRESS by $DEPLOYER_ADDRESS at $TIMESTAMP"

print_info "Message to sign: $MESSAGE"
print_info "Please sign this message with your Neo wallet and provide:"
echo "  1. Signature (hex format)"
echo "  2. Public Key (hex format)"
echo ""

# In production, this would be done by the wallet
read -p "Enter signature: " SIGNATURE
read -p "Enter public key: " PUBLIC_KEY

DEPLOY_PAYLOAD=$(cat <<EOF
{
    "contractAddress": "$CONTRACT_ADDRESS",
    "contractName": "$CONTRACT_NAME",
    "network": "$NETWORK",
    "deployerAddress": "$DEPLOYER_ADDRESS",
    "description": "${DESCRIPTION:-Smart contract interface for $CONTRACT_NAME}",
    "timestamp": $TIMESTAMP,
    "signature": "$SIGNATURE",
    "publicKey": "$PUBLIC_KEY"
}
EOF
)

print_info "Deploying WebGUI to service..."

DEPLOY_RESPONSE=$(curl -s -X POST "$WEBGUI_SERVICE_URL/api/webgui/deploy-from-manifest" \
    -H "Content-Type: application/json" \
    -d "$DEPLOY_PAYLOAD")

if [ $? -eq 0 ]; then
    # Extract subdomain from response
    SUBDOMAIN=$(echo "$DEPLOY_RESPONSE" | jq -r '.subdomain')
    
    if [ "$SUBDOMAIN" != "null" ] && [ ! -z "$SUBDOMAIN" ]; then
        print_success "WebGUI deployed successfully"
        echo "Subdomain: $SUBDOMAIN"
        echo "URL: http://$SUBDOMAIN.localhost:8888"
    else
        print_error "WebGUI deployment failed"
        echo "$DEPLOY_RESPONSE" | jq .
        exit 1
    fi
else
    print_error "Failed to connect to WebGUI service"
    exit 1
fi

# Step 3: Upload plugin if available
if [ "$GENERATE_PLUGIN" = true ] || [ ! -z "$PLUGIN_PATH" ]; then
    print_header "Step 3: Uploading Plugin"
    
    # Find plugin ZIP file
    if [ -z "$PLUGIN_PATH" ]; then
        PLUGIN_PATH=$(find ./output -name "*Plugin.zip" -type f | head -1)
    fi
    
    if [ -f "$PLUGIN_PATH" ]; then
        print_info "Uploading plugin: $PLUGIN_PATH"
        
        # Calculate plugin hash
        PLUGIN_HASH=$(sha256sum "$PLUGIN_PATH" | awk '{print $1}')
        PLUGIN_TIMESTAMP=$(date +%s)
        
        # Create message for plugin upload
        PLUGIN_MESSAGE="Upload plugin for contract $CONTRACT_ADDRESS with hash $PLUGIN_HASH at $PLUGIN_TIMESTAMP"
        
        print_info "Plugin upload message to sign: $PLUGIN_MESSAGE"
        read -p "Enter signature for plugin upload: " PLUGIN_SIGNATURE
        read -p "Enter public key (or press Enter to use same): " PLUGIN_PUBLIC_KEY
        
        if [ -z "$PLUGIN_PUBLIC_KEY" ]; then
            PLUGIN_PUBLIC_KEY=$PUBLIC_KEY
        fi
        
        UPLOAD_RESPONSE=$(curl -s -X POST "$WEBGUI_SERVICE_URL/api/webgui/$CONTRACT_ADDRESS/plugin" \
            -H "X-Timestamp: $PLUGIN_TIMESTAMP" \
            -H "X-Signature: $PLUGIN_SIGNATURE" \
            -H "X-Public-Key: $PLUGIN_PUBLIC_KEY" \
            -F "pluginFile=@$PLUGIN_PATH")
        
        if [ $? -eq 0 ]; then
            PLUGIN_URL=$(echo "$UPLOAD_RESPONSE" | jq -r '.pluginUrl')
            
            if [ "$PLUGIN_URL" != "null" ]; then
                print_success "Plugin uploaded successfully"
                echo "Download URL: $WEBGUI_SERVICE_URL$PLUGIN_URL"
            else
                print_error "Plugin upload failed"
                echo "$UPLOAD_RESPONSE" | jq .
            fi
        else
            print_error "Failed to upload plugin"
        fi
    else
        print_info "No plugin file found to upload"
    fi
fi

# Step 4: Verify deployment
print_header "Step 4: Verifying Deployment"

# Check if config is accessible
CONFIG_RESPONSE=$(curl -s "$WEBGUI_SERVICE_URL/api/webgui/$CONTRACT_ADDRESS/config")

if [ $? -eq 0 ]; then
    CONFIG_ADDRESS=$(echo "$CONFIG_RESPONSE" | jq -r '.contractAddress')
    
    if [ "$CONFIG_ADDRESS" = "$CONTRACT_ADDRESS" ]; then
        print_success "Contract configuration verified"
        
        # Display summary
        echo ""
        print_header "Deployment Summary"
        echo "Contract Name: $CONTRACT_NAME"
        echo "Contract Address: $CONTRACT_ADDRESS"
        echo "Network: $NETWORK"
        echo "WebGUI URL: http://$SUBDOMAIN.localhost:8888"
        echo "Config API: $WEBGUI_SERVICE_URL/api/webgui/$CONTRACT_ADDRESS/config"
        
        if [ ! -z "$PLUGIN_URL" ]; then
            echo "Plugin Download: $WEBGUI_SERVICE_URL$PLUGIN_URL"
        fi
        
        echo ""
        print_success "Deployment completed successfully! 🎉"
    else
        print_error "Configuration verification failed"
    fi
else
    print_error "Failed to verify deployment"
fi

# Optional: Open WebGUI in browser
if command -v xdg-open &> /dev/null; then
    read -p "Open WebGUI in browser? (y/n) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        xdg-open "http://$SUBDOMAIN.localhost:8888"
    fi
fi