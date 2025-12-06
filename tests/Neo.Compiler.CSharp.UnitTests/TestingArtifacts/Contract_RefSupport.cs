using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_RefSupport(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_RefSupport"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""incrementRefLocal"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""incrementRefStaticField"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":67,""safe"":false},{""name"":""incrementRefInstanceField"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":87,""safe"":false},{""name"":""produceOutValue"",""parameters"":[],""returntype"":""Integer"",""offset"":119,""safe"":false},{""name"":""produceOutStaticField"",""parameters"":[],""returntype"":""Integer"",""offset"":137,""safe"":false},{""name"":""produceOutInstanceField"",""parameters"":[],""returntype"":""Integer"",""offset"":154,""safe"":false},{""name"":""swapDigits"",""parameters"":[{""name"":""first"",""type"":""Integer""},{""name"":""second"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":183,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":312,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy44LjErNzljNTY3MDY4NGM1YjUyYTQyZWJhYjMyZjgxNWU1ODRlMzUuLi4AAAAAAP09AVcAAXhKYDQGWCICQFcAAVhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9gRUBXAAF4SmFFWUpgSmE0vVhhWSICQFcBAXgRwHBoYloQzkpgSmM0pVhhWFoQUdBoEM4iAkBAEEpkNAZcIgJAVwABAHtKZEVAEEphRRBKZEphNO5cYVkiAkBXAQAQEcBwaGUQSmRKYzTYXGFcXRBR0GgQziICQFcAAnlKZwd4SmY0Z14aoEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9fB55KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfIgJAVwECXnBfB0pmRWhKZwdFQFYIEGFAZCZr2w==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("incrementRefInstanceField")]
    public abstract BigInteger? IncrementRefInstanceField(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("incrementRefLocal")]
    public abstract BigInteger? IncrementRefLocal(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("incrementRefStaticField")]
    public abstract BigInteger? IncrementRefStaticField(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("produceOutInstanceField")]
    public abstract BigInteger? ProduceOutInstanceField();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("produceOutStaticField")]
    public abstract BigInteger? ProduceOutStaticField();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("produceOutValue")]
    public abstract BigInteger? ProduceOutValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("swapDigits")]
    public abstract BigInteger? SwapDigits(BigInteger? first, BigInteger? second);

    #endregion
}
