using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_InvokeCsNef(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_InvokeCsNef"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""returnInteger"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testMain"",""parameters"":[],""returntype"":""Integer"",""offset"":9,""safe"":false},{""name"":""returnString"",""parameters"":[],""returntype"":""String"",""offset"":18,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAAKkrKFDIDOkAAFkrKFDIDOkAMC2hlbGxvIHdvcmxkQI/uNDk=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: ACpKyhQyAzpA
    /// PUSHINT8 2A [1 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
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
    /// Script: ABZKyhQyAzpA
    /// PUSHINT8 16 [1 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract BigInteger? TestMain();

    #endregion
}
