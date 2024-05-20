using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class OwnableTemplate : Neo.SmartContract.Testing.SmartContract, Neo.SmartContract.Testing.TestingStandards.IOwnable
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Ownable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":0,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":44,""safe"":false},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":171,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":191,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":314,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":355,""safe"":false}],""events"":[{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractowner/Ownable.cs""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAP/aP6Q0bqUyolj8SX3a3bZDfJ/f8HZGVzdHJveQAAAA8AAP2DAQwB/9swNBBK2CQJSsoAFCgDOiICQFcAAXhB9rRr4kGSXegxQDTcQfgn7IxAVwEBNPUQ2yCXJhYMEU5vIEF1dGhvcml6YXRpb24hOnhK2ShQygAUs6skBxDbICIGeBCzqgwTb3duZXIgbXVzdCBiZSB2YWxpZOE1iv///3B4DAH/2zA0GcJKaM9KeM8MCFNldE93bmVyQZUBb2FAVwACeXhBm/ZnzkHmPxiEQAwFSGVsbG9Bm/ZnzkGSXegxIgJAVwECeSYEInR4cGgLlyYMQS1RCDATzkqARXhwaErZKFDKABSzqyQHENsgIgZoELOqDBFvd25lciBtdXN0IGV4aXN0c+FoDAH/2zA0ksJKC89KaM8MCFNldE93bmVyQZUBb2EMBVdvcmxkDAVIZWxsb0Gb9mfOQeY/GIRAVwADNef+//8Q2yCXJhYMEU5vIGF1dGhvcml6YXRpb24uOnp5eDcAAEA1wf7//6omFgwRTm8gYXV0aG9yaXphdGlvbi46NwEAQIZw5ts="));

    #endregion

    #region Events

    [DisplayName("SetOwner")]
    public event Neo.SmartContract.Testing.TestingStandards.IOwnable.delSetOwner? OnSetOwner;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt160? Owner { [DisplayName("getOwner")] get; [DisplayName("setOwner")] set; }

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion

    #region Constructor for internal use only

    protected OwnableTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
