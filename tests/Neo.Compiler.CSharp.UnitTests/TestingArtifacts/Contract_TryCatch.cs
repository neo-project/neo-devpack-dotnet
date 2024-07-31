using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_TryCatch : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_TryCatch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""try01"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""try02"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":98,""safe"":false},{""name"":""try03"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":109,""safe"":false},{""name"":""tryNest"",""parameters"":[{""name"":""throwInTry"",""type"":""Boolean""},{""name"":""throwInCatch"",""type"":""Boolean""},{""name"":""throwInFinally"",""type"":""Boolean""},{""name"":""enterOuterCatch"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":211,""safe"":false},{""name"":""throwInCatch"",""parameters"":[{""name"":""throwInTry"",""type"":""Boolean""},{""name"":""throwInCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":368,""safe"":false},{""name"":""tryFinally"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":435,""safe"":false},{""name"":""tryFinallyAndRethrow"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":523,""safe"":false},{""name"":""tryCatch"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":605,""safe"":false},{""name"":""tryWithTwoFinally"",""parameters"":[{""name"":""throwInInner"",""type"":""Boolean""},{""name"":""throwInOuter"",""type"":""Boolean""},{""name"":""enterInnerCatch"",""type"":""Boolean""},{""name"":""enterOuterCatch"",""type"":""Boolean""},{""name"":""enterInnerFinally"",""type"":""Boolean""},{""name"":""enterOuterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":689,""safe"":false},{""name"":""tryecpointCast"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1029,""safe"":false},{""name"":""tryvalidByteString2Ecpoint"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1131,""safe"":false},{""name"":""tryinvalidByteArray2UInt160"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1227,""safe"":false},{""name"":""tryvalidByteArray2UInt160"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1329,""safe"":false},{""name"":""tryinvalidByteArray2UInt256"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1425,""safe"":false},{""name"":""tryvalidByteArray2UInt256"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1527,""safe"":false},{""name"":""tryNULL2Ecpoint_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":1612,""safe"":false},{""name"":""tryNULL2Uint160_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":1800,""safe"":false},{""name"":""tryNULL2Uint256_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":1988,""safe"":false},{""name"":""tryNULL2Bytestring_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":2176,""safe"":false},{""name"":""throwCall"",""parameters"":[],""returntype"":""Any"",""offset"":198,""safe"":false},{""name"":""tryUncatchableException"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":2339,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2426,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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"));

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
