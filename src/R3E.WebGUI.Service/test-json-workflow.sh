#!/bin/bash

# Test script for JSON-based WebGUI workflow
# This script tests the complete workflow of deploying and accessing a JSON-based WebGUI

set -e

echo "🧪 Testing R3E WebGUI Service JSON-based Workflow"
echo "================================================"

# Configuration
BASE_URL="http://localhost:8888"
CONTRACT_ADDRESS="0x1234567890abcdef1234567890abcdef12345678"
CONTRACT_NAME="TestContract"
DEPLOYER_ADDRESS="NPvKVTGZapmFWABLsyvfreuqn73jCjJtN5"

echo ""
echo "1️⃣ Testing deployment from manifest..."
echo "--------------------------------------"

DEPLOY_RESPONSE=$(curl -s -X POST "$BASE_URL/api/webgui/deploy-from-manifest" \
  -H "Content-Type: application/json" \
  -d "{
    \"contractAddress\": \"$CONTRACT_ADDRESS\",
    \"contractName\": \"$CONTRACT_NAME\",
    \"network\": \"testnet\",
    \"deployerAddress\": \"$DEPLOYER_ADDRESS\",
    \"description\": \"Test contract for JSON workflow\"
  }")

echo "Deploy response:"
echo "$DEPLOY_RESPONSE" | jq .

# Extract subdomain from response
SUBDOMAIN=$(echo "$DEPLOY_RESPONSE" | jq -r '.subdomain')

if [ "$SUBDOMAIN" == "null" ] || [ -z "$SUBDOMAIN" ]; then
  echo "❌ Deployment failed: Could not get subdomain"
  exit 1
fi

echo "✅ Contract deployed to subdomain: $SUBDOMAIN"

echo ""
echo "2️⃣ Testing contract configuration retrieval..."
echo "---------------------------------------------"

CONFIG_RESPONSE=$(curl -s "$BASE_URL/api/webgui/$CONTRACT_ADDRESS/config")

echo "Configuration response:"
echo "$CONFIG_RESPONSE" | jq .

if [ "$(echo "$CONFIG_RESPONSE" | jq -r '.contractAddress')" != "$CONTRACT_ADDRESS" ]; then
  echo "❌ Configuration retrieval failed"
  exit 1
fi

echo "✅ Configuration retrieved successfully"

echo ""
echo "3️⃣ Testing subdomain access..."
echo "-------------------------------"

SUBDOMAIN_URL="http://$SUBDOMAIN.localhost:8888/subdomain"
SUBDOMAIN_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" "$SUBDOMAIN_URL")

if [ "$SUBDOMAIN_RESPONSE" -eq 200 ]; then
  echo "✅ Subdomain accessible at: $SUBDOMAIN_URL"
else
  echo "❌ Subdomain access failed with status: $SUBDOMAIN_RESPONSE"
fi

echo ""
echo "4️⃣ Testing JavaScript file access..."
echo "------------------------------------"

JS_URL="http://$SUBDOMAIN.localhost:8888/subdomain/modern-webgui.js"
JS_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" "$JS_URL")

if [ "$JS_RESPONSE" -eq 200 ]; then
  echo "✅ JavaScript files accessible"
else
  echo "❌ JavaScript file access failed with status: $JS_RESPONSE"
fi

echo ""
echo "5️⃣ Testing search functionality..."
echo "-----------------------------------"

SEARCH_RESPONSE=$(curl -s "$BASE_URL/api/webgui/search?contractAddress=$CONTRACT_ADDRESS")

echo "Search response:"
echo "$SEARCH_RESPONSE" | jq .

if [ "$(echo "$SEARCH_RESPONSE" | jq '. | length')" -gt 0 ]; then
  echo "✅ Contract found in search results"
else
  echo "❌ Contract not found in search"
fi

echo ""
echo "6️⃣ Testing home page display..."
echo "---------------------------------"

HOME_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/")

if [ "$HOME_RESPONSE" -eq 200 ]; then
  echo "✅ Home page accessible"
  
  # Check if contract appears on home page
  HOME_CONTENT=$(curl -s "$BASE_URL/")
  if echo "$HOME_CONTENT" | grep -q "$CONTRACT_NAME"; then
    echo "✅ Contract appears on home page"
  else
    echo "⚠️  Contract may not appear on home page (might need to check recent contracts)"
  fi
else
  echo "❌ Home page access failed with status: $HOME_RESPONSE"
fi

echo ""
echo "🎉 JSON-based WebGUI Workflow Test Complete!"
echo "==========================================="
echo ""
echo "Summary:"
echo "- Contract Address: $CONTRACT_ADDRESS"
echo "- Subdomain: $SUBDOMAIN"
echo "- WebGUI URL: http://$SUBDOMAIN.localhost:8888"
echo "- Config API: $BASE_URL/api/webgui/$CONTRACT_ADDRESS/config"
echo ""
echo "To view the WebGUI, open: http://$SUBDOMAIN.localhost:8888/subdomain"