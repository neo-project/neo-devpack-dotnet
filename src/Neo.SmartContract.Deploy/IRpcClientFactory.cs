using Neo.Network.RPC;
using System;

namespace Neo.SmartContract.Deploy;

public interface IRpcClientFactory
{
    RpcClient Create(Uri uri, ProtocolSettings protocolSettings);
}

internal sealed class DefaultRpcClientFactory : IRpcClientFactory
{
    public RpcClient Create(Uri uri, ProtocolSettings protocolSettings)
        => new(uri, null, null, protocolSettings);
}

