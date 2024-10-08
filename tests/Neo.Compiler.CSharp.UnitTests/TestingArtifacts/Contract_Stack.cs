using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Stack(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Stack"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test_Push_Integer"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""test_Push_Integer_Internal"",""parameters"":[],""returntype"":""Array"",""offset"":5,""safe"":false},{""name"":""test_External"",""parameters"":[],""returntype"":""Array"",""offset"":106,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIFXAAF4QMVKEM9KAf8Az0oAgM9KAH/PSgEAgM9KAf9/z0oC//8AAM9KA/////8AAAAAz0oCAAAAgM9KAv///3/PSgT//////////wAAAAAAAAAAz0oDAAAAAAAAAIDPSgP/////////f89AxUoPz0oCwL3w/89KAwDwWisX////z0CuZFC1"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: xUoPz0oCwL3w/89KAwDwWisX////z0A=
    /// 00 : OpCode.NEWSTRUCT0
    /// 01 : OpCode.DUP
    /// 02 : OpCode.PUSHM1
    /// 03 : OpCode.APPEND
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT32 C0BDF0FF
    /// 0A : OpCode.APPEND
    /// 0B : OpCode.DUP
    /// 0C : OpCode.PUSHINT64 00F05A2B17FFFFFF
    /// 15 : OpCode.APPEND
    /// 16 : OpCode.RET
    /// </remarks>
    [DisplayName("test_External")]
    public abstract IList<object>? Test_External();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.RET
    /// </remarks>
    [DisplayName("test_Push_Integer")]
    public abstract BigInteger? Test_Push_Integer(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: xUoQz0oB/wDPSgCAz0oAf89KAQCAz0oB/3/PSgL//wAAz0oD/////wAAAADPSgIAAACAz0oC////f89KBP//////////AAAAAAAAAADPSgMAAAAAAAAAgM9KA/////////9/z0A=
    /// 00 : OpCode.NEWSTRUCT0
    /// 01 : OpCode.DUP
    /// 02 : OpCode.PUSH0
    /// 03 : OpCode.APPEND
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT16 FF00
    /// 08 : OpCode.APPEND
    /// 09 : OpCode.DUP
    /// 0A : OpCode.PUSHINT8 80
    /// 0C : OpCode.APPEND
    /// 0D : OpCode.DUP
    /// 0E : OpCode.PUSHINT8 7F
    /// 10 : OpCode.APPEND
    /// 11 : OpCode.DUP
    /// 12 : OpCode.PUSHINT16 0080
    /// 15 : OpCode.APPEND
    /// 16 : OpCode.DUP
    /// 17 : OpCode.PUSHINT16 FF7F
    /// 1A : OpCode.APPEND
    /// 1B : OpCode.DUP
    /// 1C : OpCode.PUSHINT32 FFFF0000
    /// 21 : OpCode.APPEND
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2C : OpCode.APPEND
    /// 2D : OpCode.DUP
    /// 2E : OpCode.PUSHINT32 00000080
    /// 33 : OpCode.APPEND
    /// 34 : OpCode.DUP
    /// 35 : OpCode.PUSHINT32 FFFFFF7F
    /// 3A : OpCode.APPEND
    /// 3B : OpCode.DUP
    /// 3C : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 4D : OpCode.APPEND
    /// 4E : OpCode.DUP
    /// 4F : OpCode.PUSHINT64 0000000000000080
    /// 58 : OpCode.APPEND
    /// 59 : OpCode.DUP
    /// 5A : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 63 : OpCode.APPEND
    /// 64 : OpCode.RET
    /// </remarks>
    [DisplayName("test_Push_Integer_Internal")]
    public abstract IList<object>? Test_Push_Integer_Internal();

    #endregion
}
