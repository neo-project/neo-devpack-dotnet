using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Nep17ContractTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep17Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":10,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":12,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":60,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":241,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":635,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":677,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":794,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":831,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":871,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":877,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":895,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":1012,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1051,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep17/Nep17Contract.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAPAAD9HgQMB0VYQU1QTEVAGEBYStgmGEUMAQBB9rRr4kGSXegxStgmBEUQSmBAVwABeGB4DAEAQZv2Z85B5j8YhEBXAQF4cGjYJgUIIg14StkoUMoAFLOrqiYlDCBUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjpBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBEUQ2yFAVwICQZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshcWl5nnFpELUmBAlAabEkEHhowUVTi1BBL1jF7SIPaXhowUVTi1BB5j8YhAhAVwEEeHBo2CYFCCINeErZKFDKABSzq6omJAwfVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5cGjYJgUIIg15StkoUMoAFLOrqiYiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgwlVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IwkBAlAehCYJhZ6m3g1/P7//yQECUB6eTXx/v//RXt6eXg0BAhAVwEEenl4E8AMCFRyYW5zZmVyQZUBb2F5cGjYJgUJIgp5NwAAcGjYqiYfe3p4E8AfDA5vbk5FUDE3UGF5bWVudHlBYn1bUkVAVwACeZkQtSYLDAZhbW91bnQ6ebEkA0B5eDWC/v//RTXr/f//eZ5KNf/9//9FC3l4CzSHQFcAAnmZELUmCwwGYW1vdW50OnmxJANAeZt4NU3+//8kDgwJZXhjZXB0aW9uOjWp/f//eZ9KNb39//9FC3kLeDVF////QAwB/9swNA5K2CQJSsoAFCgDOkBXAAF4Qfa0a+JBkl3oMUA03kH4J+yMQFcBATT1CZcmFgwRTm8gQXV0aG9yaXphdGlvbiE6eErZKFDKABSzqyQFCSIEeLEkGAwTb3duZXIgbXVzdCBiZSB2YWxpZOA0kHB4DAH/2zA0FnhoEsAMCFNldE93bmVyQZUBb2FAVwACeXhBm/ZnzkHmPxiEQFcAAjSACZcmFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1/f7//0BXAAI1W////wmXJhYMEU5vIEF1dGhvcml6YXRpb24hOnl4NaH+//9ANTb///9ADAVIZWxsb0Gb9mfOQZJd6DFAVwECeSYDQHhwaNgmCkEtUQgwE86AeHBoStkoUMoAFLOrJAUJIgRosSQWDBFvd25lciBtdXN0IGV4aXN0c+BoDAH/2zA1R////2gLEsAMCFNldE93bmVyQZUBb2EMBVdvcmxkDAVIZWxsb0Gb9mfOQeY/GIRAVwADNab+//8JlyYWDBFObyBhdXRob3JpemF0aW9uLjp6eXg3AQBAVgFAx+o4ww=="));

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
    /// Script: VwEBeHBo2CYFCCINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : ISNULL [2 datoshi]
    /// 07 : JMPIFNOT 05 [2 datoshi]
    /// 09 : PUSHT [1 datoshi]
    /// 0A : JMP 0D [2 datoshi]
    /// 0C : LDARG0 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISTYPE 28 'ByteString' [2 datoshi]
    /// 10 : SWAP [2 datoshi]
    /// 11 : SIZE [4 datoshi]
    /// 12 : PUSHINT8 14 [1 datoshi]
    /// 14 : NUMEQUAL [8 datoshi]
    /// 15 : BOOLAND [8 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIFNOT 25 [2 datoshi]
    /// 19 : PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E [8 datoshi]
    /// 3B : THROW [512 datoshi]
    /// 3C : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 41 : PUSH1 [1 datoshi]
    /// 42 : PUSH1 [1 datoshi]
    /// 43 : NEWBUFFER [256 datoshi]
    /// 44 : TUCK [2 datoshi]
    /// 45 : PUSH0 [1 datoshi]
    /// 46 : ROT [2 datoshi]
    /// 47 : SETITEM [8192 datoshi]
    /// 48 : SWAP [2 datoshi]
    /// 49 : PUSH2 [1 datoshi]
    /// 4A : PACK [2048 datoshi]
    /// 4B : STLOC0 [2 datoshi]
    /// 4C : LDARG0 [2 datoshi]
    /// 4D : LDLOC0 [2 datoshi]
    /// 4E : UNPACK [2048 datoshi]
    /// 4F : DROP [2 datoshi]
    /// 50 : REVERSE3 [2 datoshi]
    /// 51 : CAT [2048 datoshi]
    /// 52 : SWAP [2 datoshi]
    /// 53 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 58 : DUP [2 datoshi]
    /// 59 : ISNULL [2 datoshi]
    /// 5A : JMPIFNOT 04 [2 datoshi]
    /// 5C : DROP [2 datoshi]
    /// 5D : PUSH0 [1 datoshi]
    /// 5E : CONVERT 21 'Integer' [8192 datoshi]
    /// 60 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNIAJlyYWDBFObyBBdXRob3JpemF0aW9uITp5eDX9/v//QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : CALL 80 [512 datoshi]
    /// 05 : PUSHF [1 datoshi]
    /// 06 : EQUAL [32 datoshi]
    /// 07 : JMPIFNOT 16 [2 datoshi]
    /// 09 : PUSHDATA1 4E6F20417574686F72697A6174696F6E21 [8 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : LDARG1 [2 datoshi]
    /// 1E : LDARG0 [2 datoshi]
    /// 1F : CALL_L FDFEFFFF [512 datoshi]
    /// 24 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNVv///8JlyYWDBFObyBBdXRob3JpemF0aW9uITp5eDWh/v//QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : CALL_L 5BFFFFFF [512 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : EQUAL [32 datoshi]
    /// 0A : JMPIFNOT 16 [2 datoshi]
    /// 0C : PUSHDATA1 4E6F20417574686F72697A6174696F6E21 [8 datoshi]
    /// 1F : THROW [512 datoshi]
    /// 20 : LDARG1 [2 datoshi]
    /// 21 : LDARG0 [2 datoshi]
    /// 22 : CALL_L A1FEFFFF [512 datoshi]
    /// 27 : RET [0 datoshi]
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
    /// Script: VwEEeHBo2CYFCCINeErZKFDKABSzq6omJAwfVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5cGjYJgUIIg15StkoUMoAFLOrqiYiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgwlVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IwkBAlAehCYJhZ6m3g1/P7//yQECUB6eTXx/v//RXt6eXg0BAhA
    /// 00 : INITSLOT 0104 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : ISNULL [2 datoshi]
    /// 07 : JMPIFNOT 05 [2 datoshi]
    /// 09 : PUSHT [1 datoshi]
    /// 0A : JMP 0D [2 datoshi]
    /// 0C : LDARG0 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISTYPE 28 'ByteString' [2 datoshi]
    /// 10 : SWAP [2 datoshi]
    /// 11 : SIZE [4 datoshi]
    /// 12 : PUSHINT8 14 [1 datoshi]
    /// 14 : NUMEQUAL [8 datoshi]
    /// 15 : BOOLAND [8 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIFNOT 24 [2 datoshi]
    /// 19 : PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E [8 datoshi]
    /// 3A : THROW [512 datoshi]
    /// 3B : LDARG1 [2 datoshi]
    /// 3C : STLOC0 [2 datoshi]
    /// 3D : LDLOC0 [2 datoshi]
    /// 3E : ISNULL [2 datoshi]
    /// 3F : JMPIFNOT 05 [2 datoshi]
    /// 41 : PUSHT [1 datoshi]
    /// 42 : JMP 0D [2 datoshi]
    /// 44 : LDARG1 [2 datoshi]
    /// 45 : DUP [2 datoshi]
    /// 46 : ISTYPE 28 'ByteString' [2 datoshi]
    /// 48 : SWAP [2 datoshi]
    /// 49 : SIZE [4 datoshi]
    /// 4A : PUSHINT8 14 [1 datoshi]
    /// 4C : NUMEQUAL [8 datoshi]
    /// 4D : BOOLAND [8 datoshi]
    /// 4E : NOT [4 datoshi]
    /// 4F : JMPIFNOT 22 [2 datoshi]
    /// 51 : PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E [8 datoshi]
    /// 70 : THROW [512 datoshi]
    /// 71 : LDARG2 [2 datoshi]
    /// 72 : PUSH0 [1 datoshi]
    /// 73 : LT [8 datoshi]
    /// 74 : JMPIFNOT 2A [2 datoshi]
    /// 76 : PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E [8 datoshi]
    /// 9D : THROW [512 datoshi]
    /// 9E : LDARG0 [2 datoshi]
    /// 9F : SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// A4 : JMPIF 04 [2 datoshi]
    /// A6 : PUSHF [1 datoshi]
    /// A7 : RET [0 datoshi]
    /// A8 : LDARG2 [2 datoshi]
    /// A9 : PUSH0 [1 datoshi]
    /// AA : NOTEQUAL [32 datoshi]
    /// AB : JMPIFNOT 16 [2 datoshi]
    /// AD : LDARG2 [2 datoshi]
    /// AE : NEGATE [4 datoshi]
    /// AF : LDARG0 [2 datoshi]
    /// B0 : CALL_L FCFEFFFF [512 datoshi]
    /// B5 : JMPIF 04 [2 datoshi]
    /// B7 : PUSHF [1 datoshi]
    /// B8 : RET [0 datoshi]
    /// B9 : LDARG2 [2 datoshi]
    /// BA : LDARG1 [2 datoshi]
    /// BB : CALL_L F1FEFFFF [512 datoshi]
    /// C0 : DROP [2 datoshi]
    /// C1 : LDARG3 [2 datoshi]
    /// C2 : LDARG2 [2 datoshi]
    /// C3 : LDARG1 [2 datoshi]
    /// C4 : LDARG0 [2 datoshi]
    /// C5 : CALL 04 [512 datoshi]
    /// C7 : PUSHT [1 datoshi]
    /// C8 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNab+//8JlyYWDBFObyBhdXRob3JpemF0aW9uLjp6eXg3AQBA
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : CALL_L A6FEFFFF [512 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : EQUAL [32 datoshi]
    /// 0A : JMPIFNOT 16 [2 datoshi]
    /// 0C : PUSHDATA1 4E6F20617574686F72697A6174696F6E2E [8 datoshi]
    /// 1F : THROW [512 datoshi]
    /// 20 : LDARG2 [2 datoshi]
    /// 21 : LDARG1 [2 datoshi]
    /// 22 : LDARG0 [2 datoshi]
    /// 23 : CALLT 0100 [32768 datoshi]
    /// 26 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion
}
