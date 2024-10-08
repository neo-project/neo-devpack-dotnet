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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFrFShHPShLPShPPShTPQFcCAMVKC89KC89KEM9KEM9KC89KNDBwxUoLz0oQz0o0KUpoFFHQRRBxNMlKwUVoElHQaBTOEVHQRXFFaUpoE1HQRWhAVwABQFcAAUB+EaX0"));

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
    /// Script: VwIAxUoLz0oLz0oQz0oQz0oLz0o0MHDFSgvPShDPSjQpSmgUUdBFEHE0yUrBRWgSUdBoFM4RUdBFcUVpSmgTUdBFaEA=
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
    /// 13 : OpCode.DUP
    /// 14 : OpCode.CALL 30
    /// 16 : OpCode.STLOC0
    /// 17 : OpCode.NEWSTRUCT0
    /// 18 : OpCode.DUP
    /// 19 : OpCode.PUSHNULL
    /// 1A : OpCode.APPEND
    /// 1B : OpCode.DUP
    /// 1C : OpCode.PUSH0
    /// 1D : OpCode.APPEND
    /// 1E : OpCode.DUP
    /// 1F : OpCode.CALL 29
    /// 21 : OpCode.DUP
    /// 22 : OpCode.LDLOC0
    /// 23 : OpCode.PUSH4
    /// 24 : OpCode.ROT
    /// 25 : OpCode.SETITEM
    /// 26 : OpCode.DROP
    /// 27 : OpCode.PUSH0
    /// 28 : OpCode.STLOC1
    /// 29 : OpCode.CALL C9
    /// 2B : OpCode.DUP
    /// 2C : OpCode.UNPACK
    /// 2D : OpCode.DROP
    /// 2E : OpCode.LDLOC0
    /// 2F : OpCode.PUSH2
    /// 30 : OpCode.ROT
    /// 31 : OpCode.SETITEM
    /// 32 : OpCode.LDLOC0
    /// 33 : OpCode.PUSH4
    /// 34 : OpCode.PICKITEM
    /// 35 : OpCode.PUSH1
    /// 36 : OpCode.ROT
    /// 37 : OpCode.SETITEM
    /// 38 : OpCode.DROP
    /// 39 : OpCode.STLOC1
    /// 3A : OpCode.DROP
    /// 3B : OpCode.LDLOC1
    /// 3C : OpCode.DUP
    /// 3D : OpCode.LDLOC0
    /// 3E : OpCode.PUSH3
    /// 3F : OpCode.ROT
    /// 40 : OpCode.SETITEM
    /// 41 : OpCode.DROP
    /// 42 : OpCode.LDLOC0
    /// 43 : OpCode.RET
    /// </remarks>
    [DisplayName("t1")]
    public abstract object? T1();

    #endregion
}
