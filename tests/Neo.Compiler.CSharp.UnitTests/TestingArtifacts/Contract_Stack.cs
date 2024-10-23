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
    /// 00 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 01 : OpCode.DUP	[2 datoshi]
    /// 02 : OpCode.PUSHM1	[1 datoshi]
    /// 03 : OpCode.APPEND	[8192 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.PUSHINT32 C0BDF0FF	[1 datoshi]
    /// 0A : OpCode.APPEND	[8192 datoshi]
    /// 0B : OpCode.DUP	[2 datoshi]
    /// 0C : OpCode.PUSHINT64 00F05A2B17FFFFFF	[1 datoshi]
    /// 15 : OpCode.APPEND	[8192 datoshi]
    /// 16 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("test_External")]
    public abstract IList<object>? Test_External();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : OpCode.INITSLOT 0001	[64 datoshi]
    /// 03 : OpCode.LDARG0	[2 datoshi]
    /// 04 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("test_Push_Integer")]
    public abstract BigInteger? Test_Push_Integer(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: xUoQz0oB/wDPSgCAz0oAf89KAQCAz0oB/3/PSgL//wAAz0oD/////wAAAADPSgIAAACAz0oC////f89KBP//////////AAAAAAAAAADPSgMAAAAAAAAAgM9KA/////////9/z0A=
    /// 00 : OpCode.NEWSTRUCT0	[16 datoshi]
    /// 01 : OpCode.DUP	[2 datoshi]
    /// 02 : OpCode.PUSH0	[1 datoshi]
    /// 03 : OpCode.APPEND	[8192 datoshi]
    /// 04 : OpCode.DUP	[2 datoshi]
    /// 05 : OpCode.PUSHINT16 FF00	[1 datoshi]
    /// 08 : OpCode.APPEND	[8192 datoshi]
    /// 09 : OpCode.DUP	[2 datoshi]
    /// 0A : OpCode.PUSHINT8 80	[1 datoshi]
    /// 0C : OpCode.APPEND	[8192 datoshi]
    /// 0D : OpCode.DUP	[2 datoshi]
    /// 0E : OpCode.PUSHINT8 7F	[1 datoshi]
    /// 10 : OpCode.APPEND	[8192 datoshi]
    /// 11 : OpCode.DUP	[2 datoshi]
    /// 12 : OpCode.PUSHINT16 0080	[1 datoshi]
    /// 15 : OpCode.APPEND	[8192 datoshi]
    /// 16 : OpCode.DUP	[2 datoshi]
    /// 17 : OpCode.PUSHINT16 FF7F	[1 datoshi]
    /// 1A : OpCode.APPEND	[8192 datoshi]
    /// 1B : OpCode.DUP	[2 datoshi]
    /// 1C : OpCode.PUSHINT32 FFFF0000	[1 datoshi]
    /// 21 : OpCode.APPEND	[8192 datoshi]
    /// 22 : OpCode.DUP	[2 datoshi]
    /// 23 : OpCode.PUSHINT64 FFFFFFFF00000000	[1 datoshi]
    /// 2C : OpCode.APPEND	[8192 datoshi]
    /// 2D : OpCode.DUP	[2 datoshi]
    /// 2E : OpCode.PUSHINT32 00000080	[1 datoshi]
    /// 33 : OpCode.APPEND	[8192 datoshi]
    /// 34 : OpCode.DUP	[2 datoshi]
    /// 35 : OpCode.PUSHINT32 FFFFFF7F	[1 datoshi]
    /// 3A : OpCode.APPEND	[8192 datoshi]
    /// 3B : OpCode.DUP	[2 datoshi]
    /// 3C : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000	[4 datoshi]
    /// 4D : OpCode.APPEND	[8192 datoshi]
    /// 4E : OpCode.DUP	[2 datoshi]
    /// 4F : OpCode.PUSHINT64 0000000000000080	[1 datoshi]
    /// 58 : OpCode.APPEND	[8192 datoshi]
    /// 59 : OpCode.DUP	[2 datoshi]
    /// 5A : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F	[1 datoshi]
    /// 63 : OpCode.APPEND	[8192 datoshi]
    /// 64 : OpCode.RET	[0 datoshi]
    /// </remarks>
    [DisplayName("test_Push_Integer_Internal")]
    public abstract IList<object>? Test_Push_Integer_Internal();

    #endregion
}
