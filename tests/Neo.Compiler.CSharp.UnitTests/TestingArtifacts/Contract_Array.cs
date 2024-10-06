using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Array(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Array"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getTreeByteLengthPrefix"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""getTreeByteLengthPrefix2"",""parameters"":[],""returntype"":""ByteArray"",""offset"":2,""safe"":false},{""name"":""testJaggedArray"",""parameters"":[],""returntype"":""Array"",""offset"":4,""safe"":false},{""name"":""testJaggedByteArray"",""parameters"":[],""returntype"":""Array"",""offset"":42,""safe"":false},{""name"":""testIntArray"",""parameters"":[],""returntype"":""Array"",""offset"":88,""safe"":false},{""name"":""testDefaultArray"",""parameters"":[],""returntype"":""Boolean"",""offset"":118,""safe"":false},{""name"":""testIntArrayInit"",""parameters"":[],""returntype"":""Array"",""offset"":136,""safe"":false},{""name"":""testIntArrayInit2"",""parameters"":[],""returntype"":""Array"",""offset"":161,""safe"":false},{""name"":""testIntArrayInit3"",""parameters"":[],""returntype"":""Array"",""offset"":186,""safe"":false},{""name"":""testDynamicArrayInit"",""parameters"":[{""name"":""length"",""type"":""Integer""}],""returntype"":""Array"",""offset"":211,""safe"":false},{""name"":""testDynamicArrayStringInit"",""parameters"":[{""name"":""input"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":287,""safe"":false},{""name"":""testStructArray"",""parameters"":[],""returntype"":""Any"",""offset"":294,""safe"":false},{""name"":""testDefaultState"",""parameters"":[],""returntype"":""Any"",""offset"":342,""safe"":false},{""name"":""testEmptyArray"",""parameters"":[],""returntype"":""Array"",""offset"":361,""safe"":false},{""name"":""testStructArrayInit"",""parameters"":[],""returntype"":""Any"",""offset"":368,""safe"":false},{""name"":""testByteArrayOwner"",""parameters"":[],""returntype"":""ByteArray"",""offset"":425,""safe"":false},{""name"":""testByteArrayOwnerCall"",""parameters"":[],""returntype"":""ByteArray"",""offset"":427,""safe"":false},{""name"":""testSupportedStandards"",""parameters"":[],""returntype"":""Array"",""offset"":430,""safe"":false},{""name"":""testElementBinding"",""parameters"":[],""returntype"":""Void"",""offset"":432,""safe"":false},{""name"":""testCollectionexpressions"",""parameters"":[],""returntype"":""Array"",""offset"":468,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":558,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""getBlock""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAK+8gQxQDYqd8FQmcfmTBL3ALZl2ghnZXRCbG9jawEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/WoCWEBZQFcEABQTEhEUwHAYFxYVFMBxERITERTAchITFBUUwHNramloFMBAVwQADAQBAgME2zBwDAQFBgcI2zBxDAQBAwIB2zByDAQFBAMC2zBza2ppaBTAQFcBABPEIXAQSmgQUdBFEUpoEVHQRRJKaBJR0EVoQFcBABPEIXBoEM4QlyYECEAJQFcBABMSERPAcBRKaBFR0EUVSmgSUdBFaEBXAQATEhETwHAUSmgRUdBFFUpoElHQRWhAVwEAExIRE8BwFEpoEVHQRRVKaBJR0EVoQFcCAXjEIXAQcSI8aUpoaVHQRWlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4tSTDaEBXAAF4yohAVwIAxUoLz0oLz0oQz0o0EnATxABxaEppElHQRWkSzkBXAAF4EAvQeBEL0HgSENBAVwEAxUoLz0oLz0oQz0o04nBoQFcBAMJwaEBXAwDFSgvPSgvPShDPSjTIcGgRwHFpEM5yakBXAQAMFPZkQ0mNOHjTK5lOThKDxpNEIdr+2zBwaEBaQDTgQFtAVwMAEDcAAHBoEcBxaUrYJAQQznJqStgkBxTONwEAQc/nR5ZAVwcAGBcWFRQTEhEYwHAMBXRocmVlDAN0d28MA29uZRPAcRkYFxPAFhUUE8ATEhETwBPAchMSERPAcxYVFBPAdBkYFxPAdW1saxPAdsVKaM9Kac9Kas9Kbs9AVgQMAgED2zBgDAIBA9swYQwU9mRDSY04eNMrmU5OEoPGk0Qh2v7bMGIMBk5FUC0xMAwFTkVQLTUSwGNALn4FWw=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WEA=
    /// 0000 : OpCode.LDSFLD0
    /// 0001 : OpCode.RET
    /// </remarks>
    [DisplayName("getTreeByteLengthPrefix")]
    public abstract byte[]? GetTreeByteLengthPrefix();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WUA=
    /// 0000 : OpCode.LDSFLD1
    /// 0001 : OpCode.RET
    /// </remarks>
    [DisplayName("getTreeByteLengthPrefix2")]
    public abstract byte[]? GetTreeByteLengthPrefix2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WkA=
    /// 0000 : OpCode.LDSFLD2
    /// 0001 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteArrayOwner")]
    public abstract byte[]? TestByteArrayOwner();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NOBA
    /// 0000 : OpCode.CALL E0
    /// 0002 : OpCode.RET
    /// </remarks>
    [DisplayName("testByteArrayOwnerCall")]
    public abstract byte[]? TestByteArrayOwnerCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwcAGBcWFRQTEhEYwHAMdGhyZWUMdHdvDG9uZRPAcRkYFxPAFhUUE8ATEhETwBPAchMSERPAcxYVFBPAdBkYFxPAdW1saxPAdsVKaM9Kac9Kas9Kbs9A
    /// 0000 : OpCode.INITSLOT 0700
    /// 0003 : OpCode.PUSH8
    /// 0004 : OpCode.PUSH7
    /// 0005 : OpCode.PUSH6
    /// 0006 : OpCode.PUSH5
    /// 0007 : OpCode.PUSH4
    /// 0008 : OpCode.PUSH3
    /// 0009 : OpCode.PUSH2
    /// 000A : OpCode.PUSH1
    /// 000B : OpCode.PUSH8
    /// 000C : OpCode.PACK
    /// 000D : OpCode.STLOC0
    /// 000E : OpCode.PUSHDATA1 7468726565
    /// 0015 : OpCode.PUSHDATA1 74776F
    /// 001A : OpCode.PUSHDATA1 6F6E65
    /// 001F : OpCode.PUSH3
    /// 0020 : OpCode.PACK
    /// 0021 : OpCode.STLOC1
    /// 0022 : OpCode.PUSH9
    /// 0023 : OpCode.PUSH8
    /// 0024 : OpCode.PUSH7
    /// 0025 : OpCode.PUSH3
    /// 0026 : OpCode.PACK
    /// 0027 : OpCode.PUSH6
    /// 0028 : OpCode.PUSH5
    /// 0029 : OpCode.PUSH4
    /// 002A : OpCode.PUSH3
    /// 002B : OpCode.PACK
    /// 002C : OpCode.PUSH3
    /// 002D : OpCode.PUSH2
    /// 002E : OpCode.PUSH1
    /// 002F : OpCode.PUSH3
    /// 0030 : OpCode.PACK
    /// 0031 : OpCode.PUSH3
    /// 0032 : OpCode.PACK
    /// 0033 : OpCode.STLOC2
    /// 0034 : OpCode.PUSH3
    /// 0035 : OpCode.PUSH2
    /// 0036 : OpCode.PUSH1
    /// 0037 : OpCode.PUSH3
    /// 0038 : OpCode.PACK
    /// 0039 : OpCode.STLOC3
    /// 003A : OpCode.PUSH6
    /// 003B : OpCode.PUSH5
    /// 003C : OpCode.PUSH4
    /// 003D : OpCode.PUSH3
    /// 003E : OpCode.PACK
    /// 003F : OpCode.STLOC4
    /// 0040 : OpCode.PUSH9
    /// 0041 : OpCode.PUSH8
    /// 0042 : OpCode.PUSH7
    /// 0043 : OpCode.PUSH3
    /// 0044 : OpCode.PACK
    /// 0045 : OpCode.STLOC5
    /// 0046 : OpCode.LDLOC5
    /// 0047 : OpCode.LDLOC4
    /// 0048 : OpCode.LDLOC3
    /// 0049 : OpCode.PUSH3
    /// 004A : OpCode.PACK
    /// 004B : OpCode.STLOC6
    /// 004C : OpCode.NEWSTRUCT0
    /// 004D : OpCode.DUP
    /// 004E : OpCode.LDLOC0
    /// 004F : OpCode.APPEND
    /// 0050 : OpCode.DUP
    /// 0051 : OpCode.LDLOC1
    /// 0052 : OpCode.APPEND
    /// 0053 : OpCode.DUP
    /// 0054 : OpCode.LDLOC2
    /// 0055 : OpCode.APPEND
    /// 0056 : OpCode.DUP
    /// 0057 : OpCode.LDLOC6
    /// 0058 : OpCode.APPEND
    /// 0059 : OpCode.RET
    /// </remarks>
    [DisplayName("testCollectionexpressions")]
    public abstract IList<object>? TestCollectionexpressions();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAE8QhcGgQzhCXJgQIQAlA
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSH3
    /// 0004 : OpCode.NEWARRAY_T 21
    /// 0006 : OpCode.STLOC0
    /// 0007 : OpCode.LDLOC0
    /// 0008 : OpCode.PUSH0
    /// 0009 : OpCode.PICKITEM
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.EQUAL
    /// 000C : OpCode.JMPIFNOT 04
    /// 000E : OpCode.PUSHT
    /// 000F : OpCode.RET
    /// 0010 : OpCode.PUSHF
    /// 0011 : OpCode.RET
    /// </remarks>
    [DisplayName("testDefaultArray")]
    public abstract bool? TestDefaultArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAxUoLz0oLz0oQz0o04nBoQA==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.NEWSTRUCT0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHNULL
    /// 0006 : OpCode.APPEND
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.PUSHNULL
    /// 0009 : OpCode.APPEND
    /// 000A : OpCode.DUP
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.APPEND
    /// 000D : OpCode.DUP
    /// 000E : OpCode.CALL E2
    /// 0010 : OpCode.STLOC0
    /// 0011 : OpCode.LDLOC0
    /// 0012 : OpCode.RET
    /// </remarks>
    [DisplayName("testDefaultState")]
    public abstract object? TestDefaultState();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeMQhcBBxIjxpSmhpUdBFaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaXi1JMNoQA==
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.NEWARRAY_T 21
    /// 0006 : OpCode.STLOC0
    /// 0007 : OpCode.PUSH0
    /// 0008 : OpCode.STLOC1
    /// 0009 : OpCode.JMP 3C
    /// 000B : OpCode.LDLOC1
    /// 000C : OpCode.DUP
    /// 000D : OpCode.LDLOC0
    /// 000E : OpCode.LDLOC1
    /// 000F : OpCode.ROT
    /// 0010 : OpCode.SETITEM
    /// 0011 : OpCode.DROP
    /// 0012 : OpCode.LDLOC1
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.INC
    /// 0015 : OpCode.DUP
    /// 0016 : OpCode.PUSHINT32 00000080
    /// 001B : OpCode.JMPGE 04
    /// 001D : OpCode.JMP 0A
    /// 001F : OpCode.DUP
    /// 0020 : OpCode.PUSHINT32 FFFFFF7F
    /// 0025 : OpCode.JMPLE 1E
    /// 0027 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0030 : OpCode.AND
    /// 0031 : OpCode.DUP
    /// 0032 : OpCode.PUSHINT32 FFFFFF7F
    /// 0037 : OpCode.JMPLE 0C
    /// 0039 : OpCode.PUSHINT64 0000000001000000
    /// 0042 : OpCode.SUB
    /// 0043 : OpCode.STLOC1
    /// 0044 : OpCode.DROP
    /// 0045 : OpCode.LDLOC1
    /// 0046 : OpCode.LDARG0
    /// 0047 : OpCode.LT
    /// 0048 : OpCode.JMPIF C3
    /// 004A : OpCode.LDLOC0
    /// 004B : OpCode.RET
    /// </remarks>
    [DisplayName("testDynamicArrayInit")]
    public abstract IList<object>? TestDynamicArrayInit(BigInteger? length);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMqIQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.SIZE
    /// 0005 : OpCode.NEWBUFFER
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("testDynamicArrayStringInit")]
    public abstract byte[]? TestDynamicArrayStringInit(string? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAEDcAAHBoEcBxaUrYJAQQznJqStgkBxTONwEAQc/nR5ZA
    /// 0000 : OpCode.INITSLOT 0300
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.CALLT 0000
    /// 0007 : OpCode.STLOC0
    /// 0008 : OpCode.LDLOC0
    /// 0009 : OpCode.PUSH1
    /// 000A : OpCode.PACK
    /// 000B : OpCode.STLOC1
    /// 000C : OpCode.LDLOC1
    /// 000D : OpCode.DUP
    /// 000E : OpCode.ISNULL
    /// 000F : OpCode.JMPIF 04
    /// 0011 : OpCode.PUSH0
    /// 0012 : OpCode.PICKITEM
    /// 0013 : OpCode.STLOC2
    /// 0014 : OpCode.LDLOC2
    /// 0015 : OpCode.DUP
    /// 0016 : OpCode.ISNULL
    /// 0017 : OpCode.JMPIF 07
    /// 0019 : OpCode.PUSH4
    /// 001A : OpCode.PICKITEM
    /// 001B : OpCode.CALLT 0100
    /// 001E : OpCode.SYSCALL CFE74796
    /// 0023 : OpCode.RET
    /// </remarks>
    [DisplayName("testElementBinding")]
    public abstract void TestElementBinding();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAwnBoQA==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.NEWARRAY0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("testEmptyArray")]
    public abstract IList<object>? TestEmptyArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAE8QhcBBKaBBR0EURSmgRUdBFEkpoElHQRWhA
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSH3
    /// 0004 : OpCode.NEWARRAY_T 21
    /// 0006 : OpCode.STLOC0
    /// 0007 : OpCode.PUSH0
    /// 0008 : OpCode.DUP
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.ROT
    /// 000C : OpCode.SETITEM
    /// 000D : OpCode.DROP
    /// 000E : OpCode.PUSH1
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.LDLOC0
    /// 0011 : OpCode.PUSH1
    /// 0012 : OpCode.ROT
    /// 0013 : OpCode.SETITEM
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSH2
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.LDLOC0
    /// 0018 : OpCode.PUSH2
    /// 0019 : OpCode.ROT
    /// 001A : OpCode.SETITEM
    /// 001B : OpCode.DROP
    /// 001C : OpCode.LDLOC0
    /// 001D : OpCode.RET
    /// </remarks>
    [DisplayName("testIntArray")]
    public abstract IList<object>? TestIntArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAExIRE8BwFEpoEVHQRRVKaBJR0EVoQA==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSH3
    /// 0004 : OpCode.PUSH2
    /// 0005 : OpCode.PUSH1
    /// 0006 : OpCode.PUSH3
    /// 0007 : OpCode.PACK
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.PUSH4
    /// 000A : OpCode.DUP
    /// 000B : OpCode.LDLOC0
    /// 000C : OpCode.PUSH1
    /// 000D : OpCode.ROT
    /// 000E : OpCode.SETITEM
    /// 000F : OpCode.DROP
    /// 0010 : OpCode.PUSH5
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.LDLOC0
    /// 0013 : OpCode.PUSH2
    /// 0014 : OpCode.ROT
    /// 0015 : OpCode.SETITEM
    /// 0016 : OpCode.DROP
    /// 0017 : OpCode.LDLOC0
    /// 0018 : OpCode.RET
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
    /// 0000 : OpCode.INITSLOT 0400
    /// 0003 : OpCode.PUSH4
    /// 0004 : OpCode.PUSH3
    /// 0005 : OpCode.PUSH2
    /// 0006 : OpCode.PUSH1
    /// 0007 : OpCode.PUSH4
    /// 0008 : OpCode.PACK
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.PUSH8
    /// 000B : OpCode.PUSH7
    /// 000C : OpCode.PUSH6
    /// 000D : OpCode.PUSH5
    /// 000E : OpCode.PUSH4
    /// 000F : OpCode.PACK
    /// 0010 : OpCode.STLOC1
    /// 0011 : OpCode.PUSH1
    /// 0012 : OpCode.PUSH2
    /// 0013 : OpCode.PUSH3
    /// 0014 : OpCode.PUSH1
    /// 0015 : OpCode.PUSH4
    /// 0016 : OpCode.PACK
    /// 0017 : OpCode.STLOC2
    /// 0018 : OpCode.PUSH2
    /// 0019 : OpCode.PUSH3
    /// 001A : OpCode.PUSH4
    /// 001B : OpCode.PUSH5
    /// 001C : OpCode.PUSH4
    /// 001D : OpCode.PACK
    /// 001E : OpCode.STLOC3
    /// 001F : OpCode.LDLOC3
    /// 0020 : OpCode.LDLOC2
    /// 0021 : OpCode.LDLOC1
    /// 0022 : OpCode.LDLOC0
    /// 0023 : OpCode.PUSH4
    /// 0024 : OpCode.PACK
    /// 0025 : OpCode.RET
    /// </remarks>
    [DisplayName("testJaggedArray")]
    public abstract IList<object>? TestJaggedArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQADAECAwTbMHAMBQYHCNswcQwBAwIB2zByDAUEAwLbMHNramloFMBA
    /// 0000 : OpCode.INITSLOT 0400
    /// 0003 : OpCode.PUSHDATA1 01020304
    /// 0009 : OpCode.CONVERT 30
    /// 000B : OpCode.STLOC0
    /// 000C : OpCode.PUSHDATA1 05060708
    /// 0012 : OpCode.CONVERT 30
    /// 0014 : OpCode.STLOC1
    /// 0015 : OpCode.PUSHDATA1 01030201
    /// 001B : OpCode.CONVERT 30
    /// 001D : OpCode.STLOC2
    /// 001E : OpCode.PUSHDATA1 05040302
    /// 0024 : OpCode.CONVERT 30
    /// 0026 : OpCode.STLOC3
    /// 0027 : OpCode.LDLOC3
    /// 0028 : OpCode.LDLOC2
    /// 0029 : OpCode.LDLOC1
    /// 002A : OpCode.LDLOC0
    /// 002B : OpCode.PUSH4
    /// 002C : OpCode.PACK
    /// 002D : OpCode.RET
    /// </remarks>
    [DisplayName("testJaggedByteArray")]
    public abstract IList<object>? TestJaggedByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAxUoLz0oLz0oQz0o0EnATxABxaEppElHQRWkSzkA=
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.NEWSTRUCT0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHNULL
    /// 0006 : OpCode.APPEND
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.PUSHNULL
    /// 0009 : OpCode.APPEND
    /// 000A : OpCode.DUP
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.APPEND
    /// 000D : OpCode.DUP
    /// 000E : OpCode.CALL 12
    /// 0010 : OpCode.STLOC0
    /// 0011 : OpCode.PUSH3
    /// 0012 : OpCode.NEWARRAY_T 00
    /// 0014 : OpCode.STLOC1
    /// 0015 : OpCode.LDLOC0
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.LDLOC1
    /// 0018 : OpCode.PUSH2
    /// 0019 : OpCode.ROT
    /// 001A : OpCode.SETITEM
    /// 001B : OpCode.DROP
    /// 001C : OpCode.LDLOC1
    /// 001D : OpCode.PUSH2
    /// 001E : OpCode.PICKITEM
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("testStructArray")]
    public abstract object? TestStructArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAxUoLz0oLz0oQz0o0yHBoEcBxaRDOcmpA
    /// 0000 : OpCode.INITSLOT 0300
    /// 0003 : OpCode.NEWSTRUCT0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHNULL
    /// 0006 : OpCode.APPEND
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.PUSHNULL
    /// 0009 : OpCode.APPEND
    /// 000A : OpCode.DUP
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.APPEND
    /// 000D : OpCode.DUP
    /// 000E : OpCode.CALL C8
    /// 0010 : OpCode.STLOC0
    /// 0011 : OpCode.LDLOC0
    /// 0012 : OpCode.PUSH1
    /// 0013 : OpCode.PACK
    /// 0014 : OpCode.STLOC1
    /// 0015 : OpCode.LDLOC1
    /// 0016 : OpCode.PUSH0
    /// 0017 : OpCode.PICKITEM
    /// 0018 : OpCode.STLOC2
    /// 0019 : OpCode.LDLOC2
    /// 001A : OpCode.RET
    /// </remarks>
    [DisplayName("testStructArrayInit")]
    public abstract object? TestStructArrayInit();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: W0A=
    /// 0000 : OpCode.LDSFLD3
    /// 0001 : OpCode.RET
    /// </remarks>
    [DisplayName("testSupportedStandards")]
    public abstract IList<object>? TestSupportedStandards();

    #endregion

}
