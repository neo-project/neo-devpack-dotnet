using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ByteArrayAssignment(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ByteArrayAssignment"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAssignment"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testAssignmentOutOfBounds"",""parameters"":[],""returntype"":""ByteArray"",""offset"":27,""safe"":false},{""name"":""testAssignmentOverflow"",""parameters"":[],""returntype"":""ByteArray"",""offset"":54,""safe"":false},{""name"":""testAssignmentWrongCasting"",""parameters"":[],""returntype"":""ByteArray"",""offset"":96,""safe"":false},{""name"":""testAssignmentDynamic"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":123,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAItXAQAMAwACA9swcBFKaBBR0EUUSmgSUdBFaEBXAQAMAwACA9swcBFKaBBR0EUUSmgTUdBFaEBXAgAC////f3AMAwACA9swcWhKEC4EIghKAf8AMgYB/wCRSmkQUdBFaUBXAgAMBHRlc3RwDAMAAgPbMHFoSmkQUdBFaUBXAQESiEoQEdBKEXjQcGhAN8OuBA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAMAAgPbMHARSmgQUdBFFEpoElHQRWhA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHDATA1 000203 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
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
    /// Script: VwEADAMAAgPbMHARSmgQUdBFFEpoE1HQRWhA
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHDATA1 000203 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentOutOfBounds")]
    public abstract byte[]? TestAssignmentOutOfBounds();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAAv///39wDAMAAgPbMHFoShAuBCIISgH/ADIGAf8AkUppEFHQRWlA
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 000203 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 08 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// JMPLE 06 [2 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentOverflow")]
    public abstract byte[]? TestAssignmentOverflow();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAR0ZXN0cAwDAAID2zBxaEppEFHQRWlA
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 74657374 'test' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 000203 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentWrongCasting")]
    public abstract byte[]? TestAssignmentWrongCasting();

    #endregion
}
