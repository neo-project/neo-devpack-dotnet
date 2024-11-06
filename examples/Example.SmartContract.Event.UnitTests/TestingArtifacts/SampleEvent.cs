using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleEvent(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleEvent"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""main"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false}],""events"":[{""name"":""new_event_name"",""parameters"":[{""name"":""arg1"",""type"":""ByteArray""},{""name"":""arg2"",""type"":""String""},{""name"":""arg3"",""type"":""Integer""}]},{""name"":""event2"",""parameters"":[{""name"":""arg1"",""type"":""ByteArray""},{""name"":""arg2"",""type"":""Integer""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Description"":""A sample contract that demonstrates how to use Events"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADxXAQAMAwECA9swcBoMAm9paBPADA5uZXdfZXZlbnRfbmFtZUGVAW9hADJoEsAMBmV2ZW50MkGVAW9hCUDlVbC3"));

    #endregion

    #region Events

    public delegate void delevent2(byte[]? arg1, BigInteger? arg2);

    [DisplayName("event2")]
    public event delevent2? OnEvent2;

    public delegate void delnew_event_name(byte[]? arg1, string? arg2, BigInteger? arg3);

    [DisplayName("new_event_name")]
    public event delnew_event_name? OnNew_event_name;

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("main")]
    public abstract bool? Main();

    #endregion
}
