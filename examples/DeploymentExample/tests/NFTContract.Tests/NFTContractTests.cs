using System.Numerics;
using Neo;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Exceptions;
using Neo.VM.Types;
using Xunit;

namespace NFTContract.Tests;

public class NFTContractTests : TestBase<NFTContract>
{
    [Fact]
    public void TestSymbol()
    {
        Assert.Equal("EXNFT", Contract.Symbol());
    }

    [Fact]
    public void TestDecimals()
    {
        Assert.Equal(0, Contract.Decimals());
    }

    [Fact]
    public void TestInitialTotalSupply()
    {
        Assert.Equal(BigInteger.Zero, Contract.TotalSupply());
    }

    [Fact]
    public void TestMint()
    {
        var alice = Engine.GetAccount("alice");
        
        // Initialize contract
        Engine.SetTransaction(alice);
        Contract.Initialize(alice.ScriptHash, UInt160.Zero); // Mock token contract
        
        // Create properties for NFT
        var properties = new Map
        {
            ["name"] = "Test NFT #1",
            ["description"] = "A test NFT",
            ["image"] = "https://example.com/nft1.png"
        };
        
        // Mint NFT (assuming payment is handled)
        var tokenId = Contract.Mint("https://example.com/metadata/1.json", properties);
        
        Assert.NotNull(tokenId);
        Assert.Equal(BigInteger.One, Contract.TotalSupply());
        Assert.Equal(alice.ScriptHash, Contract.OwnerOf(tokenId));
    }

    [Fact]
    public void TestTransfer()
    {
        var alice = Engine.GetAccount("alice");
        var bob = Engine.GetAccount("bob");
        
        // Initialize and mint
        Engine.SetTransaction(alice);
        Contract.Initialize(alice.ScriptHash, UInt160.Zero);
        
        var properties = new Map { ["name"] = "Test NFT" };
        var tokenId = Contract.Mint("https://example.com/metadata/1.json", properties);
        
        // Transfer NFT
        var result = Contract.Transfer(bob.ScriptHash, tokenId, null);
        Assert.True(result);
        
        // Verify ownership changed
        Assert.Equal(bob.ScriptHash, Contract.OwnerOf(tokenId));
        Assert.Equal(BigInteger.Zero, Contract.BalanceOf(alice.ScriptHash));
        Assert.Equal(BigInteger.One, Contract.BalanceOf(bob.ScriptHash));
    }

    [Fact]
    public void TestTokensOf()
    {
        var alice = Engine.GetAccount("alice");
        
        // Initialize contract
        Engine.SetTransaction(alice);
        Contract.Initialize(alice.ScriptHash, UInt160.Zero);
        
        // Mint multiple NFTs
        var properties1 = new Map { ["name"] = "NFT #1" };
        var tokenId1 = Contract.Mint("https://example.com/1.json", properties1);
        
        var properties2 = new Map { ["name"] = "NFT #2" };
        var tokenId2 = Contract.Mint("https://example.com/2.json", properties2);
        
        // Get tokens owned by alice
        var tokens = Contract.TokensOf(alice.ScriptHash);
        var tokenIterator = tokens as InteropInterface;
        
        Assert.NotNull(tokenIterator);
        Assert.Equal(BigInteger.One + BigInteger.One, Contract.BalanceOf(alice.ScriptHash));
    }

    [Fact]
    public void TestProperties()
    {
        var alice = Engine.GetAccount("alice");
        
        // Initialize and mint
        Engine.SetTransaction(alice);
        Contract.Initialize(alice.ScriptHash, UInt160.Zero);
        
        var expectedProperties = new Map
        {
            ["name"] = "Special NFT",
            ["rarity"] = "legendary",
            ["power"] = 9000
        };
        
        var tokenId = Contract.Mint("https://example.com/special.json", expectedProperties);
        
        // Get properties
        var properties = Contract.Properties(tokenId) as Map;
        Assert.NotNull(properties);
        Assert.Equal("Special NFT", properties["name"].GetString());
        Assert.Equal("legendary", properties["rarity"].GetString());
        Assert.Equal(9000, properties["power"].GetInteger());
    }

    [Fact]
    public void TestInvalidTransfer()
    {
        var alice = Engine.GetAccount("alice");
        var bob = Engine.GetAccount("bob");
        var charlie = Engine.GetAccount("charlie");
        
        // Initialize and mint
        Engine.SetTransaction(alice);
        Contract.Initialize(alice.ScriptHash, UInt160.Zero);
        
        var properties = new Map { ["name"] = "Test NFT" };
        var tokenId = Contract.Mint("https://example.com/test.json", properties);
        
        // Transfer to bob
        Contract.Transfer(bob.ScriptHash, tokenId, null);
        
        // Try to transfer from alice (no longer owner) - should fail
        Assert.Throws<TestException>(() => 
            Contract.Transfer(charlie.ScriptHash, tokenId, null));
    }
}