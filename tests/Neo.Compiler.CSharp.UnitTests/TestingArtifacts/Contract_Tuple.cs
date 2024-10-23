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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADkUExIRFL9AVwIACxAQCwsVv3AQCxK/SmgUUdBFEHE04krBRWgSUdBoFM4RUdBFcUVpSmgTUdBFaECoZ8J4"));

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
    /// Script: VwIACxAQCwsVv3AQCxK/SmgUUdBFEHE04krBRWgSUdBoFM4RUdBFcUVpSmgTUdBFaEA=
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.PUSH0 [1 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.PUSHNULL [1 datoshi]
    /// 07 : OpCode.PUSHNULL [1 datoshi]
    /// 08 : OpCode.PUSH5 [1 datoshi]
    /// 09 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 0A : OpCode.STLOC0 [2 datoshi]
    /// 0B : OpCode.PUSH0 [1 datoshi]
    /// 0C : OpCode.PUSHNULL [1 datoshi]
    /// 0D : OpCode.PUSH2 [1 datoshi]
    /// 0E : OpCode.PACKSTRUCT [2048 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.LDLOC0 [2 datoshi]
    /// 11 : OpCode.PUSH4 [1 datoshi]
    /// 12 : OpCode.ROT [2 datoshi]
    /// 13 : OpCode.SETITEM [8192 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSH0 [1 datoshi]
    /// 16 : OpCode.STLOC1 [2 datoshi]
    /// 17 : OpCode.CALL E2 [512 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.UNPACK [2048 datoshi]
    /// 1B : OpCode.DROP [2 datoshi]
    /// 1C : OpCode.LDLOC0 [2 datoshi]
    /// 1D : OpCode.PUSH2 [1 datoshi]
    /// 1E : OpCode.ROT [2 datoshi]
    /// 1F : OpCode.SETITEM [8192 datoshi]
    /// 20 : OpCode.LDLOC0 [2 datoshi]
    /// 21 : OpCode.PUSH4 [1 datoshi]
    /// 22 : OpCode.PICKITEM [64 datoshi]
    /// 23 : OpCode.PUSH1 [1 datoshi]
    /// 24 : OpCode.ROT [2 datoshi]
    /// 25 : OpCode.SETITEM [8192 datoshi]
    /// 26 : OpCode.DROP [2 datoshi]
    /// 27 : OpCode.STLOC1 [2 datoshi]
    /// 28 : OpCode.DROP [2 datoshi]
    /// 29 : OpCode.LDLOC1 [2 datoshi]
    /// 2A : OpCode.DUP [2 datoshi]
    /// 2B : OpCode.LDLOC0 [2 datoshi]
    /// 2C : OpCode.PUSH3 [1 datoshi]
    /// 2D : OpCode.ROT [2 datoshi]
    /// 2E : OpCode.SETITEM [8192 datoshi]
    /// 2F : OpCode.DROP [2 datoshi]
    /// 30 : OpCode.LDLOC0 [2 datoshi]
    /// 31 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("t1")]
    public abstract object? T1();

    #endregion
}
