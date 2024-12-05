using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Nep17ContractTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep17Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":10,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":12,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":50,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":228,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":620,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":662,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":781,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":819,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":857,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":863,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":881,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":1002,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep17/Nep17Contract.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAPAAD9DwQMB0VYQU1QTEVAGEAMAQBB9rRr4kGSXegxStgmBEUQQFcAAXgMAQBBm/ZnzkHmPxiEQFcBAXhK2SgkBkUJIgbKABSzJCUMIFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYFRRBA2yFAVwICQZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgZFECIE2yFxaXmecWkQtSYECUBpsSQQeGjBRVOLUEEvWMXtIg9peGjBRVOLUEHmPxiECEBXAQR4StkoJAZFCSIGygAUsyQkDB9UaGUgYXJndW1lbnQgImZyb20iIGlzIGludmFsaWQuOnlwaNgmBQgiEXlK2SgkBkUJIgbKABSzqiYiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgwlVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IwkBAlAehCYJhZ6m3g1/P7//yQECUB6eTXx/v//RXt6eXg0BAhAVwEEenl4E8AMCFRyYW5zZmVyQZUBb2F5cGjYJgUJIgp5NwAAcGjYqiYfe3p4E8AfDA5vbk5FUDE3UGF5bWVudHlBYn1bUkVAVwACeZkQtSYLDAZhbW91bnQ6ebEkA0B5eDWC/v//RTX6/f//eZ5KNQb+//9FC3l4CzSHQFcAAnmZELUmCwwGYW1vdW50OnmxJANAeZt4NU3+//8kDgwJZXhjZXB0aW9uOjW4/f//eZ9KNcT9//9FC3kLeDVF////QAwB/9swNA5K2CQJSsoAFCgDOkBXAAF4Qfa0a+JBkl3oMUA03kH4J+yMQFcBATT1JBYMEU5vIEF1dGhvcml6YXRpb24hOnhK2SgkBkUJIgbKABSzJAUJIgR4sSQYDBNvd25lciBtdXN0IGJlIHZhbGlk4DSOcHgMAf/bMDQWeGgSwAwIU2V0T3duZXJBlQFvYUBXAAJ5eEGb9mfOQeY/GIRAVwACNX7///8kFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1+v7//0BXAAI1WP///yQWDBFObyBBdXRob3JpemF0aW9uITp5eDWg/v//QDU1////QAwFSGVsbG9Bm/ZnzkGSXegxQFcBAnkmA0B4cGjYJgpBLVEIMBPOgHhwaErZKCQGRQkiBsoAFLMkBQkiBGixJBYMEW93bmVyIG11c3QgZXhpc3Rz4GgMAf/bMDVE////aAsSwAwIU2V0T3duZXJBlQFvYQwFV29ybGQMBUhlbGxvQZv2Z85B5j8YhEBXAAM1of7//yQWDBFObyBhdXRob3JpemF0aW9uLjp6eXg3AQBANwFHfQ=="));

    #endregion

    #region Events

    [DisplayName("SetOwner")]
    public event Neo.SmartContract.Testing.TestingStandards.IOwnable.delSetOwner? OnSetOwner;

    [DisplayName("Transfer")]
    public event Neo.SmartContract.Testing.TestingStandards.INep17Standard.delTransfer? OnTransfer;

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? Decimals { [DisplayName("decimals")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract UInt160? Owner { [DisplayName("getOwner")] get; [DisplayName("setOwner")] set; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? Symbol { [DisplayName("symbol")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? TotalSupply { [DisplayName("totalSupply")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract bool? Verify { [DisplayName("verify")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgVFEEDbIUA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISTYPE 28 'ByteString' [2 datoshi]
    /// 07 : JMPIF 06 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : PUSHF [1 datoshi]
    /// 0B : JMP 06 [2 datoshi]
    /// 0D : SIZE [4 datoshi]
    /// 0E : PUSHINT8 14 [1 datoshi]
    /// 10 : NUMEQUAL [8 datoshi]
    /// 11 : JMPIF 25 [2 datoshi]
    /// 13 : PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E [8 datoshi]
    /// 35 : THROW [512 datoshi]
    /// 36 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 3B : PUSH1 [1 datoshi]
    /// 3C : PUSH1 [1 datoshi]
    /// 3D : NEWBUFFER [256 datoshi]
    /// 3E : TUCK [2 datoshi]
    /// 3F : PUSH0 [1 datoshi]
    /// 40 : ROT [2 datoshi]
    /// 41 : SETITEM [8192 datoshi]
    /// 42 : SWAP [2 datoshi]
    /// 43 : PUSH2 [1 datoshi]
    /// 44 : PACK [2048 datoshi]
    /// 45 : STLOC0 [2 datoshi]
    /// 46 : LDARG0 [2 datoshi]
    /// 47 : LDLOC0 [2 datoshi]
    /// 48 : UNPACK [2048 datoshi]
    /// 49 : DROP [2 datoshi]
    /// 4A : REVERSE3 [2 datoshi]
    /// 4B : CAT [2048 datoshi]
    /// 4C : SWAP [2 datoshi]
    /// 4D : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 52 : DUP [2 datoshi]
    /// 53 : ISNULL [2 datoshi]
    /// 54 : JMPIFNOT 05 [2 datoshi]
    /// 56 : DROP [2 datoshi]
    /// 57 : PUSH0 [1 datoshi]
    /// 58 : RET [0 datoshi]
    /// 59 : CONVERT 21 'Integer' [8192 datoshi]
    /// 5B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNX7///8kFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1+v7//0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : CALL_L 7EFFFFFF [512 datoshi]
    /// 08 : JMPIF 16 [2 datoshi]
    /// 0A : PUSHDATA1 4E6F20417574686F72697A6174696F6E21 [8 datoshi]
    /// 1D : THROW [512 datoshi]
    /// 1E : LDARG1 [2 datoshi]
    /// 1F : LDARG0 [2 datoshi]
    /// 20 : CALL_L FAFEFFFF [512 datoshi]
    /// 25 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNVj///8kFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1oP7//0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : CALL_L 58FFFFFF [512 datoshi]
    /// 08 : JMPIF 16 [2 datoshi]
    /// 0A : PUSHDATA1 4E6F20417574686F72697A6174696F6E21 [8 datoshi]
    /// 1D : THROW [512 datoshi]
    /// 1E : LDARG1 [2 datoshi]
    /// 1F : LDARG0 [2 datoshi]
    /// 20 : CALL_L A0FEFFFF [512 datoshi]
    /// 25 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("mint")]
    public abstract void Mint(UInt160? to, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAVIZWxsb0Gb9mfOQZJd6DFA
    /// 00 : PUSHDATA1 48656C6C6F 'Hello' [8 datoshi]
    /// 07 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0C : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 11 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("myMethod")]
    public abstract string? MyMethod();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEEeErZKCQGRQkiBsoAFLMkJAwfVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5cGjYJgUIIhF5StkoJAZFCSIGygAUs6omIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMJVRoZSBhbW91bnQgbXVzdCBiZSBhIHBvc2l0aXZlIG51bWJlci46eEH4J+yMJAQJQHoQmCYWept4Nfz+//8kBAlAenk18f7//0V7enl4NAQIQA==
    /// 00 : INITSLOT 0104 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISTYPE 28 'ByteString' [2 datoshi]
    /// 07 : JMPIF 06 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : PUSHF [1 datoshi]
    /// 0B : JMP 06 [2 datoshi]
    /// 0D : SIZE [4 datoshi]
    /// 0E : PUSHINT8 14 [1 datoshi]
    /// 10 : NUMEQUAL [8 datoshi]
    /// 11 : JMPIF 24 [2 datoshi]
    /// 13 : PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E [8 datoshi]
    /// 34 : THROW [512 datoshi]
    /// 35 : LDARG1 [2 datoshi]
    /// 36 : STLOC0 [2 datoshi]
    /// 37 : LDLOC0 [2 datoshi]
    /// 38 : ISNULL [2 datoshi]
    /// 39 : JMPIFNOT 05 [2 datoshi]
    /// 3B : PUSHT [1 datoshi]
    /// 3C : JMP 11 [2 datoshi]
    /// 3E : LDARG1 [2 datoshi]
    /// 3F : DUP [2 datoshi]
    /// 40 : ISTYPE 28 'ByteString' [2 datoshi]
    /// 42 : JMPIF 06 [2 datoshi]
    /// 44 : DROP [2 datoshi]
    /// 45 : PUSHF [1 datoshi]
    /// 46 : JMP 06 [2 datoshi]
    /// 48 : SIZE [4 datoshi]
    /// 49 : PUSHINT8 14 [1 datoshi]
    /// 4B : NUMEQUAL [8 datoshi]
    /// 4C : NOT [4 datoshi]
    /// 4D : JMPIFNOT 22 [2 datoshi]
    /// 4F : PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E [8 datoshi]
    /// 6E : THROW [512 datoshi]
    /// 6F : LDARG2 [2 datoshi]
    /// 70 : PUSH0 [1 datoshi]
    /// 71 : LT [8 datoshi]
    /// 72 : JMPIFNOT 2A [2 datoshi]
    /// 74 : PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E [8 datoshi]
    /// 9B : THROW [512 datoshi]
    /// 9C : LDARG0 [2 datoshi]
    /// 9D : SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// A2 : JMPIF 04 [2 datoshi]
    /// A4 : PUSHF [1 datoshi]
    /// A5 : RET [0 datoshi]
    /// A6 : LDARG2 [2 datoshi]
    /// A7 : PUSH0 [1 datoshi]
    /// A8 : NOTEQUAL [32 datoshi]
    /// A9 : JMPIFNOT 16 [2 datoshi]
    /// AB : LDARG2 [2 datoshi]
    /// AC : NEGATE [4 datoshi]
    /// AD : LDARG0 [2 datoshi]
    /// AE : CALL_L FCFEFFFF [512 datoshi]
    /// B3 : JMPIF 04 [2 datoshi]
    /// B5 : PUSHF [1 datoshi]
    /// B6 : RET [0 datoshi]
    /// B7 : LDARG2 [2 datoshi]
    /// B8 : LDARG1 [2 datoshi]
    /// B9 : CALL_L F1FEFFFF [512 datoshi]
    /// BE : DROP [2 datoshi]
    /// BF : LDARG3 [2 datoshi]
    /// C0 : LDARG2 [2 datoshi]
    /// C1 : LDARG1 [2 datoshi]
    /// C2 : LDARG0 [2 datoshi]
    /// C3 : CALL 04 [512 datoshi]
    /// C5 : PUSHT [1 datoshi]
    /// C6 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNaH+//8kFgwRTm8gYXV0aG9yaXphdGlvbi46enl4NwEAQA==
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : CALL_L A1FEFFFF [512 datoshi]
    /// 08 : JMPIF 16 [2 datoshi]
    /// 0A : PUSHDATA1 4E6F20617574686F72697A6174696F6E2E [8 datoshi]
    /// 1D : THROW [512 datoshi]
    /// 1E : LDARG2 [2 datoshi]
    /// 1F : LDARG1 [2 datoshi]
    /// 20 : LDARG0 [2 datoshi]
    /// 21 : CALLT 0100 [32768 datoshi]
    /// 24 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion
}
