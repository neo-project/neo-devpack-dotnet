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
    [DisplayName("test_External")]
    public abstract IList<object>? Test_External();
    // 0000 : NEWSTRUCT0
    // 0001 : DUP
    // 0002 : PUSHM1
    // 0003 : APPEND
    // 0004 : DUP
    // 0005 : PUSHINT32
    // 000A : APPEND
    // 000B : DUP
    // 000C : PUSHINT64
    // 0015 : APPEND
    // 0016 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test_Push_Integer")]
    public abstract BigInteger? Test_Push_Integer(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test_Push_Integer_Internal")]
    public abstract IList<object>? Test_Push_Integer_Internal();
    // 0000 : NEWSTRUCT0
    // 0001 : DUP
    // 0002 : PUSH0
    // 0003 : APPEND
    // 0004 : DUP
    // 0005 : PUSHINT16
    // 0008 : APPEND
    // 0009 : DUP
    // 000A : PUSHINT8
    // 000C : APPEND
    // 000D : DUP
    // 000E : PUSHINT8
    // 0010 : APPEND
    // 0011 : DUP
    // 0012 : PUSHINT16
    // 0015 : APPEND
    // 0016 : DUP
    // 0017 : PUSHINT16
    // 001A : APPEND
    // 001B : DUP
    // 001C : PUSHINT32
    // 0021 : APPEND
    // 0022 : DUP
    // 0023 : PUSHINT64
    // 002C : APPEND
    // 002D : DUP
    // 002E : PUSHINT32
    // 0033 : APPEND
    // 0034 : DUP
    // 0035 : PUSHINT32
    // 003A : APPEND
    // 003B : DUP
    // 003C : PUSHINT128
    // 004D : APPEND
    // 004E : DUP
    // 004F : PUSHINT64
    // 0058 : APPEND
    // 0059 : DUP
    // 005A : PUSHINT64
    // 0063 : APPEND
    // 0064 : RET

    #endregion

}
