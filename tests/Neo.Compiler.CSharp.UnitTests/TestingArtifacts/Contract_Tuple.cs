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
    [DisplayName("getResult")]
    public abstract IList<object>? GetResult();
    // 0000 : NEWSTRUCT0
    // 0001 : DUP
    // 0002 : PUSH1
    // 0003 : APPEND
    // 0004 : DUP
    // 0005 : PUSH2
    // 0006 : APPEND
    // 0007 : DUP
    // 0008 : PUSH3
    // 0009 : APPEND
    // 000A : DUP
    // 000B : PUSH4
    // 000C : APPEND
    // 000D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("t1")]
    public abstract object? T1();
    // 0000 : INITSLOT
    // 0003 : NEWSTRUCT0
    // 0004 : DUP
    // 0005 : PUSHNULL
    // 0006 : APPEND
    // 0007 : DUP
    // 0008 : PUSHNULL
    // 0009 : APPEND
    // 000A : DUP
    // 000B : PUSH0
    // 000C : APPEND
    // 000D : DUP
    // 000E : PUSH0
    // 000F : APPEND
    // 0010 : DUP
    // 0011 : PUSHNULL
    // 0012 : APPEND
    // 0013 : DUP
    // 0014 : CALL
    // 0016 : STLOC0
    // 0017 : NEWSTRUCT0
    // 0018 : DUP
    // 0019 : PUSHNULL
    // 001A : APPEND
    // 001B : DUP
    // 001C : PUSH0
    // 001D : APPEND
    // 001E : DUP
    // 001F : CALL
    // 0021 : DUP
    // 0022 : LDLOC0
    // 0023 : PUSH4
    // 0024 : ROT
    // 0025 : SETITEM
    // 0026 : DROP
    // 0027 : PUSH0
    // 0028 : STLOC1
    // 0029 : CALL
    // 002B : DUP
    // 002C : UNPACK
    // 002D : DROP
    // 002E : LDLOC0
    // 002F : PUSH2
    // 0030 : ROT
    // 0031 : SETITEM
    // 0032 : LDLOC0
    // 0033 : PUSH4
    // 0034 : PICKITEM
    // 0035 : PUSH1
    // 0036 : ROT
    // 0037 : SETITEM
    // 0038 : DROP
    // 0039 : STLOC1
    // 003A : DROP
    // 003B : LDLOC1
    // 003C : DUP
    // 003D : LDLOC0
    // 003E : PUSH3
    // 003F : ROT
    // 0040 : SETITEM
    // 0041 : DROP
    // 0042 : LDLOC0
    // 0043 : RET

    #endregion

}
