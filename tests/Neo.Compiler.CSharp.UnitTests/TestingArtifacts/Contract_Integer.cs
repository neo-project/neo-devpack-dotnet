using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Integer(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Integer"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""divRemByte"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""divRemShort"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":27,""safe"":false},{""name"":""divRemInt"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":58,""safe"":false},{""name"":""divRemLong"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":95,""safe"":false},{""name"":""divRemSbyte"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":144,""safe"":false},{""name"":""divRemUshort"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":172,""safe"":false},{""name"":""divRemUint"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":201,""safe"":false},{""name"":""divRemUlong"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":234,""safe"":false},{""name"":""clampByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":275,""safe"":false},{""name"":""clampSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":303,""safe"":false},{""name"":""clampShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":331,""safe"":false},{""name"":""clampUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":359,""safe"":false},{""name"":""clampInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":387,""safe"":false},{""name"":""clampUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":415,""safe"":false},{""name"":""clampLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":443,""safe"":false},{""name"":""clampULong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":471,""safe"":false},{""name"":""clampBigInteger"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":499,""safe"":false},{""name"":""copySignInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":527,""safe"":false},{""name"":""copySignSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":573,""safe"":false},{""name"":""copySignShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":610,""safe"":false},{""name"":""copySignLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":650,""safe"":false},{""name"":""createCheckedInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":708,""safe"":false},{""name"":""createCheckedByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":732,""safe"":false},{""name"":""createCheckedLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":746,""safe"":false},{""name"":""createCheckedUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":782,""safe"":false},{""name"":""createCheckedChar"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":810,""safe"":false},{""name"":""createCheckedShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":826,""safe"":false},{""name"":""createCheckedSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":844,""safe"":false},{""name"":""createSaturatingInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":859,""safe"":false},{""name"":""createSaturatingByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":905,""safe"":false},{""name"":""createSaturatingLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":945,""safe"":false},{""name"":""createSaturatingUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":999,""safe"":false},{""name"":""createSaturatingChar"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1053,""safe"":false},{""name"":""createSaturatingSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1095,""safe"":false},{""name"":""isEvenIntegerInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1135,""safe"":false},{""name"":""isEventUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1144,""safe"":false},{""name"":""isEvenLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1153,""safe"":false},{""name"":""isEvenUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1162,""safe"":false},{""name"":""isEvenShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1171,""safe"":false},{""name"":""isEvenUshort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1180,""safe"":false},{""name"":""isEvenByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1189,""safe"":false},{""name"":""isEvenSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1198,""safe"":false},{""name"":""isOddIntegerInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1207,""safe"":false},{""name"":""isOddUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1216,""safe"":false},{""name"":""isOddLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1225,""safe"":false},{""name"":""isOddUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1234,""safe"":false},{""name"":""isOddShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1243,""safe"":false},{""name"":""isOddUshort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1252,""safe"":false},{""name"":""isOddByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1261,""safe"":false},{""name"":""isOddSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1270,""safe"":false},{""name"":""isNegativeInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1279,""safe"":false},{""name"":""isNegativeLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1287,""safe"":false},{""name"":""isNegativeShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1295,""safe"":false},{""name"":""isNegativeSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1303,""safe"":false},{""name"":""isPositiveInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1311,""safe"":false},{""name"":""isPositiveLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1319,""safe"":false},{""name"":""isPositiveShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1327,""safe"":false},{""name"":""isPositiveSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1335,""safe"":false},{""name"":""isPow2Int"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1343,""safe"":false},{""name"":""isPow2UInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1366,""safe"":false},{""name"":""isPow2Long"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1389,""safe"":false},{""name"":""isPow2Ulong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1412,""safe"":false},{""name"":""isPow2Short"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1435,""safe"":false},{""name"":""isPow2Ushort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1458,""safe"":false},{""name"":""isPow2Byte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1481,""safe"":false},{""name"":""isPow2Sbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1504,""safe"":false},{""name"":""leadingZeroCountInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1527,""safe"":false},{""name"":""leadingZeroCountUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1557,""safe"":false},{""name"":""leadingZeroCountLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1579,""safe"":false},{""name"":""leadingZeroCountShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1609,""safe"":false},{""name"":""leadingZeroCountUshort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1638,""safe"":false},{""name"":""leadingZeroCountByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1659,""safe"":false},{""name"":""leadingZeroCountSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1680,""safe"":false},{""name"":""log2Int"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1709,""safe"":false},{""name"":""log2UInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1743,""safe"":false},{""name"":""log2Long"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1777,""safe"":false},{""name"":""log2Short"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1811,""safe"":false},{""name"":""log2Ushort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1845,""safe"":false},{""name"":""log2Byte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1879,""safe"":false},{""name"":""log2Sbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1913,""safe"":false},{""name"":""rotateLeftInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1947,""safe"":false},{""name"":""rotateLeftUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2022,""safe"":false},{""name"":""rotateLeftLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2085,""safe"":false},{""name"":""rotateLeftULong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2200,""safe"":false},{""name"":""rotateLeftShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2295,""safe"":false},{""name"":""rotateLeftUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2347,""safe"":false},{""name"":""rotateLeftByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2391,""safe"":false},{""name"":""rotateLeftSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2427,""safe"":false},{""name"":""rotateRightInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2469,""safe"":false},{""name"":""rotateRightUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2558,""safe"":false},{""name"":""rotateRightLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2588,""safe"":false},{""name"":""rotateRightULong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2717,""safe"":false},{""name"":""rotateRightShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2755,""safe"":false},{""name"":""rotateRightUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2817,""safe"":false},{""name"":""rotateRightByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2840,""safe"":false},{""name"":""rotateRightSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2861,""safe"":false},{""name"":""popCountByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2913,""safe"":false},{""name"":""popCountSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2939,""safe"":false},{""name"":""popCountShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2965,""safe"":false},{""name"":""popCountUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2993,""safe"":false},{""name"":""popCountInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3021,""safe"":false},{""name"":""popCountUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3053,""safe"":false},{""name"":""popCountLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3085,""safe"":false},{""name"":""popCountULong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3125,""safe"":false},{""name"":""popCountBigInteger"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3165,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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ECgIEalQnCL3RSIIRRAiBEU6QFcAAXhKECgVShAwFQ9QShAoCBGpUJwi90UiCEUQIgRFOkBXAAF4ShAoFUoQMBUPUEoQKAgRqVCcIvdFIghFECIERTpAVwABeEoQKBVKEDAVD1BKECgIEalQnCL3RSIIRRAiBEU6QFcAAXhKECgVShAwFQ9QShAoCBGpUJwi90UiCEUQIgRFOkBXAAF4ShAoFUoQMBUPUEoQKAgRqVCcIvdFIghFECIERTpAVwABeEoQKBVKEDAVD1BKECgIEalQnCL3RSIIRRAiBEU6QFcAAnh5AB+RUAP/////AAAAAJFQqAP/////AAAAAJF4A/////8AAAAAkXkAIFCfAB+RqZJKAwAAAIAAAAAAMAwDAAAAAAEAAACfQFcAAnh5AB+RUAP/////AAAAAJFQqAP/////AAAAAJF4A/////8AAAAAkXkAIFCfAB+RqZID/////wAAAACRQFcAAnh5AD+RUAT//////////wAAAAAAAAAAkVCoBP//////////AAAAAAAAAACReAT//////////wAAAAAAAAAAkXkAQFCfAD+RqZJKBAAAAAAAAACAAAAAAAAAAAAwFAQAAAAAAAAAAAEAAAAAAAAAn0BXAAJ4eQA/kVAE//////////8AAAAAAAAAAJFQqAT//////////wAAAAAAAAAAkXgE//////////8AAAAAAAAAAJF5AEBQnwA/kamSBP//////////AAAAAAAAAACRQFcAAnh5H5FQAv//AACRUKgC//8AAJF4Av//AACReSBQnx+RqZJKAgCAAAAwCAIAAAEAn0BXAAJ4eR+RUAL//wAAkVCoAv//AACReAL//wAAkXkgUJ8fkamSAv//AACRQFcAAnh5F5FQAf8AkVCoAf8AkXgB/wCReRhQnxeRqZIB/wCRQFcAAnh5F5FQAf8AkVCoAf8AkXgB/wCReRhQnxeRqZJKAYAAMAYBAAGfQFcAAnh5AB+RACCiACBQn1AD/////wAAAACRUKgD/////wAAAACReAP/////AAAAAJF5ACCiACBQnwAgUJ8AH5GpkkoDAAAAgAAAAAAwDAMAAAAAAQAAAJ9AVwACeHkAH5GpeAAgeZ8AH5GokgP/////AAAAAJFAVwACeHkAP5EAQKIAQFCfUAT//////////wAAAAAAAAAAkVCoBP//////////AAAAAAAAAACReAT//////////wAAAAAAAAAAkXkAQKIAQFCfAEBQnwA/kamSSgQAAAAAAAAAgAAAAAAAAAAAMBQEAAAAAAAAAAABAAAAAAAAAJ9AVwACeHkAP5GpeABAeZ8AP5GokgT//////////wAAAAAAAAAAkUBXAAJ4eR+RIKIgUJ9QAv//AACRUKgC//8AAJF4Av//AACReSCiIFCfIFCfH5GpkkoCAIAAADAIAgAAAQCfQFcAAnh5H5GpeCB5nx+RqJIC//8AAJFAVwACeHkXkal4GHmfF5GokgH/AJFAVwACeHkXkRiiGFCfUAH/AJFQqAH/AJF4Af8AkXkYohhQnxhQnxeRqZJKAYAAMAYBAAGfQFcAAXgB/wCREFBKECgMShGRUZ5QEaki9EVAVwABeAH/AJEQUEoQKAxKEZFRnlARqSL0RUBXAAF4Av//AACREFBKECgMShGRUZ5QEaki9EVAVwABeAL//wAAkRBQShAoDEoRkVGeUBGpIvRFQFcAAXgD/////wAAAACREFBKECgMShGRUZ5QEaki9EVAVwABeAP/////AAAAAJEQUEoQKAxKEZFRnlARqSL0RUBXAAF4BP//////////AAAAAAAAAACREFBKECgMShGRUZ5QEaki9EVAVwABeAT//////////wAAAAAAAAAAkRBQShAoDEoRkVGeUBGpIvRFQFcAAXhKAgAAAIADAAAAgAAAAAC7Jg4D/////wAAAACRIkcMQlZhbHVlIG91dCBvZiByYW5nZSwgbXVzdCBiZSBiZXR3ZWVuIGludC5NaW5WYWx1ZSBhbmQgaW50Lk1heFZhbHVlLjoQUEoQKAxKEZFRnlARqSL0RUDFGBa/"));

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
