using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Stack : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Stack"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test_Push_Integer"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""test_Push_Integer_Internal"",""parameters"":[],""returntype"":""Array"",""offset"":5,""safe"":false},{""name"":""test_External"",""parameters"":[],""returntype"":""Array"",""offset"":174,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAPZXAAF4QMVKEM9KAf8Az0oAgM9KAH/PSgEAgM9KAf9/z0oF//8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADPSgX/////AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAM9KAgAAAIDPSgL///9/z0oF//////////8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADPSgMAAAAAAAAAgM9KA/////////9/z0DFSgXAvfD//////////////////////////////////////89KBQDwWisX////////////////////////////////////z0DRaMGG"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test_External")]
    public abstract IList<object>? Test_External();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test_Push_Integer")]
    public abstract BigInteger? Test_Push_Integer(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test_Push_Integer_Internal")]
    public abstract IList<object>? Test_Push_Integer_Internal();

    #endregion

    #region Constructor for internal use only

    protected Contract_Stack(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
