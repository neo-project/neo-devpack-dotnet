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
    /// Script: VwACeNiqJAUJIgV52KomTnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIISgH/ADIGAf8AkUAQQA==
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
    /// 0E : OpCode.JMPIFNOT 4E
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
    /// 1D : OpCode.DUP
    /// 1E : OpCode.PUSHINT32 00000080
    /// 23 : OpCode.JMPGE 04
    /// 25 : OpCode.JMP 0A
    /// 27 : OpCode.DUP
    /// 28 : OpCode.PUSHINT32 FFFFFF7F
    /// 2D : OpCode.JMPLE 1E
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 38 : OpCode.AND
    /// 39 : OpCode.DUP
    /// 3A : OpCode.PUSHINT32 FFFFFF7F
    /// 3F : OpCode.JMPLE 0C
    /// 41 : OpCode.PUSHINT64 0000000001000000
    /// 4A : OpCode.SUB
    /// 4B : OpCode.DUP
    /// 4C : OpCode.PUSH0
    /// 4D : OpCode.JMPGE 04
    /// 4F : OpCode.JMP 08
    /// 51 : OpCode.DUP
    /// 52 : OpCode.PUSHINT16 FF00
    /// 55 : OpCode.JMPLE 06
    /// 57 : OpCode.PUSHINT16 FF00
    /// 5A : OpCode.AND
    /// 5B : OpCode.RET
    /// 5C : OpCode.PUSH0
    /// 5D : OpCode.RET
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
    /// Script: VwACeNiqJAUJIgV52KomPnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQBBA
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
    /// 0E : OpCode.JMPIFNOT 3E
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
    /// 1D : OpCode.DUP
    /// 1E : OpCode.PUSHINT32 00000080
    /// 23 : OpCode.JMPGE 04
    /// 25 : OpCode.JMP 0A
    /// 27 : OpCode.DUP
    /// 28 : OpCode.PUSHINT32 FFFFFF7F
    /// 2D : OpCode.JMPLE 1E
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 38 : OpCode.AND
    /// 39 : OpCode.DUP
    /// 3A : OpCode.PUSHINT32 FFFFFF7F
    /// 3F : OpCode.JMPLE 0C
    /// 41 : OpCode.PUSHINT64 0000000001000000
    /// 4A : OpCode.SUB
    /// 4B : OpCode.RET
    /// 4C : OpCode.PUSH0
    /// 4D : OpCode.RET
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
    /// Script: VwACeNiqJAUJIgV52KomWnhK2CYDOnlK2CYDOp5KAwAAAAAAAACALgQiDkoD/////////38yMgT//////////wAAAAAAAAAAkUoD/////////38yFAQAAAAAAAAAAAEAAAAAAAAAn0AQQA==
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
    /// 0E : OpCode.JMPIFNOT 5A
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
    /// 1D : OpCode.DUP
    /// 1E : OpCode.PUSHINT64 0000000000000080
    /// 27 : OpCode.JMPGE 04
    /// 29 : OpCode.JMP 0E
    /// 2B : OpCode.DUP
    /// 2C : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 35 : OpCode.JMPLE 32
    /// 37 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 48 : OpCode.AND
    /// 49 : OpCode.DUP
    /// 4A : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 53 : OpCode.JMPLE 14
    /// 55 : OpCode.PUSHINT128 00000000000000000100000000000000
    /// 66 : OpCode.SUB
    /// 67 : OpCode.RET
    /// 68 : OpCode.PUSH0
    /// 69 : OpCode.RET
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
    /// Script: VwACeNiqJAUJIgV52KomV3hK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSgCALgQiB0oAfzIPAf8AkUoAfzIGAQABn0AQQA==
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
    /// 0E : OpCode.JMPIFNOT 57
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
    /// 1D : OpCode.DUP
    /// 1E : OpCode.PUSHINT32 00000080
    /// 23 : OpCode.JMPGE 04
    /// 25 : OpCode.JMP 0A
    /// 27 : OpCode.DUP
    /// 28 : OpCode.PUSHINT32 FFFFFF7F
    /// 2D : OpCode.JMPLE 1E
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 38 : OpCode.AND
    /// 39 : OpCode.DUP
    /// 3A : OpCode.PUSHINT32 FFFFFF7F
    /// 3F : OpCode.JMPLE 0C
    /// 41 : OpCode.PUSHINT64 0000000001000000
    /// 4A : OpCode.SUB
    /// 4B : OpCode.DUP
    /// 4C : OpCode.PUSHINT8 80
    /// 4E : OpCode.JMPGE 04
    /// 50 : OpCode.JMP 07
    /// 52 : OpCode.DUP
    /// 53 : OpCode.PUSHINT8 7F
    /// 55 : OpCode.JMPLE 0F
    /// 57 : OpCode.PUSHINT16 FF00
    /// 5A : OpCode.AND
    /// 5B : OpCode.DUP
    /// 5C : OpCode.PUSHINT8 7F
    /// 5E : OpCode.JMPLE 06
    /// 60 : OpCode.PUSHINT16 0001
    /// 63 : OpCode.SUB
    /// 64 : OpCode.RET
    /// 65 : OpCode.PUSH0
    /// 66 : OpCode.RET
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
    /// Script: VwACeNiqJAUJIgV52KomXnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSgEAgC4EIghKAf9/MhQC//8AAJFKAf9/MggCAAABAJ9AEEA=
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
    /// 0E : OpCode.JMPIFNOT 5E
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
    /// 1D : OpCode.DUP
    /// 1E : OpCode.PUSHINT32 00000080
    /// 23 : OpCode.JMPGE 04
    /// 25 : OpCode.JMP 0A
    /// 27 : OpCode.DUP
    /// 28 : OpCode.PUSHINT32 FFFFFF7F
    /// 2D : OpCode.JMPLE 1E
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 38 : OpCode.AND
    /// 39 : OpCode.DUP
    /// 3A : OpCode.PUSHINT32 FFFFFF7F
    /// 3F : OpCode.JMPLE 0C
    /// 41 : OpCode.PUSHINT64 0000000001000000
    /// 4A : OpCode.SUB
    /// 4B : OpCode.DUP
    /// 4C : OpCode.PUSHINT16 0080
    /// 4F : OpCode.JMPGE 04
    /// 51 : OpCode.JMP 08
    /// 53 : OpCode.DUP
    /// 54 : OpCode.PUSHINT16 FF7F
    /// 57 : OpCode.JMPLE 14
    /// 59 : OpCode.PUSHINT32 FFFF0000
    /// 5E : OpCode.AND
    /// 5F : OpCode.DUP
    /// 60 : OpCode.PUSHINT16 FF7F
    /// 63 : OpCode.JMPLE 08
    /// 65 : OpCode.PUSHINT32 00000100
    /// 6A : OpCode.SUB
    /// 6B : OpCode.RET
    /// 6C : OpCode.PUSH0
    /// 6D : OpCode.RET
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
    /// Script: VwACeNiqJAUJIgV52KomLHhK2CYDOnlK2CYDOp5KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRQBBA
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
    /// 0E : OpCode.JMPIFNOT 2C
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
    /// 1D : OpCode.DUP
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.JMPGE 04
    /// 21 : OpCode.JMP 0E
    /// 23 : OpCode.DUP
    /// 24 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2D : OpCode.JMPLE 0C
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 38 : OpCode.AND
    /// 39 : OpCode.RET
    /// 3A : OpCode.PUSH0
    /// 3B : OpCode.RET
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
    /// Script: VwACeNiqJAUJIgV52KomPHhK2CYDOnlK2CYDOp5KEC4EIhZKBP//////////AAAAAAAAAAAyFAT//////////wAAAAAAAAAAkUAQQA==
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
    /// 0E : OpCode.JMPIFNOT 3C
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
    /// 1D : OpCode.DUP
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.JMPGE 04
    /// 21 : OpCode.JMP 16
    /// 23 : OpCode.DUP
    /// 24 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 35 : OpCode.JMPLE 14
    /// 37 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 48 : OpCode.AND
    /// 49 : OpCode.RET
    /// 4A : OpCode.PUSH0
    /// 4B : OpCode.RET
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
    /// Script: VwACeNiqJAUJIgV52KomUnhK2CYDOnlK2CYDOp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIKSgL//wAAMggC//8AAJFAEEA=
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
    /// 0E : OpCode.JMPIFNOT 52
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
    /// 1D : OpCode.DUP
    /// 1E : OpCode.PUSHINT32 00000080
    /// 23 : OpCode.JMPGE 04
    /// 25 : OpCode.JMP 0A
    /// 27 : OpCode.DUP
    /// 28 : OpCode.PUSHINT32 FFFFFF7F
    /// 2D : OpCode.JMPLE 1E
    /// 2F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 38 : OpCode.AND
    /// 39 : OpCode.DUP
    /// 3A : OpCode.PUSHINT32 FFFFFF7F
    /// 3F : OpCode.JMPLE 0C
    /// 41 : OpCode.PUSHINT64 0000000001000000
    /// 4A : OpCode.SUB
    /// 4B : OpCode.DUP
    /// 4C : OpCode.PUSH0
    /// 4D : OpCode.JMPGE 04
    /// 4F : OpCode.JMP 0A
    /// 51 : OpCode.DUP
    /// 52 : OpCode.PUSHINT32 FFFF0000
    /// 57 : OpCode.JMPLE 08
    /// 59 : OpCode.PUSHINT32 FFFF0000
    /// 5E : OpCode.AND
    /// 5F : OpCode.RET
    /// 60 : OpCode.PUSH0
    /// 61 : OpCode.RET
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
