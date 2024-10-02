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
    [DisplayName("testMain")]
    public abstract void TestMain(IList<object>? args);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : SIZE
    // 0005 : PUSH1
    // 0006 : GE
    // 0007 : JMPIFNOT
    // 0009 : LDARG0
    // 000A : PUSH0
    // 000B : PICKITEM
    // 000C : JMP
    // 000E : PUSHDATA1
    // 0034 : THROW
    // 0035 : STLOC0
    // 0036 : RET

    #endregion

}
