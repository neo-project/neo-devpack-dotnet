using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Tuple(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Tuple"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""getResult"",""parameters"":[],""returntype"":""Array"",""offset"":0,""safe"":false},{""name"":""t1"",""parameters"":[],""returntype"":""Any"",""offset"":14,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEzFShHPShLPShPPShTPQFcCAMVKC89KC89KEM9KEM9KC89wxUoLz0oQz0poFFHQRRBxNM9KwUVoElHQaBTOEVHQRXFFaUpoE1HQRWhAW5sP4w=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: xUoRz0oSz0oTz0oUz0A=
    /// 00 : OpCode.NEWSTRUCT0
    /// 01 : OpCode.DUP
    /// 02 : OpCode.PUSH1
    /// 03 : OpCode.APPEND
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.APPEND
    /// 07 : OpCode.DUP
    /// 08 : OpCode.PUSH3
    /// 09 : OpCode.APPEND
    /// 0A : OpCode.DUP
    /// 0B : OpCode.PUSH4
    /// 0C : OpCode.APPEND
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("getResult")]
    public abstract IList<object>? GetResult();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAxUoLz0oLz0oQz0oQz0oLz3DFSgvPShDPSmgUUdBFEHE0z0rBRWgSUdBoFM4RUdBFcUVpSmgTUdBFaEA=
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.NEWSTRUCT0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.APPEND
    /// 07 : OpCode.DUP
    /// 08 : OpCode.PUSHNULL
    /// 09 : OpCode.APPEND
    /// 0A : OpCode.DUP
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.APPEND
    /// 0D : OpCode.DUP
    /// 0E : OpCode.PUSH0
    /// 0F : OpCode.APPEND
    /// 10 : OpCode.DUP
    /// 11 : OpCode.PUSHNULL
    /// 12 : OpCode.APPEND
    /// 13 : OpCode.STLOC0
    /// 14 : OpCode.NEWSTRUCT0
    /// 15 : OpCode.DUP
    /// 16 : OpCode.PUSHNULL
    /// 17 : OpCode.APPEND
    /// 18 : OpCode.DUP
    /// 19 : OpCode.PUSH0
    /// 1A : OpCode.APPEND
    /// 1B : OpCode.DUP
    /// 1C : OpCode.LDLOC0
    /// 1D : OpCode.PUSH4
    /// 1E : OpCode.ROT
    /// 1F : OpCode.SETITEM
    /// 20 : OpCode.DROP
    /// 21 : OpCode.PUSH0
    /// 22 : OpCode.STLOC1
    /// 23 : OpCode.CALL CF
    /// 25 : OpCode.DUP
    /// 26 : OpCode.UNPACK
    /// 27 : OpCode.DROP
    /// 28 : OpCode.LDLOC0
    /// 29 : OpCode.PUSH2
    /// 2A : OpCode.ROT
    /// 2B : OpCode.SETITEM
    /// 2C : OpCode.LDLOC0
    /// 2D : OpCode.PUSH4
    /// 2E : OpCode.PICKITEM
    /// 2F : OpCode.PUSH1
    /// 30 : OpCode.ROT
    /// 31 : OpCode.SETITEM
    /// 32 : OpCode.DROP
    /// 33 : OpCode.STLOC1
    /// 34 : OpCode.DROP
    /// 35 : OpCode.LDLOC1
    /// 36 : OpCode.DUP
    /// 37 : OpCode.LDLOC0
    /// 38 : OpCode.PUSH3
    /// 39 : OpCode.ROT
    /// 3A : OpCode.SETITEM
    /// 3B : OpCode.DROP
    /// 3C : OpCode.LDLOC0
    /// 3D : OpCode.RET
    /// </remarks>
    [DisplayName("t1")]
    public abstract object? T1();

    #endregion
}
