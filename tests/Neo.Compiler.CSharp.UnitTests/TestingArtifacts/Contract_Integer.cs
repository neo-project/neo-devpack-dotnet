using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Integer(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Integer"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""divRemByte"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""divRemShort"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":46,""safe"":false},{""name"":""divRemDeconstruction"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":92,""safe"":false},{""name"":""divRemInt"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":255,""safe"":false},{""name"":""divRemLong"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":331,""safe"":false},{""name"":""divRemSbyte"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":411,""safe"":false},{""name"":""divRemUshort"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":457,""safe"":false},{""name"":""divRemUint"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":503,""safe"":false},{""name"":""divRemUlong"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":549,""safe"":false},{""name"":""clampByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":595,""safe"":false},{""name"":""clampSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":624,""safe"":false},{""name"":""clampShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":653,""safe"":false},{""name"":""clampUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":682,""safe"":false},{""name"":""clampInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":711,""safe"":false},{""name"":""clampUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":740,""safe"":false},{""name"":""clampLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":769,""safe"":false},{""name"":""clampULong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":798,""safe"":false},{""name"":""clampBigInteger"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":827,""safe"":false},{""name"":""copySignInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":856,""safe"":false},{""name"":""copySignSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":879,""safe"":false},{""name"":""copySignShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":899,""safe"":false},{""name"":""copySignLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":920,""safe"":false},{""name"":""createCheckedInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":947,""safe"":false},{""name"":""createCheckedByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":958,""safe"":false},{""name"":""createCheckedLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":972,""safe"":false},{""name"":""createCheckedUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":983,""safe"":false},{""name"":""createCheckedChar"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1011,""safe"":false},{""name"":""createCheckedShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1027,""safe"":false},{""name"":""createCheckedSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1038,""safe"":false},{""name"":""createSaturatingInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1049,""safe"":false},{""name"":""createSaturatingByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1067,""safe"":false},{""name"":""createSaturatingLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1079,""safe"":false},{""name"":""createSaturatingUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1105,""safe"":false},{""name"":""createSaturatingChar"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1131,""safe"":false},{""name"":""createSaturatingSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1145,""safe"":false},{""name"":""isEvenIntegerInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1157,""safe"":false},{""name"":""isEventUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1165,""safe"":false},{""name"":""isEvenLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1173,""safe"":false},{""name"":""isEvenUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1181,""safe"":false},{""name"":""isEvenShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1189,""safe"":false},{""name"":""isEvenUshort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1197,""safe"":false},{""name"":""isEvenByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1205,""safe"":false},{""name"":""isEvenSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1213,""safe"":false},{""name"":""isOddIntegerInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1221,""safe"":false},{""name"":""isOddUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1229,""safe"":false},{""name"":""isOddLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1237,""safe"":false},{""name"":""isOddUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1245,""safe"":false},{""name"":""isOddShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1253,""safe"":false},{""name"":""isOddUshort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1261,""safe"":false},{""name"":""isOddByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1269,""safe"":false},{""name"":""isOddSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1277,""safe"":false},{""name"":""isNegativeInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1285,""safe"":false},{""name"":""isNegativeLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1292,""safe"":false},{""name"":""isNegativeShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1299,""safe"":false},{""name"":""isNegativeSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1306,""safe"":false},{""name"":""isPositiveInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1313,""safe"":false},{""name"":""isPositiveLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1320,""safe"":false},{""name"":""isPositiveShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1327,""safe"":false},{""name"":""isPositiveSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1334,""safe"":false},{""name"":""isPow2Int"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1341,""safe"":false},{""name"":""isPow2UInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1357,""safe"":false},{""name"":""isPow2Long"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1373,""safe"":false},{""name"":""isPow2Ulong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1389,""safe"":false},{""name"":""isPow2Short"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1405,""safe"":false},{""name"":""isPow2Ushort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1421,""safe"":false},{""name"":""isPow2Byte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1437,""safe"":false},{""name"":""isPow2Sbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1453,""safe"":false},{""name"":""leadingZeroCountInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1469,""safe"":false},{""name"":""leadingZeroCountUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1497,""safe"":false},{""name"":""leadingZeroCountLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1518,""safe"":false},{""name"":""leadingZeroCountShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1546,""safe"":false},{""name"":""leadingZeroCountUshort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1573,""safe"":false},{""name"":""leadingZeroCountByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1593,""safe"":false},{""name"":""leadingZeroCountSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1613,""safe"":false},{""name"":""leadingZeroCountBigInteger"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1640,""safe"":false},{""name"":""log2Int"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1681,""safe"":false},{""name"":""log2UInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1704,""safe"":false},{""name"":""log2Long"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1727,""safe"":false},{""name"":""log2Short"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1750,""safe"":false},{""name"":""log2Ushort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1773,""safe"":false},{""name"":""log2Byte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1796,""safe"":false},{""name"":""log2Sbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1819,""safe"":false},{""name"":""rotateLeftInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1842,""safe"":false},{""name"":""rotateLeftIntFromExpressions"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1926,""safe"":false},{""name"":""rotateLeftUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2082,""safe"":false},{""name"":""rotateLeftUIntFromExpressions"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2144,""safe"":false},{""name"":""rotateLeftLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2255,""safe"":false},{""name"":""rotateLeftULong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2387,""safe"":false},{""name"":""rotateLeftShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2481,""safe"":false},{""name"":""rotateLeftUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2539,""safe"":false},{""name"":""rotateLeftByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2583,""safe"":false},{""name"":""rotateLeftSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2619,""safe"":false},{""name"":""rotateRightInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2665,""safe"":false},{""name"":""rotateRightIntFromExpressions"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2739,""safe"":false},{""name"":""rotateRightUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2885,""safe"":false},{""name"":""rotateRightUIntFromExpressions"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2937,""safe"":false},{""name"":""rotateRightLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3038,""safe"":false},{""name"":""rotateRightULong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3152,""safe"":false},{""name"":""rotateRightShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3228,""safe"":false},{""name"":""rotateRightUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3280,""safe"":false},{""name"":""rotateRightByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3318,""safe"":false},{""name"":""rotateRightSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3350,""safe"":false},{""name"":""popCountByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3392,""safe"":false},{""name"":""popCountSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3414,""safe"":false},{""name"":""popCountShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3436,""safe"":false},{""name"":""popCountUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3460,""safe"":false},{""name"":""popCountInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3484,""safe"":false},{""name"":""popCountUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3512,""safe"":false},{""name"":""popCountLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3540,""safe"":false},{""name"":""popCountULong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3576,""safe"":false},{""name"":""popCountBigInteger"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":3612,""safe"":false},{""name"":""isPow2BigInteger"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":3670,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1mDlcBAkNweXgRTRAqGkNoMgVFIvsMDkRpdmlkZSBieSB6ZXJvOkoSTaFTolASwEBXAQJDcHl4EU0QKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzpKEk2hU6JQEsBAVwICQ3B5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86SgIAAACAKhhLDyoUQ2gyBUUi+wwIT3ZlcmZsb3c6ShJNoVOiUBLAEE0RzhFNEM4Sv0bBRXBxaABkoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2meSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcBAkNweXgRTRAqGkNoMgVFIvsMDkRpdmlkZSBieSB6ZXJvOkoCAAAAgCoYSw8qFENoMgVFIvsMCE92ZXJmbG93OkoSTaFTolASwEBXAQJDcHl4EU0QKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzpKAwAAAAAAAACAKhhLDyoUQ2gyBUUi+wwIT3ZlcmZsb3c6ShJNoVOiUBLAQFcBAkNweXgRTRAqGkNoMgVFIvsMDkRpdmlkZSBieSB6ZXJvOkoSTaFTolASwEBXAQJDcHl4EU0QKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzpKEk2hU6JQEsBAVwECQ3B5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86ShJNoVOiUBLAQFcBAkNweXgRTRAqGkNoMgVFIvsMDkRpdmlkZSBieSB6ZXJvOkoSTaFTolASwEBXAAN4eXpLSzIRRUVFDAltaW4gPiBtYXg6U7q5QFcAA3h5ektLMhFFRUUMCW1pbiA+IG1heDpTurlAVwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUBXAAN4eXpLSzIRRUVFDAltaW4gPiBtYXg6U7q5QFcAA3h5ektLMhFFRUUMCW1pbiA+IG1heDpTurlAVwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUBXAAN4eXpLSzIRRUVFDAltaW4gPiBtYXg6U7q5QFcAA3h5ektLMhFFRUUMCW1pbiA+IG1heDpTurlAVwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUBXAAJ4eRAwBZoiBJqbSgL///9/MgM6QFcAAnh5EDAFmiIEmptKAH8yAzpAVwACeHkQMAWaIgSam0oB/38yAzpAVwACeHkQMAWaIgSam0oD/////////38yAzpAVwABeErKFDIDOkBXAAF4ShABAAG7JAM6QFcAAXhKyhgyAzpAVwABeEoQBAAAAAAAAAAAAQAAAAAAAAC7JAM6QFcAAXhKEAIAAAEAuyQDOkBXAAF4SsoSMgM6QFcAAXhKyhEyAzpAVwABeAIAAACAAv///39TurlAVwABeBAB/wBTurlAVwABeAMAAAAAAAAAgAP/////////f1O6uUBXAAF4EAT//////////wAAAAAAAAAAU7q5QFcAAXgQAv//AABTurlAVwABeACAAH9TurlAVwABeBKiqkBXAAF4EqKqQFcAAXgSoqpAVwABeBKiqkBXAAF4EqKqQFcAAXgSoqpAVwABeBKiqkBXAAF4EqKqQFcAAXgSorFAVwABeBKisUBXAAF4EqKxQFcAAXgSorFAVwABeBKisUBXAAF4EqKxQFcAAXgSorFAVwABeBKisUBXAAF4ELVAVwABeBC1QFcAAXgQtUBXAAF4ELVAVwABeBC4QFcAAXgQuEBXAAF4ELhAVwABeBC4QFcAAXhKECwFRQlASp2RqkBXAAF4ShAsBUUJQEqdkapAVwABeEoQLAVFCUBKnZGqQFcAAXhKECwFRQlASp2RqkBXAAF4ShAsBUUJQEqdkapAVwABeEoQLAVFCUBKnZGqQFcAAXhKECwFRQlASp2RqkBXAAF4ShAsBUUJQEqdkapAVwABeEoQLgVFEEAQUEomCBGpUJwi+EUAIFCfQFcAAXgQUEomCBGpUJwi+EUAIFCfQFcAAXhKEC4FRRBAEFBKJggRqVCcIvhFAEBQn0BXAAF4ShAuBUUQQBBQSiYIEalQnCL4RSBQn0BXAAF4EFBKJggRqVCcIvhFIFCfQFcAAXgQUEomCBGpUJwi+EUYUJ9AVwABeEoQLgVFEEAQUEomCBGpUJwi+EUYUJ9AVwABeEoQLgVFEEAQUEomCBGpUJwi+EVKJAZFACBASgAfnhWpFahQn0BXAAF4ShAuAzoQSxEyCZxLS6kRLPtGQFcAAXhKEC4DOhBLETIJnEtLqREs+0ZAVwABeEoQLgM6EEsRMgmcS0upESz7RkBXAAF4ShAuAzoQSxEyCZxLS6kRLPtGQFcAAXhKEC4DOhBLETIJnEtLqREs+0ZAVwABeEoQLgM6EEsRMgmcS0upESz7RkBXAAF4ShAuAzoQSxEyCZxLS6kRLPtGQFcCAnh5cHFpA/////8AAAAAkWgAH5GoA/////8AAAAAkWkD/////wAAAACRaJsAH5GpkgP/////AAAAAJFKAwAAAIAAAAAAMAwDAAAAAAEAAACfQFcEAnicSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcHmcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWhpcnNrA/////8AAAAAkWoAH5GoA/////8AAAAAkWsD/////wAAAACRapsAH5GpkgP/////AAAAAJFKAwAAAIAAAAAAMAwDAAAAAAEAAACfQFcCAnh5cHFpA/////8AAAAAkWgAH5GoA/////8AAAAAkWkD/////wAAAACRaJsAH5GpkgP/////AAAAAJFAVwQCeJwD/////wAAAACRcHmcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWhpcnNrA/////8AAAAAkWoAH5GoA/////8AAAAAkWsD/////wAAAACRapsAH5GpkgP/////AAAAAJFAVwICeHlwcWkE//////////8AAAAAAAAAAJFoAD+RqAT//////////wAAAAAAAAAAkWkE//////////8AAAAAAAAAAJFomwA/kamSBP//////////AAAAAAAAAACRSgQAAAAAAAAAgAAAAAAAAAAAMBQEAAAAAAAAAAABAAAAAAAAAJ9AVwICeHlwcWkE//////////8AAAAAAAAAAJFoAD+RqAT//////////wAAAAAAAAAAkWkE//////////8AAAAAAAAAAJFomwA/kamSBP//////////AAAAAAAAAACRQFcCAnh5cHFpAv//AACRaB+RqAL//wAAkWkC//8AAJFomx+RqZIC//8AAJFKAgCAAAAwCAIAAAEAn0BXAgJ4eXBxaQL//wAAkWgfkagC//8AAJFpAv//AACRaJsfkamSAv//AACRQFcCAnh5cHFpAf8AkWgXkagB/wCRaQH/AJFomxeRqZIB/wCRQFcCAnh5cHFpAf8AkWgXkagB/wCRaQH/AJFomxeRqZIB/wCRSgGAADAGAQABn0BXAgJ4eXBxaQP/////AAAAAJFoAB+RqWkD/////wAAAACRaJsAH5GokgP/////AAAAAJFKAwAAAIAAAAAAMAwDAAAAAAEAAACfQFcEAnicSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcHmcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWhpcnNrA/////8AAAAAkWoAH5GpawP/////AAAAAJFqmwAfkaiSA/////8AAAAAkUoDAAAAgAAAAAAwDAMAAAAAAQAAAJ9AVwICeHlwcWkD/////wAAAACRaAAfkalpA/////8AAAAAkWibAB+RqJID/////wAAAACRQFcEAnicA/////8AAAAAkXB5nErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FoaXJzawP/////AAAAAJFqAB+RqWsD/////wAAAACRapsAH5GokgP/////AAAAAJFAVwICeHlwcWkE//////////8AAAAAAAAAAJFoAD+RqWkE//////////8AAAAAAAAAAJFomwA/kaiSBP//////////AAAAAAAAAACRSgQAAAAAAAAAgAAAAAAAAAAAMBQEAAAAAAAAAAABAAAAAAAAAJ9AVwICeHlwcWkE//////////8AAAAAAAAAAJFoAD+RqWkE//////////8AAAAAAAAAAJFomwA/kaiSBP//////////AAAAAAAAAACRQFcCAnh5cHFpAv//AACRaB+RqWkC//8AAJFomx+RqJIC//8AAJFKAgCAAAAwCAIAAAEAn0BXAgJ4eXBxaQL//wAAkWgfkalpAv//AACRaJsfkaiSAv//AACRQFcCAnh5cHFpAf8AkWgXkalpAf8AkWibF5GokgH/AJFAVwICeHlwcWkB/wCRaBeRqWkB/wCRaJsXkaiSAf8AkUoBgAAwBgEAAZ9AVwABeAH/AJEQSyYKS51RkVCcIvdGQFcAAXgB/wCREEsmCkudUZFQnCL3RkBXAAF4Av//AACREEsmCkudUZFQnCL3RkBXAAF4Av//AACREEsmCkudUZFQnCL3RkBXAAF4A/////8AAAAAkRBLJgpLnVGRUJwi90ZAVwABeAP/////AAAAAJEQSyYKS51RkVCcIvdGQFcAAXgE//////////8AAAAAAAAAAJEQSyYKS51RkVCcIvdGQFcAAXgE//////////8AAAAAAAAAAJEQSyYKS51RkVCcIvdGQFcAAXhKEC4mSsoULA4D/////wAAAACRIhUMEE91dCBvZiBpbnQgcmFuZ2U6EEsmCkudUZFQnCL3RkBXAAF4ShAsBUUJQEqdkapAwEEFDA==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampBigInteger")]
    public abstract BigInteger? ClampBigInteger(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampByte")]
    public abstract BigInteger? ClampByte(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampInt")]
    public abstract BigInteger? ClampInt(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampLong")]
    public abstract BigInteger? ClampLong(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampSByte")]
    public abstract BigInteger? ClampSByte(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampShort")]
    public abstract BigInteger? ClampShort(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampUInt")]
    public abstract BigInteger? ClampUInt(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampULong")]
    public abstract BigInteger? ClampULong(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syEUVFRQwJbWluID4gbWF4OlO6uUA=
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 11 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 6D696E203E206D6178 [8 datoshi]
    /// THROW [512 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampUShort")]
    public abstract BigInteger? ClampUShort(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oC////fzIDOkA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 05 [2 datoshi]
    /// ABS [4 datoshi]
    /// JMP 04 [2 datoshi]
    /// ABS [4 datoshi]
    /// NEGATE [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("copySignInt")]
    public abstract BigInteger? CopySignInt(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oD/////////38yAzpA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 05 [2 datoshi]
    /// ABS [4 datoshi]
    /// JMP 04 [2 datoshi]
    /// ABS [4 datoshi]
    /// NEGATE [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("copySignLong")]
    public abstract BigInteger? CopySignLong(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oAfzIDOkA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 05 [2 datoshi]
    /// ABS [4 datoshi]
    /// JMP 04 [2 datoshi]
    /// ABS [4 datoshi]
    /// NEGATE [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("copySignSbyte")]
    public abstract BigInteger? CopySignSbyte(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oB/38yAzpA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 05 [2 datoshi]
    /// ABS [4 datoshi]
    /// JMP 04 [2 datoshi]
    /// ABS [4 datoshi]
    /// NEGATE [4 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT16 FF7F [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("copySignShort")]
    public abstract BigInteger? CopySignShort(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQAQABuyQDOkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT16 0001 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedByte")]
    public abstract BigInteger? CreateCheckedByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQAgAAAQC7JAM6QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT32 00000100 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedChar")]
    public abstract BigInteger? CreateCheckedChar(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErKFDIDOkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedInt")]
    public abstract BigInteger? CreateCheckedInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErKGDIDOkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH8 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedLong")]
    public abstract BigInteger? CreateCheckedLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErKETIDOkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedSbyte")]
    public abstract BigInteger? CreateCheckedSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErKEjIDOkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH2 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedShort")]
    public abstract BigInteger? CreateCheckedShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQBAAAAAAAAAAAAQAAAAAAAAC7JAM6QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedUlong")]
    public abstract BigInteger? CreateCheckedUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAB/wBTurlA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingByte")]
    public abstract BigInteger? CreateSaturatingByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAC//8AAFO6uUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingChar")]
    public abstract BigInteger? CreateSaturatingChar(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAIAAACAAv///39TurlA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingInt")]
    public abstract BigInteger? CreateSaturatingInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAMAAAAAAAAAgAP/////////f1O6uUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingLong")]
    public abstract BigInteger? CreateSaturatingLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeACAAH9TurlA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 80 [1 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingSbyte")]
    public abstract BigInteger? CreateSaturatingSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAE//////////8AAAAAAAAAAFO6uUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MAX [8 datoshi]
    /// MIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingUlong")]
    public abstract BigInteger? CreateSaturatingUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86ShJNoVOiUBLAQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemByte")]
    public abstract IList<object>? DivRemByte(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICQ3B5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86SgIAAACAKhhLDyoUQ2gyBUUi+wwIT3ZlcmZsb3c6ShJNoVOiUBLAEE0RzhFNEM4Sv0bBRXBxaABkoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2meSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQA==
    /// INITSLOT 0202 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 18 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// NIP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// MUL [8 datoshi]
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
    /// LDLOC1 [2 datoshi]
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
    [DisplayName("divRemDeconstruction")]
    public abstract BigInteger? DivRemDeconstruction(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86SgIAAACAKhhLDyoUQ2gyBUUi+wwIT3ZlcmZsb3c6ShJNoVOiUBLAQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 18 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemInt")]
    public abstract IList<object>? DivRemInt(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86SgMAAAAAAAAAgCoYSw8qFENoMgVFIvsMCE92ZXJmbG93OkoSTaFTolASwEA=
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 18 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemLong")]
    public abstract IList<object>? DivRemLong(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86ShJNoVOiUBLAQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemSbyte")]
    public abstract IList<object>? DivRemSbyte(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86ShJNoVOiUBLAQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemShort")]
    public abstract IList<object>? DivRemShort(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86ShJNoVOiUBLAQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemUint")]
    public abstract IList<object>? DivRemUint(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86ShJNoVOiUBLAQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemUlong")]
    public abstract IList<object>? DivRemUlong(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86ShJNoVOiUBLAQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemUshort")]
    public abstract IList<object>? DivRemUshort(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenByte")]
    public abstract bool? IsEvenByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenIntegerInt")]
    public abstract bool? IsEvenIntegerInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenLong")]
    public abstract bool? IsEvenLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenSbyte")]
    public abstract bool? IsEvenSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenShort")]
    public abstract bool? IsEvenShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenUlong")]
    public abstract bool? IsEvenUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenUshort")]
    public abstract bool? IsEvenUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEventUInt")]
    public abstract bool? IsEventUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isNegativeInt")]
    public abstract bool? IsNegativeInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isNegativeLong")]
    public abstract bool? IsNegativeLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isNegativeSbyte")]
    public abstract bool? IsNegativeSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isNegativeShort")]
    public abstract bool? IsNegativeShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NZ [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddByte")]
    public abstract bool? IsOddByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NZ [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddIntegerInt")]
    public abstract bool? IsOddIntegerInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NZ [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddLong")]
    public abstract bool? IsOddLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NZ [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddSbyte")]
    public abstract bool? IsOddSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NZ [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddShort")]
    public abstract bool? IsOddShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NZ [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddUInt")]
    public abstract bool? IsOddUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NZ [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddUlong")]
    public abstract bool? IsOddUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NZ [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddUshort")]
    public abstract bool? IsOddUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// GE [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPositiveInt")]
    public abstract bool? IsPositiveInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// GE [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPositiveLong")]
    public abstract bool? IsPositiveLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// GE [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPositiveSbyte")]
    public abstract bool? IsPositiveSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// GE [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPositiveShort")]
    public abstract bool? IsPositiveShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLAVFCUBKnZGqQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2BigInteger")]
    public abstract bool? IsPow2BigInteger(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLAVFCUBKnZGqQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Byte")]
    public abstract bool? IsPow2Byte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLAVFCUBKnZGqQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Int")]
    public abstract bool? IsPow2Int(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLAVFCUBKnZGqQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Long")]
    public abstract bool? IsPow2Long(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLAVFCUBKnZGqQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Sbyte")]
    public abstract bool? IsPow2Sbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLAVFCUBKnZGqQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Short")]
    public abstract bool? IsPow2Short(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLAVFCUBKnZGqQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2UInt")]
    public abstract bool? IsPow2UInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLAVFCUBKnZGqQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Ulong")]
    public abstract bool? IsPow2Ulong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLAVFCUBKnZGqQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Ushort")]
    public abstract bool? IsPow2Ushort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEomCBGpUJwi+EVKJAZFACBASgAfnhWpFahQn0A=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F8 [2 datoshi]
    /// DROP [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// ADD [8 datoshi]
    /// PUSH5 [1 datoshi]
    /// SHR [8 datoshi]
    /// PUSH5 [1 datoshi]
    /// SHL [8 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountBigInteger")]
    public abstract BigInteger? LeadingZeroCountBigInteger(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBBQSiYIEalQnCL4RRhQn0A=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F8 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH8 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountByte")]
    public abstract BigInteger? LeadingZeroCountByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEomCBGpUJwi+EUAIFCfQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F8 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountInt")]
    public abstract BigInteger? LeadingZeroCountInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEomCBGpUJwi+EUAQFCfQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F8 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHINT8 40 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountLong")]
    public abstract BigInteger? LeadingZeroCountLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEomCBGpUJwi+EUYUJ9A
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F8 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH8 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountSbyte")]
    public abstract BigInteger? LeadingZeroCountSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEomCBGpUJwi+EUgUJ9A
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F8 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH16 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountShort")]
    public abstract BigInteger? LeadingZeroCountShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBBQSiYIEalQnCL4RQAgUJ9A
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F8 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountUInt")]
    public abstract BigInteger? LeadingZeroCountUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBBQSiYIEalQnCL4RSBQn0A=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F8 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH16 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountUshort")]
    public abstract BigInteger? LeadingZeroCountUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6EEsRMgmcS0upESz7RkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPLE 09 [2 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Byte")]
    public abstract BigInteger? Log2Byte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6EEsRMgmcS0upESz7RkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPLE 09 [2 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Int")]
    public abstract BigInteger? Log2Int(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6EEsRMgmcS0upESz7RkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPLE 09 [2 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Long")]
    public abstract BigInteger? Log2Long(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6EEsRMgmcS0upESz7RkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPLE 09 [2 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Sbyte")]
    public abstract BigInteger? Log2Sbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6EEsRMgmcS0upESz7RkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPLE 09 [2 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Short")]
    public abstract BigInteger? Log2Short(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6EEsRMgmcS0upESz7RkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPLE 09 [2 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2UInt")]
    public abstract BigInteger? Log2UInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6EEsRMgmcS0upESz7RkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPLE 09 [2 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Ushort")]
    public abstract BigInteger? Log2Ushort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLiZKyhQsDgP/////AAAAAJEiFQwQT3V0IG9mIGludCByYW5nZToQSyYKS51RkVCcIvdGQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 26 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPGT 0E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// JMP 15 [2 datoshi]
    /// PUSHDATA1 4F7574206F6620696E742072616E6765 [8 datoshi]
    /// THROW [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// JMPIFNOT 0A [2 datoshi]
    /// OVER [2 datoshi]
    /// DEC [4 datoshi]
    /// ROT [2 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountBigInteger")]
    public abstract BigInteger? PopCountBigInteger(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAH/AJEQSyYKS51RkVCcIvdGQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// JMPIFNOT 0A [2 datoshi]
    /// OVER [2 datoshi]
    /// DEC [4 datoshi]
    /// ROT [2 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountByte")]
    public abstract BigInteger? PopCountByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAP/////AAAAAJEQSyYKS51RkVCcIvdGQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// JMPIFNOT 0A [2 datoshi]
    /// OVER [2 datoshi]
    /// DEC [4 datoshi]
    /// ROT [2 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountInt")]
    public abstract BigInteger? PopCountInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAT//////////wAAAAAAAAAAkRBLJgpLnVGRUJwi90ZA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// JMPIFNOT 0A [2 datoshi]
    /// OVER [2 datoshi]
    /// DEC [4 datoshi]
    /// ROT [2 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountLong")]
    public abstract BigInteger? PopCountLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAH/AJEQSyYKS51RkVCcIvdGQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// JMPIFNOT 0A [2 datoshi]
    /// OVER [2 datoshi]
    /// DEC [4 datoshi]
    /// ROT [2 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountSByte")]
    public abstract BigInteger? PopCountSByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAL//wAAkRBLJgpLnVGRUJwi90ZA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// JMPIFNOT 0A [2 datoshi]
    /// OVER [2 datoshi]
    /// DEC [4 datoshi]
    /// ROT [2 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountShort")]
    public abstract BigInteger? PopCountShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAP/////AAAAAJEQSyYKS51RkVCcIvdGQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// JMPIFNOT 0A [2 datoshi]
    /// OVER [2 datoshi]
    /// DEC [4 datoshi]
    /// ROT [2 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountUInt")]
    public abstract BigInteger? PopCountUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAT//////////wAAAAAAAAAAkRBLJgpLnVGRUJwi90ZA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// JMPIFNOT 0A [2 datoshi]
    /// OVER [2 datoshi]
    /// DEC [4 datoshi]
    /// ROT [2 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountULong")]
    public abstract BigInteger? PopCountULong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAL//wAAkRBLJgpLnVGRUJwi90ZA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// JMPIFNOT 0A [2 datoshi]
    /// OVER [2 datoshi]
    /// DEC [4 datoshi]
    /// ROT [2 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountUShort")]
    public abstract BigInteger? PopCountUShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkB/wCRaBeRqAH/AJFpAf8AkWibF5GpkgH/AJFA
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftByte")]
    public abstract BigInteger? RotateLeftByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkD/////wAAAACRaAAfkagD/////wAAAACRaQP/////AAAAAJFomwAfkamSA/////8AAAAAkUoDAAAAgAAAAAAwDAMAAAAAAQAAAJ9A
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000008000000000 [1 datoshi]
    /// JMPLT 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftInt")]
    public abstract BigInteger? RotateLeftInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQCeJxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9weZxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaGlyc2sD/////wAAAACRagAfkagD/////wAAAACRawP/////AAAAAJFqmwAfkamSA/////8AAAAAkUoDAAAAgAAAAAAwDAMAAAAAAQAAAJ9A
    /// INITSLOT 0402 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// INC [4 datoshi]
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
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// INC [4 datoshi]
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
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000008000000000 [1 datoshi]
    /// JMPLT 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftIntFromExpressions")]
    public abstract BigInteger? RotateLeftIntFromExpressions(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkE//////////8AAAAAAAAAAJFoAD+RqAT//////////wAAAAAAAAAAkWkE//////////8AAAAAAAAAAJFomwA/kamSBP//////////AAAAAAAAAACRSgQAAAAAAAAAgAAAAAAAAAAAMBQEAAAAAAAAAAABAAAAAAAAAJ9A
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// JMPLT 14 [2 datoshi]
    /// PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftLong")]
    public abstract BigInteger? RotateLeftLong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkB/wCRaBeRqAH/AJFpAf8AkWibF5GpkgH/AJFKAYAAMAYBAAGfQA==
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT16 8000 [1 datoshi]
    /// JMPLT 06 [2 datoshi]
    /// PUSHINT16 0001 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftSByte")]
    public abstract BigInteger? RotateLeftSByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkC//8AAJFoH5GoAv//AACRaQL//wAAkWibH5GpkgL//wAAkUoCAIAAADAIAgAAAQCfQA==
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00800000 [1 datoshi]
    /// JMPLT 08 [2 datoshi]
    /// PUSHINT32 00000100 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftShort")]
    public abstract BigInteger? RotateLeftShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkD/////wAAAACRaAAfkagD/////wAAAACRaQP/////AAAAAJFomwAfkamSA/////8AAAAAkUA=
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftUInt")]
    public abstract BigInteger? RotateLeftUInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQCeJwD/////wAAAACRcHmcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWhpcnNrA/////8AAAAAkWoAH5GoA/////8AAAAAkWsD/////wAAAACRapsAH5GpkgP/////AAAAAJFA
    /// INITSLOT 0402 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// INC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// INC [4 datoshi]
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
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftUIntFromExpressions")]
    public abstract BigInteger? RotateLeftUIntFromExpressions(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkE//////////8AAAAAAAAAAJFoAD+RqAT//////////wAAAAAAAAAAkWkE//////////8AAAAAAAAAAJFomwA/kamSBP//////////AAAAAAAAAACRQA==
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftULong")]
    public abstract BigInteger? RotateLeftULong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkC//8AAJFoH5GoAv//AACRaQL//wAAkWibH5GpkgL//wAAkUA=
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftUShort")]
    public abstract BigInteger? RotateLeftUShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkB/wCRaBeRqWkB/wCRaJsXkaiSAf8AkUA=
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightByte")]
    public abstract BigInteger? RotateRightByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkD/////wAAAACRaAAfkalpA/////8AAAAAkWibAB+RqJID/////wAAAACRSgMAAACAAAAAADAMAwAAAAABAAAAn0A=
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000008000000000 [1 datoshi]
    /// JMPLT 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightInt")]
    public abstract BigInteger? RotateRightInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQCeJxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9weZxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xaGlyc2sD/////wAAAACRagAfkalrA/////8AAAAAkWqbAB+RqJID/////wAAAACRSgMAAACAAAAAADAMAwAAAAABAAAAn0A=
    /// INITSLOT 0402 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// INC [4 datoshi]
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
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// INC [4 datoshi]
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
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000008000000000 [1 datoshi]
    /// JMPLT 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightIntFromExpressions")]
    public abstract BigInteger? RotateRightIntFromExpressions(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkE//////////8AAAAAAAAAAJFoAD+RqWkE//////////8AAAAAAAAAAJFomwA/kaiSBP//////////AAAAAAAAAACRSgQAAAAAAAAAgAAAAAAAAAAAMBQEAAAAAAAAAAABAAAAAAAAAJ9A
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// JMPLT 14 [2 datoshi]
    /// PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightLong")]
    public abstract BigInteger? RotateRightLong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkB/wCRaBeRqWkB/wCRaJsXkaiSAf8AkUoBgAAwBgEAAZ9A
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT16 8000 [1 datoshi]
    /// JMPLT 06 [2 datoshi]
    /// PUSHINT16 0001 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightSByte")]
    public abstract BigInteger? RotateRightSByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkC//8AAJFoH5GpaQL//wAAkWibH5GokgL//wAAkUoCAIAAADAIAgAAAQCfQA==
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00800000 [1 datoshi]
    /// JMPLT 08 [2 datoshi]
    /// PUSHINT32 00000100 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightShort")]
    public abstract BigInteger? RotateRightShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkD/////wAAAACRaAAfkalpA/////8AAAAAkWibAB+RqJID/////wAAAACRQA==
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightUInt")]
    public abstract BigInteger? RotateRightUInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQCeJwD/////wAAAACRcHmcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWhpcnNrA/////8AAAAAkWoAH5GpawP/////AAAAAJFqmwAfkaiSA/////8AAAAAkUA=
    /// INITSLOT 0402 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// INC [4 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// INC [4 datoshi]
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
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightUIntFromExpressions")]
    public abstract BigInteger? RotateRightUIntFromExpressions(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkE//////////8AAAAAAAAAAJFoAD+RqWkE//////////8AAAAAAAAAAJFomwA/kaiSBP//////////AAAAAAAAAACRQA==
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightULong")]
    public abstract BigInteger? RotateRightULong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeHlwcWkC//8AAJFoH5GpaQL//wAAkWibH5GokgL//wAAkUA=
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// SHL [8 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightUShort")]
    public abstract BigInteger? RotateRightUShort(BigInteger? value, BigInteger? offset);

    #endregion
}
