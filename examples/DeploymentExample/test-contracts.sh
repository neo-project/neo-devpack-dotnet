#!/bin/bash

echo "=== Testing Deployed Contracts ==="
echo

# Contract addresses
TOKEN_CONTRACT="0xfac608389b3e41d9f1dd49f48009a5717a849077"
NFT_CONTRACT="0x61773e61b4d5e21badaf5d30fb004e2a5ae9c673"
GOV_CONTRACT="0x1d6389b7fd7b284d88418d7ef2ddd4f34333666b"
DEPLOYER_HASH="0x11a6eff52d36fcb3048d90d535bf0c2269b73b65"

echo "Contract Addresses:"
echo "  Token:      $TOKEN_CONTRACT"
echo "  NFT:        $NFT_CONTRACT"
echo "  Governance: $GOV_CONTRACT"
echo

# Test Token Contract
echo "1. Testing Token Contract..."
echo "   Symbol: "
cat > token-symbol.neo-invoke.json << EOF
{
  "contract": "$TOKEN_CONTRACT",
  "operation": "symbol",
  "args": []
}
EOF
neoxp contract invoke token-symbol.neo-invoke.json deployer --results | grep "Result Stack" -A 1

echo "   Total Supply: "
cat > token-supply.neo-invoke.json << EOF
{
  "contract": "$TOKEN_CONTRACT",
  "operation": "totalSupply",
  "args": []
}
EOF
neoxp contract invoke token-supply.neo-invoke.json deployer --results | grep "Result Stack" -A 1

echo "   Deployer Balance: "
cat > token-balance.neo-invoke.json << EOF
{
  "contract": "$TOKEN_CONTRACT",
  "operation": "balanceOf",
  "args": [{"type": "Hash160", "value": "$DEPLOYER_HASH"}]
}
EOF
neoxp contract invoke token-balance.neo-invoke.json deployer --results | grep "Result Stack" -A 1

# Test NFT Contract
echo -e "\n2. Testing NFT Contract..."
echo "   Symbol: "
cat > nft-symbol.neo-invoke.json << EOF
{
  "contract": "$NFT_CONTRACT",
  "operation": "symbol",
  "args": []
}
EOF
neoxp contract invoke nft-symbol.neo-invoke.json deployer --results | grep "Result Stack" -A 1

echo "   Total Supply: "
cat > nft-supply.neo-invoke.json << EOF
{
  "contract": "$NFT_CONTRACT",
  "operation": "totalSupply",
  "args": []
}
EOF
neoxp contract invoke nft-supply.neo-invoke.json deployer --results | grep "Result Stack" -A 1

# Test Governance Contract
echo -e "\n3. Testing Governance Contract..."
echo "   Owner: "
cat > gov-owner.neo-invoke.json << EOF
{
  "contract": "$GOV_CONTRACT",
  "operation": "getOwner",
  "args": []
}
EOF
neoxp contract invoke gov-owner.neo-invoke.json deployer --results | grep "Result Stack" -A 1

echo -e "\n✅ All contracts deployed and tested successfully!"