using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_TryCatch : Neo.SmartContract.Testing.SmartContract, IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_TryCatch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""try01"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""try02"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":96,""safe"":false},{""name"":""try03"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":105,""safe"":false},{""name"":""tryNest"",""parameters"":[{""name"":""throwInTry"",""type"":""Boolean""},{""name"":""throwInCatch"",""type"":""Boolean""},{""name"":""throwInFinally"",""type"":""Boolean""},{""name"":""enterOuterCatch"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":204,""safe"":false},{""name"":""throwInCatch"",""parameters"":[{""name"":""throwInTry"",""type"":""Boolean""},{""name"":""throwInCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":358,""safe"":false},{""name"":""tryFinally"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":423,""safe"":false},{""name"":""tryFinallyAndRethrow"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":509,""safe"":false},{""name"":""tryCatch"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":589,""safe"":false},{""name"":""tryWithTwoFinally"",""parameters"":[{""name"":""throwInInner"",""type"":""Boolean""},{""name"":""throwInOuter"",""type"":""Boolean""},{""name"":""enterInnerCatch"",""type"":""Boolean""},{""name"":""enterOuterCatch"",""type"":""Boolean""},{""name"":""enterInnerFinally"",""type"":""Boolean""},{""name"":""enterOuterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":671,""safe"":false},{""name"":""tryecpointCast"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1009,""safe"":false},{""name"":""tryvalidByteString2Ecpoint"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1109,""safe"":false},{""name"":""tryinvalidByteArray2UInt160"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1203,""safe"":false},{""name"":""tryvalidByteArray2UInt160"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1303,""safe"":false},{""name"":""tryinvalidByteArray2UInt256"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1397,""safe"":false},{""name"":""tryvalidByteArray2UInt256"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1497,""safe"":false},{""name"":""tryNULL2Ecpoint_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":1580,""safe"":false},{""name"":""tryNULL2Uint160_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":1752,""safe"":false},{""name"":""tryNULL2Uint256_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":1924,""safe"":false},{""name"":""tryNULL2Bytestring_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":2096,""safe"":false},{""name"":""throwCall"",""parameters"":[],""returntype"":""Any"",""offset"":192,""safe"":false},{""name"":""tryUncatchableException"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":2257,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2342,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2QCVcCAxBwOxgiEkpwRXgmDgwJZXhjZXB0aW9uOj1DcXkmBhNKcEU9OXomNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oQFcAA3p5eDSaQFcCAxBwOw8ZEkpwRXgmBTRIRT1DcXkmBhNKcEU9OXomNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oQAwJZXhjZXB0aW9uOlcCBBBwO1oAOw8cEkpwRXgmBTTiRT1IcRNKcEV5JgU01UU9O3omBDTNaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFPz07cXsmNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT0CaEBXAgMQcDsYLhFKcEV4Jg4MCWV4Y2VwdGlvbjo9IHESSnBFeSYODAlleGNlcHRpb246PQp6JgYTSnBFPxRKcEVoQFcBAhBwOwAYEkpwRXgmDgwJZXhjZXB0aW9uOj05eSY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hAVwECEHA7ABISSnBFeCYINbT+//9FPTl5JjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aEBXAgIQcDsSABJKcEV4Jgg1ZP7//0U9O3F5JjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU9AmhAVwIGEHA82QAAABMBAAA8TQAAAIcAAABoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEV4Jg4MCWV4Y2VwdGlvbjo9dHF6JjZoEp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFPTp8JjZoE55KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFP3kmDgwJZXhjZXB0aW9uOj10cXsmNmgUnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcEU9On0mNmgVnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcEU/aEBXAgMQcDscJhJKcEV4JgVYIgNZStgkCUrKACEoAzpxPUNxeSYGE0pwRT05eiY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hAVwICEHA7FiASSnBFWUrYJAlKygAhKAM6cT1DcXgmBhNKcEU9OXkmNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oQFcCAxBwOxwmEkpwRXgmBVgiA1pK2CQJSsoAFCgDOnE9Q3F5JgYTSnBFPTl6JjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aEBXAgIQcDsWIBJKcEVaStgkCUrKABQoAzpxPUNxeCYGE0pwRT05eSY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hAVwIDEHA7HCYSSnBFeCYQWErYJAlKygAgKAM6IgNbcT1DcXkmBhNKcEU9OXomNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oQFcCAhBwOwsVEkpwRVtxPUNxeCYGE0pwRT05eSY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hAVwMDEHAAIYjbKErYJAlKygAhKAM6cTsTHRJKcEV4JgYLSnFFPoAAAAByeSYGE0pwRT1zeiY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaXJqC5cmNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT/FSmjPSmnPQFcDAxBwABSI2yhK2CQJSsoAFCgDOnE7Ex0SSnBFeCYGC0pxRT6AAAAAcnkmBhNKcEU9c3omNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWlyaguXJjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/xUpoz0ppz0BXAwMQcAAgiNsoStgkCUrKACAoAzpxOxMdEkpwRXgmBgtKcUU+gAAAAHJ5JgYTSnBFPXN6JjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpcmoLlyY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP8VKaM9Kac9AVwMDEHAMAzEyM3E7Ex0SSnBFeCYGC0pxRT6AAAAAcnkmBhNKcEU9c3omNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWlyaguXJjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/xUpoz0ppz0BXAgMQcDsNFxJKcEV4JgM4PUNxeSYGE0pwRT05eiY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hAVgQMBgoLDA0OD2AMIQJHANsukNnwLE+fyGKrrKknJflbT93MjX/6U4aT7PRjqWEMFH7uGqvrZ+0deR1E5PX8866RcahxYgwg7c+GeRBOwpEaT+Ka19sjKkk+W5kPsdp68Me5iZSMiSVjQHIciSc="));

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
