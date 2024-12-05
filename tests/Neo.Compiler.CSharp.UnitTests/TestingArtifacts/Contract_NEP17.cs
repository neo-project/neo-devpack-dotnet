using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NEP17(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), Neo.SmartContract.Testing.TestingStandards.INep17Standard, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NEP17"",""groups"":[],""features"":{},""supportedstandards"":[""NEP-17""],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true},{""name"":""decimals"",""parameters"":[],""returntype"":""Integer"",""offset"":7,""safe"":true},{""name"":""totalSupply"",""parameters"":[],""returntype"":""Integer"",""offset"":9,""safe"":true},{""name"":""balanceOf"",""parameters"":[{""name"":""owner"",""type"":""Hash160""}],""returntype"":""Integer"",""offset"":29,""safe"":true},{""name"":""transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":207,""safe"":false}],""events"":[{""name"":""Transfer"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""to"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""getContract""]},{""contract"":""*"",""methods"":[""onNEP17Payment""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH9o/pDRupTKiWPxJfdrdtkN8n9/wtnZXRDb250cmFjdAEAAQ8AAP3eAQwEVEVTVEAYQAwBAEH2tGviQZJd6DFK2CYERRBAVwEBeErZKCQGRQkiBsoAFLMkJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgVFEEDbIUBXAgJBm/ZnzhERiE4QUdBQEsBweGjBRVOLUEGSXegxStgmBkUQIgTbIXFpeZ5xaRC1JgQJQGmxJBB4aMFFU4tQQS9Yxe0iD2l4aMFFU4tQQeY/GIQIQFcBBHhK2SgkBkUJIgbKABSzJCQMH1RoZSBhcmd1bWVudCAiZnJvbSIgaXMgaW52YWxpZC46eXBo2CYFCCIReUrZKCQGRQkiBsoAFLOqJiIMHVRoZSBhcmd1bWVudCAidG8iIGlzIGludmFsaWQuOnoQtSYqDCVUaGUgYW1vdW50IG11c3QgYmUgYSBwb3NpdGl2ZSBudW1iZXIuOnhB+CfsjCQECUB6EJgmFnqbeDX8/v//JAQJQHp5NfH+//9Fe3p5eDQECEBXAQR6eXgTwAwIVHJhbnNmZXJBlQFvYXlwaNgmBQkiCnk3AABwaNiqJh97engTwB8MDm9uTkVQMTdQYXltZW50eUFifVtSRUDAjVbv"));

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
    /// Script: VwEBeErZKCQGRQkiBsoAFLMkJQwgVGhlIGFyZ3VtZW50ICJvd25lciIgaXMgaW52YWxpZC46QZv2Z84REYhOEFHQUBLAcHhowUVTi1BBkl3oMUrYJgVFEEDbIUA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISTYPE 28 'ByteString' [2 datoshi]
    /// 07 : JMPIF 06 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : PUSHF [1 datoshi]
    /// 0B : JMP 06 [2 datoshi]
    /// 0D : SIZE [4 datoshi]
    /// 0E : PUSHINT8 14 [1 datoshi]
    /// 10 : NUMEQUAL [8 datoshi]
    /// 11 : JMPIF 25 [2 datoshi]
    /// 13 : PUSHDATA1 54686520617267756D656E7420226F776E65722220697320696E76616C69642E [8 datoshi]
    /// 35 : THROW [512 datoshi]
    /// 36 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 3B : PUSH1 [1 datoshi]
    /// 3C : PUSH1 [1 datoshi]
    /// 3D : NEWBUFFER [256 datoshi]
    /// 3E : TUCK [2 datoshi]
    /// 3F : PUSH0 [1 datoshi]
    /// 40 : ROT [2 datoshi]
    /// 41 : SETITEM [8192 datoshi]
    /// 42 : SWAP [2 datoshi]
    /// 43 : PUSH2 [1 datoshi]
    /// 44 : PACK [2048 datoshi]
    /// 45 : STLOC0 [2 datoshi]
    /// 46 : LDARG0 [2 datoshi]
    /// 47 : LDLOC0 [2 datoshi]
    /// 48 : UNPACK [2048 datoshi]
    /// 49 : DROP [2 datoshi]
    /// 4A : REVERSE3 [2 datoshi]
    /// 4B : CAT [2048 datoshi]
    /// 4C : SWAP [2 datoshi]
    /// 4D : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 52 : DUP [2 datoshi]
    /// 53 : ISNULL [2 datoshi]
    /// 54 : JMPIFNOT 05 [2 datoshi]
    /// 56 : DROP [2 datoshi]
    /// 57 : PUSH0 [1 datoshi]
    /// 58 : RET [0 datoshi]
    /// 59 : CONVERT 21 'Integer' [8192 datoshi]
    /// 5B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("balanceOf")]
    public abstract BigInteger? BalanceOf(UInt160? owner);

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEEeErZKCQGRQkiBsoAFLMkJAwfVGhlIGFyZ3VtZW50ICJmcm9tIiBpcyBpbnZhbGlkLjp5cGjYJgUIIhF5StkoJAZFCSIGygAUs6omIgwdVGhlIGFyZ3VtZW50ICJ0byIgaXMgaW52YWxpZC46ehC1JioMJVRoZSBhbW91bnQgbXVzdCBiZSBhIHBvc2l0aXZlIG51bWJlci46eEH4J+yMJAQJQHoQmCYWept4Nfz+//8kBAlAenk18f7//0V7enl4NAQIQA==
    /// 00 : INITSLOT 0104 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISTYPE 28 'ByteString' [2 datoshi]
    /// 07 : JMPIF 06 [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : PUSHF [1 datoshi]
    /// 0B : JMP 06 [2 datoshi]
    /// 0D : SIZE [4 datoshi]
    /// 0E : PUSHINT8 14 [1 datoshi]
    /// 10 : NUMEQUAL [8 datoshi]
    /// 11 : JMPIF 24 [2 datoshi]
    /// 13 : PUSHDATA1 54686520617267756D656E74202266726F6D2220697320696E76616C69642E [8 datoshi]
    /// 34 : THROW [512 datoshi]
    /// 35 : LDARG1 [2 datoshi]
    /// 36 : STLOC0 [2 datoshi]
    /// 37 : LDLOC0 [2 datoshi]
    /// 38 : ISNULL [2 datoshi]
    /// 39 : JMPIFNOT 05 [2 datoshi]
    /// 3B : PUSHT [1 datoshi]
    /// 3C : JMP 11 [2 datoshi]
    /// 3E : LDARG1 [2 datoshi]
    /// 3F : DUP [2 datoshi]
    /// 40 : ISTYPE 28 'ByteString' [2 datoshi]
    /// 42 : JMPIF 06 [2 datoshi]
    /// 44 : DROP [2 datoshi]
    /// 45 : PUSHF [1 datoshi]
    /// 46 : JMP 06 [2 datoshi]
    /// 48 : SIZE [4 datoshi]
    /// 49 : PUSHINT8 14 [1 datoshi]
    /// 4B : NUMEQUAL [8 datoshi]
    /// 4C : NOT [4 datoshi]
    /// 4D : JMPIFNOT 22 [2 datoshi]
    /// 4F : PUSHDATA1 54686520617267756D656E742022746F2220697320696E76616C69642E [8 datoshi]
    /// 6E : THROW [512 datoshi]
    /// 6F : LDARG2 [2 datoshi]
    /// 70 : PUSH0 [1 datoshi]
    /// 71 : LT [8 datoshi]
    /// 72 : JMPIFNOT 2A [2 datoshi]
    /// 74 : PUSHDATA1 54686520616D6F756E74206D757374206265206120706F736974697665206E756D6265722E [8 datoshi]
    /// 9B : THROW [512 datoshi]
    /// 9C : LDARG0 [2 datoshi]
    /// 9D : SYSCALL F827EC8C 'System.Runtime.CheckWitness' [1024 datoshi]
    /// A2 : JMPIF 04 [2 datoshi]
    /// A4 : PUSHF [1 datoshi]
    /// A5 : RET [0 datoshi]
    /// A6 : LDARG2 [2 datoshi]
    /// A7 : PUSH0 [1 datoshi]
    /// A8 : NOTEQUAL [32 datoshi]
    /// A9 : JMPIFNOT 16 [2 datoshi]
    /// AB : LDARG2 [2 datoshi]
    /// AC : NEGATE [4 datoshi]
    /// AD : LDARG0 [2 datoshi]
    /// AE : CALL_L FCFEFFFF [512 datoshi]
    /// B3 : JMPIF 04 [2 datoshi]
    /// B5 : PUSHF [1 datoshi]
    /// B6 : RET [0 datoshi]
    /// B7 : LDARG2 [2 datoshi]
    /// B8 : LDARG1 [2 datoshi]
    /// B9 : CALL_L F1FEFFFF [512 datoshi]
    /// BE : DROP [2 datoshi]
    /// BF : LDARG3 [2 datoshi]
    /// C0 : LDARG2 [2 datoshi]
    /// C1 : LDARG1 [2 datoshi]
    /// C2 : LDARG0 [2 datoshi]
    /// C3 : CALL 04 [512 datoshi]
    /// C5 : PUSHT [1 datoshi]
    /// C6 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("transfer")]
    public abstract bool? Transfer(UInt160? from, UInt160? to, BigInteger? amount, object? data = null);

    #endregion
}
