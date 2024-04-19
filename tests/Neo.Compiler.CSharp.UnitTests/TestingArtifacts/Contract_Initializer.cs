using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Initializer : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Initializer"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[],""returntype"":""Integer"",""offset"":314,""safe"":false},{""name"":""sum1"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":326,""safe"":false},{""name"":""sum2"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":338,""safe"":false},{""name"":""anonymousObjectCreation"",""parameters"":[],""returntype"":""Void"",""offset"":350,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAAD9agFXAQEQEBLASjQ7cGgQzmgRzp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfIgJAVwABeBAR0HgREtBAVwABeDQDQFcAAUBXAQMQEBLASjThShB50EoRetBwaBDOaBHOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8iAkBXAQMQEBLASjSWcHlKaBBR0EV6SmgRUdBFaBDOaBHOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8iAkBXAgHCSgBsz0oMBUhlbGxvz3BoNC9Bz+dHlsJKDAVncmFwZc9KEc/CSgwFYXBwbGXPShTPEsBxaRDONAlBz+dHlkBAQMJKNRP///8jv/7//8JKNQf///8jDf///8JKNfv+//8jTP///8JKNe/+//8jkf///yFXuYg="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("anonymousObjectCreation")]
    public abstract void AnonymousObjectCreation();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum")]
    public abstract BigInteger? Sum();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum1")]
    public abstract BigInteger? Sum1(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum2")]
    public abstract BigInteger? Sum2(BigInteger? a, BigInteger? b);

    #endregion

    #region Constructor for internal use only

    protected Contract_Initializer(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
