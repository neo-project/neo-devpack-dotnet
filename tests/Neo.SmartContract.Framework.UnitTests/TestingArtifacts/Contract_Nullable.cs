using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Nullable(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Nullable"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""bigIntegerNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""bigIntegerNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":56,""safe"":false},{""name"":""bigIntegerNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":114,""safe"":false},{""name"":""h160NullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Hash160""},{""name"":""b"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":128,""safe"":false},{""name"":""h160NullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":158,""safe"":false},{""name"":""h256NullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Hash256""},{""name"":""b"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":164,""safe"":false},{""name"":""h256NullableEqual"",""parameters"":[{""name"":""a"",""type"":""Hash256""},{""name"":""b"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":194,""safe"":false},{""name"":""byteNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":224,""safe"":false},{""name"":""byteNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":280,""safe"":false},{""name"":""byteNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":338,""safe"":false},{""name"":""sByteNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":352,""safe"":false},{""name"":""sByteNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":408,""safe"":false},{""name"":""sByteNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":466,""safe"":false},{""name"":""shortNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":480,""safe"":false},{""name"":""shortNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":536,""safe"":false},{""name"":""shortNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":594,""safe"":false},{""name"":""uShortNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":608,""safe"":false},{""name"":""uShortNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":664,""safe"":false},{""name"":""uShortNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":722,""safe"":false},{""name"":""intNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":736,""safe"":false},{""name"":""intNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":792,""safe"":false},{""name"":""intNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":850,""safe"":false},{""name"":""uIntNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":864,""safe"":false},{""name"":""uIntNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":920,""safe"":false},{""name"":""uIntNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":978,""safe"":false},{""name"":""longNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":992,""safe"":false},{""name"":""longNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1048,""safe"":false},{""name"":""longNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1106,""safe"":false},{""name"":""uLongNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1120,""safe"":false},{""name"":""uLongNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1176,""safe"":false},{""name"":""uLongNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1234,""safe"":false},{""name"":""boolNullableEqual"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1248,""safe"":false},{""name"":""boolNullableNotEqual"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1302,""safe"":false},{""name"":""boolNullableEqualNull"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1358,""safe"":false},{""name"":""byteNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1372,""safe"":false},{""name"":""sByteNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1395,""safe"":false},{""name"":""shortNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1418,""safe"":false},{""name"":""uShortNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1441,""safe"":false},{""name"":""intNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1464,""safe"":false},{""name"":""uIntNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1487,""safe"":false},{""name"":""longNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1510,""safe"":false},{""name"":""uLongNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1533,""safe"":false},{""name"":""boolNullableToString"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1556,""safe"":false},{""name"":""bigIntegerNullableToString"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1596,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/VMGVwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0BXAAJ4eZgkBQkiDnh5StgkBbMiBEUJqiQFCSIYeXhK2CQKUErYJAmzIglF2CIFRUUJqiQECUB5eJhAVwABeNgkBAlAeNiqqkBXAAJ4eZgkBQkiBXh5mCQFCSIFeXiYJAQJQHl4mEBXAAF42EBXAAJ4eZgkBQkiBXh5mCQFCSIFeXiYJAQJQHl4mEBXAAJ4eZckBQkiBXh5lyQFCSIFeXiXJAQJQHl4l0BXAAJ4eZckBQkiDXh5StgkBbMiBEUJJAUJIhd5eErYJApQStgkCbMiCUXYIgVFRQkkBAlAeXiXQFcAAnh5mCQFCSIOeHlK2CQFsyIERQmqJAUJIhh5eErYJApQStgkCbMiCUXYIgVFRQmqJAQJQHl4mEBXAAF42CQECUB42KqqQFcAAnh5lyQFCSINeHlK2CQFsyIERQkkBQkiF3l4StgkClBK2CQJsyIJRdgiBUVFCSQECUB5eJdAVwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQFcAAXjYJAQJQHjYqqpAVwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0BXAAJ4eZgkBQkiDnh5StgkBbMiBEUJqiQFCSIYeXhK2CQKUErYJAmzIglF2CIFRUUJqiQECUB5eJhAVwABeNgkBAlAeNiqqkBXAAJ4eZckBQkiDXh5StgkBbMiBEUJJAUJIhd5eErYJApQStgkCbMiCUXYIgVFRQkkBAlAeXiXQFcAAnh5mCQFCSIOeHlK2CQFsyIERQmqJAUJIhh5eErYJApQStgkCbMiCUXYIgVFRQmqJAQJQHl4mEBXAAF42CQECUB42KqqQFcAAnh5lyQFCSINeHlK2CQFsyIERQkkBQkiF3l4StgkClBK2CQJsyIJRdgiBUVFCSQECUB5eJdAVwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQFcAAXjYJAQJQHjYqqpAVwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0BXAAJ4eZgkBQkiDnh5StgkBbMiBEUJqiQFCSIYeXhK2CQKUErYJAmzIglF2CIFRUUJqiQECUB5eJhAVwABeNgkBAlAeNiqqkBXAAJ4eZckBQkiDXh5StgkBbMiBEUJJAUJIhd5eErYJApQStgkCbMiCUXYIgVFRQkkBAlAeXiXQFcAAnh5mCQFCSIOeHlK2CQFsyIERQmqJAUJIhh5eErYJApQStgkCbMiCUXYIgVFRQmqJAQJQHl4mEBXAAF42CQECUB42KqqQFcAAnh5lyQFCSINeHlK2CQFsyIERQkkBQkiF3l4StgkClBK2CQJsyIJRdgiBUVFCSQECUB5eJdAVwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQFcAAXjYJAQJQHjYqqpAVwACeHmXJAUJIg14eUrYJAWXIgRFCSQFCSIVeXhK2CQJStgkCZciCEXYIgRFCSQECUB5eJdAVwACeHmYJAUJIg54eUrYJAWXIgRFCaokBQkiFnl4StgkCUrYJAmXIghF2CIERQmqJAQJQHl4mEBXAAF42CQECUB42KqqQFcAAXhK2CQFNwAAStgmBUUMAAwBMZdAVwABeErYJAU3AABK2CYFRQwADAExl0BXAAF4StgkBTcAAErYJgVFDAAMATGXQFcAAXhK2CQFNwAAStgmBUUMAAwBMZdAVwABeErYJAU3AABK2CYFRQwADAExl0BXAAF4StgkBTcAAErYJgVFDAAMATGXQFcAAXhK2CQFNwAAStgmBUUMAAwBMZdAVwABeErYJAU3AABK2CYFRQwADAExl0BXAAF4StgkEyQLDAVGYWxzZSIIDARUcnVlStgmBUUMAAwEVHJ1ZZdAVwABeErYJAU3AABK2CYFRQwADAExl0AyW0fI").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0D [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : JMPIF 05 [2 datoshi]
    /// 18 : PUSHF [1 datoshi]
    /// 19 : JMP 17 [2 datoshi]
    /// 1B : LDARG1 [2 datoshi]
    /// 1C : LDARG0 [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : ISNULL [2 datoshi]
    /// 1F : JMPIF 0A [2 datoshi]
    /// 21 : SWAP [2 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : ISNULL [2 datoshi]
    /// 24 : JMPIF 09 [2 datoshi]
    /// 26 : NUMEQUAL [8 datoshi]
    /// 27 : JMP 09 [2 datoshi]
    /// 29 : DROP [2 datoshi]
    /// 2A : ISNULL [2 datoshi]
    /// 2B : JMP 05 [2 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : PUSHF [1 datoshi]
    /// 30 : JMPIF 04 [2 datoshi]
    /// 32 : PUSHF [1 datoshi]
    /// 33 : RET [0 datoshi]
    /// 34 : LDARG1 [2 datoshi]
    /// 35 : LDARG0 [2 datoshi]
    /// 36 : EQUAL [32 datoshi]
    /// 37 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("bigIntegerNullableEqual")]
    public abstract bool? BigIntegerNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIF 04 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : RET [0 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : NOT [4 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("bigIntegerNullableEqualNull")]
    public abstract bool? BigIntegerNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : NOTEQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0E [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIF 05 [2 datoshi]
    /// 19 : PUSHF [1 datoshi]
    /// 1A : JMP 18 [2 datoshi]
    /// 1C : LDARG1 [2 datoshi]
    /// 1D : LDARG0 [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : ISNULL [2 datoshi]
    /// 20 : JMPIF 0A [2 datoshi]
    /// 22 : SWAP [2 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : ISNULL [2 datoshi]
    /// 25 : JMPIF 09 [2 datoshi]
    /// 27 : NUMEQUAL [8 datoshi]
    /// 28 : JMP 09 [2 datoshi]
    /// 2A : DROP [2 datoshi]
    /// 2B : ISNULL [2 datoshi]
    /// 2C : JMP 05 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : DROP [2 datoshi]
    /// 30 : PUSHF [1 datoshi]
    /// 31 : NOT [4 datoshi]
    /// 32 : JMPIF 04 [2 datoshi]
    /// 34 : PUSHF [1 datoshi]
    /// 35 : RET [0 datoshi]
    /// 36 : LDARG1 [2 datoshi]
    /// 37 : LDARG0 [2 datoshi]
    /// 38 : NOTEQUAL [32 datoshi]
    /// 39 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("bigIntegerNullableNotEqual")]
    public abstract bool? BigIntegerNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : CALLT 0000 [32768 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : ISNULL [2 datoshi]
    /// 0D : JMPIFNOT 05 [2 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : PUSHDATA1 [8 datoshi]
    /// 12 : PUSHDATA1 31 '1' [8 datoshi]
    /// 15 : EQUAL [32 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("bigIntegerNullableToString")]
    public abstract bool? BigIntegerNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWXIgRFCSQFCSIVeXhK2CQJStgkCZciCEXYIgRFCSQECUB5eJdA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0D [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : EQUAL [32 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : JMPIF 05 [2 datoshi]
    /// 18 : PUSHF [1 datoshi]
    /// 19 : JMP 15 [2 datoshi]
    /// 1B : LDARG1 [2 datoshi]
    /// 1C : LDARG0 [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : ISNULL [2 datoshi]
    /// 1F : JMPIF 09 [2 datoshi]
    /// 21 : DUP [2 datoshi]
    /// 22 : ISNULL [2 datoshi]
    /// 23 : JMPIF 09 [2 datoshi]
    /// 25 : EQUAL [32 datoshi]
    /// 26 : JMP 08 [2 datoshi]
    /// 28 : DROP [2 datoshi]
    /// 29 : ISNULL [2 datoshi]
    /// 2A : JMP 04 [2 datoshi]
    /// 2C : DROP [2 datoshi]
    /// 2D : PUSHF [1 datoshi]
    /// 2E : JMPIF 04 [2 datoshi]
    /// 30 : PUSHF [1 datoshi]
    /// 31 : RET [0 datoshi]
    /// 32 : LDARG1 [2 datoshi]
    /// 33 : LDARG0 [2 datoshi]
    /// 34 : EQUAL [32 datoshi]
    /// 35 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("boolNullableEqual")]
    public abstract bool? BoolNullableEqual(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIF 04 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : RET [0 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : NOT [4 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("boolNullableEqualNull")]
    public abstract bool? BoolNullableEqualNull(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWXIgRFCaokBQkiFnl4StgkCUrYJAmXIghF2CIERQmqJAQJQHl4mEA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : NOTEQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0E [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : EQUAL [32 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIF 05 [2 datoshi]
    /// 19 : PUSHF [1 datoshi]
    /// 1A : JMP 16 [2 datoshi]
    /// 1C : LDARG1 [2 datoshi]
    /// 1D : LDARG0 [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : ISNULL [2 datoshi]
    /// 20 : JMPIF 09 [2 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : ISNULL [2 datoshi]
    /// 24 : JMPIF 09 [2 datoshi]
    /// 26 : EQUAL [32 datoshi]
    /// 27 : JMP 08 [2 datoshi]
    /// 29 : DROP [2 datoshi]
    /// 2A : ISNULL [2 datoshi]
    /// 2B : JMP 04 [2 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : PUSHF [1 datoshi]
    /// 2F : NOT [4 datoshi]
    /// 30 : JMPIF 04 [2 datoshi]
    /// 32 : PUSHF [1 datoshi]
    /// 33 : RET [0 datoshi]
    /// 34 : LDARG1 [2 datoshi]
    /// 35 : LDARG0 [2 datoshi]
    /// 36 : NOTEQUAL [32 datoshi]
    /// 37 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("boolNullableNotEqual")]
    public abstract bool? BoolNullableNotEqual(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJBMkCwwFRmFsc2UiCAwEVHJ1ZUrYJgVFDAAMBFRydWWXQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 13 [2 datoshi]
    /// 08 : JMPIF 0B [2 datoshi]
    /// 0A : PUSHDATA1 46616C7365 'False' [8 datoshi]
    /// 11 : JMP 08 [2 datoshi]
    /// 13 : PUSHDATA1 54727565 'True' [8 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : ISNULL [2 datoshi]
    /// 1B : JMPIFNOT 05 [2 datoshi]
    /// 1D : DROP [2 datoshi]
    /// 1E : PUSHDATA1 [8 datoshi]
    /// 20 : PUSHDATA1 54727565 'True' [8 datoshi]
    /// 26 : EQUAL [32 datoshi]
    /// 27 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("boolNullableToString")]
    public abstract bool? BoolNullableToString(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0D [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : JMPIF 05 [2 datoshi]
    /// 18 : PUSHF [1 datoshi]
    /// 19 : JMP 17 [2 datoshi]
    /// 1B : LDARG1 [2 datoshi]
    /// 1C : LDARG0 [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : ISNULL [2 datoshi]
    /// 1F : JMPIF 0A [2 datoshi]
    /// 21 : SWAP [2 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : ISNULL [2 datoshi]
    /// 24 : JMPIF 09 [2 datoshi]
    /// 26 : NUMEQUAL [8 datoshi]
    /// 27 : JMP 09 [2 datoshi]
    /// 29 : DROP [2 datoshi]
    /// 2A : ISNULL [2 datoshi]
    /// 2B : JMP 05 [2 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : PUSHF [1 datoshi]
    /// 30 : JMPIF 04 [2 datoshi]
    /// 32 : PUSHF [1 datoshi]
    /// 33 : RET [0 datoshi]
    /// 34 : LDARG1 [2 datoshi]
    /// 35 : LDARG0 [2 datoshi]
    /// 36 : EQUAL [32 datoshi]
    /// 37 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteNullableEqual")]
    public abstract bool? ByteNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIF 04 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : RET [0 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : NOT [4 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteNullableEqualNull")]
    public abstract bool? ByteNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : NOTEQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0E [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIF 05 [2 datoshi]
    /// 19 : PUSHF [1 datoshi]
    /// 1A : JMP 18 [2 datoshi]
    /// 1C : LDARG1 [2 datoshi]
    /// 1D : LDARG0 [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : ISNULL [2 datoshi]
    /// 20 : JMPIF 0A [2 datoshi]
    /// 22 : SWAP [2 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : ISNULL [2 datoshi]
    /// 25 : JMPIF 09 [2 datoshi]
    /// 27 : NUMEQUAL [8 datoshi]
    /// 28 : JMP 09 [2 datoshi]
    /// 2A : DROP [2 datoshi]
    /// 2B : ISNULL [2 datoshi]
    /// 2C : JMP 05 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : DROP [2 datoshi]
    /// 30 : PUSHF [1 datoshi]
    /// 31 : NOT [4 datoshi]
    /// 32 : JMPIF 04 [2 datoshi]
    /// 34 : PUSHF [1 datoshi]
    /// 35 : RET [0 datoshi]
    /// 36 : LDARG1 [2 datoshi]
    /// 37 : LDARG0 [2 datoshi]
    /// 38 : NOTEQUAL [32 datoshi]
    /// 39 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteNullableNotEqual")]
    public abstract bool? ByteNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : CALLT 0000 [32768 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : ISNULL [2 datoshi]
    /// 0D : JMPIFNOT 05 [2 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : PUSHDATA1 [8 datoshi]
    /// 12 : PUSHDATA1 31 '1' [8 datoshi]
    /// 15 : EQUAL [32 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteNullableToString")]
    public abstract bool? ByteNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("h160NullableEqualNull")]
    public abstract bool? H160NullableEqualNull(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIgV4eZgkBQkiBXl4mCQECUB5eJhA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : NOTEQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 05 [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : NOTEQUAL [32 datoshi]
    /// 0E : JMPIF 05 [2 datoshi]
    /// 10 : PUSHF [1 datoshi]
    /// 11 : JMP 05 [2 datoshi]
    /// 13 : LDARG1 [2 datoshi]
    /// 14 : LDARG0 [2 datoshi]
    /// 15 : NOTEQUAL [32 datoshi]
    /// 16 : JMPIF 04 [2 datoshi]
    /// 18 : PUSHF [1 datoshi]
    /// 19 : RET [0 datoshi]
    /// 1A : LDARG1 [2 datoshi]
    /// 1B : LDARG0 [2 datoshi]
    /// 1C : NOTEQUAL [32 datoshi]
    /// 1D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("h160NullableNotEqual")]
    public abstract bool? H160NullableNotEqual(UInt160? a, UInt160? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIgV4eZckBQkiBXl4lyQECUB5eJdA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 05 [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : EQUAL [32 datoshi]
    /// 0E : JMPIF 05 [2 datoshi]
    /// 10 : PUSHF [1 datoshi]
    /// 11 : JMP 05 [2 datoshi]
    /// 13 : LDARG1 [2 datoshi]
    /// 14 : LDARG0 [2 datoshi]
    /// 15 : EQUAL [32 datoshi]
    /// 16 : JMPIF 04 [2 datoshi]
    /// 18 : PUSHF [1 datoshi]
    /// 19 : RET [0 datoshi]
    /// 1A : LDARG1 [2 datoshi]
    /// 1B : LDARG0 [2 datoshi]
    /// 1C : EQUAL [32 datoshi]
    /// 1D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("h256NullableEqual")]
    public abstract bool? H256NullableEqual(UInt256? a, UInt256? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIgV4eZgkBQkiBXl4mCQECUB5eJhA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : NOTEQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 05 [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : NOTEQUAL [32 datoshi]
    /// 0E : JMPIF 05 [2 datoshi]
    /// 10 : PUSHF [1 datoshi]
    /// 11 : JMP 05 [2 datoshi]
    /// 13 : LDARG1 [2 datoshi]
    /// 14 : LDARG0 [2 datoshi]
    /// 15 : NOTEQUAL [32 datoshi]
    /// 16 : JMPIF 04 [2 datoshi]
    /// 18 : PUSHF [1 datoshi]
    /// 19 : RET [0 datoshi]
    /// 1A : LDARG1 [2 datoshi]
    /// 1B : LDARG0 [2 datoshi]
    /// 1C : NOTEQUAL [32 datoshi]
    /// 1D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("h256NullableNotEqual")]
    public abstract bool? H256NullableNotEqual(UInt256? a, UInt256? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0D [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : JMPIF 05 [2 datoshi]
    /// 18 : PUSHF [1 datoshi]
    /// 19 : JMP 17 [2 datoshi]
    /// 1B : LDARG1 [2 datoshi]
    /// 1C : LDARG0 [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : ISNULL [2 datoshi]
    /// 1F : JMPIF 0A [2 datoshi]
    /// 21 : SWAP [2 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : ISNULL [2 datoshi]
    /// 24 : JMPIF 09 [2 datoshi]
    /// 26 : NUMEQUAL [8 datoshi]
    /// 27 : JMP 09 [2 datoshi]
    /// 29 : DROP [2 datoshi]
    /// 2A : ISNULL [2 datoshi]
    /// 2B : JMP 05 [2 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : PUSHF [1 datoshi]
    /// 30 : JMPIF 04 [2 datoshi]
    /// 32 : PUSHF [1 datoshi]
    /// 33 : RET [0 datoshi]
    /// 34 : LDARG1 [2 datoshi]
    /// 35 : LDARG0 [2 datoshi]
    /// 36 : EQUAL [32 datoshi]
    /// 37 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("intNullableEqual")]
    public abstract bool? IntNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIF 04 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : RET [0 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : NOT [4 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("intNullableEqualNull")]
    public abstract bool? IntNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : NOTEQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0E [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIF 05 [2 datoshi]
    /// 19 : PUSHF [1 datoshi]
    /// 1A : JMP 18 [2 datoshi]
    /// 1C : LDARG1 [2 datoshi]
    /// 1D : LDARG0 [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : ISNULL [2 datoshi]
    /// 20 : JMPIF 0A [2 datoshi]
    /// 22 : SWAP [2 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : ISNULL [2 datoshi]
    /// 25 : JMPIF 09 [2 datoshi]
    /// 27 : NUMEQUAL [8 datoshi]
    /// 28 : JMP 09 [2 datoshi]
    /// 2A : DROP [2 datoshi]
    /// 2B : ISNULL [2 datoshi]
    /// 2C : JMP 05 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : DROP [2 datoshi]
    /// 30 : PUSHF [1 datoshi]
    /// 31 : NOT [4 datoshi]
    /// 32 : JMPIF 04 [2 datoshi]
    /// 34 : PUSHF [1 datoshi]
    /// 35 : RET [0 datoshi]
    /// 36 : LDARG1 [2 datoshi]
    /// 37 : LDARG0 [2 datoshi]
    /// 38 : NOTEQUAL [32 datoshi]
    /// 39 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("intNullableNotEqual")]
    public abstract bool? IntNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : CALLT 0000 [32768 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : ISNULL [2 datoshi]
    /// 0D : JMPIFNOT 05 [2 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : PUSHDATA1 [8 datoshi]
    /// 12 : PUSHDATA1 31 '1' [8 datoshi]
    /// 15 : EQUAL [32 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("intNullableToString")]
    public abstract bool? IntNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0D [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : JMPIF 05 [2 datoshi]
    /// 18 : PUSHF [1 datoshi]
    /// 19 : JMP 17 [2 datoshi]
    /// 1B : LDARG1 [2 datoshi]
    /// 1C : LDARG0 [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : ISNULL [2 datoshi]
    /// 1F : JMPIF 0A [2 datoshi]
    /// 21 : SWAP [2 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : ISNULL [2 datoshi]
    /// 24 : JMPIF 09 [2 datoshi]
    /// 26 : NUMEQUAL [8 datoshi]
    /// 27 : JMP 09 [2 datoshi]
    /// 29 : DROP [2 datoshi]
    /// 2A : ISNULL [2 datoshi]
    /// 2B : JMP 05 [2 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : PUSHF [1 datoshi]
    /// 30 : JMPIF 04 [2 datoshi]
    /// 32 : PUSHF [1 datoshi]
    /// 33 : RET [0 datoshi]
    /// 34 : LDARG1 [2 datoshi]
    /// 35 : LDARG0 [2 datoshi]
    /// 36 : EQUAL [32 datoshi]
    /// 37 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("longNullableEqual")]
    public abstract bool? LongNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIF 04 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : RET [0 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : NOT [4 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("longNullableEqualNull")]
    public abstract bool? LongNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : NOTEQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0E [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIF 05 [2 datoshi]
    /// 19 : PUSHF [1 datoshi]
    /// 1A : JMP 18 [2 datoshi]
    /// 1C : LDARG1 [2 datoshi]
    /// 1D : LDARG0 [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : ISNULL [2 datoshi]
    /// 20 : JMPIF 0A [2 datoshi]
    /// 22 : SWAP [2 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : ISNULL [2 datoshi]
    /// 25 : JMPIF 09 [2 datoshi]
    /// 27 : NUMEQUAL [8 datoshi]
    /// 28 : JMP 09 [2 datoshi]
    /// 2A : DROP [2 datoshi]
    /// 2B : ISNULL [2 datoshi]
    /// 2C : JMP 05 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : DROP [2 datoshi]
    /// 30 : PUSHF [1 datoshi]
    /// 31 : NOT [4 datoshi]
    /// 32 : JMPIF 04 [2 datoshi]
    /// 34 : PUSHF [1 datoshi]
    /// 35 : RET [0 datoshi]
    /// 36 : LDARG1 [2 datoshi]
    /// 37 : LDARG0 [2 datoshi]
    /// 38 : NOTEQUAL [32 datoshi]
    /// 39 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("longNullableNotEqual")]
    public abstract bool? LongNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : CALLT 0000 [32768 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : ISNULL [2 datoshi]
    /// 0D : JMPIFNOT 05 [2 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : PUSHDATA1 [8 datoshi]
    /// 12 : PUSHDATA1 31 '1' [8 datoshi]
    /// 15 : EQUAL [32 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("longNullableToString")]
    public abstract bool? LongNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0D [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : JMPIF 05 [2 datoshi]
    /// 18 : PUSHF [1 datoshi]
    /// 19 : JMP 17 [2 datoshi]
    /// 1B : LDARG1 [2 datoshi]
    /// 1C : LDARG0 [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : ISNULL [2 datoshi]
    /// 1F : JMPIF 0A [2 datoshi]
    /// 21 : SWAP [2 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : ISNULL [2 datoshi]
    /// 24 : JMPIF 09 [2 datoshi]
    /// 26 : NUMEQUAL [8 datoshi]
    /// 27 : JMP 09 [2 datoshi]
    /// 29 : DROP [2 datoshi]
    /// 2A : ISNULL [2 datoshi]
    /// 2B : JMP 05 [2 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : PUSHF [1 datoshi]
    /// 30 : JMPIF 04 [2 datoshi]
    /// 32 : PUSHF [1 datoshi]
    /// 33 : RET [0 datoshi]
    /// 34 : LDARG1 [2 datoshi]
    /// 35 : LDARG0 [2 datoshi]
    /// 36 : EQUAL [32 datoshi]
    /// 37 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sByteNullableEqual")]
    public abstract bool? SByteNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIF 04 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : RET [0 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : NOT [4 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sByteNullableEqualNull")]
    public abstract bool? SByteNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : NOTEQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0E [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIF 05 [2 datoshi]
    /// 19 : PUSHF [1 datoshi]
    /// 1A : JMP 18 [2 datoshi]
    /// 1C : LDARG1 [2 datoshi]
    /// 1D : LDARG0 [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : ISNULL [2 datoshi]
    /// 20 : JMPIF 0A [2 datoshi]
    /// 22 : SWAP [2 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : ISNULL [2 datoshi]
    /// 25 : JMPIF 09 [2 datoshi]
    /// 27 : NUMEQUAL [8 datoshi]
    /// 28 : JMP 09 [2 datoshi]
    /// 2A : DROP [2 datoshi]
    /// 2B : ISNULL [2 datoshi]
    /// 2C : JMP 05 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : DROP [2 datoshi]
    /// 30 : PUSHF [1 datoshi]
    /// 31 : NOT [4 datoshi]
    /// 32 : JMPIF 04 [2 datoshi]
    /// 34 : PUSHF [1 datoshi]
    /// 35 : RET [0 datoshi]
    /// 36 : LDARG1 [2 datoshi]
    /// 37 : LDARG0 [2 datoshi]
    /// 38 : NOTEQUAL [32 datoshi]
    /// 39 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sByteNullableNotEqual")]
    public abstract bool? SByteNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : CALLT 0000 [32768 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : ISNULL [2 datoshi]
    /// 0D : JMPIFNOT 05 [2 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : PUSHDATA1 [8 datoshi]
    /// 12 : PUSHDATA1 31 '1' [8 datoshi]
    /// 15 : EQUAL [32 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("sByteNullableToString")]
    public abstract bool? SByteNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0D [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : JMPIF 05 [2 datoshi]
    /// 18 : PUSHF [1 datoshi]
    /// 19 : JMP 17 [2 datoshi]
    /// 1B : LDARG1 [2 datoshi]
    /// 1C : LDARG0 [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : ISNULL [2 datoshi]
    /// 1F : JMPIF 0A [2 datoshi]
    /// 21 : SWAP [2 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : ISNULL [2 datoshi]
    /// 24 : JMPIF 09 [2 datoshi]
    /// 26 : NUMEQUAL [8 datoshi]
    /// 27 : JMP 09 [2 datoshi]
    /// 29 : DROP [2 datoshi]
    /// 2A : ISNULL [2 datoshi]
    /// 2B : JMP 05 [2 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : PUSHF [1 datoshi]
    /// 30 : JMPIF 04 [2 datoshi]
    /// 32 : PUSHF [1 datoshi]
    /// 33 : RET [0 datoshi]
    /// 34 : LDARG1 [2 datoshi]
    /// 35 : LDARG0 [2 datoshi]
    /// 36 : EQUAL [32 datoshi]
    /// 37 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("shortNullableEqual")]
    public abstract bool? ShortNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIF 04 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : RET [0 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : NOT [4 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("shortNullableEqualNull")]
    public abstract bool? ShortNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : NOTEQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0E [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIF 05 [2 datoshi]
    /// 19 : PUSHF [1 datoshi]
    /// 1A : JMP 18 [2 datoshi]
    /// 1C : LDARG1 [2 datoshi]
    /// 1D : LDARG0 [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : ISNULL [2 datoshi]
    /// 20 : JMPIF 0A [2 datoshi]
    /// 22 : SWAP [2 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : ISNULL [2 datoshi]
    /// 25 : JMPIF 09 [2 datoshi]
    /// 27 : NUMEQUAL [8 datoshi]
    /// 28 : JMP 09 [2 datoshi]
    /// 2A : DROP [2 datoshi]
    /// 2B : ISNULL [2 datoshi]
    /// 2C : JMP 05 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : DROP [2 datoshi]
    /// 30 : PUSHF [1 datoshi]
    /// 31 : NOT [4 datoshi]
    /// 32 : JMPIF 04 [2 datoshi]
    /// 34 : PUSHF [1 datoshi]
    /// 35 : RET [0 datoshi]
    /// 36 : LDARG1 [2 datoshi]
    /// 37 : LDARG0 [2 datoshi]
    /// 38 : NOTEQUAL [32 datoshi]
    /// 39 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("shortNullableNotEqual")]
    public abstract bool? ShortNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : CALLT 0000 [32768 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : ISNULL [2 datoshi]
    /// 0D : JMPIFNOT 05 [2 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : PUSHDATA1 [8 datoshi]
    /// 12 : PUSHDATA1 31 '1' [8 datoshi]
    /// 15 : EQUAL [32 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("shortNullableToString")]
    public abstract bool? ShortNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0D [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : JMPIF 05 [2 datoshi]
    /// 18 : PUSHF [1 datoshi]
    /// 19 : JMP 17 [2 datoshi]
    /// 1B : LDARG1 [2 datoshi]
    /// 1C : LDARG0 [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : ISNULL [2 datoshi]
    /// 1F : JMPIF 0A [2 datoshi]
    /// 21 : SWAP [2 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : ISNULL [2 datoshi]
    /// 24 : JMPIF 09 [2 datoshi]
    /// 26 : NUMEQUAL [8 datoshi]
    /// 27 : JMP 09 [2 datoshi]
    /// 29 : DROP [2 datoshi]
    /// 2A : ISNULL [2 datoshi]
    /// 2B : JMP 05 [2 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : PUSHF [1 datoshi]
    /// 30 : JMPIF 04 [2 datoshi]
    /// 32 : PUSHF [1 datoshi]
    /// 33 : RET [0 datoshi]
    /// 34 : LDARG1 [2 datoshi]
    /// 35 : LDARG0 [2 datoshi]
    /// 36 : EQUAL [32 datoshi]
    /// 37 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uIntNullableEqual")]
    public abstract bool? UIntNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIF 04 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : RET [0 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : NOT [4 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uIntNullableEqualNull")]
    public abstract bool? UIntNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : NOTEQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0E [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIF 05 [2 datoshi]
    /// 19 : PUSHF [1 datoshi]
    /// 1A : JMP 18 [2 datoshi]
    /// 1C : LDARG1 [2 datoshi]
    /// 1D : LDARG0 [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : ISNULL [2 datoshi]
    /// 20 : JMPIF 0A [2 datoshi]
    /// 22 : SWAP [2 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : ISNULL [2 datoshi]
    /// 25 : JMPIF 09 [2 datoshi]
    /// 27 : NUMEQUAL [8 datoshi]
    /// 28 : JMP 09 [2 datoshi]
    /// 2A : DROP [2 datoshi]
    /// 2B : ISNULL [2 datoshi]
    /// 2C : JMP 05 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : DROP [2 datoshi]
    /// 30 : PUSHF [1 datoshi]
    /// 31 : NOT [4 datoshi]
    /// 32 : JMPIF 04 [2 datoshi]
    /// 34 : PUSHF [1 datoshi]
    /// 35 : RET [0 datoshi]
    /// 36 : LDARG1 [2 datoshi]
    /// 37 : LDARG0 [2 datoshi]
    /// 38 : NOTEQUAL [32 datoshi]
    /// 39 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uIntNullableNotEqual")]
    public abstract bool? UIntNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : CALLT 0000 [32768 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : ISNULL [2 datoshi]
    /// 0D : JMPIFNOT 05 [2 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : PUSHDATA1 [8 datoshi]
    /// 12 : PUSHDATA1 31 '1' [8 datoshi]
    /// 15 : EQUAL [32 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uIntNullableToString")]
    public abstract bool? UIntNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0D [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : JMPIF 05 [2 datoshi]
    /// 18 : PUSHF [1 datoshi]
    /// 19 : JMP 17 [2 datoshi]
    /// 1B : LDARG1 [2 datoshi]
    /// 1C : LDARG0 [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : ISNULL [2 datoshi]
    /// 1F : JMPIF 0A [2 datoshi]
    /// 21 : SWAP [2 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : ISNULL [2 datoshi]
    /// 24 : JMPIF 09 [2 datoshi]
    /// 26 : NUMEQUAL [8 datoshi]
    /// 27 : JMP 09 [2 datoshi]
    /// 29 : DROP [2 datoshi]
    /// 2A : ISNULL [2 datoshi]
    /// 2B : JMP 05 [2 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : PUSHF [1 datoshi]
    /// 30 : JMPIF 04 [2 datoshi]
    /// 32 : PUSHF [1 datoshi]
    /// 33 : RET [0 datoshi]
    /// 34 : LDARG1 [2 datoshi]
    /// 35 : LDARG0 [2 datoshi]
    /// 36 : EQUAL [32 datoshi]
    /// 37 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uLongNullableEqual")]
    public abstract bool? ULongNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIF 04 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : RET [0 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : NOT [4 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uLongNullableEqualNull")]
    public abstract bool? ULongNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : NOTEQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0E [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIF 05 [2 datoshi]
    /// 19 : PUSHF [1 datoshi]
    /// 1A : JMP 18 [2 datoshi]
    /// 1C : LDARG1 [2 datoshi]
    /// 1D : LDARG0 [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : ISNULL [2 datoshi]
    /// 20 : JMPIF 0A [2 datoshi]
    /// 22 : SWAP [2 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : ISNULL [2 datoshi]
    /// 25 : JMPIF 09 [2 datoshi]
    /// 27 : NUMEQUAL [8 datoshi]
    /// 28 : JMP 09 [2 datoshi]
    /// 2A : DROP [2 datoshi]
    /// 2B : ISNULL [2 datoshi]
    /// 2C : JMP 05 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : DROP [2 datoshi]
    /// 30 : PUSHF [1 datoshi]
    /// 31 : NOT [4 datoshi]
    /// 32 : JMPIF 04 [2 datoshi]
    /// 34 : PUSHF [1 datoshi]
    /// 35 : RET [0 datoshi]
    /// 36 : LDARG1 [2 datoshi]
    /// 37 : LDARG0 [2 datoshi]
    /// 38 : NOTEQUAL [32 datoshi]
    /// 39 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uLongNullableNotEqual")]
    public abstract bool? ULongNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : CALLT 0000 [32768 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : ISNULL [2 datoshi]
    /// 0D : JMPIFNOT 05 [2 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : PUSHDATA1 [8 datoshi]
    /// 12 : PUSHDATA1 31 '1' [8 datoshi]
    /// 15 : EQUAL [32 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uLongNullableToString")]
    public abstract bool? ULongNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0D [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : JMPIF 05 [2 datoshi]
    /// 18 : PUSHF [1 datoshi]
    /// 19 : JMP 17 [2 datoshi]
    /// 1B : LDARG1 [2 datoshi]
    /// 1C : LDARG0 [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : ISNULL [2 datoshi]
    /// 1F : JMPIF 0A [2 datoshi]
    /// 21 : SWAP [2 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : ISNULL [2 datoshi]
    /// 24 : JMPIF 09 [2 datoshi]
    /// 26 : NUMEQUAL [8 datoshi]
    /// 27 : JMP 09 [2 datoshi]
    /// 29 : DROP [2 datoshi]
    /// 2A : ISNULL [2 datoshi]
    /// 2B : JMP 05 [2 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : PUSHF [1 datoshi]
    /// 30 : JMPIF 04 [2 datoshi]
    /// 32 : PUSHF [1 datoshi]
    /// 33 : RET [0 datoshi]
    /// 34 : LDARG1 [2 datoshi]
    /// 35 : LDARG0 [2 datoshi]
    /// 36 : EQUAL [32 datoshi]
    /// 37 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uShortNullableEqual")]
    public abstract bool? UShortNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIF 04 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : RET [0 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : NOT [4 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uShortNullableEqualNull")]
    public abstract bool? UShortNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : NOTEQUAL [32 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : JMP 0E [2 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 05 [2 datoshi]
    /// 11 : NUMEQUAL [8 datoshi]
    /// 12 : JMP 04 [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHF [1 datoshi]
    /// 16 : NOT [4 datoshi]
    /// 17 : JMPIF 05 [2 datoshi]
    /// 19 : PUSHF [1 datoshi]
    /// 1A : JMP 18 [2 datoshi]
    /// 1C : LDARG1 [2 datoshi]
    /// 1D : LDARG0 [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : ISNULL [2 datoshi]
    /// 20 : JMPIF 0A [2 datoshi]
    /// 22 : SWAP [2 datoshi]
    /// 23 : DUP [2 datoshi]
    /// 24 : ISNULL [2 datoshi]
    /// 25 : JMPIF 09 [2 datoshi]
    /// 27 : NUMEQUAL [8 datoshi]
    /// 28 : JMP 09 [2 datoshi]
    /// 2A : DROP [2 datoshi]
    /// 2B : ISNULL [2 datoshi]
    /// 2C : JMP 05 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : DROP [2 datoshi]
    /// 30 : PUSHF [1 datoshi]
    /// 31 : NOT [4 datoshi]
    /// 32 : JMPIF 04 [2 datoshi]
    /// 34 : PUSHF [1 datoshi]
    /// 35 : RET [0 datoshi]
    /// 36 : LDARG1 [2 datoshi]
    /// 37 : LDARG0 [2 datoshi]
    /// 38 : NOTEQUAL [32 datoshi]
    /// 39 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uShortNullableNotEqual")]
    public abstract bool? UShortNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : CALLT 0000 [32768 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : ISNULL [2 datoshi]
    /// 0D : JMPIFNOT 05 [2 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : PUSHDATA1 [8 datoshi]
    /// 12 : PUSHDATA1 31 '1' [8 datoshi]
    /// 15 : EQUAL [32 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("uShortNullableToString")]
    public abstract bool? UShortNullableToString(BigInteger? a);

    #endregion
}
