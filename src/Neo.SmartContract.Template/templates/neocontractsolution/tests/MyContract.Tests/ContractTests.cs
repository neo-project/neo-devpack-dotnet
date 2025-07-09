using System;
using System.Numerics;
using Neo;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Xunit;
using Xunit.Abstractions;

namespace MyContract.Tests
{
    #if (contractType == 'Basic')
    public class BasicContractTests : TestBase<BasicContract>
    {
        private readonly ITestOutputHelper _output;

        public BasicContractTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test_Deploy()
        {
            // Deploy should initialize the contract
            Assert.NotNull(Contract);
            
            // Check initialized state
            var storage = Engine.Storage.Find(Contract.Hash, new byte[] { });
            Assert.NotEmpty(storage);
        }

        [Fact]
        public void Test_StoreAndGetData()
        {
            // Arrange
            const string key = "testKey";
            const string value = "testValue";

            // Act
            Contract.StoreData(key, value);
            var retrieved = Contract.GetData(key);

            // Assert
            Assert.Equal(value, retrieved);
            
            // Verify event was emitted
            Assert.Single(Engine.Notifications);
            var notification = Engine.Notifications[0];
            Assert.Equal("DataStored", notification.EventName);
        }

        [Fact]
        public void Test_StoreData_EmptyKey_ThrowsException()
        {
            // Arrange
            const string key = "";
            const string value = "testValue";

            // Act & Assert
            var exception = Assert.Throws<TestException>(() => Contract.StoreData(key, value));
            Assert.Contains("Key cannot be empty", exception.Message);
        }

        #if (enableSecurityFeatures)
        [Fact]
        public void Test_Update_RequiresOwner()
        {
            // Arrange
            var unauthorizedAccount = Engine.GetAccount("NUnauthorizedAccount");
            var nefFile = new byte[] { 1, 2, 3 };
            var manifest = "{}";

            // Act & Assert
            Engine.SetSigner(unauthorizedAccount);
            var exception = Assert.Throws<TestException>(() => 
                Contract.Update(nefFile, manifest));
            Assert.Contains("Only owner can perform this action", exception.Message);
        }

        [Fact]
        public void Test_TransferOwnership()
        {
            // Arrange
            var owner = Engine.GetDefaultAccount();
            var newOwner = Engine.GetAccount("NNewOwnerAccount");

            // Act
            Engine.SetSigner(owner);
            Contract.TransferOwnership(newOwner.ScriptHash);

            // Assert
            Assert.Single(Engine.Notifications);
            var notification = Engine.Notifications[0];
            Assert.Equal("OwnershipTransferred", notification.EventName);
        }
        #endif
    }
    #endif

    #if (contractType == 'NEP17')
    public class TokenContractTests : TestBase<TokenContract>
    {
        private readonly ITestOutputHelper _output;

        public TokenContractTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test_TokenMetadata()
        {
            Assert.Equal("MYT", Contract.Symbol());
            Assert.Equal(8, Contract.Decimals());
        }

        [Fact]
        public void Test_InitialSupply()
        {
            // Initial supply should be 10,000,000 tokens
            var expectedSupply = new BigInteger(10_000_000_00000000);
            Assert.Equal(expectedSupply, Contract.TotalSupply());

            // Deployer should have all tokens
            var deployerBalance = Contract.BalanceOf(Engine.Native.ContractManagement.Hash);
            Assert.Equal(expectedSupply, deployerBalance);
        }

        [Fact]
        public void Test_Transfer_Success()
        {
            // Arrange
            var from = Engine.GetDefaultAccount();
            var to = Engine.GetAccount("NRecipientAccount");
            var amount = new BigInteger(100_00000000); // 100 tokens

            // Give sender some tokens
            Engine.SetSigner(Engine.Native.ContractManagement.Hash);
            Contract.Transfer(Engine.Native.ContractManagement.Hash, from.ScriptHash, amount * 2, null);
            Engine.ClearNotifications();

            // Act
            Engine.SetSigner(from);
            var result = Contract.Transfer(from.ScriptHash, to.ScriptHash, amount, null);

            // Assert
            Assert.True(result);
            Assert.Equal(amount, Contract.BalanceOf(to.ScriptHash));
            Assert.Equal(amount, Contract.BalanceOf(from.ScriptHash));

            // Check Transfer event
            Assert.Single(Engine.Notifications);
            var notification = Engine.Notifications[0];
            Assert.Equal("Transfer", notification.EventName);
        }

