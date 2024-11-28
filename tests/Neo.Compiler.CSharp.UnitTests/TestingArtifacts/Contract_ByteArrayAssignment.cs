using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAItXAQAMAwACA9swcBFKaBBR0EUUSmgSUdBFaEBXAQAMAwACA9swcBFKaBBR0EUUSmgTUdBFaEBXAgAC////f3AMAwACA9swcWhKEC4EIghKAf8AMgYB/wCRSmkQUdBFaUBXAgAMBHRlc3RwDAMAAgPbMHFoSmkQUdBFaUBXAQESiEoQEdBKEXjQcGhAN8OuBA==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAMAAgPbMHARSmgQUdBFFEpoElHQRWhA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHDATA1 000203 [8 datoshi]
    /// 08 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : PUSH1 [1 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : LDLOC0 [2 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : ROT [2 datoshi]
    /// 10 : SETITEM [8192 datoshi]
    /// 11 : DROP [2 datoshi]
    /// 12 : PUSH4 [1 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : LDLOC0 [2 datoshi]
    /// 15 : PUSH2 [1 datoshi]
    /// 16 : ROT [2 datoshi]
    /// 17 : SETITEM [8192 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : LDLOC0 [2 datoshi]
    /// 1A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignment")]
    public abstract byte[]? TestAssignment();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEohKEBHQShF40HBoQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSH2 [1 datoshi]
    /// 04 : NEWBUFFER [256 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : PUSH0 [1 datoshi]
    /// 07 : PUSH1 [1 datoshi]
    /// 08 : SETITEM [8192 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : PUSH1 [1 datoshi]
    /// 0B : LDARG0 [2 datoshi]
    /// 0C : SETITEM [8192 datoshi]
    /// 0D : STLOC0 [2 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentDynamic")]
    public abstract byte[]? TestAssignmentDynamic(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAMAAgPbMHARSmgQUdBFFEpoE1HQRWhA
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHDATA1 000203 [8 datoshi]
    /// 08 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : PUSH1 [1 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : LDLOC0 [2 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : ROT [2 datoshi]
    /// 10 : SETITEM [8192 datoshi]
    /// 11 : DROP [2 datoshi]
    /// 12 : PUSH4 [1 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : LDLOC0 [2 datoshi]
    /// 15 : PUSH3 [1 datoshi]
    /// 16 : ROT [2 datoshi]
    /// 17 : SETITEM [8192 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : LDLOC0 [2 datoshi]
    /// 1A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentOutOfBounds")]
    public abstract byte[]? TestAssignmentOutOfBounds();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAAv///39wDAMAAgPbMHFoShAuBCIISgH/ADIGAf8AkUppEFHQRWlA
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : PUSHDATA1 000203 [8 datoshi]
    /// 0E : CONVERT 30 'Buffer' [8192 datoshi]
    /// 10 : STLOC1 [2 datoshi]
    /// 11 : LDLOC0 [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : PUSH0 [1 datoshi]
    /// 14 : JMPGE 04 [2 datoshi]
    /// 16 : JMP 08 [2 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : PUSHINT16 FF00 [1 datoshi]
    /// 1C : JMPLE 06 [2 datoshi]
    /// 1E : PUSHINT16 FF00 [1 datoshi]
    /// 21 : AND [8 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : LDLOC1 [2 datoshi]
    /// 24 : PUSH0 [1 datoshi]
    /// 25 : ROT [2 datoshi]
    /// 26 : SETITEM [8192 datoshi]
    /// 27 : DROP [2 datoshi]
    /// 28 : LDLOC1 [2 datoshi]
    /// 29 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentOverflow")]
    public abstract byte[]? TestAssignmentOverflow();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADAR0ZXN0cAwDAAID2zBxaEppEFHQRWlA
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHDATA1 74657374 'test' [8 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : PUSHDATA1 000203 [8 datoshi]
    /// 0F : CONVERT 30 'Buffer' [8192 datoshi]
    /// 11 : STLOC1 [2 datoshi]
    /// 12 : LDLOC0 [2 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : LDLOC1 [2 datoshi]
    /// 15 : PUSH0 [1 datoshi]
    /// 16 : ROT [2 datoshi]
    /// 17 : SETITEM [8192 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : LDLOC1 [2 datoshi]
    /// 1A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAssignmentWrongCasting")]
    public abstract byte[]? TestAssignmentWrongCasting();

    #endregion
}
