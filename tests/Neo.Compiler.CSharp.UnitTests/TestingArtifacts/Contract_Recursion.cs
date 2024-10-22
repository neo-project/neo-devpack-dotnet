using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Recursion(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Recursion"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""factorial"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""hanoiTower"",""parameters"":[{""name"":""n"",""type"":""Integer""},{""name"":""src"",""type"":""Integer""},{""name"":""aux"",""type"":""Integer""},{""name"":""dst"",""type"":""Integer""}],""returntype"":""Array"",""offset"":50,""safe"":false},{""name"":""even"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":176,""safe"":false},{""name"":""odd"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":202,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAORXAAF4ELgMGk1pbnVzIG51bWJlciBub3Qgc3VwcG9ydGVk4XgSuCYKeHgRnzTUoEARQFcFBHgQtwwTQ291bnQgb2YgZGlza3MgPD0gMOF4EZcmHsJwaMVKEM9KEM9KEM9KEBHQShF50EoSe9DPaEB6e3l4EZ80vXBoxUoQz0oQz0oQz0oQeNBKEXnQShJ70M97eXp4EZ80nEpxynIQcyIMaWvOdGhsz2ucc2tqMPRoQFcAAXgQlyYECEB4ELUmB3gRniIFeBGfNANAVwABeBCXJgQJQHgQtSYHeBGeIgV4EZ80z0DyeJfO"));

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
    /// Script: VwUEeBC3DENvdW50IG9mIGRpc2tzIDw9IDDheBGXJh7CcGjFShDPShDPShDPShAR0EoRedBKEnvQz2hAent5eBGfNL1waMVKEM9KEM9KEM9KEHjQShF50EoSe9DPe3l6eBGfNJxKccpyEHMiDGlrznRobM9rnHNrajD0aEA=
    /// 00 : OpCode.INITSLOT 0504
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.GT
    /// 06 : OpCode.PUSHDATA1 436F756E74206F66206469736B73203C3D2030
    /// 1B : OpCode.ASSERTMSG
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.PUSH1
    /// 1E : OpCode.EQUAL
    /// 1F : OpCode.JMPIFNOT 1E
    /// 21 : OpCode.NEWARRAY0
    /// 22 : OpCode.STLOC0
    /// 23 : OpCode.LDLOC0
    /// 24 : OpCode.NEWSTRUCT0
    /// 25 : OpCode.DUP
    /// 26 : OpCode.PUSH0
    /// 27 : OpCode.APPEND
    /// 28 : OpCode.DUP
    /// 29 : OpCode.PUSH0
    /// 2A : OpCode.APPEND
    /// 2B : OpCode.DUP
    /// 2C : OpCode.PUSH0
    /// 2D : OpCode.APPEND
    /// 2E : OpCode.DUP
    /// 2F : OpCode.PUSH0
    /// 30 : OpCode.PUSH1
    /// 31 : OpCode.SETITEM
    /// 32 : OpCode.DUP
    /// 33 : OpCode.PUSH1
    /// 34 : OpCode.LDARG1
    /// 35 : OpCode.SETITEM
    /// 36 : OpCode.DUP
    /// 37 : OpCode.PUSH2
    /// 38 : OpCode.LDARG3
    /// 39 : OpCode.SETITEM
    /// 3A : OpCode.APPEND
    /// 3B : OpCode.LDLOC0
    /// 3C : OpCode.RET
    /// 3D : OpCode.LDARG2
    /// 3E : OpCode.LDARG3
    /// 3F : OpCode.LDARG1
    /// 40 : OpCode.LDARG0
    /// 41 : OpCode.PUSH1
    /// 42 : OpCode.SUB
    /// 43 : OpCode.CALL BD
    /// 45 : OpCode.STLOC0
    /// 46 : OpCode.LDLOC0
    /// 47 : OpCode.NEWSTRUCT0
    /// 48 : OpCode.DUP
    /// 49 : OpCode.PUSH0
    /// 4A : OpCode.APPEND
    /// 4B : OpCode.DUP
    /// 4C : OpCode.PUSH0
    /// 4D : OpCode.APPEND
    /// 4E : OpCode.DUP
    /// 4F : OpCode.PUSH0
    /// 50 : OpCode.APPEND
    /// 51 : OpCode.DUP
    /// 52 : OpCode.PUSH0
    /// 53 : OpCode.LDARG0
    /// 54 : OpCode.SETITEM
    /// 55 : OpCode.DUP
    /// 56 : OpCode.PUSH1
    /// 57 : OpCode.LDARG1
    /// 58 : OpCode.SETITEM
    /// 59 : OpCode.DUP
    /// 5A : OpCode.PUSH2
    /// 5B : OpCode.LDARG3
    /// 5C : OpCode.SETITEM
    /// 5D : OpCode.APPEND
    /// 5E : OpCode.LDARG3
    /// 5F : OpCode.LDARG1
    /// 60 : OpCode.LDARG2
    /// 61 : OpCode.LDARG0
    /// 62 : OpCode.PUSH1
    /// 63 : OpCode.SUB
    /// 64 : OpCode.CALL 9C
    /// 66 : OpCode.DUP
    /// 67 : OpCode.STLOC1
    /// 68 : OpCode.SIZE
    /// 69 : OpCode.STLOC2
    /// 6A : OpCode.PUSH0
    /// 6B : OpCode.STLOC3
    /// 6C : OpCode.JMP 0C
    /// 6E : OpCode.LDLOC1
    /// 6F : OpCode.LDLOC3
    /// 70 : OpCode.PICKITEM
    /// 71 : OpCode.STLOC4
    /// 72 : OpCode.LDLOC0
    /// 73 : OpCode.LDLOC4
    /// 74 : OpCode.APPEND
    /// 75 : OpCode.LDLOC3
    /// 76 : OpCode.INC
    /// 77 : OpCode.STLOC3
    /// 78 : OpCode.LDLOC3
    /// 79 : OpCode.LDLOC2
    /// 7A : OpCode.JMPLT F4
    /// 7C : OpCode.LDLOC0
    /// 7D : OpCode.RET
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
