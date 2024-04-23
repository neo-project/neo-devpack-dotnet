using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Array : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Array"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testJaggedArray"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""testJaggedByteArray"",""parameters"":[],""returntype"":""Array"",""offset"":40,""safe"":false},{""name"":""testIntArray"",""parameters"":[],""returntype"":""Any"",""offset"":88,""safe"":false},{""name"":""testDefaultArray"",""parameters"":[],""returntype"":""Any"",""offset"":120,""safe"":false},{""name"":""testIntArrayInit"",""parameters"":[],""returntype"":""Any"",""offset"":145,""safe"":false},{""name"":""testIntArrayInit2"",""parameters"":[],""returntype"":""Any"",""offset"":172,""safe"":false},{""name"":""testIntArrayInit3"",""parameters"":[],""returntype"":""Any"",""offset"":199,""safe"":false},{""name"":""testDynamicArrayInit"",""parameters"":[{""name"":""length"",""type"":""Integer""}],""returntype"":""Array"",""offset"":226,""safe"":false},{""name"":""testDynamicArrayStringInit"",""parameters"":[{""name"":""input"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":304,""safe"":false},{""name"":""testStructArray"",""parameters"":[],""returntype"":""Any"",""offset"":313,""safe"":false},{""name"":""testEmptyArray"",""parameters"":[],""returntype"":""Array"",""offset"":351,""safe"":false},{""name"":""testStructArrayInit"",""parameters"":[],""returntype"":""Any"",""offset"":360,""safe"":false},{""name"":""testByteArrayOwner"",""parameters"":[],""returntype"":""Any"",""offset"":433,""safe"":false},{""name"":""testByteArrayOwnerCall"",""parameters"":[],""returntype"":""Any"",""offset"":437,""safe"":false},{""name"":""testSupportedStandards"",""parameters"":[],""returntype"":""Any"",""offset"":442,""safe"":false},{""name"":""testElementBinding"",""parameters"":[],""returntype"":""Void"",""offset"":446,""safe"":false},{""name"":""testCollectionexpressions"",""parameters"":[],""returntype"":""Array"",""offset"":492,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":584,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""getBlock""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAS+8gQxQDYqd8FQmcfmTBL3ALZl2ghnZXRCbG9jawEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwoJCAcGBQQDAgEKCQgHBgUEAwIBCXRlc3RBcmdzMQEAAQ8KCQgHBgUEAwIBCgkIBwYFBAMCAQh0ZXN0Vm9pZAAAAA8AAP12AlcEABQTEhEUwHAYFxYVFMBxERITERTAchITFBUUwHNramloFMAiAkBXBAAMBAECAwTbMHAMBAUGBwjbMHEMBAEDAgHbMHIMBAUEAwLbMHNramloFMAiAkBXAQATxCFwEEpoEFHQRRFKaBFR0EUSSmgSUdBFaCICQFcBABPEIXBoEM4QlyYHEdsgIgcQ2yAiAkBXAQATEhETwHAUSmgRUdBFFUpoElHQRWgiAkBXAQATEhETwHAUSmgRUdBFFUpoElHQRWgiAkBXAQATEhETwHAUSmgRUdBFFUpoElHQRWgiAkBXAgF4xCFwEHEiPGlKaGlR0EVpSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcUVpeLUkw2giAkBXAAF4yogiAkBXAgDFSgvPSgvPShDPSjQUcBPEAHFoSmkSUdBFaRLOIgJAVwABQFcBAMJwaCICQFcEAMVKC89KC89KEM9KNOVwaBHAcRByIglpas5zayIKahG1JPYLIgJAVwEADBT2ZENJjTh40yuZTk4Sg8aTRCHa/tswcGgiAkBYIgJANNwiAkBZIgJAVwQAARAnNwAAcAERJzcAAHFpaBLAcmpK2CQEEM5za0rYJAcUzjcBAEHP50eWQFcHABgXFhUUExIRGMBwDAV0aHJlZQwDdHdvDANvbmUTwHEZGBcTwBYVFBPAExIRE8ATwHITEhETwHMWFRQTwHQZGBcTwHVtbGsTwHbFSmjPSmnPSmrPSm7PIgJAVgIMFPZkQ0mNOHjTK5lOThKDxpNEIdr+2zBgDAZORVAtMTAMBU5FUC01EsBhQD/GC3Y="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testByteArrayOwner")]
    public abstract object? TestByteArrayOwner();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testByteArrayOwnerCall")]
    public abstract object? TestByteArrayOwnerCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCollectionexpressions")]
    public abstract IList<object>? TestCollectionexpressions();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDefaultArray")]
    public abstract object? TestDefaultArray();

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
    public abstract object? TestIntArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIntArrayInit")]
    public abstract object? TestIntArrayInit();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIntArrayInit2")]
    public abstract object? TestIntArrayInit2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIntArrayInit3")]
    public abstract object? TestIntArrayInit3();

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
    public abstract object? TestSupportedStandards();

    #endregion

    #region Constructor for internal use only

    protected Contract_Array(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
