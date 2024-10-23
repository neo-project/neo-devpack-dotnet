using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Initializer(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Initializer"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""sum"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""sum1"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":77,""safe"":false},{""name"":""sum2"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":150,""safe"":false},{""name"":""anonymousObjectCreation"",""parameters"":[],""returntype"":""Void"",""offset"":229,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0gAVcBABAQEsBKNDlwaBDOaBHOnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwABeBAR0HgREtBAVwECEBASwEo07EoQeNBKEXnQcGgQzmgRzp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfQFcBAhAQEsBKNKNweEpoEFHQRXlKaBFR0EVoEM5oEc6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAgAMBUhlbGxvAGwSwHBoNClBz+dHlhEMBWdyYXBlEsAUDAVhcHBsZRLAEsBxaRDONAlBz+dHlkBAQDtWYvc="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("anonymousObjectCreation")]
    public abstract void AnonymousObjectCreation();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum")]
    public abstract BigInteger? Sum();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum1")]
    public abstract BigInteger? Sum1(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("sum2")]
    public abstract BigInteger? Sum2(BigInteger? a, BigInteger? b);

    #endregion
}
