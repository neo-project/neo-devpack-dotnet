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
    /// 00 : OpCode.PUSH4
    /// 01 : OpCode.PUSH3
    /// 02 : OpCode.PUSH2
    /// 03 : OpCode.PUSH1
    /// 04 : OpCode.PUSH4
    /// 05 : OpCode.PACKSTRUCT
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("getResult")]
    public abstract IList<object>? GetResult();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIACxAQCwsVv3AQCxK/SmgUUdBFEHE04krBRWgSUdBoFM4RUdBFcUVpSmgTUdBFaEA=
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.PUSH0
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.PUSHNULL
    /// 08 : OpCode.PUSH5
    /// 09 : OpCode.PACKSTRUCT
    /// 0A : OpCode.STLOC0
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.PUSHNULL
    /// 0D : OpCode.PUSH2
    /// 0E : OpCode.PACKSTRUCT
    /// 0F : OpCode.DUP
    /// 10 : OpCode.LDLOC0
    /// 11 : OpCode.PUSH4
    /// 12 : OpCode.ROT
    /// 13 : OpCode.SETITEM
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSH0
    /// 16 : OpCode.STLOC1
    /// 17 : OpCode.CALL E2
    /// 19 : OpCode.DUP
    /// 1A : OpCode.UNPACK
    /// 1B : OpCode.DROP
    /// 1C : OpCode.LDLOC0
    /// 1D : OpCode.PUSH2
    /// 1E : OpCode.ROT
    /// 1F : OpCode.SETITEM
    /// 20 : OpCode.LDLOC0
    /// 21 : OpCode.PUSH4
    /// 22 : OpCode.PICKITEM
    /// 23 : OpCode.PUSH1
    /// 24 : OpCode.ROT
    /// 25 : OpCode.SETITEM
    /// 26 : OpCode.DROP
    /// 27 : OpCode.STLOC1
    /// 28 : OpCode.DROP
    /// 29 : OpCode.LDLOC1
    /// 2A : OpCode.DUP
    /// 2B : OpCode.LDLOC0
    /// 2C : OpCode.PUSH3
    /// 2D : OpCode.ROT
    /// 2E : OpCode.SETITEM
    /// 2F : OpCode.DROP
    /// 30 : OpCode.LDLOC0
    /// 31 : OpCode.RET
    /// </remarks>
    [DisplayName("t1")]
    public abstract object? T1();

    #endregion
}
