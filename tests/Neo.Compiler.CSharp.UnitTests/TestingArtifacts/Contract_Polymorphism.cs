using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Polymorphism(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Polymorphism"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":169,""safe"":false},{""name"":""test"",""parameters"":[],""returntype"":""String"",""offset"":177,""safe"":false},{""name"":""test2"",""parameters"":[],""returntype"":""String"",""offset"":183,""safe"":false},{""name"":""abstractTest"",""parameters"":[],""returntype"":""String"",""offset"":189,""safe"":false},{""name"":""mul"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":192,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":138,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMZXAAN5ep5AVwABDAl0ZXN0RmluYWxAVwABeDQTeDQai9soDAUudGVzdIvbKEBXAAEMBHRlc3RAVwABeDQODAYudGVzdDKL2yhAVwABDAViYXNlMkBXAAF4NBkMEW92ZXJyaWRlbkFic3RyYWN0i9soQFcAAQwMYWJzdHJhY3RUZXN0QFcAA3l6oEBWAgsKvP///woAAAAAE8BgCwqu////CgAAAAATwGFAWBHAI1T////CI1X////CI17////CIpZZEcAiwECSFyWP"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("abstractTest")]
    public abstract string? AbstractTest();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("mul")]
    public abstract BigInteger? Mul(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test")]
    public abstract string? Test();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test2")]
    public abstract string? Test2();

    #endregion
}
