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
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIFNOT 04
    /// 0008 : OpCode.PUSHT
    /// 0009 : OpCode.RET
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.LT
    /// 000D : OpCode.JMPIFNOT 07
    /// 000F : OpCode.LDARG0
    /// 0010 : OpCode.PUSH1
    /// 0011 : OpCode.ADD
    /// 0012 : OpCode.JMP 05
    /// 0014 : OpCode.LDARG0
    /// 0015 : OpCode.PUSH1
    /// 0016 : OpCode.SUB
    /// 0017 : OpCode.CALL 03
    /// 0019 : OpCode.RET
    /// </remarks>
    [DisplayName("even")]
    public abstract bool? Even(BigInteger? n);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.GE
    /// 0006 : OpCode.PUSHDATA1 4D696E7573206E756D626572206E6F7420737570706F72746564
    /// 0022 : OpCode.ASSERTMSG
    /// 0023 : OpCode.LDARG0
    /// 0024 : OpCode.PUSH2
    /// 0025 : OpCode.GE
    /// 0026 : OpCode.JMPIFNOT 0A
    /// 0028 : OpCode.LDARG0
    /// 0029 : OpCode.LDARG0
    /// 002A : OpCode.PUSH1
    /// 002B : OpCode.SUB
    /// 002C : OpCode.CALL D4
    /// 002E : OpCode.MUL
    /// 002F : OpCode.RET
    /// 0030 : OpCode.PUSH1
    /// 0031 : OpCode.RET
    /// </remarks>
    [DisplayName("factorial")]
    public abstract BigInteger? Factorial(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0504
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.GT
    /// 0006 : OpCode.PUSHDATA1 436F756E74206F66206469736B73203C3D2030
    /// 001B : OpCode.ASSERTMSG
    /// 001C : OpCode.LDARG0
    /// 001D : OpCode.PUSH1
    /// 001E : OpCode.EQUAL
    /// 001F : OpCode.JMPIFNOT 21
    /// 0021 : OpCode.NEWARRAY0
    /// 0022 : OpCode.STLOC0
    /// 0023 : OpCode.LDLOC0
    /// 0024 : OpCode.NEWSTRUCT0
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.PUSH0
    /// 0027 : OpCode.APPEND
    /// 0028 : OpCode.DUP
    /// 0029 : OpCode.PUSH0
    /// 002A : OpCode.APPEND
    /// 002B : OpCode.DUP
    /// 002C : OpCode.PUSH0
    /// 002D : OpCode.APPEND
    /// 002E : OpCode.DUP
    /// 002F : OpCode.CALL 55
    /// 0031 : OpCode.DUP
    /// 0032 : OpCode.PUSH0
    /// 0033 : OpCode.PUSH1
    /// 0034 : OpCode.SETITEM
    /// 0035 : OpCode.DUP
    /// 0036 : OpCode.PUSH1
    /// 0037 : OpCode.LDARG1
    /// 0038 : OpCode.SETITEM
    /// 0039 : OpCode.DUP
    /// 003A : OpCode.PUSH2
    /// 003B : OpCode.LDARG3
    /// 003C : OpCode.SETITEM
    /// 003D : OpCode.APPEND
    /// 003E : OpCode.LDLOC0
    /// 003F : OpCode.RET
    /// 0040 : OpCode.LDARG2
    /// 0041 : OpCode.LDARG3
    /// 0042 : OpCode.LDARG1
    /// 0043 : OpCode.LDARG0
    /// 0044 : OpCode.PUSH1
    /// 0045 : OpCode.SUB
    /// 0046 : OpCode.CALL BA
    /// 0048 : OpCode.STLOC0
    /// 0049 : OpCode.LDLOC0
    /// 004A : OpCode.NEWSTRUCT0
    /// 004B : OpCode.DUP
    /// 004C : OpCode.PUSH0
    /// 004D : OpCode.APPEND
    /// 004E : OpCode.DUP
    /// 004F : OpCode.PUSH0
    /// 0050 : OpCode.APPEND
    /// 0051 : OpCode.DUP
    /// 0052 : OpCode.PUSH0
    /// 0053 : OpCode.APPEND
    /// 0054 : OpCode.DUP
    /// 0055 : OpCode.CALL 2F
    /// 0057 : OpCode.DUP
    /// 0058 : OpCode.PUSH0
    /// 0059 : OpCode.LDARG0
    /// 005A : OpCode.SETITEM
    /// 005B : OpCode.DUP
    /// 005C : OpCode.PUSH1
    /// 005D : OpCode.LDARG1
    /// 005E : OpCode.SETITEM
    /// 005F : OpCode.DUP
    /// 0060 : OpCode.PUSH2
    /// 0061 : OpCode.LDARG3
    /// 0062 : OpCode.SETITEM
    /// 0063 : OpCode.APPEND
    /// 0064 : OpCode.LDARG3
    /// 0065 : OpCode.LDARG1
    /// 0066 : OpCode.LDARG2
    /// 0067 : OpCode.LDARG0
    /// 0068 : OpCode.PUSH1
    /// 0069 : OpCode.SUB
    /// 006A : OpCode.CALL 96
    /// 006C : OpCode.DUP
    /// 006D : OpCode.STLOC1
    /// 006E : OpCode.SIZE
    /// 006F : OpCode.STLOC2
    /// 0070 : OpCode.PUSH0
    /// 0071 : OpCode.STLOC3
    /// 0072 : OpCode.JMP 0C
    /// 0074 : OpCode.LDLOC1
    /// 0075 : OpCode.LDLOC3
    /// 0076 : OpCode.PICKITEM
    /// 0077 : OpCode.STLOC4
    /// 0078 : OpCode.LDLOC0
    /// 0079 : OpCode.LDLOC4
    /// 007A : OpCode.APPEND
    /// 007B : OpCode.LDLOC3
    /// 007C : OpCode.INC
    /// 007D : OpCode.STLOC3
    /// 007E : OpCode.LDLOC3
    /// 007F : OpCode.LDLOC2
    /// 0080 : OpCode.JMPLT F4
    /// 0082 : OpCode.LDLOC0
    /// 0083 : OpCode.RET
    /// </remarks>
    [DisplayName("hanoiTower")]
    public abstract IList<object>? HanoiTower(BigInteger? n, BigInteger? src, BigInteger? aux, BigInteger? dst);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIFNOT 04
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.RET
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.LT
    /// 000D : OpCode.JMPIFNOT 07
    /// 000F : OpCode.LDARG0
    /// 0010 : OpCode.PUSH1
    /// 0011 : OpCode.ADD
    /// 0012 : OpCode.JMP 05
    /// 0014 : OpCode.LDARG0
    /// 0015 : OpCode.PUSH1
    /// 0016 : OpCode.SUB
    /// 0017 : OpCode.CALL CF
    /// 0019 : OpCode.RET
    /// </remarks>
    [DisplayName("odd")]
    public abstract bool? Odd(BigInteger? n);

    #endregion

}
