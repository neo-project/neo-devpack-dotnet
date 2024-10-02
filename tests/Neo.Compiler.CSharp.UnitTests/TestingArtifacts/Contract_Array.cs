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
    [DisplayName("getTreeByteLengthPrefix")]
    public abstract byte[]? GetTreeByteLengthPrefix();
    // 0000 : LDSFLD0
    // 0001 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTreeByteLengthPrefix2")]
    public abstract byte[]? GetTreeByteLengthPrefix2();
    // 0000 : LDSFLD1
    // 0001 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testByteArrayOwner")]
    public abstract byte[]? TestByteArrayOwner();
    // 0000 : LDSFLD2
    // 0001 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testByteArrayOwnerCall")]
    public abstract byte[]? TestByteArrayOwnerCall();
    // 0000 : CALL
    // 0002 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCollectionexpressions")]
    public abstract IList<object>? TestCollectionexpressions();
    // 0000 : INITSLOT
    // 0003 : PUSH8
    // 0004 : PUSH7
    // 0005 : PUSH6
    // 0006 : PUSH5
    // 0007 : PUSH4
    // 0008 : PUSH3
    // 0009 : PUSH2
    // 000A : PUSH1
    // 000B : PUSH8
    // 000C : PACK
    // 000D : STLOC0
    // 000E : PUSHDATA1
    // 0015 : PUSHDATA1
    // 001A : PUSHDATA1
    // 001F : PUSH3
    // 0020 : PACK
    // 0021 : STLOC1
    // 0022 : PUSH9
    // 0023 : PUSH8
    // 0024 : PUSH7
    // 0025 : PUSH3
    // 0026 : PACK
    // 0027 : PUSH6
    // 0028 : PUSH5
    // 0029 : PUSH4
    // 002A : PUSH3
    // 002B : PACK
    // 002C : PUSH3
    // 002D : PUSH2
    // 002E : PUSH1
    // 002F : PUSH3
    // 0030 : PACK
    // 0031 : PUSH3
    // 0032 : PACK
    // 0033 : STLOC2
    // 0034 : PUSH3
    // 0035 : PUSH2
    // 0036 : PUSH1
    // 0037 : PUSH3
    // 0038 : PACK
    // 0039 : STLOC3
    // 003A : PUSH6
    // 003B : PUSH5
    // 003C : PUSH4
    // 003D : PUSH3
    // 003E : PACK
    // 003F : STLOC4
    // 0040 : PUSH9
    // 0041 : PUSH8
    // 0042 : PUSH7
    // 0043 : PUSH3
    // 0044 : PACK
    // 0045 : STLOC5
    // 0046 : LDLOC5
    // 0047 : LDLOC4
    // 0048 : LDLOC3
    // 0049 : PUSH3
    // 004A : PACK
    // 004B : STLOC6
    // 004C : NEWSTRUCT0
    // 004D : DUP
    // 004E : LDLOC0
    // 004F : APPEND
    // 0050 : DUP
    // 0051 : LDLOC1
    // 0052 : APPEND
    // 0053 : DUP
    // 0054 : LDLOC2
    // 0055 : APPEND
    // 0056 : DUP
    // 0057 : LDLOC6
    // 0058 : APPEND
    // 0059 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDefaultArray")]
    public abstract bool? TestDefaultArray();
    // 0000 : INITSLOT
    // 0003 : PUSH3
    // 0004 : NEWARRAY_T
    // 0006 : STLOC0
    // 0007 : LDLOC0
    // 0008 : PUSH0
    // 0009 : PICKITEM
    // 000A : PUSH0
    // 000B : EQUAL
    // 000C : JMPIFNOT
    // 000E : PUSHT
    // 000F : RET
    // 0010 : PUSHF
    // 0011 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDefaultState")]
    public abstract object? TestDefaultState();
    // 0000 : INITSLOT
    // 0003 : NEWSTRUCT0
    // 0004 : DUP
    // 0005 : PUSHNULL
    // 0006 : APPEND
    // 0007 : DUP
    // 0008 : PUSHNULL
    // 0009 : APPEND
    // 000A : DUP
    // 000B : PUSH0
    // 000C : APPEND
    // 000D : DUP
    // 000E : CALL
    // 0010 : STLOC0
    // 0011 : LDLOC0
    // 0012 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDynamicArrayInit")]
    public abstract IList<object>? TestDynamicArrayInit(BigInteger? length);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : NEWARRAY_T
    // 0006 : STLOC0
    // 0007 : PUSH0
    // 0008 : STLOC1
    // 0009 : JMP
    // 000B : LDLOC1
    // 000C : DUP
    // 000D : LDLOC0
    // 000E : LDLOC1
    // 000F : ROT
    // 0010 : SETITEM
    // 0011 : DROP
    // 0012 : LDLOC1
    // 0013 : DUP
    // 0014 : INC
    // 0015 : DUP
    // 0016 : PUSHINT32
    // 001B : JMPGE
    // 001D : JMP
    // 001F : DUP
    // 0020 : PUSHINT32
    // 0025 : JMPLE
    // 0027 : PUSHINT64
    // 0030 : AND
    // 0031 : DUP
    // 0032 : PUSHINT32
    // 0037 : JMPLE
    // 0039 : PUSHINT64
    // 0042 : SUB
    // 0043 : STLOC1
    // 0044 : DROP
    // 0045 : LDLOC1
    // 0046 : LDARG0
    // 0047 : LT
    // 0048 : JMPIF
    // 004A : LDLOC0
    // 004B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDynamicArrayStringInit")]
    public abstract byte[]? TestDynamicArrayStringInit(string? input);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : SIZE
    // 0005 : NEWBUFFER
    // 0006 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testElementBinding")]
    public abstract void TestElementBinding();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : CALLT
    // 0007 : STLOC0
    // 0008 : LDLOC0
    // 0009 : PUSH1
    // 000A : PACK
    // 000B : STLOC1
    // 000C : LDLOC1
    // 000D : DUP
    // 000E : ISNULL
    // 000F : JMPIF
    // 0011 : PUSH0
    // 0012 : PICKITEM
    // 0013 : STLOC2
    // 0014 : LDLOC2
    // 0015 : DUP
    // 0016 : ISNULL
    // 0017 : JMPIF
    // 0019 : PUSH4
    // 001A : PICKITEM
    // 001B : CALLT
    // 001E : SYSCALL
    // 0023 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEmptyArray")]
    public abstract IList<object>? TestEmptyArray();
    // 0000 : INITSLOT
    // 0003 : NEWARRAY0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIntArray")]
    public abstract IList<object>? TestIntArray();
    // 0000 : INITSLOT
    // 0003 : PUSH3
    // 0004 : NEWARRAY_T
    // 0006 : STLOC0
    // 0007 : PUSH0
    // 0008 : DUP
    // 0009 : LDLOC0
    // 000A : PUSH0
    // 000B : ROT
    // 000C : SETITEM
    // 000D : DROP
    // 000E : PUSH1
    // 000F : DUP
    // 0010 : LDLOC0
    // 0011 : PUSH1
    // 0012 : ROT
    // 0013 : SETITEM
    // 0014 : DROP
    // 0015 : PUSH2
    // 0016 : DUP
    // 0017 : LDLOC0
    // 0018 : PUSH2
    // 0019 : ROT
    // 001A : SETITEM
    // 001B : DROP
    // 001C : LDLOC0
    // 001D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIntArrayInit")]
    public abstract IList<object>? TestIntArrayInit();
    // 0000 : INITSLOT
    // 0003 : PUSH3
    // 0004 : PUSH2
    // 0005 : PUSH1
    // 0006 : PUSH3
    // 0007 : PACK
    // 0008 : STLOC0
    // 0009 : PUSH4
    // 000A : DUP
    // 000B : LDLOC0
    // 000C : PUSH1
    // 000D : ROT
    // 000E : SETITEM
    // 000F : DROP
    // 0010 : PUSH5
    // 0011 : DUP
    // 0012 : LDLOC0
    // 0013 : PUSH2
    // 0014 : ROT
    // 0015 : SETITEM
    // 0016 : DROP
    // 0017 : LDLOC0
    // 0018 : RET

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
    [DisplayName("testJaggedArray")]
    public abstract IList<object>? TestJaggedArray();
    // 0000 : INITSLOT
    // 0003 : PUSH4
    // 0004 : PUSH3
    // 0005 : PUSH2
    // 0006 : PUSH1
    // 0007 : PUSH4
    // 0008 : PACK
    // 0009 : STLOC0
    // 000A : PUSH8
    // 000B : PUSH7
    // 000C : PUSH6
    // 000D : PUSH5
    // 000E : PUSH4
    // 000F : PACK
    // 0010 : STLOC1
    // 0011 : PUSH1
    // 0012 : PUSH2
    // 0013 : PUSH3
    // 0014 : PUSH1
    // 0015 : PUSH4
    // 0016 : PACK
    // 0017 : STLOC2
    // 0018 : PUSH2
    // 0019 : PUSH3
    // 001A : PUSH4
    // 001B : PUSH5
    // 001C : PUSH4
    // 001D : PACK
    // 001E : STLOC3
    // 001F : LDLOC3
    // 0020 : LDLOC2
    // 0021 : LDLOC1
    // 0022 : LDLOC0
    // 0023 : PUSH4
    // 0024 : PACK
    // 0025 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testJaggedByteArray")]
    public abstract IList<object>? TestJaggedByteArray();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0009 : CONVERT
    // 000B : STLOC0
    // 000C : PUSHDATA1
    // 0012 : CONVERT
    // 0014 : STLOC1
    // 0015 : PUSHDATA1
    // 001B : CONVERT
    // 001D : STLOC2
    // 001E : PUSHDATA1
    // 0024 : CONVERT
    // 0026 : STLOC3
    // 0027 : LDLOC3
    // 0028 : LDLOC2
    // 0029 : LDLOC1
    // 002A : LDLOC0
    // 002B : PUSH4
    // 002C : PACK
    // 002D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStructArray")]
    public abstract object? TestStructArray();
    // 0000 : INITSLOT
    // 0003 : NEWSTRUCT0
    // 0004 : DUP
    // 0005 : PUSHNULL
    // 0006 : APPEND
    // 0007 : DUP
    // 0008 : PUSHNULL
    // 0009 : APPEND
    // 000A : DUP
    // 000B : PUSH0
    // 000C : APPEND
    // 000D : DUP
    // 000E : CALL
    // 0010 : STLOC0
    // 0011 : PUSH3
    // 0012 : NEWARRAY_T
    // 0014 : STLOC1
    // 0015 : LDLOC0
    // 0016 : DUP
    // 0017 : LDLOC1
    // 0018 : PUSH2
    // 0019 : ROT
    // 001A : SETITEM
    // 001B : DROP
    // 001C : LDLOC1
    // 001D : PUSH2
    // 001E : PICKITEM
    // 001F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStructArrayInit")]
    public abstract object? TestStructArrayInit();
    // 0000 : INITSLOT
    // 0003 : NEWSTRUCT0
    // 0004 : DUP
    // 0005 : PUSHNULL
    // 0006 : APPEND
    // 0007 : DUP
    // 0008 : PUSHNULL
    // 0009 : APPEND
    // 000A : DUP
    // 000B : PUSH0
    // 000C : APPEND
    // 000D : DUP
    // 000E : CALL
    // 0010 : STLOC0
    // 0011 : LDLOC0
    // 0012 : PUSH1
    // 0013 : PACK
    // 0014 : STLOC1
    // 0015 : LDLOC1
    // 0016 : PUSH0
    // 0017 : PICKITEM
    // 0018 : STLOC2
    // 0019 : LDLOC2
    // 001A : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSupportedStandards")]
    public abstract IList<object>? TestSupportedStandards();
    // 0000 : LDSFLD3
    // 0001 : RET

    #endregion

}
