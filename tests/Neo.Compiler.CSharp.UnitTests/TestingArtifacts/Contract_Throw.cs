using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Throw(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Throw"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[{""name"":""args"",""type"":""Array""}],""returntype"":""Void"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADdXAQF4yhG4Jgd4EM4iKQwkUGxlYXNlIHN1cHBseSBhdCBsZWFzdCBvbmUgYXJndW1lbnQuOnBA2o6Ljg=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeMoRuCYHeBDOIikMJFBsZWFzZSBzdXBwbHkgYXQgbGVhc3Qgb25lIGFyZ3VtZW50LjpwQA==
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// GE [8 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// JMP 29 [2 datoshi]
    /// PUSHDATA1 506C6561736520737570706C79206174206C65617374206F6E6520617267756D656E742E [8 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain(IList<object>? args);

    #endregion
}
