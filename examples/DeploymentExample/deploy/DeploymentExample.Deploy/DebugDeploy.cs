using System;
using System.IO;
using System.Threading.Tasks;
using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Manifest;
using Neo.SmartContract;
using Neo.VM;

namespace DeploymentExample.Deploy
{
    class DebugDeploy
    {
        public static async Task DebugDeployment()
        {
            var toolkit = new DeploymentToolkit();
            toolkit.SetNetwork("testnet");
            toolkit.SetWifKey("KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb");

            Console.WriteLine("=== Debug Deployment ===");
            
            // Compile the simple contract
            var contractPath = "../../src/DeploymentExample.Contract/SimpleContract.cs";
            
            // Create minimal contract without deployment
            var minimalContract = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

namespace Test
{
    [DisplayName(""Test.MinimalContract"")]
    [ManifestExtra(""Author"", ""Test"")]
    [ContractPermission(""*"", ""*"")]
    public class MinimalContract : SmartContract
    {
        [DisplayName(""test"")]
        [Safe]
        public static string Test()
        {
            return ""Hello"";
        }
    }
}";

            // Write minimal contract
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var tempFile = Path.Combine(tempDir, "MinimalContract.cs");
            await File.WriteAllTextAsync(tempFile, minimalContract);
            
            try
            {
                Console.WriteLine($"Deploying minimal contract from: {tempFile}");
                var result = await toolkit.DeployAsync(tempFile);
                
                if (result.Success)
                {
                    Console.WriteLine($"✅ Success! Contract deployed at: {result.ContractHash}");
                    Console.WriteLine($"   Transaction: {result.TransactionHash}");
                    
                    // Test the contract
                    await Task.Delay(15000);
                    var testResult = await toolkit.CallAsync<string>(result.ContractHash.ToString(), "test");
                    Console.WriteLine($"   Test result: {testResult}");
                }
                else
                {
                    Console.WriteLine($"❌ Deployment failed: {result.ErrorMessage}");
                }
            }
            finally
            {
                // Cleanup
                Directory.Delete(tempDir, true);
            }
        }
    }
}