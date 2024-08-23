using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Out : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Out"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testOutVar"",""parameters"":[],""returntype"":""Integer"",""offset"":31,""safe"":false},{""name"":""testExistingVar"",""parameters"":[],""returntype"":""Integer"",""offset"":38,""safe"":false},{""name"":""testOutDiscard"",""parameters"":[],""returntype"":""Void"",""offset"":48,""safe"":false},{""name"":""testOutInLoop"",""parameters"":[],""returntype"":""Integer"",""offset"":60,""safe"":false},{""name"":""testOutConditional"",""parameters"":[{""name"":""flag"",""type"":""Boolean""}],""returntype"":""String"",""offset"":184,""safe"":false},{""name"":""testOutSwitch"",""parameters"":[{""name"":""option"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":220,""safe"":false},{""name"":""testNestedOut"",""parameters"":[],""returntype"":""Integer"",""offset"":267,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":334,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/VEBVwABACpKYEVAVwADGkphRQwFSGVsbG9KYkUISmNFQBBgWDTeWEBXAQBoYFg01FhAEGMLYhBhW1pZNNBAVwIAEHAQcSJuEGBYNLhoWJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFaUqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FFaRW1JJFoQFcAAXgmEBBgWDU/////WDcAACIREGMLYhBhW1pZNTT///9aQFcBAXhwaBGXJAloEpckDyIeEGBYNRD///9YIhQQYwtiEGFbWlk1CP///1kiAw9AEGRcNANAVwABXGRYNen+//9cEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFYFQI8KM2A="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testExistingVar")]
    public abstract BigInteger? TestExistingVar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNestedOut")]
    public abstract BigInteger? TestNestedOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testOutConditional")]
    public abstract string? TestOutConditional(bool? flag);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testOutDiscard")]
    public abstract void TestOutDiscard();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testOutInLoop")]
    public abstract BigInteger? TestOutInLoop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testOutSwitch")]
    public abstract BigInteger? TestOutSwitch(BigInteger? option);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testOutVar")]
    public abstract BigInteger? TestOutVar();

    #endregion

    #region Constructor for internal use only

    protected Contract_Out(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
