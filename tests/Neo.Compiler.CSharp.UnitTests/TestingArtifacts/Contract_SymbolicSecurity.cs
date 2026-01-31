using Neo.SmartContract.Manifest;
using System;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SymbolicSecurity(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    public static Neo.SmartContract.NefFile Nef => throw new NotImplementedException();
    public static ContractManifest Manifest => throw new NotImplementedException();
}
