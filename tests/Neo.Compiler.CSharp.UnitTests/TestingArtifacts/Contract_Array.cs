using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Array(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Array"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getTreeByteLengthPrefix"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""getTreeByteLengthPrefix2"",""parameters"":[],""returntype"":""ByteArray"",""offset"":2,""safe"":false},{""name"":""testJaggedArray"",""parameters"":[],""returntype"":""Array"",""offset"":4,""safe"":false},{""name"":""testJaggedByteArray"",""parameters"":[],""returntype"":""Array"",""offset"":42,""safe"":false},{""name"":""testIntArray"",""parameters"":[],""returntype"":""Array"",""offset"":88,""safe"":false},{""name"":""testDefaultArray"",""parameters"":[],""returntype"":""Boolean"",""offset"":118,""safe"":false},{""name"":""testIntArrayInit"",""parameters"":[],""returntype"":""Array"",""offset"":136,""safe"":false},{""name"":""testIntArrayInit2"",""parameters"":[],""returntype"":""Array"",""offset"":161,""safe"":false},{""name"":""testIntArrayInit3"",""parameters"":[],""returntype"":""Array"",""offset"":186,""safe"":false},{""name"":""testDynamicArrayInit"",""parameters"":[{""name"":""length"",""type"":""Integer""}],""returntype"":""Array"",""offset"":211,""safe"":false},{""name"":""testDynamicArrayStringInit"",""parameters"":[{""name"":""input"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":287,""safe"":false},{""name"":""testStructArray"",""parameters"":[],""returntype"":""Any"",""offset"":294,""safe"":false},{""name"":""testDefaultState"",""parameters"":[],""returntype"":""Any"",""offset"":318,""safe"":false},{""name"":""testEmptyArray"",""parameters"":[],""returntype"":""Array"",""offset"":329,""safe"":false},{""name"":""testStructArrayInit"",""parameters"":[],""returntype"":""Any"",""offset"":336,""safe"":false},{""name"":""testByteArrayOwner"",""parameters"":[],""returntype"":""ByteArray"",""offset"":385,""safe"":false},{""name"":""testByteArrayOwnerCall"",""parameters"":[],""returntype"":""ByteArray"",""offset"":387,""safe"":false},{""name"":""testSupportedStandards"",""parameters"":[],""returntype"":""Array"",""offset"":390,""safe"":false},{""name"":""testElementBinding"",""parameters"":[],""returntype"":""Void"",""offset"":392,""safe"":false},{""name"":""testCollectionexpressions"",""parameters"":[],""returntype"":""Array"",""offset"":428,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":511,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""getBlock""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAK+8gQxQDYqd8FQmcfmTBL3ALZl2ghnZXRCbG9jawEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/TsCWEBZQFcEABQTEhEUwHAYFxYVFMBxERITERTAchITFBUUwHNramloFMBAVwQADAQBAgME2zBwDAQFBgcI2zBxDAQBAwIB2zByDAQFBAMC2zBza2ppaBTAQFcBABPEIXAQSmgQUdBFEUpoEVHQRRJKaBJR0EVoQFcBABPEIXBoEM4QlyYECEAJQFcBABMSERPAcBRKaBFR0EUVSmgSUdBFaEBXAQATEhETwHAUSmgRUdBFFUpoElHQRWhAVwEAExIRE8BwFEpoEVHQRRVKaBJR0EVoQFcCAXjEIXAQcSI8aUpoaVHQRWlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4tSTDaEBXAAF4yohAVwIAEAsLE79wE8QAcWhKaRJR0EVpEs5AVwEAEAsLE79waEBXAQDCcGhAVwMAEAsLE79waBHAcWkQznJqQFcBAAwU9mRDSY04eNMrmU5OEoPGk0Qh2v7bMHBoQFpANOBAW0BXAwAQNwAAcGgRwHFpStgkBBDOcmpK2CQHFM43AQBBz+dHlkBXBwAYFxYVFBMSERjAcAwFdGhyZWUMA3R3bwwDb25lE8BxGRgXE8AWFRQTwBMSERPAE8ByExIRE8BzFhUUE8B0GRgXE8B1bWxrE8B2bmppaBS/QFYEDAIBA9swYAwCAQPbMGEMFPZkQ0mNOHjTK5lOThKDxpNEIdr+2zBiDAZORVAtMTAMBU5FUC01EsBjQHQkW6o="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WEA=
    /// 00 : LDSFLD0 [2 datoshi]
    /// 01 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTreeByteLengthPrefix")]
    public abstract byte[]? GetTreeByteLengthPrefix();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WUA=
    /// 00 : LDSFLD1 [2 datoshi]
    /// 01 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTreeByteLengthPrefix2")]
    public abstract byte[]? GetTreeByteLengthPrefix2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WkA=
    /// 00 : LDSFLD2 [2 datoshi]
    /// 01 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayOwner")]
    public abstract byte[]? TestByteArrayOwner();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NOBA
    /// 00 : CALL E0 [512 datoshi]
    /// 02 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayOwnerCall")]
    public abstract byte[]? TestByteArrayOwnerCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwcAGBcWFRQTEhEYwHAMBXRocmVlDAN0d28MA29uZRPAcRkYFxPAFhUUE8ATEhETwBPAchMSERPAcxYVFBPAdBkYFxPAdW1saxPAdm5qaWgUv0A=
    /// 00 : INITSLOT 0700 [64 datoshi]
    /// 03 : PUSH8 [1 datoshi]
    /// 04 : PUSH7 [1 datoshi]
    /// 05 : PUSH6 [1 datoshi]
    /// 06 : PUSH5 [1 datoshi]
    /// 07 : PUSH4 [1 datoshi]
    /// 08 : PUSH3 [1 datoshi]
    /// 09 : PUSH2 [1 datoshi]
    /// 0A : PUSH1 [1 datoshi]
    /// 0B : PUSH8 [1 datoshi]
    /// 0C : PACK [2048 datoshi]
    /// 0D : STLOC0 [2 datoshi]
    /// 0E : PUSHDATA1 7468726565 'three' [8 datoshi]
    /// 15 : PUSHDATA1 74776F 'two' [8 datoshi]
    /// 1A : PUSHDATA1 6F6E65 'one' [8 datoshi]
    /// 1F : PUSH3 [1 datoshi]
    /// 20 : PACK [2048 datoshi]
    /// 21 : STLOC1 [2 datoshi]
    /// 22 : PUSH9 [1 datoshi]
    /// 23 : PUSH8 [1 datoshi]
    /// 24 : PUSH7 [1 datoshi]
    /// 25 : PUSH3 [1 datoshi]
    /// 26 : PACK [2048 datoshi]
    /// 27 : PUSH6 [1 datoshi]
    /// 28 : PUSH5 [1 datoshi]
    /// 29 : PUSH4 [1 datoshi]
    /// 2A : PUSH3 [1 datoshi]
    /// 2B : PACK [2048 datoshi]
    /// 2C : PUSH3 [1 datoshi]
    /// 2D : PUSH2 [1 datoshi]
    /// 2E : PUSH1 [1 datoshi]
    /// 2F : PUSH3 [1 datoshi]
    /// 30 : PACK [2048 datoshi]
    /// 31 : PUSH3 [1 datoshi]
    /// 32 : PACK [2048 datoshi]
    /// 33 : STLOC2 [2 datoshi]
    /// 34 : PUSH3 [1 datoshi]
    /// 35 : PUSH2 [1 datoshi]
    /// 36 : PUSH1 [1 datoshi]
    /// 37 : PUSH3 [1 datoshi]
    /// 38 : PACK [2048 datoshi]
    /// 39 : STLOC3 [2 datoshi]
    /// 3A : PUSH6 [1 datoshi]
    /// 3B : PUSH5 [1 datoshi]
    /// 3C : PUSH4 [1 datoshi]
    /// 3D : PUSH3 [1 datoshi]
    /// 3E : PACK [2048 datoshi]
    /// 3F : STLOC4 [2 datoshi]
    /// 40 : PUSH9 [1 datoshi]
    /// 41 : PUSH8 [1 datoshi]
    /// 42 : PUSH7 [1 datoshi]
    /// 43 : PUSH3 [1 datoshi]
    /// 44 : PACK [2048 datoshi]
    /// 45 : STLOC5 [2 datoshi]
    /// 46 : LDLOC5 [2 datoshi]
    /// 47 : LDLOC4 [2 datoshi]
    /// 48 : LDLOC3 [2 datoshi]
    /// 49 : PUSH3 [1 datoshi]
    /// 4A : PACK [2048 datoshi]
    /// 4B : STLOC6 [2 datoshi]
    /// 4C : LDLOC6 [2 datoshi]
    /// 4D : LDLOC2 [2 datoshi]
    /// 4E : LDLOC1 [2 datoshi]
    /// 4F : LDLOC0 [2 datoshi]
    /// 50 : PUSH4 [1 datoshi]
    /// 51 : PACKSTRUCT [2048 datoshi]
    /// 52 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCollectionexpressions")]
    public abstract IList<object>? TestCollectionexpressions();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAE8QhcGgQzhCXJgQIQAlA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH3 [1 datoshi]
    /// 04 : NEWARRAY_T 21 'Integer' [512 datoshi]
    /// 06 : STLOC0 [2 datoshi]
    /// 07 : LDLOC0 [2 datoshi]
    /// 08 : PUSH0 [1 datoshi]
    /// 09 : PICKITEM [64 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : EQUAL [32 datoshi]
    /// 0C : JMPIFNOT 04 [2 datoshi]
    /// 0E : PUSHT [1 datoshi]
    /// 0F : RET [0 datoshi]
    /// 10 : PUSHF [1 datoshi]
    /// 11 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDefaultArray")]
    public abstract bool? TestDefaultArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEAsLE79waEA=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSHNULL [1 datoshi]
    /// 06 : PUSH3 [1 datoshi]
    /// 07 : PACKSTRUCT [2048 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDefaultState")]
    public abstract object? TestDefaultState();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeMQhcBBxIjxpSmhpUdBFaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMNoQA==
    /// 00 : INITSLOT 0201 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : NEWARRAY_T 21 'Integer' [512 datoshi]
    /// 06 : STLOC0 [2 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : STLOC1 [2 datoshi]
    /// 09 : JMP 3C [2 datoshi]
    /// 0B : LDLOC1 [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : LDLOC0 [2 datoshi]
    /// 0E : LDLOC1 [2 datoshi]
    /// 0F : ROT [2 datoshi]
    /// 10 : SETITEM [8192 datoshi]
    /// 11 : DROP [2 datoshi]
    /// 12 : LDLOC1 [2 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : INC [4 datoshi]
    /// 15 : DUP [2 datoshi]
    /// 16 : PUSHINT32 00000080 [1 datoshi]
    /// 1B : JMPGE 04 [2 datoshi]
    /// 1D : JMP 0A [2 datoshi]
    /// 1F : DUP [2 datoshi]
    /// 20 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 25 : JMPLE 1E [2 datoshi]
    /// 27 : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 30 : AND [8 datoshi]
    /// 31 : DUP [2 datoshi]
    /// 32 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 37 : JMPLE 0C [2 datoshi]
    /// 39 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 42 : SUB [8 datoshi]
    /// 43 : STLOC1 [2 datoshi]
    /// 44 : DROP [2 datoshi]
    /// 45 : LDLOC1 [2 datoshi]
    /// 46 : LDARG0 [2 datoshi]
    /// 47 : LT [8 datoshi]
    /// 48 : JMPIF C3 [2 datoshi]
    /// 4A : LDLOC0 [2 datoshi]
    /// 4B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDynamicArrayInit")]
    public abstract IList<object>? TestDynamicArrayInit(BigInteger? length);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMqIQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SIZE [4 datoshi]
    /// 05 : NEWBUFFER [256 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDynamicArrayStringInit")]
    public abstract byte[]? TestDynamicArrayStringInit(string? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAEDcAAHBoEcBxaUrYJAQQznJqStgkBxTONwEAQc/nR5ZA
    /// 00 : INITSLOT 0300 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : CALLT 0000 [32768 datoshi]
    /// 07 : STLOC0 [2 datoshi]
    /// 08 : LDLOC0 [2 datoshi]
    /// 09 : PUSH1 [1 datoshi]
    /// 0A : PACK [2048 datoshi]
    /// 0B : STLOC1 [2 datoshi]
    /// 0C : LDLOC1 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : ISNULL [2 datoshi]
    /// 0F : JMPIF 04 [2 datoshi]
    /// 11 : PUSH0 [1 datoshi]
    /// 12 : PICKITEM [64 datoshi]
    /// 13 : STLOC2 [2 datoshi]
    /// 14 : LDLOC2 [2 datoshi]
    /// 15 : DUP [2 datoshi]
    /// 16 : ISNULL [2 datoshi]
    /// 17 : JMPIF 07 [2 datoshi]
    /// 19 : PUSH4 [1 datoshi]
    /// 1A : PICKITEM [64 datoshi]
    /// 1B : CALLT 0100 [32768 datoshi]
    /// 1E : SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 23 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testElementBinding")]
    public abstract void TestElementBinding();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAwnBoQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : NEWARRAY0 [16 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEmptyArray")]
    public abstract IList<object>? TestEmptyArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAE8QhcBBKaBBR0EURSmgRUdBFEkpoElHQRWhA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH3 [1 datoshi]
    /// 04 : NEWARRAY_T 21 'Integer' [512 datoshi]
    /// 06 : STLOC0 [2 datoshi]
    /// 07 : PUSH0 [1 datoshi]
    /// 08 : DUP [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : ROT [2 datoshi]
    /// 0C : SETITEM [8192 datoshi]
    /// 0D : DROP [2 datoshi]
    /// 0E : PUSH1 [1 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : LDLOC0 [2 datoshi]
    /// 11 : PUSH1 [1 datoshi]
    /// 12 : ROT [2 datoshi]
    /// 13 : SETITEM [8192 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSH2 [1 datoshi]
    /// 16 : DUP [2 datoshi]
    /// 17 : LDLOC0 [2 datoshi]
    /// 18 : PUSH2 [1 datoshi]
    /// 19 : ROT [2 datoshi]
    /// 1A : SETITEM [8192 datoshi]
    /// 1B : DROP [2 datoshi]
    /// 1C : LDLOC0 [2 datoshi]
    /// 1D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntArray")]
    public abstract IList<object>? TestIntArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAExIRE8BwFEpoEVHQRRVKaBJR0EVoQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH3 [1 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : PUSH1 [1 datoshi]
    /// 06 : PUSH3 [1 datoshi]
    /// 07 : PACK [2048 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : PUSH4 [1 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : PUSH1 [1 datoshi]
    /// 0D : ROT [2 datoshi]
    /// 0E : SETITEM [8192 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : PUSH5 [1 datoshi]
    /// 11 : DUP [2 datoshi]
    /// 12 : LDLOC0 [2 datoshi]
    /// 13 : PUSH2 [1 datoshi]
    /// 14 : ROT [2 datoshi]
    /// 15 : SETITEM [8192 datoshi]
    /// 16 : DROP [2 datoshi]
    /// 17 : LDLOC0 [2 datoshi]
    /// 18 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntArrayInit")]
    public abstract IList<object>? TestIntArrayInit();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAExIRE8BwFEpoEVHQRRVKaBJR0EVoQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH3 [1 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : PUSH1 [1 datoshi]
    /// 06 : PUSH3 [1 datoshi]
    /// 07 : PACK [2048 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : PUSH4 [1 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : PUSH1 [1 datoshi]
    /// 0D : ROT [2 datoshi]
    /// 0E : SETITEM [8192 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : PUSH5 [1 datoshi]
    /// 11 : DUP [2 datoshi]
    /// 12 : LDLOC0 [2 datoshi]
    /// 13 : PUSH2 [1 datoshi]
    /// 14 : ROT [2 datoshi]
    /// 15 : SETITEM [8192 datoshi]
    /// 16 : DROP [2 datoshi]
    /// 17 : LDLOC0 [2 datoshi]
    /// 18 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntArrayInit2")]
    public abstract IList<object>? TestIntArrayInit2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAExIRE8BwFEpoEVHQRRVKaBJR0EVoQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSH3 [1 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : PUSH1 [1 datoshi]
    /// 06 : PUSH3 [1 datoshi]
    /// 07 : PACK [2048 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : PUSH4 [1 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : LDLOC0 [2 datoshi]
    /// 0C : PUSH1 [1 datoshi]
    /// 0D : ROT [2 datoshi]
    /// 0E : SETITEM [8192 datoshi]
    /// 0F : DROP [2 datoshi]
    /// 10 : PUSH5 [1 datoshi]
    /// 11 : DUP [2 datoshi]
    /// 12 : LDLOC0 [2 datoshi]
    /// 13 : PUSH2 [1 datoshi]
    /// 14 : ROT [2 datoshi]
    /// 15 : SETITEM [8192 datoshi]
    /// 16 : DROP [2 datoshi]
    /// 17 : LDLOC0 [2 datoshi]
    /// 18 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntArrayInit3")]
    public abstract IList<object>? TestIntArrayInit3();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQAFBMSERTAcBgXFhUUwHEREhMRFMByEhMUFRTAc2tqaWgUwEA=
    /// 00 : INITSLOT 0400 [64 datoshi]
    /// 03 : PUSH4 [1 datoshi]
    /// 04 : PUSH3 [1 datoshi]
    /// 05 : PUSH2 [1 datoshi]
    /// 06 : PUSH1 [1 datoshi]
    /// 07 : PUSH4 [1 datoshi]
    /// 08 : PACK [2048 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : PUSH8 [1 datoshi]
    /// 0B : PUSH7 [1 datoshi]
    /// 0C : PUSH6 [1 datoshi]
    /// 0D : PUSH5 [1 datoshi]
    /// 0E : PUSH4 [1 datoshi]
    /// 0F : PACK [2048 datoshi]
    /// 10 : STLOC1 [2 datoshi]
    /// 11 : PUSH1 [1 datoshi]
    /// 12 : PUSH2 [1 datoshi]
    /// 13 : PUSH3 [1 datoshi]
    /// 14 : PUSH1 [1 datoshi]
    /// 15 : PUSH4 [1 datoshi]
    /// 16 : PACK [2048 datoshi]
    /// 17 : STLOC2 [2 datoshi]
    /// 18 : PUSH2 [1 datoshi]
    /// 19 : PUSH3 [1 datoshi]
    /// 1A : PUSH4 [1 datoshi]
    /// 1B : PUSH5 [1 datoshi]
    /// 1C : PUSH4 [1 datoshi]
    /// 1D : PACK [2048 datoshi]
    /// 1E : STLOC3 [2 datoshi]
    /// 1F : LDLOC3 [2 datoshi]
    /// 20 : LDLOC2 [2 datoshi]
    /// 21 : LDLOC1 [2 datoshi]
    /// 22 : LDLOC0 [2 datoshi]
    /// 23 : PUSH4 [1 datoshi]
    /// 24 : PACK [2048 datoshi]
    /// 25 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testJaggedArray")]
    public abstract IList<object>? TestJaggedArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQADAQBAgME2zBwDAQFBgcI2zBxDAQBAwIB2zByDAQFBAMC2zBza2ppaBTAQA==
    /// 00 : INITSLOT 0400 [64 datoshi]
    /// 03 : PUSHDATA1 01020304 [8 datoshi]
    /// 09 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0B : STLOC0 [2 datoshi]
    /// 0C : PUSHDATA1 05060708 [8 datoshi]
    /// 12 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 14 : STLOC1 [2 datoshi]
    /// 15 : PUSHDATA1 01030201 [8 datoshi]
    /// 1B : CONVERT 30 'Buffer' [8192 datoshi]
    /// 1D : STLOC2 [2 datoshi]
    /// 1E : PUSHDATA1 05040302 [8 datoshi]
    /// 24 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 26 : STLOC3 [2 datoshi]
    /// 27 : LDLOC3 [2 datoshi]
    /// 28 : LDLOC2 [2 datoshi]
    /// 29 : LDLOC1 [2 datoshi]
    /// 2A : LDLOC0 [2 datoshi]
    /// 2B : PUSH4 [1 datoshi]
    /// 2C : PACK [2048 datoshi]
    /// 2D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testJaggedByteArray")]
    public abstract IList<object>? TestJaggedByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEAsLE79wE8QAcWhKaRJR0EVpEs5A
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSHNULL [1 datoshi]
    /// 06 : PUSH3 [1 datoshi]
    /// 07 : PACKSTRUCT [2048 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : PUSH3 [1 datoshi]
    /// 0A : NEWARRAY_T 00 'Any' [512 datoshi]
    /// 0C : STLOC1 [2 datoshi]
    /// 0D : LDLOC0 [2 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : LDLOC1 [2 datoshi]
    /// 10 : PUSH2 [1 datoshi]
    /// 11 : ROT [2 datoshi]
    /// 12 : SETITEM [8192 datoshi]
    /// 13 : DROP [2 datoshi]
    /// 14 : LDLOC1 [2 datoshi]
    /// 15 : PUSH2 [1 datoshi]
    /// 16 : PICKITEM [64 datoshi]
    /// 17 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStructArray")]
    public abstract object? TestStructArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAEAsLE79waBHAcWkQznJqQA==
    /// 00 : INITSLOT 0300 [64 datoshi]
    /// 03 : PUSH0 [1 datoshi]
    /// 04 : PUSHNULL [1 datoshi]
    /// 05 : PUSHNULL [1 datoshi]
    /// 06 : PUSH3 [1 datoshi]
    /// 07 : PACKSTRUCT [2048 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : PUSH1 [1 datoshi]
    /// 0B : PACK [2048 datoshi]
    /// 0C : STLOC1 [2 datoshi]
    /// 0D : LDLOC1 [2 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : PICKITEM [64 datoshi]
    /// 10 : STLOC2 [2 datoshi]
    /// 11 : LDLOC2 [2 datoshi]
    /// 12 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStructArrayInit")]
    public abstract object? TestStructArrayInit();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: W0A=
    /// 00 : LDSFLD3 [2 datoshi]
    /// 01 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSupportedStandards")]
    public abstract IList<object>? TestSupportedStandards();

    #endregion
}
