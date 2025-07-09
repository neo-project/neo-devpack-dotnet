using Neo;
using Neo.SmartContract.Deploy;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace DeploymentExample.Deploy
{
    /// <summary>
    /// Tests for already deployed contracts on the testnet
    /// </summary>
    public class TestDeployedContracts
    {
        // Contract addresses
        private static readonly UInt160 TokenContractAddress = UInt160.Parse("0xe5af8922400736cfac7337955dcd0e8d98f608cd");
        private static readonly UInt160 NFTContractAddress = UInt160.Parse("0xea414034cc25ddcc681ef6841a178ad4a0bc37e2");
        private static readonly UInt160 GovernanceContractAddress = UInt160.Parse("0x5d773713bcfb164d7f7f8e6877269341f9f6c2b1");
        
        // Account information
        private const string WifKey = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb";
        private const string AccountAddress = "NTmHjwiadq4g3VHpJ5FQigQcD4fF5m8TyX";
        
        public static async Task RunTestsAsync()
        {
            try
            {
                Console.WriteLine("=== Testing Deployed Contracts ===");
                Console.WriteLine($"Token Contract: {TokenContractAddress}");
                Console.WriteLine($"NFT Contract: {NFTContractAddress}");
                Console.WriteLine($"Governance Contract: {GovernanceContractAddress}");
                Console.WriteLine($"Account: {AccountAddress}");
                Console.WriteLine();
                
                // Create deployment toolkit
                var toolkit = new DeploymentToolkit();
                toolkit.SetNetwork("testnet");
                toolkit.SetWifKey(WifKey);
                
                // Verify account
                var deployerAddress = await toolkit.GetDeployerAccountAsync();
                Console.WriteLine($"Deployer Address: {deployerAddress}");
                
                // Check GAS balance
                var gasBalance = await toolkit.GetGasBalanceAsync();
                Console.WriteLine($"GAS Balance: {gasBalance} GAS");
                Console.WriteLine();
                
                // Test contracts
                await TestTokenContract(toolkit);
                await TestNFTContract(toolkit);
                await TestGovernanceContract(toolkit);
                await TestContractInteractions(toolkit);
                
                Console.WriteLine("\n✅ All tests completed!");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"\n❌ Test failed: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        
        private static async Task TestTokenContract(DeploymentToolkit toolkit)
        {
            Console.WriteLine("=== Testing Token Contract ===");
            
            try
            {
                // Check if contract is deployed by trying to call a method
                Console.WriteLine("1. Checking if contract is deployed...");
                try
                {
                    // Try to get the symbol - if this works, contract exists
                    var testSymbol = await toolkit.CallAsync<string>(TokenContractAddress.ToString(), "symbol");
                    Console.WriteLine("   ✓ Contract is deployed");
                }
                catch
                {
                    Console.WriteLine("   ❌ Contract not found at address");
                    return;
                }
                
                // Get token info
                Console.WriteLine("\n2. Getting token information...");
                var symbol = await toolkit.CallAsync<string>(TokenContractAddress.ToString(), "symbol");
                var decimals = await toolkit.CallAsync<BigInteger>(TokenContractAddress.ToString(), "decimals");
                var totalSupply = await toolkit.CallAsync<BigInteger>(TokenContractAddress.ToString(), "totalSupply");
                
                Console.WriteLine($"   Symbol: {symbol}");
                Console.WriteLine($"   Decimals: {decimals}");
                Console.WriteLine($"   Total Supply: {totalSupply / BigInteger.Pow(10, (int)decimals)} {symbol}");
                
                // Check owner
                Console.WriteLine("\n3. Checking contract owner...");
                try
                {
                    var owner = await toolkit.CallAsync<string>(TokenContractAddress.ToString(), "getOwner");
                    Console.WriteLine($"   Owner: {owner}");
                }
                catch
                {
                    Console.WriteLine("   ⚠️  No getOwner method found");
                }
                
                // Check balance
                Console.WriteLine("\n4. Checking account balance...");
                var balance = await toolkit.CallAsync<BigInteger>(
                    TokenContractAddress.ToString(), 
                    "balanceOf", 
                    AccountAddress
                );
                Console.WriteLine($"   Balance: {balance / BigInteger.Pow(10, (int)decimals)} {symbol}");
                
                // Try to initialize if not already initialized
                Console.WriteLine("\n5. Checking initialization status...");
                try
                {
                    var isInitialized = await toolkit.CallAsync<bool>(TokenContractAddress.ToString(), "isInitialized");
                    Console.WriteLine($"   Is Initialized: {isInitialized}");
                    
                    if (!isInitialized)
                    {
                        Console.WriteLine("   Attempting to initialize...");
                        var txHash = await toolkit.InvokeAsync(
                            TokenContractAddress.ToString(), 
                            "initialize",
                            AccountAddress,
                            new BigInteger(1000000) * BigInteger.Pow(10, (int)decimals) // 1M tokens
                        );
                        Console.WriteLine($"   ✓ Initialize transaction: {txHash}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ⚠️  Initialization check failed: {ex.Message}");
                }
                
                Console.WriteLine("\n✓ Token contract tests completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Token contract test failed: {ex.Message}");
            }
        }
        
        private static async Task TestNFTContract(DeploymentToolkit toolkit)
        {
            Console.WriteLine("\n=== Testing NFT Contract ===");
            
            try
            {
                // Check if contract is deployed by trying to call a method
                Console.WriteLine("1. Checking if contract is deployed...");
                try
                {
                    // Try to get the name - if this works, contract exists
                    var testName = await toolkit.CallAsync<string>(NFTContractAddress.ToString(), "name");
                    Console.WriteLine("   ✓ Contract is deployed");
                }
                catch
                {
                    Console.WriteLine("   ❌ Contract not found at address");
                    return;
                }
                
                // Get NFT info
                Console.WriteLine("\n2. Getting NFT information...");
                var symbol = await toolkit.CallAsync<string>(NFTContractAddress.ToString(), "symbol");
                var decimals = await toolkit.CallAsync<BigInteger>(NFTContractAddress.ToString(), "decimals");
                var totalSupply = await toolkit.CallAsync<BigInteger>(NFTContractAddress.ToString(), "totalSupply");
                
                Console.WriteLine($"   Symbol: {symbol}");
                Console.WriteLine($"   Decimals: {decimals}");
                Console.WriteLine($"   Total Supply: {totalSupply} NFTs");
                
                // Check owner
                Console.WriteLine("\n3. Checking contract owner...");
                try
                {
                    var owner = await toolkit.CallAsync<string>(NFTContractAddress.ToString(), "getOwner");
                    Console.WriteLine($"   Owner: {owner}");
                }
                catch
                {
                    Console.WriteLine("   ⚠️  No getOwner method found");
                }
                
                // Check mint price
                Console.WriteLine("\n4. Checking mint price...");
                try
                {
                    var mintPrice = await toolkit.CallAsync<BigInteger>(NFTContractAddress.ToString(), "getMintPrice");
                    Console.WriteLine($"   Mint Price: {mintPrice / BigInteger.Pow(10, 8)} tokens");
                }
                catch
                {
                    Console.WriteLine("   ⚠️  No getMintPrice method found");
                }
                
                // Check initialization
                Console.WriteLine("\n5. Checking initialization status...");
                try
                {
                    var isInitialized = await toolkit.CallAsync<bool>(NFTContractAddress.ToString(), "isInitialized");
                    Console.WriteLine($"   Is Initialized: {isInitialized}");
                    
                    if (!isInitialized)
                    {
                        Console.WriteLine("   Attempting to initialize...");
                        var txHash = await toolkit.InvokeAsync(
                            NFTContractAddress.ToString(), 
                            "initialize",
                            AccountAddress,
                            TokenContractAddress,
                            new BigInteger(10) * BigInteger.Pow(10, 8) // 10 tokens per NFT
                        );
                        Console.WriteLine($"   ✓ Initialize transaction: {txHash}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ⚠️  Initialization check failed: {ex.Message}");
                }
                
                // Check owned tokens
                Console.WriteLine("\n6. Checking owned tokens...");
                try
                {
                    var tokensOf = await toolkit.CallAsync<object[]>(NFTContractAddress.ToString(), "tokensOf", AccountAddress);
                    Console.WriteLine($"   Owned NFTs: {tokensOf?.Length ?? 0}");
                }
                catch
                {
                    Console.WriteLine("   ⚠️  Could not check owned tokens");
                }
                
                Console.WriteLine("\n✓ NFT contract tests completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ NFT contract test failed: {ex.Message}");
            }
        }
        
        private static async Task TestGovernanceContract(DeploymentToolkit toolkit)
        {
            Console.WriteLine("\n=== Testing Governance Contract ===");
            
            try
            {
                // Check if contract is deployed by trying to call a method
                Console.WriteLine("1. Checking if contract is deployed...");
                try
                {
                    // Try to get the owner - if this works, contract exists
                    var testOwner = await toolkit.CallAsync<string>(GovernanceContractAddress.ToString(), "getOwner");
                    Console.WriteLine("   ✓ Contract is deployed");
                }
                catch
                {
                    Console.WriteLine("   ❌ Contract not found at address");
                    return;
                }
                
                // Check council membership
                Console.WriteLine("\n2. Checking council membership...");
                var isCouncilMember = await toolkit.CallAsync<bool>(
                    GovernanceContractAddress.ToString(),
                    "isCouncilMember",
                    AccountAddress
                );
                Console.WriteLine($"   Is Council Member: {isCouncilMember}");
                
                // Get proposal count
                Console.WriteLine("\n3. Getting proposal count...");
                var proposalCount = await toolkit.CallAsync<BigInteger>(
                    GovernanceContractAddress.ToString(),
                    "getProposalCount"
                );
                Console.WriteLine($"   Total Proposals: {proposalCount}");
                
                // Check initialization
                Console.WriteLine("\n4. Checking initialization status...");
                try
                {
                    var isInitialized = await toolkit.CallAsync<bool>(GovernanceContractAddress.ToString(), "isInitialized");
                    Console.WriteLine($"   Is Initialized: {isInitialized}");
                    
                    if (!isInitialized)
                    {
                        Console.WriteLine("   Attempting to initialize...");
                        var txHash = await toolkit.InvokeAsync(
                            GovernanceContractAddress.ToString(), 
                            "initialize",
                            new object[] { AccountAddress }, // Initial council members
                            NFTContractAddress
                        );
                        Console.WriteLine($"   ✓ Initialize transaction: {txHash}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ⚠️  Initialization check failed: {ex.Message}");
                }
                
                // Test proposal creation if council member
                if (isCouncilMember)
                {
                    Console.WriteLine("\n5. Testing proposal creation...");
                    try
                    {
                        var txHash = await toolkit.InvokeAsync(
                            GovernanceContractAddress.ToString(),
                            "createProposal",
                            1, // Mint tokens proposal type
                            new BigInteger(1000) * BigInteger.Pow(10, 8), // 1000 tokens
                            "Test proposal for minting tokens"
                        );
                        Console.WriteLine($"   ✓ Proposal created: {txHash}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   ⚠️  Proposal creation failed: {ex.Message}");
                    }
                }
                
                Console.WriteLine("\n✓ Governance contract tests completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Governance contract test failed: {ex.Message}");
            }
        }
        
        private static async Task TestContractInteractions(DeploymentToolkit toolkit)
        {
            Console.WriteLine("\n=== Testing Contract Interactions ===");
            
            try
            {
                // Test token approval for NFT contract
                Console.WriteLine("1. Testing token approval for NFT minting...");
                try
                {
                    var currentAllowance = await toolkit.CallAsync<BigInteger>(
                        TokenContractAddress.ToString(),
                        "allowance",
                        AccountAddress,
                        NFTContractAddress
                    );
                    Console.WriteLine($"   Current Allowance: {currentAllowance / BigInteger.Pow(10, 8)} tokens");
                    
                    if (currentAllowance == 0)
                    {
                        Console.WriteLine("   Setting approval for 100 tokens...");
                        var txHash = await toolkit.InvokeAsync(
                            TokenContractAddress.ToString(),
                            "approve",
                            NFTContractAddress,
                            new BigInteger(100) * BigInteger.Pow(10, 8) // 100 tokens
                        );
                        Console.WriteLine($"   ✓ Approval transaction: {txHash}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ⚠️  Token approval failed: {ex.Message}");
                }
                
                // Test NFT minting
                Console.WriteLine("\n2. Testing NFT minting...");
                try
                {
                    var txHash = await toolkit.InvokeAsync(
                        NFTContractAddress.ToString(),
                        "mint",
                        "https://example.com/nft/test.json",
                        new object[] { 
                            new object[] { "name", "Test NFT" },
                            new object[] { "description", "A test NFT minted from TestDeployedContracts" }
                        }
                    );
                    Console.WriteLine($"   ✓ NFT minting transaction: {txHash}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ⚠️  NFT minting failed: {ex.Message}");
                }
                
                // Test governance voting
                Console.WriteLine("\n3. Testing governance voting...");
                try
                {
                    var proposalCount = await toolkit.CallAsync<BigInteger>(
                        GovernanceContractAddress.ToString(),
                        "getProposalCount"
                    );
                    
                    if (proposalCount > 0)
                    {
                        var latestProposalId = proposalCount - 1;
                        Console.WriteLine($"   Voting on proposal {latestProposalId}...");
                        
                        var voteTx = await toolkit.InvokeAsync(
                            GovernanceContractAddress.ToString(),
                            "vote",
                            latestProposalId,
                            true // Vote yes
                        );
                        Console.WriteLine($"   ✓ Vote transaction: {voteTx}");
                    }
                    else
                    {
                        Console.WriteLine("   ⚠️  No proposals to vote on");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ⚠️  Voting failed: {ex.Message}");
                }
                
                Console.WriteLine("\n✓ Contract interaction tests completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Contract interaction test failed: {ex.Message}");
            }
        }
    }
}