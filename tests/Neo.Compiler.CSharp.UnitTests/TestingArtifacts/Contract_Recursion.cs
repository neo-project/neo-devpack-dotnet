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
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIFNOT 04
    /// 08 : OpCode.PUSHT
    /// 09 : OpCode.RET
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.LT
    /// 0D : OpCode.JMPIFNOT 07
    /// 0F : OpCode.LDARG0
    /// 10 : OpCode.PUSH1
    /// 11 : OpCode.ADD
    /// 12 : OpCode.JMP 05
    /// 14 : OpCode.LDARG0
    /// 15 : OpCode.PUSH1
    /// 16 : OpCode.SUB
    /// 17 : OpCode.CALL 03
    /// 19 : OpCode.RET
    /// </remarks>
    [DisplayName("even")]
    public abstract bool? Even(BigInteger? n);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4DE1pbnVzIG51bWJlciBub3Qgc3VwcG9ydGVk4XgSuCYKeHgRnzTUoEARQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.GE
    /// 06 : OpCode.PUSHDATA1 4D696E7573206E756D626572206E6F7420737570706F72746564
    /// 22 : OpCode.ASSERTMSG
    /// 23 : OpCode.LDARG0
    /// 24 : OpCode.PUSH2
    /// 25 : OpCode.GE
    /// 26 : OpCode.JMPIFNOT 0A
    /// 28 : OpCode.LDARG0
    /// 29 : OpCode.LDARG0
    /// 2A : OpCode.PUSH1
    /// 2B : OpCode.SUB
    /// 2C : OpCode.CALL D4
    /// 2E : OpCode.MUL
    /// 2F : OpCode.RET
    /// 30 : OpCode.PUSH1
    /// 31 : OpCode.RET
    /// </remarks>
    [DisplayName("factorial")]
    public abstract BigInteger? Factorial(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUEeBC3DENvdW50IG9mIGRpc2tzIDw9IDDheBGXJhnCcGgQEBATv0oQEdBKEXnQShJ70M9oQHp7eXgRnzTCcGgQEBATv0oQeNBKEXnQShJ70M97eXp4EZ80pkpxynIQcyIMaWvOdGhsz2ucc2tqMPRoQA==
    /// 00 : OpCode.INITSLOT 0504
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.GT
    /// 06 : OpCode.PUSHDATA1 436F756E74206F66206469736B73203C3D2030
    /// 1B : OpCode.ASSERTMSG
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.PUSH1
    /// 1E : OpCode.EQUAL
    /// 1F : OpCode.JMPIFNOT 19
    /// 21 : OpCode.NEWARRAY0
    /// 22 : OpCode.STLOC0
    /// 23 : OpCode.LDLOC0
    /// 24 : OpCode.PUSH0
    /// 25 : OpCode.PUSH0
    /// 26 : OpCode.PUSH0
    /// 27 : OpCode.PUSH3
    /// 28 : OpCode.PACKSTRUCT
    /// 29 : OpCode.DUP
    /// 2A : OpCode.PUSH0
    /// 2B : OpCode.PUSH1
    /// 2C : OpCode.SETITEM
    /// 2D : OpCode.DUP
    /// 2E : OpCode.PUSH1
    /// 2F : OpCode.LDARG1
    /// 30 : OpCode.SETITEM
    /// 31 : OpCode.DUP
    /// 32 : OpCode.PUSH2
    /// 33 : OpCode.LDARG3
    /// 34 : OpCode.SETITEM
    /// 35 : OpCode.APPEND
    /// 36 : OpCode.LDLOC0
    /// 37 : OpCode.RET
    /// 38 : OpCode.LDARG2
    /// 39 : OpCode.LDARG3
    /// 3A : OpCode.LDARG1
    /// 3B : OpCode.LDARG0
    /// 3C : OpCode.PUSH1
    /// 3D : OpCode.SUB
    /// 3E : OpCode.CALL C2
    /// 40 : OpCode.STLOC0
    /// 41 : OpCode.LDLOC0
    /// 42 : OpCode.PUSH0
    /// 43 : OpCode.PUSH0
    /// 44 : OpCode.PUSH0
    /// 45 : OpCode.PUSH3
    /// 46 : OpCode.PACKSTRUCT
    /// 47 : OpCode.DUP
    /// 48 : OpCode.PUSH0
    /// 49 : OpCode.LDARG0
    /// 4A : OpCode.SETITEM
    /// 4B : OpCode.DUP
    /// 4C : OpCode.PUSH1
    /// 4D : OpCode.LDARG1
    /// 4E : OpCode.SETITEM
    /// 4F : OpCode.DUP
    /// 50 : OpCode.PUSH2
    /// 51 : OpCode.LDARG3
    /// 52 : OpCode.SETITEM
    /// 53 : OpCode.APPEND
    /// 54 : OpCode.LDARG3
    /// 55 : OpCode.LDARG1
    /// 56 : OpCode.LDARG2
    /// 57 : OpCode.LDARG0
    /// 58 : OpCode.PUSH1
    /// 59 : OpCode.SUB
    /// 5A : OpCode.CALL A6
    /// 5C : OpCode.DUP
    /// 5D : OpCode.STLOC1
    /// 5E : OpCode.SIZE
    /// 5F : OpCode.STLOC2
    /// 60 : OpCode.PUSH0
    /// 61 : OpCode.STLOC3
    /// 62 : OpCode.JMP 0C
    /// 64 : OpCode.LDLOC1
    /// 65 : OpCode.LDLOC3
    /// 66 : OpCode.PICKITEM
    /// 67 : OpCode.STLOC4
    /// 68 : OpCode.LDLOC0
    /// 69 : OpCode.LDLOC4
    /// 6A : OpCode.APPEND
    /// 6B : OpCode.LDLOC3
    /// 6C : OpCode.INC
    /// 6D : OpCode.STLOC3
    /// 6E : OpCode.LDLOC3
    /// 6F : OpCode.LDLOC2
    /// 70 : OpCode.JMPLT F4
    /// 72 : OpCode.LDLOC0
    /// 73 : OpCode.RET
    /// </remarks>
    [DisplayName("hanoiTower")]
    public abstract IList<object>? HanoiTower(BigInteger? n, BigInteger? src, BigInteger? aux, BigInteger? dst);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBCXJgQJQHgQtSYHeBGeIgV4EZ80z0A=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIFNOT 04
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.RET
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.LT
    /// 0D : OpCode.JMPIFNOT 07
    /// 0F : OpCode.LDARG0
    /// 10 : OpCode.PUSH1
    /// 11 : OpCode.ADD
    /// 12 : OpCode.JMP 05
    /// 14 : OpCode.LDARG0
    /// 15 : OpCode.PUSH1
    /// 16 : OpCode.SUB
    /// 17 : OpCode.CALL CF
    /// 19 : OpCode.RET
    /// </remarks>
    [DisplayName("odd")]
    public abstract bool? Odd(BigInteger? n);

    #endregion
}
