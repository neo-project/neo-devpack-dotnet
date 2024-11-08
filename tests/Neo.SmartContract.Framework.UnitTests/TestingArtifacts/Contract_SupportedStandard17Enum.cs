using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_SupportedStandard17Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_SupportedStandard17Enum"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":1,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":35,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":184,""safe"":false},{""name"":""onNEP17Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":457,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""\u003CYour Name Or Company Here\u003E"",""Description"":""\u003CDescription Here\u003E"",""Email"":""\u003CYour Public Email Here\u003E"",""Version"":""\u003CVersion String Here\u003E"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/src/Neo.SmartContract.Template"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ8AAP3NAUBXAQAMAQA0DXBoStgmBEUQ2yFAVwABeEH2tGviQZJd6DFAVwEBeHBo2CYFCCINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46DAEBeIvbKDSucGhK2CYERRDbIUBXBAJBm/ZnznAMAQF4i9socWloQZJd6DFyakrYJgRFENshc2t5nnNrELUmBAlAa7EkC2loQS9Yxe0iCmtpaEHmPxiECEBXAQR4cGjYJgUIIg14StkoUMoAFLOrqiYkDB9UaGUgYXJndW1lbnQgImZyb20iIGlzIGludmFsaWQuOnhwaNgmBQgiDXlK2ShQygAUs6uqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOnoQtSYqDCVUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjCQECUB6EJgmFnqbeDUL////JAQJQHp5NQD///9Fe3p5eDQECEBXAQR6eXgTwAwIVHJhbnNmZXJBlQFvYXlwaNgmBQkiCnk3AABwaNiqJh97engTwB8MDm9uTkVQMTdQYXltZW50eUFifVtSRUBXAANAJwdlHg=="));

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
    /// Script: VwEBeHBo2CYFCCINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46DAEBeIvbKDSucGhK2CYERRDbIUA=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.ISNULL [2 datoshi]
    /// 07 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 09 : OpCode.PUSHT [1 datoshi]
    /// 0A : OpCode.JMP 0D [2 datoshi]
    /// 0C : OpCode.LDARG0 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISTYPE 28 [2 datoshi]
    /// 10 : OpCode.SWAP [2 datoshi]
    /// 11 : OpCode.SIZE [4 datoshi]
    /// 12 : OpCode.PUSHINT8 14 [1 datoshi]
    /// 14 : OpCode.NUMEQUAL [8 datoshi]
    /// 15 : OpCode.BOOLAND [8 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIFNOT 25 [2 datoshi]
    /// 19 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E [8 datoshi]
    /// 3B : OpCode.THROW [512 datoshi]
    /// 3C : OpCode.PUSHDATA1 01 [8 datoshi]
    /// 3F : OpCode.LDARG0 [2 datoshi]
    /// 40 : OpCode.CAT [2048 datoshi]
    /// 41 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 43 : OpCode.CALL AE [512 datoshi]
    /// 45 : OpCode.STLOC0 [2 datoshi]
    /// 46 : OpCode.LDLOC0 [2 datoshi]
    /// 47 : OpCode.DUP [2 datoshi]
    /// 48 : OpCode.ISNULL [2 datoshi]
    /// 49 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 4B : OpCode.DROP [2 datoshi]
    /// 4C : OpCode.PUSH0 [1 datoshi]
    /// 4D : OpCode.CONVERT 21 'Integer' [8192 datoshi]
    /// 4F : OpCode.RET [0 datoshi]
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
    /// Script: VwEEeHBo2CYFCCINeErZKFDKABSzq6omJAwfVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp4cGjYJgUIIg15StkoUMoAFLOrqiYiDB1UaGUgYXJndW1lbnQgInRvIiBpcyBpbnZhbGlkLjp6ELUmKgwlVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IwkBAlAehCYJhZ6m3g1C////yQECUB6eTUA////RXt6eXg0BAhA
    /// 00 : OpCode.INITSLOT 0104 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.ISNULL [2 datoshi]
    /// 07 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 09 : OpCode.PUSHT [1 datoshi]
    /// 0A : OpCode.JMP 0D [2 datoshi]
    /// 0C : OpCode.LDARG0 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISTYPE 28 [2 datoshi]
    /// 10 : OpCode.SWAP [2 datoshi]
    /// 11 : OpCode.SIZE [4 datoshi]
    /// 12 : OpCode.PUSHINT8 14 [1 datoshi]
    /// 14 : OpCode.NUMEQUAL [8 datoshi]
    /// 15 : OpCode.BOOLAND [8 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIFNOT 24 [2 datoshi]
    /// 19 : OpCode.PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E [8 datoshi]
    /// 3A : OpCode.THROW [512 datoshi]
    /// 3B : OpCode.LDARG0 [2 datoshi]
    /// 3C : OpCode.STLOC0 [2 datoshi]
    /// 3D : OpCode.LDLOC0 [2 datoshi]
    /// 3E : OpCode.ISNULL [2 datoshi]
    /// 3F : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 41 : OpCode.PUSHT [1 datoshi]
    /// 42 : OpCode.JMP 0D [2 datoshi]
    /// 44 : OpCode.LDARG1 [2 datoshi]
    /// 45 : OpCode.DUP [2 datoshi]
    /// 46 : OpCode.ISTYPE 28 [2 datoshi]
    /// 48 : OpCode.SWAP [2 datoshi]
    /// 49 : OpCode.SIZE [4 datoshi]
    /// 4A : OpCode.PUSHINT8 14 [1 datoshi]
    /// 4C : OpCode.NUMEQUAL [8 datoshi]
    /// 4D : OpCode.BOOLAND [8 datoshi]
    /// 4E : OpCode.NOT [4 datoshi]
    /// 4F : OpCode.JMPIFNOT 22 [2 datoshi]
    /// 51 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E [8 datoshi]
    /// 70 : OpCode.THROW [512 datoshi]
    /// 71 : OpCode.LDARG2 [2 datoshi]
    /// 72 : OpCode.PUSH0 [1 datoshi]
    /// 73 : OpCode.LT [8 datoshi]
    /// 74 : OpCode.JMPIFNOT 2A [2 datoshi]
    /// 76 : OpCode.PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E [8 datoshi]
    /// 9D : OpCode.THROW [512 datoshi]
    /// 9E : OpCode.LDARG0 [2 datoshi]
    /// 9F : OpCode.SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// A4 : OpCode.JMPIF 04 [2 datoshi]
    /// A6 : OpCode.PUSHF [1 datoshi]
    /// A7 : OpCode.RET [0 datoshi]
    /// A8 : OpCode.LDARG2 [2 datoshi]
    /// A9 : OpCode.PUSH0 [1 datoshi]
    /// AA : OpCode.NOTEQUAL [32 datoshi]
    /// AB : OpCode.JMPIFNOT 16 [2 datoshi]
    /// AD : OpCode.LDARG2 [2 datoshi]
    /// AE : OpCode.NEGATE [4 datoshi]
    /// AF : OpCode.LDARG0 [2 datoshi]
    /// B0 : OpCode.CALL_L 0BFFFFFF [512 datoshi]
    /// B5 : OpCode.JMPIF 04 [2 datoshi]
    /// B7 : OpCode.PUSHF [1 datoshi]
    /// B8 : OpCode.RET [0 datoshi]
    /// B9 : OpCode.LDARG2 [2 datoshi]
    /// BA : OpCode.LDARG1 [2 datoshi]
    /// BB : OpCode.CALL_L 00FFFFFF [512 datoshi]
    /// C0 : OpCode.DROP [2 datoshi]
    /// C1 : OpCode.LDARG3 [2 datoshi]
    /// C2 : OpCode.LDARG2 [2 datoshi]
    /// C3 : OpCode.LDARG1 [2 datoshi]
    /// C4 : OpCode.LDARG0 [2 datoshi]
    /// C5 : OpCode.CALL 04 [512 datoshi]
    /// C7 : OpCode.PUSHT [1 datoshi]
    /// C8 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    #endregion
}
