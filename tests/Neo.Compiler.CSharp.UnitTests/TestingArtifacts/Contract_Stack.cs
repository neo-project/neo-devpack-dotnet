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
    /// PUSHINT64 00F05A2B17FFFFFF [1 datoshi]
    /// PUSHINT32 C0BDF0FF [1 datoshi]
    /// PUSHM1 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_External")]
    public abstract IList<object>? Test_External();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_Push_Integer")]
    public abstract BigInteger? Test_Push_Integer(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: A/////////9/AwAAAAAAAACABP//////////AAAAAAAAAAAC////fwIAAACAA/////8AAAAAAv//AAAB/38BAIAAfwCAAf8AEB2/QA==
    /// PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// PUSHINT16 FF7F [1 datoshi]
    /// PUSHINT16 0080 [1 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// PUSHINT8 80 [1 datoshi]
    /// PUSHINT16 FF00 [1 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSH13 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("test_Push_Integer_Internal")]
    public abstract IList<object>? Test_Push_Integer_Internal();

    #endregion
}
