using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Neo;
using Neo.Network.RPC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;
using CompilationOptions = Neo.Compiler.CompilationOptions;

namespace Neo.SmartContract.Deploy.UnitTests;

public class DeploymentToolkitTests : TestBase
{
    private const string ValidWif = "KzjaqMvqzF1uup6KrTKRxTgjcXE7PbKLRH84e6ckyXDt3fu7afUb";

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
        Assert.Equal("https://rpc10.n3.nspcc.ru:10331", toolkit.CurrentNetwork.RpcUrl);
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
        Assert.Equal("http://seed2t5.neo.org:20332", toolkit.CurrentNetwork.RpcUrl);
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
        Assert.Equal("http://localhost:50012", toolkit.CurrentNetwork.RpcUrl);
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
        Assert.Equal(customRpc, toolkit.CurrentNetwork.RpcUrl);
    }

    [Fact]
    public void UseNetwork_WithProfile_ShouldUpdateCurrentNetwork()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act
        var result = toolkit.UseNetwork(NetworkProfile.MainNet);

        // Assert
        Assert.Same(toolkit, result);
        Assert.Equal(NetworkProfile.MainNet.RpcUrl, toolkit.CurrentNetwork.RpcUrl);
    }

    [Fact]
    public async Task DeployFromManifestAsync_ShouldDeployAllContracts()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            var nef1 = Path.Combine(tempDir, "First.nef");
            var manifest1 = Path.Combine(tempDir, "First.manifest.json");
            var nef2 = Path.Combine(tempDir, "Second.nef");
            var manifest2 = Path.Combine(tempDir, "Second.manifest.json");

            File.WriteAllText(nef1, string.Empty);
            File.WriteAllText(manifest1, "{}");
            File.WriteAllText(nef2, string.Empty);
            File.WriteAllText(manifest2, "{}");

            var manifestPath = Path.Combine(tempDir, "deployment.json");
            var manifestContent = System.Text.Json.JsonSerializer.Serialize(new
            {
                network = "mainnet",
                wif = ValidWif,
                waitForConfirmation = true,
                confirmationRetries = 5,
                confirmationDelaySeconds = 1,
                contracts = new object?[]
                {
                    new
                    {
                        name = "First",
                        nef = Path.GetFileName(nef1),
                        manifest = Path.GetFileName(manifest1),
                        initParams = new object?[] { "admin", 42, true }
                    },
                    new
                    {
                        name = "Second",
                        nef = Path.GetFileName(nef2),
                        manifest = Path.GetFileName(manifest2),
                        waitForConfirmation = false
                    }
                }
            });
            File.WriteAllText(manifestPath, manifestContent);

            var options = new DeploymentOptions
            {
                Network = NetworkProfile.TestNet,
                WaitForConfirmation = false,
                ConfirmationRetries = 3,
                ConfirmationDelaySeconds = 1
            };

            var toolkit = new TestDeploymentToolkit(options);
            toolkit.SetWifKey(ValidWif);

            // Act
            var deployments = await toolkit.DeployFromManifestAsync(manifestPath);

            // Assert
            Assert.Equal(2, toolkit.Calls.Count);
            Assert.Equal(NetworkProfile.TestNet.RpcUrl, toolkit.CurrentNetwork.RpcUrl);
            var first = toolkit.Calls[0];
            Assert.Equal(Path.GetFullPath(nef1), first.NefPath);
            Assert.Equal(Path.GetFullPath(manifest1), first.ManifestPath);
            Assert.Equal(new object?[] { "admin", 42L, true }, first.Parameters);
            Assert.True(first.WaitForConfirmation);
            Assert.Equal(5, first.ConfirmationRetries);
            Assert.Equal(1, first.ConfirmationDelaySeconds);

            var second = toolkit.Calls[1];
            Assert.Equal(Path.GetFullPath(nef2), second.NefPath);
            Assert.False(second.WaitForConfirmation);
            Assert.Equal(5, second.ConfirmationRetries); // inherits manifest value
            Assert.Equal(1, second.ConfirmationDelaySeconds);

            Assert.Equal(2, deployments.Count);
            Assert.True(deployments.ContainsKey("First"));
            Assert.True(deployments.ContainsKey("Second"));
        }
        finally
        {
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
        }
    }

    [Fact]
    public async Task DeployAsync_WithUnsupportedExtension_ShouldThrow()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var tempFile = Path.GetTempFileName();
        var invalidPath = Path.ChangeExtension(tempFile, ".txt");
        File.Move(tempFile, invalidPath);

        // Act & Assert
        try
        {
            await Assert.ThrowsAsync<NotSupportedException>(() => toolkit.DeployAsync(invalidPath));
        }
        finally
        {
            if (File.Exists(invalidPath)) File.Delete(invalidPath);
        }
    }

    [Fact]
    public async Task DeployAsync_ShouldCompileAndDeployArtifact()
    {
        // Arrange
        var toolkit = new TestDeploymentToolkit();
        toolkit.SetWifKey(ValidWif);
        toolkit.CompileArtifacts = new[] { CreateDummyArtifact("TestContract") };

        var tempProject = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".csproj");
        File.WriteAllText(tempProject, "<Project></Project>");

        try
        {
            var deployment = await toolkit.DeployAsync(tempProject, new object?[] { 1, "value" });

            Assert.Single(toolkit.Calls);
            var call = toolkit.Calls[0];
            Assert.NotNull(call.Parameters);
            Assert.Equal(2, call.Parameters!.Length);
            Assert.Equal(1L, Convert.ToInt64(call.Parameters[0]!));
            Assert.Equal("value", call.Parameters[1]);
            Assert.NotNull(deployment);
        }
        finally
        {
            if (File.Exists(tempProject)) File.Delete(tempProject);
        }
    }

    [Fact]
    public async Task DeployArtifactsAsync_RequestOverload_ShouldRespectRequestOptions()
    {
        // Arrange
        var toolkit = new TestDeploymentToolkit();
        toolkit.SetWifKey(ValidWif);

        var nef = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".nef");
        var manifest = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".manifest.json");
        File.WriteAllText(nef, string.Empty);
        File.WriteAllText(manifest, "{}");

        try
        {
            var request = new DeploymentArtifactsRequest(nef, manifest)
                .WithInitParams("owner", 123)
                .WithConfirmationPolicy(true, retries: 9, delaySeconds: 3);

            await toolkit.DeployArtifactsAsync(request);

            Assert.Single(toolkit.Calls);
            var call = toolkit.Calls[0];
            Assert.NotNull(call.Parameters);
            Assert.Equal("owner", call.Parameters![0]);
            Assert.Equal(123L, Convert.ToInt64(call.Parameters[1]!));
            Assert.True(call.WaitForConfirmation);
            Assert.Equal(9, call.ConfirmationRetries);
            Assert.Equal(3, call.ConfirmationDelaySeconds);
        }
        finally
        {
            if (File.Exists(nef)) File.Delete(nef);
            if (File.Exists(manifest)) File.Delete(manifest);
        }
    }

    [Fact]
    public async Task CompileAsync_ShouldReturnArtifacts()
    {
        // Arrange
        var toolkit = new TestDeploymentToolkit();
        toolkit.CompileArtifacts = new[] { CreateDummyArtifact("TestContract") };

        var tempSource = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".cs");
        File.WriteAllText(tempSource, "// dummy");

        try
        {
            var artifacts = await toolkit.CompileAsync(tempSource);
            Assert.Single(artifacts);
            Assert.Equal("TestContract", artifacts[0].ContractName);
        }
        finally
        {
            if (File.Exists(tempSource)) File.Delete(tempSource);
        }
    }

    [Fact(Skip = "GetGasBalance is implemented; requires RPC to run")]
    public async Task GetGasBalance_WithoutImplementation_ShouldThrowNotImplementedException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var testAddress = "NXXxXXxXXxXXxXXxXXxXXxXXxXXxXXxXXxX";

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(
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

    [Fact(Skip = "ContractExistsAsync is implemented; requires RPC to run")]
    public async Task ContractExistsAsync_WithoutImplementation_ShouldThrowNotImplementedException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var contractHash = "0x1234567890123456789012345678901234567890";

        toolkit.SetNetwork("testnet");

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(
            () => toolkit.ContractExistsAsync(contractHash)
        );
    }

    #endregion

    #region Network Magic Tests

    [Fact(Skip = "CallAsync is implemented; this test requires RPC to validate magic retrieval path")]
    public async Task UpdateAsync_ShouldRetrieveNetworkMagicFromRpc_WhenNotConfigured()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        toolkit.SetNetwork("testnet");

        // Act & Assert
        // This test verifies that when NetworkMagic is not configured,
        // the toolkit will attempt to retrieve it from RPC
        // Currently throws NotImplementedException, but the framework is in place
        await Assert.ThrowsAsync<NotImplementedException>(
            () => toolkit.CallAsync<string>("0x1234567890123456789012345678901234567890", "test")
        );
    }

    [Fact]
    public void SetNetwork_WithKnownNetworks_ShouldConfigureCorrectRpcUrl()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var testCases = new Dictionary<string, string>
        {
            { "mainnet", "https://rpc10.n3.nspcc.ru:10331" },
            { "testnet", "http://seed2t5.neo.org:20332" },
            { "local", "http://localhost:50012" },
            { "private", "http://localhost:50012" }
        };

        foreach (var testCase in testCases)
        {
            // Act
            toolkit.SetNetwork(testCase.Key);

            // Assert
            Assert.Equal(testCase.Value, toolkit.CurrentNetwork.RpcUrl);
        }
    }

    [Fact]
    public void SetNetwork_WithHttpUrl_ShouldUseAsRpcUrl()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var customUrls = new[]
        {
            "http://localhost:10332",
            "https://custom.neo.rpc:443",
            "http://192.168.1.100:10332"
        };

        foreach (var url in customUrls)
        {
            // Act
            toolkit.SetNetwork(url);

            // Assert
            Assert.Equal(url, toolkit.CurrentNetwork.RpcUrl);
        }
    }

    [Fact]
    public void SetNetwork_ShouldBeCaseInsensitive()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var variations = new[] { "MAINNET", "MainNet", "mainnet", "MaInNeT" };

        foreach (var variation in variations)
        {
            // Act
            toolkit.SetNetwork(variation);

            // Assert
            Assert.Equal("https://rpc10.n3.nspcc.ru:10331", toolkit.CurrentNetwork.RpcUrl);
        }
    }

    [Fact]
    public void SetNetwork_WithEmptyOrNull_ShouldThrowArgumentException()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => toolkit.SetNetwork(""));
        Assert.Throws<ArgumentException>(() => toolkit.SetNetwork("   "));
        Assert.Throws<ArgumentException>(() => toolkit.SetNetwork(null!));
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

    private sealed class TestDeploymentToolkit : DeploymentToolkit
    {
        public List<DeploymentCall> Calls { get; } = new();
        public IReadOnlyList<CompiledContractArtifact>? CompileArtifacts { get; set; }

        public TestDeploymentToolkit(DeploymentOptions? options = null)
            : base(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build(), options, new NoopRpcClientFactory())
        {
        }

        public override Task<ContractDeploymentInfo> DeployArtifactsAsync(DeploymentArtifactsRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            return DeployArtifactsAsync(
                request.NefPath,
                request.ManifestPath,
                request.InitParams,
                request.WaitForConfirmation,
                request.ConfirmationRetries,
                request.ConfirmationDelaySeconds,
                cancellationToken);
        }

        protected override Task<IReadOnlyList<CompiledContractArtifact>> CompileContractsAsync(string path, CompilationOptions compilationOptions, string? targetContractName, CancellationToken cancellationToken)
        {
            if (CompileArtifacts is null)
            {
                throw new InvalidOperationException("CompileArtifacts must be set for TestDeploymentToolkit.");
            }

            return Task.FromResult(CompileArtifacts);
        }

        public override Task<ContractDeploymentInfo> DeployArtifactsAsync(string nefPath, string manifestPath, object?[]? initParams = null, bool? waitForConfirmation = null, int? confirmationRetries = null, int? confirmationDelaySeconds = null, CancellationToken cancellationToken = default)
        {
            Calls.Add(new DeploymentCall(
                Path.GetFullPath(nefPath),
                Path.GetFullPath(manifestPath),
                initParams,
                waitForConfirmation,
                confirmationRetries,
                confirmationDelaySeconds));

            return Task.FromResult(new ContractDeploymentInfo
            {
                ContractHash = UInt160.Zero,
                TransactionHash = UInt256.Zero
            });
        }
    }

    private sealed class NoopRpcClientFactory : IRpcClientFactory
    {
        public RpcClient Create(Uri uri, ProtocolSettings protocolSettings)
            => throw new InvalidOperationException("RPC should not be invoked during unit tests.");
    }

    private sealed record DeploymentCall(
        string NefPath,
        string ManifestPath,
        object?[]? Parameters,
        bool? WaitForConfirmation,
        int? ConfirmationRetries,
        int? ConfirmationDelaySeconds);

    private static DeploymentToolkit.CompiledContractArtifact CreateDummyArtifact(string name)
    {
        var script = new byte[] { (byte)OpCode.RET };
        var nef = new NefFile
        {
            Compiler = "unit-test",
            Source = string.Empty,
            Tokens = Array.Empty<MethodToken>(),
            Script = script
        };
        nef.CheckSum = NefFile.ComputeChecksum(nef);

        var manifest = new ContractManifest
        {
            Name = name,
            Groups = Array.Empty<ContractGroup>(),
            SupportedStandards = Array.Empty<string>(),
            Abi = new ContractAbi
            {
                Methods = new[]
                {
                    new ContractMethodDescriptor
                    {
                        Name = "main",
                        Parameters = Array.Empty<ContractParameterDefinition>(),
                        ReturnType = ContractParameterType.Void,
                        Offset = 0,
                        Safe = true
                    }
                },
                Events = Array.Empty<ContractEventDescriptor>()
            },
            Permissions = new[] { ContractPermission.DefaultPermission },
            Trusts = WildcardContainer<ContractPermissionDescriptor>.CreateWildcard(),
            Extra = new JObject()
        };

        return new DeploymentToolkit.CompiledContractArtifact(name, nef, manifest);
    }
}
