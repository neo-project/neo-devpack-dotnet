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
