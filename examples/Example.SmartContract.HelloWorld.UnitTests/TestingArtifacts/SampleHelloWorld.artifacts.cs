using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleHelloWorld(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleHelloWorld"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sayHello"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":true}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Description"":""A simple \u0060hello world\u0060 contract"",""E-mail"":""dev@neo.org"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy44LjErODk2ZjU2ZGIzZmM4YjU5OGU3YmExYTM1ODE4OGRiYzNmZmYuLi4AAAAAABIMDUhlbGxvLCBXb3JsZCEiAkA+VJV6").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Properties

    /// <summary>
    /// Safe property
    /// </summary>
    public abstract string? SayHello { [DisplayName("sayHello")] get; }

    #endregion

}
