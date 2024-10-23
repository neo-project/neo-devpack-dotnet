using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Stack(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Stack"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test_Push_Integer"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""test_Push_Integer_Internal"",""parameters"":[],""returntype"":""Array"",""offset"":5,""safe"":false},{""name"":""test_External"",""parameters"":[],""returntype"":""Array"",""offset"":81,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGNXAAF4QAP/////////fwMAAAAAAAAAgAT//////////wAAAAAAAAAAAv///38CAAAAgAP/////AAAAAAL//wAAAf9/AQCAAH8AgAH/ABAdv0ADAPBaKxf///8CwL3w/w8Tv0C5P7Cr"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AwDwWisX////AsC98P8PE79A
    /// 00 : OpCode.PUSHINT64 00F05A2B17FFFFFF
    /// 09 : OpCode.PUSHINT32 C0BDF0FF
    /// 0E : OpCode.PUSHM1
    /// 0F : OpCode.PUSH3
    /// 10 : OpCode.PACKSTRUCT
    /// 11 : OpCode.RET
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
    /// Script: A/////////9/AwAAAAAAAACABP//////////AAAAAAAAAAAC////fwIAAACAA/////8AAAAAAv//AAAB/38BAIAAfwCAAf8AEB2/QA==
    /// 00 : OpCode.PUSHINT64 FFFFFFFFFFFFFF7F
    /// 09 : OpCode.PUSHINT64 0000000000000080
    /// 12 : OpCode.PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000
    /// 23 : OpCode.PUSHINT32 FFFFFF7F
    /// 28 : OpCode.PUSHINT32 00000080
    /// 2D : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 36 : OpCode.PUSHINT32 FFFF0000
    /// 3B : OpCode.PUSHINT16 FF7F
    /// 3E : OpCode.PUSHINT16 0080
    /// 41 : OpCode.PUSHINT8 7F
    /// 43 : OpCode.PUSHINT8 80
    /// 45 : OpCode.PUSHINT16 FF00
    /// 48 : OpCode.PUSH0
    /// 49 : OpCode.PUSH13
    /// 4A : OpCode.PACKSTRUCT
    /// 4B : OpCode.RET
    /// </remarks>
    [DisplayName("test_Push_Integer_Internal")]
    public abstract IList<object>? Test_Push_Integer_Internal();

    #endregion
}
