using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NativeContracts : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NativeContracts"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""oracleMinimumResponseFee"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""nEOSymbol"",""parameters"":[],""returntype"":""String"",""offset"":8,""safe"":false},{""name"":""gASSymbol"",""parameters"":[],""returntype"":""String"",""offset"":14,""safe"":false},{""name"":""getOracleNodes"",""parameters"":[],""returntype"":""Array"",""offset"":20,""safe"":false},{""name"":""nEOHash"",""parameters"":[],""returntype"":""Hash160"",""offset"":28,""safe"":false},{""name"":""ledgerHash"",""parameters"":[],""returntype"":""Hash160"",""offset"":53,""safe"":false},{""name"":""ledgerCurrentHash"",""parameters"":[],""returntype"":""Hash256"",""offset"":78,""safe"":false},{""name"":""ledgerCurrentIndex"",""parameters"":[],""returntype"":""Integer"",""offset"":84,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]},{""contract"":""0x49cf4e5378ffcd4dec034fd98a174c5491e395e2"",""methods"":[""getDesignatedByRole""]},{""contract"":""0xd2a4cff31913016155e38e474a2c06d08be276cf"",""methods"":[""symbol""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""currentHash"",""currentIndex""]},{""contract"":""0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5"",""methods"":[""symbol""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAcKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAP9WPqQLwoPU0OBcSOowWz8qBzQO8Gc3ltYm9sAAABD8924ovQBixKR47jVWEBExnzz6TSBnN5bWJvbAAAAQ/ileORVEwXitlPA+xNzf94U07PSRNnZXREZXNpZ25hdGVkQnlSb2xlAgABD77yBDFANip3wVCZx+ZMEvcAtmXaC2N1cnJlbnRIYXNoAAABD77yBDFANip3wVCZx+ZMEvcAtmXaDGN1cnJlbnRJbmRleAAAAQ8AAFoCgJaYACICQDcCACICQDcDACICQBAYNwQAIgJADBT1Y+pAvCg9TQ4FxI6jBbPyoHNA7yICQAwUvvIEMUA2KnfBUJnH5kwS9wC2ZdoiAkA3BQAiAkA3BgAiAkBj48au"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("gASSymbol")]
    public abstract string? GASSymbol();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("getOracleNodes")]
    public abstract IList<object>? GetOracleNodes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("ledgerCurrentHash")]
    public abstract UInt256? LedgerCurrentHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("ledgerCurrentIndex")]
    public abstract BigInteger? LedgerCurrentIndex();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("ledgerHash")]
    public abstract UInt160? LedgerHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nEOHash")]
    public abstract UInt160? NEOHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("nEOSymbol")]
    public abstract string? NEOSymbol();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("oracleMinimumResponseFee")]
    public abstract BigInteger? OracleMinimumResponseFee();

    #endregion

    #region Constructor for internal use only

    protected Contract_NativeContracts(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
