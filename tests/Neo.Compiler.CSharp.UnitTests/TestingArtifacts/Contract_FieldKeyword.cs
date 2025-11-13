using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_FieldKeyword(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_FieldKeyword"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""update"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":54,""safe"":false},{""name"":""recordLastPositiveSequence"",""parameters"":[{""name"":""first"",""type"":""Integer""},{""name"":""second"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":67,""safe"":false},{""name"":""recordLastNonZeroSequence"",""parameters"":[{""name"":""first"",""type"":""Integer""},{""name"":""second"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":85,""safe"":false},{""name"":""accumulateWallet"",""parameters"":[{""name"":""firstDeposit"",""type"":""Integer""},{""name"":""secondDeposit"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":103,""safe"":false},{""name"":""trackPositiveWallet"",""parameters"":[{""name"":""first"",""type"":""Boolean""},{""name"":""second"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":207,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":259,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy44LjErNzljNTY3MDY4NGM1YjUyYTQyZWJhYjMyZjgxNWU1ODRlMzUuLi4AAAAAAP0MAVhAVwABeBC4JgV4IgMQSmBFQFlAVwABeBC3JgV4IgNZSmFFQFpAVwABeBCXJgVaIgN4SmJFQFcAAXhKNMdFNMIiAkBXAAJ4SjTMRXlKNMdFNMIiAkBXAAJ4SjTMRXlKNMdFNMIiAkBXAQIJEBLAcHhKaDQPRXlKaDQJRWg0TCICQFcAAnkQuCY3eBDOeZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfIgV4EM5KeBBR0EVAVwABeBDOQFcBAgkQEsBweEpoNA9FeUpoNAlFaDQYIgJAVwACeBHOJgUIIgN5SngRUdBFQFcAAXgRzkBWAxBgEGEQYkBkrlnI").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("accumulateWallet")]
    public abstract BigInteger? AccumulateWallet(BigInteger? firstDeposit, BigInteger? secondDeposit);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("recordLastNonZeroSequence")]
    public abstract BigInteger? RecordLastNonZeroSequence(BigInteger? first, BigInteger? second);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("recordLastPositiveSequence")]
    public abstract BigInteger? RecordLastPositiveSequence(BigInteger? first, BigInteger? second);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("trackPositiveWallet")]
    public abstract bool? TrackPositiveWallet(bool? first, bool? second);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("update")]
    public abstract BigInteger? Update(BigInteger? value);

    #endregion
}
