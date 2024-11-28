using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Event(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Event"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""Void"",""offset"":0,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":37,""safe"":false}],""events"":[{""name"":""transfer"",""parameters"":[{""name"":""arg1"",""type"":""ByteArray""},{""name"":""arg2"",""type"":""ByteArray""},{""name"":""arg3"",""type"":""Integer""}]}]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACwXYAhhWAwDBAUG2zAMAwECA9swE8AMCHRyYW5zZmVyQZUBb2FAVgIQYAlhQIsCNj0=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Events

    public delegate void deltransfer(byte[]? arg1, byte[]? arg2, BigInteger? arg3);

    [DisplayName("transfer")]
    public event deltransfer? OnTransfer;

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: F2AIYVgMAwQFBtswDAMBAgPbMBPADAh0cmFuc2ZlckGVAW9hQA==
    /// 00 : PUSH7 [1 datoshi]
    /// 01 : STSFLD0 [2 datoshi]
    /// 02 : PUSHT [1 datoshi]
    /// 03 : STSFLD1 [2 datoshi]
    /// 04 : LDSFLD0 [2 datoshi]
    /// 05 : PUSHDATA1 040506 [8 datoshi]
    /// 0A : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0C : PUSHDATA1 010203 [8 datoshi]
    /// 11 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 13 : PUSH3 [1 datoshi]
    /// 14 : PACK [2048 datoshi]
    /// 15 : PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// 1F : SYSCALL 95016F61 'System.Runtime.Notify' [32768 datoshi]
    /// 24 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract void Test();

    #endregion
}
