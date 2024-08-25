using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Array : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Array"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getTreeByteLengthPrefix"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""getTreeByteLengthPrefix2"",""parameters"":[],""returntype"":""ByteArray"",""offset"":3,""safe"":false},{""name"":""testJaggedArray"",""parameters"":[],""returntype"":""Array"",""offset"":6,""safe"":false},{""name"":""testJaggedByteArray"",""parameters"":[],""returntype"":""Array"",""offset"":44,""safe"":false},{""name"":""testIntArray"",""parameters"":[],""returntype"":""Array"",""offset"":90,""safe"":false},{""name"":""testDefaultArray"",""parameters"":[],""returntype"":""Boolean"",""offset"":120,""safe"":false},{""name"":""testIntArrayInit"",""parameters"":[],""returntype"":""Array"",""offset"":139,""safe"":false},{""name"":""testIntArrayInit2"",""parameters"":[],""returntype"":""Array"",""offset"":164,""safe"":false},{""name"":""testIntArrayInit3"",""parameters"":[],""returntype"":""Array"",""offset"":189,""safe"":false},{""name"":""testDynamicArrayInit"",""parameters"":[{""name"":""length"",""type"":""Integer""}],""returntype"":""Array"",""offset"":214,""safe"":false},{""name"":""testDynamicArrayStringInit"",""parameters"":[{""name"":""input"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":290,""safe"":false},{""name"":""testStructArray"",""parameters"":[],""returntype"":""Any"",""offset"":297,""safe"":false},{""name"":""testDefaultState"",""parameters"":[],""returntype"":""Any"",""offset"":345,""safe"":false},{""name"":""testEmptyArray"",""parameters"":[],""returntype"":""Array"",""offset"":364,""safe"":false},{""name"":""testStructArrayInit"",""parameters"":[],""returntype"":""Any"",""offset"":371,""safe"":false},{""name"":""testByteArrayOwner"",""parameters"":[],""returntype"":""ByteArray"",""offset"":428,""safe"":false},{""name"":""testByteArrayOwnerCall"",""parameters"":[],""returntype"":""ByteArray"",""offset"":431,""safe"":false},{""name"":""testSupportedStandards"",""parameters"":[],""returntype"":""Array"",""offset"":434,""safe"":false},{""name"":""testElementBinding"",""parameters"":[],""returntype"":""Void"",""offset"":437,""safe"":false},{""name"":""testCollectionexpressions"",""parameters"":[],""returntype"":""Array"",""offset"":473,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":563,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""getBlock""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAK+8gQxQDYqd8FQmcfmTBL3ALZl2ghnZXRCbG9jawEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/W8CXwBAXwFAVwQAFBMSERTAcBgXFhUUwHEREhMRFMByEhMUFRTAc2tqaWgUwEBXBAAMBAECAwTbMHAMBAUGBwjbMHEMBAEDAgHbMHIMBAUEAwLbMHNramloFMBAVwEAE8QhcBBKaBBR0EURSmgRUdBFEkpoElHQRWhAVwEAE8QhcGgQzhCXJgUIIgMJQFcBABMSERPAcBRKaBFR0EUVSmgSUdBFaEBXAQATEhETwHAUSmgRUdBFFUpoElHQRWhAVwEAExIRE8BwFEpoEVHQRRVKaBJR0EVoQFcCAXjEIXAQcSI8aUpoaVHQRWlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWl4tSTDaEBXAAF4yohAVwIAxUoLz0oLz0oQz0o0EnATxABxaEppElHQRWkSzkBXAAF4EAvQeBEL0HgSENBAVwEAxUoLz0oLz0oQz0o04nBoQFcBAMJwaEBXAwDFSgvPSgvPShDPSjTIcGgRwHFpEM5yakBXAQAMFPZkQ0mNOHjTK5lOThKDxpNEIdr+2zBwaEBfAkA030BfA0BXAwAQNwAAcGgRwHFpStgkBBDOcmpK2CQHFM43AQBBz+dHlkBXBwAYFxYVFBMSERjAcAwFdGhyZWUMA3R3bwwDb25lE8BxGRgXE8AWFRQTwBMSERPAE8ByExIRE8BzFhUUE8B0GRgXE8B1bWxrE8B2xUpoz0ppz0pqz0puz0BWBAwCAQPbMGAMAgED2zBhDBT2ZENJjTh40yuZTk4Sg8aTRCHa/tswYgwGTkVQLTEwDAVORVAtNRLAY0AgGHjb"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTreeByteLengthPrefix")]
    public abstract byte[]? GetTreeByteLengthPrefix();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getTreeByteLengthPrefix2")]
    public abstract byte[]? GetTreeByteLengthPrefix2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testByteArrayOwner")]
    public abstract byte[]? TestByteArrayOwner();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testByteArrayOwnerCall")]
    public abstract byte[]? TestByteArrayOwnerCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCollectionexpressions")]
    public abstract IList<object>? TestCollectionexpressions();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDefaultArray")]
    public abstract bool? TestDefaultArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDefaultState")]
    public abstract object? TestDefaultState();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDynamicArrayInit")]
    public abstract IList<object>? TestDynamicArrayInit(BigInteger? length);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDynamicArrayStringInit")]
    public abstract byte[]? TestDynamicArrayStringInit(string? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testElementBinding")]
    public abstract void TestElementBinding();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEmptyArray")]
    public abstract IList<object>? TestEmptyArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIntArray")]
    public abstract IList<object>? TestIntArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIntArrayInit")]
    public abstract IList<object>? TestIntArrayInit();

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

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testJaggedByteArray")]
    public abstract IList<object>? TestJaggedByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStructArray")]
    public abstract object? TestStructArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testStructArrayInit")]
    public abstract object? TestStructArrayInit();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSupportedStandards")]
    public abstract IList<object>? TestSupportedStandards();

    #endregion

    #region Constructor for internal use only

    protected Contract_Array(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
