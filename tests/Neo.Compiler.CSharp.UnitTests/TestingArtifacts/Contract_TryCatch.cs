using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_TryCatch(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_TryCatch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""try01"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""try02"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":92,""safe"":false},{""name"":""try03"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":101,""safe"":false},{""name"":""tryNest"",""parameters"":[{""name"":""throwInTry"",""type"":""Boolean""},{""name"":""throwInCatch"",""type"":""Boolean""},{""name"":""throwInFinally"",""type"":""Boolean""},{""name"":""enterOuterCatch"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":196,""safe"":false},{""name"":""throwInCatch"",""parameters"":[{""name"":""throwInTry"",""type"":""Boolean""},{""name"":""throwInCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":346,""safe"":false},{""name"":""tryFinally"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":403,""safe"":false},{""name"":""tryFinallyAndRethrow"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":487,""safe"":false},{""name"":""tryCatch"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":565,""safe"":false},{""name"":""tryWithTwoFinally"",""parameters"":[{""name"":""throwInInner"",""type"":""Boolean""},{""name"":""throwInOuter"",""type"":""Boolean""},{""name"":""enterInnerCatch"",""type"":""Boolean""},{""name"":""enterOuterCatch"",""type"":""Boolean""},{""name"":""enterInnerFinally"",""type"":""Boolean""},{""name"":""enterOuterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":645,""safe"":false},{""name"":""tryecpointCast"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":969,""safe"":false},{""name"":""tryvalidByteString2Ecpoint"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1106,""safe"":false},{""name"":""tryinvalidByteArray2UInt160"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1230,""safe"":false},{""name"":""tryvalidByteArray2UInt160"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1354,""safe"":false},{""name"":""tryinvalidByteArray2UInt256"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1465,""safe"":false},{""name"":""tryvalidByteArray2UInt256"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1601,""safe"":false},{""name"":""tryNULL2Ecpoint_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":1713,""safe"":false},{""name"":""tryNULL2Uint160_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":1873,""safe"":false},{""name"":""tryNULL2Uint256_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":2033,""safe"":false},{""name"":""tryNULL2Bytestring_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":2193,""safe"":false},{""name"":""throwCall"",""parameters"":[],""returntype"":""Any"",""offset"":184,""safe"":false},{""name"":""tryUncatchableException"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":2342,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP13CVcCAxBwOxYeEnB4Jg4MCWV4Y2VwdGlvbjo9QXF5JgQTcD05eiY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hAVwADenl4NJ5AVwIDEHA7DRUScHgmBTRGRT1BcXkmBBNwPTl6JjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aEAMCWV4Y2VwdGlvbjpXAgQQcDtWADsNGBJweCYFNORFPUZxE3B5JgU02UU9O3omBDTRaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFPz07cXsmNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT0CaEBXAgMQcDsWKhFweCYODAlleGNlcHRpb246PRxxEnB5Jg4MCWV4Y2VwdGlvbjo9CHomBBNwPxRwaEBXAQIQcDsAFhJweCYODAlleGNlcHRpb246PTl5JjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aEBXAQIQcDsAEBJweCYINcT+//9FPTl5JjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aEBXAgIQcDsQABJweCYINXb+//9FPTtxeSY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFPQJoQFcCBhBwPM8AAAAHAQAAO0d/aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFeCYODAlleGNlcHRpb246PXBxeiY0aBKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3A9OHwmNGgTnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wP3kmDgwJZXhjZXB0aW9uOj1wcXsmNGgUnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wPTh9JjRoFZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcD9oQFcCAxBwO0NLEnB4JgwMBgoLDA0ODyIlDCECRwDbLpDZ8CxPn8hiq6ypJyX5W0/dzI1/+lOGk+z0Y6lK2CQJSsoAISgDOnE9QXF5JgQTcD05eiY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hAVwICEHA7Nj4ScAwhAkcA2y6Q2fAsT5/IYqusqScl+VtP3cyNf/pThpPs9GOpStgkCUrKACEoAzpxPUFxeCYEE3A9OXkmNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oQFcCAxBwOzY+EnB4JgwMBgoLDA0ODyIYDBR+7hqr62ftHXkdROT1/POukXGocUrYJAlKygAUKAM6cT1BcXkmBBNwPTl6JjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aEBXAgIQcDspMRJwDBR+7hqr62ftHXkdROT1/POukXGocUrYJAlKygAUKAM6cT1BcXgmBBNwPTl5JjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aEBXAgMQcDtCShJweCYXDAYKCwwNDg9K2CQJSsoAICgDOiIkDCDtz4Z5EE7CkRpP4prX2yMqST5bmQ+x2nrwx7mJlIyJJXE9QXF5JgQTcD05eiY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hAVwICEHA7KjIScAwg7c+GeRBOwpEaT+Ka19sjKkk+W5kPsdp68Me5iZSMiSVxPUFxeCYEE3A9OXkmNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oQFcDAxBwACGI2yhK2CQJSsoAISgDOnE7DBQScHgmBAtxPXtyeSYEE3A9c3omNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWlyaguXJjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aWgSv0BXAwMQcAAUiNsoStgkCUrKABQoAzpxOwwUEnB4JgQLcT17cnkmBBNwPXN6JjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpcmoLlyY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2loEr9AVwMDEHAAIIjbKErYJAlKygAgKAM6cTsMFBJweCYEC3E9e3J5JgQTcD1zeiY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaXJqC5cmNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9paBK/QFcDAxBwDAMxMjNxOwwUEnB4JgQLcT17cnkmBBNwPXN6JjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpcmoLlyY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2loEr9AVwIDEHA7CxMScHgmAzg9QXF5JgQTcD05eiY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hA68zPMA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DGV4Y2VwdGlvbjo=
    /// 00 : OpCode.PUSHDATA1 657863657074696F6E
    /// 0B : OpCode.THROW
    /// </remarks>
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
}
