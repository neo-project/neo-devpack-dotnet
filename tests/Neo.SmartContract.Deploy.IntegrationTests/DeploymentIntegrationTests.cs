using System;
using System.IO;
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
/// Integration tests for contract deployment functionality
/// </summary>
public class DeploymentIntegrationTests : IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly string _testContractPath;
    private readonly string _testNefPath;
    private readonly string _testManifestPath;
    private DeploymentToolkit? _toolkit;
    private NeoContractToolkit? _neoToolkit;
    private readonly string _testWif = "L1QqQJnpBwbsPGAuutuzPTac8piqvbR1HRjrY5qHup48TBCBFe4g"; // Test private key
    private readonly string _testRpcUrl = "http://localhost:50012"; // Local test node

    public DeploymentIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
        var testDir = Path.Combine(Path.GetTempPath(), "neotest", Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        
        _testContractPath = Path.Combine(testDir, "TestContract.csproj");
        _testNefPath = Path.Combine(testDir, "TestContract.nef");
        _testManifestPath = Path.Combine(testDir, "TestContract.manifest.json");
        
        CreateTestContract(testDir);
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
        
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _toolkit?.Dispose();
        if (_neoToolkit != null)
            await _neoToolkit.DisposeAsync();
        
        // Clean up test files
        var testDir = Path.GetDirectoryName(_testContractPath);
        if (testDir != null && Directory.Exists(testDir))
        {
            Directory.Delete(testDir, true);
        }
    }

    [Fact(Skip = "Requires local Neo node")]
    public async Task DeployAsync_WithValidContract_ShouldDeploy()
    {
        // Arrange
        _output.WriteLine($"Test contract path: {_testContractPath}");

        // Act
        var deploymentInfo = await _toolkit!.DeployAsync(_testContractPath);

        // Assert
        Assert.NotNull(deploymentInfo);
        Assert.NotEqual(UInt160.Zero, deploymentInfo.ContractHash);
        Assert.NotEqual(UInt256.Zero, deploymentInfo.TransactionHash);
        Assert.True(deploymentInfo.GasConsumed > 0);
        
        _output.WriteLine($"Contract deployed at: {deploymentInfo.ContractHash}");
        _output.WriteLine($"Transaction hash: {deploymentInfo.TransactionHash}");
    }

    [Fact(Skip = "Requires local Neo node")]
    public async Task DeployArtifactsAsync_WithPreCompiledFiles_ShouldDeploy()
    {
        // Arrange
        CreateTestArtifacts();

        // Act
        var deploymentInfo = await _toolkit!.DeployArtifactsAsync(_testNefPath, _testManifestPath);

        // Assert
        Assert.NotNull(deploymentInfo);
        Assert.NotEqual(UInt160.Zero, deploymentInfo.ContractHash);
        Assert.NotEqual(UInt256.Zero, deploymentInfo.TransactionHash);
    }

    [Fact(Skip = "Requires local Neo node")]
    public async Task ContractExistsAsync_AfterDeployment_ShouldReturnTrue()
    {
        // Arrange
        CreateTestArtifacts();
        var deploymentInfo = await _toolkit!.DeployArtifactsAsync(_testNefPath, _testManifestPath);

        // Act
        var exists = await _toolkit.ContractExistsAsync(deploymentInfo.ContractHash.ToString());

        // Assert
        Assert.True(exists);
    }

    [Fact(Skip = "Requires local Neo node")]
    public async Task CallAsync_WithDeployedContract_ShouldReturnValue()
    {
        // Arrange
        CreateTestArtifacts();
        var deploymentInfo = await _toolkit!.DeployArtifactsAsync(_testNefPath, _testManifestPath);
        
        // Act
        var result = await _toolkit.CallAsync<string>(
            deploymentInfo.ContractHash.ToString(), 
            "getValue");

        // Assert
        Assert.NotNull(result);
        _output.WriteLine($"Contract returned: {result}");
    }

    [Fact(Skip = "Requires local Neo node")]
    public async Task InvokeAsync_WithDeployedContract_ShouldChangeState()
    {
        // Arrange
        CreateTestArtifacts();
        var deploymentInfo = await _toolkit!.DeployArtifactsAsync(_testNefPath, _testManifestPath);
        
        // Act
        var txHash = await _toolkit.InvokeAsync(
            deploymentInfo.ContractHash.ToString(), 
            "setValue", 
            "Hello, Neo!");

        // Assert
        Assert.NotEqual(UInt256.Zero, txHash);
        _output.WriteLine($"Invocation transaction: {txHash}");
    }

    [Fact(Skip = "Requires local Neo node")]
    public async Task GetGasBalanceAsync_WithValidAccount_ShouldReturnBalance()
    {
        // Act
        var balance = await _toolkit!.GetGasBalanceAsync();

        // Assert
        Assert.True(balance >= 0);
        _output.WriteLine($"Account GAS balance: {balance}");
    }

    [Fact(Skip = "Requires local Neo node")]
    public async Task NeoToolkit_CompileAndDeploy_ShouldWork()
    {
        // Arrange
        var options = new DeploymentOptions
        {
            WifKey = _testWif,
            RpcUrl = _testRpcUrl,
            WaitForConfirmation = false
        };

        // Act
        var deploymentInfo = await _neoToolkit!.DeployFromSourceAsync(_testContractPath, options);

        // Assert
        Assert.NotNull(deploymentInfo);
        Assert.NotEqual(UInt160.Zero, deploymentInfo.ContractHash);
        
        // Verify contract exists
        var exists = await _neoToolkit.ContractExistsAsync(deploymentInfo.ContractHash, _testRpcUrl);
        Assert.True(exists);
    }

    [Fact(Skip = "Requires local Neo node")]
    public async Task NeoToolkit_DeployWithInitParams_ShouldPassParameters()
    {
        // Arrange
        CreateTestArtifacts();
        var options = new DeploymentOptions
        {
            WifKey = _testWif,
            RpcUrl = _testRpcUrl,
            WaitForConfirmation = false
        };
        var initParams = new object[] { "InitialValue", 42 };

        // Act
        var deploymentInfo = await _neoToolkit!.DeployFromFilesAsync(
            _testNefPath, 
            _testManifestPath, 
            options, 
            initParams);

        // Assert
        Assert.NotNull(deploymentInfo);
        Assert.NotEqual(UInt160.Zero, deploymentInfo.ContractHash);
    }

    [Fact]
    public void SetNetwork_WithPredefinedNetworks_ShouldConfigureCorrectly()
    {
        // Test mainnet
        var toolkit1 = new DeploymentToolkit().SetNetwork("mainnet");
        Assert.NotNull(toolkit1);

        // Test testnet
        var toolkit2 = new DeploymentToolkit().SetNetwork("testnet");
        Assert.NotNull(toolkit2);

        // Test local
        var toolkit3 = new DeploymentToolkit().SetNetwork("local");
        Assert.NotNull(toolkit3);

        // Test custom URL
        var toolkit4 = new DeploymentToolkit().SetNetwork("http://custom:10332");
        Assert.NotNull(toolkit4);
        
        // Cleanup
        toolkit1.Dispose();
        toolkit2.Dispose();
        toolkit3.Dispose();
        toolkit4.Dispose();
    }

    [Fact]
    public void SetWifKey_WithInvalidKey_ShouldThrow()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => toolkit.SetWifKey("invalid_wif_key"));
        
        // Cleanup
        toolkit.Dispose();
    }

    private void CreateTestContract(string testDir)
    {
        // Create a simple test contract project
        var projectContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.6.2"" />
  </ItemGroup>
</Project>";
        File.WriteAllText(_testContractPath, projectContent);

        // Create a simple test contract
        var contractContent = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace TestContract
{
    [ManifestExtra(""Author"", ""Test"")]
    [ManifestExtra(""Email"", ""test@test.com"")]
    [ManifestExtra(""Description"", ""Test Contract"")]
    public class TestContract : SmartContract
    {
        private static readonly byte[] StorageKey = ""value""u8;

        public static void _deploy(object data, bool update)
        {
            if (!update)
            {
                Storage.Put(Storage.CurrentContext, StorageKey, ""Initial Value"");
            }
        }

        public static string GetValue()
        {
            return Storage.Get(Storage.CurrentContext, StorageKey);
        }

        public static void SetValue(string value)
        {
            Storage.Put(Storage.CurrentContext, StorageKey, value);
        }
    }
}";
        var contractPath = Path.Combine(testDir, "TestContract.cs");
        File.WriteAllText(contractPath, contractContent);
    }

    private void CreateTestArtifacts()
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
            0x00, 0x00, 0x00, 0x00 // Checksum (will be invalid but ok for test)
        };
        File.WriteAllBytes(_testNefPath, nefContent);

        // Create minimal manifest
        var manifestContent = @"{
            ""name"": ""TestContract"",
            ""groups"": [],
            ""features"": {},
            ""supportedstandards"": [],
            ""abi"": {
                ""methods"": [
                    {
                        ""name"": ""_deploy"",
                        ""parameters"": [
                            { ""name"": ""data"", ""type"": ""Any"" },
                            { ""name"": ""update"", ""type"": ""Boolean"" }
                        ],
                        ""returntype"": ""Void"",
                        ""offset"": 0,
                        ""safe"": false
                    },
                    {
                        ""name"": ""getValue"",
                        ""parameters"": [],
                        ""returntype"": ""String"",
                        ""offset"": 0,
                        ""safe"": true
                    },
                    {
                        ""name"": ""setValue"",
                        ""parameters"": [
                            { ""name"": ""value"", ""type"": ""String"" }
                        ],
                        ""returntype"": ""Void"",
                        ""offset"": 0,
                        ""safe"": false
                    }
                ],
                ""events"": []
            },
            ""permissions"": [
                { ""contract"": ""*"", ""methods"": ""*"" }
            ],
            ""trusts"": [],
            ""extra"": {
                ""Author"": ""Test"",
                ""Email"": ""test@test.com"",
                ""Description"": ""Test Contract""
            }
        }";
        File.WriteAllText(_testManifestPath, manifestContent);
    }
}