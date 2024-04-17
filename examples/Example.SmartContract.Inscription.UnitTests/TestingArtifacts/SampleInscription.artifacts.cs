using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleInscription : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleInscription"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""addInscription"",""parameters"":[{""name"":""address"",""type"":""Hash160""},{""name"":""inscription"",""type"":""String""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""getInscription"",""parameters"":[{""name"":""address"",""type"":""Hash160""}],""returntype"":""String"",""offset"":121,""safe"":true}],""events"":[{""name"":""InscriptionAdded"",""parameters"":[{""name"":""arg1"",""type"":""Hash160""},{""name"":""arg2"",""type"":""String""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Description"":""A sample inscription contract."",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy42LjIrZmFiMWEyZWVhZGYyMTE2NjhiMjg0ZWZiYTgwYzFhNTU3ZTYuLi4AAAAAAJBXAAJ4Qfgn7IyqJjIMLVVuYXV0aG9yaXplZDogQ2FsbGVyIGlzIG5vdCB0aGUgYWRkcmVzcyBvd25lcjp5eEGb9mfOQeY/GITCSnjPSnnPDBBJbnNjcmlwdGlvbkFkZGVkQZUBb2FAQfgn7IxAQeY/GIRAQZv2Z85AVwABeEGb9mfOQZJd6DEiAkBBkl3oMUA0H+ne"));

    #endregion

    #region Events

    public delegate void delInscriptionAdded(UInt160? arg1, string? arg2);

    [DisplayName("InscriptionAdded")]
    public event delInscriptionAdded? OnInscriptionAdded;

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("getInscription")]
    public abstract string? GetInscription(UInt160? address);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("addInscription")]
    public abstract void AddInscription(UInt160? address, string? inscription);

    #endregion

    #region Constructor for internal use only

    protected SampleInscription(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
