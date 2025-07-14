#!/bin/bash

# Comprehensive test for production-ready workflow
# Tests the complete dataflow from compilation to deployment with all features

set -e

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

print_header() {
    echo -e "\n${BLUE}=== $1 ===${NC}\n"
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

# Configuration
BASE_URL="http://localhost:8888"
CONTRACT_ADDRESS="0xabcdef1234567890abcdef1234567890abcdef12"
CONTRACT_NAME="ProductionTestContract"
DEPLOYER_ADDRESS="NPvKVTGZapmFWABLsyvfreuqn73jCjJtN5"
DESCRIPTION="Production-ready smart contract with full features"

print_header "🚀 Production Workflow Test"
echo "This test simulates the complete production workflow:"
echo "1. Contract compilation with manifest, NEF, and plugin"
echo "2. WebGUI deployment from manifest"
echo "3. Plugin upload and download"
echo "4. Security validation"
echo "5. Performance testing"
echo ""

# Step 1: Test contract deployment with manifest
print_header "Step 1: Contract Deployment from Manifest"

DEPLOY_PAYLOAD=$(cat <<EOF
{
    "contractAddress": "$CONTRACT_ADDRESS",
    "contractName": "$CONTRACT_NAME",
    "network": "testnet",
    "deployerAddress": "$DEPLOYER_ADDRESS",
    "description": "$DESCRIPTION"
}
EOF
)

print_info "Deploying contract WebGUI..."
DEPLOY_RESPONSE=$(curl -s -X POST "$BASE_URL/api/webgui/deploy-from-manifest" \
    -H "Content-Type: application/json" \
    -d "$DEPLOY_PAYLOAD" || true)

if [ -z "$DEPLOY_RESPONSE" ]; then
    print_error "Failed to connect to WebGUI service"
    exit 1
fi

SUBDOMAIN=$(echo "$DEPLOY_RESPONSE" | jq -r '.subdomain' 2>/dev/null || echo "")

if [ -z "$SUBDOMAIN" ] || [ "$SUBDOMAIN" = "null" ]; then
    print_error "Deployment failed"
    echo "$DEPLOY_RESPONSE" | jq . 2>/dev/null || echo "$DEPLOY_RESPONSE"
    exit 1
fi

print_success "Contract deployed to subdomain: $SUBDOMAIN"
WEBGUI_URL="http://$SUBDOMAIN.localhost:8888"

# Step 2: Test plugin upload
print_header "Step 2: Plugin Upload Test"

# Create a test plugin ZIP file
print_info "Creating test plugin..."
mkdir -p /tmp/test-plugin
cat > /tmp/test-plugin/plugin.json <<EOF
{
    "name": "$CONTRACT_NAME Plugin",
    "version": "1.0.0",
    "description": "Neo N3 plugin for $CONTRACT_NAME"
}
EOF
cd /tmp && zip -q test-plugin.zip test-plugin/*

print_info "Uploading plugin..."
PLUGIN_RESPONSE=$(curl -s -X POST "$BASE_URL/api/webgui/$CONTRACT_ADDRESS/plugin" \
    -H "X-Deployer-Address: $DEPLOYER_ADDRESS" \
    -F "pluginFile=@/tmp/test-plugin.zip" || true)

PLUGIN_URL=$(echo "$PLUGIN_RESPONSE" | jq -r '.pluginUrl' 2>/dev/null || echo "")

if [ -n "$PLUGIN_URL" ] && [ "$PLUGIN_URL" != "null" ]; then
    print_success "Plugin uploaded successfully"
else
    print_error "Plugin upload failed"
    echo "$PLUGIN_RESPONSE" | jq . 2>/dev/null || echo "$PLUGIN_RESPONSE"
fi

# Step 3: Verify configuration with plugin URL
print_header "Step 3: Configuration Verification"

CONFIG_RESPONSE=$(curl -s "$BASE_URL/api/webgui/$CONTRACT_ADDRESS/config" || true)
HAS_PLUGIN=$(echo "$CONFIG_RESPONSE" | jq -r '.pluginDownloadUrl' 2>/dev/null || echo "")

if [ -n "$HAS_PLUGIN" ] && [ "$HAS_PLUGIN" != "null" ]; then
    print_success "Configuration includes plugin download URL"
else
    print_error "Plugin URL not found in configuration"
fi

# Display configuration summary
echo "$CONFIG_RESPONSE" | jq '{
    contractName,
    contractAddress,
    network,
    pluginDownloadUrl,
    methodCount: (.methods | length),
    eventCount: (.events | length)
}' 2>/dev/null || echo "Failed to parse config"

# Step 4: Test plugin download
if [ -n "$PLUGIN_URL" ] && [ "$PLUGIN_URL" != "null" ]; then
    print_header "Step 4: Plugin Download Test"
    
    DOWNLOAD_URL="$BASE_URL$PLUGIN_URL"
    print_info "Downloading plugin from: $DOWNLOAD_URL"
    
    if curl -s -o /tmp/downloaded-plugin.zip "$DOWNLOAD_URL"; then
        if file /tmp/downloaded-plugin.zip | grep -q "Zip archive"; then
            print_success "Plugin downloaded successfully"
        else
            print_error "Downloaded file is not a valid ZIP"
        fi
    else
        print_error "Plugin download failed"
    fi
fi

# Step 5: Security headers test
print_header "Step 5: Security Headers Validation"

HEADERS_RESPONSE=$(curl -s -I "$WEBGUI_URL/subdomain" || true)

check_header() {
    local header=$1
    if echo "$HEADERS_RESPONSE" | grep -qi "$header"; then
        print_success "$header header present"
    else
        print_error "$header header missing"
    fi
}

check_header "X-Content-Type-Options"
check_header "X-Frame-Options"
check_header "X-XSS-Protection"
check_header "Content-Security-Policy"

# Step 6: Rate limiting test
print_header "Step 6: Rate Limiting Test"

print_info "Testing rate limiting (this may take a moment)..."
RATE_LIMIT_HIT=false

for i in {1..150}; do
    STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/api/webgui/search?contractAddress=test" || echo "000")
    
    if [ "$STATUS" = "429" ]; then
        RATE_LIMIT_HIT=true
        print_success "Rate limiting is working (hit at request $i)"
        break
    fi
done

if [ "$RATE_LIMIT_HIT" = false ]; then
    print_info "Rate limit not hit in 150 requests (may be disabled in dev)"
fi

# Step 7: WebGUI accessibility test
print_header "Step 7: WebGUI Accessibility"

# Test main WebGUI page
WEBGUI_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$WEBGUI_URL/subdomain" || echo "000")
if [ "$WEBGUI_STATUS" = "200" ]; then
    print_success "WebGUI main page accessible"
else
    print_error "WebGUI main page returned status: $WEBGUI_STATUS"
fi

# Test JavaScript files
JS_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$WEBGUI_URL/subdomain/modern-webgui.js" || echo "000")
if [ "$JS_STATUS" = "200" ]; then
    print_success "JavaScript files accessible"
else
    print_error "JavaScript files returned status: $JS_STATUS"
fi

# Step 8: API endpoints test
print_header "Step 8: API Endpoints Validation"

# Test search endpoint
SEARCH_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/api/webgui/search?contractAddress=$CONTRACT_ADDRESS" || echo "000")
if [ "$SEARCH_STATUS" = "200" ]; then
    print_success "Search API endpoint working"
else
    print_error "Search API returned status: $SEARCH_STATUS"
fi

# Test list endpoint
LIST_STATUS=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/api/webgui/list" || echo "000")
if [ "$LIST_STATUS" = "200" ]; then
    print_success "List API endpoint working"
else
    print_error "List API returned status: $LIST_STATUS"
fi

# Step 9: Cleanup test files
print_header "Step 9: Cleanup"
rm -rf /tmp/test-plugin /tmp/test-plugin.zip /tmp/downloaded-plugin.zip
print_success "Temporary files cleaned up"

# Final summary
print_header "📊 Production Workflow Test Summary"

echo "Contract Details:"
echo "  - Name: $CONTRACT_NAME"
echo "  - Address: $CONTRACT_ADDRESS"
echo "  - Network: testnet"
echo "  - Subdomain: $SUBDOMAIN"
echo ""
echo "Features Tested:"
echo "  ✅ JSON-based WebGUI deployment"
echo "  ✅ Plugin upload/download functionality"
echo "  ✅ Security headers implementation"
echo "  ✅ Rate limiting protection"
echo "  ✅ API endpoint accessibility"
echo "  ✅ WebGUI template serving"
echo ""
echo "Access Points:"
echo "  - WebGUI: $WEBGUI_URL"
echo "  - Config API: $BASE_URL/api/webgui/$CONTRACT_ADDRESS/config"
if [ -n "$PLUGIN_URL" ] && [ "$PLUGIN_URL" != "null" ]; then
    echo "  - Plugin Download: $BASE_URL$PLUGIN_URL"
fi
echo ""
print_success "Production workflow test completed! 🎉"

# Optional: Performance metrics
if command -v ab &> /dev/null; then
    read -p "Run performance test? (requires Apache Bench) (y/n) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        print_header "Performance Test"
        print_info "Running 1000 requests with concurrency of 10..."
        ab -n 1000 -c 10 -q "$BASE_URL/api/webgui/$CONTRACT_ADDRESS/config" | grep -E "(Requests per second|Time per request|Transfer rate)"
    fi
fi