using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Testing;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Integration;

/// <summary>
/// Integration tests for the complete deployment workflow using Neo Testing framework
/// </summary>
public class DeploymentIntegrationTests : TestBase
{
    [Fact]
    public async Task FullDeploymentWorkflow_ShouldDeployAndInvokeContract()
    {
        // Arrange
        var toolkit = CreateToolkit();
        var contractPath = CreateTestContract();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        // Create test wallet
        var testWalletPath = Path.Combine(outputDir, "test.wallet.json");
        await CreateTestWalletFile(testWalletPath);

        // Set up compilation options
        var compilationOptions = new CompilationOptions
        {
            SourcePath = contractPath,
            OutputDirectory = outputDir,
            ContractName = "TestContract",
            GenerateDebugInfo = true,
            Optimize = true
        };

        // Set up deployment options
        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688"), // Example account
            GasLimit = 50_000_000,
            WaitForConfirmation = false // Skip confirmation in tests
        };

        try
        {
            // Load test wallet
            await toolkit.LoadWalletAsync(testWalletPath, "test");
            var deployerAccount = toolkit.GetDeployerAccount();
            Assert.NotNull(deployerAccount);

            // Step 1: Compile and deploy the contract
            var deploymentResult = await toolkit.CompileAndDeployAsync(compilationOptions, deploymentOptions);

            // Assert deployment succeeded
            Assert.NotNull(deploymentResult);
            Assert.True(deploymentResult.Success);
            Assert.NotEqual(UInt160.Zero, deploymentResult.ContractHash);
            Assert.NotEqual(UInt256.Zero, deploymentResult.TransactionHash);

            // Step 2: Test contract invocation - call a read-only method
            var getValue = await toolkit.CallContractAsync<int>(
                deploymentResult.ContractHash, 
                "getValue");
            
            Assert.Equal(42, getValue);

            // Step 3: Test contract invocation - call method with parameters
            var testResult = await toolkit.CallContractAsync<string>(
                deploymentResult.ContractHash, 
                "testMethod", 
                "World");
            
            Assert.Equal("Hello World", testResult);

            // Step 4: Send a transaction to invoke a method
            var txHash = await toolkit.InvokeContractAsync(
                deploymentResult.ContractHash,
                "testMethod",
                "Neo");

            Assert.NotEqual(UInt256.Zero, txHash);

            // Step 5: Verify contract exists
            var contractExists = await toolkit.ContractExistsAsync(deploymentResult.ContractHash);
            Assert.True(contractExists);
        }
        finally
        {
            // Cleanup
            Directory.Delete(Path.GetDirectoryName(contractPath)!, true);
            Directory.Delete(outputDir, true);
        }
    }

    [Fact]
    public async Task ArtifactBasedDeployment_ShouldWork()
    {
        // Arrange
        var toolkit = CreateToolkit();
        var contractPath = CreateTestContract();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        var testWalletPath = Path.Combine(outputDir, "test.wallet.json");
        await CreateTestWalletFile(testWalletPath);

        // First compile to get artifacts
        var compilationOptions = new CompilationOptions
        {
            SourcePath = contractPath,
            OutputDirectory = outputDir,
            ContractName = "TestContract"
        };

        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688"), // Example account
            GasLimit = 50_000_000,
            WaitForConfirmation = false
        };

        try
        {
            await toolkit.LoadWalletAsync(testWalletPath, "test");

            // Step 1: Compile first to get artifacts
            var compilerService = new Neo.SmartContract.Deploy.Services.ContractCompilerService(
                LoggerFactory.CreateLogger<Neo.SmartContract.Deploy.Services.ContractCompilerService>());
            
            var compiledContract = await compilerService.CompileAsync(compilationOptions);

            // Step 2: Deploy from artifacts
            var deploymentResult = await toolkit.DeployFromArtifactsAsync(
                compiledContract.NefFilePath,
                compiledContract.ManifestFilePath,
                deploymentOptions);

            // Assert
            Assert.NotNull(deploymentResult);
            Assert.True(deploymentResult.Success);
            Assert.NotEqual(UInt160.Zero, deploymentResult.ContractHash);

            // Verify deployed contract works
            var getValue = await toolkit.CallContractAsync<int>(
                deploymentResult.ContractHash, 
                "getValue");
            
            Assert.Equal(42, getValue);
        }
        finally
        {
            // Cleanup
            Directory.Delete(Path.GetDirectoryName(contractPath)!, true);
            Directory.Delete(outputDir, true);
        }
    }

    [Fact]
    public async Task ContractUpdate_ShouldWork()
    {
        // Arrange
        var toolkit = CreateToolkit();
        var originalContractPath = CreateTestContract();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        var testWalletPath = Path.Combine(outputDir, "test.wallet.json");
        await CreateTestWalletFile(testWalletPath);

        var compilationOptions = new CompilationOptions
        {
            SourcePath = originalContractPath,
            OutputDirectory = outputDir,
            ContractName = "TestContract"
        };

        var deploymentOptions = new DeploymentOptions
        {
            DeployerAccount = UInt160.Parse("0xb1983fa2021e0c36e5e37c2771b8bb7b5c525688"), // Example account
            GasLimit = 50_000_000,
            WaitForConfirmation = false
        };

        try
        {
            await toolkit.LoadWalletAsync(testWalletPath, "test");

            // Step 1: Deploy original contract
            var originalDeployment = await toolkit.CompileAndDeployAsync(compilationOptions, deploymentOptions);
            Assert.True(originalDeployment.Success);

            // Step 2: Create updated contract
            var updatedContractPath = CreateUpdatedTestContract();
            var updateCompilationOptions = new CompilationOptions
            {
                SourcePath = updatedContractPath,
                OutputDirectory = outputDir,
                ContractName = "TestContract"
            };

            // Step 3: Update the contract
            var updateResult = await toolkit.UpdateContractAsync(
                originalDeployment.ContractHash,
                updateCompilationOptions,
                deploymentOptions);

            // Assert update succeeded
            Assert.NotNull(updateResult);
            Assert.True(updateResult.Success);
            Assert.Equal(originalDeployment.ContractHash, updateResult.ContractHash); // Same hash

            // Step 4: Verify updated contract behavior
            var newValue = await toolkit.CallContractAsync<int>(
                updateResult.ContractHash, 
                "getValue");
            
            Assert.Equal(100, newValue); // Updated return value

            // Cleanup updated contract
            Directory.Delete(Path.GetDirectoryName(updatedContractPath)!, true);
        }
        finally
        {
            // Cleanup
            Directory.Delete(Path.GetDirectoryName(originalContractPath)!, true);
            Directory.Delete(outputDir, true);
        }
    }

    private string CreateUpdatedTestContract()
    {
        // Create an updated version of the test contract with different return value
        var contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace TestContract
{
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Description"", ""Updated Test Contract"")]
    [ManifestExtra(""Version"", ""2.0.0"")]
    public class TestContract : SmartContract
    {
        [DisplayName(""TestMethod"")]
        public static string TestMethod(string input)
        {
            return ""Hello "" + input + "" (Updated)"";
        }

        [DisplayName(""GetValue"")]
        public static int GetValue()
        {
            return 100; // Updated value
        }

        public static void _deploy(object data, bool update)
        {
            if (update)
            {
                // Migration logic for updates
                Storage.Put(Storage.CurrentContext, ""version"", ""2.0"");
            }
            else
            {
                Storage.Put(Storage.CurrentContext, ""initialized"", true);
            }
        }
    }
}";

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var contractPath = Path.Combine(tempDir, "TestContract.cs");
        File.WriteAllText(contractPath, contractCode);
        return contractPath;
    }

    private async Task CreateTestWalletFile(string walletPath)
    {
        // Create a simple test wallet
        var walletJson = @"{
  ""name"": ""test-wallet"",
  ""version"": ""1.0"",
  ""scrypt"": {
    ""n"": 16384,
    ""r"": 8,
    ""p"": 8
  },
  ""accounts"": [
    {
      ""address"": ""NiNmXL8FjEUEs1nfX9uHFBNaenxDHJtmuB"",
      ""label"": ""test-account"",
      ""isDefault"": true,
      ""lock"": false,
      ""key"": ""6PYL2NWjJRudDyQE7xD99vPkgDDQ4jiqPF6LVyeCbnYUAe8DhCvKp6vL3C"",
      ""contract"": {
        ""script"": ""DCEDeK2z93hJM8m7kM7BpRJGK4tO6cMVZZkXnxRm6aJ8lBsLQZVEDXg="",
        ""parameters"": [
          {
            ""name"": ""signature"",
            ""type"": ""Signature""
          }
        ],
        ""deployed"": false
      },
      ""extra"": null
    }
  ],
  ""extra"": null
}";
        await File.WriteAllTextAsync(walletPath, walletJson);
    }
}