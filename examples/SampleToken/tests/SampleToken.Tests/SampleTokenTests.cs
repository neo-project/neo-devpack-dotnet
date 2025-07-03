using Neo;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Xunit;

namespace SampleToken.Tests
{
    public class SampleTokenTests : TestBase<Contract.SampleToken>
    {
        [Fact]
        public void Test_Deploy()
        {
            // Deploy should initialize the token with the correct settings
            Assert.Equal("Sample Token", Contract.GetName());
            Assert.Equal("SAMPLE", Contract.GetSymbol());
            Assert.Equal(8, Contract.GetDecimals());
            Assert.Equal(100_000_000_00000000, Contract.TotalSupply());
        }

        [Fact]
        public void Test_InitialBalance()
        {
            // The deployer should have all initial tokens
            var owner = Contract.GetOwner();
            var balance = Contract.BalanceOf(owner);
            Assert.Equal(100_000_000_00000000, balance);
        }

        [Fact]
        public void Test_Transfer()
        {
            var owner = Contract.GetOwner();
            var alice = TestEngine.GetNewSigner();
            var amount = 1000_00000000; // 1000 tokens

            // Transfer from owner to alice
            TestEngine.SetSigner(owner);
            var result = Contract.Transfer(owner, alice.Account, amount, null);
            Assert.True(result);

            // Check balances
            var ownerBalance = Contract.BalanceOf(owner);
            var aliceBalance = Contract.BalanceOf(alice.Account);
            
            Assert.Equal(99_999_000_00000000, ownerBalance);
            Assert.Equal(1000_00000000, aliceBalance);
        }

        [Fact]
        public void Test_TransferInsufficientBalance()
        {
            var owner = Contract.GetOwner();
            var alice = TestEngine.GetNewSigner();
            var amount = 200_000_000_00000000; // More than total supply

            TestEngine.SetSigner(owner);
            var result = Contract.Transfer(owner, alice.Account, amount, null);
            Assert.False(result);
        }

        [Fact]
        public void Test_Mint()
        {
            var owner = Contract.GetOwner();
            var alice = TestEngine.GetNewSigner();
            var mintAmount = 50_000_000_00000000; // 50 million tokens

            TestEngine.SetSigner(owner);
            var result = Contract.Mint(alice.Account, mintAmount);
            Assert.True(result);

            // Check new total supply and balance
            Assert.Equal(150_000_000_00000000, Contract.TotalSupply());
            Assert.Equal(50_000_000_00000000, Contract.BalanceOf(alice.Account));
        }

        [Fact]
        public void Test_MintExceedsMaxSupply()
        {
            var owner = Contract.GetOwner();
            var alice = TestEngine.GetNewSigner();
            var mintAmount = 950_000_000_00000000; // Would exceed max supply

            TestEngine.SetSigner(owner);
            Assert.Throws<Exception>(() => Contract.Mint(alice.Account, mintAmount));
        }

        [Fact]
        public void Test_SetMinter()
        {
            var owner = Contract.GetOwner();
            var minter = TestEngine.GetNewSigner();

            // Set minter
            TestEngine.SetSigner(owner);
            var result = Contract.SetMinter(minter.Account, true);
            Assert.True(result);
            Assert.True(Contract.IsMinter(minter.Account));

            // Minter can mint
            TestEngine.SetSigner(minter);
            result = Contract.Mint(minter.Account, 1000_00000000);
            Assert.True(result);

            // Remove minter
            TestEngine.SetSigner(owner);
            result = Contract.SetMinter(minter.Account, false);
            Assert.True(result);
            Assert.False(Contract.IsMinter(minter.Account));
        }

        [Fact]
        public void Test_TransferOwnership()
        {
            var owner = Contract.GetOwner();
            var newOwner = TestEngine.GetNewSigner();

            TestEngine.SetSigner(owner);
            var result = Contract.TransferOwnership(newOwner.Account);
            Assert.True(result);

            Assert.Equal(newOwner.Account, Contract.GetOwner());
        }

        [Fact]
        public void Test_GetRemainingSupply()
        {
            var remaining = Contract.GetRemainingSupply();
            Assert.Equal(900_000_000_00000000, remaining); // 1 billion - 100 million
        }
    }
}