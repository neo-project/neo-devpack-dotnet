using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NativeContracts(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NativeContracts"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""oracleMinimumResponseFee"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""nEOSymbol"",""parameters"":[],""returntype"":""String"",""offset"":6,""safe"":false},{""name"":""gASSymbol"",""parameters"":[],""returntype"":""String"",""offset"":10,""safe"":false},{""name"":""getOracleNodes"",""parameters"":[],""returntype"":""Array"",""offset"":14,""safe"":false},{""name"":""nEOHash"",""parameters"":[],""returntype"":""Hash160"",""offset"":20,""safe"":false},{""name"":""ledgerHash"",""parameters"":[],""returntype"":""Hash160"",""offset"":43,""safe"":false},{""name"":""ledgerCurrentHash"",""parameters"":[],""returntype"":""Hash256"",""offset"":66,""safe"":false},{""name"":""ledgerCurrentIndex"",""parameters"":[],""returntype"":""Integer"",""offset"":70,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0x49cf4e5378ffcd4dec034fd98a174c5491e395e2"",""methods"":[""getDesignatedByRole""]},{""contract"":""0xd2a4cff31913016155e38e474a2c06d08be276cf"",""methods"":[""symbol""]},{""contract"":""0xda65b600f7124ce6c79950c1772a36403104f2be"",""methods"":[""currentHash"",""currentIndex""]},{""contract"":""0xef4073a0f2b305a38ec4050e4d3d28bc40ea63f5"",""methods"":[""symbol""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAX1Y+pAvCg9TQ4FxI6jBbPyoHNA7wZzeW1ib2wAAAEPz3bii9AGLEpHjuNVYQETGfPPpNIGc3ltYm9sAAABD+KV45FUTBeK2U8D7E3N/3hTTs9JE2dldERlc2lnbmF0ZWRCeVJvbGUCAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoLY3VycmVudEhhc2gAAAEPvvIEMUA2KnfBUJnH5kwS9wC2ZdoMY3VycmVudEluZGV4AAABDwAASgKAlpgAQDcAAEA3AQBAEBg3AgBADBT1Y+pAvCg9TQ4FxI6jBbPyoHNA70AMFL7yBDFANip3wVCZx+ZMEvcAtmXaQDcDAEA3BABAAq/GWA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NwEAQA==
    /// CALLT 0100 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("gASSymbol")]
    public abstract string? GASSymbol();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EBg3AgBA
    /// PUSH0 [1 datoshi]
    /// PUSH8 [1 datoshi]
    /// CALLT 0200 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getOracleNodes")]
    public abstract IList<object>? GetOracleNodes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NwMAQA==
    /// CALLT 0300 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("ledgerCurrentHash")]
    public abstract UInt256? LedgerCurrentHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NwQAQA==
    /// CALLT 0400 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("ledgerCurrentIndex")]
    public abstract BigInteger? LedgerCurrentIndex();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DBS+8gQxQDYqd8FQmcfmTBL3ALZl2kA=
    /// PUSHDATA1 BEF2043140362A77C15099C7E64C12F700B665DA [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("ledgerHash")]
    public abstract UInt160? LedgerHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DBT1Y+pAvCg9TQ4FxI6jBbPyoHNA70A=
    /// PUSHDATA1 F563EA40BC283D4D0E05C48EA305B3F2A07340EF [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("nEOHash")]
    public abstract UInt160? NEOHash();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NwAAQA==
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("nEOSymbol")]
    public abstract string? NEOSymbol();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AoCWmABA
    /// PUSHINT32 80969800 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("oracleMinimumResponseFee")]
    public abstract BigInteger? OracleMinimumResponseFee();

    #endregion
}
