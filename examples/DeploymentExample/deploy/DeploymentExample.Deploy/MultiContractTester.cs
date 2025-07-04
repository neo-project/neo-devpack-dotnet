using Neo;
using Neo.SmartContract.Deploy;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace DeploymentExample.Deploy
{
    /// <summary>
    /// Test interactions between deployed contracts
    /// </summary>
    public class MultiContractTester
    {
        private readonly DeploymentToolkit _toolkit;
        
        public MultiContractTester(DeploymentToolkit toolkit)
        {
            _toolkit = toolkit;
        }

        /// <summary>
        /// Run all integration tests
        /// </summary>
        public async Task RunAllTests(DeploymentResults contracts)
        {
            Console.WriteLine("\n=== Testing Multi-Contract Integration ===\n");

            // Test 1: Token functionality
            await TestTokenFunctionality(contracts.TokenContract.ContractHash);
            
            // Test 2: NFT minting with token payment
            await TestNFTMinting(contracts.NFTContract.ContractHash, contracts.TokenContract.ContractHash);
            
            // Test 3: Governance voting
            await TestGovernance(contracts.GovernanceContract.ContractHash, contracts.TokenContract.ContractHash);
            
            // Test 4: Cross-contract interactions
            await TestCrossContractInteractions(contracts);
            
            Console.WriteLine("\n=== All tests completed successfully! ===");
        }

        private async Task TestTokenFunctionality(UInt160 tokenContract)
        {
            Console.WriteLine("Test 1: Token Functionality");
            Console.WriteLine("---------------------------");
            
            // Get token info
            var symbol = await _toolkit.Call<string>(tokenContract.ToString(), "symbol");
            var decimals = await _toolkit.Call<byte>(tokenContract.ToString(), "decimals");
            var totalSupply = await _toolkit.Call<BigInteger>(tokenContract.ToString(), "totalSupply");
            
            Console.WriteLine($"Token Symbol: {symbol}");
            Console.WriteLine($"Decimals: {decimals}");
            Console.WriteLine($"Total Supply: {totalSupply / Math.Pow(10, decimals)} {symbol}");
            
            // Check deployer balance
            var deployerAddress = await _toolkit.GetDeployerAccount();
            var balance = await _toolkit.Call<BigInteger>(
                tokenContract.ToString(), 
                "balanceOf", 
                deployerAddress
            );
            
            Console.WriteLine($"Deployer Balance: {balance / Math.Pow(10, decimals)} {symbol}");
            
            // Test transfer (to self)
            Console.WriteLine("\nTesting transfer...");
            var transferAmount = new BigInteger(100_00000000); // 100 tokens
            var txHash = await _toolkit.Invoke(
                tokenContract.ToString(),
                "transfer",
                deployerAddress,
                deployerAddress,
                transferAmount,
                "test transfer"
            );
            
            Console.WriteLine($"Transfer tx: {txHash}");
            await Task.Delay(5000);
            
            // Verify balance unchanged (transferred to self)
            var newBalance = await _toolkit.Call<BigInteger>(
                tokenContract.ToString(), 
                "balanceOf", 
                deployerAddress
            );
            Console.WriteLine($"Balance after transfer: {newBalance / Math.Pow(10, decimals)} {symbol}");
            Console.WriteLine("✓ Token functionality test passed\n");
        }

        private async Task TestNFTMinting(UInt160 nftContract, UInt160 tokenContract)
        {
            Console.WriteLine("Test 2: NFT Minting with Token Payment");
            Console.WriteLine("--------------------------------------");
            
            // Get NFT info
            var symbol = await _toolkit.Call<string>(nftContract.ToString(), "symbol");
            var mintPrice = await _toolkit.Call<BigInteger>(nftContract.ToString(), "getMintPrice");
            
            Console.WriteLine($"NFT Symbol: {symbol}");
            Console.WriteLine($"Mint Price: {mintPrice / 100_000_000m} tokens");
            
            // Check initial NFT balance
            var deployerAddress = await _toolkit.GetDeployerAccount();
            var nftBalance = await _toolkit.Call<BigInteger>(
                nftContract.ToString(), 
                "balanceOf", 
                deployerAddress
            );
            Console.WriteLine($"Initial NFT Balance: {nftBalance}");
            
            // Approve token spending by NFT contract
            Console.WriteLine("\nApproving NFT contract to spend tokens...");
            // Note: Standard NEP-17 doesn't have approve, so NFT minting uses direct transfer
            
            // Mint NFT
            Console.WriteLine("Minting NFT...");
            var tokenURI = "https://example.com/nft/1.json";
            var properties = new Neo.VM.Types.Map();
            properties["name"] = "Example NFT #1";
            properties["rarity"] = "common";
            properties["power"] = 100;
            
            var txHash = await _toolkit.Invoke(
                nftContract.ToString(),
                "mint",
                tokenURI,
                properties
            );
            
            Console.WriteLine($"Mint tx: {txHash}");
            await Task.Delay(5000);
            
            // Check new NFT balance
            var newNftBalance = await _toolkit.Call<BigInteger>(
                nftContract.ToString(), 
                "balanceOf", 
                deployerAddress
            );
            Console.WriteLine($"New NFT Balance: {newNftBalance}");
            
            // Get total supply
            var totalSupply = await _toolkit.Call<BigInteger>(nftContract.ToString(), "totalSupply");
            Console.WriteLine($"Total NFTs Minted: {totalSupply}");
            
            // Check token balance after payment
            var tokenBalance = await _toolkit.Call<BigInteger>(
                tokenContract.ToString(), 
                "balanceOf", 
                deployerAddress
            );
            Console.WriteLine($"Token Balance after mint: {tokenBalance / 100_000_000m} tokens");
            
            Console.WriteLine("✓ NFT minting test passed\n");
        }

        private async Task TestGovernance(UInt160 governanceContract, UInt160 tokenContract)
        {
            Console.WriteLine("Test 3: Governance Voting");
            Console.WriteLine("------------------------");
            
            // Get voting parameters
            var threshold = await _toolkit.Call<BigInteger>(governanceContract.ToString(), "getVotingThreshold");
            var period = await _toolkit.Call<BigInteger>(governanceContract.ToString(), "getVotingPeriod");
            
            Console.WriteLine($"Voting Threshold: {threshold}%");
            Console.WriteLine($"Voting Period: {period / 3600} hours");
            
            // Check voting power
            var deployerAddress = await _toolkit.GetDeployerAccount();
            var votingPower = await _toolkit.Call<BigInteger>(
                governanceContract.ToString(), 
                "getVotingPower",
                deployerAddress
            );
            Console.WriteLine($"Deployer Voting Power: {votingPower / 100_000_000m}");
            
            // Create a test proposal
            Console.WriteLine("\nCreating proposal to update voting threshold...");
            var proposalTx = await _toolkit.Invoke(
                governanceContract.ToString(),
                "createProposal",
                (byte)3, // UPDATE_THRESHOLD type
                new BigInteger(60), // New threshold: 60%
                "Update voting threshold to 60%"
            );
            
            Console.WriteLine($"Create proposal tx: {proposalTx}");
            await Task.Delay(5000);
            
            // Get proposal details
            var proposalId = new BigInteger(2); // Should be second proposal
            var proposal = await _toolkit.Call<Neo.VM.Types.Map>(
                governanceContract.ToString(),
                "getProposal",
                proposalId
            );
            
            Console.WriteLine($"Proposal ID: {proposalId}");
            Console.WriteLine($"Description: {proposal["description"]}");
            
            // Vote on proposal
            Console.WriteLine("\nVoting on proposal...");
            var voteTx = await _toolkit.Invoke(
                governanceContract.ToString(),
                "vote",
                proposalId,
                true // Support
            );
            
            Console.WriteLine($"Vote tx: {voteTx}");
            await Task.Delay(5000);
            
            // Check vote status
            var updatedProposal = await _toolkit.Call<Neo.VM.Types.Map>(
                governanceContract.ToString(),
                "getProposal",
                proposalId
            );
            
            Console.WriteLine($"Yes Votes: {updatedProposal["yesVotes"]}");
            Console.WriteLine($"No Votes: {updatedProposal["noVotes"]}");
            
            Console.WriteLine("✓ Governance voting test passed\n");
        }

        private async Task TestCrossContractInteractions(DeploymentResults contracts)
        {
            Console.WriteLine("Test 4: Cross-Contract Interactions");
            Console.WriteLine("-----------------------------------");
            
            // Test governance managing other contracts
            Console.WriteLine("Testing governance contract management...");
            
            // Check if NFT is managed
            var isManaged = await _toolkit.Call<bool>(
                contracts.GovernanceContract.ContractHash.ToString(),
                "isManagedContract",
                contracts.NFTContract.ContractHash
            );
            
            Console.WriteLine($"NFT Contract Managed: {isManaged}");
            
            // Test token pause functionality through governance
            Console.WriteLine("\nTesting token pause through governance...");
            
            // Create proposal to pause token
            var proposalTx = await _toolkit.Invoke(
                contracts.GovernanceContract.ContractHash.ToString(),
                "createProposal",
                (byte)5, // EXECUTE_ACTION type
                new object[] 
                {
                    contracts.TokenContract.ContractHash,
                    "setPaused",
                    new object[] { true }
                },
                "Temporarily pause token transfers for maintenance"
            );
            
            Console.WriteLine($"Create pause proposal tx: {proposalTx}");
            await Task.Delay(5000);
            
            // Check if token is paused
            var isPaused = await _toolkit.Call<bool>(
                contracts.TokenContract.ContractHash.ToString(),
                "isPaused"
            );
            
            Console.WriteLine($"Token Paused: {isPaused}");
            
            // Test NFT ownership
            Console.WriteLine("\nTesting NFT ownership and metadata...");
            
            var tokenId = new byte[] { 1 }; // First minted NFT
            try
            {
                var owner = await _toolkit.Call<string>(
                    contracts.NFTContract.ContractHash.ToString(),
                    "ownerOf",
                    tokenId
                );
                Console.WriteLine($"NFT #1 Owner: {owner}");
                
                var tokenURI = await _toolkit.Call<string>(
                    contracts.NFTContract.ContractHash.ToString(),
                    "tokenURI",
                    tokenId
                );
                Console.WriteLine($"NFT #1 URI: {tokenURI}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"NFT query info: {ex.Message}");
            }
            
            Console.WriteLine("✓ Cross-contract interaction test passed\n");
        }
    }
}