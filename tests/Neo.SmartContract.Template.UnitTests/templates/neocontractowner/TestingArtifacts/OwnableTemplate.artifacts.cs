using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class OwnableTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.IOwnable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Ownable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":0,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":42,""safe"":false},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":159,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":177,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":291,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":330,""safe"":false}],""events"":[{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractowner/Ownable.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAP/aP6Q0bqUyolj8SX3a3bZDfJ/f8HZGVzdHJveQAAAA8AAP1pAQwB/9swNA5K2CQJSsoAFCgDOkBXAAF4Qfa0a+JBkl3oMUA03kH4J+yMQFcBATT1CZcmFgwRTm8gQXV0aG9yaXphdGlvbiE6eErZKFDKABSzqyQFCSIEeLEkGAwTb3duZXIgbXVzdCBiZSB2YWxpZOA0kHB4DAH/2zA0FnhoEsAMCFNldE93bmVyQZUBb2FAVwACeXhBm/ZnzkHmPxiEQAwFSGVsbG9Bm/ZnzkGSXegxQFcBAnkmA0B4cGjYJgpBLVEIMBPOgHhwaErZKFDKABSzqyQFCSIEaLEkFgwRb3duZXIgbXVzdCBleGlzdHPgaAwB/9swNJpoCxLADAhTZXRPd25lckGVAW9hDAVXb3JsZAwFSGVsbG9Bm/ZnzkHmPxiEQFcAAzX8/v//CZcmFgwRTm8gYXV0aG9yaXphdGlvbi46enl4NwAAQDXY/v//JBYMEU5vIGF1dGhvcml6YXRpb24uOjcBAEAT5iek"));

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
    /// Script: Ndj+//8kFgwRTm8gYXV0aG9yaXphdGlvbi46NwEAQA==
    /// 00 : OpCode.CALL_L D8FEFFFF [512 datoshi]
    /// 05 : OpCode.JMPIF 16 [2 datoshi]
    /// 07 : OpCode.PUSHDATA1 4E6F20617574686F72697A6174696F6E2E [8 datoshi]
    /// 1A : OpCode.THROW [512 datoshi]
    /// 1B : OpCode.CALLT 0100 [32768 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAVIZWxsb0Gb9mfOQZJd6DFA
    /// 00 : OpCode.PUSHDATA1 48656C6C6F 'Hello' [8 datoshi]
    /// 07 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0C : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 11 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNfz+//8JlyYWDBFObyBhdXRob3JpemF0aW9uLjp6eXg3AABA
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.CALL_L FCFEFFFF [512 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.EQUAL [32 datoshi]
    /// 0A : OpCode.JMPIFNOT 16 [2 datoshi]
    /// 0C : OpCode.PUSHDATA1 4E6F20617574686F72697A6174696F6E2E [8 datoshi]
    /// 1F : OpCode.THROW [512 datoshi]
    /// 20 : OpCode.LDARG2 [2 datoshi]
    /// 21 : OpCode.LDARG1 [2 datoshi]
    /// 22 : OpCode.LDARG0 [2 datoshi]
    /// 23 : OpCode.CALLT 0000 [32768 datoshi]
    /// 26 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion
}
