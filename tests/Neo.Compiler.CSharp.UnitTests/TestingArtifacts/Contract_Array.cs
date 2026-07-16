using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Array(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Array"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getTreeByteLengthPrefix"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""getTreeByteLengthPrefix2"",""parameters"":[],""returntype"":""ByteArray"",""offset"":2,""safe"":false},{""name"":""testJaggedArray"",""parameters"":[],""returntype"":""Array"",""offset"":4,""safe"":false},{""name"":""testJaggedByteArray"",""parameters"":[],""returntype"":""Array"",""offset"":43,""safe"":false},{""name"":""testIntArray"",""parameters"":[],""returntype"":""Array"",""offset"":90,""safe"":false},{""name"":""testDefaultArray"",""parameters"":[],""returntype"":""Boolean"",""offset"":111,""safe"":false},{""name"":""testIntArrayInit"",""parameters"":[],""returntype"":""Array"",""offset"":129,""safe"":false},{""name"":""testIntArrayInit2"",""parameters"":[],""returntype"":""Array"",""offset"":148,""safe"":false},{""name"":""testIntArrayInit3"",""parameters"":[],""returntype"":""Array"",""offset"":167,""safe"":false},{""name"":""testDynamicArrayInit"",""parameters"":[{""name"":""length"",""type"":""Integer""}],""returntype"":""Array"",""offset"":186,""safe"":false},{""name"":""testDynamicArrayStringInit"",""parameters"":[{""name"":""input"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":246,""safe"":false},{""name"":""testStructArray"",""parameters"":[],""returntype"":""Any"",""offset"":253,""safe"":false},{""name"":""testDefaultState"",""parameters"":[],""returntype"":""Any"",""offset"":274,""safe"":false},{""name"":""testEmptyArray"",""parameters"":[],""returntype"":""Array"",""offset"":285,""safe"":false},{""name"":""testStructArrayInit"",""parameters"":[],""returntype"":""Any"",""offset"":292,""safe"":false},{""name"":""testByteArrayOwner"",""parameters"":[],""returntype"":""ByteArray"",""offset"":341,""safe"":false},{""name"":""testByteArrayOwnerCall"",""parameters"":[],""returntype"":""ByteArray"",""offset"":343,""safe"":false},{""name"":""testSupportedStandards"",""parameters"":[],""returntype"":""Array"",""offset"":346,""safe"":false},{""name"":""testElementBinding"",""parameters"":[],""returntype"":""Void"",""offset"":348,""safe"":false},{""name"":""testCollectionexpressions"",""parameters"":[],""returntype"":""Array"",""offset"":384,""safe"":false},{""name"":""multiDimensionalCreatedBySize"",""parameters"":[],""returntype"":""Array"",""offset"":469,""safe"":false},{""name"":""multiDimensionalNewOnly"",""parameters"":[],""returntype"":""Integer"",""offset"":513,""safe"":false},{""name"":""multiDimensionalInitializer"",""parameters"":[],""returntype"":""Array"",""offset"":546,""safe"":false},{""name"":""multiDimensionalImplicitInitializer"",""parameters"":[],""returntype"":""Array"",""offset"":557,""safe"":false},{""name"":""multiDimensionalAssignments"",""parameters"":[],""returntype"":""Integer"",""offset"":568,""safe"":false},{""name"":""multiDimensionalCoalesce"",""parameters"":[],""returntype"":""Array"",""offset"":794,""safe"":false},{""name"":""multiDimensionalForeachSum"",""parameters"":[],""returntype"":""Integer"",""offset"":874,""safe"":false},{""name"":""multiDimensionalByteInitializer"",""parameters"":[],""returntype"":""Array"",""offset"":970,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":985,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""getBlock""]}],""trusts"":[],""extra"":{""Version"":""3.10.0"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAK+8gQxQDYqd8FQmcfmTBL3ALZl2ghnZXRCbG9jawEAAQXA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABBQAA/RUEWEBZQFcEABQTEhEUwHAYFxYVFMBxERITERTAchITFBUUwHNoaWprVBTAQFcEAAwEAQIDBNswcAwEBQYHCNswcQwEAQMCAdswcgwEBQQDAtswc2hpamtUFMBAVwEAE8QhcGgQENBoERHQaBIS0GhAVwEAE8QhcGgQzhCXJgQIQAlAVwEAExIRE8BwaBEU0GgSFdBoQFcBABMSERPAcGgRFNBoEhXQaEBXAQATEhETwHBoERTQaBIV0GhAVwIBeMQhcBBxIixoaWnQaUqcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeLUk02hAVwABeMqIQFcCABALCxO/cBPEAHFpEmjQaRLOQFcBABALCxO/cGhAVwEAwnBoQFcDABALCxO/cGgRwHFpEM5yakBXAQAMFPZkQ0mNOHjTK5lOThKDxpNEIdr+2zBwaEBaQDTgQFtAVwMAEDcAAHBoEcBxaUrYJAQQznJqStgkBxTONwEAQc/nR5ZAVwcAGBcWFRQTEhEYwHAMBXRocmVlDAN0d28MA29uZRPAcRkYFxPAFhUUE8ATEhETwBPAchMSERPAcxYVFBPAdBkYFxPAdWtsbVMTwHZoaWpuVBS/QFcFABJxE3Jpw3MQdCIMasQhbGtT0GycdGxpMPRrcGgQzhEV0GgRzhIX0GhAVwUAEnETcmnDcxB0IgxqxCFsa1PQbJx0bGkw9GtwACpAFBMSwBIREsASwEAYFxLAFhUSwBLAQFcFABJxEnJpw3MQdCIMasQhbGtT0GycdGxpMPRrcGgQzhAT0GgRzhFoEM4QzpycSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACf0GgRzhFLS84VnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pUU9BFaBHOEUtLzkpUU51KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ/QRWgRzhFLS86cSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSlRT0EVoEc4RzkBXBQARcRJyacNzEHQiC2rDbGtT0GycdGxpMPVrcGgQzhBLS87YJAXOIgwMBGxlZnRKVFPQRWgQzhFLS87YJAXOIg0MBXJpZ2h0SlRT0EVoQFcIABYVFBPAExIRE8ASwHAQcWhyanYQdCJBamzOdwcQdSIvbwdtznNpa55KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xbZx1bW8HyjDPbJx0bG7KML5pQAwCAwTbMAwCAQLbMBLAQFYEDAIBA9swYAwCAQPbMGEMFPZkQ0mNOHjTK5lOThKDxpNEIdr+2zBiDAZORVAtMTAMBU5FUC01EsBjQKBcWU0=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WEA=
    /// LDSFLD0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTreeByteLengthPrefix")]
    public abstract byte[]? GetTreeByteLengthPrefix();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WUA=
    /// LDSFLD1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getTreeByteLengthPrefix2")]
    public abstract byte[]? GetTreeByteLengthPrefix2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUAEnEScmnDcxB0IgxqxCFsa1PQbJx0bGkw9GtwaBDOEBPQaBHOEWgQzhDOnJxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ/QaBHOEUtLzhWeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSlRT0EVoEc4RS0vOSlRTnUrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn9BFaBHOEUtLzpxKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KVFPQRWgRzhHOQA==
    /// INITSLOT 0500 [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// NEWARRAY [512 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC4 [2 datoshi]
    /// JMP 0C [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// NEWARRAY_T 21 'Integer' [512 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC4 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLT F4 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// INC [4 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH5 [1 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// REVERSE4 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// DUP [2 datoshi]
    /// REVERSE4 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// DEC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// REVERSE4 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("multiDimensionalAssignments")]
    public abstract BigInteger? MultiDimensionalAssignments();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAIDBNswDAIBAtswEsBA
    /// PUSHDATA1 0304 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// PUSHDATA1 0102 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("multiDimensionalByteInitializer")]
    public abstract IList<object>? MultiDimensionalByteInitializer();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUAEXEScmnDcxB0Igtqw2xrU9BsnHRsaTD1a3BoEM4QS0vO2CQFziIMDARsZWZ0SlRT0EVoEM4RS0vO2CQFziINDAVyaWdodEpUU9BFaEA=
    /// INITSLOT 0500 [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// NEWARRAY [512 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC4 [2 datoshi]
    /// JMP 0B [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// NEWARRAY [512 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC4 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLT F5 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// JMP 0C [2 datoshi]
    /// PUSHDATA1 6C656674 'left' [8 datoshi]
    /// DUP [2 datoshi]
    /// REVERSE4 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// JMP 0D [2 datoshi]
    /// PUSHDATA1 7269676874 'right' [8 datoshi]
    /// DUP [2 datoshi]
    /// REVERSE4 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("multiDimensionalCoalesce")]
    public abstract IList<object>? MultiDimensionalCoalesce();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUAEnETcmnDcxB0IgxqxCFsa1PQbJx0bGkw9GtwaBDOERXQaBHOEhfQaEA=
    /// INITSLOT 0500 [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// NEWARRAY [512 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC4 [2 datoshi]
    /// JMP 0C [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// NEWARRAY_T 21 'Integer' [512 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC4 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLT F4 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH7 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("multiDimensionalCreatedBySize")]
    public abstract IList<object>? MultiDimensionalCreatedBySize();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwgAFhUUE8ATEhETwBLAcBBxaHJqdhB0IkFqbM53BxB1Ii9vB23Oc2lrnkrKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FtnHVtbwfKMM9snHRsbsowvmlA
    /// INITSLOT 0800 [64 datoshi]
    /// PUSH6 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// STLOC6 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC4 [2 datoshi]
    /// JMP 41 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC 07 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC5 [2 datoshi]
    /// JMP 2F [2 datoshi]
    /// LDLOC 07 [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC5 [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// LDLOC 07 [2 datoshi]
    /// SIZE [4 datoshi]
    /// JMPLT CF [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC6 [2 datoshi]
    /// SIZE [4 datoshi]
    /// JMPLT BE [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("multiDimensionalForeachSum")]
    public abstract BigInteger? MultiDimensionalForeachSum();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: GBcSwBYVEsASwEA=
    /// PUSH8 [1 datoshi]
    /// PUSH7 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH6 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("multiDimensionalImplicitInitializer")]
    public abstract IList<object>? MultiDimensionalImplicitInitializer();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FBMSwBIREsASwEA=
    /// PUSH4 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("multiDimensionalInitializer")]
    public abstract IList<object>? MultiDimensionalInitializer();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUAEnETcmnDcxB0IgxqxCFsa1PQbJx0bGkw9GtwACpA
    /// INITSLOT 0500 [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// NEWARRAY [512 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC4 [2 datoshi]
    /// JMP 0C [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// NEWARRAY_T 21 'Integer' [512 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC4 [2 datoshi]
    /// INC [4 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLT F4 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHINT8 2A [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("multiDimensionalNewOnly")]
    public abstract BigInteger? MultiDimensionalNewOnly();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WkA=
    /// LDSFLD2 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayOwner")]
    public abstract byte[]? TestByteArrayOwner();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NOBA
    /// CALL E0 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testByteArrayOwnerCall")]
    public abstract byte[]? TestByteArrayOwnerCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwcAGBcWFRQTEhEYwHAMBXRocmVlDAN0d28MA29uZRPAcRkYFxPAFhUUE8ATEhETwBPAchMSERPAcxYVFBPAdBkYFxPAdWtsbVMTwHZoaWpuVBS/QA==
    /// INITSLOT 0700 [64 datoshi]
    /// PUSH8 [1 datoshi]
    /// PUSH7 [1 datoshi]
    /// PUSH6 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH8 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 7468726565 'three' [8 datoshi]
    /// PUSHDATA1 74776F 'two' [8 datoshi]
    /// PUSHDATA1 6F6E65 'one' [8 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH9 [1 datoshi]
    /// PUSH8 [1 datoshi]
    /// PUSH7 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH6 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSH6 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC4 [2 datoshi]
    /// PUSH9 [1 datoshi]
    /// PUSH8 [1 datoshi]
    /// PUSH7 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC5 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC6 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC6 [2 datoshi]
    /// REVERSE4 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCollectionexpressions")]
    public abstract IList<object>? TestCollectionexpressions();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAE8QhcGgQzhCXJgQIQAlA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH3 [1 datoshi]
    /// NEWARRAY_T 21 'Integer' [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDefaultArray")]
    public abstract bool? TestDefaultArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEAsLE79waEA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDefaultState")]
    public abstract object? TestDefaultState();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeMQhcBBxIixoaWnQaUqcSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeLUk02hA
    /// INITSLOT 0201 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// NEWARRAY_T 21 'Integer' [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// JMP 2C [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// DUP [2 datoshi]
    /// INC [4 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STLOC1 [2 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LT [8 datoshi]
    /// JMPIF D3 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDynamicArrayInit")]
    public abstract IList<object>? TestDynamicArrayInit(BigInteger? length);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeMqIQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDynamicArrayStringInit")]
    public abstract byte[]? TestDynamicArrayStringInit(string? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAEDcAAHBoEcBxaUrYJAQQznJqStgkBxTONwEAQc/nR5ZA
    /// INITSLOT 0300 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 07 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// SYSCALL CFE74796 'System.Runtime.Log' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testElementBinding")]
    public abstract void TestElementBinding();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAwnBoQA==
    /// INITSLOT 0100 [64 datoshi]
    /// NEWARRAY0 [16 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEmptyArray")]
    public abstract IList<object>? TestEmptyArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAE8QhcGgQENBoERHQaBIS0GhA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH3 [1 datoshi]
    /// NEWARRAY_T 21 'Integer' [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntArray")]
    public abstract IList<object>? TestIntArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAExIRE8BwaBEU0GgSFdBoQA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntArrayInit")]
    public abstract IList<object>? TestIntArrayInit();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAExIRE8BwaBEU0GgSFdBoQA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntArrayInit2")]
    public abstract IList<object>? TestIntArrayInit2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAExIRE8BwaBEU0GgSFdBoQA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIntArrayInit3")]
    public abstract IList<object>? TestIntArrayInit3();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQAFBMSERTAcBgXFhUUwHEREhMRFMByEhMUFRTAc2hpamtUFMBA
    /// INITSLOT 0400 [64 datoshi]
    /// PUSH4 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH8 [1 datoshi]
    /// PUSH7 [1 datoshi]
    /// PUSH6 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// PUSH5 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// REVERSE4 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testJaggedArray")]
    public abstract IList<object>? TestJaggedArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQADAQBAgME2zBwDAQFBgcI2zBxDAQBAwIB2zByDAQFBAMC2zBzaGlqa1QUwEA=
    /// INITSLOT 0400 [64 datoshi]
    /// PUSHDATA1 01020304 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 05060708 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSHDATA1 01030201 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHDATA1 05040302 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// REVERSE4 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testJaggedByteArray")]
    public abstract IList<object>? TestJaggedByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAEAsLE79wE8QAcWkSaNBpEs5A
    /// INITSLOT 0200 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// NEWARRAY_T 00 'Any' [512 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStructArray")]
    public abstract object? TestStructArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAEAsLE79waBHAcWkQznJqQA==
    /// INITSLOT 0300 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStructArrayInit")]
    public abstract object? TestStructArrayInit();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: W0A=
    /// LDSFLD3 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSupportedStandards")]
    public abstract IList<object>? TestSupportedStandards();

    #endregion
}
