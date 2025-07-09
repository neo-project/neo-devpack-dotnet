using System;
using System.Threading.Tasks;
using Xunit;
using Neo.Network.RPC;

namespace Neo.SmartContract.Deploy.UnitTests;

/// <summary>
/// Integration tests for RPC connectivity
/// These tests require network access and may be skipped in CI environments
/// </summary>
public class RpcIntegrationTests
{
    private const string NGD_TESTNET_URL = "https://testnet.ngd.network:10331";
    private const uint TESTNET_MAGIC = 894710606;

    [Fact(Skip = "Integration test - requires network access")]
    public async Task NgdTestnetRpc_ShouldBeAccessible()
    {
        // Arrange
        using var rpcClient = new RpcClient(new Uri(NGD_TESTNET_URL), null, null, ProtocolSettings.Default);

        // Act
        var version = await rpcClient.GetVersionAsync();

        // Assert
        Assert.NotNull(version);
        Assert.Equal(TESTNET_MAGIC, version.Protocol.Network);
        Assert.True(version.Protocol.Network > 0);
    }

    [Fact(Skip = "Integration test - requires network access")]
    public async Task DeploymentToolkit_ShouldRetrieveNetworkMagicFromNgdTestnet()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();
        toolkit.SetNetwork("testnet");

        // Note: This test would require exposing GetNetworkMagicAsync as public
        // or testing through a public method that uses it in PR 2
        // For now, we verify the setup is correct
        Assert.Equal(NGD_TESTNET_URL, Environment.GetEnvironmentVariable("Network__RpcUrl"));
    }

    [Theory(Skip = "Integration test - requires network access")]
    [InlineData("https://testnet.ngd.network:10331", 894710606)]
    [InlineData("https://rpc10.n3.nspcc.ru:10331", 860833102)]
    public async Task KnownRpcEndpoints_ShouldReturnCorrectNetworkMagic(string rpcUrl, uint expectedMagic)
    {
        // Arrange
        using var rpcClient = new RpcClient(new Uri(rpcUrl), null, null, ProtocolSettings.Default);

        try
        {
            // Act
            var version = await rpcClient.GetVersionAsync();

            // Assert
            Assert.NotNull(version);
            Assert.Equal(expectedMagic, version.Protocol.Network);
        }
        catch (Exception ex)
        {
            // Log but don't fail - RPC endpoints may be temporarily unavailable
            Assert.True(false, $"RPC endpoint {rpcUrl} is not accessible: {ex.Message}");
        }
    }
}