using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Syscall_Out(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Syscall_Out"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testByteTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testSByteTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":39,""safe"":false},{""name"":""testShortTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":79,""safe"":false},{""name"":""testUShortTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":122,""safe"":false},{""name"":""testIntTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":163,""safe"":false},{""name"":""testUIntTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":212,""safe"":false},{""name"":""testLongTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":257,""safe"":false},{""name"":""testULongTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":318,""safe"":false},{""name"":""testBoolTryParse"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""Array"",""offset"":375,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":600,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAA/VsCVwEBEGBYeFBFNwAAStgkDkoQAQABuyYGYAgiBEUJcMVKaM9KWM9AVwEBEGFZeFBFNwAAStgkD0oAgAGAALsmBmEIIgRFCXDFSmjPSlnPQFcBARBiWnhQRTcAAErYJBJKAQCAAgCAAAC7JgZiCCIERQlwxUpoz0paz0BXAQEQY1t4UEU3AABK2CQQShACAAABALsmBmMIIgRFCXDFSmjPSlvPQFcBARBkXHhQRTcAAErYJBhKAgAAAIADAAAAgAAAAAC7JgZkCCIERQlwxUpoz0pcz0BXAQEQZV14UEU3AABK2CQUShADAAAAAAEAAAC7JgZlCCIERQlwxUpoz0pdz0BXAQEQZl54UEU3AABK2CQkSgMAAAAAAAAAgAQAAAAAAAAAgAAAAAAAAAAAuyYGZggiBEUJcMVKaM9KXs9AVwEBEGcHXwd4UEU3AABK2CQdShAEAAAAAAAAAAABAAAAAAAAALsmB2cHCCIERQlwxUpoz0pfB89AVwEBEGcIXwh4UEVKDAR0cnVllyW4AAAASgwEVFJVRZclqwAAAEoMBFRydWWXJZ4AAABKDAF0lyWUAAAASgwBVJcligAAAEoMATGXJYAAAABKDAN5ZXOXJHRKDANZRVOXJGtKDAF5lyRkSgwBWZckXUoMBWZhbHNllyRZSgwFRkFMU0WXJE5KDAVGYWxzZZckQ0oMAWaXJDxKDAFGlyQ1SgwBMJckLkoMAm5vlyQmSgwCTk+XJB5KDAFulyQXSgwBTpckEEUJZwgJIg5FCGcICCIHRQlnCAhwxUpoz0pfCM9AVglA7icQAw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGcIXwh4UEVKDHRydWWXJbgAAABKDFRSVUWXJasAAABKDFRydWWXJZ4AAABKDHSXJZQAAABKDFSXJYoAAABKDDGXJYAAAABKDHllc5ckdEoMWUVTlyRrSgx5lyRkSgxZlyRdSgxmYWxzZZckWUoMRkFMU0WXJE5KDEZhbHNllyRDSgxmlyQ8SgxGlyQ1SgwwlyQuSgxub5ckJkoMTk+XJB5KDG6XJBdKDE6XJBBFCWcICSIORQhnCAgiB0UJZwgIcMVKaM9KXwjPQA==
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.PUSH0	[1 datoshi]
    /// 04 : OpCode.STSFLD 08	[2 datoshi]
    /// 06 : OpCode.LDSFLD 08	[2 datoshi]
    /// 08 : OpCode.LDARG0	[2 datoshi]
    /// 09 : OpCode.SWAP	[2 datoshi]
    /// 0A : OpCode.DROP	[2 datoshi]
    /// 0B : OpCode.DUP	[2 datoshi]
    /// 0C : OpCode.PUSHDATA1 74727565	[8 datoshi]
    /// 12 : OpCode.EQUAL	[32 datoshi]
    /// 13 : OpCode.JMPIF_L B8000000	[2 datoshi]
    /// 18 : OpCode.DUP	[2 datoshi]
    /// 19 : OpCode.PUSHDATA1 54525545	[8 datoshi]
    /// 1F : OpCode.EQUAL	[32 datoshi]
    /// 20 : OpCode.JMPIF_L AB000000	[2 datoshi]
    /// 25 : OpCode.DUP	[2 datoshi]
    /// 26 : OpCode.PUSHDATA1 54727565	[8 datoshi]
    /// 2C : OpCode.EQUAL	[32 datoshi]
    /// 2D : OpCode.JMPIF_L 9E000000	[2 datoshi]
    /// 32 : OpCode.DUP	[2 datoshi]
    /// 33 : OpCode.PUSHDATA1 74	[8 datoshi]
    /// 36 : OpCode.EQUAL	[32 datoshi]
    /// 37 : OpCode.JMPIF_L 94000000	[2 datoshi]
    /// 3C : OpCode.DUP	[2 datoshi]
    /// 3D : OpCode.PUSHDATA1 54	[8 datoshi]
    /// 40 : OpCode.EQUAL	[32 datoshi]
    /// 41 : OpCode.JMPIF_L 8A000000	[2 datoshi]
    /// 46 : OpCode.DUP	[2 datoshi]
    /// 47 : OpCode.PUSHDATA1 31	[8 datoshi]
    /// 4A : OpCode.EQUAL	[32 datoshi]
    /// 4B : OpCode.JMPIF_L 80000000	[2 datoshi]
    /// 50 : OpCode.DUP	[2 datoshi]
    /// 51 : OpCode.PUSHDATA1 796573	[8 datoshi]
    /// 56 : OpCode.EQUAL	[32 datoshi]
    /// 57 : OpCode.JMPIF 74	[2 datoshi]
    /// 59 : OpCode.DUP	[2 datoshi]
    /// 5A : OpCode.PUSHDATA1 594553	[8 datoshi]
    /// 5F : OpCode.EQUAL	[32 datoshi]
    /// 60 : OpCode.JMPIF 6B	[2 datoshi]
    /// 62 : OpCode.DUP	[2 datoshi]
    /// 63 : OpCode.PUSHDATA1 79	[8 datoshi]
    /// 66 : OpCode.EQUAL	[32 datoshi]
    /// 67 : OpCode.JMPIF 64	[2 datoshi]
    /// 69 : OpCode.DUP	[2 datoshi]
    /// 6A : OpCode.PUSHDATA1 59	[8 datoshi]
    /// 6D : OpCode.EQUAL	[32 datoshi]
    /// 6E : OpCode.JMPIF 5D	[2 datoshi]
    /// 70 : OpCode.DUP	[2 datoshi]
    /// 71 : OpCode.PUSHDATA1 66616C7365	[8 datoshi]
    /// 78 : OpCode.EQUAL	[32 datoshi]
    /// 79 : OpCode.JMPIF 59	[2 datoshi]
    /// 7B : OpCode.DUP	[2 datoshi]
    /// 7C : OpCode.PUSHDATA1 46414C5345	[8 datoshi]
    /// 83 : OpCode.EQUAL	[32 datoshi]
    /// 84 : OpCode.JMPIF 4E	[2 datoshi]
    /// 86 : OpCode.DUP	[2 datoshi]
    /// 87 : OpCode.PUSHDATA1 46616C7365	[8 datoshi]
    /// 8E : OpCode.EQUAL	[32 datoshi]
    /// 8F : OpCode.JMPIF 43	[2 datoshi]
    /// 91 : OpCode.DUP	[2 datoshi]
    /// 92 : OpCode.PUSHDATA1 66	[8 datoshi]
    /// 95 : OpCode.EQUAL	[32 datoshi]
    /// 96 : OpCode.JMPIF 3C	[2 datoshi]
    /// 98 : OpCode.DUP	[2 datoshi]
    /// 99 : OpCode.PUSHDATA1 46	[8 datoshi]
    /// 9C : OpCode.EQUAL	[32 datoshi]
    /// 9D : OpCode.JMPIF 35	[2 datoshi]
    /// 9F : OpCode.DUP	[2 datoshi]
    /// A0 : OpCode.PUSHDATA1 30	[8 datoshi]
    /// A3 : OpCode.EQUAL	[32 datoshi]
    /// A4 : OpCode.JMPIF 2E	[2 datoshi]
    /// A6 : OpCode.DUP	[2 datoshi]
    /// A7 : OpCode.PUSHDATA1 6E6F	[8 datoshi]
    /// AB : OpCode.EQUAL	[32 datoshi]
    /// AC : OpCode.JMPIF 26	[2 datoshi]
    /// AE : OpCode.DUP	[2 datoshi]
    /// AF : OpCode.PUSHDATA1 4E4F	[8 datoshi]
    /// B3 : OpCode.EQUAL	[32 datoshi]
    /// B4 : OpCode.JMPIF 1E	[2 datoshi]
    /// B6 : OpCode.DUP	[2 datoshi]
    /// B7 : OpCode.PUSHDATA1 6E	[8 datoshi]
    /// BA : OpCode.EQUAL	[32 datoshi]
    /// BB : OpCode.JMPIF 17	[2 datoshi]
    /// BD : OpCode.DUP	[2 datoshi]
    /// BE : OpCode.PUSHDATA1 4E	[8 datoshi]
    /// C1 : OpCode.EQUAL	[32 datoshi]
    /// C2 : OpCode.JMPIF 10	[2 datoshi]
    /// C4 : OpCode.DROP	[2 datoshi]
    /// C5 : OpCode.PUSHF	[1 datoshi]
    /// C6 : OpCode.STSFLD 08	[2 datoshi]
    /// C8 : OpCode.PUSHF	[1 datoshi]
    /// C9 : OpCode.JMP 0E	[2 datoshi]
    /// CB : OpCode.DROP	[2 datoshi]
    /// CC : OpCode.PUSHT	[1 datoshi]
    /// CD : OpCode.STSFLD 08	[2 datoshi]
    /// CF : OpCode.PUSHT	[1 datoshi]
    /// D0 : OpCode.JMP 07	[2 datoshi]
    /// D2 : OpCode.DROP	[2 datoshi]
    /// D3 : OpCode.PUSHF	[1 datoshi]
    /// D4 : OpCode.STSFLD 08	[2 datoshi]
    /// D6 : OpCode.PUSHT	[1 datoshi]
    /// D7 : OpCode.STLOC0	[2 datoshi]
    /// D8 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// D9 : OpCode.DUP	[2 datoshi]
    /// DA : OpCode.LDLOC0	[2 datoshi]
    /// DB : OpCode.APPEND	[8192 datoshi]
    /// DC : OpCode.DUP	[2 datoshi]
    /// DD : OpCode.LDSFLD 08	[2 datoshi]
    /// DF : OpCode.APPEND	[8192 datoshi]
    /// E0 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testBoolTryParse")]
    public abstract IList<object>? TestBoolTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGBYeFBFNwAAStgkDkoQAQABuyYGYAgiBEUJcMVKaM9KWM9A
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.PUSH0	[1 datoshi]
    /// 04 : OpCode.STSFLD0	[2 datoshi]
    /// 05 : OpCode.LDSFLD0	[2 datoshi]
    /// 06 : OpCode.LDARG0	[2 datoshi]
    /// 07 : OpCode.SWAP	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.CALLT 0000	[32768 datoshi]
    /// 0C : OpCode.DUP	[2 datoshi]
    /// 0D : OpCode.ISNULL	[2 datoshi]
    /// 0E : OpCode.JMPIF 0E	[2 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSH0	[1 datoshi]
    /// 12 : OpCode.PUSHINT16 0001	[1 datoshi]
    /// 15 : OpCode.WITHIN	[8 datoshi]
    /// 16 : OpCode.JMPIFNOT 06	[2 datoshi]
    /// 18 : OpCode.STSFLD0	[2 datoshi]
    /// 19 : OpCode.PUSHT	[1 datoshi]
    /// 1A : OpCode.JMP 04	[2 datoshi]
    /// 1C : OpCode.DROP	[2 datoshi]
    /// 1D : OpCode.PUSHF	[1 datoshi]
    /// 1E : OpCode.STLOC0	[2 datoshi]
    /// 1F : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 20 : OpCode.DUP	[2 datoshi]
    /// 21 : OpCode.LDLOC0	[2 datoshi]
    /// 22 : OpCode.APPEND	[8192 datoshi]
    /// 23 : OpCode.DUP	[2 datoshi]
    /// 24 : OpCode.LDSFLD0	[2 datoshi]
    /// 25 : OpCode.APPEND	[8192 datoshi]
    /// 26 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testByteTryParse")]
    public abstract IList<object>? TestByteTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGRceFBFNwAAStgkGEoCAAAAgAMAAACAAAAAALsmBmQIIgRFCXDFSmjPSlzPQA==
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.PUSH0	[1 datoshi]
    /// 04 : OpCode.STSFLD4	[2 datoshi]
    /// 05 : OpCode.LDSFLD4	[2 datoshi]
    /// 06 : OpCode.LDARG0	[2 datoshi]
    /// 07 : OpCode.SWAP	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.CALLT 0000	[32768 datoshi]
    /// 0C : OpCode.DUP	[2 datoshi]
    /// 0D : OpCode.ISNULL	[2 datoshi]
    /// 0E : OpCode.JMPIF 18	[2 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 16 : OpCode.PUSHINT64 0000008000000000	[1 datoshi]
    /// 1F : OpCode.WITHIN	[8 datoshi]
    /// 20 : OpCode.JMPIFNOT 06	[2 datoshi]
    /// 22 : OpCode.STSFLD4	[2 datoshi]
    /// 23 : OpCode.PUSHT	[1 datoshi]
    /// 24 : OpCode.JMP 04	[2 datoshi]
    /// 26 : OpCode.DROP	[2 datoshi]
    /// 27 : OpCode.PUSHF	[1 datoshi]
    /// 28 : OpCode.STLOC0	[2 datoshi]
    /// 29 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 2A : OpCode.DUP	[2 datoshi]
    /// 2B : OpCode.LDLOC0	[2 datoshi]
    /// 2C : OpCode.APPEND	[8192 datoshi]
    /// 2D : OpCode.DUP	[2 datoshi]
    /// 2E : OpCode.LDSFLD4	[2 datoshi]
    /// 2F : OpCode.APPEND	[8192 datoshi]
    /// 30 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testIntTryParse")]
    public abstract IList<object>? TestIntTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGZeeFBFNwAAStgkJEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALsmBmYIIgRFCXDFSmjPSl7PQA==
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.PUSH0	[1 datoshi]
    /// 04 : OpCode.STSFLD6	[2 datoshi]
    /// 05 : OpCode.LDSFLD6	[2 datoshi]
    /// 06 : OpCode.LDARG0	[2 datoshi]
    /// 07 : OpCode.SWAP	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.CALLT 0000	[32768 datoshi]
    /// 0C : OpCode.DUP	[2 datoshi]
    /// 0D : OpCode.ISNULL	[2 datoshi]
    /// 0E : OpCode.JMPIF 24	[2 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSHINT64 0000000000000080	[1 datoshi]
    /// 1A : OpCode.PUSHINT128 00000000000000800000000000000000	[4 datoshi]
    /// 2B : OpCode.WITHIN	[8 datoshi]
    /// 2C : OpCode.JMPIFNOT 06	[2 datoshi]
    /// 2E : OpCode.STSFLD6	[2 datoshi]
    /// 2F : OpCode.PUSHT	[1 datoshi]
    /// 30 : OpCode.JMP 04	[2 datoshi]
    /// 32 : OpCode.DROP	[2 datoshi]
    /// 33 : OpCode.PUSHF	[1 datoshi]
    /// 34 : OpCode.STLOC0	[2 datoshi]
    /// 35 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 36 : OpCode.DUP	[2 datoshi]
    /// 37 : OpCode.LDLOC0	[2 datoshi]
    /// 38 : OpCode.APPEND	[8192 datoshi]
    /// 39 : OpCode.DUP	[2 datoshi]
    /// 3A : OpCode.LDSFLD6	[2 datoshi]
    /// 3B : OpCode.APPEND	[8192 datoshi]
    /// 3C : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testLongTryParse")]
    public abstract IList<object>? TestLongTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGFZeFBFNwAAStgkD0oAgAGAALsmBmEIIgRFCXDFSmjPSlnPQA==
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.PUSH0	[1 datoshi]
    /// 04 : OpCode.STSFLD1	[2 datoshi]
    /// 05 : OpCode.LDSFLD1	[2 datoshi]
    /// 06 : OpCode.LDARG0	[2 datoshi]
    /// 07 : OpCode.SWAP	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.CALLT 0000	[32768 datoshi]
    /// 0C : OpCode.DUP	[2 datoshi]
    /// 0D : OpCode.ISNULL	[2 datoshi]
    /// 0E : OpCode.JMPIF 0F	[2 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSHINT8 80	[1 datoshi]
    /// 13 : OpCode.PUSHINT16 8000	[1 datoshi]
    /// 16 : OpCode.WITHIN	[8 datoshi]
    /// 17 : OpCode.JMPIFNOT 06	[2 datoshi]
    /// 19 : OpCode.STSFLD1	[2 datoshi]
    /// 1A : OpCode.PUSHT	[1 datoshi]
    /// 1B : OpCode.JMP 04	[2 datoshi]
    /// 1D : OpCode.DROP	[2 datoshi]
    /// 1E : OpCode.PUSHF	[1 datoshi]
    /// 1F : OpCode.STLOC0	[2 datoshi]
    /// 20 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 21 : OpCode.DUP	[2 datoshi]
    /// 22 : OpCode.LDLOC0	[2 datoshi]
    /// 23 : OpCode.APPEND	[8192 datoshi]
    /// 24 : OpCode.DUP	[2 datoshi]
    /// 25 : OpCode.LDSFLD1	[2 datoshi]
    /// 26 : OpCode.APPEND	[8192 datoshi]
    /// 27 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testSByteTryParse")]
    public abstract IList<object>? TestSByteTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGJaeFBFNwAAStgkEkoBAIACAIAAALsmBmIIIgRFCXDFSmjPSlrPQA==
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.PUSH0	[1 datoshi]
    /// 04 : OpCode.STSFLD2	[2 datoshi]
    /// 05 : OpCode.LDSFLD2	[2 datoshi]
    /// 06 : OpCode.LDARG0	[2 datoshi]
    /// 07 : OpCode.SWAP	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.CALLT 0000	[32768 datoshi]
    /// 0C : OpCode.DUP	[2 datoshi]
    /// 0D : OpCode.ISNULL	[2 datoshi]
    /// 0E : OpCode.JMPIF 12	[2 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSHINT16 0080	[1 datoshi]
    /// 14 : OpCode.PUSHINT32 00800000	[1 datoshi]
    /// 19 : OpCode.WITHIN	[8 datoshi]
    /// 1A : OpCode.JMPIFNOT 06	[2 datoshi]
    /// 1C : OpCode.STSFLD2	[2 datoshi]
    /// 1D : OpCode.PUSHT	[1 datoshi]
    /// 1E : OpCode.JMP 04	[2 datoshi]
    /// 20 : OpCode.DROP	[2 datoshi]
    /// 21 : OpCode.PUSHF	[1 datoshi]
    /// 22 : OpCode.STLOC0	[2 datoshi]
    /// 23 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 24 : OpCode.DUP	[2 datoshi]
    /// 25 : OpCode.LDLOC0	[2 datoshi]
    /// 26 : OpCode.APPEND	[8192 datoshi]
    /// 27 : OpCode.DUP	[2 datoshi]
    /// 28 : OpCode.LDSFLD2	[2 datoshi]
    /// 29 : OpCode.APPEND	[8192 datoshi]
    /// 2A : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testShortTryParse")]
    public abstract IList<object>? TestShortTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGVdeFBFNwAAStgkFEoQAwAAAAABAAAAuyYGZQgiBEUJcMVKaM9KXc9A
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.PUSH0	[1 datoshi]
    /// 04 : OpCode.STSFLD5	[2 datoshi]
    /// 05 : OpCode.LDSFLD5	[2 datoshi]
    /// 06 : OpCode.LDARG0	[2 datoshi]
    /// 07 : OpCode.SWAP	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.CALLT 0000	[32768 datoshi]
    /// 0C : OpCode.DUP	[2 datoshi]
    /// 0D : OpCode.ISNULL	[2 datoshi]
    /// 0E : OpCode.JMPIF 14	[2 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSH0	[1 datoshi]
    /// 12 : OpCode.PUSHINT64 0000000001000000	[1 datoshi]
    /// 1B : OpCode.WITHIN	[8 datoshi]
    /// 1C : OpCode.JMPIFNOT 06	[2 datoshi]
    /// 1E : OpCode.STSFLD5	[2 datoshi]
    /// 1F : OpCode.PUSHT	[1 datoshi]
    /// 20 : OpCode.JMP 04	[2 datoshi]
    /// 22 : OpCode.DROP	[2 datoshi]
    /// 23 : OpCode.PUSHF	[1 datoshi]
    /// 24 : OpCode.STLOC0	[2 datoshi]
    /// 25 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 26 : OpCode.DUP	[2 datoshi]
    /// 27 : OpCode.LDLOC0	[2 datoshi]
    /// 28 : OpCode.APPEND	[8192 datoshi]
    /// 29 : OpCode.DUP	[2 datoshi]
    /// 2A : OpCode.LDSFLD5	[2 datoshi]
    /// 2B : OpCode.APPEND	[8192 datoshi]
    /// 2C : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUIntTryParse")]
    public abstract IList<object>? TestUIntTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGcHXwd4UEU3AABK2CQdShAEAAAAAAAAAAABAAAAAAAAALsmB2cHCCIERQlwxUpoz0pfB89A
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.PUSH0	[1 datoshi]
    /// 04 : OpCode.STSFLD 07	[2 datoshi]
    /// 06 : OpCode.LDSFLD 07	[2 datoshi]
    /// 08 : OpCode.LDARG0	[2 datoshi]
    /// 09 : OpCode.SWAP	[2 datoshi]
    /// 0A : OpCode.DROP	[2 datoshi]
    /// 0B : OpCode.CALLT 0000	[32768 datoshi]
    /// 0E : OpCode.DUP	[2 datoshi]
    /// 0F : OpCode.ISNULL	[2 datoshi]
    /// 10 : OpCode.JMPIF 1D	[2 datoshi]
    /// 12 : OpCode.DUP	[2 datoshi]
    /// 13 : OpCode.PUSH0	[1 datoshi]
    /// 14 : OpCode.PUSHINT128 00000000000000000100000000000000	[4 datoshi]
    /// 25 : OpCode.WITHIN	[8 datoshi]
    /// 26 : OpCode.JMPIFNOT 07	[2 datoshi]
    /// 28 : OpCode.STSFLD 07	[2 datoshi]
    /// 2A : OpCode.PUSHT	[1 datoshi]
    /// 2B : OpCode.JMP 04	[2 datoshi]
    /// 2D : OpCode.DROP	[2 datoshi]
    /// 2E : OpCode.PUSHF	[1 datoshi]
    /// 2F : OpCode.STLOC0	[2 datoshi]
    /// 30 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 31 : OpCode.DUP	[2 datoshi]
    /// 32 : OpCode.LDLOC0	[2 datoshi]
    /// 33 : OpCode.APPEND	[8192 datoshi]
    /// 34 : OpCode.DUP	[2 datoshi]
    /// 35 : OpCode.LDSFLD 07	[2 datoshi]
    /// 37 : OpCode.APPEND	[8192 datoshi]
    /// 38 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testULongTryParse")]
    public abstract IList<object>? TestULongTryParse(string? s);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEGNbeFBFNwAAStgkEEoQAgAAAQC7JgZjCCIERQlwxUpoz0pbz0A=
    /// 00 : OpCode.INITSLOT 0101	[64 datoshi]
    /// 03 : OpCode.PUSH0	[1 datoshi]
    /// 04 : OpCode.STSFLD3	[2 datoshi]
    /// 05 : OpCode.LDSFLD3	[2 datoshi]
    /// 06 : OpCode.LDARG0	[2 datoshi]
    /// 07 : OpCode.SWAP	[2 datoshi]
    /// 08 : OpCode.DROP	[2 datoshi]
    /// 09 : OpCode.CALLT 0000	[32768 datoshi]
    /// 0C : OpCode.DUP	[2 datoshi]
    /// 0D : OpCode.ISNULL	[2 datoshi]
    /// 0E : OpCode.JMPIF 10	[2 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSH0	[1 datoshi]
    /// 12 : OpCode.PUSHINT32 00000100	[1 datoshi]
    /// 17 : OpCode.WITHIN	[8 datoshi]
    /// 18 : OpCode.JMPIFNOT 06	[2 datoshi]
    /// 1A : OpCode.STSFLD3	[2 datoshi]
    /// 1B : OpCode.PUSHT	[1 datoshi]
    /// 1C : OpCode.JMP 04	[2 datoshi]
    /// 1E : OpCode.DROP	[2 datoshi]
    /// 1F : OpCode.PUSHF	[1 datoshi]
    /// 20 : OpCode.STLOC0	[2 datoshi]
    /// 21 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 22 : OpCode.DUP	[2 datoshi]
    /// 23 : OpCode.LDLOC0	[2 datoshi]
    /// 24 : OpCode.APPEND	[8192 datoshi]
    /// 25 : OpCode.DUP	[2 datoshi]
    /// 26 : OpCode.LDSFLD3	[2 datoshi]
    /// 27 : OpCode.APPEND	[8192 datoshi]
    /// 28 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("testUShortTryParse")]
    public abstract IList<object>? TestUShortTryParse(string? s);

    #endregion
}
