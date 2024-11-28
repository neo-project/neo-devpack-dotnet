using Neo.Cryptography.ECC;
using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0tCFcAAnjYJgUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEBXAAJ4eZ5AVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiY+eErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AEEBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAAnjYJgUJIgV52KokBAlAeErYJgM6eUrYJgM6t0BXAAJ4ebdAVwABeErYJgRFEEBXAAF4QFcAAnjYJgUJIgV52KomLHhK2CYDOnlK2CYDOp5KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRQBBAVwACeHmeShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUBXAAJ42CYFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42CYFCSIFediqJlp4StgmAzp5StgmAzqeSgMAAAAAAAAAgC4EIg5KA/////////9/MjIE//////////8AAAAAAAAAAJFKA/////////9/MhQEAAAAAAAAAAABAAAAAAAAAJ9AEEBXAAJ4eZ5KAwAAAAAAAACALgQiDkoD/////////38yMgT//////////wAAAAAAAAAAkUoD/////////38yFAQAAAAAAAAAAAEAAAAAAAAAn0BXAAJ42CYFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42CYFCSIFediqJjx4StgmAzp5StgmAzqeShAuBCIWSgT//////////wAAAAAAAAAAMhQE//////////8AAAAAAAAAAJFAEEBXAAJ4eZ5KEC4EIhZKBP//////////AAAAAAAAAAAyFAT//////////wAAAAAAAAAAkUBXAAJ42CYFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42CYFCSIFediqJl54StgmAzp5StgmAzqeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oBAIAuBCIISgH/fzIUAv//AACRSgH/fzIIAgAAAQCfQBBAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oBAIAuBCIISgH/fzIUAv//AACRSgH/fzIIAgAAAQCfQFcAAnjYJgUJIgV52KokBAlAeErYJgM6eUrYJgM6t0BXAAJ4ebdAVwABeErYJgRFEEBXAAF4QFcAAnjYJgUJIgV52KomUnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIKSgL//wAAMggC//8AAJFAEEBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIKSgL//wAAMggC//8AAJFAVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiZXeErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KAIAuBCIHSgB/Mg8B/wCRSgB/MgYBAAGfQBBAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oAgC4EIgdKAH8yDwH/AJFKAH8yBgEAAZ9AVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiZOeErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KEC4EIghKAf8AMgYB/wCRQBBAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCEoB/wAyBgH/AJFAVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiQECUB4StgmAzokBAlAeUrYJgM6QFcAAngkBAlAeUBXAAJ42CYFCSIFediqJAQJQHhK2CYDOiYECEB5StgmAzpAVwACeCYECEB5QFcAAXhK2CYERQlAVwABeEBXAAF4StgmGUUMFAAAAAAAAAAAAAAAAAAAAAAAAAAAQFcAAXhAVwABeErYJiVFDCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBXAAF4QFcAAXhK2CQDykrYJgRFEEBXAAF4ykBXAAF4StgkA8pK2CYERRBAVwABeMpAVwABeErYJAPKStgmBEUQQFcAAXjKQFcAAXhK2CQDykrYJgRFEEBXAAF4ykBXAAF4StgmBUUMAEBXAAF4QFcAAnhK2CYFRQwAeUrYJgVFDACL2yhAVwACeHmL2yhA9utLJA==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIFNOT 10 [2 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : ISNULL [2 datoshi]
    /// 12 : JMPIFNOT 03 [2 datoshi]
    /// 14 : THROW [512 datoshi]
    /// 15 : LDARG1 [2 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : ISNULL [2 datoshi]
    /// 18 : JMPIFNOT 03 [2 datoshi]
    /// 1A : THROW [512 datoshi]
    /// 1B : ADD [8 datoshi]
    /// 1C : RET [0 datoshi]
    /// 1D : PUSH0 [1 datoshi]
    /// 1E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerAdd")]
    public abstract BigInteger? TestBigIntegerAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerAddNonNullable")]
    public abstract BigInteger? TestBigIntegerAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIF 04 [2 datoshi]
    /// 0F : PUSHF [1 datoshi]
    /// 10 : RET [0 datoshi]
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ISNULL [2 datoshi]
    /// 14 : JMPIFNOT 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : LDARG1 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : ISNULL [2 datoshi]
    /// 1A : JMPIFNOT 03 [2 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : GT [8 datoshi]
    /// 1E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerCompare")]
    public abstract bool? TestBigIntegerCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : GT [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerCompareNonNullable")]
    public abstract bool? TestBigIntegerCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 04 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerDefault")]
    public abstract BigInteger? TestBigIntegerDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerDefaultNonNullable")]
    public abstract BigInteger? TestBigIntegerDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzokBAlAeUrYJgM6QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIF 04 [2 datoshi]
    /// 0F : PUSHF [1 datoshi]
    /// 10 : RET [0 datoshi]
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ISNULL [2 datoshi]
    /// 14 : JMPIFNOT 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : JMPIF 04 [2 datoshi]
    /// 19 : PUSHF [1 datoshi]
    /// 1A : RET [0 datoshi]
    /// 1B : LDARG1 [2 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : ISNULL [2 datoshi]
    /// 1E : JMPIFNOT 03 [2 datoshi]
    /// 20 : THROW [512 datoshi]
    /// 21 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolAnd")]
    public abstract bool? TestBoolAnd(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCQECUB5QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : JMPIF 04 [2 datoshi]
    /// 06 : PUSHF [1 datoshi]
    /// 07 : RET [0 datoshi]
    /// 08 : LDARG1 [2 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolAndNonNullable")]
    public abstract bool? TestBoolAndNonNullable(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFCUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 04 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSHF [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolDefault")]
    public abstract bool? TestBoolDefault(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolDefaultNonNullable")]
    public abstract bool? TestBoolDefaultNonNullable(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzomBAhAeUrYJgM6QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIF 04 [2 datoshi]
    /// 0F : PUSHF [1 datoshi]
    /// 10 : RET [0 datoshi]
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ISNULL [2 datoshi]
    /// 14 : JMPIFNOT 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : JMPIFNOT 04 [2 datoshi]
    /// 19 : PUSHT [1 datoshi]
    /// 1A : RET [0 datoshi]
    /// 1B : LDARG1 [2 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : ISNULL [2 datoshi]
    /// 1E : JMPIFNOT 03 [2 datoshi]
    /// 20 : THROW [512 datoshi]
    /// 21 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolOr")]
    public abstract bool? TestBoolOr(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCYECEB5QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : JMPIFNOT 04 [2 datoshi]
    /// 06 : PUSHT [1 datoshi]
    /// 07 : RET [0 datoshi]
    /// 08 : LDARG1 [2 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolOrNonNullable")]
    public abstract bool? TestBoolOrNonNullable(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiZOeErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KEC4EIghKAf8AMgYB/wCRQBBA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIFNOT 4E [2 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : ISNULL [2 datoshi]
    /// 12 : JMPIFNOT 03 [2 datoshi]
    /// 14 : THROW [512 datoshi]
    /// 15 : LDARG1 [2 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : ISNULL [2 datoshi]
    /// 18 : JMPIFNOT 03 [2 datoshi]
    /// 1A : THROW [512 datoshi]
    /// 1B : ADD [8 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSHINT32 00000080 [1 datoshi]
    /// 22 : JMPGE 04 [2 datoshi]
    /// 24 : JMP 0A [2 datoshi]
    /// 26 : DUP [2 datoshi]
    /// 27 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2C : JMPLE 1E [2 datoshi]
    /// 2E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 37 : AND [8 datoshi]
    /// 38 : DUP [2 datoshi]
    /// 39 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3E : JMPLE 0C [2 datoshi]
    /// 40 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 49 : SUB [8 datoshi]
    /// 4A : DUP [2 datoshi]
    /// 4B : PUSH0 [1 datoshi]
    /// 4C : JMPGE 04 [2 datoshi]
    /// 4E : JMP 08 [2 datoshi]
    /// 50 : DUP [2 datoshi]
    /// 51 : PUSHINT16 FF00 [1 datoshi]
    /// 54 : JMPLE 06 [2 datoshi]
    /// 56 : PUSHINT16 FF00 [1 datoshi]
    /// 59 : AND [8 datoshi]
    /// 5A : RET [0 datoshi]
    /// 5B : PUSH0 [1 datoshi]
    /// 5C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteAdd")]
    public abstract BigInteger? TestByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCEoB/wAyBgH/AJFA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSHINT32 00000080 [1 datoshi]
    /// 0C : JMPGE 04 [2 datoshi]
    /// 0E : JMP 0A [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 16 : JMPLE 1E [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : JMPLE 0C [2 datoshi]
    /// 2A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 33 : SUB [8 datoshi]
    /// 34 : DUP [2 datoshi]
    /// 35 : PUSH0 [1 datoshi]
    /// 36 : JMPGE 04 [2 datoshi]
    /// 38 : JMP 08 [2 datoshi]
    /// 3A : DUP [2 datoshi]
    /// 3B : PUSHINT16 FF00 [1 datoshi]
    /// 3E : JMPLE 06 [2 datoshi]
    /// 40 : PUSHINT16 FF00 [1 datoshi]
    /// 43 : AND [8 datoshi]
    /// 44 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteAddNonNullable")]
    public abstract BigInteger? TestByteAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 03 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : JMPIFNOT 04 [2 datoshi]
    /// 0D : DROP [2 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayLength")]
    public abstract BigInteger? TestByteArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SIZE [4 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayLengthNonNullable")]
    public abstract BigInteger? TestByteArrayLengthNonNullable(byte[]? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIF 04 [2 datoshi]
    /// 0F : PUSHF [1 datoshi]
    /// 10 : RET [0 datoshi]
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ISNULL [2 datoshi]
    /// 14 : JMPIFNOT 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : LDARG1 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : ISNULL [2 datoshi]
    /// 1A : JMPIFNOT 03 [2 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : GT [8 datoshi]
    /// 1E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteCompare")]
    public abstract bool? TestByteCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : GT [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteCompareNonNullable")]
    public abstract bool? TestByteCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 04 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteDefault")]
    public abstract BigInteger? TestByteDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteDefaultNonNullable")]
    public abstract BigInteger? TestByteDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiY+eErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AEEA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIFNOT 3E [2 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : ISNULL [2 datoshi]
    /// 12 : JMPIFNOT 03 [2 datoshi]
    /// 14 : THROW [512 datoshi]
    /// 15 : LDARG1 [2 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : ISNULL [2 datoshi]
    /// 18 : JMPIFNOT 03 [2 datoshi]
    /// 1A : THROW [512 datoshi]
    /// 1B : ADD [8 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSHINT32 00000080 [1 datoshi]
    /// 22 : JMPGE 04 [2 datoshi]
    /// 24 : JMP 0A [2 datoshi]
    /// 26 : DUP [2 datoshi]
    /// 27 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2C : JMPLE 1E [2 datoshi]
    /// 2E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 37 : AND [8 datoshi]
    /// 38 : DUP [2 datoshi]
    /// 39 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3E : JMPLE 0C [2 datoshi]
    /// 40 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 49 : SUB [8 datoshi]
    /// 4A : RET [0 datoshi]
    /// 4B : PUSH0 [1 datoshi]
    /// 4C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntAdd")]
    public abstract BigInteger? TestIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSHINT32 00000080 [1 datoshi]
    /// 0C : JMPGE 04 [2 datoshi]
    /// 0E : JMP 0A [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 16 : JMPLE 1E [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : JMPLE 0C [2 datoshi]
    /// 2A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 33 : SUB [8 datoshi]
    /// 34 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntAddNonNullable")]
    public abstract BigInteger? TestIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIF 04 [2 datoshi]
    /// 0F : PUSHF [1 datoshi]
    /// 10 : RET [0 datoshi]
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ISNULL [2 datoshi]
    /// 14 : JMPIFNOT 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : LDARG1 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : ISNULL [2 datoshi]
    /// 1A : JMPIFNOT 03 [2 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : GT [8 datoshi]
    /// 1E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntCompare")]
    public abstract bool? TestIntCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : GT [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntCompareNonNullable")]
    public abstract bool? TestIntCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 04 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntDefault")]
    public abstract BigInteger? TestIntDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntDefaultNonNullable")]
    public abstract BigInteger? TestIntDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiZaeErYJgM6eUrYJgM6nkoDAAAAAAAAAIAuBCIOSgP/////////fzIyBP//////////AAAAAAAAAACRSgP/////////fzIUBAAAAAAAAAAAAQAAAAAAAACfQBBA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIFNOT 5A [2 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : ISNULL [2 datoshi]
    /// 12 : JMPIFNOT 03 [2 datoshi]
    /// 14 : THROW [512 datoshi]
    /// 15 : LDARG1 [2 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : ISNULL [2 datoshi]
    /// 18 : JMPIFNOT 03 [2 datoshi]
    /// 1A : THROW [512 datoshi]
    /// 1B : ADD [8 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSHINT64 0000000000000080 [1 datoshi]
    /// 26 : JMPGE 04 [2 datoshi]
    /// 28 : JMP 0E [2 datoshi]
    /// 2A : DUP [2 datoshi]
    /// 2B : PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// 34 : JMPLE 32 [2 datoshi]
    /// 36 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 47 : AND [8 datoshi]
    /// 48 : DUP [2 datoshi]
    /// 49 : PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// 52 : JMPLE 14 [2 datoshi]
    /// 54 : PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// 65 : SUB [8 datoshi]
    /// 66 : RET [0 datoshi]
    /// 67 : PUSH0 [1 datoshi]
    /// 68 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongAdd")]
    public abstract BigInteger? TestLongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgMAAAAAAAAAgC4EIg5KA/////////9/MjIE//////////8AAAAAAAAAAJFKA/////////9/MhQEAAAAAAAAAAABAAAAAAAAAJ9A
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSHINT64 0000000000000080 [1 datoshi]
    /// 10 : JMPGE 04 [2 datoshi]
    /// 12 : JMP 0E [2 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// 1E : JMPLE 32 [2 datoshi]
    /// 20 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 31 : AND [8 datoshi]
    /// 32 : DUP [2 datoshi]
    /// 33 : PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// 3C : JMPLE 14 [2 datoshi]
    /// 3E : PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// 4F : SUB [8 datoshi]
    /// 50 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongAddNonNullable")]
    public abstract BigInteger? TestLongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIF 04 [2 datoshi]
    /// 0F : PUSHF [1 datoshi]
    /// 10 : RET [0 datoshi]
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ISNULL [2 datoshi]
    /// 14 : JMPIFNOT 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : LDARG1 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : ISNULL [2 datoshi]
    /// 1A : JMPIFNOT 03 [2 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : GT [8 datoshi]
    /// 1E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongCompare")]
    public abstract bool? TestLongCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : GT [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongCompareNonNullable")]
    public abstract bool? TestLongCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 04 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongDefault")]
    public abstract BigInteger? TestLongDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongDefaultNonNullable")]
    public abstract BigInteger? TestLongDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiZXeErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KAIAuBCIHSgB/Mg8B/wCRSgB/MgYBAAGfQBBA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIFNOT 57 [2 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : ISNULL [2 datoshi]
    /// 12 : JMPIFNOT 03 [2 datoshi]
    /// 14 : THROW [512 datoshi]
    /// 15 : LDARG1 [2 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : ISNULL [2 datoshi]
    /// 18 : JMPIFNOT 03 [2 datoshi]
    /// 1A : THROW [512 datoshi]
    /// 1B : ADD [8 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSHINT32 00000080 [1 datoshi]
    /// 22 : JMPGE 04 [2 datoshi]
    /// 24 : JMP 0A [2 datoshi]
    /// 26 : DUP [2 datoshi]
    /// 27 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2C : JMPLE 1E [2 datoshi]
    /// 2E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 37 : AND [8 datoshi]
    /// 38 : DUP [2 datoshi]
    /// 39 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3E : JMPLE 0C [2 datoshi]
    /// 40 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 49 : SUB [8 datoshi]
    /// 4A : DUP [2 datoshi]
    /// 4B : PUSHINT8 80 [1 datoshi]
    /// 4D : JMPGE 04 [2 datoshi]
    /// 4F : JMP 07 [2 datoshi]
    /// 51 : DUP [2 datoshi]
    /// 52 : PUSHINT8 7F [1 datoshi]
    /// 54 : JMPLE 0F [2 datoshi]
    /// 56 : PUSHINT16 FF00 [1 datoshi]
    /// 59 : AND [8 datoshi]
    /// 5A : DUP [2 datoshi]
    /// 5B : PUSHINT8 7F [1 datoshi]
    /// 5D : JMPLE 06 [2 datoshi]
    /// 5F : PUSHINT16 0001 [1 datoshi]
    /// 62 : SUB [8 datoshi]
    /// 63 : RET [0 datoshi]
    /// 64 : PUSH0 [1 datoshi]
    /// 65 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteAdd")]
    public abstract BigInteger? TestSByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oAgC4EIgdKAH8yDwH/AJFKAH8yBgEAAZ9A
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSHINT32 00000080 [1 datoshi]
    /// 0C : JMPGE 04 [2 datoshi]
    /// 0E : JMP 0A [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 16 : JMPLE 1E [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : JMPLE 0C [2 datoshi]
    /// 2A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 33 : SUB [8 datoshi]
    /// 34 : DUP [2 datoshi]
    /// 35 : PUSHINT8 80 [1 datoshi]
    /// 37 : JMPGE 04 [2 datoshi]
    /// 39 : JMP 07 [2 datoshi]
    /// 3B : DUP [2 datoshi]
    /// 3C : PUSHINT8 7F [1 datoshi]
    /// 3E : JMPLE 0F [2 datoshi]
    /// 40 : PUSHINT16 FF00 [1 datoshi]
    /// 43 : AND [8 datoshi]
    /// 44 : DUP [2 datoshi]
    /// 45 : PUSHINT8 7F [1 datoshi]
    /// 47 : JMPLE 06 [2 datoshi]
    /// 49 : PUSHINT16 0001 [1 datoshi]
    /// 4C : SUB [8 datoshi]
    /// 4D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteAddNonNullable")]
    public abstract BigInteger? TestSByteAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIF 04 [2 datoshi]
    /// 0F : PUSHF [1 datoshi]
    /// 10 : RET [0 datoshi]
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ISNULL [2 datoshi]
    /// 14 : JMPIFNOT 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : LDARG1 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : ISNULL [2 datoshi]
    /// 1A : JMPIFNOT 03 [2 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : GT [8 datoshi]
    /// 1E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteCompare")]
    public abstract bool? TestSByteCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : GT [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteCompareNonNullable")]
    public abstract bool? TestSByteCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 04 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteDefault")]
    public abstract BigInteger? TestSByteDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteDefaultNonNullable")]
    public abstract BigInteger? TestSByteDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiZeeErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KAQCALgQiCEoB/38yFAL//wAAkUoB/38yCAIAAAEAn0AQQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIFNOT 5E [2 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : ISNULL [2 datoshi]
    /// 12 : JMPIFNOT 03 [2 datoshi]
    /// 14 : THROW [512 datoshi]
    /// 15 : LDARG1 [2 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : ISNULL [2 datoshi]
    /// 18 : JMPIFNOT 03 [2 datoshi]
    /// 1A : THROW [512 datoshi]
    /// 1B : ADD [8 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSHINT32 00000080 [1 datoshi]
    /// 22 : JMPGE 04 [2 datoshi]
    /// 24 : JMP 0A [2 datoshi]
    /// 26 : DUP [2 datoshi]
    /// 27 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2C : JMPLE 1E [2 datoshi]
    /// 2E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 37 : AND [8 datoshi]
    /// 38 : DUP [2 datoshi]
    /// 39 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3E : JMPLE 0C [2 datoshi]
    /// 40 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 49 : SUB [8 datoshi]
    /// 4A : DUP [2 datoshi]
    /// 4B : PUSHINT16 0080 [1 datoshi]
    /// 4E : JMPGE 04 [2 datoshi]
    /// 50 : JMP 08 [2 datoshi]
    /// 52 : DUP [2 datoshi]
    /// 53 : PUSHINT16 FF7F [1 datoshi]
    /// 56 : JMPLE 14 [2 datoshi]
    /// 58 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 5D : AND [8 datoshi]
    /// 5E : DUP [2 datoshi]
    /// 5F : PUSHINT16 FF7F [1 datoshi]
    /// 62 : JMPLE 08 [2 datoshi]
    /// 64 : PUSHINT32 00000100 [1 datoshi]
    /// 69 : SUB [8 datoshi]
    /// 6A : RET [0 datoshi]
    /// 6B : PUSH0 [1 datoshi]
    /// 6C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortAdd")]
    public abstract BigInteger? TestShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oBAIAuBCIISgH/fzIUAv//AACRSgH/fzIIAgAAAQCfQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSHINT32 00000080 [1 datoshi]
    /// 0C : JMPGE 04 [2 datoshi]
    /// 0E : JMP 0A [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 16 : JMPLE 1E [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : JMPLE 0C [2 datoshi]
    /// 2A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 33 : SUB [8 datoshi]
    /// 34 : DUP [2 datoshi]
    /// 35 : PUSHINT16 0080 [1 datoshi]
    /// 38 : JMPGE 04 [2 datoshi]
    /// 3A : JMP 08 [2 datoshi]
    /// 3C : DUP [2 datoshi]
    /// 3D : PUSHINT16 FF7F [1 datoshi]
    /// 40 : JMPLE 14 [2 datoshi]
    /// 42 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 47 : AND [8 datoshi]
    /// 48 : DUP [2 datoshi]
    /// 49 : PUSHINT16 FF7F [1 datoshi]
    /// 4C : JMPLE 08 [2 datoshi]
    /// 4E : PUSHINT32 00000100 [1 datoshi]
    /// 53 : SUB [8 datoshi]
    /// 54 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortAddNonNullable")]
    public abstract BigInteger? TestShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIF 04 [2 datoshi]
    /// 0F : PUSHF [1 datoshi]
    /// 10 : RET [0 datoshi]
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ISNULL [2 datoshi]
    /// 14 : JMPIFNOT 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : LDARG1 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : ISNULL [2 datoshi]
    /// 1A : JMPIFNOT 03 [2 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : GT [8 datoshi]
    /// 1E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortCompare")]
    public abstract bool? TestShortCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : GT [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortCompareNonNullable")]
    public abstract bool? TestShortCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 04 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortDefault")]
    public abstract BigInteger? TestShortDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortDefaultNonNullable")]
    public abstract BigInteger? TestShortDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeErYJgVFDAB5StgmBUUMAIvbKEA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSHDATA1 [8 datoshi]
    /// 0B : LDARG1 [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : ISNULL [2 datoshi]
    /// 0E : JMPIFNOT 05 [2 datoshi]
    /// 10 : DROP [2 datoshi]
    /// 11 : PUSHDATA1 [8 datoshi]
    /// 13 : CAT [2048 datoshi]
    /// 14 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringConcat")]
    public abstract string? TestStringConcat(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2yhA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : CAT [2048 datoshi]
    /// 06 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 08 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringConcatNonNullable")]
    public abstract string? TestStringConcatNonNullable(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgVFDABA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSHDATA1 [8 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringDefault")]
    public abstract string? TestStringDefault(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringDefaultNonNullable")]
    public abstract string? TestStringDefaultNonNullable(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 03 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : JMPIFNOT 04 [2 datoshi]
    /// 0D : DROP [2 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringLength")]
    public abstract BigInteger? TestStringLength(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SIZE [4 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringLengthNonNullable")]
    public abstract BigInteger? TestStringLengthNonNullable(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 03 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : JMPIFNOT 04 [2 datoshi]
    /// 0D : DROP [2 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160ArrayLength")]
    public abstract BigInteger? TestUInt160ArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SIZE [4 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160ArrayLengthNonNullable")]
    public abstract BigInteger? TestUInt160ArrayLengthNonNullable(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJhlFDBQAAAAAAAAAAAAAAAAAAAAAAAAAAEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 19 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// 1F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160Default")]
    public abstract UInt160? TestUInt160Default(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160DefaultNonNullable")]
    public abstract UInt160? TestUInt160DefaultNonNullable(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 03 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : ISNULL [2 datoshi]
    /// 0B : JMPIFNOT 04 [2 datoshi]
    /// 0D : DROP [2 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256ArrayLength")]
    public abstract BigInteger? TestUInt256ArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SIZE [4 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256ArrayLengthNonNullable")]
    public abstract BigInteger? TestUInt256ArrayLengthNonNullable(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJiVFDCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 25 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000 [8 datoshi]
    /// 2B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256Default")]
    public abstract UInt256? TestUInt256Default(UInt256? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256DefaultNonNullable")]
    public abstract UInt256? TestUInt256DefaultNonNullable(UInt256? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYseErYJgM6eUrYJgM6nkoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFAEEA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIFNOT 2C [2 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : ISNULL [2 datoshi]
    /// 12 : JMPIFNOT 03 [2 datoshi]
    /// 14 : THROW [512 datoshi]
    /// 15 : LDARG1 [2 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : ISNULL [2 datoshi]
    /// 18 : JMPIFNOT 03 [2 datoshi]
    /// 1A : THROW [512 datoshi]
    /// 1B : ADD [8 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSH0 [1 datoshi]
    /// 1E : JMPGE 04 [2 datoshi]
    /// 20 : JMP 0E [2 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 2C : JMPLE 0C [2 datoshi]
    /// 2E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 37 : AND [8 datoshi]
    /// 38 : RET [0 datoshi]
    /// 39 : PUSH0 [1 datoshi]
    /// 3A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntAdd")]
    public abstract BigInteger? TestUIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : JMPGE 04 [2 datoshi]
    /// 0A : JMP 0E [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 16 : JMPLE 0C [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntAddNonNullable")]
    public abstract BigInteger? TestUIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIF 04 [2 datoshi]
    /// 0F : PUSHF [1 datoshi]
    /// 10 : RET [0 datoshi]
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ISNULL [2 datoshi]
    /// 14 : JMPIFNOT 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : LDARG1 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : ISNULL [2 datoshi]
    /// 1A : JMPIFNOT 03 [2 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : GT [8 datoshi]
    /// 1E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntCompare")]
    public abstract bool? TestUIntCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : GT [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntCompareNonNullable")]
    public abstract bool? TestUIntCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 04 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntDefault")]
    public abstract BigInteger? TestUIntDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntDefaultNonNullable")]
    public abstract BigInteger? TestUIntDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiY8eErYJgM6eUrYJgM6nkoQLgQiFkoE//////////8AAAAAAAAAADIUBP//////////AAAAAAAAAACRQBBA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIFNOT 3C [2 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : ISNULL [2 datoshi]
    /// 12 : JMPIFNOT 03 [2 datoshi]
    /// 14 : THROW [512 datoshi]
    /// 15 : LDARG1 [2 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : ISNULL [2 datoshi]
    /// 18 : JMPIFNOT 03 [2 datoshi]
    /// 1A : THROW [512 datoshi]
    /// 1B : ADD [8 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSH0 [1 datoshi]
    /// 1E : JMPGE 04 [2 datoshi]
    /// 20 : JMP 16 [2 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 34 : JMPLE 14 [2 datoshi]
    /// 36 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 47 : AND [8 datoshi]
    /// 48 : RET [0 datoshi]
    /// 49 : PUSH0 [1 datoshi]
    /// 4A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongAdd")]
    public abstract BigInteger? TestULongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeShAuBCIWSgT//////////wAAAAAAAAAAMhQE//////////8AAAAAAAAAAJFA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : JMPGE 04 [2 datoshi]
    /// 0A : JMP 16 [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 1E : JMPLE 14 [2 datoshi]
    /// 20 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 31 : AND [8 datoshi]
    /// 32 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongAddNonNullable")]
    public abstract BigInteger? TestULongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIF 04 [2 datoshi]
    /// 0F : PUSHF [1 datoshi]
    /// 10 : RET [0 datoshi]
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ISNULL [2 datoshi]
    /// 14 : JMPIFNOT 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : LDARG1 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : ISNULL [2 datoshi]
    /// 1A : JMPIFNOT 03 [2 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : GT [8 datoshi]
    /// 1E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongCompare")]
    public abstract bool? TestULongCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : GT [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongCompareNonNullable")]
    public abstract bool? TestULongCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 04 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongDefault")]
    public abstract BigInteger? TestULongDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongDefaultNonNullable")]
    public abstract BigInteger? TestULongDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiZSeErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KEC4EIgpKAv//AAAyCAL//wAAkUAQQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIFNOT 52 [2 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : ISNULL [2 datoshi]
    /// 12 : JMPIFNOT 03 [2 datoshi]
    /// 14 : THROW [512 datoshi]
    /// 15 : LDARG1 [2 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : ISNULL [2 datoshi]
    /// 18 : JMPIFNOT 03 [2 datoshi]
    /// 1A : THROW [512 datoshi]
    /// 1B : ADD [8 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSHINT32 00000080 [1 datoshi]
    /// 22 : JMPGE 04 [2 datoshi]
    /// 24 : JMP 0A [2 datoshi]
    /// 26 : DUP [2 datoshi]
    /// 27 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 2C : JMPLE 1E [2 datoshi]
    /// 2E : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 37 : AND [8 datoshi]
    /// 38 : DUP [2 datoshi]
    /// 39 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 3E : JMPLE 0C [2 datoshi]
    /// 40 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 49 : SUB [8 datoshi]
    /// 4A : DUP [2 datoshi]
    /// 4B : PUSH0 [1 datoshi]
    /// 4C : JMPGE 04 [2 datoshi]
    /// 4E : JMP 0A [2 datoshi]
    /// 50 : DUP [2 datoshi]
    /// 51 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 56 : JMPLE 08 [2 datoshi]
    /// 58 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 5D : AND [8 datoshi]
    /// 5E : RET [0 datoshi]
    /// 5F : PUSH0 [1 datoshi]
    /// 60 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortAdd")]
    public abstract BigInteger? TestUShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCkoC//8AADIIAv//AACRQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSHINT32 00000080 [1 datoshi]
    /// 0C : JMPGE 04 [2 datoshi]
    /// 0E : JMP 0A [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 16 : JMPLE 1E [2 datoshi]
    /// 18 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : JMPLE 0C [2 datoshi]
    /// 2A : PUSHINT64 0000000001000000 [1 datoshi]
    /// 33 : SUB [8 datoshi]
    /// 34 : DUP [2 datoshi]
    /// 35 : PUSH0 [1 datoshi]
    /// 36 : JMPGE 04 [2 datoshi]
    /// 38 : JMP 0A [2 datoshi]
    /// 3A : DUP [2 datoshi]
    /// 3B : PUSHINT32 FFFF0000 [1 datoshi]
    /// 40 : JMPLE 08 [2 datoshi]
    /// 42 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 47 : AND [8 datoshi]
    /// 48 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortAddNonNullable")]
    public abstract BigInteger? TestUShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : JMPIFNOT 05 [2 datoshi]
    /// 07 : PUSHF [1 datoshi]
    /// 08 : JMP 05 [2 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : ISNULL [2 datoshi]
    /// 0C : NOT [4 datoshi]
    /// 0D : JMPIF 04 [2 datoshi]
    /// 0F : PUSHF [1 datoshi]
    /// 10 : RET [0 datoshi]
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ISNULL [2 datoshi]
    /// 14 : JMPIFNOT 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : LDARG1 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : ISNULL [2 datoshi]
    /// 1A : JMPIFNOT 03 [2 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : GT [8 datoshi]
    /// 1E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortCompare")]
    public abstract bool? TestUShortCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : GT [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortCompareNonNullable")]
    public abstract bool? TestUShortCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 04 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortDefault")]
    public abstract BigInteger? TestUShortDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortDefaultNonNullable")]
    public abstract BigInteger? TestUShortDefaultNonNullable(BigInteger? a);

    #endregion
}
