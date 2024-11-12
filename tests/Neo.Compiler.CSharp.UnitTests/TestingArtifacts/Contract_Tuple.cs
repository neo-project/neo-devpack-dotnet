using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Tuple(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Tuple"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getResult"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""t1"",""parameters"":[],""returntype"":""Any"",""offset"":7,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFMUExIRFL9AVwIACxAQCwsVv0o0LXAQCxK/SjQxSmgUUdBFEHE03ErBRWgSUdBoFM4RUdBFcUVpSmgTUdBFaEBXAAF4EhDQeBMQ0EBXAAF4ERDQQHaQFSs="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FBMSERS/QA==
    /// 00 : OpCode.PUSH4 [1 datoshi]
    /// 01 : OpCode.PUSH3 [1 datoshi]
    /// 02 : OpCode.PUSH2 [1 datoshi]
    /// 03 : OpCode.PUSH1 [1 datoshi]
    /// 04 : OpCode.PUSH4 [1 datoshi]
    /// 05 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("getResult")]
    public abstract IList<object>? GetResult();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIACxAQCwsVv0o0LXAQCxK/SjQxSmgUUdBFEHE03ErBRWgSUdBoFM4RUdBFcUVpSmgTUdBFaEA=
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.PUSHNULL [1 datoshi]
    /// 07 : OpCode.PUSHNULL [1 datoshi]
    /// 08 : OpCode.PUSH5 [1 datoshi]
    /// 09 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 0A : OpCode.DUP [2 datoshi]
    /// 0B : OpCode.CALL 2D [512 datoshi]
    /// 0D : OpCode.STLOC0 [2 datoshi]
    /// 0E : OpCode.PUSH0 [1 datoshi]
    /// 0F : OpCode.PUSHNULL [1 datoshi]
    /// 10 : OpCode.PUSH2 [1 datoshi]
    /// 11 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.CALL 31 [512 datoshi]
    /// 15 : OpCode.DUP [2 datoshi]
    /// 16 : OpCode.LDLOC0 [2 datoshi]
    /// 17 : OpCode.PUSH4 [1 datoshi]
    /// 18 : OpCode.ROT [2 datoshi]
    /// 19 : OpCode.SETITEM [8192 datoshi]
    /// 1A : OpCode.DROP [2 datoshi]
    /// 1B : OpCode.PUSH0 [1 datoshi]
    /// 1C : OpCode.STLOC1 [2 datoshi]
    /// 1D : OpCode.CALL DC [512 datoshi]
    /// 1F : OpCode.DUP [2 datoshi]
    /// 20 : OpCode.UNPACK [2048 datoshi]
    /// 21 : OpCode.DROP [2 datoshi]
    /// 22 : OpCode.LDLOC0 [2 datoshi]
    /// 23 : OpCode.PUSH2 [1 datoshi]
    /// 24 : OpCode.ROT [2 datoshi]
    /// 25 : OpCode.SETITEM [8192 datoshi]
    /// 26 : OpCode.LDLOC0 [2 datoshi]
    /// 27 : OpCode.PUSH4 [1 datoshi]
    /// 28 : OpCode.PICKITEM [64 datoshi]
    /// 29 : OpCode.PUSH1 [1 datoshi]
    /// 2A : OpCode.ROT [2 datoshi]
    /// 2B : OpCode.SETITEM [8192 datoshi]
    /// 2C : OpCode.DROP [2 datoshi]
    /// 2D : OpCode.STLOC1 [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.LDLOC1 [2 datoshi]
    /// 30 : OpCode.DUP [2 datoshi]
    /// 31 : OpCode.LDLOC0 [2 datoshi]
    /// 32 : OpCode.PUSH3 [1 datoshi]
    /// 33 : OpCode.ROT [2 datoshi]
    /// 34 : OpCode.SETITEM [8192 datoshi]
    /// 35 : OpCode.DROP [2 datoshi]
    /// 36 : OpCode.LDLOC0 [2 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("t1")]
    public abstract object? T1();

    #endregion
}
