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
    /// 00 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 01 : OpCode.DUP	[2 datoshi]
    /// 02 : OpCode.PUSH1	[1 datoshi]
    /// 03 : OpCode.APPEND	[8192 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.PUSH2	[1 datoshi]
    /// 06 : OpCode.APPEND	[8192 datoshi]
    /// 07 : OpCode.DUP	[2 datoshi]
    /// 08 : OpCode.PUSH3	[1 datoshi]
    /// 09 : OpCode.APPEND	[8192 datoshi]
    /// 0A : OpCode.DUP	[2 datoshi]
    /// 0B : OpCode.PUSH4	[1 datoshi]
    /// 0C : OpCode.APPEND	[8192 datoshi]
    /// 0D : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("getResult")]
    public abstract IList<object>? GetResult();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAxUoLz0oLz0oQz0oQz0oLz3DFSgvPShDPSmgUUdBFEHE0z0rBRWgSUdBoFM4RUdBFcUVpSmgTUdBFaEA=
    /// 00 : OpCode.INITSLOT 0200	[64 datoshi]
    /// 03 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.PUSHNULL	[1 datoshi]
    /// 06 : OpCode.APPEND	[8192 datoshi]
    /// 07 : OpCode.DUP	[2 datoshi]
    /// 08 : OpCode.PUSHNULL	[1 datoshi]
    /// 09 : OpCode.APPEND	[8192 datoshi]
    /// 0A : OpCode.DUP	[2 datoshi]
    /// 0B : OpCode.PUSH0	[1 datoshi]
    /// 0C : OpCode.APPEND	[8192 datoshi]
    /// 0D : OpCode.DUP	[2 datoshi]
    /// 0E : OpCode.PUSH0	[1 datoshi]
    /// 0F : OpCode.APPEND	[8192 datoshi]
    /// 10 : OpCode.DUP	[2 datoshi]
    /// 11 : OpCode.PUSHNULL	[1 datoshi]
    /// 12 : OpCode.APPEND	[8192 datoshi]
    /// 13 : OpCode.STLOC0	[2 datoshi]
    /// 14 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 15 : OpCode.DUP	[2 datoshi]
    /// 16 : OpCode.PUSHNULL	[1 datoshi]
    /// 17 : OpCode.APPEND	[8192 datoshi]
    /// 18 : OpCode.DUP	[2 datoshi]
    /// 19 : OpCode.PUSH0	[1 datoshi]
    /// 1A : OpCode.APPEND	[8192 datoshi]
    /// 1B : OpCode.DUP	[2 datoshi]
    /// 1C : OpCode.LDLOC0	[2 datoshi]
    /// 1D : OpCode.PUSH4	[1 datoshi]
    /// 1E : OpCode.ROT	[2 datoshi]
    /// 1F : OpCode.SETITEM	[8192 datoshi]
    /// 20 : OpCode.DROP	[2 datoshi]
    /// 21 : OpCode.PUSH0	[1 datoshi]
    /// 22 : OpCode.STLOC1	[2 datoshi]
    /// 23 : OpCode.CALL CF	[512 datoshi]
    /// 25 : OpCode.DUP	[2 datoshi]
    /// 26 : OpCode.UNPACK	[2048 datoshi]
    /// 27 : OpCode.DROP	[2 datoshi]
    /// 28 : OpCode.LDLOC0	[2 datoshi]
    /// 29 : OpCode.PUSH2	[1 datoshi]
    /// 2A : OpCode.ROT	[2 datoshi]
    /// 2B : OpCode.SETITEM	[8192 datoshi]
    /// 2C : OpCode.LDLOC0	[2 datoshi]
    /// 2D : OpCode.PUSH4	[1 datoshi]
    /// 2E : OpCode.PICKITEM	[64 datoshi]
    /// 2F : OpCode.PUSH1	[1 datoshi]
    /// 30 : OpCode.ROT	[2 datoshi]
    /// 31 : OpCode.SETITEM	[8192 datoshi]
    /// 32 : OpCode.DROP	[2 datoshi]
    /// 33 : OpCode.STLOC1	[2 datoshi]
    /// 34 : OpCode.DROP	[2 datoshi]
    /// 35 : OpCode.LDLOC1	[2 datoshi]
    /// 36 : OpCode.DUP	[2 datoshi]
    /// 37 : OpCode.LDLOC0	[2 datoshi]
    /// 38 : OpCode.PUSH3	[1 datoshi]
    /// 39 : OpCode.ROT	[2 datoshi]
    /// 3A : OpCode.SETITEM	[8192 datoshi]
    /// 3B : OpCode.DROP	[2 datoshi]
    /// 3C : OpCode.LDLOC0	[2 datoshi]
    /// 3D : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("t1")]
    public abstract object? T1();

    #endregion
}