        [Fact]
        public void Test_Transfer_InsufficientBalance()
        {
            // Arrange
            var from = Engine.GetAccount("NPoorAccount");
            var to = Engine.GetAccount("NRecipientAccount");
            var amount = new BigInteger(100_00000000);

            // Act
            Engine.SetSigner(from);
            var result = Contract.Transfer(from.ScriptHash, to.ScriptHash, amount, null);

            // Assert
            Assert.False(result);
            Assert.Equal(0, Contract.BalanceOf(to.ScriptHash));
        }

        [Fact]
        public void Test_Transfer_InvalidAmount()
        {
            // Arrange
            var from = Engine.GetDefaultAccount();
            var to = Engine.GetAccount("NRecipientAccount");
            var amount = new BigInteger(-100);

            // Act & Assert
            Engine.SetSigner(from);
            var exception = Assert.Throws<TestException>(() => 
                Contract.Transfer(from.ScriptHash, to.ScriptHash, amount, null));
            Assert.Contains("Amount must be non-negative", exception.Message);
        }

        #if (enableSecurityFeatures)
        [Fact]
        public void Test_Mint_RequiresOwner()
        {
            // Arrange
            var unauthorizedAccount = Engine.GetAccount("NUnauthorizedAccount");
            var to = Engine.GetAccount("NRecipientAccount");
            var amount = new BigInteger(1000_00000000);

            // Act & Assert
            Engine.SetSigner(unauthorizedAccount);
            var exception = Assert.Throws<TestException>(() => 
                Contract.Mint(to.ScriptHash, amount));
            Assert.Contains("Only owner can perform this action", exception.Message);
        }

        [Fact]
        public void Test_Pause_Unpause()
        {
            // Arrange
            var owner = Engine.GetDefaultAccount();
            var from = owner;
            var to = Engine.GetAccount("NRecipientAccount");
            var amount = new BigInteger(100_00000000);

            // Give sender some tokens
            Engine.SetSigner(Engine.Native.ContractManagement.Hash);
            Contract.Transfer(Engine.Native.ContractManagement.Hash, from.ScriptHash, amount * 2, null);

            // Act - Pause
            Engine.SetSigner(owner);
            Contract.Pause();

            // Assert - Transfer should fail when paused
            var exception = Assert.Throws<TestException>(() => 
                Contract.Transfer(from.ScriptHash, to.ScriptHash, amount, null));
            Assert.Contains("Contract is paused", exception.Message);

            // Act - Unpause
            Contract.Unpause();

            // Assert - Transfer should work after unpause
            var result = Contract.Transfer(from.ScriptHash, to.ScriptHash, amount, null);
            Assert.True(result);
        }
        #endif
    }
    #endif

    #if (contractType == 'NEP11')
    public class NFTContractTests : TestBase<NFTContract>
    {
        private readonly ITestOutputHelper _output;

        public NFTContractTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test_NFTMetadata()
        {
            Assert.Equal("MYNFT", Contract.Symbol());
            Assert.Equal(0, Contract.Decimals());
        }

        [Fact]
        public void Test_Mint_NFT()
        {
            // Arrange
            var owner = Engine.GetDefaultAccount();
            var to = Engine.GetAccount("NRecipientAccount");
            var properties = new Neo.VM.Types.Map
            {
                ["name"] = "Test NFT #1",
                ["description"] = "This is a test NFT",
                ["image"] = "https://example.com/nft1.png",
                ["attributes"] = new Neo.VM.Types.Array
                {
                    new Neo.VM.Types.Map
                    {
                        ["trait_type"] = "Rarity",
                        ["value"] = "Common"
                    }
                }
            };

            // Act
            #if (enableSecurityFeatures)
            Engine.SetSigner(owner);
            Contract.SetMinter(owner.ScriptHash, true);
            #endif
            
            var tokenId = Contract.Mint(to.ScriptHash, properties);

            // Assert
            Assert.NotNull(tokenId);
            Assert.Equal(1, Contract.TotalSupply());
            Assert.Equal(1, Contract.BalanceOf(to.ScriptHash));
            Assert.Equal(to.ScriptHash, Contract.OwnerOf(tokenId));

            // Check properties
            var storedProps = Contract.Properties(tokenId);
            Assert.Equal("Test NFT #1", storedProps["name"]);
        }

