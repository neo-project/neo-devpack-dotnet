using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Neo.SmartContract.Deploy.Interfaces;
using Neo.SmartContract.Deploy.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests;

public class DeploymentToolkitTests : TestBase
{
    public DeploymentToolkitTests()
    {
        // No setup needed for this basic test class
    }

    [Fact]
    public void Constructor_ShouldInitializeWithDefaultConfiguration()
    {
        // Act
        var toolkit = new DeploymentToolkit();

        // Assert
        Assert.NotNull(toolkit);
    }

    [Fact]
    public void SetNetwork_ShouldConfigureMainNet()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act
        var result = toolkit.SetNetwork("mainnet");

        // Assert
        Assert.Same(toolkit, result);
        Assert.Equal("https://rpc10.n3.nspcc.ru:10331", Environment.GetEnvironmentVariable("Network__RpcUrl"));
        Assert.Equal("mainnet", Environment.GetEnvironmentVariable("Network__Network"));
    }

    [Fact]
    public void SetNetwork_ShouldConfigureTestNet()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act
        var result = toolkit.SetNetwork("testnet");

        // Assert
        Assert.Same(toolkit, result);
        Assert.Equal("https://testnet1.neo.coz.io:443", Environment.GetEnvironmentVariable("Network__RpcUrl"));
        Assert.Equal("testnet", Environment.GetEnvironmentVariable("Network__Network"));
    }

    [Fact]
    public void SetNetwork_ShouldConfigureLocalNetwork()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act
        var result = toolkit.SetNetwork("local");

        // Assert
        Assert.Same(toolkit, result);
        Assert.Equal("http://localhost:50012", Environment.GetEnvironmentVariable("Network__RpcUrl"));
        Assert.Equal("private", Environment.GetEnvironmentVariable("Network__Network"));
    }

    [Fact]
    public void SetNetwork_ShouldAcceptCustomRpcUrl()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var customRpc = "http://custom.rpc:10332";

        // Act
        var result = toolkit.SetNetwork(customRpc);

        // Assert
        Assert.Same(toolkit, result);
        Assert.Equal(customRpc, Environment.GetEnvironmentVariable("Network__RpcUrl"));
        Assert.Equal("custom", Environment.GetEnvironmentVariable("Network__Network"));
    }

    [Fact]
    public async Task DeployArtifacts_WithoutWifKey_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        
        var nefPath = Path.Combine(tempDir, "test.nef");
        var manifestPath = Path.Combine(tempDir, "test.manifest.json");
        
        try
        {
            // Create minimal NEF file
            var nefContent = new byte[] 
            { 
                0x4E, 0x45, 0x46, 0x33, // Magic
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, // Compiler
                0x00, 0x00, 0x00, 0x00, // Source
                0x00, // Reserved
                0x00, 0x00, // Method count
                0x01, 0x00, // Script length
                0x40, // RET opcode
                0x00, 0x00, 0x00, 0x00 // Checksum
            };
            await File.WriteAllBytesAsync(nefPath, nefContent);

            // Create minimal manifest
            var manifestContent = @"{
                ""name"": ""TestContract"",
                ""groups"": [],
                ""features"": {},
                ""supportedstandards"": [],
                ""abi"": {
                    ""methods"": [{
                        ""name"": ""test"",
                        ""parameters"": [],
                        ""returntype"": ""Void"",
                        ""offset"": 0,
                        ""safe"": true
                    }],
                    ""events"": []
                },
                ""permissions"": [],
                ""trusts"": [],
                ""extra"": null
            }";
            await File.WriteAllTextAsync(manifestPath, manifestContent);

            // Act & Assert
            // Should throw InvalidOperationException for missing WIF key
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => toolkit.DeployArtifactsAsync(nefPath, manifestPath)
            );
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public async Task GetGasBalance_WithInvalidAddress_ShouldThrowArgumentException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var testAddress = "invalid-address";
        toolkit.SetWifKey("KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb");
        toolkit.SetNetwork("testnet");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => toolkit.GetGasBalanceAsync(testAddress)
        );
    }

    [Fact]
    public async Task GetDeployerAccount_WithoutWifKey_ShouldThrowException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => toolkit.GetDeployerAccountAsync()
        );
    }

    #region WIF Key Tests

    [Fact]
    public void SetWifKey_WithValidKey_ShouldSetKeySuccessfully()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var validWifKey = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb";

        // Act
        var result = toolkit.SetWifKey(validWifKey);

        // Assert
        Assert.Same(toolkit, result);
        // The WIF key should be set internally for signing
    }

    [Fact]
    public void SetWifKey_WithInvalidKey_ShouldThrowArgumentException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var invalidWifKey = "invalid-wif-key";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(
            () => toolkit.SetWifKey(invalidWifKey)
        );

        Assert.Contains("Invalid WIF key", exception.Message);
    }

    [Fact]
    public void SetWifKey_WithNullOrEmpty_ShouldThrowArgumentException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => toolkit.SetWifKey(""));
        Assert.Throws<ArgumentException>(() => toolkit.SetWifKey(null!));
    }

    [Fact]
    public async Task GetDeployerAccount_WithWifKey_ShouldReturnAccount()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var validWifKey = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb";

        toolkit.SetWifKey(validWifKey);

        // Act
        var account = await toolkit.GetDeployerAccountAsync();

        // Assert
        Assert.NotNull(account);
        Assert.NotEqual(UInt160.Zero, account);
    }

    [Fact]
    public async Task ContractExistsAsync_WithEmptyHash_ShouldThrowArgumentException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        toolkit.SetNetwork("testnet");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => toolkit.ContractExistsAsync("")
        );
    }

    #endregion

    private new string CreateTestContract()
    {
        var contractCode = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using Neo.SmartContract.Framework.Native;
using System;
using System.ComponentModel;

namespace TestContract
{
    [ManifestExtra(""Author"", ""Neo"")]
    [ManifestExtra(""Description"", ""Test Contract with Update"")]
    [ManifestExtra(""Version"", ""1.0.0"")]
    public class TestContract : SmartContract
    {
        [DisplayName(""testMethod"")]
        public static string TestMethod(string input)
        {
            return ""Hello "" + input;
        }

        [DisplayName(""getValue"")]
        public static int GetValue()
        {
            return 42;
        }

        public static void _deploy(object data, bool update)
        {
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
}
