using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class OwnableTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.IOwnable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Ownable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":0,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":30,""safe"":false},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":141,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":160,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":287,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":324,""safe"":false}],""events"":[{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""destroy"",""update""]}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractowner/Ownable.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAP/aP6Q0bqUyolj8SX3a3bZDfJ/f8HZGVzdHJveQAAAA8AAP1jAQwB/xGNQdWNXuhK2CQJSsoAFCgDOkA06kH4J+yMQFcCATT1JBYMEU5vIEF1dGhvcml6YXRpb24hOnhK2SgkBkUJIgbKABSzJAUJIgR4sXAME293bmVyIG11c3QgYmUgdmFsaWRxaCQEaeA0lnB4DAH/EY1BOQzjCmh4UBLADAhTZXRPd25lckGVAW9hQEH2tGviDAVIZWxsb1BBkl3oMUBXAwJ5JgNAeHBo2CYKQS1RCDATzoB4cGhK2SgkBkUJIgbKABSzJAUJIgRosXEMEW93bmVyIG11c3QgZXhpc3RzcmkkBGrgaAwB/xGNQTkM4woLaFASwAwIU2V0T3duZXJBlQFvYUGb9mfODAVIZWxsbwwFV29ybGRTQeY/GIRAVwADNfT+//8kFgwRTm8gYXV0aG9yaXphdGlvbi46enl4NwAAQDXS/v//JBYMEU5vIGF1dGhvcml6YXRpb24uOjcBAEC7L0i4").AsSerializable<Neo.SmartContract.NefFile>();

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
    /// <remarks>
    /// Script: NdL+//8kFgwRTm8gYXV0aG9yaXphdGlvbi46NwEAQA==
    /// CALL_L D2FEFFFF [512 datoshi]
    /// JMPIF 16 [2 datoshi]
    /// PUSHDATA1 4E6F20617574686F72697A6174696F6E2E [8 datoshi]
    /// THROW [512 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Qfa0a+IMBUhlbGxvUEGSXegxQA==
    /// SYSCALL F6B46BE2 'System.Storage.GetReadOnlyContext' [16 datoshi]
    /// PUSHDATA1 48656C6C6F 'Hello' [8 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNfT+//8kFgwRTm8gYXV0aG9yaXphdGlvbi46enl4NwAAQA==
    /// INITSLOT 0003 [64 datoshi]
    /// CALL_L F4FEFFFF [512 datoshi]
    /// JMPIF 16 [2 datoshi]
    /// PUSHDATA1 4E6F20617574686F72697A6174696F6E2E [8 datoshi]
    /// THROW [512 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion
}
