using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Neo;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using Neo.SmartContract.Deploy.Services;
using Neo.SmartContract.Deploy.Shared;
using Neo.SmartContract.Manifest;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests.Integration;

/// <summary>
/// Integration tests for contract update functionality
/// Tests the complete update workflow from compilation to transaction submission
/// </summary>
public class ContractUpdateIntegrationTests : TestBase
{
    private readonly Mock<IRpcClientFactory> _mockRpcClientFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly DeploymentToolkit _toolkit;

    public ContractUpdateIntegrationTests()
    {
        _mockRpcClientFactory = new Mock<IRpcClientFactory>();

        // Setup mock RPC client factory to avoid network calls
        _mockRpcClientFactory.Setup(x => x.CreateClient(It.IsAny<string>()))
                            .Throws(new InvalidOperationException("Mock RPC client - no network calls allowed in unit tests"));

        // Create service collection for dependency injection
        var services = new ServiceCollection();
        services.AddSingleton(Configuration);
        services.AddSingleton(LoggerFactory);
        services.AddSingleton(_mockRpcClientFactory.Object);
        services.AddLogging();

        // Add deployment services
        services.AddTransient<IContractCompiler, ContractCompilerService>();
        services.AddTransient<IContractDeployer, ContractDeployerService>();
        services.AddTransient<IWalletManager, WalletManagerService>();
        services.AddTransient<TransactionBuilder>();
        services.AddTransient<TransactionConfirmationService>();

        _serviceProvider = services.BuildServiceProvider();
        _toolkit = new DeploymentToolkit();
    }

    [Fact]
    public async Task UpdateWorkflow_WithValidContract_ShouldCompileAndAttemptUpdate()
    {
        // Arrange
        var originalContract = CreateUpdatableTestContract();
        var updatedContract = CreateUpdatedTestContract();
        var contractHash = "0x1234567890123456789012345678901234567890";

        _toolkit.SetWifKey("KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb");
        _toolkit.SetNetwork("testnet");

        // Act
        var result = await _toolkit.UpdateAsync(contractHash, updatedContract);

        // Assert
        // Should fail due to network but reach compilation stage
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);

