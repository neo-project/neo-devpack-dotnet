using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Out(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Out"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testOutVar"",""parameters"":[],""returntype"":""Integer"",""offset"":31,""safe"":false},{""name"":""testExistingVar"",""parameters"":[],""returntype"":""Integer"",""offset"":38,""safe"":false},{""name"":""testMultipleOut"",""parameters"":[],""returntype"":""String"",""offset"":48,""safe"":false},{""name"":""testOutDiscard"",""parameters"":[],""returntype"":""Void"",""offset"":97,""safe"":false},{""name"":""testOutInLoop"",""parameters"":[],""returntype"":""Integer"",""offset"":109,""safe"":false},{""name"":""testOutConditional"",""parameters"":[{""name"":""flag"",""type"":""Boolean""}],""returntype"":""String"",""offset"":233,""safe"":false},{""name"":""testOutSwitch"",""parameters"":[{""name"":""option"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":268,""safe"":false},{""name"":""testNestedOut"",""parameters"":[],""returntype"":""Array"",""offset"":313,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":393,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/YwBVwABACpKYEVAVwADGkphRQwFSGVsbG9KYkUISmNFQBBgWDTeWEBXAQBoYFg01FhAEGMLYhBhW1pZNNBZNwAADAIsIItaiwwCLCCLWyYKDARUcnVlIgkMBUZhbHNli9soQBBjC2IQYVtaWTSfQFcCABBwEHEibhBgWDSHaFieSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRWlKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xRWkVtSSRaEBXAAF4Jg8QYFg1Dv///1g3AABAEGMLYhBhW1pZNQT///9aQFcBAXhwaBGXJAloEpckDiIcEGBYNeD+//9YQBBjC2IQYVtaWTXZ/v//WUAPQFcBABBkXDQLcMVKaM9KXM9AVwABXGRYNbD+//9YZFwSoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVgVAa5jG6w=="));

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
    [DisplayName("testMultipleOut")]
    public abstract string? TestMultipleOut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNestedOut")]
    public abstract IList<object>? TestNestedOut();

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

}
