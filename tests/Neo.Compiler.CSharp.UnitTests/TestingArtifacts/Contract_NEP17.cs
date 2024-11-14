using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NEP17(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NEP17"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":7,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":9,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":35,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":216,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":489,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP17Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ8AAP3sAQwEVEVTVEAYQFjYJhcMAQBB9rRr4kGSXegxStgmBEUQSmBAVwEBeHBo2CYFCCINeErZKFDKABSzq6omJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgRFENshQFcCAkGb9mfOERGIThBR0FASwHB4aMFFU4tQQZJd6DFK2CYERRDbIXFpeZ5xaRC1JgQJQGmxJBB4aMFFU4tQQS9Yxe0iD2l4aMFFU4tQQeY/GIQIQFcBBHhwaNgmBQgiDXhK2ShQygAUs6uqJiQMH1RoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eXBo2CYFCCINeUrZKFDKABSzq6omIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMJVRoZSBhbW91bnQgbXVzdCBiZSBhIHBvc2l0aXZlIG51bWJlci46eEH4J+yMJAQJQHoQmCYWept4Nfz+//8kBAlAenk18f7//0V7enl4NAQIQFcBBHp5eBPADAhUcmFuc2ZlckGVAW9heXBo2CYFCSIKeTcAAHBo2KomH3t6eBPAHwwOb25ORVAxN1BheW1lbnR5QWJ9W1JFQFYBQGfMK50="));

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

    #endregion
}
