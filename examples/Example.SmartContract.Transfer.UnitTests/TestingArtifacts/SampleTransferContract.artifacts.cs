using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleTransferContract : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleTransferContract"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""transfer"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":70,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Description"":""A sample contract to demonstrate how to transfer NEO and GAS"",""Version"":""1.0.0.0"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy42LjIrZmFiMWEyZWVhZGYyMTE2NjhiMjg0ZWZiYTgwYzFhNTU3ZTYuLi4AAAP1Y+pAvCg9TQ4FxI6jBbPyoHNA7wh0cmFuc2ZlcgQAAQ/PduKL0AYsSkeO41VhARMZ88+k0gh0cmFuc2ZlcgQAAQ/PduKL0AYsSkeO41VhARMZ88+k0gliYWxhbmNlT2YBAAEPAABgVwACWEH4J+yMOQt5eEHb/qh0NwAAORHbIEHb/qh0NwIAeEHb/qh0NwEAOUA5QEH4J+yMQDcAAEBB2/6odEA3AQBANwIAQFYBDBRimTMeV4xmRTH0v5J/2NgxK9I0OGBAvPy54A=="));

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
