using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Attribute : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Attribute"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test"",""parameters"":[],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""reentrantTest"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Void"",""offset"":334,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":305,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""base64Decode""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrAxiYXNlNjREZWNvZGUBAAEPAAD9WAFY2CYrCxHASlnPDBxBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUFBQUE9EU00CGBYNCgIQFcAAng0HHk3AADbMNsoStgkCUrKABQoAzpKeBBR0EVAVwABQFcAAXgQzkH4J+yMqiYODAlleGNlcHRpb246QFcAAlrYJiALCxLASlvPDA1yZWVudHJhbnRUZXN0Af8AEk00HmJaNEB5EJcmBCIMeQB7lyYGEHg0yVo1ZwAAAEBXAAN4NZ3///96SngRUdBFQZv2Z855EYhOEFHQUBLASngQUdBFQFcBAXgRzngQzsFFU4tQQZJd6DFwaAuXDA9BbHJlYWR5IGVudGVyZWThEXgRzngQzsFFU4tQQeY/GIRAVwABeBHOeBDOwUVTi1BBL1jF7UBXAAF4NANAVwABQFYECgAAAAAKHv///xLAYQrS////CpL///8SwGNAwko01iMf////QGEEcMU="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("reentrantTest")]
    public abstract void ReentrantTest(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test")]
    public abstract bool? Test();

    #endregion

    #region Constructor for internal use only

    protected Contract_Attribute(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
