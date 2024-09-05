using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Integer(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Integer"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""divRemByte"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""divRemShort"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":27,""safe"":false},{""name"":""divRemInt"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":58,""safe"":false},{""name"":""divRemLong"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":95,""safe"":false},{""name"":""divRemSbyte"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":144,""safe"":false},{""name"":""divRemUshort"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":172,""safe"":false},{""name"":""divRemUint"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":201,""safe"":false},{""name"":""divRemUlong"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":234,""safe"":false},{""name"":""clampByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":275,""safe"":false},{""name"":""clampSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":311,""safe"":false},{""name"":""clampShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":347,""safe"":false},{""name"":""clampUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":383,""safe"":false},{""name"":""clampInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":419,""safe"":false},{""name"":""clampUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":455,""safe"":false},{""name"":""clampLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":491,""safe"":false},{""name"":""clampULong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":527,""safe"":false},{""name"":""clampBigInteger"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":563,""safe"":false},{""name"":""copySignInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":599,""safe"":false},{""name"":""copySignSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":645,""safe"":false},{""name"":""copySignShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":682,""safe"":false},{""name"":""copySignLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":722,""safe"":false},{""name"":""createCheckedInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":780,""safe"":false},{""name"":""createCheckedByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":804,""safe"":false},{""name"":""createCheckedLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":818,""safe"":false},{""name"":""createCheckedUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":854,""safe"":false},{""name"":""createCheckedChar"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":882,""safe"":false},{""name"":""createCheckedShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":898,""safe"":false},{""name"":""createCheckedSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":916,""safe"":false},{""name"":""createSaturatingInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":931,""safe"":false},{""name"":""createSaturatingByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":975,""safe"":false},{""name"":""createSaturatingLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1013,""safe"":false},{""name"":""createSaturatingUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1065,""safe"":false},{""name"":""createSaturatingChar"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1117,""safe"":false},{""name"":""createSaturatingSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1157,""safe"":false},{""name"":""isEvenIntegerInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1195,""safe"":false},{""name"":""isEventUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1204,""safe"":false},{""name"":""isEvenLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1213,""safe"":false},{""name"":""isEvenUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1222,""safe"":false},{""name"":""isEvenShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1231,""safe"":false},{""name"":""isEvenUshort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1240,""safe"":false},{""name"":""isEvenByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1249,""safe"":false},{""name"":""isEvenSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1258,""safe"":false},{""name"":""isOddIntegerInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1267,""safe"":false},{""name"":""isOddUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1276,""safe"":false},{""name"":""isOddLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1285,""safe"":false},{""name"":""isOddUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1294,""safe"":false},{""name"":""isOddShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1303,""safe"":false},{""name"":""isOddUshort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1312,""safe"":false},{""name"":""isOddByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1321,""safe"":false},{""name"":""isOddSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1330,""safe"":false},{""name"":""isNegativeInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1339,""safe"":false},{""name"":""isNegativeLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1347,""safe"":false},{""name"":""isNegativeShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1355,""safe"":false},{""name"":""isNegativeSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1363,""safe"":false},{""name"":""isPositiveInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1371,""safe"":false},{""name"":""isPositiveLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1379,""safe"":false},{""name"":""isPositiveShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1387,""safe"":false},{""name"":""isPositiveSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1395,""safe"":false},{""name"":""isPow2Int"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1403,""safe"":false},{""name"":""isPow2UInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1425,""safe"":false},{""name"":""isPow2Long"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1447,""safe"":false},{""name"":""isPow2Ulong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1469,""safe"":false},{""name"":""isPow2Short"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1491,""safe"":false},{""name"":""isPow2Ushort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1513,""safe"":false},{""name"":""isPow2Byte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1535,""safe"":false},{""name"":""isPow2Sbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1557,""safe"":false},{""name"":""leadingZeroCountInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1579,""safe"":false},{""name"":""leadingZeroCountUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1608,""safe"":false},{""name"":""leadingZeroCountLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1630,""safe"":false},{""name"":""leadingZeroCountShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1659,""safe"":false},{""name"":""leadingZeroCountUshort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1687,""safe"":false},{""name"":""leadingZeroCountByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1708,""safe"":false},{""name"":""leadingZeroCountSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1729,""safe"":false},{""name"":""log2Int"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1757,""safe"":false},{""name"":""log2UInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1789,""safe"":false},{""name"":""log2Long"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1821,""safe"":false},{""name"":""log2Short"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1853,""safe"":false},{""name"":""log2Ushort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1885,""safe"":false},{""name"":""log2Byte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1917,""safe"":false},{""name"":""log2Sbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1949,""safe"":false},{""name"":""rotateLeftInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1981,""safe"":false},{""name"":""rotateLeftUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2056,""safe"":false},{""name"":""rotateLeftLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2119,""safe"":false},{""name"":""rotateLeftULong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2234,""safe"":false},{""name"":""rotateLeftShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2329,""safe"":false},{""name"":""rotateLeftUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2381,""safe"":false},{""name"":""rotateLeftByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2425,""safe"":false},{""name"":""rotateLeftSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2461,""safe"":false},{""name"":""rotateRightInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2503,""safe"":false},{""name"":""rotateRightUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2592,""safe"":false},{""name"":""rotateRightLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2622,""safe"":false},{""name"":""rotateRightULong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2751,""safe"":false},{""name"":""rotateRightShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2789,""safe"":false},{""name"":""rotateRightUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2851,""safe"":false},{""name"":""rotateRightByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2874,""safe"":false},{""name"":""rotateRightSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2895,""safe"":false},{""name"":""popCountByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2947,""safe"":false},{""name"":""popCountSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2973,""safe"":false},{""name"":""popCountShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2999,""safe"":false},{""name"":""popCountUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3027,""safe"":false},{""name"":""popCountInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3055,""safe"":false},{""name"":""popCountUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3087,""safe"":false},{""name"":""popCountLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3119,""safe"":false},{""name"":""popCountULong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3159,""safe"":false},{""name"":""popCountBigInteger"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3199,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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RRhQn0BXAAF4ShAuBUUQQBBQShAoCBGpUJwi90UYUJ9AVwABeEoQKBRKEDATD1BKECgIEalQnCL3RUBFEEBFOkBXAAF4ShAoFEoQMBMPUEoQKAgRqVCcIvdFQEUQQEU6QFcAAXhKECgUShAwEw9QShAoCBGpUJwi90VARRBARTpAVwABeEoQKBRKEDATD1BKECgIEalQnCL3RUBFEEBFOkBXAAF4ShAoFEoQMBMPUEoQKAgRqVCcIvdFQEUQQEU6QFcAAXhKECgUShAwEw9QShAoCBGpUJwi90VARRBARTpAVwABeEoQKBRKEDATD1BKECgIEalQnCL3RUBFEEBFOkBXAAJ4eQAfkVAD/////wAAAACRUKgD/////wAAAACReAP/////AAAAAJF5ACBQnwAfkamSSgMAAACAAAAAADAMAwAAAAABAAAAn0BXAAJ4eQAfkVAD/////wAAAACRUKgD/////wAAAACReAP/////AAAAAJF5ACBQnwAfkamSA/////8AAAAAkUBXAAJ4eQA/kVAE//////////8AAAAAAAAAAJFQqAT//////////wAAAAAAAAAAkXgE//////////8AAAAAAAAAAJF5AEBQnwA/kamSSgQAAAAAAAAAgAAAAAAAAAAAMBQEAAAAAAAAAAABAAAAAAAAAJ9AVwACeHkAP5FQBP//////////AAAAAAAAAACRUKgE//////////8AAAAAAAAAAJF4BP//////////AAAAAAAAAACReQBAUJ8AP5GpkgT//////////wAAAAAAAAAAkUBXAAJ4eR+RUAL//wAAkVCoAv//AACReAL//wAAkXkgUJ8fkamSSgIAgAAAMAgCAAABAJ9AVwACeHkfkVAC//8AAJFQqAL//wAAkXgC//8AAJF5IFCfH5GpkgL//wAAkUBXAAJ4eReRUAH/AJFQqAH/AJF4Af8AkXkYUJ8XkamSAf8AkUBXAAJ4eReRUAH/AJFQqAH/AJF4Af8AkXkYUJ8XkamSSgGAADAGAQABn0BXAAJ4eQAfkQAgogAgUJ9QA/////8AAAAAkVCoA/////8AAAAAkXgD/////wAAAACReQAgogAgUJ8AIFCfAB+RqZJKAwAAAIAAAAAAMAwDAAAAAAEAAACfQFcAAnh5AB+RqXgAIHmfAB+RqJID/////wAAAACRQFcAAnh5AD+RAECiAEBQn1AE//////////8AAAAAAAAAAJFQqAT//////////wAAAAAAAAAAkXgE//////////8AAAAAAAAAAJF5AECiAEBQnwBAUJ8AP5GpkkoEAAAAAAAAAIAAAAAAAAAAADAUBAAAAAAAAAAAAQAAAAAAAACfQFcAAnh5AD+RqXgAQHmfAD+RqJIE//////////8AAAAAAAAAAJFAVwACeHkfkSCiIFCfUAL//wAAkVCoAv//AACReAL//wAAkXkgoiBQnyBQnx+RqZJKAgCAAAAwCAIAAAEAn0BXAAJ4eR+RqXggeZ8fkaiSAv//AACRQFcAAnh5F5GpeBh5nxeRqJIB/wCRQFcAAnh5F5EYohhQn1AB/wCRUKgB/wCReAH/AJF5GKIYUJ8YUJ8XkamSSgGAADAGAQABn0BXAAF4Af8AkRBQShAoDEoRkVGeUBGpIvRFQFcAAXgB/wCREFBKECgMShGRUZ5QEaki9EVAVwABeAL//wAAkRBQShAoDEoRkVGeUBGpIvRFQFcAAXgC//8AAJEQUEoQKAxKEZFRnlARqSL0RUBXAAF4A/////8AAAAAkRBQShAoDEoRkVGeUBGpIvRFQFcAAXgD/////wAAAACREFBKECgMShGRUZ5QEaki9EVAVwABeAT//////////wAAAAAAAAAAkRBQShAoDEoRkVGeUBGpIvRFQFcAAXgE//////////8AAAAAAAAAAJEQUEoQKAxKEZFRnlARqSL0RUBXAAF4SgIAAACAAwAAAIAAAAAAuyYOA/////8AAAAAkSJHDEJWYWx1ZSBvdXQgb2YgcmFuZ2UsIG11c3QgYmUgYmV0d2VlbiBpbnQuTWluVmFsdWUgYW5kIGludC5NYXhWYWx1ZS46EFBKECgMShGRUZ5QEaki9EVAVGDWdw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampBigInteger")]
    public abstract BigInteger? ClampBigInteger(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampByte")]
    public abstract BigInteger? ClampByte(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampInt")]
    public abstract BigInteger? ClampInt(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampLong")]
    public abstract BigInteger? ClampLong(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampSByte")]
    public abstract BigInteger? ClampSByte(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampShort")]
    public abstract BigInteger? ClampShort(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampUInt")]
    public abstract BigInteger? ClampUInt(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampULong")]
    public abstract BigInteger? ClampULong(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampUShort")]
    public abstract BigInteger? ClampUShort(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("copySignInt")]
    public abstract BigInteger? CopySignInt(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("copySignLong")]
    public abstract BigInteger? CopySignLong(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("copySignSbyte")]
    public abstract BigInteger? CopySignSbyte(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("copySignShort")]
    public abstract BigInteger? CopySignShort(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedByte")]
    public abstract BigInteger? CreateCheckedByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedChar")]
    public abstract BigInteger? CreateCheckedChar(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedInt")]
    public abstract BigInteger? CreateCheckedInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedLong")]
    public abstract BigInteger? CreateCheckedLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedSbyte")]
    public abstract BigInteger? CreateCheckedSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedShort")]
    public abstract BigInteger? CreateCheckedShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedUlong")]
    public abstract BigInteger? CreateCheckedUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createSaturatingByte")]
    public abstract BigInteger? CreateSaturatingByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createSaturatingChar")]
    public abstract BigInteger? CreateSaturatingChar(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createSaturatingInt")]
    public abstract BigInteger? CreateSaturatingInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createSaturatingLong")]
    public abstract BigInteger? CreateSaturatingLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createSaturatingSbyte")]
    public abstract BigInteger? CreateSaturatingSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createSaturatingUlong")]
    public abstract BigInteger? CreateSaturatingUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemByte")]
    public abstract IList<object>? DivRemByte(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemInt")]
    public abstract IList<object>? DivRemInt(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemLong")]
    public abstract IList<object>? DivRemLong(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemSbyte")]
    public abstract IList<object>? DivRemSbyte(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemShort")]
    public abstract IList<object>? DivRemShort(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemUint")]
    public abstract IList<object>? DivRemUint(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemUlong")]
    public abstract IList<object>? DivRemUlong(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemUshort")]
    public abstract IList<object>? DivRemUshort(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenByte")]
    public abstract bool? IsEvenByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenIntegerInt")]
    public abstract bool? IsEvenIntegerInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenLong")]
    public abstract bool? IsEvenLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenSbyte")]
    public abstract bool? IsEvenSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenShort")]
    public abstract bool? IsEvenShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEventUInt")]
    public abstract bool? IsEventUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenUlong")]
    public abstract bool? IsEvenUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenUshort")]
    public abstract bool? IsEvenUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isNegativeInt")]
    public abstract bool? IsNegativeInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isNegativeLong")]
    public abstract bool? IsNegativeLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isNegativeSbyte")]
    public abstract bool? IsNegativeSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isNegativeShort")]
    public abstract bool? IsNegativeShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddByte")]
    public abstract bool? IsOddByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddIntegerInt")]
    public abstract bool? IsOddIntegerInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddLong")]
    public abstract bool? IsOddLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddSbyte")]
    public abstract bool? IsOddSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddShort")]
    public abstract bool? IsOddShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddUInt")]
    public abstract bool? IsOddUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddUlong")]
    public abstract bool? IsOddUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddUshort")]
    public abstract bool? IsOddUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPositiveInt")]
    public abstract bool? IsPositiveInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPositiveLong")]
    public abstract bool? IsPositiveLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPositiveSbyte")]
    public abstract bool? IsPositiveSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPositiveShort")]
    public abstract bool? IsPositiveShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Byte")]
    public abstract bool? IsPow2Byte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Int")]
    public abstract bool? IsPow2Int(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Long")]
    public abstract bool? IsPow2Long(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Sbyte")]
    public abstract bool? IsPow2Sbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Short")]
    public abstract bool? IsPow2Short(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2UInt")]
    public abstract bool? IsPow2UInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Ulong")]
    public abstract bool? IsPow2Ulong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Ushort")]
    public abstract bool? IsPow2Ushort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountByte")]
    public abstract BigInteger? LeadingZeroCountByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountInt")]
    public abstract BigInteger? LeadingZeroCountInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountLong")]
    public abstract BigInteger? LeadingZeroCountLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountSbyte")]
    public abstract BigInteger? LeadingZeroCountSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountShort")]
    public abstract BigInteger? LeadingZeroCountShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountUInt")]
    public abstract BigInteger? LeadingZeroCountUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountUshort")]
    public abstract BigInteger? LeadingZeroCountUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2Byte")]
    public abstract BigInteger? Log2Byte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2Int")]
    public abstract BigInteger? Log2Int(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2Long")]
    public abstract BigInteger? Log2Long(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2Sbyte")]
    public abstract BigInteger? Log2Sbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2Short")]
    public abstract BigInteger? Log2Short(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2UInt")]
    public abstract BigInteger? Log2UInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2Ushort")]
    public abstract BigInteger? Log2Ushort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountBigInteger")]
    public abstract BigInteger? PopCountBigInteger(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountByte")]
    public abstract BigInteger? PopCountByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountInt")]
    public abstract BigInteger? PopCountInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountLong")]
    public abstract BigInteger? PopCountLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountSByte")]
    public abstract BigInteger? PopCountSByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountShort")]
    public abstract BigInteger? PopCountShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountUInt")]
    public abstract BigInteger? PopCountUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountULong")]
    public abstract BigInteger? PopCountULong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountUShort")]
    public abstract BigInteger? PopCountUShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftByte")]
    public abstract BigInteger? RotateLeftByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftInt")]
    public abstract BigInteger? RotateLeftInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftLong")]
    public abstract BigInteger? RotateLeftLong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftSByte")]
    public abstract BigInteger? RotateLeftSByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftShort")]
    public abstract BigInteger? RotateLeftShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftUInt")]
    public abstract BigInteger? RotateLeftUInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftULong")]
    public abstract BigInteger? RotateLeftULong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftUShort")]
    public abstract BigInteger? RotateLeftUShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightByte")]
    public abstract BigInteger? RotateRightByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightInt")]
    public abstract BigInteger? RotateRightInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightLong")]
    public abstract BigInteger? RotateRightLong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightSByte")]
    public abstract BigInteger? RotateRightSByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightShort")]
    public abstract BigInteger? RotateRightShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightUInt")]
    public abstract BigInteger? RotateRightUInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightULong")]
    public abstract BigInteger? RotateRightULong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightUShort")]
    public abstract BigInteger? RotateRightUShort(BigInteger? value, BigInteger? offset);

    #endregion

}
