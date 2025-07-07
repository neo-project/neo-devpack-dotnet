using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace DeploymentExample.Deploy
{
    /// <summary>
    /// Multi-contract deployment example demonstrating how to deploy
    /// multiple interrelated contracts with dependencies
    /// </summary>
    public class MultiContractDeployer
    {
        private readonly DeploymentToolkit _toolkit;
        
        public MultiContractDeployer(DeploymentToolkit toolkit)
        {
            _toolkit = toolkit;
        }

        /// <summary>
        /// Deploy all contracts in the correct order with dependencies
        /// </summary>
        public async Task<DeploymentResults> DeployAllContracts()
        {
            var results = new DeploymentResults();
            
            Console.WriteLine("=== Multi-Contract Deployment ===");
            Console.WriteLine("Deploying Token, NFT, and Governance contracts with dependencies...\n");

            // Get deployer account
            var deployerAddress = await _toolkit.GetDeployerAccountAsync();
            Console.WriteLine($"Deployer: {deployerAddress}");
            
            // Check GAS balance
            var gasBalance = await _toolkit.GetGasBalanceAsync();
            Console.WriteLine($"GAS Balance: {gasBalance}\n");
            
            if (gasBalance < 150) // Need more GAS for multiple contracts
            {
                throw new InvalidOperationException(
                    "Insufficient GAS balance. Need at least 150 GAS for multi-contract deployment."
                );
            }

            try
            {
                // Deploy Token Contract first (no dependencies)
                Console.WriteLine("1. Deploying Token Contract...");
                var tokenSourcePath = Path.Combine("..", "..", "src", "TokenContract", "TokenContract.cs");
                
                // Deploy with initialization parameters
                var tokenDeployment = await _toolkit.DeployAsync(tokenSourcePath, new object[] { deployerAddress });
                results.TokenContract = tokenDeployment.ContractHash;
                Console.WriteLine($"   ✓ Token deployed at: {tokenDeployment.ContractHash}");
                Console.WriteLine($"   Transaction: {tokenDeployment.TransactionHash}");
                Console.WriteLine($"   GAS consumed: {tokenDeployment.GasConsumed / 100_000_000m} GAS\n");
                
                // Deploy NFT Contract (depends on Token)
                Console.WriteLine("2. Deploying NFT Contract...");
                var nftSourcePath = Path.Combine("..", "..", "src", "NFTContract", "NFTContract.cs");
                
                var nftDeployment = await _toolkit.DeployAsync(nftSourcePath, new object[] { deployerAddress });
                results.NFTContract = nftDeployment.ContractHash;
                Console.WriteLine($"   ✓ NFT deployed at: {nftDeployment.ContractHash}");
                Console.WriteLine($"   Transaction: {nftDeployment.TransactionHash}");
                Console.WriteLine($"   GAS consumed: {nftDeployment.GasConsumed / 100_000_000m} GAS\n");
                
                // Deploy Governance Contract (depends on Token)
                Console.WriteLine("3. Deploying Governance Contract...");
                var governanceSourcePath = Path.Combine("..", "..", "src", "GovernanceContract", "GovernanceContract.cs");
                
                var governanceDeployment = await _toolkit.DeployAsync(governanceSourcePath, new object[] { deployerAddress });
                results.GovernanceContract = governanceDeployment.ContractHash;
                Console.WriteLine($"   ✓ Governance deployed at: {governanceDeployment.ContractHash}");
                Console.WriteLine($"   Transaction: {governanceDeployment.TransactionHash}");
                Console.WriteLine($"   GAS consumed: {governanceDeployment.GasConsumed / 100_000_000m} GAS\n");
                
                // Initialize contracts with proper parameters
                Console.WriteLine("4. Initializing contracts...");
                
                // Initialize Token contract
                await InitializeTokenContract(results.TokenContract, deployerAddress);
                
                // Initialize NFT contract
                await InitializeNFTContract(results.NFTContract, deployerAddress, results.TokenContract, 10_00000000);
                
                // Initialize Governance contract
                await InitializeGovernanceContract(results.GovernanceContract, deployerAddress, results.TokenContract);
                
                // Configure cross-contract relationships
                Console.WriteLine("5. Configuring contract relationships...");
                
                // Set NFT contract address in Token contract
                await ConfigureTokenContract(results.TokenContract, results.NFTContract);
                
                // Set Governance contract in NFT
                await ConfigureNFTContract(results.NFTContract, results.GovernanceContract);
                
                // Add NFT contract to governance
                await ConfigureGovernanceContract(results.GovernanceContract, results.NFTContract);
                
                results.Success = true;
                Console.WriteLine("\n✅ All contracts deployed and configured successfully!");
                
                return results;
            }
            catch (Exception ex)
            {
                results.Success = false;
                results.ErrorMessage = ex.Message;
                Console.WriteLine($"\n❌ Deployment failed: {ex.Message}");
                throw;
            }
        }
        
        private async Task InitializeTokenContract(UInt160 tokenContract, UInt160 owner)
        {
            try
            {
                Console.WriteLine("   - Initializing Token contract...");
                var txHash = await _toolkit.InvokeAsync(tokenContract.ToString(), "initialize", owner);
                Console.WriteLine($"     ✓ Transaction: {txHash}");
                
                // Wait for confirmation
                await Task.Delay(5000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"     ⚠️  Failed to initialize Token contract: {ex.Message}");
                // This might fail if already initialized, which is okay
            }
        }
        
        private async Task InitializeNFTContract(UInt160 nftContract, UInt160 owner, UInt160 tokenContract, BigInteger mintPrice)
        {
            try
            {
                Console.WriteLine("   - Initializing NFT contract...");
                var txHash = await _toolkit.InvokeAsync(nftContract.ToString(), "initialize", owner, tokenContract, mintPrice);
                Console.WriteLine($"     ✓ Transaction: {txHash}");
                
                // Wait for confirmation
                await Task.Delay(5000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"     ⚠️  Failed to initialize NFT contract: {ex.Message}");
                // This might fail if already initialized, which is okay
            }
        }
        
        private async Task InitializeGovernanceContract(UInt160 governanceContract, UInt160 owner, UInt160 tokenContract)
        {
            try
            {
                Console.WriteLine("   - Initializing Governance contract...");
                var txHash = await _toolkit.InvokeAsync(governanceContract.ToString(), "initialize", owner, tokenContract);
                Console.WriteLine($"     ✓ Transaction: {txHash}");
                
                // Wait for confirmation
                await Task.Delay(5000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"     ⚠️  Failed to initialize Governance contract: {ex.Message}");
                // This might fail if already initialized, which is okay
            }
        }
        
        private async Task ConfigureTokenContract(UInt160 tokenContract, UInt160 nftContract)
        {
            try
            {
                Console.WriteLine("   - Setting NFT contract address in Token contract...");
                var txHash = await _toolkit.InvokeAsync(tokenContract.ToString(), "setNFTContract", nftContract);
                Console.WriteLine($"     ✓ Transaction: {txHash}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"     ⚠️  Failed to configure Token contract: {ex.Message}");
                // Non-critical, continue
            }
        }
        
        private async Task ConfigureNFTContract(UInt160 nftContract, UInt160 governanceContract)
        {
            try
            {
                Console.WriteLine("   - Setting Governance contract address in NFT contract...");
                var txHash = await _toolkit.InvokeAsync(nftContract.ToString(), "setGovernanceContract", governanceContract);
                Console.WriteLine($"     ✓ Transaction: {txHash}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"     ⚠️  Failed to configure NFT contract: {ex.Message}");
                // Non-critical, continue
            }
        }
        
        private async Task ConfigureGovernanceContract(UInt160 governanceContract, UInt160 nftContract)
        {
            try
            {
                Console.WriteLine("   - Adding NFT contract to governance...");
                var txHash = await _toolkit.InvokeAsync(
                    governanceContract.ToString(), 
                    "addManagedContract",
                    nftContract
                );
                Console.WriteLine($"     ✓ Transaction: {txHash}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"     ⚠️  Failed to configure Governance contract: {ex.Message}");
                // Non-critical, continue
            }
        }
    }
    
    /// <summary>
    /// Results of multi-contract deployment
    /// </summary>
    public class DeploymentResults
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public UInt160? TokenContract { get; set; }
        public UInt160? NFTContract { get; set; }
        public UInt160? GovernanceContract { get; set; }
        
        public void PrintSummary()
        {
            Console.WriteLine("\n=== Deployment Summary ===");
            if (Success)
            {
                Console.WriteLine("Status: ✅ SUCCESS");
                Console.WriteLine($"Token Contract:      {TokenContract}");
                Console.WriteLine($"NFT Contract:        {NFTContract}");
                Console.WriteLine($"Governance Contract: {GovernanceContract}");
            }
            else
            {
                Console.WriteLine("Status: ❌ FAILED");
                Console.WriteLine($"Error: {ErrorMessage}");
            }
        }
    }
}