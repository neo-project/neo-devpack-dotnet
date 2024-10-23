using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Array(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Array"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getTreeByteLengthPrefix"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""getTreeByteLengthPrefix2"",""parameters"":[],""returntype"":""ByteArray"",""offset"":2,""safe"":false},{""name"":""testJaggedArray"",""parameters"":[],""returntype"":""Array"",""offset"":4,""safe"":false},{""name"":""testJaggedByteArray"",""parameters"":[],""returntype"":""Array"",""offset"":42,""safe"":false},{""name"":""testIntArray"",""parameters"":[],""returntype"":""Array"",""offset"":88,""safe"":false},{""name"":""testDefaultArray"",""parameters"":[],""returntype"":""Boolean"",""offset"":118,""safe"":false},{""name"":""testIntArrayInit"",""parameters"":[],""returntype"":""Array"",""offset"":136,""safe"":false},{""name"":""testIntArrayInit2"",""parameters"":[],""returntype"":""Array"",""offset"":161,""safe"":false},{""name"":""testIntArrayInit3"",""parameters"":[],""returntype"":""Array"",""offset"":186,""safe"":false},{""name"":""testDynamicArrayInit"",""parameters"":[{""name"":""length"",""type"":""Integer""}],""returntype"":""Array"",""offset"":211,""safe"":false},{""name"":""testDynamicArrayStringInit"",""parameters"":[{""name"":""input"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":287,""safe"":false},{""name"":""testStructArray"",""parameters"":[],""returntype"":""Any"",""offset"":294,""safe"":false},{""name"":""testDefaultState"",""parameters"":[],""returntype"":""Any"",""offset"":337,""safe"":false},{""name"":""testEmptyArray"",""parameters"":[],""returntype"":""Array"",""offset"":351,""safe"":false},{""name"":""testStructArrayInit"",""parameters"":[],""returntype"":""Any"",""offset"":358,""safe"":false},{""name"":""testByteArrayOwner"",""parameters"":[],""returntype"":""ByteArray"",""offset"":410,""safe"":false},{""name"":""testByteArrayOwnerCall"",""parameters"":[],""returntype"":""ByteArray"",""offset"":412,""safe"":false},{""name"":""testSupportedStandards"",""parameters"":[],""returntype"":""Array"",""offset"":415,""safe"":false},{""name"":""testElementBinding"",""parameters"":[],""returntype"":""Void"",""offset"":417,""safe"":false},{""name"":""testCollectionexpressions"",""parameters"":[],""returntype"":""Array"",""offset"":453,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":536,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""getBlock""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAK+8gQxQDYqd8FQmcfmTBL3ALZl2ghnZXRCbG9jawEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/VQCWEBZQFcEABQTEhEUwHAYFxYVFMBxERITERTAchITFBUUwHNramloFMBAVwQADAQBAgME2zBwDAQFBgcI2zBxDAQBAwIB2zByDAQFBAMC2zBza2ppaBTAQFcBABPEIXAQSmgQUdBFEUpoEVHQRRJKaBJR0EVoQFcBABPEIXBoEM4QlyYECEAJQFcBABMSERPAcBRKaBFR0EUVSmgSUdBFaEBXAQATEhETwHAUSmgRUdBFFUpoElHQRWhAVwEAExIRE8BwFEpoEVHQRRVKaBJR0EVoQFcCAXjEIXAQcSI8aUpoaVHQRWlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4tSTDaEBXAAF4yohAVwIAEAsLE79KNBJwE8QAcWhKaRJR0EVpEs5AVwABeBAL0HgRC9B4EhDQQFcBABALCxO/SjTncGhAVwEAwnBoQFcDABALCxO/SjTScGgRwHFpEM5yakBXAQAMFPZkQ0mNOHjTK5lOThKDxpNEIdr+2zBwaEBaQDTgQFtAVwMAEDcAAHBoEcBxaUrYJAQQznJqStgkBxTONwEAQc/nR5ZAVwcAGBcWFRQTEhEYwHAMBXRocmVlDAN0d28MA29uZRPAcRkYFxPAFhUUE8ATEhETwBPAchMSERPAcxYVFBPAdBkYFxPAdW1saxPAdm5qaWgUv0BWBAwCAQPbMGAMAgED2zBhDBT2ZENJjTh40yuZTk4Sg8aTRCHa/tswYgwGTkVQLTEwDAVORVAtNRLAY0B6B66z"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WEA=
    /// 00 : OpCode.LDSFLD0
    /// 01 : OpCode.RET
    /// </remarks>
    [DisplayName("getTreeByteLengthPrefix")]
    public abstract byte[]? GetTreeByteLengthPrefix();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WUA=
    /// 00 : OpCode.LDSFLD1
    /// 01 : OpCode.RET
    /// </remarks>
    [DisplayName("getTreeByteLengthPrefix2")]
    public abstract byte[]? GetTreeByteLengthPrefix2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WkA=
    /// 00 : OpCode.LDSFLD2
    /// 01 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteArrayOwner")]
    public abstract byte[]? TestByteArrayOwner();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NOBA
    /// 00 : OpCode.CALL E0
    /// 02 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteArrayOwnerCall")]
    public abstract byte[]? TestByteArrayOwnerCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwcAGBcWFRQTEhEYwHAMdGhyZWUMdHdvDG9uZRPAcRkYFxPAFhUUE8ATEhETwBPAchMSERPAcxYVFBPAdBkYFxPAdW1saxPAdm5qaWgUv0A=
    /// 00 : OpCode.INITSLOT 0700
    /// 03 : OpCode.PUSH8
    /// 04 : OpCode.PUSH7
    /// 05 : OpCode.PUSH6
    /// 06 : OpCode.PUSH5
    /// 07 : OpCode.PUSH4
    /// 08 : OpCode.PUSH3
    /// 09 : OpCode.PUSH2
    /// 0A : OpCode.PUSH1
    /// 0B : OpCode.PUSH8
    /// 0C : OpCode.PACK
    /// 0D : OpCode.STLOC0
    /// 0E : OpCode.PUSHDATA1 7468726565
    /// 15 : OpCode.PUSHDATA1 74776F
    /// 1A : OpCode.PUSHDATA1 6F6E65
    /// 1F : OpCode.PUSH3
    /// 20 : OpCode.PACK
    /// 21 : OpCode.STLOC1
    /// 22 : OpCode.PUSH9
    /// 23 : OpCode.PUSH8
    /// 24 : OpCode.PUSH7
    /// 25 : OpCode.PUSH3
    /// 26 : OpCode.PACK
    /// 27 : OpCode.PUSH6
    /// 28 : OpCode.PUSH5
    /// 29 : OpCode.PUSH4
    /// 2A : OpCode.PUSH3
    /// 2B : OpCode.PACK
    /// 2C : OpCode.PUSH3
    /// 2D : OpCode.PUSH2
    /// 2E : OpCode.PUSH1
    /// 2F : OpCode.PUSH3
    /// 30 : OpCode.PACK
    /// 31 : OpCode.PUSH3
    /// 32 : OpCode.PACK
    /// 33 : OpCode.STLOC2
    /// 34 : OpCode.PUSH3
    /// 35 : OpCode.PUSH2
    /// 36 : OpCode.PUSH1
    /// 37 : OpCode.PUSH3
    /// 38 : OpCode.PACK
    /// 39 : OpCode.STLOC3
    /// 3A : OpCode.PUSH6
    /// 3B : OpCode.PUSH5
    /// 3C : OpCode.PUSH4
    /// 3D : OpCode.PUSH3
    /// 3E : OpCode.PACK
    /// 3F : OpCode.STLOC4
    /// 40 : OpCode.PUSH9
    /// 41 : OpCode.PUSH8
    /// 42 : OpCode.PUSH7
    /// 43 : OpCode.PUSH3
    /// 44 : OpCode.PACK
    /// 45 : OpCode.STLOC5
    /// 46 : OpCode.LDLOC5
    /// 47 : OpCode.LDLOC4
    /// 48 : OpCode.LDLOC3
    /// 49 : OpCode.PUSH3
    /// 4A : OpCode.PACK
    /// 4B : OpCode.STLOC6
    /// 4C : OpCode.LDLOC6
    /// 4D : OpCode.LDLOC2
    /// 4E : OpCode.LDLOC1
    /// 4F : OpCode.LDLOC0
    /// 50 : OpCode.PUSH4
    /// 51 : OpCode.PACKSTRUCT
    /// 52 : OpCode.RET
    /// </remarks>
    [DisplayName("testCollectionexpressions")]
    public abstract IList<object>? TestCollectionexpressions();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAE8QhcGgQzhCXJgQIQAlA
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH3
    /// 04 : OpCode.NEWARRAY_T 21
    /// 06 : OpCode.STLOC0
    /// 07 : OpCode.LDLOC0
    /// 08 : OpCode.PUSH0
    /// 09 : OpCode.PICKITEM
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.EQUAL
    /// 0C : OpCode.JMPIFNOT 04
    /// 0E : OpCode.PUSHT
    /// 0F : OpCode.RET
    /// 10 : OpCode.PUSHF
    /// 11 : OpCode.RET
    /// </remarks>
    [DisplayName("testDefaultArray")]
    public abstract bool? TestDefaultArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEAsLE79KNOdwaEA=
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.PUSH3
    /// 07 : OpCode.PACKSTRUCT
    /// 08 : OpCode.DUP
    /// 09 : OpCode.CALL E7
    /// 0B : OpCode.STLOC0
    /// 0C : OpCode.LDLOC0
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("testDefaultState")]
    public abstract object? TestDefaultState();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeMQhcBBxIjxpSmhpUdBFaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMNoQA==
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.NEWARRAY_T 21
    /// 06 : OpCode.STLOC0
    /// 07 : OpCode.PUSH0
    /// 08 : OpCode.STLOC1
    /// 09 : OpCode.JMP 3C
    /// 0B : OpCode.LDLOC1
    /// 0C : OpCode.DUP
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.LDLOC1
    /// 0F : OpCode.ROT
    /// 10 : OpCode.SETITEM
    /// 11 : OpCode.DROP
    /// 12 : OpCode.LDLOC1
    /// 13 : OpCode.DUP
    /// 14 : OpCode.INC
    /// 15 : OpCode.DUP
    /// 16 : OpCode.PUSHINT32 00000080
    /// 1B : OpCode.JMPGE 04
    /// 1D : OpCode.JMP 0A
    /// 1F : OpCode.DUP
    /// 20 : OpCode.PUSHINT32 FFFFFF7F
    /// 25 : OpCode.JMPLE 1E
    /// 27 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 30 : OpCode.AND
    /// 31 : OpCode.DUP
    /// 32 : OpCode.PUSHINT32 FFFFFF7F
    /// 37 : OpCode.JMPLE 0C
    /// 39 : OpCode.PUSHINT64 0000000001000000
    /// 42 : OpCode.SUB
    /// 43 : OpCode.STLOC1
    /// 44 : OpCode.DROP
    /// 45 : OpCode.LDLOC1
    /// 46 : OpCode.LDARG0
    /// 47 : OpCode.LT
    /// 48 : OpCode.JMPIF C3
    /// 4A : OpCode.LDLOC0
    /// 4B : OpCode.RET
    /// </remarks>
    [DisplayName("testDynamicArrayInit")]
    public abstract IList<object>? TestDynamicArrayInit(BigInteger? length);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMqIQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.SIZE
    /// 05 : OpCode.NEWBUFFER
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testDynamicArrayStringInit")]
    public abstract byte[]? TestDynamicArrayStringInit(string? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAEDcAAHBoEcBxaUrYJAQQznJqStgkBxTONwEAQc/nR5ZA
    /// 00 : OpCode.INITSLOT 0300
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.CALLT 0000
    /// 07 : OpCode.STLOC0
    /// 08 : OpCode.LDLOC0
    /// 09 : OpCode.PUSH1
    /// 0A : OpCode.PACK
    /// 0B : OpCode.STLOC1
    /// 0C : OpCode.LDLOC1
    /// 0D : OpCode.DUP
    /// 0E : OpCode.ISNULL
    /// 0F : OpCode.JMPIF 04
    /// 11 : OpCode.PUSH0
    /// 12 : OpCode.PICKITEM
    /// 13 : OpCode.STLOC2
    /// 14 : OpCode.LDLOC2
    /// 15 : OpCode.DUP
    /// 16 : OpCode.ISNULL
    /// 17 : OpCode.JMPIF 07
    /// 19 : OpCode.PUSH4
    /// 1A : OpCode.PICKITEM
    /// 1B : OpCode.CALLT 0100
    /// 1E : OpCode.SYSCALL CFE74796
    /// 23 : OpCode.RET
    /// </remarks>
    [DisplayName("testElementBinding")]
    public abstract void TestElementBinding();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAwnBoQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("testEmptyArray")]
    public abstract IList<object>? TestEmptyArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAE8QhcBBKaBBR0EURSmgRUdBFEkpoElHQRWhA
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH3
    /// 04 : OpCode.NEWARRAY_T 21
    /// 06 : OpCode.STLOC0
    /// 07 : OpCode.PUSH0
    /// 08 : OpCode.DUP
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.ROT
    /// 0C : OpCode.SETITEM
    /// 0D : OpCode.DROP
    /// 0E : OpCode.PUSH1
    /// 0F : OpCode.DUP
    /// 10 : OpCode.LDLOC0
    /// 11 : OpCode.PUSH1
    /// 12 : OpCode.ROT
    /// 13 : OpCode.SETITEM
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSH2
    /// 16 : OpCode.DUP
    /// 17 : OpCode.LDLOC0
    /// 18 : OpCode.PUSH2
    /// 19 : OpCode.ROT
    /// 1A : OpCode.SETITEM
    /// 1B : OpCode.DROP
    /// 1C : OpCode.LDLOC0
    /// 1D : OpCode.RET
    /// </remarks>
    [DisplayName("testIntArray")]
    public abstract IList<object>? TestIntArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAExIRE8BwFEpoEVHQRRVKaBJR0EVoQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH3
    /// 04 : OpCode.PUSH2
    /// 05 : OpCode.PUSH1
    /// 06 : OpCode.PUSH3
    /// 07 : OpCode.PACK
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.PUSH4
    /// 0A : OpCode.DUP
    /// 0B : OpCode.LDLOC0
    /// 0C : OpCode.PUSH1
    /// 0D : OpCode.ROT
    /// 0E : OpCode.SETITEM
    /// 0F : OpCode.DROP
    /// 10 : OpCode.PUSH5
    /// 11 : OpCode.DUP
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.PUSH2
    /// 14 : OpCode.ROT
    /// 15 : OpCode.SETITEM
    /// 16 : OpCode.DROP
    /// 17 : OpCode.LDLOC0
    /// 18 : OpCode.RET
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
    /// 00 : OpCode.INITSLOT 0400
    /// 03 : OpCode.PUSH4
    /// 04 : OpCode.PUSH3
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PUSH1
    /// 07 : OpCode.PUSH4
    /// 08 : OpCode.PACK
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.PUSH8
    /// 0B : OpCode.PUSH7
    /// 0C : OpCode.PUSH6
    /// 0D : OpCode.PUSH5
    /// 0E : OpCode.PUSH4
    /// 0F : OpCode.PACK
    /// 10 : OpCode.STLOC1
    /// 11 : OpCode.PUSH1
    /// 12 : OpCode.PUSH2
    /// 13 : OpCode.PUSH3
    /// 14 : OpCode.PUSH1
    /// 15 : OpCode.PUSH4
    /// 16 : OpCode.PACK
    /// 17 : OpCode.STLOC2
    /// 18 : OpCode.PUSH2
    /// 19 : OpCode.PUSH3
    /// 1A : OpCode.PUSH4
    /// 1B : OpCode.PUSH5
    /// 1C : OpCode.PUSH4
    /// 1D : OpCode.PACK
    /// 1E : OpCode.STLOC3
    /// 1F : OpCode.LDLOC3
    /// 20 : OpCode.LDLOC2
    /// 21 : OpCode.LDLOC1
    /// 22 : OpCode.LDLOC0
    /// 23 : OpCode.PUSH4
    /// 24 : OpCode.PACK
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("testJaggedArray")]
    public abstract IList<object>? TestJaggedArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQADAECAwTbMHAMBQYHCNswcQwBAwIB2zByDAUEAwLbMHNramloFMBA
    /// 00 : OpCode.INITSLOT 0400
    /// 03 : OpCode.PUSHDATA1 01020304
    /// 09 : OpCode.CONVERT 30
    /// 0B : OpCode.STLOC0
    /// 0C : OpCode.PUSHDATA1 05060708
    /// 12 : OpCode.CONVERT 30
    /// 14 : OpCode.STLOC1
    /// 15 : OpCode.PUSHDATA1 01030201
    /// 1B : OpCode.CONVERT 30
    /// 1D : OpCode.STLOC2
    /// 1E : OpCode.PUSHDATA1 05040302
    /// 24 : OpCode.CONVERT 30
    /// 26 : OpCode.STLOC3
    /// 27 : OpCode.LDLOC3
    /// 28 : OpCode.LDLOC2
    /// 29 : OpCode.LDLOC1
    /// 2A : OpCode.LDLOC0
    /// 2B : OpCode.PUSH4
    /// 2C : OpCode.PACK
    /// 2D : OpCode.RET
    /// </remarks>
    [DisplayName("testJaggedByteArray")]
    public abstract IList<object>? TestJaggedByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEAsLE79KNBJwE8QAcWhKaRJR0EVpEs5A
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.PUSH3
    /// 07 : OpCode.PACKSTRUCT
    /// 08 : OpCode.DUP
    /// 09 : OpCode.CALL 12
    /// 0B : OpCode.STLOC0
    /// 0C : OpCode.PUSH3
    /// 0D : OpCode.NEWARRAY_T 00
    /// 0F : OpCode.STLOC1
    /// 10 : OpCode.LDLOC0
    /// 11 : OpCode.DUP
    /// 12 : OpCode.LDLOC1
    /// 13 : OpCode.PUSH2
    /// 14 : OpCode.ROT
    /// 15 : OpCode.SETITEM
    /// 16 : OpCode.DROP
    /// 17 : OpCode.LDLOC1
    /// 18 : OpCode.PUSH2
    /// 19 : OpCode.PICKITEM
    /// 1A : OpCode.RET
    /// </remarks>
    [DisplayName("testStructArray")]
    public abstract object? TestStructArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAEAsLE79KNNJwaBHAcWkQznJqQA==
    /// 00 : OpCode.INITSLOT 0300
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.PUSH3
    /// 07 : OpCode.PACKSTRUCT
    /// 08 : OpCode.DUP
    /// 09 : OpCode.CALL D2
    /// 0B : OpCode.STLOC0
    /// 0C : OpCode.LDLOC0
    /// 0D : OpCode.PUSH1
    /// 0E : OpCode.PACK
    /// 0F : OpCode.STLOC1
    /// 10 : OpCode.LDLOC1
    /// 11 : OpCode.PUSH0
    /// 12 : OpCode.PICKITEM
    /// 13 : OpCode.STLOC2
    /// 14 : OpCode.LDLOC2
    /// 15 : OpCode.RET
    /// </remarks>
    [DisplayName("testStructArrayInit")]
    public abstract object? TestStructArrayInit();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: W0A=
    /// 00 : OpCode.LDSFLD3
    /// 01 : OpCode.RET
    /// </remarks>
    [DisplayName("testSupportedStandards")]
    public abstract IList<object>? TestSupportedStandards();

    #endregion
}
