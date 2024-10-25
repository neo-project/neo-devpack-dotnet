using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard17Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard17Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":2,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":36,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":191,""safe"":false},{""name"":""onNEP17Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":479,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Email"":""\u003CYour Public Email Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ8AAP3jAUBAVwEADAEANA1waErYJgRFENshQFcAAXhB9rRr4kGSXegxQFcBAXhwaAuXJgUIIhF4StkoJAZFCSIGygAUs6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46DAEBeIvbKDSpcGhK2CYERRDbIUBXBAJBm/ZnznAMAQF4i9socWloQZJd6DFyakrYJgRFENshc2t5nnNrELUmBAlAaxCzJgtpaEEvWMXtIgpraWhB5j8YhAhAVwEEeHBoC5cmBQgiEXhK2SgkBkUJIgbKABSzqiYkDB9UaGUgYXJndW1lbnQgImZyb20iIGlzIGludmFsaWQuOnlwaAuXJgUIIhF5StkoJAZFCSIGygAUs6omIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMJVRoZSBhbW91bnQgbXVzdCBiZSBhIHBvc2l0aXZlIG51bWJlci46eEH4J+yMqiYECUB6EJgmF3qbeDX//v//qiYECUB6eTXz/v//RXt6eXg0BAhAVwEEenl4E8AMCFRyYW5zZmVyQZUBb2F5cGgLl6okBQkiC3k3AABwaAuXqiYfe3p4E8AfDA5vbk5FUDE3UGF5bWVudHlBYn1bUkVAVwADQFvN9KQ="));

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
    /// Script: VwEBeHBoC5cmBQgiEXhK2SgkBkUJIgbKABSzqiYlDFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOgwBeIvbKDSpcGhK2CYERRDbIUA=
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 11
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ISTYPE 28
    /// 11 : OpCode.JMPIF 06
    /// 13 : OpCode.DROP
    /// 14 : OpCode.PUSHF
    /// 15 : OpCode.JMP 06
    /// 17 : OpCode.SIZE
    /// 18 : OpCode.PUSHINT8 14
    /// 1A : OpCode.NUMEQUAL
    /// 1B : OpCode.NOT
    /// 1C : OpCode.JMPIFNOT 25
    /// 1E : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 40 : OpCode.THROW
    /// 41 : OpCode.PUSHDATA1 01
    /// 44 : OpCode.LDARG0
    /// 45 : OpCode.CAT
    /// 46 : OpCode.CONVERT 28
    /// 48 : OpCode.CALL A9
    /// 4A : OpCode.STLOC0
    /// 4B : OpCode.LDLOC0
    /// 4C : OpCode.DUP
    /// 4D : OpCode.ISNULL
    /// 4E : OpCode.JMPIFNOT 04
    /// 50 : OpCode.DROP
    /// 51 : OpCode.PUSH0
    /// 52 : OpCode.CONVERT 21
    /// 54 : OpCode.RET
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
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.RET
    /// </remarks>
    [DisplayName("onNEP17Payment")]
    public abstract void OnNEP17Payment(UInt160? from, BigInteger? amount, object? data = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEEeHBoC5cmBQgiEXhK2SgkBkUJIgbKABSzqiYkDFRoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eXBoC5cmBQgiEXlK2SgkBkUJIgbKABSzqiYiDFRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOnoQtSYqDFRoZSBhbW91bnQgbXVzdCBiZSBhIHBvc2l0aXZlIG51bWJlci46eEH4J+yMqiYECUB6EJgmF3qbeDX//v//qiYECUB6eTXz/v//RXt6eXg0BAhA
    /// 00 : OpCode.INITSLOT 0104
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 11
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ISTYPE 28
    /// 11 : OpCode.JMPIF 06
    /// 13 : OpCode.DROP
    /// 14 : OpCode.PUSHF
    /// 15 : OpCode.JMP 06
    /// 17 : OpCode.SIZE
    /// 18 : OpCode.PUSHINT8 14
    /// 1A : OpCode.NUMEQUAL
    /// 1B : OpCode.NOT
    /// 1C : OpCode.JMPIFNOT 24
    /// 1E : OpCode.PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E
    /// 3F : OpCode.THROW
    /// 40 : OpCode.LDARG1
    /// 41 : OpCode.STLOC0
    /// 42 : OpCode.LDLOC0
    /// 43 : OpCode.PUSHNULL
    /// 44 : OpCode.EQUAL
    /// 45 : OpCode.JMPIFNOT 05
    /// 47 : OpCode.PUSHT
    /// 48 : OpCode.JMP 11
    /// 4A : OpCode.LDARG1
    /// 4B : OpCode.DUP
    /// 4C : OpCode.ISTYPE 28
    /// 4E : OpCode.JMPIF 06
    /// 50 : OpCode.DROP
    /// 51 : OpCode.PUSHF
    /// 52 : OpCode.JMP 06
    /// 54 : OpCode.SIZE
    /// 55 : OpCode.PUSHINT8 14
    /// 57 : OpCode.NUMEQUAL
    /// 58 : OpCode.NOT
    /// 59 : OpCode.JMPIFNOT 22
    /// 5B : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 7A : OpCode.THROW
    /// 7B : OpCode.LDARG2
    /// 7C : OpCode.PUSH0
    /// 7D : OpCode.LT
    /// 7E : OpCode.JMPIFNOT 2A
    /// 80 : OpCode.PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E
    /// A7 : OpCode.THROW
    /// A8 : OpCode.LDARG0
    /// A9 : OpCode.SYSCALL F827EC8C
    /// AE : OpCode.NOT
    /// AF : OpCode.JMPIFNOT 04
    /// B1 : OpCode.PUSHF
    /// B2 : OpCode.RET
    /// B3 : OpCode.LDARG2
    /// B4 : OpCode.PUSH0
    /// B5 : OpCode.NOTEQUAL
    /// B6 : OpCode.JMPIFNOT 17
    /// B8 : OpCode.LDARG2
    /// B9 : OpCode.NEGATE
    /// BA : OpCode.LDARG0
    /// BB : OpCode.CALL_L FFFEFFFF
    /// C0 : OpCode.NOT
    /// C1 : OpCode.JMPIFNOT 04
    /// C3 : OpCode.PUSHF
    /// C4 : OpCode.RET
    /// C5 : OpCode.LDARG2
    /// C6 : OpCode.LDARG1
    /// C7 : OpCode.CALL_L F3FEFFFF
    /// CC : OpCode.DROP
    /// CD : OpCode.LDARG3
    /// CE : OpCode.LDARG2
    /// CF : OpCode.LDARG1
    /// D0 : OpCode.LDARG0
    /// D1 : OpCode.CALL 04
    /// D3 : OpCode.PUSHT
    /// D4 : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    #endregion
}
