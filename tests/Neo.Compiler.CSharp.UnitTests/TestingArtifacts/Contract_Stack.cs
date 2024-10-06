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
    /// 0000 : OpCode.NEWSTRUCT0
    /// 0001 : OpCode.DUP
    /// 0002 : OpCode.PUSHM1
    /// 0003 : OpCode.APPEND
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT32 C0BDF0FF
    /// 000A : OpCode.APPEND
    /// 000B : OpCode.DUP
    /// 000C : OpCode.PUSHINT64 00F05A2B17FFFFFF
    /// 0015 : OpCode.APPEND
    /// 0016 : OpCode.RET
    /// </remarks>
    [DisplayName("test_External")]
    public abstract IList<object>? Test_External();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.RET
    /// </remarks>
    [DisplayName("test_Push_Integer")]
    public abstract BigInteger? Test_Push_Integer(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: xUoQz0oB/wDPSgCAz0oAf89KAQCAz0oB/3/PSgL//wAAz0oD/////wAAAADPSgIAAACAz0oC////f89KBP//////////AAAAAAAAAADPSgMAAAAAAAAAgM9KA/////////9/z0A=
    /// 0000 : OpCode.NEWSTRUCT0
    /// 0001 : OpCode.DUP
    /// 0002 : OpCode.PUSH0
    /// 0003 : OpCode.APPEND
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT16 FF00
    /// 0008 : OpCode.APPEND
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.PUSHINT8 80
    /// 000C : OpCode.APPEND
    /// 000D : OpCode.DUP
    /// 000E : OpCode.PUSHINT8 7F
    /// 0010 : OpCode.APPEND
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.PUSHINT16 0080
    /// 0015 : OpCode.APPEND
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.PUSHINT16 FF7F
    /// 001A : OpCode.APPEND
    /// 001B : OpCode.DUP
    /// 001C : OpCode.PUSHINT32 FFFF0000
    /// 0021 : OpCode.APPEND
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 002C : OpCode.APPEND
    /// 002D : OpCode.DUP
    /// 002E : OpCode.PUSHINT32 00000080
    /// 0033 : OpCode.APPEND
    /// 0034 : OpCode.DUP
    /// 0035 : OpCode.PUSHINT32 FFFFFF7F
    /// 003A : OpCode.APPEND
    /// 003B : OpCode.DUP
    /// 003C : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 004D : OpCode.APPEND
    /// 004E : OpCode.DUP
    /// 004F : OpCode.PUSHINT64 0000000000000080
    /// 0058 : OpCode.APPEND
    /// 0059 : OpCode.DUP
    /// 005A : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 0063 : OpCode.APPEND
    /// 0064 : OpCode.RET
    /// </remarks>
    [DisplayName("test_Push_Integer_Internal")]
    public abstract IList<object>? Test_Push_Integer_Internal();

    #endregion

}