        [Fact]
        public void Test_Transfer_NFT()
        {
            // Arrange
            var owner = Engine.GetDefaultAccount();
            var from = Engine.GetAccount("NFromAccount");
            var to = Engine.GetAccount("NToAccount");

            // Mint NFT to 'from' account
            #if (enableSecurityFeatures)
            Engine.SetSigner(owner);
            Contract.SetMinter(owner.ScriptHash, true);
            #endif
            
            var properties = new Neo.VM.Types.Map { ["name"] = "Test NFT" };
            var tokenId = Contract.Mint(from.ScriptHash, properties);
            Engine.ClearNotifications();

            // Act
            Engine.SetSigner(from);
            var result = Contract.Transfer(to.ScriptHash, tokenId, null);

            // Assert
            Assert.True(result);
            Assert.Equal(to.ScriptHash, Contract.OwnerOf(tokenId));
            Assert.Equal(0, Contract.BalanceOf(from.ScriptHash));
            Assert.Equal(1, Contract.BalanceOf(to.ScriptHash));

            // Check Transfer event
            Assert.Single(Engine.Notifications);
            var notification = Engine.Notifications[0];
            Assert.Equal("Transfer", notification.EventName);
        }

        [Fact]
        public void Test_TokensOf()
        {
            // Arrange
            var owner = Engine.GetDefaultAccount();
            var account = Engine.GetAccount("NCollectorAccount");

            #if (enableSecurityFeatures)
            Engine.SetSigner(owner);
            Contract.SetMinter(owner.ScriptHash, true);
            #endif

            // Mint multiple NFTs
            var tokenIds = new ByteString[3];
            for (int i = 0; i < 3; i++)
            {
                var properties = new Neo.VM.Types.Map { ["name"] = $"NFT #{i + 1}" };
                tokenIds[i] = Contract.Mint(account.ScriptHash, properties);
            }

            // Act
            var ownedTokens = Contract.TokensOf(account.ScriptHash);
            var tokenCount = 0;
            while (ownedTokens.Next())
            {
                tokenCount++;
            }

            // Assert
            Assert.Equal(3, tokenCount);
            Assert.Equal(3, Contract.BalanceOf(account.ScriptHash));
        }

        #if (enableSecurityFeatures)
        [Fact]
        public void Test_Burn_NFT()
        {
            // Arrange
            var owner = Engine.GetDefaultAccount();
            var tokenOwner = Engine.GetAccount("NTokenOwner");

            Engine.SetSigner(owner);
            Contract.SetMinter(owner.ScriptHash, true);

            var properties = new Neo.VM.Types.Map { ["name"] = "Burnable NFT" };
            var tokenId = Contract.Mint(tokenOwner.ScriptHash, properties);

            // Act
            Engine.SetSigner(tokenOwner);
            Contract.Burn(tokenId);

            // Assert
            Assert.Equal(0, Contract.TotalSupply());
            Assert.Equal(0, Contract.BalanceOf(tokenOwner.ScriptHash));
            Assert.Null(Contract.OwnerOf(tokenId));
        }
        #endif
    }
    #endif

    #if (contractType == 'Governance')
    public class GovernanceContractTests : TestBase<GovernanceContract>
    {
        private readonly ITestOutputHelper _output;

        public GovernanceContractTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test_CreateProposal()
        {
            // Arrange
            var proposer = Engine.GetDefaultAccount();
            var title = "Test Proposal";
            var description = "This is a test proposal";
            var ipfsHash = "QmTestHash123";
            var actions = new byte[] { };

            // Give proposer voting power
            #if (enableSecurityFeatures)
            Engine.SetSigner(proposer);
            #endif
            Contract.SetVotingPower(proposer.ScriptHash, 1000);

            // Act
            Engine.SetSigner(proposer);
            var proposalId = Contract.CreateProposal(title, description, ipfsHash, actions);

            // Assert
            Assert.Equal(1, proposalId);
            
            var proposal = Contract.GetProposal(proposalId);
            Assert.NotNull(proposal);
            Assert.Equal(title, proposal.Title);
            Assert.Equal(description, proposal.Description);
            Assert.Equal(proposer.ScriptHash, proposal.Proposer);
            Assert.False(proposal.Executed);

            // Check event
            Assert.Single(Engine.Notifications);
            var notification = Engine.Notifications[0];
            Assert.Equal("ProposalCreated", notification.EventName);
        }

