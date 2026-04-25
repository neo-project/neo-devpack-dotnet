using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class SampleContractCall(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleContractCall"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""onNEP17Payment"",""parameters"":[{""name"":""from"",""type"":""Hash160""},{""name"":""amount"",""type"":""Integer""},{""name"":""data"",""type"":""Any""}],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":107,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Author"":""core-dev"",""Version"":""0.0.1"",""Description"":""A sample contract to demonstrate how to call a contract"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy45LjErMzY2NWY4MzEyYmJlZWQxZGY5NjRjNTM2M2QxNjZjOWQ2NGYuLi4AAAAAAIVXBAN6cGjZIUomBGhxqiYFCCIHaQB7s6omBCI+Qdv+qHRxQTlTbjxyaRHAFQwJYmFsYW5jZU9makFifVtSc2tqaRPAHwwLZHVtbXlNZXRob2RYQWJ9W1JFQEHb/qh0QEE5U248QEFifVtSQFYBDBRekBBogrxXM212exXV7S6cBT6oE2BA2uovsA==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("onNEP17Payment")]
    public abstract void OnNEP17Payment(UInt160? from, BigInteger? amount, object? data = null);

    #endregion
}
