using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Polymorphism : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Polymorphism"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":306,""safe"":false},{""name"":""test"",""parameters"":[],""returntype"":""String"",""offset"":321,""safe"":false},{""name"":""test2"",""parameters"":[],""returntype"":""String"",""offset"":336,""safe"":false},{""name"":""mul"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":351,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":264,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1vAVcAA3l6nkoCAAAAgC4EIgpKAv///38yTgX/////AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJFKAv///38yJAUAAAAAAQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAJ9AVwABeDQDQFcAAUBXAAEMBHRlc3RAVwABeDQDQFcAAXg04EBXAAF4NA0MBS50ZXN0i9soQFcAAQwEYmFzZUBXAAN5eqBKAgAAAIAuBCIKSgL///9/Mk4F/////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACRSgL///9/MiQFAAAAAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACfQFYDCo////8KAAAAABLAYApx////ClT///8SwGEKdf///woAAAAAEsBiQMJKWM9KNS7///8jxP7//8JKWc9KNTT///8jJf///8JKWc9KNSX///8jLv///8JKWs9KNR3///8jOv///0BpGZ2r"));

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

    #region Constructor for internal use only

    protected Contract_Polymorphism(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
