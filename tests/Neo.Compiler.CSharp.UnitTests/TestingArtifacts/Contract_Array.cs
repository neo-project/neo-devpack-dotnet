using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Array(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Array"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getTreeByteLengthPrefix"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""getTreeByteLengthPrefix2"",""parameters"":[],""returntype"":""ByteArray"",""offset"":2,""safe"":false},{""name"":""testJaggedArray"",""parameters"":[],""returntype"":""Array"",""offset"":4,""safe"":false},{""name"":""testJaggedByteArray"",""parameters"":[],""returntype"":""Array"",""offset"":42,""safe"":false},{""name"":""testIntArray"",""parameters"":[],""returntype"":""Array"",""offset"":88,""safe"":false},{""name"":""testDefaultArray"",""parameters"":[],""returntype"":""Boolean"",""offset"":118,""safe"":false},{""name"":""testIntArrayInit"",""parameters"":[],""returntype"":""Array"",""offset"":136,""safe"":false},{""name"":""testIntArrayInit2"",""parameters"":[],""returntype"":""Array"",""offset"":161,""safe"":false},{""name"":""testIntArrayInit3"",""parameters"":[],""returntype"":""Array"",""offset"":186,""safe"":false},{""name"":""testDynamicArrayInit"",""parameters"":[{""name"":""length"",""type"":""Integer""}],""returntype"":""Array"",""offset"":211,""safe"":false},{""name"":""testDynamicArrayStringInit"",""parameters"":[{""name"":""input"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":241,""safe"":false},{""name"":""testStructArray"",""parameters"":[],""returntype"":""Any"",""offset"":248,""safe"":false},{""name"":""testDefaultState"",""parameters"":[],""returntype"":""Any"",""offset"":291,""safe"":false},{""name"":""testEmptyArray"",""parameters"":[],""returntype"":""Array"",""offset"":305,""safe"":false},{""name"":""testStructArrayInit"",""parameters"":[],""returntype"":""Any"",""offset"":312,""safe"":false},{""name"":""testByteArrayOwner"",""parameters"":[],""returntype"":""ByteArray"",""offset"":364,""safe"":false},{""name"":""testByteArrayOwnerCall"",""parameters"":[],""returntype"":""ByteArray"",""offset"":366,""safe"":false},{""name"":""testSupportedStandards"",""parameters"":[],""returntype"":""Array"",""offset"":369,""safe"":false},{""name"":""testElementBinding"",""parameters"":[],""returntype"":""Void"",""offset"":371,""safe"":false},{""name"":""testCollectionexpressions"",""parameters"":[],""returntype"":""Array"",""offset"":407,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":490,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""getBlock""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAK+8gQxQDYqd8FQmcfmTBL3ALZl2ghnZXRCbG9jawEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/SYCWEBZQFcEABQTEhEUwHAYFxYVFMBxERITERTAchITFBUUwHNramloFMBAVwQADAQBAgME2zBwDAQFBgcI2zBxDAQBAwIB2zByDAQFBAMC2zBza2ppaBTAQFcBABPEIXAQSmgQUdBFEUpoEVHQRRJKaBJR0EVoQFcBABPEIXBoEM4QlyYECEAJQFcBABMSERPAcBRKaBFR0EUVSmgSUdBFaEBXAQATEhETwHAUSmgRUdBFFUpoElHQRWhAVwEAExIRE8BwFEpoEVHQRRVKaBJR0EVoQFcCAXjEIXAQcSIOaUpoaVHQRWlKnHFFaXi1JPFoQFcAAXjKiEBXAgAQCwsTv0o0EnATxABxaEppElHQRWkSzkBXAAF4EAvQeBEL0HgSENBAVwEAEAsLE79KNOdwaEBXAQDCcGhAVwMAEAsLE79KNNJwaBHAcWkQznJqQFcBAAwU9mRDSY04eNMrmU5OEoPGk0Qh2v7bMHBoQFpANOBAW0BXAwAQNwAAcGgRwHFpStgkBBDOcmpK2CQHFM43AQBBz+dHlkBXBwAYFxYVFBMSERjAcAwFdGhyZWUMA3R3bwwDb25lE8BxGRgXE8AWFRQTwBMSERPAE8ByExIRE8BzFhUUE8B0GRgXE8B1bWxrE8B2bmppaBS/QFYEDAIBA9swYAwCAQPbMGEMFPZkQ0mNOHjTK5lOThKDxpNEIdr+2zBiDAZORVAtMTAMBU5FUC01EsBjQN+4BNE="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WEA=
    /// 00 : OpCode.LDSFLD0 [2 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTreeByteLengthPrefix")]
    public abstract byte[]? GetTreeByteLengthPrefix();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WUA=
    /// 00 : OpCode.LDSFLD1 [2 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTreeByteLengthPrefix2")]
    public abstract byte[]? GetTreeByteLengthPrefix2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WkA=
    /// 00 : OpCode.LDSFLD2 [2 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayOwner")]
    public abstract byte[]? TestByteArrayOwner();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NOBA
    /// 00 : OpCode.CALL E0 [512 datoshi]
    /// 02 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayOwnerCall")]
    public abstract byte[]? TestByteArrayOwnerCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwcAGBcWFRQTEhEYwHAMdGhyZWUMdHdvDG9uZRPAcRkYFxPAFhUUE8ATEhETwBPAchMSERPAcxYVFBPAdBkYFxPAdW1saxPAdm5qaWgUv0A=
    /// 00 : OpCode.INITSLOT 0700 [64 datoshi]
    /// 03 : OpCode.PUSH8 [1 datoshi]
    /// 04 : OpCode.PUSH7 [1 datoshi]
    /// 05 : OpCode.PUSH6 [1 datoshi]
    /// 06 : OpCode.PUSH5 [1 datoshi]
    /// 07 : OpCode.PUSH4 [1 datoshi]
    /// 08 : OpCode.PUSH3 [1 datoshi]
    /// 09 : OpCode.PUSH2 [1 datoshi]
    /// 0A : OpCode.PUSH1 [1 datoshi]
    /// 0B : OpCode.PUSH8 [1 datoshi]
    /// 0C : OpCode.PACK [2048 datoshi]
    /// 0D : OpCode.STLOC0 [2 datoshi]
    /// 0E : OpCode.PUSHDATA1 7468726565 [8 datoshi]
    /// 15 : OpCode.PUSHDATA1 74776F [8 datoshi]
    /// 1A : OpCode.PUSHDATA1 6F6E65 [8 datoshi]
    /// 1F : OpCode.PUSH3 [1 datoshi]
    /// 20 : OpCode.PACK [2048 datoshi]
    /// 21 : OpCode.STLOC1 [2 datoshi]
    /// 22 : OpCode.PUSH9 [1 datoshi]
    /// 23 : OpCode.PUSH8 [1 datoshi]
    /// 24 : OpCode.PUSH7 [1 datoshi]
    /// 25 : OpCode.PUSH3 [1 datoshi]
    /// 26 : OpCode.PACK [2048 datoshi]
    /// 27 : OpCode.PUSH6 [1 datoshi]
    /// 28 : OpCode.PUSH5 [1 datoshi]
    /// 29 : OpCode.PUSH4 [1 datoshi]
    /// 2A : OpCode.PUSH3 [1 datoshi]
    /// 2B : OpCode.PACK [2048 datoshi]
    /// 2C : OpCode.PUSH3 [1 datoshi]
    /// 2D : OpCode.PUSH2 [1 datoshi]
    /// 2E : OpCode.PUSH1 [1 datoshi]
    /// 2F : OpCode.PUSH3 [1 datoshi]
    /// 30 : OpCode.PACK [2048 datoshi]
    /// 31 : OpCode.PUSH3 [1 datoshi]
    /// 32 : OpCode.PACK [2048 datoshi]
    /// 33 : OpCode.STLOC2 [2 datoshi]
    /// 34 : OpCode.PUSH3 [1 datoshi]
    /// 35 : OpCode.PUSH2 [1 datoshi]
    /// 36 : OpCode.PUSH1 [1 datoshi]
    /// 37 : OpCode.PUSH3 [1 datoshi]
    /// 38 : OpCode.PACK [2048 datoshi]
    /// 39 : OpCode.STLOC3 [2 datoshi]
    /// 3A : OpCode.PUSH6 [1 datoshi]
    /// 3B : OpCode.PUSH5 [1 datoshi]
    /// 3C : OpCode.PUSH4 [1 datoshi]
    /// 3D : OpCode.PUSH3 [1 datoshi]
    /// 3E : OpCode.PACK [2048 datoshi]
    /// 3F : OpCode.STLOC4 [2 datoshi]
    /// 40 : OpCode.PUSH9 [1 datoshi]
    /// 41 : OpCode.PUSH8 [1 datoshi]
    /// 42 : OpCode.PUSH7 [1 datoshi]
    /// 43 : OpCode.PUSH3 [1 datoshi]
    /// 44 : OpCode.PACK [2048 datoshi]
    /// 45 : OpCode.STLOC5 [2 datoshi]
    /// 46 : OpCode.LDLOC5 [2 datoshi]
    /// 47 : OpCode.LDLOC4 [2 datoshi]
    /// 48 : OpCode.LDLOC3 [2 datoshi]
    /// 49 : OpCode.PUSH3 [1 datoshi]
    /// 4A : OpCode.PACK [2048 datoshi]
    /// 4B : OpCode.STLOC6 [2 datoshi]
    /// 4C : OpCode.LDLOC6 [2 datoshi]
    /// 4D : OpCode.LDLOC2 [2 datoshi]
    /// 4E : OpCode.LDLOC1 [2 datoshi]
    /// 4F : OpCode.LDLOC0 [2 datoshi]
    /// 50 : OpCode.PUSH4 [1 datoshi]
    /// 51 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 52 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCollectionexpressions")]
    public abstract IList<object>? TestCollectionexpressions();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAE8QhcGgQzhCXJgQIQAlA
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSH3 [1 datoshi]
    /// 04 : OpCode.NEWARRAY_T 21 [512 datoshi]
    /// 06 : OpCode.STLOC0 [2 datoshi]
    /// 07 : OpCode.LDLOC0 [2 datoshi]
    /// 08 : OpCode.PUSH0 [1 datoshi]
    /// 09 : OpCode.PICKITEM [64 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.EQUAL [32 datoshi]
    /// 0C : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 0E : OpCode.PUSHT [1 datoshi]
    /// 0F : OpCode.RET [0 datoshi]
    /// 10 : OpCode.PUSHF [1 datoshi]
    /// 11 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDefaultArray")]
    public abstract bool? TestDefaultArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEAsLE79KNOdwaEA=
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSHNULL [1 datoshi]
    /// 06 : OpCode.PUSH3 [1 datoshi]
    /// 07 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 08 : OpCode.DUP [2 datoshi]
    /// 09 : OpCode.CALL E7 [512 datoshi]
    /// 0B : OpCode.STLOC0 [2 datoshi]
    /// 0C : OpCode.LDLOC0 [2 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDefaultState")]
    public abstract object? TestDefaultState();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeMQhcBBxIg5pSmhpUdBFaUqccUVpeLUk8WhA
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.NEWARRAY_T 21 [512 datoshi]
    /// 06 : OpCode.STLOC0 [2 datoshi]
    /// 07 : OpCode.PUSH0 [1 datoshi]
    /// 08 : OpCode.STLOC1 [2 datoshi]
    /// 09 : OpCode.JMP 0E [2 datoshi]
    /// 0B : OpCode.LDLOC1 [2 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.LDLOC0 [2 datoshi]
    /// 0E : OpCode.LDLOC1 [2 datoshi]
    /// 0F : OpCode.ROT [2 datoshi]
    /// 10 : OpCode.SETITEM [8192 datoshi]
    /// 11 : OpCode.DROP [2 datoshi]
    /// 12 : OpCode.LDLOC1 [2 datoshi]
    /// 13 : OpCode.DUP [2 datoshi]
    /// 14 : OpCode.INC [4 datoshi]
    /// 15 : OpCode.STLOC1 [2 datoshi]
    /// 16 : OpCode.DROP [2 datoshi]
    /// 17 : OpCode.LDLOC1 [2 datoshi]
    /// 18 : OpCode.LDARG0 [2 datoshi]
    /// 19 : OpCode.LT [8 datoshi]
    /// 1A : OpCode.JMPIF F1 [2 datoshi]
    /// 1C : OpCode.LDLOC0 [2 datoshi]
    /// 1D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDynamicArrayInit")]
    public abstract IList<object>? TestDynamicArrayInit(BigInteger? length);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMqIQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.SIZE [4 datoshi]
    /// 05 : OpCode.NEWBUFFER [256 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDynamicArrayStringInit")]
    public abstract byte[]? TestDynamicArrayStringInit(string? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAEDcAAHBoEcBxaUrYJAQQznJqStgkBxTONwEAQc/nR5ZA
    /// 00 : OpCode.INITSLOT 0300 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.CALLT 0000 [32768 datoshi]
    /// 07 : OpCode.STLOC0 [2 datoshi]
    /// 08 : OpCode.LDLOC0 [2 datoshi]
    /// 09 : OpCode.PUSH1 [1 datoshi]
    /// 0A : OpCode.PACK [2048 datoshi]
    /// 0B : OpCode.STLOC1 [2 datoshi]
    /// 0C : OpCode.LDLOC1 [2 datoshi]
    /// 0D : OpCode.DUP [2 datoshi]
    /// 0E : OpCode.ISNULL [2 datoshi]
    /// 0F : OpCode.JMPIF 04 [2 datoshi]
    /// 11 : OpCode.PUSH0 [1 datoshi]
    /// 12 : OpCode.PICKITEM [64 datoshi]
    /// 13 : OpCode.STLOC2 [2 datoshi]
    /// 14 : OpCode.LDLOC2 [2 datoshi]
    /// 15 : OpCode.DUP [2 datoshi]
    /// 16 : OpCode.ISNULL [2 datoshi]
    /// 17 : OpCode.JMPIF 07 [2 datoshi]
    /// 19 : OpCode.PUSH4 [1 datoshi]
    /// 1A : OpCode.PICKITEM [64 datoshi]
    /// 1B : OpCode.CALLT 0100 [32768 datoshi]
    /// 1E : OpCode.SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// 23 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testElementBinding")]
    public abstract void TestElementBinding();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAwnBoQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEmptyArray")]
    public abstract IList<object>? TestEmptyArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAE8QhcBBKaBBR0EURSmgRUdBFEkpoElHQRWhA
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSH3 [1 datoshi]
    /// 04 : OpCode.NEWARRAY_T 21 [512 datoshi]
    /// 06 : OpCode.STLOC0 [2 datoshi]
    /// 07 : OpCode.PUSH0 [1 datoshi]
    /// 08 : OpCode.DUP [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.ROT [2 datoshi]
    /// 0C : OpCode.SETITEM [8192 datoshi]
    /// 0D : OpCode.DROP [2 datoshi]
    /// 0E : OpCode.PUSH1 [1 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.LDLOC0 [2 datoshi]
    /// 11 : OpCode.PUSH1 [1 datoshi]
    /// 12 : OpCode.ROT [2 datoshi]
    /// 13 : OpCode.SETITEM [8192 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSH2 [1 datoshi]
    /// 16 : OpCode.DUP [2 datoshi]
    /// 17 : OpCode.LDLOC0 [2 datoshi]
    /// 18 : OpCode.PUSH2 [1 datoshi]
    /// 19 : OpCode.ROT [2 datoshi]
    /// 1A : OpCode.SETITEM [8192 datoshi]
    /// 1B : OpCode.DROP [2 datoshi]
    /// 1C : OpCode.LDLOC0 [2 datoshi]
    /// 1D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntArray")]
    public abstract IList<object>? TestIntArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAExIRE8BwFEpoEVHQRRVKaBJR0EVoQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSH3 [1 datoshi]
    /// 04 : OpCode.PUSH2 [1 datoshi]
    /// 05 : OpCode.PUSH1 [1 datoshi]
    /// 06 : OpCode.PUSH3 [1 datoshi]
    /// 07 : OpCode.PACK [2048 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.PUSH4 [1 datoshi]
    /// 0A : OpCode.DUP [2 datoshi]
    /// 0B : OpCode.LDLOC0 [2 datoshi]
    /// 0C : OpCode.PUSH1 [1 datoshi]
    /// 0D : OpCode.ROT [2 datoshi]
    /// 0E : OpCode.SETITEM [8192 datoshi]
    /// 0F : OpCode.DROP [2 datoshi]
    /// 10 : OpCode.PUSH5 [1 datoshi]
    /// 11 : OpCode.DUP [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.PUSH2 [1 datoshi]
    /// 14 : OpCode.ROT [2 datoshi]
    /// 15 : OpCode.SETITEM [8192 datoshi]
    /// 16 : OpCode.DROP [2 datoshi]
    /// 17 : OpCode.LDLOC0 [2 datoshi]
    /// 18 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntArrayInit")]
    public abstract IList<object>? TestIntArrayInit();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIntArrayInit2")]
    public abstract IList<object>? TestIntArrayInit2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIntArrayInit3")]
    public abstract IList<object>? TestIntArrayInit3();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQAFBMSERTAcBgXFhUUwHEREhMRFMByEhMUFRTAc2tqaWgUwEA=
    /// 00 : OpCode.INITSLOT 0400 [64 datoshi]
    /// 03 : OpCode.PUSH4 [1 datoshi]
    /// 04 : OpCode.PUSH3 [1 datoshi]
    /// 05 : OpCode.PUSH2 [1 datoshi]
    /// 06 : OpCode.PUSH1 [1 datoshi]
    /// 07 : OpCode.PUSH4 [1 datoshi]
    /// 08 : OpCode.PACK [2048 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.PUSH8 [1 datoshi]
    /// 0B : OpCode.PUSH7 [1 datoshi]
    /// 0C : OpCode.PUSH6 [1 datoshi]
    /// 0D : OpCode.PUSH5 [1 datoshi]
    /// 0E : OpCode.PUSH4 [1 datoshi]
    /// 0F : OpCode.PACK [2048 datoshi]
    /// 10 : OpCode.STLOC1 [2 datoshi]
    /// 11 : OpCode.PUSH1 [1 datoshi]
    /// 12 : OpCode.PUSH2 [1 datoshi]
    /// 13 : OpCode.PUSH3 [1 datoshi]
    /// 14 : OpCode.PUSH1 [1 datoshi]
    /// 15 : OpCode.PUSH4 [1 datoshi]
    /// 16 : OpCode.PACK [2048 datoshi]
    /// 17 : OpCode.STLOC2 [2 datoshi]
    /// 18 : OpCode.PUSH2 [1 datoshi]
    /// 19 : OpCode.PUSH3 [1 datoshi]
    /// 1A : OpCode.PUSH4 [1 datoshi]
    /// 1B : OpCode.PUSH5 [1 datoshi]
    /// 1C : OpCode.PUSH4 [1 datoshi]
    /// 1D : OpCode.PACK [2048 datoshi]
    /// 1E : OpCode.STLOC3 [2 datoshi]
    /// 1F : OpCode.LDLOC3 [2 datoshi]
    /// 20 : OpCode.LDLOC2 [2 datoshi]
    /// 21 : OpCode.LDLOC1 [2 datoshi]
    /// 22 : OpCode.LDLOC0 [2 datoshi]
    /// 23 : OpCode.PUSH4 [1 datoshi]
    /// 24 : OpCode.PACK [2048 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testJaggedArray")]
    public abstract IList<object>? TestJaggedArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQADAECAwTbMHAMBQYHCNswcQwBAwIB2zByDAUEAwLbMHNramloFMBA
    /// 00 : OpCode.INITSLOT 0400 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 01020304 [8 datoshi]
    /// 09 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 0B : OpCode.STLOC0 [2 datoshi]
    /// 0C : OpCode.PUSHDATA1 05060708 [8 datoshi]
    /// 12 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 14 : OpCode.STLOC1 [2 datoshi]
    /// 15 : OpCode.PUSHDATA1 01030201 [8 datoshi]
    /// 1B : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 1D : OpCode.STLOC2 [2 datoshi]
    /// 1E : OpCode.PUSHDATA1 05040302 [8 datoshi]
    /// 24 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 26 : OpCode.STLOC3 [2 datoshi]
    /// 27 : OpCode.LDLOC3 [2 datoshi]
    /// 28 : OpCode.LDLOC2 [2 datoshi]
    /// 29 : OpCode.LDLOC1 [2 datoshi]
    /// 2A : OpCode.LDLOC0 [2 datoshi]
    /// 2B : OpCode.PUSH4 [1 datoshi]
    /// 2C : OpCode.PACK [2048 datoshi]
    /// 2D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testJaggedByteArray")]
    public abstract IList<object>? TestJaggedByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEAsLE79KNBJwE8QAcWhKaRJR0EVpEs5A
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSHNULL [1 datoshi]
    /// 06 : OpCode.PUSH3 [1 datoshi]
    /// 07 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 08 : OpCode.DUP [2 datoshi]
    /// 09 : OpCode.CALL 12 [512 datoshi]
    /// 0B : OpCode.STLOC0 [2 datoshi]
    /// 0C : OpCode.PUSH3 [1 datoshi]
    /// 0D : OpCode.NEWARRAY_T 00 [512 datoshi]
    /// 0F : OpCode.STLOC1 [2 datoshi]
    /// 10 : OpCode.LDLOC0 [2 datoshi]
    /// 11 : OpCode.DUP [2 datoshi]
    /// 12 : OpCode.LDLOC1 [2 datoshi]
    /// 13 : OpCode.PUSH2 [1 datoshi]
    /// 14 : OpCode.ROT [2 datoshi]
    /// 15 : OpCode.SETITEM [8192 datoshi]
    /// 16 : OpCode.DROP [2 datoshi]
    /// 17 : OpCode.LDLOC1 [2 datoshi]
    /// 18 : OpCode.PUSH2 [1 datoshi]
    /// 19 : OpCode.PICKITEM [64 datoshi]
    /// 1A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStructArray")]
    public abstract object? TestStructArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAEAsLE79KNNJwaBHAcWkQznJqQA==
    /// 00 : OpCode.INITSLOT 0300 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.PUSHNULL [1 datoshi]
    /// 05 : OpCode.PUSHNULL [1 datoshi]
    /// 06 : OpCode.PUSH3 [1 datoshi]
    /// 07 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 08 : OpCode.DUP [2 datoshi]
    /// 09 : OpCode.CALL D2 [512 datoshi]
    /// 0B : OpCode.STLOC0 [2 datoshi]
    /// 0C : OpCode.LDLOC0 [2 datoshi]
    /// 0D : OpCode.PUSH1 [1 datoshi]
    /// 0E : OpCode.PACK [2048 datoshi]
    /// 0F : OpCode.STLOC1 [2 datoshi]
    /// 10 : OpCode.LDLOC1 [2 datoshi]
    /// 11 : OpCode.PUSH0 [1 datoshi]
    /// 12 : OpCode.PICKITEM [64 datoshi]
    /// 13 : OpCode.STLOC2 [2 datoshi]
    /// 14 : OpCode.LDLOC2 [2 datoshi]
    /// 15 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStructArrayInit")]
    public abstract object? TestStructArrayInit();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: W0A=
    /// 00 : OpCode.LDSFLD3 [2 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSupportedStandards")]
    public abstract IList<object>? TestSupportedStandards();

    #endregion
}
