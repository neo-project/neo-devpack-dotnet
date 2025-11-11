using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Neo;
using Neo.Network.RPC;
using Neo.Network.P2P.Payloads;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Neo.Json;
using Neo.Cryptography.ECC;
using Neo.Cryptography;
using Neo.Network.RPC.Models;
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
        Assert.Equal("https://mainnet1.neo.coz.io:443", toolkit.CurrentNetwork.RpcUrl);
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
    public void ConfigureOptions_ShouldUpdateToolkitAndReturnClone()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act
        toolkit.ConfigureOptions(options =>
        {
            options.WaitForConfirmation = true;
            options.ConfirmationRetries = 99;
            options.ConfirmationDelaySeconds = 7;
        });

        // Assert
        var configured = toolkit.Options;
        Assert.True(configured.WaitForConfirmation);
        Assert.Equal(99, configured.ConfirmationRetries);
        Assert.Equal(7, configured.ConfirmationDelaySeconds);

        configured.WaitForConfirmation = false;
        configured.ConfirmationRetries = 1;

        var reloaded = toolkit.Options;
        Assert.True(reloaded.WaitForConfirmation);
        Assert.Equal(99, reloaded.ConfirmationRetries);
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
    public async Task DeployFromManifestAsync_WithNonArrayInitParams_ShouldThrow()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            var nef = Path.Combine(tempDir, "Invalid.nef");
            var manifest = Path.Combine(tempDir, "Invalid.manifest.json");
            File.WriteAllText(nef, string.Empty);
            File.WriteAllText(manifest, "{}");

            var manifestPath = Path.Combine(tempDir, "deployment.json");
            var manifestContent = System.Text.Json.JsonSerializer.Serialize(new
            {
                network = "mainnet",
                wif = ValidWif,
                contracts = new object?[]
                {
                    new
                    {
                        name = "InvalidInit",
                        nef = Path.GetFileName(nef),
                        manifest = Path.GetFileName(manifest),
                        initParams = new { owner = "admin" }
                    }
                }
            });
            File.WriteAllText(manifestPath, manifestContent);

            var toolkit = new TestDeploymentToolkit();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => toolkit.DeployFromManifestAsync(manifestPath));
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
    public async Task DeployFromManifestAsync_WithTypedParameters_ShouldParseContractParameters()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            var nef = Path.Combine(tempDir, "Typed.nef");
            var manifest = Path.Combine(tempDir, "Typed.manifest.json");
            File.WriteAllText(nef, string.Empty);
            File.WriteAllText(manifest, "{}");

            var manifestPath = Path.Combine(tempDir, "deployment.json");
            var manifestContent = System.Text.Json.JsonSerializer.Serialize(new
            {
                contracts = new object?[]
                {
                    new
                    {
                        name = "Typed",
                        nef = Path.GetFileName(nef),
                        manifest = Path.GetFileName(manifest),
                        initParams = new object[]
                        {
                            new { type = "Hash160", value = UInt160.Zero.ToString() },
                            new { type = "Integer", value = "5" }
                        }
                    }
                }
            });
            File.WriteAllText(manifestPath, manifestContent);

            var toolkit = new TestDeploymentToolkit();
            await toolkit.DeployFromManifestAsync(manifestPath);

            var call = Assert.Single(toolkit.Calls);
            Assert.NotNull(call.Parameters);
            var parameters = call.Parameters!;

            var hashParam = Assert.IsType<ContractParameter>(parameters[0]);
            Assert.Equal(ContractParameterType.Hash160, hashParam.Type);
            Assert.Equal(UInt160.Zero, hashParam.Value);

            var intParam = Assert.IsType<ContractParameter>(parameters[1]);
            Assert.Equal(ContractParameterType.Integer, intParam.Type);
            Assert.Equal(new BigInteger(5), (BigInteger)intParam.Value);
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
        toolkit.SetNetwork("testnet");
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

    [Fact]
    public async Task GetGasBalanceAsync_ShouldReturnValueFromNep17()
    {
        // Arrange
        var invokeResponses = new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["invokescript"] = new Queue<JToken>(new[]
            {
                CreateInvokeResultJson(new Neo.VM.Types.Integer(new System.Numerics.BigInteger(12345))),
                CreateInvokeResultJson(new Neo.VM.Types.Integer(new System.Numerics.BigInteger(2)))
            })
        };

        var factory = CreateFactoryWithVersion(
            NetworkProfile.TestNet.NetworkMagic ?? 894710606u,
            settings => new StubRpcClient(invokeResponses, settings));

        var options = new DeploymentOptions { Network = NetworkProfile.TestNet };
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var toolkit = new DeploymentToolkit(config, options, factory);

        // Act
        var balance = await toolkit.GetGasBalanceAsync("0x11223344556677889900AABBCCDDEEFF00112233");

        // Assert
        Assert.Equal(123.45m, balance);
        Assert.Empty(invokeResponses["invokescript"]);
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
    public async Task ContractExistsAsync_ShouldReturnTrue_WhenContractStateFound()
    {
        // Arrange
        var contractState = CreateDummyContractState("ExistingContract");
        var responses = new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["getcontractstate"] = new Queue<JToken>(new[] { contractState.ToJson() })
        };

        var factory = CreateFactoryWithVersion(
            NetworkProfile.TestNet.NetworkMagic ?? 894710606u,
            settings => new StubRpcClient(responses, settings));

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var toolkit = new DeploymentToolkit(config, new DeploymentOptions { Network = NetworkProfile.TestNet }, factory);

        // Act
        var exists = await toolkit.ContractExistsAsync(contractState.Hash.ToString());

        // Assert
        Assert.True(exists);
        Assert.Empty(responses["getcontractstate"]);
    }

    [Fact]
    public async Task ContractExistsAsync_ShouldPropagate_WhenRpcThrowsUnexpectedError()
    {
        // Arrange
        var handlers = new Dictionary<string, Func<JToken[], JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["getcontractstate"] = _ => throw new InvalidOperationException("not found")
        };

        var factory = CreateFactoryWithVersion(
            NetworkProfile.TestNet.NetworkMagic ?? 894710606u,
            settings => new StubRpcClient(new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase), settings, handlers));

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var toolkit = new DeploymentToolkit(config, new DeploymentOptions { Network = NetworkProfile.TestNet }, factory);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            toolkit.ContractExistsAsync("0x0123456789abcdef0123456789abcdef01234567"));
    }

    [Fact]
    public async Task ContractExistsAsync_ShouldReturnFalse_WhenRpcReportsUnknownContract()
    {
        // Arrange
        var handlers = new Dictionary<string, Func<JToken[], JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["getcontractstate"] = _ => throw new RpcException(-102, "unknown contract")
        };

        var factory = CreateFactoryWithVersion(
            NetworkProfile.TestNet.NetworkMagic ?? 894710606u,
            settings => new StubRpcClient(new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase), settings, handlers));

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var toolkit = new DeploymentToolkit(config, new DeploymentOptions { Network = NetworkProfile.TestNet }, factory);

        // Act
        var exists = await toolkit.ContractExistsAsync("0x0123456789abcdef0123456789abcdef01234567");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task NetworkMagic_ShouldFallbackToKnownProfile_WhenRpcUnavailable()
    {
        // Arrange
        var contractState = CreateDummyContractState("FallbackTest");
        var responses = new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["getcontractstate"] = new Queue<JToken>(new[] { contractState.ToJson() })
        };

        var failingHandlers = new Dictionary<string, Func<JToken[], JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["getversion"] = _ => throw new InvalidOperationException("unavailable")
        };

        var successHandlers = new Dictionary<string, Func<JToken[], JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["getversion"] = _ => CreateVersionResponse(NetworkProfile.TestNet.NetworkMagic ?? 894710606u)
        };

        var factory = new QueueRpcClientFactory(new Func<ProtocolSettings, RpcClient>[]
        {
            settings => new StubRpcClient(new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase), settings, failingHandlers),
            settings => new StubRpcClient(responses, settings, successHandlers)
        });

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var toolkit = new DeploymentToolkit(config, new DeploymentOptions { Network = NetworkProfile.TestNet }, factory);

        // Act
        var exists = await toolkit.ContractExistsAsync(contractState.Hash.ToString());

        // Assert
        Assert.True(exists);
        Assert.Equal(NetworkProfile.TestNet.NetworkMagic, toolkit.CurrentNetwork.NetworkMagic);
    }

    [Fact]
    public async Task GetNetworkMagicAsync_ShouldThrottleRpcRetries()
    {
        var failingHandlers = new Dictionary<string, Func<JToken[], JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["getversion"] = _ => throw new InvalidOperationException("RPC unavailable")
        };

        var factory = new QueueRpcClientFactory(new Func<ProtocolSettings, RpcClient>[]
        {
            settings => new StubRpcClient(new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase), settings, failingHandlers)
        });

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var toolkit = new DeploymentToolkit(config, new DeploymentOptions { Network = NetworkProfile.TestNet }, factory);

        var intervalProp = typeof(DeploymentToolkit).GetProperty("NetworkMagicRetryInterval", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        var originalInterval = (TimeSpan)intervalProp!.GetValue(null)!;

        try
        {
            intervalProp.SetValue(null, TimeSpan.FromSeconds(30));

            var first = await InvokeNetworkMagicAsync(toolkit);
            Assert.Equal<uint>(NetworkProfile.TestNet.NetworkMagic ?? 894710606, first);

            var second = await InvokeNetworkMagicAsync(toolkit);
            Assert.Equal(first, second);
        }
        finally
        {
            intervalProp.SetValue(null, originalInterval);
        }
    }

    [Fact]
    public async Task GetNetworkMagicAsync_ShouldRetryRpcAfterCooldown()
    {
        var failingHandlers = new Dictionary<string, Func<JToken[], JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["getversion"] = _ => throw new InvalidOperationException("down")
        };

        var successMagic = 0x1234ABCDu;
        var successHandlers = new Dictionary<string, Func<JToken[], JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["getversion"] = _ => CreateVersionResponse(successMagic)
        };

        var factory = new QueueRpcClientFactory(new Func<ProtocolSettings, RpcClient>[]
        {
            settings => new StubRpcClient(new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase), settings, failingHandlers),
            settings => new StubRpcClient(new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase), settings, successHandlers)
        });

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var toolkit = new DeploymentToolkit(config, new DeploymentOptions { Network = NetworkProfile.TestNet }, factory);

        var intervalProp = typeof(DeploymentToolkit).GetProperty("NetworkMagicRetryInterval", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        var originalInterval = (TimeSpan)intervalProp!.GetValue(null)!;

        try
        {
            intervalProp.SetValue(null, TimeSpan.FromMilliseconds(50));

            var first = await InvokeNetworkMagicAsync(toolkit);
            Assert.Equal<uint>(NetworkProfile.TestNet.NetworkMagic ?? 894710606, first);

            await Task.Delay((TimeSpan)intervalProp.GetValue(null)! + TimeSpan.FromMilliseconds(25));

            var second = await InvokeNetworkMagicAsync(toolkit);
            Assert.Equal<uint>(successMagic, second);
            Assert.Equal<uint?>(successMagic, toolkit.CurrentNetwork.NetworkMagic);
        }
        finally
        {
            intervalProp.SetValue(null, originalInterval);
        }
    }

    [Fact]
    public async Task DeployArtifactsAsync_ShouldUseProvidedSignersAndTransactionSigner()
    {
        // Arrange
        var artifact = CreateDummyArtifact("SignerUsage");
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);
        var nefPath = Path.Combine(tempDir, "SignerUsage.nef");
        var manifestPath = Path.Combine(tempDir, "SignerUsage.manifest.json");

        using (var ms = new MemoryStream())
        using (var writer = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
        {
            artifact.Nef.Serialize(writer);
            writer.Flush();
            File.WriteAllBytes(nefPath, ms.ToArray());
        }

        File.WriteAllText(manifestPath, artifact.Manifest.ToJson().ToString());

        var options = new DeploymentOptions
        {
            WaitForConfirmation = false
        };


        var rpcHandlers = new Dictionary<string, Func<JToken[], JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["getversion"] = _ => CreateVersionResponse(NetworkProfile.TestNet.NetworkMagic ?? 894710606u),
            ["sendrawtransaction"] = _ => new JObject { ["hash"] = new JString(UInt256.Zero.ToString()) },
            ["calculatenetworkfee"] = _ => new JObject { ["networkfee"] = new JNumber(0) }
        };

        var versionHandlers = new Dictionary<string, Func<JToken[], JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["getversion"] = _ => CreateVersionResponse(NetworkProfile.TestNet.NetworkMagic ?? 894710606u)
        };

        var factory = new QueueRpcClientFactory(new Func<ProtocolSettings, RpcClient>[]
        {
            settings => new StubRpcClient(new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase), settings, versionHandlers),
            settings => new StubRpcClient(new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase), settings, rpcHandlers)
        });

        var toolkit = new RecordingDeploymentToolkit(options, factory);

        var signerAccount = UInt160.Parse("0x0123456789abcdef0123456789abcdef01234567");
        var signer = new Signer { Account = signerAccount, Scopes = WitnessScope.CalledByEntry };
        var signerInvoked = false;

        try
        {
            var result = await toolkit.DeployArtifactsAsync(
                nefPath,
                manifestPath,
                initParams: null,
                waitForConfirmation: false,
                confirmationRetries: null,
                confirmationDelaySeconds: null,
                cancellationToken: CancellationToken.None,
                signers: new[] { signer },
                transactionSignerAsync: (tm, ct) =>
                {
                    signerInvoked = true;
                    return Task.FromResult(new Transaction
                    {
                        Script = tm.Tx.Script,
                        Signers = tm.Tx.Signers,
                        Attributes = Array.Empty<TransactionAttribute>(),
                        Witnesses = Array.Empty<Witness>()
                    });
                });

            Assert.NotNull(result.TransactionHash);
            Assert.NotNull(result.ContractHash);
            Assert.True(signerInvoked);
            Assert.NotNull(toolkit.ObservedSigners);
            Assert.Single(toolkit.ObservedSigners!);
            Assert.Equal(signerAccount, toolkit.ObservedSigners![0].Account);
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public async Task DeployFromManifestAsync_ShouldThrowOnDuplicateContractKeys()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDir);

        try
        {
            var nef1 = Path.Combine(tempDir, "Dup.nef");
            var manifest1 = Path.Combine(tempDir, "Dup.manifest.json");
            File.WriteAllText(nef1, string.Empty);
            File.WriteAllText(manifest1, "{}");

            var nef2 = Path.Combine(tempDir, "Dup2.nef");
            var manifest2 = Path.Combine(tempDir, "Dup2.manifest.json");
            File.WriteAllText(nef2, string.Empty);
            File.WriteAllText(manifest2, "{}");

            var manifestPath = Path.Combine(tempDir, "deployment.json");
            var manifestContent = System.Text.Json.JsonSerializer.Serialize(new
            {
                wif = ValidWif,
                contracts = new object?[]
                {
                    new { name = "Duplicate", nef = Path.GetFileName(nef1), manifest = Path.GetFileName(manifest1) },
                    new { name = "Duplicate", nef = Path.GetFileName(nef2), manifest = Path.GetFileName(manifest2) }
                }
            });
            File.WriteAllText(manifestPath, manifestContent);

            var toolkit = new TestDeploymentToolkit();
            toolkit.SetWifKey(ValidWif);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => toolkit.DeployFromManifestAsync(manifestPath));
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }

    #endregion

    #region Network Magic Tests

    [Fact]
    public async Task GetProtocolSettingsAsync_WithExplicitNetworkMagic_ShouldPreserveProfileValue()
    {
        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var profile = new NetworkProfile("custom", "http://localhost:50012", 123u, 0x42);
        var options = new DeploymentOptions { Network = profile };

        using var toolkit = new DeploymentToolkit(config, options, new NoopRpcClientFactory());
        var method = typeof(DeploymentToolkit).GetMethod("GetProtocolSettingsAsync", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("GetProtocolSettingsAsync not found.");

        var task = (Task<ProtocolSettings>)method.Invoke(toolkit, Array.Empty<object>())!;
        var settings = await task;

        Assert.Equal(123u, settings.Network);
        Assert.Equal(0x42, settings.AddressVersion);
    }

    [Fact]
    public async Task CallAsync_ShouldRetrieveNetworkMagicFromRpc_WhenNotConfigured()
    {
        // Arrange
        var version = new RpcVersion
        {
            TcpPort = 20332,
            Nonce = 1,
            UserAgent = "/Neo:unit-test/",
            Protocol = new RpcVersion.RpcProtocol
            {
                Network = 0xCAFE_BABE,
                ValidatorsCount = 7,
                MillisecondsPerBlock = 15000,
                MaxValidUntilBlockIncrement = 86400000,
                MaxTraceableBlocks = 100000,
                AddressVersion = 0x35,
                MaxTransactionsPerBlock = 512,
                MemoryPoolMaxTransactions = 5000,
                InitialGasDistribution = 5200000000000000,
                Hardforks = new Dictionary<Hardfork, uint>(),
                SeedList = Array.Empty<string>(),
                StandbyCommittee = Array.Empty<ECPoint>()
            }
        };

        var versionResponses = new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["getversion"] = new Queue<JToken>(new[] { version.ToJson() })
        };

        var invokeResponses = new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase)
        {
            ["invokescript"] = new Queue<JToken>(new[]
            {
                CreateInvokeResultJson(new Neo.VM.Types.ByteString(Encoding.UTF8.GetBytes("hello")))
            })
        };

        var factory = new QueueRpcClientFactory(new Func<ProtocolSettings, RpcClient>[]
        {
            settings => new StubRpcClient(versionResponses, settings),
            settings => new StubRpcClient(invokeResponses, settings)
        });

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();
        var toolkit = new DeploymentToolkit(config, options: null, rpcClientFactory: factory);

        // Act
        var result = await toolkit.CallAsync<string>("0x11223344556677889900AABBCCDDEEFF00112233", "echo");

        // Assert
        Assert.Equal("hello", result);
        Assert.Empty(versionResponses["getversion"]);
        Assert.Empty(invokeResponses["invokescript"]);
        Assert.Equal<uint?>(0xCAFEBABEu, toolkit.CurrentNetwork.NetworkMagic);
    }

    [Fact]
    public void SetNetwork_WithKnownNetworks_ShouldConfigureCorrectRpcUrl()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        var testCases = new Dictionary<string, string>
        {
            { "mainnet", "https://mainnet1.neo.coz.io:443" },
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
            Assert.Equal("https://mainnet1.neo.coz.io:443", toolkit.CurrentNetwork.RpcUrl);
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

    private sealed class RecordingDeploymentToolkit : DeploymentToolkit
    {
        public RecordingDeploymentToolkit(DeploymentOptions options, IRpcClientFactory factory)
            : base(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build(), options, factory)
        {
        }

        public Signer[]? ObservedSigners { get; private set; }

        protected override Task<TransactionManager> CreateTransactionManagerAsync(RpcClient rpcClient, ReadOnlyMemory<byte> script, Signer[] signers, CancellationToken cancellationToken)
        {
            ObservedSigners = signers.ToArray();

            var tx = new Transaction
            {
                Version = 0,
                Signers = signers,
                Script = script.ToArray(),
                Attributes = Array.Empty<TransactionAttribute>(),
                Witnesses = Array.Empty<Witness>()
            };

            return Task.FromResult<TransactionManager>(new FakeTransactionManager(tx, rpcClient));
        }
    }

    private sealed class FakeTransactionManager : TransactionManager
    {
        public FakeTransactionManager(Transaction tx, RpcClient rpcClient)
            : base(tx, rpcClient)
        {
        }
    }

    private sealed class TestDeploymentToolkit : DeploymentToolkit
    {
        public List<DeploymentCall> Calls { get; } = new();
        public IReadOnlyList<CompiledContractArtifact>? CompileArtifacts { get; set; }

        public TestDeploymentToolkit(DeploymentOptions? options = null)
            : base(
                new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build(),
                options ?? new DeploymentOptions { Network = NetworkProfile.TestNet },
                new NoopRpcClientFactory())
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
                cancellationToken,
                request.Signers,
                request.TransactionSignerAsync);
        }

        protected override Task<IReadOnlyList<CompiledContractArtifact>> CompileContractsAsync(string path, CompilationOptions compilationOptions, string? targetContractName, CancellationToken cancellationToken)
        {
            if (CompileArtifacts is null)
            {
                throw new InvalidOperationException("CompileArtifacts must be set for TestDeploymentToolkit.");
            }

            return Task.FromResult(CompileArtifacts);
        }

        public override Task<ContractDeploymentInfo> DeployArtifactsAsync(string nefPath, string manifestPath, object?[]? initParams = null, bool? waitForConfirmation = null, int? confirmationRetries = null, int? confirmationDelaySeconds = null, CancellationToken cancellationToken = default, IReadOnlyList<Signer>? signers = null, Func<TransactionManager, CancellationToken, Task<Transaction>>? transactionSignerAsync = null)
        {
            Calls.Add(new DeploymentCall(
                Path.GetFullPath(nefPath),
                Path.GetFullPath(manifestPath),
                initParams,
                waitForConfirmation,
                confirmationRetries,
                confirmationDelaySeconds,
                signers,
                transactionSignerAsync));

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
        int? ConfirmationDelaySeconds,
        IReadOnlyList<Signer>? Signers,
        Func<TransactionManager, CancellationToken, Task<Transaction>>? TransactionSignerAsync);

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

    private static ContractState CreateDummyContractState(string name)
    {
        var artifact = CreateDummyArtifact(name);
        var hash = new UInt160(Crypto.Hash160(Encoding.UTF8.GetBytes(name)));
        return new ContractState
        {
            Id = 1,
            UpdateCounter = 0,
            Hash = hash,
            Nef = artifact.Nef,
            Manifest = artifact.Manifest
        };
    }

    private static JToken CreateInvokeResultJson(params Neo.VM.Types.StackItem[] stackItems)
    {
        var result = new RpcInvokeResult
        {
            Script = string.Empty,
            State = VMState.HALT,
            GasConsumed = 0,
            Stack = stackItems
        };
        return result.ToJson();
    }

    private static QueueRpcClientFactory CreateFactoryWithVersion(uint networkMagic, params Func<ProtocolSettings, RpcClient>[] clients)
    {
        var creators = new List<Func<ProtocolSettings, RpcClient>>
        {
            settings => new StubRpcClient(new Dictionary<string, Queue<JToken>>(StringComparer.OrdinalIgnoreCase)
            {
                ["getversion"] = new Queue<JToken>(new[] { CreateVersionResponse(networkMagic) })
            }, settings)
        };
        creators.AddRange(clients);
        return new QueueRpcClientFactory(creators);
    }

    private static JToken CreateVersionResponse(uint networkMagic)
    {
        var version = new RpcVersion
        {
            TcpPort = 20332,
            Nonce = 1,
            UserAgent = "/Neo:unit-test/",
            Protocol = new RpcVersion.RpcProtocol
            {
                Network = networkMagic,
                ValidatorsCount = 7,
                MillisecondsPerBlock = 15000,
                MaxValidUntilBlockIncrement = 86400000,
                MaxTraceableBlocks = 100000,
                AddressVersion = 0x35,
                MaxTransactionsPerBlock = 512,
                MemoryPoolMaxTransactions = 5000,
                InitialGasDistribution = 5200000000000000,
                Hardforks = new Dictionary<Hardfork, uint>(),
                SeedList = Array.Empty<string>(),
                StandbyCommittee = Array.Empty<ECPoint>()
            }
        };
        return version.ToJson();
    }

    private static async Task<uint> InvokeNetworkMagicAsync(DeploymentToolkit toolkit)
    {
        var method = typeof(DeploymentToolkit).GetMethod("GetNetworkMagicAsync", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("GetNetworkMagicAsync not found.");
        return await ((Task<uint>)method.Invoke(toolkit, Array.Empty<object>())!).ConfigureAwait(false);
    }

    private sealed class QueueRpcClientFactory : IRpcClientFactory
    {
        private readonly Queue<Func<ProtocolSettings, RpcClient>> _creators;

        public QueueRpcClientFactory(IEnumerable<Func<ProtocolSettings, RpcClient>> creators)
        {
            _creators = new Queue<Func<ProtocolSettings, RpcClient>>(creators);
        }

        public RpcClient Create(Uri uri, ProtocolSettings protocolSettings)
        {
            if (_creators.Count == 0)
                throw new InvalidOperationException("No RPC client configured for this test.");

            return _creators.Dequeue()(protocolSettings);
        }
    }

    private sealed class StubRpcClient : RpcClient
    {
        private readonly Dictionary<string, Queue<JToken>> _responses;
        private readonly Dictionary<string, Func<JToken[], JToken>> _handlers;

        public StubRpcClient(
            Dictionary<string, Queue<JToken>> responses,
            ProtocolSettings protocolSettings,
            Dictionary<string, Func<JToken[], JToken>>? handlers = null)
            : base(new HttpClient(new StubHttpMessageHandler()), new Uri("http://localhost:10332"), protocolSettings)
        {
            _responses = responses;
            _handlers = handlers ?? new Dictionary<string, Func<JToken[], JToken>>(StringComparer.OrdinalIgnoreCase);
        }

        public override Task<JToken> RpcSendAsync(string method, params JToken[] paraArgs)
        {
            if (_responses.TryGetValue(method, out var queue) && queue.Count > 0)
            {
                return Task.FromResult(queue.Dequeue());
            }

            if (_handlers.TryGetValue(method, out var handler))
            {
                return Task.FromResult(handler(paraArgs));
            }

            throw new InvalidOperationException($"Unexpected RPC method '{method}'.");
        }
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}")
            };
            return Task.FromResult(response);
        }
    }
}
