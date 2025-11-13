using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_RefLocals(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_RefLocals"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""incrementViaRefLocal"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""rebindRefLocal"",""parameters"":[{""name"":""first"",""type"":""Integer""},{""name"":""second"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":59,""safe"":false},{""name"":""rewriteArrayElement"",""parameters"":[{""name"":""index"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":120,""safe"":false},{""name"":""updateInstanceField"",""parameters"":[{""name"":""start"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":147,""safe"":false},{""name"":""updateStaticField"",""parameters"":[{""name"":""start"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":220,""safe"":false},{""name"":""updateNestedHolder"",""parameters"":[{""name"":""start"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":282,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":361,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy44LjErNzljNTY3MDY4NGM1YjUyYTQyZWJhYjMyZjgxNWU1ODRlMzUuLi4AAAAAAP1uAVcBAXgVnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KgEV4IgJAVwECeUV5Ep5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSoFFeSICQFcEARcVExPAcGhyeHMAY0pqa1HQRWh4ziICQFcDAXgRwHBocmoQzhSeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pqEFHQRWgQziICQEBXAQF4SmBFWEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2BFWCICQFcDAXgRwBHAcGgQznJqEM4TnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KahBR0EVoEM4QziICQEBWARBgQNq5RDo=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("incrementViaRefLocal")]
    public abstract BigInteger? IncrementViaRefLocal(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rebindRefLocal")]
    public abstract BigInteger? RebindRefLocal(BigInteger? first, BigInteger? second);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("rewriteArrayElement")]
    public abstract BigInteger? RewriteArrayElement(BigInteger? index);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("updateInstanceField")]
    public abstract BigInteger? UpdateInstanceField(BigInteger? start);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("updateNestedHolder")]
    public abstract BigInteger? UpdateNestedHolder(BigInteger? start);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("updateStaticField")]
    public abstract BigInteger? UpdateStaticField(BigInteger? start);

    #endregion
}
