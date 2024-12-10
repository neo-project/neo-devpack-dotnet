using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Recursion(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Recursion"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""factorial"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""hanoiTower"",""parameters"":[{""name"":""n"",""type"":""Integer""},{""name"":""src"",""type"":""Integer""},{""name"":""aux"",""type"":""Integer""},{""name"":""dst"",""type"":""Integer""}],""returntype"":""Array"",""offset"":52,""safe"":false},{""name"":""even"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":170,""safe"":false},{""name"":""odd"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":196,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAN5XAAF4ELgkHwwaTWludXMgbnVtYmVyIG5vdCBzdXBwb3J0ZWTgeBK4Jgp4eBGfNNKgQBFAVwUEeBC3JBgME0NvdW50IG9mIGRpc2tzIDw9IDDgeBGXJhnCcGgQEBATv0oQEdBKEXnQShJ70M9oQHp7eXgRnzTAcGgQEBATv0oQeNBKEXnQShJ70M97eXp4EZ80pEpxynIQcyIMaWvOdGhsz2ucc2tqMPRoQFcAAXgQlyYECEB4ELUmB3gRniIFeBGfNANAVwABeBCXJgQJQHgQtSYHeBGeIgV4EZ80z0DHiVku"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBCXJgQIQHgQtSYHeBGeIgV4EZ80A0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : JMPIFNOT 04 [2 datoshi]
    /// 08 : PUSHT [1 datoshi]
    /// 09 : RET [0 datoshi]
    /// 0A : LDARG0 [2 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : LT [8 datoshi]
    /// 0D : JMPIFNOT 07 [2 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : PUSH1 [1 datoshi]
    /// 11 : ADD [8 datoshi]
    /// 12 : JMP 05 [2 datoshi]
    /// 14 : LDARG0 [2 datoshi]
    /// 15 : PUSH1 [1 datoshi]
    /// 16 : SUB [8 datoshi]
    /// 17 : CALL 03 [512 datoshi]
    /// 19 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("even")]
    public abstract bool? Even(BigInteger? n);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4JB8MGk1pbnVzIG51bWJlciBub3Qgc3VwcG9ydGVk4HgSuCYKeHgRnzTSoEARQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : GE [8 datoshi]
    /// 06 : JMPIF 1F [2 datoshi]
    /// 08 : PUSHDATA1 4D696E7573206E756D626572206E6F7420737570706F72746564 [8 datoshi]
    /// 24 : ABORTMSG [0 datoshi]
    /// 25 : LDARG0 [2 datoshi]
    /// 26 : PUSH2 [1 datoshi]
    /// 27 : GE [8 datoshi]
    /// 28 : JMPIFNOT 0A [2 datoshi]
    /// 2A : LDARG0 [2 datoshi]
    /// 2B : LDARG0 [2 datoshi]
    /// 2C : PUSH1 [1 datoshi]
    /// 2D : SUB [8 datoshi]
    /// 2E : CALL D2 [512 datoshi]
    /// 30 : MUL [8 datoshi]
    /// 31 : RET [0 datoshi]
    /// 32 : PUSH1 [1 datoshi]
    /// 33 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("factorial")]
    public abstract BigInteger? Factorial(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUEeBC3JBgME0NvdW50IG9mIGRpc2tzIDw9IDDgeBGXJhnCcGgQEBATv0oQEdBKEXnQShJ70M9oQHp7eXgRnzTAcGgQEBATv0oQeNBKEXnQShJ70M97eXp4EZ80pEpxynIQcyIMaWvOdGhsz2ucc2tqMPRoQA==
    /// 00 : INITSLOT 0504 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : GT [8 datoshi]
    /// 06 : JMPIF 18 [2 datoshi]
    /// 08 : PUSHDATA1 436F756E74206F66206469736B73203C3D2030 [8 datoshi]
    /// 1D : ABORTMSG [0 datoshi]
    /// 1E : LDARG0 [2 datoshi]
    /// 1F : PUSH1 [1 datoshi]
    /// 20 : EQUAL [32 datoshi]
    /// 21 : JMPIFNOT 19 [2 datoshi]
    /// 23 : NEWARRAY0 [16 datoshi]
    /// 24 : STLOC0 [2 datoshi]
    /// 25 : LDLOC0 [2 datoshi]
    /// 26 : PUSH0 [1 datoshi]
    /// 27 : PUSH0 [1 datoshi]
    /// 28 : PUSH0 [1 datoshi]
    /// 29 : PUSH3 [1 datoshi]
    /// 2A : PACKSTRUCT [2048 datoshi]
    /// 2B : DUP [2 datoshi]
    /// 2C : PUSH0 [1 datoshi]
    /// 2D : PUSH1 [1 datoshi]
    /// 2E : SETITEM [8192 datoshi]
    /// 2F : DUP [2 datoshi]
    /// 30 : PUSH1 [1 datoshi]
    /// 31 : LDARG1 [2 datoshi]
    /// 32 : SETITEM [8192 datoshi]
    /// 33 : DUP [2 datoshi]
    /// 34 : PUSH2 [1 datoshi]
    /// 35 : LDARG3 [2 datoshi]
    /// 36 : SETITEM [8192 datoshi]
    /// 37 : APPEND [8192 datoshi]
    /// 38 : LDLOC0 [2 datoshi]
    /// 39 : RET [0 datoshi]
    /// 3A : LDARG2 [2 datoshi]
    /// 3B : LDARG3 [2 datoshi]
    /// 3C : LDARG1 [2 datoshi]
    /// 3D : LDARG0 [2 datoshi]
    /// 3E : PUSH1 [1 datoshi]
    /// 3F : SUB [8 datoshi]
    /// 40 : CALL C0 [512 datoshi]
    /// 42 : STLOC0 [2 datoshi]
    /// 43 : LDLOC0 [2 datoshi]
    /// 44 : PUSH0 [1 datoshi]
    /// 45 : PUSH0 [1 datoshi]
    /// 46 : PUSH0 [1 datoshi]
    /// 47 : PUSH3 [1 datoshi]
    /// 48 : PACKSTRUCT [2048 datoshi]
    /// 49 : DUP [2 datoshi]
    /// 4A : PUSH0 [1 datoshi]
    /// 4B : LDARG0 [2 datoshi]
    /// 4C : SETITEM [8192 datoshi]
    /// 4D : DUP [2 datoshi]
    /// 4E : PUSH1 [1 datoshi]
    /// 4F : LDARG1 [2 datoshi]
    /// 50 : SETITEM [8192 datoshi]
    /// 51 : DUP [2 datoshi]
    /// 52 : PUSH2 [1 datoshi]
    /// 53 : LDARG3 [2 datoshi]
    /// 54 : SETITEM [8192 datoshi]
    /// 55 : APPEND [8192 datoshi]
    /// 56 : LDARG3 [2 datoshi]
    /// 57 : LDARG1 [2 datoshi]
    /// 58 : LDARG2 [2 datoshi]
    /// 59 : LDARG0 [2 datoshi]
    /// 5A : PUSH1 [1 datoshi]
    /// 5B : SUB [8 datoshi]
    /// 5C : CALL A4 [512 datoshi]
    /// 5E : DUP [2 datoshi]
    /// 5F : STLOC1 [2 datoshi]
    /// 60 : SIZE [4 datoshi]
    /// 61 : STLOC2 [2 datoshi]
    /// 62 : PUSH0 [1 datoshi]
    /// 63 : STLOC3 [2 datoshi]
    /// 64 : JMP 0C [2 datoshi]
    /// 66 : LDLOC1 [2 datoshi]
    /// 67 : LDLOC3 [2 datoshi]
    /// 68 : PICKITEM [64 datoshi]
    /// 69 : STLOC4 [2 datoshi]
    /// 6A : LDLOC0 [2 datoshi]
    /// 6B : LDLOC4 [2 datoshi]
    /// 6C : APPEND [8192 datoshi]
    /// 6D : LDLOC3 [2 datoshi]
    /// 6E : INC [4 datoshi]
    /// 6F : STLOC3 [2 datoshi]
    /// 70 : LDLOC3 [2 datoshi]
    /// 71 : LDLOC2 [2 datoshi]
    /// 72 : JMPLT F4 [2 datoshi]
    /// 74 : LDLOC0 [2 datoshi]
    /// 75 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("hanoiTower")]
    public abstract IList<object>? HanoiTower(BigInteger? n, BigInteger? src, BigInteger? aux, BigInteger? dst);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBCXJgQJQHgQtSYHeBGeIgV4EZ80z0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : JMPIFNOT 04 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : RET [0 datoshi]
    /// 0A : LDARG0 [2 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : LT [8 datoshi]
    /// 0D : JMPIFNOT 07 [2 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : PUSH1 [1 datoshi]
    /// 11 : ADD [8 datoshi]
    /// 12 : JMP 05 [2 datoshi]
    /// 14 : LDARG0 [2 datoshi]
    /// 15 : PUSH1 [1 datoshi]
    /// 16 : SUB [8 datoshi]
    /// 17 : CALL CF [512 datoshi]
    /// 19 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("odd")]
    public abstract bool? Odd(BigInteger? n);

    #endregion
}
