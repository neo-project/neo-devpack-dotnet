using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Optimize(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Optimize"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""unitTest_001"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""testVoid"",""parameters"":[],""returntype"":""Void"",""offset"":14,""safe"":false},{""name"":""testArgs1"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""ByteArray"",""offset"":27,""safe"":false},{""name"":""testArgs2"",""parameters"":[{""name"":""a"",""type"":""ByteArray""}],""returntype"":""Any"",""offset"":48,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADVXAQAMBAECAwTbMHBoQFcBAAwEAQIDBNswcEBXAQEMBAECAwPbMHB4SmgTUdBFaEBXAAF4QH4bEdg=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBDAQBAgMD2zBweEpoE1HQRWhA
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : PUSHDATA1 01020303 [8 datoshi]
    /// 09 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0B : STLOC0 [2 datoshi]
    /// 0C : LDARG0 [2 datoshi]
    /// 0D : DUP [2 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : PUSH3 [1 datoshi]
    /// 10 : ROT [2 datoshi]
    /// 11 : SETITEM [8192 datoshi]
    /// 12 : DROP [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testArgs1")]
    public abstract byte[]? TestArgs1(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testArgs2")]
    public abstract object? TestArgs2(byte[]? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAQBAgME2zBwQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHDATA1 01020304 [8 datoshi]
    /// 09 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0B : STLOC0 [2 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testVoid")]
    public abstract void TestVoid();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADAQBAgME2zBwaEA=
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHDATA1 01020304 [8 datoshi]
    /// 09 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0B : STLOC0 [2 datoshi]
    /// 0C : LDLOC0 [2 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("unitTest_001")]
    public abstract byte[]? UnitTest_001();

    #endregion
}
