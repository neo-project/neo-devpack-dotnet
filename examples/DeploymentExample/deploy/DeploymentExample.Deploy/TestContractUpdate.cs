using System;
using System.IO;
using System.Threading.Tasks;
using Neo;
using Neo.SmartContract.Deploy;

namespace DeploymentExample.Deploy;

/// <summary>
/// Test contract update with the _deploy pattern
/// </summary>
public class TestContractUpdate
{
    private const string WIF_KEY = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb";
    private const string TOKEN_CONTRACT_HASH = "0xe5af8922400736cfac7337955dcd0e8d98f608cd";
    
    public static async Task RunUpdateTest()
    {
        Console.WriteLine("=== Testing Contract Update with _deploy Pattern ===");
        
        var toolkit = new DeploymentToolkit();
        toolkit.SetWifKey(WIF_KEY);
        toolkit.SetNetwork("testnet");
        
        try
        {
            // First, verify the contract exists and get its current state
            Console.WriteLine("\n1. Checking current contract state...");
            var symbol = await toolkit.CallAsync<string>(TOKEN_CONTRACT_HASH, "symbol");
            Console.WriteLine($"   Current Symbol: {symbol}");
            
            var totalSupply = await toolkit.CallAsync<System.Numerics.BigInteger>(TOKEN_CONTRACT_HASH, "totalSupply");
            Console.WriteLine($"   Current Total Supply: {totalSupply}");
            
            // Initialize the contract if needed
            Console.WriteLine("\n2. Initializing contract (if not already initialized)...");
            var deployerAddress = await toolkit.GetDeployerAccountAsync();
            
            try
            {
                var initTx = await toolkit.InvokeAsync(
                    TOKEN_CONTRACT_HASH, 
                    "initialize",
                    deployerAddress
                );
                Console.WriteLine($"   Initialization TX: {initTx}");
                await Task.Delay(15000); // Wait for confirmation
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Initialization skipped: {ex.Message}");
            }
            
            // Now test the update
            Console.WriteLine("\n3. Attempting to update the contract...");
            Console.WriteLine("   This will call ContractManagement.Update");
            Console.WriteLine("   Which triggers the contract's _deploy method with update=true");
            
            // Read the original contract source
            var contractPath = Path.Combine("..", "..", "src", "TokenContract", "TokenContract.cs");
            var sourceCode = await File.ReadAllTextAsync(contractPath);
            
            // Make a minimal change to trigger recompilation
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            var updatedSource = sourceCode;
            
            // Add or update a comment to force recompilation
            if (updatedSource.Contains("// Last updated:"))
            {
                updatedSource = System.Text.RegularExpressions.Regex.Replace(
                    updatedSource,
                    @"// Last updated: .*",
                    $"// Last updated: {timestamp}"
                );
            }
            else
            {
                updatedSource = updatedSource.Replace(
                    "namespace TokenContract",
                    $"// Last updated: {timestamp}\nnamespace TokenContract"
                );
            }
            
            // Save to temp file
            var tempPath = Path.GetTempFileName() + ".cs";
            await File.WriteAllTextAsync(tempPath, updatedSource);
            
            Console.WriteLine($"   Updating contract at {TOKEN_CONTRACT_HASH}...");
            
            var updateResult = await toolkit.UpdateAsync(TOKEN_CONTRACT_HASH, tempPath);
            
            if (updateResult.Success)
            {
                Console.WriteLine($"\n✅ Contract update successful!");
                Console.WriteLine($"   Transaction: {updateResult.TransactionHash}");
                Console.WriteLine($"   Gas Consumed: {updateResult.GasConsumed / 100_000_000m} GAS");
                
                // Wait for confirmation
                Console.WriteLine("\n4. Waiting for confirmation...");
                await Task.Delay(15000);
                
                // Verify the update
                Console.WriteLine("\n5. Verifying contract after update...");
                var symbolAfter = await toolkit.CallAsync<string>(TOKEN_CONTRACT_HASH, "symbol");
                Console.WriteLine($"   Symbol after update: {symbolAfter}");
                
                var totalSupplyAfter = await toolkit.CallAsync<System.Numerics.BigInteger>(TOKEN_CONTRACT_HASH, "totalSupply");
                Console.WriteLine($"   Total Supply after update: {totalSupplyAfter}");
                
                if (symbol == symbolAfter && totalSupply == totalSupplyAfter)
                {
                    Console.WriteLine("\n✅ Contract state preserved after update!");
                }
            }
            else
            {
                Console.WriteLine($"\n❌ Contract update failed: {updateResult.ErrorMessage}");
                
                if (updateResult.ErrorMessage?.Contains("Only owner can update") == true)
                {
                    Console.WriteLine("\n💡 This error is expected if:");
                    Console.WriteLine("   - The contract's _deploy method checks for owner authorization");
                    Console.WriteLine("   - We're not using the original deployer's key");
                    Console.WriteLine("   - The contract hasn't been initialized with our account as owner");
                }
            }
            
            // Cleanup
            try { File.Delete(tempPath); } catch { }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}