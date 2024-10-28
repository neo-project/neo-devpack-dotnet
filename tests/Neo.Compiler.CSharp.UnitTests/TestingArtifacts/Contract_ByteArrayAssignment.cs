using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ByteArrayAssignment(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ByteArrayAssignment"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAssignment"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testAssignmentOutOfBounds"",""parameters"":[],""returntype"":""ByteArray"",""offset"":27,""safe"":false},{""name"":""testAssignmentOverflow"",""parameters"":[],""returntype"":""ByteArray"",""offset"":54,""safe"":false},{""name"":""testAssignmentWrongCasting"",""parameters"":[],""returntype"":""ByteArray"",""offset"":96,""safe"":false},{""name"":""testAssignmentDynamic"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":123,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xc8c95d940fdb9d83b0b298cd8cbc8a1930c78a70"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJwiscwGYq8jM2YsrCDndsPlF3JyAl0ZXN0QXJnczEBAAEPcIrHMBmKvIzNmLKwg53bD5RdycgIdGVzdFZvaWQAAAAPAACLVwEADAMAAgPbMHARSmgQUdBFFEpoElHQRWhAVwEADAMAAgPbMHARSmgQUdBFFEpoE1HQRWhAVwIAAv///39wDAMAAgPbMHFoShAuBCIISgH/ADIGAf8AkUppEFHQRWlAVwIADAR0ZXN0cAwDAAID2zBxaEppEFHQRWlAVwEBEohKEBHQShF40HBoQHOCNU8="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAACA9swcBFKaBBR0EUUSmgSUdBFaEA=
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
    [DisplayName("testAssignmentDynamic")]
    public abstract byte[]? TestAssignmentDynamic(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssignmentOutOfBounds")]
    public abstract byte[]? TestAssignmentOutOfBounds();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssignmentOverflow")]
    public abstract byte[]? TestAssignmentOverflow();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssignmentWrongCasting")]
    public abstract byte[]? TestAssignmentWrongCasting();

    #endregion
}
