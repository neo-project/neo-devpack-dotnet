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
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.EQUAL [32 datoshi]
    /// 06 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 08 : OpCode.PUSHT [1 datoshi]
    /// 09 : OpCode.RET [0 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.PUSH0 [1 datoshi]
    /// 0C : OpCode.LT [8 datoshi]
    /// 0D : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.PUSH1 [1 datoshi]
    /// 11 : OpCode.ADD [8 datoshi]
    /// 12 : OpCode.JMP 05 [2 datoshi]
    /// 14 : OpCode.LDARG0 [2 datoshi]
    /// 15 : OpCode.PUSH1 [1 datoshi]
    /// 16 : OpCode.SUB [8 datoshi]
    /// 17 : OpCode.CALL 03 [512 datoshi]
    /// 19 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("even")]
    public abstract bool? Even(BigInteger? n);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4JB8MTWludXMgbnVtYmVyIG5vdCBzdXBwb3J0ZWTgeBK4Jgp4eBGfNNKgQBFA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.GE [8 datoshi]
    /// 06 : OpCode.JMPIF 1F [2 datoshi]
    /// 08 : OpCode.PUSHDATA1 4D696E7573206E756D626572206E6F7420737570706F72746564 [8 datoshi]
    /// 24 : OpCode.ABORTMSG [0 datoshi]
    /// 25 : OpCode.LDARG0 [2 datoshi]
    /// 26 : OpCode.PUSH2 [1 datoshi]
    /// 27 : OpCode.GE [8 datoshi]
    /// 28 : OpCode.JMPIFNOT 0A [2 datoshi]
    /// 2A : OpCode.LDARG0 [2 datoshi]
    /// 2B : OpCode.LDARG0 [2 datoshi]
    /// 2C : OpCode.PUSH1 [1 datoshi]
    /// 2D : OpCode.SUB [8 datoshi]
    /// 2E : OpCode.CALL D2 [512 datoshi]
    /// 30 : OpCode.MUL [8 datoshi]
    /// 31 : OpCode.RET [0 datoshi]
    /// 32 : OpCode.PUSH1 [1 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("factorial")]
    public abstract BigInteger? Factorial(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUEeBC3JBgMQ291bnQgb2YgZGlza3MgPD0gMOB4EZcmGcJwaBAQEBO/ShAR0EoRedBKEnvQz2hAent5eBGfNMBwaBAQEBO/ShB40EoRedBKEnvQz3t5engRnzSkSnHKchBzIgxpa850aGzPa5xza2ow9GhA
    /// 00 : OpCode.INITSLOT 0504 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.GT [8 datoshi]
    /// 06 : OpCode.JMPIF 18 [2 datoshi]
    /// 08 : OpCode.PUSHDATA1 436F756E74206F66206469736B73203C3D2030 [8 datoshi]
    /// 1D : OpCode.ABORTMSG [0 datoshi]
    /// 1E : OpCode.LDARG0 [2 datoshi]
    /// 1F : OpCode.PUSH1 [1 datoshi]
    /// 20 : OpCode.EQUAL [32 datoshi]
    /// 21 : OpCode.JMPIFNOT 19 [2 datoshi]
    /// 23 : OpCode.NEWARRAY0 [16 datoshi]
    /// 24 : OpCode.STLOC0 [2 datoshi]
    /// 25 : OpCode.LDLOC0 [2 datoshi]
    /// 26 : OpCode.PUSH0 [1 datoshi]
    /// 27 : OpCode.PUSH0 [1 datoshi]
    /// 28 : OpCode.PUSH0 [1 datoshi]
    /// 29 : OpCode.PUSH3 [1 datoshi]
    /// 2A : OpCode.PACKSTRUCT [2048 datoshi]
    /// 2B : OpCode.DUP [2 datoshi]
    /// 2C : OpCode.PUSH0 [1 datoshi]
    /// 2D : OpCode.PUSH1 [1 datoshi]
    /// 2E : OpCode.SETITEM [8192 datoshi]
    /// 2F : OpCode.DUP [2 datoshi]
    /// 30 : OpCode.PUSH1 [1 datoshi]
    /// 31 : OpCode.LDARG1 [2 datoshi]
    /// 32 : OpCode.SETITEM [8192 datoshi]
    /// 33 : OpCode.DUP [2 datoshi]
    /// 34 : OpCode.PUSH2 [1 datoshi]
    /// 35 : OpCode.LDARG3 [2 datoshi]
    /// 36 : OpCode.SETITEM [8192 datoshi]
    /// 37 : OpCode.APPEND [8192 datoshi]
    /// 38 : OpCode.LDLOC0 [2 datoshi]
    /// 39 : OpCode.RET [0 datoshi]
    /// 3A : OpCode.LDARG2 [2 datoshi]
    /// 3B : OpCode.LDARG3 [2 datoshi]
    /// 3C : OpCode.LDARG1 [2 datoshi]
    /// 3D : OpCode.LDARG0 [2 datoshi]
    /// 3E : OpCode.PUSH1 [1 datoshi]
    /// 3F : OpCode.SUB [8 datoshi]
    /// 40 : OpCode.CALL C0 [512 datoshi]
    /// 42 : OpCode.STLOC0 [2 datoshi]
    /// 43 : OpCode.LDLOC0 [2 datoshi]
    /// 44 : OpCode.PUSH0 [1 datoshi]
    /// 45 : OpCode.PUSH0 [1 datoshi]
    /// 46 : OpCode.PUSH0 [1 datoshi]
    /// 47 : OpCode.PUSH3 [1 datoshi]
    /// 48 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 49 : OpCode.DUP [2 datoshi]
    /// 4A : OpCode.PUSH0 [1 datoshi]
    /// 4B : OpCode.LDARG0 [2 datoshi]
    /// 4C : OpCode.SETITEM [8192 datoshi]
    /// 4D : OpCode.DUP [2 datoshi]
    /// 4E : OpCode.PUSH1 [1 datoshi]
    /// 4F : OpCode.LDARG1 [2 datoshi]
    /// 50 : OpCode.SETITEM [8192 datoshi]
    /// 51 : OpCode.DUP [2 datoshi]
    /// 52 : OpCode.PUSH2 [1 datoshi]
    /// 53 : OpCode.LDARG3 [2 datoshi]
    /// 54 : OpCode.SETITEM [8192 datoshi]
    /// 55 : OpCode.APPEND [8192 datoshi]
    /// 56 : OpCode.LDARG3 [2 datoshi]
    /// 57 : OpCode.LDARG1 [2 datoshi]
    /// 58 : OpCode.LDARG2 [2 datoshi]
    /// 59 : OpCode.LDARG0 [2 datoshi]
    /// 5A : OpCode.PUSH1 [1 datoshi]
    /// 5B : OpCode.SUB [8 datoshi]
    /// 5C : OpCode.CALL A4 [512 datoshi]
    /// 5E : OpCode.DUP [2 datoshi]
    /// 5F : OpCode.STLOC1 [2 datoshi]
    /// 60 : OpCode.SIZE [4 datoshi]
    /// 61 : OpCode.STLOC2 [2 datoshi]
    /// 62 : OpCode.PUSH0 [1 datoshi]
    /// 63 : OpCode.STLOC3 [2 datoshi]
    /// 64 : OpCode.JMP 0C [2 datoshi]
    /// 66 : OpCode.LDLOC1 [2 datoshi]
    /// 67 : OpCode.LDLOC3 [2 datoshi]
    /// 68 : OpCode.PICKITEM [64 datoshi]
    /// 69 : OpCode.STLOC4 [2 datoshi]
    /// 6A : OpCode.LDLOC0 [2 datoshi]
    /// 6B : OpCode.LDLOC4 [2 datoshi]
    /// 6C : OpCode.APPEND [8192 datoshi]
    /// 6D : OpCode.LDLOC3 [2 datoshi]
    /// 6E : OpCode.INC [4 datoshi]
    /// 6F : OpCode.STLOC3 [2 datoshi]
    /// 70 : OpCode.LDLOC3 [2 datoshi]
    /// 71 : OpCode.LDLOC2 [2 datoshi]
    /// 72 : OpCode.JMPLT F4 [2 datoshi]
    /// 74 : OpCode.LDLOC0 [2 datoshi]
    /// 75 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("hanoiTower")]
    public abstract IList<object>? HanoiTower(BigInteger? n, BigInteger? src, BigInteger? aux, BigInteger? dst);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBCXJgQJQHgQtSYHeBGeIgV4EZ80z0A=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.EQUAL [32 datoshi]
    /// 06 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.RET [0 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.PUSH0 [1 datoshi]
    /// 0C : OpCode.LT [8 datoshi]
    /// 0D : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.PUSH1 [1 datoshi]
    /// 11 : OpCode.ADD [8 datoshi]
    /// 12 : OpCode.JMP 05 [2 datoshi]
    /// 14 : OpCode.LDARG0 [2 datoshi]
    /// 15 : OpCode.PUSH1 [1 datoshi]
    /// 16 : OpCode.SUB [8 datoshi]
    /// 17 : OpCode.CALL CF [512 datoshi]
    /// 19 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("odd")]
    public abstract bool? Odd(BigInteger? n);

    #endregion
}
