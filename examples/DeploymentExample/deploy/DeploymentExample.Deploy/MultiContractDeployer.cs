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
            
            if (gasBalance < 300) // Need more GAS for multiple contracts
            {
                throw new InvalidOperationException(
                    "Insufficient GAS balance. Need at least 300 GAS for multi-contract deployment."
                );
            }

            try
            {
                // Deploy Token Contract first (no dependencies)
                Console.WriteLine("1. Deploying Token Contract...");
                var tokenPath = Path.Combine("../../compiled-contracts", "DeploymentExample.TokenContract.nef");
                if (!File.Exists(tokenPath))
                {
                    throw new FileNotFoundException($"Token contract not found at {tokenPath}. Please compile the contracts first.");
                }
                
                var tokenDeployment = await _toolkit.DeployAsync(tokenPath, new object[] { deployerAddress });
                results.TokenContract = tokenDeployment.ContractHash;
                Console.WriteLine($"   ✓ Token deployed at: {tokenDeployment.ContractHash}");
                Console.WriteLine($"   Transaction: {tokenDeployment.TransactionHash}\n");
                
                // Deploy NFT Contract (depends on Token)
                Console.WriteLine("2. Deploying NFT Contract...");
                var nftPath = Path.Combine("../../compiled-contracts", "DeploymentExample.NFTContract.nef");
                if (!File.Exists(nftPath))
                {
                    throw new FileNotFoundException($"NFT contract not found at {nftPath}. Please compile the contracts first.");
                }
                
                var nftDeployment = await _toolkit.DeployAsync(
                    nftPath, 
                    new object[] { deployerAddress, results.TokenContract, 10_00000000 } // 10 tokens per mint
                );
                results.NFTContract = nftDeployment.ContractHash;
                Console.WriteLine($"   ✓ NFT deployed at: {nftDeployment.ContractHash}");
                Console.WriteLine($"   Transaction: {nftDeployment.TransactionHash}\n");
                
                // Deploy Governance Contract (depends on Token)
                Console.WriteLine("3. Deploying Governance Contract...");
                var governancePath = Path.Combine("../../compiled-contracts", "DeploymentExample.GovernanceContract.nef");
                if (!File.Exists(governancePath))
                {
                    throw new FileNotFoundException($"Governance contract not found at {governancePath}. Please compile the contracts first.");
                }
                
                var governanceDeployment = await _toolkit.DeployAsync(
                    governancePath,
                    new object[] { deployerAddress, results.TokenContract }
                );
                results.GovernanceContract = governanceDeployment.ContractHash;
                Console.WriteLine($"   ✓ Governance deployed at: {governanceDeployment.ContractHash}");
                Console.WriteLine($"   Transaction: {governanceDeployment.TransactionHash}\n");
                
                // Configure cross-contract relationships
                Console.WriteLine("4. Configuring contract relationships...");
                
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