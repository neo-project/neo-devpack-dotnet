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
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0D
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.JMPIF 05
    /// 18 : OpCode.PUSHF
    /// 19 : OpCode.JMP 17
    /// 1B : OpCode.LDARG1
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ISNULL
    /// 1F : OpCode.JMPIF 0A
    /// 21 : OpCode.SWAP
    /// 22 : OpCode.DUP
    /// 23 : OpCode.ISNULL
    /// 24 : OpCode.JMPIF 09
    /// 26 : OpCode.NUMEQUAL
    /// 27 : OpCode.JMP 09
    /// 29 : OpCode.DROP
    /// 2A : OpCode.ISNULL
    /// 2B : OpCode.JMP 05
    /// 2D : OpCode.DROP
    /// 2E : OpCode.DROP
    /// 2F : OpCode.PUSHF
    /// 30 : OpCode.JMPIF 04
    /// 32 : OpCode.PUSHF
    /// 33 : OpCode.RET
    /// 34 : OpCode.LDARG1
    /// 35 : OpCode.LDARG0
    /// 36 : OpCode.EQUAL
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("bigIntegerNullableEqual")]
    public abstract bool? BigIntegerNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXJAQJQHjYqqpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 04
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.RET
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.NOT
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("bigIntegerNullableEqualNull")]
    public abstract bool? BigIntegerNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0E
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIF 05
    /// 19 : OpCode.PUSHF
    /// 1A : OpCode.JMP 18
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.DUP
    /// 1F : OpCode.ISNULL
    /// 20 : OpCode.JMPIF 0A
    /// 22 : OpCode.SWAP
    /// 23 : OpCode.DUP
    /// 24 : OpCode.ISNULL
    /// 25 : OpCode.JMPIF 09
    /// 27 : OpCode.NUMEQUAL
    /// 28 : OpCode.JMP 09
    /// 2A : OpCode.DROP
    /// 2B : OpCode.ISNULL
    /// 2C : OpCode.JMP 05
    /// 2E : OpCode.DROP
    /// 2F : OpCode.DROP
    /// 30 : OpCode.PUSHF
    /// 31 : OpCode.NOT
    /// 32 : OpCode.JMPIF 04
    /// 34 : OpCode.PUSHF
    /// 35 : OpCode.RET
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.LDARG0
    /// 38 : OpCode.NOTEQUAL
    /// 39 : OpCode.RET
    /// </remarks>
    [DisplayName("bigIntegerNullableNotEqual")]
    public abstract bool? BigIntegerNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwMMZdA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.CALLT 0000
    /// 0B : OpCode.DUP
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.JMPIFNOT 05
    /// 0F : OpCode.DROP
    /// 10 : OpCode.PUSHDATA1
    /// 12 : OpCode.PUSHDATA1 31
    /// 15 : OpCode.EQUAL
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("bigIntegerNullableToString")]
    public abstract bool? BigIntegerNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWXIgRFCSQFCSIVeXhK2CQJStgkCZciCEXYIgRFCSQECUB5eJdA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0D
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.EQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.JMPIF 05
    /// 18 : OpCode.PUSHF
    /// 19 : OpCode.JMP 15
    /// 1B : OpCode.LDARG1
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ISNULL
    /// 1F : OpCode.JMPIF 09
    /// 21 : OpCode.DUP
    /// 22 : OpCode.ISNULL
    /// 23 : OpCode.JMPIF 09
    /// 25 : OpCode.EQUAL
    /// 26 : OpCode.JMP 08
    /// 28 : OpCode.DROP
    /// 29 : OpCode.ISNULL
    /// 2A : OpCode.JMP 04
    /// 2C : OpCode.DROP
    /// 2D : OpCode.PUSHF
    /// 2E : OpCode.JMPIF 04
    /// 30 : OpCode.PUSHF
    /// 31 : OpCode.RET
    /// 32 : OpCode.LDARG1
    /// 33 : OpCode.LDARG0
    /// 34 : OpCode.EQUAL
    /// 35 : OpCode.RET
    /// </remarks>
    [DisplayName("boolNullableEqual")]
    public abstract bool? BoolNullableEqual(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXJAQJQHjYqqpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 04
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.RET
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.NOT
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("boolNullableEqualNull")]
    public abstract bool? BoolNullableEqualNull(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWXIgRFCaokBQkiFnl4StgkCUrYJAmXIghF2CIERQmqJAQJQHl4mEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0E
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.EQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIF 05
    /// 19 : OpCode.PUSHF
    /// 1A : OpCode.JMP 16
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.DUP
    /// 1F : OpCode.ISNULL
    /// 20 : OpCode.JMPIF 09
    /// 22 : OpCode.DUP
    /// 23 : OpCode.ISNULL
    /// 24 : OpCode.JMPIF 09
    /// 26 : OpCode.EQUAL
    /// 27 : OpCode.JMP 08
    /// 29 : OpCode.DROP
    /// 2A : OpCode.ISNULL
    /// 2B : OpCode.JMP 04
    /// 2D : OpCode.DROP
    /// 2E : OpCode.PUSHF
    /// 2F : OpCode.NOT
    /// 30 : OpCode.JMPIF 04
    /// 32 : OpCode.PUSHF
    /// 33 : OpCode.RET
    /// 34 : OpCode.LDARG1
    /// 35 : OpCode.LDARG0
    /// 36 : OpCode.NOTEQUAL
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("boolNullableNotEqual")]
    public abstract bool? BoolNullableNotEqual(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJBMkCwxGYWxzZSIIDFRydWVK2CYFRQwMVHJ1ZZdA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 13
    /// 08 : OpCode.JMPIF 0B
    /// 0A : OpCode.PUSHDATA1 46616C7365
    /// 11 : OpCode.JMP 08
    /// 13 : OpCode.PUSHDATA1 54727565
    /// 19 : OpCode.DUP
    /// 1A : OpCode.ISNULL
    /// 1B : OpCode.JMPIFNOT 05
    /// 1D : OpCode.DROP
    /// 1E : OpCode.PUSHDATA1
    /// 20 : OpCode.PUSHDATA1 54727565
    /// 26 : OpCode.EQUAL
    /// 27 : OpCode.RET
    /// </remarks>
    [DisplayName("boolNullableToString")]
    public abstract bool? BoolNullableToString(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0D
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.JMPIF 05
    /// 18 : OpCode.PUSHF
    /// 19 : OpCode.JMP 17
    /// 1B : OpCode.LDARG1
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ISNULL
    /// 1F : OpCode.JMPIF 0A
    /// 21 : OpCode.SWAP
    /// 22 : OpCode.DUP
    /// 23 : OpCode.ISNULL
    /// 24 : OpCode.JMPIF 09
    /// 26 : OpCode.NUMEQUAL
    /// 27 : OpCode.JMP 09
    /// 29 : OpCode.DROP
    /// 2A : OpCode.ISNULL
    /// 2B : OpCode.JMP 05
    /// 2D : OpCode.DROP
    /// 2E : OpCode.DROP
    /// 2F : OpCode.PUSHF
    /// 30 : OpCode.JMPIF 04
    /// 32 : OpCode.PUSHF
    /// 33 : OpCode.RET
    /// 34 : OpCode.LDARG1
    /// 35 : OpCode.LDARG0
    /// 36 : OpCode.EQUAL
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("byteNullableEqual")]
    public abstract bool? ByteNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXJAQJQHjYqqpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 04
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.RET
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.NOT
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("byteNullableEqualNull")]
    public abstract bool? ByteNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0E
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIF 05
    /// 19 : OpCode.PUSHF
    /// 1A : OpCode.JMP 18
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.DUP
    /// 1F : OpCode.ISNULL
    /// 20 : OpCode.JMPIF 0A
    /// 22 : OpCode.SWAP
    /// 23 : OpCode.DUP
    /// 24 : OpCode.ISNULL
    /// 25 : OpCode.JMPIF 09
    /// 27 : OpCode.NUMEQUAL
    /// 28 : OpCode.JMP 09
    /// 2A : OpCode.DROP
    /// 2B : OpCode.ISNULL
    /// 2C : OpCode.JMP 05
    /// 2E : OpCode.DROP
    /// 2F : OpCode.DROP
    /// 30 : OpCode.PUSHF
    /// 31 : OpCode.NOT
    /// 32 : OpCode.JMPIF 04
    /// 34 : OpCode.PUSHF
    /// 35 : OpCode.RET
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.LDARG0
    /// 38 : OpCode.NOTEQUAL
    /// 39 : OpCode.RET
    /// </remarks>
    [DisplayName("byteNullableNotEqual")]
    public abstract bool? ByteNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwMMZdA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.CALLT 0000
    /// 0B : OpCode.DUP
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.JMPIFNOT 05
    /// 0F : OpCode.DROP
    /// 10 : OpCode.PUSHDATA1
    /// 12 : OpCode.PUSHDATA1 31
    /// 15 : OpCode.EQUAL
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("byteNullableToString")]
    public abstract bool? ByteNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("h160NullableEqualNull")]
    public abstract bool? H160NullableEqualNull(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIgZ4eZeqJAUJIgZ5eJeqJAQJQHl4mEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 06
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.EQUAL
    /// 0E : OpCode.NOT
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.PUSHF
    /// 12 : OpCode.JMP 06
    /// 14 : OpCode.LDARG1
    /// 15 : OpCode.LDARG0
    /// 16 : OpCode.EQUAL
    /// 17 : OpCode.NOT
    /// 18 : OpCode.JMPIF 04
    /// 1A : OpCode.PUSHF
    /// 1B : OpCode.RET
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.NOTEQUAL
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("h160NullableNotEqual")]
    public abstract bool? H160NullableNotEqual(UInt160? a, UInt160? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIgV4eZckBQkiBXl4lyQECUB5eJdA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.EQUAL
    /// 0E : OpCode.JMPIF 05
    /// 10 : OpCode.PUSHF
    /// 11 : OpCode.JMP 05
    /// 13 : OpCode.LDARG1
    /// 14 : OpCode.LDARG0
    /// 15 : OpCode.EQUAL
    /// 16 : OpCode.JMPIF 04
    /// 18 : OpCode.PUSHF
    /// 19 : OpCode.RET
    /// 1A : OpCode.LDARG1
    /// 1B : OpCode.LDARG0
    /// 1C : OpCode.EQUAL
    /// 1D : OpCode.RET
    /// </remarks>
    [DisplayName("h256NullableEqual")]
    public abstract bool? H256NullableEqual(UInt256? a, UInt256? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIgZ4eZeqJAUJIgZ5eJeqJAQJQHl4mEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 06
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.EQUAL
    /// 0E : OpCode.NOT
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.PUSHF
    /// 12 : OpCode.JMP 06
    /// 14 : OpCode.LDARG1
    /// 15 : OpCode.LDARG0
    /// 16 : OpCode.EQUAL
    /// 17 : OpCode.NOT
    /// 18 : OpCode.JMPIF 04
    /// 1A : OpCode.PUSHF
    /// 1B : OpCode.RET
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.NOTEQUAL
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("h256NullableNotEqual")]
    public abstract bool? H256NullableNotEqual(UInt256? a, UInt256? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0D
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.JMPIF 05
    /// 18 : OpCode.PUSHF
    /// 19 : OpCode.JMP 17
    /// 1B : OpCode.LDARG1
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ISNULL
    /// 1F : OpCode.JMPIF 0A
    /// 21 : OpCode.SWAP
    /// 22 : OpCode.DUP
    /// 23 : OpCode.ISNULL
    /// 24 : OpCode.JMPIF 09
    /// 26 : OpCode.NUMEQUAL
    /// 27 : OpCode.JMP 09
    /// 29 : OpCode.DROP
    /// 2A : OpCode.ISNULL
    /// 2B : OpCode.JMP 05
    /// 2D : OpCode.DROP
    /// 2E : OpCode.DROP
    /// 2F : OpCode.PUSHF
    /// 30 : OpCode.JMPIF 04
    /// 32 : OpCode.PUSHF
    /// 33 : OpCode.RET
    /// 34 : OpCode.LDARG1
    /// 35 : OpCode.LDARG0
    /// 36 : OpCode.EQUAL
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("intNullableEqual")]
    public abstract bool? IntNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXJAQJQHjYqqpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 04
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.RET
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.NOT
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("intNullableEqualNull")]
    public abstract bool? IntNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0E
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIF 05
    /// 19 : OpCode.PUSHF
    /// 1A : OpCode.JMP 18
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.DUP
    /// 1F : OpCode.ISNULL
    /// 20 : OpCode.JMPIF 0A
    /// 22 : OpCode.SWAP
    /// 23 : OpCode.DUP
    /// 24 : OpCode.ISNULL
    /// 25 : OpCode.JMPIF 09
    /// 27 : OpCode.NUMEQUAL
    /// 28 : OpCode.JMP 09
    /// 2A : OpCode.DROP
    /// 2B : OpCode.ISNULL
    /// 2C : OpCode.JMP 05
    /// 2E : OpCode.DROP
    /// 2F : OpCode.DROP
    /// 30 : OpCode.PUSHF
    /// 31 : OpCode.NOT
    /// 32 : OpCode.JMPIF 04
    /// 34 : OpCode.PUSHF
    /// 35 : OpCode.RET
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.LDARG0
    /// 38 : OpCode.NOTEQUAL
    /// 39 : OpCode.RET
    /// </remarks>
    [DisplayName("intNullableNotEqual")]
    public abstract bool? IntNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwMMZdA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.CALLT 0000
    /// 0B : OpCode.DUP
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.JMPIFNOT 05
    /// 0F : OpCode.DROP
    /// 10 : OpCode.PUSHDATA1
    /// 12 : OpCode.PUSHDATA1 31
    /// 15 : OpCode.EQUAL
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("intNullableToString")]
    public abstract bool? IntNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0D
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.JMPIF 05
    /// 18 : OpCode.PUSHF
    /// 19 : OpCode.JMP 17
    /// 1B : OpCode.LDARG1
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ISNULL
    /// 1F : OpCode.JMPIF 0A
    /// 21 : OpCode.SWAP
    /// 22 : OpCode.DUP
    /// 23 : OpCode.ISNULL
    /// 24 : OpCode.JMPIF 09
    /// 26 : OpCode.NUMEQUAL
    /// 27 : OpCode.JMP 09
    /// 29 : OpCode.DROP
    /// 2A : OpCode.ISNULL
    /// 2B : OpCode.JMP 05
    /// 2D : OpCode.DROP
    /// 2E : OpCode.DROP
    /// 2F : OpCode.PUSHF
    /// 30 : OpCode.JMPIF 04
    /// 32 : OpCode.PUSHF
    /// 33 : OpCode.RET
    /// 34 : OpCode.LDARG1
    /// 35 : OpCode.LDARG0
    /// 36 : OpCode.EQUAL
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("longNullableEqual")]
    public abstract bool? LongNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXJAQJQHjYqqpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 04
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.RET
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.NOT
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("longNullableEqualNull")]
    public abstract bool? LongNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0E
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIF 05
    /// 19 : OpCode.PUSHF
    /// 1A : OpCode.JMP 18
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.DUP
    /// 1F : OpCode.ISNULL
    /// 20 : OpCode.JMPIF 0A
    /// 22 : OpCode.SWAP
    /// 23 : OpCode.DUP
    /// 24 : OpCode.ISNULL
    /// 25 : OpCode.JMPIF 09
    /// 27 : OpCode.NUMEQUAL
    /// 28 : OpCode.JMP 09
    /// 2A : OpCode.DROP
    /// 2B : OpCode.ISNULL
    /// 2C : OpCode.JMP 05
    /// 2E : OpCode.DROP
    /// 2F : OpCode.DROP
    /// 30 : OpCode.PUSHF
    /// 31 : OpCode.NOT
    /// 32 : OpCode.JMPIF 04
    /// 34 : OpCode.PUSHF
    /// 35 : OpCode.RET
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.LDARG0
    /// 38 : OpCode.NOTEQUAL
    /// 39 : OpCode.RET
    /// </remarks>
    [DisplayName("longNullableNotEqual")]
    public abstract bool? LongNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwMMZdA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.CALLT 0000
    /// 0B : OpCode.DUP
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.JMPIFNOT 05
    /// 0F : OpCode.DROP
    /// 10 : OpCode.PUSHDATA1
    /// 12 : OpCode.PUSHDATA1 31
    /// 15 : OpCode.EQUAL
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("longNullableToString")]
    public abstract bool? LongNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0D
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.JMPIF 05
    /// 18 : OpCode.PUSHF
    /// 19 : OpCode.JMP 17
    /// 1B : OpCode.LDARG1
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ISNULL
    /// 1F : OpCode.JMPIF 0A
    /// 21 : OpCode.SWAP
    /// 22 : OpCode.DUP
    /// 23 : OpCode.ISNULL
    /// 24 : OpCode.JMPIF 09
    /// 26 : OpCode.NUMEQUAL
    /// 27 : OpCode.JMP 09
    /// 29 : OpCode.DROP
    /// 2A : OpCode.ISNULL
    /// 2B : OpCode.JMP 05
    /// 2D : OpCode.DROP
    /// 2E : OpCode.DROP
    /// 2F : OpCode.PUSHF
    /// 30 : OpCode.JMPIF 04
    /// 32 : OpCode.PUSHF
    /// 33 : OpCode.RET
    /// 34 : OpCode.LDARG1
    /// 35 : OpCode.LDARG0
    /// 36 : OpCode.EQUAL
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("sByteNullableEqual")]
    public abstract bool? SByteNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXJAQJQHjYqqpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 04
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.RET
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.NOT
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("sByteNullableEqualNull")]
    public abstract bool? SByteNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0E
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIF 05
    /// 19 : OpCode.PUSHF
    /// 1A : OpCode.JMP 18
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.DUP
    /// 1F : OpCode.ISNULL
    /// 20 : OpCode.JMPIF 0A
    /// 22 : OpCode.SWAP
    /// 23 : OpCode.DUP
    /// 24 : OpCode.ISNULL
    /// 25 : OpCode.JMPIF 09
    /// 27 : OpCode.NUMEQUAL
    /// 28 : OpCode.JMP 09
    /// 2A : OpCode.DROP
    /// 2B : OpCode.ISNULL
    /// 2C : OpCode.JMP 05
    /// 2E : OpCode.DROP
    /// 2F : OpCode.DROP
    /// 30 : OpCode.PUSHF
    /// 31 : OpCode.NOT
    /// 32 : OpCode.JMPIF 04
    /// 34 : OpCode.PUSHF
    /// 35 : OpCode.RET
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.LDARG0
    /// 38 : OpCode.NOTEQUAL
    /// 39 : OpCode.RET
    /// </remarks>
    [DisplayName("sByteNullableNotEqual")]
    public abstract bool? SByteNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwMMZdA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.CALLT 0000
    /// 0B : OpCode.DUP
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.JMPIFNOT 05
    /// 0F : OpCode.DROP
    /// 10 : OpCode.PUSHDATA1
    /// 12 : OpCode.PUSHDATA1 31
    /// 15 : OpCode.EQUAL
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("sByteNullableToString")]
    public abstract bool? SByteNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0D
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.JMPIF 05
    /// 18 : OpCode.PUSHF
    /// 19 : OpCode.JMP 17
    /// 1B : OpCode.LDARG1
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ISNULL
    /// 1F : OpCode.JMPIF 0A
    /// 21 : OpCode.SWAP
    /// 22 : OpCode.DUP
    /// 23 : OpCode.ISNULL
    /// 24 : OpCode.JMPIF 09
    /// 26 : OpCode.NUMEQUAL
    /// 27 : OpCode.JMP 09
    /// 29 : OpCode.DROP
    /// 2A : OpCode.ISNULL
    /// 2B : OpCode.JMP 05
    /// 2D : OpCode.DROP
    /// 2E : OpCode.DROP
    /// 2F : OpCode.PUSHF
    /// 30 : OpCode.JMPIF 04
    /// 32 : OpCode.PUSHF
    /// 33 : OpCode.RET
    /// 34 : OpCode.LDARG1
    /// 35 : OpCode.LDARG0
    /// 36 : OpCode.EQUAL
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("shortNullableEqual")]
    public abstract bool? ShortNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXJAQJQHjYqqpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 04
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.RET
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.NOT
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("shortNullableEqualNull")]
    public abstract bool? ShortNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0E
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIF 05
    /// 19 : OpCode.PUSHF
    /// 1A : OpCode.JMP 18
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.DUP
    /// 1F : OpCode.ISNULL
    /// 20 : OpCode.JMPIF 0A
    /// 22 : OpCode.SWAP
    /// 23 : OpCode.DUP
    /// 24 : OpCode.ISNULL
    /// 25 : OpCode.JMPIF 09
    /// 27 : OpCode.NUMEQUAL
    /// 28 : OpCode.JMP 09
    /// 2A : OpCode.DROP
    /// 2B : OpCode.ISNULL
    /// 2C : OpCode.JMP 05
    /// 2E : OpCode.DROP
    /// 2F : OpCode.DROP
    /// 30 : OpCode.PUSHF
    /// 31 : OpCode.NOT
    /// 32 : OpCode.JMPIF 04
    /// 34 : OpCode.PUSHF
    /// 35 : OpCode.RET
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.LDARG0
    /// 38 : OpCode.NOTEQUAL
    /// 39 : OpCode.RET
    /// </remarks>
    [DisplayName("shortNullableNotEqual")]
    public abstract bool? ShortNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwMMZdA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.CALLT 0000
    /// 0B : OpCode.DUP
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.JMPIFNOT 05
    /// 0F : OpCode.DROP
    /// 10 : OpCode.PUSHDATA1
    /// 12 : OpCode.PUSHDATA1 31
    /// 15 : OpCode.EQUAL
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("shortNullableToString")]
    public abstract bool? ShortNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0D
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.JMPIF 05
    /// 18 : OpCode.PUSHF
    /// 19 : OpCode.JMP 17
    /// 1B : OpCode.LDARG1
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ISNULL
    /// 1F : OpCode.JMPIF 0A
    /// 21 : OpCode.SWAP
    /// 22 : OpCode.DUP
    /// 23 : OpCode.ISNULL
    /// 24 : OpCode.JMPIF 09
    /// 26 : OpCode.NUMEQUAL
    /// 27 : OpCode.JMP 09
    /// 29 : OpCode.DROP
    /// 2A : OpCode.ISNULL
    /// 2B : OpCode.JMP 05
    /// 2D : OpCode.DROP
    /// 2E : OpCode.DROP
    /// 2F : OpCode.PUSHF
    /// 30 : OpCode.JMPIF 04
    /// 32 : OpCode.PUSHF
    /// 33 : OpCode.RET
    /// 34 : OpCode.LDARG1
    /// 35 : OpCode.LDARG0
    /// 36 : OpCode.EQUAL
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("uIntNullableEqual")]
    public abstract bool? UIntNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXJAQJQHjYqqpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 04
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.RET
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.NOT
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("uIntNullableEqualNull")]
    public abstract bool? UIntNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0E
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIF 05
    /// 19 : OpCode.PUSHF
    /// 1A : OpCode.JMP 18
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.DUP
    /// 1F : OpCode.ISNULL
    /// 20 : OpCode.JMPIF 0A
    /// 22 : OpCode.SWAP
    /// 23 : OpCode.DUP
    /// 24 : OpCode.ISNULL
    /// 25 : OpCode.JMPIF 09
    /// 27 : OpCode.NUMEQUAL
    /// 28 : OpCode.JMP 09
    /// 2A : OpCode.DROP
    /// 2B : OpCode.ISNULL
    /// 2C : OpCode.JMP 05
    /// 2E : OpCode.DROP
    /// 2F : OpCode.DROP
    /// 30 : OpCode.PUSHF
    /// 31 : OpCode.NOT
    /// 32 : OpCode.JMPIF 04
    /// 34 : OpCode.PUSHF
    /// 35 : OpCode.RET
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.LDARG0
    /// 38 : OpCode.NOTEQUAL
    /// 39 : OpCode.RET
    /// </remarks>
    [DisplayName("uIntNullableNotEqual")]
    public abstract bool? UIntNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwMMZdA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.CALLT 0000
    /// 0B : OpCode.DUP
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.JMPIFNOT 05
    /// 0F : OpCode.DROP
    /// 10 : OpCode.PUSHDATA1
    /// 12 : OpCode.PUSHDATA1 31
    /// 15 : OpCode.EQUAL
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("uIntNullableToString")]
    public abstract bool? UIntNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0D
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.JMPIF 05
    /// 18 : OpCode.PUSHF
    /// 19 : OpCode.JMP 17
    /// 1B : OpCode.LDARG1
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ISNULL
    /// 1F : OpCode.JMPIF 0A
    /// 21 : OpCode.SWAP
    /// 22 : OpCode.DUP
    /// 23 : OpCode.ISNULL
    /// 24 : OpCode.JMPIF 09
    /// 26 : OpCode.NUMEQUAL
    /// 27 : OpCode.JMP 09
    /// 29 : OpCode.DROP
    /// 2A : OpCode.ISNULL
    /// 2B : OpCode.JMP 05
    /// 2D : OpCode.DROP
    /// 2E : OpCode.DROP
    /// 2F : OpCode.PUSHF
    /// 30 : OpCode.JMPIF 04
    /// 32 : OpCode.PUSHF
    /// 33 : OpCode.RET
    /// 34 : OpCode.LDARG1
    /// 35 : OpCode.LDARG0
    /// 36 : OpCode.EQUAL
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("uLongNullableEqual")]
    public abstract bool? ULongNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXJAQJQHjYqqpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 04
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.RET
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.NOT
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("uLongNullableEqualNull")]
    public abstract bool? ULongNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0E
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIF 05
    /// 19 : OpCode.PUSHF
    /// 1A : OpCode.JMP 18
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.DUP
    /// 1F : OpCode.ISNULL
    /// 20 : OpCode.JMPIF 0A
    /// 22 : OpCode.SWAP
    /// 23 : OpCode.DUP
    /// 24 : OpCode.ISNULL
    /// 25 : OpCode.JMPIF 09
    /// 27 : OpCode.NUMEQUAL
    /// 28 : OpCode.JMP 09
    /// 2A : OpCode.DROP
    /// 2B : OpCode.ISNULL
    /// 2C : OpCode.JMP 05
    /// 2E : OpCode.DROP
    /// 2F : OpCode.DROP
    /// 30 : OpCode.PUSHF
    /// 31 : OpCode.NOT
    /// 32 : OpCode.JMPIF 04
    /// 34 : OpCode.PUSHF
    /// 35 : OpCode.RET
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.LDARG0
    /// 38 : OpCode.NOTEQUAL
    /// 39 : OpCode.RET
    /// </remarks>
    [DisplayName("uLongNullableNotEqual")]
    public abstract bool? ULongNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwMMZdA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.CALLT 0000
    /// 0B : OpCode.DUP
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.JMPIFNOT 05
    /// 0F : OpCode.DROP
    /// 10 : OpCode.PUSHDATA1
    /// 12 : OpCode.PUSHDATA1 31
    /// 15 : OpCode.EQUAL
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("uLongNullableToString")]
    public abstract bool? ULongNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0D
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.JMPIF 05
    /// 18 : OpCode.PUSHF
    /// 19 : OpCode.JMP 17
    /// 1B : OpCode.LDARG1
    /// 1C : OpCode.LDARG0
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ISNULL
    /// 1F : OpCode.JMPIF 0A
    /// 21 : OpCode.SWAP
    /// 22 : OpCode.DUP
    /// 23 : OpCode.ISNULL
    /// 24 : OpCode.JMPIF 09
    /// 26 : OpCode.NUMEQUAL
    /// 27 : OpCode.JMP 09
    /// 29 : OpCode.DROP
    /// 2A : OpCode.ISNULL
    /// 2B : OpCode.JMP 05
    /// 2D : OpCode.DROP
    /// 2E : OpCode.DROP
    /// 2F : OpCode.PUSHF
    /// 30 : OpCode.JMPIF 04
    /// 32 : OpCode.PUSHF
    /// 33 : OpCode.RET
    /// 34 : OpCode.LDARG1
    /// 35 : OpCode.LDARG0
    /// 36 : OpCode.EQUAL
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("uShortNullableEqual")]
    public abstract bool? UShortNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXJAQJQHjYqqpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.JMPIF 04
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.RET
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.NOT
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("uShortNullableEqualNull")]
    public abstract bool? UShortNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 0E
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 05
    /// 11 : OpCode.NUMEQUAL
    /// 12 : OpCode.JMP 04
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHF
    /// 16 : OpCode.NOT
    /// 17 : OpCode.JMPIF 05
    /// 19 : OpCode.PUSHF
    /// 1A : OpCode.JMP 18
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.DUP
    /// 1F : OpCode.ISNULL
    /// 20 : OpCode.JMPIF 0A
    /// 22 : OpCode.SWAP
    /// 23 : OpCode.DUP
    /// 24 : OpCode.ISNULL
    /// 25 : OpCode.JMPIF 09
    /// 27 : OpCode.NUMEQUAL
    /// 28 : OpCode.JMP 09
    /// 2A : OpCode.DROP
    /// 2B : OpCode.ISNULL
    /// 2C : OpCode.JMP 05
    /// 2E : OpCode.DROP
    /// 2F : OpCode.DROP
    /// 30 : OpCode.PUSHF
    /// 31 : OpCode.NOT
    /// 32 : OpCode.JMPIF 04
    /// 34 : OpCode.PUSHF
    /// 35 : OpCode.RET
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.LDARG0
    /// 38 : OpCode.NOTEQUAL
    /// 39 : OpCode.RET
    /// </remarks>
    [DisplayName("uShortNullableNotEqual")]
    public abstract bool? UShortNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwMMZdA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.CALLT 0000
    /// 0B : OpCode.DUP
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.JMPIFNOT 05
    /// 0F : OpCode.DROP
    /// 10 : OpCode.PUSHDATA1
    /// 12 : OpCode.PUSHDATA1 31
    /// 15 : OpCode.EQUAL
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("uShortNullableToString")]
    public abstract bool? UShortNullableToString(BigInteger? a);

    #endregion
}
