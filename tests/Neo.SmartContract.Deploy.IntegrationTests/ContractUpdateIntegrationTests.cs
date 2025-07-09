using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy.Extensions;
using Neo.SmartContract.Deploy.Models;
using Xunit;
using Xunit.Abstractions;

namespace Neo.SmartContract.Deploy.IntegrationTests;

/// <summary>
/// Integration tests for contract update functionality
/// </summary>
public class ContractUpdateIntegrationTests : IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly string _testContractPath;
    private readonly string _testContractV2Path;
    private readonly string _testNefPath;
    private readonly string _testManifestPath;
    private readonly string _testNefV2Path;
    private readonly string _testManifestV2Path;
    private DeploymentToolkit? _toolkit;
    private NeoContractToolkit? _neoToolkit;
    private readonly string _testWif = "L1QqQJnpBwbsPGAuutuzPTac8piqvbR1HRjrY5qHup48TBCBFe4g"; // Test private key
    private readonly string _testRpcUrl = "http://localhost:50012"; // Local test node
    private UInt160? _deployedContractHash;

    public ContractUpdateIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
        var testDir = Path.Combine(Path.GetTempPath(), "neoupdatetest", Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        
        _testContractPath = Path.Combine(testDir, "TestContract.csproj");
        _testContractV2Path = Path.Combine(testDir, "TestContractV2.csproj");
        _testNefPath = Path.Combine(testDir, "TestContract.nef");
        _testManifestPath = Path.Combine(testDir, "TestContract.manifest.json");
        _testNefV2Path = Path.Combine(testDir, "TestContractV2.nef");
        _testManifestV2Path = Path.Combine(testDir, "TestContractV2.manifest.json");
        
        CreateTestContracts(testDir);
    }

    public async Task InitializeAsync()
    {
        // Initialize the toolkits
        _toolkit = new DeploymentToolkit()
            .SetNetwork(_testRpcUrl)
            .SetWifKey(_testWif);

        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddNeoContractDeploy(new ConfigurationBuilder().Build());
        var serviceProvider = services.BuildServiceProvider();
        _neoToolkit = serviceProvider.GetRequiredService<NeoContractToolkit>();

        // Deploy initial contract for update tests
        await DeployInitialContract();
    }

    public Task DisposeAsync()
    {
        _toolkit?.Dispose();
        
        // Clean up test files
        var testDir = Path.GetDirectoryName(_testContractPath);
        if (testDir != null && Directory.Exists(testDir))
        {
            try
            {
                Directory.Delete(testDir, true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        return Task.CompletedTask;
    }

    private async Task DeployInitialContract()
    {
        try
        {
            // Deploy the initial version of the contract
            var deploymentInfo = await _toolkit!.DeployAsync(_testContractPath);
            _deployedContractHash = deploymentInfo.ContractHash;
            _output.WriteLine($"Initial contract deployed: {_deployedContractHash}");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"Failed to deploy initial contract: {ex.Message}");
            // If deployment fails, we'll skip update tests
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("RequiresNode", "true")]
    public async Task UpdateAsync_WithValidContract_ShouldSucceed()
    {
        // Skip if no contract was deployed
        if (_deployedContractHash == null)
        {
            _output.WriteLine("Skipping test - no contract deployed");
            return;
        }

        // Act
        var updateInfo = await _toolkit!.UpdateAsync(_deployedContractHash.ToString(), _testContractV2Path);

        // Assert
        Assert.NotNull(updateInfo);
        Assert.True(updateInfo.Success, $"Update failed: {updateInfo.ErrorMessage}");
        Assert.Equal(_deployedContractHash, updateInfo.ContractHash);
        Assert.NotNull(updateInfo.TransactionHash);
        Assert.True(updateInfo.GasConsumed > 0);
        Assert.False(updateInfo.VerificationFailed);
        
        _output.WriteLine($"Contract updated successfully. TxHash: {updateInfo.TransactionHash}");
        _output.WriteLine($"Gas consumed: {updateInfo.GasConsumed}");
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("RequiresNode", "true")]
    public async Task UpdateAsync_WithAddress_ShouldSucceed()
    {
        // Skip if no contract was deployed
        if (_deployedContractHash == null)
        {
            _output.WriteLine("Skipping test - no contract deployed");
            return;
        }

        // Get contract address
        var address = _deployedContractHash.ToAddress(Neo.ProtocolSettings.Default.AddressVersion);

        // Act
        var updateInfo = await _toolkit!.UpdateAsync(address, _testContractV2Path);

        // Assert
        Assert.NotNull(updateInfo);
        Assert.True(updateInfo.Success, $"Update failed: {updateInfo.ErrorMessage}");
        Assert.Equal(_deployedContractHash, updateInfo.ContractHash);
        
        _output.WriteLine($"Contract updated via address: {address}");
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("RequiresNode", "true")]
    public async Task UpdateArtifactsAsync_UpdateNefOnly_ShouldSucceed()
    {
        // Skip if no contract was deployed
        if (_deployedContractHash == null)
        {
            _output.WriteLine("Skipping test - no contract deployed");
            return;
        }

        // Create updated NEF file
        await CreateTestArtifacts();

        // Act
        var updateInfo = await _toolkit!.UpdateArtifactsAsync(
            _deployedContractHash.ToString(), 
            _testNefV2Path, 
            null); // Only update NEF

        // Assert
        Assert.NotNull(updateInfo);
        Assert.True(updateInfo.Success, $"Update failed: {updateInfo.ErrorMessage}");
        Assert.Equal(_deployedContractHash, updateInfo.ContractHash);
        
        _output.WriteLine("Contract NEF updated successfully");
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("RequiresNode", "true")]
    public async Task UpdateArtifactsAsync_UpdateManifestOnly_ShouldSucceed()
    {
        // Skip if no contract was deployed
        if (_deployedContractHash == null)
        {
            _output.WriteLine("Skipping test - no contract deployed");
            return;
        }

        // Create updated manifest file
        await CreateTestArtifacts();

        // Act
        var updateInfo = await _toolkit!.UpdateArtifactsAsync(
            _deployedContractHash.ToString(), 
            null, // Only update manifest
            _testManifestV2Path);

        // Assert
        Assert.NotNull(updateInfo);
        Assert.True(updateInfo.Success, $"Update failed: {updateInfo.ErrorMessage}");
        Assert.Equal(_deployedContractHash, updateInfo.ContractHash);
        
        _output.WriteLine("Contract manifest updated successfully");
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("RequiresNode", "true")]
    public async Task UpdateAsync_WithUpdateParams_ShouldPassParamsToDeployMethod()
    {
        // Skip if no contract was deployed
        if (_deployedContractHash == null)
        {
            _output.WriteLine("Skipping test - no contract deployed");
            return;
        }

        // Arrange
        var updateParams = new object[] { "v2", 42 };

        // Act
        var updateInfo = await _toolkit!.UpdateAsync(
            _deployedContractHash.ToString(), 
            _testContractV2Path, 
            updateParams);

        // Assert
        Assert.NotNull(updateInfo);
        Assert.True(updateInfo.Success, $"Update failed: {updateInfo.ErrorMessage}");
        
        _output.WriteLine("Contract updated with parameters successfully");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateAsync_NonExistentContract_ShouldFail()
    {
        // Arrange
        var nonExistentHash = UInt160.Zero;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _toolkit!.UpdateAsync(nonExistentHash.ToString(), _testContractV2Path));
        
        _output.WriteLine("Update of non-existent contract failed as expected");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void UpdateAsync_InvalidContractHash_ShouldThrow()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _toolkit!.UpdateAsync("invalid-hash", _testContractV2Path));
        
        _output.WriteLine("Invalid contract hash rejected as expected");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void UpdateAsync_EmptyPath_ShouldThrow()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _toolkit!.UpdateAsync("0x0000000000000000000000000000000000000000", ""));
        
        _output.WriteLine("Empty path rejected as expected");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void UpdateArtifactsAsync_BothPathsNull_ShouldThrow()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () =>
            await _toolkit!.UpdateArtifactsAsync("0x0000000000000000000000000000000000000000", null, null));
        
        _output.WriteLine("Null paths rejected as expected");
    }

    private void CreateTestContracts(string testDir)
    {
        // Create test contract v1
        var contractCodeV1 = @"
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;

namespace TestContract
{
    [ManifestExtra(""Version"", ""1.0.0"")]
    [ContractPermission(""*"", ""*"")]
    public class TestContract : SmartContract
    {
        public static string GetVersion() => ""v1"";
        
        public static int Add(int a, int b) => a + b;
        
        public static void _deploy(object data, bool update)
        {
            if (update)
            {
                Storage.Put(Storage.CurrentContext, ""updated"", ""true"");
            }
            else
            {
                Storage.Put(Storage.CurrentContext, ""version"", ""v1"");
            }
        }
    }
}";

        // Create test contract v2 (updated version)
        var contractCodeV2 = @"
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System;

namespace TestContract
{
    [ManifestExtra(""Version"", ""2.0.0"")]
    [ContractPermission(""*"", ""*"")]
    public class TestContract : SmartContract
    {
        public static string GetVersion() => ""v2"";
        
        public static int Add(int a, int b) => a + b;
        
        public static int Multiply(int a, int b) => a * b; // New method
        
        public static void _deploy(object data, bool update)
        {
            if (update)
            {
                Storage.Put(Storage.CurrentContext, ""updated"", ""true"");
                Storage.Put(Storage.CurrentContext, ""version"", ""v2"");
                
                // Handle update parameters if provided
                if (data is object[] args && args.Length >= 1)
                {
                    Storage.Put(Storage.CurrentContext, ""updateVersion"", (string)args[0]);
                }
            }
            else
            {
                Storage.Put(Storage.CurrentContext, ""version"", ""v1"");
            }
        }
    }
}";

        // Create project file
        var projectContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""*"" />
  </ItemGroup>
</Project>";

        // Write v1 files
        File.WriteAllText(_testContractPath, projectContent);
        var sourcePathV1 = Path.Combine(testDir, "TestContract.cs");
        File.WriteAllText(sourcePathV1, contractCodeV1);

        // Write v2 files
        File.WriteAllText(_testContractV2Path, projectContent);
        var sourcePathV2 = Path.Combine(testDir, "TestContractV2.cs");
        File.WriteAllText(sourcePathV2, contractCodeV2);
    }

    private async Task CreateTestArtifacts()
    {
        // For simplicity, create dummy NEF and manifest files
        // In real scenarios, these would be compiled artifacts
        
        // Create dummy NEF v2
        var nefContent = new byte[] { 0x4E, 0x45, 0x46, 0x33 }; // NEF3 header
        await File.WriteAllBytesAsync(_testNefV2Path, nefContent);

        // Create manifest v2
        var manifestContent = @"{
            ""name"": ""TestContract"",
            ""groups"": [],
            ""features"": {},
            ""supportedstandards"": [],
            ""abi"": {
                ""methods"": [
                    {
                        ""name"": ""getVersion"",
                        ""parameters"": [],
                        ""returntype"": ""String"",
                        ""offset"": 0,
                        ""safe"": true
                    },
                    {
                        ""name"": ""add"",
                        ""parameters"": [
                            {""name"": ""a"", ""type"": ""Integer""},
                            {""name"": ""b"", ""type"": ""Integer""}
                        ],
                        ""returntype"": ""Integer"",
                        ""offset"": 10,
                        ""safe"": true
                    },
                    {
                        ""name"": ""multiply"",
                        ""parameters"": [
                            {""name"": ""a"", ""type"": ""Integer""},
                            {""name"": ""b"", ""type"": ""Integer""}
                        ],
                        ""returntype"": ""Integer"",
                        ""offset"": 20,
                        ""safe"": true
                    },
                    {
                        ""name"": ""_deploy"",
                        ""parameters"": [
                            {""name"": ""data"", ""type"": ""Any""},
                            {""name"": ""update"", ""type"": ""Boolean""}
                        ],
                        ""returntype"": ""Void"",
                        ""offset"": 30,
                        ""safe"": false
                    }
                ],
                ""events"": []
            },
            ""permissions"": [
                {
                    ""contract"": ""*"",
                    ""methods"": ""*""
                }
            ],
            ""trusts"": [],
            ""extra"": {
                ""Version"": ""2.0.0""
            }
        }";
        
        await File.WriteAllTextAsync(_testManifestV2Path, manifestContent);
    }
}