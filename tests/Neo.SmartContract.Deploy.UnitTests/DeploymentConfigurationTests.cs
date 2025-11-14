using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo;
using Neo.Network.RPC;
using Neo.SmartContract.Deploy;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Neo.SmartContract.Deploy.UnitTests;

[TestClass]
public class DeploymentConfigurationTests
{
    [TestMethod]
    public void LoadsDeploymentSectionAndNetworks()
    {
        const string json = """
{
  "deployment": {
    "defaultNetwork": "mainnet",
    "networks": {
      "mainnet": {
        "rpcUrl": "https://example.org",
        "networkMagic": 42
      }
    }
  }
}
""";
        using var document = JsonDocument.Parse(json);
        var configuration = DeploymentConfiguration.FromJson(document.RootElement, null);

        Assert.AreEqual("mainnet", configuration.DefaultNetwork);
        Assert.IsTrue(configuration.TryGetNetwork("mainnet", out var network));
        Assert.AreEqual((uint)42, network.NetworkMagic);
        Assert.AreEqual(new Uri("https://example.org"), network.RpcUris[0]);
    }

    [TestMethod]
    public async Task ToolkitUsesConfiguredMagicWithoutRpc()
    {
        const string json = """
{
  "defaultNetwork": "testnet",
  "networks": {
    "testnet": {
      "rpcUrls": [ "https://example.org" ],
      "networkMagic": 1234
    }
  }
}
""";
        using var document = JsonDocument.Parse(json);
        var configuration = DeploymentConfiguration.FromJson(document.RootElement, null);
        var factory = new ThrowingRpcClientFactory();
        var toolkit = new DeploymentToolkit(configuration, factory.Create);

        toolkit.UseNetwork("testnet");
        var magic = await toolkit.GetNetworkMagicAsync();

        Assert.AreEqual((uint)1234, magic);
        Assert.IsFalse(factory.CreateInvoked);
    }

    [TestMethod]
    public void KnownProfilesExposeNetworkMagic()
    {
        Assert.IsTrue(NetworkProfile.TryGetKnown("mainnet", out var profile));
        Assert.AreEqual(860833102u, profile.NetworkMagic);
        Assert.IsTrue(profile.RpcUris.Count > 0);
    }

    private sealed class ThrowingRpcClientFactory
    {
        public bool CreateInvoked { get; private set; }

        public RpcClient Create(Uri uri, ProtocolSettings protocolSettings)
        {
            CreateInvoked = true;
            throw new InvalidOperationException("RPC client creation was not expected in this test.");
        }
    }
}
