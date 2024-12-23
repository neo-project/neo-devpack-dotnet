using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP37ClcAAnl4ShJNoVOiEsBAVwACeXhKEk2hU6ISwEBXAAJ5eEoSTaFTohLAQFcAAnl4ShJNoVOiEsBAVwACeXhKEk2hU6ISwEBXAAJ5eEoSTaFTohLAQFcAAnl4ShJNoVOiEsBAVwACeXhKEk2hU6ISwEBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAN4eXpLSzIDOlO6uUBXAAJ4eRAwBZoiBJqbSgL///9/MgM6QFcAAnh5EDAFmiIEmptKAH8yAzpAVwACeHkQMAWaIgSam0oB/38yAzpAVwACeHkQMAWaIgSam0oD/////////38yAzpAVwABeEoCAAAAgAMAAACAAAAAALskAzpAVwABeEoQAQABuyQDOkBXAAF4SgMAAAAAAAAAgAQAAAAAAAAAgAAAAAAAAAAAuyQDOkBXAAF4ShAEAAAAAAAAAAABAAAAAAAAALskAzpAVwABeEoQAgAAAQC7JAM6QFcAAXhKAQCAAgCAAAC7JAM6QFcAAXhKAIABgAC7JAM6QFcAAXgCAAAAgAL///9/SlFKUTADOlFKUUpRLAtFSlFKUTAIRUBTRUVAUEVAVwABeBAB/wBKUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUBXAAF4AwAAAAAAAACAA/////////9/SlFKUTADOlFKUUpRLAtFSlFKUTAIRUBTRUVAUEVAVwABeBAE//////////8AAAAAAAAAAEpRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQFcAAXgQAv//AABKUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUBXAAF4AIAAf0pRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQFcAAXgSoqpAVwABeBKiqkBXAAF4EqKqQFcAAXgSoqpAVwABeBKiqkBXAAF4EqKqQFcAAXgSoqpAVwABeBKiqkBXAAF4EqKxQFcAAXgSorFAVwABeBKisUBXAAF4EqKxQFcAAXgSorFAVwABeBKisUBXAAF4EqKxQFcAAXgSorFAVwABeBC1QFcAAXgQtUBXAAF4ELVAVwABeBC1QFcAAXgQuEBXAAF4ELhAVwABeBC4QFcAAXgQuEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAqBUUiCEqdkRAoBAlACEBXAAF4ShAuBUUQQBBQShAoCBGpUJwi90UAIFCfQFcAAXgQUEoQKAgRqVCcIvdFACBQn0BXAAF4ShAuBUUQQBBQShAoCBGpUJwi90UAQFCfQFcAAXhKEC4FRRBAEFBKECgIEalQnCL3RSBQn0BXAAF4EFBKECgIEalQnCL3RSBQn0BXAAF4EFBKECgIEalQnCL3RRhQn0BXAAF4ShAuBUUQQBBQShAoCBGpUJwi90UYUJ9AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwABeEoQLgM6ShAoDBCcS0upECz7Rp1AVwACeHkAH5FQA/////8AAAAAkVCoA/////8AAAAAkXgD/////wAAAACReQAgUJ8AH5GpkkoDAAAAgAAAAAAwDAMAAAAAAQAAAJ9AVwACeHkAH5FQA/////8AAAAAkVCoA/////8AAAAAkXgD/////wAAAACReQAgUJ8AH5GpkgP/////AAAAAJFAVwACeHkAP5FQBP//////////AAAAAAAAAACRUKgE//////////8AAAAAAAAAAJF4BP//////////AAAAAAAAAACReQBAUJ8AP5GpkkoEAAAAAAAAAIAAAAAAAAAAADAUBAAAAAAAAAAAAQAAAAAAAACfQFcAAnh5AD+RUAT//////////wAAAAAAAAAAkVCoBP//////////AAAAAAAAAACReAT//////////wAAAAAAAAAAkXkAQFCfAD+RqZIE//////////8AAAAAAAAAAJFAVwACeHkfkVAC//8AAJFQqAL//wAAkXgC//8AAJF5IFCfH5GpkkoCAIAAADAIAgAAAQCfQFcAAnh5H5FQAv//AACRUKgC//8AAJF4Av//AACReSBQnx+RqZIC//8AAJFAVwACeHkXkVAB/wCRUKgB/wCReAH/AJF5GFCfF5GpkgH/AJFAVwACeHkXkVAB/wCRUKgB/wCReAH/AJF5GFCfF5GpkkoBgAAwBgEAAZ9AVwACeHkAH5EAIKIAIFCfUAP/////AAAAAJFQqAP/////AAAAAJF4A/////8AAAAAkXkAIKIAIFCfACBQnwAfkamSSgMAAACAAAAAADAMAwAAAAABAAAAn0BXAAJ4eQAfkal4ACB5nwAfkaiSA/////8AAAAAkUBXAAJ4eQA/kQBAogBAUJ9QBP//////////AAAAAAAAAACRUKgE//////////8AAAAAAAAAAJF4BP//////////AAAAAAAAAACReQBAogBAUJ8AQFCfAD+RqZJKBAAAAAAAAACAAAAAAAAAAAAwFAQAAAAAAAAAAAEAAAAAAAAAn0BXAAJ4eQA/kal4AEB5nwA/kaiSBP//////////AAAAAAAAAACRQFcAAnh5H5EgoiBQn1AC//8AAJFQqAL//wAAkXgC//8AAJF5IKIgUJ8gUJ8fkamSSgIAgAAAMAgCAAABAJ9AVwACeHkfkal4IHmfH5GokgL//wAAkUBXAAJ4eReRqXgYeZ8XkaiSAf8AkUBXAAJ4eReRGKIYUJ9QAf8AkVCoAf8AkXgB/wCReRiiGFCfGFCfF5GpkkoBgAAwBgEAAZ9AVwABeAH/AJEQUEoQKAxKEZFRnlARqSL0RUBXAAF4Af8AkRBQShAoDEoRkVGeUBGpIvRFQFcAAXgC//8AAJEQUEoQKAxKEZFRnlARqSL0RUBXAAF4Av//AACREFBKECgMShGRUZ5QEaki9EVAVwABeAP/////AAAAAJEQUEoQKAxKEZFRnlARqSL0RUBXAAF4A/////8AAAAAkRBQShAoDEoRkVGeUBGpIvRFQFcAAXgE//////////8AAAAAAAAAAJEQUEoQKAxKEZFRnlARqSL0RUBXAAF4BP//////////AAAAAAAAAACREFBKECgMShGRUZ5QEaki9EVAVwABeEoCAAAAgAMAAACAAAAAALsmDgP/////AAAAAJEiRwxCVmFsdWUgb3V0IG9mIHJhbmdlLCBtdXN0IGJlIGJldHdlZW4gaW50Lk1pblZhbHVlIGFuZCBpbnQuTWF4VmFsdWUuOhBQShAoDEoRkVGeUBGpIvRFQFcAAXhKECoFRSIISp2RECgECUAIQIplBfI="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 03 [2 datoshi]
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
    /// Script: VwADeHl6S0syAzpTurlA
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 03 [2 datoshi]
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
    /// Script: VwADeHl6S0syAzpTurlA
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 03 [2 datoshi]
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
    /// Script: VwADeHl6S0syAzpTurlA
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 03 [2 datoshi]
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
    /// Script: VwADeHl6S0syAzpTurlA
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 03 [2 datoshi]
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
    /// Script: VwADeHl6S0syAzpTurlA
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 03 [2 datoshi]
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
    /// Script: VwADeHl6S0syAzpTurlA
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 03 [2 datoshi]
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
    /// Script: VwADeHl6S0syAzpTurlA
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 03 [2 datoshi]
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
    /// Script: VwADeHl6S0syAzpTurlA
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// JMPLE 03 [2 datoshi]
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
    /// Script: VwABeEoCAAAAgAMAAACAAAAAALskAzpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// PUSHINT64 0000008000000000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedInt")]
    public abstract BigInteger? CreateCheckedInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALskAzpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedLong")]
    public abstract BigInteger? CreateCheckedLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAgAGAALskAzpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 80 [1 datoshi]
    /// PUSHINT16 8000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createCheckedSbyte")]
    public abstract BigInteger? CreateCheckedSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoBAIACAIAAALskAzpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT16 0080 [1 datoshi]
    /// PUSHINT32 00800000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
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
    /// Script: VwABeBAB/wBKUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPLT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPGT 0B [2 datoshi]
    /// DROP [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPLT 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// REVERSE3 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingByte")]
    public abstract BigInteger? CreateSaturatingByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAC//8AAEpRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPLT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPGT 0B [2 datoshi]
    /// DROP [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPLT 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// REVERSE3 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingChar")]
    public abstract BigInteger? CreateSaturatingChar(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAIAAACAAv///39KUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPLT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPGT 0B [2 datoshi]
    /// DROP [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPLT 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// REVERSE3 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingInt")]
    public abstract BigInteger? CreateSaturatingInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAMAAAAAAAAAgAP/////////f0pRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPLT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPGT 0B [2 datoshi]
    /// DROP [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPLT 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// REVERSE3 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingLong")]
    public abstract BigInteger? CreateSaturatingLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeACAAH9KUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 80 [1 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPLT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPGT 0B [2 datoshi]
    /// DROP [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPLT 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// REVERSE3 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingSbyte")]
    public abstract BigInteger? CreateSaturatingSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAE//////////8AAAAAAAAAAEpRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPLT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPGT 0B [2 datoshi]
    /// DROP [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// JMPLT 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// REVERSE3 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("createSaturatingUlong")]
    public abstract BigInteger? CreateSaturatingUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
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
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
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
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
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
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
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
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
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
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
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
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
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
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
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
    [DisplayName("isEventUInt")]
    public abstract bool? IsEventUInt(BigInteger? value);

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
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2BigInteger")]
    public abstract bool? IsPow2BigInteger(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Byte")]
    public abstract bool? IsPow2Byte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Int")]
    public abstract bool? IsPow2Int(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Long")]
    public abstract bool? IsPow2Long(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Sbyte")]
    public abstract bool? IsPow2Sbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Short")]
    public abstract bool? IsPow2Short(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2UInt")]
    public abstract bool? IsPow2UInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Ulong")]
    public abstract bool? IsPow2Ulong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// DEC [4 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isPow2Ushort")]
    public abstract bool? IsPow2Ushort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBBQShAoCBGpUJwi90UYUJ9A
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
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
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFACBQn0A=
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
    /// PUSH0 [1 datoshi]
    /// JMPEQ 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
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
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFAEBQn0A=
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
    /// PUSH0 [1 datoshi]
    /// JMPEQ 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
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
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFGFCfQA==
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
    /// PUSH0 [1 datoshi]
    /// JMPEQ 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
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
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFIFCfQA==
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
    /// PUSH0 [1 datoshi]
    /// JMPEQ 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
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
    /// Script: VwABeBBQShAoCBGpUJwi90UAIFCfQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
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
    /// Script: VwABeBBQShAoCBGpUJwi90UgUJ9A
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 08 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP F7 [2 datoshi]
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
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// DEC [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Byte")]
    public abstract BigInteger? Log2Byte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// DEC [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Int")]
    public abstract BigInteger? Log2Int(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// DEC [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Long")]
    public abstract BigInteger? Log2Long(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// DEC [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Sbyte")]
    public abstract BigInteger? Log2Sbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// DEC [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Short")]
    public abstract BigInteger? Log2Short(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// DEC [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2UInt")]
    public abstract BigInteger? Log2UInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// INC [4 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// SHR [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGT FB [2 datoshi]
    /// NIP [2 datoshi]
    /// DEC [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("log2Ushort")]
    public abstract BigInteger? Log2Ushort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoCAAAAgAMAAACAAAAAALsmDgP/////AAAAAJEiRwxCVmFsdWUgb3V0IG9mIHJhbmdlLCBtdXN0IGJlIGJldHdlZW4gaW50Lk1pblZhbHVlIGFuZCBpbnQuTWF4VmFsdWUuOhBQShAoDEoRkVGeUBGpIvRFQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// PUSHINT64 0000008000000000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 0E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// JMP 47 [2 datoshi]
    /// PUSHDATA1 56616C7565206F7574206F662072616E67652C206D757374206265206265747765656E20696E742E4D696E56616C756520616E6420696E742E4D617856616C75652E [8 datoshi]
    /// THROW [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// AND [8 datoshi]
    /// ROT [2 datoshi]
    /// ADD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// JMP F4 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountBigInteger")]
    public abstract BigInteger? PopCountBigInteger(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAH/AJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// AND [8 datoshi]
    /// ROT [2 datoshi]
    /// ADD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// JMP F4 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountByte")]
    public abstract BigInteger? PopCountByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAP/////AAAAAJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// AND [8 datoshi]
    /// ROT [2 datoshi]
    /// ADD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// JMP F4 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountInt")]
    public abstract BigInteger? PopCountInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAT//////////wAAAAAAAAAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// AND [8 datoshi]
    /// ROT [2 datoshi]
    /// ADD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// JMP F4 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountLong")]
    public abstract BigInteger? PopCountLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAH/AJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// AND [8 datoshi]
    /// ROT [2 datoshi]
    /// ADD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// JMP F4 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountSByte")]
    public abstract BigInteger? PopCountSByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAL//wAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// AND [8 datoshi]
    /// ROT [2 datoshi]
    /// ADD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// JMP F4 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountShort")]
    public abstract BigInteger? PopCountShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAP/////AAAAAJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// AND [8 datoshi]
    /// ROT [2 datoshi]
    /// ADD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// JMP F4 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountUInt")]
    public abstract BigInteger? PopCountUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAT//////////wAAAAAAAAAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// AND [8 datoshi]
    /// ROT [2 datoshi]
    /// ADD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// JMP F4 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountULong")]
    public abstract BigInteger? PopCountULong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAL//wAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// SWAP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPEQ 0C [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// AND [8 datoshi]
    /// ROT [2 datoshi]
    /// ADD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// SHR [8 datoshi]
    /// JMP F4 [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("popCountUShort")]
    public abstract BigInteger? PopCountUShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkXkVAB/wCRUKgB/wCReAH/AJF5GFCfF5GpkgH/AJFA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH8 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
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
    /// Script: VwACeHkAH5FQA/////8AAAAAkVCoA/////8AAAAAkXgD/////wAAAACReQAgUJ8AH5GpkkoDAAAAgAAAAAAwDAMAAAAAAQAAAJ9A
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
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
    /// Script: VwACeHkAP5FQBP//////////AAAAAAAAAACRUKgE//////////8AAAAAAAAAAJF4BP//////////AAAAAAAAAACReQBAUJ8AP5GpkkoEAAAAAAAAAIAAAAAAAAAAADAUBAAAAAAAAAAAAQAAAAAAAACfQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 40 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
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
    /// Script: VwACeHkXkVAB/wCRUKgB/wCReAH/AJF5GFCfF5GpkkoBgAAwBgEAAZ9A
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH8 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
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
    /// Script: VwACeHkfkVAC//8AAJFQqAL//wAAkXgC//8AAJF5IFCfH5GpkkoCAIAAADAIAgAAAQCfQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH16 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
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
    /// Script: VwACeHkAH5FQA/////8AAAAAkVCoA/////8AAAAAkXgD/////wAAAACReQAgUJ8AH5GpkgP/////AAAAAJFA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
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
    /// Script: VwACeHkAP5FQBP//////////AAAAAAAAAACRUKgE//////////8AAAAAAAAAAJF4BP//////////AAAAAAAAAACReQBAUJ8AP5GpkgT//////////wAAAAAAAAAAkUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 40 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
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
    /// Script: VwACeHkfkVAC//8AAJFQqAL//wAAkXgC//8AAJF5IFCfH5GpkgL//wAAkUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH16 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
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
    /// Script: VwACeHkXkal4GHmfF5GokgH/AJFA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH8 [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// SUB [8 datoshi]
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
    /// Script: VwACeHkAH5EAIKIAIFCfUAP/////AAAAAJFQqAP/////AAAAAJF4A/////8AAAAAkXkAIKIAIFCfACBQnwAfkamSSgMAAACAAAAAADAMAwAAAAABAAAAn0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// MOD [8 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// MOD [8 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
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
    /// Script: VwACeHkAP5EAQKIAQFCfUAT//////////wAAAAAAAAAAkVCoBP//////////AAAAAAAAAACReAT//////////wAAAAAAAAAAkXkAQKIAQFCfAEBQnwA/kamSSgQAAAAAAAAAgAAAAAAAAAAAMBQEAAAAAAAAAAABAAAAAAAAAJ9A
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// PUSHINT8 40 [1 datoshi]
    /// MOD [8 datoshi]
    /// PUSHINT8 40 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 40 [1 datoshi]
    /// MOD [8 datoshi]
    /// PUSHINT8 40 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// PUSHINT8 40 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
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
    /// Script: VwACeHkXkRiiGFCfUAH/AJFQqAH/AJF4Af8AkXkYohhQnxhQnxeRqZJKAYAAMAYBAAGfQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH8 [1 datoshi]
    /// MOD [8 datoshi]
    /// PUSH8 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH8 [1 datoshi]
    /// MOD [8 datoshi]
    /// PUSH8 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// PUSH8 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// PUSH7 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
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
    /// Script: VwACeHkfkSCiIFCfUAL//wAAkVCoAv//AACReAL//wAAkXkgoiBQnyBQnx+RqZJKAgCAAAAwCAIAAAEAn0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH16 [1 datoshi]
    /// MOD [8 datoshi]
    /// PUSH16 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// SWAP [2 datoshi]
    /// SHL [8 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// AND [8 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH16 [1 datoshi]
    /// MOD [8 datoshi]
    /// PUSH16 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// PUSH16 [1 datoshi]
    /// SWAP [2 datoshi]
    /// SUB [8 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// OR [8 datoshi]
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
    /// Script: VwACeHkAH5GpeAAgeZ8AH5GokgP/////AAAAAJFA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 1F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// SUB [8 datoshi]
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
    /// Script: VwACeHkAP5GpeABAeZ8AP5GokgT//////////wAAAAAAAAAAkUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 40 [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// SUB [8 datoshi]
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
    /// Script: VwACeHkfkal4IHmfH5GokgL//wAAkUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// SHR [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH16 [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// SUB [8 datoshi]
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
