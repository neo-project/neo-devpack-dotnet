using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Params(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Params"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""Integer"",""offset"":31,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADdXBQEQcHhKccpyEHMiDWlrznRobJ5wa5xza2ow82hAwjTgERHANNueExISwDTUnhUUEsA0zZ5Ae9zc2w=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: wjTgERHANNueExISwDTUnhUUEsA0zZ5A
    /// 00 : OpCode.NEWARRAY0 [16 datoshi]
    /// 01 : OpCode.CALL E0 [512 datoshi]
    /// 03 : OpCode.PUSH1 [1 datoshi]
    /// 04 : OpCode.PUSH1 [1 datoshi]
    /// 05 : OpCode.PACK [2048 datoshi]
    /// 06 : OpCode.CALL DB [512 datoshi]
    /// 08 : OpCode.ADD [8 datoshi]
    /// 09 : OpCode.PUSH3 [1 datoshi]
    /// 0A : OpCode.PUSH2 [1 datoshi]
    /// 0B : OpCode.PUSH2 [1 datoshi]
    /// 0C : OpCode.PACK [2048 datoshi]
    /// 0D : OpCode.CALL D4 [512 datoshi]
    /// 0F : OpCode.ADD [8 datoshi]
    /// 10 : OpCode.PUSH5 [1 datoshi]
    /// 11 : OpCode.PUSH4 [1 datoshi]
    /// 12 : OpCode.PUSH2 [1 datoshi]
    /// 13 : OpCode.PACK [2048 datoshi]
    /// 14 : OpCode.CALL CD [512 datoshi]
    /// 16 : OpCode.ADD [8 datoshi]
    /// 17 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract BigInteger? Test();

    #endregion
}
