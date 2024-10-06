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
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIFNOT 10
    /// 0010 : OpCode.LDARG0
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.ISNULL
    /// 0013 : OpCode.JMPIFNOT 03
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.LDARG1
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.ISNULL
    /// 0019 : OpCode.JMPIFNOT 03
    /// 001B : OpCode.THROW
    /// 001C : OpCode.ADD
    /// 001D : OpCode.RET
    /// 001E : OpCode.PUSH0
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerAdd")]
    public abstract BigInteger? TestBigIntegerAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.ADD
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerAddNonNullable")]
    public abstract BigInteger? TestBigIntegerAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIF 04
    /// 0010 : OpCode.PUSHF
    /// 0011 : OpCode.RET
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.ISNULL
    /// 0015 : OpCode.JMPIFNOT 03
    /// 0017 : OpCode.THROW
    /// 0018 : OpCode.LDARG1
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.ISNULL
    /// 001B : OpCode.JMPIFNOT 03
    /// 001D : OpCode.THROW
    /// 001E : OpCode.GT
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerCompare")]
    public abstract bool? TestBigIntegerCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.GT
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerCompareNonNullable")]
    public abstract bool? TestBigIntegerCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 04
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerDefault")]
    public abstract BigInteger? TestBigIntegerDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("testBigIntegerDefaultNonNullable")]
    public abstract BigInteger? TestBigIntegerDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6JAQJQHlK2CYDOkA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIF 04
    /// 0010 : OpCode.PUSHF
    /// 0011 : OpCode.RET
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.ISNULL
    /// 0015 : OpCode.JMPIFNOT 03
    /// 0017 : OpCode.THROW
    /// 0018 : OpCode.JMPIF 04
    /// 001A : OpCode.PUSHF
    /// 001B : OpCode.RET
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ISNULL
    /// 001F : OpCode.JMPIFNOT 03
    /// 0021 : OpCode.THROW
    /// 0022 : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolAnd")]
    public abstract bool? TestBoolAnd(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCQECUB5QA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.JMPIF 04
    /// 0006 : OpCode.PUSHF
    /// 0007 : OpCode.RET
    /// 0008 : OpCode.LDARG1
    /// 0009 : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolAndNonNullable")]
    public abstract bool? TestBoolAndNonNullable(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFCUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 04
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSHF
    /// 000A : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolDefault")]
    public abstract bool? TestBoolDefault(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolDefaultNonNullable")]
    public abstract bool? TestBoolDefaultNonNullable(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6JgQIQHlK2CYDOkA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIF 04
    /// 0010 : OpCode.PUSHF
    /// 0011 : OpCode.RET
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.ISNULL
    /// 0015 : OpCode.JMPIFNOT 03
    /// 0017 : OpCode.THROW
    /// 0018 : OpCode.JMPIFNOT 04
    /// 001A : OpCode.PUSHT
    /// 001B : OpCode.RET
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ISNULL
    /// 001F : OpCode.JMPIFNOT 03
    /// 0021 : OpCode.THROW
    /// 0022 : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolOr")]
    public abstract bool? TestBoolOr(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCYECEB5QA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.JMPIFNOT 04
    /// 0006 : OpCode.PUSHT
    /// 0007 : OpCode.RET
    /// 0008 : OpCode.LDARG1
    /// 0009 : OpCode.RET
    /// </remarks>
    [DisplayName("testBoolOrNonNullable")]
    public abstract bool? TestBoolOrNonNullable(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomTnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIISgH/ADIGAf8AkUAQQA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIFNOT 4E
    /// 0010 : OpCode.LDARG0
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.ISNULL
    /// 0013 : OpCode.JMPIFNOT 03
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.LDARG1
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.ISNULL
    /// 0019 : OpCode.JMPIFNOT 03
    /// 001B : OpCode.THROW
    /// 001C : OpCode.ADD
    /// 001D : OpCode.DUP
    /// 001E : OpCode.PUSHINT32 00000080
    /// 0023 : OpCode.JMPGE 04
    /// 0025 : OpCode.JMP 0A
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.PUSHINT32 FFFFFF7F
    /// 002D : OpCode.JMPLE 1E
    /// 002F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0038 : OpCode.AND
    /// 0039 : OpCode.DUP
    /// 003A : OpCode.PUSHINT32 FFFFFF7F
    /// 003F : OpCode.JMPLE 0C
    /// 0041 : OpCode.PUSHINT64 0000000001000000
    /// 004A : OpCode.SUB
    /// 004B : OpCode.DUP
    /// 004C : OpCode.PUSH0
    /// 004D : OpCode.JMPGE 04
    /// 004F : OpCode.JMP 08
    /// 0051 : OpCode.DUP
    /// 0052 : OpCode.PUSHINT16 FF00
    /// 0055 : OpCode.JMPLE 06
    /// 0057 : OpCode.PUSHINT16 FF00
    /// 005A : OpCode.AND
    /// 005B : OpCode.RET
    /// 005C : OpCode.PUSH0
    /// 005D : OpCode.RET
    /// </remarks>
    [DisplayName("testByteAdd")]
    public abstract BigInteger? TestByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCEoB/wAyBgH/AJFA
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.ADD
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSHINT32 00000080
    /// 000C : OpCode.JMPGE 04
    /// 000E : OpCode.JMP 0A
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHINT32 FFFFFF7F
    /// 0016 : OpCode.JMPLE 1E
    /// 0018 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0021 : OpCode.AND
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSHINT32 FFFFFF7F
    /// 0028 : OpCode.JMPLE 0C
    /// 002A : OpCode.PUSHINT64 0000000001000000
    /// 0033 : OpCode.SUB
    /// 0034 : OpCode.DUP
    /// 0035 : OpCode.PUSH0
    /// 0036 : OpCode.JMPGE 04
    /// 0038 : OpCode.JMP 08
    /// 003A : OpCode.DUP
    /// 003B : OpCode.PUSHINT16 FF00
    /// 003E : OpCode.JMPLE 06
    /// 0040 : OpCode.PUSHINT16 FF00
    /// 0043 : OpCode.AND
    /// 0044 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteAddNonNullable")]
    public abstract BigInteger? TestByteAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 03
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.ISNULL
    /// 000B : OpCode.JMPIFNOT 04
    /// 000D : OpCode.DROP
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.RET
    /// </remarks>
    [DisplayName("testByteArrayLength")]
    public abstract BigInteger? TestByteArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.SIZE
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteArrayLengthNonNullable")]
    public abstract BigInteger? TestByteArrayLengthNonNullable(byte[]? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIF 04
    /// 0010 : OpCode.PUSHF
    /// 0011 : OpCode.RET
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.ISNULL
    /// 0015 : OpCode.JMPIFNOT 03
    /// 0017 : OpCode.THROW
    /// 0018 : OpCode.LDARG1
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.ISNULL
    /// 001B : OpCode.JMPIFNOT 03
    /// 001D : OpCode.THROW
    /// 001E : OpCode.GT
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("testByteCompare")]
    public abstract bool? TestByteCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.GT
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteCompareNonNullable")]
    public abstract bool? TestByteCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 04
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.RET
    /// </remarks>
    [DisplayName("testByteDefault")]
    public abstract BigInteger? TestByteDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteDefaultNonNullable")]
    public abstract BigInteger? TestByteDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomPnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQBBA
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIFNOT 3E
    /// 0010 : OpCode.LDARG0
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.ISNULL
    /// 0013 : OpCode.JMPIFNOT 03
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.LDARG1
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.ISNULL
    /// 0019 : OpCode.JMPIFNOT 03
    /// 001B : OpCode.THROW
    /// 001C : OpCode.ADD
    /// 001D : OpCode.DUP
    /// 001E : OpCode.PUSHINT32 00000080
    /// 0023 : OpCode.JMPGE 04
    /// 0025 : OpCode.JMP 0A
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.PUSHINT32 FFFFFF7F
    /// 002D : OpCode.JMPLE 1E
    /// 002F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0038 : OpCode.AND
    /// 0039 : OpCode.DUP
    /// 003A : OpCode.PUSHINT32 FFFFFF7F
    /// 003F : OpCode.JMPLE 0C
    /// 0041 : OpCode.PUSHINT64 0000000001000000
    /// 004A : OpCode.SUB
    /// 004B : OpCode.RET
    /// 004C : OpCode.PUSH0
    /// 004D : OpCode.RET
    /// </remarks>
    [DisplayName("testIntAdd")]
    public abstract BigInteger? TestIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.ADD
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSHINT32 00000080
    /// 000C : OpCode.JMPGE 04
    /// 000E : OpCode.JMP 0A
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHINT32 FFFFFF7F
    /// 0016 : OpCode.JMPLE 1E
    /// 0018 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0021 : OpCode.AND
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSHINT32 FFFFFF7F
    /// 0028 : OpCode.JMPLE 0C
    /// 002A : OpCode.PUSHINT64 0000000001000000
    /// 0033 : OpCode.SUB
    /// 0034 : OpCode.RET
    /// </remarks>
    [DisplayName("testIntAddNonNullable")]
    public abstract BigInteger? TestIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIF 04
    /// 0010 : OpCode.PUSHF
    /// 0011 : OpCode.RET
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.ISNULL
    /// 0015 : OpCode.JMPIFNOT 03
    /// 0017 : OpCode.THROW
    /// 0018 : OpCode.LDARG1
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.ISNULL
    /// 001B : OpCode.JMPIFNOT 03
    /// 001D : OpCode.THROW
    /// 001E : OpCode.GT
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("testIntCompare")]
    public abstract bool? TestIntCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.GT
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("testIntCompareNonNullable")]
    public abstract bool? TestIntCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 04
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.RET
    /// </remarks>
    [DisplayName("testIntDefault")]
    public abstract BigInteger? TestIntDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("testIntDefaultNonNullable")]
    public abstract BigInteger? TestIntDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomWnhK2CYDOnlK2CYDOp5KAwAAAAAAAACALgQiDkoD/////////38yMgT//////////wAAAAAAAAAAkUoD/////////38yFAQAAAAAAAAAAAEAAAAAAAAAn0AQQA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIFNOT 5A
    /// 0010 : OpCode.LDARG0
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.ISNULL
    /// 0013 : OpCode.JMPIFNOT 03
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.LDARG1
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.ISNULL
    /// 0019 : OpCode.JMPIFNOT 03
    /// 001B : OpCode.THROW
    /// 001C : OpCode.ADD
    /// 001D : OpCode.DUP
    /// 001E : OpCode.PUSHINT64 0000000000000080
    /// 0027 : OpCode.JMPGE 04
    /// 0029 : OpCode.JMP 0E
    /// 002B : OpCode.DUP
    /// 002C : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 0035 : OpCode.JMPLE 32
    /// 0037 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0048 : OpCode.AND
    /// 0049 : OpCode.DUP
    /// 004A : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 0053 : OpCode.JMPLE 14
    /// 0055 : OpCode.PUSHINT128 00000000000000000100000000000000
    /// 0066 : OpCode.SUB
    /// 0067 : OpCode.RET
    /// 0068 : OpCode.PUSH0
    /// 0069 : OpCode.RET
    /// </remarks>
    [DisplayName("testLongAdd")]
    public abstract BigInteger? TestLongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgMAAAAAAAAAgC4EIg5KA/////////9/MjIE//////////8AAAAAAAAAAJFKA/////////9/MhQEAAAAAAAAAAABAAAAAAAAAJ9A
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.ADD
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSHINT64 0000000000000080
    /// 0010 : OpCode.JMPGE 04
    /// 0012 : OpCode.JMP 0E
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 001E : OpCode.JMPLE 32
    /// 0020 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0031 : OpCode.AND
    /// 0032 : OpCode.DUP
    /// 0033 : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 003C : OpCode.JMPLE 14
    /// 003E : OpCode.PUSHINT128 00000000000000000100000000000000
    /// 004F : OpCode.SUB
    /// 0050 : OpCode.RET
    /// </remarks>
    [DisplayName("testLongAddNonNullable")]
    public abstract BigInteger? TestLongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIF 04
    /// 0010 : OpCode.PUSHF
    /// 0011 : OpCode.RET
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.ISNULL
    /// 0015 : OpCode.JMPIFNOT 03
    /// 0017 : OpCode.THROW
    /// 0018 : OpCode.LDARG1
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.ISNULL
    /// 001B : OpCode.JMPIFNOT 03
    /// 001D : OpCode.THROW
    /// 001E : OpCode.GT
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("testLongCompare")]
    public abstract bool? TestLongCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.GT
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("testLongCompareNonNullable")]
    public abstract bool? TestLongCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 04
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.RET
    /// </remarks>
    [DisplayName("testLongDefault")]
    public abstract BigInteger? TestLongDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("testLongDefaultNonNullable")]
    public abstract BigInteger? TestLongDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomV3hK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSgCALgQiB0oAfzIPAf8AkUoAfzIGAQABn0AQQA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIFNOT 57
    /// 0010 : OpCode.LDARG0
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.ISNULL
    /// 0013 : OpCode.JMPIFNOT 03
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.LDARG1
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.ISNULL
    /// 0019 : OpCode.JMPIFNOT 03
    /// 001B : OpCode.THROW
    /// 001C : OpCode.ADD
    /// 001D : OpCode.DUP
    /// 001E : OpCode.PUSHINT32 00000080
    /// 0023 : OpCode.JMPGE 04
    /// 0025 : OpCode.JMP 0A
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.PUSHINT32 FFFFFF7F
    /// 002D : OpCode.JMPLE 1E
    /// 002F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0038 : OpCode.AND
    /// 0039 : OpCode.DUP
    /// 003A : OpCode.PUSHINT32 FFFFFF7F
    /// 003F : OpCode.JMPLE 0C
    /// 0041 : OpCode.PUSHINT64 0000000001000000
    /// 004A : OpCode.SUB
    /// 004B : OpCode.DUP
    /// 004C : OpCode.PUSHINT8 80
    /// 004E : OpCode.JMPGE 04
    /// 0050 : OpCode.JMP 07
    /// 0052 : OpCode.DUP
    /// 0053 : OpCode.PUSHINT8 7F
    /// 0055 : OpCode.JMPLE 0F
    /// 0057 : OpCode.PUSHINT16 FF00
    /// 005A : OpCode.AND
    /// 005B : OpCode.DUP
    /// 005C : OpCode.PUSHINT8 7F
    /// 005E : OpCode.JMPLE 06
    /// 0060 : OpCode.PUSHINT16 0001
    /// 0063 : OpCode.SUB
    /// 0064 : OpCode.RET
    /// 0065 : OpCode.PUSH0
    /// 0066 : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteAdd")]
    public abstract BigInteger? TestSByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oAgC4EIgdKAH8yDwH/AJFKAH8yBgEAAZ9A
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.ADD
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSHINT32 00000080
    /// 000C : OpCode.JMPGE 04
    /// 000E : OpCode.JMP 0A
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHINT32 FFFFFF7F
    /// 0016 : OpCode.JMPLE 1E
    /// 0018 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0021 : OpCode.AND
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSHINT32 FFFFFF7F
    /// 0028 : OpCode.JMPLE 0C
    /// 002A : OpCode.PUSHINT64 0000000001000000
    /// 0033 : OpCode.SUB
    /// 0034 : OpCode.DUP
    /// 0035 : OpCode.PUSHINT8 80
    /// 0037 : OpCode.JMPGE 04
    /// 0039 : OpCode.JMP 07
    /// 003B : OpCode.DUP
    /// 003C : OpCode.PUSHINT8 7F
    /// 003E : OpCode.JMPLE 0F
    /// 0040 : OpCode.PUSHINT16 FF00
    /// 0043 : OpCode.AND
    /// 0044 : OpCode.DUP
    /// 0045 : OpCode.PUSHINT8 7F
    /// 0047 : OpCode.JMPLE 06
    /// 0049 : OpCode.PUSHINT16 0001
    /// 004C : OpCode.SUB
    /// 004D : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteAddNonNullable")]
    public abstract BigInteger? TestSByteAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIF 04
    /// 0010 : OpCode.PUSHF
    /// 0011 : OpCode.RET
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.ISNULL
    /// 0015 : OpCode.JMPIFNOT 03
    /// 0017 : OpCode.THROW
    /// 0018 : OpCode.LDARG1
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.ISNULL
    /// 001B : OpCode.JMPIFNOT 03
    /// 001D : OpCode.THROW
    /// 001E : OpCode.GT
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteCompare")]
    public abstract bool? TestSByteCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.GT
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteCompareNonNullable")]
    public abstract bool? TestSByteCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 04
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteDefault")]
    public abstract BigInteger? TestSByteDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("testSByteDefaultNonNullable")]
    public abstract BigInteger? TestSByteDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomXnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSgEAgC4EIghKAf9/MhQC//8AAJFKAf9/MggCAAABAJ9AEEA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIFNOT 5E
    /// 0010 : OpCode.LDARG0
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.ISNULL
    /// 0013 : OpCode.JMPIFNOT 03
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.LDARG1
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.ISNULL
    /// 0019 : OpCode.JMPIFNOT 03
    /// 001B : OpCode.THROW
    /// 001C : OpCode.ADD
    /// 001D : OpCode.DUP
    /// 001E : OpCode.PUSHINT32 00000080
    /// 0023 : OpCode.JMPGE 04
    /// 0025 : OpCode.JMP 0A
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.PUSHINT32 FFFFFF7F
    /// 002D : OpCode.JMPLE 1E
    /// 002F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0038 : OpCode.AND
    /// 0039 : OpCode.DUP
    /// 003A : OpCode.PUSHINT32 FFFFFF7F
    /// 003F : OpCode.JMPLE 0C
    /// 0041 : OpCode.PUSHINT64 0000000001000000
    /// 004A : OpCode.SUB
    /// 004B : OpCode.DUP
    /// 004C : OpCode.PUSHINT16 0080
    /// 004F : OpCode.JMPGE 04
    /// 0051 : OpCode.JMP 08
    /// 0053 : OpCode.DUP
    /// 0054 : OpCode.PUSHINT16 FF7F
    /// 0057 : OpCode.JMPLE 14
    /// 0059 : OpCode.PUSHINT32 FFFF0000
    /// 005E : OpCode.AND
    /// 005F : OpCode.DUP
    /// 0060 : OpCode.PUSHINT16 FF7F
    /// 0063 : OpCode.JMPLE 08
    /// 0065 : OpCode.PUSHINT32 00000100
    /// 006A : OpCode.SUB
    /// 006B : OpCode.RET
    /// 006C : OpCode.PUSH0
    /// 006D : OpCode.RET
    /// </remarks>
    [DisplayName("testShortAdd")]
    public abstract BigInteger? TestShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oBAIAuBCIISgH/fzIUAv//AACRSgH/fzIIAgAAAQCfQA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.ADD
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSHINT32 00000080
    /// 000C : OpCode.JMPGE 04
    /// 000E : OpCode.JMP 0A
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHINT32 FFFFFF7F
    /// 0016 : OpCode.JMPLE 1E
    /// 0018 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0021 : OpCode.AND
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSHINT32 FFFFFF7F
    /// 0028 : OpCode.JMPLE 0C
    /// 002A : OpCode.PUSHINT64 0000000001000000
    /// 0033 : OpCode.SUB
    /// 0034 : OpCode.DUP
    /// 0035 : OpCode.PUSHINT16 0080
    /// 0038 : OpCode.JMPGE 04
    /// 003A : OpCode.JMP 08
    /// 003C : OpCode.DUP
    /// 003D : OpCode.PUSHINT16 FF7F
    /// 0040 : OpCode.JMPLE 14
    /// 0042 : OpCode.PUSHINT32 FFFF0000
    /// 0047 : OpCode.AND
    /// 0048 : OpCode.DUP
    /// 0049 : OpCode.PUSHINT16 FF7F
    /// 004C : OpCode.JMPLE 08
    /// 004E : OpCode.PUSHINT32 00000100
    /// 0053 : OpCode.SUB
    /// 0054 : OpCode.RET
    /// </remarks>
    [DisplayName("testShortAddNonNullable")]
    public abstract BigInteger? TestShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIF 04
    /// 0010 : OpCode.PUSHF
    /// 0011 : OpCode.RET
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.ISNULL
    /// 0015 : OpCode.JMPIFNOT 03
    /// 0017 : OpCode.THROW
    /// 0018 : OpCode.LDARG1
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.ISNULL
    /// 001B : OpCode.JMPIFNOT 03
    /// 001D : OpCode.THROW
    /// 001E : OpCode.GT
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("testShortCompare")]
    public abstract bool? TestShortCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.GT
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("testShortCompareNonNullable")]
    public abstract bool? TestShortCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 04
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.RET
    /// </remarks>
    [DisplayName("testShortDefault")]
    public abstract BigInteger? TestShortDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("testShortDefaultNonNullable")]
    public abstract BigInteger? TestShortDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeErYJgVFDHlK2CYFRQyL2yhA
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSHDATA1
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.DUP
    /// 000D : OpCode.ISNULL
    /// 000E : OpCode.JMPIFNOT 05
    /// 0010 : OpCode.DROP
    /// 0011 : OpCode.PUSHDATA1
    /// 0013 : OpCode.CAT
    /// 0014 : OpCode.CONVERT 28
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("testStringConcat")]
    public abstract string? TestStringConcat(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2yhA
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.CAT
    /// 0006 : OpCode.CONVERT 28
    /// 0008 : OpCode.RET
    /// </remarks>
    [DisplayName("testStringConcatNonNullable")]
    public abstract string? TestStringConcatNonNullable(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgVFDEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSHDATA1
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("testStringDefault")]
    public abstract string? TestStringDefault(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("testStringDefaultNonNullable")]
    public abstract string? TestStringDefaultNonNullable(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 03
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.ISNULL
    /// 000B : OpCode.JMPIFNOT 04
    /// 000D : OpCode.DROP
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.RET
    /// </remarks>
    [DisplayName("testStringLength")]
    public abstract BigInteger? TestStringLength(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.SIZE
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("testStringLengthNonNullable")]
    public abstract BigInteger? TestStringLengthNonNullable(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 03
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.ISNULL
    /// 000B : OpCode.JMPIFNOT 04
    /// 000D : OpCode.DROP
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt160ArrayLength")]
    public abstract BigInteger? TestUInt160ArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.SIZE
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt160ArrayLengthNonNullable")]
    public abstract BigInteger? TestUInt160ArrayLengthNonNullable(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJhlFDAAAAAAAAAAAAAAAAAAAAAAAAAAAQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 19
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt160Default")]
    public abstract UInt160? TestUInt160Default(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt160DefaultNonNullable")]
    public abstract UInt160? TestUInt160DefaultNonNullable(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 03
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.ISNULL
    /// 000B : OpCode.JMPIFNOT 04
    /// 000D : OpCode.DROP
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt256ArrayLength")]
    public abstract BigInteger? TestUInt256ArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.SIZE
    /// 0005 : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt256ArrayLengthNonNullable")]
    public abstract BigInteger? TestUInt256ArrayLengthNonNullable(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJiVFDAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 25
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000
    /// 002B : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt256Default")]
    public abstract UInt256? TestUInt256Default(UInt256? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("testUInt256DefaultNonNullable")]
    public abstract UInt256? TestUInt256DefaultNonNullable(UInt256? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomLHhK2CYDOnlK2CYDOp5KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRQBBA
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIFNOT 2C
    /// 0010 : OpCode.LDARG0
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.ISNULL
    /// 0013 : OpCode.JMPIFNOT 03
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.LDARG1
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.ISNULL
    /// 0019 : OpCode.JMPIFNOT 03
    /// 001B : OpCode.THROW
    /// 001C : OpCode.ADD
    /// 001D : OpCode.DUP
    /// 001E : OpCode.PUSH0
    /// 001F : OpCode.JMPGE 04
    /// 0021 : OpCode.JMP 0E
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 002D : OpCode.JMPLE 0C
    /// 002F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0038 : OpCode.AND
    /// 0039 : OpCode.RET
    /// 003A : OpCode.PUSH0
    /// 003B : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntAdd")]
    public abstract BigInteger? TestUIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.ADD
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSH0
    /// 0008 : OpCode.JMPGE 04
    /// 000A : OpCode.JMP 0E
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0016 : OpCode.JMPLE 0C
    /// 0018 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0021 : OpCode.AND
    /// 0022 : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntAddNonNullable")]
    public abstract BigInteger? TestUIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIF 04
    /// 0010 : OpCode.PUSHF
    /// 0011 : OpCode.RET
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.ISNULL
    /// 0015 : OpCode.JMPIFNOT 03
    /// 0017 : OpCode.THROW
    /// 0018 : OpCode.LDARG1
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.ISNULL
    /// 001B : OpCode.JMPIFNOT 03
    /// 001D : OpCode.THROW
    /// 001E : OpCode.GT
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntCompare")]
    public abstract bool? TestUIntCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.GT
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntCompareNonNullable")]
    public abstract bool? TestUIntCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 04
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntDefault")]
    public abstract BigInteger? TestUIntDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("testUIntDefaultNonNullable")]
    public abstract BigInteger? TestUIntDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomPHhK2CYDOnlK2CYDOp5KEC4EIhZKBP//////////AAAAAAAAAAAyFAT//////////wAAAAAAAAAAkUAQQA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIFNOT 3C
    /// 0010 : OpCode.LDARG0
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.ISNULL
    /// 0013 : OpCode.JMPIFNOT 03
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.LDARG1
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.ISNULL
    /// 0019 : OpCode.JMPIFNOT 03
    /// 001B : OpCode.THROW
    /// 001C : OpCode.ADD
    /// 001D : OpCode.DUP
    /// 001E : OpCode.PUSH0
    /// 001F : OpCode.JMPGE 04
    /// 0021 : OpCode.JMP 16
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0035 : OpCode.JMPLE 14
    /// 0037 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0048 : OpCode.AND
    /// 0049 : OpCode.RET
    /// 004A : OpCode.PUSH0
    /// 004B : OpCode.RET
    /// </remarks>
    [DisplayName("testULongAdd")]
    public abstract BigInteger? TestULongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeShAuBCIWSgT//////////wAAAAAAAAAAMhQE//////////8AAAAAAAAAAJFA
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.ADD
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSH0
    /// 0008 : OpCode.JMPGE 04
    /// 000A : OpCode.JMP 16
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 001E : OpCode.JMPLE 14
    /// 0020 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0031 : OpCode.AND
    /// 0032 : OpCode.RET
    /// </remarks>
    [DisplayName("testULongAddNonNullable")]
    public abstract BigInteger? TestULongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIF 04
    /// 0010 : OpCode.PUSHF
    /// 0011 : OpCode.RET
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.ISNULL
    /// 0015 : OpCode.JMPIFNOT 03
    /// 0017 : OpCode.THROW
    /// 0018 : OpCode.LDARG1
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.ISNULL
    /// 001B : OpCode.JMPIFNOT 03
    /// 001D : OpCode.THROW
    /// 001E : OpCode.GT
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("testULongCompare")]
    public abstract bool? TestULongCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.GT
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("testULongCompareNonNullable")]
    public abstract bool? TestULongCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 04
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.RET
    /// </remarks>
    [DisplayName("testULongDefault")]
    public abstract BigInteger? TestULongDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("testULongDefaultNonNullable")]
    public abstract BigInteger? TestULongDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KomUnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIKSgL//wAAMggC//8AAJFAEEA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIFNOT 52
    /// 0010 : OpCode.LDARG0
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.ISNULL
    /// 0013 : OpCode.JMPIFNOT 03
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.LDARG1
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.ISNULL
    /// 0019 : OpCode.JMPIFNOT 03
    /// 001B : OpCode.THROW
    /// 001C : OpCode.ADD
    /// 001D : OpCode.DUP
    /// 001E : OpCode.PUSHINT32 00000080
    /// 0023 : OpCode.JMPGE 04
    /// 0025 : OpCode.JMP 0A
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.PUSHINT32 FFFFFF7F
    /// 002D : OpCode.JMPLE 1E
    /// 002F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0038 : OpCode.AND
    /// 0039 : OpCode.DUP
    /// 003A : OpCode.PUSHINT32 FFFFFF7F
    /// 003F : OpCode.JMPLE 0C
    /// 0041 : OpCode.PUSHINT64 0000000001000000
    /// 004A : OpCode.SUB
    /// 004B : OpCode.DUP
    /// 004C : OpCode.PUSH0
    /// 004D : OpCode.JMPGE 04
    /// 004F : OpCode.JMP 0A
    /// 0051 : OpCode.DUP
    /// 0052 : OpCode.PUSHINT32 FFFF0000
    /// 0057 : OpCode.JMPLE 08
    /// 0059 : OpCode.PUSHINT32 FFFF0000
    /// 005E : OpCode.AND
    /// 005F : OpCode.RET
    /// 0060 : OpCode.PUSH0
    /// 0061 : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortAdd")]
    public abstract BigInteger? TestUShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCkoC//8AADIIAv//AACRQA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.ADD
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSHINT32 00000080
    /// 000C : OpCode.JMPGE 04
    /// 000E : OpCode.JMP 0A
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHINT32 FFFFFF7F
    /// 0016 : OpCode.JMPLE 1E
    /// 0018 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0021 : OpCode.AND
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSHINT32 FFFFFF7F
    /// 0028 : OpCode.JMPLE 0C
    /// 002A : OpCode.PUSHINT64 0000000001000000
    /// 0033 : OpCode.SUB
    /// 0034 : OpCode.DUP
    /// 0035 : OpCode.PUSH0
    /// 0036 : OpCode.JMPGE 04
    /// 0038 : OpCode.JMP 0A
    /// 003A : OpCode.DUP
    /// 003B : OpCode.PUSHINT32 FFFF0000
    /// 0040 : OpCode.JMPLE 08
    /// 0042 : OpCode.PUSHINT32 FFFF0000
    /// 0047 : OpCode.AND
    /// 0048 : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortAddNonNullable")]
    public abstract BigInteger? TestUShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNiqJAUJIgV52KokBAlAeErYJgM6eUrYJgM6t0A=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.ISNULL
    /// 0005 : OpCode.NOT
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.JMP 05
    /// 000B : OpCode.LDARG1
    /// 000C : OpCode.ISNULL
    /// 000D : OpCode.NOT
    /// 000E : OpCode.JMPIF 04
    /// 0010 : OpCode.PUSHF
    /// 0011 : OpCode.RET
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.ISNULL
    /// 0015 : OpCode.JMPIFNOT 03
    /// 0017 : OpCode.THROW
    /// 0018 : OpCode.LDARG1
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.ISNULL
    /// 001B : OpCode.JMPIFNOT 03
    /// 001D : OpCode.THROW
    /// 001E : OpCode.GT
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortCompare")]
    public abstract bool? TestUShortCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.GT
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortCompareNonNullable")]
    public abstract bool? TestUShortCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 04
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortDefault")]
    public abstract BigInteger? TestUShortDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("testUShortDefaultNonNullable")]
    public abstract BigInteger? TestUShortDefaultNonNullable(BigInteger? a);

    #endregion

}
