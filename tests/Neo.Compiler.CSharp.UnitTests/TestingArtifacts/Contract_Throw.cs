using Neo.Cryptography.ECC;
using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADdXAQF4yhG4Jgd4EM4iKQwkUGxlYXNlIHN1cHBseSBhdCBsZWFzdCBvbmUgYXJndW1lbnQuOnBA2o6Ljg==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeMoRuCYHeBDOIikMJFBsZWFzZSBzdXBwbHkgYXQgbGVhc3Qgb25lIGFyZ3VtZW50LjpwQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SIZE [4 datoshi]
    /// 05 : PUSH1 [1 datoshi]
    /// 06 : GE [8 datoshi]
    /// 07 : JMPIFNOT 07 [2 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : PUSH0 [1 datoshi]
    /// 0B : PICKITEM [64 datoshi]
    /// 0C : JMP 29 [2 datoshi]
    /// 0E : PUSHDATA1 506C6561736520737570706C79206174206C65617374206F6E6520617267756D656E742E [8 datoshi]
    /// 34 : THROW [512 datoshi]
    /// 35 : STLOC0 [2 datoshi]
    /// 36 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract void TestMain(IList<object>? args);

    #endregion
}
