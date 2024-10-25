using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NullableType(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NullableType"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testBigIntegerAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testBigIntegerAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":32,""safe"":false},{""name"":""testBigIntegerCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":39,""safe"":false},{""name"":""testBigIntegerCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":71,""safe"":false},{""name"":""testBigIntegerDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":78,""safe"":false},{""name"":""testBigIntegerDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":89,""safe"":false},{""name"":""testIntAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":94,""safe"":false},{""name"":""testIntAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":126,""safe"":false},{""name"":""testIntCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":133,""safe"":false},{""name"":""testIntCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":165,""safe"":false},{""name"":""testIntDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":172,""safe"":false},{""name"":""testIntDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":183,""safe"":false},{""name"":""testUIntAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":188,""safe"":false},{""name"":""testUIntAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":220,""safe"":false},{""name"":""testUIntCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":227,""safe"":false},{""name"":""testUIntCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":259,""safe"":false},{""name"":""testUIntDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":266,""safe"":false},{""name"":""testUIntDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":277,""safe"":false},{""name"":""testLongAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":282,""safe"":false},{""name"":""testLongAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":314,""safe"":false},{""name"":""testLongCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":321,""safe"":false},{""name"":""testLongCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":353,""safe"":false},{""name"":""testLongDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":360,""safe"":false},{""name"":""testLongDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":371,""safe"":false},{""name"":""testULongAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":376,""safe"":false},{""name"":""testULongAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":408,""safe"":false},{""name"":""testULongCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":415,""safe"":false},{""name"":""testULongCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":447,""safe"":false},{""name"":""testULongDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":454,""safe"":false},{""name"":""testULongDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":465,""safe"":false},{""name"":""testShortAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":470,""safe"":false},{""name"":""testShortAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":502,""safe"":false},{""name"":""testShortCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":509,""safe"":false},{""name"":""testShortCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":541,""safe"":false},{""name"":""testShortDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":548,""safe"":false},{""name"":""testShortDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":559,""safe"":false},{""name"":""testUShortAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":564,""safe"":false},{""name"":""testUShortAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":596,""safe"":false},{""name"":""testUShortCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":603,""safe"":false},{""name"":""testUShortCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":635,""safe"":false},{""name"":""testUShortDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":642,""safe"":false},{""name"":""testUShortDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":653,""safe"":false},{""name"":""testSByteAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":658,""safe"":false},{""name"":""testSByteAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":690,""safe"":false},{""name"":""testSByteCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":697,""safe"":false},{""name"":""testSByteCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":729,""safe"":false},{""name"":""testSByteDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":736,""safe"":false},{""name"":""testSByteDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":747,""safe"":false},{""name"":""testByteAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":752,""safe"":false},{""name"":""testByteAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":784,""safe"":false},{""name"":""testByteCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":791,""safe"":false},{""name"":""testByteCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":823,""safe"":false},{""name"":""testByteDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":830,""safe"":false},{""name"":""testByteDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":841,""safe"":false},{""name"":""testBoolAnd"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":846,""safe"":false},{""name"":""testBoolAndNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":881,""safe"":false},{""name"":""testBoolOr"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":891,""safe"":false},{""name"":""testBoolOrNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":926,""safe"":false},{""name"":""testBoolDefault"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":936,""safe"":false},{""name"":""testBoolDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":947,""safe"":false},{""name"":""testUInt160Default"",""parameters"":[{""name"":""a"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":952,""safe"":false},{""name"":""testUInt160DefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":984,""safe"":false},{""name"":""testUInt256Default"",""parameters"":[{""name"":""a"",""type"":""Hash256""}],""returntype"":""Hash256"",""offset"":989,""safe"":false},{""name"":""testUInt256DefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Hash256""}],""returntype"":""Hash256"",""offset"":1033,""safe"":false},{""name"":""testUInt160ArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1038,""safe"":false},{""name"":""testUInt160ArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1054,""safe"":false},{""name"":""testUInt256ArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1060,""safe"":false},{""name"":""testUInt256ArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1076,""safe"":false},{""name"":""testByteArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1082,""safe"":false},{""name"":""testByteArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":1098,""safe"":false},{""name"":""testStringLength"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Integer"",""offset"":1104,""safe"":false},{""name"":""testStringLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Integer"",""offset"":1120,""safe"":false},{""name"":""testStringDefault"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""String"",""offset"":1126,""safe"":false},{""name"":""testStringDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""String"",""offset"":1138,""safe"":false},{""name"":""testStringConcat"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""}],""returntype"":""String"",""offset"":1143,""safe"":false},{""name"":""testStringConcatNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""}],""returntype"":""String"",""offset"":1166,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2XBFcAAnjYqiQFCSIFediqJhB4StgmAzp5StgmAzqeQBBAVwACeHmeQFcAAnjYqiQFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42KokBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQFcAAnh5nkBXAAJ42KokBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNiqJAUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEBXAAJ4eZ5AVwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0BXAAJ4ebdAVwABeErYJgRFEEBXAAF4QFcAAnjYqiQFCSIFediqJhB4StgmAzp5StgmAzqeQBBAVwACeHmeQFcAAnjYqiQFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42KokBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQFcAAnh5nkBXAAJ42KokBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNiqJAUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEBXAAJ4eZ5AVwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0BXAAJ4ebdAVwABeErYJgRFEEBXAAF4QFcAAnjYqiQFCSIFediqJhB4StgmAzp5StgmAzqeQBBAVwACeHmeQFcAAnjYqiQFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42KokBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQFcAAnh5nkBXAAJ42KokBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNiqJAUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEBXAAJ4eZ5AVwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0BXAAJ4ebdAVwABeErYJgRFEEBXAAF4QFcAAnjYqiQFCSIFediqJAQJQHhK2CYDOiQECUB5StgmAzpAVwACeCQECUB5QFcAAnjYqiQFCSIFediqJAQJQHhK2CYDOiYECEB5StgmAzpAVwACeCYECEB5QFcAAXhK2CYERQlAVwABeEBXAAF4StgmGUUMFAAAAAAAAAAAAAAAAAAAAAAAAAAAQFcAAXhAVwABeErYJiVFDCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBXAAF4QFcAAXhK2CQDykrYJgRFEEBXAAF4ykBXAAF4StgkA8pK2CYERRBAVwABeMpAVwABeErYJAPKStgmBEUQQFcAAXjKQFcAAXhK2CQDykrYJgRFEEBXAAF4ykBXAAF4StgmBUUMAEBXAAF4QFcAAnhK2CYFRQwAeUrYJgVFDACL2yhAVwACeHmL2yhAD+xz/Q=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIFNOT 10
    /// 10 : OpCode.LDARG0
    /// 11 : OpCode.DUP
    /// 12 : OpCode.ISNULL
    /// 13 : OpCode.JMPIFNOT 03
    /// 15 : OpCode.THROW
    /// 16 : OpCode.LDARG1
    /// 17 : OpCode.DUP
    /// 18 : OpCode.ISNULL
    /// 19 : OpCode.JMPIFNOT 03
    /// 1B : OpCode.THROW
    /// 1C : OpCode.ADD
    /// 1D : OpCode.RET
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerAdd")]
    public abstract BigInteger? TestBigIntegerAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerAddNonNullable")]
    public abstract BigInteger? TestBigIntegerAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIF 04
    /// 10 : OpCode.PUSHF
    /// 11 : OpCode.RET
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.DUP
    /// 14 : OpCode.ISNULL
    /// 15 : OpCode.JMPIFNOT 03
    /// 17 : OpCode.THROW
    /// 18 : OpCode.LDARG1
    /// 19 : OpCode.DUP
    /// 1A : OpCode.ISNULL
    /// 1B : OpCode.JMPIFNOT 03
    /// 1D : OpCode.THROW
    /// 1E : OpCode.GT
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerCompare")]
    public abstract bool? TestBigIntegerCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.GT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerCompareNonNullable")]
    public abstract bool? TestBigIntegerCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 04
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerDefault")]
    public abstract BigInteger? TestBigIntegerDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerDefaultNonNullable")]
    public abstract BigInteger? TestBigIntegerDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6JAQJQHlK2CYDOkA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIF 04
    /// 10 : OpCode.PUSHF
    /// 11 : OpCode.RET
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.DUP
    /// 14 : OpCode.ISNULL
    /// 15 : OpCode.JMPIFNOT 03
    /// 17 : OpCode.THROW
    /// 18 : OpCode.JMPIF 04
    /// 1A : OpCode.PUSHF
    /// 1B : OpCode.RET
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ISNULL
    /// 1F : OpCode.JMPIFNOT 03
    /// 21 : OpCode.THROW
    /// 22 : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolAnd")]
    public abstract bool? TestBoolAnd(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCQECUB5QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.JMPIF 04
    /// 06 : OpCode.PUSHF
    /// 07 : OpCode.RET
    /// 08 : OpCode.LDARG1
    /// 09 : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolAndNonNullable")]
    public abstract bool? TestBoolAndNonNullable(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFCUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 04
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSHF
    /// 0A : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolDefault")]
    public abstract bool? TestBoolDefault(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolDefaultNonNullable")]
    public abstract bool? TestBoolDefaultNonNullable(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6JgQIQHlK2CYDOkA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIF 04
    /// 10 : OpCode.PUSHF
    /// 11 : OpCode.RET
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.DUP
    /// 14 : OpCode.ISNULL
    /// 15 : OpCode.JMPIFNOT 03
    /// 17 : OpCode.THROW
    /// 18 : OpCode.JMPIFNOT 04
    /// 1A : OpCode.PUSHT
    /// 1B : OpCode.RET
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ISNULL
    /// 1F : OpCode.JMPIFNOT 03
    /// 21 : OpCode.THROW
    /// 22 : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolOr")]
    public abstract bool? TestBoolOr(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCYECEB5QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.JMPIFNOT 04
    /// 06 : OpCode.PUSHT
    /// 07 : OpCode.RET
    /// 08 : OpCode.LDARG1
    /// 09 : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolOrNonNullable")]
    public abstract bool? TestBoolOrNonNullable(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIFNOT 10
    /// 10 : OpCode.LDARG0
    /// 11 : OpCode.DUP
    /// 12 : OpCode.ISNULL
    /// 13 : OpCode.JMPIFNOT 03
    /// 15 : OpCode.THROW
    /// 16 : OpCode.LDARG1
    /// 17 : OpCode.DUP
    /// 18 : OpCode.ISNULL
    /// 19 : OpCode.JMPIFNOT 03
    /// 1B : OpCode.THROW
    /// 1C : OpCode.ADD
    /// 1D : OpCode.RET
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testByteAdd")]
    public abstract BigInteger? TestByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteAddNonNullable")]
    public abstract BigInteger? TestByteAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 03
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.DUP
    /// 0A : OpCode.ISNULL
    /// 0B : OpCode.JMPIFNOT 04
    /// 0D : OpCode.DROP
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.RET
    /// </remarks>
    [DisplayName("testByteArrayLength")]
    public abstract BigInteger? TestByteArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.SIZE
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteArrayLengthNonNullable")]
    public abstract BigInteger? TestByteArrayLengthNonNullable(byte[]? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIF 04
    /// 10 : OpCode.PUSHF
    /// 11 : OpCode.RET
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.DUP
    /// 14 : OpCode.ISNULL
    /// 15 : OpCode.JMPIFNOT 03
    /// 17 : OpCode.THROW
    /// 18 : OpCode.LDARG1
    /// 19 : OpCode.DUP
    /// 1A : OpCode.ISNULL
    /// 1B : OpCode.JMPIFNOT 03
    /// 1D : OpCode.THROW
    /// 1E : OpCode.GT
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testByteCompare")]
    public abstract bool? TestByteCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.GT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteCompareNonNullable")]
    public abstract bool? TestByteCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 04
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.RET
    /// </remarks>
    [DisplayName("testByteDefault")]
    public abstract BigInteger? TestByteDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteDefaultNonNullable")]
    public abstract BigInteger? TestByteDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIFNOT 10
    /// 10 : OpCode.LDARG0
    /// 11 : OpCode.DUP
    /// 12 : OpCode.ISNULL
    /// 13 : OpCode.JMPIFNOT 03
    /// 15 : OpCode.THROW
    /// 16 : OpCode.LDARG1
    /// 17 : OpCode.DUP
    /// 18 : OpCode.ISNULL
    /// 19 : OpCode.JMPIFNOT 03
    /// 1B : OpCode.THROW
    /// 1C : OpCode.ADD
    /// 1D : OpCode.RET
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testIntAdd")]
    public abstract BigInteger? TestIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testIntAddNonNullable")]
    public abstract BigInteger? TestIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIF 04
    /// 10 : OpCode.PUSHF
    /// 11 : OpCode.RET
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.DUP
    /// 14 : OpCode.ISNULL
    /// 15 : OpCode.JMPIFNOT 03
    /// 17 : OpCode.THROW
    /// 18 : OpCode.LDARG1
    /// 19 : OpCode.DUP
    /// 1A : OpCode.ISNULL
    /// 1B : OpCode.JMPIFNOT 03
    /// 1D : OpCode.THROW
    /// 1E : OpCode.GT
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testIntCompare")]
    public abstract bool? TestIntCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.GT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testIntCompareNonNullable")]
    public abstract bool? TestIntCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 04
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.RET
    /// </remarks>
    [DisplayName("testIntDefault")]
    public abstract BigInteger? TestIntDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("testIntDefaultNonNullable")]
    public abstract BigInteger? TestIntDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIFNOT 10
    /// 10 : OpCode.LDARG0
    /// 11 : OpCode.DUP
    /// 12 : OpCode.ISNULL
    /// 13 : OpCode.JMPIFNOT 03
    /// 15 : OpCode.THROW
    /// 16 : OpCode.LDARG1
    /// 17 : OpCode.DUP
    /// 18 : OpCode.ISNULL
    /// 19 : OpCode.JMPIFNOT 03
    /// 1B : OpCode.THROW
    /// 1C : OpCode.ADD
    /// 1D : OpCode.RET
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testLongAdd")]
    public abstract BigInteger? TestLongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testLongAddNonNullable")]
    public abstract BigInteger? TestLongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIF 04
    /// 10 : OpCode.PUSHF
    /// 11 : OpCode.RET
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.DUP
    /// 14 : OpCode.ISNULL
    /// 15 : OpCode.JMPIFNOT 03
    /// 17 : OpCode.THROW
    /// 18 : OpCode.LDARG1
    /// 19 : OpCode.DUP
    /// 1A : OpCode.ISNULL
    /// 1B : OpCode.JMPIFNOT 03
    /// 1D : OpCode.THROW
    /// 1E : OpCode.GT
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testLongCompare")]
    public abstract bool? TestLongCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.GT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testLongCompareNonNullable")]
    public abstract bool? TestLongCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 04
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.RET
    /// </remarks>
    [DisplayName("testLongDefault")]
    public abstract BigInteger? TestLongDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("testLongDefaultNonNullable")]
    public abstract BigInteger? TestLongDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIFNOT 10
    /// 10 : OpCode.LDARG0
    /// 11 : OpCode.DUP
    /// 12 : OpCode.ISNULL
    /// 13 : OpCode.JMPIFNOT 03
    /// 15 : OpCode.THROW
    /// 16 : OpCode.LDARG1
    /// 17 : OpCode.DUP
    /// 18 : OpCode.ISNULL
    /// 19 : OpCode.JMPIFNOT 03
    /// 1B : OpCode.THROW
    /// 1C : OpCode.ADD
    /// 1D : OpCode.RET
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteAdd")]
    public abstract BigInteger? TestSByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteAddNonNullable")]
    public abstract BigInteger? TestSByteAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIF 04
    /// 10 : OpCode.PUSHF
    /// 11 : OpCode.RET
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.DUP
    /// 14 : OpCode.ISNULL
    /// 15 : OpCode.JMPIFNOT 03
    /// 17 : OpCode.THROW
    /// 18 : OpCode.LDARG1
    /// 19 : OpCode.DUP
    /// 1A : OpCode.ISNULL
    /// 1B : OpCode.JMPIFNOT 03
    /// 1D : OpCode.THROW
    /// 1E : OpCode.GT
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteCompare")]
    public abstract bool? TestSByteCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.GT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteCompareNonNullable")]
    public abstract bool? TestSByteCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 04
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteDefault")]
    public abstract BigInteger? TestSByteDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteDefaultNonNullable")]
    public abstract BigInteger? TestSByteDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIFNOT 10
    /// 10 : OpCode.LDARG0
    /// 11 : OpCode.DUP
    /// 12 : OpCode.ISNULL
    /// 13 : OpCode.JMPIFNOT 03
    /// 15 : OpCode.THROW
    /// 16 : OpCode.LDARG1
    /// 17 : OpCode.DUP
    /// 18 : OpCode.ISNULL
    /// 19 : OpCode.JMPIFNOT 03
    /// 1B : OpCode.THROW
    /// 1C : OpCode.ADD
    /// 1D : OpCode.RET
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testShortAdd")]
    public abstract BigInteger? TestShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testShortAddNonNullable")]
    public abstract BigInteger? TestShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIF 04
    /// 10 : OpCode.PUSHF
    /// 11 : OpCode.RET
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.DUP
    /// 14 : OpCode.ISNULL
    /// 15 : OpCode.JMPIFNOT 03
    /// 17 : OpCode.THROW
    /// 18 : OpCode.LDARG1
    /// 19 : OpCode.DUP
    /// 1A : OpCode.ISNULL
    /// 1B : OpCode.JMPIFNOT 03
    /// 1D : OpCode.THROW
    /// 1E : OpCode.GT
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testShortCompare")]
    public abstract bool? TestShortCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.GT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testShortCompareNonNullable")]
    public abstract bool? TestShortCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 04
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.RET
    /// </remarks>
    [DisplayName("testShortDefault")]
    public abstract BigInteger? TestShortDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("testShortDefaultNonNullable")]
    public abstract BigInteger? TestShortDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeErYJgVFDHlK2CYFRQyL2yhA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSHDATA1
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.DUP
    /// 0D : OpCode.ISNULL
    /// 0E : OpCode.JMPIFNOT 05
    /// 10 : OpCode.DROP
    /// 11 : OpCode.PUSHDATA1
    /// 13 : OpCode.CAT
    /// 14 : OpCode.CONVERT 28
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("testStringConcat")]
    public abstract string? TestStringConcat(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2yhA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.CAT
    /// 06 : OpCode.CONVERT 28
    /// 08 : OpCode.RET
    /// </remarks>
    [DisplayName("testStringConcatNonNullable")]
    public abstract string? TestStringConcatNonNullable(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgVFDEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSHDATA1
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("testStringDefault")]
    public abstract string? TestStringDefault(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("testStringDefaultNonNullable")]
    public abstract string? TestStringDefaultNonNullable(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 03
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.DUP
    /// 0A : OpCode.ISNULL
    /// 0B : OpCode.JMPIFNOT 04
    /// 0D : OpCode.DROP
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.RET
    /// </remarks>
    [DisplayName("testStringLength")]
    public abstract BigInteger? TestStringLength(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.SIZE
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("testStringLengthNonNullable")]
    public abstract BigInteger? TestStringLengthNonNullable(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 03
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.DUP
    /// 0A : OpCode.ISNULL
    /// 0B : OpCode.JMPIFNOT 04
    /// 0D : OpCode.DROP
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt160ArrayLength")]
    public abstract BigInteger? TestUInt160ArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.SIZE
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt160ArrayLengthNonNullable")]
    public abstract BigInteger? TestUInt160ArrayLengthNonNullable(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJhlFDAAAAAAAAAAAAAAAAAAAAAAAAAAAQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 19
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt160Default")]
    public abstract UInt160? TestUInt160Default(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt160DefaultNonNullable")]
    public abstract UInt160? TestUInt160DefaultNonNullable(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 03
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.DUP
    /// 0A : OpCode.ISNULL
    /// 0B : OpCode.JMPIFNOT 04
    /// 0D : OpCode.DROP
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt256ArrayLength")]
    public abstract BigInteger? TestUInt256ArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.SIZE
    /// 05 : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt256ArrayLengthNonNullable")]
    public abstract BigInteger? TestUInt256ArrayLengthNonNullable(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJiVFDAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 25
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000
    /// 2B : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt256Default")]
    public abstract UInt256? TestUInt256Default(UInt256? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt256DefaultNonNullable")]
    public abstract UInt256? TestUInt256DefaultNonNullable(UInt256? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIFNOT 10
    /// 10 : OpCode.LDARG0
    /// 11 : OpCode.DUP
    /// 12 : OpCode.ISNULL
    /// 13 : OpCode.JMPIFNOT 03
    /// 15 : OpCode.THROW
    /// 16 : OpCode.LDARG1
    /// 17 : OpCode.DUP
    /// 18 : OpCode.ISNULL
    /// 19 : OpCode.JMPIFNOT 03
    /// 1B : OpCode.THROW
    /// 1C : OpCode.ADD
    /// 1D : OpCode.RET
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntAdd")]
    public abstract BigInteger? TestUIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntAddNonNullable")]
    public abstract BigInteger? TestUIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIF 04
    /// 10 : OpCode.PUSHF
    /// 11 : OpCode.RET
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.DUP
    /// 14 : OpCode.ISNULL
    /// 15 : OpCode.JMPIFNOT 03
    /// 17 : OpCode.THROW
    /// 18 : OpCode.LDARG1
    /// 19 : OpCode.DUP
    /// 1A : OpCode.ISNULL
    /// 1B : OpCode.JMPIFNOT 03
    /// 1D : OpCode.THROW
    /// 1E : OpCode.GT
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntCompare")]
    public abstract bool? TestUIntCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.GT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntCompareNonNullable")]
    public abstract bool? TestUIntCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 04
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntDefault")]
    public abstract BigInteger? TestUIntDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntDefaultNonNullable")]
    public abstract BigInteger? TestUIntDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIFNOT 10
    /// 10 : OpCode.LDARG0
    /// 11 : OpCode.DUP
    /// 12 : OpCode.ISNULL
    /// 13 : OpCode.JMPIFNOT 03
    /// 15 : OpCode.THROW
    /// 16 : OpCode.LDARG1
    /// 17 : OpCode.DUP
    /// 18 : OpCode.ISNULL
    /// 19 : OpCode.JMPIFNOT 03
    /// 1B : OpCode.THROW
    /// 1C : OpCode.ADD
    /// 1D : OpCode.RET
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testULongAdd")]
    public abstract BigInteger? TestULongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testULongAddNonNullable")]
    public abstract BigInteger? TestULongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIF 04
    /// 10 : OpCode.PUSHF
    /// 11 : OpCode.RET
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.DUP
    /// 14 : OpCode.ISNULL
    /// 15 : OpCode.JMPIFNOT 03
    /// 17 : OpCode.THROW
    /// 18 : OpCode.LDARG1
    /// 19 : OpCode.DUP
    /// 1A : OpCode.ISNULL
    /// 1B : OpCode.JMPIFNOT 03
    /// 1D : OpCode.THROW
    /// 1E : OpCode.GT
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testULongCompare")]
    public abstract bool? TestULongCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.GT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testULongCompareNonNullable")]
    public abstract bool? TestULongCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 04
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.RET
    /// </remarks>
    [DisplayName("testULongDefault")]
    public abstract BigInteger? TestULongDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("testULongDefaultNonNullable")]
    public abstract BigInteger? TestULongDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIFNOT 10
    /// 10 : OpCode.LDARG0
    /// 11 : OpCode.DUP
    /// 12 : OpCode.ISNULL
    /// 13 : OpCode.JMPIFNOT 03
    /// 15 : OpCode.THROW
    /// 16 : OpCode.LDARG1
    /// 17 : OpCode.DUP
    /// 18 : OpCode.ISNULL
    /// 19 : OpCode.JMPIFNOT 03
    /// 1B : OpCode.THROW
    /// 1C : OpCode.ADD
    /// 1D : OpCode.RET
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortAdd")]
    public abstract BigInteger? TestUShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortAddNonNullable")]
    public abstract BigInteger? TestUShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.NOT
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.JMP 05
    /// 0B : OpCode.LDARG1
    /// 0C : OpCode.ISNULL
    /// 0D : OpCode.NOT
    /// 0E : OpCode.JMPIF 04
    /// 10 : OpCode.PUSHF
    /// 11 : OpCode.RET
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.DUP
    /// 14 : OpCode.ISNULL
    /// 15 : OpCode.JMPIFNOT 03
    /// 17 : OpCode.THROW
    /// 18 : OpCode.LDARG1
    /// 19 : OpCode.DUP
    /// 1A : OpCode.ISNULL
    /// 1B : OpCode.JMPIFNOT 03
    /// 1D : OpCode.THROW
    /// 1E : OpCode.GT
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortCompare")]
    public abstract bool? TestUShortCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.GT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortCompareNonNullable")]
    public abstract bool? TestUShortCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 04
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortDefault")]
    public abstract BigInteger? TestUShortDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortDefaultNonNullable")]
    public abstract BigInteger? TestUShortDefaultNonNullable(BigInteger? a);

    #endregion
}
