using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_TryCatch : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_TryCatch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""try01"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""try02"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":92,""safe"":false},{""name"":""try03"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":184,""safe"":false},{""name"":""tryNest"",""parameters"":[{""name"":""throwInTry"",""type"":""Boolean""},{""name"":""throwInCatch"",""type"":""Boolean""},{""name"":""throwInFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":278,""safe"":false},{""name"":""throwInCatch"",""parameters"":[{""name"":""throwInTry"",""type"":""Boolean""},{""name"":""throwInCatch"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":429,""safe"":false},{""name"":""tryFinally"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":485,""safe"":false},{""name"":""tryFinallyAndRethrow"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":570,""safe"":false},{""name"":""tryCatch"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":648,""safe"":false},{""name"":""tryWithTwoFinally"",""parameters"":[{""name"":""throwInInner"",""type"":""Boolean""},{""name"":""throwInOuter"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":728,""safe"":false},{""name"":""tryecpointCast"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1056,""safe"":false},{""name"":""tryvalidByteString2Ecpoint"",""parameters"":[],""returntype"":""Integer"",""offset"":1152,""safe"":false},{""name"":""tryinvalidByteArray2UInt160"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1242,""safe"":false},{""name"":""tryvalidByteArray2UInt160"",""parameters"":[],""returntype"":""Integer"",""offset"":1338,""safe"":false},{""name"":""tryinvalidByteArray2UInt256"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1428,""safe"":false},{""name"":""tryvalidByteArray2UInt256"",""parameters"":[],""returntype"":""Integer"",""offset"":1524,""safe"":false},{""name"":""tryNULL2Ecpoint_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":1603,""safe"":false},{""name"":""tryNULL2Uint160_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":1768,""safe"":false},{""name"":""tryNULL2Uint256_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":1933,""safe"":false},{""name"":""tryNULL2Bytestring_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":2098,""safe"":false},{""name"":""throwcall"",""parameters"":[],""returntype"":""Any"",""offset"":266,""safe"":false},{""name"":""tryUncatchableException"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":2252,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2333,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2HCVcCARBwOxgfEkpwRXgmDgwJZXhjZXB0aW9uOj09cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwIBEHA7GB8SSnBFeCYODAlleGNlcHRpb246PT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAgEQcDsOFRJKcEV4JgQ0Qz09cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJADAlleGNlcHRpb246VwIDEHA7WAA7DhoSSnBFeCYENOI9R3ETSnBFeSYENNY9O3omBDTPaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFPz04cWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT0CaCICQFcCAhBwOxguEUpwRXgmDgwJZXhjZXB0aW9uOj0AcRJKcEV5Jg4MCWV4Y2VwdGlvbjo9ABNKcEU/VwEBEHA7ABgSSnBFeCYODAlleGNlcHRpb246PTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcBARBwOwAREkpwRXgmBzXB/v//PTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCARBwOxEAEkpwRXgmBzVz/v//PThxaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFPQJoIgJAVwICEHA80wAAAAoBAAA8TQAAAIQAAABoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEV4Jg4MCWV4Y2VwdGlvbjo9bnFoEp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFPTdoE55KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFP3kmDgwJZXhjZXB0aW9uOj1ucWgUnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcEU9N2gVnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcEU/aCICQFcCARBwOxwjEkpwRXgmBVgiA1lK2CQJSsoAISgDOnE9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCABBwOxYdEkpwRVlK2CQJSsoAISgDOnE9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCARBwOxwjEkpwRXgmBVgiA1pK2CQJSsoAFCgDOnE9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCABBwOxYdEkpwRVpK2CQJSsoAFCgDOnE9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCARBwOxwjEkpwRXgmEFhK2CQJSsoAICgDOiIDW3E9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCABBwOwsSEkpwRVtxPT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAwEQcAAhiNsoStgkCUrKACEoAzpxOxAXEkpwRXgmBgtKcUU9d3ITSnBFPXBoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpcmoLlyY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP8VKaM9Kac8iAkBXAwEQcAAUiNsoStgkCUrKABQoAzpxOxAXEkpwRXgmBgtKcUU9d3ITSnBFPXBoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpcmoLlyY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP8VKaM9Kac8iAkBXAwEQcAAgiNsoStgkCUrKACAoAzpxOxAXEkpwRXgmBgtKcUU9d3ITSnBFPXBoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpcmoLlyY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP8VKaM9Kac8iAkBXAwEQcAwDMTIzcTsQFxJKcEV4JgYLSnFFPXdyE0pwRT1waEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaXJqC5cmNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT/FSmjPSmnPIgJAVwIBEHA7DRQSSnBFeCYDOD09cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVgQMBgoLDA0OD2AMIQJHANsukNnwLE+fyGKrrKknJflbT93MjX/6U4aT7PRjqWEMFH7uGqvrZ+0deR1E5PX8866RcahxYgwg7c+GeRBOwpEaT+Ka19sjKkk+W5kPsdp68Me5iZSMiSVjQGCUxMU="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("throwcall")]
    public abstract object? Throwcall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("throwInCatch")]
    public abstract BigInteger? ThrowInCatch(bool? throwInTry, bool? throwInCatch);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try01")]
    public abstract BigInteger? Try01(bool? throwException);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try02")]
    public abstract BigInteger? Try02(bool? throwException);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try03")]
    public abstract BigInteger? Try03(bool? throwException);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryCatch")]
    public abstract BigInteger? TryCatch(bool? throwException);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryecpointCast")]
    public abstract BigInteger? TryecpointCast(bool? useInvalidECpoint);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryFinally")]
    public abstract BigInteger? TryFinally(bool? throwException);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryFinallyAndRethrow")]
    public abstract BigInteger? TryFinallyAndRethrow(bool? throwException);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryinvalidByteArray2UInt160")]
    public abstract BigInteger? TryinvalidByteArray2UInt160(bool? useInvalidECpoint);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryinvalidByteArray2UInt256")]
    public abstract BigInteger? TryinvalidByteArray2UInt256(bool? useInvalidECpoint);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNest")]
    public abstract BigInteger? TryNest(bool? throwInTry, bool? throwInCatch, bool? throwInFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Bytestring_1")]
    public abstract IList<object>? TryNULL2Bytestring_1(bool? setToNull);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Ecpoint_1")]
    public abstract IList<object>? TryNULL2Ecpoint_1(bool? setToNull);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Uint160_1")]
    public abstract IList<object>? TryNULL2Uint160_1(bool? setToNull);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Uint256_1")]
    public abstract IList<object>? TryNULL2Uint256_1(bool? setToNull);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryUncatchableException")]
    public abstract BigInteger? TryUncatchableException(bool? throwException);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryvalidByteArray2UInt160")]
    public abstract BigInteger? TryvalidByteArray2UInt160();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryvalidByteArray2UInt256")]
    public abstract BigInteger? TryvalidByteArray2UInt256();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryvalidByteString2Ecpoint")]
    public abstract BigInteger? TryvalidByteString2Ecpoint();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryWithTwoFinally")]
    public abstract BigInteger? TryWithTwoFinally(bool? throwInInner, bool? throwInOuter);

    #endregion

    #region Constructor for internal use only

    protected Contract_TryCatch(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
