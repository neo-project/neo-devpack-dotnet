using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGNXAAF4QAP/////////fwMAAAAAAAAAgAT//////////wAAAAAAAAAAAv///38CAAAAgAP/////AAAAAAL//wAAAf9/AQCAAH8AgAH/ABAdv0ADAPBaKxf///8CwL3w/w8Tv0C5P7Cr").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AwDwWisX////AsC98P8PE79A
    /// 00 : PUSHINT64 00F05A2B17FFFFFF [1 datoshi]
    /// 09 : PUSHINT32 C0BDF0FF [1 datoshi]
    /// 0E : PUSHM1 [1 datoshi]
    /// 0F : PUSH3 [1 datoshi]
    /// 10 : PACKSTRUCT [2048 datoshi]
    /// 11 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_External")]
    public abstract IList<object>? Test_External();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_Push_Integer")]
    public abstract BigInteger? Test_Push_Integer(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: A/////////9/AwAAAAAAAACABP//////////AAAAAAAAAAAC////fwIAAACAA/////8AAAAAAv//AAAB/38BAIAAfwCAAf8AEB2/QA==
    /// 00 : PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// 09 : PUSHINT64 0000000000000080 [1 datoshi]
    /// 12 : PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// 23 : PUSHINT32 FFFFFF7F [1 datoshi]
    /// 28 : PUSHINT32 00000080 [1 datoshi]
    /// 2D : PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// 36 : PUSHINT32 FFFF0000 [1 datoshi]
    /// 3B : PUSHINT16 FF7F [1 datoshi]
    /// 3E : PUSHINT16 0080 [1 datoshi]
    /// 41 : PUSHINT8 7F [1 datoshi]
    /// 43 : PUSHINT8 80 [1 datoshi]
    /// 45 : PUSHINT16 FF00 [1 datoshi]
    /// 48 : PUSH0 [1 datoshi]
    /// 49 : PUSH13 [1 datoshi]
    /// 4A : PACKSTRUCT [2048 datoshi]
    /// 4B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_Push_Integer_Internal")]
    public abstract IList<object>? Test_Push_Integer_Internal();

    #endregion
}
