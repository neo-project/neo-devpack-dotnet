using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_String : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_String"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""testEqual"",""parameters"":[],""returntype"":""Void"",""offset"":82,""safe"":false},{""name"":""testSubstring"",""parameters"":[],""returntype"":""Void"",""offset"":127,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""currentHash"",""getBlock""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAUKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPvvIEMUA2KnfBUJnH5kwS9wC2ZdoIZ2V0QmxvY2sBAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoLY3VycmVudEhhc2gAAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwEaXRvYQEAAQ8AAKNXAwAMBE1hcmtwDABxNwMANwIAFM5yDAdIZWxsbywgaIsMASCLaYsMFyEgQ3VycmVudCB0aW1lc3RhbXAgaXMgi2o3BACLDAEui9soQc/nR5ZAVwIADAVoZWxsb3AMBWhlbGxvcWhplyQLDAVGYWxzZSIIDARUcnVlQc/nR5ZAVwEADAgwMTIzNDU2N3BoEUvKS5+MQc/nR5ZoERSMQc/nR5ZAVNlpIg=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEqual")]
    public abstract void TestEqual();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testMain")]
    public abstract void TestMain();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSubstring")]
    public abstract void TestSubstring();

    #endregion

    #region Constructor for internal use only

    protected Contract_String(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
