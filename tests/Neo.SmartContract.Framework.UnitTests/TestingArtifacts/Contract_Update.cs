using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Update(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Update"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""imUpdated"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Version"":""0.0.1"",""Description"":""Neo Framework Test Contract"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/tests/Neo.SmartContract.Framework.TestContracts"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAYMA1lFU0DmhhMr").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DANZRVNA
    /// PUSHDATA1 594553 'YES' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("imUpdated")]
    public abstract string? ImUpdated();

    #endregion
}
