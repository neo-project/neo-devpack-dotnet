using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_DirectInit(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_DirectInit"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testGetUInt160"",""parameters"":[],""returntype"":""Hash160"",""offset"":0,""safe"":false},{""name"":""testGetECPoint"",""parameters"":[],""returntype"":""PublicKey"",""offset"":23,""safe"":false},{""name"":""testGetUInt256"",""parameters"":[],""returntype"":""Hash256"",""offset"":59,""safe"":false},{""name"":""testGetString"",""parameters"":[],""returntype"":""String"",""offset"":94,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGwMFH7uGqvrZ+0deR1E5PX8866RcahxQAwhAkcA2y6Q2fAsT5/IYqusqScl+VtP3cyNf/pThpPs9GOpQAwg7c+GeRBOwpEaT+Ka19sjKkk+W5kPsdp68Me5iZSMiSVADAtoZWxsbyB3b3JsZEDGUdKy"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DCECRwDbLpDZ8CxPn8hiq6ypJyX5W0/dzI1/+lOGk+z0Y6lA
    /// PUSHDATA1 024700DB2E90D9F02C4F9FC862ABACA92725F95B4FDDCC8D7FFA538693ECF463A9 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetECPoint")]
    public abstract ECPoint? TestGetECPoint();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAtoZWxsbyB3b3JsZEA=
    /// PUSHDATA1 68656C6C6F20776F726C64 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetString")]
    public abstract string? TestGetString();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DBR+7hqr62ftHXkdROT1/POukXGocUA=
    /// PUSHDATA1 7EEE1AABEB67ED1D791D44E4F5FCF3AE9171A871 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetUInt160")]
    public abstract UInt160? TestGetUInt160();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DCDtz4Z5EE7CkRpP4prX2yMqST5bmQ+x2nrwx7mJlIyJJUA=
    /// PUSHDATA1 EDCF8679104EC2911A4FE29AD7DB232A493E5B990FB1DA7AF0C7B989948C8925 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetUInt256")]
    public abstract UInt256? TestGetUInt256();

    #endregion
}
