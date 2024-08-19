using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_TryCatch : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_TryCatch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""try01"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""try02"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":144,""safe"":false},{""name"":""try03"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":156,""safe"":false},{""name"":""tryNest"",""parameters"":[{""name"":""throwInTry"",""type"":""Boolean""},{""name"":""throwInCatch"",""type"":""Boolean""},{""name"":""throwInFinally"",""type"":""Boolean""},{""name"":""enterOuterCatch"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":303,""safe"":false},{""name"":""throwInCatch"",""parameters"":[{""name"":""throwInTry"",""type"":""Boolean""},{""name"":""throwInCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":559,""safe"":false},{""name"":""tryFinally"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":624,""safe"":false},{""name"":""tryFinallyAndRethrow"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":758,""safe"":false},{""name"":""tryCatch"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":886,""safe"":false},{""name"":""tryWithTwoFinally"",""parameters"":[{""name"":""throwInInner"",""type"":""Boolean""},{""name"":""throwInOuter"",""type"":""Boolean""},{""name"":""enterInnerCatch"",""type"":""Boolean""},{""name"":""enterOuterCatch"",""type"":""Boolean""},{""name"":""enterInnerFinally"",""type"":""Boolean""},{""name"":""enterOuterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1016,""safe"":false},{""name"":""tryecpointCast"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1600,""safe"":false},{""name"":""tryvalidByteString2Ecpoint"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1748,""safe"":false},{""name"":""tryinvalidByteArray2UInt160"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1890,""safe"":false},{""name"":""tryvalidByteArray2UInt160"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":2038,""safe"":false},{""name"":""tryinvalidByteArray2UInt256"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":2180,""safe"":false},{""name"":""tryvalidByteArray2UInt256"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":2328,""safe"":false},{""name"":""tryNULL2Ecpoint_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":2459,""safe"":false},{""name"":""tryNULL2Uint160_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":2730,""safe"":false},{""name"":""tryNULL2Uint256_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":3001,""safe"":false},{""name"":""tryNULL2Bytestring_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":3272,""safe"":false},{""name"":""throwCall"",""parameters"":[],""returntype"":""Any"",""offset"":291,""safe"":false},{""name"":""tryUncatchableException"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":3532,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":3665,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("throwCall")]
    public abstract object? ThrowCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("throwInCatch")]
    public abstract BigInteger? ThrowInCatch(bool? throwInTry, bool? throwInCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try01")]
    public abstract BigInteger? Try01(bool? throwException, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try02")]
    public abstract BigInteger? Try02(bool? throwException, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try03")]
    public abstract BigInteger? Try03(bool? throwException, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryCatch")]
    public abstract BigInteger? TryCatch(bool? throwException, bool? enterCatch);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryecpointCast")]
    public abstract BigInteger? TryecpointCast(bool? useInvalidECpoint, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryFinally")]
    public abstract BigInteger? TryFinally(bool? throwException, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryFinallyAndRethrow")]
    public abstract BigInteger? TryFinallyAndRethrow(bool? throwException, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryinvalidByteArray2UInt160")]
    public abstract BigInteger? TryinvalidByteArray2UInt160(bool? useInvalidECpoint, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryinvalidByteArray2UInt256")]
    public abstract BigInteger? TryinvalidByteArray2UInt256(bool? useInvalidECpoint, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNest")]
    public abstract BigInteger? TryNest(bool? throwInTry, bool? throwInCatch, bool? throwInFinally, bool? enterOuterCatch);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Bytestring_1")]
    public abstract IList<object>? TryNULL2Bytestring_1(bool? setToNull, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Ecpoint_1")]
    public abstract IList<object>? TryNULL2Ecpoint_1(bool? setToNull, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Uint160_1")]
    public abstract IList<object>? TryNULL2Uint160_1(bool? setToNull, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Uint256_1")]
    public abstract IList<object>? TryNULL2Uint256_1(bool? setToNull, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryUncatchableException")]
    public abstract BigInteger? TryUncatchableException(bool? throwException, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryvalidByteArray2UInt160")]
    public abstract BigInteger? TryvalidByteArray2UInt160(bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryvalidByteArray2UInt256")]
    public abstract BigInteger? TryvalidByteArray2UInt256(bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryvalidByteString2Ecpoint")]
    public abstract BigInteger? TryvalidByteString2Ecpoint(bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryWithTwoFinally")]
    public abstract BigInteger? TryWithTwoFinally(bool? throwInInner, bool? throwInOuter, bool? enterInnerCatch, bool? enterOuterCatch, bool? enterInnerFinally, bool? enterOuterFinally);

    #endregion

    #region Constructor for internal use only

    protected Contract_TryCatch(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
