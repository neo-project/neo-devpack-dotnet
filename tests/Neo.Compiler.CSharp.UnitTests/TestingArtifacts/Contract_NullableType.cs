using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NullableType(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NullableType"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testBigIntegerAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testBigIntegerAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":32,""safe"":false},{""name"":""testBigIntegerCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":39,""safe"":false},{""name"":""testBigIntegerCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":71,""safe"":false},{""name"":""testBigIntegerDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":78,""safe"":false},{""name"":""testBigIntegerDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":89,""safe"":false},{""name"":""testIntAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":94,""safe"":false},{""name"":""testIntAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":172,""safe"":false},{""name"":""testIntCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":225,""safe"":false},{""name"":""testIntCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":257,""safe"":false},{""name"":""testIntDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":264,""safe"":false},{""name"":""testIntDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":275,""safe"":false},{""name"":""testUIntAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":280,""safe"":false},{""name"":""testUIntAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":340,""safe"":false},{""name"":""testUIntCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":375,""safe"":false},{""name"":""testUIntCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":407,""safe"":false},{""name"":""testUIntDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":414,""safe"":false},{""name"":""testUIntDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":425,""safe"":false},{""name"":""testLongAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":430,""safe"":false},{""name"":""testLongAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":536,""safe"":false},{""name"":""testLongCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":617,""safe"":false},{""name"":""testLongCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":649,""safe"":false},{""name"":""testLongDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":656,""safe"":false},{""name"":""testLongDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":667,""safe"":false},{""name"":""testULongAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":672,""safe"":false},{""name"":""testULongAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":748,""safe"":false},{""name"":""testULongCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":799,""safe"":false},{""name"":""testULongCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":831,""safe"":false},{""name"":""testULongDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":838,""safe"":false},{""name"":""testULongDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":849,""safe"":false},{""name"":""testShortAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":854,""safe"":false},{""name"":""testShortAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":964,""safe"":false},{""name"":""testShortCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1049,""safe"":false},{""name"":""testShortCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1081,""safe"":false},{""name"":""testShortDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1088,""safe"":false},{""name"":""testShortDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1099,""safe"":false},{""name"":""testUShortAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1104,""safe"":false},{""name"":""testUShortAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1202,""safe"":false},{""name"":""testUShortCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1275,""safe"":false},{""name"":""testUShortCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1307,""safe"":false},{""name"":""testUShortDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1314,""safe"":false},{""name"":""testUShortDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1325,""safe"":false},{""name"":""testSByteAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1330,""safe"":false},{""name"":""testSByteAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1433,""safe"":false},{""name"":""testSByteCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1511,""safe"":false},{""name"":""testSByteCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1543,""safe"":false},{""name"":""testSByteDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1550,""safe"":false},{""name"":""testSByteDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1561,""safe"":false},{""name"":""testByteAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1566,""safe"":false},{""name"":""testByteAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1660,""safe"":false},{""name"":""testByteCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1729,""safe"":false},{""name"":""testByteCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1761,""safe"":false},{""name"":""testByteDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1768,""safe"":false},{""name"":""testByteDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1779,""safe"":false},{""name"":""testBoolAnd"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1784,""safe"":false},{""name"":""testBoolAndNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1819,""safe"":false},{""name"":""testBoolOr"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1829,""safe"":false},{""name"":""testBoolOrNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1864,""safe"":false},{""name"":""testBoolDefault"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1874,""safe"":false},{""name"":""testBoolDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1885,""safe"":false},{""name"":""testUInt160Default"",""parameters"":[{""name"":""a"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":1890,""safe"":false},{""name"":""testUInt160DefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":1922,""safe"":false},{""name"":""testUInt256Default"",""parameters"":[{""name"":""a"",""type"":""Hash256""}],""returntype"":""Hash256"",""offset"":1927,""safe"":false},{""name"":""testUInt256DefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Hash256""}],""returntype"":""Hash256"",""offset"":1971,""safe"":false},{""name"":""testUInt160ArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1976,""safe"":false},{""name"":""testUInt160ArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1992,""safe"":false},{""name"":""testUInt256ArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1998,""safe"":false},{""name"":""testUInt256ArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2014,""safe"":false},{""name"":""testByteArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":2020,""safe"":false},{""name"":""testByteArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":2036,""safe"":false},{""name"":""testStringLength"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Integer"",""offset"":2042,""safe"":false},{""name"":""testStringLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Integer"",""offset"":2058,""safe"":false},{""name"":""testStringDefault"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""String"",""offset"":2064,""safe"":false},{""name"":""testStringDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""String"",""offset"":2076,""safe"":false},{""name"":""testStringConcat"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""}],""returntype"":""String"",""offset"":2081,""safe"":false},{""name"":""testStringConcatNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""}],""returntype"":""String"",""offset"":2104,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1BCFcAAnjYqiQFCSIFediqJhB4StgmAzp5StgmAzqeQBBAVwACeHmeQFcAAnjYqiQFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42KokBQkiBXnYqiY+eErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AEEBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAAnjYqiQFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42KokBQkiBXnYqiYseErYJgM6eUrYJgM6nkoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFAEEBXAAJ4eZ5KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRQFcAAnjYqiQFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42KokBQkiBXnYqiZaeErYJgM6eUrYJgM6nkoDAAAAAAAAAIAuBCIOSgP/////////fzIyBP//////////AAAAAAAAAACRSgP/////////fzIUBAAAAAAAAAAAAQAAAAAAAACfQBBAVwACeHmeSgMAAAAAAAAAgC4EIg5KA/////////9/MjIE//////////8AAAAAAAAAAJFKA/////////9/MhQEAAAAAAAAAAABAAAAAAAAAJ9AVwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0BXAAJ4ebdAVwABeErYJgRFEEBXAAF4QFcAAnjYqiQFCSIFediqJjx4StgmAzp5StgmAzqeShAuBCIWSgT//////////wAAAAAAAAAAMhQE//////////8AAAAAAAAAAJFAEEBXAAJ4eZ5KEC4EIhZKBP//////////AAAAAAAAAAAyFAT//////////wAAAAAAAAAAkUBXAAJ42KokBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNiqJAUJIgV52KomXnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSgEAgC4EIghKAf9/MhQC//8AAJFKAf9/MggCAAABAJ9AEEBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSgEAgC4EIghKAf9/MhQC//8AAJFKAf9/MggCAAABAJ9AVwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0BXAAJ4ebdAVwABeErYJgRFEEBXAAF4QFcAAnjYqiQFCSIFediqJlJ4StgmAzp5StgmAzqeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCkoC//8AADIIAv//AACRQBBAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCkoC//8AADIIAv//AACRQFcAAnjYqiQFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42KokBQkiBXnYqiZXeErYJgM6eUrYJgM6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KAIAuBCIHSgB/Mg8B/wCRSgB/MgYBAAGfQBBAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oAgC4EIgdKAH8yDwH/AJFKAH8yBgEAAZ9AVwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0BXAAJ4ebdAVwABeErYJgRFEEBXAAF4QFcAAnjYqiQFCSIFediqJk54StgmAzp5StgmAzqeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCEoB/wAyBgH/AJFAEEBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIISgH/ADIGAf8AkUBXAAJ42KokBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNiqJAUJIgV52KokBAlAeErYJgM6JAQJQHlK2CYDOkBXAAJ4JAQJQHlAVwACeNiqJAUJIgV52KokBAlAeErYJgM6JgQIQHlK2CYDOkBXAAJ4JgQIQHlAVwABeErYJgRFCUBXAAF4QFcAAXhK2CYZRQwUAAAAAAAAAAAAAAAAAAAAAAAAAABAVwABeEBXAAF4StgmJUUMIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQFcAAXhAVwABeErYJAPKStgmBEUQQFcAAXjKQFcAAXhK2CQDykrYJgRFEEBXAAF4ykBXAAF4StgkA8pK2CYERRBAVwABeMpAVwABeErYJAPKStgmBEUQQFcAAXjKQFcAAXhK2CYFRQwAQFcAAXhAVwACeErYJgVFDAB5StgmBUUMAIvbKEBXAAJ4eYvbKEDPhfuc"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEA=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIFNOT 10	[2 datoshi]
    /// 10 : OpCode.LDARG0	[2 datoshi]
    /// 11 : OpCode.DUP	[2 datoshi]
    /// 12 : OpCode.ISNULL	[2 datoshi]
    /// 13 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 15 : OpCode.THROW	[512 datoshi]
    /// 16 : OpCode.LDARG1	[2 datoshi]
    /// 17 : OpCode.DUP	[2 datoshi]
    /// 18 : OpCode.ISNULL	[2 datoshi]
    /// 19 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1B : OpCode.THROW	[512 datoshi]
    /// 1C : OpCode.ADD	[8 datoshi]
    /// 1D : OpCode.RET	[0 datoshi]
    /// 1E : OpCode.PUSH0	[1 datoshi]
    /// 1F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerAdd")]
    public abstract BigInteger? TestBigIntegerAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.ADD	[8 datoshi]
    /// 06 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerAddNonNullable")]
    public abstract BigInteger? TestBigIntegerAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIF 04	[2 datoshi]
    /// 10 : OpCode.PUSHF	[1 datoshi]
    /// 11 : OpCode.RET	[0 datoshi]
    /// 12 : OpCode.LDARG0	[2 datoshi]
    /// 13 : OpCode.DUP	[2 datoshi]
    /// 14 : OpCode.ISNULL	[2 datoshi]
    /// 15 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 17 : OpCode.THROW	[512 datoshi]
    /// 18 : OpCode.LDARG1	[2 datoshi]
    /// 19 : OpCode.DUP	[2 datoshi]
    /// 1A : OpCode.ISNULL	[2 datoshi]
    /// 1B : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1D : OpCode.THROW	[512 datoshi]
    /// 1E : OpCode.GT	[8 datoshi]
    /// 1F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerCompare")]
    public abstract bool? TestBigIntegerCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.GT	[8 datoshi]
    /// 06 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerCompareNonNullable")]
    public abstract bool? TestBigIntegerCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSH0	[1 datoshi]
    /// 0A : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerDefault")]
    public abstract BigInteger? TestBigIntegerDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerDefaultNonNullable")]
    public abstract BigInteger? TestBigIntegerDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6JAQJQHlK2CYDOkA=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIF 04	[2 datoshi]
    /// 10 : OpCode.PUSHF	[1 datoshi]
    /// 11 : OpCode.RET	[0 datoshi]
    /// 12 : OpCode.LDARG0	[2 datoshi]
    /// 13 : OpCode.DUP	[2 datoshi]
    /// 14 : OpCode.ISNULL	[2 datoshi]
    /// 15 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 17 : OpCode.THROW	[512 datoshi]
    /// 18 : OpCode.JMPIF 04	[2 datoshi]
    /// 1A : OpCode.PUSHF	[1 datoshi]
    /// 1B : OpCode.RET	[0 datoshi]
    /// 1C : OpCode.LDARG1	[2 datoshi]
    /// 1D : OpCode.DUP	[2 datoshi]
    /// 1E : OpCode.ISNULL	[2 datoshi]
    /// 1F : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 21 : OpCode.THROW	[512 datoshi]
    /// 22 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testBoolAnd")]
    public abstract bool? TestBoolAnd(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCQECUB5QA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.JMPIF 04	[2 datoshi]
    /// 06 : OpCode.PUSHF	[1 datoshi]
    /// 07 : OpCode.RET	[0 datoshi]
    /// 08 : OpCode.LDARG1	[2 datoshi]
    /// 09 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testBoolAndNonNullable")]
    public abstract bool? TestBoolAndNonNullable(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFCUA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSHF	[1 datoshi]
    /// 0A : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testBoolDefault")]
    public abstract bool? TestBoolDefault(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testBoolDefaultNonNullable")]
    public abstract bool? TestBoolDefaultNonNullable(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6JgQIQHlK2CYDOkA=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIF 04	[2 datoshi]
    /// 10 : OpCode.PUSHF	[1 datoshi]
    /// 11 : OpCode.RET	[0 datoshi]
    /// 12 : OpCode.LDARG0	[2 datoshi]
    /// 13 : OpCode.DUP	[2 datoshi]
    /// 14 : OpCode.ISNULL	[2 datoshi]
    /// 15 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 17 : OpCode.THROW	[512 datoshi]
    /// 18 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 1A : OpCode.PUSHT	[1 datoshi]
    /// 1B : OpCode.RET	[0 datoshi]
    /// 1C : OpCode.LDARG1	[2 datoshi]
    /// 1D : OpCode.DUP	[2 datoshi]
    /// 1E : OpCode.ISNULL	[2 datoshi]
    /// 1F : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 21 : OpCode.THROW	[512 datoshi]
    /// 22 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testBoolOr")]
    public abstract bool? TestBoolOr(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCYECEB5QA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 06 : OpCode.PUSHT	[1 datoshi]
    /// 07 : OpCode.RET	[0 datoshi]
    /// 08 : OpCode.LDARG1	[2 datoshi]
    /// 09 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testBoolOrNonNullable")]
    public abstract bool? TestBoolOrNonNullable(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomTnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIISgH/ADIGAf8AkUAQQA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIFNOT 4E	[2 datoshi]
    /// 10 : OpCode.LDARG0	[2 datoshi]
    /// 11 : OpCode.DUP	[2 datoshi]
    /// 12 : OpCode.ISNULL	[2 datoshi]
    /// 13 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 15 : OpCode.THROW	[512 datoshi]
    /// 16 : OpCode.LDARG1	[2 datoshi]
    /// 17 : OpCode.DUP	[2 datoshi]
    /// 18 : OpCode.ISNULL	[2 datoshi]
    /// 19 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1B : OpCode.THROW	[512 datoshi]
    /// 1C : OpCode.ADD	[8 datoshi]
    /// 1D : OpCode.DUP	[2 datoshi]
    /// 1E : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 23 : OpCode.JMPGE 04	[2 datoshi]
    /// 25 : OpCode.JMP 0A	[2 datoshi]
    /// 27 : OpCode.DUP	[2 datoshi]
    /// 28 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 2D : OpCode.JMPLE 1E	[2 datoshi]
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 38 : OpCode.AND	[8 datoshi]
    /// 39 : OpCode.DUP	[2 datoshi]
    /// 3A : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 3F : OpCode.JMPLE 0C	[2 datoshi]
    /// 41 : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 4A : OpCode.SUB	[8 datoshi]
    /// 4B : OpCode.DUP	[2 datoshi]
    /// 4C : OpCode.PUSH0	[1 datoshi]
    /// 4D : OpCode.JMPGE 04	[2 datoshi]
    /// 4F : OpCode.JMP 08	[2 datoshi]
    /// 51 : OpCode.DUP	[2 datoshi]
    /// 52 : OpCode.PUSHINT16 FF00	[1 datoshi]
    /// 55 : OpCode.JMPLE 06	[2 datoshi]
    /// 57 : OpCode.PUSHINT16 FF00	[1 datoshi]
    /// 5A : OpCode.AND	[8 datoshi]
    /// 5B : OpCode.RET	[0 datoshi]
    /// 5C : OpCode.PUSH0	[1 datoshi]
    /// 5D : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testByteAdd")]
    public abstract BigInteger? TestByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCEoB/wAyBgH/AJFA
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.ADD	[8 datoshi]
    /// 06 : OpCode.DUP	[2 datoshi]
    /// 07 : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 0C : OpCode.JMPGE 04	[2 datoshi]
    /// 0E : OpCode.JMP 0A	[2 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 16 : OpCode.JMPLE 1E	[2 datoshi]
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 21 : OpCode.AND	[8 datoshi]
    /// 22 : OpCode.DUP	[2 datoshi]
    /// 23 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 28 : OpCode.JMPLE 0C	[2 datoshi]
    /// 2A : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 33 : OpCode.SUB	[8 datoshi]
    /// 34 : OpCode.DUP	[2 datoshi]
    /// 35 : OpCode.PUSH0	[1 datoshi]
    /// 36 : OpCode.JMPGE 04	[2 datoshi]
    /// 38 : OpCode.JMP 08	[2 datoshi]
    /// 3A : OpCode.DUP	[2 datoshi]
    /// 3B : OpCode.PUSHINT16 FF00	[1 datoshi]
    /// 3E : OpCode.JMPLE 06	[2 datoshi]
    /// 40 : OpCode.PUSHINT16 FF00	[1 datoshi]
    /// 43 : OpCode.AND	[8 datoshi]
    /// 44 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testByteAddNonNullable")]
    public abstract BigInteger? TestByteAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIF 03	[2 datoshi]
    /// 08 : OpCode.SIZE	[4 datoshi]
    /// 09 : OpCode.DUP	[2 datoshi]
    /// 0A : OpCode.ISNULL	[2 datoshi]
    /// 0B : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 0D : OpCode.DROP	[2 datoshi]
    /// 0E : OpCode.PUSH0	[1 datoshi]
    /// 0F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayLength")]
    public abstract BigInteger? TestByteArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.SIZE	[4 datoshi]
    /// 05 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayLengthNonNullable")]
    public abstract BigInteger? TestByteArrayLengthNonNullable(byte[]? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIF 04	[2 datoshi]
    /// 10 : OpCode.PUSHF	[1 datoshi]
    /// 11 : OpCode.RET	[0 datoshi]
    /// 12 : OpCode.LDARG0	[2 datoshi]
    /// 13 : OpCode.DUP	[2 datoshi]
    /// 14 : OpCode.ISNULL	[2 datoshi]
    /// 15 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 17 : OpCode.THROW	[512 datoshi]
    /// 18 : OpCode.LDARG1	[2 datoshi]
    /// 19 : OpCode.DUP	[2 datoshi]
    /// 1A : OpCode.ISNULL	[2 datoshi]
    /// 1B : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1D : OpCode.THROW	[512 datoshi]
    /// 1E : OpCode.GT	[8 datoshi]
    /// 1F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testByteCompare")]
    public abstract bool? TestByteCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.GT	[8 datoshi]
    /// 06 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testByteCompareNonNullable")]
    public abstract bool? TestByteCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSH0	[1 datoshi]
    /// 0A : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testByteDefault")]
    public abstract BigInteger? TestByteDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testByteDefaultNonNullable")]
    public abstract BigInteger? TestByteDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomPnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQBBA
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIFNOT 3E	[2 datoshi]
    /// 10 : OpCode.LDARG0	[2 datoshi]
    /// 11 : OpCode.DUP	[2 datoshi]
    /// 12 : OpCode.ISNULL	[2 datoshi]
    /// 13 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 15 : OpCode.THROW	[512 datoshi]
    /// 16 : OpCode.LDARG1	[2 datoshi]
    /// 17 : OpCode.DUP	[2 datoshi]
    /// 18 : OpCode.ISNULL	[2 datoshi]
    /// 19 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1B : OpCode.THROW	[512 datoshi]
    /// 1C : OpCode.ADD	[8 datoshi]
    /// 1D : OpCode.DUP	[2 datoshi]
    /// 1E : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 23 : OpCode.JMPGE 04	[2 datoshi]
    /// 25 : OpCode.JMP 0A	[2 datoshi]
    /// 27 : OpCode.DUP	[2 datoshi]
    /// 28 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 2D : OpCode.JMPLE 1E	[2 datoshi]
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 38 : OpCode.AND	[8 datoshi]
    /// 39 : OpCode.DUP	[2 datoshi]
    /// 3A : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 3F : OpCode.JMPLE 0C	[2 datoshi]
    /// 41 : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 4A : OpCode.SUB	[8 datoshi]
    /// 4B : OpCode.RET	[0 datoshi]
    /// 4C : OpCode.PUSH0	[1 datoshi]
    /// 4D : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testIntAdd")]
    public abstract BigInteger? TestIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.ADD	[8 datoshi]
    /// 06 : OpCode.DUP	[2 datoshi]
    /// 07 : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 0C : OpCode.JMPGE 04	[2 datoshi]
    /// 0E : OpCode.JMP 0A	[2 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 16 : OpCode.JMPLE 1E	[2 datoshi]
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 21 : OpCode.AND	[8 datoshi]
    /// 22 : OpCode.DUP	[2 datoshi]
    /// 23 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 28 : OpCode.JMPLE 0C	[2 datoshi]
    /// 2A : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 33 : OpCode.SUB	[8 datoshi]
    /// 34 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testIntAddNonNullable")]
    public abstract BigInteger? TestIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIF 04	[2 datoshi]
    /// 10 : OpCode.PUSHF	[1 datoshi]
    /// 11 : OpCode.RET	[0 datoshi]
    /// 12 : OpCode.LDARG0	[2 datoshi]
    /// 13 : OpCode.DUP	[2 datoshi]
    /// 14 : OpCode.ISNULL	[2 datoshi]
    /// 15 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 17 : OpCode.THROW	[512 datoshi]
    /// 18 : OpCode.LDARG1	[2 datoshi]
    /// 19 : OpCode.DUP	[2 datoshi]
    /// 1A : OpCode.ISNULL	[2 datoshi]
    /// 1B : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1D : OpCode.THROW	[512 datoshi]
    /// 1E : OpCode.GT	[8 datoshi]
    /// 1F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testIntCompare")]
    public abstract bool? TestIntCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.GT	[8 datoshi]
    /// 06 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testIntCompareNonNullable")]
    public abstract bool? TestIntCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSH0	[1 datoshi]
    /// 0A : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testIntDefault")]
    public abstract BigInteger? TestIntDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testIntDefaultNonNullable")]
    public abstract BigInteger? TestIntDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomWnhK2CYDOnlK2CYDOp5KAwAAAAAAAACALgQiDkoD/////////38yMgT//////////wAAAAAAAAAAkUoD/////////38yFAQAAAAAAAAAAAEAAAAAAAAAn0AQQA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIFNOT 5A	[2 datoshi]
    /// 10 : OpCode.LDARG0	[2 datoshi]
    /// 11 : OpCode.DUP	[2 datoshi]
    /// 12 : OpCode.ISNULL	[2 datoshi]
    /// 13 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 15 : OpCode.THROW	[512 datoshi]
    /// 16 : OpCode.LDARG1	[2 datoshi]
    /// 17 : OpCode.DUP	[2 datoshi]
    /// 18 : OpCode.ISNULL	[2 datoshi]
    /// 19 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1B : OpCode.THROW	[512 datoshi]
    /// 1C : OpCode.ADD	[8 datoshi]
    /// 1D : OpCode.DUP	[2 datoshi]
    /// 1E : OpCode.PUSHINT64 0000000000000080	[1 datoshi]
    /// 27 : OpCode.JMPGE 04	[2 datoshi]
    /// 29 : OpCode.JMP 0E	[2 datoshi]
    /// 2B : OpCode.DUP	[2 datoshi]
    /// 2C : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F	[1 datoshi]
    /// 35 : OpCode.JMPLE 32	[2 datoshi]
    /// 37 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000	[4 datoshi]
    /// 48 : OpCode.AND	[8 datoshi]
    /// 49 : OpCode.DUP	[2 datoshi]
    /// 4A : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F	[1 datoshi]
    /// 53 : OpCode.JMPLE 14	[2 datoshi]
    /// 55 : OpCode.PUSHINT128 00000000000000000100000000000000	[4 datoshi]
    /// 66 : OpCode.SUB	[8 datoshi]
    /// 67 : OpCode.RET	[0 datoshi]
    /// 68 : OpCode.PUSH0	[1 datoshi]
    /// 69 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testLongAdd")]
    public abstract BigInteger? TestLongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgMAAAAAAAAAgC4EIg5KA/////////9/MjIE//////////8AAAAAAAAAAJFKA/////////9/MhQEAAAAAAAAAAABAAAAAAAAAJ9A
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.ADD	[8 datoshi]
    /// 06 : OpCode.DUP	[2 datoshi]
    /// 07 : OpCode.PUSHINT64 0000000000000080	[1 datoshi]
    /// 10 : OpCode.JMPGE 04	[2 datoshi]
    /// 12 : OpCode.JMP 0E	[2 datoshi]
    /// 14 : OpCode.DUP	[2 datoshi]
    /// 15 : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F	[1 datoshi]
    /// 1E : OpCode.JMPLE 32	[2 datoshi]
    /// 20 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000	[4 datoshi]
    /// 31 : OpCode.AND	[8 datoshi]
    /// 32 : OpCode.DUP	[2 datoshi]
    /// 33 : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F	[1 datoshi]
    /// 3C : OpCode.JMPLE 14	[2 datoshi]
    /// 3E : OpCode.PUSHINT128 00000000000000000100000000000000	[4 datoshi]
    /// 4F : OpCode.SUB	[8 datoshi]
    /// 50 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testLongAddNonNullable")]
    public abstract BigInteger? TestLongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIF 04	[2 datoshi]
    /// 10 : OpCode.PUSHF	[1 datoshi]
    /// 11 : OpCode.RET	[0 datoshi]
    /// 12 : OpCode.LDARG0	[2 datoshi]
    /// 13 : OpCode.DUP	[2 datoshi]
    /// 14 : OpCode.ISNULL	[2 datoshi]
    /// 15 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 17 : OpCode.THROW	[512 datoshi]
    /// 18 : OpCode.LDARG1	[2 datoshi]
    /// 19 : OpCode.DUP	[2 datoshi]
    /// 1A : OpCode.ISNULL	[2 datoshi]
    /// 1B : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1D : OpCode.THROW	[512 datoshi]
    /// 1E : OpCode.GT	[8 datoshi]
    /// 1F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testLongCompare")]
    public abstract bool? TestLongCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.GT	[8 datoshi]
    /// 06 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testLongCompareNonNullable")]
    public abstract bool? TestLongCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSH0	[1 datoshi]
    /// 0A : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testLongDefault")]
    public abstract BigInteger? TestLongDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testLongDefaultNonNullable")]
    public abstract BigInteger? TestLongDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomV3hK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSgCALgQiB0oAfzIPAf8AkUoAfzIGAQABn0AQQA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIFNOT 57	[2 datoshi]
    /// 10 : OpCode.LDARG0	[2 datoshi]
    /// 11 : OpCode.DUP	[2 datoshi]
    /// 12 : OpCode.ISNULL	[2 datoshi]
    /// 13 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 15 : OpCode.THROW	[512 datoshi]
    /// 16 : OpCode.LDARG1	[2 datoshi]
    /// 17 : OpCode.DUP	[2 datoshi]
    /// 18 : OpCode.ISNULL	[2 datoshi]
    /// 19 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1B : OpCode.THROW	[512 datoshi]
    /// 1C : OpCode.ADD	[8 datoshi]
    /// 1D : OpCode.DUP	[2 datoshi]
    /// 1E : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 23 : OpCode.JMPGE 04	[2 datoshi]
    /// 25 : OpCode.JMP 0A	[2 datoshi]
    /// 27 : OpCode.DUP	[2 datoshi]
    /// 28 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 2D : OpCode.JMPLE 1E	[2 datoshi]
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 38 : OpCode.AND	[8 datoshi]
    /// 39 : OpCode.DUP	[2 datoshi]
    /// 3A : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 3F : OpCode.JMPLE 0C	[2 datoshi]
    /// 41 : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 4A : OpCode.SUB	[8 datoshi]
    /// 4B : OpCode.DUP	[2 datoshi]
    /// 4C : OpCode.PUSHINT8 80	[1 datoshi]
    /// 4E : OpCode.JMPGE 04	[2 datoshi]
    /// 50 : OpCode.JMP 07	[2 datoshi]
    /// 52 : OpCode.DUP	[2 datoshi]
    /// 53 : OpCode.PUSHINT8 7F	[1 datoshi]
    /// 55 : OpCode.JMPLE 0F	[2 datoshi]
    /// 57 : OpCode.PUSHINT16 FF00	[1 datoshi]
    /// 5A : OpCode.AND	[8 datoshi]
    /// 5B : OpCode.DUP	[2 datoshi]
    /// 5C : OpCode.PUSHINT8 7F	[1 datoshi]
    /// 5E : OpCode.JMPLE 06	[2 datoshi]
    /// 60 : OpCode.PUSHINT16 0001	[1 datoshi]
    /// 63 : OpCode.SUB	[8 datoshi]
    /// 64 : OpCode.RET	[0 datoshi]
    /// 65 : OpCode.PUSH0	[1 datoshi]
    /// 66 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testSByteAdd")]
    public abstract BigInteger? TestSByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oAgC4EIgdKAH8yDwH/AJFKAH8yBgEAAZ9A
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.ADD	[8 datoshi]
    /// 06 : OpCode.DUP	[2 datoshi]
    /// 07 : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 0C : OpCode.JMPGE 04	[2 datoshi]
    /// 0E : OpCode.JMP 0A	[2 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 16 : OpCode.JMPLE 1E	[2 datoshi]
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 21 : OpCode.AND	[8 datoshi]
    /// 22 : OpCode.DUP	[2 datoshi]
    /// 23 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 28 : OpCode.JMPLE 0C	[2 datoshi]
    /// 2A : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 33 : OpCode.SUB	[8 datoshi]
    /// 34 : OpCode.DUP	[2 datoshi]
    /// 35 : OpCode.PUSHINT8 80	[1 datoshi]
    /// 37 : OpCode.JMPGE 04	[2 datoshi]
    /// 39 : OpCode.JMP 07	[2 datoshi]
    /// 3B : OpCode.DUP	[2 datoshi]
    /// 3C : OpCode.PUSHINT8 7F	[1 datoshi]
    /// 3E : OpCode.JMPLE 0F	[2 datoshi]
    /// 40 : OpCode.PUSHINT16 FF00	[1 datoshi]
    /// 43 : OpCode.AND	[8 datoshi]
    /// 44 : OpCode.DUP	[2 datoshi]
    /// 45 : OpCode.PUSHINT8 7F	[1 datoshi]
    /// 47 : OpCode.JMPLE 06	[2 datoshi]
    /// 49 : OpCode.PUSHINT16 0001	[1 datoshi]
    /// 4C : OpCode.SUB	[8 datoshi]
    /// 4D : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testSByteAddNonNullable")]
    public abstract BigInteger? TestSByteAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIF 04	[2 datoshi]
    /// 10 : OpCode.PUSHF	[1 datoshi]
    /// 11 : OpCode.RET	[0 datoshi]
    /// 12 : OpCode.LDARG0	[2 datoshi]
    /// 13 : OpCode.DUP	[2 datoshi]
    /// 14 : OpCode.ISNULL	[2 datoshi]
    /// 15 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 17 : OpCode.THROW	[512 datoshi]
    /// 18 : OpCode.LDARG1	[2 datoshi]
    /// 19 : OpCode.DUP	[2 datoshi]
    /// 1A : OpCode.ISNULL	[2 datoshi]
    /// 1B : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1D : OpCode.THROW	[512 datoshi]
    /// 1E : OpCode.GT	[8 datoshi]
    /// 1F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testSByteCompare")]
    public abstract bool? TestSByteCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.GT	[8 datoshi]
    /// 06 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testSByteCompareNonNullable")]
    public abstract bool? TestSByteCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSH0	[1 datoshi]
    /// 0A : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testSByteDefault")]
    public abstract BigInteger? TestSByteDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testSByteDefaultNonNullable")]
    public abstract BigInteger? TestSByteDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomXnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSgEAgC4EIghKAf9/MhQC//8AAJFKAf9/MggCAAABAJ9AEEA=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIFNOT 5E	[2 datoshi]
    /// 10 : OpCode.LDARG0	[2 datoshi]
    /// 11 : OpCode.DUP	[2 datoshi]
    /// 12 : OpCode.ISNULL	[2 datoshi]
    /// 13 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 15 : OpCode.THROW	[512 datoshi]
    /// 16 : OpCode.LDARG1	[2 datoshi]
    /// 17 : OpCode.DUP	[2 datoshi]
    /// 18 : OpCode.ISNULL	[2 datoshi]
    /// 19 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1B : OpCode.THROW	[512 datoshi]
    /// 1C : OpCode.ADD	[8 datoshi]
    /// 1D : OpCode.DUP	[2 datoshi]
    /// 1E : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 23 : OpCode.JMPGE 04	[2 datoshi]
    /// 25 : OpCode.JMP 0A	[2 datoshi]
    /// 27 : OpCode.DUP	[2 datoshi]
    /// 28 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 2D : OpCode.JMPLE 1E	[2 datoshi]
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 38 : OpCode.AND	[8 datoshi]
    /// 39 : OpCode.DUP	[2 datoshi]
    /// 3A : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 3F : OpCode.JMPLE 0C	[2 datoshi]
    /// 41 : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 4A : OpCode.SUB	[8 datoshi]
    /// 4B : OpCode.DUP	[2 datoshi]
    /// 4C : OpCode.PUSHINT16 0080	[1 datoshi]
    /// 4F : OpCode.JMPGE 04	[2 datoshi]
    /// 51 : OpCode.JMP 08	[2 datoshi]
    /// 53 : OpCode.DUP	[2 datoshi]
    /// 54 : OpCode.PUSHINT16 FF7F	[1 datoshi]
    /// 57 : OpCode.JMPLE 14	[2 datoshi]
    /// 59 : OpCode.PUSHINT32 FFFF0000	[1 datoshi]
    /// 5E : OpCode.AND	[8 datoshi]
    /// 5F : OpCode.DUP	[2 datoshi]
    /// 60 : OpCode.PUSHINT16 FF7F	[1 datoshi]
    /// 63 : OpCode.JMPLE 08	[2 datoshi]
    /// 65 : OpCode.PUSHINT32 00000100	[1 datoshi]
    /// 6A : OpCode.SUB	[8 datoshi]
    /// 6B : OpCode.RET	[0 datoshi]
    /// 6C : OpCode.PUSH0	[1 datoshi]
    /// 6D : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testShortAdd")]
    public abstract BigInteger? TestShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oBAIAuBCIISgH/fzIUAv//AACRSgH/fzIIAgAAAQCfQA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.ADD	[8 datoshi]
    /// 06 : OpCode.DUP	[2 datoshi]
    /// 07 : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 0C : OpCode.JMPGE 04	[2 datoshi]
    /// 0E : OpCode.JMP 0A	[2 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 16 : OpCode.JMPLE 1E	[2 datoshi]
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 21 : OpCode.AND	[8 datoshi]
    /// 22 : OpCode.DUP	[2 datoshi]
    /// 23 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 28 : OpCode.JMPLE 0C	[2 datoshi]
    /// 2A : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 33 : OpCode.SUB	[8 datoshi]
    /// 34 : OpCode.DUP	[2 datoshi]
    /// 35 : OpCode.PUSHINT16 0080	[1 datoshi]
    /// 38 : OpCode.JMPGE 04	[2 datoshi]
    /// 3A : OpCode.JMP 08	[2 datoshi]
    /// 3C : OpCode.DUP	[2 datoshi]
    /// 3D : OpCode.PUSHINT16 FF7F	[1 datoshi]
    /// 40 : OpCode.JMPLE 14	[2 datoshi]
    /// 42 : OpCode.PUSHINT32 FFFF0000	[1 datoshi]
    /// 47 : OpCode.AND	[8 datoshi]
    /// 48 : OpCode.DUP	[2 datoshi]
    /// 49 : OpCode.PUSHINT16 FF7F	[1 datoshi]
    /// 4C : OpCode.JMPLE 08	[2 datoshi]
    /// 4E : OpCode.PUSHINT32 00000100	[1 datoshi]
    /// 53 : OpCode.SUB	[8 datoshi]
    /// 54 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testShortAddNonNullable")]
    public abstract BigInteger? TestShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIF 04	[2 datoshi]
    /// 10 : OpCode.PUSHF	[1 datoshi]
    /// 11 : OpCode.RET	[0 datoshi]
    /// 12 : OpCode.LDARG0	[2 datoshi]
    /// 13 : OpCode.DUP	[2 datoshi]
    /// 14 : OpCode.ISNULL	[2 datoshi]
    /// 15 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 17 : OpCode.THROW	[512 datoshi]
    /// 18 : OpCode.LDARG1	[2 datoshi]
    /// 19 : OpCode.DUP	[2 datoshi]
    /// 1A : OpCode.ISNULL	[2 datoshi]
    /// 1B : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1D : OpCode.THROW	[512 datoshi]
    /// 1E : OpCode.GT	[8 datoshi]
    /// 1F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testShortCompare")]
    public abstract bool? TestShortCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.GT	[8 datoshi]
    /// 06 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testShortCompareNonNullable")]
    public abstract bool? TestShortCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSH0	[1 datoshi]
    /// 0A : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testShortDefault")]
    public abstract BigInteger? TestShortDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testShortDefaultNonNullable")]
    public abstract BigInteger? TestShortDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeErYJgVFDHlK2CYFRQyL2yhA
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSHDATA1	[8 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.DUP	[2 datoshi]
    /// 0D : OpCode.ISNULL	[2 datoshi]
    /// 0E : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 10 : OpCode.DROP	[2 datoshi]
    /// 11 : OpCode.PUSHDATA1	[8 datoshi]
    /// 13 : OpCode.CAT	[2048 datoshi]
    /// 14 : OpCode.CONVERT 28	[8192 datoshi]
    /// 16 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testStringConcat")]
    public abstract string? TestStringConcat(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2yhA
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.CAT	[2048 datoshi]
    /// 06 : OpCode.CONVERT 28	[8192 datoshi]
    /// 08 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testStringConcatNonNullable")]
    public abstract string? TestStringConcatNonNullable(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgVFDEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 05	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSHDATA1	[8 datoshi]
    /// 0B : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testStringDefault")]
    public abstract string? TestStringDefault(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testStringDefaultNonNullable")]
    public abstract string? TestStringDefaultNonNullable(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIF 03	[2 datoshi]
    /// 08 : OpCode.SIZE	[4 datoshi]
    /// 09 : OpCode.DUP	[2 datoshi]
    /// 0A : OpCode.ISNULL	[2 datoshi]
    /// 0B : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 0D : OpCode.DROP	[2 datoshi]
    /// 0E : OpCode.PUSH0	[1 datoshi]
    /// 0F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testStringLength")]
    public abstract BigInteger? TestStringLength(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.SIZE	[4 datoshi]
    /// 05 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testStringLengthNonNullable")]
    public abstract BigInteger? TestStringLengthNonNullable(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIF 03	[2 datoshi]
    /// 08 : OpCode.SIZE	[4 datoshi]
    /// 09 : OpCode.DUP	[2 datoshi]
    /// 0A : OpCode.ISNULL	[2 datoshi]
    /// 0B : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 0D : OpCode.DROP	[2 datoshi]
    /// 0E : OpCode.PUSH0	[1 datoshi]
    /// 0F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160ArrayLength")]
    public abstract BigInteger? TestUInt160ArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.SIZE	[4 datoshi]
    /// 05 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160ArrayLengthNonNullable")]
    public abstract BigInteger? TestUInt160ArrayLengthNonNullable(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJhlFDAAAAAAAAAAAAAAAAAAAAAAAAAAAQA==
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 19	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000	[8 datoshi]
    /// 1F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160Default")]
    public abstract UInt160? TestUInt160Default(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160DefaultNonNullable")]
    public abstract UInt160? TestUInt160DefaultNonNullable(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIF 03	[2 datoshi]
    /// 08 : OpCode.SIZE	[4 datoshi]
    /// 09 : OpCode.DUP	[2 datoshi]
    /// 0A : OpCode.ISNULL	[2 datoshi]
    /// 0B : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 0D : OpCode.DROP	[2 datoshi]
    /// 0E : OpCode.PUSH0	[1 datoshi]
    /// 0F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256ArrayLength")]
    public abstract BigInteger? TestUInt256ArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.SIZE	[4 datoshi]
    /// 05 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256ArrayLengthNonNullable")]
    public abstract BigInteger? TestUInt256ArrayLengthNonNullable(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJiVFDAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQA==
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 25	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000	[8 datoshi]
    /// 2B : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256Default")]
    public abstract UInt256? TestUInt256Default(UInt256? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256DefaultNonNullable")]
    public abstract UInt256? TestUInt256DefaultNonNullable(UInt256? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomLHhK2CYDOnlK2CYDOp5KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRQBBA
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIFNOT 2C	[2 datoshi]
    /// 10 : OpCode.LDARG0	[2 datoshi]
    /// 11 : OpCode.DUP	[2 datoshi]
    /// 12 : OpCode.ISNULL	[2 datoshi]
    /// 13 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 15 : OpCode.THROW	[512 datoshi]
    /// 16 : OpCode.LDARG1	[2 datoshi]
    /// 17 : OpCode.DUP	[2 datoshi]
    /// 18 : OpCode.ISNULL	[2 datoshi]
    /// 19 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1B : OpCode.THROW	[512 datoshi]
    /// 1C : OpCode.ADD	[8 datoshi]
    /// 1D : OpCode.DUP	[2 datoshi]
    /// 1E : OpCode.PUSH0	[1 datoshi]
    /// 1F : OpCode.JMPGE 04	[2 datoshi]
    /// 21 : OpCode.JMP 0E	[2 datoshi]
    /// 23 : OpCode.DUP	[2 datoshi]
    /// 24 : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 2D : OpCode.JMPLE 0C	[2 datoshi]
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 38 : OpCode.AND	[8 datoshi]
    /// 39 : OpCode.RET	[0 datoshi]
    /// 3A : OpCode.PUSH0	[1 datoshi]
    /// 3B : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUIntAdd")]
    public abstract BigInteger? TestUIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUA=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.ADD	[8 datoshi]
    /// 06 : OpCode.DUP	[2 datoshi]
    /// 07 : OpCode.PUSH0	[1 datoshi]
    /// 08 : OpCode.JMPGE 04	[2 datoshi]
    /// 0A : OpCode.JMP 0E	[2 datoshi]
    /// 0C : OpCode.DUP	[2 datoshi]
    /// 0D : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 16 : OpCode.JMPLE 0C	[2 datoshi]
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 21 : OpCode.AND	[8 datoshi]
    /// 22 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUIntAddNonNullable")]
    public abstract BigInteger? TestUIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIF 04	[2 datoshi]
    /// 10 : OpCode.PUSHF	[1 datoshi]
    /// 11 : OpCode.RET	[0 datoshi]
    /// 12 : OpCode.LDARG0	[2 datoshi]
    /// 13 : OpCode.DUP	[2 datoshi]
    /// 14 : OpCode.ISNULL	[2 datoshi]
    /// 15 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 17 : OpCode.THROW	[512 datoshi]
    /// 18 : OpCode.LDARG1	[2 datoshi]
    /// 19 : OpCode.DUP	[2 datoshi]
    /// 1A : OpCode.ISNULL	[2 datoshi]
    /// 1B : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1D : OpCode.THROW	[512 datoshi]
    /// 1E : OpCode.GT	[8 datoshi]
    /// 1F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUIntCompare")]
    public abstract bool? TestUIntCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.GT	[8 datoshi]
    /// 06 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUIntCompareNonNullable")]
    public abstract bool? TestUIntCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSH0	[1 datoshi]
    /// 0A : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUIntDefault")]
    public abstract BigInteger? TestUIntDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUIntDefaultNonNullable")]
    public abstract BigInteger? TestUIntDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomPHhK2CYDOnlK2CYDOp5KEC4EIhZKBP//////////AAAAAAAAAAAyFAT//////////wAAAAAAAAAAkUAQQA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIFNOT 3C	[2 datoshi]
    /// 10 : OpCode.LDARG0	[2 datoshi]
    /// 11 : OpCode.DUP	[2 datoshi]
    /// 12 : OpCode.ISNULL	[2 datoshi]
    /// 13 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 15 : OpCode.THROW	[512 datoshi]
    /// 16 : OpCode.LDARG1	[2 datoshi]
    /// 17 : OpCode.DUP	[2 datoshi]
    /// 18 : OpCode.ISNULL	[2 datoshi]
    /// 19 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1B : OpCode.THROW	[512 datoshi]
    /// 1C : OpCode.ADD	[8 datoshi]
    /// 1D : OpCode.DUP	[2 datoshi]
    /// 1E : OpCode.PUSH0	[1 datoshi]
    /// 1F : OpCode.JMPGE 04	[2 datoshi]
    /// 21 : OpCode.JMP 16	[2 datoshi]
    /// 23 : OpCode.DUP	[2 datoshi]
    /// 24 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000	[4 datoshi]
    /// 35 : OpCode.JMPLE 14	[2 datoshi]
    /// 37 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000	[4 datoshi]
    /// 48 : OpCode.AND	[8 datoshi]
    /// 49 : OpCode.RET	[0 datoshi]
    /// 4A : OpCode.PUSH0	[1 datoshi]
    /// 4B : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testULongAdd")]
    public abstract BigInteger? TestULongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeShAuBCIWSgT//////////wAAAAAAAAAAMhQE//////////8AAAAAAAAAAJFA
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.ADD	[8 datoshi]
    /// 06 : OpCode.DUP	[2 datoshi]
    /// 07 : OpCode.PUSH0	[1 datoshi]
    /// 08 : OpCode.JMPGE 04	[2 datoshi]
    /// 0A : OpCode.JMP 16	[2 datoshi]
    /// 0C : OpCode.DUP	[2 datoshi]
    /// 0D : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000	[4 datoshi]
    /// 1E : OpCode.JMPLE 14	[2 datoshi]
    /// 20 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000	[4 datoshi]
    /// 31 : OpCode.AND	[8 datoshi]
    /// 32 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testULongAddNonNullable")]
    public abstract BigInteger? TestULongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIF 04	[2 datoshi]
    /// 10 : OpCode.PUSHF	[1 datoshi]
    /// 11 : OpCode.RET	[0 datoshi]
    /// 12 : OpCode.LDARG0	[2 datoshi]
    /// 13 : OpCode.DUP	[2 datoshi]
    /// 14 : OpCode.ISNULL	[2 datoshi]
    /// 15 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 17 : OpCode.THROW	[512 datoshi]
    /// 18 : OpCode.LDARG1	[2 datoshi]
    /// 19 : OpCode.DUP	[2 datoshi]
    /// 1A : OpCode.ISNULL	[2 datoshi]
    /// 1B : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1D : OpCode.THROW	[512 datoshi]
    /// 1E : OpCode.GT	[8 datoshi]
    /// 1F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testULongCompare")]
    public abstract bool? TestULongCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.GT	[8 datoshi]
    /// 06 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testULongCompareNonNullable")]
    public abstract bool? TestULongCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSH0	[1 datoshi]
    /// 0A : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testULongDefault")]
    public abstract BigInteger? TestULongDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testULongDefaultNonNullable")]
    public abstract BigInteger? TestULongDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomUnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIKSgL//wAAMggC//8AAJFAEEA=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIFNOT 52	[2 datoshi]
    /// 10 : OpCode.LDARG0	[2 datoshi]
    /// 11 : OpCode.DUP	[2 datoshi]
    /// 12 : OpCode.ISNULL	[2 datoshi]
    /// 13 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 15 : OpCode.THROW	[512 datoshi]
    /// 16 : OpCode.LDARG1	[2 datoshi]
    /// 17 : OpCode.DUP	[2 datoshi]
    /// 18 : OpCode.ISNULL	[2 datoshi]
    /// 19 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1B : OpCode.THROW	[512 datoshi]
    /// 1C : OpCode.ADD	[8 datoshi]
    /// 1D : OpCode.DUP	[2 datoshi]
    /// 1E : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 23 : OpCode.JMPGE 04	[2 datoshi]
    /// 25 : OpCode.JMP 0A	[2 datoshi]
    /// 27 : OpCode.DUP	[2 datoshi]
    /// 28 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 2D : OpCode.JMPLE 1E	[2 datoshi]
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 38 : OpCode.AND	[8 datoshi]
    /// 39 : OpCode.DUP	[2 datoshi]
    /// 3A : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 3F : OpCode.JMPLE 0C	[2 datoshi]
    /// 41 : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 4A : OpCode.SUB	[8 datoshi]
    /// 4B : OpCode.DUP	[2 datoshi]
    /// 4C : OpCode.PUSH0	[1 datoshi]
    /// 4D : OpCode.JMPGE 04	[2 datoshi]
    /// 4F : OpCode.JMP 0A	[2 datoshi]
    /// 51 : OpCode.DUP	[2 datoshi]
    /// 52 : OpCode.PUSHINT32 FFFF0000	[1 datoshi]
    /// 57 : OpCode.JMPLE 08	[2 datoshi]
    /// 59 : OpCode.PUSHINT32 FFFF0000	[1 datoshi]
    /// 5E : OpCode.AND	[8 datoshi]
    /// 5F : OpCode.RET	[0 datoshi]
    /// 60 : OpCode.PUSH0	[1 datoshi]
    /// 61 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUShortAdd")]
    public abstract BigInteger? TestUShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCkoC//8AADIIAv//AACRQA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.ADD	[8 datoshi]
    /// 06 : OpCode.DUP	[2 datoshi]
    /// 07 : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 0C : OpCode.JMPGE 04	[2 datoshi]
    /// 0E : OpCode.JMP 0A	[2 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 16 : OpCode.JMPLE 1E	[2 datoshi]
    /// 18 : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 21 : OpCode.AND	[8 datoshi]
    /// 22 : OpCode.DUP	[2 datoshi]
    /// 23 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 28 : OpCode.JMPLE 0C	[2 datoshi]
    /// 2A : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 33 : OpCode.SUB	[8 datoshi]
    /// 34 : OpCode.DUP	[2 datoshi]
    /// 35 : OpCode.PUSH0	[1 datoshi]
    /// 36 : OpCode.JMPGE 04	[2 datoshi]
    /// 38 : OpCode.JMP 0A	[2 datoshi]
    /// 3A : OpCode.DUP	[2 datoshi]
    /// 3B : OpCode.PUSHINT32 FFFF0000	[1 datoshi]
    /// 40 : OpCode.JMPLE 08	[2 datoshi]
    /// 42 : OpCode.PUSHINT32 FFFF0000	[1 datoshi]
    /// 47 : OpCode.AND	[8 datoshi]
    /// 48 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUShortAddNonNullable")]
    public abstract BigInteger? TestUShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.ISNULL	[2 datoshi]
    /// 05 : OpCode.NOT	[4 datoshi]
    /// 06 : OpCode.JMPIF 05	[2 datoshi]
    /// 08 : OpCode.PUSHF	[1 datoshi]
    /// 09 : OpCode.JMP 05	[2 datoshi]
    /// 0B : OpCode.LDARG1	[2 datoshi]
    /// 0C : OpCode.ISNULL	[2 datoshi]
    /// 0D : OpCode.NOT	[4 datoshi]
    /// 0E : OpCode.JMPIF 04	[2 datoshi]
    /// 10 : OpCode.PUSHF	[1 datoshi]
    /// 11 : OpCode.RET	[0 datoshi]
    /// 12 : OpCode.LDARG0	[2 datoshi]
    /// 13 : OpCode.DUP	[2 datoshi]
    /// 14 : OpCode.ISNULL	[2 datoshi]
    /// 15 : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 17 : OpCode.THROW	[512 datoshi]
    /// 18 : OpCode.LDARG1	[2 datoshi]
    /// 19 : OpCode.DUP	[2 datoshi]
    /// 1A : OpCode.ISNULL	[2 datoshi]
    /// 1B : OpCode.JMPIFNOT 03	[2 datoshi]
    /// 1D : OpCode.THROW	[512 datoshi]
    /// 1E : OpCode.GT	[8 datoshi]
    /// 1F : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUShortCompare")]
    public abstract bool? TestUShortCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 00 : OpCode.INITSLOT 0002	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.LDARG1	[2 datoshi]
    /// 05 : OpCode.GT	[8 datoshi]
    /// 06 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUShortCompareNonNullable")]
    public abstract bool? TestUShortCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.ISNULL	[2 datoshi]
    /// 06 : OpCode.JMPIFNOT 04	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.PUSH0	[1 datoshi]
    /// 0A : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUShortDefault")]
    public abstract BigInteger? TestUShortDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUShortDefaultNonNullable")]
    public abstract BigInteger? TestUShortDefaultNonNullable(BigInteger? a);

    #endregion
}
