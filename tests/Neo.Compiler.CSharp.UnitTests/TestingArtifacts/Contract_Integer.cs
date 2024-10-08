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
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG2
    /// 06 : OpCode.OVER
    /// 07 : OpCode.OVER
    /// 08 : OpCode.JMPLE 03
    /// 0A : OpCode.THROW
    /// 0B : OpCode.REVERSE3
    /// 0C : OpCode.MAX
    /// 0D : OpCode.MIN
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("clampBigInteger")]
    public abstract BigInteger? ClampBigInteger(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG2
    /// 06 : OpCode.OVER
    /// 07 : OpCode.OVER
    /// 08 : OpCode.JMPLE 03
    /// 0A : OpCode.THROW
    /// 0B : OpCode.REVERSE3
    /// 0C : OpCode.MAX
    /// 0D : OpCode.MIN
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("clampByte")]
    public abstract BigInteger? ClampByte(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG2
    /// 06 : OpCode.OVER
    /// 07 : OpCode.OVER
    /// 08 : OpCode.JMPLE 03
    /// 0A : OpCode.THROW
    /// 0B : OpCode.REVERSE3
    /// 0C : OpCode.MAX
    /// 0D : OpCode.MIN
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("clampInt")]
    public abstract BigInteger? ClampInt(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG2
    /// 06 : OpCode.OVER
    /// 07 : OpCode.OVER
    /// 08 : OpCode.JMPLE 03
    /// 0A : OpCode.THROW
    /// 0B : OpCode.REVERSE3
    /// 0C : OpCode.MAX
    /// 0D : OpCode.MIN
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("clampLong")]
    public abstract BigInteger? ClampLong(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG2
    /// 06 : OpCode.OVER
    /// 07 : OpCode.OVER
    /// 08 : OpCode.JMPLE 03
    /// 0A : OpCode.THROW
    /// 0B : OpCode.REVERSE3
    /// 0C : OpCode.MAX
    /// 0D : OpCode.MIN
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("clampSByte")]
    public abstract BigInteger? ClampSByte(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG2
    /// 06 : OpCode.OVER
    /// 07 : OpCode.OVER
    /// 08 : OpCode.JMPLE 03
    /// 0A : OpCode.THROW
    /// 0B : OpCode.REVERSE3
    /// 0C : OpCode.MAX
    /// 0D : OpCode.MIN
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("clampShort")]
    public abstract BigInteger? ClampShort(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG2
    /// 06 : OpCode.OVER
    /// 07 : OpCode.OVER
    /// 08 : OpCode.JMPLE 03
    /// 0A : OpCode.THROW
    /// 0B : OpCode.REVERSE3
    /// 0C : OpCode.MAX
    /// 0D : OpCode.MIN
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("clampUInt")]
    public abstract BigInteger? ClampUInt(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG2
    /// 06 : OpCode.OVER
    /// 07 : OpCode.OVER
    /// 08 : OpCode.JMPLE 03
    /// 0A : OpCode.THROW
    /// 0B : OpCode.REVERSE3
    /// 0C : OpCode.MAX
    /// 0D : OpCode.MIN
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("clampULong")]
    public abstract BigInteger? ClampULong(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG2
    /// 06 : OpCode.OVER
    /// 07 : OpCode.OVER
    /// 08 : OpCode.JMPLE 03
    /// 0A : OpCode.THROW
    /// 0B : OpCode.REVERSE3
    /// 0C : OpCode.MAX
    /// 0D : OpCode.MIN
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("clampUShort")]
    public abstract BigInteger? ClampUShort(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oC////fzIDOkA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPLT 05
    /// 08 : OpCode.ABS
    /// 09 : OpCode.JMP 04
    /// 0B : OpCode.ABS
    /// 0C : OpCode.NEGATE
    /// 0D : OpCode.DUP
    /// 0E : OpCode.PUSHINT32 FFFFFF7F
    /// 13 : OpCode.JMPLE 03
    /// 15 : OpCode.THROW
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("copySignInt")]
    public abstract BigInteger? CopySignInt(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oD/////////38yAzpA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPLT 05
    /// 08 : OpCode.ABS
    /// 09 : OpCode.JMP 04
    /// 0B : OpCode.ABS
    /// 0C : OpCode.NEGATE
    /// 0D : OpCode.DUP
    /// 0E : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 17 : OpCode.JMPLE 03
    /// 19 : OpCode.THROW
    /// 1A : OpCode.RET
    /// </remarks>
    [DisplayName("copySignLong")]
    public abstract BigInteger? CopySignLong(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oAfzIDOkA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPLT 05
    /// 08 : OpCode.ABS
    /// 09 : OpCode.JMP 04
    /// 0B : OpCode.ABS
    /// 0C : OpCode.NEGATE
    /// 0D : OpCode.DUP
    /// 0E : OpCode.PUSHINT8 7F
    /// 10 : OpCode.JMPLE 03
    /// 12 : OpCode.THROW
    /// 13 : OpCode.RET
    /// </remarks>
    [DisplayName("copySignSbyte")]
    public abstract BigInteger? CopySignSbyte(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oB/38yAzpA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPLT 05
    /// 08 : OpCode.ABS
    /// 09 : OpCode.JMP 04
    /// 0B : OpCode.ABS
    /// 0C : OpCode.NEGATE
    /// 0D : OpCode.DUP
    /// 0E : OpCode.PUSHINT16 FF7F
    /// 11 : OpCode.JMPLE 03
    /// 13 : OpCode.THROW
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("copySignShort")]
    public abstract BigInteger? CopySignShort(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQAQABuyQDOkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.PUSHINT16 0001
    /// 09 : OpCode.WITHIN
    /// 0A : OpCode.JMPIF 03
    /// 0C : OpCode.THROW
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedByte")]
    public abstract BigInteger? CreateCheckedByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQAgAAAQC7JAM6QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.PUSHINT32 00000100
    /// 0B : OpCode.WITHIN
    /// 0C : OpCode.JMPIF 03
    /// 0E : OpCode.THROW
    /// 0F : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedChar")]
    public abstract BigInteger? CreateCheckedChar(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoCAAAAgAMAAACAAAAAALskAzpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT32 00000080
    /// 0A : OpCode.PUSHINT64 0000008000000000
    /// 13 : OpCode.WITHIN
    /// 14 : OpCode.JMPIF 03
    /// 16 : OpCode.THROW
    /// 17 : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedInt")]
    public abstract BigInteger? CreateCheckedInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALskAzpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT64 0000000000000080
    /// 0E : OpCode.PUSHINT128 00000000000000800000000000000000
    /// 1F : OpCode.WITHIN
    /// 20 : OpCode.JMPIF 03
    /// 22 : OpCode.THROW
    /// 23 : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedLong")]
    public abstract BigInteger? CreateCheckedLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAgAGAALskAzpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT8 80
    /// 07 : OpCode.PUSHINT16 8000
    /// 0A : OpCode.WITHIN
    /// 0B : OpCode.JMPIF 03
    /// 0D : OpCode.THROW
    /// 0E : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedSbyte")]
    public abstract BigInteger? CreateCheckedSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoBAIACAIAAALskAzpA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT16 0080
    /// 08 : OpCode.PUSHINT32 00800000
    /// 0D : OpCode.WITHIN
    /// 0E : OpCode.JMPIF 03
    /// 10 : OpCode.THROW
    /// 11 : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedShort")]
    public abstract BigInteger? CreateCheckedShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQBAAAAAAAAAAAAQAAAAAAAAC7JAM6QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.PUSHINT128 00000000000000000100000000000000
    /// 17 : OpCode.WITHIN
    /// 18 : OpCode.JMPIF 03
    /// 1A : OpCode.THROW
    /// 1B : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedUlong")]
    public abstract BigInteger? CreateCheckedUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAB/wBKUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.PUSHINT16 FF00
    /// 08 : OpCode.DUP
    /// 09 : OpCode.ROT
    /// 0A : OpCode.DUP
    /// 0B : OpCode.ROT
    /// 0C : OpCode.JMPLT 03
    /// 0E : OpCode.THROW
    /// 0F : OpCode.ROT
    /// 10 : OpCode.DUP
    /// 11 : OpCode.ROT
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ROT
    /// 14 : OpCode.JMPGT 0B
    /// 16 : OpCode.DROP
    /// 17 : OpCode.DUP
    /// 18 : OpCode.ROT
    /// 19 : OpCode.DUP
    /// 1A : OpCode.ROT
    /// 1B : OpCode.JMPLT 08
    /// 1D : OpCode.DROP
    /// 1E : OpCode.RET
    /// 1F : OpCode.REVERSE3
    /// 20 : OpCode.DROP
    /// 21 : OpCode.DROP
    /// 22 : OpCode.RET
    /// 23 : OpCode.SWAP
    /// 24 : OpCode.DROP
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("createSaturatingByte")]
    public abstract BigInteger? CreateSaturatingByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAC//8AAEpRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.PUSHINT32 FFFF0000
    /// 0A : OpCode.DUP
    /// 0B : OpCode.ROT
    /// 0C : OpCode.DUP
    /// 0D : OpCode.ROT
    /// 0E : OpCode.JMPLT 03
    /// 10 : OpCode.THROW
    /// 11 : OpCode.ROT
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ROT
    /// 14 : OpCode.DUP
    /// 15 : OpCode.ROT
    /// 16 : OpCode.JMPGT 0B
    /// 18 : OpCode.DROP
    /// 19 : OpCode.DUP
    /// 1A : OpCode.ROT
    /// 1B : OpCode.DUP
    /// 1C : OpCode.ROT
    /// 1D : OpCode.JMPLT 08
    /// 1F : OpCode.DROP
    /// 20 : OpCode.RET
    /// 21 : OpCode.REVERSE3
    /// 22 : OpCode.DROP
    /// 23 : OpCode.DROP
    /// 24 : OpCode.RET
    /// 25 : OpCode.SWAP
    /// 26 : OpCode.DROP
    /// 27 : OpCode.RET
    /// </remarks>
    [DisplayName("createSaturatingChar")]
    public abstract BigInteger? CreateSaturatingChar(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAIAAACAAv///39KUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT32 00000080
    /// 09 : OpCode.PUSHINT32 FFFFFF7F
    /// 0E : OpCode.DUP
    /// 0F : OpCode.ROT
    /// 10 : OpCode.DUP
    /// 11 : OpCode.ROT
    /// 12 : OpCode.JMPLT 03
    /// 14 : OpCode.THROW
    /// 15 : OpCode.ROT
    /// 16 : OpCode.DUP
    /// 17 : OpCode.ROT
    /// 18 : OpCode.DUP
    /// 19 : OpCode.ROT
    /// 1A : OpCode.JMPGT 0B
    /// 1C : OpCode.DROP
    /// 1D : OpCode.DUP
    /// 1E : OpCode.ROT
    /// 1F : OpCode.DUP
    /// 20 : OpCode.ROT
    /// 21 : OpCode.JMPLT 08
    /// 23 : OpCode.DROP
    /// 24 : OpCode.RET
    /// 25 : OpCode.REVERSE3
    /// 26 : OpCode.DROP
    /// 27 : OpCode.DROP
    /// 28 : OpCode.RET
    /// 29 : OpCode.SWAP
    /// 2A : OpCode.DROP
    /// 2B : OpCode.RET
    /// </remarks>
    [DisplayName("createSaturatingInt")]
    public abstract BigInteger? CreateSaturatingInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAMAAAAAAAAAgAP/////////f0pRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT64 0000000000000080
    /// 0D : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 16 : OpCode.DUP
    /// 17 : OpCode.ROT
    /// 18 : OpCode.DUP
    /// 19 : OpCode.ROT
    /// 1A : OpCode.JMPLT 03
    /// 1C : OpCode.THROW
    /// 1D : OpCode.ROT
    /// 1E : OpCode.DUP
    /// 1F : OpCode.ROT
    /// 20 : OpCode.DUP
    /// 21 : OpCode.ROT
    /// 22 : OpCode.JMPGT 0B
    /// 24 : OpCode.DROP
    /// 25 : OpCode.DUP
    /// 26 : OpCode.ROT
    /// 27 : OpCode.DUP
    /// 28 : OpCode.ROT
    /// 29 : OpCode.JMPLT 08
    /// 2B : OpCode.DROP
    /// 2C : OpCode.RET
    /// 2D : OpCode.REVERSE3
    /// 2E : OpCode.DROP
    /// 2F : OpCode.DROP
    /// 30 : OpCode.RET
    /// 31 : OpCode.SWAP
    /// 32 : OpCode.DROP
    /// 33 : OpCode.RET
    /// </remarks>
    [DisplayName("createSaturatingLong")]
    public abstract BigInteger? CreateSaturatingLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeACAAH9KUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT8 80
    /// 06 : OpCode.PUSHINT8 7F
    /// 08 : OpCode.DUP
    /// 09 : OpCode.ROT
    /// 0A : OpCode.DUP
    /// 0B : OpCode.ROT
    /// 0C : OpCode.JMPLT 03
    /// 0E : OpCode.THROW
    /// 0F : OpCode.ROT
    /// 10 : OpCode.DUP
    /// 11 : OpCode.ROT
    /// 12 : OpCode.DUP
    /// 13 : OpCode.ROT
    /// 14 : OpCode.JMPGT 0B
    /// 16 : OpCode.DROP
    /// 17 : OpCode.DUP
    /// 18 : OpCode.ROT
    /// 19 : OpCode.DUP
    /// 1A : OpCode.ROT
    /// 1B : OpCode.JMPLT 08
    /// 1D : OpCode.DROP
    /// 1E : OpCode.RET
    /// 1F : OpCode.REVERSE3
    /// 20 : OpCode.DROP
    /// 21 : OpCode.DROP
    /// 22 : OpCode.RET
    /// 23 : OpCode.SWAP
    /// 24 : OpCode.DROP
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("createSaturatingSbyte")]
    public abstract BigInteger? CreateSaturatingSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAE//////////8AAAAAAAAAAEpRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 16 : OpCode.DUP
    /// 17 : OpCode.ROT
    /// 18 : OpCode.DUP
    /// 19 : OpCode.ROT
    /// 1A : OpCode.JMPLT 03
    /// 1C : OpCode.THROW
    /// 1D : OpCode.ROT
    /// 1E : OpCode.DUP
    /// 1F : OpCode.ROT
    /// 20 : OpCode.DUP
    /// 21 : OpCode.ROT
    /// 22 : OpCode.JMPGT 0B
    /// 24 : OpCode.DROP
    /// 25 : OpCode.DUP
    /// 26 : OpCode.ROT
    /// 27 : OpCode.DUP
    /// 28 : OpCode.ROT
    /// 29 : OpCode.JMPLT 08
    /// 2B : OpCode.DROP
    /// 2C : OpCode.RET
    /// 2D : OpCode.REVERSE3
    /// 2E : OpCode.DROP
    /// 2F : OpCode.DROP
    /// 30 : OpCode.RET
    /// 31 : OpCode.SWAP
    /// 32 : OpCode.DROP
    /// 33 : OpCode.RET
    /// </remarks>
    [DisplayName("createSaturatingUlong")]
    public abstract BigInteger? CreateSaturatingUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH2
    /// 07 : OpCode.PICK
    /// 08 : OpCode.DIV
    /// 09 : OpCode.REVERSE3
    /// 0A : OpCode.MOD
    /// 0B : OpCode.PUSH2
    /// 0C : OpCode.PACK
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemByte")]
    public abstract IList<object>? DivRemByte(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH2
    /// 07 : OpCode.PICK
    /// 08 : OpCode.DIV
    /// 09 : OpCode.REVERSE3
    /// 0A : OpCode.MOD
    /// 0B : OpCode.PUSH2
    /// 0C : OpCode.PACK
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemInt")]
    public abstract IList<object>? DivRemInt(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH2
    /// 07 : OpCode.PICK
    /// 08 : OpCode.DIV
    /// 09 : OpCode.REVERSE3
    /// 0A : OpCode.MOD
    /// 0B : OpCode.PUSH2
    /// 0C : OpCode.PACK
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemLong")]
    public abstract IList<object>? DivRemLong(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH2
    /// 07 : OpCode.PICK
    /// 08 : OpCode.DIV
    /// 09 : OpCode.REVERSE3
    /// 0A : OpCode.MOD
    /// 0B : OpCode.PUSH2
    /// 0C : OpCode.PACK
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemSbyte")]
    public abstract IList<object>? DivRemSbyte(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH2
    /// 07 : OpCode.PICK
    /// 08 : OpCode.DIV
    /// 09 : OpCode.REVERSE3
    /// 0A : OpCode.MOD
    /// 0B : OpCode.PUSH2
    /// 0C : OpCode.PACK
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemShort")]
    public abstract IList<object>? DivRemShort(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH2
    /// 07 : OpCode.PICK
    /// 08 : OpCode.DIV
    /// 09 : OpCode.REVERSE3
    /// 0A : OpCode.MOD
    /// 0B : OpCode.PUSH2
    /// 0C : OpCode.PACK
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemUint")]
    public abstract IList<object>? DivRemUint(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH2
    /// 07 : OpCode.PICK
    /// 08 : OpCode.DIV
    /// 09 : OpCode.REVERSE3
    /// 0A : OpCode.MOD
    /// 0B : OpCode.PUSH2
    /// 0C : OpCode.PACK
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemUlong")]
    public abstract IList<object>? DivRemUlong(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH2
    /// 07 : OpCode.PICK
    /// 08 : OpCode.DIV
    /// 09 : OpCode.REVERSE3
    /// 0A : OpCode.MOD
    /// 0B : OpCode.PUSH2
    /// 0C : OpCode.PACK
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemUshort")]
    public abstract IList<object>? DivRemUshort(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NOT
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenByte")]
    public abstract bool? IsEvenByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NOT
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenIntegerInt")]
    public abstract bool? IsEvenIntegerInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NOT
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenLong")]
    public abstract bool? IsEvenLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NOT
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenSbyte")]
    public abstract bool? IsEvenSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NOT
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenShort")]
    public abstract bool? IsEvenShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NOT
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isEventUInt")]
    public abstract bool? IsEventUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NOT
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenUlong")]
    public abstract bool? IsEvenUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NOT
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenUshort")]
    public abstract bool? IsEvenUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.LT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("isNegativeInt")]
    public abstract bool? IsNegativeInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.LT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("isNegativeLong")]
    public abstract bool? IsNegativeLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.LT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("isNegativeSbyte")]
    public abstract bool? IsNegativeSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.LT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("isNegativeShort")]
    public abstract bool? IsNegativeShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NZ
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddByte")]
    public abstract bool? IsOddByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NZ
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddIntegerInt")]
    public abstract bool? IsOddIntegerInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NZ
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddLong")]
    public abstract bool? IsOddLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NZ
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddSbyte")]
    public abstract bool? IsOddSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NZ
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddShort")]
    public abstract bool? IsOddShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NZ
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddUInt")]
    public abstract bool? IsOddUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NZ
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddUlong")]
    public abstract bool? IsOddUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.MOD
    /// 06 : OpCode.NZ
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddUshort")]
    public abstract bool? IsOddUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.GE
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("isPositiveInt")]
    public abstract bool? IsPositiveInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.GE
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("isPositiveLong")]
    public abstract bool? IsPositiveLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.GE
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("isPositiveSbyte")]
    public abstract bool? IsPositiveSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.GE
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("isPositiveShort")]
    public abstract bool? IsPositiveShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPNE 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.JMP 08
    /// 0B : OpCode.DUP
    /// 0C : OpCode.DEC
    /// 0D : OpCode.AND
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.JMPEQ 04
    /// 11 : OpCode.PUSHF
    /// 12 : OpCode.RET
    /// 13 : OpCode.PUSHT
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2BigInteger")]
    public abstract bool? IsPow2BigInteger(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPNE 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.JMP 08
    /// 0B : OpCode.DUP
    /// 0C : OpCode.DEC
    /// 0D : OpCode.AND
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.JMPEQ 04
    /// 11 : OpCode.PUSHF
    /// 12 : OpCode.RET
    /// 13 : OpCode.PUSHT
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Byte")]
    public abstract bool? IsPow2Byte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPNE 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.JMP 08
    /// 0B : OpCode.DUP
    /// 0C : OpCode.DEC
    /// 0D : OpCode.AND
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.JMPEQ 04
    /// 11 : OpCode.PUSHF
    /// 12 : OpCode.RET
    /// 13 : OpCode.PUSHT
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Int")]
    public abstract bool? IsPow2Int(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPNE 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.JMP 08
    /// 0B : OpCode.DUP
    /// 0C : OpCode.DEC
    /// 0D : OpCode.AND
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.JMPEQ 04
    /// 11 : OpCode.PUSHF
    /// 12 : OpCode.RET
    /// 13 : OpCode.PUSHT
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Long")]
    public abstract bool? IsPow2Long(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPNE 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.JMP 08
    /// 0B : OpCode.DUP
    /// 0C : OpCode.DEC
    /// 0D : OpCode.AND
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.JMPEQ 04
    /// 11 : OpCode.PUSHF
    /// 12 : OpCode.RET
    /// 13 : OpCode.PUSHT
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Sbyte")]
    public abstract bool? IsPow2Sbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPNE 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.JMP 08
    /// 0B : OpCode.DUP
    /// 0C : OpCode.DEC
    /// 0D : OpCode.AND
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.JMPEQ 04
    /// 11 : OpCode.PUSHF
    /// 12 : OpCode.RET
    /// 13 : OpCode.PUSHT
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Short")]
    public abstract bool? IsPow2Short(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPNE 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.JMP 08
    /// 0B : OpCode.DUP
    /// 0C : OpCode.DEC
    /// 0D : OpCode.AND
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.JMPEQ 04
    /// 11 : OpCode.PUSHF
    /// 12 : OpCode.RET
    /// 13 : OpCode.PUSHT
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2UInt")]
    public abstract bool? IsPow2UInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPNE 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.JMP 08
    /// 0B : OpCode.DUP
    /// 0C : OpCode.DEC
    /// 0D : OpCode.AND
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.JMPEQ 04
    /// 11 : OpCode.PUSHF
    /// 12 : OpCode.RET
    /// 13 : OpCode.PUSHT
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Ulong")]
    public abstract bool? IsPow2Ulong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPNE 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.JMP 08
    /// 0B : OpCode.DUP
    /// 0C : OpCode.DEC
    /// 0D : OpCode.AND
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.JMPEQ 04
    /// 11 : OpCode.PUSHF
    /// 12 : OpCode.RET
    /// 13 : OpCode.PUSHT
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Ushort")]
    public abstract bool? IsPow2Ushort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBBQShAoCBGpUJwi90UYUJ9A
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.SWAP
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSH0
    /// 08 : OpCode.JMPEQ 08
    /// 0A : OpCode.PUSH1
    /// 0B : OpCode.SHR
    /// 0C : OpCode.SWAP
    /// 0D : OpCode.INC
    /// 0E : OpCode.JMP F7
    /// 10 : OpCode.DROP
    /// 11 : OpCode.PUSH8
    /// 12 : OpCode.SWAP
    /// 13 : OpCode.SUB
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountByte")]
    public abstract BigInteger? LeadingZeroCountByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFACBQn0A=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.RET
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.SWAP
    /// 0D : OpCode.DUP
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.JMPEQ 08
    /// 11 : OpCode.PUSH1
    /// 12 : OpCode.SHR
    /// 13 : OpCode.SWAP
    /// 14 : OpCode.INC
    /// 15 : OpCode.JMP F7
    /// 17 : OpCode.DROP
    /// 18 : OpCode.PUSHINT8 20
    /// 1A : OpCode.SWAP
    /// 1B : OpCode.SUB
    /// 1C : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountInt")]
    public abstract BigInteger? LeadingZeroCountInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFAEBQn0A=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.RET
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.SWAP
    /// 0D : OpCode.DUP
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.JMPEQ 08
    /// 11 : OpCode.PUSH1
    /// 12 : OpCode.SHR
    /// 13 : OpCode.SWAP
    /// 14 : OpCode.INC
    /// 15 : OpCode.JMP F7
    /// 17 : OpCode.DROP
    /// 18 : OpCode.PUSHINT8 40
    /// 1A : OpCode.SWAP
    /// 1B : OpCode.SUB
    /// 1C : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountLong")]
    public abstract BigInteger? LeadingZeroCountLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFGFCfQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.RET
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.SWAP
    /// 0D : OpCode.DUP
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.JMPEQ 08
    /// 11 : OpCode.PUSH1
    /// 12 : OpCode.SHR
    /// 13 : OpCode.SWAP
    /// 14 : OpCode.INC
    /// 15 : OpCode.JMP F7
    /// 17 : OpCode.DROP
    /// 18 : OpCode.PUSH8
    /// 19 : OpCode.SWAP
    /// 1A : OpCode.SUB
    /// 1B : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountSbyte")]
    public abstract BigInteger? LeadingZeroCountSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFIFCfQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 05
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.RET
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.SWAP
    /// 0D : OpCode.DUP
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.JMPEQ 08
    /// 11 : OpCode.PUSH1
    /// 12 : OpCode.SHR
    /// 13 : OpCode.SWAP
    /// 14 : OpCode.INC
    /// 15 : OpCode.JMP F7
    /// 17 : OpCode.DROP
    /// 18 : OpCode.PUSH16
    /// 19 : OpCode.SWAP
    /// 1A : OpCode.SUB
    /// 1B : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountShort")]
    public abstract BigInteger? LeadingZeroCountShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBBQShAoCBGpUJwi90UAIFCfQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.SWAP
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSH0
    /// 08 : OpCode.JMPEQ 08
    /// 0A : OpCode.PUSH1
    /// 0B : OpCode.SHR
    /// 0C : OpCode.SWAP
    /// 0D : OpCode.INC
    /// 0E : OpCode.JMP F7
    /// 10 : OpCode.DROP
    /// 11 : OpCode.PUSHINT8 20
    /// 13 : OpCode.SWAP
    /// 14 : OpCode.SUB
    /// 15 : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountUInt")]
    public abstract BigInteger? LeadingZeroCountUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBBQShAoCBGpUJwi90UgUJ9A
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.SWAP
    /// 06 : OpCode.DUP
    /// 07 : OpCode.PUSH0
    /// 08 : OpCode.JMPEQ 08
    /// 0A : OpCode.PUSH1
    /// 0B : OpCode.SHR
    /// 0C : OpCode.SWAP
    /// 0D : OpCode.INC
    /// 0E : OpCode.JMP F7
    /// 10 : OpCode.DROP
    /// 11 : OpCode.PUSH16
    /// 12 : OpCode.SWAP
    /// 13 : OpCode.SUB
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountUshort")]
    public abstract BigInteger? LeadingZeroCountUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 03
    /// 08 : OpCode.THROW
    /// 09 : OpCode.DUP
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.JMPEQ 0C
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.INC
    /// 0F : OpCode.OVER
    /// 10 : OpCode.OVER
    /// 11 : OpCode.SHR
    /// 12 : OpCode.PUSH0
    /// 13 : OpCode.JMPGT FB
    /// 15 : OpCode.NIP
    /// 16 : OpCode.DEC
    /// 17 : OpCode.RET
    /// </remarks>
    [DisplayName("log2Byte")]
    public abstract BigInteger? Log2Byte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 03
    /// 08 : OpCode.THROW
    /// 09 : OpCode.DUP
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.JMPEQ 0C
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.INC
    /// 0F : OpCode.OVER
    /// 10 : OpCode.OVER
    /// 11 : OpCode.SHR
    /// 12 : OpCode.PUSH0
    /// 13 : OpCode.JMPGT FB
    /// 15 : OpCode.NIP
    /// 16 : OpCode.DEC
    /// 17 : OpCode.RET
    /// </remarks>
    [DisplayName("log2Int")]
    public abstract BigInteger? Log2Int(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 03
    /// 08 : OpCode.THROW
    /// 09 : OpCode.DUP
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.JMPEQ 0C
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.INC
    /// 0F : OpCode.OVER
    /// 10 : OpCode.OVER
    /// 11 : OpCode.SHR
    /// 12 : OpCode.PUSH0
    /// 13 : OpCode.JMPGT FB
    /// 15 : OpCode.NIP
    /// 16 : OpCode.DEC
    /// 17 : OpCode.RET
    /// </remarks>
    [DisplayName("log2Long")]
    public abstract BigInteger? Log2Long(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 03
    /// 08 : OpCode.THROW
    /// 09 : OpCode.DUP
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.JMPEQ 0C
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.INC
    /// 0F : OpCode.OVER
    /// 10 : OpCode.OVER
    /// 11 : OpCode.SHR
    /// 12 : OpCode.PUSH0
    /// 13 : OpCode.JMPGT FB
    /// 15 : OpCode.NIP
    /// 16 : OpCode.DEC
    /// 17 : OpCode.RET
    /// </remarks>
    [DisplayName("log2Sbyte")]
    public abstract BigInteger? Log2Sbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 03
    /// 08 : OpCode.THROW
    /// 09 : OpCode.DUP
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.JMPEQ 0C
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.INC
    /// 0F : OpCode.OVER
    /// 10 : OpCode.OVER
    /// 11 : OpCode.SHR
    /// 12 : OpCode.PUSH0
    /// 13 : OpCode.JMPGT FB
    /// 15 : OpCode.NIP
    /// 16 : OpCode.DEC
    /// 17 : OpCode.RET
    /// </remarks>
    [DisplayName("log2Short")]
    public abstract BigInteger? Log2Short(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 03
    /// 08 : OpCode.THROW
    /// 09 : OpCode.DUP
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.JMPEQ 0C
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.INC
    /// 0F : OpCode.OVER
    /// 10 : OpCode.OVER
    /// 11 : OpCode.SHR
    /// 12 : OpCode.PUSH0
    /// 13 : OpCode.JMPGT FB
    /// 15 : OpCode.NIP
    /// 16 : OpCode.DEC
    /// 17 : OpCode.RET
    /// </remarks>
    [DisplayName("log2UInt")]
    public abstract BigInteger? Log2UInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.JMPGE 03
    /// 08 : OpCode.THROW
    /// 09 : OpCode.DUP
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.JMPEQ 0C
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.INC
    /// 0F : OpCode.OVER
    /// 10 : OpCode.OVER
    /// 11 : OpCode.SHR
    /// 12 : OpCode.PUSH0
    /// 13 : OpCode.JMPGT FB
    /// 15 : OpCode.NIP
    /// 16 : OpCode.DEC
    /// 17 : OpCode.RET
    /// </remarks>
    [DisplayName("log2Ushort")]
    public abstract BigInteger? Log2Ushort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoCAAAAgAMAAACAAAAAALsmDgP/////AAAAAJEiRwxWYWx1ZSBvdXQgb2YgcmFuZ2UsIG11c3QgYmUgYmV0d2VlbiBpbnQuTWluVmFsdWUgYW5kIGludC5NYXhWYWx1ZS46EFBKECgMShGRUZ5QEaki9EVA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT32 00000080
    /// 0A : OpCode.PUSHINT64 0000008000000000
    /// 13 : OpCode.WITHIN
    /// 14 : OpCode.JMPIFNOT 0E
    /// 16 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1F : OpCode.AND
    /// 20 : OpCode.JMP 47
    /// 22 : OpCode.PUSHDATA1 56616C7565206F7574206F662072616E67652C206D757374206265206265747765656E20696E742E4D696E56616C756520616E6420696E742E4D617856616C75652E
    /// 66 : OpCode.THROW
    /// 67 : OpCode.PUSH0
    /// 68 : OpCode.SWAP
    /// 69 : OpCode.DUP
    /// 6A : OpCode.PUSH0
    /// 6B : OpCode.JMPEQ 0C
    /// 6D : OpCode.DUP
    /// 6E : OpCode.PUSH1
    /// 6F : OpCode.AND
    /// 70 : OpCode.ROT
    /// 71 : OpCode.ADD
    /// 72 : OpCode.SWAP
    /// 73 : OpCode.PUSH1
    /// 74 : OpCode.SHR
    /// 75 : OpCode.JMP F4
    /// 77 : OpCode.DROP
    /// 78 : OpCode.RET
    /// </remarks>
    [DisplayName("popCountBigInteger")]
    public abstract BigInteger? PopCountBigInteger(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAH/AJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT16 FF00
    /// 07 : OpCode.AND
    /// 08 : OpCode.PUSH0
    /// 09 : OpCode.SWAP
    /// 0A : OpCode.DUP
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.JMPEQ 0C
    /// 0E : OpCode.DUP
    /// 0F : OpCode.PUSH1
    /// 10 : OpCode.AND
    /// 11 : OpCode.ROT
    /// 12 : OpCode.ADD
    /// 13 : OpCode.SWAP
    /// 14 : OpCode.PUSH1
    /// 15 : OpCode.SHR
    /// 16 : OpCode.JMP F4
    /// 18 : OpCode.DROP
    /// 19 : OpCode.RET
    /// </remarks>
    [DisplayName("popCountByte")]
    public abstract BigInteger? PopCountByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAP/////AAAAAJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0D : OpCode.AND
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.SWAP
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSH0
    /// 12 : OpCode.JMPEQ 0C
    /// 14 : OpCode.DUP
    /// 15 : OpCode.PUSH1
    /// 16 : OpCode.AND
    /// 17 : OpCode.ROT
    /// 18 : OpCode.ADD
    /// 19 : OpCode.SWAP
    /// 1A : OpCode.PUSH1
    /// 1B : OpCode.SHR
    /// 1C : OpCode.JMP F4
    /// 1E : OpCode.DROP
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("popCountInt")]
    public abstract BigInteger? PopCountInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAT//////////wAAAAAAAAAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 15 : OpCode.AND
    /// 16 : OpCode.PUSH0
    /// 17 : OpCode.SWAP
    /// 18 : OpCode.DUP
    /// 19 : OpCode.PUSH0
    /// 1A : OpCode.JMPEQ 0C
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSH1
    /// 1E : OpCode.AND
    /// 1F : OpCode.ROT
    /// 20 : OpCode.ADD
    /// 21 : OpCode.SWAP
    /// 22 : OpCode.PUSH1
    /// 23 : OpCode.SHR
    /// 24 : OpCode.JMP F4
    /// 26 : OpCode.DROP
    /// 27 : OpCode.RET
    /// </remarks>
    [DisplayName("popCountLong")]
    public abstract BigInteger? PopCountLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAH/AJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT16 FF00
    /// 07 : OpCode.AND
    /// 08 : OpCode.PUSH0
    /// 09 : OpCode.SWAP
    /// 0A : OpCode.DUP
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.JMPEQ 0C
    /// 0E : OpCode.DUP
    /// 0F : OpCode.PUSH1
    /// 10 : OpCode.AND
    /// 11 : OpCode.ROT
    /// 12 : OpCode.ADD
    /// 13 : OpCode.SWAP
    /// 14 : OpCode.PUSH1
    /// 15 : OpCode.SHR
    /// 16 : OpCode.JMP F4
    /// 18 : OpCode.DROP
    /// 19 : OpCode.RET
    /// </remarks>
    [DisplayName("popCountSByte")]
    public abstract BigInteger? PopCountSByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAL//wAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT32 FFFF0000
    /// 09 : OpCode.AND
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.SWAP
    /// 0C : OpCode.DUP
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.JMPEQ 0C
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSH1
    /// 12 : OpCode.AND
    /// 13 : OpCode.ROT
    /// 14 : OpCode.ADD
    /// 15 : OpCode.SWAP
    /// 16 : OpCode.PUSH1
    /// 17 : OpCode.SHR
    /// 18 : OpCode.JMP F4
    /// 1A : OpCode.DROP
    /// 1B : OpCode.RET
    /// </remarks>
    [DisplayName("popCountShort")]
    public abstract BigInteger? PopCountShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAP/////AAAAAJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0D : OpCode.AND
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.SWAP
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSH0
    /// 12 : OpCode.JMPEQ 0C
    /// 14 : OpCode.DUP
    /// 15 : OpCode.PUSH1
    /// 16 : OpCode.AND
    /// 17 : OpCode.ROT
    /// 18 : OpCode.ADD
    /// 19 : OpCode.SWAP
    /// 1A : OpCode.PUSH1
    /// 1B : OpCode.SHR
    /// 1C : OpCode.JMP F4
    /// 1E : OpCode.DROP
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("popCountUInt")]
    public abstract BigInteger? PopCountUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAT//////////wAAAAAAAAAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 15 : OpCode.AND
    /// 16 : OpCode.PUSH0
    /// 17 : OpCode.SWAP
    /// 18 : OpCode.DUP
    /// 19 : OpCode.PUSH0
    /// 1A : OpCode.JMPEQ 0C
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSH1
    /// 1E : OpCode.AND
    /// 1F : OpCode.ROT
    /// 20 : OpCode.ADD
    /// 21 : OpCode.SWAP
    /// 22 : OpCode.PUSH1
    /// 23 : OpCode.SHR
    /// 24 : OpCode.JMP F4
    /// 26 : OpCode.DROP
    /// 27 : OpCode.RET
    /// </remarks>
    [DisplayName("popCountULong")]
    public abstract BigInteger? PopCountULong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAL//wAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT32 FFFF0000
    /// 09 : OpCode.AND
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.SWAP
    /// 0C : OpCode.DUP
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.JMPEQ 0C
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSH1
    /// 12 : OpCode.AND
    /// 13 : OpCode.ROT
    /// 14 : OpCode.ADD
    /// 15 : OpCode.SWAP
    /// 16 : OpCode.PUSH1
    /// 17 : OpCode.SHR
    /// 18 : OpCode.JMP F4
    /// 1A : OpCode.DROP
    /// 1B : OpCode.RET
    /// </remarks>
    [DisplayName("popCountUShort")]
    public abstract BigInteger? PopCountUShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkXkVAB/wCRUKgB/wCReAH/AJF5GFCfF5GpkgH/AJFA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSH7
    /// 06 : OpCode.AND
    /// 07 : OpCode.SWAP
    /// 08 : OpCode.PUSHINT16 FF00
    /// 0B : OpCode.AND
    /// 0C : OpCode.SWAP
    /// 0D : OpCode.SHL
    /// 0E : OpCode.PUSHINT16 FF00
    /// 11 : OpCode.AND
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.PUSHINT16 FF00
    /// 16 : OpCode.AND
    /// 17 : OpCode.LDARG1
    /// 18 : OpCode.PUSH8
    /// 19 : OpCode.SWAP
    /// 1A : OpCode.SUB
    /// 1B : OpCode.PUSH7
    /// 1C : OpCode.AND
    /// 1D : OpCode.SHR
    /// 1E : OpCode.OR
    /// 1F : OpCode.PUSHINT16 FF00
    /// 22 : OpCode.AND
    /// 23 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftByte")]
    public abstract BigInteger? RotateLeftByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5FQA/////8AAAAAkVCoA/////8AAAAAkXgD/////wAAAACReQAgUJ8AH5GpkkoDAAAAgAAAAAAwDAMAAAAAAQAAAJ9A
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSHINT8 1F
    /// 07 : OpCode.AND
    /// 08 : OpCode.SWAP
    /// 09 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 12 : OpCode.AND
    /// 13 : OpCode.SWAP
    /// 14 : OpCode.SHL
    /// 15 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1E : OpCode.AND
    /// 1F : OpCode.LDARG0
    /// 20 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 29 : OpCode.AND
    /// 2A : OpCode.LDARG1
    /// 2B : OpCode.PUSHINT8 20
    /// 2D : OpCode.SWAP
    /// 2E : OpCode.SUB
    /// 2F : OpCode.PUSHINT8 1F
    /// 31 : OpCode.AND
    /// 32 : OpCode.SHR
    /// 33 : OpCode.OR
    /// 34 : OpCode.DUP
    /// 35 : OpCode.PUSHINT64 0000008000000000
    /// 3E : OpCode.JMPLT 0C
    /// 40 : OpCode.PUSHINT64 0000000001000000
    /// 49 : OpCode.SUB
    /// 4A : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftInt")]
    public abstract BigInteger? RotateLeftInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAP5FQBP//////////AAAAAAAAAACRUKgE//////////8AAAAAAAAAAJF4BP//////////AAAAAAAAAACReQBAUJ8AP5GpkkoEAAAAAAAAAIAAAAAAAAAAADAUBAAAAAAAAAAAAQAAAAAAAACfQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSHINT8 3F
    /// 07 : OpCode.AND
    /// 08 : OpCode.SWAP
    /// 09 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 1A : OpCode.AND
    /// 1B : OpCode.SWAP
    /// 1C : OpCode.SHL
    /// 1D : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 2E : OpCode.AND
    /// 2F : OpCode.LDARG0
    /// 30 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 41 : OpCode.AND
    /// 42 : OpCode.LDARG1
    /// 43 : OpCode.PUSHINT8 40
    /// 45 : OpCode.SWAP
    /// 46 : OpCode.SUB
    /// 47 : OpCode.PUSHINT8 3F
    /// 49 : OpCode.AND
    /// 4A : OpCode.SHR
    /// 4B : OpCode.OR
    /// 4C : OpCode.DUP
    /// 4D : OpCode.PUSHINT128 00000000000000800000000000000000
    /// 5E : OpCode.JMPLT 14
    /// 60 : OpCode.PUSHINT128 00000000000000000100000000000000
    /// 71 : OpCode.SUB
    /// 72 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftLong")]
    public abstract BigInteger? RotateLeftLong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkXkVAB/wCRUKgB/wCReAH/AJF5GFCfF5GpkkoBgAAwBgEAAZ9A
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSH7
    /// 06 : OpCode.AND
    /// 07 : OpCode.SWAP
    /// 08 : OpCode.PUSHINT16 FF00
    /// 0B : OpCode.AND
    /// 0C : OpCode.SWAP
    /// 0D : OpCode.SHL
    /// 0E : OpCode.PUSHINT16 FF00
    /// 11 : OpCode.AND
    /// 12 : OpCode.LDARG0
    /// 13 : OpCode.PUSHINT16 FF00
    /// 16 : OpCode.AND
    /// 17 : OpCode.LDARG1
    /// 18 : OpCode.PUSH8
    /// 19 : OpCode.SWAP
    /// 1A : OpCode.SUB
    /// 1B : OpCode.PUSH7
    /// 1C : OpCode.AND
    /// 1D : OpCode.SHR
    /// 1E : OpCode.OR
    /// 1F : OpCode.DUP
    /// 20 : OpCode.PUSHINT16 8000
    /// 23 : OpCode.JMPLT 06
    /// 25 : OpCode.PUSHINT16 0001
    /// 28 : OpCode.SUB
    /// 29 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftSByte")]
    public abstract BigInteger? RotateLeftSByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkfkVAC//8AAJFQqAL//wAAkXgC//8AAJF5IFCfH5GpkkoCAIAAADAIAgAAAQCfQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSH15
    /// 06 : OpCode.AND
    /// 07 : OpCode.SWAP
    /// 08 : OpCode.PUSHINT32 FFFF0000
    /// 0D : OpCode.AND
    /// 0E : OpCode.SWAP
    /// 0F : OpCode.SHL
    /// 10 : OpCode.PUSHINT32 FFFF0000
    /// 15 : OpCode.AND
    /// 16 : OpCode.LDARG0
    /// 17 : OpCode.PUSHINT32 FFFF0000
    /// 1C : OpCode.AND
    /// 1D : OpCode.LDARG1
    /// 1E : OpCode.PUSH16
    /// 1F : OpCode.SWAP
    /// 20 : OpCode.SUB
    /// 21 : OpCode.PUSH15
    /// 22 : OpCode.AND
    /// 23 : OpCode.SHR
    /// 24 : OpCode.OR
    /// 25 : OpCode.DUP
    /// 26 : OpCode.PUSHINT32 00800000
    /// 2B : OpCode.JMPLT 08
    /// 2D : OpCode.PUSHINT32 00000100
    /// 32 : OpCode.SUB
    /// 33 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftShort")]
    public abstract BigInteger? RotateLeftShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5FQA/////8AAAAAkVCoA/////8AAAAAkXgD/////wAAAACReQAgUJ8AH5GpkgP/////AAAAAJFA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSHINT8 1F
    /// 07 : OpCode.AND
    /// 08 : OpCode.SWAP
    /// 09 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 12 : OpCode.AND
    /// 13 : OpCode.SWAP
    /// 14 : OpCode.SHL
    /// 15 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1E : OpCode.AND
    /// 1F : OpCode.LDARG0
    /// 20 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 29 : OpCode.AND
    /// 2A : OpCode.LDARG1
    /// 2B : OpCode.PUSHINT8 20
    /// 2D : OpCode.SWAP
    /// 2E : OpCode.SUB
    /// 2F : OpCode.PUSHINT8 1F
    /// 31 : OpCode.AND
    /// 32 : OpCode.SHR
    /// 33 : OpCode.OR
    /// 34 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 3D : OpCode.AND
    /// 3E : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftUInt")]
    public abstract BigInteger? RotateLeftUInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAP5FQBP//////////AAAAAAAAAACRUKgE//////////8AAAAAAAAAAJF4BP//////////AAAAAAAAAACReQBAUJ8AP5GpkgT//////////wAAAAAAAAAAkUA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSHINT8 3F
    /// 07 : OpCode.AND
    /// 08 : OpCode.SWAP
    /// 09 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 1A : OpCode.AND
    /// 1B : OpCode.SWAP
    /// 1C : OpCode.SHL
    /// 1D : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 2E : OpCode.AND
    /// 2F : OpCode.LDARG0
    /// 30 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 41 : OpCode.AND
    /// 42 : OpCode.LDARG1
    /// 43 : OpCode.PUSHINT8 40
    /// 45 : OpCode.SWAP
    /// 46 : OpCode.SUB
    /// 47 : OpCode.PUSHINT8 3F
    /// 49 : OpCode.AND
    /// 4A : OpCode.SHR
    /// 4B : OpCode.OR
    /// 4C : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 5D : OpCode.AND
    /// 5E : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftULong")]
    public abstract BigInteger? RotateLeftULong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkfkVAC//8AAJFQqAL//wAAkXgC//8AAJF5IFCfH5GpkgL//wAAkUA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSH15
    /// 06 : OpCode.AND
    /// 07 : OpCode.SWAP
    /// 08 : OpCode.PUSHINT32 FFFF0000
    /// 0D : OpCode.AND
    /// 0E : OpCode.SWAP
    /// 0F : OpCode.SHL
    /// 10 : OpCode.PUSHINT32 FFFF0000
    /// 15 : OpCode.AND
    /// 16 : OpCode.LDARG0
    /// 17 : OpCode.PUSHINT32 FFFF0000
    /// 1C : OpCode.AND
    /// 1D : OpCode.LDARG1
    /// 1E : OpCode.PUSH16
    /// 1F : OpCode.SWAP
    /// 20 : OpCode.SUB
    /// 21 : OpCode.PUSH15
    /// 22 : OpCode.AND
    /// 23 : OpCode.SHR
    /// 24 : OpCode.OR
    /// 25 : OpCode.PUSHINT32 FFFF0000
    /// 2A : OpCode.AND
    /// 2B : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftUShort")]
    public abstract BigInteger? RotateLeftUShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkXkal4GHmfF5GokgH/AJFA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSH7
    /// 06 : OpCode.AND
    /// 07 : OpCode.SHR
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.PUSH8
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.SUB
    /// 0C : OpCode.PUSH7
    /// 0D : OpCode.AND
    /// 0E : OpCode.SHL
    /// 0F : OpCode.OR
    /// 10 : OpCode.PUSHINT16 FF00
    /// 13 : OpCode.AND
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightByte")]
    public abstract BigInteger? RotateRightByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5EAIKIAIFCfUAP/////AAAAAJFQqAP/////AAAAAJF4A/////8AAAAAkXkAIKIAIFCfACBQnwAfkamSSgMAAACAAAAAADAMAwAAAAABAAAAn0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSHINT8 1F
    /// 07 : OpCode.AND
    /// 08 : OpCode.PUSHINT8 20
    /// 0A : OpCode.MOD
    /// 0B : OpCode.PUSHINT8 20
    /// 0D : OpCode.SWAP
    /// 0E : OpCode.SUB
    /// 0F : OpCode.SWAP
    /// 10 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 19 : OpCode.AND
    /// 1A : OpCode.SWAP
    /// 1B : OpCode.SHL
    /// 1C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 25 : OpCode.AND
    /// 26 : OpCode.LDARG0
    /// 27 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 30 : OpCode.AND
    /// 31 : OpCode.LDARG1
    /// 32 : OpCode.PUSHINT8 20
    /// 34 : OpCode.MOD
    /// 35 : OpCode.PUSHINT8 20
    /// 37 : OpCode.SWAP
    /// 38 : OpCode.SUB
    /// 39 : OpCode.PUSHINT8 20
    /// 3B : OpCode.SWAP
    /// 3C : OpCode.SUB
    /// 3D : OpCode.PUSHINT8 1F
    /// 3F : OpCode.AND
    /// 40 : OpCode.SHR
    /// 41 : OpCode.OR
    /// 42 : OpCode.DUP
    /// 43 : OpCode.PUSHINT64 0000008000000000
    /// 4C : OpCode.JMPLT 0C
    /// 4E : OpCode.PUSHINT64 0000000001000000
    /// 57 : OpCode.SUB
    /// 58 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightInt")]
    public abstract BigInteger? RotateRightInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAP5EAQKIAQFCfUAT//////////wAAAAAAAAAAkVCoBP//////////AAAAAAAAAACReAT//////////wAAAAAAAAAAkXkAQKIAQFCfAEBQnwA/kamSSgQAAAAAAAAAgAAAAAAAAAAAMBQEAAAAAAAAAAABAAAAAAAAAJ9A
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSHINT8 3F
    /// 07 : OpCode.AND
    /// 08 : OpCode.PUSHINT8 40
    /// 0A : OpCode.MOD
    /// 0B : OpCode.PUSHINT8 40
    /// 0D : OpCode.SWAP
    /// 0E : OpCode.SUB
    /// 0F : OpCode.SWAP
    /// 10 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 21 : OpCode.AND
    /// 22 : OpCode.SWAP
    /// 23 : OpCode.SHL
    /// 24 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 35 : OpCode.AND
    /// 36 : OpCode.LDARG0
    /// 37 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 48 : OpCode.AND
    /// 49 : OpCode.LDARG1
    /// 4A : OpCode.PUSHINT8 40
    /// 4C : OpCode.MOD
    /// 4D : OpCode.PUSHINT8 40
    /// 4F : OpCode.SWAP
    /// 50 : OpCode.SUB
    /// 51 : OpCode.PUSHINT8 40
    /// 53 : OpCode.SWAP
    /// 54 : OpCode.SUB
    /// 55 : OpCode.PUSHINT8 3F
    /// 57 : OpCode.AND
    /// 58 : OpCode.SHR
    /// 59 : OpCode.OR
    /// 5A : OpCode.DUP
    /// 5B : OpCode.PUSHINT128 00000000000000800000000000000000
    /// 6C : OpCode.JMPLT 14
    /// 6E : OpCode.PUSHINT128 00000000000000000100000000000000
    /// 7F : OpCode.SUB
    /// 80 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightLong")]
    public abstract BigInteger? RotateRightLong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkXkRiiGFCfUAH/AJFQqAH/AJF4Af8AkXkYohhQnxhQnxeRqZJKAYAAMAYBAAGfQA==
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSH7
    /// 06 : OpCode.AND
    /// 07 : OpCode.PUSH8
    /// 08 : OpCode.MOD
    /// 09 : OpCode.PUSH8
    /// 0A : OpCode.SWAP
    /// 0B : OpCode.SUB
    /// 0C : OpCode.SWAP
    /// 0D : OpCode.PUSHINT16 FF00
    /// 10 : OpCode.AND
    /// 11 : OpCode.SWAP
    /// 12 : OpCode.SHL
    /// 13 : OpCode.PUSHINT16 FF00
    /// 16 : OpCode.AND
    /// 17 : OpCode.LDARG0
    /// 18 : OpCode.PUSHINT16 FF00
    /// 1B : OpCode.AND
    /// 1C : OpCode.LDARG1
    /// 1D : OpCode.PUSH8
    /// 1E : OpCode.MOD
    /// 1F : OpCode.PUSH8
    /// 20 : OpCode.SWAP
    /// 21 : OpCode.SUB
    /// 22 : OpCode.PUSH8
    /// 23 : OpCode.SWAP
    /// 24 : OpCode.SUB
    /// 25 : OpCode.PUSH7
    /// 26 : OpCode.AND
    /// 27 : OpCode.SHR
    /// 28 : OpCode.OR
    /// 29 : OpCode.DUP
    /// 2A : OpCode.PUSHINT16 8000
    /// 2D : OpCode.JMPLT 06
    /// 2F : OpCode.PUSHINT16 0001
    /// 32 : OpCode.SUB
    /// 33 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightSByte")]
    public abstract BigInteger? RotateRightSByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkfkSCiIFCfUAL//wAAkVCoAv//AACReAL//wAAkXkgoiBQnyBQnx+RqZJKAgCAAAAwCAIAAAEAn0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSH15
    /// 06 : OpCode.AND
    /// 07 : OpCode.PUSH16
    /// 08 : OpCode.MOD
    /// 09 : OpCode.PUSH16
    /// 0A : OpCode.SWAP
    /// 0B : OpCode.SUB
    /// 0C : OpCode.SWAP
    /// 0D : OpCode.PUSHINT32 FFFF0000
    /// 12 : OpCode.AND
    /// 13 : OpCode.SWAP
    /// 14 : OpCode.SHL
    /// 15 : OpCode.PUSHINT32 FFFF0000
    /// 1A : OpCode.AND
    /// 1B : OpCode.LDARG0
    /// 1C : OpCode.PUSHINT32 FFFF0000
    /// 21 : OpCode.AND
    /// 22 : OpCode.LDARG1
    /// 23 : OpCode.PUSH16
    /// 24 : OpCode.MOD
    /// 25 : OpCode.PUSH16
    /// 26 : OpCode.SWAP
    /// 27 : OpCode.SUB
    /// 28 : OpCode.PUSH16
    /// 29 : OpCode.SWAP
    /// 2A : OpCode.SUB
    /// 2B : OpCode.PUSH15
    /// 2C : OpCode.AND
    /// 2D : OpCode.SHR
    /// 2E : OpCode.OR
    /// 2F : OpCode.DUP
    /// 30 : OpCode.PUSHINT32 00800000
    /// 35 : OpCode.JMPLT 08
    /// 37 : OpCode.PUSHINT32 00000100
    /// 3C : OpCode.SUB
    /// 3D : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightShort")]
    public abstract BigInteger? RotateRightShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5GpeAAgeZ8AH5GokgP/////AAAAAJFA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSHINT8 1F
    /// 07 : OpCode.AND
    /// 08 : OpCode.SHR
    /// 09 : OpCode.LDARG0
    /// 0A : OpCode.PUSHINT8 20
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.SUB
    /// 0E : OpCode.PUSHINT8 1F
    /// 10 : OpCode.AND
    /// 11 : OpCode.SHL
    /// 12 : OpCode.OR
    /// 13 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 1C : OpCode.AND
    /// 1D : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightUInt")]
    public abstract BigInteger? RotateRightUInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAP5GpeABAeZ8AP5GokgT//////////wAAAAAAAAAAkUA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSHINT8 3F
    /// 07 : OpCode.AND
    /// 08 : OpCode.SHR
    /// 09 : OpCode.LDARG0
    /// 0A : OpCode.PUSHINT8 40
    /// 0C : OpCode.LDARG1
    /// 0D : OpCode.SUB
    /// 0E : OpCode.PUSHINT8 3F
    /// 10 : OpCode.AND
    /// 11 : OpCode.SHL
    /// 12 : OpCode.OR
    /// 13 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 24 : OpCode.AND
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightULong")]
    public abstract BigInteger? RotateRightULong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkfkal4IHmfH5GokgL//wAAkUA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.PUSH15
    /// 06 : OpCode.AND
    /// 07 : OpCode.SHR
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.PUSH16
    /// 0A : OpCode.LDARG1
    /// 0B : OpCode.SUB
    /// 0C : OpCode.PUSH15
    /// 0D : OpCode.AND
    /// 0E : OpCode.SHL
    /// 0F : OpCode.OR
    /// 10 : OpCode.PUSHINT32 FFFF0000
    /// 15 : OpCode.AND
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightUShort")]
    public abstract BigInteger? RotateRightUShort(BigInteger? value, BigInteger? offset);

    #endregion
}
