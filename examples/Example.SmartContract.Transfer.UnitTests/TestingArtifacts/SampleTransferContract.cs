using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleTransferContract : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleTransferContract"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":42,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Description"":""A sample contract to demonstrate how to transfer NEO and GAS"",""Version"":""1.0.0.0"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1Y+pAvCg9TQ4FxI6jBbPyoHNA7wh0cmFuc2ZlcgQAAQ/PduKL0AYsSkeO41VhARMZ88+k0gh0cmFuc2ZlcgQAAQ/PduKL0AYsSkeO41VhARMZ88+k0gliYWxhbmNlT2YBAAEPAABEVwACWEH4J+yMOQt5eEHb/qh0NwAAOQhB2/6odDcCAHhB2/6odDcBADlAVgEMFGKZMx5XjGZFMfS/kn/Y2DEr0jQ4YECGYika"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("transfer")]
    public abstract void Transfer(UInt160? to, BigInteger? amount);

    #endregion

    #region Constructor for internal use only

    protected SampleTransferContract(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
