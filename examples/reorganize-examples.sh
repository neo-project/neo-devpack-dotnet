#!/bin/bash

# Script to reorganize examples into categorized structure
# This script creates symbolic links to maintain backward compatibility

echo "🔄 Reorganizing NEO Smart Contract Examples..."

# Create symbolic links for beginner examples
echo "📁 Setting up Beginner examples..."
ln -sfn ../Example.SmartContract.HelloWorld 01-beginner/HelloWorld
ln -sfn ../Example.SmartContract.HelloWorld.UnitTests 01-beginner/HelloWorld.UnitTests
ln -sfn ../Example.SmartContract.Storage 01-beginner/Storage
ln -sfn ../Example.SmartContract.Storage.UnitTests 01-beginner/Storage.UnitTests
ln -sfn ../Example.SmartContract.Event 01-beginner/Events
ln -sfn ../Example.SmartContract.Event.UnitTests 01-beginner/Events.UnitTests
ln -sfn ../Example.SmartContract.Exception 01-beginner/Exception
ln -sfn ../Example.SmartContract.Exception.UnitTests 01-beginner/Exception.UnitTests

# Create symbolic links for intermediate examples
echo "📁 Setting up Intermediate examples..."
ln -sfn ../Example.SmartContract.Modifier 02-intermediate/Modifier
ln -sfn ../Example.SmartContract.Modifier.UnitTests 02-intermediate/Modifier.UnitTests
ln -sfn ../Example.SmartContract.ContractCall 02-intermediate/ContractCall
ln -sfn ../Example.SmartContract.ContractCall.UnitTests 02-intermediate/ContractCall.UnitTests
ln -sfn ../Example.SmartContract.Transfer 02-intermediate/Transfer
ln -sfn ../Example.SmartContract.Transfer.UnitTests 02-intermediate/Transfer.UnitTests

# Create symbolic links for advanced examples
echo "📁 Setting up Advanced examples..."
ln -sfn ../Example.SmartContract.Oracle 03-advanced/Oracle
ln -sfn ../Example.SmartContract.Oracle.UnitTests 03-advanced/Oracle.UnitTests
ln -sfn ../Example.SmartContract.ZKP 03-advanced/ZKP
ln -sfn ../Example.SmartContract.ZKP.UnitTests 03-advanced/ZKP.UnitTests
ln -sfn ../Example.SmartContract.Inscription 03-advanced/Inscription
ln -sfn ../Example.SmartContract.Inscription.UnitTests 03-advanced/Inscription.UnitTests

# Create symbolic links for token standards
echo "📁 Setting up Token Standards examples..."
ln -sfn ../Example.SmartContract.NEP17 04-token-standards/NEP17
ln -sfn ../Example.SmartContract.NEP17.UnitTests 04-token-standards/NEP17.UnitTests
ln -sfn ../Example.SmartContract.NFT 04-token-standards/NEP11
ln -sfn ../Example.SmartContract.NFT.UnitTests 04-token-standards/NEP11.UnitTests
ln -sfn ../Example.SmartContract.SampleRoyaltyNEP11Token 04-token-standards/RoyaltyNFT
ln -sfn ../Example.SmartContract.SampleRoyaltyNEP11Token.UnitTests 04-token-standards/RoyaltyNFT.UnitTests

echo "✅ Reorganization complete!"
echo ""
echo "📝 Note: Original examples remain in place with symbolic links in categorized folders."
echo "This maintains backward compatibility while providing better organization."
echo ""
echo "📚 Check out the new README.md for the improved learning path!"