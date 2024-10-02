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
    [DisplayName("bigIntegerNullableEqual")]
    public abstract bool? BigIntegerNullableEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : JMPIF
    // 0018 : PUSHF
    // 0019 : JMP
    // 001B : LDARG1
    // 001C : LDARG0
    // 001D : DUP
    // 001E : ISNULL
    // 001F : JMPIF
    // 0021 : SWAP
    // 0022 : DUP
    // 0023 : ISNULL
    // 0024 : JMPIF
    // 0026 : NUMEQUAL
    // 0027 : JMP
    // 0029 : DROP
    // 002A : ISNULL
    // 002B : JMP
    // 002D : DROP
    // 002E : DROP
    // 002F : PUSHF
    // 0030 : JMPIF
    // 0032 : PUSHF
    // 0033 : RET
    // 0034 : LDARG1
    // 0035 : LDARG0
    // 0036 : EQUAL
    // 0037 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bigIntegerNullableEqualNull")]
    public abstract bool? BigIntegerNullableEqualNull(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHNULL
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : RET
    // 000A : LDARG0
    // 000B : ISNULL
    // 000C : NOT
    // 000D : NOT
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bigIntegerNullableNotEqual")]
    public abstract bool? BigIntegerNullableNotEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : NOTEQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : NOT
    // 0017 : JMPIF
    // 0019 : PUSHF
    // 001A : JMP
    // 001C : LDARG1
    // 001D : LDARG0
    // 001E : DUP
    // 001F : ISNULL
    // 0020 : JMPIF
    // 0022 : SWAP
    // 0023 : DUP
    // 0024 : ISNULL
    // 0025 : JMPIF
    // 0027 : NUMEQUAL
    // 0028 : JMP
    // 002A : DROP
    // 002B : ISNULL
    // 002C : JMP
    // 002E : DROP
    // 002F : DROP
    // 0030 : PUSHF
    // 0031 : NOT
    // 0032 : JMPIF
    // 0034 : PUSHF
    // 0035 : RET
    // 0036 : LDARG1
    // 0037 : LDARG0
    // 0038 : NOTEQUAL
    // 0039 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bigIntegerNullableToString")]
    public abstract bool? BigIntegerNullableToString(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : ISNULL
    // 0006 : JMPIF
    // 0008 : CALLT
    // 000B : DUP
    // 000C : ISNULL
    // 000D : JMPIFNOT
    // 000F : DROP
    // 0010 : PUSHDATA1
    // 0012 : PUSHDATA1
    // 0015 : EQUAL
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("boolNullableEqual")]
    public abstract bool? BoolNullableEqual(bool? a, bool? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : EQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : JMPIF
    // 0018 : PUSHF
    // 0019 : JMP
    // 001B : LDARG1
    // 001C : LDARG0
    // 001D : DUP
    // 001E : ISNULL
    // 001F : JMPIF
    // 0021 : DUP
    // 0022 : ISNULL
    // 0023 : JMPIF
    // 0025 : EQUAL
    // 0026 : JMP
    // 0028 : DROP
    // 0029 : ISNULL
    // 002A : JMP
    // 002C : DROP
    // 002D : PUSHF
    // 002E : JMPIF
    // 0030 : PUSHF
    // 0031 : RET
    // 0032 : LDARG1
    // 0033 : LDARG0
    // 0034 : EQUAL
    // 0035 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("boolNullableEqualNull")]
    public abstract bool? BoolNullableEqualNull(bool? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHNULL
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : RET
    // 000A : LDARG0
    // 000B : ISNULL
    // 000C : NOT
    // 000D : NOT
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("boolNullableNotEqual")]
    public abstract bool? BoolNullableNotEqual(bool? a, bool? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : NOTEQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : EQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : NOT
    // 0017 : JMPIF
    // 0019 : PUSHF
    // 001A : JMP
    // 001C : LDARG1
    // 001D : LDARG0
    // 001E : DUP
    // 001F : ISNULL
    // 0020 : JMPIF
    // 0022 : DUP
    // 0023 : ISNULL
    // 0024 : JMPIF
    // 0026 : EQUAL
    // 0027 : JMP
    // 0029 : DROP
    // 002A : ISNULL
    // 002B : JMP
    // 002D : DROP
    // 002E : PUSHF
    // 002F : NOT
    // 0030 : JMPIF
    // 0032 : PUSHF
    // 0033 : RET
    // 0034 : LDARG1
    // 0035 : LDARG0
    // 0036 : NOTEQUAL
    // 0037 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("boolNullableToString")]
    public abstract bool? BoolNullableToString(bool? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : ISNULL
    // 0006 : JMPIF
    // 0008 : JMPIF
    // 000A : PUSHDATA1
    // 0011 : JMP
    // 0013 : PUSHDATA1
    // 0019 : DUP
    // 001A : ISNULL
    // 001B : JMPIFNOT
    // 001D : DROP
    // 001E : PUSHDATA1
    // 0020 : PUSHDATA1
    // 0026 : EQUAL
    // 0027 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteNullableEqual")]
    public abstract bool? ByteNullableEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : JMPIF
    // 0018 : PUSHF
    // 0019 : JMP
    // 001B : LDARG1
    // 001C : LDARG0
    // 001D : DUP
    // 001E : ISNULL
    // 001F : JMPIF
    // 0021 : SWAP
    // 0022 : DUP
    // 0023 : ISNULL
    // 0024 : JMPIF
    // 0026 : NUMEQUAL
    // 0027 : JMP
    // 0029 : DROP
    // 002A : ISNULL
    // 002B : JMP
    // 002D : DROP
    // 002E : DROP
    // 002F : PUSHF
    // 0030 : JMPIF
    // 0032 : PUSHF
    // 0033 : RET
    // 0034 : LDARG1
    // 0035 : LDARG0
    // 0036 : EQUAL
    // 0037 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteNullableEqualNull")]
    public abstract bool? ByteNullableEqualNull(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHNULL
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : RET
    // 000A : LDARG0
    // 000B : ISNULL
    // 000C : NOT
    // 000D : NOT
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteNullableNotEqual")]
    public abstract bool? ByteNullableNotEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : NOTEQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : NOT
    // 0017 : JMPIF
    // 0019 : PUSHF
    // 001A : JMP
    // 001C : LDARG1
    // 001D : LDARG0
    // 001E : DUP
    // 001F : ISNULL
    // 0020 : JMPIF
    // 0022 : SWAP
    // 0023 : DUP
    // 0024 : ISNULL
    // 0025 : JMPIF
    // 0027 : NUMEQUAL
    // 0028 : JMP
    // 002A : DROP
    // 002B : ISNULL
    // 002C : JMP
    // 002E : DROP
    // 002F : DROP
    // 0030 : PUSHF
    // 0031 : NOT
    // 0032 : JMPIF
    // 0034 : PUSHF
    // 0035 : RET
    // 0036 : LDARG1
    // 0037 : LDARG0
    // 0038 : NOTEQUAL
    // 0039 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteNullableToString")]
    public abstract bool? ByteNullableToString(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : ISNULL
    // 0006 : JMPIF
    // 0008 : CALLT
    // 000B : DUP
    // 000C : ISNULL
    // 000D : JMPIFNOT
    // 000F : DROP
    // 0010 : PUSHDATA1
    // 0012 : PUSHDATA1
    // 0015 : EQUAL
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("h160NullableEqualNull")]
    public abstract bool? H160NullableEqualNull(UInt160? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHNULL
    // 0005 : EQUAL
    // 0006 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("h160NullableNotEqual")]
    public abstract bool? H160NullableNotEqual(UInt160? a, UInt160? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : NOTEQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : EQUAL
    // 000E : NOT
    // 000F : JMPIF
    // 0011 : PUSHF
    // 0012 : JMP
    // 0014 : LDARG1
    // 0015 : LDARG0
    // 0016 : EQUAL
    // 0017 : NOT
    // 0018 : JMPIF
    // 001A : PUSHF
    // 001B : RET
    // 001C : LDARG1
    // 001D : LDARG0
    // 001E : NOTEQUAL
    // 001F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("h256NullableEqual")]
    public abstract bool? H256NullableEqual(UInt256? a, UInt256? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : EQUAL
    // 000E : JMPIF
    // 0010 : PUSHF
    // 0011 : JMP
    // 0013 : LDARG1
    // 0014 : LDARG0
    // 0015 : EQUAL
    // 0016 : JMPIF
    // 0018 : PUSHF
    // 0019 : RET
    // 001A : LDARG1
    // 001B : LDARG0
    // 001C : EQUAL
    // 001D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("h256NullableNotEqual")]
    public abstract bool? H256NullableNotEqual(UInt256? a, UInt256? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : NOTEQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : EQUAL
    // 000E : NOT
    // 000F : JMPIF
    // 0011 : PUSHF
    // 0012 : JMP
    // 0014 : LDARG1
    // 0015 : LDARG0
    // 0016 : EQUAL
    // 0017 : NOT
    // 0018 : JMPIF
    // 001A : PUSHF
    // 001B : RET
    // 001C : LDARG1
    // 001D : LDARG0
    // 001E : NOTEQUAL
    // 001F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intNullableEqual")]
    public abstract bool? IntNullableEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : JMPIF
    // 0018 : PUSHF
    // 0019 : JMP
    // 001B : LDARG1
    // 001C : LDARG0
    // 001D : DUP
    // 001E : ISNULL
    // 001F : JMPIF
    // 0021 : SWAP
    // 0022 : DUP
    // 0023 : ISNULL
    // 0024 : JMPIF
    // 0026 : NUMEQUAL
    // 0027 : JMP
    // 0029 : DROP
    // 002A : ISNULL
    // 002B : JMP
    // 002D : DROP
    // 002E : DROP
    // 002F : PUSHF
    // 0030 : JMPIF
    // 0032 : PUSHF
    // 0033 : RET
    // 0034 : LDARG1
    // 0035 : LDARG0
    // 0036 : EQUAL
    // 0037 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intNullableEqualNull")]
    public abstract bool? IntNullableEqualNull(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHNULL
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : RET
    // 000A : LDARG0
    // 000B : ISNULL
    // 000C : NOT
    // 000D : NOT
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intNullableNotEqual")]
    public abstract bool? IntNullableNotEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : NOTEQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : NOT
    // 0017 : JMPIF
    // 0019 : PUSHF
    // 001A : JMP
    // 001C : LDARG1
    // 001D : LDARG0
    // 001E : DUP
    // 001F : ISNULL
    // 0020 : JMPIF
    // 0022 : SWAP
    // 0023 : DUP
    // 0024 : ISNULL
    // 0025 : JMPIF
    // 0027 : NUMEQUAL
    // 0028 : JMP
    // 002A : DROP
    // 002B : ISNULL
    // 002C : JMP
    // 002E : DROP
    // 002F : DROP
    // 0030 : PUSHF
    // 0031 : NOT
    // 0032 : JMPIF
    // 0034 : PUSHF
    // 0035 : RET
    // 0036 : LDARG1
    // 0037 : LDARG0
    // 0038 : NOTEQUAL
    // 0039 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intNullableToString")]
    public abstract bool? IntNullableToString(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : ISNULL
    // 0006 : JMPIF
    // 0008 : CALLT
    // 000B : DUP
    // 000C : ISNULL
    // 000D : JMPIFNOT
    // 000F : DROP
    // 0010 : PUSHDATA1
    // 0012 : PUSHDATA1
    // 0015 : EQUAL
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("longNullableEqual")]
    public abstract bool? LongNullableEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : JMPIF
    // 0018 : PUSHF
    // 0019 : JMP
    // 001B : LDARG1
    // 001C : LDARG0
    // 001D : DUP
    // 001E : ISNULL
    // 001F : JMPIF
    // 0021 : SWAP
    // 0022 : DUP
    // 0023 : ISNULL
    // 0024 : JMPIF
    // 0026 : NUMEQUAL
    // 0027 : JMP
    // 0029 : DROP
    // 002A : ISNULL
    // 002B : JMP
    // 002D : DROP
    // 002E : DROP
    // 002F : PUSHF
    // 0030 : JMPIF
    // 0032 : PUSHF
    // 0033 : RET
    // 0034 : LDARG1
    // 0035 : LDARG0
    // 0036 : EQUAL
    // 0037 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("longNullableEqualNull")]
    public abstract bool? LongNullableEqualNull(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHNULL
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : RET
    // 000A : LDARG0
    // 000B : ISNULL
    // 000C : NOT
    // 000D : NOT
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("longNullableNotEqual")]
    public abstract bool? LongNullableNotEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : NOTEQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : NOT
    // 0017 : JMPIF
    // 0019 : PUSHF
    // 001A : JMP
    // 001C : LDARG1
    // 001D : LDARG0
    // 001E : DUP
    // 001F : ISNULL
    // 0020 : JMPIF
    // 0022 : SWAP
    // 0023 : DUP
    // 0024 : ISNULL
    // 0025 : JMPIF
    // 0027 : NUMEQUAL
    // 0028 : JMP
    // 002A : DROP
    // 002B : ISNULL
    // 002C : JMP
    // 002E : DROP
    // 002F : DROP
    // 0030 : PUSHF
    // 0031 : NOT
    // 0032 : JMPIF
    // 0034 : PUSHF
    // 0035 : RET
    // 0036 : LDARG1
    // 0037 : LDARG0
    // 0038 : NOTEQUAL
    // 0039 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("longNullableToString")]
    public abstract bool? LongNullableToString(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : ISNULL
    // 0006 : JMPIF
    // 0008 : CALLT
    // 000B : DUP
    // 000C : ISNULL
    // 000D : JMPIFNOT
    // 000F : DROP
    // 0010 : PUSHDATA1
    // 0012 : PUSHDATA1
    // 0015 : EQUAL
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sByteNullableEqual")]
    public abstract bool? SByteNullableEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : JMPIF
    // 0018 : PUSHF
    // 0019 : JMP
    // 001B : LDARG1
    // 001C : LDARG0
    // 001D : DUP
    // 001E : ISNULL
    // 001F : JMPIF
    // 0021 : SWAP
    // 0022 : DUP
    // 0023 : ISNULL
    // 0024 : JMPIF
    // 0026 : NUMEQUAL
    // 0027 : JMP
    // 0029 : DROP
    // 002A : ISNULL
    // 002B : JMP
    // 002D : DROP
    // 002E : DROP
    // 002F : PUSHF
    // 0030 : JMPIF
    // 0032 : PUSHF
    // 0033 : RET
    // 0034 : LDARG1
    // 0035 : LDARG0
    // 0036 : EQUAL
    // 0037 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sByteNullableEqualNull")]
    public abstract bool? SByteNullableEqualNull(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHNULL
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : RET
    // 000A : LDARG0
    // 000B : ISNULL
    // 000C : NOT
    // 000D : NOT
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sByteNullableNotEqual")]
    public abstract bool? SByteNullableNotEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : NOTEQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : NOT
    // 0017 : JMPIF
    // 0019 : PUSHF
    // 001A : JMP
    // 001C : LDARG1
    // 001D : LDARG0
    // 001E : DUP
    // 001F : ISNULL
    // 0020 : JMPIF
    // 0022 : SWAP
    // 0023 : DUP
    // 0024 : ISNULL
    // 0025 : JMPIF
    // 0027 : NUMEQUAL
    // 0028 : JMP
    // 002A : DROP
    // 002B : ISNULL
    // 002C : JMP
    // 002E : DROP
    // 002F : DROP
    // 0030 : PUSHF
    // 0031 : NOT
    // 0032 : JMPIF
    // 0034 : PUSHF
    // 0035 : RET
    // 0036 : LDARG1
    // 0037 : LDARG0
    // 0038 : NOTEQUAL
    // 0039 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sByteNullableToString")]
    public abstract bool? SByteNullableToString(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : ISNULL
    // 0006 : JMPIF
    // 0008 : CALLT
    // 000B : DUP
    // 000C : ISNULL
    // 000D : JMPIFNOT
    // 000F : DROP
    // 0010 : PUSHDATA1
    // 0012 : PUSHDATA1
    // 0015 : EQUAL
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("shortNullableEqual")]
    public abstract bool? ShortNullableEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : JMPIF
    // 0018 : PUSHF
    // 0019 : JMP
    // 001B : LDARG1
    // 001C : LDARG0
    // 001D : DUP
    // 001E : ISNULL
    // 001F : JMPIF
    // 0021 : SWAP
    // 0022 : DUP
    // 0023 : ISNULL
    // 0024 : JMPIF
    // 0026 : NUMEQUAL
    // 0027 : JMP
    // 0029 : DROP
    // 002A : ISNULL
    // 002B : JMP
    // 002D : DROP
    // 002E : DROP
    // 002F : PUSHF
    // 0030 : JMPIF
    // 0032 : PUSHF
    // 0033 : RET
    // 0034 : LDARG1
    // 0035 : LDARG0
    // 0036 : EQUAL
    // 0037 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("shortNullableEqualNull")]
    public abstract bool? ShortNullableEqualNull(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHNULL
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : RET
    // 000A : LDARG0
    // 000B : ISNULL
    // 000C : NOT
    // 000D : NOT
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("shortNullableNotEqual")]
    public abstract bool? ShortNullableNotEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : NOTEQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : NOT
    // 0017 : JMPIF
    // 0019 : PUSHF
    // 001A : JMP
    // 001C : LDARG1
    // 001D : LDARG0
    // 001E : DUP
    // 001F : ISNULL
    // 0020 : JMPIF
    // 0022 : SWAP
    // 0023 : DUP
    // 0024 : ISNULL
    // 0025 : JMPIF
    // 0027 : NUMEQUAL
    // 0028 : JMP
    // 002A : DROP
    // 002B : ISNULL
    // 002C : JMP
    // 002E : DROP
    // 002F : DROP
    // 0030 : PUSHF
    // 0031 : NOT
    // 0032 : JMPIF
    // 0034 : PUSHF
    // 0035 : RET
    // 0036 : LDARG1
    // 0037 : LDARG0
    // 0038 : NOTEQUAL
    // 0039 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("shortNullableToString")]
    public abstract bool? ShortNullableToString(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : ISNULL
    // 0006 : JMPIF
    // 0008 : CALLT
    // 000B : DUP
    // 000C : ISNULL
    // 000D : JMPIFNOT
    // 000F : DROP
    // 0010 : PUSHDATA1
    // 0012 : PUSHDATA1
    // 0015 : EQUAL
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uIntNullableEqual")]
    public abstract bool? UIntNullableEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : JMPIF
    // 0018 : PUSHF
    // 0019 : JMP
    // 001B : LDARG1
    // 001C : LDARG0
    // 001D : DUP
    // 001E : ISNULL
    // 001F : JMPIF
    // 0021 : SWAP
    // 0022 : DUP
    // 0023 : ISNULL
    // 0024 : JMPIF
    // 0026 : NUMEQUAL
    // 0027 : JMP
    // 0029 : DROP
    // 002A : ISNULL
    // 002B : JMP
    // 002D : DROP
    // 002E : DROP
    // 002F : PUSHF
    // 0030 : JMPIF
    // 0032 : PUSHF
    // 0033 : RET
    // 0034 : LDARG1
    // 0035 : LDARG0
    // 0036 : EQUAL
    // 0037 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uIntNullableEqualNull")]
    public abstract bool? UIntNullableEqualNull(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHNULL
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : RET
    // 000A : LDARG0
    // 000B : ISNULL
    // 000C : NOT
    // 000D : NOT
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uIntNullableNotEqual")]
    public abstract bool? UIntNullableNotEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : NOTEQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : NOT
    // 0017 : JMPIF
    // 0019 : PUSHF
    // 001A : JMP
    // 001C : LDARG1
    // 001D : LDARG0
    // 001E : DUP
    // 001F : ISNULL
    // 0020 : JMPIF
    // 0022 : SWAP
    // 0023 : DUP
    // 0024 : ISNULL
    // 0025 : JMPIF
    // 0027 : NUMEQUAL
    // 0028 : JMP
    // 002A : DROP
    // 002B : ISNULL
    // 002C : JMP
    // 002E : DROP
    // 002F : DROP
    // 0030 : PUSHF
    // 0031 : NOT
    // 0032 : JMPIF
    // 0034 : PUSHF
    // 0035 : RET
    // 0036 : LDARG1
    // 0037 : LDARG0
    // 0038 : NOTEQUAL
    // 0039 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uIntNullableToString")]
    public abstract bool? UIntNullableToString(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : ISNULL
    // 0006 : JMPIF
    // 0008 : CALLT
    // 000B : DUP
    // 000C : ISNULL
    // 000D : JMPIFNOT
    // 000F : DROP
    // 0010 : PUSHDATA1
    // 0012 : PUSHDATA1
    // 0015 : EQUAL
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uLongNullableEqual")]
    public abstract bool? ULongNullableEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : JMPIF
    // 0018 : PUSHF
    // 0019 : JMP
    // 001B : LDARG1
    // 001C : LDARG0
    // 001D : DUP
    // 001E : ISNULL
    // 001F : JMPIF
    // 0021 : SWAP
    // 0022 : DUP
    // 0023 : ISNULL
    // 0024 : JMPIF
    // 0026 : NUMEQUAL
    // 0027 : JMP
    // 0029 : DROP
    // 002A : ISNULL
    // 002B : JMP
    // 002D : DROP
    // 002E : DROP
    // 002F : PUSHF
    // 0030 : JMPIF
    // 0032 : PUSHF
    // 0033 : RET
    // 0034 : LDARG1
    // 0035 : LDARG0
    // 0036 : EQUAL
    // 0037 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uLongNullableEqualNull")]
    public abstract bool? ULongNullableEqualNull(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHNULL
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : RET
    // 000A : LDARG0
    // 000B : ISNULL
    // 000C : NOT
    // 000D : NOT
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uLongNullableNotEqual")]
    public abstract bool? ULongNullableNotEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : NOTEQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : NOT
    // 0017 : JMPIF
    // 0019 : PUSHF
    // 001A : JMP
    // 001C : LDARG1
    // 001D : LDARG0
    // 001E : DUP
    // 001F : ISNULL
    // 0020 : JMPIF
    // 0022 : SWAP
    // 0023 : DUP
    // 0024 : ISNULL
    // 0025 : JMPIF
    // 0027 : NUMEQUAL
    // 0028 : JMP
    // 002A : DROP
    // 002B : ISNULL
    // 002C : JMP
    // 002E : DROP
    // 002F : DROP
    // 0030 : PUSHF
    // 0031 : NOT
    // 0032 : JMPIF
    // 0034 : PUSHF
    // 0035 : RET
    // 0036 : LDARG1
    // 0037 : LDARG0
    // 0038 : NOTEQUAL
    // 0039 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uLongNullableToString")]
    public abstract bool? ULongNullableToString(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : ISNULL
    // 0006 : JMPIF
    // 0008 : CALLT
    // 000B : DUP
    // 000C : ISNULL
    // 000D : JMPIFNOT
    // 000F : DROP
    // 0010 : PUSHDATA1
    // 0012 : PUSHDATA1
    // 0015 : EQUAL
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uShortNullableEqual")]
    public abstract bool? UShortNullableEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : JMPIF
    // 0018 : PUSHF
    // 0019 : JMP
    // 001B : LDARG1
    // 001C : LDARG0
    // 001D : DUP
    // 001E : ISNULL
    // 001F : JMPIF
    // 0021 : SWAP
    // 0022 : DUP
    // 0023 : ISNULL
    // 0024 : JMPIF
    // 0026 : NUMEQUAL
    // 0027 : JMP
    // 0029 : DROP
    // 002A : ISNULL
    // 002B : JMP
    // 002D : DROP
    // 002E : DROP
    // 002F : PUSHF
    // 0030 : JMPIF
    // 0032 : PUSHF
    // 0033 : RET
    // 0034 : LDARG1
    // 0035 : LDARG0
    // 0036 : EQUAL
    // 0037 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uShortNullableEqualNull")]
    public abstract bool? UShortNullableEqualNull(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHNULL
    // 0005 : EQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : RET
    // 000A : LDARG0
    // 000B : ISNULL
    // 000C : NOT
    // 000D : NOT
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uShortNullableNotEqual")]
    public abstract bool? UShortNullableNotEqual(BigInteger? a, BigInteger? b);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : NOTEQUAL
    // 0006 : JMPIF
    // 0008 : PUSHF
    // 0009 : JMP
    // 000B : LDARG0
    // 000C : LDARG1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : NUMEQUAL
    // 0012 : JMP
    // 0014 : DROP
    // 0015 : PUSHF
    // 0016 : NOT
    // 0017 : JMPIF
    // 0019 : PUSHF
    // 001A : JMP
    // 001C : LDARG1
    // 001D : LDARG0
    // 001E : DUP
    // 001F : ISNULL
    // 0020 : JMPIF
    // 0022 : SWAP
    // 0023 : DUP
    // 0024 : ISNULL
    // 0025 : JMPIF
    // 0027 : NUMEQUAL
    // 0028 : JMP
    // 002A : DROP
    // 002B : ISNULL
    // 002C : JMP
    // 002E : DROP
    // 002F : DROP
    // 0030 : PUSHF
    // 0031 : NOT
    // 0032 : JMPIF
    // 0034 : PUSHF
    // 0035 : RET
    // 0036 : LDARG1
    // 0037 : LDARG0
    // 0038 : NOTEQUAL
    // 0039 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uShortNullableToString")]
    public abstract bool? UShortNullableToString(BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : ISNULL
    // 0006 : JMPIF
    // 0008 : CALLT
    // 000B : DUP
    // 000C : ISNULL
    // 000D : JMPIFNOT
    // 000F : DROP
    // 0010 : PUSHDATA1
    // 0012 : PUSHDATA1
    // 0015 : EQUAL
    // 0016 : RET

    #endregion

}
