using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_StaticVar(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_StaticVar"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testinitalvalue"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""testMain"",""parameters"":[],""returntype"":""Integer"",""offset"":14,""safe"":false},{""name"":""testBigIntegerParse"",""parameters"":[],""returntype"":""Integer"",""offset"":122,""safe"":false},{""name"":""testBigIntegerParse2"",""parameters"":[{""name"":""text"",""type"":""String""}],""returntype"":""Integer"",""offset"":127,""safe"":false},{""name"":""testGetUInt160"",""parameters"":[],""returntype"":""Hash160"",""offset"":135,""safe"":false},{""name"":""testGetECPoint"",""parameters"":[],""returntype"":""PublicKey"",""offset"":158,""safe"":false},{""name"":""testGetString"",""parameters"":[],""returntype"":""String"",""offset"":194,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":208,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""atoi""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARhdG9pAQABDwAA1QwLaGVsbG8gd29ybGRANAY0N1hAWBWeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2BAWBegSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2BAAHgTnkBXAAF4NwAAQAwUfu4aq+tn7R15HUTk9fzzrpFxqHFADCECRwDbLpDZ8CxPn8hiq6ypJyX5W0/dzI1/+lOGk+z0Y6lADAtoZWxsbyB3b3JsZEBWBhFgQCyGUes="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AHgTnkA=
    /// 00 : OpCode.PUSHINT8 78 [1 datoshi]
    /// 02 : OpCode.PUSH3 [1 datoshi]
    /// 03 : OpCode.ADD [8 datoshi]
    /// 04 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerParse")]
    public abstract BigInteger? TestBigIntegerParse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDcAAEA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALLT 0000 [32768 datoshi]
    /// 07 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testBigIntegerParse2")]
    public abstract BigInteger? TestBigIntegerParse2(string? text);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DCECRwDbLpDZ8CxPn8hiq6ypJyX5W0/dzI1/+lOGk+z0Y6lA
    /// 00 : OpCode.PUSHDATA1 024700DB2E90D9F02C4F9FC862ABACA92725F95B4FDDCC8D7FFA538693ECF463A9 [8 datoshi]
    /// 23 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetECPoint")]
    public abstract ECPoint? TestGetECPoint();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAtoZWxsbyB3b3JsZEA=
    /// 00 : OpCode.PUSHDATA1 68656C6C6F20776F726C64 [8 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetString")]
    public abstract string? TestGetString();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DBR+7hqr62ftHXkdROT1/POukXGocUA=
    /// 00 : OpCode.PUSHDATA1 7EEE1AABEB67ED1D791D44E4F5FCF3AE9171A871 [8 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetUInt160")]
    public abstract UInt160? TestGetUInt160();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAtoZWxsbyB3b3JsZEA=
    /// 00 : OpCode.PUSHDATA1 68656C6C6F20776F726C64 [8 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testinitalvalue")]
    public abstract string? Testinitalvalue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: NAY0N1hA
    /// 00 : OpCode.CALL 06 [512 datoshi]
    /// 02 : OpCode.CALL 37 [512 datoshi]
    /// 04 : OpCode.LDSFLD0 [2 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMain")]
    public abstract BigInteger? TestMain();

    #endregion
}
