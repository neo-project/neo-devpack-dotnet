using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NullableType(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NullableType"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testBigIntegerAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testBigIntegerAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":31,""safe"":false},{""name"":""testBigIntegerCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":38,""safe"":false},{""name"":""testBigIntegerCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":69,""safe"":false},{""name"":""testBigIntegerDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":76,""safe"":false},{""name"":""testBigIntegerDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":87,""safe"":false},{""name"":""testIntAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":92,""safe"":false},{""name"":""testIntAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":123,""safe"":false},{""name"":""testIntCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":130,""safe"":false},{""name"":""testIntCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":161,""safe"":false},{""name"":""testIntDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":168,""safe"":false},{""name"":""testIntDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":179,""safe"":false},{""name"":""testUIntAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":184,""safe"":false},{""name"":""testUIntAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":215,""safe"":false},{""name"":""testUIntCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":222,""safe"":false},{""name"":""testUIntCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":253,""safe"":false},{""name"":""testUIntDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":260,""safe"":false},{""name"":""testUIntDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":271,""safe"":false},{""name"":""testLongAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":276,""safe"":false},{""name"":""testLongAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":307,""safe"":false},{""name"":""testLongCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":314,""safe"":false},{""name"":""testLongCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":345,""safe"":false},{""name"":""testLongDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":352,""safe"":false},{""name"":""testLongDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":363,""safe"":false},{""name"":""testULongAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":368,""safe"":false},{""name"":""testULongAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":399,""safe"":false},{""name"":""testULongCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":406,""safe"":false},{""name"":""testULongCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":437,""safe"":false},{""name"":""testULongDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":444,""safe"":false},{""name"":""testULongDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":455,""safe"":false},{""name"":""testShortAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":460,""safe"":false},{""name"":""testShortAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":491,""safe"":false},{""name"":""testShortCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":498,""safe"":false},{""name"":""testShortCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":529,""safe"":false},{""name"":""testShortDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":536,""safe"":false},{""name"":""testShortDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":547,""safe"":false},{""name"":""testUShortAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":552,""safe"":false},{""name"":""testUShortAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":583,""safe"":false},{""name"":""testUShortCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":590,""safe"":false},{""name"":""testUShortCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":621,""safe"":false},{""name"":""testUShortDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":628,""safe"":false},{""name"":""testUShortDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":639,""safe"":false},{""name"":""testSByteAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":644,""safe"":false},{""name"":""testSByteAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":675,""safe"":false},{""name"":""testSByteCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":682,""safe"":false},{""name"":""testSByteCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":713,""safe"":false},{""name"":""testSByteDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":720,""safe"":false},{""name"":""testSByteDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":731,""safe"":false},{""name"":""testByteAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":736,""safe"":false},{""name"":""testByteAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":767,""safe"":false},{""name"":""testByteCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":774,""safe"":false},{""name"":""testByteCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":805,""safe"":false},{""name"":""testByteDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":812,""safe"":false},{""name"":""testByteDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":823,""safe"":false},{""name"":""testBoolAnd"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":828,""safe"":false},{""name"":""testBoolAndNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":862,""safe"":false},{""name"":""testBoolOr"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":872,""safe"":false},{""name"":""testBoolOrNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":906,""safe"":false},{""name"":""testBoolDefault"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":916,""safe"":false},{""name"":""testBoolDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":927,""safe"":false},{""name"":""testUInt160Default"",""parameters"":[{""name"":""a"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":932,""safe"":false},{""name"":""testUInt160DefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":964,""safe"":false},{""name"":""testUInt256Default"",""parameters"":[{""name"":""a"",""type"":""Hash256""}],""returntype"":""Hash256"",""offset"":969,""safe"":false},{""name"":""testUInt256DefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Hash256""}],""returntype"":""Hash256"",""offset"":1013,""safe"":false},{""name"":""testUInt160ArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1018,""safe"":false},{""name"":""testUInt160ArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1034,""safe"":false},{""name"":""testUInt256ArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1040,""safe"":false},{""name"":""testUInt256ArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1056,""safe"":false},{""name"":""testByteArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1062,""safe"":false},{""name"":""testByteArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":1078,""safe"":false},{""name"":""testStringLength"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Integer"",""offset"":1084,""safe"":false},{""name"":""testStringLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Integer"",""offset"":1100,""safe"":false},{""name"":""testStringDefault"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""String"",""offset"":1106,""safe"":false},{""name"":""testStringDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""String"",""offset"":1118,""safe"":false},{""name"":""testStringConcat"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""}],""returntype"":""String"",""offset"":1123,""safe"":false},{""name"":""testStringConcatNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""}],""returntype"":""String"",""offset"":1146,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2DBFcAAnjYJgUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEBXAAJ4eZ5AVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQFcAAnh5nkBXAAJ42CYFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42CYFCSIFediqJhB4StgmAzp5StgmAzqeQBBAVwACeHmeQFcAAnjYJgUJIgV52KokBAlAeErYJgM6eUrYJgM6t0BXAAJ4ebdAVwABeErYJgRFEEBXAAF4QFcAAnjYJgUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEBXAAJ4eZ5AVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQFcAAnh5nkBXAAJ42CYFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42CYFCSIFediqJhB4StgmAzp5StgmAzqeQBBAVwACeHmeQFcAAnjYJgUJIgV52KokBAlAeErYJgM6eUrYJgM6t0BXAAJ4ebdAVwABeErYJgRFEEBXAAF4QFcAAnjYJgUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEBXAAJ4eZ5AVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQFcAAnh5nkBXAAJ42CYFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42CYFCSIFediqJhB4StgmAzp5StgmAzqeQBBAVwACeHmeQFcAAnjYJgUJIgV52KokBAlAeErYJgM6eUrYJgM6t0BXAAJ4ebdAVwABeErYJgRFEEBXAAF4QFcAAnjYJgUJIgV52KokBAlAeErYJgM6JAQJQHlK2CYDOkBXAAJ4JAQJQHlAVwACeNgmBQkiBXnYqiQECUB4StgmAzomBAhAeUrYJgM6QFcAAngmBAhAeUBXAAF4StgmBEUJQFcAAXhAVwABeErYJhlFDBQAAAAAAAAAAAAAAAAAAAAAAAAAAEBXAAF4QFcAAXhK2CYlRQwgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABAVwABeEBXAAF4StgkA8pK2CYERRBAVwABeMpAVwABeErYJAPKStgmBEUQQFcAAXjKQFcAAXhK2CQDykrYJgRFEEBXAAF4ykBXAAF4StgkA8pK2CYERRBAVwABeMpAVwABeErYJgVFDABAVwABeEBXAAJ4StgmBUUMAHlK2CYFRQwAi9soQFcAAnh5i9soQLyb72w="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIFNOT 10 [2 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.ISNULL [2 datoshi]
    /// 12 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 14 : OpCode.THROW [512 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.ISNULL [2 datoshi]
    /// 18 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1A : OpCode.THROW [512 datoshi]
    /// 1B : OpCode.ADD [8 datoshi]
    /// 1C : OpCode.RET [0 datoshi]
    /// 1D : OpCode.PUSH0 [1 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerAdd")]
    public abstract BigInteger? TestBigIntegerAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.ADD [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerAddNonNullable")]
    public abstract BigInteger? TestBigIntegerAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIF 04 [2 datoshi]
    /// 0F : OpCode.PUSHF [1 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.ISNULL [2 datoshi]
    /// 14 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 16 : OpCode.THROW [512 datoshi]
    /// 17 : OpCode.LDARG1 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.ISNULL [2 datoshi]
    /// 1A : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1C : OpCode.THROW [512 datoshi]
    /// 1D : OpCode.GT [8 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerCompare")]
    public abstract bool? TestBigIntegerCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.GT [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerCompareNonNullable")]
    public abstract bool? TestBigIntegerCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSH0 [1 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerDefault")]
    public abstract BigInteger? TestBigIntegerDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerDefaultNonNullable")]
    public abstract BigInteger? TestBigIntegerDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzokBAlAeUrYJgM6QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIF 04 [2 datoshi]
    /// 0F : OpCode.PUSHF [1 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.ISNULL [2 datoshi]
    /// 14 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 16 : OpCode.THROW [512 datoshi]
    /// 17 : OpCode.JMPIF 04 [2 datoshi]
    /// 19 : OpCode.PUSHF [1 datoshi]
    /// 1A : OpCode.RET [0 datoshi]
    /// 1B : OpCode.LDARG1 [2 datoshi]
    /// 1C : OpCode.DUP [2 datoshi]
    /// 1D : OpCode.ISNULL [2 datoshi]
    /// 1E : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 20 : OpCode.THROW [512 datoshi]
    /// 21 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolAnd")]
    public abstract bool? TestBoolAnd(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCQECUB5QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.JMPIF 04 [2 datoshi]
    /// 06 : OpCode.PUSHF [1 datoshi]
    /// 07 : OpCode.RET [0 datoshi]
    /// 08 : OpCode.LDARG1 [2 datoshi]
    /// 09 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolAndNonNullable")]
    public abstract bool? TestBoolAndNonNullable(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFCUA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSHF [1 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolDefault")]
    public abstract bool? TestBoolDefault(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolDefaultNonNullable")]
    public abstract bool? TestBoolDefaultNonNullable(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzomBAhAeUrYJgM6QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIF 04 [2 datoshi]
    /// 0F : OpCode.PUSHF [1 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.ISNULL [2 datoshi]
    /// 14 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 16 : OpCode.THROW [512 datoshi]
    /// 17 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 19 : OpCode.PUSHT [1 datoshi]
    /// 1A : OpCode.RET [0 datoshi]
    /// 1B : OpCode.LDARG1 [2 datoshi]
    /// 1C : OpCode.DUP [2 datoshi]
    /// 1D : OpCode.ISNULL [2 datoshi]
    /// 1E : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 20 : OpCode.THROW [512 datoshi]
    /// 21 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolOr")]
    public abstract bool? TestBoolOr(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCYECEB5QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 06 : OpCode.PUSHT [1 datoshi]
    /// 07 : OpCode.RET [0 datoshi]
    /// 08 : OpCode.LDARG1 [2 datoshi]
    /// 09 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolOrNonNullable")]
    public abstract bool? TestBoolOrNonNullable(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIFNOT 10 [2 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.ISNULL [2 datoshi]
    /// 12 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 14 : OpCode.THROW [512 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.ISNULL [2 datoshi]
    /// 18 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1A : OpCode.THROW [512 datoshi]
    /// 1B : OpCode.ADD [8 datoshi]
    /// 1C : OpCode.RET [0 datoshi]
    /// 1D : OpCode.PUSH0 [1 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteAdd")]
    public abstract BigInteger? TestByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.ADD [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteAddNonNullable")]
    public abstract BigInteger? TestByteAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 03 [2 datoshi]
    /// 08 : OpCode.SIZE [4 datoshi]
    /// 09 : OpCode.DUP [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 0D : OpCode.DROP [2 datoshi]
    /// 0E : OpCode.PUSH0 [1 datoshi]
    /// 0F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayLength")]
    public abstract BigInteger? TestByteArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.SIZE [4 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayLengthNonNullable")]
    public abstract BigInteger? TestByteArrayLengthNonNullable(byte[]? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIF 04 [2 datoshi]
    /// 0F : OpCode.PUSHF [1 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.ISNULL [2 datoshi]
    /// 14 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 16 : OpCode.THROW [512 datoshi]
    /// 17 : OpCode.LDARG1 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.ISNULL [2 datoshi]
    /// 1A : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1C : OpCode.THROW [512 datoshi]
    /// 1D : OpCode.GT [8 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteCompare")]
    public abstract bool? TestByteCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.GT [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteCompareNonNullable")]
    public abstract bool? TestByteCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSH0 [1 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteDefault")]
    public abstract BigInteger? TestByteDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteDefaultNonNullable")]
    public abstract BigInteger? TestByteDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIFNOT 10 [2 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.ISNULL [2 datoshi]
    /// 12 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 14 : OpCode.THROW [512 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.ISNULL [2 datoshi]
    /// 18 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1A : OpCode.THROW [512 datoshi]
    /// 1B : OpCode.ADD [8 datoshi]
    /// 1C : OpCode.RET [0 datoshi]
    /// 1D : OpCode.PUSH0 [1 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntAdd")]
    public abstract BigInteger? TestIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.ADD [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntAddNonNullable")]
    public abstract BigInteger? TestIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIF 04 [2 datoshi]
    /// 0F : OpCode.PUSHF [1 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.ISNULL [2 datoshi]
    /// 14 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 16 : OpCode.THROW [512 datoshi]
    /// 17 : OpCode.LDARG1 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.ISNULL [2 datoshi]
    /// 1A : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1C : OpCode.THROW [512 datoshi]
    /// 1D : OpCode.GT [8 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntCompare")]
    public abstract bool? TestIntCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.GT [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntCompareNonNullable")]
    public abstract bool? TestIntCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSH0 [1 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntDefault")]
    public abstract BigInteger? TestIntDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntDefaultNonNullable")]
    public abstract BigInteger? TestIntDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIFNOT 10 [2 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.ISNULL [2 datoshi]
    /// 12 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 14 : OpCode.THROW [512 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.ISNULL [2 datoshi]
    /// 18 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1A : OpCode.THROW [512 datoshi]
    /// 1B : OpCode.ADD [8 datoshi]
    /// 1C : OpCode.RET [0 datoshi]
    /// 1D : OpCode.PUSH0 [1 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongAdd")]
    public abstract BigInteger? TestLongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.ADD [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongAddNonNullable")]
    public abstract BigInteger? TestLongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIF 04 [2 datoshi]
    /// 0F : OpCode.PUSHF [1 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.ISNULL [2 datoshi]
    /// 14 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 16 : OpCode.THROW [512 datoshi]
    /// 17 : OpCode.LDARG1 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.ISNULL [2 datoshi]
    /// 1A : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1C : OpCode.THROW [512 datoshi]
    /// 1D : OpCode.GT [8 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongCompare")]
    public abstract bool? TestLongCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.GT [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongCompareNonNullable")]
    public abstract bool? TestLongCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSH0 [1 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongDefault")]
    public abstract BigInteger? TestLongDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongDefaultNonNullable")]
    public abstract BigInteger? TestLongDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIFNOT 10 [2 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.ISNULL [2 datoshi]
    /// 12 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 14 : OpCode.THROW [512 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.ISNULL [2 datoshi]
    /// 18 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1A : OpCode.THROW [512 datoshi]
    /// 1B : OpCode.ADD [8 datoshi]
    /// 1C : OpCode.RET [0 datoshi]
    /// 1D : OpCode.PUSH0 [1 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteAdd")]
    public abstract BigInteger? TestSByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.ADD [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteAddNonNullable")]
    public abstract BigInteger? TestSByteAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIF 04 [2 datoshi]
    /// 0F : OpCode.PUSHF [1 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.ISNULL [2 datoshi]
    /// 14 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 16 : OpCode.THROW [512 datoshi]
    /// 17 : OpCode.LDARG1 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.ISNULL [2 datoshi]
    /// 1A : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1C : OpCode.THROW [512 datoshi]
    /// 1D : OpCode.GT [8 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteCompare")]
    public abstract bool? TestSByteCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.GT [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteCompareNonNullable")]
    public abstract bool? TestSByteCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSH0 [1 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteDefault")]
    public abstract BigInteger? TestSByteDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteDefaultNonNullable")]
    public abstract BigInteger? TestSByteDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIFNOT 10 [2 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.ISNULL [2 datoshi]
    /// 12 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 14 : OpCode.THROW [512 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.ISNULL [2 datoshi]
    /// 18 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1A : OpCode.THROW [512 datoshi]
    /// 1B : OpCode.ADD [8 datoshi]
    /// 1C : OpCode.RET [0 datoshi]
    /// 1D : OpCode.PUSH0 [1 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortAdd")]
    public abstract BigInteger? TestShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.ADD [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortAddNonNullable")]
    public abstract BigInteger? TestShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIF 04 [2 datoshi]
    /// 0F : OpCode.PUSHF [1 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.ISNULL [2 datoshi]
    /// 14 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 16 : OpCode.THROW [512 datoshi]
    /// 17 : OpCode.LDARG1 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.ISNULL [2 datoshi]
    /// 1A : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1C : OpCode.THROW [512 datoshi]
    /// 1D : OpCode.GT [8 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortCompare")]
    public abstract bool? TestShortCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.GT [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortCompareNonNullable")]
    public abstract bool? TestShortCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSH0 [1 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortDefault")]
    public abstract BigInteger? TestShortDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortDefaultNonNullable")]
    public abstract BigInteger? TestShortDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeErYJgVFDHlK2CYFRQyL2yhA
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSHDATA1 [8 datoshi]
    /// 0B : OpCode.LDARG1 [2 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.ISNULL [2 datoshi]
    /// 0E : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 10 : OpCode.DROP [2 datoshi]
    /// 11 : OpCode.PUSHDATA1 [8 datoshi]
    /// 13 : OpCode.CAT [2048 datoshi]
    /// 14 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringConcat")]
    public abstract string? TestStringConcat(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2yhA
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.CAT [2048 datoshi]
    /// 06 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringConcatNonNullable")]
    public abstract string? TestStringConcatNonNullable(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgVFDEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSHDATA1 [8 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringDefault")]
    public abstract string? TestStringDefault(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringDefaultNonNullable")]
    public abstract string? TestStringDefaultNonNullable(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 03 [2 datoshi]
    /// 08 : OpCode.SIZE [4 datoshi]
    /// 09 : OpCode.DUP [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 0D : OpCode.DROP [2 datoshi]
    /// 0E : OpCode.PUSH0 [1 datoshi]
    /// 0F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringLength")]
    public abstract BigInteger? TestStringLength(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.SIZE [4 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringLengthNonNullable")]
    public abstract BigInteger? TestStringLengthNonNullable(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 03 [2 datoshi]
    /// 08 : OpCode.SIZE [4 datoshi]
    /// 09 : OpCode.DUP [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 0D : OpCode.DROP [2 datoshi]
    /// 0E : OpCode.PUSH0 [1 datoshi]
    /// 0F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160ArrayLength")]
    public abstract BigInteger? TestUInt160ArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.SIZE [4 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160ArrayLengthNonNullable")]
    public abstract BigInteger? TestUInt160ArrayLengthNonNullable(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJhlFDAAAAAAAAAAAAAAAAAAAAAAAAAAAQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 19 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 1F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160Default")]
    public abstract UInt160? TestUInt160Default(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160DefaultNonNullable")]
    public abstract UInt160? TestUInt160DefaultNonNullable(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIF 03 [2 datoshi]
    /// 08 : OpCode.SIZE [4 datoshi]
    /// 09 : OpCode.DUP [2 datoshi]
    /// 0A : OpCode.ISNULL [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 0D : OpCode.DROP [2 datoshi]
    /// 0E : OpCode.PUSH0 [1 datoshi]
    /// 0F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256ArrayLength")]
    public abstract BigInteger? TestUInt256ArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.SIZE [4 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256ArrayLengthNonNullable")]
    public abstract BigInteger? TestUInt256ArrayLengthNonNullable(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJiVFDAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 25 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000 [8 datoshi]
    /// 2B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256Default")]
    public abstract UInt256? TestUInt256Default(UInt256? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256DefaultNonNullable")]
    public abstract UInt256? TestUInt256DefaultNonNullable(UInt256? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIFNOT 10 [2 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.ISNULL [2 datoshi]
    /// 12 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 14 : OpCode.THROW [512 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.ISNULL [2 datoshi]
    /// 18 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1A : OpCode.THROW [512 datoshi]
    /// 1B : OpCode.ADD [8 datoshi]
    /// 1C : OpCode.RET [0 datoshi]
    /// 1D : OpCode.PUSH0 [1 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntAdd")]
    public abstract BigInteger? TestUIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.ADD [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntAddNonNullable")]
    public abstract BigInteger? TestUIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIF 04 [2 datoshi]
    /// 0F : OpCode.PUSHF [1 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.ISNULL [2 datoshi]
    /// 14 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 16 : OpCode.THROW [512 datoshi]
    /// 17 : OpCode.LDARG1 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.ISNULL [2 datoshi]
    /// 1A : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1C : OpCode.THROW [512 datoshi]
    /// 1D : OpCode.GT [8 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntCompare")]
    public abstract bool? TestUIntCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.GT [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntCompareNonNullable")]
    public abstract bool? TestUIntCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSH0 [1 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntDefault")]
    public abstract BigInteger? TestUIntDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntDefaultNonNullable")]
    public abstract BigInteger? TestUIntDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIFNOT 10 [2 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.ISNULL [2 datoshi]
    /// 12 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 14 : OpCode.THROW [512 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.ISNULL [2 datoshi]
    /// 18 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1A : OpCode.THROW [512 datoshi]
    /// 1B : OpCode.ADD [8 datoshi]
    /// 1C : OpCode.RET [0 datoshi]
    /// 1D : OpCode.PUSH0 [1 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongAdd")]
    public abstract BigInteger? TestULongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.ADD [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongAddNonNullable")]
    public abstract BigInteger? TestULongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIF 04 [2 datoshi]
    /// 0F : OpCode.PUSHF [1 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.ISNULL [2 datoshi]
    /// 14 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 16 : OpCode.THROW [512 datoshi]
    /// 17 : OpCode.LDARG1 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.ISNULL [2 datoshi]
    /// 1A : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1C : OpCode.THROW [512 datoshi]
    /// 1D : OpCode.GT [8 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongCompare")]
    public abstract bool? TestULongCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.GT [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongCompareNonNullable")]
    public abstract bool? TestULongCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSH0 [1 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongDefault")]
    public abstract BigInteger? TestULongDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongDefaultNonNullable")]
    public abstract BigInteger? TestULongDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIFNOT 10 [2 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.DUP [2 datoshi]
    /// 11 : OpCode.ISNULL [2 datoshi]
    /// 12 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 14 : OpCode.THROW [512 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.ISNULL [2 datoshi]
    /// 18 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1A : OpCode.THROW [512 datoshi]
    /// 1B : OpCode.ADD [8 datoshi]
    /// 1C : OpCode.RET [0 datoshi]
    /// 1D : OpCode.PUSH0 [1 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortAdd")]
    public abstract BigInteger? TestUShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.ADD [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortAddNonNullable")]
    public abstract BigInteger? TestUShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.ISNULL [2 datoshi]
    /// 05 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 07 : OpCode.PUSHF [1 datoshi]
    /// 08 : OpCode.JMP 05 [2 datoshi]
    /// 0A : OpCode.LDARG1 [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.NOT [4 datoshi]
    /// 0D : OpCode.JMPIF 04 [2 datoshi]
    /// 0F : OpCode.PUSHF [1 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.ISNULL [2 datoshi]
    /// 14 : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 16 : OpCode.THROW [512 datoshi]
    /// 17 : OpCode.LDARG1 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.ISNULL [2 datoshi]
    /// 1A : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 1C : OpCode.THROW [512 datoshi]
    /// 1D : OpCode.GT [8 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortCompare")]
    public abstract bool? TestUShortCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.GT [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortCompareNonNullable")]
    public abstract bool? TestUShortCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.ISNULL [2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 08 : OpCode.DROP [2 datoshi]
    /// 09 : OpCode.PUSH0 [1 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortDefault")]
    public abstract BigInteger? TestUShortDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortDefaultNonNullable")]
    public abstract BigInteger? TestUShortDefaultNonNullable(BigInteger? a);

    #endregion
}
