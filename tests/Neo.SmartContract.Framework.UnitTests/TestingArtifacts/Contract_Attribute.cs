using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Attribute(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Attribute"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""reentrantB"",""parameters"":[],""returntype"":""Void"",""offset"":113,""safe"":false},{""name"":""reentrantA"",""parameters"":[],""returntype"":""Void"",""offset"":269,""safe"":false},{""name"":""reentrantTest"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":319,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":496,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""base64Decode""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrAxiYXNlNjREZWNvZGUBAAEPAAD9GgJY2CYrCxHASlnPDBxBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUE9EU00CGBYNCgIQFcAAng0HHk3AADbMNsoStgkCUrKABQoAzpKeBBR0EVAVwABQFcAAXgQzkH4J+yMqiYODAlleGNlcHRpb246QFrYJh4LCxLASlvPDAtub1JlZW50cmFudAH/ABJNNA1iWjQsWjVkAAAAQFcAA3g0s3pKeBFR0EVBm/ZnznkRiE4QUdBQEsBKeBBR0EVAVwEBeBHOeBDOwUVTi1BBkl3oMXBoC5cMD0FscmVhZHkgZW50ZXJlZOEReBHOeBDOwUVTi1BB5j8YhEBXAAF4Ec54EM7BRVOLUEEvWMXtQFzYJiELCxLASlvPDAtub1JlZW50cmFudAH/ABJNNXH///9kXDWN////NTv///9cNL1AVwABXdgmIAsLEsBKXs8MDXJlZW50cmFudFRlc3QB/wASTTQaZV00PHgQlyYEIgt4AHuXJgUQNMpdNGRAVwADeDXT/v//ekp4EVHQRUGb9mfOeRGIThBR0FASwEp4EFHQRUBXAQF4Ec54EM7BRVOLUEGSXegxcGgLlwwPQWxyZWFkeSBlbnRlcmVk4RF4Ec54EM7BRVOLUEHmPxiEQFcAAXgRzngQzsFFU4tQQS9Yxe1AVgcKAAAAAApf/v//EsBhCvr+//8Kuv7//xLAYwrQ////CpD///8SwGZAAKqtGw=="));

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
