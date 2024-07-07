using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Attribute : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Attribute"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""reentrantTest"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":336,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":307,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""base64Decode""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrAxiYXNlNjREZWNvZGUBAAEPAAD9WQFY2CYrCxHASlnPDBxBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUE9EU00CmBYNCoIIgJAVwACeDQceTcAANsw2yhK2CQJSsoAFCgDOkp4EFHQRUBXAAFAVwABeBDOQfgn7IyqJg4MCWV4Y2VwdGlvbjpAVwACWtgmIAsLEsBKW88MDXJlZW50cmFudFRlc3QB/wASTTQeYlo0QHkQlyYEIgx5AHuXJgYQeDTJWjVnAAAAQFcAA3g1nf///3pKeBFR0EVBm/ZnznkRiE4QUdBQEsBKeBBR0EVAVwEBeBHOeBDOwUVTi1BBkl3oMXBoC5cMD0FscmVhZHkgZW50ZXJlZOEReBHOeBDOwUVTi1BB5j8YhEBXAAF4Ec54EM7BRVOLUEEvWMXtQFcAAXg0A0BXAAFAVgQKAAAAAAoe////EsBhCtL///8Kkv///xLAY0DCSjTWIx////+rWi90"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("reentrantTest")]
    public abstract void ReentrantTest(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test")]
    public abstract bool? Test();

    #endregion

    #region Constructor for internal use only

    protected Contract_Attribute(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
