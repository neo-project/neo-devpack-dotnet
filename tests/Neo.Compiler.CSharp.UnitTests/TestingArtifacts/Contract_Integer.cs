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
    [DisplayName("clampBigInteger")]
    public abstract BigInteger? ClampBigInteger(BigInteger? value, BigInteger? min, BigInteger? max);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : LDARG2
    // 0006 : OVER
    // 0007 : OVER
    // 0008 : JMPLE
    // 000A : THROW
    // 000B : REVERSE3
    // 000C : MAX
    // 000D : MIN
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampByte")]
    public abstract BigInteger? ClampByte(BigInteger? value, BigInteger? min, BigInteger? max);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : LDARG2
    // 0006 : OVER
    // 0007 : OVER
    // 0008 : JMPLE
    // 000A : THROW
    // 000B : REVERSE3
    // 000C : MAX
    // 000D : MIN
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampInt")]
    public abstract BigInteger? ClampInt(BigInteger? value, BigInteger? min, BigInteger? max);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : LDARG2
    // 0006 : OVER
    // 0007 : OVER
    // 0008 : JMPLE
    // 000A : THROW
    // 000B : REVERSE3
    // 000C : MAX
    // 000D : MIN
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampLong")]
    public abstract BigInteger? ClampLong(BigInteger? value, BigInteger? min, BigInteger? max);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : LDARG2
    // 0006 : OVER
    // 0007 : OVER
    // 0008 : JMPLE
    // 000A : THROW
    // 000B : REVERSE3
    // 000C : MAX
    // 000D : MIN
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampSByte")]
    public abstract BigInteger? ClampSByte(BigInteger? value, BigInteger? min, BigInteger? max);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : LDARG2
    // 0006 : OVER
    // 0007 : OVER
    // 0008 : JMPLE
    // 000A : THROW
    // 000B : REVERSE3
    // 000C : MAX
    // 000D : MIN
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampShort")]
    public abstract BigInteger? ClampShort(BigInteger? value, BigInteger? min, BigInteger? max);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : LDARG2
    // 0006 : OVER
    // 0007 : OVER
    // 0008 : JMPLE
    // 000A : THROW
    // 000B : REVERSE3
    // 000C : MAX
    // 000D : MIN
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampUInt")]
    public abstract BigInteger? ClampUInt(BigInteger? value, BigInteger? min, BigInteger? max);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : LDARG2
    // 0006 : OVER
    // 0007 : OVER
    // 0008 : JMPLE
    // 000A : THROW
    // 000B : REVERSE3
    // 000C : MAX
    // 000D : MIN
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampULong")]
    public abstract BigInteger? ClampULong(BigInteger? value, BigInteger? min, BigInteger? max);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : LDARG2
    // 0006 : OVER
    // 0007 : OVER
    // 0008 : JMPLE
    // 000A : THROW
    // 000B : REVERSE3
    // 000C : MAX
    // 000D : MIN
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("clampUShort")]
    public abstract BigInteger? ClampUShort(BigInteger? value, BigInteger? min, BigInteger? max);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : LDARG2
    // 0006 : OVER
    // 0007 : OVER
    // 0008 : JMPLE
    // 000A : THROW
    // 000B : REVERSE3
    // 000C : MAX
    // 000D : MIN
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("copySignInt")]
    public abstract BigInteger? CopySignInt(BigInteger? value, BigInteger? sign);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSH0
    // 0006 : JMPLT
    // 0008 : ABS
    // 0009 : JMP
    // 000B : ABS
    // 000C : NEGATE
    // 000D : DUP
    // 000E : PUSHINT32
    // 0013 : JMPLE
    // 0015 : THROW
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("copySignLong")]
    public abstract BigInteger? CopySignLong(BigInteger? value, BigInteger? sign);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSH0
    // 0006 : JMPLT
    // 0008 : ABS
    // 0009 : JMP
    // 000B : ABS
    // 000C : NEGATE
    // 000D : DUP
    // 000E : PUSHINT64
    // 0017 : JMPLE
    // 0019 : THROW
    // 001A : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("copySignSbyte")]
    public abstract BigInteger? CopySignSbyte(BigInteger? value, BigInteger? sign);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSH0
    // 0006 : JMPLT
    // 0008 : ABS
    // 0009 : JMP
    // 000B : ABS
    // 000C : NEGATE
    // 000D : DUP
    // 000E : PUSHINT8
    // 0010 : JMPLE
    // 0012 : THROW
    // 0013 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("copySignShort")]
    public abstract BigInteger? CopySignShort(BigInteger? value, BigInteger? sign);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSH0
    // 0006 : JMPLT
    // 0008 : ABS
    // 0009 : JMP
    // 000B : ABS
    // 000C : NEGATE
    // 000D : DUP
    // 000E : PUSHINT16
    // 0011 : JMPLE
    // 0013 : THROW
    // 0014 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedByte")]
    public abstract BigInteger? CreateCheckedByte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : PUSHINT16
    // 0009 : WITHIN
    // 000A : JMPIF
    // 000C : THROW
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedChar")]
    public abstract BigInteger? CreateCheckedChar(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : PUSHINT32
    // 000B : WITHIN
    // 000C : JMPIF
    // 000E : THROW
    // 000F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedInt")]
    public abstract BigInteger? CreateCheckedInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSHINT32
    // 000A : PUSHINT64
    // 0013 : WITHIN
    // 0014 : JMPIF
    // 0016 : THROW
    // 0017 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedLong")]
    public abstract BigInteger? CreateCheckedLong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSHINT64
    // 000E : PUSHINT128
    // 001F : WITHIN
    // 0020 : JMPIF
    // 0022 : THROW
    // 0023 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedSbyte")]
    public abstract BigInteger? CreateCheckedSbyte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSHINT8
    // 0007 : PUSHINT16
    // 000A : WITHIN
    // 000B : JMPIF
    // 000D : THROW
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedShort")]
    public abstract BigInteger? CreateCheckedShort(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSHINT16
    // 0008 : PUSHINT32
    // 000D : WITHIN
    // 000E : JMPIF
    // 0010 : THROW
    // 0011 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createCheckedUlong")]
    public abstract BigInteger? CreateCheckedUlong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : PUSHINT128
    // 0017 : WITHIN
    // 0018 : JMPIF
    // 001A : THROW
    // 001B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createSaturatingByte")]
    public abstract BigInteger? CreateSaturatingByte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : PUSHINT16
    // 0008 : DUP
    // 0009 : ROT
    // 000A : DUP
    // 000B : ROT
    // 000C : JMPLT
    // 000E : THROW
    // 000F : ROT
    // 0010 : DUP
    // 0011 : ROT
    // 0012 : DUP
    // 0013 : ROT
    // 0014 : JMPGT
    // 0016 : DROP
    // 0017 : DUP
    // 0018 : ROT
    // 0019 : DUP
    // 001A : ROT
    // 001B : JMPLT
    // 001D : DROP
    // 001E : RET
    // 001F : REVERSE3
    // 0020 : DROP
    // 0021 : DROP
    // 0022 : RET
    // 0023 : SWAP
    // 0024 : DROP
    // 0025 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createSaturatingChar")]
    public abstract BigInteger? CreateSaturatingChar(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : PUSHINT32
    // 000A : DUP
    // 000B : ROT
    // 000C : DUP
    // 000D : ROT
    // 000E : JMPLT
    // 0010 : THROW
    // 0011 : ROT
    // 0012 : DUP
    // 0013 : ROT
    // 0014 : DUP
    // 0015 : ROT
    // 0016 : JMPGT
    // 0018 : DROP
    // 0019 : DUP
    // 001A : ROT
    // 001B : DUP
    // 001C : ROT
    // 001D : JMPLT
    // 001F : DROP
    // 0020 : RET
    // 0021 : REVERSE3
    // 0022 : DROP
    // 0023 : DROP
    // 0024 : RET
    // 0025 : SWAP
    // 0026 : DROP
    // 0027 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createSaturatingInt")]
    public abstract BigInteger? CreateSaturatingInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT32
    // 0009 : PUSHINT32
    // 000E : DUP
    // 000F : ROT
    // 0010 : DUP
    // 0011 : ROT
    // 0012 : JMPLT
    // 0014 : THROW
    // 0015 : ROT
    // 0016 : DUP
    // 0017 : ROT
    // 0018 : DUP
    // 0019 : ROT
    // 001A : JMPGT
    // 001C : DROP
    // 001D : DUP
    // 001E : ROT
    // 001F : DUP
    // 0020 : ROT
    // 0021 : JMPLT
    // 0023 : DROP
    // 0024 : RET
    // 0025 : REVERSE3
    // 0026 : DROP
    // 0027 : DROP
    // 0028 : RET
    // 0029 : SWAP
    // 002A : DROP
    // 002B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createSaturatingLong")]
    public abstract BigInteger? CreateSaturatingLong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT64
    // 000D : PUSHINT64
    // 0016 : DUP
    // 0017 : ROT
    // 0018 : DUP
    // 0019 : ROT
    // 001A : JMPLT
    // 001C : THROW
    // 001D : ROT
    // 001E : DUP
    // 001F : ROT
    // 0020 : DUP
    // 0021 : ROT
    // 0022 : JMPGT
    // 0024 : DROP
    // 0025 : DUP
    // 0026 : ROT
    // 0027 : DUP
    // 0028 : ROT
    // 0029 : JMPLT
    // 002B : DROP
    // 002C : RET
    // 002D : REVERSE3
    // 002E : DROP
    // 002F : DROP
    // 0030 : RET
    // 0031 : SWAP
    // 0032 : DROP
    // 0033 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createSaturatingSbyte")]
    public abstract BigInteger? CreateSaturatingSbyte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT8
    // 0006 : PUSHINT8
    // 0008 : DUP
    // 0009 : ROT
    // 000A : DUP
    // 000B : ROT
    // 000C : JMPLT
    // 000E : THROW
    // 000F : ROT
    // 0010 : DUP
    // 0011 : ROT
    // 0012 : DUP
    // 0013 : ROT
    // 0014 : JMPGT
    // 0016 : DROP
    // 0017 : DUP
    // 0018 : ROT
    // 0019 : DUP
    // 001A : ROT
    // 001B : JMPLT
    // 001D : DROP
    // 001E : RET
    // 001F : REVERSE3
    // 0020 : DROP
    // 0021 : DROP
    // 0022 : RET
    // 0023 : SWAP
    // 0024 : DROP
    // 0025 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("createSaturatingUlong")]
    public abstract BigInteger? CreateSaturatingUlong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : PUSHINT128
    // 0016 : DUP
    // 0017 : ROT
    // 0018 : DUP
    // 0019 : ROT
    // 001A : JMPLT
    // 001C : THROW
    // 001D : ROT
    // 001E : DUP
    // 001F : ROT
    // 0020 : DUP
    // 0021 : ROT
    // 0022 : JMPGT
    // 0024 : DROP
    // 0025 : DUP
    // 0026 : ROT
    // 0027 : DUP
    // 0028 : ROT
    // 0029 : JMPLT
    // 002B : DROP
    // 002C : RET
    // 002D : REVERSE3
    // 002E : DROP
    // 002F : DROP
    // 0030 : RET
    // 0031 : SWAP
    // 0032 : DROP
    // 0033 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemByte")]
    public abstract IList<object>? DivRemByte(BigInteger? left, BigInteger? right);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : LDARG0
    // 0005 : DUP
    // 0006 : PUSH2
    // 0007 : PICK
    // 0008 : DIV
    // 0009 : REVERSE3
    // 000A : MOD
    // 000B : PUSH2
    // 000C : PACK
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemInt")]
    public abstract IList<object>? DivRemInt(BigInteger? left, BigInteger? right);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : LDARG0
    // 0005 : DUP
    // 0006 : PUSH2
    // 0007 : PICK
    // 0008 : DIV
    // 0009 : REVERSE3
    // 000A : MOD
    // 000B : PUSH2
    // 000C : PACK
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemLong")]
    public abstract IList<object>? DivRemLong(BigInteger? left, BigInteger? right);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : LDARG0
    // 0005 : DUP
    // 0006 : PUSH2
    // 0007 : PICK
    // 0008 : DIV
    // 0009 : REVERSE3
    // 000A : MOD
    // 000B : PUSH2
    // 000C : PACK
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemSbyte")]
    public abstract IList<object>? DivRemSbyte(BigInteger? left, BigInteger? right);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : LDARG0
    // 0005 : DUP
    // 0006 : PUSH2
    // 0007 : PICK
    // 0008 : DIV
    // 0009 : REVERSE3
    // 000A : MOD
    // 000B : PUSH2
    // 000C : PACK
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemShort")]
    public abstract IList<object>? DivRemShort(BigInteger? left, BigInteger? right);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : LDARG0
    // 0005 : DUP
    // 0006 : PUSH2
    // 0007 : PICK
    // 0008 : DIV
    // 0009 : REVERSE3
    // 000A : MOD
    // 000B : PUSH2
    // 000C : PACK
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemUint")]
    public abstract IList<object>? DivRemUint(BigInteger? left, BigInteger? right);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : LDARG0
    // 0005 : DUP
    // 0006 : PUSH2
    // 0007 : PICK
    // 0008 : DIV
    // 0009 : REVERSE3
    // 000A : MOD
    // 000B : PUSH2
    // 000C : PACK
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemUlong")]
    public abstract IList<object>? DivRemUlong(BigInteger? left, BigInteger? right);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : LDARG0
    // 0005 : DUP
    // 0006 : PUSH2
    // 0007 : PICK
    // 0008 : DIV
    // 0009 : REVERSE3
    // 000A : MOD
    // 000B : PUSH2
    // 000C : PACK
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("divRemUshort")]
    public abstract IList<object>? DivRemUshort(BigInteger? left, BigInteger? right);
    // 0000 : INITSLOT
    // 0003 : LDARG1
    // 0004 : LDARG0
    // 0005 : DUP
    // 0006 : PUSH2
    // 0007 : PICK
    // 0008 : DIV
    // 0009 : REVERSE3
    // 000A : MOD
    // 000B : PUSH2
    // 000C : PACK
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenByte")]
    public abstract bool? IsEvenByte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NOT
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenIntegerInt")]
    public abstract bool? IsEvenIntegerInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NOT
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenLong")]
    public abstract bool? IsEvenLong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NOT
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenSbyte")]
    public abstract bool? IsEvenSbyte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NOT
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenShort")]
    public abstract bool? IsEvenShort(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NOT
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEventUInt")]
    public abstract bool? IsEventUInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NOT
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenUlong")]
    public abstract bool? IsEvenUlong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NOT
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isEvenUshort")]
    public abstract bool? IsEvenUshort(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NOT
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isNegativeInt")]
    public abstract bool? IsNegativeInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : LT
    // 0006 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isNegativeLong")]
    public abstract bool? IsNegativeLong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : LT
    // 0006 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isNegativeSbyte")]
    public abstract bool? IsNegativeSbyte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : LT
    // 0006 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isNegativeShort")]
    public abstract bool? IsNegativeShort(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : LT
    // 0006 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddByte")]
    public abstract bool? IsOddByte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NZ
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddIntegerInt")]
    public abstract bool? IsOddIntegerInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NZ
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddLong")]
    public abstract bool? IsOddLong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NZ
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddSbyte")]
    public abstract bool? IsOddSbyte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NZ
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddShort")]
    public abstract bool? IsOddShort(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NZ
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddUInt")]
    public abstract bool? IsOddUInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NZ
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddUlong")]
    public abstract bool? IsOddUlong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NZ
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isOddUshort")]
    public abstract bool? IsOddUshort(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH2
    // 0005 : MOD
    // 0006 : NZ
    // 0007 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPositiveInt")]
    public abstract bool? IsPositiveInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : GE
    // 0006 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPositiveLong")]
    public abstract bool? IsPositiveLong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : GE
    // 0006 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPositiveSbyte")]
    public abstract bool? IsPositiveSbyte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : GE
    // 0006 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPositiveShort")]
    public abstract bool? IsPositiveShort(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : GE
    // 0006 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2BigInteger")]
    public abstract bool? IsPow2BigInteger(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPNE
    // 0008 : DROP
    // 0009 : JMP
    // 000B : DUP
    // 000C : DEC
    // 000D : AND
    // 000E : PUSH0
    // 000F : JMPEQ
    // 0011 : PUSHF
    // 0012 : RET
    // 0013 : PUSHT
    // 0014 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Byte")]
    public abstract bool? IsPow2Byte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPNE
    // 0008 : DROP
    // 0009 : JMP
    // 000B : DUP
    // 000C : DEC
    // 000D : AND
    // 000E : PUSH0
    // 000F : JMPEQ
    // 0011 : PUSHF
    // 0012 : RET
    // 0013 : PUSHT
    // 0014 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Int")]
    public abstract bool? IsPow2Int(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPNE
    // 0008 : DROP
    // 0009 : JMP
    // 000B : DUP
    // 000C : DEC
    // 000D : AND
    // 000E : PUSH0
    // 000F : JMPEQ
    // 0011 : PUSHF
    // 0012 : RET
    // 0013 : PUSHT
    // 0014 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Long")]
    public abstract bool? IsPow2Long(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPNE
    // 0008 : DROP
    // 0009 : JMP
    // 000B : DUP
    // 000C : DEC
    // 000D : AND
    // 000E : PUSH0
    // 000F : JMPEQ
    // 0011 : PUSHF
    // 0012 : RET
    // 0013 : PUSHT
    // 0014 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Sbyte")]
    public abstract bool? IsPow2Sbyte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPNE
    // 0008 : DROP
    // 0009 : JMP
    // 000B : DUP
    // 000C : DEC
    // 000D : AND
    // 000E : PUSH0
    // 000F : JMPEQ
    // 0011 : PUSHF
    // 0012 : RET
    // 0013 : PUSHT
    // 0014 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Short")]
    public abstract bool? IsPow2Short(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPNE
    // 0008 : DROP
    // 0009 : JMP
    // 000B : DUP
    // 000C : DEC
    // 000D : AND
    // 000E : PUSH0
    // 000F : JMPEQ
    // 0011 : PUSHF
    // 0012 : RET
    // 0013 : PUSHT
    // 0014 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2UInt")]
    public abstract bool? IsPow2UInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPNE
    // 0008 : DROP
    // 0009 : JMP
    // 000B : DUP
    // 000C : DEC
    // 000D : AND
    // 000E : PUSH0
    // 000F : JMPEQ
    // 0011 : PUSHF
    // 0012 : RET
    // 0013 : PUSHT
    // 0014 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Ulong")]
    public abstract bool? IsPow2Ulong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPNE
    // 0008 : DROP
    // 0009 : JMP
    // 000B : DUP
    // 000C : DEC
    // 000D : AND
    // 000E : PUSH0
    // 000F : JMPEQ
    // 0011 : PUSHF
    // 0012 : RET
    // 0013 : PUSHT
    // 0014 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("isPow2Ushort")]
    public abstract bool? IsPow2Ushort(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPNE
    // 0008 : DROP
    // 0009 : JMP
    // 000B : DUP
    // 000C : DEC
    // 000D : AND
    // 000E : PUSH0
    // 000F : JMPEQ
    // 0011 : PUSHF
    // 0012 : RET
    // 0013 : PUSHT
    // 0014 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountByte")]
    public abstract BigInteger? LeadingZeroCountByte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : SWAP
    // 0006 : DUP
    // 0007 : PUSH0
    // 0008 : JMPEQ
    // 000A : PUSH1
    // 000B : SHR
    // 000C : SWAP
    // 000D : INC
    // 000E : JMP
    // 0010 : DROP
    // 0011 : PUSH8
    // 0012 : SWAP
    // 0013 : SUB
    // 0014 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountInt")]
    public abstract BigInteger? LeadingZeroCountInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : DROP
    // 0009 : PUSH0
    // 000A : RET
    // 000B : PUSH0
    // 000C : SWAP
    // 000D : DUP
    // 000E : PUSH0
    // 000F : JMPEQ
    // 0011 : PUSH1
    // 0012 : SHR
    // 0013 : SWAP
    // 0014 : INC
    // 0015 : JMP
    // 0017 : DROP
    // 0018 : PUSHINT8
    // 001A : SWAP
    // 001B : SUB
    // 001C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountLong")]
    public abstract BigInteger? LeadingZeroCountLong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : DROP
    // 0009 : PUSH0
    // 000A : RET
    // 000B : PUSH0
    // 000C : SWAP
    // 000D : DUP
    // 000E : PUSH0
    // 000F : JMPEQ
    // 0011 : PUSH1
    // 0012 : SHR
    // 0013 : SWAP
    // 0014 : INC
    // 0015 : JMP
    // 0017 : DROP
    // 0018 : PUSHINT8
    // 001A : SWAP
    // 001B : SUB
    // 001C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountSbyte")]
    public abstract BigInteger? LeadingZeroCountSbyte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : DROP
    // 0009 : PUSH0
    // 000A : RET
    // 000B : PUSH0
    // 000C : SWAP
    // 000D : DUP
    // 000E : PUSH0
    // 000F : JMPEQ
    // 0011 : PUSH1
    // 0012 : SHR
    // 0013 : SWAP
    // 0014 : INC
    // 0015 : JMP
    // 0017 : DROP
    // 0018 : PUSH8
    // 0019 : SWAP
    // 001A : SUB
    // 001B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountShort")]
    public abstract BigInteger? LeadingZeroCountShort(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : DROP
    // 0009 : PUSH0
    // 000A : RET
    // 000B : PUSH0
    // 000C : SWAP
    // 000D : DUP
    // 000E : PUSH0
    // 000F : JMPEQ
    // 0011 : PUSH1
    // 0012 : SHR
    // 0013 : SWAP
    // 0014 : INC
    // 0015 : JMP
    // 0017 : DROP
    // 0018 : PUSH16
    // 0019 : SWAP
    // 001A : SUB
    // 001B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountUInt")]
    public abstract BigInteger? LeadingZeroCountUInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : SWAP
    // 0006 : DUP
    // 0007 : PUSH0
    // 0008 : JMPEQ
    // 000A : PUSH1
    // 000B : SHR
    // 000C : SWAP
    // 000D : INC
    // 000E : JMP
    // 0010 : DROP
    // 0011 : PUSHINT8
    // 0013 : SWAP
    // 0014 : SUB
    // 0015 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("leadingZeroCountUshort")]
    public abstract BigInteger? LeadingZeroCountUshort(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSH0
    // 0005 : SWAP
    // 0006 : DUP
    // 0007 : PUSH0
    // 0008 : JMPEQ
    // 000A : PUSH1
    // 000B : SHR
    // 000C : SWAP
    // 000D : INC
    // 000E : JMP
    // 0010 : DROP
    // 0011 : PUSH16
    // 0012 : SWAP
    // 0013 : SUB
    // 0014 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2Byte")]
    public abstract BigInteger? Log2Byte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : THROW
    // 0009 : DUP
    // 000A : PUSH0
    // 000B : JMPEQ
    // 000D : PUSH0
    // 000E : INC
    // 000F : OVER
    // 0010 : OVER
    // 0011 : SHR
    // 0012 : PUSH0
    // 0013 : JMPGT
    // 0015 : NIP
    // 0016 : DEC
    // 0017 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2Int")]
    public abstract BigInteger? Log2Int(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : THROW
    // 0009 : DUP
    // 000A : PUSH0
    // 000B : JMPEQ
    // 000D : PUSH0
    // 000E : INC
    // 000F : OVER
    // 0010 : OVER
    // 0011 : SHR
    // 0012 : PUSH0
    // 0013 : JMPGT
    // 0015 : NIP
    // 0016 : DEC
    // 0017 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2Long")]
    public abstract BigInteger? Log2Long(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : THROW
    // 0009 : DUP
    // 000A : PUSH0
    // 000B : JMPEQ
    // 000D : PUSH0
    // 000E : INC
    // 000F : OVER
    // 0010 : OVER
    // 0011 : SHR
    // 0012 : PUSH0
    // 0013 : JMPGT
    // 0015 : NIP
    // 0016 : DEC
    // 0017 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2Sbyte")]
    public abstract BigInteger? Log2Sbyte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : THROW
    // 0009 : DUP
    // 000A : PUSH0
    // 000B : JMPEQ
    // 000D : PUSH0
    // 000E : INC
    // 000F : OVER
    // 0010 : OVER
    // 0011 : SHR
    // 0012 : PUSH0
    // 0013 : JMPGT
    // 0015 : NIP
    // 0016 : DEC
    // 0017 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2Short")]
    public abstract BigInteger? Log2Short(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : THROW
    // 0009 : DUP
    // 000A : PUSH0
    // 000B : JMPEQ
    // 000D : PUSH0
    // 000E : INC
    // 000F : OVER
    // 0010 : OVER
    // 0011 : SHR
    // 0012 : PUSH0
    // 0013 : JMPGT
    // 0015 : NIP
    // 0016 : DEC
    // 0017 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2UInt")]
    public abstract BigInteger? Log2UInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : THROW
    // 0009 : DUP
    // 000A : PUSH0
    // 000B : JMPEQ
    // 000D : PUSH0
    // 000E : INC
    // 000F : OVER
    // 0010 : OVER
    // 0011 : SHR
    // 0012 : PUSH0
    // 0013 : JMPGT
    // 0015 : NIP
    // 0016 : DEC
    // 0017 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("log2Ushort")]
    public abstract BigInteger? Log2Ushort(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : JMPGE
    // 0008 : THROW
    // 0009 : DUP
    // 000A : PUSH0
    // 000B : JMPEQ
    // 000D : PUSH0
    // 000E : INC
    // 000F : OVER
    // 0010 : OVER
    // 0011 : SHR
    // 0012 : PUSH0
    // 0013 : JMPGT
    // 0015 : NIP
    // 0016 : DEC
    // 0017 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountBigInteger")]
    public abstract BigInteger? PopCountBigInteger(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSHINT32
    // 000A : PUSHINT64
    // 0013 : WITHIN
    // 0014 : JMPIFNOT
    // 0016 : PUSHINT64
    // 001F : AND
    // 0020 : JMP
    // 0022 : PUSHDATA1
    // 0066 : THROW
    // 0067 : PUSH0
    // 0068 : SWAP
    // 0069 : DUP
    // 006A : PUSH0
    // 006B : JMPEQ
    // 006D : DUP
    // 006E : PUSH1
    // 006F : AND
    // 0070 : ROT
    // 0071 : ADD
    // 0072 : SWAP
    // 0073 : PUSH1
    // 0074 : SHR
    // 0075 : JMP
    // 0077 : DROP
    // 0078 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountByte")]
    public abstract BigInteger? PopCountByte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT16
    // 0007 : AND
    // 0008 : PUSH0
    // 0009 : SWAP
    // 000A : DUP
    // 000B : PUSH0
    // 000C : JMPEQ
    // 000E : DUP
    // 000F : PUSH1
    // 0010 : AND
    // 0011 : ROT
    // 0012 : ADD
    // 0013 : SWAP
    // 0014 : PUSH1
    // 0015 : SHR
    // 0016 : JMP
    // 0018 : DROP
    // 0019 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountInt")]
    public abstract BigInteger? PopCountInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT64
    // 000D : AND
    // 000E : PUSH0
    // 000F : SWAP
    // 0010 : DUP
    // 0011 : PUSH0
    // 0012 : JMPEQ
    // 0014 : DUP
    // 0015 : PUSH1
    // 0016 : AND
    // 0017 : ROT
    // 0018 : ADD
    // 0019 : SWAP
    // 001A : PUSH1
    // 001B : SHR
    // 001C : JMP
    // 001E : DROP
    // 001F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountLong")]
    public abstract BigInteger? PopCountLong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT128
    // 0015 : AND
    // 0016 : PUSH0
    // 0017 : SWAP
    // 0018 : DUP
    // 0019 : PUSH0
    // 001A : JMPEQ
    // 001C : DUP
    // 001D : PUSH1
    // 001E : AND
    // 001F : ROT
    // 0020 : ADD
    // 0021 : SWAP
    // 0022 : PUSH1
    // 0023 : SHR
    // 0024 : JMP
    // 0026 : DROP
    // 0027 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountSByte")]
    public abstract BigInteger? PopCountSByte(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT16
    // 0007 : AND
    // 0008 : PUSH0
    // 0009 : SWAP
    // 000A : DUP
    // 000B : PUSH0
    // 000C : JMPEQ
    // 000E : DUP
    // 000F : PUSH1
    // 0010 : AND
    // 0011 : ROT
    // 0012 : ADD
    // 0013 : SWAP
    // 0014 : PUSH1
    // 0015 : SHR
    // 0016 : JMP
    // 0018 : DROP
    // 0019 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountShort")]
    public abstract BigInteger? PopCountShort(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT32
    // 0009 : AND
    // 000A : PUSH0
    // 000B : SWAP
    // 000C : DUP
    // 000D : PUSH0
    // 000E : JMPEQ
    // 0010 : DUP
    // 0011 : PUSH1
    // 0012 : AND
    // 0013 : ROT
    // 0014 : ADD
    // 0015 : SWAP
    // 0016 : PUSH1
    // 0017 : SHR
    // 0018 : JMP
    // 001A : DROP
    // 001B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountUInt")]
    public abstract BigInteger? PopCountUInt(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT64
    // 000D : AND
    // 000E : PUSH0
    // 000F : SWAP
    // 0010 : DUP
    // 0011 : PUSH0
    // 0012 : JMPEQ
    // 0014 : DUP
    // 0015 : PUSH1
    // 0016 : AND
    // 0017 : ROT
    // 0018 : ADD
    // 0019 : SWAP
    // 001A : PUSH1
    // 001B : SHR
    // 001C : JMP
    // 001E : DROP
    // 001F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountULong")]
    public abstract BigInteger? PopCountULong(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT128
    // 0015 : AND
    // 0016 : PUSH0
    // 0017 : SWAP
    // 0018 : DUP
    // 0019 : PUSH0
    // 001A : JMPEQ
    // 001C : DUP
    // 001D : PUSH1
    // 001E : AND
    // 001F : ROT
    // 0020 : ADD
    // 0021 : SWAP
    // 0022 : PUSH1
    // 0023 : SHR
    // 0024 : JMP
    // 0026 : DROP
    // 0027 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("popCountUShort")]
    public abstract BigInteger? PopCountUShort(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT32
    // 0009 : AND
    // 000A : PUSH0
    // 000B : SWAP
    // 000C : DUP
    // 000D : PUSH0
    // 000E : JMPEQ
    // 0010 : DUP
    // 0011 : PUSH1
    // 0012 : AND
    // 0013 : ROT
    // 0014 : ADD
    // 0015 : SWAP
    // 0016 : PUSH1
    // 0017 : SHR
    // 0018 : JMP
    // 001A : DROP
    // 001B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftByte")]
    public abstract BigInteger? RotateLeftByte(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSH7
    // 0006 : AND
    // 0007 : SWAP
    // 0008 : PUSHINT16
    // 000B : AND
    // 000C : SWAP
    // 000D : SHL
    // 000E : PUSHINT16
    // 0011 : AND
    // 0012 : LDARG0
    // 0013 : PUSHINT16
    // 0016 : AND
    // 0017 : LDARG1
    // 0018 : PUSH8
    // 0019 : SWAP
    // 001A : SUB
    // 001B : PUSH7
    // 001C : AND
    // 001D : SHR
    // 001E : OR
    // 001F : PUSHINT16
    // 0022 : AND
    // 0023 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftInt")]
    public abstract BigInteger? RotateLeftInt(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSHINT8
    // 0007 : AND
    // 0008 : SWAP
    // 0009 : PUSHINT64
    // 0012 : AND
    // 0013 : SWAP
    // 0014 : SHL
    // 0015 : PUSHINT64
    // 001E : AND
    // 001F : LDARG0
    // 0020 : PUSHINT64
    // 0029 : AND
    // 002A : LDARG1
    // 002B : PUSHINT8
    // 002D : SWAP
    // 002E : SUB
    // 002F : PUSHINT8
    // 0031 : AND
    // 0032 : SHR
    // 0033 : OR
    // 0034 : DUP
    // 0035 : PUSHINT64
    // 003E : JMPLT
    // 0040 : PUSHINT64
    // 0049 : SUB
    // 004A : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftLong")]
    public abstract BigInteger? RotateLeftLong(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSHINT8
    // 0007 : AND
    // 0008 : SWAP
    // 0009 : PUSHINT128
    // 001A : AND
    // 001B : SWAP
    // 001C : SHL
    // 001D : PUSHINT128
    // 002E : AND
    // 002F : LDARG0
    // 0030 : PUSHINT128
    // 0041 : AND
    // 0042 : LDARG1
    // 0043 : PUSHINT8
    // 0045 : SWAP
    // 0046 : SUB
    // 0047 : PUSHINT8
    // 0049 : AND
    // 004A : SHR
    // 004B : OR
    // 004C : DUP
    // 004D : PUSHINT128
    // 005E : JMPLT
    // 0060 : PUSHINT128
    // 0071 : SUB
    // 0072 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftSByte")]
    public abstract BigInteger? RotateLeftSByte(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSH7
    // 0006 : AND
    // 0007 : SWAP
    // 0008 : PUSHINT16
    // 000B : AND
    // 000C : SWAP
    // 000D : SHL
    // 000E : PUSHINT16
    // 0011 : AND
    // 0012 : LDARG0
    // 0013 : PUSHINT16
    // 0016 : AND
    // 0017 : LDARG1
    // 0018 : PUSH8
    // 0019 : SWAP
    // 001A : SUB
    // 001B : PUSH7
    // 001C : AND
    // 001D : SHR
    // 001E : OR
    // 001F : DUP
    // 0020 : PUSHINT16
    // 0023 : JMPLT
    // 0025 : PUSHINT16
    // 0028 : SUB
    // 0029 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftShort")]
    public abstract BigInteger? RotateLeftShort(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSH15
    // 0006 : AND
    // 0007 : SWAP
    // 0008 : PUSHINT32
    // 000D : AND
    // 000E : SWAP
    // 000F : SHL
    // 0010 : PUSHINT32
    // 0015 : AND
    // 0016 : LDARG0
    // 0017 : PUSHINT32
    // 001C : AND
    // 001D : LDARG1
    // 001E : PUSH16
    // 001F : SWAP
    // 0020 : SUB
    // 0021 : PUSH15
    // 0022 : AND
    // 0023 : SHR
    // 0024 : OR
    // 0025 : DUP
    // 0026 : PUSHINT32
    // 002B : JMPLT
    // 002D : PUSHINT32
    // 0032 : SUB
    // 0033 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftUInt")]
    public abstract BigInteger? RotateLeftUInt(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSHINT8
    // 0007 : AND
    // 0008 : SWAP
    // 0009 : PUSHINT64
    // 0012 : AND
    // 0013 : SWAP
    // 0014 : SHL
    // 0015 : PUSHINT64
    // 001E : AND
    // 001F : LDARG0
    // 0020 : PUSHINT64
    // 0029 : AND
    // 002A : LDARG1
    // 002B : PUSHINT8
    // 002D : SWAP
    // 002E : SUB
    // 002F : PUSHINT8
    // 0031 : AND
    // 0032 : SHR
    // 0033 : OR
    // 0034 : PUSHINT64
    // 003D : AND
    // 003E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftULong")]
    public abstract BigInteger? RotateLeftULong(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSHINT8
    // 0007 : AND
    // 0008 : SWAP
    // 0009 : PUSHINT128
    // 001A : AND
    // 001B : SWAP
    // 001C : SHL
    // 001D : PUSHINT128
    // 002E : AND
    // 002F : LDARG0
    // 0030 : PUSHINT128
    // 0041 : AND
    // 0042 : LDARG1
    // 0043 : PUSHINT8
    // 0045 : SWAP
    // 0046 : SUB
    // 0047 : PUSHINT8
    // 0049 : AND
    // 004A : SHR
    // 004B : OR
    // 004C : PUSHINT128
    // 005D : AND
    // 005E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateLeftUShort")]
    public abstract BigInteger? RotateLeftUShort(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSH15
    // 0006 : AND
    // 0007 : SWAP
    // 0008 : PUSHINT32
    // 000D : AND
    // 000E : SWAP
    // 000F : SHL
    // 0010 : PUSHINT32
    // 0015 : AND
    // 0016 : LDARG0
    // 0017 : PUSHINT32
    // 001C : AND
    // 001D : LDARG1
    // 001E : PUSH16
    // 001F : SWAP
    // 0020 : SUB
    // 0021 : PUSH15
    // 0022 : AND
    // 0023 : SHR
    // 0024 : OR
    // 0025 : PUSHINT32
    // 002A : AND
    // 002B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightByte")]
    public abstract BigInteger? RotateRightByte(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSH7
    // 0006 : AND
    // 0007 : SHR
    // 0008 : LDARG0
    // 0009 : PUSH8
    // 000A : LDARG1
    // 000B : SUB
    // 000C : PUSH7
    // 000D : AND
    // 000E : SHL
    // 000F : OR
    // 0010 : PUSHINT16
    // 0013 : AND
    // 0014 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightInt")]
    public abstract BigInteger? RotateRightInt(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSHINT8
    // 0007 : AND
    // 0008 : PUSHINT8
    // 000A : MOD
    // 000B : PUSHINT8
    // 000D : SWAP
    // 000E : SUB
    // 000F : SWAP
    // 0010 : PUSHINT64
    // 0019 : AND
    // 001A : SWAP
    // 001B : SHL
    // 001C : PUSHINT64
    // 0025 : AND
    // 0026 : LDARG0
    // 0027 : PUSHINT64
    // 0030 : AND
    // 0031 : LDARG1
    // 0032 : PUSHINT8
    // 0034 : MOD
    // 0035 : PUSHINT8
    // 0037 : SWAP
    // 0038 : SUB
    // 0039 : PUSHINT8
    // 003B : SWAP
    // 003C : SUB
    // 003D : PUSHINT8
    // 003F : AND
    // 0040 : SHR
    // 0041 : OR
    // 0042 : DUP
    // 0043 : PUSHINT64
    // 004C : JMPLT
    // 004E : PUSHINT64
    // 0057 : SUB
    // 0058 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightLong")]
    public abstract BigInteger? RotateRightLong(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSHINT8
    // 0007 : AND
    // 0008 : PUSHINT8
    // 000A : MOD
    // 000B : PUSHINT8
    // 000D : SWAP
    // 000E : SUB
    // 000F : SWAP
    // 0010 : PUSHINT128
    // 0021 : AND
    // 0022 : SWAP
    // 0023 : SHL
    // 0024 : PUSHINT128
    // 0035 : AND
    // 0036 : LDARG0
    // 0037 : PUSHINT128
    // 0048 : AND
    // 0049 : LDARG1
    // 004A : PUSHINT8
    // 004C : MOD
    // 004D : PUSHINT8
    // 004F : SWAP
    // 0050 : SUB
    // 0051 : PUSHINT8
    // 0053 : SWAP
    // 0054 : SUB
    // 0055 : PUSHINT8
    // 0057 : AND
    // 0058 : SHR
    // 0059 : OR
    // 005A : DUP
    // 005B : PUSHINT128
    // 006C : JMPLT
    // 006E : PUSHINT128
    // 007F : SUB
    // 0080 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightSByte")]
    public abstract BigInteger? RotateRightSByte(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSH7
    // 0006 : AND
    // 0007 : PUSH8
    // 0008 : MOD
    // 0009 : PUSH8
    // 000A : SWAP
    // 000B : SUB
    // 000C : SWAP
    // 000D : PUSHINT16
    // 0010 : AND
    // 0011 : SWAP
    // 0012 : SHL
    // 0013 : PUSHINT16
    // 0016 : AND
    // 0017 : LDARG0
    // 0018 : PUSHINT16
    // 001B : AND
    // 001C : LDARG1
    // 001D : PUSH8
    // 001E : MOD
    // 001F : PUSH8
    // 0020 : SWAP
    // 0021 : SUB
    // 0022 : PUSH8
    // 0023 : SWAP
    // 0024 : SUB
    // 0025 : PUSH7
    // 0026 : AND
    // 0027 : SHR
    // 0028 : OR
    // 0029 : DUP
    // 002A : PUSHINT16
    // 002D : JMPLT
    // 002F : PUSHINT16
    // 0032 : SUB
    // 0033 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightShort")]
    public abstract BigInteger? RotateRightShort(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSH15
    // 0006 : AND
    // 0007 : PUSH16
    // 0008 : MOD
    // 0009 : PUSH16
    // 000A : SWAP
    // 000B : SUB
    // 000C : SWAP
    // 000D : PUSHINT32
    // 0012 : AND
    // 0013 : SWAP
    // 0014 : SHL
    // 0015 : PUSHINT32
    // 001A : AND
    // 001B : LDARG0
    // 001C : PUSHINT32
    // 0021 : AND
    // 0022 : LDARG1
    // 0023 : PUSH16
    // 0024 : MOD
    // 0025 : PUSH16
    // 0026 : SWAP
    // 0027 : SUB
    // 0028 : PUSH16
    // 0029 : SWAP
    // 002A : SUB
    // 002B : PUSH15
    // 002C : AND
    // 002D : SHR
    // 002E : OR
    // 002F : DUP
    // 0030 : PUSHINT32
    // 0035 : JMPLT
    // 0037 : PUSHINT32
    // 003C : SUB
    // 003D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightUInt")]
    public abstract BigInteger? RotateRightUInt(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSHINT8
    // 0007 : AND
    // 0008 : SHR
    // 0009 : LDARG0
    // 000A : PUSHINT8
    // 000C : LDARG1
    // 000D : SUB
    // 000E : PUSHINT8
    // 0010 : AND
    // 0011 : SHL
    // 0012 : OR
    // 0013 : PUSHINT64
    // 001C : AND
    // 001D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightULong")]
    public abstract BigInteger? RotateRightULong(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSHINT8
    // 0007 : AND
    // 0008 : SHR
    // 0009 : LDARG0
    // 000A : PUSHINT8
    // 000C : LDARG1
    // 000D : SUB
    // 000E : PUSHINT8
    // 0010 : AND
    // 0011 : SHL
    // 0012 : OR
    // 0013 : PUSHINT128
    // 0024 : AND
    // 0025 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rotateRightUShort")]
    public abstract BigInteger? RotateRightUShort(BigInteger? value, BigInteger? offset);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : LDARG1
    // 0005 : PUSH15
    // 0006 : AND
    // 0007 : SHR
    // 0008 : LDARG0
    // 0009 : PUSH16
    // 000A : LDARG1
    // 000B : SUB
    // 000C : PUSH15
    // 000D : AND
    // 000E : SHL
    // 000F : OR
    // 0010 : PUSHINT32
    // 0015 : AND
    // 0016 : RET

    #endregion

}
