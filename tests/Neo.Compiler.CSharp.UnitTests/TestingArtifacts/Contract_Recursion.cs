using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Recursion(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Recursion"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""factorial"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""hanoiTower"",""parameters"":[{""name"":""n"",""type"":""Integer""},{""name"":""src"",""type"":""Integer""},{""name"":""aux"",""type"":""Integer""},{""name"":""dst"",""type"":""Integer""}],""returntype"":""Array"",""offset"":50,""safe"":false},{""name"":""even"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":186,""safe"":false},{""name"":""odd"",""parameters"":[{""name"":""n"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":212,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAO5XAAF4ELgMGk1pbnVzIG51bWJlciBub3Qgc3VwcG9ydGVk4XgSuCYKeHgRnzTUoEARQFcFBHgQtwwTQ291bnQgb2YgZGlza3MgPD0gMOF4EZcmIcJwaMVKEM9KEM9KEM9KNFVKEBHQShF50EoSe9DPaEB6e3l4EZ80unBoxUoQz0oQz0oQz0o0L0oQeNBKEXnQShJ70M97eXp4EZ80lkpxynIQcyIMaWvOdGhsz2ucc2tqMPRoQFcAAUBXAAF4EJcmBAhAeBC1Jgd4EZ4iBXgRnzQDQFcAAXgQlyYECUB4ELUmB3gRniIFeBGfNM9AIOeXHg=="));

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
    /// Script: VwUEeBC3DENvdW50IG9mIGRpc2tzIDw9IDDheBGXJiHCcGjFShDPShDPShDPSjRVShAR0EoRedBKEnvQz2hAent5eBGfNLpwaMVKEM9KEM9KEM9KNC9KEHjQShF50EoSe9DPe3l6eBGfNJZKccpyEHMiDGlrznRobM9rnHNrajD0aEA=
    /// 00 : OpCode.INITSLOT 0504
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.GT
    /// 06 : OpCode.PUSHDATA1 436F756E74206F66206469736B73203C3D2030
    /// 1B : OpCode.ASSERTMSG
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.PUSH1
    /// 1E : OpCode.EQUAL
    /// 1F : OpCode.JMPIFNOT 21
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
    /// 2F : OpCode.CALL 55
    /// 31 : OpCode.DUP
    /// 32 : OpCode.PUSH0
    /// 33 : OpCode.PUSH1
    /// 34 : OpCode.SETITEM
    /// 35 : OpCode.DUP
    /// 36 : OpCode.PUSH1
    /// 37 : OpCode.LDARG1
    /// 38 : OpCode.SETITEM
    /// 39 : OpCode.DUP
    /// 3A : OpCode.PUSH2
    /// 3B : OpCode.LDARG3
    /// 3C : OpCode.SETITEM
    /// 3D : OpCode.APPEND
    /// 3E : OpCode.LDLOC0
    /// 3F : OpCode.RET
    /// 40 : OpCode.LDARG2
    /// 41 : OpCode.LDARG3
    /// 42 : OpCode.LDARG1
    /// 43 : OpCode.LDARG0
    /// 44 : OpCode.PUSH1
    /// 45 : OpCode.SUB
    /// 46 : OpCode.CALL BA
    /// 48 : OpCode.STLOC0
    /// 49 : OpCode.LDLOC0
    /// 4A : OpCode.NEWSTRUCT0
    /// 4B : OpCode.DUP
    /// 4C : OpCode.PUSH0
    /// 4D : OpCode.APPEND
    /// 4E : OpCode.DUP
    /// 4F : OpCode.PUSH0
    /// 50 : OpCode.APPEND
    /// 51 : OpCode.DUP
    /// 52 : OpCode.PUSH0
    /// 53 : OpCode.APPEND
    /// 54 : OpCode.DUP
    /// 55 : OpCode.CALL 2F
    /// 57 : OpCode.DUP
    /// 58 : OpCode.PUSH0
    /// 59 : OpCode.LDARG0
    /// 5A : OpCode.SETITEM
    /// 5B : OpCode.DUP
    /// 5C : OpCode.PUSH1
    /// 5D : OpCode.LDARG1
    /// 5E : OpCode.SETITEM
    /// 5F : OpCode.DUP
    /// 60 : OpCode.PUSH2
    /// 61 : OpCode.LDARG3
    /// 62 : OpCode.SETITEM
    /// 63 : OpCode.APPEND
    /// 64 : OpCode.LDARG3
    /// 65 : OpCode.LDARG1
    /// 66 : OpCode.LDARG2
    /// 67 : OpCode.LDARG0
    /// 68 : OpCode.PUSH1
    /// 69 : OpCode.SUB
    /// 6A : OpCode.CALL 96
    /// 6C : OpCode.DUP
    /// 6D : OpCode.STLOC1
    /// 6E : OpCode.SIZE
    /// 6F : OpCode.STLOC2
    /// 70 : OpCode.PUSH0
    /// 71 : OpCode.STLOC3
    /// 72 : OpCode.JMP 0C
    /// 74 : OpCode.LDLOC1
    /// 75 : OpCode.LDLOC3
    /// 76 : OpCode.PICKITEM
    /// 77 : OpCode.STLOC4
    /// 78 : OpCode.LDLOC0
    /// 79 : OpCode.LDLOC4
    /// 7A : OpCode.APPEND
    /// 7B : OpCode.LDLOC3
    /// 7C : OpCode.INC
    /// 7D : OpCode.STLOC3
    /// 7E : OpCode.LDLOC3
    /// 7F : OpCode.LDLOC2
    /// 80 : OpCode.JMPLT F4
    /// 82 : OpCode.LDLOC0
    /// 83 : OpCode.RET
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
