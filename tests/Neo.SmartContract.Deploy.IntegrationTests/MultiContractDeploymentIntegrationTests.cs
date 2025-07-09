using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neo;
using Neo.SmartContract.Deploy.Extensions;
using Neo.SmartContract.Deploy.Models;
using Xunit;

namespace Neo.SmartContract.Deploy.IntegrationTests;

[Collection("IntegrationTests")]
[Trait("Category", "Integration")]
public class MultiContractDeploymentIntegrationTests : IAsyncLifetime
{
    private readonly IServiceProvider _serviceProvider;
    private readonly DeploymentToolkit _toolkit;
    private readonly string _testDataPath;
    private string? _wifKey;

    public MultiContractDeploymentIntegrationTests()
    {
        // Set up test data path
        _testDataPath = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "MultiContract");
        Directory.CreateDirectory(_testDataPath);

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Set up services
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        services.AddNeoContractDeploy(configuration);
        
        _serviceProvider = services.BuildServiceProvider();
        _toolkit = new DeploymentToolkit();
    }

    public async Task InitializeAsync()
    {
        // Get WIF key from environment or configuration
        _wifKey = Environment.GetEnvironmentVariable("NEO_TEST_WIF_KEY");
        
        if (string.IsNullOrEmpty(_wifKey))
        {
            // Skip tests if no WIF key is provided
            return;
        }

        _toolkit.SetNetwork("testnet").SetWifKey(_wifKey);
        
        // Create test contracts
        await CreateTestContractsAsync();
    }

    public Task DisposeAsync()
    {
        _toolkit?.Dispose();
        (_serviceProvider as IDisposable)?.Dispose();
        
        // Clean up test data
        if (Directory.Exists(_testDataPath))
        {
            Directory.Delete(_testDataPath, true);
        }
        
        return Task.CompletedTask;
    }

    [SkippableFact]
    public async Task DeployMultipleContracts_WithManifestBuilder_ShouldSucceed()
    {
        Skip.If(string.IsNullOrEmpty(_wifKey), "No WIF key provided for testing");

        // Arrange
        var manifest = _toolkit.CreateManifestBuilder()
            .WithName("Test Multi-Contract Deployment")
            .WithDescription("Integration test for multi-contract deployment")
            .WithSettings(s =>
            {
                s.VerifyAfterDeploy = true;
                s.WaitForConfirmation = true;
                s.DryRun = false;
            })
            .AddContract("token", "TestToken", Path.Combine(_testDataPath, "TestToken.cs"))
            .AddContract("dex", "TestDEX", Path.Combine(_testDataPath, "TestDEX.cs"), c => c
                .DependsOn("token")
                .WithInitParams("@contract:token", 1000000)
                .WithDescription("Decentralized Exchange"))
            .AddInteraction("dex", "token", "approve", i => i
                .WithParams("@deployer", 1000000)
                .WithDescription("Approve DEX to spend tokens")
                .WithOrder(1))
            .Build();

        // Act
        var result = await _toolkit.DeployMultipleAsync(manifest);

        // Assert
        Assert.Equal(DeploymentStatus.Completed, result.Status);
        Assert.Equal(2, result.DeployedContracts.Count);
        Assert.True(result.DeployedContracts.ContainsKey("token"));
        Assert.True(result.DeployedContracts.ContainsKey("dex"));
        Assert.Equal(1, result.InteractionResults.Count);
        Assert.True(result.InteractionResults[0].Success);
        
        // Verify contracts exist on chain
        var tokenExists = await _toolkit.ContractExistsAsync(result.DeployedContracts["token"].ContractHash.ToString());
        var dexExists = await _toolkit.ContractExistsAsync(result.DeployedContracts["dex"].ContractHash.ToString());
        
        Assert.True(tokenExists);
        Assert.True(dexExists);
    }

    [SkippableFact]
    public async Task DeployMultipleContracts_FromManifestFile_ShouldSucceed()
    {
        Skip.If(string.IsNullOrEmpty(_wifKey), "No WIF key provided for testing");

        // Arrange
        var manifestPath = Path.Combine(_testDataPath, "deployment-manifest.json");
        var manifest = new DeploymentManifest
        {
            Name = "File-based Deployment Test",
            Version = "1.0",
            Contracts = new List<ContractDefinition>
            {
                new()
                {
                    Id = "governance",
                    Name = "TestGovernance",
                    ProjectPath = Path.Combine(_testDataPath, "TestGovernance.cs"),
                    Tags = { "governance", "core" }
                },
                new()
                {
                    Id = "treasury",
                    Name = "TestTreasury",
                    ProjectPath = Path.Combine(_testDataPath, "TestTreasury.cs"),
                    Dependencies = { "governance" },
                    InitParams = new List<object> { "@contract:governance" },
                    Tags = { "treasury", "finance" }
                }
            },
            Settings = new DeploymentSettings
            {
                VerifyAfterDeploy = true,
                WaitForConfirmation = true
            }
        };

        // Save manifest to file
        var json = System.Text.Json.JsonSerializer.Serialize(manifest, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        });
        await File.WriteAllTextAsync(manifestPath, json);

        // Act
        var result = await _toolkit.DeployFromManifestAsync(manifestPath);

        // Assert
        Assert.Equal(DeploymentStatus.Completed, result.Status);
        Assert.Equal(2, result.DeployedContracts.Count);
        Assert.Equal(0, result.FailedDeployments.Count);
    }

    [SkippableFact]
    public async Task DeployMultipleContracts_WithBatching_ShouldSucceed()
    {
        Skip.If(string.IsNullOrEmpty(_wifKey), "No WIF key provided for testing");

        // Arrange
        var manifest = _toolkit.CreateManifestBuilder()
            .WithName("Batch Deployment Test")
            .EnableBatching(3)
            .AddContract("contract1", "Contract1", Path.Combine(_testDataPath, "SimpleContract1.cs"))
            .AddContract("contract2", "Contract2", Path.Combine(_testDataPath, "SimpleContract2.cs"))
            .AddContract("contract3", "Contract3", Path.Combine(_testDataPath, "SimpleContract3.cs"))
            .AddContract("contract4", "Contract4", Path.Combine(_testDataPath, "SimpleContract4.cs"), c => c
                .DependsOn("contract1", "contract2"))
            .Build();

        // Act
        var result = await _toolkit.DeployMultipleAsync(manifest);

        // Assert
        Assert.Equal(DeploymentStatus.Completed, result.Status);
        Assert.Equal(4, result.DeployedContracts.Count);
        Assert.True(result.Summary.Duration.HasValue);
        Assert.True(result.Summary.Duration.Value.TotalSeconds > 0);
    }

    [SkippableFact]
    public async Task DeployMultipleContracts_WithPartialFailure_ShouldHandleGracefully()
    {
        Skip.If(string.IsNullOrEmpty(_wifKey), "No WIF key provided for testing");

        // Arrange
        var manifest = _toolkit.CreateManifestBuilder()
            .WithName("Partial Failure Test")
            .ContinueOnError(true)
            .AddContract("valid", "ValidContract", Path.Combine(_testDataPath, "SimpleContract1.cs"))
            .AddContract("invalid", "InvalidContract", "non-existent-path.cs")
            .AddContract("dependent", "DependentContract", Path.Combine(_testDataPath, "SimpleContract2.cs"), c => c
                .DependsOn("valid"))
            .Build();

        // Act
        var result = await _toolkit.DeployMultipleAsync(manifest);

        // Assert
        Assert.Equal(DeploymentStatus.PartiallyCompleted, result.Status);
        Assert.Equal(2, result.DeployedContracts.Count);
        Assert.Equal(1, result.FailedDeployments.Count);
        Assert.True(result.FailedDeployments.ContainsKey("invalid"));
        Assert.True(result.DeployedContracts.ContainsKey("valid"));
        Assert.True(result.DeployedContracts.ContainsKey("dependent"));
    }

    [SkippableFact]
    public async Task ResolveContractDependencies_WithComplexGraph_ShouldReturnCorrectOrder()
    {
        Skip.If(string.IsNullOrEmpty(_wifKey), "No WIF key provided for testing");

        // Arrange
        var contracts = new List<ContractDefinition>
        {
            new() { Id = "ui", Dependencies = { "api", "auth" } },
            new() { Id = "api", Dependencies = { "database", "cache" } },
            new() { Id = "auth", Dependencies = { "database" } },
            new() { Id = "cache", Dependencies = { } },
            new() { Id = "database", Dependencies = { } }
        };

        // Act
        var ordered = _toolkit.ResolveDependencyOrder(contracts);

        // Assert
        Assert.Equal(5, ordered.Count);
        
        // Create order map
        var orderMap = new Dictionary<string, int>();
        for (int i = 0; i < ordered.Count; i++)
        {
            orderMap[ordered[i].Id] = i;
        }
        
        // Verify dependencies are respected
        Assert.True(orderMap["database"] < orderMap["api"]);
        Assert.True(orderMap["database"] < orderMap["auth"]);
        Assert.True(orderMap["cache"] < orderMap["api"]);
        Assert.True(orderMap["api"] < orderMap["ui"]);
        Assert.True(orderMap["auth"] < orderMap["ui"]);
    }

    private async Task CreateTestContractsAsync()
    {
        // Create simple test contract files
        var contracts = new Dictionary<string, string>
        {
            ["TestToken.cs"] = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace TestContracts
{
    [Contract]
    public class TestToken : Nep17Token
    {
        public override string Symbol => ""TST"";
        public override byte Decimals => 8;
        
        [InitialValue(""1000000000_00000000"", ContractParameterType.Integer)]
        private static readonly ulong InitialSupply;
        
        public static void _deploy(object data, bool update)
        {
            if (!update)
            {
                var deployer = Runtime.ExecutingScriptHash;
                Mint(deployer, InitialSupply);
            }
        }
        
        public static bool Approve(UInt160 spender, ulong amount)
        {
            return true;
        }
    }
}",

            ["TestDEX.cs"] = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace TestContracts
{
    [Contract]
    public class TestDEX : SmartContract
    {
        private static StorageMap TokenStorage => new(Storage.CurrentContext, ""Tokens"");
        
        public static void _deploy(object data, bool update)
        {
            if (!update && data is object[] args && args.Length >= 2)
            {
                var tokenHash = (UInt160)args[0];
                var initialLiquidity = (ulong)args[1];
                TokenStorage.Put(""token"", tokenHash);
            }
        }
        
        public static UInt160 GetToken()
        {
            return (UInt160)TokenStorage.Get(""token"");
        }
    }
}",

            ["TestGovernance.cs"] = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace TestContracts
{
    [Contract]
    public class TestGovernance : SmartContract
    {
        private static StorageMap ConfigStorage => new(Storage.CurrentContext, ""Config"");
        
        public static void Initialize()
        {
            ConfigStorage.Put(""initialized"", 1);
        }
        
        public static bool IsInitialized()
        {
            return ConfigStorage.Get(""initialized"") != null;
        }
    }
}",

            ["TestTreasury.cs"] = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace TestContracts
{
    [Contract]
    public class TestTreasury : SmartContract
    {
        private static StorageMap ConfigStorage => new(Storage.CurrentContext, ""Config"");
        
        public static void _deploy(object data, bool update)
        {
            if (!update && data is object[] args && args.Length >= 1)
            {
                var governanceHash = (UInt160)args[0];
                ConfigStorage.Put(""governance"", governanceHash);
            }
        }
        
        public static UInt160 GetGovernance()
        {
            return (UInt160)ConfigStorage.Get(""governance"");
        }
    }
}",

            ["SimpleContract1.cs"] = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace TestContracts
{
    [Contract]
    public class SimpleContract1 : SmartContract
    {
        public static string GetName() => ""SimpleContract1"";
    }
}",

            ["SimpleContract2.cs"] = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace TestContracts
{
    [Contract]
    public class SimpleContract2 : SmartContract
    {
        public static string GetName() => ""SimpleContract2"";
    }
}",

            ["SimpleContract3.cs"] = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace TestContracts
{
    [Contract]
    public class SimpleContract3 : SmartContract
    {
        public static string GetName() => ""SimpleContract3"";
    }
}",

            ["SimpleContract4.cs"] = @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace TestContracts
{
    [Contract]
    public class SimpleContract4 : SmartContract
    {
        public static string GetName() => ""SimpleContract4"";
    }
}"
        };

        foreach (var kvp in contracts)
        {
            await File.WriteAllTextAsync(Path.Combine(_testDataPath, kvp.Key), kvp.Value);
        }
    }
}