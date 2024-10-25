using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NullableType(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NullableType"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testBigIntegerAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testBigIntegerAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":31,""safe"":false},{""name"":""testBigIntegerCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":38,""safe"":false},{""name"":""testBigIntegerCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":69,""safe"":false},{""name"":""testBigIntegerDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":76,""safe"":false},{""name"":""testBigIntegerDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":87,""safe"":false},{""name"":""testIntAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":92,""safe"":false},{""name"":""testIntAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":169,""safe"":false},{""name"":""testIntCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":222,""safe"":false},{""name"":""testIntCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":253,""safe"":false},{""name"":""testIntDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":260,""safe"":false},{""name"":""testIntDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":271,""safe"":false},{""name"":""testUIntAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":276,""safe"":false},{""name"":""testUIntAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":335,""safe"":false},{""name"":""testUIntCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":370,""safe"":false},{""name"":""testUIntCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":401,""safe"":false},{""name"":""testUIntDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":408,""safe"":false},{""name"":""testUIntDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":419,""safe"":false},{""name"":""testLongAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":424,""safe"":false},{""name"":""testLongAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":529,""safe"":false},{""name"":""testLongCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":610,""safe"":false},{""name"":""testLongCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":641,""safe"":false},{""name"":""testLongDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":648,""safe"":false},{""name"":""testLongDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":659,""safe"":false},{""name"":""testULongAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":664,""safe"":false},{""name"":""testULongAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":739,""safe"":false},{""name"":""testULongCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":790,""safe"":false},{""name"":""testULongCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":821,""safe"":false},{""name"":""testULongDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":828,""safe"":false},{""name"":""testULongDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":839,""safe"":false},{""name"":""testShortAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":844,""safe"":false},{""name"":""testShortAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":953,""safe"":false},{""name"":""testShortCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1038,""safe"":false},{""name"":""testShortCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1069,""safe"":false},{""name"":""testShortDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1076,""safe"":false},{""name"":""testShortDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1087,""safe"":false},{""name"":""testUShortAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1092,""safe"":false},{""name"":""testUShortAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1189,""safe"":false},{""name"":""testUShortCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1262,""safe"":false},{""name"":""testUShortCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1293,""safe"":false},{""name"":""testUShortDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1300,""safe"":false},{""name"":""testUShortDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1311,""safe"":false},{""name"":""testSByteAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1316,""safe"":false},{""name"":""testSByteAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1418,""safe"":false},{""name"":""testSByteCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1496,""safe"":false},{""name"":""testSByteCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1527,""safe"":false},{""name"":""testSByteDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1534,""safe"":false},{""name"":""testSByteDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1545,""safe"":false},{""name"":""testByteAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1550,""safe"":false},{""name"":""testByteAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1643,""safe"":false},{""name"":""testByteCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1712,""safe"":false},{""name"":""testByteCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1743,""safe"":false},{""name"":""testByteDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1750,""safe"":false},{""name"":""testByteDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1761,""safe"":false},{""name"":""testBoolAnd"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1766,""safe"":false},{""name"":""testBoolAndNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1800,""safe"":false},{""name"":""testBoolOr"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1810,""safe"":false},{""name"":""testBoolOrNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1844,""safe"":false},{""name"":""testBoolDefault"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1854,""safe"":false},{""name"":""testBoolDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1865,""safe"":false},{""name"":""testUInt160Default"",""parameters"":[{""name"":""a"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":1870,""safe"":false},{""name"":""testUInt160DefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":1902,""safe"":false},{""name"":""testUInt256Default"",""parameters"":[{""name"":""a"",""type"":""Hash256""}],""returntype"":""Hash256"",""offset"":1907,""safe"":false},{""name"":""testUInt256DefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Hash256""}],""returntype"":""Hash256"",""offset"":1951,""safe"":false},{""name"":""testUInt160ArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1956,""safe"":false},{""name"":""testUInt160ArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1972,""safe"":false},{""name"":""testUInt256ArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1978,""safe"":false},{""name"":""testUInt256ArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1994,""safe"":false},{""name"":""testByteArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2000,""safe"":false},{""name"":""testByteArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":2016,""safe"":false},{""name"":""testStringLength"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Integer"",""offset"":2022,""safe"":false},{""name"":""testStringLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Integer"",""offset"":2038,""safe"":false},{""name"":""testStringDefault"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""String"",""offset"":2044,""safe"":false},{""name"":""testStringDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""String"",""offset"":2056,""safe"":false},{""name"":""testStringConcat"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""}],""returntype"":""String"",""offset"":2061,""safe"":false},{""name"":""testStringConcatNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""}],""returntype"":""String"",""offset"":2084,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0tCFcAAnjYJgUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEBXAAJ4eZ5AVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiY+eErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AEEBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAAnjYJgUJIgV52KokBAlAeErYJgM6eUrYJgM6t0BXAAJ4ebdAVwABeErYJgRFEEBXAAF4QFcAAnjYJgUJIgV52KomLHhK2CYDOnlK2CYDOp5KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRQBBAVwACeHmeShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUBXAAJ42CYFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42CYFCSIFediqJlp4StgmAzp5StgmAzqeSgMAAAAAAAAAgC4EIg5KA/////////9/MjIE//////////8AAAAAAAAAAJFKA/////////9/MhQEAAAAAAAAAAABAAAAAAAAAJ9AEEBXAAJ4eZ5KAwAAAAAAAACALgQiDkoD/////////38yMgT//////////wAAAAAAAAAAkUoD/////////38yFAQAAAAAAAAAAAEAAAAAAAAAn0BXAAJ42CYFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42CYFCSIFediqJjx4StgmAzp5StgmAzqeShAuBCIWSgT//////////wAAAAAAAAAAMhQE//////////8AAAAAAAAAAJFAEEBXAAJ4eZ5KEC4EIhZKBP//////////AAAAAAAAAAAyFAT//////////wAAAAAAAAAAkUBXAAJ42CYFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42CYFCSIFediqJl54StgmAzp5StgmAzqeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oBAIAuBCIISgH/fzIUAv//AACRSgH/fzIIAgAAAQCfQBBAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oBAIAuBCIISgH/fzIUAv//AACRSgH/fzIIAgAAAQCfQFcAAnjYJgUJIgV52KokBAlAeErYJgM6eUrYJgM6t0BXAAJ4ebdAVwABeErYJgRFEEBXAAF4QFcAAnjYJgUJIgV52KomUnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIKSgL//wAAMggC//8AAJFAEEBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIKSgL//wAAMggC//8AAJFAVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiZXeErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KAIAuBCIHSgB/Mg8B/wCRSgB/MgYBAAGfQBBAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oAgC4EIgdKAH8yDwH/AJFKAH8yBgEAAZ9AVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiZOeErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KEC4EIghKAf8AMgYB/wCRQBBAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCEoB/wAyBgH/AJFAVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiQECUB4StgmAzokBAlAeUrYJgM6QFcAAngkBAlAeUBXAAJ42CYFCSIFediqJAQJQHhK2CYDOiYECEB5StgmAzpAVwACeCYECEB5QFcAAXhK2CYERQlAVwABeEBXAAF4StgmGUUMFAAAAAAAAAAAAAAAAAAAAAAAAAAAQFcAAXhAVwABeErYJiVFDCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBXAAF4QFcAAXhK2CQDykrYJgRFEEBXAAF4ykBXAAF4StgkA8pK2CYERRBAVwABeMpAVwABeErYJAPKStgmBEUQQFcAAXjKQFcAAXhK2CQDykrYJgRFEEBXAAF4ykBXAAF4StgmBUUMAEBXAAF4QFcAAnhK2CYFRQwAeUrYJgVFDACL2yhAVwACeHmL2yhA9utLJA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIFNOT 10
    /// 0F : OpCode.LDARG0
    /// 10 : OpCode.DUP
    /// 11 : OpCode.ISNULL
    /// 12 : OpCode.JMPIFNOT 03
    /// 14 : OpCode.THROW
    /// 15 : OpCode.LDARG1
    /// 16 : OpCode.DUP
    /// 17 : OpCode.ISNULL
    /// 18 : OpCode.JMPIFNOT 03
    /// 1A : OpCode.THROW
    /// 1B : OpCode.ADD
    /// 1C : OpCode.RET
    /// 1D : OpCode.PUSH0
    /// 1E : OpCode.RET
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
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIF 04
    /// 0F : OpCode.PUSHF
    /// 10 : OpCode.RET
    /// 11 : OpCode.LDARG0
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ISNULL
    /// 14 : OpCode.JMPIFNOT 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.LDARG1
    /// 18 : OpCode.DUP
    /// 19 : OpCode.ISNULL
    /// 1A : OpCode.JMPIFNOT 03
    /// 1C : OpCode.THROW
    /// 1D : OpCode.GT
    /// 1E : OpCode.RET
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
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzokBAlAeUrYJgM6QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIF 04
    /// 0F : OpCode.PUSHF
    /// 10 : OpCode.RET
    /// 11 : OpCode.LDARG0
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ISNULL
    /// 14 : OpCode.JMPIFNOT 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.JMPIF 04
    /// 19 : OpCode.PUSHF
    /// 1A : OpCode.RET
    /// 1B : OpCode.LDARG1
    /// 1C : OpCode.DUP
    /// 1D : OpCode.ISNULL
    /// 1E : OpCode.JMPIFNOT 03
    /// 20 : OpCode.THROW
    /// 21 : OpCode.RET
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
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzomBAhAeUrYJgM6QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIF 04
    /// 0F : OpCode.PUSHF
    /// 10 : OpCode.RET
    /// 11 : OpCode.LDARG0
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ISNULL
    /// 14 : OpCode.JMPIFNOT 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.JMPIFNOT 04
    /// 19 : OpCode.PUSHT
    /// 1A : OpCode.RET
    /// 1B : OpCode.LDARG1
    /// 1C : OpCode.DUP
    /// 1D : OpCode.ISNULL
    /// 1E : OpCode.JMPIFNOT 03
    /// 20 : OpCode.THROW
    /// 21 : OpCode.RET
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
    /// Script: VwACeNgmBQkiBXnYqiZOeErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KEC4EIghKAf8AMgYB/wCRQBBA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIFNOT 4E
    /// 0F : OpCode.LDARG0
    /// 10 : OpCode.DUP
    /// 11 : OpCode.ISNULL
    /// 12 : OpCode.JMPIFNOT 03
    /// 14 : OpCode.THROW
    /// 15 : OpCode.LDARG1
    /// 16 : OpCode.DUP
    /// 17 : OpCode.ISNULL
    /// 18 : OpCode.JMPIFNOT 03
    /// 1A : OpCode.THROW
    /// 1B : OpCode.ADD
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSHINT32 00000080
    /// 22 : OpCode.JMPGE 04
    /// 24 : OpCode.JMP 0A
    /// 26 : OpCode.DUP
    /// 27 : OpCode.PUSHINT32 FFFFFF7F
    /// 2C : OpCode.JMPLE 1E
    /// 2E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 37 : OpCode.AND
    /// 38 : OpCode.DUP
    /// 39 : OpCode.PUSHINT32 FFFFFF7F
    /// 3E : OpCode.JMPLE 0C
    /// 40 : OpCode.PUSHINT64 0000000001000000
    /// 49 : OpCode.SUB
    /// 4A : OpCode.DUP
    /// 4B : OpCode.PUSH0
    /// 4C : OpCode.JMPGE 04
    /// 4E : OpCode.JMP 08
    /// 50 : OpCode.DUP
    /// 51 : OpCode.PUSHINT16 FF00
    /// 54 : OpCode.JMPLE 06
    /// 56 : OpCode.PUSHINT16 FF00
    /// 59 : OpCode.AND
    /// 5A : OpCode.RET
    /// 5B : OpCode.PUSH0
    /// 5C : OpCode.RET
    /// </remarks>
    [DisplayName("testByteAdd")]
    public abstract BigInteger? TestByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCEoB/wAyBgH/AJFA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSHINT32 00000080
    /// 0C : OpCode.JMPGE 04
    /// 0E : OpCode.JMP 0A
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT32 FFFFFF7F
    /// 16 : OpCode.JMPLE 1E
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT32 FFFFFF7F
    /// 28 : OpCode.JMPLE 0C
    /// 2A : OpCode.PUSHINT64 0000000001000000
    /// 33 : OpCode.SUB
    /// 34 : OpCode.DUP
    /// 35 : OpCode.PUSH0
    /// 36 : OpCode.JMPGE 04
    /// 38 : OpCode.JMP 08
    /// 3A : OpCode.DUP
    /// 3B : OpCode.PUSHINT16 FF00
    /// 3E : OpCode.JMPLE 06
    /// 40 : OpCode.PUSHINT16 FF00
    /// 43 : OpCode.AND
    /// 44 : OpCode.RET
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
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIF 04
    /// 0F : OpCode.PUSHF
    /// 10 : OpCode.RET
    /// 11 : OpCode.LDARG0
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ISNULL
    /// 14 : OpCode.JMPIFNOT 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.LDARG1
    /// 18 : OpCode.DUP
    /// 19 : OpCode.ISNULL
    /// 1A : OpCode.JMPIFNOT 03
    /// 1C : OpCode.THROW
    /// 1D : OpCode.GT
    /// 1E : OpCode.RET
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
    /// Script: VwACeNgmBQkiBXnYqiY+eErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AEEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIFNOT 3E
    /// 0F : OpCode.LDARG0
    /// 10 : OpCode.DUP
    /// 11 : OpCode.ISNULL
    /// 12 : OpCode.JMPIFNOT 03
    /// 14 : OpCode.THROW
    /// 15 : OpCode.LDARG1
    /// 16 : OpCode.DUP
    /// 17 : OpCode.ISNULL
    /// 18 : OpCode.JMPIFNOT 03
    /// 1A : OpCode.THROW
    /// 1B : OpCode.ADD
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSHINT32 00000080
    /// 22 : OpCode.JMPGE 04
    /// 24 : OpCode.JMP 0A
    /// 26 : OpCode.DUP
    /// 27 : OpCode.PUSHINT32 FFFFFF7F
    /// 2C : OpCode.JMPLE 1E
    /// 2E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 37 : OpCode.AND
    /// 38 : OpCode.DUP
    /// 39 : OpCode.PUSHINT32 FFFFFF7F
    /// 3E : OpCode.JMPLE 0C
    /// 40 : OpCode.PUSHINT64 0000000001000000
    /// 49 : OpCode.SUB
    /// 4A : OpCode.RET
    /// 4B : OpCode.PUSH0
    /// 4C : OpCode.RET
    /// </remarks>
    [DisplayName("testIntAdd")]
    public abstract BigInteger? TestIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSHINT32 00000080
    /// 0C : OpCode.JMPGE 04
    /// 0E : OpCode.JMP 0A
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT32 FFFFFF7F
    /// 16 : OpCode.JMPLE 1E
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT32 FFFFFF7F
    /// 28 : OpCode.JMPLE 0C
    /// 2A : OpCode.PUSHINT64 0000000001000000
    /// 33 : OpCode.SUB
    /// 34 : OpCode.RET
    /// </remarks>
    [DisplayName("testIntAddNonNullable")]
    public abstract BigInteger? TestIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIF 04
    /// 0F : OpCode.PUSHF
    /// 10 : OpCode.RET
    /// 11 : OpCode.LDARG0
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ISNULL
    /// 14 : OpCode.JMPIFNOT 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.LDARG1
    /// 18 : OpCode.DUP
    /// 19 : OpCode.ISNULL
    /// 1A : OpCode.JMPIFNOT 03
    /// 1C : OpCode.THROW
    /// 1D : OpCode.GT
    /// 1E : OpCode.RET
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
    /// Script: VwACeNgmBQkiBXnYqiZaeErYJgM6eUrYJgM6nkoDAAAAAAAAAIAuBCIOSgP/////////fzIyBP//////////AAAAAAAAAACRSgP/////////fzIUBAAAAAAAAAAAAQAAAAAAAACfQBBA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIFNOT 5A
    /// 0F : OpCode.LDARG0
    /// 10 : OpCode.DUP
    /// 11 : OpCode.ISNULL
    /// 12 : OpCode.JMPIFNOT 03
    /// 14 : OpCode.THROW
    /// 15 : OpCode.LDARG1
    /// 16 : OpCode.DUP
    /// 17 : OpCode.ISNULL
    /// 18 : OpCode.JMPIFNOT 03
    /// 1A : OpCode.THROW
    /// 1B : OpCode.ADD
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSHINT64 0000000000000080
    /// 26 : OpCode.JMPGE 04
    /// 28 : OpCode.JMP 0E
    /// 2A : OpCode.DUP
    /// 2B : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 34 : OpCode.JMPLE 32
    /// 36 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 47 : OpCode.AND
    /// 48 : OpCode.DUP
    /// 49 : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 52 : OpCode.JMPLE 14
    /// 54 : OpCode.PUSHINT128 00000000000000000100000000000000
    /// 65 : OpCode.SUB
    /// 66 : OpCode.RET
    /// 67 : OpCode.PUSH0
    /// 68 : OpCode.RET
    /// </remarks>
    [DisplayName("testLongAdd")]
    public abstract BigInteger? TestLongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgMAAAAAAAAAgC4EIg5KA/////////9/MjIE//////////8AAAAAAAAAAJFKA/////////9/MhQEAAAAAAAAAAABAAAAAAAAAJ9A
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSHINT64 0000000000000080
    /// 10 : OpCode.JMPGE 04
    /// 12 : OpCode.JMP 0E
    /// 14 : OpCode.DUP
    /// 15 : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 1E : OpCode.JMPLE 32
    /// 20 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 31 : OpCode.AND
    /// 32 : OpCode.DUP
    /// 33 : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 3C : OpCode.JMPLE 14
    /// 3E : OpCode.PUSHINT128 00000000000000000100000000000000
    /// 4F : OpCode.SUB
    /// 50 : OpCode.RET
    /// </remarks>
    [DisplayName("testLongAddNonNullable")]
    public abstract BigInteger? TestLongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIF 04
    /// 0F : OpCode.PUSHF
    /// 10 : OpCode.RET
    /// 11 : OpCode.LDARG0
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ISNULL
    /// 14 : OpCode.JMPIFNOT 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.LDARG1
    /// 18 : OpCode.DUP
    /// 19 : OpCode.ISNULL
    /// 1A : OpCode.JMPIFNOT 03
    /// 1C : OpCode.THROW
    /// 1D : OpCode.GT
    /// 1E : OpCode.RET
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
    /// Script: VwACeNgmBQkiBXnYqiZXeErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KAIAuBCIHSgB/Mg8B/wCRSgB/MgYBAAGfQBBA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIFNOT 57
    /// 0F : OpCode.LDARG0
    /// 10 : OpCode.DUP
    /// 11 : OpCode.ISNULL
    /// 12 : OpCode.JMPIFNOT 03
    /// 14 : OpCode.THROW
    /// 15 : OpCode.LDARG1
    /// 16 : OpCode.DUP
    /// 17 : OpCode.ISNULL
    /// 18 : OpCode.JMPIFNOT 03
    /// 1A : OpCode.THROW
    /// 1B : OpCode.ADD
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSHINT32 00000080
    /// 22 : OpCode.JMPGE 04
    /// 24 : OpCode.JMP 0A
    /// 26 : OpCode.DUP
    /// 27 : OpCode.PUSHINT32 FFFFFF7F
    /// 2C : OpCode.JMPLE 1E
    /// 2E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 37 : OpCode.AND
    /// 38 : OpCode.DUP
    /// 39 : OpCode.PUSHINT32 FFFFFF7F
    /// 3E : OpCode.JMPLE 0C
    /// 40 : OpCode.PUSHINT64 0000000001000000
    /// 49 : OpCode.SUB
    /// 4A : OpCode.DUP
    /// 4B : OpCode.PUSHINT8 80
    /// 4D : OpCode.JMPGE 04
    /// 4F : OpCode.JMP 07
    /// 51 : OpCode.DUP
    /// 52 : OpCode.PUSHINT8 7F
    /// 54 : OpCode.JMPLE 0F
    /// 56 : OpCode.PUSHINT16 FF00
    /// 59 : OpCode.AND
    /// 5A : OpCode.DUP
    /// 5B : OpCode.PUSHINT8 7F
    /// 5D : OpCode.JMPLE 06
    /// 5F : OpCode.PUSHINT16 0001
    /// 62 : OpCode.SUB
    /// 63 : OpCode.RET
    /// 64 : OpCode.PUSH0
    /// 65 : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteAdd")]
    public abstract BigInteger? TestSByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oAgC4EIgdKAH8yDwH/AJFKAH8yBgEAAZ9A
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSHINT32 00000080
    /// 0C : OpCode.JMPGE 04
    /// 0E : OpCode.JMP 0A
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT32 FFFFFF7F
    /// 16 : OpCode.JMPLE 1E
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT32 FFFFFF7F
    /// 28 : OpCode.JMPLE 0C
    /// 2A : OpCode.PUSHINT64 0000000001000000
    /// 33 : OpCode.SUB
    /// 34 : OpCode.DUP
    /// 35 : OpCode.PUSHINT8 80
    /// 37 : OpCode.JMPGE 04
    /// 39 : OpCode.JMP 07
    /// 3B : OpCode.DUP
    /// 3C : OpCode.PUSHINT8 7F
    /// 3E : OpCode.JMPLE 0F
    /// 40 : OpCode.PUSHINT16 FF00
    /// 43 : OpCode.AND
    /// 44 : OpCode.DUP
    /// 45 : OpCode.PUSHINT8 7F
    /// 47 : OpCode.JMPLE 06
    /// 49 : OpCode.PUSHINT16 0001
    /// 4C : OpCode.SUB
    /// 4D : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteAddNonNullable")]
    public abstract BigInteger? TestSByteAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIF 04
    /// 0F : OpCode.PUSHF
    /// 10 : OpCode.RET
    /// 11 : OpCode.LDARG0
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ISNULL
    /// 14 : OpCode.JMPIFNOT 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.LDARG1
    /// 18 : OpCode.DUP
    /// 19 : OpCode.ISNULL
    /// 1A : OpCode.JMPIFNOT 03
    /// 1C : OpCode.THROW
    /// 1D : OpCode.GT
    /// 1E : OpCode.RET
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
    /// Script: VwACeNgmBQkiBXnYqiZeeErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KAQCALgQiCEoB/38yFAL//wAAkUoB/38yCAIAAAEAn0AQQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIFNOT 5E
    /// 0F : OpCode.LDARG0
    /// 10 : OpCode.DUP
    /// 11 : OpCode.ISNULL
    /// 12 : OpCode.JMPIFNOT 03
    /// 14 : OpCode.THROW
    /// 15 : OpCode.LDARG1
    /// 16 : OpCode.DUP
    /// 17 : OpCode.ISNULL
    /// 18 : OpCode.JMPIFNOT 03
    /// 1A : OpCode.THROW
    /// 1B : OpCode.ADD
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSHINT32 00000080
    /// 22 : OpCode.JMPGE 04
    /// 24 : OpCode.JMP 0A
    /// 26 : OpCode.DUP
    /// 27 : OpCode.PUSHINT32 FFFFFF7F
    /// 2C : OpCode.JMPLE 1E
    /// 2E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 37 : OpCode.AND
    /// 38 : OpCode.DUP
    /// 39 : OpCode.PUSHINT32 FFFFFF7F
    /// 3E : OpCode.JMPLE 0C
    /// 40 : OpCode.PUSHINT64 0000000001000000
    /// 49 : OpCode.SUB
    /// 4A : OpCode.DUP
    /// 4B : OpCode.PUSHINT16 0080
    /// 4E : OpCode.JMPGE 04
    /// 50 : OpCode.JMP 08
    /// 52 : OpCode.DUP
    /// 53 : OpCode.PUSHINT16 FF7F
    /// 56 : OpCode.JMPLE 14
    /// 58 : OpCode.PUSHINT32 FFFF0000
    /// 5D : OpCode.AND
    /// 5E : OpCode.DUP
    /// 5F : OpCode.PUSHINT16 FF7F
    /// 62 : OpCode.JMPLE 08
    /// 64 : OpCode.PUSHINT32 00000100
    /// 69 : OpCode.SUB
    /// 6A : OpCode.RET
    /// 6B : OpCode.PUSH0
    /// 6C : OpCode.RET
    /// </remarks>
    [DisplayName("testShortAdd")]
    public abstract BigInteger? TestShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oBAIAuBCIISgH/fzIUAv//AACRSgH/fzIIAgAAAQCfQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSHINT32 00000080
    /// 0C : OpCode.JMPGE 04
    /// 0E : OpCode.JMP 0A
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT32 FFFFFF7F
    /// 16 : OpCode.JMPLE 1E
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT32 FFFFFF7F
    /// 28 : OpCode.JMPLE 0C
    /// 2A : OpCode.PUSHINT64 0000000001000000
    /// 33 : OpCode.SUB
    /// 34 : OpCode.DUP
    /// 35 : OpCode.PUSHINT16 0080
    /// 38 : OpCode.JMPGE 04
    /// 3A : OpCode.JMP 08
    /// 3C : OpCode.DUP
    /// 3D : OpCode.PUSHINT16 FF7F
    /// 40 : OpCode.JMPLE 14
    /// 42 : OpCode.PUSHINT32 FFFF0000
    /// 47 : OpCode.AND
    /// 48 : OpCode.DUP
    /// 49 : OpCode.PUSHINT16 FF7F
    /// 4C : OpCode.JMPLE 08
    /// 4E : OpCode.PUSHINT32 00000100
    /// 53 : OpCode.SUB
    /// 54 : OpCode.RET
    /// </remarks>
    [DisplayName("testShortAddNonNullable")]
    public abstract BigInteger? TestShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIF 04
    /// 0F : OpCode.PUSHF
    /// 10 : OpCode.RET
    /// 11 : OpCode.LDARG0
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ISNULL
    /// 14 : OpCode.JMPIFNOT 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.LDARG1
    /// 18 : OpCode.DUP
    /// 19 : OpCode.ISNULL
    /// 1A : OpCode.JMPIFNOT 03
    /// 1C : OpCode.THROW
    /// 1D : OpCode.GT
    /// 1E : OpCode.RET
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
    /// Script: VwACeNgmBQkiBXnYqiYseErYJgM6eUrYJgM6nkoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFAEEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIFNOT 2C
    /// 0F : OpCode.LDARG0
    /// 10 : OpCode.DUP
    /// 11 : OpCode.ISNULL
    /// 12 : OpCode.JMPIFNOT 03
    /// 14 : OpCode.THROW
    /// 15 : OpCode.LDARG1
    /// 16 : OpCode.DUP
    /// 17 : OpCode.ISNULL
    /// 18 : OpCode.JMPIFNOT 03
    /// 1A : OpCode.THROW
    /// 1B : OpCode.ADD
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSH0
    /// 1E : OpCode.JMPGE 04
    /// 20 : OpCode.JMP 0E
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2C : OpCode.JMPLE 0C
    /// 2E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 37 : OpCode.AND
    /// 38 : OpCode.RET
    /// 39 : OpCode.PUSH0
    /// 3A : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntAdd")]
    public abstract BigInteger? TestUIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSH0
    /// 08 : OpCode.JMPGE 04
    /// 0A : OpCode.JMP 0E
    /// 0C : OpCode.DUP
    /// 0D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 16 : OpCode.JMPLE 0C
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntAddNonNullable")]
    public abstract BigInteger? TestUIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIF 04
    /// 0F : OpCode.PUSHF
    /// 10 : OpCode.RET
    /// 11 : OpCode.LDARG0
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ISNULL
    /// 14 : OpCode.JMPIFNOT 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.LDARG1
    /// 18 : OpCode.DUP
    /// 19 : OpCode.ISNULL
    /// 1A : OpCode.JMPIFNOT 03
    /// 1C : OpCode.THROW
    /// 1D : OpCode.GT
    /// 1E : OpCode.RET
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
    /// Script: VwACeNgmBQkiBXnYqiY8eErYJgM6eUrYJgM6nkoQLgQiFkoE//////////8AAAAAAAAAADIUBP//////////AAAAAAAAAACRQBBA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIFNOT 3C
    /// 0F : OpCode.LDARG0
    /// 10 : OpCode.DUP
    /// 11 : OpCode.ISNULL
    /// 12 : OpCode.JMPIFNOT 03
    /// 14 : OpCode.THROW
    /// 15 : OpCode.LDARG1
    /// 16 : OpCode.DUP
    /// 17 : OpCode.ISNULL
    /// 18 : OpCode.JMPIFNOT 03
    /// 1A : OpCode.THROW
    /// 1B : OpCode.ADD
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSH0
    /// 1E : OpCode.JMPGE 04
    /// 20 : OpCode.JMP 16
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 34 : OpCode.JMPLE 14
    /// 36 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 47 : OpCode.AND
    /// 48 : OpCode.RET
    /// 49 : OpCode.PUSH0
    /// 4A : OpCode.RET
    /// </remarks>
    [DisplayName("testULongAdd")]
    public abstract BigInteger? TestULongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeShAuBCIWSgT//////////wAAAAAAAAAAMhQE//////////8AAAAAAAAAAJFA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSH0
    /// 08 : OpCode.JMPGE 04
    /// 0A : OpCode.JMP 16
    /// 0C : OpCode.DUP
    /// 0D : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 1E : OpCode.JMPLE 14
    /// 20 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 31 : OpCode.AND
    /// 32 : OpCode.RET
    /// </remarks>
    [DisplayName("testULongAddNonNullable")]
    public abstract BigInteger? TestULongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIF 04
    /// 0F : OpCode.PUSHF
    /// 10 : OpCode.RET
    /// 11 : OpCode.LDARG0
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ISNULL
    /// 14 : OpCode.JMPIFNOT 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.LDARG1
    /// 18 : OpCode.DUP
    /// 19 : OpCode.ISNULL
    /// 1A : OpCode.JMPIFNOT 03
    /// 1C : OpCode.THROW
    /// 1D : OpCode.GT
    /// 1E : OpCode.RET
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
    /// Script: VwACeNgmBQkiBXnYqiZSeErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KEC4EIgpKAv//AAAyCAL//wAAkUAQQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIFNOT 52
    /// 0F : OpCode.LDARG0
    /// 10 : OpCode.DUP
    /// 11 : OpCode.ISNULL
    /// 12 : OpCode.JMPIFNOT 03
    /// 14 : OpCode.THROW
    /// 15 : OpCode.LDARG1
    /// 16 : OpCode.DUP
    /// 17 : OpCode.ISNULL
    /// 18 : OpCode.JMPIFNOT 03
    /// 1A : OpCode.THROW
    /// 1B : OpCode.ADD
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSHINT32 00000080
    /// 22 : OpCode.JMPGE 04
    /// 24 : OpCode.JMP 0A
    /// 26 : OpCode.DUP
    /// 27 : OpCode.PUSHINT32 FFFFFF7F
    /// 2C : OpCode.JMPLE 1E
    /// 2E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 37 : OpCode.AND
    /// 38 : OpCode.DUP
    /// 39 : OpCode.PUSHINT32 FFFFFF7F
    /// 3E : OpCode.JMPLE 0C
    /// 40 : OpCode.PUSHINT64 0000000001000000
    /// 49 : OpCode.SUB
    /// 4A : OpCode.DUP
    /// 4B : OpCode.PUSH0
    /// 4C : OpCode.JMPGE 04
    /// 4E : OpCode.JMP 0A
    /// 50 : OpCode.DUP
    /// 51 : OpCode.PUSHINT32 FFFF0000
    /// 56 : OpCode.JMPLE 08
    /// 58 : OpCode.PUSHINT32 FFFF0000
    /// 5D : OpCode.AND
    /// 5E : OpCode.RET
    /// 5F : OpCode.PUSH0
    /// 60 : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortAdd")]
    public abstract BigInteger? TestUShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCkoC//8AADIIAv//AACRQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.ADD
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSHINT32 00000080
    /// 0C : OpCode.JMPGE 04
    /// 0E : OpCode.JMP 0A
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHINT32 FFFFFF7F
    /// 16 : OpCode.JMPLE 1E
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT32 FFFFFF7F
    /// 28 : OpCode.JMPLE 0C
    /// 2A : OpCode.PUSHINT64 0000000001000000
    /// 33 : OpCode.SUB
    /// 34 : OpCode.DUP
    /// 35 : OpCode.PUSH0
    /// 36 : OpCode.JMPGE 04
    /// 38 : OpCode.JMP 0A
    /// 3A : OpCode.DUP
    /// 3B : OpCode.PUSHINT32 FFFF0000
    /// 40 : OpCode.JMPLE 08
    /// 42 : OpCode.PUSHINT32 FFFF0000
    /// 47 : OpCode.AND
    /// 48 : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortAddNonNullable")]
    public abstract BigInteger? TestUShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.ISNULL
    /// 05 : OpCode.JMPIFNOT 05
    /// 07 : OpCode.PUSHF
    /// 08 : OpCode.JMP 05
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.ISNULL
    /// 0C : OpCode.NOT
    /// 0D : OpCode.JMPIF 04
    /// 0F : OpCode.PUSHF
    /// 10 : OpCode.RET
    /// 11 : OpCode.LDARG0
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ISNULL
    /// 14 : OpCode.JMPIFNOT 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.LDARG1
    /// 18 : OpCode.DUP
    /// 19 : OpCode.ISNULL
    /// 1A : OpCode.JMPIFNOT 03
    /// 1C : OpCode.THROW
    /// 1D : OpCode.GT
    /// 1E : OpCode.RET
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
