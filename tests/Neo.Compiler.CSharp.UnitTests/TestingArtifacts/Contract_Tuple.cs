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
    /// 0000 : OpCode.NEWSTRUCT0
    /// 0001 : OpCode.DUP
    /// 0002 : OpCode.PUSH1
    /// 0003 : OpCode.APPEND
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH2
    /// 0006 : OpCode.APPEND
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.PUSH3
    /// 0009 : OpCode.APPEND
    /// 000A : OpCode.DUP
    /// 000B : OpCode.PUSH4
    /// 000C : OpCode.APPEND
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("getResult")]
    public abstract IList<object>? GetResult();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAxUoLz0oLz0oQz0oQz0oLz0o0MHDFSgvPShDPSjQpSmgUUdBFEHE0yUrBRWgSUdBoFM4RUdBFcUVpSmgTUdBFaEA=
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.NEWSTRUCT0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHNULL
    /// 0006 : OpCode.APPEND
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.PUSHNULL
    /// 0009 : OpCode.APPEND
    /// 000A : OpCode.DUP
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.APPEND
    /// 000D : OpCode.DUP
    /// 000E : OpCode.PUSH0
    /// 000F : OpCode.APPEND
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSHNULL
    /// 0012 : OpCode.APPEND
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.CALL 30
    /// 0016 : OpCode.STLOC0
    /// 0017 : OpCode.NEWSTRUCT0
    /// 0018 : OpCode.DUP
    /// 0019 : OpCode.PUSHNULL
    /// 001A : OpCode.APPEND
    /// 001B : OpCode.DUP
    /// 001C : OpCode.PUSH0
    /// 001D : OpCode.APPEND
    /// 001E : OpCode.DUP
    /// 001F : OpCode.CALL 29
    /// 0021 : OpCode.DUP
    /// 0022 : OpCode.LDLOC0
    /// 0023 : OpCode.PUSH4
    /// 0024 : OpCode.ROT
    /// 0025 : OpCode.SETITEM
    /// 0026 : OpCode.DROP
    /// 0027 : OpCode.PUSH0
    /// 0028 : OpCode.STLOC1
    /// 0029 : OpCode.CALL C9
    /// 002B : OpCode.DUP
    /// 002C : OpCode.UNPACK
    /// 002D : OpCode.DROP
    /// 002E : OpCode.LDLOC0
    /// 002F : OpCode.PUSH2
    /// 0030 : OpCode.ROT
    /// 0031 : OpCode.SETITEM
    /// 0032 : OpCode.LDLOC0
    /// 0033 : OpCode.PUSH4
    /// 0034 : OpCode.PICKITEM
    /// 0035 : OpCode.PUSH1
    /// 0036 : OpCode.ROT
    /// 0037 : OpCode.SETITEM
    /// 0038 : OpCode.DROP
    /// 0039 : OpCode.STLOC1
    /// 003A : OpCode.DROP
    /// 003B : OpCode.LDLOC1
    /// 003C : OpCode.DUP
    /// 003D : OpCode.LDLOC0
    /// 003E : OpCode.PUSH3
    /// 003F : OpCode.ROT
    /// 0040 : OpCode.SETITEM
    /// 0041 : OpCode.DROP
    /// 0042 : OpCode.LDLOC0
    /// 0043 : OpCode.RET
    /// </remarks>
    [DisplayName("t1")]
    public abstract object? T1();

    #endregion

}
