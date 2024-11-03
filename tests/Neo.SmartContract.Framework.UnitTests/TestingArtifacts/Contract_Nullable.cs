using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/VMGVwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0BXAAJ4eZgkBQkiDnh5StgkBbMiBEUJqiQFCSIYeXhK2CQKUErYJAmzIglF2CIFRUUJqiQECUB5eJhAVwABeNgkBAlAeNiqqkBXAAJ4eZgkBQkiBXh5mCQFCSIFeXiYJAQJQHl4mEBXAAF42EBXAAJ4eZgkBQkiBXh5mCQFCSIFeXiYJAQJQHl4mEBXAAJ4eZckBQkiBXh5lyQFCSIFeXiXJAQJQHl4l0BXAAJ4eZckBQkiDXh5StgkBbMiBEUJJAUJIhd5eErYJApQStgkCbMiCUXYIgVFRQkkBAlAeXiXQFcAAnh5mCQFCSIOeHlK2CQFsyIERQmqJAUJIhh5eErYJApQStgkCbMiCUXYIgVFRQmqJAQJQHl4mEBXAAF42CQECUB42KqqQFcAAnh5lyQFCSINeHlK2CQFsyIERQkkBQkiF3l4StgkClBK2CQJsyIJRdgiBUVFCSQECUB5eJdAVwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQFcAAXjYJAQJQHjYqqpAVwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0BXAAJ4eZgkBQkiDnh5StgkBbMiBEUJqiQFCSIYeXhK2CQKUErYJAmzIglF2CIFRUUJqiQECUB5eJhAVwABeNgkBAlAeNiqqkBXAAJ4eZckBQkiDXh5StgkBbMiBEUJJAUJIhd5eErYJApQStgkCbMiCUXYIgVFRQkkBAlAeXiXQFcAAnh5mCQFCSIOeHlK2CQFsyIERQmqJAUJIhh5eErYJApQStgkCbMiCUXYIgVFRQmqJAQJQHl4mEBXAAF42CQECUB42KqqQFcAAnh5lyQFCSINeHlK2CQFsyIERQkkBQkiF3l4StgkClBK2CQJsyIJRdgiBUVFCSQECUB5eJdAVwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQFcAAXjYJAQJQHjYqqpAVwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0BXAAJ4eZgkBQkiDnh5StgkBbMiBEUJqiQFCSIYeXhK2CQKUErYJAmzIglF2CIFRUUJqiQECUB5eJhAVwABeNgkBAlAeNiqqkBXAAJ4eZckBQkiDXh5StgkBbMiBEUJJAUJIhd5eErYJApQStgkCbMiCUXYIgVFRQkkBAlAeXiXQFcAAnh5mCQFCSIOeHlK2CQFsyIERQmqJAUJIhh5eErYJApQStgkCbMiCUXYIgVFRQmqJAQJQHl4mEBXAAF42CQECUB42KqqQFcAAnh5lyQFCSINeHlK2CQFsyIERQkkBQkiF3l4StgkClBK2CQJsyIJRdgiBUVFCSQECUB5eJdAVwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQFcAAXjYJAQJQHjYqqpAVwACeHmXJAUJIg14eUrYJAWXIgRFCSQFCSIVeXhK2CQJStgkCZciCEXYIgRFCSQECUB5eJdAVwACeHmYJAUJIg54eUrYJAWXIgRFCaokBQkiFnl4StgkCUrYJAmXIghF2CIERQmqJAQJQHl4mEBXAAF42CQECUB42KqqQFcAAXhK2CQFNwAAStgmBUUMAAwBMZdAVwABeErYJAU3AABK2CYFRQwADAExl0BXAAF4StgkBTcAAErYJgVFDAAMATGXQFcAAXhK2CQFNwAAStgmBUUMAAwBMZdAVwABeErYJAU3AABK2CYFRQwADAExl0BXAAF4StgkBTcAAErYJgVFDAAMATGXQFcAAXhK2CQFNwAAStgmBUUMAAwBMZdAVwABeErYJAU3AABK2CYFRQwADAExl0BXAAF4StgkEyQLDAVGYWxzZSIIDARUcnVlStgmBUUMAAwEVHJ1ZZdAVwABeErYJAU3AABK2CYFRQwADAExl0AyW0fI"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.EQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0D [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.JMPIF 05 [2 datoshi]
    /// 18 : OpCode.PUSHF [1 datoshi]
    /// 19 : OpCode.JMP 17 [2 datoshi]
    /// 1B : OpCode.LDARG1 [2 datoshi]
    /// 1C : OpCode.LDARG0 [2 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.ISNULL [2 datoshi]
    /// 1F : OpCode.JMPIF 0A [2 datoshi]
    /// 21 : OpCode.SWAP [2 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.ISNULL [2 datoshi]
    /// 24 : OpCode.JMPIF 09 [2 datoshi]
    /// 26 : OpCode.NUMEQUAL [8 datoshi]
    /// 27 : OpCode.JMP 09 [2 datoshi]
    /// 29 : OpCode.DROP [2 datoshi]
    /// 2A : OpCode.ISNULL [2 datoshi]
    /// 2B : OpCode.JMP 05 [2 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.PUSHF [1 datoshi]
    /// 30 : OpCode.JMPIF 04 [2 datoshi]
    /// 32 : OpCode.PUSHF [1 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// 34 : OpCode.LDARG1 [2 datoshi]
    /// 35 : OpCode.LDARG0 [2 datoshi]
    /// 36 : OpCode.EQUAL [32 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("bigIntegerNullableEqual")]
    public abstract bool? BigIntegerNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIF 04 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.NOT [4 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("bigIntegerNullableEqualNull")]
    public abstract bool? BigIntegerNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.NOTEQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0E [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIF 05 [2 datoshi]
    /// 19 : OpCode.PUSHF [1 datoshi]
    /// 1A : OpCode.JMP 18 [2 datoshi]
    /// 1C : OpCode.LDARG1 [2 datoshi]
    /// 1D : OpCode.LDARG0 [2 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.ISNULL [2 datoshi]
    /// 20 : OpCode.JMPIF 0A [2 datoshi]
    /// 22 : OpCode.SWAP [2 datoshi]
    /// 23 : OpCode.DUP [2 datoshi]
    /// 24 : OpCode.ISNULL [2 datoshi]
    /// 25 : OpCode.JMPIF 09 [2 datoshi]
    /// 27 : OpCode.NUMEQUAL [8 datoshi]
    /// 28 : OpCode.JMP 09 [2 datoshi]
    /// 2A : OpCode.DROP [2 datoshi]
    /// 2B : OpCode.ISNULL [2 datoshi]
    /// 2C : OpCode.JMP 05 [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.DROP [2 datoshi]
    /// 30 : OpCode.PUSHF [1 datoshi]
    /// 31 : OpCode.NOT [4 datoshi]
    /// 32 : OpCode.JMPIF 04 [2 datoshi]
    /// 34 : OpCode.PUSHF [1 datoshi]
    /// 35 : OpCode.RET [0 datoshi]
    /// 36 : OpCode.LDARG1 [2 datoshi]
    /// 37 : OpCode.LDARG0 [2 datoshi]
    /// 38 : OpCode.NOTEQUAL [32 datoshi]
    /// 39 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("bigIntegerNullableNotEqual")]
    public abstract bool? BigIntegerNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.ISNULL [2 datoshi]
    /// 0D : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0F : OpCode.DROP [2 datoshi]
    /// 10 : OpCode.PUSHDATA1 [8 datoshi]
    /// 12 : OpCode.PUSHDATA1 31 [8 datoshi]
    /// 15 : OpCode.EQUAL [32 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("bigIntegerNullableToString")]
    public abstract bool? BigIntegerNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWXIgRFCSQFCSIVeXhK2CQJStgkCZciCEXYIgRFCSQECUB5eJdA
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.EQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0D [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.EQUAL [32 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.JMPIF 05 [2 datoshi]
    /// 18 : OpCode.PUSHF [1 datoshi]
    /// 19 : OpCode.JMP 15 [2 datoshi]
    /// 1B : OpCode.LDARG1 [2 datoshi]
    /// 1C : OpCode.LDARG0 [2 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.ISNULL [2 datoshi]
    /// 1F : OpCode.JMPIF 09 [2 datoshi]
    /// 21 : OpCode.DUP [2 datoshi]
    /// 22 : OpCode.ISNULL [2 datoshi]
    /// 23 : OpCode.JMPIF 09 [2 datoshi]
    /// 25 : OpCode.EQUAL [32 datoshi]
    /// 26 : OpCode.JMP 08 [2 datoshi]
    /// 28 : OpCode.DROP [2 datoshi]
    /// 29 : OpCode.ISNULL [2 datoshi]
    /// 2A : OpCode.JMP 04 [2 datoshi]
    /// 2C : OpCode.DROP [2 datoshi]
    /// 2D : OpCode.PUSHF [1 datoshi]
    /// 2E : OpCode.JMPIF 04 [2 datoshi]
    /// 30 : OpCode.PUSHF [1 datoshi]
    /// 31 : OpCode.RET [0 datoshi]
    /// 32 : OpCode.LDARG1 [2 datoshi]
    /// 33 : OpCode.LDARG0 [2 datoshi]
    /// 34 : OpCode.EQUAL [32 datoshi]
    /// 35 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("boolNullableEqual")]
    public abstract bool? BoolNullableEqual(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIF 04 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.NOT [4 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("boolNullableEqualNull")]
    public abstract bool? BoolNullableEqualNull(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWXIgRFCaokBQkiFnl4StgkCUrYJAmXIghF2CIERQmqJAQJQHl4mEA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.NOTEQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0E [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.EQUAL [32 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIF 05 [2 datoshi]
    /// 19 : OpCode.PUSHF [1 datoshi]
    /// 1A : OpCode.JMP 16 [2 datoshi]
    /// 1C : OpCode.LDARG1 [2 datoshi]
    /// 1D : OpCode.LDARG0 [2 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.ISNULL [2 datoshi]
    /// 20 : OpCode.JMPIF 09 [2 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.ISNULL [2 datoshi]
    /// 24 : OpCode.JMPIF 09 [2 datoshi]
    /// 26 : OpCode.EQUAL [32 datoshi]
    /// 27 : OpCode.JMP 08 [2 datoshi]
    /// 29 : OpCode.DROP [2 datoshi]
    /// 2A : OpCode.ISNULL [2 datoshi]
    /// 2B : OpCode.JMP 04 [2 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.PUSHF [1 datoshi]
    /// 2F : OpCode.NOT [4 datoshi]
    /// 30 : OpCode.JMPIF 04 [2 datoshi]
    /// 32 : OpCode.PUSHF [1 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// 34 : OpCode.LDARG1 [2 datoshi]
    /// 35 : OpCode.LDARG0 [2 datoshi]
    /// 36 : OpCode.NOTEQUAL [32 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("boolNullableNotEqual")]
    public abstract bool? BoolNullableNotEqual(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJBMkCwwFRmFsc2UiCAwEVHJ1ZUrYJgVFDAAMBFRydWWXQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 13 [2 datoshi]
    /// 08 : OpCode.JMPIF 0B [2 datoshi]
    /// 0A : OpCode.PUSHDATA1 46616C7365 [8 datoshi]
    /// 11 : OpCode.JMP 08 [2 datoshi]
    /// 13 : OpCode.PUSHDATA1 54727565 [8 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.ISNULL [2 datoshi]
    /// 1B : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 1D : OpCode.DROP [2 datoshi]
    /// 1E : OpCode.PUSHDATA1 [8 datoshi]
    /// 20 : OpCode.PUSHDATA1 54727565 [8 datoshi]
    /// 26 : OpCode.EQUAL [32 datoshi]
    /// 27 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("boolNullableToString")]
    public abstract bool? BoolNullableToString(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.EQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0D [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.JMPIF 05 [2 datoshi]
    /// 18 : OpCode.PUSHF [1 datoshi]
    /// 19 : OpCode.JMP 17 [2 datoshi]
    /// 1B : OpCode.LDARG1 [2 datoshi]
    /// 1C : OpCode.LDARG0 [2 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.ISNULL [2 datoshi]
    /// 1F : OpCode.JMPIF 0A [2 datoshi]
    /// 21 : OpCode.SWAP [2 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.ISNULL [2 datoshi]
    /// 24 : OpCode.JMPIF 09 [2 datoshi]
    /// 26 : OpCode.NUMEQUAL [8 datoshi]
    /// 27 : OpCode.JMP 09 [2 datoshi]
    /// 29 : OpCode.DROP [2 datoshi]
    /// 2A : OpCode.ISNULL [2 datoshi]
    /// 2B : OpCode.JMP 05 [2 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.PUSHF [1 datoshi]
    /// 30 : OpCode.JMPIF 04 [2 datoshi]
    /// 32 : OpCode.PUSHF [1 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// 34 : OpCode.LDARG1 [2 datoshi]
    /// 35 : OpCode.LDARG0 [2 datoshi]
    /// 36 : OpCode.EQUAL [32 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteNullableEqual")]
    public abstract bool? ByteNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIF 04 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.NOT [4 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteNullableEqualNull")]
    public abstract bool? ByteNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.NOTEQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0E [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIF 05 [2 datoshi]
    /// 19 : OpCode.PUSHF [1 datoshi]
    /// 1A : OpCode.JMP 18 [2 datoshi]
    /// 1C : OpCode.LDARG1 [2 datoshi]
    /// 1D : OpCode.LDARG0 [2 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.ISNULL [2 datoshi]
    /// 20 : OpCode.JMPIF 0A [2 datoshi]
    /// 22 : OpCode.SWAP [2 datoshi]
    /// 23 : OpCode.DUP [2 datoshi]
    /// 24 : OpCode.ISNULL [2 datoshi]
    /// 25 : OpCode.JMPIF 09 [2 datoshi]
    /// 27 : OpCode.NUMEQUAL [8 datoshi]
    /// 28 : OpCode.JMP 09 [2 datoshi]
    /// 2A : OpCode.DROP [2 datoshi]
    /// 2B : OpCode.ISNULL [2 datoshi]
    /// 2C : OpCode.JMP 05 [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.DROP [2 datoshi]
    /// 30 : OpCode.PUSHF [1 datoshi]
    /// 31 : OpCode.NOT [4 datoshi]
    /// 32 : OpCode.JMPIF 04 [2 datoshi]
    /// 34 : OpCode.PUSHF [1 datoshi]
    /// 35 : OpCode.RET [0 datoshi]
    /// 36 : OpCode.LDARG1 [2 datoshi]
    /// 37 : OpCode.LDARG0 [2 datoshi]
    /// 38 : OpCode.NOTEQUAL [32 datoshi]
    /// 39 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteNullableNotEqual")]
    public abstract bool? ByteNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.ISNULL [2 datoshi]
    /// 0D : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0F : OpCode.DROP [2 datoshi]
    /// 10 : OpCode.PUSHDATA1 [8 datoshi]
    /// 12 : OpCode.PUSHDATA1 31 [8 datoshi]
    /// 15 : OpCode.EQUAL [32 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("byteNullableToString")]
    public abstract bool? ByteNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNhA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("h160NullableEqualNull")]
    public abstract bool? H160NullableEqualNull(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIgV4eZgkBQkiBXl4mCQECUB5eJhA
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.NOTEQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 05 [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.NOTEQUAL [32 datoshi]
    /// 0E : OpCode.JMPIF 05 [2 datoshi]
    /// 10 : OpCode.PUSHF [1 datoshi]
    /// 11 : OpCode.JMP 05 [2 datoshi]
    /// 13 : OpCode.LDARG1 [2 datoshi]
    /// 14 : OpCode.LDARG0 [2 datoshi]
    /// 15 : OpCode.NOTEQUAL [32 datoshi]
    /// 16 : OpCode.JMPIF 04 [2 datoshi]
    /// 18 : OpCode.PUSHF [1 datoshi]
    /// 19 : OpCode.RET [0 datoshi]
    /// 1A : OpCode.LDARG1 [2 datoshi]
    /// 1B : OpCode.LDARG0 [2 datoshi]
    /// 1C : OpCode.NOTEQUAL [32 datoshi]
    /// 1D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("h160NullableNotEqual")]
    public abstract bool? H160NullableNotEqual(UInt160? a, UInt160? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIgV4eZckBQkiBXl4lyQECUB5eJdA
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.EQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 05 [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.EQUAL [32 datoshi]
    /// 0E : OpCode.JMPIF 05 [2 datoshi]
    /// 10 : OpCode.PUSHF [1 datoshi]
    /// 11 : OpCode.JMP 05 [2 datoshi]
    /// 13 : OpCode.LDARG1 [2 datoshi]
    /// 14 : OpCode.LDARG0 [2 datoshi]
    /// 15 : OpCode.EQUAL [32 datoshi]
    /// 16 : OpCode.JMPIF 04 [2 datoshi]
    /// 18 : OpCode.PUSHF [1 datoshi]
    /// 19 : OpCode.RET [0 datoshi]
    /// 1A : OpCode.LDARG1 [2 datoshi]
    /// 1B : OpCode.LDARG0 [2 datoshi]
    /// 1C : OpCode.EQUAL [32 datoshi]
    /// 1D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("h256NullableEqual")]
    public abstract bool? H256NullableEqual(UInt256? a, UInt256? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIgV4eZgkBQkiBXl4mCQECUB5eJhA
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.NOTEQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 05 [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.NOTEQUAL [32 datoshi]
    /// 0E : OpCode.JMPIF 05 [2 datoshi]
    /// 10 : OpCode.PUSHF [1 datoshi]
    /// 11 : OpCode.JMP 05 [2 datoshi]
    /// 13 : OpCode.LDARG1 [2 datoshi]
    /// 14 : OpCode.LDARG0 [2 datoshi]
    /// 15 : OpCode.NOTEQUAL [32 datoshi]
    /// 16 : OpCode.JMPIF 04 [2 datoshi]
    /// 18 : OpCode.PUSHF [1 datoshi]
    /// 19 : OpCode.RET [0 datoshi]
    /// 1A : OpCode.LDARG1 [2 datoshi]
    /// 1B : OpCode.LDARG0 [2 datoshi]
    /// 1C : OpCode.NOTEQUAL [32 datoshi]
    /// 1D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("h256NullableNotEqual")]
    public abstract bool? H256NullableNotEqual(UInt256? a, UInt256? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.EQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0D [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.JMPIF 05 [2 datoshi]
    /// 18 : OpCode.PUSHF [1 datoshi]
    /// 19 : OpCode.JMP 17 [2 datoshi]
    /// 1B : OpCode.LDARG1 [2 datoshi]
    /// 1C : OpCode.LDARG0 [2 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.ISNULL [2 datoshi]
    /// 1F : OpCode.JMPIF 0A [2 datoshi]
    /// 21 : OpCode.SWAP [2 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.ISNULL [2 datoshi]
    /// 24 : OpCode.JMPIF 09 [2 datoshi]
    /// 26 : OpCode.NUMEQUAL [8 datoshi]
    /// 27 : OpCode.JMP 09 [2 datoshi]
    /// 29 : OpCode.DROP [2 datoshi]
    /// 2A : OpCode.ISNULL [2 datoshi]
    /// 2B : OpCode.JMP 05 [2 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.PUSHF [1 datoshi]
    /// 30 : OpCode.JMPIF 04 [2 datoshi]
    /// 32 : OpCode.PUSHF [1 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// 34 : OpCode.LDARG1 [2 datoshi]
    /// 35 : OpCode.LDARG0 [2 datoshi]
    /// 36 : OpCode.EQUAL [32 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("intNullableEqual")]
    public abstract bool? IntNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIF 04 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.NOT [4 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("intNullableEqualNull")]
    public abstract bool? IntNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.NOTEQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0E [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIF 05 [2 datoshi]
    /// 19 : OpCode.PUSHF [1 datoshi]
    /// 1A : OpCode.JMP 18 [2 datoshi]
    /// 1C : OpCode.LDARG1 [2 datoshi]
    /// 1D : OpCode.LDARG0 [2 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.ISNULL [2 datoshi]
    /// 20 : OpCode.JMPIF 0A [2 datoshi]
    /// 22 : OpCode.SWAP [2 datoshi]
    /// 23 : OpCode.DUP [2 datoshi]
    /// 24 : OpCode.ISNULL [2 datoshi]
    /// 25 : OpCode.JMPIF 09 [2 datoshi]
    /// 27 : OpCode.NUMEQUAL [8 datoshi]
    /// 28 : OpCode.JMP 09 [2 datoshi]
    /// 2A : OpCode.DROP [2 datoshi]
    /// 2B : OpCode.ISNULL [2 datoshi]
    /// 2C : OpCode.JMP 05 [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.DROP [2 datoshi]
    /// 30 : OpCode.PUSHF [1 datoshi]
    /// 31 : OpCode.NOT [4 datoshi]
    /// 32 : OpCode.JMPIF 04 [2 datoshi]
    /// 34 : OpCode.PUSHF [1 datoshi]
    /// 35 : OpCode.RET [0 datoshi]
    /// 36 : OpCode.LDARG1 [2 datoshi]
    /// 37 : OpCode.LDARG0 [2 datoshi]
    /// 38 : OpCode.NOTEQUAL [32 datoshi]
    /// 39 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("intNullableNotEqual")]
    public abstract bool? IntNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.ISNULL [2 datoshi]
    /// 0D : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0F : OpCode.DROP [2 datoshi]
    /// 10 : OpCode.PUSHDATA1 [8 datoshi]
    /// 12 : OpCode.PUSHDATA1 31 [8 datoshi]
    /// 15 : OpCode.EQUAL [32 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("intNullableToString")]
    public abstract bool? IntNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.EQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0D [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.JMPIF 05 [2 datoshi]
    /// 18 : OpCode.PUSHF [1 datoshi]
    /// 19 : OpCode.JMP 17 [2 datoshi]
    /// 1B : OpCode.LDARG1 [2 datoshi]
    /// 1C : OpCode.LDARG0 [2 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.ISNULL [2 datoshi]
    /// 1F : OpCode.JMPIF 0A [2 datoshi]
    /// 21 : OpCode.SWAP [2 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.ISNULL [2 datoshi]
    /// 24 : OpCode.JMPIF 09 [2 datoshi]
    /// 26 : OpCode.NUMEQUAL [8 datoshi]
    /// 27 : OpCode.JMP 09 [2 datoshi]
    /// 29 : OpCode.DROP [2 datoshi]
    /// 2A : OpCode.ISNULL [2 datoshi]
    /// 2B : OpCode.JMP 05 [2 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.PUSHF [1 datoshi]
    /// 30 : OpCode.JMPIF 04 [2 datoshi]
    /// 32 : OpCode.PUSHF [1 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// 34 : OpCode.LDARG1 [2 datoshi]
    /// 35 : OpCode.LDARG0 [2 datoshi]
    /// 36 : OpCode.EQUAL [32 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("longNullableEqual")]
    public abstract bool? LongNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIF 04 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.NOT [4 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("longNullableEqualNull")]
    public abstract bool? LongNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.NOTEQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0E [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIF 05 [2 datoshi]
    /// 19 : OpCode.PUSHF [1 datoshi]
    /// 1A : OpCode.JMP 18 [2 datoshi]
    /// 1C : OpCode.LDARG1 [2 datoshi]
    /// 1D : OpCode.LDARG0 [2 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.ISNULL [2 datoshi]
    /// 20 : OpCode.JMPIF 0A [2 datoshi]
    /// 22 : OpCode.SWAP [2 datoshi]
    /// 23 : OpCode.DUP [2 datoshi]
    /// 24 : OpCode.ISNULL [2 datoshi]
    /// 25 : OpCode.JMPIF 09 [2 datoshi]
    /// 27 : OpCode.NUMEQUAL [8 datoshi]
    /// 28 : OpCode.JMP 09 [2 datoshi]
    /// 2A : OpCode.DROP [2 datoshi]
    /// 2B : OpCode.ISNULL [2 datoshi]
    /// 2C : OpCode.JMP 05 [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.DROP [2 datoshi]
    /// 30 : OpCode.PUSHF [1 datoshi]
    /// 31 : OpCode.NOT [4 datoshi]
    /// 32 : OpCode.JMPIF 04 [2 datoshi]
    /// 34 : OpCode.PUSHF [1 datoshi]
    /// 35 : OpCode.RET [0 datoshi]
    /// 36 : OpCode.LDARG1 [2 datoshi]
    /// 37 : OpCode.LDARG0 [2 datoshi]
    /// 38 : OpCode.NOTEQUAL [32 datoshi]
    /// 39 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("longNullableNotEqual")]
    public abstract bool? LongNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.ISNULL [2 datoshi]
    /// 0D : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0F : OpCode.DROP [2 datoshi]
    /// 10 : OpCode.PUSHDATA1 [8 datoshi]
    /// 12 : OpCode.PUSHDATA1 31 [8 datoshi]
    /// 15 : OpCode.EQUAL [32 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("longNullableToString")]
    public abstract bool? LongNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.EQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0D [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.JMPIF 05 [2 datoshi]
    /// 18 : OpCode.PUSHF [1 datoshi]
    /// 19 : OpCode.JMP 17 [2 datoshi]
    /// 1B : OpCode.LDARG1 [2 datoshi]
    /// 1C : OpCode.LDARG0 [2 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.ISNULL [2 datoshi]
    /// 1F : OpCode.JMPIF 0A [2 datoshi]
    /// 21 : OpCode.SWAP [2 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.ISNULL [2 datoshi]
    /// 24 : OpCode.JMPIF 09 [2 datoshi]
    /// 26 : OpCode.NUMEQUAL [8 datoshi]
    /// 27 : OpCode.JMP 09 [2 datoshi]
    /// 29 : OpCode.DROP [2 datoshi]
    /// 2A : OpCode.ISNULL [2 datoshi]
    /// 2B : OpCode.JMP 05 [2 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.PUSHF [1 datoshi]
    /// 30 : OpCode.JMPIF 04 [2 datoshi]
    /// 32 : OpCode.PUSHF [1 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// 34 : OpCode.LDARG1 [2 datoshi]
    /// 35 : OpCode.LDARG0 [2 datoshi]
    /// 36 : OpCode.EQUAL [32 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sByteNullableEqual")]
    public abstract bool? SByteNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIF 04 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.NOT [4 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sByteNullableEqualNull")]
    public abstract bool? SByteNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.NOTEQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0E [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIF 05 [2 datoshi]
    /// 19 : OpCode.PUSHF [1 datoshi]
    /// 1A : OpCode.JMP 18 [2 datoshi]
    /// 1C : OpCode.LDARG1 [2 datoshi]
    /// 1D : OpCode.LDARG0 [2 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.ISNULL [2 datoshi]
    /// 20 : OpCode.JMPIF 0A [2 datoshi]
    /// 22 : OpCode.SWAP [2 datoshi]
    /// 23 : OpCode.DUP [2 datoshi]
    /// 24 : OpCode.ISNULL [2 datoshi]
    /// 25 : OpCode.JMPIF 09 [2 datoshi]
    /// 27 : OpCode.NUMEQUAL [8 datoshi]
    /// 28 : OpCode.JMP 09 [2 datoshi]
    /// 2A : OpCode.DROP [2 datoshi]
    /// 2B : OpCode.ISNULL [2 datoshi]
    /// 2C : OpCode.JMP 05 [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.DROP [2 datoshi]
    /// 30 : OpCode.PUSHF [1 datoshi]
    /// 31 : OpCode.NOT [4 datoshi]
    /// 32 : OpCode.JMPIF 04 [2 datoshi]
    /// 34 : OpCode.PUSHF [1 datoshi]
    /// 35 : OpCode.RET [0 datoshi]
    /// 36 : OpCode.LDARG1 [2 datoshi]
    /// 37 : OpCode.LDARG0 [2 datoshi]
    /// 38 : OpCode.NOTEQUAL [32 datoshi]
    /// 39 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sByteNullableNotEqual")]
    public abstract bool? SByteNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.ISNULL [2 datoshi]
    /// 0D : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0F : OpCode.DROP [2 datoshi]
    /// 10 : OpCode.PUSHDATA1 [8 datoshi]
    /// 12 : OpCode.PUSHDATA1 31 [8 datoshi]
    /// 15 : OpCode.EQUAL [32 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sByteNullableToString")]
    public abstract bool? SByteNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.EQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0D [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.JMPIF 05 [2 datoshi]
    /// 18 : OpCode.PUSHF [1 datoshi]
    /// 19 : OpCode.JMP 17 [2 datoshi]
    /// 1B : OpCode.LDARG1 [2 datoshi]
    /// 1C : OpCode.LDARG0 [2 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.ISNULL [2 datoshi]
    /// 1F : OpCode.JMPIF 0A [2 datoshi]
    /// 21 : OpCode.SWAP [2 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.ISNULL [2 datoshi]
    /// 24 : OpCode.JMPIF 09 [2 datoshi]
    /// 26 : OpCode.NUMEQUAL [8 datoshi]
    /// 27 : OpCode.JMP 09 [2 datoshi]
    /// 29 : OpCode.DROP [2 datoshi]
    /// 2A : OpCode.ISNULL [2 datoshi]
    /// 2B : OpCode.JMP 05 [2 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.PUSHF [1 datoshi]
    /// 30 : OpCode.JMPIF 04 [2 datoshi]
    /// 32 : OpCode.PUSHF [1 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// 34 : OpCode.LDARG1 [2 datoshi]
    /// 35 : OpCode.LDARG0 [2 datoshi]
    /// 36 : OpCode.EQUAL [32 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("shortNullableEqual")]
    public abstract bool? ShortNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIF 04 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.NOT [4 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("shortNullableEqualNull")]
    public abstract bool? ShortNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.NOTEQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0E [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIF 05 [2 datoshi]
    /// 19 : OpCode.PUSHF [1 datoshi]
    /// 1A : OpCode.JMP 18 [2 datoshi]
    /// 1C : OpCode.LDARG1 [2 datoshi]
    /// 1D : OpCode.LDARG0 [2 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.ISNULL [2 datoshi]
    /// 20 : OpCode.JMPIF 0A [2 datoshi]
    /// 22 : OpCode.SWAP [2 datoshi]
    /// 23 : OpCode.DUP [2 datoshi]
    /// 24 : OpCode.ISNULL [2 datoshi]
    /// 25 : OpCode.JMPIF 09 [2 datoshi]
    /// 27 : OpCode.NUMEQUAL [8 datoshi]
    /// 28 : OpCode.JMP 09 [2 datoshi]
    /// 2A : OpCode.DROP [2 datoshi]
    /// 2B : OpCode.ISNULL [2 datoshi]
    /// 2C : OpCode.JMP 05 [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.DROP [2 datoshi]
    /// 30 : OpCode.PUSHF [1 datoshi]
    /// 31 : OpCode.NOT [4 datoshi]
    /// 32 : OpCode.JMPIF 04 [2 datoshi]
    /// 34 : OpCode.PUSHF [1 datoshi]
    /// 35 : OpCode.RET [0 datoshi]
    /// 36 : OpCode.LDARG1 [2 datoshi]
    /// 37 : OpCode.LDARG0 [2 datoshi]
    /// 38 : OpCode.NOTEQUAL [32 datoshi]
    /// 39 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("shortNullableNotEqual")]
    public abstract bool? ShortNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.ISNULL [2 datoshi]
    /// 0D : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0F : OpCode.DROP [2 datoshi]
    /// 10 : OpCode.PUSHDATA1 [8 datoshi]
    /// 12 : OpCode.PUSHDATA1 31 [8 datoshi]
    /// 15 : OpCode.EQUAL [32 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("shortNullableToString")]
    public abstract bool? ShortNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.EQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0D [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.JMPIF 05 [2 datoshi]
    /// 18 : OpCode.PUSHF [1 datoshi]
    /// 19 : OpCode.JMP 17 [2 datoshi]
    /// 1B : OpCode.LDARG1 [2 datoshi]
    /// 1C : OpCode.LDARG0 [2 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.ISNULL [2 datoshi]
    /// 1F : OpCode.JMPIF 0A [2 datoshi]
    /// 21 : OpCode.SWAP [2 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.ISNULL [2 datoshi]
    /// 24 : OpCode.JMPIF 09 [2 datoshi]
    /// 26 : OpCode.NUMEQUAL [8 datoshi]
    /// 27 : OpCode.JMP 09 [2 datoshi]
    /// 29 : OpCode.DROP [2 datoshi]
    /// 2A : OpCode.ISNULL [2 datoshi]
    /// 2B : OpCode.JMP 05 [2 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.PUSHF [1 datoshi]
    /// 30 : OpCode.JMPIF 04 [2 datoshi]
    /// 32 : OpCode.PUSHF [1 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// 34 : OpCode.LDARG1 [2 datoshi]
    /// 35 : OpCode.LDARG0 [2 datoshi]
    /// 36 : OpCode.EQUAL [32 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uIntNullableEqual")]
    public abstract bool? UIntNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIF 04 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.NOT [4 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uIntNullableEqualNull")]
    public abstract bool? UIntNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.NOTEQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0E [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIF 05 [2 datoshi]
    /// 19 : OpCode.PUSHF [1 datoshi]
    /// 1A : OpCode.JMP 18 [2 datoshi]
    /// 1C : OpCode.LDARG1 [2 datoshi]
    /// 1D : OpCode.LDARG0 [2 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.ISNULL [2 datoshi]
    /// 20 : OpCode.JMPIF 0A [2 datoshi]
    /// 22 : OpCode.SWAP [2 datoshi]
    /// 23 : OpCode.DUP [2 datoshi]
    /// 24 : OpCode.ISNULL [2 datoshi]
    /// 25 : OpCode.JMPIF 09 [2 datoshi]
    /// 27 : OpCode.NUMEQUAL [8 datoshi]
    /// 28 : OpCode.JMP 09 [2 datoshi]
    /// 2A : OpCode.DROP [2 datoshi]
    /// 2B : OpCode.ISNULL [2 datoshi]
    /// 2C : OpCode.JMP 05 [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.DROP [2 datoshi]
    /// 30 : OpCode.PUSHF [1 datoshi]
    /// 31 : OpCode.NOT [4 datoshi]
    /// 32 : OpCode.JMPIF 04 [2 datoshi]
    /// 34 : OpCode.PUSHF [1 datoshi]
    /// 35 : OpCode.RET [0 datoshi]
    /// 36 : OpCode.LDARG1 [2 datoshi]
    /// 37 : OpCode.LDARG0 [2 datoshi]
    /// 38 : OpCode.NOTEQUAL [32 datoshi]
    /// 39 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uIntNullableNotEqual")]
    public abstract bool? UIntNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.ISNULL [2 datoshi]
    /// 0D : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0F : OpCode.DROP [2 datoshi]
    /// 10 : OpCode.PUSHDATA1 [8 datoshi]
    /// 12 : OpCode.PUSHDATA1 31 [8 datoshi]
    /// 15 : OpCode.EQUAL [32 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uIntNullableToString")]
    public abstract bool? UIntNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.EQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0D [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.JMPIF 05 [2 datoshi]
    /// 18 : OpCode.PUSHF [1 datoshi]
    /// 19 : OpCode.JMP 17 [2 datoshi]
    /// 1B : OpCode.LDARG1 [2 datoshi]
    /// 1C : OpCode.LDARG0 [2 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.ISNULL [2 datoshi]
    /// 1F : OpCode.JMPIF 0A [2 datoshi]
    /// 21 : OpCode.SWAP [2 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.ISNULL [2 datoshi]
    /// 24 : OpCode.JMPIF 09 [2 datoshi]
    /// 26 : OpCode.NUMEQUAL [8 datoshi]
    /// 27 : OpCode.JMP 09 [2 datoshi]
    /// 29 : OpCode.DROP [2 datoshi]
    /// 2A : OpCode.ISNULL [2 datoshi]
    /// 2B : OpCode.JMP 05 [2 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.PUSHF [1 datoshi]
    /// 30 : OpCode.JMPIF 04 [2 datoshi]
    /// 32 : OpCode.PUSHF [1 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// 34 : OpCode.LDARG1 [2 datoshi]
    /// 35 : OpCode.LDARG0 [2 datoshi]
    /// 36 : OpCode.EQUAL [32 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uLongNullableEqual")]
    public abstract bool? ULongNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIF 04 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.NOT [4 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uLongNullableEqualNull")]
    public abstract bool? ULongNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.NOTEQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0E [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIF 05 [2 datoshi]
    /// 19 : OpCode.PUSHF [1 datoshi]
    /// 1A : OpCode.JMP 18 [2 datoshi]
    /// 1C : OpCode.LDARG1 [2 datoshi]
    /// 1D : OpCode.LDARG0 [2 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.ISNULL [2 datoshi]
    /// 20 : OpCode.JMPIF 0A [2 datoshi]
    /// 22 : OpCode.SWAP [2 datoshi]
    /// 23 : OpCode.DUP [2 datoshi]
    /// 24 : OpCode.ISNULL [2 datoshi]
    /// 25 : OpCode.JMPIF 09 [2 datoshi]
    /// 27 : OpCode.NUMEQUAL [8 datoshi]
    /// 28 : OpCode.JMP 09 [2 datoshi]
    /// 2A : OpCode.DROP [2 datoshi]
    /// 2B : OpCode.ISNULL [2 datoshi]
    /// 2C : OpCode.JMP 05 [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.DROP [2 datoshi]
    /// 30 : OpCode.PUSHF [1 datoshi]
    /// 31 : OpCode.NOT [4 datoshi]
    /// 32 : OpCode.JMPIF 04 [2 datoshi]
    /// 34 : OpCode.PUSHF [1 datoshi]
    /// 35 : OpCode.RET [0 datoshi]
    /// 36 : OpCode.LDARG1 [2 datoshi]
    /// 37 : OpCode.LDARG0 [2 datoshi]
    /// 38 : OpCode.NOTEQUAL [32 datoshi]
    /// 39 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uLongNullableNotEqual")]
    public abstract bool? ULongNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.ISNULL [2 datoshi]
    /// 0D : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0F : OpCode.DROP [2 datoshi]
    /// 10 : OpCode.PUSHDATA1 [8 datoshi]
    /// 12 : OpCode.PUSHDATA1 31 [8 datoshi]
    /// 15 : OpCode.EQUAL [32 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uLongNullableToString")]
    public abstract bool? ULongNullableToString(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmXJAUJIg14eUrYJAWzIgRFCSQFCSIXeXhK2CQKUErYJAmzIglF2CIFRUUJJAQJQHl4l0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.EQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0D [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.JMPIF 05 [2 datoshi]
    /// 18 : OpCode.PUSHF [1 datoshi]
    /// 19 : OpCode.JMP 17 [2 datoshi]
    /// 1B : OpCode.LDARG1 [2 datoshi]
    /// 1C : OpCode.LDARG0 [2 datoshi]
    /// 1D : OpCode.DUP [2 datoshi]
    /// 1E : OpCode.ISNULL [2 datoshi]
    /// 1F : OpCode.JMPIF 0A [2 datoshi]
    /// 21 : OpCode.SWAP [2 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.ISNULL [2 datoshi]
    /// 24 : OpCode.JMPIF 09 [2 datoshi]
    /// 26 : OpCode.NUMEQUAL [8 datoshi]
    /// 27 : OpCode.JMP 09 [2 datoshi]
    /// 29 : OpCode.DROP [2 datoshi]
    /// 2A : OpCode.ISNULL [2 datoshi]
    /// 2B : OpCode.JMP 05 [2 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.PUSHF [1 datoshi]
    /// 30 : OpCode.JMPIF 04 [2 datoshi]
    /// 32 : OpCode.PUSHF [1 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// 34 : OpCode.LDARG1 [2 datoshi]
    /// 35 : OpCode.LDARG0 [2 datoshi]
    /// 36 : OpCode.EQUAL [32 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uShortNullableEqual")]
    public abstract bool? UShortNullableEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNgkBAlAeNiqqkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIF 04 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.NOT [4 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uShortNullableEqualNull")]
    public abstract bool? UShortNullableEqualNull(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmYJAUJIg54eUrYJAWzIgRFCaokBQkiGHl4StgkClBK2CQJsyIJRdgiBUVFCaokBAlAeXiYQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.NOTEQUAL [32 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.PUSHF [1 datoshi]
    /// 09 : OpCode.JMP 0E [2 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.LDARG1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 05 [2 datoshi]
    /// 11 : OpCode.NUMEQUAL [8 datoshi]
    /// 12 : OpCode.JMP 04 [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHF [1 datoshi]
    /// 16 : OpCode.NOT [4 datoshi]
    /// 17 : OpCode.JMPIF 05 [2 datoshi]
    /// 19 : OpCode.PUSHF [1 datoshi]
    /// 1A : OpCode.JMP 18 [2 datoshi]
    /// 1C : OpCode.LDARG1 [2 datoshi]
    /// 1D : OpCode.LDARG0 [2 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.ISNULL [2 datoshi]
    /// 20 : OpCode.JMPIF 0A [2 datoshi]
    /// 22 : OpCode.SWAP [2 datoshi]
    /// 23 : OpCode.DUP [2 datoshi]
    /// 24 : OpCode.ISNULL [2 datoshi]
    /// 25 : OpCode.JMPIF 09 [2 datoshi]
    /// 27 : OpCode.NUMEQUAL [8 datoshi]
    /// 28 : OpCode.JMP 09 [2 datoshi]
    /// 2A : OpCode.DROP [2 datoshi]
    /// 2B : OpCode.ISNULL [2 datoshi]
    /// 2C : OpCode.JMP 05 [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.DROP [2 datoshi]
    /// 30 : OpCode.PUSHF [1 datoshi]
    /// 31 : OpCode.NOT [4 datoshi]
    /// 32 : OpCode.JMPIF 04 [2 datoshi]
    /// 34 : OpCode.PUSHF [1 datoshi]
    /// 35 : OpCode.RET [0 datoshi]
    /// 36 : OpCode.LDARG1 [2 datoshi]
    /// 37 : OpCode.LDARG0 [2 datoshi]
    /// 38 : OpCode.NOTEQUAL [32 datoshi]
    /// 39 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uShortNullableNotEqual")]
    public abstract bool? UShortNullableNotEqual(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAU3AABK2CYFRQwADAExl0A=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 05 [2 datoshi]
    /// 08 : OpCode.CALLT 0000 [32768 datoshi]
    /// 0B : OpCode.DUP [2 datoshi]
    /// 0C : OpCode.ISNULL [2 datoshi]
    /// 0D : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0F : OpCode.DROP [2 datoshi]
    /// 10 : OpCode.PUSHDATA1 [8 datoshi]
    /// 12 : OpCode.PUSHDATA1 31 [8 datoshi]
    /// 15 : OpCode.EQUAL [32 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("uShortNullableToString")]
    public abstract bool? UShortNullableToString(BigInteger? a);

    #endregion
}
