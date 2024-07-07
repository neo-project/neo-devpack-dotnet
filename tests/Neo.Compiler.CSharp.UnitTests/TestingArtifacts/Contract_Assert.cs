using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Assert : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Assert"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testAssertFalse"",""parameters"":[],""returntype"":""Integer"",""offset"":153,""safe"":false},{""name"":""testAssertInFunction"",""parameters"":[],""returntype"":""Integer"",""offset"":165,""safe"":false},{""name"":""testAssertInTry"",""parameters"":[],""returntype"":""Integer"",""offset"":177,""safe"":false},{""name"":""testAssertInCatch"",""parameters"":[],""returntype"":""Integer"",""offset"":189,""safe"":false},{""name"":""testAssertInFinally"",""parameters"":[],""returntype"":""Integer"",""offset"":201,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPAADVVwEBEHAIORFKcEUJOQBkSnBFaCICQFcAAXg0A0BXAAFAVwEBEHB4NNlKcEURSnBFaCICQFcCARBwOwsSeDTDSnBFPQBxEUpwRT0AEkpwRT9XAgEQcDsTHBFKcEUMCWV4Y2VwdGlvbjpxeDSWSnBFPQASSnBFP1cCARBwOwkQEUpwRT0TcRJKcEU9DHg1dP///0pwRT9oIgJAwko1e////yNg////wko1b////yN1////wko1Y////yN8////wko1V////yOM////wko1S////yOm////GkTznA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertFalse")]
    public abstract BigInteger? TestAssertFalse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertInCatch")]
    public abstract BigInteger? TestAssertInCatch();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertInFinally")]
    public abstract BigInteger? TestAssertInFinally();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertInFunction")]
    public abstract BigInteger? TestAssertInFunction();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAssertInTry")]
    public abstract BigInteger? TestAssertInTry();

    #endregion

    #region Constructor for internal use only

    protected Contract_Assert(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
