using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Initializer : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Initializer"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[],""returntype"":""Integer"",""offset"":314,""safe"":false},{""name"":""sum1"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":326,""safe"":false},{""name"":""sum2"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":338,""safe"":false},{""name"":""anonymousObjectCreation"",""parameters"":[],""returntype"":""Void"",""offset"":350,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1nAVcBARAQEsBKNDtwaBDOaBHOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8iAkBXAAF4EBHQeBES0EBXAAF4NANAVwABQFcBAxAQEsBKNOFKEHnQShF60HBoEM5oEc6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnyICQFcBAxAQEsBKNJZweUpoEFHQRXpKaBFR0EVoEM5oEc6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnyICQFcCAcJKAGzPSgwFSGVsbG/PcGg0L0HP50eWwkoMBWdyYXBlz0oRz8JKDAVhcHBsZc9KFM8SwHFpEM40CUHP50eWQEBAwko1E////yO//v//wko1B////yMN////wko1+/7//yNM////wko17/7//yKRki7sCg=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("anonymousObjectCreation")]
    public abstract void AnonymousObjectCreation();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum")]
    public abstract BigInteger? Sum();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum1")]
    public abstract BigInteger? Sum1(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum2")]
    public abstract BigInteger? Sum2(BigInteger? a, BigInteger? b);

    #endregion

    #region Constructor for internal use only

    protected Contract_Initializer(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
