using Neo.SmartContract.Manifest;

namespace Neo.SmartContract.Testing;

public interface IContractInfo
{
    public static abstract NefFile Nef { get; }
    public static abstract ContractManifest Manifest { get; }
}
