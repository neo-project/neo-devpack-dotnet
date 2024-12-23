using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_StaticClass(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_StaticClass"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testStaticClass"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":66,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEtZNAZaNDlAVwABWBGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2BAVwABWEBWAxFgwmHCYkBDH0c0"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: WTQGWjQ5QA==
    /// LDSFLD1 [2 datoshi]
    /// CALL 06 [512 datoshi]
    /// LDSFLD2 [2 datoshi]
    /// CALL 39 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticClass")]
    public abstract BigInteger? TestStaticClass();

    #endregion
}
