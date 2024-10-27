using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NEP17(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NEP17"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":7,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":9,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":43,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":186,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP17Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ8AAP2/AQwEVEVTVEAYQFcBAAwBADQNcGhK2CYERRDbIUBXAAF4Qfa0a+JBkl3oMUBXAQF4StkoJAZFCSIGygAUsyQlDCBUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjoMAQF4i9soNLRwaErYJgRFENshQFcEAkGb9mfOcAwBAXiL2yhxaWhBkl3oMXJqStgmBEUQ2yFza3mec2sQtSYECUBrsSQLaWhBL1jF7SIKa2loQeY/GIQIQFcABHhK2SgkBkUJIgbKABSzJCQMH1RoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eUrZKCQGRQkiBsoAFLMkIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMJVRoZSBhbW91bnQgbXVzdCBiZSBhIHBvc2l0aXZlIG51bWJlci46eEH4J+yMJAQJQHoQmCYWept4NRf///8kBAlAenk1DP///0V7enl4NAQIQFcBBHp5eBPADAhUcmFuc2ZlckGVAW9heXBo2CYFCSIKeTcAAHBo2KomH3t6eBPAHwwOb25ORVAxN1BheW1lbnR5QWJ9W1JFQD9ivfk="));

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
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISTYPE 28
    /// 07 : OpCode.JMPIF 06
    /// 09 : OpCode.DROP
    /// 0A : OpCode.PUSHF
    /// 0B : OpCode.JMP 06
    /// 0D : OpCode.SIZE
    /// 0E : OpCode.PUSHINT8 14
    /// 10 : OpCode.NUMEQUAL
    /// 11 : OpCode.JMPIF 25
    /// 13 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 35 : OpCode.THROW
    /// 36 : OpCode.PUSHDATA1 01
    /// 39 : OpCode.LDARG0
    /// 3A : OpCode.CAT
    /// 3B : OpCode.CONVERT 28
    /// 3D : OpCode.CALL B4
    /// 3F : OpCode.STLOC0
    /// 40 : OpCode.LDLOC0
    /// 41 : OpCode.DUP
    /// 42 : OpCode.ISNULL
    /// 43 : OpCode.JMPIFNOT 04
    /// 45 : OpCode.DROP
    /// 46 : OpCode.PUSH0
    /// 47 : OpCode.CONVERT 21
    /// 49 : OpCode.RET
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwAEeErZKCQGRQkiBsoAFLMkJAxUaGUgYXJndW1lbnQgImZyb20iIGlzIGludmFsaWQuOnlK2SgkBkUJIgbKABSzJCIMVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IwkBAlAehCYJhZ6m3g1F////yQECUB6eTUM////RXt6eXg0BAhA
    /// 00 : OpCode.INITSLOT 0004
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISTYPE 28
    /// 07 : OpCode.JMPIF 06
    /// 09 : OpCode.DROP
    /// 0A : OpCode.PUSHF
    /// 0B : OpCode.JMP 06
    /// 0D : OpCode.SIZE
    /// 0E : OpCode.PUSHINT8 14
    /// 10 : OpCode.NUMEQUAL
    /// 11 : OpCode.JMPIF 24
    /// 13 : OpCode.PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E
    /// 34 : OpCode.THROW
    /// 35 : OpCode.LDARG1
    /// 36 : OpCode.DUP
    /// 37 : OpCode.ISTYPE 28
    /// 39 : OpCode.JMPIF 06
    /// 3B : OpCode.DROP
    /// 3C : OpCode.PUSHF
    /// 3D : OpCode.JMP 06
    /// 3F : OpCode.SIZE
    /// 40 : OpCode.PUSHINT8 14
    /// 42 : OpCode.NUMEQUAL
    /// 43 : OpCode.JMPIF 22
    /// 45 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 64 : OpCode.THROW
    /// 65 : OpCode.LDARG2
    /// 66 : OpCode.PUSH0
    /// 67 : OpCode.LT
    /// 68 : OpCode.JMPIFNOT 2A
    /// 6A : OpCode.PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E
    /// 91 : OpCode.THROW
    /// 92 : OpCode.LDARG0
    /// 93 : OpCode.SYSCALL F827EC8C
    /// 98 : OpCode.JMPIF 04
    /// 9A : OpCode.PUSHF
    /// 9B : OpCode.RET
    /// 9C : OpCode.LDARG2
    /// 9D : OpCode.PUSH0
    /// 9E : OpCode.NOTEQUAL
    /// 9F : OpCode.JMPIFNOT 16
    /// A1 : OpCode.LDARG2
    /// A2 : OpCode.NEGATE
    /// A3 : OpCode.LDARG0
    /// A4 : OpCode.CALL_L 17FFFFFF
    /// A9 : OpCode.JMPIF 04
    /// AB : OpCode.PUSHF
    /// AC : OpCode.RET
    /// AD : OpCode.LDARG2
    /// AE : OpCode.LDARG1
    /// AF : OpCode.CALL_L 0CFFFFFF
    /// B4 : OpCode.DROP
    /// B5 : OpCode.LDARG3
    /// B6 : OpCode.LDARG2
    /// B7 : OpCode.LDARG1
    /// B8 : OpCode.LDARG0
    /// B9 : OpCode.CALL 04
    /// BB : OpCode.PUSHT
    /// BC : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    #endregion
}
