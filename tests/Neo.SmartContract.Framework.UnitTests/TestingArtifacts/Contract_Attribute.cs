using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Attribute(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Attribute"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""reentrantB"",""parameters"":[],""returntype"":""Void"",""offset"":104,""safe"":false},{""name"":""reentrantA"",""parameters"":[],""returntype"":""Void"",""offset"":255,""safe"":false},{""name"":""reentrantTest"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":303,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":472,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""base64Decode""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrAxiYXNlNjREZWNvZGUBAAEPAAD9AgJY2CYpWQsSwAwcQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBPRFNNAhgWDQhCEBXAAJ5NwAA2zDbKErYJAlKygAUKAM6SngQUdBFQFcAAXgQzkH4J+yMqiYODAlleGNlcHRpb246QFrYJhxbCwsTwAwLbm9SZWVudHJhbnQB/wASTTQNYlo0KVo1YQAAAEBXAAN6SngRUdBFQZv2Z855EYhOEFHQUBLASngQUdBFQFcBAXgRzngQzsFFU4tQQZJd6DFwaAuXDA9BbHJlYWR5IGVudGVyZWThEXgRzngQzsFFU4tQQeY/GIRAVwABeBHOeBDOwUVTi1BBL1jF7UBc2CYfWwsLE8AMC25vUmVlbnRyYW50Af8AEk01dv///2RcNY////81Qv///1w0v0BXAAFd2CYeXgsLE8AMDXJlZW50cmFudFRlc3QB/wASTTQaZV00NngQlyYEIgt4AHuXJgUQNMxdNF5AVwADekp4EVHQRUGb9mfOeRGIThBR0FASwEp4EFHQRUBXAQF4Ec54EM7BRVOLUEGSXegxcGgLlwwPQWxyZWFkeSBlbnRlcmVk4RF4Ec54EM7BRVOLUEHmPxiEQFcAAXgRzngQzsFFU4tQQS9Yxe1AVgcKAAAAAApu/v//EsBhCgT///8KxP7//xLAYwrQ////CpD///8SwGZA0ejI6A=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("reentrantA")]
    public abstract void ReentrantA();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("reentrantB")]
    public abstract void ReentrantB();

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
}
