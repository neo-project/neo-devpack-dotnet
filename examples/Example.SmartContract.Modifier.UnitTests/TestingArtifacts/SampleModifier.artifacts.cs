using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleModifier : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleModifier"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":148,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""base64Decode""]}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Description"":""A sample contract to demonstrate how to use modifiers"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy42LjIrZmFiMWEyZWVhZGYyMTE2NjhiMjg0ZWZiYTgwYzFhNTU3ZTYuLi4AAAHA7znO4OTpJcbCoGp54UQN2G/OrAxiYXNlNjREZWNvZGUBAAEPAACkWNgmKwsRwEpZzwwcQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBPRFNNAxgWDRBEdsgIgJAVwACeDQceTcAANsw2yhK2CQJSsoAFCgDOkp4EFHQRUBXAAFA2yhK2CQJSsoAFCgDOkDbMEA3AABAVwABeBDOQfgn7IyqJg4MCWV4Y2VwdGlvbjpAQfgn7IxAVwABQFYCCvr///8K1P///xLAYUBOgcjG"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test")]
    public abstract bool? Test();

    #endregion

    #region Constructor for internal use only

    protected SampleModifier(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
