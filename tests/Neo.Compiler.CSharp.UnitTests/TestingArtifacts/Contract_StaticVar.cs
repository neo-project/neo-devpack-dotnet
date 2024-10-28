using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_StaticVar(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_StaticVar"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testinitalvalue"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""testMain"",""parameters"":[],""returntype"":""Integer"",""offset"":14,""safe"":false},{""name"":""testBigIntegerParse"",""parameters"":[],""returntype"":""Integer"",""offset"":30,""safe"":false},{""name"":""testBigIntegerParse2"",""parameters"":[{""name"":""text"",""type"":""String""}],""returntype"":""Integer"",""offset"":35,""safe"":false},{""name"":""testGetUInt160"",""parameters"":[],""returntype"":""Hash160"",""offset"":43,""safe"":false},{""name"":""testGetECPoint"",""parameters"":[],""returntype"":""PublicKey"",""offset"":66,""safe"":false},{""name"":""testGetString"",""parameters"":[],""returntype"":""String"",""offset"":102,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":116,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAAeQwLaGVsbG8gd29ybGRANAY0CVlAWRWeYUBZF6BhQAB4E55AVwABeDcAAEAMFH7uGqvrZ+0deR1E5PX8866RcahxQAwhAkcA2y6Q2fAsT5/IYqusqScl+VtP3cyNf/pThpPs9GOpQAwLaGVsbG8gd29ybGRAVgYRYUDD5R8I"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testBigIntegerParse")]
    public abstract BigInteger? TestBigIntegerParse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testBigIntegerParse2")]
    public abstract BigInteger? TestBigIntegerParse2(string? text);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetECPoint")]
    public abstract ECPoint? TestGetECPoint();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetString")]
    public abstract string? TestGetString();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetUInt160")]
    public abstract UInt160? TestGetUInt160();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testinitalvalue")]
    public abstract string? Testinitalvalue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NAY0CVlA
    /// 00 : OpCode.CALL 06 [512 datoshi]
    /// 02 : OpCode.CALL 09 [512 datoshi]
    /// 04 : OpCode.LDSFLD1 [2 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract BigInteger? TestMain();

    #endregion
}
