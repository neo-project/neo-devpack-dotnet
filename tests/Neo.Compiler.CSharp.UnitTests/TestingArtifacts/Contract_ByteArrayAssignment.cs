using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_ByteArrayAssignment : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ByteArrayAssignment"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAssignment"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testAssignmentOutOfBounds"",""parameters"":[],""returntype"":""ByteArray"",""offset"":29,""safe"":false},{""name"":""testAssignmentOverflow"",""parameters"":[],""returntype"":""ByteArray"",""offset"":58,""safe"":false},{""name"":""testAssignmentWrongCasting"",""parameters"":[],""returntype"":""ByteArray"",""offset"":102,""safe"":false},{""name"":""testAssignmentDynamic"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":131,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAACVVwEADAMAAgPbMHARSmgQUdBFFEpoElHQRWgiAkBXAQAMAwACA9swcBFKaBBR0EUUSmgTUdBFaCICQFcCAAL///9/cAwDAAID2zBxaEoQLgQiCEoB/wAyBgH/AJFKaRBR0EVpIgJAVwIADAR0ZXN0cAwDAAID2zBxaEppEFHQRWkiAkBXAQESiEoQEdBKEXjQcGgiAkBAeptL"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
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

    #region Constructor for internal use only

    protected Contract_ByteArrayAssignment(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
