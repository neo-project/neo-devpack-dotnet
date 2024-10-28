using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Polymorphism(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Polymorphism"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":378,""safe"":false},{""name"":""sumToBeOverriden"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":386,""safe"":false},{""name"":""test"",""parameters"":[],""returntype"":""String"",""offset"":392,""safe"":false},{""name"":""test2"",""parameters"":[],""returntype"":""String"",""offset"":398,""safe"":false},{""name"":""abstractTest"",""parameters"":[],""returntype"":""String"",""offset"":404,""safe"":false},{""name"":""mul"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":410,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":347,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2gAVcAA3l6nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwADenl4NGN5eqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwADenl4NVz///9AVwABDAl0ZXN0RmluYWxAVwABeDQTeDQai9soDAUudGVzdIvbKEBXAAEMBHRlc3RAVwABeDQODAYudGVzdDKL2yhAVwABDAViYXNlMkBXAAF4NBkMEW92ZXJyaWRlbkFic3RyYWN0i9soQFcAAQwMYWJzdHJhY3RUZXN0QFcAA3l6oEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVgILCo7///8KAAAAABPAYAsKgP///woAAAAAE8BhQFgRwCOD/v//wiOy/v//wiMh////wiMq////wiNi////WRHAIolAS7ugrQ=="));

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
    [DisplayName("sumToBeOverriden")]
    public abstract BigInteger? SumToBeOverriden(BigInteger? a, BigInteger? b);

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
