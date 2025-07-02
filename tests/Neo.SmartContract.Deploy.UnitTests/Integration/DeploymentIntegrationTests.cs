using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Testing;
using System;
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
        var projectPath = CreateTestContractProject();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        // Create test wallet
        var testWalletPath = Path.Combine(outputDir, "test.wallet.json");
        await CreateTestWalletFile(testWalletPath);

        // Set up compilation options for project compilation
        var compilationOptions = new CompilationOptions
        {
            ProjectPath = projectPath,
            OutputDirectory = outputDir,
            ContractName = "TestContract",
            GenerateDebugInfo = true,
            Optimize = true
        };

        try
        {
            // Load test wallet first
            await toolkit.LoadWalletAsync(testWalletPath, "123456");
            var deployerAccount = toolkit.GetDeployerAccount();

            // Set up deployment options after loading wallet
            var deploymentOptions = new DeploymentOptions
            {
                DeployerAccount = deployerAccount,
                GasLimit = 50_000_000,
                WaitForConfirmation = false // Skip confirmation in tests
            };
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
            try
            {
                // Clean up the project directory (go up two levels from .csproj file)
                var projectDir = new DirectoryInfo(Path.GetDirectoryName(projectPath)!).Parent?.FullName;
                if (projectDir != null && Directory.Exists(projectDir))
                    Directory.Delete(projectDir, true);
            }
            catch { /* Ignore cleanup errors */ }
            
            try
            {
                if (Directory.Exists(outputDir))
                    Directory.Delete(outputDir, true);
            }
            catch { /* Ignore cleanup errors */ }
        }
    }

    [Fact]
    public async Task ArtifactBasedDeployment_ShouldWork()
    {
        // Arrange
        var toolkit = CreateToolkit();
        var projectPath = CreateTestContractProject();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        var testWalletPath = Path.Combine(outputDir, "test.wallet.json");
        await CreateTestWalletFile(testWalletPath);

        // First compile to get artifacts
        var compilationOptions = new CompilationOptions
        {
            ProjectPath = projectPath,
            OutputDirectory = outputDir,
            ContractName = "TestContract"
        };

        try
        {
            await toolkit.LoadWalletAsync(testWalletPath, "123456");

            var deploymentOptions = new DeploymentOptions
            {
                DeployerAccount = toolkit.GetDeployerAccount(),
                GasLimit = 50_000_000,
                WaitForConfirmation = false
            };

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
            // Cleanup project directory (go up two levels from project file)
            var projectDir = new DirectoryInfo(Path.GetDirectoryName(projectPath)!).Parent!.FullName;
            if (Directory.Exists(projectDir))
                Directory.Delete(projectDir, true);
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);
        }
    }

    [Fact]
    public async Task ContractUpdate_ShouldWork()
    {
        // Arrange
        var toolkit = CreateToolkit();
        var originalProjectPath = CreateTestContractProject();
        var outputDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputDir);

        var testWalletPath = Path.Combine(outputDir, "test.wallet.json");
        await CreateTestWalletFile(testWalletPath);

        var compilationOptions = new CompilationOptions
        {
            ProjectPath = originalProjectPath,
            OutputDirectory = outputDir,
            ContractName = "TestContract"
        };

        string? updatedProjectPath = null;
        try
        {
            await toolkit.LoadWalletAsync(testWalletPath, "123456");

            var deploymentOptions = new DeploymentOptions
            {
                DeployerAccount = toolkit.GetDeployerAccount(),
                GasLimit = 50_000_000,
                WaitForConfirmation = false
            };

            // Step 1: Deploy original contract
            var originalDeployment = await toolkit.CompileAndDeployAsync(compilationOptions, deploymentOptions);
            Assert.True(originalDeployment.Success);

            // Step 2: Create updated contract
            updatedProjectPath = CreateUpdatedTestContractProject();
            var updateCompilationOptions = new CompilationOptions
            {
                ProjectPath = updatedProjectPath,
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
        }
        finally
        {
            // Cleanup
            var originalProjectDir = new DirectoryInfo(Path.GetDirectoryName(originalProjectPath)!).Parent!.FullName;
            if (Directory.Exists(originalProjectDir))
                Directory.Delete(originalProjectDir, true);
            
            if (updatedProjectPath != null)
            {
                var updatedProjectDir = new DirectoryInfo(Path.GetDirectoryName(updatedProjectPath)!).Parent!.FullName;
                if (Directory.Exists(updatedProjectDir))
                    Directory.Delete(updatedProjectDir, true);
            }
            
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);
        }
    }

    private string CreateUpdatedTestContractProject()
    {
        // Create an updated version of the test contract with different return value
        var contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

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

        return CreateTestContractProject("TestContract", contractCode);
    }

}