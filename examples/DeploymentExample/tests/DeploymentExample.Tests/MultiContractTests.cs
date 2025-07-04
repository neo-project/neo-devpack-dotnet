using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using Neo.VM.Types;
using System;
using System.Linq;
using System.Numerics;

namespace DeploymentExample.Tests
{
    [TestClass]
    public class MultiContractTests : TestBase<TokenContract>
    {
        private TokenContract _tokenContract;
        private NFTContract _nftContract;
        private GovernanceContract _governanceContract;

        [TestInitialize]
        public void Setup()
        {
            // Deploy Token Contract first
            _tokenContract = Contract.Deploy<TokenContract>(
                Engine.GetScriptHash(Alice) // Alice as initial owner
            );

            // Deploy NFT Contract with Token dependency
            _nftContract = Contract.Deploy<NFTContract>(
                Engine.GetScriptHash(Alice), // Owner
                _tokenContract.Hash,         // Token contract for payments
                10_00000000                  // Mint price: 10 tokens
            );

            // Deploy Governance Contract
            _governanceContract = Contract.Deploy<GovernanceContract>(
                Engine.GetScriptHash(Alice), // Owner
                _tokenContract.Hash          // Token contract for voting power
            );

            // Configure Token to recognize Governance
            _tokenContract.SetGovernance(_governanceContract.Hash);
        }

        [TestMethod]
        public void Test_TokenContract_Basic_Functionality()
        {
            // Test token properties
            Assert.AreEqual("EXT", _tokenContract.Symbol());
            Assert.AreEqual(8, _tokenContract.Decimals());
            Assert.AreEqual(100_000_000_00000000, _tokenContract.TotalSupply());

            // Test initial balance
            var aliceBalance = _tokenContract.BalanceOf(Engine.GetScriptHash(Alice));
            Assert.AreEqual(100_000_000_00000000, aliceBalance);

            // Test transfer
            var transferAmount = 1000_00000000; // 1000 tokens
            _tokenContract.Transfer(
                Engine.GetScriptHash(Alice),
                Engine.GetScriptHash(Bob),
                transferAmount,
                null
            );

            // Verify balances after transfer
            var newAliceBalance = _tokenContract.BalanceOf(Engine.GetScriptHash(Alice));
            var bobBalance = _tokenContract.BalanceOf(Engine.GetScriptHash(Bob));
            
            Assert.AreEqual(99_999_000_00000000, newAliceBalance);
            Assert.AreEqual(1000_00000000, bobBalance);
        }

        [TestMethod]
        public void Test_NFT_Minting_With_Token_Payment()
        {
            // Check initial NFT supply
            Assert.AreEqual(0, _nftContract.TotalSupply());
            Assert.AreEqual(0, _nftContract.BalanceOf(Engine.GetScriptHash(Alice)));

            // Check mint price
            Assert.AreEqual(10_00000000, _nftContract.GetMintPrice());

            // Mint NFT
            var tokenURI = "https://example.com/nft/1.json";
            var properties = new Map();
            properties["name"] = "Test NFT";
            properties["rarity"] = "common";

            var tokenId = _nftContract.Mint(tokenURI, properties);

            // Verify NFT was minted
            Assert.AreEqual(1, _nftContract.TotalSupply());
            Assert.AreEqual(1, _nftContract.BalanceOf(Engine.GetScriptHash(Alice)));
            Assert.AreEqual(Engine.GetScriptHash(Alice), _nftContract.OwnerOf(tokenId));

            // Verify token URI and properties
            Assert.AreEqual(tokenURI, _nftContract.TokenURI(tokenId));
            var storedProps = _nftContract.Properties(tokenId);
            Assert.AreEqual("Test NFT", storedProps["name"]);
            Assert.AreEqual("common", storedProps["rarity"]);

            // Verify token payment was deducted
            var expectedBalance = 100_000_000_00000000 - 10_00000000; // Total - mint price
            Assert.AreEqual(expectedBalance, _tokenContract.BalanceOf(Engine.GetScriptHash(Alice)));
        }