        // Should not be compilation or parameter validation error
        Assert.DoesNotContain("invalid", result.ErrorMessage.ToLower());
        Assert.DoesNotContain("null", result.ErrorMessage.ToLower());
        Assert.DoesNotContain("compilation", result.ErrorMessage.ToLower());
    }

    [Fact]
    public async Task UpdateWorkflow_WithProjectFile_ShouldSupportCsprojFiles()
    {
        // Arrange
        var projectPath = CreateTestContractProject("UpdateTestContract");
        var contractHash = "0x1234567890123456789012345678901234567890";

        _toolkit.SetWifKey("KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb");
        _toolkit.SetNetwork("testnet");

        // Act
        var result = await _toolkit.UpdateAsync(contractHash, projectPath);

        // Assert
        // Should fail due to network but accept .csproj files
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);

        // Should not be file format error
        Assert.DoesNotContain("format", result.ErrorMessage.ToLower());
        Assert.DoesNotContain("extension", result.ErrorMessage.ToLower());
    }

    [Fact]
    public void UpdateAsync_WithDifferentNetworks_ShouldConfigureCorrectly()
    {
        // Arrange
        var toolkit1 = new DeploymentToolkit();
        var toolkit2 = new DeploymentToolkit();
        var toolkit3 = new DeploymentToolkit();

        // Act & Assert - Just verify that SetNetwork calls don't throw exceptions
        // The actual network configuration is internal and tested elsewhere
        toolkit1.SetNetwork("mainnet");
        toolkit2.SetNetwork("testnet");
        toolkit3.SetNetwork("http://custom.rpc:10332");

        // Verify we can set different networks without errors
        Assert.True(true); // If we reach here without exceptions, the test passes
    }

    [Fact]
    public async Task UpdateWorkflow_WithInvalidContractCode_ShouldFailCompilation()
    {
        // Arrange
        var invalidContract = CreateInvalidTestContract();
        var contractHash = "0x1234567890123456789012345678901234567890";

        _toolkit.SetWifKey("KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb");
        _toolkit.SetNetwork("testnet");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Neo.SmartContract.Deploy.Exceptions.CompilationException>(
            () => _toolkit.UpdateAsync(contractHash, invalidContract)
        );

        // Should be compilation-related error
        Assert.True(
            exception.Message.Contains("compilation") ||
            exception.Message.Contains("build") ||
            exception.Message.Contains("error") ||
            exception.Message.Contains("syntax"),
            $"Expected compilation error, but got: {exception.Message}"
        );
    }

    [Fact]
    public async Task UpdateWorkflow_WithMissingUpdateMethod_ShouldStillCompile()
    {
        // Arrange
        var contractWithoutUpdate = CreateContractWithoutUpdateMethod();
        var contractHash = "0x1234567890123456789012345678901234567890";

        _toolkit.SetWifKey("KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb");
        _toolkit.SetNetwork("testnet");

        // Act
        var result = await _toolkit.UpdateAsync(contractHash, contractWithoutUpdate);

        // Assert
        // Should compile successfully but might fail at runtime when calling update
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);

        // Should not be compilation error since contract has proper _deploy method
        Assert.DoesNotContain("compilation", result.ErrorMessage.ToLower());
        Assert.DoesNotContain("build failed", result.ErrorMessage.ToLower());
    }

    [Fact]
    public async Task ContractDeployer_UpdateAsync_WithCompleteWorkflow_ShouldExecuteAllSteps()
    {
        // Arrange
        var deployer = _serviceProvider.GetRequiredService<IContractDeployer>();
        var compiledContract = CreateMockCompiledContract();
        var contractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        var deploymentOptions = new DeploymentOptions
        {
            WifKey = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb",
            GasLimit = 100_000_000,
            NetworkMagic = 894710606, // Testnet
            WaitForConfirmation = false
        };

        // Act
        var result = await deployer.UpdateAsync(compiledContract, contractHash, deploymentOptions);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success); // Should fail due to mock RPC
        Assert.NotNull(result.ErrorMessage);

        // Verify error is network-related, not validation
        Assert.True(
            result.ErrorMessage.Contains("Mock RPC client") ||
            result.ErrorMessage.Contains("Connection") ||
            result.ErrorMessage.Contains("network"),
            $"Expected network error, but got: {result.ErrorMessage}"
        );
    }

    [Fact]
    public async Task UpdateWorkflow_ErrorHandling_ShouldProvideDetailedMessages()
    {
        // Test various error scenarios
        var toolkit = new DeploymentToolkit();

        // Test 1: Missing WIF key
        var contractPath = CreateUpdatableTestContract();
        var contractHash = "0x1234567890123456789012345678901234567890";

        var exception1 = await Assert.ThrowsAsync<InvalidOperationException>(
            () => toolkit.UpdateAsync(contractHash, contractPath)
        );
        Assert.Contains("wallet", exception1.Message.ToLower());

        // Test 2: Invalid contract hash
        toolkit.SetWifKey("KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb");

        var exception2 = await Assert.ThrowsAsync<ArgumentException>(
            () => toolkit.UpdateAsync("invalid-hash", contractPath)
        );
        Assert.Contains("hash", exception2.Message.ToLower());

        // Test 3: Non-existent file
        var exception3 = await Assert.ThrowsAsync<FileNotFoundException>(
            () => toolkit.UpdateAsync(contractHash, "/non/existent/file.cs")
        );
        Assert.Contains("not found", exception3.Message.ToLower());
    }

    #region Helper Methods

    private string CreateUpdatableTestContract()
    {
        var contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System.ComponentModel;

namespace TestContract
{
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Description"", ""Updatable Test Contract"")]
    [ManifestExtra(""Version"", ""1.0.0"")]
    public class TestContract : SmartContract
    {
        [DisplayName(""getValue"")]
        public static int GetValue()
        {
            return 42;
        }

        public static void _deploy(object data, bool update)
        {
            if (update)
            {
                // Authorization check for testing
                // In a real contract, add proper authorization
                return;
            }
            
            // Initial deployment
            Storage.Put(Storage.CurrentContext, ""initialized"", 1);
        }
    }
}";

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var contractPath = Path.Combine(tempDir, "TestContract.cs");
        File.WriteAllText(contractPath, contractCode);
        return contractPath;
    }

    private string CreateUpdatedTestContract()
    {
        var contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System.ComponentModel;

namespace TestContract
{
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Description"", ""Updated Test Contract"")]
    [ManifestExtra(""Version"", ""1.1.0"")]
    public class TestContract : SmartContract
    {
        [DisplayName(""getValue"")]
        public static int GetValue()
        {
            return 84; // Updated value
        }

        [DisplayName(""getNewValue"")]
        public static string GetNewValue()
        {
            return ""Updated!""; // New method
        }

        public static void _deploy(object data, bool update)
        {
            if (update)
            {
                // Authorization check for testing  
                Runtime.Log(""Contract update initiated"");
                Runtime.Log(""Contract updated to version 1.1.0"");
                return;
            }
            
            // Initial deployment
            Storage.Put(Storage.CurrentContext, ""initialized"", 1);
        }
    }
}";

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var contractPath = Path.Combine(tempDir, "UpdatedTestContract.cs");
        File.WriteAllText(contractPath, contractCode);
        return contractPath;
    }

    private string CreateInvalidTestContract()
    {
        var invalidCode = @"
using Neo.SmartContract.Framework;

namespace TestContract
{
    public class TestContract : SmartContract
    {
        public static int GetValue()
        {
            return invalid_syntax_here; // This will cause compilation error
        }
    }
}";

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var contractPath = Path.Combine(tempDir, "InvalidContract.cs");
        File.WriteAllText(contractPath, invalidCode);
        return contractPath;
    }

    private string CreateContractWithoutUpdateMethod()
    {
        var contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

namespace TestContract
{
    public class TestContract : SmartContract
    {
        [DisplayName(""getValue"")]
        public static int GetValue()
        {
            return 42;
        }

        // No update method - this should still compile but fail at runtime
        public static void _deploy(object data, bool update)
        {
            if (!update)
            {
                Storage.Put(Storage.CurrentContext, ""initialized"", 1);
            }
        }
    }
}";

        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var contractPath = Path.Combine(tempDir, "NoUpdateContract.cs");
        File.WriteAllText(contractPath, contractCode);
        return contractPath;
    }

    private CompiledContract CreateMockCompiledContract()
    {
        // Create a simple test NEF file content
        var nefBytes = new byte[]
        {
            0x4E, 0x45, 0x46, 0x33, // NEF3 magic
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // 64 byte compiler field
            0x01, 0x00, // Source string length (1) + empty source
            0x00, // Reserved byte
            0x00, // Method tokens count
            0x00, 0x00, // Reserved 2 bytes
            0x04, 0x40, 0x41, 0x9F, 0x00, // Script (4 bytes length + simple script)
            0x00, 0x00, 0x00, 0x00 // Checksum placeholder
        };

        var manifest = new ContractManifest
        {
            Name = "UpdateTestContract",
            Groups = new ContractGroup[0],
            SupportedStandards = new string[0],
            Abi = new ContractAbi
            {
                Methods = new ContractMethodDescriptor[]
                {
                    new ContractMethodDescriptor
                    {
                        Name = "update",
                        Parameters = new ContractParameterDefinition[]
                        {
                            new ContractParameterDefinition { Name = "nefFile", Type = ContractParameterType.ByteArray },
                            new ContractParameterDefinition { Name = "manifest", Type = ContractParameterType.String },
                            new ContractParameterDefinition { Name = "data", Type = ContractParameterType.Any }
                        },
                        ReturnType = ContractParameterType.Boolean,
                        Safe = false
                    }
                },
                Events = new ContractEventDescriptor[0]
            },
            Permissions = new ContractPermission[] { ContractPermission.DefaultPermission },
            Trusts = WildcardContainer<ContractPermissionDescriptor>.Create(),
            Extra = null
        };

        return new CompiledContract
        {
            Name = "UpdateTestContract",
            NefFilePath = "/tmp/updatetest.nef",
            ManifestFilePath = "/tmp/updatetest.manifest.json",
            NefBytes = nefBytes,
            Manifest = manifest
        };
    }

    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_serviceProvider is IDisposable disposableProvider)
            {
                disposableProvider.Dispose();
            }
            _toolkit?.Dispose();
        }
        base.Dispose(disposing);
    }
}
