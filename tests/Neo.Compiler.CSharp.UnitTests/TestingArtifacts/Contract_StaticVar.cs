using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_StaticVar(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_StaticVar"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testinitalvalue"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""testMain"",""parameters"":[],""returntype"":""Integer"",""offset"":14,""safe"":false},{""name"":""testBigIntegerParse"",""parameters"":[],""returntype"":""Integer"",""offset"":96,""safe"":false},{""name"":""testBigIntegerParse2"",""parameters"":[{""name"":""text"",""type"":""String""}],""returntype"":""Integer"",""offset"":99,""safe"":false},{""name"":""testGetUInt160"",""parameters"":[],""returntype"":""Hash160"",""offset"":107,""safe"":false},{""name"":""testGetECPoint"",""parameters"":[],""returntype"":""PublicKey"",""offset"":130,""safe"":false},{""name"":""testGetString"",""parameters"":[],""returntype"":""String"",""offset"":166,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":180,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABBQAAuQwLaGVsbG8gd29ybGRANAY0KllAWRWeSsoUMh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfYUBZF6BKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9hQAB7QFcAAXg3AABADBR+7hqr62ftHXkdROT1/POukXGocUAMIQJHANsukNnwLE+fyGKrrKknJflbT93MjX/6U4aT7PRjqUAMC2hlbGxvIHdvcmxkQFYGEWFAL0KAsg==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AHtA
    /// PUSHINT8 7B [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerParse")]
    public abstract BigInteger? TestBigIntegerParse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerParse2")]
    public abstract BigInteger? TestBigIntegerParse2(string? text);

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
    /// Script: NAY0KllA
    /// CALL 06 [512 datoshi]
    /// CALL 2A [512 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract BigInteger? TestMain();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAtoZWxsbyB3b3JsZEA=
    /// PUSHDATA1 68656C6C6F20776F726C64 [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testinitalvalue")]
    public abstract string? Testinitalvalue();

    #endregion
}
