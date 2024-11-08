using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Nep17ContractTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep17Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":10,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":12,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":72,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":215,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":597,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":639,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":758,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":796,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":834,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":840,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":858,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":979,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep17/Nep17Contract.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAPAAD9+AMMB0VYQU1QTEVAGEBXAQAMAQA0DXBoStgmBEUQ2yFAVwABeEH2tGviQZJd6DFAVwABeAwBADQDQFcAAnl4QZv2Z85B5j8YhEBXAQF4StkoJAZFCSIGygAUsyQlDCBUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjoMAQF4i9soNJpwaErYJgRFENshQFcEAkGb9mfOcAwBAXiL2yhxaWhBkl3oMXJqStgmBEUQ2yFza3mec2sQtSYECUBrsSQLaWhBL1jF7SIKa2loQeY/GIQIQFcABHhK2SgkBkUJIgbKABSzJCQMH1RoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eUrZKCQGRQkiBsoAFLMkIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMJVRoZSBhbW91bnQgbXVzdCBiZSBhIHBvc2l0aXZlIG51bWJlci46eEH4J+yMJAQJQHoQmCYWept4NRf///8kBAlAenk1DP///0V7enl4NAQIQFcBBHp5eBPADAhUcmFuc2ZlckGVAW9heXBo2CYFCSIKeTcAAHBo2KomH3t6eBPAHwwOb25ORVAxN1BheW1lbnR5QWJ9W1JFQFcAAnmZELUmCwwGYW1vdW50OnmxJANAeXg1nf7//0U1Ef7//3meSjUr/v//RQt5eAs0h0BXAAJ5mRC1JgsMBmFtb3VudDp5sSQDQHmbeDVo/v//JA4MCWV4Y2VwdGlvbjo1z/3//3mfSjXp/f//RQt5C3g1Rf///0AMAf/bMDQOStgkCUrKABQoAzpAVwABeEH2tGviQZJd6DFANN5B+CfsjEBXAQE09SQWDBFObyBBdXRob3JpemF0aW9uITp4StkoJAZFCSIGygAUsyQFCSIEeLEkGAwTb3duZXIgbXVzdCBiZSB2YWxpZOA0jnB4DAH/2zA0FnhoEsAMCFNldE93bmVyQZUBb2FAVwACeXhBm/ZnzkHmPxiEQFcAAjV+////JBYMEU5vIEF1dGhvcml6YXRpb24hOnl4Nfr+//9AVwACNVj///8kFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1oP7//0A1Nf///0AMBUhlbGxvQZv2Z85Bkl3oMUBXAQJ5JgNAeHBo2CYKQS1RCDATzoB4cGhK2SgkBkUJIgbKABSzJAUJIgRosSQWDBFvd25lciBtdXN0IGV4aXN0c+BoDAH/2zA1RP///2gLEsAMCFNldE93bmVyQZUBb2EMBVdvcmxkDAVIZWxsb0Gb9mfOQeY/GIRAVwADNaH+//8kFgwRTm8gYXV0aG9yaXphdGlvbi46enl4NwEAQOeFsrs="));

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
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46DAEBeIvbKDSacGhK2CYERRDbIUA=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISTYPE 28 [2 datoshi]
    /// 07 : OpCode.JMPIF 06 [2 datoshi]
    /// 09 : OpCode.DROP [2 datoshi]
    /// 0A : OpCode.PUSHF [1 datoshi]
    /// 0B : OpCode.JMP 06 [2 datoshi]
    /// 0D : OpCode.SIZE [4 datoshi]
    /// 0E : OpCode.PUSHINT8 14 [1 datoshi]
    /// 10 : OpCode.NUMEQUAL [8 datoshi]
    /// 11 : OpCode.JMPIF 25 [2 datoshi]
    /// 13 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E [8 datoshi]
    /// 35 : OpCode.THROW [512 datoshi]
    /// 36 : OpCode.PUSHDATA1 01 [8 datoshi]
    /// 39 : OpCode.LDARG0 [2 datoshi]
    /// 3A : OpCode.CAT [2048 datoshi]
    /// 3B : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 3D : OpCode.CALL 9A [512 datoshi]
    /// 3F : OpCode.STLOC0 [2 datoshi]
    /// 40 : OpCode.LDLOC0 [2 datoshi]
    /// 41 : OpCode.DUP [2 datoshi]
    /// 42 : OpCode.ISNULL [2 datoshi]
    /// 43 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 45 : OpCode.DROP [2 datoshi]
    /// 46 : OpCode.PUSH0 [1 datoshi]
    /// 47 : OpCode.CONVERT 21 'Integer' [8192 datoshi]
    /// 49 : OpCode.RET [0 datoshi]
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
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.CALL_L 7EFFFFFF [512 datoshi]
    /// 08 : OpCode.JMPIF 16 [2 datoshi]
    /// 0A : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21 [8 datoshi]
    /// 1D : OpCode.THROW [512 datoshi]
    /// 1E : OpCode.LDARG1 [2 datoshi]
    /// 1F : OpCode.LDARG0 [2 datoshi]
    /// 20 : OpCode.CALL_L FAFEFFFF [512 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNVj///8kFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1oP7//0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.CALL_L 58FFFFFF [512 datoshi]
    /// 08 : OpCode.JMPIF 16 [2 datoshi]
    /// 0A : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21 [8 datoshi]
    /// 1D : OpCode.THROW [512 datoshi]
    /// 1E : OpCode.LDARG1 [2 datoshi]
    /// 1F : OpCode.LDARG0 [2 datoshi]
    /// 20 : OpCode.CALL_L A0FEFFFF [512 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("mint")]
    public abstract void Mint(UInt160? to, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAVIZWxsb0Gb9mfOQZJd6DFA
    /// 00 : OpCode.PUSHDATA1 48656C6C6F [8 datoshi]
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
    /// Script: VwAEeErZKCQGRQkiBsoAFLMkJAwfVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5StkoJAZFCSIGygAUsyQiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgwlVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IwkBAlAehCYJhZ6m3g1F////yQECUB6eTUM////RXt6eXg0BAhA
    /// 00 : OpCode.INITSLOT 0004 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISTYPE 28 [2 datoshi]
    /// 07 : OpCode.JMPIF 06 [2 datoshi]
    /// 09 : OpCode.DROP [2 datoshi]
    /// 0A : OpCode.PUSHF [1 datoshi]
    /// 0B : OpCode.JMP 06 [2 datoshi]
    /// 0D : OpCode.SIZE [4 datoshi]
    /// 0E : OpCode.PUSHINT8 14 [1 datoshi]
    /// 10 : OpCode.NUMEQUAL [8 datoshi]
    /// 11 : OpCode.JMPIF 24 [2 datoshi]
    /// 13 : OpCode.PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E [8 datoshi]
    /// 34 : OpCode.THROW [512 datoshi]
    /// 35 : OpCode.LDARG1 [2 datoshi]
    /// 36 : OpCode.DUP [2 datoshi]
    /// 37 : OpCode.ISTYPE 28 [2 datoshi]
    /// 39 : OpCode.JMPIF 06 [2 datoshi]
    /// 3B : OpCode.DROP [2 datoshi]
    /// 3C : OpCode.PUSHF [1 datoshi]
    /// 3D : OpCode.JMP 06 [2 datoshi]
    /// 3F : OpCode.SIZE [4 datoshi]
    /// 40 : OpCode.PUSHINT8 14 [1 datoshi]
    /// 42 : OpCode.NUMEQUAL [8 datoshi]
    /// 43 : OpCode.JMPIF 22 [2 datoshi]
    /// 45 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E [8 datoshi]
    /// 64 : OpCode.THROW [512 datoshi]
    /// 65 : OpCode.LDARG2 [2 datoshi]
    /// 66 : OpCode.PUSH0 [1 datoshi]
    /// 67 : OpCode.LT [8 datoshi]
    /// 68 : OpCode.JMPIFNOT 2A [2 datoshi]
    /// 6A : OpCode.PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E [8 datoshi]
    /// 91 : OpCode.THROW [512 datoshi]
    /// 92 : OpCode.LDARG0 [2 datoshi]
    /// 93 : OpCode.SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// 98 : OpCode.JMPIF 04 [2 datoshi]
    /// 9A : OpCode.PUSHF [1 datoshi]
    /// 9B : OpCode.RET [0 datoshi]
    /// 9C : OpCode.LDARG2 [2 datoshi]
    /// 9D : OpCode.PUSH0 [1 datoshi]
    /// 9E : OpCode.NOTEQUAL [32 datoshi]
    /// 9F : OpCode.JMPIFNOT 16 [2 datoshi]
    /// A1 : OpCode.LDARG2 [2 datoshi]
    /// A2 : OpCode.NEGATE [4 datoshi]
    /// A3 : OpCode.LDARG0 [2 datoshi]
    /// A4 : OpCode.CALL_L 17FFFFFF [512 datoshi]
    /// A9 : OpCode.JMPIF 04 [2 datoshi]
    /// AB : OpCode.PUSHF [1 datoshi]
    /// AC : OpCode.RET [0 datoshi]
    /// AD : OpCode.LDARG2 [2 datoshi]
    /// AE : OpCode.LDARG1 [2 datoshi]
    /// AF : OpCode.CALL_L 0CFFFFFF [512 datoshi]
    /// B4 : OpCode.DROP [2 datoshi]
    /// B5 : OpCode.LDARG3 [2 datoshi]
    /// B6 : OpCode.LDARG2 [2 datoshi]
    /// B7 : OpCode.LDARG1 [2 datoshi]
    /// B8 : OpCode.LDARG0 [2 datoshi]
    /// B9 : OpCode.CALL 04 [512 datoshi]
    /// BB : OpCode.PUSHT [1 datoshi]
    /// BC : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNaH+//8kFgwRTm8gYXV0aG9yaXphdGlvbi46enl4NwEAQA==
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.CALL_L A1FEFFFF [512 datoshi]
    /// 08 : OpCode.JMPIF 16 [2 datoshi]
    /// 0A : OpCode.PUSHDATA1 4E6F20617574686F72697A6174696F6E2E [8 datoshi]
    /// 1D : OpCode.THROW [512 datoshi]
    /// 1E : OpCode.LDARG2 [2 datoshi]
    /// 1F : OpCode.LDARG1 [2 datoshi]
    /// 20 : OpCode.LDARG0 [2 datoshi]
    /// 21 : OpCode.CALLT 0100 [32768 datoshi]
    /// 24 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion
}
