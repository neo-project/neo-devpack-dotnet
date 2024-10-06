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
    /// 0000 : OpCode.INITSLOT 0003
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.LDARG2
    /// 0006 : OpCode.OVER
    /// 0007 : OpCode.OVER
    /// 0008 : OpCode.JMPLE 03
    /// 000A : OpCode.THROW
    /// 000B : OpCode.REVERSE3
    /// 000C : OpCode.MAX
    /// 000D : OpCode.MIN
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("clampBigInteger")]
    public abstract BigInteger? ClampBigInteger(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 0000 : OpCode.INITSLOT 0003
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.LDARG2
    /// 0006 : OpCode.OVER
    /// 0007 : OpCode.OVER
    /// 0008 : OpCode.JMPLE 03
    /// 000A : OpCode.THROW
    /// 000B : OpCode.REVERSE3
    /// 000C : OpCode.MAX
    /// 000D : OpCode.MIN
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("clampByte")]
    public abstract BigInteger? ClampByte(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 0000 : OpCode.INITSLOT 0003
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.LDARG2
    /// 0006 : OpCode.OVER
    /// 0007 : OpCode.OVER
    /// 0008 : OpCode.JMPLE 03
    /// 000A : OpCode.THROW
    /// 000B : OpCode.REVERSE3
    /// 000C : OpCode.MAX
    /// 000D : OpCode.MIN
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("clampInt")]
    public abstract BigInteger? ClampInt(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 0000 : OpCode.INITSLOT 0003
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.LDARG2
    /// 0006 : OpCode.OVER
    /// 0007 : OpCode.OVER
    /// 0008 : OpCode.JMPLE 03
    /// 000A : OpCode.THROW
    /// 000B : OpCode.REVERSE3
    /// 000C : OpCode.MAX
    /// 000D : OpCode.MIN
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("clampLong")]
    public abstract BigInteger? ClampLong(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 0000 : OpCode.INITSLOT 0003
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.LDARG2
    /// 0006 : OpCode.OVER
    /// 0007 : OpCode.OVER
    /// 0008 : OpCode.JMPLE 03
    /// 000A : OpCode.THROW
    /// 000B : OpCode.REVERSE3
    /// 000C : OpCode.MAX
    /// 000D : OpCode.MIN
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("clampSByte")]
    public abstract BigInteger? ClampSByte(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 0000 : OpCode.INITSLOT 0003
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.LDARG2
    /// 0006 : OpCode.OVER
    /// 0007 : OpCode.OVER
    /// 0008 : OpCode.JMPLE 03
    /// 000A : OpCode.THROW
    /// 000B : OpCode.REVERSE3
    /// 000C : OpCode.MAX
    /// 000D : OpCode.MIN
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("clampShort")]
    public abstract BigInteger? ClampShort(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 0000 : OpCode.INITSLOT 0003
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.LDARG2
    /// 0006 : OpCode.OVER
    /// 0007 : OpCode.OVER
    /// 0008 : OpCode.JMPLE 03
    /// 000A : OpCode.THROW
    /// 000B : OpCode.REVERSE3
    /// 000C : OpCode.MAX
    /// 000D : OpCode.MIN
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("clampUInt")]
    public abstract BigInteger? ClampUInt(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 0000 : OpCode.INITSLOT 0003
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.LDARG2
    /// 0006 : OpCode.OVER
    /// 0007 : OpCode.OVER
    /// 0008 : OpCode.JMPLE 03
    /// 000A : OpCode.THROW
    /// 000B : OpCode.REVERSE3
    /// 000C : OpCode.MAX
    /// 000D : OpCode.MIN
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("clampULong")]
    public abstract BigInteger? ClampULong(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeHl6S0syAzpTurlA
    /// 0000 : OpCode.INITSLOT 0003
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.LDARG2
    /// 0006 : OpCode.OVER
    /// 0007 : OpCode.OVER
    /// 0008 : OpCode.JMPLE 03
    /// 000A : OpCode.THROW
    /// 000B : OpCode.REVERSE3
    /// 000C : OpCode.MAX
    /// 000D : OpCode.MIN
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("clampUShort")]
    public abstract BigInteger? ClampUShort(BigInteger? value, BigInteger? min, BigInteger? max);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oC////fzIDOkA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPLT 05
    /// 0008 : OpCode.ABS
    /// 0009 : OpCode.JMP 04
    /// 000B : OpCode.ABS
    /// 000C : OpCode.NEGATE
    /// 000D : OpCode.DUP
    /// 000E : OpCode.PUSHINT32 FFFFFF7F
    /// 0013 : OpCode.JMPLE 03
    /// 0015 : OpCode.THROW
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("copySignInt")]
    public abstract BigInteger? CopySignInt(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oD/////////38yAzpA
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPLT 05
    /// 0008 : OpCode.ABS
    /// 0009 : OpCode.JMP 04
    /// 000B : OpCode.ABS
    /// 000C : OpCode.NEGATE
    /// 000D : OpCode.DUP
    /// 000E : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 0017 : OpCode.JMPLE 03
    /// 0019 : OpCode.THROW
    /// 001A : OpCode.RET
    /// </remarks>
    [DisplayName("copySignLong")]
    public abstract BigInteger? CopySignLong(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oAfzIDOkA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPLT 05
    /// 0008 : OpCode.ABS
    /// 0009 : OpCode.JMP 04
    /// 000B : OpCode.ABS
    /// 000C : OpCode.NEGATE
    /// 000D : OpCode.DUP
    /// 000E : OpCode.PUSHINT8 7F
    /// 0010 : OpCode.JMPLE 03
    /// 0012 : OpCode.THROW
    /// 0013 : OpCode.RET
    /// </remarks>
    [DisplayName("copySignSbyte")]
    public abstract BigInteger? CopySignSbyte(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkQMAWaIgSam0oB/38yAzpA
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPLT 05
    /// 0008 : OpCode.ABS
    /// 0009 : OpCode.JMP 04
    /// 000B : OpCode.ABS
    /// 000C : OpCode.NEGATE
    /// 000D : OpCode.DUP
    /// 000E : OpCode.PUSHINT16 FF7F
    /// 0011 : OpCode.JMPLE 03
    /// 0013 : OpCode.THROW
    /// 0014 : OpCode.RET
    /// </remarks>
    [DisplayName("copySignShort")]
    public abstract BigInteger? CopySignShort(BigInteger? value, BigInteger? sign);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQAQABuyQDOkA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.PUSHINT16 0001
    /// 0009 : OpCode.WITHIN
    /// 000A : OpCode.JMPIF 03
    /// 000C : OpCode.THROW
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedByte")]
    public abstract BigInteger? CreateCheckedByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQAgAAAQC7JAM6QA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.PUSHINT32 00000100
    /// 000B : OpCode.WITHIN
    /// 000C : OpCode.JMPIF 03
    /// 000E : OpCode.THROW
    /// 000F : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedChar")]
    public abstract BigInteger? CreateCheckedChar(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoCAAAAgAMAAACAAAAAALskAzpA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT32 00000080
    /// 000A : OpCode.PUSHINT64 0000008000000000
    /// 0013 : OpCode.WITHIN
    /// 0014 : OpCode.JMPIF 03
    /// 0016 : OpCode.THROW
    /// 0017 : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedInt")]
    public abstract BigInteger? CreateCheckedInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALskAzpA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT64 0000000000000080
    /// 000E : OpCode.PUSHINT128 00000000000000800000000000000000
    /// 001F : OpCode.WITHIN
    /// 0020 : OpCode.JMPIF 03
    /// 0022 : OpCode.THROW
    /// 0023 : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedLong")]
    public abstract BigInteger? CreateCheckedLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAgAGAALskAzpA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT8 80
    /// 0007 : OpCode.PUSHINT16 8000
    /// 000A : OpCode.WITHIN
    /// 000B : OpCode.JMPIF 03
    /// 000D : OpCode.THROW
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedSbyte")]
    public abstract BigInteger? CreateCheckedSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoBAIACAIAAALskAzpA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT16 0080
    /// 0008 : OpCode.PUSHINT32 00800000
    /// 000D : OpCode.WITHIN
    /// 000E : OpCode.JMPIF 03
    /// 0010 : OpCode.THROW
    /// 0011 : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedShort")]
    public abstract BigInteger? CreateCheckedShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQBAAAAAAAAAAAAQAAAAAAAAC7JAM6QA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.PUSHINT128 00000000000000000100000000000000
    /// 0017 : OpCode.WITHIN
    /// 0018 : OpCode.JMPIF 03
    /// 001A : OpCode.THROW
    /// 001B : OpCode.RET
    /// </remarks>
    [DisplayName("createCheckedUlong")]
    public abstract BigInteger? CreateCheckedUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAB/wBKUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.PUSHINT16 FF00
    /// 0008 : OpCode.DUP
    /// 0009 : OpCode.ROT
    /// 000A : OpCode.DUP
    /// 000B : OpCode.ROT
    /// 000C : OpCode.JMPLT 03
    /// 000E : OpCode.THROW
    /// 000F : OpCode.ROT
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.ROT
    /// 0012 : OpCode.DUP
    /// 0013 : OpCode.ROT
    /// 0014 : OpCode.JMPGT 0B
    /// 0016 : OpCode.DROP
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.ROT
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.ROT
    /// 001B : OpCode.JMPLT 08
    /// 001D : OpCode.DROP
    /// 001E : OpCode.RET
    /// 001F : OpCode.REVERSE3
    /// 0020 : OpCode.DROP
    /// 0021 : OpCode.DROP
    /// 0022 : OpCode.RET
    /// 0023 : OpCode.SWAP
    /// 0024 : OpCode.DROP
    /// 0025 : OpCode.RET
    /// </remarks>
    [DisplayName("createSaturatingByte")]
    public abstract BigInteger? CreateSaturatingByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAC//8AAEpRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.PUSHINT32 FFFF0000
    /// 000A : OpCode.DUP
    /// 000B : OpCode.ROT
    /// 000C : OpCode.DUP
    /// 000D : OpCode.ROT
    /// 000E : OpCode.JMPLT 03
    /// 0010 : OpCode.THROW
    /// 0011 : OpCode.ROT
    /// 0012 : OpCode.DUP
    /// 0013 : OpCode.ROT
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.ROT
    /// 0016 : OpCode.JMPGT 0B
    /// 0018 : OpCode.DROP
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.ROT
    /// 001B : OpCode.DUP
    /// 001C : OpCode.ROT
    /// 001D : OpCode.JMPLT 08
    /// 001F : OpCode.DROP
    /// 0020 : OpCode.RET
    /// 0021 : OpCode.REVERSE3
    /// 0022 : OpCode.DROP
    /// 0023 : OpCode.DROP
    /// 0024 : OpCode.RET
    /// 0025 : OpCode.SWAP
    /// 0026 : OpCode.DROP
    /// 0027 : OpCode.RET
    /// </remarks>
    [DisplayName("createSaturatingChar")]
    public abstract BigInteger? CreateSaturatingChar(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAIAAACAAv///39KUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT32 00000080
    /// 0009 : OpCode.PUSHINT32 FFFFFF7F
    /// 000E : OpCode.DUP
    /// 000F : OpCode.ROT
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.ROT
    /// 0012 : OpCode.JMPLT 03
    /// 0014 : OpCode.THROW
    /// 0015 : OpCode.ROT
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.ROT
    /// 0018 : OpCode.DUP
    /// 0019 : OpCode.ROT
    /// 001A : OpCode.JMPGT 0B
    /// 001C : OpCode.DROP
    /// 001D : OpCode.DUP
    /// 001E : OpCode.ROT
    /// 001F : OpCode.DUP
    /// 0020 : OpCode.ROT
    /// 0021 : OpCode.JMPLT 08
    /// 0023 : OpCode.DROP
    /// 0024 : OpCode.RET
    /// 0025 : OpCode.REVERSE3
    /// 0026 : OpCode.DROP
    /// 0027 : OpCode.DROP
    /// 0028 : OpCode.RET
    /// 0029 : OpCode.SWAP
    /// 002A : OpCode.DROP
    /// 002B : OpCode.RET
    /// </remarks>
    [DisplayName("createSaturatingInt")]
    public abstract BigInteger? CreateSaturatingInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAMAAAAAAAAAgAP/////////f0pRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT64 0000000000000080
    /// 000D : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.ROT
    /// 0018 : OpCode.DUP
    /// 0019 : OpCode.ROT
    /// 001A : OpCode.JMPLT 03
    /// 001C : OpCode.THROW
    /// 001D : OpCode.ROT
    /// 001E : OpCode.DUP
    /// 001F : OpCode.ROT
    /// 0020 : OpCode.DUP
    /// 0021 : OpCode.ROT
    /// 0022 : OpCode.JMPGT 0B
    /// 0024 : OpCode.DROP
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.ROT
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.ROT
    /// 0029 : OpCode.JMPLT 08
    /// 002B : OpCode.DROP
    /// 002C : OpCode.RET
    /// 002D : OpCode.REVERSE3
    /// 002E : OpCode.DROP
    /// 002F : OpCode.DROP
    /// 0030 : OpCode.RET
    /// 0031 : OpCode.SWAP
    /// 0032 : OpCode.DROP
    /// 0033 : OpCode.RET
    /// </remarks>
    [DisplayName("createSaturatingLong")]
    public abstract BigInteger? CreateSaturatingLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeACAAH9KUUpRMAM6UUpRSlEsC0VKUUpRMAhFQFNFRUBQRUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT8 80
    /// 0006 : OpCode.PUSHINT8 7F
    /// 0008 : OpCode.DUP
    /// 0009 : OpCode.ROT
    /// 000A : OpCode.DUP
    /// 000B : OpCode.ROT
    /// 000C : OpCode.JMPLT 03
    /// 000E : OpCode.THROW
    /// 000F : OpCode.ROT
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.ROT
    /// 0012 : OpCode.DUP
    /// 0013 : OpCode.ROT
    /// 0014 : OpCode.JMPGT 0B
    /// 0016 : OpCode.DROP
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.ROT
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.ROT
    /// 001B : OpCode.JMPLT 08
    /// 001D : OpCode.DROP
    /// 001E : OpCode.RET
    /// 001F : OpCode.REVERSE3
    /// 0020 : OpCode.DROP
    /// 0021 : OpCode.DROP
    /// 0022 : OpCode.RET
    /// 0023 : OpCode.SWAP
    /// 0024 : OpCode.DROP
    /// 0025 : OpCode.RET
    /// </remarks>
    [DisplayName("createSaturatingSbyte")]
    public abstract BigInteger? CreateSaturatingSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBAE//////////8AAAAAAAAAAEpRSlEwAzpRSlFKUSwLRUpRSlEwCEVAU0VFQFBFQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.ROT
    /// 0018 : OpCode.DUP
    /// 0019 : OpCode.ROT
    /// 001A : OpCode.JMPLT 03
    /// 001C : OpCode.THROW
    /// 001D : OpCode.ROT
    /// 001E : OpCode.DUP
    /// 001F : OpCode.ROT
    /// 0020 : OpCode.DUP
    /// 0021 : OpCode.ROT
    /// 0022 : OpCode.JMPGT 0B
    /// 0024 : OpCode.DROP
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.ROT
    /// 0027 : OpCode.DUP
    /// 0028 : OpCode.ROT
    /// 0029 : OpCode.JMPLT 08
    /// 002B : OpCode.DROP
    /// 002C : OpCode.RET
    /// 002D : OpCode.REVERSE3
    /// 002E : OpCode.DROP
    /// 002F : OpCode.DROP
    /// 0030 : OpCode.RET
    /// 0031 : OpCode.SWAP
    /// 0032 : OpCode.DROP
    /// 0033 : OpCode.RET
    /// </remarks>
    [DisplayName("createSaturatingUlong")]
    public abstract BigInteger? CreateSaturatingUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSH2
    /// 0007 : OpCode.PICK
    /// 0008 : OpCode.DIV
    /// 0009 : OpCode.REVERSE3
    /// 000A : OpCode.MOD
    /// 000B : OpCode.PUSH2
    /// 000C : OpCode.PACK
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemByte")]
    public abstract IList<object>? DivRemByte(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSH2
    /// 0007 : OpCode.PICK
    /// 0008 : OpCode.DIV
    /// 0009 : OpCode.REVERSE3
    /// 000A : OpCode.MOD
    /// 000B : OpCode.PUSH2
    /// 000C : OpCode.PACK
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemInt")]
    public abstract IList<object>? DivRemInt(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSH2
    /// 0007 : OpCode.PICK
    /// 0008 : OpCode.DIV
    /// 0009 : OpCode.REVERSE3
    /// 000A : OpCode.MOD
    /// 000B : OpCode.PUSH2
    /// 000C : OpCode.PACK
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemLong")]
    public abstract IList<object>? DivRemLong(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSH2
    /// 0007 : OpCode.PICK
    /// 0008 : OpCode.DIV
    /// 0009 : OpCode.REVERSE3
    /// 000A : OpCode.MOD
    /// 000B : OpCode.PUSH2
    /// 000C : OpCode.PACK
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemSbyte")]
    public abstract IList<object>? DivRemSbyte(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSH2
    /// 0007 : OpCode.PICK
    /// 0008 : OpCode.DIV
    /// 0009 : OpCode.REVERSE3
    /// 000A : OpCode.MOD
    /// 000B : OpCode.PUSH2
    /// 000C : OpCode.PACK
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemShort")]
    public abstract IList<object>? DivRemShort(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSH2
    /// 0007 : OpCode.PICK
    /// 0008 : OpCode.DIV
    /// 0009 : OpCode.REVERSE3
    /// 000A : OpCode.MOD
    /// 000B : OpCode.PUSH2
    /// 000C : OpCode.PACK
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemUint")]
    public abstract IList<object>? DivRemUint(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSH2
    /// 0007 : OpCode.PICK
    /// 0008 : OpCode.DIV
    /// 0009 : OpCode.REVERSE3
    /// 000A : OpCode.MOD
    /// 000B : OpCode.PUSH2
    /// 000C : OpCode.PACK
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemUlong")]
    public abstract IList<object>? DivRemUlong(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXhKEk2hU6ISwEA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG1
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.PUSH2
    /// 0007 : OpCode.PICK
    /// 0008 : OpCode.DIV
    /// 0009 : OpCode.REVERSE3
    /// 000A : OpCode.MOD
    /// 000B : OpCode.PUSH2
    /// 000C : OpCode.PACK
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("divRemUshort")]
    public abstract IList<object>? DivRemUshort(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NOT
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenByte")]
    public abstract bool? IsEvenByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NOT
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenIntegerInt")]
    public abstract bool? IsEvenIntegerInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NOT
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenLong")]
    public abstract bool? IsEvenLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NOT
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenSbyte")]
    public abstract bool? IsEvenSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NOT
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenShort")]
    public abstract bool? IsEvenShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NOT
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isEventUInt")]
    public abstract bool? IsEventUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NOT
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenUlong")]
    public abstract bool? IsEvenUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NOT
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isEvenUshort")]
    public abstract bool? IsEvenUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.LT
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("isNegativeInt")]
    public abstract bool? IsNegativeInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.LT
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("isNegativeLong")]
    public abstract bool? IsNegativeLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.LT
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("isNegativeSbyte")]
    public abstract bool? IsNegativeSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC1QA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.LT
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("isNegativeShort")]
    public abstract bool? IsNegativeShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NZ
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddByte")]
    public abstract bool? IsOddByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NZ
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddIntegerInt")]
    public abstract bool? IsOddIntegerInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NZ
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddLong")]
    public abstract bool? IsOddLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NZ
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddSbyte")]
    public abstract bool? IsOddSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NZ
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddShort")]
    public abstract bool? IsOddShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NZ
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddUInt")]
    public abstract bool? IsOddUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NZ
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddUlong")]
    public abstract bool? IsOddUlong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKisUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.MOD
    /// 0006 : OpCode.NZ
    /// 0007 : OpCode.RET
    /// </remarks>
    [DisplayName("isOddUshort")]
    public abstract bool? IsOddUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.GE
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("isPositiveInt")]
    public abstract bool? IsPositiveInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.GE
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("isPositiveLong")]
    public abstract bool? IsPositiveLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.GE
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("isPositiveSbyte")]
    public abstract bool? IsPositiveSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBC4QA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.GE
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("isPositiveShort")]
    public abstract bool? IsPositiveShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPNE 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.JMP 08
    /// 000B : OpCode.DUP
    /// 000C : OpCode.DEC
    /// 000D : OpCode.AND
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.JMPEQ 04
    /// 0011 : OpCode.PUSHF
    /// 0012 : OpCode.RET
    /// 0013 : OpCode.PUSHT
    /// 0014 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2BigInteger")]
    public abstract bool? IsPow2BigInteger(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPNE 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.JMP 08
    /// 000B : OpCode.DUP
    /// 000C : OpCode.DEC
    /// 000D : OpCode.AND
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.JMPEQ 04
    /// 0011 : OpCode.PUSHF
    /// 0012 : OpCode.RET
    /// 0013 : OpCode.PUSHT
    /// 0014 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Byte")]
    public abstract bool? IsPow2Byte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPNE 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.JMP 08
    /// 000B : OpCode.DUP
    /// 000C : OpCode.DEC
    /// 000D : OpCode.AND
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.JMPEQ 04
    /// 0011 : OpCode.PUSHF
    /// 0012 : OpCode.RET
    /// 0013 : OpCode.PUSHT
    /// 0014 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Int")]
    public abstract bool? IsPow2Int(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPNE 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.JMP 08
    /// 000B : OpCode.DUP
    /// 000C : OpCode.DEC
    /// 000D : OpCode.AND
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.JMPEQ 04
    /// 0011 : OpCode.PUSHF
    /// 0012 : OpCode.RET
    /// 0013 : OpCode.PUSHT
    /// 0014 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Long")]
    public abstract bool? IsPow2Long(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPNE 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.JMP 08
    /// 000B : OpCode.DUP
    /// 000C : OpCode.DEC
    /// 000D : OpCode.AND
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.JMPEQ 04
    /// 0011 : OpCode.PUSHF
    /// 0012 : OpCode.RET
    /// 0013 : OpCode.PUSHT
    /// 0014 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Sbyte")]
    public abstract bool? IsPow2Sbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPNE 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.JMP 08
    /// 000B : OpCode.DUP
    /// 000C : OpCode.DEC
    /// 000D : OpCode.AND
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.JMPEQ 04
    /// 0011 : OpCode.PUSHF
    /// 0012 : OpCode.RET
    /// 0013 : OpCode.PUSHT
    /// 0014 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Short")]
    public abstract bool? IsPow2Short(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPNE 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.JMP 08
    /// 000B : OpCode.DUP
    /// 000C : OpCode.DEC
    /// 000D : OpCode.AND
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.JMPEQ 04
    /// 0011 : OpCode.PUSHF
    /// 0012 : OpCode.RET
    /// 0013 : OpCode.PUSHT
    /// 0014 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2UInt")]
    public abstract bool? IsPow2UInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPNE 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.JMP 08
    /// 000B : OpCode.DUP
    /// 000C : OpCode.DEC
    /// 000D : OpCode.AND
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.JMPEQ 04
    /// 0011 : OpCode.PUSHF
    /// 0012 : OpCode.RET
    /// 0013 : OpCode.PUSHT
    /// 0014 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Ulong")]
    public abstract bool? IsPow2Ulong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQKgVFIghKnZEQKAQJQAhA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPNE 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.JMP 08
    /// 000B : OpCode.DUP
    /// 000C : OpCode.DEC
    /// 000D : OpCode.AND
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.JMPEQ 04
    /// 0011 : OpCode.PUSHF
    /// 0012 : OpCode.RET
    /// 0013 : OpCode.PUSHT
    /// 0014 : OpCode.RET
    /// </remarks>
    [DisplayName("isPow2Ushort")]
    public abstract bool? IsPow2Ushort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBBQShAoCBGpUJwi90UYUJ9A
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.SWAP
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSH0
    /// 0008 : OpCode.JMPEQ 08
    /// 000A : OpCode.PUSH1
    /// 000B : OpCode.SHR
    /// 000C : OpCode.SWAP
    /// 000D : OpCode.INC
    /// 000E : OpCode.JMP F7
    /// 0010 : OpCode.DROP
    /// 0011 : OpCode.PUSH8
    /// 0012 : OpCode.SWAP
    /// 0013 : OpCode.SUB
    /// 0014 : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountByte")]
    public abstract BigInteger? LeadingZeroCountByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFACBQn0A=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.RET
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.SWAP
    /// 000D : OpCode.DUP
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.JMPEQ 08
    /// 0011 : OpCode.PUSH1
    /// 0012 : OpCode.SHR
    /// 0013 : OpCode.SWAP
    /// 0014 : OpCode.INC
    /// 0015 : OpCode.JMP F7
    /// 0017 : OpCode.DROP
    /// 0018 : OpCode.PUSHINT8 20
    /// 001A : OpCode.SWAP
    /// 001B : OpCode.SUB
    /// 001C : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountInt")]
    public abstract BigInteger? LeadingZeroCountInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFAEBQn0A=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.RET
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.SWAP
    /// 000D : OpCode.DUP
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.JMPEQ 08
    /// 0011 : OpCode.PUSH1
    /// 0012 : OpCode.SHR
    /// 0013 : OpCode.SWAP
    /// 0014 : OpCode.INC
    /// 0015 : OpCode.JMP F7
    /// 0017 : OpCode.DROP
    /// 0018 : OpCode.PUSHINT8 40
    /// 001A : OpCode.SWAP
    /// 001B : OpCode.SUB
    /// 001C : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountLong")]
    public abstract BigInteger? LeadingZeroCountLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFGFCfQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.RET
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.SWAP
    /// 000D : OpCode.DUP
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.JMPEQ 08
    /// 0011 : OpCode.PUSH1
    /// 0012 : OpCode.SHR
    /// 0013 : OpCode.SWAP
    /// 0014 : OpCode.INC
    /// 0015 : OpCode.JMP F7
    /// 0017 : OpCode.DROP
    /// 0018 : OpCode.PUSH8
    /// 0019 : OpCode.SWAP
    /// 001A : OpCode.SUB
    /// 001B : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountSbyte")]
    public abstract BigInteger? LeadingZeroCountSbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgVFEEAQUEoQKAgRqVCcIvdFIFCfQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 05
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.RET
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.SWAP
    /// 000D : OpCode.DUP
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.JMPEQ 08
    /// 0011 : OpCode.PUSH1
    /// 0012 : OpCode.SHR
    /// 0013 : OpCode.SWAP
    /// 0014 : OpCode.INC
    /// 0015 : OpCode.JMP F7
    /// 0017 : OpCode.DROP
    /// 0018 : OpCode.PUSH16
    /// 0019 : OpCode.SWAP
    /// 001A : OpCode.SUB
    /// 001B : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountShort")]
    public abstract BigInteger? LeadingZeroCountShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBBQShAoCBGpUJwi90UAIFCfQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.SWAP
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSH0
    /// 0008 : OpCode.JMPEQ 08
    /// 000A : OpCode.PUSH1
    /// 000B : OpCode.SHR
    /// 000C : OpCode.SWAP
    /// 000D : OpCode.INC
    /// 000E : OpCode.JMP F7
    /// 0010 : OpCode.DROP
    /// 0011 : OpCode.PUSHINT8 20
    /// 0013 : OpCode.SWAP
    /// 0014 : OpCode.SUB
    /// 0015 : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountUInt")]
    public abstract BigInteger? LeadingZeroCountUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBBQShAoCBGpUJwi90UgUJ9A
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSH0
    /// 0005 : OpCode.SWAP
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.PUSH0
    /// 0008 : OpCode.JMPEQ 08
    /// 000A : OpCode.PUSH1
    /// 000B : OpCode.SHR
    /// 000C : OpCode.SWAP
    /// 000D : OpCode.INC
    /// 000E : OpCode.JMP F7
    /// 0010 : OpCode.DROP
    /// 0011 : OpCode.PUSH16
    /// 0012 : OpCode.SWAP
    /// 0013 : OpCode.SUB
    /// 0014 : OpCode.RET
    /// </remarks>
    [DisplayName("leadingZeroCountUshort")]
    public abstract BigInteger? LeadingZeroCountUshort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 03
    /// 0008 : OpCode.THROW
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.JMPEQ 0C
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.INC
    /// 000F : OpCode.OVER
    /// 0010 : OpCode.OVER
    /// 0011 : OpCode.SHR
    /// 0012 : OpCode.PUSH0
    /// 0013 : OpCode.JMPGT FB
    /// 0015 : OpCode.NIP
    /// 0016 : OpCode.DEC
    /// 0017 : OpCode.RET
    /// </remarks>
    [DisplayName("log2Byte")]
    public abstract BigInteger? Log2Byte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 03
    /// 0008 : OpCode.THROW
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.JMPEQ 0C
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.INC
    /// 000F : OpCode.OVER
    /// 0010 : OpCode.OVER
    /// 0011 : OpCode.SHR
    /// 0012 : OpCode.PUSH0
    /// 0013 : OpCode.JMPGT FB
    /// 0015 : OpCode.NIP
    /// 0016 : OpCode.DEC
    /// 0017 : OpCode.RET
    /// </remarks>
    [DisplayName("log2Int")]
    public abstract BigInteger? Log2Int(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 03
    /// 0008 : OpCode.THROW
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.JMPEQ 0C
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.INC
    /// 000F : OpCode.OVER
    /// 0010 : OpCode.OVER
    /// 0011 : OpCode.SHR
    /// 0012 : OpCode.PUSH0
    /// 0013 : OpCode.JMPGT FB
    /// 0015 : OpCode.NIP
    /// 0016 : OpCode.DEC
    /// 0017 : OpCode.RET
    /// </remarks>
    [DisplayName("log2Long")]
    public abstract BigInteger? Log2Long(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 03
    /// 0008 : OpCode.THROW
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.JMPEQ 0C
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.INC
    /// 000F : OpCode.OVER
    /// 0010 : OpCode.OVER
    /// 0011 : OpCode.SHR
    /// 0012 : OpCode.PUSH0
    /// 0013 : OpCode.JMPGT FB
    /// 0015 : OpCode.NIP
    /// 0016 : OpCode.DEC
    /// 0017 : OpCode.RET
    /// </remarks>
    [DisplayName("log2Sbyte")]
    public abstract BigInteger? Log2Sbyte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 03
    /// 0008 : OpCode.THROW
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.JMPEQ 0C
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.INC
    /// 000F : OpCode.OVER
    /// 0010 : OpCode.OVER
    /// 0011 : OpCode.SHR
    /// 0012 : OpCode.PUSH0
    /// 0013 : OpCode.JMPGT FB
    /// 0015 : OpCode.NIP
    /// 0016 : OpCode.DEC
    /// 0017 : OpCode.RET
    /// </remarks>
    [DisplayName("log2Short")]
    public abstract BigInteger? Log2Short(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 03
    /// 0008 : OpCode.THROW
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.JMPEQ 0C
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.INC
    /// 000F : OpCode.OVER
    /// 0010 : OpCode.OVER
    /// 0011 : OpCode.SHR
    /// 0012 : OpCode.PUSH0
    /// 0013 : OpCode.JMPGT FB
    /// 0015 : OpCode.NIP
    /// 0016 : OpCode.DEC
    /// 0017 : OpCode.RET
    /// </remarks>
    [DisplayName("log2UInt")]
    public abstract BigInteger? Log2UInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQLgM6ShAoDBCcS0upECz7Rp1A
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.JMPGE 03
    /// 0008 : OpCode.THROW
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.JMPEQ 0C
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.INC
    /// 000F : OpCode.OVER
    /// 0010 : OpCode.OVER
    /// 0011 : OpCode.SHR
    /// 0012 : OpCode.PUSH0
    /// 0013 : OpCode.JMPGT FB
    /// 0015 : OpCode.NIP
    /// 0016 : OpCode.DEC
    /// 0017 : OpCode.RET
    /// </remarks>
    [DisplayName("log2Ushort")]
    public abstract BigInteger? Log2Ushort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoCAAAAgAMAAACAAAAAALsmDgP/////AAAAAJEiRwxWYWx1ZSBvdXQgb2YgcmFuZ2UsIG11c3QgYmUgYmV0d2VlbiBpbnQuTWluVmFsdWUgYW5kIGludC5NYXhWYWx1ZS46EFBKECgMShGRUZ5QEaki9EVA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT32 00000080
    /// 000A : OpCode.PUSHINT64 0000008000000000
    /// 0013 : OpCode.WITHIN
    /// 0014 : OpCode.JMPIFNOT 0E
    /// 0016 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001F : OpCode.AND
    /// 0020 : OpCode.JMP 47
    /// 0022 : OpCode.PUSHDATA1 56616C7565206F7574206F662072616E67652C206D757374206265206265747765656E20696E742E4D696E56616C756520616E6420696E742E4D617856616C75652E
    /// 0066 : OpCode.THROW
    /// 0067 : OpCode.PUSH0
    /// 0068 : OpCode.SWAP
    /// 0069 : OpCode.DUP
    /// 006A : OpCode.PUSH0
    /// 006B : OpCode.JMPEQ 0C
    /// 006D : OpCode.DUP
    /// 006E : OpCode.PUSH1
    /// 006F : OpCode.AND
    /// 0070 : OpCode.ROT
    /// 0071 : OpCode.ADD
    /// 0072 : OpCode.SWAP
    /// 0073 : OpCode.PUSH1
    /// 0074 : OpCode.SHR
    /// 0075 : OpCode.JMP F4
    /// 0077 : OpCode.DROP
    /// 0078 : OpCode.RET
    /// </remarks>
    [DisplayName("popCountBigInteger")]
    public abstract BigInteger? PopCountBigInteger(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAH/AJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT16 FF00
    /// 0007 : OpCode.AND
    /// 0008 : OpCode.PUSH0
    /// 0009 : OpCode.SWAP
    /// 000A : OpCode.DUP
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.JMPEQ 0C
    /// 000E : OpCode.DUP
    /// 000F : OpCode.PUSH1
    /// 0010 : OpCode.AND
    /// 0011 : OpCode.ROT
    /// 0012 : OpCode.ADD
    /// 0013 : OpCode.SWAP
    /// 0014 : OpCode.PUSH1
    /// 0015 : OpCode.SHR
    /// 0016 : OpCode.JMP F4
    /// 0018 : OpCode.DROP
    /// 0019 : OpCode.RET
    /// </remarks>
    [DisplayName("popCountByte")]
    public abstract BigInteger? PopCountByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAP/////AAAAAJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 000D : OpCode.AND
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.SWAP
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSH0
    /// 0012 : OpCode.JMPEQ 0C
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.PUSH1
    /// 0016 : OpCode.AND
    /// 0017 : OpCode.ROT
    /// 0018 : OpCode.ADD
    /// 0019 : OpCode.SWAP
    /// 001A : OpCode.PUSH1
    /// 001B : OpCode.SHR
    /// 001C : OpCode.JMP F4
    /// 001E : OpCode.DROP
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("popCountInt")]
    public abstract BigInteger? PopCountInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAT//////////wAAAAAAAAAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0015 : OpCode.AND
    /// 0016 : OpCode.PUSH0
    /// 0017 : OpCode.SWAP
    /// 0018 : OpCode.DUP
    /// 0019 : OpCode.PUSH0
    /// 001A : OpCode.JMPEQ 0C
    /// 001C : OpCode.DUP
    /// 001D : OpCode.PUSH1
    /// 001E : OpCode.AND
    /// 001F : OpCode.ROT
    /// 0020 : OpCode.ADD
    /// 0021 : OpCode.SWAP
    /// 0022 : OpCode.PUSH1
    /// 0023 : OpCode.SHR
    /// 0024 : OpCode.JMP F4
    /// 0026 : OpCode.DROP
    /// 0027 : OpCode.RET
    /// </remarks>
    [DisplayName("popCountLong")]
    public abstract BigInteger? PopCountLong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAH/AJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT16 FF00
    /// 0007 : OpCode.AND
    /// 0008 : OpCode.PUSH0
    /// 0009 : OpCode.SWAP
    /// 000A : OpCode.DUP
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.JMPEQ 0C
    /// 000E : OpCode.DUP
    /// 000F : OpCode.PUSH1
    /// 0010 : OpCode.AND
    /// 0011 : OpCode.ROT
    /// 0012 : OpCode.ADD
    /// 0013 : OpCode.SWAP
    /// 0014 : OpCode.PUSH1
    /// 0015 : OpCode.SHR
    /// 0016 : OpCode.JMP F4
    /// 0018 : OpCode.DROP
    /// 0019 : OpCode.RET
    /// </remarks>
    [DisplayName("popCountSByte")]
    public abstract BigInteger? PopCountSByte(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAL//wAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT32 FFFF0000
    /// 0009 : OpCode.AND
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.SWAP
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.JMPEQ 0C
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSH1
    /// 0012 : OpCode.AND
    /// 0013 : OpCode.ROT
    /// 0014 : OpCode.ADD
    /// 0015 : OpCode.SWAP
    /// 0016 : OpCode.PUSH1
    /// 0017 : OpCode.SHR
    /// 0018 : OpCode.JMP F4
    /// 001A : OpCode.DROP
    /// 001B : OpCode.RET
    /// </remarks>
    [DisplayName("popCountShort")]
    public abstract BigInteger? PopCountShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAP/////AAAAAJEQUEoQKAxKEZFRnlARqSL0RUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 000D : OpCode.AND
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.SWAP
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSH0
    /// 0012 : OpCode.JMPEQ 0C
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.PUSH1
    /// 0016 : OpCode.AND
    /// 0017 : OpCode.ROT
    /// 0018 : OpCode.ADD
    /// 0019 : OpCode.SWAP
    /// 001A : OpCode.PUSH1
    /// 001B : OpCode.SHR
    /// 001C : OpCode.JMP F4
    /// 001E : OpCode.DROP
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("popCountUInt")]
    public abstract BigInteger? PopCountUInt(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAT//////////wAAAAAAAAAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0015 : OpCode.AND
    /// 0016 : OpCode.PUSH0
    /// 0017 : OpCode.SWAP
    /// 0018 : OpCode.DUP
    /// 0019 : OpCode.PUSH0
    /// 001A : OpCode.JMPEQ 0C
    /// 001C : OpCode.DUP
    /// 001D : OpCode.PUSH1
    /// 001E : OpCode.AND
    /// 001F : OpCode.ROT
    /// 0020 : OpCode.ADD
    /// 0021 : OpCode.SWAP
    /// 0022 : OpCode.PUSH1
    /// 0023 : OpCode.SHR
    /// 0024 : OpCode.JMP F4
    /// 0026 : OpCode.DROP
    /// 0027 : OpCode.RET
    /// </remarks>
    [DisplayName("popCountULong")]
    public abstract BigInteger? PopCountULong(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAL//wAAkRBQShAoDEoRkVGeUBGpIvRFQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT32 FFFF0000
    /// 0009 : OpCode.AND
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.SWAP
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.JMPEQ 0C
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSH1
    /// 0012 : OpCode.AND
    /// 0013 : OpCode.ROT
    /// 0014 : OpCode.ADD
    /// 0015 : OpCode.SWAP
    /// 0016 : OpCode.PUSH1
    /// 0017 : OpCode.SHR
    /// 0018 : OpCode.JMP F4
    /// 001A : OpCode.DROP
    /// 001B : OpCode.RET
    /// </remarks>
    [DisplayName("popCountUShort")]
    public abstract BigInteger? PopCountUShort(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkXkVAB/wCRUKgB/wCReAH/AJF5GFCfF5GpkgH/AJFA
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSH7
    /// 0006 : OpCode.AND
    /// 0007 : OpCode.SWAP
    /// 0008 : OpCode.PUSHINT16 FF00
    /// 000B : OpCode.AND
    /// 000C : OpCode.SWAP
    /// 000D : OpCode.SHL
    /// 000E : OpCode.PUSHINT16 FF00
    /// 0011 : OpCode.AND
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.PUSHINT16 FF00
    /// 0016 : OpCode.AND
    /// 0017 : OpCode.LDARG1
    /// 0018 : OpCode.PUSH8
    /// 0019 : OpCode.SWAP
    /// 001A : OpCode.SUB
    /// 001B : OpCode.PUSH7
    /// 001C : OpCode.AND
    /// 001D : OpCode.SHR
    /// 001E : OpCode.OR
    /// 001F : OpCode.PUSHINT16 FF00
    /// 0022 : OpCode.AND
    /// 0023 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftByte")]
    public abstract BigInteger? RotateLeftByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5FQA/////8AAAAAkVCoA/////8AAAAAkXgD/////wAAAACReQAgUJ8AH5GpkkoDAAAAgAAAAAAwDAMAAAAAAQAAAJ9A
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSHINT8 1F
    /// 0007 : OpCode.AND
    /// 0008 : OpCode.SWAP
    /// 0009 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0012 : OpCode.AND
    /// 0013 : OpCode.SWAP
    /// 0014 : OpCode.SHL
    /// 0015 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001E : OpCode.AND
    /// 001F : OpCode.LDARG0
    /// 0020 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0029 : OpCode.AND
    /// 002A : OpCode.LDARG1
    /// 002B : OpCode.PUSHINT8 20
    /// 002D : OpCode.SWAP
    /// 002E : OpCode.SUB
    /// 002F : OpCode.PUSHINT8 1F
    /// 0031 : OpCode.AND
    /// 0032 : OpCode.SHR
    /// 0033 : OpCode.OR
    /// 0034 : OpCode.DUP
    /// 0035 : OpCode.PUSHINT64 0000008000000000
    /// 003E : OpCode.JMPLT 0C
    /// 0040 : OpCode.PUSHINT64 0000000001000000
    /// 0049 : OpCode.SUB
    /// 004A : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftInt")]
    public abstract BigInteger? RotateLeftInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAP5FQBP//////////AAAAAAAAAACRUKgE//////////8AAAAAAAAAAJF4BP//////////AAAAAAAAAACReQBAUJ8AP5GpkkoEAAAAAAAAAIAAAAAAAAAAADAUBAAAAAAAAAAAAQAAAAAAAACfQA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSHINT8 3F
    /// 0007 : OpCode.AND
    /// 0008 : OpCode.SWAP
    /// 0009 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 001A : OpCode.AND
    /// 001B : OpCode.SWAP
    /// 001C : OpCode.SHL
    /// 001D : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 002E : OpCode.AND
    /// 002F : OpCode.LDARG0
    /// 0030 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0041 : OpCode.AND
    /// 0042 : OpCode.LDARG1
    /// 0043 : OpCode.PUSHINT8 40
    /// 0045 : OpCode.SWAP
    /// 0046 : OpCode.SUB
    /// 0047 : OpCode.PUSHINT8 3F
    /// 0049 : OpCode.AND
    /// 004A : OpCode.SHR
    /// 004B : OpCode.OR
    /// 004C : OpCode.DUP
    /// 004D : OpCode.PUSHINT128 00000000000000800000000000000000
    /// 005E : OpCode.JMPLT 14
    /// 0060 : OpCode.PUSHINT128 00000000000000000100000000000000
    /// 0071 : OpCode.SUB
    /// 0072 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftLong")]
    public abstract BigInteger? RotateLeftLong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkXkVAB/wCRUKgB/wCReAH/AJF5GFCfF5GpkkoBgAAwBgEAAZ9A
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSH7
    /// 0006 : OpCode.AND
    /// 0007 : OpCode.SWAP
    /// 0008 : OpCode.PUSHINT16 FF00
    /// 000B : OpCode.AND
    /// 000C : OpCode.SWAP
    /// 000D : OpCode.SHL
    /// 000E : OpCode.PUSHINT16 FF00
    /// 0011 : OpCode.AND
    /// 0012 : OpCode.LDARG0
    /// 0013 : OpCode.PUSHINT16 FF00
    /// 0016 : OpCode.AND
    /// 0017 : OpCode.LDARG1
    /// 0018 : OpCode.PUSH8
    /// 0019 : OpCode.SWAP
    /// 001A : OpCode.SUB
    /// 001B : OpCode.PUSH7
    /// 001C : OpCode.AND
    /// 001D : OpCode.SHR
    /// 001E : OpCode.OR
    /// 001F : OpCode.DUP
    /// 0020 : OpCode.PUSHINT16 8000
    /// 0023 : OpCode.JMPLT 06
    /// 0025 : OpCode.PUSHINT16 0001
    /// 0028 : OpCode.SUB
    /// 0029 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftSByte")]
    public abstract BigInteger? RotateLeftSByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkfkVAC//8AAJFQqAL//wAAkXgC//8AAJF5IFCfH5GpkkoCAIAAADAIAgAAAQCfQA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSH15
    /// 0006 : OpCode.AND
    /// 0007 : OpCode.SWAP
    /// 0008 : OpCode.PUSHINT32 FFFF0000
    /// 000D : OpCode.AND
    /// 000E : OpCode.SWAP
    /// 000F : OpCode.SHL
    /// 0010 : OpCode.PUSHINT32 FFFF0000
    /// 0015 : OpCode.AND
    /// 0016 : OpCode.LDARG0
    /// 0017 : OpCode.PUSHINT32 FFFF0000
    /// 001C : OpCode.AND
    /// 001D : OpCode.LDARG1
    /// 001E : OpCode.PUSH16
    /// 001F : OpCode.SWAP
    /// 0020 : OpCode.SUB
    /// 0021 : OpCode.PUSH15
    /// 0022 : OpCode.AND
    /// 0023 : OpCode.SHR
    /// 0024 : OpCode.OR
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.PUSHINT32 00800000
    /// 002B : OpCode.JMPLT 08
    /// 002D : OpCode.PUSHINT32 00000100
    /// 0032 : OpCode.SUB
    /// 0033 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftShort")]
    public abstract BigInteger? RotateLeftShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5FQA/////8AAAAAkVCoA/////8AAAAAkXgD/////wAAAACReQAgUJ8AH5GpkgP/////AAAAAJFA
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSHINT8 1F
    /// 0007 : OpCode.AND
    /// 0008 : OpCode.SWAP
    /// 0009 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0012 : OpCode.AND
    /// 0013 : OpCode.SWAP
    /// 0014 : OpCode.SHL
    /// 0015 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001E : OpCode.AND
    /// 001F : OpCode.LDARG0
    /// 0020 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0029 : OpCode.AND
    /// 002A : OpCode.LDARG1
    /// 002B : OpCode.PUSHINT8 20
    /// 002D : OpCode.SWAP
    /// 002E : OpCode.SUB
    /// 002F : OpCode.PUSHINT8 1F
    /// 0031 : OpCode.AND
    /// 0032 : OpCode.SHR
    /// 0033 : OpCode.OR
    /// 0034 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 003D : OpCode.AND
    /// 003E : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftUInt")]
    public abstract BigInteger? RotateLeftUInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAP5FQBP//////////AAAAAAAAAACRUKgE//////////8AAAAAAAAAAJF4BP//////////AAAAAAAAAACReQBAUJ8AP5GpkgT//////////wAAAAAAAAAAkUA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSHINT8 3F
    /// 0007 : OpCode.AND
    /// 0008 : OpCode.SWAP
    /// 0009 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 001A : OpCode.AND
    /// 001B : OpCode.SWAP
    /// 001C : OpCode.SHL
    /// 001D : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 002E : OpCode.AND
    /// 002F : OpCode.LDARG0
    /// 0030 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0041 : OpCode.AND
    /// 0042 : OpCode.LDARG1
    /// 0043 : OpCode.PUSHINT8 40
    /// 0045 : OpCode.SWAP
    /// 0046 : OpCode.SUB
    /// 0047 : OpCode.PUSHINT8 3F
    /// 0049 : OpCode.AND
    /// 004A : OpCode.SHR
    /// 004B : OpCode.OR
    /// 004C : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 005D : OpCode.AND
    /// 005E : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftULong")]
    public abstract BigInteger? RotateLeftULong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkfkVAC//8AAJFQqAL//wAAkXgC//8AAJF5IFCfH5GpkgL//wAAkUA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSH15
    /// 0006 : OpCode.AND
    /// 0007 : OpCode.SWAP
    /// 0008 : OpCode.PUSHINT32 FFFF0000
    /// 000D : OpCode.AND
    /// 000E : OpCode.SWAP
    /// 000F : OpCode.SHL
    /// 0010 : OpCode.PUSHINT32 FFFF0000
    /// 0015 : OpCode.AND
    /// 0016 : OpCode.LDARG0
    /// 0017 : OpCode.PUSHINT32 FFFF0000
    /// 001C : OpCode.AND
    /// 001D : OpCode.LDARG1
    /// 001E : OpCode.PUSH16
    /// 001F : OpCode.SWAP
    /// 0020 : OpCode.SUB
    /// 0021 : OpCode.PUSH15
    /// 0022 : OpCode.AND
    /// 0023 : OpCode.SHR
    /// 0024 : OpCode.OR
    /// 0025 : OpCode.PUSHINT32 FFFF0000
    /// 002A : OpCode.AND
    /// 002B : OpCode.RET
    /// </remarks>
    [DisplayName("rotateLeftUShort")]
    public abstract BigInteger? RotateLeftUShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkXkal4GHmfF5GokgH/AJFA
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSH7
    /// 0006 : OpCode.AND
    /// 0007 : OpCode.SHR
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.PUSH8
    /// 000A : OpCode.LDARG1
    /// 000B : OpCode.SUB
    /// 000C : OpCode.PUSH7
    /// 000D : OpCode.AND
    /// 000E : OpCode.SHL
    /// 000F : OpCode.OR
    /// 0010 : OpCode.PUSHINT16 FF00
    /// 0013 : OpCode.AND
    /// 0014 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightByte")]
    public abstract BigInteger? RotateRightByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5EAIKIAIFCfUAP/////AAAAAJFQqAP/////AAAAAJF4A/////8AAAAAkXkAIKIAIFCfACBQnwAfkamSSgMAAACAAAAAADAMAwAAAAABAAAAn0A=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSHINT8 1F
    /// 0007 : OpCode.AND
    /// 0008 : OpCode.PUSHINT8 20
    /// 000A : OpCode.MOD
    /// 000B : OpCode.PUSHINT8 20
    /// 000D : OpCode.SWAP
    /// 000E : OpCode.SUB
    /// 000F : OpCode.SWAP
    /// 0010 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0019 : OpCode.AND
    /// 001A : OpCode.SWAP
    /// 001B : OpCode.SHL
    /// 001C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0025 : OpCode.AND
    /// 0026 : OpCode.LDARG0
    /// 0027 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0030 : OpCode.AND
    /// 0031 : OpCode.LDARG1
    /// 0032 : OpCode.PUSHINT8 20
    /// 0034 : OpCode.MOD
    /// 0035 : OpCode.PUSHINT8 20
    /// 0037 : OpCode.SWAP
    /// 0038 : OpCode.SUB
    /// 0039 : OpCode.PUSHINT8 20
    /// 003B : OpCode.SWAP
    /// 003C : OpCode.SUB
    /// 003D : OpCode.PUSHINT8 1F
    /// 003F : OpCode.AND
    /// 0040 : OpCode.SHR
    /// 0041 : OpCode.OR
    /// 0042 : OpCode.DUP
    /// 0043 : OpCode.PUSHINT64 0000008000000000
    /// 004C : OpCode.JMPLT 0C
    /// 004E : OpCode.PUSHINT64 0000000001000000
    /// 0057 : OpCode.SUB
    /// 0058 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightInt")]
    public abstract BigInteger? RotateRightInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAP5EAQKIAQFCfUAT//////////wAAAAAAAAAAkVCoBP//////////AAAAAAAAAACReAT//////////wAAAAAAAAAAkXkAQKIAQFCfAEBQnwA/kamSSgQAAAAAAAAAgAAAAAAAAAAAMBQEAAAAAAAAAAABAAAAAAAAAJ9A
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSHINT8 3F
    /// 0007 : OpCode.AND
    /// 0008 : OpCode.PUSHINT8 40
    /// 000A : OpCode.MOD
    /// 000B : OpCode.PUSHINT8 40
    /// 000D : OpCode.SWAP
    /// 000E : OpCode.SUB
    /// 000F : OpCode.SWAP
    /// 0010 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0021 : OpCode.AND
    /// 0022 : OpCode.SWAP
    /// 0023 : OpCode.SHL
    /// 0024 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0035 : OpCode.AND
    /// 0036 : OpCode.LDARG0
    /// 0037 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0048 : OpCode.AND
    /// 0049 : OpCode.LDARG1
    /// 004A : OpCode.PUSHINT8 40
    /// 004C : OpCode.MOD
    /// 004D : OpCode.PUSHINT8 40
    /// 004F : OpCode.SWAP
    /// 0050 : OpCode.SUB
    /// 0051 : OpCode.PUSHINT8 40
    /// 0053 : OpCode.SWAP
    /// 0054 : OpCode.SUB
    /// 0055 : OpCode.PUSHINT8 3F
    /// 0057 : OpCode.AND
    /// 0058 : OpCode.SHR
    /// 0059 : OpCode.OR
    /// 005A : OpCode.DUP
    /// 005B : OpCode.PUSHINT128 00000000000000800000000000000000
    /// 006C : OpCode.JMPLT 14
    /// 006E : OpCode.PUSHINT128 00000000000000000100000000000000
    /// 007F : OpCode.SUB
    /// 0080 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightLong")]
    public abstract BigInteger? RotateRightLong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkXkRiiGFCfUAH/AJFQqAH/AJF4Af8AkXkYohhQnxhQnxeRqZJKAYAAMAYBAAGfQA==
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSH7
    /// 0006 : OpCode.AND
    /// 0007 : OpCode.PUSH8
    /// 0008 : OpCode.MOD
    /// 0009 : OpCode.PUSH8
    /// 000A : OpCode.SWAP
    /// 000B : OpCode.SUB
    /// 000C : OpCode.SWAP
    /// 000D : OpCode.PUSHINT16 FF00
    /// 0010 : OpCode.AND
    /// 0011 : OpCode.SWAP
    /// 0012 : OpCode.SHL
    /// 0013 : OpCode.PUSHINT16 FF00
    /// 0016 : OpCode.AND
    /// 0017 : OpCode.LDARG0
    /// 0018 : OpCode.PUSHINT16 FF00
    /// 001B : OpCode.AND
    /// 001C : OpCode.LDARG1
    /// 001D : OpCode.PUSH8
    /// 001E : OpCode.MOD
    /// 001F : OpCode.PUSH8
    /// 0020 : OpCode.SWAP
    /// 0021 : OpCode.SUB
    /// 0022 : OpCode.PUSH8
    /// 0023 : OpCode.SWAP
    /// 0024 : OpCode.SUB
    /// 0025 : OpCode.PUSH7
    /// 0026 : OpCode.AND
    /// 0027 : OpCode.SHR
    /// 0028 : OpCode.OR
    /// 0029 : OpCode.DUP
    /// 002A : OpCode.PUSHINT16 8000
    /// 002D : OpCode.JMPLT 06
    /// 002F : OpCode.PUSHINT16 0001
    /// 0032 : OpCode.SUB
    /// 0033 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightSByte")]
    public abstract BigInteger? RotateRightSByte(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkfkSCiIFCfUAL//wAAkVCoAv//AACReAL//wAAkXkgoiBQnyBQnx+RqZJKAgCAAAAwCAIAAAEAn0A=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSH15
    /// 0006 : OpCode.AND
    /// 0007 : OpCode.PUSH16
    /// 0008 : OpCode.MOD
    /// 0009 : OpCode.PUSH16
    /// 000A : OpCode.SWAP
    /// 000B : OpCode.SUB
    /// 000C : OpCode.SWAP
    /// 000D : OpCode.PUSHINT32 FFFF0000
    /// 0012 : OpCode.AND
    /// 0013 : OpCode.SWAP
    /// 0014 : OpCode.SHL
    /// 0015 : OpCode.PUSHINT32 FFFF0000
    /// 001A : OpCode.AND
    /// 001B : OpCode.LDARG0
    /// 001C : OpCode.PUSHINT32 FFFF0000
    /// 0021 : OpCode.AND
    /// 0022 : OpCode.LDARG1
    /// 0023 : OpCode.PUSH16
    /// 0024 : OpCode.MOD
    /// 0025 : OpCode.PUSH16
    /// 0026 : OpCode.SWAP
    /// 0027 : OpCode.SUB
    /// 0028 : OpCode.PUSH16
    /// 0029 : OpCode.SWAP
    /// 002A : OpCode.SUB
    /// 002B : OpCode.PUSH15
    /// 002C : OpCode.AND
    /// 002D : OpCode.SHR
    /// 002E : OpCode.OR
    /// 002F : OpCode.DUP
    /// 0030 : OpCode.PUSHINT32 00800000
    /// 0035 : OpCode.JMPLT 08
    /// 0037 : OpCode.PUSHINT32 00000100
    /// 003C : OpCode.SUB
    /// 003D : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightShort")]
    public abstract BigInteger? RotateRightShort(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAH5GpeAAgeZ8AH5GokgP/////AAAAAJFA
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSHINT8 1F
    /// 0007 : OpCode.AND
    /// 0008 : OpCode.SHR
    /// 0009 : OpCode.LDARG0
    /// 000A : OpCode.PUSHINT8 20
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.SUB
    /// 000E : OpCode.PUSHINT8 1F
    /// 0010 : OpCode.AND
    /// 0011 : OpCode.SHL
    /// 0012 : OpCode.OR
    /// 0013 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 001C : OpCode.AND
    /// 001D : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightUInt")]
    public abstract BigInteger? RotateRightUInt(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkAP5GpeABAeZ8AP5GokgT//////////wAAAAAAAAAAkUA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSHINT8 3F
    /// 0007 : OpCode.AND
    /// 0008 : OpCode.SHR
    /// 0009 : OpCode.LDARG0
    /// 000A : OpCode.PUSHINT8 40
    /// 000C : OpCode.LDARG1
    /// 000D : OpCode.SUB
    /// 000E : OpCode.PUSHINT8 3F
    /// 0010 : OpCode.AND
    /// 0011 : OpCode.SHL
    /// 0012 : OpCode.OR
    /// 0013 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 0024 : OpCode.AND
    /// 0025 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightULong")]
    public abstract BigInteger? RotateRightULong(BigInteger? value, BigInteger? offset);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHkfkal4IHmfH5GokgL//wAAkUA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.PUSH15
    /// 0006 : OpCode.AND
    /// 0007 : OpCode.SHR
    /// 0008 : OpCode.LDARG0
    /// 0009 : OpCode.PUSH16
    /// 000A : OpCode.LDARG1
    /// 000B : OpCode.SUB
    /// 000C : OpCode.PUSH15
    /// 000D : OpCode.AND
    /// 000E : OpCode.SHL
    /// 000F : OpCode.OR
    /// 0010 : OpCode.PUSHINT32 FFFF0000
    /// 0015 : OpCode.AND
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("rotateRightUShort")]
    public abstract BigInteger? RotateRightUShort(BigInteger? value, BigInteger? offset);

    #endregion

}
