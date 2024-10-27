using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard17Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard17Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":2,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":36,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":179,""safe"":false},{""name"":""onNEP17Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":440,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Email"":""\u003CYour Public Email Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ8AAP28AUBAVwEADAEANA1waErYJgRFENshQFcAAXhB9rRr4kGSXegxQFcBAXhK2SgkBkUJIgbKABSzJCUMIFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOgwBAXiL2yg0tHBoStgmBEUQ2yFAVwQCQZv2Z85wDAEBeIvbKHFpaEGSXegxcmpK2CYERRDbIXNreZ5zaxC1JgQJQGuxJAtpaEEvWMXtIgpraWhB5j8YhAhAVwAEeErZKCQGRQkiBsoAFLMkJAwfVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5StkoJAZFCSIGygAUsyQiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgwlVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IwkBAlAehCYJhZ6m3g1F////yQECUB6eTUM////RXt6eXg0BAhAVwEEenl4E8AMCFRyYW5zZmVyQZUBb2F5cGjYJgUJIgp5NwAAcGjYqiYfe3p4E8AfDA5vbk5FUDE3UGF5bWVudHlBYn1bUkVAVwADQIfs8Vg="));

    #endregion

    #region Events

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
    public abstract string? Symbol { [DisplayName("symbol")] get; }

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract BigInteger? TotalSupply { [DisplayName("totalSupply")] get; }

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJQxUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjoMAXiL2yg0tHBoStgmBEUQ2yFA
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
    /// 3D : OpCode.CALL B4 [512 datoshi]
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
    /// Script: VwADQA==
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("onNEP17Payment")]
    public abstract void OnNEP17Payment(UInt160? from, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwAEeErZKCQGRQkiBsoAFLMkJAxUaGUgYXJndW1lbnQgImZyb20iIGlzIGludmFsaWQuOnlK2SgkBkUJIgbKABSzJCIMVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IwkBAlAehCYJhZ6m3g1F////yQECUB6eTUM////RXt6eXg0BAhA
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

    #endregion
}
