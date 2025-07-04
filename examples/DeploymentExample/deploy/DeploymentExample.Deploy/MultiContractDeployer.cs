using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Models;
using System;
using System.Collections.Generic;
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
            var deployerAddress = await _toolkit.GetDeployerAccount();
            Console.WriteLine($"Deployer: {deployerAddress}");
            
            // Check GAS balance
            var gasBalance = await _toolkit.GetGasBalance();
            Console.WriteLine($"GAS Balance: {gasBalance}\n");
            
            if (gasBalance < 300) // Need more GAS for multiple contracts
            {
                throw new InvalidOperationException(
                    "Insufficient GAS balance. Need at least 300 GAS for multi-contract deployment."
                );
            }

            try
            {
                // Step 1: Deploy Token Contract (no dependencies)
                Console.WriteLine("Step 1: Deploying Token Contract...");
                var tokenResult = await DeployTokenContract(deployerAddress);
                results.TokenContract = tokenResult;
                Console.WriteLine($"✓ Token deployed: {tokenResult.ContractHash}\n");

                // Step 2: Deploy NFT Contract (depends on Token for payments)
                Console.WriteLine("Step 2: Deploying NFT Contract...");
                var nftResult = await DeployNFTContract(deployerAddress, tokenResult.ContractHash);
                results.NFTContract = nftResult;
                Console.WriteLine($"✓ NFT deployed: {nftResult.ContractHash}\n");

                // Step 3: Deploy Governance Contract (manages Token and NFT)
                Console.WriteLine("Step 3: Deploying Governance Contract...");
                var govResult = await DeployGovernanceContract(deployerAddress, tokenResult.ContractHash);
                results.GovernanceContract = govResult;
                Console.WriteLine($"✓ Governance deployed: {govResult.ContractHash}\n");

                // Step 4: Update Token to recognize Governance
                Console.WriteLine("Step 4: Configuring Token contract...");
                await ConfigureTokenContract(tokenResult.ContractHash, govResult.ContractHash);
                Console.WriteLine("✓ Token configured\n");

                // Step 5: Initialize Governance with managed contracts
                Console.WriteLine("Step 5: Initializing Governance...");
                await InitializeGovernance(govResult.ContractHash, tokenResult.ContractHash, nftResult.ContractHash);
                Console.WriteLine("✓ Governance initialized\n");

                // Display summary
                DisplayDeploymentSummary(results);
                
                return results;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\nDeployment failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deploy using a manifest file
        /// </summary>
        public async Task<DeploymentResults> DeployFromManifest(string manifestPath)
        {
            Console.WriteLine("=== Deploying from Manifest ===");
            Console.WriteLine($"Manifest: {manifestPath}\n");

            // Create deployment requests
            var requests = new List<ContractDeploymentRequest>
            {
                new ContractDeploymentRequest
                {
                    Name = "TokenContract",
                    SourcePath = "../../src/DeploymentExample.Contract/TokenContract.cs",
                    InitialParameters = new List<object> { await _toolkit.GetDeployerAccount() },
                    GasLimit = 100_00000000
                },
                new ContractDeploymentRequest
                {
                    Name = "NFTContract",
                    SourcePath = "../../src/DeploymentExample.Contract/NFTContract.cs",
                    Dependencies = new List<string> { "TokenContract" },
                    InjectDependencies = true,
                    InitialParameters = new List<object> 
                    { 
                        await _toolkit.GetDeployerAccount(),
                        null, // Token contract will be injected
                        10_00000000 // Mint price: 10 tokens
                    },
                    GasLimit = 100_00000000
                },
                new ContractDeploymentRequest
                {
                    Name = "GovernanceContract",
                    SourcePath = "../../src/DeploymentExample.Contract/GovernanceContract.cs",
                    Dependencies = new List<string> { "TokenContract" },
                    InjectDependencies = true,
                    InitialParameters = new List<object> 
                    { 
                        await _toolkit.GetDeployerAccount(),
                        null // Token contract will be injected
                    },
                    GasLimit = 100_00000000,
                    PostDeploymentActions = new List<PostDeploymentAction>
                    {
                        new PostDeploymentAction
                        {
                            Method = "createProposal",
                            Parameters = new List<object> 
                            { 
                                1, // ADD_CONTRACT type
                                "@NFTContract", // Reference to NFT contract
                                "Add NFT contract to governance"
                            },
                            Required = false
                        }
                    }
                }
            };

            // Deploy all contracts
            var deploymentOptions = new DeploymentOptions
            {
                Network = _toolkit.GetCurrentNetwork(),
                GasPrice = 1_00000000,
                WaitForConfirmation = true
            };

            var result = await _toolkit.DeployMultipleContracts(requests, deploymentOptions);
            
            // Convert to our results format
            var results = new DeploymentResults
            {
                TokenContract = result.DeployedContracts["TokenContract"],
                NFTContract = result.DeployedContracts["NFTContract"],
                GovernanceContract = result.DeployedContracts["GovernanceContract"]
            };

            DisplayDeploymentSummary(results);
            return results;
        }

        private async Task<ContractDeploymentInfo> DeployTokenContract(string deployerAddress)
        {
            var result = await _toolkit.Deploy(
                "../../src/DeploymentExample.Contract/TokenContract.cs",
                new object[] { deployerAddress } // Just owner, no governance yet
            );
            
            return result;
        }

        private async Task<ContractDeploymentInfo> DeployNFTContract(string deployerAddress, UInt160 tokenContract)
        {
            var result = await _toolkit.Deploy(
                "../../src/DeploymentExample.Contract/NFTContract.cs",
                new object[] 
                { 
                    deployerAddress,
                    tokenContract, // Token contract for payments
                    10_00000000 // Mint price: 10 tokens
                }
            );
            
            return result;
        }

        private async Task<ContractDeploymentInfo> DeployGovernanceContract(string deployerAddress, UInt160 tokenContract)
        {
            var result = await _toolkit.Deploy(
                "../../src/DeploymentExample.Contract/GovernanceContract.cs",
                new object[] 
                { 
                    deployerAddress,
                    tokenContract // Token contract for voting power
                }
            );
            
            return result;
        }

        private async Task ConfigureTokenContract(UInt160 tokenContract, UInt160 governanceContract)
        {
            // Set governance contract on token
            var txHash = await _toolkit.Invoke(
                tokenContract.ToString(),
                "setGovernance",
                governanceContract
            );
            
            Console.WriteLine($"  Set governance tx: {txHash}");
            await Task.Delay(5000); // Wait for confirmation
        }

        private async Task InitializeGovernance(UInt160 governanceContract, UInt160 tokenContract, UInt160 nftContract)
        {
            // Create proposal to add NFT contract to managed contracts
            var txHash = await _toolkit.Invoke(
                governanceContract.ToString(),
                "createProposal",
                (byte)1, // PROPOSAL_TYPE_ADD_CONTRACT
                nftContract,
                "Add NFT contract to governance management"
            );
            
            Console.WriteLine($"  Create proposal tx: {txHash}");
            await Task.Delay(5000); // Wait for confirmation
            
            // Get proposal ID (should be 1)
            var proposalId = new BigInteger(1);
            
            // Vote on the proposal
            txHash = await _toolkit.Invoke(
                governanceContract.ToString(),
                "vote",
                proposalId,
                true // Support
            );
            
            Console.WriteLine($"  Vote tx: {txHash}");
        }

        private void DisplayDeploymentSummary(DeploymentResults results)
        {
            Console.WriteLine("\n=== Deployment Summary ===");
            Console.WriteLine($"Token Contract:      {results.TokenContract.ContractHash}");
            Console.WriteLine($"  Transaction:       {results.TokenContract.TransactionHash}");
            Console.WriteLine($"  GAS Consumed:      {results.TokenContract.GasConsumed / 100_000_000m} GAS");
            
            Console.WriteLine($"\nNFT Contract:        {results.NFTContract.ContractHash}");
            Console.WriteLine($"  Transaction:       {results.NFTContract.TransactionHash}");
            Console.WriteLine($"  GAS Consumed:      {results.NFTContract.GasConsumed / 100_000_000m} GAS");
            
            Console.WriteLine($"\nGovernance Contract: {results.GovernanceContract.ContractHash}");
            Console.WriteLine($"  Transaction:       {results.GovernanceContract.TransactionHash}");
            Console.WriteLine($"  GAS Consumed:      {results.GovernanceContract.GasConsumed / 100_000_000m} GAS");
            
            var totalGas = (results.TokenContract.GasConsumed + 
                           results.NFTContract.GasConsumed + 
                           results.GovernanceContract.GasConsumed) / 100_000_000m;
            Console.WriteLine($"\nTotal GAS Consumed:  {totalGas} GAS");
        }
    }

    /// <summary>
    /// Results from multi-contract deployment
    /// </summary>
    public class DeploymentResults
    {
        public ContractDeploymentInfo TokenContract { get; set; }
        public ContractDeploymentInfo NFTContract { get; set; }
        public ContractDeploymentInfo GovernanceContract { get; set; }
    }
}