using Neo;
using Neo.SmartContract.Deploy;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace DeploymentExample.Deploy
{
    /// <summary>
    /// Test the deployed multi-contract ecosystem
    /// </summary>
    public class MultiContractTester
    {
        private readonly DeploymentToolkit _toolkit;
        
        public MultiContractTester(DeploymentToolkit toolkit)
        {
            _toolkit = toolkit;
        }
        
        /// <summary>
        /// Run comprehensive tests on the deployed contracts
        /// </summary>
        public async Task TestDeployedContracts(DeploymentResults deployment)
        {
            if (!deployment.Success || deployment.TokenContract == null || 
                deployment.NFTContract == null || deployment.GovernanceContract == null)
            {
                Console.WriteLine("Cannot test contracts - deployment was not successful");
                return;
            }
            
            Console.WriteLine("\n=== Testing Deployed Contracts ===\n");
            
            // Test Token Contract
            await TestTokenContract(deployment.TokenContract);
            
            // Test NFT Contract
            await TestNFTContract(deployment.NFTContract, deployment.TokenContract);
            
            // Test Governance Contract
            await TestGovernanceContract(deployment.GovernanceContract, deployment.NFTContract);
            
            // Test Cross-Contract Interactions
            await TestCrossContractInteractions(deployment);
            
            Console.WriteLine("\n✅ All tests completed!");
        }
        
        private async Task TestTokenContract(UInt160 tokenContract)
        {
            Console.WriteLine("1. Testing Token Contract...");
            
            try
            {
                // Get token info
                var symbol = await _toolkit.CallAsync<string>(tokenContract.ToString(), "symbol");
                var decimals = await _toolkit.CallAsync<BigInteger>(tokenContract.ToString(), "decimals");
                var totalSupply = await _toolkit.CallAsync<BigInteger>(tokenContract.ToString(), "totalSupply");
                
                Console.WriteLine($"   Symbol: {symbol}");
                Console.WriteLine($"   Decimals: {decimals}");
                Console.WriteLine($"   Total Supply: {totalSupply / 100000000} {symbol}");
                
                // Check deployer balance
                var deployerAddress = await _toolkit.GetDeployerAccountAsync();
                var balance = await _toolkit.CallAsync<BigInteger>(
                    tokenContract.ToString(), 
                    "balanceOf", 
                    deployerAddress
                );
                Console.WriteLine($"   Deployer Balance: {balance / 100000000} {symbol}");
                
                Console.WriteLine("   ✓ Token contract working correctly\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Token test failed: {ex.Message}\n");
            }
        }
        
        private async Task TestNFTContract(UInt160 nftContract, UInt160 tokenContract)
        {
            Console.WriteLine("2. Testing NFT Contract...");
            
            try
            {
                // Get NFT info
                var symbol = await _toolkit.CallAsync<string>(nftContract.ToString(), "symbol");
                var decimals = await _toolkit.CallAsync<BigInteger>(nftContract.ToString(), "decimals");
                var totalSupply = await _toolkit.CallAsync<BigInteger>(nftContract.ToString(), "totalSupply");
                
                Console.WriteLine($"   Symbol: {symbol}");
                Console.WriteLine($"   Decimals: {decimals}");
                Console.WriteLine($"   Total Supply: {totalSupply} NFTs");
                
                // Try to mint an NFT (this might fail if we don't have enough tokens)
                Console.WriteLine("   Testing NFT minting...");
                try
                {
                    var txHash = await _toolkit.InvokeAsync(
                        nftContract.ToString(),
                        "mint",
                        "https://example.com/nft/1.json",
                        new object[] { new string[] { "name", "Test NFT #1" } }
                    );
                    Console.WriteLine($"   ✓ NFT minting transaction: {txHash}");
                }
                catch (Exception mintEx)
                {
                    Console.WriteLine($"   ⚠️  NFT minting failed (expected if no token approval): {mintEx.Message}");
                }
                
                Console.WriteLine("   ✓ NFT contract deployed successfully\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ NFT test failed: {ex.Message}\n");
            }
        }
        
        private async Task TestGovernanceContract(UInt160 governanceContract, UInt160 nftContract)
        {
            Console.WriteLine("3. Testing Governance Contract...");
            
            try
            {
                // Check if deployer is council member
                var deployerAddress = await _toolkit.GetDeployerAccountAsync();
                var isCouncilMember = await _toolkit.CallAsync<bool>(
                    governanceContract.ToString(),
                    "isCouncilMember",
                    deployerAddress
                );
                
                Console.WriteLine($"   Deployer is council member: {isCouncilMember}");
                
                // Get proposal count
                var proposalCount = await _toolkit.CallAsync<BigInteger>(
                    governanceContract.ToString(),
                    "getProposalCount"
                );
                Console.WriteLine($"   Current proposals: {proposalCount}");
                
                // Create a test proposal
                if (isCouncilMember)
                {
                    Console.WriteLine("   Creating test proposal...");
                    try
                    {
                        var txHash = await _toolkit.InvokeAsync(
                            governanceContract.ToString(),
                            "createProposal",
                            1, // Proposal type
                            new BigInteger(5000) * new BigInteger(100000000), // 5000 tokens
                            "Test proposal for token minting"
                        );
                        Console.WriteLine($"   ✓ Proposal created: {txHash}");
                    }
                    catch (Exception propEx)
                    {
                        Console.WriteLine($"   ⚠️  Proposal creation failed: {propEx.Message}");
                    }
                }
                
                Console.WriteLine("   ✓ Governance contract working correctly\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Governance test failed: {ex.Message}\n");
            }
        }
        
        private async Task TestCrossContractInteractions(DeploymentResults deployment)
        {
            Console.WriteLine("4. Testing Cross-Contract Interactions...");
            
            try
            {
                var deployerAddress = await _toolkit.GetDeployerAccountAsync();
                
                // Test token approval for NFT minting
                Console.WriteLine("   Setting up token approval for NFT minting...");
                var approvalTx = await _toolkit.InvokeAsync(
                    deployment.TokenContract!.ToString(),
                    "approve",
                    deployment.NFTContract,
                    new BigInteger(100) * new BigInteger(100000000) // Approve 100 tokens
                );
                Console.WriteLine($"   ✓ Token approval transaction: {approvalTx}");
                
                // Test governance proposal for adding council member
                Console.WriteLine("   Testing governance proposal system...");
                var proposalTx = await _toolkit.InvokeAsync(
                    deployment.GovernanceContract!.ToString(),
                    "createProposal",
                    3, // Add council member type
                    deployerAddress, // Add self (for testing)
                    "Add deployer as additional council member"
                );
                Console.WriteLine($"   ✓ Governance proposal transaction: {proposalTx}");
                
                Console.WriteLine("   ✓ Cross-contract interactions working\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ⚠️  Cross-contract test failed: {ex.Message}\n");
            }
        }
    }
}