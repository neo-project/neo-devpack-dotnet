using System;
using System.IO;
using Xunit;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.UnitTests;

public class NetworkMagicTests : IDisposable
{
    private readonly string _tempConfigPath;
    private readonly string _tempDir;

    public NetworkMagicTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        _tempConfigPath = Path.Combine(_tempDir, "appsettings.json");
    }

    [Fact]
    public async Task NetworkConfiguration_WithExplicitNetworkMagic_ShouldUseConfiguredValue()
    {
        // Arrange
        var config = @"{
            ""Network"": {
                ""Networks"": {
                    ""custom"": {
                        ""RpcUrl"": ""http://localhost:10332"",
                        ""NetworkMagic"": 12345678
                    }
                }
            }
        }";
        File.WriteAllText(_tempConfigPath, config);

        // Act
        var toolkit = new DeploymentToolkit(_tempConfigPath);
        toolkit.SetNetwork("custom");

        // Assert
        // The network magic should be used from configuration when methods that require it are called
        // This is verified indirectly through the configuration loading
        Assert.Equal<uint?>(12345678, toolkit.CurrentNetwork.NetworkMagic);
        var resolved = await InvokeGetNetworkMagicAsync(toolkit);
        Assert.Equal<uint>(12345678, resolved);
    }

    [Fact]
    public async Task NetworkConfiguration_WithoutNetworkMagic_ShouldKeepValueUnsetUntilResolved()
    {
        // Arrange
        var config = @"{
            ""Network"": {
                ""Networks"": {
                    ""custom"": {
                        ""RpcUrl"": ""http://localhost:10332""
                    }
                }
            }
        }";
        File.WriteAllText(_tempConfigPath, config);

        // Act
        var toolkit = new DeploymentToolkit(_tempConfigPath);
        toolkit.SetNetwork("custom");

        // Assert
        Assert.Null(toolkit.CurrentNetwork.NetworkMagic);
        await Assert.ThrowsAsync<InvalidOperationException>(() => InvokeGetNetworkMagicAsync(toolkit));
    }

    [Fact]
    public async Task NetworkConfiguration_GlobalNetworkMagic_ShouldBeUsedAsFallback()
    {
        // Arrange
        var config = @"{
            ""Network"": {
                ""NetworkMagic"": 87654321,
                ""RpcUrl"": ""http://localhost:10332""
            }
        }";
        File.WriteAllText(_tempConfigPath, config);

        var toolkit = new DeploymentToolkit(_tempConfigPath);
        var resolved = await InvokeGetNetworkMagicAsync(toolkit);
        Assert.Equal<uint>(87654321, resolved);
        Assert.Equal<uint?>(87654321, toolkit.CurrentNetwork.NetworkMagic);
    }

    [Theory]
    [InlineData("mainnet", 860833102)]
    [InlineData("testnet", 894710606)]
    [InlineData("MAINNET", 860833102)]
    [InlineData("TESTNET", 894710606)]
    public async Task KnownNetworks_ShouldHaveCorrectDefaultNetworkMagic(string network, uint expectedMagic)
    {
        var toolkit = new DeploymentToolkit();
        toolkit.SetNetwork(network);

        Assert.Equal<uint?>(expectedMagic, toolkit.CurrentNetwork.NetworkMagic);
        var resolved = await InvokeGetNetworkMagicAsync(toolkit);
        Assert.Equal<uint>(expectedMagic, resolved);
    }

    [Fact]
    public async Task NetworkConfiguration_MixedConfiguration_ShouldPrioritizeCorrectly()
    {
        // Arrange
        var config = @"{
            ""Network"": {
                ""NetworkMagic"": 11111111,
                ""Networks"": {
                    ""network1"": {
                        ""RpcUrl"": ""http://localhost:10332"",
                        ""NetworkMagic"": 22222222
                    },
                    ""network2"": {
                        ""RpcUrl"": ""http://localhost:10333""
                    }
                }
            }
        }";
        File.WriteAllText(_tempConfigPath, config);

        // Act
        var toolkit = new DeploymentToolkit(_tempConfigPath);

        // Test network1 - should use specific network magic
        toolkit.SetNetwork("network1");
        Assert.Equal<uint?>(22222222, toolkit.CurrentNetwork.NetworkMagic);
        var network1Magic = await InvokeGetNetworkMagicAsync(toolkit);
        Assert.Equal<uint>(22222222, network1Magic);

        // Test network2 - should fall back to global or RPC
        toolkit.SetNetwork("network2");
        Assert.Null(toolkit.CurrentNetwork.NetworkMagic);
        var network2Magic = await InvokeGetNetworkMagicAsync(toolkit);
        Assert.Equal<uint>(11111111, network2Magic);
        Assert.Equal<uint?>(11111111, toolkit.CurrentNetwork.NetworkMagic);
    }

    [Fact]
    public void SetNetwork_MultipleTimes_ShouldUpdateConfiguration()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act & Assert - Mainnet
        toolkit.SetNetwork("mainnet");
        Assert.Equal("https://mainnet1.neo.coz.io:443", toolkit.CurrentNetwork.RpcUrl);

        // Act & Assert - Testnet
        toolkit.SetNetwork("testnet");
        Assert.Equal("http://seed2t5.neo.org:20332", toolkit.CurrentNetwork.RpcUrl);

        // Act & Assert - Custom
        toolkit.SetNetwork("http://custom:10332");
        Assert.Equal("http://custom:10332", toolkit.CurrentNetwork.RpcUrl);
    }

    private static async Task<uint> InvokeGetNetworkMagicAsync(DeploymentToolkit toolkit)
    {
        var method = typeof(DeploymentToolkit).GetMethod("GetNetworkMagicAsync", BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("GetNetworkMagicAsync not found.");
        var task = (Task<uint>)method.Invoke(toolkit, Array.Empty<object>())!;
        return await task.ConfigureAwait(false);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, true);
        }

        var intervalProp = typeof(DeploymentToolkit).GetProperty("NetworkMagicRetryInterval", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        intervalProp?.SetValue(null, TimeSpan.FromSeconds(30));
    }
}
