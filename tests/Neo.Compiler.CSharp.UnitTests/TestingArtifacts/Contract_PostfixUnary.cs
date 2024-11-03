using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_PostfixUnary(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_PostfixUnary"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""isValid"",""parameters"":[{""name"":""person"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":74,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAF1XAQALEAsTwAwESm9obks0JXBoNDcmHWhKEc5OnBFQ0EVoEs4RS0vOSlRTnNBFaBDOQAtAVwACeBIAUAA8AFATwNB5SngQUdBFQFcBAXhwaNgmBAlAeBDOcGjYqkDtxq/U"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2CYECUB4EM5waNiqQA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDLOC0 [2 datoshi]
    /// 06 : OpCode.ISNULL [2 datoshi]
    /// 07 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 09 : OpCode.PUSHF [1 datoshi]
    /// 0A : OpCode.RET [0 datoshi]
    /// 0B : OpCode.LDARG0 [2 datoshi]
    /// 0C : OpCode.PUSH0 [1 datoshi]
    /// 0D : OpCode.PICKITEM [64 datoshi]
    /// 0E : OpCode.STLOC0 [2 datoshi]
    /// 0F : OpCode.LDLOC0 [2 datoshi]
    /// 10 : OpCode.ISNULL [2 datoshi]
    /// 11 : OpCode.NOT [4 datoshi]
    /// 12 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("isValid")]
    public abstract bool? IsValid(object? person = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxALE8AMBEpvaG5LNCVwaDQ3Jh1oShHOTpwRUNBFaBLOEUtLzkpUU5zQRWgQzkALQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.PUSHNULL [1 datoshi]
    /// 06 : OpCode.PUSH3 [1 datoshi]
    /// 07 : OpCode.PACK [2048 datoshi]
    /// 08 : OpCode.PUSHDATA1 4A6F686E [8 datoshi]
    /// 0E : OpCode.OVER [2 datoshi]
    /// 0F : OpCode.CALL 25 [512 datoshi]
    /// 11 : OpCode.STLOC0 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.CALL 37 [512 datoshi]
    /// 15 : OpCode.JMPIFNOT 1D [2 datoshi]
    /// 17 : OpCode.LDLOC0 [2 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.PUSH1 [1 datoshi]
    /// 1A : OpCode.PICKITEM [64 datoshi]
    /// 1B : OpCode.TUCK [2 datoshi]
    /// 1C : OpCode.INC [4 datoshi]
    /// 1D : OpCode.PUSH1 [1 datoshi]
    /// 1E : OpCode.SWAP [2 datoshi]
    /// 1F : OpCode.SETITEM [8192 datoshi]
    /// 20 : OpCode.DROP [2 datoshi]
    /// 21 : OpCode.LDLOC0 [2 datoshi]
    /// 22 : OpCode.PUSH2 [1 datoshi]
    /// 23 : OpCode.PICKITEM [64 datoshi]
    /// 24 : OpCode.PUSH1 [1 datoshi]
    /// 25 : OpCode.OVER [2 datoshi]
    /// 26 : OpCode.OVER [2 datoshi]
    /// 27 : OpCode.PICKITEM [64 datoshi]
    /// 28 : OpCode.DUP [2 datoshi]
    /// 29 : OpCode.REVERSE4 [2 datoshi]
    /// 2A : OpCode.REVERSE3 [2 datoshi]
    /// 2B : OpCode.INC [4 datoshi]
    /// 2C : OpCode.SETITEM [8192 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.LDLOC0 [2 datoshi]
    /// 2F : OpCode.PUSH0 [1 datoshi]
    /// 30 : OpCode.PICKITEM [64 datoshi]
    /// 31 : OpCode.RET [0 datoshi]
    /// 32 : OpCode.PUSHNULL [1 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("test")]
    public abstract string? Test();

    #endregion
}
