using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Recursion(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Recursion"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""factorial"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""hanoiTower"",""parameters"":[{""name"":""n"",""type"":""Integer""},{""name"":""src"",""type"":""Integer""},{""name"":""aux"",""type"":""Integer""},{""name"":""dst"",""type"":""Integer""}],""returntype"":""Array"",""offset"":50,""safe"":false},{""name"":""even"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":142,""safe"":false},{""name"":""odd"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":168,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMJXAAF4ELgMGk1pbnVzIG51bWJlciBub3Qgc3VwcG9ydGVk4XgSuCYKeHgRnzTUoEARQFcFBHgQtwwTQ291bnQgb2YgZGlza3MgPD0gMOF4EZcmDcJwaHt5ERO/z2hAent5eBGfNM5waHt5eBO/z3t5engRnzS+SnHKchBzIgxpa850aGzPa5xza2ow9GhAVwABeBCXJgQIQHgQtSYHeBGeIgV4EZ80A0BXAAF4EJcmBAlAeBC1Jgd4EZ4iBXgRnzTPQCNkHR8="));

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
    /// Script: VwUEeBC3DENvdW50IG9mIGRpc2tzIDw9IDDheBGXJg3CcGh7eRETv89oQHp7eXgRnzTOcGh7eXgTv897eXp4EZ80vkpxynIQcyIMaWvOdGhsz2ucc2tqMPRoQA==
    /// 00 : OpCode.INITSLOT 0504
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.GT
    /// 06 : OpCode.PUSHDATA1 436F756E74206F66206469736B73203C3D2030
    /// 1B : OpCode.ASSERTMSG
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.PUSH1
    /// 1E : OpCode.EQUAL
    /// 1F : OpCode.JMPIFNOT 0D
    /// 21 : OpCode.NEWARRAY0
    /// 22 : OpCode.STLOC0
    /// 23 : OpCode.LDLOC0
    /// 24 : OpCode.LDARG3
    /// 25 : OpCode.LDARG1
    /// 26 : OpCode.PUSH1
    /// 27 : OpCode.PUSH3
    /// 28 : OpCode.PACKSTRUCT
    /// 29 : OpCode.APPEND
    /// 2A : OpCode.LDLOC0
    /// 2B : OpCode.RET
    /// 2C : OpCode.LDARG2
    /// 2D : OpCode.LDARG3
    /// 2E : OpCode.LDARG1
    /// 2F : OpCode.LDARG0
    /// 30 : OpCode.PUSH1
    /// 31 : OpCode.SUB
    /// 32 : OpCode.CALL CE
    /// 34 : OpCode.STLOC0
    /// 35 : OpCode.LDLOC0
    /// 36 : OpCode.LDARG3
    /// 37 : OpCode.LDARG1
    /// 38 : OpCode.LDARG0
    /// 39 : OpCode.PUSH3
    /// 3A : OpCode.PACKSTRUCT
    /// 3B : OpCode.APPEND
    /// 3C : OpCode.LDARG3
    /// 3D : OpCode.LDARG1
    /// 3E : OpCode.LDARG2
    /// 3F : OpCode.LDARG0
    /// 40 : OpCode.PUSH1
    /// 41 : OpCode.SUB
    /// 42 : OpCode.CALL BE
    /// 44 : OpCode.DUP
    /// 45 : OpCode.STLOC1
    /// 46 : OpCode.SIZE
    /// 47 : OpCode.STLOC2
    /// 48 : OpCode.PUSH0
    /// 49 : OpCode.STLOC3
    /// 4A : OpCode.JMP 0C
    /// 4C : OpCode.LDLOC1
    /// 4D : OpCode.LDLOC3
    /// 4E : OpCode.PICKITEM
    /// 4F : OpCode.STLOC4
    /// 50 : OpCode.LDLOC0
    /// 51 : OpCode.LDLOC4
    /// 52 : OpCode.APPEND
    /// 53 : OpCode.LDLOC3
    /// 54 : OpCode.INC
    /// 55 : OpCode.STLOC3
    /// 56 : OpCode.LDLOC3
    /// 57 : OpCode.LDLOC2
    /// 58 : OpCode.JMPLT F4
    /// 5A : OpCode.LDLOC0
    /// 5B : OpCode.RET
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
