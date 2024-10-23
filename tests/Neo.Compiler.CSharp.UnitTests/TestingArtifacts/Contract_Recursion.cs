using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Recursion(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Recursion"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""factorial"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""hanoiTower"",""parameters"":[{""name"":""n"",""type"":""Integer""},{""name"":""src"",""type"":""Integer""},{""name"":""aux"",""type"":""Integer""},{""name"":""dst"",""type"":""Integer""}],""returntype"":""Array"",""offset"":50,""safe"":false},{""name"":""even"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":166,""safe"":false},{""name"":""odd"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":192,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANpXAAF4ELgMGk1pbnVzIG51bWJlciBub3Qgc3VwcG9ydGVk4XgSuCYKeHgRnzTUoEARQFcFBHgQtwwTQ291bnQgb2YgZGlza3MgPD0gMOF4EZcmGcJwaBAQEBO/ShAR0EoRedBKEnvQz2hAent5eBGfNMJwaBAQEBO/ShB40EoRedBKEnvQz3t5engRnzSmSnHKchBzIgxpa850aGzPa5xza2ow9GhAVwABeBCXJgQIQHgQtSYHeBGeIgV4EZ80A0BXAAF4EJcmBAlAeBC1Jgd4EZ4iBXgRnzTPQJ8ZMX8="));

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
    /// Script: VwABeBC4DE1pbnVzIG51bWJlciBub3Qgc3VwcG9ydGVk4XgSuCYKeHgRnzTUoEARQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.GE [8 datoshi]
    /// 06 : OpCode.PUSHDATA1 4D696E7573206E756D626572206E6F7420737570706F72746564 [8 datoshi]
    /// 22 : OpCode.ASSERTMSG [1 datoshi]
    /// 23 : OpCode.LDARG0 [2 datoshi]
    /// 24 : OpCode.PUSH2 [1 datoshi]
    /// 25 : OpCode.GE [8 datoshi]
    /// 26 : OpCode.JMPIFNOT 0A [2 datoshi]
    /// 28 : OpCode.LDARG0 [2 datoshi]
    /// 29 : OpCode.LDARG0 [2 datoshi]
    /// 2A : OpCode.PUSH1 [1 datoshi]
    /// 2B : OpCode.SUB [8 datoshi]
    /// 2C : OpCode.CALL D4 [512 datoshi]
    /// 2E : OpCode.MUL [8 datoshi]
    /// 2F : OpCode.RET [0 datoshi]
    /// 30 : OpCode.PUSH1 [1 datoshi]
    /// 31 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("factorial")]
    public abstract BigInteger? Factorial(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUEeBC3DENvdW50IG9mIGRpc2tzIDw9IDDheBGXJhnCcGgQEBATv0oQEdBKEXnQShJ70M9oQHp7eXgRnzTCcGgQEBATv0oQeNBKEXnQShJ70M97eXp4EZ80pkpxynIQcyIMaWvOdGhsz2ucc2tqMPRoQA==
    /// 00 : OpCode.INITSLOT 0504 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.GT [8 datoshi]
    /// 06 : OpCode.PUSHDATA1 436F756E74206F66206469736B73203C3D2030 [8 datoshi]
    /// 1B : OpCode.ASSERTMSG [1 datoshi]
    /// 1C : OpCode.LDARG0 [2 datoshi]
    /// 1D : OpCode.PUSH1 [1 datoshi]
    /// 1E : OpCode.EQUAL [32 datoshi]
    /// 1F : OpCode.JMPIFNOT 19 [2 datoshi]
    /// 21 : OpCode.NEWARRAY0 [16 datoshi]
    /// 22 : OpCode.STLOC0 [2 datoshi]
    /// 23 : OpCode.LDLOC0 [2 datoshi]
    /// 24 : OpCode.PUSH0 [1 datoshi]
    /// 25 : OpCode.PUSH0 [1 datoshi]
    /// 26 : OpCode.PUSH0 [1 datoshi]
    /// 27 : OpCode.PUSH3 [1 datoshi]
    /// 28 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 29 : OpCode.DUP [2 datoshi]
    /// 2A : OpCode.PUSH0 [1 datoshi]
    /// 2B : OpCode.PUSH1 [1 datoshi]
    /// 2C : OpCode.SETITEM [8192 datoshi]
    /// 2D : OpCode.DUP [2 datoshi]
    /// 2E : OpCode.PUSH1 [1 datoshi]
    /// 2F : OpCode.LDARG1 [2 datoshi]
    /// 30 : OpCode.SETITEM [8192 datoshi]
    /// 31 : OpCode.DUP [2 datoshi]
    /// 32 : OpCode.PUSH2 [1 datoshi]
    /// 33 : OpCode.LDARG3 [2 datoshi]
    /// 34 : OpCode.SETITEM [8192 datoshi]
    /// 35 : OpCode.APPEND [8192 datoshi]
    /// 36 : OpCode.LDLOC0 [2 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// 38 : OpCode.LDARG2 [2 datoshi]
    /// 39 : OpCode.LDARG3 [2 datoshi]
    /// 3A : OpCode.LDARG1 [2 datoshi]
    /// 3B : OpCode.LDARG0 [2 datoshi]
    /// 3C : OpCode.PUSH1 [1 datoshi]
    /// 3D : OpCode.SUB [8 datoshi]
    /// 3E : OpCode.CALL C2 [512 datoshi]
    /// 40 : OpCode.STLOC0 [2 datoshi]
    /// 41 : OpCode.LDLOC0 [2 datoshi]
    /// 42 : OpCode.PUSH0 [1 datoshi]
    /// 43 : OpCode.PUSH0 [1 datoshi]
    /// 44 : OpCode.PUSH0 [1 datoshi]
    /// 45 : OpCode.PUSH3 [1 datoshi]
    /// 46 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 47 : OpCode.DUP [2 datoshi]
    /// 48 : OpCode.PUSH0 [1 datoshi]
    /// 49 : OpCode.LDARG0 [2 datoshi]
    /// 4A : OpCode.SETITEM [8192 datoshi]
    /// 4B : OpCode.DUP [2 datoshi]
    /// 4C : OpCode.PUSH1 [1 datoshi]
    /// 4D : OpCode.LDARG1 [2 datoshi]
    /// 4E : OpCode.SETITEM [8192 datoshi]
    /// 4F : OpCode.DUP [2 datoshi]
    /// 50 : OpCode.PUSH2 [1 datoshi]
    /// 51 : OpCode.LDARG3 [2 datoshi]
    /// 52 : OpCode.SETITEM [8192 datoshi]
    /// 53 : OpCode.APPEND [8192 datoshi]
    /// 54 : OpCode.LDARG3 [2 datoshi]
    /// 55 : OpCode.LDARG1 [2 datoshi]
    /// 56 : OpCode.LDARG2 [2 datoshi]
    /// 57 : OpCode.LDARG0 [2 datoshi]
    /// 58 : OpCode.PUSH1 [1 datoshi]
    /// 59 : OpCode.SUB [8 datoshi]
    /// 5A : OpCode.CALL A6 [512 datoshi]
    /// 5C : OpCode.DUP [2 datoshi]
    /// 5D : OpCode.STLOC1 [2 datoshi]
    /// 5E : OpCode.SIZE [4 datoshi]
    /// 5F : OpCode.STLOC2 [2 datoshi]
    /// 60 : OpCode.PUSH0 [1 datoshi]
    /// 61 : OpCode.STLOC3 [2 datoshi]
    /// 62 : OpCode.JMP 0C [2 datoshi]
    /// 64 : OpCode.LDLOC1 [2 datoshi]
    /// 65 : OpCode.LDLOC3 [2 datoshi]
    /// 66 : OpCode.PICKITEM [64 datoshi]
    /// 67 : OpCode.STLOC4 [2 datoshi]
    /// 68 : OpCode.LDLOC0 [2 datoshi]
    /// 69 : OpCode.LDLOC4 [2 datoshi]
    /// 6A : OpCode.APPEND [8192 datoshi]
    /// 6B : OpCode.LDLOC3 [2 datoshi]
    /// 6C : OpCode.INC [4 datoshi]
    /// 6D : OpCode.STLOC3 [2 datoshi]
    /// 6E : OpCode.LDLOC3 [2 datoshi]
    /// 6F : OpCode.LDLOC2 [2 datoshi]
    /// 70 : OpCode.JMPLT F4 [2 datoshi]
    /// 72 : OpCode.LDLOC0 [2 datoshi]
    /// 73 : OpCode.RET [0 datoshi]
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
