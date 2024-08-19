using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_CheckedUnchecked : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_CheckedUnchecked"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""addChecked"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""addUnchecked"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":25,""safe"":false},{""name"":""castChecked"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":126,""safe"":false},{""name"":""castUnchecked"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":173,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3+AFcAAnh5nkoCAAAAgC4DOkoC////fzIDOkBXAAJ4eZ5KAgAAAIAuBCIKSgL///9/Mk4F/////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACRSgL///9/MiQFAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACfQFcAAXhKEC4DOkoF/////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAyAzpAVwABeEoQLgQiJkoF/////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAyJAX/////AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJFAIbzuOA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("addChecked")]
    public abstract BigInteger? AddChecked(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("addUnchecked")]
    public abstract BigInteger? AddUnchecked(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("castChecked")]
    public abstract BigInteger? CastChecked(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("castUnchecked")]
    public abstract BigInteger? CastUnchecked(BigInteger? a);

    #endregion

    #region Constructor for internal use only

    protected Contract_CheckedUnchecked(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
