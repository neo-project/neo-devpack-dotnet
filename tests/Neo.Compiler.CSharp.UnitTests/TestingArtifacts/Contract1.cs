using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract1 : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract1"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_001"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testVoid"",""parameters"":[],""returntype"":""Void"",""offset"":16,""safe"":false},{""name"":""testArgs1"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":29,""safe"":false},{""name"":""testArgs2"",""parameters"":[{""name"":""a"",""type"":""ByteArray""}],""returntype"":""Any"",""offset"":52,""safe"":false},{""name"":""testArgs3"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Void"",""offset"":59,""safe"":false},{""name"":""testArgs4"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":115,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAADeVwEADAQBAgME2zBwaCICQFcBAAwEAQIDBNswcEBXAQEMBAECAwPbMHB4SmgTUdBFaCICQFcAAXgiAkBXAAJ4Ep5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSoBFQFcAAngSnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KgEV4eZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfIgJAeudHvQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testArgs1")]
    public abstract byte[]? TestArgs1(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testArgs2")]
    public abstract object? TestArgs2(byte[]? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testArgs3")]
    public abstract void TestArgs3(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testArgs4")]
    public abstract BigInteger? TestArgs4(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testVoid")]
    public abstract void TestVoid();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("unitTest_001")]
    public abstract byte[]? UnitTest_001();

    #endregion

    #region Constructor for internal use only

    protected Contract1(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
