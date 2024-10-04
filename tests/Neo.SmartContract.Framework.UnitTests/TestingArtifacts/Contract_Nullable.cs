using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Nullable(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Nullable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""bigIntegerNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""bigIntegerNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":56,""safe"":false},{""name"":""bigIntegerNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":114,""safe"":false},{""name"":""h160NullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Hash160""},{""name"":""b"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":129,""safe"":false},{""name"":""h160NullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":161,""safe"":false},{""name"":""h256NullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Hash256""},{""name"":""b"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":168,""safe"":false},{""name"":""h256NullableEqual"",""parameters"":[{""name"":""a"",""type"":""Hash256""},{""name"":""b"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":200,""safe"":false},{""name"":""byteNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":230,""safe"":false},{""name"":""byteNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":286,""safe"":false},{""name"":""byteNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":344,""safe"":false},{""name"":""sByteNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":359,""safe"":false},{""name"":""sByteNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":415,""safe"":false},{""name"":""sByteNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":473,""safe"":false},{""name"":""shortNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":488,""safe"":false},{""name"":""shortNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":544,""safe"":false},{""name"":""shortNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":602,""safe"":false},{""name"":""uShortNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":617,""safe"":false},{""name"":""uShortNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":673,""safe"":false},{""name"":""uShortNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":731,""safe"":false},{""name"":""intNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":746,""safe"":false},{""name"":""intNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":802,""safe"":false},{""name"":""intNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":860,""safe"":false},{""name"":""uIntNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":875,""safe"":false},{""name"":""uIntNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":931,""safe"":false},{""name"":""uIntNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":989,""safe"":false},{""name"":""longNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1004,""safe"":false},{""name"":""longNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1060,""safe"":false},{""name"":""longNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1118,""safe"":false},{""name"":""uLongNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1133,""safe"":false},{""name"":""uLongNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1189,""safe"":false},{""name"":""uLongNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1247,""safe"":false},{""name"":""boolNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1262,""safe"":false},{""name"":""boolNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1316,""safe"":false},{""name"":""boolNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1372,""safe"":false},{""name"":""byteNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1387,""safe"":false},{""name"":""sByteNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1410,""safe"":false},{""name"":""shortNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1433,""safe"":false},{""name"":""uShortNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1456,""safe"":false},{""name"":""intNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1479,""safe"":false},{""name"":""uIntNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1502,""safe"":false},{""name"":""longNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1525,""safe"":false},{""name"":""uLongNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1548,""safe"":false},{""name"":""boolNullableToString"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1571,""safe"":false},{""name"":""bigIntegerNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1611,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0D
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.JMPIF 05
    /// 0018 : OpCode.PUSHF
    /// 0019 : OpCode.JMP 17
    /// 001B : OpCode.LDARG1
    /// 001C : OpCode.LDARG0
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ISNULL
    /// 001F : OpCode.JMPIF 0A
    /// 0021 : OpCode.SWAP
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.ISNULL
    /// 0024 : OpCode.JMPIF 09
    /// 0026 : OpCode.NUMEQUAL
    /// 0027 : OpCode.JMP 09
    /// 0029 : OpCode.DROP
    /// 002A : OpCode.ISNULL
    /// 002B : OpCode.JMP 05
    /// 002D : OpCode.DROP
    /// 002E : OpCode.DROP
    /// 002F : OpCode.PUSHF
    /// 0030 : OpCode.JMPIF 04
    /// 0032 : OpCode.PUSHF
    /// 0033 : OpCode.RET
    /// 0034 : OpCode.LDARG1
    /// 0035 : OpCode.LDARG0
    /// 0036 : OpCode.EQUAL
    /// 0037 : OpCode.RET
    /// </remarks>
    [DisplayName("bigIntegerNullableEqual")]
    public abstract bool? BigIntegerNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHNULL
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 04
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.RET
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.ISNULL
    /// 000C : OpCode.NOT
    /// 000D : OpCode.NOT
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("bigIntegerNullableEqualNull")]
    public abstract bool? BigIntegerNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0E
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.NOT
    /// 0017 : OpCode.JMPIF 05
    /// 0019 : OpCode.PUSHF
    /// 001A : OpCode.JMP 18
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.DUP
    /// 001F : OpCode.ISNULL
    /// 0020 : OpCode.JMPIF 0A
    /// 0022 : OpCode.SWAP
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.ISNULL
    /// 0025 : OpCode.JMPIF 09
    /// 0027 : OpCode.NUMEQUAL
    /// 0028 : OpCode.JMP 09
    /// 002A : OpCode.DROP
    /// 002B : OpCode.ISNULL
    /// 002C : OpCode.JMP 05
    /// 002E : OpCode.DROP
    /// 002F : OpCode.DROP
    /// 0030 : OpCode.PUSHF
    /// 0031 : OpCode.NOT
    /// 0032 : OpCode.JMPIF 04
    /// 0034 : OpCode.PUSHF
    /// 0035 : OpCode.RET
    /// 0036 : OpCode.LDARG1
    /// 0037 : OpCode.LDARG0
    /// 0038 : OpCode.NOTEQUAL
    /// 0039 : OpCode.RET
    /// </remarks>
    [DisplayName("bigIntegerNullableNotEqual")]
    public abstract bool? BigIntegerNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.CALLT 0000
    /// 000B : OpCode.DUP
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.JMPIFNOT 05
    /// 000F : OpCode.DROP
    /// 0010 : OpCode.PUSHDATA1
    /// 0012 : OpCode.PUSHDATA1 31
    /// 0015 : OpCode.EQUAL
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("bigIntegerNullableToString")]
    public abstract bool? BigIntegerNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0D
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.EQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.JMPIF 05
    /// 0018 : OpCode.PUSHF
    /// 0019 : OpCode.JMP 15
    /// 001B : OpCode.LDARG1
    /// 001C : OpCode.LDARG0
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ISNULL
    /// 001F : OpCode.JMPIF 09
    /// 0021 : OpCode.DUP
    /// 0022 : OpCode.ISNULL
    /// 0023 : OpCode.JMPIF 09
    /// 0025 : OpCode.EQUAL
    /// 0026 : OpCode.JMP 08
    /// 0028 : OpCode.DROP
    /// 0029 : OpCode.ISNULL
    /// 002A : OpCode.JMP 04
    /// 002C : OpCode.DROP
    /// 002D : OpCode.PUSHF
    /// 002E : OpCode.JMPIF 04
    /// 0030 : OpCode.PUSHF
    /// 0031 : OpCode.RET
    /// 0032 : OpCode.LDARG1
    /// 0033 : OpCode.LDARG0
    /// 0034 : OpCode.EQUAL
    /// 0035 : OpCode.RET
    /// </remarks>
    [DisplayName("boolNullableEqual")]
    public abstract bool? BoolNullableEqual(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHNULL
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 04
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.RET
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.ISNULL
    /// 000C : OpCode.NOT
    /// 000D : OpCode.NOT
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("boolNullableEqualNull")]
    public abstract bool? BoolNullableEqualNull(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0E
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.EQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.NOT
    /// 0017 : OpCode.JMPIF 05
    /// 0019 : OpCode.PUSHF
    /// 001A : OpCode.JMP 16
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.DUP
    /// 001F : OpCode.ISNULL
    /// 0020 : OpCode.JMPIF 09
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.ISNULL
    /// 0024 : OpCode.JMPIF 09
    /// 0026 : OpCode.EQUAL
    /// 0027 : OpCode.JMP 08
    /// 0029 : OpCode.DROP
    /// 002A : OpCode.ISNULL
    /// 002B : OpCode.JMP 04
    /// 002D : OpCode.DROP
    /// 002E : OpCode.PUSHF
    /// 002F : OpCode.NOT
    /// 0030 : OpCode.JMPIF 04
    /// 0032 : OpCode.PUSHF
    /// 0033 : OpCode.RET
    /// 0034 : OpCode.LDARG1
    /// 0035 : OpCode.LDARG0
    /// 0036 : OpCode.NOTEQUAL
    /// 0037 : OpCode.RET
    /// </remarks>
    [DisplayName("boolNullableNotEqual")]
    public abstract bool? BoolNullableNotEqual(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 13
    /// 0008 : OpCode.JMPIF 0B
    /// 000A : OpCode.PUSHDATA1 46616C7365
    /// 0011 : OpCode.JMP 08
    /// 0013 : OpCode.PUSHDATA1 54727565
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.ISNULL
    /// 001B : OpCode.JMPIFNOT 05
    /// 001D : OpCode.DROP
    /// 001E : OpCode.PUSHDATA1
    /// 0020 : OpCode.PUSHDATA1 54727565
    /// 0026 : OpCode.EQUAL
    /// 0027 : OpCode.RET
    /// </remarks>
    [DisplayName("boolNullableToString")]
    public abstract bool? BoolNullableToString(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0D
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.JMPIF 05
    /// 0018 : OpCode.PUSHF
    /// 0019 : OpCode.JMP 17
    /// 001B : OpCode.LDARG1
    /// 001C : OpCode.LDARG0
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ISNULL
    /// 001F : OpCode.JMPIF 0A
    /// 0021 : OpCode.SWAP
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.ISNULL
    /// 0024 : OpCode.JMPIF 09
    /// 0026 : OpCode.NUMEQUAL
    /// 0027 : OpCode.JMP 09
    /// 0029 : OpCode.DROP
    /// 002A : OpCode.ISNULL
    /// 002B : OpCode.JMP 05
    /// 002D : OpCode.DROP
    /// 002E : OpCode.DROP
    /// 002F : OpCode.PUSHF
    /// 0030 : OpCode.JMPIF 04
    /// 0032 : OpCode.PUSHF
    /// 0033 : OpCode.RET
    /// 0034 : OpCode.LDARG1
    /// 0035 : OpCode.LDARG0
    /// 0036 : OpCode.EQUAL
    /// 0037 : OpCode.RET
    /// </remarks>
    [DisplayName("byteNullableEqual")]
    public abstract bool? ByteNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHNULL
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 04
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.RET
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.ISNULL
    /// 000C : OpCode.NOT
    /// 000D : OpCode.NOT
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("byteNullableEqualNull")]
    public abstract bool? ByteNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0E
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.NOT
    /// 0017 : OpCode.JMPIF 05
    /// 0019 : OpCode.PUSHF
    /// 001A : OpCode.JMP 18
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.DUP
    /// 001F : OpCode.ISNULL
    /// 0020 : OpCode.JMPIF 0A
    /// 0022 : OpCode.SWAP
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.ISNULL
    /// 0025 : OpCode.JMPIF 09
    /// 0027 : OpCode.NUMEQUAL
    /// 0028 : OpCode.JMP 09
    /// 002A : OpCode.DROP
    /// 002B : OpCode.ISNULL
    /// 002C : OpCode.JMP 05
    /// 002E : OpCode.DROP
    /// 002F : OpCode.DROP
    /// 0030 : OpCode.PUSHF
    /// 0031 : OpCode.NOT
    /// 0032 : OpCode.JMPIF 04
    /// 0034 : OpCode.PUSHF
    /// 0035 : OpCode.RET
    /// 0036 : OpCode.LDARG1
    /// 0037 : OpCode.LDARG0
    /// 0038 : OpCode.NOTEQUAL
    /// 0039 : OpCode.RET
    /// </remarks>
    [DisplayName("byteNullableNotEqual")]
    public abstract bool? ByteNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.CALLT 0000
    /// 000B : OpCode.DUP
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.JMPIFNOT 05
    /// 000F : OpCode.DROP
    /// 0010 : OpCode.PUSHDATA1
    /// 0012 : OpCode.PUSHDATA1 31
    /// 0015 : OpCode.EQUAL
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("byteNullableToString")]
    public abstract bool? ByteNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHNULL
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("h160NullableEqualNull")]
    public abstract bool? H160NullableEqualNull(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 06
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.EQUAL
    /// 000E : OpCode.NOT
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.PUSHF
    /// 0012 : OpCode.JMP 06
    /// 0014 : OpCode.LDARG1
    /// 0015 : OpCode.LDARG0
    /// 0016 : OpCode.EQUAL
    /// 0017 : OpCode.NOT
    /// 0018 : OpCode.JMPIF 04
    /// 001A : OpCode.PUSHF
    /// 001B : OpCode.RET
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.NOTEQUAL
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("h160NullableNotEqual")]
    public abstract bool? H160NullableNotEqual(UInt160? a, UInt160? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.EQUAL
    /// 000E : OpCode.JMPIF 05
    /// 0010 : OpCode.PUSHF
    /// 0011 : OpCode.JMP 05
    /// 0013 : OpCode.LDARG1
    /// 0014 : OpCode.LDARG0
    /// 0015 : OpCode.EQUAL
    /// 0016 : OpCode.JMPIF 04
    /// 0018 : OpCode.PUSHF
    /// 0019 : OpCode.RET
    /// 001A : OpCode.LDARG1
    /// 001B : OpCode.LDARG0
    /// 001C : OpCode.EQUAL
    /// 001D : OpCode.RET
    /// </remarks>
    [DisplayName("h256NullableEqual")]
    public abstract bool? H256NullableEqual(UInt256? a, UInt256? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 06
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.EQUAL
    /// 000E : OpCode.NOT
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.PUSHF
    /// 0012 : OpCode.JMP 06
    /// 0014 : OpCode.LDARG1
    /// 0015 : OpCode.LDARG0
    /// 0016 : OpCode.EQUAL
    /// 0017 : OpCode.NOT
    /// 0018 : OpCode.JMPIF 04
    /// 001A : OpCode.PUSHF
    /// 001B : OpCode.RET
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.NOTEQUAL
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("h256NullableNotEqual")]
    public abstract bool? H256NullableNotEqual(UInt256? a, UInt256? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0D
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.JMPIF 05
    /// 0018 : OpCode.PUSHF
    /// 0019 : OpCode.JMP 17
    /// 001B : OpCode.LDARG1
    /// 001C : OpCode.LDARG0
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ISNULL
    /// 001F : OpCode.JMPIF 0A
    /// 0021 : OpCode.SWAP
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.ISNULL
    /// 0024 : OpCode.JMPIF 09
    /// 0026 : OpCode.NUMEQUAL
    /// 0027 : OpCode.JMP 09
    /// 0029 : OpCode.DROP
    /// 002A : OpCode.ISNULL
    /// 002B : OpCode.JMP 05
    /// 002D : OpCode.DROP
    /// 002E : OpCode.DROP
    /// 002F : OpCode.PUSHF
    /// 0030 : OpCode.JMPIF 04
    /// 0032 : OpCode.PUSHF
    /// 0033 : OpCode.RET
    /// 0034 : OpCode.LDARG1
    /// 0035 : OpCode.LDARG0
    /// 0036 : OpCode.EQUAL
    /// 0037 : OpCode.RET
    /// </remarks>
    [DisplayName("intNullableEqual")]
    public abstract bool? IntNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHNULL
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 04
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.RET
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.ISNULL
    /// 000C : OpCode.NOT
    /// 000D : OpCode.NOT
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("intNullableEqualNull")]
    public abstract bool? IntNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0E
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.NOT
    /// 0017 : OpCode.JMPIF 05
    /// 0019 : OpCode.PUSHF
    /// 001A : OpCode.JMP 18
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.DUP
    /// 001F : OpCode.ISNULL
    /// 0020 : OpCode.JMPIF 0A
    /// 0022 : OpCode.SWAP
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.ISNULL
    /// 0025 : OpCode.JMPIF 09
    /// 0027 : OpCode.NUMEQUAL
    /// 0028 : OpCode.JMP 09
    /// 002A : OpCode.DROP
    /// 002B : OpCode.ISNULL
    /// 002C : OpCode.JMP 05
    /// 002E : OpCode.DROP
    /// 002F : OpCode.DROP
    /// 0030 : OpCode.PUSHF
    /// 0031 : OpCode.NOT
    /// 0032 : OpCode.JMPIF 04
    /// 0034 : OpCode.PUSHF
    /// 0035 : OpCode.RET
    /// 0036 : OpCode.LDARG1
    /// 0037 : OpCode.LDARG0
    /// 0038 : OpCode.NOTEQUAL
    /// 0039 : OpCode.RET
    /// </remarks>
    [DisplayName("intNullableNotEqual")]
    public abstract bool? IntNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.CALLT 0000
    /// 000B : OpCode.DUP
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.JMPIFNOT 05
    /// 000F : OpCode.DROP
    /// 0010 : OpCode.PUSHDATA1
    /// 0012 : OpCode.PUSHDATA1 31
    /// 0015 : OpCode.EQUAL
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("intNullableToString")]
    public abstract bool? IntNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0D
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.JMPIF 05
    /// 0018 : OpCode.PUSHF
    /// 0019 : OpCode.JMP 17
    /// 001B : OpCode.LDARG1
    /// 001C : OpCode.LDARG0
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ISNULL
    /// 001F : OpCode.JMPIF 0A
    /// 0021 : OpCode.SWAP
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.ISNULL
    /// 0024 : OpCode.JMPIF 09
    /// 0026 : OpCode.NUMEQUAL
    /// 0027 : OpCode.JMP 09
    /// 0029 : OpCode.DROP
    /// 002A : OpCode.ISNULL
    /// 002B : OpCode.JMP 05
    /// 002D : OpCode.DROP
    /// 002E : OpCode.DROP
    /// 002F : OpCode.PUSHF
    /// 0030 : OpCode.JMPIF 04
    /// 0032 : OpCode.PUSHF
    /// 0033 : OpCode.RET
    /// 0034 : OpCode.LDARG1
    /// 0035 : OpCode.LDARG0
    /// 0036 : OpCode.EQUAL
    /// 0037 : OpCode.RET
    /// </remarks>
    [DisplayName("longNullableEqual")]
    public abstract bool? LongNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHNULL
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 04
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.RET
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.ISNULL
    /// 000C : OpCode.NOT
    /// 000D : OpCode.NOT
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("longNullableEqualNull")]
    public abstract bool? LongNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0E
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.NOT
    /// 0017 : OpCode.JMPIF 05
    /// 0019 : OpCode.PUSHF
    /// 001A : OpCode.JMP 18
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.DUP
    /// 001F : OpCode.ISNULL
    /// 0020 : OpCode.JMPIF 0A
    /// 0022 : OpCode.SWAP
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.ISNULL
    /// 0025 : OpCode.JMPIF 09
    /// 0027 : OpCode.NUMEQUAL
    /// 0028 : OpCode.JMP 09
    /// 002A : OpCode.DROP
    /// 002B : OpCode.ISNULL
    /// 002C : OpCode.JMP 05
    /// 002E : OpCode.DROP
    /// 002F : OpCode.DROP
    /// 0030 : OpCode.PUSHF
    /// 0031 : OpCode.NOT
    /// 0032 : OpCode.JMPIF 04
    /// 0034 : OpCode.PUSHF
    /// 0035 : OpCode.RET
    /// 0036 : OpCode.LDARG1
    /// 0037 : OpCode.LDARG0
    /// 0038 : OpCode.NOTEQUAL
    /// 0039 : OpCode.RET
    /// </remarks>
    [DisplayName("longNullableNotEqual")]
    public abstract bool? LongNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.CALLT 0000
    /// 000B : OpCode.DUP
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.JMPIFNOT 05
    /// 000F : OpCode.DROP
    /// 0010 : OpCode.PUSHDATA1
    /// 0012 : OpCode.PUSHDATA1 31
    /// 0015 : OpCode.EQUAL
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("longNullableToString")]
    public abstract bool? LongNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0D
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.JMPIF 05
    /// 0018 : OpCode.PUSHF
    /// 0019 : OpCode.JMP 17
    /// 001B : OpCode.LDARG1
    /// 001C : OpCode.LDARG0
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ISNULL
    /// 001F : OpCode.JMPIF 0A
    /// 0021 : OpCode.SWAP
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.ISNULL
    /// 0024 : OpCode.JMPIF 09
    /// 0026 : OpCode.NUMEQUAL
    /// 0027 : OpCode.JMP 09
    /// 0029 : OpCode.DROP
    /// 002A : OpCode.ISNULL
    /// 002B : OpCode.JMP 05
    /// 002D : OpCode.DROP
    /// 002E : OpCode.DROP
    /// 002F : OpCode.PUSHF
    /// 0030 : OpCode.JMPIF 04
    /// 0032 : OpCode.PUSHF
    /// 0033 : OpCode.RET
    /// 0034 : OpCode.LDARG1
    /// 0035 : OpCode.LDARG0
    /// 0036 : OpCode.EQUAL
    /// 0037 : OpCode.RET
    /// </remarks>
    [DisplayName("sByteNullableEqual")]
    public abstract bool? SByteNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHNULL
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 04
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.RET
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.ISNULL
    /// 000C : OpCode.NOT
    /// 000D : OpCode.NOT
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("sByteNullableEqualNull")]
    public abstract bool? SByteNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0E
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.NOT
    /// 0017 : OpCode.JMPIF 05
    /// 0019 : OpCode.PUSHF
    /// 001A : OpCode.JMP 18
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.DUP
    /// 001F : OpCode.ISNULL
    /// 0020 : OpCode.JMPIF 0A
    /// 0022 : OpCode.SWAP
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.ISNULL
    /// 0025 : OpCode.JMPIF 09
    /// 0027 : OpCode.NUMEQUAL
    /// 0028 : OpCode.JMP 09
    /// 002A : OpCode.DROP
    /// 002B : OpCode.ISNULL
    /// 002C : OpCode.JMP 05
    /// 002E : OpCode.DROP
    /// 002F : OpCode.DROP
    /// 0030 : OpCode.PUSHF
    /// 0031 : OpCode.NOT
    /// 0032 : OpCode.JMPIF 04
    /// 0034 : OpCode.PUSHF
    /// 0035 : OpCode.RET
    /// 0036 : OpCode.LDARG1
    /// 0037 : OpCode.LDARG0
    /// 0038 : OpCode.NOTEQUAL
    /// 0039 : OpCode.RET
    /// </remarks>
    [DisplayName("sByteNullableNotEqual")]
    public abstract bool? SByteNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.CALLT 0000
    /// 000B : OpCode.DUP
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.JMPIFNOT 05
    /// 000F : OpCode.DROP
    /// 0010 : OpCode.PUSHDATA1
    /// 0012 : OpCode.PUSHDATA1 31
    /// 0015 : OpCode.EQUAL
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("sByteNullableToString")]
    public abstract bool? SByteNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0D
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.JMPIF 05
    /// 0018 : OpCode.PUSHF
    /// 0019 : OpCode.JMP 17
    /// 001B : OpCode.LDARG1
    /// 001C : OpCode.LDARG0
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ISNULL
    /// 001F : OpCode.JMPIF 0A
    /// 0021 : OpCode.SWAP
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.ISNULL
    /// 0024 : OpCode.JMPIF 09
    /// 0026 : OpCode.NUMEQUAL
    /// 0027 : OpCode.JMP 09
    /// 0029 : OpCode.DROP
    /// 002A : OpCode.ISNULL
    /// 002B : OpCode.JMP 05
    /// 002D : OpCode.DROP
    /// 002E : OpCode.DROP
    /// 002F : OpCode.PUSHF
    /// 0030 : OpCode.JMPIF 04
    /// 0032 : OpCode.PUSHF
    /// 0033 : OpCode.RET
    /// 0034 : OpCode.LDARG1
    /// 0035 : OpCode.LDARG0
    /// 0036 : OpCode.EQUAL
    /// 0037 : OpCode.RET
    /// </remarks>
    [DisplayName("shortNullableEqual")]
    public abstract bool? ShortNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHNULL
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 04
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.RET
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.ISNULL
    /// 000C : OpCode.NOT
    /// 000D : OpCode.NOT
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("shortNullableEqualNull")]
    public abstract bool? ShortNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0E
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.NOT
    /// 0017 : OpCode.JMPIF 05
    /// 0019 : OpCode.PUSHF
    /// 001A : OpCode.JMP 18
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.DUP
    /// 001F : OpCode.ISNULL
    /// 0020 : OpCode.JMPIF 0A
    /// 0022 : OpCode.SWAP
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.ISNULL
    /// 0025 : OpCode.JMPIF 09
    /// 0027 : OpCode.NUMEQUAL
    /// 0028 : OpCode.JMP 09
    /// 002A : OpCode.DROP
    /// 002B : OpCode.ISNULL
    /// 002C : OpCode.JMP 05
    /// 002E : OpCode.DROP
    /// 002F : OpCode.DROP
    /// 0030 : OpCode.PUSHF
    /// 0031 : OpCode.NOT
    /// 0032 : OpCode.JMPIF 04
    /// 0034 : OpCode.PUSHF
    /// 0035 : OpCode.RET
    /// 0036 : OpCode.LDARG1
    /// 0037 : OpCode.LDARG0
    /// 0038 : OpCode.NOTEQUAL
    /// 0039 : OpCode.RET
    /// </remarks>
    [DisplayName("shortNullableNotEqual")]
    public abstract bool? ShortNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.CALLT 0000
    /// 000B : OpCode.DUP
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.JMPIFNOT 05
    /// 000F : OpCode.DROP
    /// 0010 : OpCode.PUSHDATA1
    /// 0012 : OpCode.PUSHDATA1 31
    /// 0015 : OpCode.EQUAL
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("shortNullableToString")]
    public abstract bool? ShortNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0D
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.JMPIF 05
    /// 0018 : OpCode.PUSHF
    /// 0019 : OpCode.JMP 17
    /// 001B : OpCode.LDARG1
    /// 001C : OpCode.LDARG0
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ISNULL
    /// 001F : OpCode.JMPIF 0A
    /// 0021 : OpCode.SWAP
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.ISNULL
    /// 0024 : OpCode.JMPIF 09
    /// 0026 : OpCode.NUMEQUAL
    /// 0027 : OpCode.JMP 09
    /// 0029 : OpCode.DROP
    /// 002A : OpCode.ISNULL
    /// 002B : OpCode.JMP 05
    /// 002D : OpCode.DROP
    /// 002E : OpCode.DROP
    /// 002F : OpCode.PUSHF
    /// 0030 : OpCode.JMPIF 04
    /// 0032 : OpCode.PUSHF
    /// 0033 : OpCode.RET
    /// 0034 : OpCode.LDARG1
    /// 0035 : OpCode.LDARG0
    /// 0036 : OpCode.EQUAL
    /// 0037 : OpCode.RET
    /// </remarks>
    [DisplayName("uIntNullableEqual")]
    public abstract bool? UIntNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHNULL
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 04
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.RET
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.ISNULL
    /// 000C : OpCode.NOT
    /// 000D : OpCode.NOT
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("uIntNullableEqualNull")]
    public abstract bool? UIntNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0E
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.NOT
    /// 0017 : OpCode.JMPIF 05
    /// 0019 : OpCode.PUSHF
    /// 001A : OpCode.JMP 18
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.DUP
    /// 001F : OpCode.ISNULL
    /// 0020 : OpCode.JMPIF 0A
    /// 0022 : OpCode.SWAP
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.ISNULL
    /// 0025 : OpCode.JMPIF 09
    /// 0027 : OpCode.NUMEQUAL
    /// 0028 : OpCode.JMP 09
    /// 002A : OpCode.DROP
    /// 002B : OpCode.ISNULL
    /// 002C : OpCode.JMP 05
    /// 002E : OpCode.DROP
    /// 002F : OpCode.DROP
    /// 0030 : OpCode.PUSHF
    /// 0031 : OpCode.NOT
    /// 0032 : OpCode.JMPIF 04
    /// 0034 : OpCode.PUSHF
    /// 0035 : OpCode.RET
    /// 0036 : OpCode.LDARG1
    /// 0037 : OpCode.LDARG0
    /// 0038 : OpCode.NOTEQUAL
    /// 0039 : OpCode.RET
    /// </remarks>
    [DisplayName("uIntNullableNotEqual")]
    public abstract bool? UIntNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.CALLT 0000
    /// 000B : OpCode.DUP
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.JMPIFNOT 05
    /// 000F : OpCode.DROP
    /// 0010 : OpCode.PUSHDATA1
    /// 0012 : OpCode.PUSHDATA1 31
    /// 0015 : OpCode.EQUAL
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("uIntNullableToString")]
    public abstract bool? UIntNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0D
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.JMPIF 05
    /// 0018 : OpCode.PUSHF
    /// 0019 : OpCode.JMP 17
    /// 001B : OpCode.LDARG1
    /// 001C : OpCode.LDARG0
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ISNULL
    /// 001F : OpCode.JMPIF 0A
    /// 0021 : OpCode.SWAP
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.ISNULL
    /// 0024 : OpCode.JMPIF 09
    /// 0026 : OpCode.NUMEQUAL
    /// 0027 : OpCode.JMP 09
    /// 0029 : OpCode.DROP
    /// 002A : OpCode.ISNULL
    /// 002B : OpCode.JMP 05
    /// 002D : OpCode.DROP
    /// 002E : OpCode.DROP
    /// 002F : OpCode.PUSHF
    /// 0030 : OpCode.JMPIF 04
    /// 0032 : OpCode.PUSHF
    /// 0033 : OpCode.RET
    /// 0034 : OpCode.LDARG1
    /// 0035 : OpCode.LDARG0
    /// 0036 : OpCode.EQUAL
    /// 0037 : OpCode.RET
    /// </remarks>
    [DisplayName("uLongNullableEqual")]
    public abstract bool? ULongNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHNULL
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 04
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.RET
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.ISNULL
    /// 000C : OpCode.NOT
    /// 000D : OpCode.NOT
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("uLongNullableEqualNull")]
    public abstract bool? ULongNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0E
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.NOT
    /// 0017 : OpCode.JMPIF 05
    /// 0019 : OpCode.PUSHF
    /// 001A : OpCode.JMP 18
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.DUP
    /// 001F : OpCode.ISNULL
    /// 0020 : OpCode.JMPIF 0A
    /// 0022 : OpCode.SWAP
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.ISNULL
    /// 0025 : OpCode.JMPIF 09
    /// 0027 : OpCode.NUMEQUAL
    /// 0028 : OpCode.JMP 09
    /// 002A : OpCode.DROP
    /// 002B : OpCode.ISNULL
    /// 002C : OpCode.JMP 05
    /// 002E : OpCode.DROP
    /// 002F : OpCode.DROP
    /// 0030 : OpCode.PUSHF
    /// 0031 : OpCode.NOT
    /// 0032 : OpCode.JMPIF 04
    /// 0034 : OpCode.PUSHF
    /// 0035 : OpCode.RET
    /// 0036 : OpCode.LDARG1
    /// 0037 : OpCode.LDARG0
    /// 0038 : OpCode.NOTEQUAL
    /// 0039 : OpCode.RET
    /// </remarks>
    [DisplayName("uLongNullableNotEqual")]
    public abstract bool? ULongNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.CALLT 0000
    /// 000B : OpCode.DUP
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.JMPIFNOT 05
    /// 000F : OpCode.DROP
    /// 0010 : OpCode.PUSHDATA1
    /// 0012 : OpCode.PUSHDATA1 31
    /// 0015 : OpCode.EQUAL
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("uLongNullableToString")]
    public abstract bool? ULongNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0D
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.JMPIF 05
    /// 0018 : OpCode.PUSHF
    /// 0019 : OpCode.JMP 17
    /// 001B : OpCode.LDARG1
    /// 001C : OpCode.LDARG0
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ISNULL
    /// 001F : OpCode.JMPIF 0A
    /// 0021 : OpCode.SWAP
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.ISNULL
    /// 0024 : OpCode.JMPIF 09
    /// 0026 : OpCode.NUMEQUAL
    /// 0027 : OpCode.JMP 09
    /// 0029 : OpCode.DROP
    /// 002A : OpCode.ISNULL
    /// 002B : OpCode.JMP 05
    /// 002D : OpCode.DROP
    /// 002E : OpCode.DROP
    /// 002F : OpCode.PUSHF
    /// 0030 : OpCode.JMPIF 04
    /// 0032 : OpCode.PUSHF
    /// 0033 : OpCode.RET
    /// 0034 : OpCode.LDARG1
    /// 0035 : OpCode.LDARG0
    /// 0036 : OpCode.EQUAL
    /// 0037 : OpCode.RET
    /// </remarks>
    [DisplayName("uShortNullableEqual")]
    public abstract bool? UShortNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHNULL
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.JMPIF 04
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.RET
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.ISNULL
    /// 000C : OpCode.NOT
    /// 000D : OpCode.NOT
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("uShortNullableEqualNull")]
    public abstract bool? UShortNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 0E
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 05
    /// 0011 : OpCode.NUMEQUAL
    /// 0012 : OpCode.JMP 04
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHF
    /// 0016 : OpCode.NOT
    /// 0017 : OpCode.JMPIF 05
    /// 0019 : OpCode.PUSHF
    /// 001A : OpCode.JMP 18
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.DUP
    /// 001F : OpCode.ISNULL
    /// 0020 : OpCode.JMPIF 0A
    /// 0022 : OpCode.SWAP
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.ISNULL
    /// 0025 : OpCode.JMPIF 09
    /// 0027 : OpCode.NUMEQUAL
    /// 0028 : OpCode.JMP 09
    /// 002A : OpCode.DROP
    /// 002B : OpCode.ISNULL
    /// 002C : OpCode.JMP 05
    /// 002E : OpCode.DROP
    /// 002F : OpCode.DROP
    /// 0030 : OpCode.PUSHF
    /// 0031 : OpCode.NOT
    /// 0032 : OpCode.JMPIF 04
    /// 0034 : OpCode.PUSHF
    /// 0035 : OpCode.RET
    /// 0036 : OpCode.LDARG1
    /// 0037 : OpCode.LDARG0
    /// 0038 : OpCode.NOTEQUAL
    /// 0039 : OpCode.RET
    /// </remarks>
    [DisplayName("uShortNullableNotEqual")]
    public abstract bool? UShortNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.CALLT 0000
    /// 000B : OpCode.DUP
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.JMPIFNOT 05
    /// 000F : OpCode.DROP
    /// 0010 : OpCode.PUSHDATA1
    /// 0012 : OpCode.PUSHDATA1 31
    /// 0015 : OpCode.EQUAL
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("uShortNullableToString")]
    public abstract bool? UShortNullableToString(BigInteger? a);

    #endregion

}