        [TestMethod]
        public void Test_NFT_Transfer()
        {
            // Mint an NFT first
            var tokenURI = "https://example.com/nft/1.json";
            var properties = new Map();
            properties["name"] = "Test NFT";

            var tokenId = _nftContract.Mint(tokenURI, properties);

            // Transfer NFT from Alice to Bob
            _nftContract.Transfer(Engine.GetScriptHash(Bob), tokenId, null);

            // Verify ownership changed
            Assert.AreEqual(Engine.GetScriptHash(Bob), _nftContract.OwnerOf(tokenId));
            Assert.AreEqual(0, _nftContract.BalanceOf(Engine.GetScriptHash(Alice)));
            Assert.AreEqual(1, _nftContract.BalanceOf(Engine.GetScriptHash(Bob)));
        }

        [TestMethod]
        public void Test_Governance_Proposal_Creation()
        {
            // Check initial voting power
            var votingPower = _governanceContract.GetVotingPower(Engine.GetScriptHash(Alice));
            Assert.IsTrue(votingPower > 0, "Alice should have voting power based on token balance");

            // Create a proposal
            var proposalId = _governanceContract.CreateProposal(
                1, // ADD_CONTRACT type
                _nftContract.Hash,
                "Add NFT contract to governance"
            );

            Assert.AreEqual(1, proposalId);

            // Get proposal details
            var proposal = _governanceContract.GetProposal(proposalId);
            Assert.AreEqual(Engine.GetScriptHash(Alice), proposal["proposer"]);
            Assert.AreEqual(1, proposal["type"]);
            Assert.AreEqual("Add NFT contract to governance", proposal["description"]);
            Assert.AreEqual(0, proposal["yesVotes"]);
            Assert.AreEqual(0, proposal["noVotes"]);
            Assert.AreEqual(false, proposal["executed"]);
        }

        [TestMethod]
        public void Test_Governance_Voting()
        {
            // Create a proposal first
            var proposalId = _governanceContract.CreateProposal(
                1, // ADD_CONTRACT type
                _nftContract.Hash,
                "Add NFT contract to governance"
            );

            // Vote on the proposal
            _governanceContract.Vote(proposalId, true); // Support

            // Check updated proposal
            var proposal = _governanceContract.GetProposal(proposalId);
            var expectedVotes = _tokenContract.BalanceOf(Engine.GetScriptHash(Alice));
            Assert.AreEqual(expectedVotes, proposal["yesVotes"]);
            Assert.AreEqual(0, proposal["noVotes"]);
        }

        [TestMethod]
        public void Test_Token_Governance_Integration()
        {
            // Verify governance is set correctly
            Assert.AreEqual(_governanceContract.Hash, _tokenContract.GetGovernance());

            // Test that governance can mint tokens
            var initialSupply = _tokenContract.TotalSupply();
            var mintAmount = 1000_00000000; // 1000 tokens

            // This would require a governance proposal in real scenario
            // For testing, we call directly as governance contract
            Engine.CallContract(_governanceContract.Hash, _tokenContract.Hash, "mint", 
                Engine.GetScriptHash(Bob), mintAmount);

            // Verify tokens were minted
            var newSupply = _tokenContract.TotalSupply();
            var bobBalance = _tokenContract.BalanceOf(Engine.GetScriptHash(Bob));
            
            Assert.AreEqual(initialSupply + mintAmount, newSupply);
            Assert.AreEqual(mintAmount, bobBalance);
        }

        [TestMethod]
        public void Test_Cross_Contract_Interactions()
        {
            // Test that NFT contract can call token contract for payments
            var initialBalance = _tokenContract.BalanceOf(Engine.GetScriptHash(Alice));
            var mintPrice = _nftContract.GetMintPrice();

            // Mint NFT (should trigger token transfer)
            var tokenURI = "https://example.com/nft/cross-test.json";
            var properties = new Map();
            properties["test"] = "cross-contract";

            _nftContract.Mint(tokenURI, properties);

            // Verify payment was processed
            var newBalance = _tokenContract.BalanceOf(Engine.GetScriptHash(Alice));
            Assert.AreEqual(initialBalance - mintPrice, newBalance);

            // Verify owner received payment
            var ownerBalance = _tokenContract.BalanceOf(_nftContract.GetOwner());
            Assert.AreEqual(mintPrice, ownerBalance);
        }

