using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class OwnableTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.IOwnable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Ownable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":0,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":42,""safe"":false},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":165,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":183,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":301,""safe"":false},{""name"":""destroy"",""parameters"":[],""returntype"":""Void"",""offset"":340,""safe"":false}],""events"":[{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractowner/Ownable.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAP/aP6Q0bqUyolj8SX3a3bZDfJ/f8HZGVzdHJveQAAAA8AAP10AQwB/9swNA5K2CQJSsoAFCgDOkBXAAF4Qfa0a+JBkl3oMUA03kH4J+yMQFcBATT1CZcmFgwRTm8gQXV0aG9yaXphdGlvbiE6eErZKFDKABSzqyQFCSIGeBCzqgwTb3duZXIgbXVzdCBiZSB2YWxpZOE1kP///3B4DAH/2zA0GcJKaM9KeM8MCFNldE93bmVyQZUBb2FAVwACeXhBm/ZnzkHmPxiEQAwFSGVsbG9Bm/ZnzkGSXegxQFcBAnkmA0B4cGgLlyYKQS1RCDATzoB4cGhK2ShQygAUs6skBQkiBmgQs6oMEW93bmVyIG11c3QgZXhpc3Rz4WgMAf/bMDSZwkoLz0pozwwIU2V0T3duZXJBlQFvYQwFV29ybGQMBUhlbGxvQZv2Z85B5j8YhEBXAAM18v7//wmXJhYMEU5vIGF1dGhvcml6YXRpb24uOnp5eDcAAEA1zv7//6omFgwRTm8gYXV0aG9yaXphdGlvbi46NwEAQIBKMQE="));

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
    /// Script: Nc7+//+qJhYMTm8gYXV0aG9yaXphdGlvbi46NwEAQA==
    /// 00 : OpCode.CALL_L CEFEFFFF 	-> 512 datoshi
    /// 05 : OpCode.NOT 	-> 4 datoshi
    /// 06 : OpCode.JMPIFNOT 16 	-> 2 datoshi
    /// 08 : OpCode.PUSHDATA1 4E6F20617574686F72697A6174696F6E2E 	-> 8 datoshi
    /// 1B : OpCode.THROW 	-> 512 datoshi
    /// 1C : OpCode.CALLT 0100 	-> 32768 datoshi
    /// 1F : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("destroy")]
    public abstract void Destroy();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DEhlbGxvQZv2Z85Bkl3oMUA=
    /// 00 : OpCode.PUSHDATA1 48656C6C6F 	-> 8 datoshi
    /// 07 : OpCode.SYSCALL 9BF667CE 	-> 0 datoshi
    /// 0C : OpCode.SYSCALL 925DE831 	-> 0 datoshi
    /// 11 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNfL+//8JlyYWDE5vIGF1dGhvcml6YXRpb24uOnp5eDcAAEA=
    /// 00 : OpCode.INITSLOT 0003 	-> 64 datoshi
    /// 03 : OpCode.CALL_L F2FEFFFF 	-> 512 datoshi
    /// 08 : OpCode.PUSHF 	-> 1 datoshi
    /// 09 : OpCode.EQUAL 	-> 32 datoshi
    /// 0A : OpCode.JMPIFNOT 16 	-> 2 datoshi
    /// 0C : OpCode.PUSHDATA1 4E6F20617574686F72697A6174696F6E2E 	-> 8 datoshi
    /// 1F : OpCode.THROW 	-> 512 datoshi
    /// 20 : OpCode.LDARG2 	-> 2 datoshi
    /// 21 : OpCode.LDARG1 	-> 2 datoshi
    /// 22 : OpCode.LDARG0 	-> 2 datoshi
    /// 23 : OpCode.CALLT 0000 	-> 32768 datoshi
    /// 26 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion
}
