using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_NullableType(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NullableType"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testBigIntegerAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testBigIntegerAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":31,""safe"":false},{""name"":""testBigIntegerCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":38,""safe"":false},{""name"":""testBigIntegerCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":69,""safe"":false},{""name"":""testBigIntegerDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":76,""safe"":false},{""name"":""testBigIntegerDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":87,""safe"":false},{""name"":""testIntAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":92,""safe"":false},{""name"":""testIntAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":156,""safe"":false},{""name"":""testIntCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":196,""safe"":false},{""name"":""testIntCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":227,""safe"":false},{""name"":""testIntDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":234,""safe"":false},{""name"":""testIntDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":245,""safe"":false},{""name"":""testUIntAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":250,""safe"":false},{""name"":""testUIntAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":309,""safe"":false},{""name"":""testUIntCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":344,""safe"":false},{""name"":""testUIntCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":375,""safe"":false},{""name"":""testUIntDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":382,""safe"":false},{""name"":""testUIntDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":393,""safe"":false},{""name"":""testLongAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":398,""safe"":false},{""name"":""testLongAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":482,""safe"":false},{""name"":""testLongCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":542,""safe"":false},{""name"":""testLongCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":573,""safe"":false},{""name"":""testLongDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":580,""safe"":false},{""name"":""testLongDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":591,""safe"":false},{""name"":""testULongAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":596,""safe"":false},{""name"":""testULongAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":671,""safe"":false},{""name"":""testULongCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":722,""safe"":false},{""name"":""testULongCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":753,""safe"":false},{""name"":""testULongDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":760,""safe"":false},{""name"":""testULongDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":771,""safe"":false},{""name"":""testShortAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":776,""safe"":false},{""name"":""testShortAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":863,""safe"":false},{""name"":""testShortCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":926,""safe"":false},{""name"":""testShortCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":957,""safe"":false},{""name"":""testShortDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":964,""safe"":false},{""name"":""testShortDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":975,""safe"":false},{""name"":""testUShortAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":980,""safe"":false},{""name"":""testUShortAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1064,""safe"":false},{""name"":""testUShortCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1124,""safe"":false},{""name"":""testUShortCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1155,""safe"":false},{""name"":""testUShortDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1162,""safe"":false},{""name"":""testUShortDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1173,""safe"":false},{""name"":""testSByteAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1178,""safe"":false},{""name"":""testSByteAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1260,""safe"":false},{""name"":""testSByteCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1318,""safe"":false},{""name"":""testSByteCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1349,""safe"":false},{""name"":""testSByteDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1356,""safe"":false},{""name"":""testSByteDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1367,""safe"":false},{""name"":""testByteAdd"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1372,""safe"":false},{""name"":""testByteAddNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1452,""safe"":false},{""name"":""testByteCompare"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1508,""safe"":false},{""name"":""testByteCompareNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1539,""safe"":false},{""name"":""testByteDefault"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1546,""safe"":false},{""name"":""testByteDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1557,""safe"":false},{""name"":""testBoolAnd"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1562,""safe"":false},{""name"":""testBoolAndNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1596,""safe"":false},{""name"":""testBoolOr"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1606,""safe"":false},{""name"":""testBoolOrNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""},{""name"":""b"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1640,""safe"":false},{""name"":""testBoolDefault"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1650,""safe"":false},{""name"":""testBoolDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1661,""safe"":false},{""name"":""testUInt160Default"",""parameters"":[{""name"":""a"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":1666,""safe"":false},{""name"":""testUInt160DefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Hash160""}],""returntype"":""Hash160"",""offset"":1698,""safe"":false},{""name"":""testUInt256Default"",""parameters"":[{""name"":""a"",""type"":""Hash256""}],""returntype"":""Hash256"",""offset"":1703,""safe"":false},{""name"":""testUInt256DefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""Hash256""}],""returntype"":""Hash256"",""offset"":1747,""safe"":false},{""name"":""testUInt160ArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1752,""safe"":false},{""name"":""testUInt160ArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1768,""safe"":false},{""name"":""testUInt256ArrayLength"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1774,""safe"":false},{""name"":""testUInt256ArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1790,""safe"":false},{""name"":""testByteArrayLength"",""parameters"":[{""name"":""a"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":1796,""safe"":false},{""name"":""testByteArrayLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":1812,""safe"":false},{""name"":""testStringLength"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Integer"",""offset"":1818,""safe"":false},{""name"":""testStringLengthNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Integer"",""offset"":1834,""safe"":false},{""name"":""testStringDefault"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""String"",""offset"":1840,""safe"":false},{""name"":""testStringDefaultNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""String"",""offset"":1852,""safe"":false},{""name"":""testStringConcat"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""}],""returntype"":""String"",""offset"":1857,""safe"":false},{""name"":""testStringConcatNonNullable"",""parameters"":[{""name"":""a"",""type"":""String""},{""name"":""b"",""type"":""String""}],""returntype"":""String"",""offset"":1880,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.0"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1hB1cAAnjYJgUJIgV52KomEHhK2CYDOnlK2CYDOp5AEEBXAAJ4eZ5AVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiYxeErYJgM6eUrYJgM6nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0AQQFcAAnh5nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAJ42CYFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42CYFCSIFediqJix4StgmAzp5StgmAzqeShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUAQQFcAAnh5nkoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFAVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiZFeErYJgM6eUrYJgM6nkrKGDIyBP//////////AAAAAAAAAACRSgP/////////fzIUBAAAAAAAAAAAAQAAAAAAAACfQBBAVwACeHmeSsoYMjIE//////////8AAAAAAAAAAJFKA/////////9/MhQEAAAAAAAAAAABAAAAAAAAAJ9AVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiY8eErYJgM6eUrYJgM6nkoQLgQiFkoE//////////8AAAAAAAAAADIUBP//////////AAAAAAAAAACRQBBAVwACeHmeShAuBCIWSgT//////////wAAAAAAAAAAMhQE//////////8AAAAAAAAAAJFAVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiZIeErYJgM6eUrYJgM6nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0rKEjIUAv//AACRSgH/fzIIAgAAAQCfQBBAVwACeHmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSsoSMhQC//8AAJFKAf9/MggCAAABAJ9AVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiZFeErYJgM6eUrYJgM6nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCkoC//8AADIIAv//AACRQBBAVwACeHmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIKSgL//wAAMggC//8AAJFAVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiZDeErYJgM6eUrYJgM6nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0rKETIPAf8AkUoAfzIGAQABn0AQQFcAAnh5nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0rKETIPAf8AkUoAfzIGAQABn0BXAAJ42CYFCSIFediqJAQJQHhK2CYDOnlK2CYDOrdAVwACeHm3QFcAAXhK2CYERRBAVwABeEBXAAJ42CYFCSIFediqJkF4StgmAzp5StgmAzqeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIISgH/ADIGAf8AkUAQQFcAAnh5nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCEoB/wAyBgH/AJFAVwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QFcAAnh5t0BXAAF4StgmBEUQQFcAAXhAVwACeNgmBQkiBXnYqiQECUB4StgmAzokBAlAeUrYJgM6QFcAAngkBAlAeUBXAAJ42CYFCSIFediqJAQJQHhK2CYDOiYECEB5StgmAzpAVwACeCYECEB5QFcAAXhK2CYERQlAVwABeEBXAAF4StgmGUUMFAAAAAAAAAAAAAAAAAAAAAAAAAAAQFcAAXhAVwABeErYJiVFDCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEBXAAF4QFcAAXhK2CQDykrYJgRFEEBXAAF4ykBXAAF4StgkA8pK2CYERRBAVwABeMpAVwABeErYJAPKStgmBEUQQFcAAXjKQFcAAXhK2CQDykrYJgRFEEBXAAF4ykBXAAF4StgmBUUMAEBXAAF4QFcAAnhK2CYFRQwAeUrYJgVFDACL2yhAVwACeHmL2yhAlfu86g==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYQeErYJgM6eUrYJgM6nkAQQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIFNOT 10 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ADD [8 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerAdd")]
    public abstract BigInteger? TestBigIntegerAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerAddNonNullable")]
    public abstract BigInteger? TestBigIntegerAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerCompare")]
    public abstract bool? TestBigIntegerCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerCompareNonNullable")]
    public abstract bool? TestBigIntegerCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerDefault")]
    public abstract BigInteger? TestBigIntegerDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerDefaultNonNullable")]
    public abstract BigInteger? TestBigIntegerDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzokBAlAeUrYJgM6QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolAnd")]
    public abstract bool? TestBoolAnd(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCQECUB5QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolAndNonNullable")]
    public abstract bool? TestBoolAndNonNullable(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFCUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolDefault")]
    public abstract bool? TestBoolDefault(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolDefaultNonNullable")]
    public abstract bool? TestBoolDefaultNonNullable(bool? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzomBAhAeUrYJgM6QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolOr")]
    public abstract bool? TestBoolOr(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeCYECEB5QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBoolOrNonNullable")]
    public abstract bool? TestBoolOrNonNullable(bool? a, bool? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiZBeErYJgM6eUrYJgM6nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCEoB/wAyBgH/AJFAEEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIFNOT 41 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// JMPLE 06 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteAdd")]
    public abstract BigInteger? TestByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIISgH/ADIGAf8AkUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// JMPLE 06 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteAddNonNullable")]
    public abstract BigInteger? TestByteAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// SIZE [4 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayLength")]
    public abstract BigInteger? TestByteArrayLength(byte[]? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayLengthNonNullable")]
    public abstract BigInteger? TestByteArrayLengthNonNullable(byte[]? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteCompare")]
    public abstract bool? TestByteCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteCompareNonNullable")]
    public abstract bool? TestByteCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteDefault")]
    public abstract BigInteger? TestByteDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteDefaultNonNullable")]
    public abstract BigInteger? TestByteDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYxeErYJgM6eUrYJgM6nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0AQQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIFNOT 31 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntAdd")]
    public abstract BigInteger? TestIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntAddNonNullable")]
    public abstract BigInteger? TestIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntCompare")]
    public abstract bool? TestIntCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntCompareNonNullable")]
    public abstract bool? TestIntCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntDefault")]
    public abstract BigInteger? TestIntDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntDefaultNonNullable")]
    public abstract BigInteger? TestIntDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiZFeErYJgM6eUrYJgM6nkrKGDIyBP//////////AAAAAAAAAACRSgP/////////fzIUBAAAAAAAAAAAAQAAAAAAAACfQBBA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIFNOT 45 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH8 [1 datoshi]
    /// JMPLE 32 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// JMPLE 14 [2 datoshi]
    /// PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongAdd")]
    public abstract BigInteger? TestLongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSsoYMjIE//////////8AAAAAAAAAAJFKA/////////9/MhQEAAAAAAAAAAABAAAAAAAAAJ9A
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH8 [1 datoshi]
    /// JMPLE 32 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// JMPLE 14 [2 datoshi]
    /// PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongAddNonNullable")]
    public abstract BigInteger? TestLongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongCompare")]
    public abstract bool? TestLongCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongCompareNonNullable")]
    public abstract bool? TestLongCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongDefault")]
    public abstract BigInteger? TestLongDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLongDefaultNonNullable")]
    public abstract BigInteger? TestLongDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiZDeErYJgM6eUrYJgM6nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0rKETIPAf8AkUoAfzIGAQABn0AQQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIFNOT 43 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPLE 0F [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// JMPLE 06 [2 datoshi]
    /// PUSHINT16 0001 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteAdd")]
    public abstract BigInteger? TestSByteAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSsoRMg8B/wCRSgB/MgYBAAGfQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPLE 0F [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// JMPLE 06 [2 datoshi]
    /// PUSHINT16 0001 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteAddNonNullable")]
    public abstract BigInteger? TestSByteAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteCompare")]
    public abstract bool? TestSByteCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteCompareNonNullable")]
    public abstract bool? TestSByteCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteDefault")]
    public abstract BigInteger? TestSByteDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSByteDefaultNonNullable")]
    public abstract BigInteger? TestSByteDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiZIeErYJgM6eUrYJgM6nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0rKEjIUAv//AACRSgH/fzIIAgAAAQCfQBBA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIFNOT 48 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH2 [1 datoshi]
    /// JMPLE 14 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT16 FF7F [1 datoshi]
    /// JMPLE 08 [2 datoshi]
    /// PUSHINT32 00000100 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortAdd")]
    public abstract BigInteger? TestShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSsoSMhQC//8AAJFKAf9/MggCAAABAJ9A
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH2 [1 datoshi]
    /// JMPLE 14 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT16 FF7F [1 datoshi]
    /// JMPLE 08 [2 datoshi]
    /// PUSHINT32 00000100 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortAddNonNullable")]
    public abstract BigInteger? TestShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortCompare")]
    public abstract bool? TestShortCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortCompareNonNullable")]
    public abstract bool? TestShortCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortDefault")]
    public abstract BigInteger? TestShortDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testShortDefaultNonNullable")]
    public abstract BigInteger? TestShortDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeErYJgVFDAB5StgmBUUMAIvbKEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringConcat")]
    public abstract string? TestStringConcat(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2yhA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringConcatNonNullable")]
    public abstract string? TestStringConcatNonNullable(string? a, string? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgVFDABA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringDefault")]
    public abstract string? TestStringDefault(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringDefaultNonNullable")]
    public abstract string? TestStringDefaultNonNullable(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// SIZE [4 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringLength")]
    public abstract BigInteger? TestStringLength(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStringLengthNonNullable")]
    public abstract BigInteger? TestStringLengthNonNullable(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// SIZE [4 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160ArrayLength")]
    public abstract BigInteger? TestUInt160ArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160ArrayLengthNonNullable")]
    public abstract BigInteger? TestUInt160ArrayLengthNonNullable(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJhlFDBQAAAAAAAAAAAAAAAAAAAAAAAAAAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 19 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160Default")]
    public abstract UInt160? TestUInt160Default(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt160DefaultNonNullable")]
    public abstract UInt160? TestUInt160DefaultNonNullable(UInt160? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKStgmBEUQQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// SIZE [4 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256ArrayLength")]
    public abstract BigInteger? TestUInt256ArrayLength(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256ArrayLengthNonNullable")]
    public abstract BigInteger? TestUInt256ArrayLengthNonNullable(IList<object>? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJiVFDCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 25 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256Default")]
    public abstract UInt256? TestUInt256Default(UInt256? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUInt256DefaultNonNullable")]
    public abstract UInt256? TestUInt256DefaultNonNullable(UInt256? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiYseErYJgM6eUrYJgM6nkoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFAEEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIFNOT 2C [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0E [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntAdd")]
    public abstract BigInteger? TestUIntAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0E [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntAddNonNullable")]
    public abstract BigInteger? TestUIntAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntCompare")]
    public abstract bool? TestUIntCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntCompareNonNullable")]
    public abstract bool? TestUIntCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntDefault")]
    public abstract BigInteger? TestUIntDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUIntDefaultNonNullable")]
    public abstract BigInteger? TestUIntDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiY8eErYJgM6eUrYJgM6nkoQLgQiFkoE//////////8AAAAAAAAAADIUBP//////////AAAAAAAAAACRQBBA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIFNOT 3C [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 16 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// JMPLE 14 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongAdd")]
    public abstract BigInteger? TestULongAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeShAuBCIWSgT//////////wAAAAAAAAAAMhQE//////////8AAAAAAAAAAJFA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 16 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// JMPLE 14 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongAddNonNullable")]
    public abstract BigInteger? TestULongAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongCompare")]
    public abstract bool? TestULongCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongCompareNonNullable")]
    public abstract bool? TestULongCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongDefault")]
    public abstract BigInteger? TestULongDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testULongDefaultNonNullable")]
    public abstract BigInteger? TestULongDefaultNonNullable(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiZFeErYJgM6eUrYJgM6nkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCkoC//8AADIIAv//AACRQBBA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIFNOT 45 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 08 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortAdd")]
    public abstract BigInteger? TestUShortAdd(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIKSgL//wAAMggC//8AAJFA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 08 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortAddNonNullable")]
    public abstract BigInteger? TestUShortAddNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeNgmBQkiBXnYqiQECUB4StgmAzp5StgmAzq3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ISNULL [2 datoshi]
    /// NOT [4 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortCompare")]
    public abstract bool? TestUShortCompare(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHm3QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// GT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortCompareNonNullable")]
    public abstract bool? TestUShortCompareNonNullable(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgRFEEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortDefault")]
    public abstract BigInteger? TestUShortDefault(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testUShortDefaultNonNullable")]
    public abstract BigInteger? TestUShortDefaultNonNullable(BigInteger? a);

    #endregion
}
