using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neo;
using Neo.Cryptography.ECC;
using Neo.Json;
using Neo.Network.RPC;
using Neo.Network.RPC.Models;
using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests;

/// <summary>
/// RPC client behaviour tests backed by deterministic JSON-RPC responses.
/// </summary>
public class RpcIntegrationTests
{
    private const string NEO_TESTNET_URL = "http://seed2t5.neo.org:20332";
    private const uint TESTNET_MAGIC = 894710606;

    [Fact]
    public async Task RpcClient_ShouldParseVersionResponse()
    {
        // Arrange
        var version = CreateVersionModel(TESTNET_MAGIC);
        using var rpcClient = new RpcClient(
            new HttpClient(new JsonRpcMessageHandler(version.ToJson())),
            new Uri(NEO_TESTNET_URL),
            ProtocolSettings.Default);

        // Act
        var result = await rpcClient.GetVersionAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TESTNET_MAGIC, result.Protocol.Network);
        Assert.True(result.Protocol.Network > 0);
    }

    [Fact]
    public void DeploymentToolkit_ShouldUseKnownNetworkProfileWhenSelectingTestnet()
    {
        // Arrange
        var toolkit = new DeploymentToolkit();

        // Act
        toolkit.SetNetwork("testnet");

        // Assert
        Assert.Equal(NEO_TESTNET_URL, toolkit.CurrentNetwork.RpcUrl);
        Assert.Equal<uint?>(TESTNET_MAGIC, toolkit.CurrentNetwork.NetworkMagic);
    }

    [Theory]
    [InlineData("http://seed2t5.neo.org:20332", 894710606u)]
    [InlineData("https://mainnet1.neo.coz.io:443", 860833102u)]
    public async Task RpcClient_ShouldReportExpectedNetworkMagic(string rpcUrl, uint expectedMagic)
    {
        // Arrange
        var version = CreateVersionModel(expectedMagic);
        using var rpcClient = new RpcClient(
            new HttpClient(new JsonRpcMessageHandler(version.ToJson())),
            new Uri(rpcUrl),
            ProtocolSettings.Default);

        // Act
        var result = await rpcClient.GetVersionAsync();

        // Assert
        Assert.Equal(expectedMagic, result.Protocol.Network);
    }

    private static RpcVersion CreateVersionModel(uint networkMagic)
    {
        return new RpcVersion
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
    }

    private sealed class JsonRpcMessageHandler : HttpMessageHandler
    {
        private readonly string _responseContent;

        public JsonRpcMessageHandler(JObject result)
        {
            var response = new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = 1,
                ["result"] = result
            };
            _responseContent = response.ToString();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            });
        }
    }
}
