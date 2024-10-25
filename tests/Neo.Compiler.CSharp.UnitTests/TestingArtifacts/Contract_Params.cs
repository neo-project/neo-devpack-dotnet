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
    /// 00 : OpCode.NEWARRAY0
    /// 01 : OpCode.CALL E0
    /// 03 : OpCode.PUSH1
    /// 04 : OpCode.PUSH1
    /// 05 : OpCode.PACK
    /// 06 : OpCode.CALL DB
    /// 08 : OpCode.ADD
    /// 09 : OpCode.PUSH3
    /// 0A : OpCode.PUSH2
    /// 0B : OpCode.PUSH2
    /// 0C : OpCode.PACK
    /// 0D : OpCode.CALL D4
    /// 0F : OpCode.ADD
    /// 10 : OpCode.PUSH5
    /// 11 : OpCode.PUSH4
    /// 12 : OpCode.PUSH2
    /// 13 : OpCode.PACK
    /// 14 : OpCode.CALL CD
    /// 16 : OpCode.ADD
    /// 17 : OpCode.RET
    /// </remarks>
    [DisplayName("test")]
    public abstract BigInteger? Test();

    #endregion
}
