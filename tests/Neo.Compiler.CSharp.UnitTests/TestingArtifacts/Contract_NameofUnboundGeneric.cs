using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NameofUnboundGeneric(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NameofUnboundGeneric"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""Dictionary"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""describeNested"",""parameters"":[],""returntype"":""String"",""offset"":35,""safe"":false},{""name"":""List"",""parameters"":[],""returntype"":""String"",""offset"":78,""safe"":false},{""name"":""describeNestedMembers"",""parameters"":[],""returntype"":""String"",""offset"":113,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy44LjErNzljNTY3MDY4NGM1YjUyYTQyZWJhYjMyZjgxNWU1ODRlMzUuLi4AAAAAAJ1XAgAMCkRpY3Rpb25hcnlwDARGdW5jcWgMATqLaYvbKCICQFcCAAwKRW51bWVyYXRvcnAMDEtleVZhbHVlUGFpcnFoDAF8i2mL2ygiAkBXAgAMBExpc3RwDApWYWx1ZVR1cGxlcWgMATqLaYvbKCICQFcCAAwNS2V5Q29sbGVjdGlvbnAMCkVudW1lcmF0b3JxaAwBfItpi9soIgJAzGqwzA==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("describeNested")]
    public abstract string? DescribeNested();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("describeNestedMembers")]
    public abstract string? DescribeNestedMembers();

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract string? Dictionary();

    /// <summary>
    /// Unsafe method
    /// </summary>
    public abstract string? List();

    #endregion
}
