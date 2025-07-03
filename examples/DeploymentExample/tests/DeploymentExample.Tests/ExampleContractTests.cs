using System.Numerics;
using Neo;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Xunit;
using Xunit.Abstractions;

namespace DeploymentExample.Tests
{
    public class ExampleContractTests : TestBase<ExampleContract>
    {
        private readonly ITestOutputHelper _output;

        public ExampleContractTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestDeploy()
        {
            // Deploy with owner
            var owner = TestEngine.GetDefaultAccount("owner");
            var snapshot = TestEngine.Snapshot.Clone();
            var deployer = TestEngine.Deploy<ExampleContract>(snapshot, owner.ScriptHash);
            
            Assert.NotNull(deployer);
            
            // Check owner was set
            var currentOwner = Contract.GetOwner();
            Assert.Equal(owner.ScriptHash, currentOwner);
            
            // Check counter was initialized
            var counter = Contract.GetCounter();
            Assert.Equal(new BigInteger(0), counter);
        }

        [Fact]
        public void TestGetOwner()
        {
            var owner = Contract.GetOwner();
            Assert.NotNull(owner);
            Assert.NotEqual(UInt160.Zero, owner);
        }

        [Fact]
        public void TestSetOwner()
        {
            // Get current owner
            var currentOwner = Contract.GetOwner();
            var ownerAccount = TestEngine.GetAccount(currentOwner);
            
            // Create new owner
            var newOwner = TestEngine.GetDefaultAccount("newOwner");
            
            // Transfer ownership
            TestEngine.SetSigner(ownerAccount);
            var result = Contract.SetOwner(newOwner.ScriptHash);
            Assert.True(result);
            
            // Verify ownership changed
            var updatedOwner = Contract.GetOwner();
            Assert.Equal(newOwner.ScriptHash, updatedOwner);
        }

        [Fact]
        public void TestSetOwner_NotOwner_ShouldFail()
        {
            // Create non-owner account
            var nonOwner = TestEngine.GetDefaultAccount("nonOwner");
            var newOwner = TestEngine.GetDefaultAccount("newOwner");
            
            // Try to transfer ownership as non-owner
            TestEngine.SetSigner(nonOwner);
            
            var exception = Assert.Throws<Exception>(() => 
                Contract.SetOwner(newOwner.ScriptHash)
            );
            Assert.Contains("Only owner can transfer ownership", exception.Message);
        }

        [Fact]
        public void TestIncrement()
        {
            // Setup signer
            var user = TestEngine.GetDefaultAccount("user");
            TestEngine.SetSigner(user);
            
            // Initial counter should be 0
            var initial = Contract.GetCounter();
            Assert.Equal(new BigInteger(0), initial);
            
            // Increment
            var result = Contract.Increment();
            Assert.Equal(new BigInteger(1), result);
            
            // Verify counter was incremented
            var counter = Contract.GetCounter();
            Assert.Equal(new BigInteger(1), counter);
        }

        [Fact]
        public void TestIncrement_Multiple()
        {
            var user = TestEngine.GetDefaultAccount("user");
            TestEngine.SetSigner(user);
            
            // Increment multiple times
            for (int i = 1; i <= 5; i++)
            {
                var result = Contract.Increment();
                Assert.Equal(new BigInteger(i), result);
            }
            
            // Final counter should be 5
            var counter = Contract.GetCounter();
            Assert.Equal(new BigInteger(5), counter);
        }

        [Fact]
        public void TestGetCounter()
        {
            var counter = Contract.GetCounter();
            Assert.Equal(new BigInteger(0), counter);
            
            // Increment and check again
            var user = TestEngine.GetDefaultAccount("user");
            TestEngine.SetSigner(user);
            Contract.Increment();
            
            counter = Contract.GetCounter();
            Assert.Equal(new BigInteger(1), counter);
        }

        [Fact]
        public void TestMultiply()
        {
            var result = Contract.Multiply(new BigInteger(5), new BigInteger(7));
            Assert.Equal(new BigInteger(35), result);
            
            result = Contract.Multiply(new BigInteger(-3), new BigInteger(4));
            Assert.Equal(new BigInteger(-12), result);
            
            result = Contract.Multiply(new BigInteger(0), new BigInteger(100));
            Assert.Equal(new BigInteger(0), result);
        }

        [Fact]
        public void TestGetInfo()
        {
            var info = Contract.GetInfo();
            Assert.NotNull(info);
            
            Assert.Equal("DeploymentExample", info["name"]);
            Assert.Equal("1.0.0", info["version"]);
            Assert.NotNull(info["owner"]);
            Assert.Equal(new BigInteger(0), info["counter"]);
            
            // Increment counter and check info again
            var user = TestEngine.GetDefaultAccount("user");
            TestEngine.SetSigner(user);
            Contract.Increment();
            
            info = Contract.GetInfo();
            Assert.Equal(new BigInteger(1), info["counter"]);
        }

        [Fact]
        public void TestVerify()
        {
            // Verify should return true when signed by owner
            var owner = Contract.GetOwner();
            var ownerAccount = TestEngine.GetAccount(owner);
            TestEngine.SetSigner(ownerAccount);
            
            var result = Contract.Verify();
            Assert.True(result);
            
            // Verify should return false when signed by non-owner
            var nonOwner = TestEngine.GetDefaultAccount("nonOwner");
            TestEngine.SetSigner(nonOwner);
            
            result = Contract.Verify();
            Assert.False(result);
        }
    }
}