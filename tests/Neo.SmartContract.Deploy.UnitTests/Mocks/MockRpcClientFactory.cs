using System;
using Neo.Network.RPC;
using Neo.SmartContract.Deploy.Shared;

namespace Neo.SmartContract.Deploy.UnitTests.Mocks;

/// <summary>
/// Mock RPC client factory for unit testing
/// </summary>
public class MockRpcClientFactory : IRpcClientFactory
{
    private readonly Func<RpcClient> _clientFactory;

    public MockRpcClientFactory(Func<RpcClient>? clientFactory = null)
    {
        _clientFactory = clientFactory ?? (() => new MockRpcClient());
    }

    public RpcClient CreateClient(string? networkName = null)
    {
        return _clientFactory();
    }

    public string GetRpcUrl(string? networkName = null)
    {
        return "http://localhost:10332";
    }

    public string GetNetworkName()
    {
        return "test";
    }

    public void ClearPool()
    {
        // No-op for mock
    }
}
