using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_IndexOrRange : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_IndexOrRange"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testMain"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/Z4BVxQADAoBAgMEBQYHCAkK2zBwaErKUBBRS5+McWgTUBBRS5+McmhKylASUUufjHNoFVATUUufjHRoSspQSsoSn1FLn4x1aErKE59QEFFLn4x2aErKFJ9QE1FLn4x3B2hKyhKfUErKFJ9RS5+MdwhoEM53CWnKNwAAQc/nR5ZqyjcAAEHP50eWa8o3AABBz+dHlmzKNwAAQc/nR5ZtyjcAAEHP50eWbso3AABBz+dHlm8HyjcAAEHP50eWbwjKNwAAQc/nR5ZvCTcAAEHP50eWDAkxMjM0NTY3ODl3Cm8KSspQEFFLn4zbKHcLbwoTUBBRS5+M2yh3DG8KSspQElFLn4zbKHcNbwoVUBNRS5+M2yh3Dm8KSspQSsoSn1FLn4zbKHcPbwpKyhOfUBBRS5+M2yh3EG8KSsoUn1ATUUufjNsodxFvCkrKEp9QSsoUn1FLn4zbKHcSbwoQzncTbwtBz+dHlm8MQc/nR5ZvDUHP50eWbw5Bz+dHlm8PQc/nR5ZvEEHP50eWbxFBz+dHlm8SQc/nR5ZvE9soQc/nR5ZAs0P5Fg=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testMain")]
    public abstract void TestMain();

    #endregion

    #region Constructor for internal use only

    protected Contract_IndexOrRange(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
