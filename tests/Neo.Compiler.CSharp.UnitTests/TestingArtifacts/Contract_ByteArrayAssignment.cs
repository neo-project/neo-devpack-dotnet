using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_ByteArrayAssignment(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ByteArrayAssignment"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAssignment"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testAssignmentOutOfBounds"",""parameters"":[],""returntype"":""ByteArray"",""offset"":21,""safe"":false},{""name"":""testAssignmentOverflow"",""parameters"":[],""returntype"":""ByteArray"",""offset"":42,""safe"":false},{""name"":""testAssignmentWrongCasting"",""parameters"":[],""returntype"":""ByteArray"",""offset"":69,""safe"":false},{""name"":""testAssignmentDynamic"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":93,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAG1XAQAMAwACAxONcGgQEdBoEhTQaEBXAQAMAwACAxONcGgQEdBoExTQaEBXAgAC////f3AMAwACAxONcWkQaAH/AJHQaUBXAgAMBHRlc3RwDAMAAgMTjXFpEGjQaUBXAQESiEoQEdBKEXjQcGhA/YkxNw==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAMAAgMTjXBoEBHQaBIU0GhA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHDATA1 000203 [8 datoshi]
    /// PUSH3 [1 datoshi]
    /// LEFT [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignment")]
    public abstract byte[]? TestAssignment();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEohKEBHQShF40HBoQA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentDynamic")]
    public abstract byte[]? TestAssignmentDynamic(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAMAAgMTjXBoEBHQaBMU0GhA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHDATA1 000203 [8 datoshi]
    /// PUSH3 [1 datoshi]
    /// LEFT [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH4 [1 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentOutOfBounds")]
    public abstract byte[]? TestAssignmentOutOfBounds();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAAv///39wDAMAAgMTjXFpEGgB/wCR0GlA
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 000203 [8 datoshi]
    /// PUSH3 [1 datoshi]
    /// LEFT [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentOverflow")]
    public abstract byte[]? TestAssignmentOverflow();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAR0ZXN0cAwDAAIDE41xaRBo0GlA
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 74657374 'test' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 000203 [8 datoshi]
    /// PUSH3 [1 datoshi]
    /// LEFT [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentWrongCasting")]
    public abstract byte[]? TestAssignmentWrongCasting();

    #endregion
}
