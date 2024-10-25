using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard17Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard17Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":1,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":2,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":36,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":187,""safe"":false},{""name"":""onNEP17Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":467,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Email"":""\u003CYour Public Email Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ8AAP3XAUBAVwEADAEANA1waErYJgRFENshQFcAAXhB9rRr4kGSXegxQFcBAXhwaAuXJgUIIg14StkoUMoAFLOrqiYlDCBUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjoMAQF4i9soNK1waErYJgRFENshQFcEAkGb9mfOcAwBAXiL2yhxaWhBkl3oMXJqStgmBEUQ2yFza3mec2sQtSYECUBrELMmC2loQS9Yxe0iCmtpaEHmPxiECEBXAQR4cGgLlyYFCCINeErZKFDKABSzq6omJAwfVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5cGgLlyYFCCINeUrZKFDKABSzq6omIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMJVRoZSBhbW91bnQgbXVzdCBiZSBhIHBvc2l0aXZlIG51bWJlci46eEH4J+yMqiYECUB6EJgmF3qbeDUH////qiYECUB6eTX7/v//RXt6eXg0BAhAVwEEenl4E8AMCFRyYW5zZmVyQZUBb2F5cGgLl6okBQkiC3k3AABwaAuXqiYfe3p4E8AfDA5vbk5FUDE3UGF5bWVudHlBYn1bUkVAVwADQB89XVo="));

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
    /// Script: VwEBeHBoC5cmBQgiDXhK2ShQygAUs6uqJiUMVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46DAF4i9soNK1waErYJgRFENshQA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 0D
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ISTYPE 28
    /// 11 : OpCode.SWAP
    /// 12 : OpCode.SIZE
    /// 13 : OpCode.PUSHINT8 14
    /// 15 : OpCode.NUMEQUAL
    /// 16 : OpCode.BOOLAND
    /// 17 : OpCode.NOT
    /// 18 : OpCode.JMPIFNOT 25
    /// 1A : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 3C : OpCode.THROW
    /// 3D : OpCode.PUSHDATA1 01
    /// 40 : OpCode.LDARG0
    /// 41 : OpCode.CAT
    /// 42 : OpCode.CONVERT 28
    /// 44 : OpCode.CALL AD
    /// 46 : OpCode.STLOC0
    /// 47 : OpCode.LDLOC0
    /// 48 : OpCode.DUP
    /// 49 : OpCode.ISNULL
    /// 4A : OpCode.JMPIFNOT 04
    /// 4C : OpCode.DROP
    /// 4D : OpCode.PUSH0
    /// 4E : OpCode.CONVERT 21
    /// 50 : OpCode.RET
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
    /// Script: VwEEeHBoC5cmBQgiDXhK2ShQygAUs6uqJiQMVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5cGgLlyYFCCINeUrZKFDKABSzq6omIgxUaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgxUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjKomBAlAehCYJhd6m3g1B////6omBAlAenk1+/7//0V7enl4NAQIQA==
    /// 00 : OpCode.INITSLOT 0104
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.JMPIFNOT 05
    /// 0A : OpCode.PUSHT
    /// 0B : OpCode.JMP 0D
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ISTYPE 28
    /// 11 : OpCode.SWAP
    /// 12 : OpCode.SIZE
    /// 13 : OpCode.PUSHINT8 14
    /// 15 : OpCode.NUMEQUAL
    /// 16 : OpCode.BOOLAND
    /// 17 : OpCode.NOT
    /// 18 : OpCode.JMPIFNOT 24
    /// 1A : OpCode.PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E
    /// 3B : OpCode.THROW
    /// 3C : OpCode.LDARG1
    /// 3D : OpCode.STLOC0
    /// 3E : OpCode.LDLOC0
    /// 3F : OpCode.PUSHNULL
    /// 40 : OpCode.EQUAL
    /// 41 : OpCode.JMPIFNOT 05
    /// 43 : OpCode.PUSHT
    /// 44 : OpCode.JMP 0D
    /// 46 : OpCode.LDARG1
    /// 47 : OpCode.DUP
    /// 48 : OpCode.ISTYPE 28
    /// 4A : OpCode.SWAP
    /// 4B : OpCode.SIZE
    /// 4C : OpCode.PUSHINT8 14
    /// 4E : OpCode.NUMEQUAL
    /// 4F : OpCode.BOOLAND
    /// 50 : OpCode.NOT
    /// 51 : OpCode.JMPIFNOT 22
    /// 53 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 72 : OpCode.THROW
    /// 73 : OpCode.LDARG2
    /// 74 : OpCode.PUSH0
    /// 75 : OpCode.LT
    /// 76 : OpCode.JMPIFNOT 2A
    /// 78 : OpCode.PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E
    /// 9F : OpCode.THROW
    /// A0 : OpCode.LDARG0
    /// A1 : OpCode.SYSCALL F827EC8C
    /// A6 : OpCode.NOT
    /// A7 : OpCode.JMPIFNOT 04
    /// A9 : OpCode.PUSHF
    /// AA : OpCode.RET
    /// AB : OpCode.LDARG2
    /// AC : OpCode.PUSH0
    /// AD : OpCode.NOTEQUAL
    /// AE : OpCode.JMPIFNOT 17
    /// B0 : OpCode.LDARG2
    /// B1 : OpCode.NEGATE
    /// B2 : OpCode.LDARG0
    /// B3 : OpCode.CALL_L 07FFFFFF
    /// B8 : OpCode.NOT
    /// B9 : OpCode.JMPIFNOT 04
    /// BB : OpCode.PUSHF
    /// BC : OpCode.RET
    /// BD : OpCode.LDARG2
    /// BE : OpCode.LDARG1
    /// BF : OpCode.CALL_L FBFEFFFF
    /// C4 : OpCode.DROP
    /// C5 : OpCode.LDARG3
    /// C6 : OpCode.LDARG2
    /// C7 : OpCode.LDARG1
    /// C8 : OpCode.LDARG0
    /// C9 : OpCode.CALL 04
    /// CB : OpCode.PUSHT
    /// CC : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    #endregion
}
