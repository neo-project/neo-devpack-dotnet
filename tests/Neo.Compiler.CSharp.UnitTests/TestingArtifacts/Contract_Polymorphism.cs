using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Polymorphism(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Polymorphism"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":293,""safe"":false},{""name"":""test"",""parameters"":[],""returntype"":""String"",""offset"":308,""safe"":false},{""name"":""test2"",""parameters"":[],""returntype"":""String"",""offset"":320,""safe"":false},{""name"":""abstractTest"",""parameters"":[],""returntype"":""String"",""offset"":332,""safe"":false},{""name"":""mul"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":344,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":262,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1oAVcAA3l6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwABeDQDQFcAAUBXAAEMCXRlc3RGaW5hbEBXAAF4NANAVwABeDQDQFcAAXg01EBXAAF4NBN4NBqL2ygMBS50ZXN0i9soQFcAAQwEdGVzdEBXAAF4NA4MBi50ZXN0MovbKEBXAAEMBWJhc2UyQFcAAXg0GQwRb3ZlcnJpZGVuQWJzdHJhY3SL2yhAVwABDAxhYnN0cmFjdFRlc3RAVwADeXqgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BWAgsKjv///woAAAAAE8BgCwqA////CgAAAAATwGFAwkpYz0o1C////yPR/v//wko1Gf///yMF////wko1Df///yMd////wko1Af///yNP////wkpZz0o1AP///yNv////QEdvZl0="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("abstractTest")]
    public abstract string? AbstractTest();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("mul")]
    public abstract BigInteger? Mul(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test")]
    public abstract string? Test();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test2")]
    public abstract string? Test2();

    #endregion
}
