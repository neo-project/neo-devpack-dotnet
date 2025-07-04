using System.Numerics;
using Neo;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Exceptions;
using Xunit;

namespace TokenContract.Tests;

public class TokenContractTests : SmartContractTestBase<DeploymentExample.Contract.TokenContract>
{
    [Fact]
    public void TestSymbol()
    {
        Assert.Equal("EXT", Contract.Symbol());
    }

    [Fact]
    public void TestDecimals()
    {
        Assert.Equal(8, Contract.Decimals());
    }

    [Fact]
    public void TestTotalSupply()
    {
        var expected = new BigInteger(100_000_000_00000000);
        Assert.Equal(expected, Contract.TotalSupply());
    }

    [Fact]
    public void TestTransfer()
    {
        var alice = Engine.GetAccount("alice");
        var bob = Engine.GetAccount("bob");
        
        // Initialize contract with owner
        Engine.SetTransaction(alice);
        Contract.Initialize(alice.ScriptHash);
        
        // Transfer from alice to bob
        var amount = new BigInteger(1000_00000000); // 1000 tokens
        var result = Contract.Transfer(alice.ScriptHash, bob.ScriptHash, amount, null);
        Assert.True(result);
        
        // Check balances
        Assert.Equal(amount, Contract.BalanceOf(bob.ScriptHash));
        Assert.Equal(Contract.TotalSupply() - amount, Contract.BalanceOf(alice.ScriptHash));
    }

    [Fact]
    public void TestTransferInsufficientBalance()
    {
        var alice = Engine.GetAccount("alice");
        var bob = Engine.GetAccount("bob");
        
        // Initialize contract
        Engine.SetTransaction(alice);
        Contract.Initialize(alice.ScriptHash);
        
        // Try to transfer more than balance
        Engine.SetTransaction(bob);
        var amount = new BigInteger(1000_00000000);
        
        Assert.Throws<TestException>(() => 
            Contract.Transfer(bob.ScriptHash, alice.ScriptHash, amount, null));
    }

    [Fact]
    public void TestMint()
    {
        var alice = Engine.GetAccount("alice");
        var bob = Engine.GetAccount("bob");
        
        // Initialize contract with alice as owner
        Engine.SetTransaction(alice);
        Contract.Initialize(alice.ScriptHash);
        
        // Mint tokens to bob
        var mintAmount = new BigInteger(5000_00000000);
        var result = Contract.Mint(bob.ScriptHash, mintAmount);
        Assert.True(result);
        
        // Check new balance and total supply
        Assert.Equal(mintAmount, Contract.BalanceOf(bob.ScriptHash));
        Assert.Equal(new BigInteger(100_000_000_00000000) + mintAmount, Contract.TotalSupply());
    }

    [Fact]
    public void TestBurn()
    {
        var alice = Engine.GetAccount("alice");
        
        // Initialize contract
        Engine.SetTransaction(alice);
        Contract.Initialize(alice.ScriptHash);
        
        // Burn tokens
        var burnAmount = new BigInteger(1000_00000000);
        var initialSupply = Contract.TotalSupply();
        var initialBalance = Contract.BalanceOf(alice.ScriptHash);
        
        var result = Contract.Burn(alice.ScriptHash, burnAmount);
        Assert.True(result);
        
        // Check reduced balance and total supply
        Assert.Equal(initialBalance - burnAmount, Contract.BalanceOf(alice.ScriptHash));
        Assert.Equal(initialSupply - burnAmount, Contract.TotalSupply());
    }

    [Fact]
    public void TestPauseUnpause()
    {
        var alice = Engine.GetAccount("alice");
        var bob = Engine.GetAccount("bob");
        
        // Initialize contract
        Engine.SetTransaction(alice);
        Contract.Initialize(alice.ScriptHash);
        
        // Transfer should work when not paused
        var amount = new BigInteger(100_00000000);
        Assert.True(Contract.Transfer(alice.ScriptHash, bob.ScriptHash, amount, null));
        
        // Pause the contract
        Contract.Pause();
        
        // Transfer should fail when paused
        Assert.Throws<TestException>(() => 
            Contract.Transfer(alice.ScriptHash, bob.ScriptHash, amount, null));
        
        // Unpause the contract
        Contract.Unpause();
        
        // Transfer should work again
        Assert.True(Contract.Transfer(alice.ScriptHash, bob.ScriptHash, amount, null));
    }
}