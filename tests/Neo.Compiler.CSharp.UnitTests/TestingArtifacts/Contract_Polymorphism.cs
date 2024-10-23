using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Polymorphism(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Polymorphism"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":185,""safe"":false},{""name"":""test"",""parameters"":[],""returntype"":""String"",""offset"":193,""safe"":false},{""name"":""test2"",""parameters"":[],""returntype"":""String"",""offset"":201,""safe"":false},{""name"":""mul"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":209,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":143,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAANpXAAN5ep5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcAAQwEdGVzdEBXAAF4NA0MBS50ZXN0i9soQFcAAQwEYmFzZUBXAAN5eqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFYDCr////8KAAAAABLAYAqh////CpL///8SwGEKpf///woAAAAAEsBiQFgRwCNE////WRHAI3H///9ZEcAjc////1oRwCOG////QE2A0dU="));

    #endregion

    #region Unsafe methods

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
