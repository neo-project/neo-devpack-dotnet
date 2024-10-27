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
    /// Script: VwEBeHBo2CYFCCINeErZKFDKABSzq6omJQxUaGUgYXJndW1lbnQgIm93bmVyIiBpcyBpbnZhbGlkLjpBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBEUQ2yFA
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.ISNULL
    /// 07 : OpCode.JMPIFNOT 05
    /// 09 : OpCode.PUSHT
    /// 0A : OpCode.JMP 0D
    /// 0C : OpCode.LDARG0
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISTYPE 28
    /// 10 : OpCode.SWAP
    /// 11 : OpCode.SIZE
    /// 12 : OpCode.PUSHINT8 14
    /// 14 : OpCode.NUMEQUAL
    /// 15 : OpCode.BOOLAND
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIFNOT 25
    /// 19 : OpCode.PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E
    /// 3B : OpCode.THROW
    /// 3C : OpCode.SYSCALL 9BF667CE
    /// 41 : OpCode.PUSH1
    /// 42 : OpCode.PUSH1
    /// 43 : OpCode.NEWBUFFER
    /// 44 : OpCode.TUCK
    /// 45 : OpCode.PUSH0
    /// 46 : OpCode.ROT
    /// 47 : OpCode.SETITEM
    /// 48 : OpCode.SWAP
    /// 49 : OpCode.PUSH2
    /// 4A : OpCode.PACK
    /// 4B : OpCode.STLOC0
    /// 4C : OpCode.LDARG0
    /// 4D : OpCode.LDLOC0
    /// 4E : OpCode.UNPACK
    /// 4F : OpCode.DROP
    /// 50 : OpCode.REVERSE3
    /// 51 : OpCode.CAT
    /// 52 : OpCode.SWAP
    /// 53 : OpCode.SYSCALL 925DE831
    /// 58 : OpCode.DUP
    /// 59 : OpCode.ISNULL
    /// 5A : OpCode.JMPIFNOT 04
    /// 5C : OpCode.DROP
    /// 5D : OpCode.PUSH0
    /// 5E : OpCode.CONVERT 21
    /// 60 : OpCode.RET
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEEeHBo2CYFCCINeErZKFDKABSzq6omJAxUaGUgYXJndW1lbnQgImZyb20iIGlzIGludmFsaWQuOnlwaNgmBQgiDXlK2ShQygAUs6uqJiIMVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMVGhlIGFtb3VudCBtdXN0IGJlIGEgcG9zaXRpdmUgbnVtYmVyLjp4Qfgn7IwkBAlAehCYJhZ6m3g1/P7//yQECUB6eTXx/v//RXt6eXg0BAhA
    /// 00 : OpCode.INITSLOT 0104
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.ISNULL
    /// 07 : OpCode.JMPIFNOT 05
    /// 09 : OpCode.PUSHT
    /// 0A : OpCode.JMP 0D
    /// 0C : OpCode.LDARG0
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISTYPE 28
    /// 10 : OpCode.SWAP
    /// 11 : OpCode.SIZE
    /// 12 : OpCode.PUSHINT8 14
    /// 14 : OpCode.NUMEQUAL
    /// 15 : OpCode.BOOLAND
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIFNOT 24
    /// 19 : OpCode.PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E
    /// 3A : OpCode.THROW
    /// 3B : OpCode.LDARG1
    /// 3C : OpCode.STLOC0
    /// 3D : OpCode.LDLOC0
    /// 3E : OpCode.ISNULL
    /// 3F : OpCode.JMPIFNOT 05
    /// 41 : OpCode.PUSHT
    /// 42 : OpCode.JMP 0D
    /// 44 : OpCode.LDARG1
    /// 45 : OpCode.DUP
    /// 46 : OpCode.ISTYPE 28
    /// 48 : OpCode.SWAP
    /// 49 : OpCode.SIZE
    /// 4A : OpCode.PUSHINT8 14
    /// 4C : OpCode.NUMEQUAL
    /// 4D : OpCode.BOOLAND
    /// 4E : OpCode.NOT
    /// 4F : OpCode.JMPIFNOT 22
    /// 51 : OpCode.PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E
    /// 70 : OpCode.THROW
    /// 71 : OpCode.LDARG2
    /// 72 : OpCode.PUSH0
    /// 73 : OpCode.LT
    /// 74 : OpCode.JMPIFNOT 2A
    /// 76 : OpCode.PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E
    /// 9D : OpCode.THROW
    /// 9E : OpCode.LDARG0
    /// 9F : OpCode.SYSCALL F827EC8C
    /// A4 : OpCode.JMPIF 04
    /// A6 : OpCode.PUSHF
    /// A7 : OpCode.RET
    /// A8 : OpCode.LDARG2
    /// A9 : OpCode.PUSH0
    /// AA : OpCode.NOTEQUAL
    /// AB : OpCode.JMPIFNOT 16
    /// AD : OpCode.LDARG2
    /// AE : OpCode.NEGATE
    /// AF : OpCode.LDARG0
    /// B0 : OpCode.CALL_L FCFEFFFF
    /// B5 : OpCode.JMPIF 04
    /// B7 : OpCode.PUSHF
    /// B8 : OpCode.RET
    /// B9 : OpCode.LDARG2
    /// BA : OpCode.LDARG1
    /// BB : OpCode.CALL_L F1FEFFFF
    /// C0 : OpCode.DROP
    /// C1 : OpCode.LDARG3
    /// C2 : OpCode.LDARG2
    /// C3 : OpCode.LDARG1
    /// C4 : OpCode.LDARG0
    /// C5 : OpCode.CALL 04
    /// C7 : OpCode.PUSHT
    /// C8 : OpCode.RET
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    #endregion
}
