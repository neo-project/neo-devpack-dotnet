using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Nep17ContractTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep17Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":10,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":12,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":72,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":217,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":609,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":651,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":771,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":810,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":849,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":855,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":873,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":995,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep17/Nep17Contract.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAPAAD9CQQMB0VYQU1QTEVAGEBXAQAMAQA0DXBoStgmBEUQ2yFAVwABeEH2tGviQZJd6DFAVwABeAwBADQDQFcAAnl4QZv2Z85B5j8YhEBXAQF4StkoJAZFCSIGygAUs6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46DAEBeIvbKDSZcGhK2CYERRDbIUBXBAJBm/ZnznAMAQF4i9socWloQZJd6DFyakrYJgRFENshc2t5nnNrELUmBAlAaxCzJgtpaEEvWMXtIgpraWhB5j8YhAhAVwAEeErZKCQGRQkiBsoAFLOqJiQMH1RoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eUrZKCQGRQkiBsoAFLOqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOnoQtSYqDCVUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjKomBAlAehCYJhd6m3g1E////6omBAlAenk1B////0V7enl4NAQIQFcBBHp5eBPADAhUcmFuc2ZlckGVAW9heXBoC5eqJAUJIgt5NwAAcGgLl6omH3t6eBPAHwwOb25ORVAxN1BheW1lbnR5QWJ9W1JFQFcAAnmZELUmCwwGYW1vdW50OnkQsyYDQHl4NZT+//9FNQf+//95nko1If7//0ULeXgLNINAVwACeZkQtSYLDAZhbW91bnQ6eRCzJgNAeZt4NV7+//+qJg4MCWV4Y2VwdGlvbjo1w/3//3mfSjXd/f//RQt5C3g1P////0AMAf/bMDQOStgkCUrKABQoAzpAVwABeEH2tGviQZJd6DFANN5B+CfsjEBXAQE09aomFgwRTm8gQXV0aG9yaXphdGlvbiE6eErZKCQGRQkiBsoAFLMkBQkiBngQs6oME293bmVyIG11c3QgYmUgdmFsaWThNI1weAwB/9swNBZ4aBLADAhTZXRPd25lckGVAW9hQFcAAnl4QZv2Z85B5j8YhEBXAAI1ff///6omFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg19v7//0BXAAI1Vv///6omFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1mv7//0A1Mv///0AMBUhlbGxvQZv2Z85Bkl3oMUBXAQJ5JgNAeHBoC5cmCkEtUQgwE86AeHBoStkoJAZFCSIGygAUsyQFCSIGaBCzqgwRb3duZXIgbXVzdCBleGlzdHPhaAwB/9swNUH///9oCxLADAhTZXRPd25lckGVAW9hDAVXb3JsZAwFSGVsbG9Bm/ZnzkHmPxiEQFcAAzWd/v//qiYWDBFObyBhdXRob3JpemF0aW9uLjp6eXg3AQBAM3w9IQ=="));

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
    /// Script: VwEBeErZKCQGRQkiBsoAFLOqJiUMVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46DAF4i9soNJlwaErYJgRFENshQA==
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
    /// 3E : OpCode.CALL 99
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
    /// Script: VwACNX3///+qJhYMTm8gQXV0aG9yaXphdGlvbiE6eXg19v7//0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.CALL_L 7DFFFFFF
    /// 08 : OpCode.NOT
    /// 09 : OpCode.JMPIFNOT 16
    /// 0B : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21
    /// 1E : OpCode.THROW
    /// 1F : OpCode.LDARG1
    /// 20 : OpCode.LDARG0
    /// 21 : OpCode.CALL_L F6FEFFFF
    /// 26 : OpCode.RET
    /// </remarks>
    [DisplayName("burn")]
    public abstract void Burn(UInt160? account, BigInteger? amount);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACNVb///+qJhYMTm8gQXV0aG9yaXphdGlvbiE6eXg1mv7//0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.CALL_L 56FFFFFF
    /// 08 : OpCode.NOT
    /// 09 : OpCode.JMPIFNOT 16
    /// 0B : OpCode.PUSHDATA1 4E6F20417574686F72697A6174696F6E21
    /// 1E : OpCode.THROW
    /// 1F : OpCode.LDARG1
    /// 20 : OpCode.LDARG0
    /// 21 : OpCode.CALL_L 9AFEFFFF
    /// 26 : OpCode.RET
    /// </remarks>
    [DisplayName("mint")]
    public abstract void Mint(UInt160? to, BigInteger? amount);

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

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADNZ3+//+qJhYMTm8gYXV0aG9yaXphdGlvbi46enl4NwEAQA==
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.CALL_L 9DFEFFFF
    /// 08 : OpCode.NOT
    /// 09 : OpCode.JMPIFNOT 16
    /// 0B : OpCode.PUSHDATA1 4E6F20617574686F72697A6174696F6E2E
    /// 1E : OpCode.THROW
    /// 1F : OpCode.LDARG2
    /// 20 : OpCode.LDARG1
    /// 21 : OpCode.LDARG0
    /// 22 : OpCode.CALLT 0100
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("update")]
    public abstract void Update(byte[]? nefFile, string? manifest, object? data = null);

    #endregion
}
