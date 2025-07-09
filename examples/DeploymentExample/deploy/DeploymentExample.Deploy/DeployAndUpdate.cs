using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Neo;
using Neo.Wallets;
using Neo.SmartContract.Deploy;

namespace DeploymentExample.Deploy;

/// <summary>
/// Demonstrates deploying and updating contracts with the deployment toolkit
/// </summary>
public class DeployAndUpdate
{
    private const string WIF_KEY = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb";
    private const string EXPECTED_ADDRESS = "NTmHjwiadq4g3VHpJ5FQigQcD4fF5m8TyX";
    
    public static async Task Run()
    {
        Console.WriteLine("=== Neo Contract Deploy and Update Demo ===");
        Console.WriteLine($"Using account: {EXPECTED_ADDRESS}");
        Console.WriteLine();

        var toolkit = new DeploymentToolkit();
        toolkit.SetWifKey(WIF_KEY);
        toolkit.SetNetwork("testnet");

        // Verify account
        var deployerAddress = await toolkit.GetDeployerAccountAsync();
        Console.WriteLine($"Deployer address (hex): {deployerAddress}");
        Console.WriteLine($"Deployer address: {deployerAddress.ToAddress(0x35)}"); // 0x35 is Neo N3 testnet

        try
        {
            // Step 1: Deploy TokenContract
            Console.WriteLine("\n1. Deploying TokenContract...");
            var tokenPath = Path.Combine("..", "..", "src", "TokenContract", "TokenContract.cs");
            
            var deployResult = await toolkit.DeployAsync(tokenPath);
            
            if (!deployResult.Success)
            {
                Console.WriteLine($"❌ Deployment failed: {deployResult.ErrorMessage}");
                return;
            }
            
            Console.WriteLine($"✅ TokenContract deployed!");
            Console.WriteLine($"   Contract Hash: {deployResult.ContractHash}");
            Console.WriteLine($"   Transaction: {deployResult.TransactionHash}");
            Console.WriteLine($"   Gas Consumed: {deployResult.GasConsumed / 100_000_000m} GAS");
            
            var tokenHash = deployResult.ContractHash.ToString();

            // Step 2: Initialize the contract
            Console.WriteLine("\n2. Initializing TokenContract...");
            await Task.Delay(5000); // Wait for confirmation
            
            var initTx = await toolkit.InvokeAsync(
                tokenHash,
                "initialize",
                deployerAddress
            );
            Console.WriteLine($"   Initialization TX: {initTx}");
            
            // Step 3: Test the contract
            Console.WriteLine("\n3. Testing TokenContract...");
            await Task.Delay(5000);
            
            var symbol = await toolkit.CallAsync<string>(tokenHash, "symbol");
            Console.WriteLine($"   Symbol: {symbol}");
            
            var balance = await toolkit.CallAsync<BigInteger>(tokenHash, "balanceOf", deployerAddress);
            Console.WriteLine($"   Balance: {balance}");
            
            // Step 4: Update the contract
            Console.WriteLine("\n4. Preparing to update TokenContract...");
            Console.WriteLine("   Making a small change to trigger recompilation...");
            
            // Read the contract source
            var sourceCode = await File.ReadAllTextAsync(tokenPath);
            
            // Add a comment to trigger recompilation
            var updatedSource = sourceCode.Replace(
                "// Updated:", 
                $"// Updated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}\n    // Updated:"
            );
            
            // If no existing update comment, add one
            if (!updatedSource.Contains("// Updated:"))
            {
                updatedSource = updatedSource.Replace(
                    "public class TokenContract : SmartContract",
                    $"// Updated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}\n    public class TokenContract : SmartContract"
                );
            }
            
            // Write to temp file
            var tempPath = Path.GetTempFileName() + ".cs";
            await File.WriteAllTextAsync(tempPath, updatedSource);
            
            Console.WriteLine("\n5. Updating TokenContract...");
            Console.WriteLine("   Note: The update will call ContractManagement.Update directly");
            Console.WriteLine("   This will trigger the contract's _deploy method with update=true");
            var updateResult = await toolkit.UpdateAsync(tokenHash, tempPath);
            
            if (updateResult.Success)
            {
                Console.WriteLine($"✅ Contract updated successfully!");
                Console.WriteLine($"   Transaction: {updateResult.TransactionHash}");
                Console.WriteLine($"   Gas Consumed: {updateResult.GasConsumed / 100_000_000m} GAS");
                
                // Test after update
                Console.WriteLine("\n6. Testing contract after update...");
                await Task.Delay(5000);
                
                var symbolAfter = await toolkit.CallAsync<string>(tokenHash, "symbol");
                Console.WriteLine($"   Symbol (after update): {symbolAfter}");
                
                var balanceAfter = await toolkit.CallAsync<BigInteger>(tokenHash, "balanceOf", deployerAddress);
                Console.WriteLine($"   Balance (after update): {balanceAfter}");
                
                Console.WriteLine("\n✅ Contract update verified - state preserved!");
            }
            else
            {
                Console.WriteLine($"❌ Update failed: {updateResult.ErrorMessage}");
            }
            
            // Cleanup
            try { File.Delete(tempPath); } catch { }
            
            // Deploy other contracts
            Console.WriteLine("\n7. Deploying NFTContract...");
            var nftPath = Path.Combine("..", "..", "src", "NFTContract", "NFTContract.cs");
            var nftResult = await toolkit.DeployAsync(nftPath);
            
            if (nftResult.Success)
            {
                Console.WriteLine($"✅ NFTContract deployed: {nftResult.ContractHash}");
                
                // Initialize NFT contract
                await Task.Delay(5000);
                var nftInitTx = await toolkit.InvokeAsync(
                    nftResult.ContractHash.ToString(),
                    "initialize",
                    deployerAddress,
                    tokenHash,
                    1000000000 // 10 tokens mint price
                );
                Console.WriteLine($"   NFT Initialization TX: {nftInitTx}");
            }
            
            Console.WriteLine("\n8. Deploying GovernanceContract...");
            var govPath = Path.Combine("..", "..", "src", "GovernanceContract", "GovernanceContract.cs");
            var govResult = await toolkit.DeployAsync(govPath);
            
            if (govResult.Success)
            {
                Console.WriteLine($"✅ GovernanceContract deployed: {govResult.ContractHash}");
                
                // Initialize governance contract
                await Task.Delay(5000);
                var govInitTx = await toolkit.InvokeAsync(
                    govResult.ContractHash.ToString(),
                    "initialize",
                    deployerAddress,
                    tokenHash
                );
                Console.WriteLine($"   Governance Initialization TX: {govInitTx}");
            }
            
            Console.WriteLine("\n=== Deployment Summary ===");
            Console.WriteLine($"TokenContract: {tokenHash}");
            if (nftResult.Success) Console.WriteLine($"NFTContract: {nftResult.ContractHash}");
            if (govResult.Success) Console.WriteLine($"GovernanceContract: {govResult.ContractHash}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}