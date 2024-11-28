using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Integer(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Integer"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""divRemByte"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""divRemShort"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":14,""safe"":false},{""name"":""divRemInt"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":28,""safe"":false},{""name"":""divRemLong"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":42,""safe"":false},{""name"":""divRemSbyte"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":56,""safe"":false},{""name"":""divRemUshort"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":70,""safe"":false},{""name"":""divRemUint"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":84,""safe"":false},{""name"":""divRemUlong"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Array"",""offset"":98,""safe"":false},{""name"":""clampByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":112,""safe"":false},{""name"":""clampSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":127,""safe"":false},{""name"":""clampShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":142,""safe"":false},{""name"":""clampUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":157,""safe"":false},{""name"":""clampInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":172,""safe"":false},{""name"":""clampUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":187,""safe"":false},{""name"":""clampLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":202,""safe"":false},{""name"":""clampULong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":217,""safe"":false},{""name"":""clampBigInteger"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""min"",""type"":""Integer""},{""name"":""max"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":232,""safe"":false},{""name"":""copySignInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":247,""safe"":false},{""name"":""copySignSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":270,""safe"":false},{""name"":""copySignShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":290,""safe"":false},{""name"":""copySignLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""sign"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":311,""safe"":false},{""name"":""createCheckedInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":338,""safe"":false},{""name"":""createCheckedByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":362,""safe"":false},{""name"":""createCheckedLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":376,""safe"":false},{""name"":""createCheckedUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":412,""safe"":false},{""name"":""createCheckedChar"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":440,""safe"":false},{""name"":""createCheckedShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":456,""safe"":false},{""name"":""createCheckedSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":474,""safe"":false},{""name"":""createSaturatingInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":489,""safe"":false},{""name"":""createSaturatingByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":533,""safe"":false},{""name"":""createSaturatingLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":571,""safe"":false},{""name"":""createSaturatingUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":623,""safe"":false},{""name"":""createSaturatingChar"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":675,""safe"":false},{""name"":""createSaturatingSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":715,""safe"":false},{""name"":""isEvenIntegerInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":753,""safe"":false},{""name"":""isEventUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":761,""safe"":false},{""name"":""isEvenLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":769,""safe"":false},{""name"":""isEvenUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":777,""safe"":false},{""name"":""isEvenShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":785,""safe"":false},{""name"":""isEvenUshort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":793,""safe"":false},{""name"":""isEvenByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":801,""safe"":false},{""name"":""isEvenSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":809,""safe"":false},{""name"":""isOddIntegerInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":817,""safe"":false},{""name"":""isOddUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":825,""safe"":false},{""name"":""isOddLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":833,""safe"":false},{""name"":""isOddUlong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":841,""safe"":false},{""name"":""isOddShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":849,""safe"":false},{""name"":""isOddUshort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":857,""safe"":false},{""name"":""isOddByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":865,""safe"":false},{""name"":""isOddSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":873,""safe"":false},{""name"":""isNegativeInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":881,""safe"":false},{""name"":""isNegativeLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":888,""safe"":false},{""name"":""isNegativeShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":895,""safe"":false},{""name"":""isNegativeSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":902,""safe"":false},{""name"":""isPositiveInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":909,""safe"":false},{""name"":""isPositiveLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":916,""safe"":false},{""name"":""isPositiveShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":923,""safe"":false},{""name"":""isPositiveSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":930,""safe"":false},{""name"":""isPow2Int"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":937,""safe"":false},{""name"":""isPow2UInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":958,""safe"":false},{""name"":""isPow2Long"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":979,""safe"":false},{""name"":""isPow2Ulong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1000,""safe"":false},{""name"":""isPow2Short"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1021,""safe"":false},{""name"":""isPow2Ushort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1042,""safe"":false},{""name"":""isPow2Byte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1063,""safe"":false},{""name"":""isPow2Sbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1084,""safe"":false},{""name"":""leadingZeroCountInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1105,""safe"":false},{""name"":""leadingZeroCountUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1134,""safe"":false},{""name"":""leadingZeroCountLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1156,""safe"":false},{""name"":""leadingZeroCountShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1185,""safe"":false},{""name"":""leadingZeroCountUshort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1213,""safe"":false},{""name"":""leadingZeroCountByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1234,""safe"":false},{""name"":""leadingZeroCountSbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1255,""safe"":false},{""name"":""log2Int"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1283,""safe"":false},{""name"":""log2UInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1307,""safe"":false},{""name"":""log2Long"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1331,""safe"":false},{""name"":""log2Short"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1355,""safe"":false},{""name"":""log2Ushort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1379,""safe"":false},{""name"":""log2Byte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1403,""safe"":false},{""name"":""log2Sbyte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1427,""safe"":false},{""name"":""rotateLeftInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1451,""safe"":false},{""name"":""rotateLeftUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1526,""safe"":false},{""name"":""rotateLeftLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1589,""safe"":false},{""name"":""rotateLeftULong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1704,""safe"":false},{""name"":""rotateLeftShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1799,""safe"":false},{""name"":""rotateLeftUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1851,""safe"":false},{""name"":""rotateLeftByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1895,""safe"":false},{""name"":""rotateLeftSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1931,""safe"":false},{""name"":""rotateRightInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1973,""safe"":false},{""name"":""rotateRightUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2062,""safe"":false},{""name"":""rotateRightLong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2092,""safe"":false},{""name"":""rotateRightULong"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2221,""safe"":false},{""name"":""rotateRightShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2259,""safe"":false},{""name"":""rotateRightUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2321,""safe"":false},{""name"":""rotateRightByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2344,""safe"":false},{""name"":""rotateRightSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""offset"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2365,""safe"":false},{""name"":""popCountByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2417,""safe"":false},{""name"":""popCountSByte"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2443,""safe"":false},{""name"":""popCountShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2469,""safe"":false},{""name"":""popCountUShort"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2497,""safe"":false},{""name"":""popCountInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2525,""safe"":false},{""name"":""popCountUInt"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2557,""safe"":false},{""name"":""popCountLong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2589,""safe"":false},{""name"":""popCountULong"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2629,""safe"":false},{""name"":""popCountBigInteger"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2669,""safe"":false},{""name"":""isPow2BigInteger"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":2790,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP37ClcAAnl4ShJNoVOiEsBAVwACeXhKEk2hU6ISwEBXAAJ5eEoSTaFTohLAQFcAAnl4ShJNoVOiEsBAVwACeXhKEk2hU6ISwEBXAAJ5eEoSTaFTohLAQFcAAnl4ShJNoVOiEsBAVwACeXhKEk2hU6ISwEBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAJ4eRAwBZoiBJqbSgL///9/MgM6QFcAAnh5EDAFmiIEmptKAH8yAzpAVwACeHkQMAWaIgSam0oB/38yAzpAVwACeHkQMAWaIgSam0oD/////////38yAzpAVwABeEoCAAAAgAMAAACAAAAAALskAzpAVwABeEoQAQABuyQDOkBXAAF4SgMAAAAAAAAAgAQAAAAAAAAAgAAAAAAAAAAAuyQDOkBXAAF4ShAEAAAAAAAAAAABAAAAAAAAALskAzpAVwABeEoQAgAAAQC7JAM6QFcAAXhKAQCAAgCAAAC7JAM6QFcAAXhKAIABgAC7JAM6QFcAAXgCAAAAgAL///9/SlFKUTADOlFKUUpRLAtFSlFKUTAIRUBTRUVAUEVAVwABeBAB/wBKUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUBXAAF4AwAAAAAAAACAA/////////9/SlFKUTADOlFKUUpRLAtFSlFKUTAIRUBTRUVAUEVAVwABeBAE//////////8AAAAAAAAAAEpRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQFcAAXgQAv//AABKUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUBXAAF4AIAAf0pRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQFcAAXgSoqpAVwABeBKiqkBXAAF4EqKqQFcAAXgSoqpAVwABeBKiqkBXAAF4EqKqQFcAAXgSoqpAVwABeBKiqkBXAAF4EqKxQFcAAXgSorFAVwABeBKisUBXAAF4EqKxQFcAAXgSorFAVwABeBKisUBXAAF4EqKxQFcAAXgSorFAVwABeBC1QFcAAXgQtUBXAAF4ELVAVwABeBC1QFcAAXgQuEBXAAF4ELhAVwABeBC4QFcAAXgQuEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAuBUUQQBBQShAoCBGpUJwi90UAIFCfQFcAAXgQUEoQKAgRqVCcIvdFACBQn0BXAAF4ShAuBUUQQBBQShAoCBGpUJwi90UAQFCfQFcAAXhKEC4FRRBAEFBKECgIEalQnCL3RSBQn0BXAAF4EFBKECgIEalQnCL3RSBQn0BXAAF4EFBKECgIEalQnCL3RRhQn0BXAAF4ShAuBUUQQBBQShAoCBGpUJwi90UYUJ9AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwACeHkAH5FQA/////8AAAAAkVCoA/////8AAAAAkXgD/////wAAAACReQAgUJ8AH5GpkkoDAAAAgAAAAAAwDAMAAAAAAQAAAJ9AVwACeHkAH5FQA/////8AAAAAkVCoA/////8AAAAAkXgD/////wAAAACReQAgUJ8AH5GpkgP/////AAAAAJFAVwACeHkAP5FQBP//////////AAAAAAAAAACRUKgE//////////8AAAAAAAAAAJF4BP//////////AAAAAAAAAACReQBAUJ8AP5GpkkoEAAAAAAAAAIAAAAAAAAAAADAUBAAAAAAAAAAAAQAAAAAAAACfQFcAAnh5AD+RUAT//////////wAAAAAAAAAAkVCoBP//////////AAAAAAAAAACReAT//////////wAAAAAAAAAAkXkAQFCfAD+RqZIE//////////8AAAAAAAAAAJFAVwACeHkfkVAC//8AAJFQqAL//wAAkXgC//8AAJF5IFCfH5GpkkoCAIAAADAIAgAAAQCfQFcAAnh5H5FQAv//AACRUKgC//8AAJF4Av//AACReSBQnx+RqZIC//8AAJFAVwACeHkXkVAB/wCRUKgB/wCReAH/AJF5GFCfF5GpkgH/AJFAVwACeHkXkVAB/wCRUKgB/wCReAH/AJF5GFCfF5GpkkoBgAAwBgEAAZ9AVwACeHkAH5EAIKIAIFCfUAP/////AAAAAJFQqAP/////AAAAAJF4A/////8AAAAAkXkAIKIAIFCfACBQnwAfkamSSgMAAACAAAAAADAMAwAAAAABAAAAn0BXAAJ4eQAfkal4ACB5nwAfkaiSA/////8AAAAAkUBXAAJ4eQA/kQBAogBAUJ9QBP//////////AAAAAAAAAACRUKgE//////////8AAAAAAAAAAJF4BP//////////AAAAAAAAAACReQBAogBAUJ8AQFCfAD+RqZJKBAAAAAAAAACAAAAAAAAAAAAwFAQAAAAAAAAAAAEAAAAAAAAAn0BXAAJ4eQA/kal4AEB5nwA/kaiSBP//////////AAAAAAAAAACRQFcAAnh5H5EgoiBQn1AC//8AAJFQqAL//wAAkXgC//8AAJF5IKIgUJ8gUJ8fkamSSgIAgAAAMAgCAAABAJ9AVwACeHkfkal4IHmfH5GokgL//wAAkUBXAAJ4eReRqXgYeZ8XkaiSAf8AkUBXAAJ4eReRGKIYUJ9QAf8AkVCoAf8AkXgB/wCReRiiGFCfGFCfF5GpkkoBgAAwBgEAAZ9AVwABeAH/AJEQUEoQKAxKEZFRnlARqSL0RUBXAAF4Af8AkRBQShAoDEoRkVGeUBGpIvRFQFcAAXgC//8AAJEQUEoQKAxKEZFRnlARqSL0RUBXAAF4Av//AACREFBKECgMShGRUZ5QEaki9EVAVwABeAP/////AAAAAJEQUEoQKAxKEZFRnlARqSL0RUBXAAF4A/////8AAAAAkRBQShAoDEoRkVGeUBGpIvRFQFcAAXgE//////////8AAAAAAAAAAJEQUEoQKAxKEZFRnlARqSL0RUBXAAF4BP//////////AAAAAAAAAACREFBKECgMShGRUZ5QEaki9EVAVwABeEoCAAAAgAMAAACAAAAAALsmDgP/////AAAAAJEiRwxCVmFsdWUgb3V0IG9mIHJhbmdlLCBtdXN0IGJlIGJldHdlZW4gaW50Lk1pblZhbHVlIGFuZCBpbnQuTWF4VmFsdWUuOhBQShAoDEoRkVGeUBGpIvRFQFcAAXhKECoFRSIISp2RECgECUAIQIplBfI=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : LDARG2 [2 datoshi]
    /// 06 : OVER [2 datoshi]
    /// 07 : OVER [2 datoshi]
    /// 08 : JMPLE 03 [2 datoshi]
    /// 0A : THROW [512 datoshi]
    /// 0B : REVERSE3 [2 datoshi]
    /// 0C : MAX [8 datoshi]
    /// 0D : MIN [8 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampBigInteger")]
    public abstract BigInteger? ClampBigInteger(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : LDARG2 [2 datoshi]
    /// 06 : OVER [2 datoshi]
    /// 07 : OVER [2 datoshi]
    /// 08 : JMPLE 03 [2 datoshi]
    /// 0A : THROW [512 datoshi]
    /// 0B : REVERSE3 [2 datoshi]
    /// 0C : MAX [8 datoshi]
    /// 0D : MIN [8 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampByte")]
    public abstract BigInteger? ClampByte(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : LDARG2 [2 datoshi]
    /// 06 : OVER [2 datoshi]
    /// 07 : OVER [2 datoshi]
    /// 08 : JMPLE 03 [2 datoshi]
    /// 0A : THROW [512 datoshi]
    /// 0B : REVERSE3 [2 datoshi]
    /// 0C : MAX [8 datoshi]
    /// 0D : MIN [8 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampInt")]
    public abstract BigInteger? ClampInt(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : LDARG2 [2 datoshi]
    /// 06 : OVER [2 datoshi]
    /// 07 : OVER [2 datoshi]
    /// 08 : JMPLE 03 [2 datoshi]
    /// 0A : THROW [512 datoshi]
    /// 0B : REVERSE3 [2 datoshi]
    /// 0C : MAX [8 datoshi]
    /// 0D : MIN [8 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampLong")]
    public abstract BigInteger? ClampLong(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : LDARG2 [2 datoshi]
    /// 06 : OVER [2 datoshi]
    /// 07 : OVER [2 datoshi]
    /// 08 : JMPLE 03 [2 datoshi]
    /// 0A : THROW [512 datoshi]
    /// 0B : REVERSE3 [2 datoshi]
    /// 0C : MAX [8 datoshi]
    /// 0D : MIN [8 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampSByte")]
    public abstract BigInteger? ClampSByte(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : LDARG2 [2 datoshi]
    /// 06 : OVER [2 datoshi]
    /// 07 : OVER [2 datoshi]
    /// 08 : JMPLE 03 [2 datoshi]
    /// 0A : THROW [512 datoshi]
    /// 0B : REVERSE3 [2 datoshi]
    /// 0C : MAX [8 datoshi]
    /// 0D : MIN [8 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampShort")]
    public abstract BigInteger? ClampShort(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : LDARG2 [2 datoshi]
    /// 06 : OVER [2 datoshi]
    /// 07 : OVER [2 datoshi]
    /// 08 : JMPLE 03 [2 datoshi]
    /// 0A : THROW [512 datoshi]
    /// 0B : REVERSE3 [2 datoshi]
    /// 0C : MAX [8 datoshi]
    /// 0D : MIN [8 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampUInt")]
    public abstract BigInteger? ClampUInt(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : LDARG2 [2 datoshi]
    /// 06 : OVER [2 datoshi]
    /// 07 : OVER [2 datoshi]
    /// 08 : JMPLE 03 [2 datoshi]
    /// 0A : THROW [512 datoshi]
    /// 0B : REVERSE3 [2 datoshi]
    /// 0C : MAX [8 datoshi]
    /// 0D : MIN [8 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampULong")]
    public abstract BigInteger? ClampULong(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : LDARG2 [2 datoshi]
    /// 06 : OVER [2 datoshi]
    /// 07 : OVER [2 datoshi]
    /// 08 : JMPLE 03 [2 datoshi]
    /// 0A : THROW [512 datoshi]
    /// 0B : REVERSE3 [2 datoshi]
    /// 0C : MAX [8 datoshi]
    /// 0D : MIN [8 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("clampUShort")]
    public abstract BigInteger? ClampUShort(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oC////fzIDOkA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPLT 05 [2 datoshi]
    /// 08 : ABS [4 datoshi]
    /// 09 : JMP 04 [2 datoshi]
    /// 0B : ABS [4 datoshi]
    /// 0C : NEGATE [4 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 13 : JMPLE 03 [2 datoshi]
    /// 15 : THROW [512 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("copySignInt")]
    public abstract BigInteger? CopySignInt(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oD/////////38yAzpA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPLT 05 [2 datoshi]
    /// 08 : ABS [4 datoshi]
    /// 09 : JMP 04 [2 datoshi]
    /// 0B : ABS [4 datoshi]
    /// 0C : NEGATE [4 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// 17 : JMPLE 03 [2 datoshi]
    /// 19 : THROW [512 datoshi]
    /// 1A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("copySignLong")]
    public abstract BigInteger? CopySignLong(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oAfzIDOkA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPLT 05 [2 datoshi]
    /// 08 : ABS [4 datoshi]
    /// 09 : JMP 04 [2 datoshi]
    /// 0B : ABS [4 datoshi]
    /// 0C : NEGATE [4 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : PUSHINT8 7F [1 datoshi]
    /// 10 : JMPLE 03 [2 datoshi]
    /// 12 : THROW [512 datoshi]
    /// 13 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("copySignSbyte")]
    public abstract BigInteger? CopySignSbyte(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oB/38yAzpA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPLT 05 [2 datoshi]
    /// 08 : ABS [4 datoshi]
    /// 09 : JMP 04 [2 datoshi]
    /// 0B : ABS [4 datoshi]
    /// 0C : NEGATE [4 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : PUSHINT16 FF7F [1 datoshi]
    /// 11 : JMPLE 03 [2 datoshi]
    /// 13 : THROW [512 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("copySignShort")]
    public abstract BigInteger? CopySignShort(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQAQABuyQDOkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : PUSHINT16 0001 [1 datoshi]
    /// 09 : WITHIN [8 datoshi]
    /// 0A : JMPIF 03 [2 datoshi]
    /// 0C : THROW [512 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedByte")]
    public abstract BigInteger? CreateCheckedByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQAgAAAQC7JAM6QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : PUSHINT32 00000100 [1 datoshi]
    /// 0B : WITHIN [8 datoshi]
    /// 0C : JMPIF 03 [2 datoshi]
    /// 0E : THROW [512 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedChar")]
    public abstract BigInteger? CreateCheckedChar(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoCAAAAgAMAAACAAAAAALskAzpA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSHINT32 00000080 [1 datoshi]
    /// 0A : PUSHINT64 0000008000000000 [1 datoshi]
    /// 13 : WITHIN [8 datoshi]
    /// 14 : JMPIF 03 [2 datoshi]
    /// 16 : THROW [512 datoshi]
    /// 17 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedInt")]
    public abstract BigInteger? CreateCheckedInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALskAzpA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSHINT64 0000000000000080 [1 datoshi]
    /// 0E : PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// 1F : WITHIN [8 datoshi]
    /// 20 : JMPIF 03 [2 datoshi]
    /// 22 : THROW [512 datoshi]
    /// 23 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedLong")]
    public abstract BigInteger? CreateCheckedLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAgAGAALskAzpA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSHINT8 80 [1 datoshi]
    /// 07 : PUSHINT16 8000 [1 datoshi]
    /// 0A : WITHIN [8 datoshi]
    /// 0B : JMPIF 03 [2 datoshi]
    /// 0D : THROW [512 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedSbyte")]
    public abstract BigInteger? CreateCheckedSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoBAIACAIAAALskAzpA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSHINT16 0080 [1 datoshi]
    /// 08 : PUSHINT32 00800000 [1 datoshi]
    /// 0D : WITHIN [8 datoshi]
    /// 0E : JMPIF 03 [2 datoshi]
    /// 10 : THROW [512 datoshi]
    /// 11 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedShort")]
    public abstract BigInteger? CreateCheckedShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQBAAAAAAAAAAAAQAAAAAAAAC7JAM6QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// 17 : WITHIN [8 datoshi]
    /// 18 : JMPIF 03 [2 datoshi]
    /// 1A : THROW [512 datoshi]
    /// 1B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedUlong")]
    public abstract BigInteger? CreateCheckedUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAB/wBKUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PUSHINT16 FF00 [1 datoshi]
    /// 08 : DUP [2 datoshi]
    /// 09 : ROT [2 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : ROT [2 datoshi]
    /// 0C : JMPLT 03 [2 datoshi]
    /// 0E : THROW [512 datoshi]
    /// 0F : ROT [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : ROT [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ROT [2 datoshi]
    /// 14 : JMPGT 0B [2 datoshi]
    /// 16 : DROP [2 datoshi]
    /// 17 : DUP [2 datoshi]
    /// 18 : ROT [2 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : ROT [2 datoshi]
    /// 1B : JMPLT 08 [2 datoshi]
    /// 1D : DROP [2 datoshi]
    /// 1E : RET [0 datoshi]
    /// 1F : REVERSE3 [2 datoshi]
    /// 20 : DROP [2 datoshi]
    /// 21 : DROP [2 datoshi]
    /// 22 : RET [0 datoshi]
    /// 23 : SWAP [2 datoshi]
    /// 24 : DROP [2 datoshi]
    /// 25 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingByte")]
    public abstract BigInteger? CreateSaturatingByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAC//8AAEpRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : ROT [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : ROT [2 datoshi]
    /// 0E : JMPLT 03 [2 datoshi]
    /// 10 : THROW [512 datoshi]
    /// 11 : ROT [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ROT [2 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : ROT [2 datoshi]
    /// 16 : JMPGT 0B [2 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : ROT [2 datoshi]
    /// 1B : DUP [2 datoshi]
    /// 1C : ROT [2 datoshi]
    /// 1D : JMPLT 08 [2 datoshi]
    /// 1F : DROP [2 datoshi]
    /// 20 : RET [0 datoshi]
    /// 21 : REVERSE3 [2 datoshi]
    /// 22 : DROP [2 datoshi]
    /// 23 : DROP [2 datoshi]
    /// 24 : RET [0 datoshi]
    /// 25 : SWAP [2 datoshi]
    /// 26 : DROP [2 datoshi]
    /// 27 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingChar")]
    public abstract BigInteger? CreateSaturatingChar(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAIAAACAAv///39KUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT32 00000080 [1 datoshi]
    /// 09 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : ROT [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : ROT [2 datoshi]
    /// 12 : JMPLT 03 [2 datoshi]
    /// 14 : THROW [512 datoshi]
    /// 15 : ROT [2 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : ROT [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : ROT [2 datoshi]
    /// 1A : JMPGT 0B [2 datoshi]
    /// 1C : DROP [2 datoshi]
    /// 1D : DUP [2 datoshi]
    /// 1E : ROT [2 datoshi]
    /// 1F : DUP [2 datoshi]
    /// 20 : ROT [2 datoshi]
    /// 21 : JMPLT 08 [2 datoshi]
    /// 23 : DROP [2 datoshi]
    /// 24 : RET [0 datoshi]
    /// 25 : REVERSE3 [2 datoshi]
    /// 26 : DROP [2 datoshi]
    /// 27 : DROP [2 datoshi]
    /// 28 : RET [0 datoshi]
    /// 29 : SWAP [2 datoshi]
    /// 2A : DROP [2 datoshi]
    /// 2B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingInt")]
    public abstract BigInteger? CreateSaturatingInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAMAAAAAAAAAgAP/////////f0pRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT64 0000000000000080 [1 datoshi]
    /// 0D : PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : ROT [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : ROT [2 datoshi]
    /// 1A : JMPLT 03 [2 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : ROT [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : ROT [2 datoshi]
    /// 20 : DUP [2 datoshi]
    /// 21 : ROT [2 datoshi]
    /// 22 : JMPGT 0B [2 datoshi]
    /// 24 : DROP [2 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : ROT [2 datoshi]
    /// 27 : DUP [2 datoshi]
    /// 28 : ROT [2 datoshi]
    /// 29 : JMPLT 08 [2 datoshi]
    /// 2B : DROP [2 datoshi]
    /// 2C : RET [0 datoshi]
    /// 2D : REVERSE3 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : DROP [2 datoshi]
    /// 30 : RET [0 datoshi]
    /// 31 : SWAP [2 datoshi]
    /// 32 : DROP [2 datoshi]
    /// 33 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingLong")]
    public abstract BigInteger? CreateSaturatingLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeACAAH9KUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT8 80 [1 datoshi]
    /// 06 : PUSHINT8 7F [1 datoshi]
    /// 08 : DUP [2 datoshi]
    /// 09 : ROT [2 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : ROT [2 datoshi]
    /// 0C : JMPLT 03 [2 datoshi]
    /// 0E : THROW [512 datoshi]
    /// 0F : ROT [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : ROT [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : ROT [2 datoshi]
    /// 14 : JMPGT 0B [2 datoshi]
    /// 16 : DROP [2 datoshi]
    /// 17 : DUP [2 datoshi]
    /// 18 : ROT [2 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : ROT [2 datoshi]
    /// 1B : JMPLT 08 [2 datoshi]
    /// 1D : DROP [2 datoshi]
    /// 1E : RET [0 datoshi]
    /// 1F : REVERSE3 [2 datoshi]
    /// 20 : DROP [2 datoshi]
    /// 21 : DROP [2 datoshi]
    /// 22 : RET [0 datoshi]
    /// 23 : SWAP [2 datoshi]
    /// 24 : DROP [2 datoshi]
    /// 25 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingSbyte")]
    public abstract BigInteger? CreateSaturatingSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAE//////////8AAAAAAAAAAEpRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : ROT [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : ROT [2 datoshi]
    /// 1A : JMPLT 03 [2 datoshi]
    /// 1C : THROW [512 datoshi]
    /// 1D : ROT [2 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : ROT [2 datoshi]
    /// 20 : DUP [2 datoshi]
    /// 21 : ROT [2 datoshi]
    /// 22 : JMPGT 0B [2 datoshi]
    /// 24 : DROP [2 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : ROT [2 datoshi]
    /// 27 : DUP [2 datoshi]
    /// 28 : ROT [2 datoshi]
    /// 29 : JMPLT 08 [2 datoshi]
    /// 2B : DROP [2 datoshi]
    /// 2C : RET [0 datoshi]
    /// 2D : REVERSE3 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : DROP [2 datoshi]
    /// 30 : RET [0 datoshi]
    /// 31 : SWAP [2 datoshi]
    /// 32 : DROP [2 datoshi]
    /// 33 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingUlong")]
    public abstract BigInteger? CreateSaturatingUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSH2 [1 datoshi]
    /// 07 : PICK [2 datoshi]
    /// 08 : DIV [8 datoshi]
    /// 09 : REVERSE3 [2 datoshi]
    /// 0A : MOD [8 datoshi]
    /// 0B : PUSH2 [1 datoshi]
    /// 0C : PACK [2048 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemByte")]
    public abstract IList<object>? DivRemByte(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSH2 [1 datoshi]
    /// 07 : PICK [2 datoshi]
    /// 08 : DIV [8 datoshi]
    /// 09 : REVERSE3 [2 datoshi]
    /// 0A : MOD [8 datoshi]
    /// 0B : PUSH2 [1 datoshi]
    /// 0C : PACK [2048 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemInt")]
    public abstract IList<object>? DivRemInt(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSH2 [1 datoshi]
    /// 07 : PICK [2 datoshi]
    /// 08 : DIV [8 datoshi]
    /// 09 : REVERSE3 [2 datoshi]
    /// 0A : MOD [8 datoshi]
    /// 0B : PUSH2 [1 datoshi]
    /// 0C : PACK [2048 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemLong")]
    public abstract IList<object>? DivRemLong(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSH2 [1 datoshi]
    /// 07 : PICK [2 datoshi]
    /// 08 : DIV [8 datoshi]
    /// 09 : REVERSE3 [2 datoshi]
    /// 0A : MOD [8 datoshi]
    /// 0B : PUSH2 [1 datoshi]
    /// 0C : PACK [2048 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemSbyte")]
    public abstract IList<object>? DivRemSbyte(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSH2 [1 datoshi]
    /// 07 : PICK [2 datoshi]
    /// 08 : DIV [8 datoshi]
    /// 09 : REVERSE3 [2 datoshi]
    /// 0A : MOD [8 datoshi]
    /// 0B : PUSH2 [1 datoshi]
    /// 0C : PACK [2048 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemShort")]
    public abstract IList<object>? DivRemShort(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSH2 [1 datoshi]
    /// 07 : PICK [2 datoshi]
    /// 08 : DIV [8 datoshi]
    /// 09 : REVERSE3 [2 datoshi]
    /// 0A : MOD [8 datoshi]
    /// 0B : PUSH2 [1 datoshi]
    /// 0C : PACK [2048 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemUint")]
    public abstract IList<object>? DivRemUint(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSH2 [1 datoshi]
    /// 07 : PICK [2 datoshi]
    /// 08 : DIV [8 datoshi]
    /// 09 : REVERSE3 [2 datoshi]
    /// 0A : MOD [8 datoshi]
    /// 0B : PUSH2 [1 datoshi]
    /// 0C : PACK [2048 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemUlong")]
    public abstract IList<object>? DivRemUlong(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG1 [2 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSH2 [1 datoshi]
    /// 07 : PICK [2 datoshi]
    /// 08 : DIV [8 datoshi]
    /// 09 : REVERSE3 [2 datoshi]
    /// 0A : MOD [8 datoshi]
    /// 0B : PUSH2 [1 datoshi]
    /// 0C : PACK [2048 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("divRemUshort")]
    public abstract IList<object>? DivRemUshort(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NOT [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenByte")]
    public abstract bool? IsEvenByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NOT [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenIntegerInt")]
    public abstract bool? IsEvenIntegerInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NOT [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenLong")]
    public abstract bool? IsEvenLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NOT [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenSbyte")]
    public abstract bool? IsEvenSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NOT [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenShort")]
    public abstract bool? IsEvenShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NOT [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEventUInt")]
    public abstract bool? IsEventUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NOT [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenUlong")]
    public abstract bool? IsEvenUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NOT [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEvenUshort")]
    public abstract bool? IsEvenUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : LT [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isNegativeInt")]
    public abstract bool? IsNegativeInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : LT [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isNegativeLong")]
    public abstract bool? IsNegativeLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : LT [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isNegativeSbyte")]
    public abstract bool? IsNegativeSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : LT [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isNegativeShort")]
    public abstract bool? IsNegativeShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NZ [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddByte")]
    public abstract bool? IsOddByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NZ [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddIntegerInt")]
    public abstract bool? IsOddIntegerInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NZ [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddLong")]
    public abstract bool? IsOddLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NZ [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddSbyte")]
    public abstract bool? IsOddSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NZ [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddShort")]
    public abstract bool? IsOddShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NZ [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddUInt")]
    public abstract bool? IsOddUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NZ [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddUlong")]
    public abstract bool? IsOddUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NZ [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isOddUshort")]
    public abstract bool? IsOddUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : GE [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPositiveInt")]
    public abstract bool? IsPositiveInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : GE [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPositiveLong")]
    public abstract bool? IsPositiveLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : GE [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPositiveSbyte")]
    public abstract bool? IsPositiveSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : GE [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPositiveShort")]
    public abstract bool? IsPositiveShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPNE 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : JMP 08 [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : DEC [4 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : JMPEQ 04 [2 datoshi]
    /// 11 : PUSHF [1 datoshi]
    /// 12 : RET [0 datoshi]
    /// 13 : PUSHT [1 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2BigInteger")]
    public abstract bool? IsPow2BigInteger(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPNE 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : JMP 08 [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : DEC [4 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : JMPEQ 04 [2 datoshi]
    /// 11 : PUSHF [1 datoshi]
    /// 12 : RET [0 datoshi]
    /// 13 : PUSHT [1 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Byte")]
    public abstract bool? IsPow2Byte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPNE 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : JMP 08 [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : DEC [4 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : JMPEQ 04 [2 datoshi]
    /// 11 : PUSHF [1 datoshi]
    /// 12 : RET [0 datoshi]
    /// 13 : PUSHT [1 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Int")]
    public abstract bool? IsPow2Int(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPNE 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : JMP 08 [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : DEC [4 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : JMPEQ 04 [2 datoshi]
    /// 11 : PUSHF [1 datoshi]
    /// 12 : RET [0 datoshi]
    /// 13 : PUSHT [1 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Long")]
    public abstract bool? IsPow2Long(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPNE 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : JMP 08 [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : DEC [4 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : JMPEQ 04 [2 datoshi]
    /// 11 : PUSHF [1 datoshi]
    /// 12 : RET [0 datoshi]
    /// 13 : PUSHT [1 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Sbyte")]
    public abstract bool? IsPow2Sbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPNE 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : JMP 08 [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : DEC [4 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : JMPEQ 04 [2 datoshi]
    /// 11 : PUSHF [1 datoshi]
    /// 12 : RET [0 datoshi]
    /// 13 : PUSHT [1 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Short")]
    public abstract bool? IsPow2Short(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPNE 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : JMP 08 [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : DEC [4 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : JMPEQ 04 [2 datoshi]
    /// 11 : PUSHF [1 datoshi]
    /// 12 : RET [0 datoshi]
    /// 13 : PUSHT [1 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2UInt")]
    public abstract bool? IsPow2UInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPNE 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : JMP 08 [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : DEC [4 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : JMPEQ 04 [2 datoshi]
    /// 11 : PUSHF [1 datoshi]
    /// 12 : RET [0 datoshi]
    /// 13 : PUSHT [1 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Ulong")]
    public abstract bool? IsPow2Ulong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPNE 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : JMP 08 [2 datoshi]
    /// 0B : DUP [2 datoshi]
    /// 0C : DEC [4 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : JMPEQ 04 [2 datoshi]
    /// 11 : PUSHF [1 datoshi]
    /// 12 : RET [0 datoshi]
    /// 13 : PUSHT [1 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Ushort")]
    public abstract bool? IsPow2Ushort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBBQShAoCBGpUJwi90UYUJ9A
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : SWAP [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : JMPEQ 08 [2 datoshi]
    /// 0A : PUSH1 [1 datoshi]
    /// 0B : SHR [8 datoshi]
    /// 0C : SWAP [2 datoshi]
    /// 0D : INC [4 datoshi]
    /// 0E : JMP F7 [2 datoshi]
    /// 10 : DROP [2 datoshi]
    /// 11 : PUSH8 [1 datoshi]
    /// 12 : SWAP [2 datoshi]
    /// 13 : SUB [8 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountByte")]
    public abstract BigInteger? LeadingZeroCountByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFACBQn0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPGE 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : SWAP [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : JMPEQ 08 [2 datoshi]
    /// 11 : PUSH1 [1 datoshi]
    /// 12 : SHR [8 datoshi]
    /// 13 : SWAP [2 datoshi]
    /// 14 : INC [4 datoshi]
    /// 15 : JMP F7 [2 datoshi]
    /// 17 : DROP [2 datoshi]
    /// 18 : PUSHINT8 20 [1 datoshi]
    /// 1A : SWAP [2 datoshi]
    /// 1B : SUB [8 datoshi]
    /// 1C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountInt")]
    public abstract BigInteger? LeadingZeroCountInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFAEBQn0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPGE 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : SWAP [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : JMPEQ 08 [2 datoshi]
    /// 11 : PUSH1 [1 datoshi]
    /// 12 : SHR [8 datoshi]
    /// 13 : SWAP [2 datoshi]
    /// 14 : INC [4 datoshi]
    /// 15 : JMP F7 [2 datoshi]
    /// 17 : DROP [2 datoshi]
    /// 18 : PUSHINT8 40 [1 datoshi]
    /// 1A : SWAP [2 datoshi]
    /// 1B : SUB [8 datoshi]
    /// 1C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountLong")]
    public abstract BigInteger? LeadingZeroCountLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFGFCfQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPGE 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : SWAP [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : JMPEQ 08 [2 datoshi]
    /// 11 : PUSH1 [1 datoshi]
    /// 12 : SHR [8 datoshi]
    /// 13 : SWAP [2 datoshi]
    /// 14 : INC [4 datoshi]
    /// 15 : JMP F7 [2 datoshi]
    /// 17 : DROP [2 datoshi]
    /// 18 : PUSH8 [1 datoshi]
    /// 19 : SWAP [2 datoshi]
    /// 1A : SUB [8 datoshi]
    /// 1B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountSbyte")]
    public abstract BigInteger? LeadingZeroCountSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFIFCfQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPGE 05 [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : RET [0 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : SWAP [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : JMPEQ 08 [2 datoshi]
    /// 11 : PUSH1 [1 datoshi]
    /// 12 : SHR [8 datoshi]
    /// 13 : SWAP [2 datoshi]
    /// 14 : INC [4 datoshi]
    /// 15 : JMP F7 [2 datoshi]
    /// 17 : DROP [2 datoshi]
    /// 18 : PUSH16 [1 datoshi]
    /// 19 : SWAP [2 datoshi]
    /// 1A : SUB [8 datoshi]
    /// 1B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountShort")]
    public abstract BigInteger? LeadingZeroCountShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBBQShAoCBGpUJwi90UAIFCfQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : SWAP [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : JMPEQ 08 [2 datoshi]
    /// 0A : PUSH1 [1 datoshi]
    /// 0B : SHR [8 datoshi]
    /// 0C : SWAP [2 datoshi]
    /// 0D : INC [4 datoshi]
    /// 0E : JMP F7 [2 datoshi]
    /// 10 : DROP [2 datoshi]
    /// 11 : PUSHINT8 20 [1 datoshi]
    /// 13 : SWAP [2 datoshi]
    /// 14 : SUB [8 datoshi]
    /// 15 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountUInt")]
    public abstract BigInteger? LeadingZeroCountUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBBQShAoCBGpUJwi90UgUJ9A
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : SWAP [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : JMPEQ 08 [2 datoshi]
    /// 0A : PUSH1 [1 datoshi]
    /// 0B : SHR [8 datoshi]
    /// 0C : SWAP [2 datoshi]
    /// 0D : INC [4 datoshi]
    /// 0E : JMP F7 [2 datoshi]
    /// 10 : DROP [2 datoshi]
    /// 11 : PUSH16 [1 datoshi]
    /// 12 : SWAP [2 datoshi]
    /// 13 : SUB [8 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("leadingZeroCountUshort")]
    public abstract BigInteger? LeadingZeroCountUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPGE 03 [2 datoshi]
    /// 08 : THROW [512 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : JMPEQ 0C [2 datoshi]
    /// 0D : PUSH0 [1 datoshi]
    /// 0E : INC [4 datoshi]
    /// 0F : OVER [2 datoshi]
    /// 10 : OVER [2 datoshi]
    /// 11 : SHR [8 datoshi]
    /// 12 : PUSH0 [1 datoshi]
    /// 13 : JMPGT FB [2 datoshi]
    /// 15 : NIP [2 datoshi]
    /// 16 : DEC [4 datoshi]
    /// 17 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Byte")]
    public abstract BigInteger? Log2Byte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPGE 03 [2 datoshi]
    /// 08 : THROW [512 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : JMPEQ 0C [2 datoshi]
    /// 0D : PUSH0 [1 datoshi]
    /// 0E : INC [4 datoshi]
    /// 0F : OVER [2 datoshi]
    /// 10 : OVER [2 datoshi]
    /// 11 : SHR [8 datoshi]
    /// 12 : PUSH0 [1 datoshi]
    /// 13 : JMPGT FB [2 datoshi]
    /// 15 : NIP [2 datoshi]
    /// 16 : DEC [4 datoshi]
    /// 17 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Int")]
    public abstract BigInteger? Log2Int(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPGE 03 [2 datoshi]
    /// 08 : THROW [512 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : JMPEQ 0C [2 datoshi]
    /// 0D : PUSH0 [1 datoshi]
    /// 0E : INC [4 datoshi]
    /// 0F : OVER [2 datoshi]
    /// 10 : OVER [2 datoshi]
    /// 11 : SHR [8 datoshi]
    /// 12 : PUSH0 [1 datoshi]
    /// 13 : JMPGT FB [2 datoshi]
    /// 15 : NIP [2 datoshi]
    /// 16 : DEC [4 datoshi]
    /// 17 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Long")]
    public abstract BigInteger? Log2Long(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPGE 03 [2 datoshi]
    /// 08 : THROW [512 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : JMPEQ 0C [2 datoshi]
    /// 0D : PUSH0 [1 datoshi]
    /// 0E : INC [4 datoshi]
    /// 0F : OVER [2 datoshi]
    /// 10 : OVER [2 datoshi]
    /// 11 : SHR [8 datoshi]
    /// 12 : PUSH0 [1 datoshi]
    /// 13 : JMPGT FB [2 datoshi]
    /// 15 : NIP [2 datoshi]
    /// 16 : DEC [4 datoshi]
    /// 17 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Sbyte")]
    public abstract BigInteger? Log2Sbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPGE 03 [2 datoshi]
    /// 08 : THROW [512 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : JMPEQ 0C [2 datoshi]
    /// 0D : PUSH0 [1 datoshi]
    /// 0E : INC [4 datoshi]
    /// 0F : OVER [2 datoshi]
    /// 10 : OVER [2 datoshi]
    /// 11 : SHR [8 datoshi]
    /// 12 : PUSH0 [1 datoshi]
    /// 13 : JMPGT FB [2 datoshi]
    /// 15 : NIP [2 datoshi]
    /// 16 : DEC [4 datoshi]
    /// 17 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Short")]
    public abstract BigInteger? Log2Short(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPGE 03 [2 datoshi]
    /// 08 : THROW [512 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : JMPEQ 0C [2 datoshi]
    /// 0D : PUSH0 [1 datoshi]
    /// 0E : INC [4 datoshi]
    /// 0F : OVER [2 datoshi]
    /// 10 : OVER [2 datoshi]
    /// 11 : SHR [8 datoshi]
    /// 12 : PUSH0 [1 datoshi]
    /// 13 : JMPGT FB [2 datoshi]
    /// 15 : NIP [2 datoshi]
    /// 16 : DEC [4 datoshi]
    /// 17 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2UInt")]
    public abstract BigInteger? Log2UInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : JMPGE 03 [2 datoshi]
    /// 08 : THROW [512 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : JMPEQ 0C [2 datoshi]
    /// 0D : PUSH0 [1 datoshi]
    /// 0E : INC [4 datoshi]
    /// 0F : OVER [2 datoshi]
    /// 10 : OVER [2 datoshi]
    /// 11 : SHR [8 datoshi]
    /// 12 : PUSH0 [1 datoshi]
    /// 13 : JMPGT FB [2 datoshi]
    /// 15 : NIP [2 datoshi]
    /// 16 : DEC [4 datoshi]
    /// 17 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Ushort")]
    public abstract BigInteger? Log2Ushort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoCAAAAgAMAAACAAAAAALsmDgP/////AAAAAJEiRwxCVmFsdWUgb3V0IG9mIHJhbmdlLCBtdXN0IGJlIGJldHdlZW4gaW50Lk1pblZhbHVlIGFuZCBpbnQuTWF4VmFsdWUuOhBQShAoDEoRkVGeUBGpIvRFQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSHINT32 00000080 [1 datoshi]
    /// 0A : PUSHINT64 0000008000000000 [1 datoshi]
    /// 13 : WITHIN [8 datoshi]
    /// 14 : JMPIFNOT 0E [2 datoshi]
    /// 16 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1F : AND [8 datoshi]
    /// 20 : JMP 47 [2 datoshi]
    /// 22 : PUSHDATA1 56616C7565206F7574206F662072616E67652C206D757374206265206265747765656E20696E742E4D696E56616C756520616E6420696E742E4D617856616C75652E [8 datoshi]
    /// 66 : THROW [512 datoshi]
    /// 67 : PUSH0 [1 datoshi]
    /// 68 : SWAP [2 datoshi]
    /// 69 : DUP [2 datoshi]
    /// 6A : PUSH0 [1 datoshi]
    /// 6B : JMPEQ 0C [2 datoshi]
    /// 6D : DUP [2 datoshi]
    /// 6E : PUSH1 [1 datoshi]
    /// 6F : AND [8 datoshi]
    /// 70 : ROT [2 datoshi]
    /// 71 : ADD [8 datoshi]
    /// 72 : SWAP [2 datoshi]
    /// 73 : PUSH1 [1 datoshi]
    /// 74 : SHR [8 datoshi]
    /// 75 : JMP F4 [2 datoshi]
    /// 77 : DROP [2 datoshi]
    /// 78 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountBigInteger")]
    public abstract BigInteger? PopCountBigInteger(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAH/AJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT16 FF00 [1 datoshi]
    /// 07 : AND [8 datoshi]
    /// 08 : PUSH0 [1 datoshi]
    /// 09 : SWAP [2 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : JMPEQ 0C [2 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSH1 [1 datoshi]
    /// 10 : AND [8 datoshi]
    /// 11 : ROT [2 datoshi]
    /// 12 : ADD [8 datoshi]
    /// 13 : SWAP [2 datoshi]
    /// 14 : PUSH1 [1 datoshi]
    /// 15 : SHR [8 datoshi]
    /// 16 : JMP F4 [2 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountByte")]
    public abstract BigInteger? PopCountByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAP/////AAAAAJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : SWAP [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSH0 [1 datoshi]
    /// 12 : JMPEQ 0C [2 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : PUSH1 [1 datoshi]
    /// 16 : AND [8 datoshi]
    /// 17 : ROT [2 datoshi]
    /// 18 : ADD [8 datoshi]
    /// 19 : SWAP [2 datoshi]
    /// 1A : PUSH1 [1 datoshi]
    /// 1B : SHR [8 datoshi]
    /// 1C : JMP F4 [2 datoshi]
    /// 1E : DROP [2 datoshi]
    /// 1F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountInt")]
    public abstract BigInteger? PopCountInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAT//////////wAAAAAAAAAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 15 : AND [8 datoshi]
    /// 16 : PUSH0 [1 datoshi]
    /// 17 : SWAP [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : PUSH0 [1 datoshi]
    /// 1A : JMPEQ 0C [2 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSH1 [1 datoshi]
    /// 1E : AND [8 datoshi]
    /// 1F : ROT [2 datoshi]
    /// 20 : ADD [8 datoshi]
    /// 21 : SWAP [2 datoshi]
    /// 22 : PUSH1 [1 datoshi]
    /// 23 : SHR [8 datoshi]
    /// 24 : JMP F4 [2 datoshi]
    /// 26 : DROP [2 datoshi]
    /// 27 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountLong")]
    public abstract BigInteger? PopCountLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAH/AJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT16 FF00 [1 datoshi]
    /// 07 : AND [8 datoshi]
    /// 08 : PUSH0 [1 datoshi]
    /// 09 : SWAP [2 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : JMPEQ 0C [2 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSH1 [1 datoshi]
    /// 10 : AND [8 datoshi]
    /// 11 : ROT [2 datoshi]
    /// 12 : ADD [8 datoshi]
    /// 13 : SWAP [2 datoshi]
    /// 14 : PUSH1 [1 datoshi]
    /// 15 : SHR [8 datoshi]
    /// 16 : JMP F4 [2 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountSByte")]
    public abstract BigInteger? PopCountSByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAL//wAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 09 : AND [8 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : SWAP [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSH0 [1 datoshi]
    /// 0E : JMPEQ 0C [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSH1 [1 datoshi]
    /// 12 : AND [8 datoshi]
    /// 13 : ROT [2 datoshi]
    /// 14 : ADD [8 datoshi]
    /// 15 : SWAP [2 datoshi]
    /// 16 : PUSH1 [1 datoshi]
    /// 17 : SHR [8 datoshi]
    /// 18 : JMP F4 [2 datoshi]
    /// 1A : DROP [2 datoshi]
    /// 1B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountShort")]
    public abstract BigInteger? PopCountShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAP/////AAAAAJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : SWAP [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSH0 [1 datoshi]
    /// 12 : JMPEQ 0C [2 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : PUSH1 [1 datoshi]
    /// 16 : AND [8 datoshi]
    /// 17 : ROT [2 datoshi]
    /// 18 : ADD [8 datoshi]
    /// 19 : SWAP [2 datoshi]
    /// 1A : PUSH1 [1 datoshi]
    /// 1B : SHR [8 datoshi]
    /// 1C : JMP F4 [2 datoshi]
    /// 1E : DROP [2 datoshi]
    /// 1F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountUInt")]
    public abstract BigInteger? PopCountUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAT//////////wAAAAAAAAAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 15 : AND [8 datoshi]
    /// 16 : PUSH0 [1 datoshi]
    /// 17 : SWAP [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : PUSH0 [1 datoshi]
    /// 1A : JMPEQ 0C [2 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSH1 [1 datoshi]
    /// 1E : AND [8 datoshi]
    /// 1F : ROT [2 datoshi]
    /// 20 : ADD [8 datoshi]
    /// 21 : SWAP [2 datoshi]
    /// 22 : PUSH1 [1 datoshi]
    /// 23 : SHR [8 datoshi]
    /// 24 : JMP F4 [2 datoshi]
    /// 26 : DROP [2 datoshi]
    /// 27 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountULong")]
    public abstract BigInteger? PopCountULong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAL//wAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 09 : AND [8 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : SWAP [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSH0 [1 datoshi]
    /// 0E : JMPEQ 0C [2 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : PUSH1 [1 datoshi]
    /// 12 : AND [8 datoshi]
    /// 13 : ROT [2 datoshi]
    /// 14 : ADD [8 datoshi]
    /// 15 : SWAP [2 datoshi]
    /// 16 : PUSH1 [1 datoshi]
    /// 17 : SHR [8 datoshi]
    /// 18 : JMP F4 [2 datoshi]
    /// 1A : DROP [2 datoshi]
    /// 1B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountUShort")]
    public abstract BigInteger? PopCountUShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkXkVAB/wCRUKgB/wCReAH/AJF5GFCfF5GpkgH/AJFA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSH7 [1 datoshi]
    /// 06 : AND [8 datoshi]
    /// 07 : SWAP [2 datoshi]
    /// 08 : PUSHINT16 FF00 [1 datoshi]
    /// 0B : AND [8 datoshi]
    /// 0C : SWAP [2 datoshi]
    /// 0D : SHL [8 datoshi]
    /// 0E : PUSHINT16 FF00 [1 datoshi]
    /// 11 : AND [8 datoshi]
    /// 12 : LDARG0 [2 datoshi]
    /// 13 : PUSHINT16 FF00 [1 datoshi]
    /// 16 : AND [8 datoshi]
    /// 17 : LDARG1 [2 datoshi]
    /// 18 : PUSH8 [1 datoshi]
    /// 19 : SWAP [2 datoshi]
    /// 1A : SUB [8 datoshi]
    /// 1B : PUSH7 [1 datoshi]
    /// 1C : AND [8 datoshi]
    /// 1D : SHR [8 datoshi]
    /// 1E : OR [8 datoshi]
    /// 1F : PUSHINT16 FF00 [1 datoshi]
    /// 22 : AND [8 datoshi]
    /// 23 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftByte")]
    public abstract BigInteger? RotateLeftByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5FQA/////8AAAAAkVCoA/////8AAAAAkXgD/////wAAAACReQAgUJ8AH5GpkkoDAAAAgAAAAAAwDAMAAAAAAQAAAJ9A
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSHINT8 1F [1 datoshi]
    /// 07 : AND [8 datoshi]
    /// 08 : SWAP [2 datoshi]
    /// 09 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 12 : AND [8 datoshi]
    /// 13 : SWAP [2 datoshi]
    /// 14 : SHL [8 datoshi]
    /// 15 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1E : AND [8 datoshi]
    /// 1F : LDARG0 [2 datoshi]
    /// 20 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 29 : AND [8 datoshi]
    /// 2A : LDARG1 [2 datoshi]
    /// 2B : PUSHINT8 20 [1 datoshi]
    /// 2D : SWAP [2 datoshi]
    /// 2E : SUB [8 datoshi]
    /// 2F : PUSHINT8 1F [1 datoshi]
    /// 31 : AND [8 datoshi]
    /// 32 : SHR [8 datoshi]
    /// 33 : OR [8 datoshi]
    /// 34 : DUP [2 datoshi]
    /// 35 : PUSHINT64 0000008000000000 [1 datoshi]
    /// 3E : JMPLT 0C [2 datoshi]
    /// 40 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 49 : SUB [8 datoshi]
    /// 4A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftInt")]
    public abstract BigInteger? RotateLeftInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAP5FQBP//////////AAAAAAAAAACRUKgE//////////8AAAAAAAAAAJF4BP//////////AAAAAAAAAACReQBAUJ8AP5GpkkoEAAAAAAAAAIAAAAAAAAAAADAUBAAAAAAAAAAAAQAAAAAAAACfQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSHINT8 3F [1 datoshi]
    /// 07 : AND [8 datoshi]
    /// 08 : SWAP [2 datoshi]
    /// 09 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 1A : AND [8 datoshi]
    /// 1B : SWAP [2 datoshi]
    /// 1C : SHL [8 datoshi]
    /// 1D : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 2E : AND [8 datoshi]
    /// 2F : LDARG0 [2 datoshi]
    /// 30 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 41 : AND [8 datoshi]
    /// 42 : LDARG1 [2 datoshi]
    /// 43 : PUSHINT8 40 [1 datoshi]
    /// 45 : SWAP [2 datoshi]
    /// 46 : SUB [8 datoshi]
    /// 47 : PUSHINT8 3F [1 datoshi]
    /// 49 : AND [8 datoshi]
    /// 4A : SHR [8 datoshi]
    /// 4B : OR [8 datoshi]
    /// 4C : DUP [2 datoshi]
    /// 4D : PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// 5E : JMPLT 14 [2 datoshi]
    /// 60 : PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// 71 : SUB [8 datoshi]
    /// 72 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftLong")]
    public abstract BigInteger? RotateLeftLong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkXkVAB/wCRUKgB/wCReAH/AJF5GFCfF5GpkkoBgAAwBgEAAZ9A
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSH7 [1 datoshi]
    /// 06 : AND [8 datoshi]
    /// 07 : SWAP [2 datoshi]
    /// 08 : PUSHINT16 FF00 [1 datoshi]
    /// 0B : AND [8 datoshi]
    /// 0C : SWAP [2 datoshi]
    /// 0D : SHL [8 datoshi]
    /// 0E : PUSHINT16 FF00 [1 datoshi]
    /// 11 : AND [8 datoshi]
    /// 12 : LDARG0 [2 datoshi]
    /// 13 : PUSHINT16 FF00 [1 datoshi]
    /// 16 : AND [8 datoshi]
    /// 17 : LDARG1 [2 datoshi]
    /// 18 : PUSH8 [1 datoshi]
    /// 19 : SWAP [2 datoshi]
    /// 1A : SUB [8 datoshi]
    /// 1B : PUSH7 [1 datoshi]
    /// 1C : AND [8 datoshi]
    /// 1D : SHR [8 datoshi]
    /// 1E : OR [8 datoshi]
    /// 1F : DUP [2 datoshi]
    /// 20 : PUSHINT16 8000 [1 datoshi]
    /// 23 : JMPLT 06 [2 datoshi]
    /// 25 : PUSHINT16 0001 [1 datoshi]
    /// 28 : SUB [8 datoshi]
    /// 29 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftSByte")]
    public abstract BigInteger? RotateLeftSByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkfkVAC//8AAJFQqAL//wAAkXgC//8AAJF5IFCfH5GpkkoCAIAAADAIAgAAAQCfQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSH15 [1 datoshi]
    /// 06 : AND [8 datoshi]
    /// 07 : SWAP [2 datoshi]
    /// 08 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : SWAP [2 datoshi]
    /// 0F : SHL [8 datoshi]
    /// 10 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 15 : AND [8 datoshi]
    /// 16 : LDARG0 [2 datoshi]
    /// 17 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 1C : AND [8 datoshi]
    /// 1D : LDARG1 [2 datoshi]
    /// 1E : PUSH16 [1 datoshi]
    /// 1F : SWAP [2 datoshi]
    /// 20 : SUB [8 datoshi]
    /// 21 : PUSH15 [1 datoshi]
    /// 22 : AND [8 datoshi]
    /// 23 : SHR [8 datoshi]
    /// 24 : OR [8 datoshi]
    /// 25 : DUP [2 datoshi]
    /// 26 : PUSHINT32 00800000 [1 datoshi]
    /// 2B : JMPLT 08 [2 datoshi]
    /// 2D : PUSHINT32 00000100 [1 datoshi]
    /// 32 : SUB [8 datoshi]
    /// 33 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftShort")]
    public abstract BigInteger? RotateLeftShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5FQA/////8AAAAAkVCoA/////8AAAAAkXgD/////wAAAACReQAgUJ8AH5GpkgP/////AAAAAJFA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSHINT8 1F [1 datoshi]
    /// 07 : AND [8 datoshi]
    /// 08 : SWAP [2 datoshi]
    /// 09 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 12 : AND [8 datoshi]
    /// 13 : SWAP [2 datoshi]
    /// 14 : SHL [8 datoshi]
    /// 15 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1E : AND [8 datoshi]
    /// 1F : LDARG0 [2 datoshi]
    /// 20 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 29 : AND [8 datoshi]
    /// 2A : LDARG1 [2 datoshi]
    /// 2B : PUSHINT8 20 [1 datoshi]
    /// 2D : SWAP [2 datoshi]
    /// 2E : SUB [8 datoshi]
    /// 2F : PUSHINT8 1F [1 datoshi]
    /// 31 : AND [8 datoshi]
    /// 32 : SHR [8 datoshi]
    /// 33 : OR [8 datoshi]
    /// 34 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 3D : AND [8 datoshi]
    /// 3E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftUInt")]
    public abstract BigInteger? RotateLeftUInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAP5FQBP//////////AAAAAAAAAACRUKgE//////////8AAAAAAAAAAJF4BP//////////AAAAAAAAAACReQBAUJ8AP5GpkgT//////////wAAAAAAAAAAkUA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSHINT8 3F [1 datoshi]
    /// 07 : AND [8 datoshi]
    /// 08 : SWAP [2 datoshi]
    /// 09 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 1A : AND [8 datoshi]
    /// 1B : SWAP [2 datoshi]
    /// 1C : SHL [8 datoshi]
    /// 1D : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 2E : AND [8 datoshi]
    /// 2F : LDARG0 [2 datoshi]
    /// 30 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 41 : AND [8 datoshi]
    /// 42 : LDARG1 [2 datoshi]
    /// 43 : PUSHINT8 40 [1 datoshi]
    /// 45 : SWAP [2 datoshi]
    /// 46 : SUB [8 datoshi]
    /// 47 : PUSHINT8 3F [1 datoshi]
    /// 49 : AND [8 datoshi]
    /// 4A : SHR [8 datoshi]
    /// 4B : OR [8 datoshi]
    /// 4C : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 5D : AND [8 datoshi]
    /// 5E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftULong")]
    public abstract BigInteger? RotateLeftULong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkfkVAC//8AAJFQqAL//wAAkXgC//8AAJF5IFCfH5GpkgL//wAAkUA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSH15 [1 datoshi]
    /// 06 : AND [8 datoshi]
    /// 07 : SWAP [2 datoshi]
    /// 08 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : SWAP [2 datoshi]
    /// 0F : SHL [8 datoshi]
    /// 10 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 15 : AND [8 datoshi]
    /// 16 : LDARG0 [2 datoshi]
    /// 17 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 1C : AND [8 datoshi]
    /// 1D : LDARG1 [2 datoshi]
    /// 1E : PUSH16 [1 datoshi]
    /// 1F : SWAP [2 datoshi]
    /// 20 : SUB [8 datoshi]
    /// 21 : PUSH15 [1 datoshi]
    /// 22 : AND [8 datoshi]
    /// 23 : SHR [8 datoshi]
    /// 24 : OR [8 datoshi]
    /// 25 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 2A : AND [8 datoshi]
    /// 2B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateLeftUShort")]
    public abstract BigInteger? RotateLeftUShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkXkal4GHmfF5GokgH/AJFA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSH7 [1 datoshi]
    /// 06 : AND [8 datoshi]
    /// 07 : SHR [8 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : PUSH8 [1 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : SUB [8 datoshi]
    /// 0C : PUSH7 [1 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : SHL [8 datoshi]
    /// 0F : OR [8 datoshi]
    /// 10 : PUSHINT16 FF00 [1 datoshi]
    /// 13 : AND [8 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightByte")]
    public abstract BigInteger? RotateRightByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5EAIKIAIFCfUAP/////AAAAAJFQqAP/////AAAAAJF4A/////8AAAAAkXkAIKIAIFCfACBQnwAfkamSSgMAAACAAAAAADAMAwAAAAABAAAAn0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSHINT8 1F [1 datoshi]
    /// 07 : AND [8 datoshi]
    /// 08 : PUSHINT8 20 [1 datoshi]
    /// 0A : MOD [8 datoshi]
    /// 0B : PUSHINT8 20 [1 datoshi]
    /// 0D : SWAP [2 datoshi]
    /// 0E : SUB [8 datoshi]
    /// 0F : SWAP [2 datoshi]
    /// 10 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 19 : AND [8 datoshi]
    /// 1A : SWAP [2 datoshi]
    /// 1B : SHL [8 datoshi]
    /// 1C : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 25 : AND [8 datoshi]
    /// 26 : LDARG0 [2 datoshi]
    /// 27 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 30 : AND [8 datoshi]
    /// 31 : LDARG1 [2 datoshi]
    /// 32 : PUSHINT8 20 [1 datoshi]
    /// 34 : MOD [8 datoshi]
    /// 35 : PUSHINT8 20 [1 datoshi]
    /// 37 : SWAP [2 datoshi]
    /// 38 : SUB [8 datoshi]
    /// 39 : PUSHINT8 20 [1 datoshi]
    /// 3B : SWAP [2 datoshi]
    /// 3C : SUB [8 datoshi]
    /// 3D : PUSHINT8 1F [1 datoshi]
    /// 3F : AND [8 datoshi]
    /// 40 : SHR [8 datoshi]
    /// 41 : OR [8 datoshi]
    /// 42 : DUP [2 datoshi]
    /// 43 : PUSHINT64 0000008000000000 [1 datoshi]
    /// 4C : JMPLT 0C [2 datoshi]
    /// 4E : PUSHINT64 0000000001000000 [1 datoshi]
    /// 57 : SUB [8 datoshi]
    /// 58 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightInt")]
    public abstract BigInteger? RotateRightInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAP5EAQKIAQFCfUAT//////////wAAAAAAAAAAkVCoBP//////////AAAAAAAAAACReAT//////////wAAAAAAAAAAkXkAQKIAQFCfAEBQnwA/kamSSgQAAAAAAAAAgAAAAAAAAAAAMBQEAAAAAAAAAAABAAAAAAAAAJ9A
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSHINT8 3F [1 datoshi]
    /// 07 : AND [8 datoshi]
    /// 08 : PUSHINT8 40 [1 datoshi]
    /// 0A : MOD [8 datoshi]
    /// 0B : PUSHINT8 40 [1 datoshi]
    /// 0D : SWAP [2 datoshi]
    /// 0E : SUB [8 datoshi]
    /// 0F : SWAP [2 datoshi]
    /// 10 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : SWAP [2 datoshi]
    /// 23 : SHL [8 datoshi]
    /// 24 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 35 : AND [8 datoshi]
    /// 36 : LDARG0 [2 datoshi]
    /// 37 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 48 : AND [8 datoshi]
    /// 49 : LDARG1 [2 datoshi]
    /// 4A : PUSHINT8 40 [1 datoshi]
    /// 4C : MOD [8 datoshi]
    /// 4D : PUSHINT8 40 [1 datoshi]
    /// 4F : SWAP [2 datoshi]
    /// 50 : SUB [8 datoshi]
    /// 51 : PUSHINT8 40 [1 datoshi]
    /// 53 : SWAP [2 datoshi]
    /// 54 : SUB [8 datoshi]
    /// 55 : PUSHINT8 3F [1 datoshi]
    /// 57 : AND [8 datoshi]
    /// 58 : SHR [8 datoshi]
    /// 59 : OR [8 datoshi]
    /// 5A : DUP [2 datoshi]
    /// 5B : PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// 6C : JMPLT 14 [2 datoshi]
    /// 6E : PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// 7F : SUB [8 datoshi]
    /// 80 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightLong")]
    public abstract BigInteger? RotateRightLong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkXkRiiGFCfUAH/AJFQqAH/AJF4Af8AkXkYohhQnxhQnxeRqZJKAYAAMAYBAAGfQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSH7 [1 datoshi]
    /// 06 : AND [8 datoshi]
    /// 07 : PUSH8 [1 datoshi]
    /// 08 : MOD [8 datoshi]
    /// 09 : PUSH8 [1 datoshi]
    /// 0A : SWAP [2 datoshi]
    /// 0B : SUB [8 datoshi]
    /// 0C : SWAP [2 datoshi]
    /// 0D : PUSHINT16 FF00 [1 datoshi]
    /// 10 : AND [8 datoshi]
    /// 11 : SWAP [2 datoshi]
    /// 12 : SHL [8 datoshi]
    /// 13 : PUSHINT16 FF00 [1 datoshi]
    /// 16 : AND [8 datoshi]
    /// 17 : LDARG0 [2 datoshi]
    /// 18 : PUSHINT16 FF00 [1 datoshi]
    /// 1B : AND [8 datoshi]
    /// 1C : LDARG1 [2 datoshi]
    /// 1D : PUSH8 [1 datoshi]
    /// 1E : MOD [8 datoshi]
    /// 1F : PUSH8 [1 datoshi]
    /// 20 : SWAP [2 datoshi]
    /// 21 : SUB [8 datoshi]
    /// 22 : PUSH8 [1 datoshi]
    /// 23 : SWAP [2 datoshi]
    /// 24 : SUB [8 datoshi]
    /// 25 : PUSH7 [1 datoshi]
    /// 26 : AND [8 datoshi]
    /// 27 : SHR [8 datoshi]
    /// 28 : OR [8 datoshi]
    /// 29 : DUP [2 datoshi]
    /// 2A : PUSHINT16 8000 [1 datoshi]
    /// 2D : JMPLT 06 [2 datoshi]
    /// 2F : PUSHINT16 0001 [1 datoshi]
    /// 32 : SUB [8 datoshi]
    /// 33 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightSByte")]
    public abstract BigInteger? RotateRightSByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkfkSCiIFCfUAL//wAAkVCoAv//AACReAL//wAAkXkgoiBQnyBQnx+RqZJKAgCAAAAwCAIAAAEAn0A=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSH15 [1 datoshi]
    /// 06 : AND [8 datoshi]
    /// 07 : PUSH16 [1 datoshi]
    /// 08 : MOD [8 datoshi]
    /// 09 : PUSH16 [1 datoshi]
    /// 0A : SWAP [2 datoshi]
    /// 0B : SUB [8 datoshi]
    /// 0C : SWAP [2 datoshi]
    /// 0D : PUSHINT32 FFFF0000 [1 datoshi]
    /// 12 : AND [8 datoshi]
    /// 13 : SWAP [2 datoshi]
    /// 14 : SHL [8 datoshi]
    /// 15 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 1A : AND [8 datoshi]
    /// 1B : LDARG0 [2 datoshi]
    /// 1C : PUSHINT32 FFFF0000 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : LDARG1 [2 datoshi]
    /// 23 : PUSH16 [1 datoshi]
    /// 24 : MOD [8 datoshi]
    /// 25 : PUSH16 [1 datoshi]
    /// 26 : SWAP [2 datoshi]
    /// 27 : SUB [8 datoshi]
    /// 28 : PUSH16 [1 datoshi]
    /// 29 : SWAP [2 datoshi]
    /// 2A : SUB [8 datoshi]
    /// 2B : PUSH15 [1 datoshi]
    /// 2C : AND [8 datoshi]
    /// 2D : SHR [8 datoshi]
    /// 2E : OR [8 datoshi]
    /// 2F : DUP [2 datoshi]
    /// 30 : PUSHINT32 00800000 [1 datoshi]
    /// 35 : JMPLT 08 [2 datoshi]
    /// 37 : PUSHINT32 00000100 [1 datoshi]
    /// 3C : SUB [8 datoshi]
    /// 3D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightShort")]
    public abstract BigInteger? RotateRightShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5GpeAAgeZ8AH5GokgP/////AAAAAJFA
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSHINT8 1F [1 datoshi]
    /// 07 : AND [8 datoshi]
    /// 08 : SHR [8 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : PUSHINT8 20 [1 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : SUB [8 datoshi]
    /// 0E : PUSHINT8 1F [1 datoshi]
    /// 10 : AND [8 datoshi]
    /// 11 : SHL [8 datoshi]
    /// 12 : OR [8 datoshi]
    /// 13 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 1C : AND [8 datoshi]
    /// 1D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightUInt")]
    public abstract BigInteger? RotateRightUInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAP5GpeABAeZ8AP5GokgT//////////wAAAAAAAAAAkUA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSHINT8 3F [1 datoshi]
    /// 07 : AND [8 datoshi]
    /// 08 : SHR [8 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : PUSHINT8 40 [1 datoshi]
    /// 0C : LDARG1 [2 datoshi]
    /// 0D : SUB [8 datoshi]
    /// 0E : PUSHINT8 3F [1 datoshi]
    /// 10 : AND [8 datoshi]
    /// 11 : SHL [8 datoshi]
    /// 12 : OR [8 datoshi]
    /// 13 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 24 : AND [8 datoshi]
    /// 25 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightULong")]
    public abstract BigInteger? RotateRightULong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkfkal4IHmfH5GokgL//wAAkUA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : PUSH15 [1 datoshi]
    /// 06 : AND [8 datoshi]
    /// 07 : SHR [8 datoshi]
    /// 08 : LDARG0 [2 datoshi]
    /// 09 : PUSH16 [1 datoshi]
    /// 0A : LDARG1 [2 datoshi]
    /// 0B : SUB [8 datoshi]
    /// 0C : PUSH15 [1 datoshi]
    /// 0D : AND [8 datoshi]
    /// 0E : SHL [8 datoshi]
    /// 0F : OR [8 datoshi]
    /// 10 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 15 : AND [8 datoshi]
    /// 16 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("rotateRightUShort")]
    public abstract BigInteger? RotateRightUShort(BigInteger? value, BigInteger? offset);

    #endregion
}