        [Fact]
        public void Test_Vote_OnProposal()
        {
            // Arrange
            var proposer = Engine.GetDefaultAccount();
            var voter1 = Engine.GetAccount("NVoter1");
            var voter2 = Engine.GetAccount("NVoter2");

            // Setup voting power
            #if (enableSecurityFeatures)
            Engine.SetSigner(proposer);
            #endif
            Contract.SetVotingPower(proposer.ScriptHash, 1000);
            Contract.SetVotingPower(voter1.ScriptHash, 500);
            Contract.SetVotingPower(voter2.ScriptHash, 300);

            // Create proposal
            Engine.SetSigner(proposer);
            var proposalId = Contract.CreateProposal("Test", "Test", "", new byte[] { });
            Engine.ClearNotifications();

            // Act - Vote
            Engine.SetSigner(voter1);
            Contract.Vote(proposalId, true); // Vote for

            Engine.SetSigner(voter2);
            Contract.Vote(proposalId, false); // Vote against

            // Assert
            var proposal = Contract.GetProposal(proposalId);
            Assert.Equal(500, proposal.ForVotes);
            Assert.Equal(300, proposal.AgainstVotes);
            Assert.Equal(800, proposal.TotalVotes);

            // Check events
            Assert.Equal(2, Engine.Notifications.Count);
            Assert.All(Engine.Notifications, n => Assert.Equal("Voted", n.EventName));
        }

        [Fact]
        public void Test_Execute_PassedProposal()
        {
            // Arrange
            var proposer = Engine.GetDefaultAccount();
            var voters = new[]
            {
                Engine.GetAccount("NVoter1"),
                Engine.GetAccount("NVoter2"),
                Engine.GetAccount("NVoter3")
            };

            // Setup voting power
            #if (enableSecurityFeatures)
            Engine.SetSigner(proposer);
            #endif
            Contract.SetVotingPower(proposer.ScriptHash, 1000);
            foreach (var voter in voters)
            {
                Contract.SetVotingPower(voter.ScriptHash, 2000);
            }

            // Create proposal
            Engine.SetSigner(proposer);
            var proposalId = Contract.CreateProposal("Test", "Test", "", new byte[] { });

            // Vote (majority for)
            Engine.SetSigner(voters[0]);
            Contract.Vote(proposalId, true);
            Engine.SetSigner(voters[1]);
            Contract.Vote(proposalId, true);
            Engine.SetSigner(voters[2]);
            Contract.Vote(proposalId, false);

            // Fast forward time past voting period
            Engine.SetTime(Engine.Native.Ledger.CurrentTime + 8 * 24 * 3600 * 1000);

            // Act - Execute
            Engine.ClearNotifications();
            Contract.Execute(proposalId);

            // Assert
            var proposal = Contract.GetProposal(proposalId);
            Assert.True(proposal.Executed);

            // Check event
            Assert.Single(Engine.Notifications);
            var notification = Engine.Notifications[0];
            Assert.Equal("ProposalExecuted", notification.EventName);
        }

        [Fact]
        public void Test_Configuration()
        {
            // Assert default values
            Assert.Equal(7 * 24 * 3600 * 1000, Contract.GetVotingPeriod());
            Assert.Equal(30, Contract.GetQuorumPercentage());
            Assert.Equal(51, Contract.GetPassPercentage());

            #if (enableSecurityFeatures)
            // Arrange
            var owner = Engine.GetDefaultAccount();

            // Act - Update configuration
            Engine.SetSigner(owner);
            Contract.SetVotingPeriod(3 * 24 * 3600 * 1000); // 3 days
            Contract.SetQuorumPercentage(40);
            Contract.SetPassPercentage(60);

            // Assert
            Assert.Equal(3 * 24 * 3600 * 1000, Contract.GetVotingPeriod());
            Assert.Equal(40, Contract.GetQuorumPercentage());
            Assert.Equal(60, Contract.GetPassPercentage());
            #endif
        }
    }
    #endif

    /// <summary>
    /// Base test class that provides common test functionality
    /// </summary>
    /// <typeparam name="TContract">The contract type being tested</typeparam>
    public abstract class TestBase<TContract> : IDisposable
        where TContract : SmartContract
    {
        protected readonly TestEngine Engine;
        protected readonly TContract Contract;

        protected TestBase()
        {
            Engine = new TestEngine();
            Engine.Reset();
            Contract = Engine.Deploy<TContract>(true);
        }

        public void Dispose()
        {
            Engine?.Dispose();
        }
    }

    /// <summary>
    /// Extension methods for test helpers
    /// </summary>
    public static class TestExtensions
    {
        public static Neo.SmartContract.Testing.Account GetAccount(this TestEngine engine, string name)
        {
            return engine.CreateAccount(1000_00000000, name);
        }

        public static Neo.SmartContract.Testing.Account GetDefaultAccount(this TestEngine engine)
        {
            return engine.GetAccount("DefaultAccount");
        }

        public static void ClearNotifications(this TestEngine engine)
        {
            engine.Notifications.Clear();
        }

        public static void SetTime(this TestEngine engine, ulong timestamp)
        {
            // This would need proper implementation to mock time in tests
            // For now, it's a placeholder
        }
    }
}