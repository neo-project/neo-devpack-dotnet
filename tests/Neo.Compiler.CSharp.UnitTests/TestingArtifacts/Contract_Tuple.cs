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
    /// Script: VwIACxAQCwsVv3AQCxK/SmgUUdBFEHE04krBRWgSUdBoFM4RUdBFcUVpSmgTUdBFaEA=
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : PUSH0 [1 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : PUSHNULL [1 datoshi]
    /// 07 : PUSHNULL [1 datoshi]
    /// 08 : PUSH5 [1 datoshi]
    /// 09 : PACKSTRUCT [2048 datoshi]
    /// 0A : STLOC0 [2 datoshi]
    /// 0B : PUSH0 [1 datoshi]
    /// 0C : PUSHNULL [1 datoshi]
    /// 0D : PUSH2 [1 datoshi]
    /// 0E : PACKSTRUCT [2048 datoshi]
    /// 0F : DUP [2 datoshi]
    /// 10 : LDLOC0 [2 datoshi]
    /// 11 : PUSH4 [1 datoshi]
    /// 12 : ROT [2 datoshi]
    /// 13 : SETITEM [8192 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSH0 [1 datoshi]
    /// 16 : STLOC1 [2 datoshi]
    /// 17 : CALL E2 [512 datoshi]
    /// 19 : DUP [2 datoshi]
    /// 1A : UNPACK [2048 datoshi]
    /// 1B : DROP [2 datoshi]
    /// 1C : LDLOC0 [2 datoshi]
    /// 1D : PUSH2 [1 datoshi]
    /// 1E : ROT [2 datoshi]
    /// 1F : SETITEM [8192 datoshi]
    /// 20 : LDLOC0 [2 datoshi]
    /// 21 : PUSH4 [1 datoshi]
    /// 22 : PICKITEM [64 datoshi]
    /// 23 : PUSH1 [1 datoshi]
    /// 24 : ROT [2 datoshi]
    /// 25 : SETITEM [8192 datoshi]
    /// 26 : DROP [2 datoshi]
    /// 27 : STLOC1 [2 datoshi]
    /// 28 : DROP [2 datoshi]
    /// 29 : LDLOC1 [2 datoshi]
    /// 2A : DUP [2 datoshi]
    /// 2B : LDLOC0 [2 datoshi]
    /// 2C : PUSH3 [1 datoshi]
    /// 2D : ROT [2 datoshi]
    /// 2E : SETITEM [8192 datoshi]
    /// 2F : DROP [2 datoshi]
    /// 30 : LDLOC0 [2 datoshi]
    /// 31 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("t1")]
    public abstract object? T1();

    #endregion
}
