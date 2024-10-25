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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGBXAQALEAsTwAwESm9obks0JXBoNDcmHWhKEc5OnBFQ0EVoEs4RS0vOSlRTnNBFaBDOQAtAVwACeBIAUAA8AFATwNB5SngQUdBFQFcBAXhwaAuXqiQECUB4EM5waAuXqkC6FsDO"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoC5eqJAQJQHgQznBoC5eqQA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.NOT
    /// 09 : OpCode.JMPIF 04
    /// 0B : OpCode.PUSHF
    /// 0C : OpCode.RET
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.PICKITEM
    /// 10 : OpCode.STLOC0
    /// 11 : OpCode.LDLOC0
    /// 12 : OpCode.PUSHNULL
    /// 13 : OpCode.EQUAL
    /// 14 : OpCode.NOT
    /// 15 : OpCode.RET
    /// </remarks>
    [DisplayName("isValid")]
    public abstract bool? IsValid(object? person = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxALE8AMSm9obks0JXBoNDcmHWhKEc5OnBFQ0EVoEs4RS0vOSlRTnNBFaBDOQAtA
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.PUSH3
    /// 07 : OpCode.PACK
    /// 08 : OpCode.PUSHDATA1 4A6F686E
    /// 0E : OpCode.OVER
    /// 0F : OpCode.CALL 25
    /// 11 : OpCode.STLOC0
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.CALL 37
    /// 15 : OpCode.JMPIFNOT 1D
    /// 17 : OpCode.LDLOC0
    /// 18 : OpCode.DUP
    /// 19 : OpCode.PUSH1
    /// 1A : OpCode.PICKITEM
    /// 1B : OpCode.TUCK
    /// 1C : OpCode.INC
    /// 1D : OpCode.PUSH1
    /// 1E : OpCode.SWAP
    /// 1F : OpCode.SETITEM
    /// 20 : OpCode.DROP
    /// 21 : OpCode.LDLOC0
    /// 22 : OpCode.PUSH2
    /// 23 : OpCode.PICKITEM
    /// 24 : OpCode.PUSH1
    /// 25 : OpCode.OVER
    /// 26 : OpCode.OVER
    /// 27 : OpCode.PICKITEM
    /// 28 : OpCode.DUP
    /// 29 : OpCode.REVERSE4
    /// 2A : OpCode.REVERSE3
    /// 2B : OpCode.INC
    /// 2C : OpCode.SETITEM
    /// 2D : OpCode.DROP
    /// 2E : OpCode.LDLOC0
    /// 2F : OpCode.PUSH0
    /// 30 : OpCode.PICKITEM
    /// 31 : OpCode.RET
    /// 32 : OpCode.PUSHNULL
    /// 33 : OpCode.RET
    /// </remarks>
    [DisplayName("test")]
    public abstract string? Test();

    #endregion
}
