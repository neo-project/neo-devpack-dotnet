using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ByteArrayAssignment(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ByteArrayAssignment"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAssignment"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testAssignmentOutOfBounds"",""parameters"":[],""returntype"":""ByteArray"",""offset"":27,""safe"":false},{""name"":""testAssignmentOverflow"",""parameters"":[],""returntype"":""ByteArray"",""offset"":54,""safe"":false},{""name"":""testAssignmentWrongCasting"",""parameters"":[],""returntype"":""ByteArray"",""offset"":96,""safe"":false},{""name"":""testAssignmentDynamic"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":123,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x5d46269ff23ec37de131e4791d5e5c964b140704"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIEBxRLllxeHXnkMeF9wz7ynyZGXQl0ZXN0QXJnczEBAAEPBAcUS5ZcXh155DHhfcM+8p8mRl0IdGVzdFZvaWQAAAAPAACLVwEADAMAAgPbMHARSmgQUdBFFEpoElHQRWhAVwEADAMAAgPbMHARSmgQUdBFFEpoE1HQRWhAVwIAAv///39wDAMAAgPbMHFoShAuBCIISgH/ADIGAf8AkUppEFHQRWlAVwIADAR0ZXN0cAwDAAID2zBxaEppEFHQRWlAVwEBEohKEBHQShF40HBoQDxtkps="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAACA9swcBFKaBBR0EUUSmgSUdBFaEA=
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHDATA1 000203
    /// 08 : OpCode.CONVERT 30
    /// 0A : OpCode.STLOC0
    /// 0B : OpCode.PUSH1
    /// 0C : OpCode.DUP
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.ROT
    /// 10 : OpCode.SETITEM
    /// 11 : OpCode.DROP
    /// 12 : OpCode.PUSH4
    /// 13 : OpCode.DUP
    /// 14 : OpCode.LDLOC0
    /// 15 : OpCode.PUSH2
    /// 16 : OpCode.ROT
    /// 17 : OpCode.SETITEM
    /// 18 : OpCode.DROP
    /// 19 : OpCode.LDLOC0
    /// 1A : OpCode.RET
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