        [TestMethod]
        public void Test_Governance_Contract_Management()
        {
            // Initially, NFT should not be managed
            Assert.IsFalse(_governanceContract.IsManagedContract(_nftContract.Hash));

            // Create and vote on proposal to add NFT to governance
            var proposalId = _governanceContract.CreateProposal(
                1, // ADD_CONTRACT type
                _nftContract.Hash,
                "Add NFT contract to governance"
            );

            _governanceContract.Vote(proposalId, true);

            // Fast-forward time to after voting period (in real deployment, would wait)
            Engine.IncreaseTime(TimeSpan.FromDays(8));

            // Execute the proposal
            _governanceContract.ExecuteProposal(proposalId);

            // Verify NFT is now managed
            Assert.IsTrue(_governanceContract.IsManagedContract(_nftContract.Hash));
        }

        [TestMethod]
        public void Test_Token_Pause_Functionality()
        {
            // Initially not paused
            Assert.IsFalse(_tokenContract.IsPaused());

            // Pause through governance
            _tokenContract.SetPaused(true);
            Assert.IsTrue(_tokenContract.IsPaused());

            // Try to transfer while paused (should fail)
            Assert.ThrowsException<Exception>(() =>
            {
                _tokenContract.Transfer(
                    Engine.GetScriptHash(Alice),
                    Engine.GetScriptHash(Bob),
                    1000_00000000,
                    null
                );
            });

            // Unpause
            _tokenContract.SetPaused(false);
            Assert.IsFalse(_tokenContract.IsPaused());

            // Transfer should work now
            _tokenContract.Transfer(
                Engine.GetScriptHash(Alice),
                Engine.GetScriptHash(Bob),
                1000_00000000,
                null
            );

            Assert.AreEqual(1000_00000000, _tokenContract.BalanceOf(Engine.GetScriptHash(Bob)));
        }

        [TestMethod]
        public void Test_Events_Emission()
        {
            // Test Token Transfer event
            var transferAmount = 500_00000000;
            _tokenContract.Transfer(
                Engine.GetScriptHash(Alice),
                Engine.GetScriptHash(Bob),
                transferAmount,
                null
            );

            // Verify Transfer event was emitted
            var transferEvents = Engine.GetNotifications(_tokenContract.Hash)
                .Where(n => n.EventName == "Transfer")
                .ToArray();
            
            Assert.AreEqual(1, transferEvents.Length);
            Assert.AreEqual(Engine.GetScriptHash(Alice), transferEvents[0].State[0]);
            Assert.AreEqual(Engine.GetScriptHash(Bob), transferEvents[0].State[1]);
            Assert.AreEqual(transferAmount, transferEvents[0].State[2]);

            // Test NFT Mint event
            var tokenURI = "https://example.com/nft/event-test.json";
            var properties = new Map();
            properties["event"] = "test";

            var tokenId = _nftContract.Mint(tokenURI, properties);

            // Verify Minted event was emitted
            var mintEvents = Engine.GetNotifications(_nftContract.Hash)
                .Where(n => n.EventName == "Minted")
                .ToArray();
            
            Assert.AreEqual(1, mintEvents.Length);
            Assert.AreEqual(Engine.GetScriptHash(Alice), mintEvents[0].State[0]);
            Assert.AreEqual(tokenId, mintEvents[0].State[1]);
            Assert.AreEqual(tokenURI, mintEvents[0].State[2]);
        }

        [TestMethod]
        public void Test_Access_Control()
        {
            // Only owner should be able to set governance
            Engine.SetCallingAccount(Bob); // Switch to Bob

            Assert.ThrowsException<Exception>(() =>
            {
                _tokenContract.SetGovernance(_governanceContract.Hash);
            });

            // Only owner should be able to set NFT mint price
            Assert.ThrowsException<Exception>(() =>
            {
                _nftContract.SetMintPrice(20_00000000);
            });

            // Switch back to Alice (owner)
            Engine.SetCallingAccount(Alice);

            // These should work
            _tokenContract.SetGovernance(_governanceContract.Hash);
            _nftContract.SetMintPrice(20_00000000);
            Assert.AreEqual(20_00000000, _nftContract.GetMintPrice());
        }
    }
}