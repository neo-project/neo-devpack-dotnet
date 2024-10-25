using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NEP17(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NEP17"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":7,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":9,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":43,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":188,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP17Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ8AAP3IAQwEVEVTVEAYQFcBAAwBADQNcGhK2CYERRDbIUBXAAF4Qfa0a+JBkl3oMUBXAQF4StkoJAZFCSIGygAUs6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46DAEBeIvbKDSzcGhK2CYERRDbIUBXBAJBm/ZnznAMAQF4i9socWloQZJd6DFyakrYJgRFENshc2t5nnNrELUmBAlAaxCzJgtpaEEvWMXtIgpraWhB5j8YhAhAVwAEeErZKCQGRQkiBsoAFLOqJiQMH1RoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eUrZKCQGRQkiBsoAFLOqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOnoQtSYqDCVUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjKomBAlAehCYJhd6m3g1E////6omBAlAenk1B////0V7enl4NAQIQFcBBHp5eBPADAhUcmFuc2ZlckGVAW9heXBoC5eqJAUJIgt5NwAAcGgLl6omH3t6eBPAHwwOb25ORVAxN1BheW1lbnR5QWJ9W1JFQB0jt9E="));

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
    /// Script: VwEBeErZKCQGRQkiBsoAFLOqJiUMVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46DAF4i9soNLNwaErYJgRFENshQA==
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
    /// 11 : OpCode.NOT
    /// 12 : OpCode.JMPIFNOT 25
    /// 14 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 36 : OpCode.THROW
    /// 37 : OpCode.PUSHDATA1 01
    /// 3A : OpCode.LDARG0
    /// 3B : OpCode.CAT
    /// 3C : OpCode.CONVERT 28
    /// 3E : OpCode.CALL B3
    /// 40 : OpCode.STLOC0
    /// 41 : OpCode.LDLOC0
    /// 42 : OpCode.DUP
    /// 43 : OpCode.ISNULL
    /// 44 : OpCode.JMPIFNOT 04
    /// 46 : OpCode.DROP
    /// 47 : OpCode.PUSH0
    /// 48 : OpCode.CONVERT 21
    /// 4A : OpCode.RET
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwAEeErZKCQGRQkiBsoAFLOqJiQMVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5StkoJAZFCSIGygAUs6omIgxUaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgxUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjKomBAlAehCYJhd6m3g1E////6omBAlAenk1B////0V7enl4NAQIQA==
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
    /// 11 : OpCode.NOT
    /// 12 : OpCode.JMPIFNOT 24
    /// 14 : OpCode.PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E
    /// 35 : OpCode.THROW
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.DUP
    /// 38 : OpCode.ISTYPE 28
    /// 3A : OpCode.JMPIF 06
    /// 3C : OpCode.DROP
    /// 3D : OpCode.PUSHF
    /// 3E : OpCode.JMP 06
    /// 40 : OpCode.SIZE
    /// 41 : OpCode.PUSHINT8 14
    /// 43 : OpCode.NUMEQUAL
    /// 44 : OpCode.NOT
    /// 45 : OpCode.JMPIFNOT 22
    /// 47 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 66 : OpCode.THROW
    /// 67 : OpCode.LDARG2
    /// 68 : OpCode.PUSH0
    /// 69 : OpCode.LT
    /// 6A : OpCode.JMPIFNOT 2A
    /// 6C : OpCode.PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E
    /// 93 : OpCode.THROW
    /// 94 : OpCode.LDARG0
    /// 95 : OpCode.SYSCALL F827EC8C
    /// 9A : OpCode.NOT
    /// 9B : OpCode.JMPIFNOT 04
    /// 9D : OpCode.PUSHF
    /// 9E : OpCode.RET
    /// 9F : OpCode.LDARG2
    /// A0 : OpCode.PUSH0
    /// A1 : OpCode.NOTEQUAL
    /// A2 : OpCode.JMPIFNOT 17
    /// A4 : OpCode.LDARG2
    /// A5 : OpCode.NEGATE
    /// A6 : OpCode.LDARG0
    /// A7 : OpCode.CALL_L 13FFFFFF
    /// AC : OpCode.NOT
    /// AD : OpCode.JMPIFNOT 04
    /// AF : OpCode.PUSHF
    /// B0 : OpCode.RET
    /// B1 : OpCode.LDARG2
    /// B2 : OpCode.LDARG1
    /// B3 : OpCode.CALL_L 07FFFFFF
    /// B8 : OpCode.DROP
    /// B9 : OpCode.LDARG3
    /// BA : OpCode.LDARG2
    /// BB : OpCode.LDARG1
    /// BC : OpCode.LDARG0
    /// BD : OpCode.CALL 04
    /// BF : OpCode.PUSHT
    /// C0 : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    #endregion
}
