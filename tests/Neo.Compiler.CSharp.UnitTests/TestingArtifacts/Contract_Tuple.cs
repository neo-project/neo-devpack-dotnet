using Neo.Cryptography.ECC;
using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFMUExIRFL9AVwIACxAQCwsVv0o0LXAQCxK/SjQxSmgUUdBFEHE03ErBRWgSUdBoFM4RUdBFcUVpSmgTUdBFaEBXAAF4EhDQeBMQ0EBXAAF4ERDQQHaQFSs=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FBMSERS/QA==
    /// 00 : PUSH4 [1 datoshi]
    /// 01 : PUSH3 [1 datoshi]
    /// 02 : PUSH2 [1 datoshi]
    /// 03 : PUSH1 [1 datoshi]
    /// 04 : PUSH4 [1 datoshi]
    /// 05 : PACKSTRUCT [2048 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("getResult")]
    public abstract IList<object>? GetResult();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIACxAQCwsVv0o0LXAQCxK/SjQxSmgUUdBFEHE03ErBRWgSUdBoFM4RUdBFcUVpSmgTUdBFaEA=
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : PUSHNULL [1 datoshi]
    /// 07 : PUSHNULL [1 datoshi]
    /// 08 : PUSH5 [1 datoshi]
    /// 09 : PACKSTRUCT [2048 datoshi]
    /// 0A : DUP [2 datoshi]
    /// 0B : CALL 2D [512 datoshi]
    /// 0D : STLOC0 [2 datoshi]
    /// 0E : PUSH0 [1 datoshi]
    /// 0F : PUSHNULL [1 datoshi]
    /// 10 : PUSH2 [1 datoshi]
    /// 11 : PACKSTRUCT [2048 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : CALL 31 [512 datoshi]
    /// 15 : DUP [2 datoshi]
    /// 16 : LDLOC0 [2 datoshi]
    /// 17 : PUSH4 [1 datoshi]
    /// 18 : ROT [2 datoshi]
    /// 19 : SETITEM [8192 datoshi]
    /// 1A : DROP [2 datoshi]
    /// 1B : PUSH0 [1 datoshi]
    /// 1C : STLOC1 [2 datoshi]
    /// 1D : CALL DC [512 datoshi]
    /// 1F : DUP [2 datoshi]
    /// 20 : UNPACK [2048 datoshi]
    /// 21 : DROP [2 datoshi]
    /// 22 : LDLOC0 [2 datoshi]
    /// 23 : PUSH2 [1 datoshi]
    /// 24 : ROT [2 datoshi]
    /// 25 : SETITEM [8192 datoshi]
    /// 26 : LDLOC0 [2 datoshi]
    /// 27 : PUSH4 [1 datoshi]
    /// 28 : PICKITEM [64 datoshi]
    /// 29 : PUSH1 [1 datoshi]
    /// 2A : ROT [2 datoshi]
    /// 2B : SETITEM [8192 datoshi]
    /// 2C : DROP [2 datoshi]
    /// 2D : STLOC1 [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : LDLOC1 [2 datoshi]
    /// 30 : DUP [2 datoshi]
    /// 31 : LDLOC0 [2 datoshi]
    /// 32 : PUSH3 [1 datoshi]
    /// 33 : ROT [2 datoshi]
    /// 34 : SETITEM [8192 datoshi]
    /// 35 : DROP [2 datoshi]
    /// 36 : LDLOC0 [2 datoshi]
    /// 37 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("t1")]
    public abstract object? T1();

    #endregion
}
