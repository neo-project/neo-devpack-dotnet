using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_InvokeCsNef(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_InvokeCsNef"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""returnInteger"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testMain"",""parameters"":[],""returntype"":""Integer"",""offset"":3,""safe"":false},{""name"":""returnString"",""parameters"":[],""returntype"":""String"",""offset"":6,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABQAKkAAFkAMC2hlbGxvIHdvcmxkQM2Xsro="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: ACpA
    /// PUSHINT8 2A [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("returnInteger")]
    public abstract BigInteger? ReturnInteger();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAtoZWxsbyB3b3JsZEA=
    /// PUSHDATA1 68656C6C6F20776F726C64 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("returnString")]
    public abstract string? ReturnString();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: ABZA
    /// PUSHINT8 16 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract BigInteger? TestMain();

    #endregion
}
