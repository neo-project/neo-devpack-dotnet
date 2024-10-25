using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Nep17ContractTemplate(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, Neo.SmartContract.Testing.TestingStandards.IOwnable, Neo.SmartContract.Testing.TestingStandards.IVerificable, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Nep17Contract"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":10,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":12,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":72,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":227,""safe"":false},{""name"":""getOwner"",""parameters"":[],""returntype"":""Hash160"",""offset"":639,""safe"":true},{""name"":""setOwner"",""parameters"":[{""name"":""newOwner"",""type"":""Hash160""}],""returntype"":""Void"",""offset"":681,""safe"":false},{""name"":""burn"",""parameters"":[{""name"":""account"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":801,""safe"":false},{""name"":""mint"",""parameters"":[{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}],""returntype"":""Void"",""offset"":840,""safe"":false},{""name"":""verify"",""parameters"":[],""returntype"":""Boolean"",""offset"":879,""safe"":true},{""name"":""myMethod"",""parameters"":[],""returntype"":""String"",""offset"":885,""safe"":false},{""name"":""_deploy"",""parameters"":[{""name"":""data"",""type"":""Any""},{""name"":""update"",""type"":""Boolean""}],""returntype"":""Void"",""offset"":903,""safe"":false},{""name"":""update"",""parameters"":[{""name"":""nefFile"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":1025,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]},{""name"":""SetOwner"",""parameters"":[{""name"":""previousOwner"",""type"":""Hash160""},{""name"":""newOwner"",""type"":""Hash160""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template/templates/neocontractnep17/Nep17Contract.cs"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAL9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZ1cGRhdGUDAAAPAAD9JwQMB0VYQU1QTEVAGEBXAQAMAQA0DXBoStgmBEUQ2yFAVwABeEH2tGviQZJd6DFAVwABeAwBADQDQFcAAnl4QZv2Z85B5j8YhEBXAQF4cGgLlyYFCCIReErZKCQGRQkiBsoAFLOqJiUMIFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOgwBAXiL2yg0j3BoStgmBEUQ2yFAVwQCQZv2Z85wDAEBeIvbKHFpaEGSXegxcmpK2CYERRDbIXNreZ5zaxC1JgQJQGsQsyYLaWhBL1jF7SIKa2loQeY/GIQIQFcBBHhwaAuXJgUIIhF4StkoJAZFCSIGygAUs6omJAwfVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5cGgLlyYFCCIReUrZKCQGRQkiBsoAFLOqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOnoQtSYqDCVUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjKomBAlAehCYJhd6m3g1//7//6omBAlAenk18/7//0V7enl4NAQIQFcBBHp5eBPADAhUcmFuc2ZlckGVAW9heXBoC5eqJAUJIgt5NwAAcGgLl6omH3t6eBPAHwwOb25ORVAxN1BheW1lbnR5QWJ9W1JFQFcAAnmZELUmCwwGYW1vdW50OnkQsyYDQHl4NYD+//9FNen9//95nko1A/7//0ULeXgLNINAVwACeZkQtSYLDAZhbW91bnQ6eRCzJgNAeZt4NUr+//+qJg4MCWV4Y2VwdGlvbjo1pf3//3mfSjW//f//RQt5C3g1P////0AMAf/bMDQOStgkCUrKABQoAzpAVwABeEH2tGviQZJd6DFANN5B+CfsjEBXAQE09aomFgwRTm8gQXV0aG9yaXphdGlvbiE6eErZKCQGRQkiBsoAFLMkBQkiBngQs6oME293bmVyIG11c3QgYmUgdmFsaWThNI1weAwB/9swNBZ4aBLADAhTZXRPd25lckGVAW9hQFcAAnl4QZv2Z85B5j8YhEBXAAI1ff///6omFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg19v7//0BXAAI1Vv///6omFgwRTm8gQXV0aG9yaXphdGlvbiE6eXg1mv7//0A1Mv///0AMBUhlbGxvQZv2Z85Bkl3oMUBXAQJ5JgNAeHBoC5cmCkEtUQgwE86AeHBoStkoJAZFCSIGygAUsyQFCSIGaBCzqgwRb3duZXIgbXVzdCBleGlzdHPhaAwB/9swNUH///9oCxLADAhTZXRPd25lckGVAW9hDAVXb3JsZAwFSGVsbG9Bm/ZnzkHmPxiEQFcAAzWd/v//qiYWDBFObyBhdXRob3JpemF0aW9uLjp6eXg3AQBA0ddUbA=="));

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
    /// Script: VwEBeHBoC5cmBQgiEXhK2SgkBkUJIgbKABSzqiYlDFRoZSBhcmd1bWVudCAib3duZXIiIGlzIGludmFsaWQuOgwBeIvbKDSPcGhK2CYERRDbIUA=
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
    /// 48 : OpCode.CALL 8F
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
