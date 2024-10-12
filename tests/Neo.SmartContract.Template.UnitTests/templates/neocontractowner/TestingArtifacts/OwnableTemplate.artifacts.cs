using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class OwnableTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.IOwnable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Ownable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":0,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":42,""safe"":false},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":162,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":180,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":298,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":337,""safe"":false}],""events"":[{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractowner/Ownable.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAP/aP6Q0bqUyolj8SX3a3bZDfJ/f8HZGVzdHJveQAAAA8AAP1xAQwB/9swNA5K2CQJSsoAFCgDOkBXAAF4Qfa0a+JBkl3oMUA03kH4J+yMQFcBATT1CZcmFgwRTm8gQXV0aG9yaXphdGlvbiE6eErZKFDKABSzqyQFCSIGeBCzqgwTb3duZXIgbXVzdCBiZSB2YWxpZOE0kHB4DAH/2zA0GcJKaM9KeM8MCFNldE93bmVyQZUBb2FAVwACeXhBm/ZnzkHmPxiEQAwFSGVsbG9Bm/ZnzkGSXegxQFcBAnkmA0B4cGgLlyYKQS1RCDATzoB4cGhK2ShQygAUs6skBQkiBmgQs6oMEW93bmVyIG11c3QgZXhpc3Rz4WgMAf/bMDSZwkoLz0pozwwIU2V0T3duZXJBlQFvYQwFV29ybGQMBUhlbGxvQZv2Z85B5j8YhEBXAAM19f7//wmXJhYMEU5vIGF1dGhvcml6YXRpb24uOnp5eDcAAEA10f7//6omFgwRTm8gYXV0aG9yaXphdGlvbi46NwEAQGYr3Ck="));

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
    /// Script: NdH+//+qJhYMTm8gYXV0aG9yaXphdGlvbi46NwEAQA==
    /// 00 : OpCode.CALL_L D1FEFFFF
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIFNOT 16
    /// 08 : OpCode.PUSHDATA1 4E6F20617574686F72697A6174696F6E2E
    /// 1B : OpCode.THROW
    /// 1C : OpCode.CALLT 0100
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DEhlbGxvQZv2Z85Bkl3oMUA=
    /// 00 : OpCode.PUSHDATA1 48656C6C6F
    /// 07 : OpCode.SYSCALL 9BF667CE
    /// 0C : OpCode.SYSCALL 925DE831
    /// 11 : OpCode.RET
    /// </remarks>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNfX+//8JlyYWDE5vIGF1dGhvcml6YXRpb24uOnp5eDcAAEA=
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.CALL_L F5FEFFFF
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.EQUAL
    /// 0A : OpCode.JMPIFNOT 16
    /// 0C : OpCode.PUSHDATA1 4E6F20617574686F72697A6174696F6E2E
    /// 1F : OpCode.THROW
    /// 20 : OpCode.LDARG2
    /// 21 : OpCode.LDARG1
    /// 22 : OpCode.LDARG0
    /// 23 : OpCode.CALLT 0000
    /// 26 : OpCode.RET
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion
}
