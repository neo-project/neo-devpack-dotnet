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
    
    public RpcClient CreateClient(string rpcUrl)
    {
        return _clientFactory();
    }
    
    public void ClearPool()
    {
        // No-op for mock
    }
}