using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACwXYAhhWAwDBAUG2zAMAwECA9swE8AMCHRyYW5zZmVyQZUBb2FAVgIQYAlhQIsCNj0="));

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
    /// PUSH7 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// PUSHDATA1 040506 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// PUSHDATA1 010203 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHDATA1 7472616E73666572 'transfer' [8 datoshi]
    /// SYSCALL 95016F61 'System.Runtime.Notify' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract void Test();

    #endregion
}
