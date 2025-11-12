using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_RefForeachSpan(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_RefForeachSpan"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""incrementAll"",""parameters"":[{""name"":""first"",""type"":""Integer""},{""name"":""second"",""type"":""Integer""},{""name"":""third"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""incrementArrayValues"",""parameters"":[{""name"":""first"",""type"":""Integer""},{""name"":""second"",""type"":""Integer""},{""name"":""third"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":175,""safe"":false},{""name"":""segmentDouble"",""parameters"":[{""name"":""first"",""type"":""Integer""},{""name"":""second"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":435,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy44LjErNzljNTY3MDY4NGM1YjUyYTQyZWJhYjMyZjgxNWU1ODRlMzUuLi4AAAAAAP1hAlcJA3p5eBPAcGhxaUpyynMQdCJFanZsdwdubwfOEp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSm5vB1HQRWycdGxrMLsQcmlKc8p0EHUiQ2t3B213CGpvB28Izp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnJFbZx1bWwwvWoiAkBXCQN6eXgTwHBocRByaUpzynQQdSOCAAAAa3cHbXcIakqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3IQlyYEIj5vB28IzkqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn28HbwhR0EVtnHVtbDCBaBDOaBHOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9oEs6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnyICQFcJAnl4EsBwaHFpSnLKcxB0IkVqdmx3B25vB84SoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9Kbm8HUdBFbJx0bGswuxByaUpzynQQdSJDa3cHbXcIam8HbwjOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KckVtnHVtbDC9aiICQFeTAII=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("incrementAll")]
    public abstract BigInteger? IncrementAll(BigInteger? first, BigInteger? second, BigInteger? third);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("incrementArrayValues")]
    public abstract BigInteger? IncrementArrayValues(BigInteger? first, BigInteger? second, BigInteger? third);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("segmentDouble")]
    public abstract BigInteger? SegmentDouble(BigInteger? first, BigInteger? second);

    #endregion
}
