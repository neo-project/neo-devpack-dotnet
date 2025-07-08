using System;
using System.IO;
using System.Threading.Tasks;
using Neo;
using Neo.SmartContract.Deploy;

namespace DeploymentExample.Deploy;

/// <summary>
/// Demonstrates contract deployment and update functionality
/// </summary>
public class UpdateDemo
{
    public static async Task RunDemo()
    {
        Console.WriteLine("=== Contract Update Demo ===");
        Console.WriteLine("This demo shows how to deploy and update a contract with update functionality.");
        Console.WriteLine();

        // Create deployment toolkit
        var toolkit = new DeploymentToolkit();
        
        // Use the WIF key for deployment
        var wifKey = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb";
        toolkit.SetWifKey(wifKey);
        toolkit.SetNetwork("testnet"); // Use testnet for demo

        var deployerAddress = await toolkit.GetDeployerAccountAsync();
        Console.WriteLine($"Deployer: {deployerAddress}");

        try
        {
            // Step 1: Deploy the Token contract with update functionality
            Console.WriteLine("\n1. Deploying TokenContract with update method...");
            
            var contractPath = Path.Combine("..", "..", "src", "TokenContract", "TokenContract.cs");
            var deployResult = await toolkit.DeployAsync(contractPath);
            
            if (deployResult.Success)
            {
                Console.WriteLine($"✅ Contract deployed successfully!");
                Console.WriteLine($"   Contract Hash: {deployResult.ContractHash}");
                Console.WriteLine($"   Transaction: {deployResult.TransactionHash}");
                
                // Initialize the contract
                Console.WriteLine("\n2. Initializing contract...");
                var initTx = await toolkit.InvokeAsync(
                    deployResult.ContractHash.ToString(),
                    "initialize",
                    deployerAddress
                );
                Console.WriteLine($"   Initialization TX: {initTx}");
                
                // Wait a moment
                await Task.Delay(5000);
                
                // Check the contract works
                Console.WriteLine("\n3. Testing contract functionality...");
                var symbol = await toolkit.CallAsync<string>(
                    deployResult.ContractHash.ToString(),
                    "symbol"
                );
                Console.WriteLine($"   Token Symbol: {symbol}");
                
                try 
                {
                    // getOwner returns UInt160, not string
                    var ownerBytes = await toolkit.CallAsync<byte[]>(
                        deployResult.ContractHash.ToString(),
                        "getOwner"
                    );
                    var ownerHash = new UInt160(ownerBytes);
                    Console.WriteLine($"   Contract Owner: {ownerHash}");
                }
                catch (Exception)
                {
                    // If not initialized, it might be zero
                    Console.WriteLine($"   Contract Owner: (not yet initialized)");
                }
                
                // Step 2: Update the contract
                Console.WriteLine("\n4. Updating contract...");
                Console.WriteLine("   Adding version comment to trigger recompilation...");
                
                // Read the contract source
                var sourceCode = await File.ReadAllTextAsync(contractPath);
                
                // Add a version comment to trigger recompilation
                var updatedSource = sourceCode.Replace(
                    "public class TokenContract : SmartContract",
                    "// Updated: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "\n    public class TokenContract : SmartContract"
                );
                
                // Write the updated source to a temp file
                var tempPath = Path.GetTempFileName() + ".cs";
                await File.WriteAllTextAsync(tempPath, updatedSource);
                
                try
                {
                    // Deploy the update
                    var updateResult = await toolkit.UpdateAsync(
                        deployResult.ContractHash.ToString(),
                        tempPath
                    );
                    
                    if (updateResult.Success)
                    {
                        Console.WriteLine($"✅ Contract updated successfully!");
                        Console.WriteLine($"   Update TX: {updateResult.TransactionHash}");
                        
                        // Verify the contract still works
                        Console.WriteLine("\n5. Verifying updated contract...");
                        
                        await Task.Delay(5000);
                        
                        var symbolAfter = await toolkit.CallAsync<string>(
                            deployResult.ContractHash.ToString(),
                            "symbol"
                        );
                        Console.WriteLine($"   Token Symbol (after update): {symbolAfter}");
                        
                        try
                        {
                            var ownerBytesAfter = await toolkit.CallAsync<byte[]>(
                                deployResult.ContractHash.ToString(),
                                "getOwner"
                            );
                            var ownerHashAfter = new UInt160(ownerBytesAfter);
                            Console.WriteLine($"   Contract Owner (after update): {ownerHashAfter}");
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"   Contract Owner (after update): (not initialized)");
                        }
                        
                        Console.WriteLine("\n✅ Update demo completed successfully!");
                        Console.WriteLine("   The contract was deployed with update functionality");
                        Console.WriteLine("   and successfully updated while maintaining its state.");
                    }
                    else
                    {
                        Console.WriteLine($"❌ Contract update failed: {updateResult.ErrorMessage}");
                    }
                }
                finally
                {
                    // Clean up temp file
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);
                }
            }
            else
            {
                Console.WriteLine($"❌ Contract deployment failed: {deployResult.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Demo failed: {ex.Message}");
            Console.WriteLine($"   Stack trace: {ex.StackTrace}");
        }
    }
}