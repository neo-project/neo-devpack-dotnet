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
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 000203 [8 datoshi]
    /// 08 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 0A : OpCode.STLOC0 [2 datoshi]
    /// 0B : OpCode.PUSH1 [1 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.LDLOC0 [2 datoshi]
    /// 0E : OpCode.PUSH0 [1 datoshi]
    /// 0F : OpCode.ROT [2 datoshi]
    /// 10 : OpCode.SETITEM [8192 datoshi]
    /// 11 : OpCode.DROP [2 datoshi]
    /// 12 : OpCode.PUSH4 [1 datoshi]
    /// 13 : OpCode.DUP [2 datoshi]
    /// 14 : OpCode.LDLOC0 [2 datoshi]
    /// 15 : OpCode.PUSH2 [1 datoshi]
    /// 16 : OpCode.ROT [2 datoshi]
    /// 17 : OpCode.SETITEM [8192 datoshi]
    /// 18 : OpCode.DROP [2 datoshi]
    /// 19 : OpCode.LDLOC0 [2 datoshi]
    /// 1A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignment")]
    public abstract byte[]? TestAssignment();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEohKEBHQShF40HBoQA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.PUSH2 [1 datoshi]
    /// 04 : OpCode.NEWBUFFER [256 datoshi]
    /// 05 : OpCode.DUP [2 datoshi]
    /// 06 : OpCode.PUSH0 [1 datoshi]
    /// 07 : OpCode.PUSH1 [1 datoshi]
    /// 08 : OpCode.SETITEM [8192 datoshi]
    /// 09 : OpCode.DUP [2 datoshi]
    /// 0A : OpCode.PUSH1 [1 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.SETITEM [8192 datoshi]
    /// 0D : OpCode.STLOC0 [2 datoshi]
    /// 0E : OpCode.LDLOC0 [2 datoshi]
    /// 0F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentDynamic")]
    public abstract byte[]? TestAssignmentDynamic(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAMAAgPbMHARSmgQUdBFFEpoE1HQRWhA
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 000203 [8 datoshi]
    /// 08 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 0A : OpCode.STLOC0 [2 datoshi]
    /// 0B : OpCode.PUSH1 [1 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.LDLOC0 [2 datoshi]
    /// 0E : OpCode.PUSH0 [1 datoshi]
    /// 0F : OpCode.ROT [2 datoshi]
    /// 10 : OpCode.SETITEM [8192 datoshi]
    /// 11 : OpCode.DROP [2 datoshi]
    /// 12 : OpCode.PUSH4 [1 datoshi]
    /// 13 : OpCode.DUP [2 datoshi]
    /// 14 : OpCode.LDLOC0 [2 datoshi]
    /// 15 : OpCode.PUSH3 [1 datoshi]
    /// 16 : OpCode.ROT [2 datoshi]
    /// 17 : OpCode.SETITEM [8192 datoshi]
    /// 18 : OpCode.DROP [2 datoshi]
    /// 19 : OpCode.LDLOC0 [2 datoshi]
    /// 1A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentOutOfBounds")]
    public abstract byte[]? TestAssignmentOutOfBounds();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAAv///39wDAMAAgPbMHFoShAuBCIISgH/ADIGAf8AkUppEFHQRWlA
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSHINT32 FFFFFF7F [1 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.PUSHDATA1 000203 [8 datoshi]
    /// 0E : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 10 : OpCode.STLOC1 [2 datoshi]
    /// 11 : OpCode.LDLOC0 [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.PUSH0 [1 datoshi]
    /// 14 : OpCode.JMPGE 04 [2 datoshi]
    /// 16 : OpCode.JMP 08 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.PUSHINT16 FF00 [1 datoshi]
    /// 1C : OpCode.JMPLE 06 [2 datoshi]
    /// 1E : OpCode.PUSHINT16 FF00 [1 datoshi]
    /// 21 : OpCode.AND [8 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.LDLOC1 [2 datoshi]
    /// 24 : OpCode.PUSH0 [1 datoshi]
    /// 25 : OpCode.ROT [2 datoshi]
    /// 26 : OpCode.SETITEM [8192 datoshi]
    /// 27 : OpCode.DROP [2 datoshi]
    /// 28 : OpCode.LDLOC1 [2 datoshi]
    /// 29 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentOverflow")]
    public abstract byte[]? TestAssignmentOverflow();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAR0ZXN0cAwDAAID2zBxaEppEFHQRWlA
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 74657374 'test' [8 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.PUSHDATA1 000203 [8 datoshi]
    /// 0F : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 11 : OpCode.STLOC1 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.DUP [2 datoshi]
    /// 14 : OpCode.LDLOC1 [2 datoshi]
    /// 15 : OpCode.PUSH0 [1 datoshi]
    /// 16 : OpCode.ROT [2 datoshi]
    /// 17 : OpCode.SETITEM [8192 datoshi]
    /// 18 : OpCode.DROP [2 datoshi]
    /// 19 : OpCode.LDLOC1 [2 datoshi]
    /// 1A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentWrongCasting")]
    public abstract byte[]? TestAssignmentWrongCasting();

    #endregion
}
