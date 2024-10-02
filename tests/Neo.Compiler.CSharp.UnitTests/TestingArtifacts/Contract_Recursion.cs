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
    [DisplayName("even")]
    public abstract bool? Even(BigInteger? n);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : EQUAL
    // 0006 : JMPIFNOT
    // 0008 : PUSHT
    // 0009 : RET
    // 000A : LDARG0
    // 000B : PUSH0
    // 000C : LT
    // 000D : JMPIFNOT
    // 000F : LDARG0
    // 0010 : PUSH1
    // 0011 : ADD
    // 0012 : JMP
    // 0014 : LDARG0
    // 0015 : PUSH1
    // 0016 : SUB
    // 0017 : CALL
    // 0019 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("factorial")]
    public abstract BigInteger? Factorial(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : GE
    // 0006 : PUSHDATA1
    // 0022 : ASSERTMSG
    // 0023 : LDARG0
    // 0024 : PUSH2
    // 0025 : GE
    // 0026 : JMPIFNOT
    // 0028 : LDARG0
    // 0029 : LDARG0
    // 002A : PUSH1
    // 002B : SUB
    // 002C : CALL
    // 002E : MUL
    // 002F : RET
    // 0030 : PUSH1
    // 0031 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("hanoiTower")]
    public abstract IList<object>? HanoiTower(BigInteger? n, BigInteger? src, BigInteger? aux, BigInteger? dst);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : GT
    // 0006 : PUSHDATA1
    // 001B : ASSERTMSG
    // 001C : LDARG0
    // 001D : PUSH1
    // 001E : EQUAL
    // 001F : JMPIFNOT
    // 0021 : NEWARRAY0
    // 0022 : STLOC0
    // 0023 : LDLOC0
    // 0024 : NEWSTRUCT0
    // 0025 : DUP
    // 0026 : PUSH0
    // 0027 : APPEND
    // 0028 : DUP
    // 0029 : PUSH0
    // 002A : APPEND
    // 002B : DUP
    // 002C : PUSH0
    // 002D : APPEND
    // 002E : DUP
    // 002F : CALL
    // 0031 : DUP
    // 0032 : PUSH0
    // 0033 : PUSH1
    // 0034 : SETITEM
    // 0035 : DUP
    // 0036 : PUSH1
    // 0037 : LDARG1
    // 0038 : SETITEM
    // 0039 : DUP
    // 003A : PUSH2
    // 003B : LDARG3
    // 003C : SETITEM
    // 003D : APPEND
    // 003E : LDLOC0
    // 003F : RET
    // 0040 : LDARG2
    // 0041 : LDARG3
    // 0042 : LDARG1
    // 0043 : LDARG0
    // 0044 : PUSH1
    // 0045 : SUB
    // 0046 : CALL
    // 0048 : STLOC0
    // 0049 : LDLOC0
    // 004A : NEWSTRUCT0
    // 004B : DUP
    // 004C : PUSH0
    // 004D : APPEND
    // 004E : DUP
    // 004F : PUSH0
    // 0050 : APPEND
    // 0051 : DUP
    // 0052 : PUSH0
    // 0053 : APPEND
    // 0054 : DUP
    // 0055 : CALL
    // 0057 : DUP
    // 0058 : PUSH0
    // 0059 : LDARG0
    // 005A : SETITEM
    // 005B : DUP
    // 005C : PUSH1
    // 005D : LDARG1
    // 005E : SETITEM
    // 005F : DUP
    // 0060 : PUSH2
    // 0061 : LDARG3
    // 0062 : SETITEM
    // 0063 : APPEND
    // 0064 : LDARG3
    // 0065 : LDARG1
    // 0066 : LDARG2
    // 0067 : LDARG0
    // 0068 : PUSH1
    // 0069 : SUB
    // 006A : CALL
    // 006C : DUP
    // 006D : STLOC1
    // 006E : SIZE
    // 006F : STLOC2
    // 0070 : PUSH0
    // 0071 : STLOC3
    // 0072 : JMP
    // 0074 : LDLOC1
    // 0075 : LDLOC3
    // 0076 : PICKITEM
    // 0077 : STLOC4
    // 0078 : LDLOC0
    // 0079 : LDLOC4
    // 007A : APPEND
    // 007B : LDLOC3
    // 007C : INC
    // 007D : STLOC3
    // 007E : LDLOC3
    // 007F : LDLOC2
    // 0080 : JMPLT
    // 0082 : LDLOC0
    // 0083 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("odd")]
    public abstract bool? Odd(BigInteger? n);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : EQUAL
    // 0006 : JMPIFNOT
    // 0008 : PUSHF
    // 0009 : RET
    // 000A : LDARG0
    // 000B : PUSH0
    // 000C : LT
    // 000D : JMPIFNOT
    // 000F : LDARG0
    // 0010 : PUSH1
    // 0011 : ADD
    // 0012 : JMP
    // 0014 : LDARG0
    // 0015 : PUSH1
    // 0016 : SUB
    // 0017 : CALL
    // 0019 : RET

    #endregion

}
