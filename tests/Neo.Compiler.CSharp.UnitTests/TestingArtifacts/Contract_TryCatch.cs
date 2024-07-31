using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_TryCatch : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_TryCatch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""try01"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""try02"",""parameters"":[],""returntype"":""Integer"",""offset"":77,""safe"":false},{""name"":""try03"",""parameters"":[],""returntype"":""Integer"",""offset"":166,""safe"":false},{""name"":""tryNest"",""parameters"":[],""returntype"":""Integer"",""offset"":259,""safe"":false},{""name"":""throwInCatch"",""parameters"":[],""returntype"":""Integer"",""offset"":404,""safe"":false},{""name"":""codeAfterFinally"",""parameters"":[],""returntype"":""Integer"",""offset"":462,""safe"":false},{""name"":""tryFinally"",""parameters"":[],""returntype"":""Integer"",""offset"":496,""safe"":false},{""name"":""tryFinallyAndRethrow"",""parameters"":[],""returntype"":""Integer"",""offset"":566,""safe"":false},{""name"":""tryCatch"",""parameters"":[],""returntype"":""Integer"",""offset"":642,""safe"":false},{""name"":""tryWithTwoFinally"",""parameters"":[],""returntype"":""Integer"",""offset"":720,""safe"":false},{""name"":""tryecpointCast"",""parameters"":[],""returntype"":""Integer"",""offset"":1012,""safe"":false},{""name"":""tryvalidByteString2Ecpoint"",""parameters"":[],""returntype"":""Integer"",""offset"":1102,""safe"":false},{""name"":""tryinvalidByteArray2UInt160"",""parameters"":[],""returntype"":""Integer"",""offset"":1192,""safe"":false},{""name"":""tryvalidByteArray2UInt160"",""parameters"":[],""returntype"":""Integer"",""offset"":1282,""safe"":false},{""name"":""tryinvalidByteArray2UInt256"",""parameters"":[],""returntype"":""Integer"",""offset"":1372,""safe"":false},{""name"":""tryvalidByteArray2UInt256"",""parameters"":[],""returntype"":""Integer"",""offset"":1462,""safe"":false},{""name"":""tryNULL2Ecpoint_1"",""parameters"":[],""returntype"":""Array"",""offset"":1541,""safe"":false},{""name"":""tryNULL2Uint160_1"",""parameters"":[],""returntype"":""Array"",""offset"":1717,""safe"":false},{""name"":""tryNULL2Uint256_1"",""parameters"":[],""returntype"":""Array"",""offset"":1893,""safe"":false},{""name"":""tryNULL2Bytestring_1"",""parameters"":[],""returntype"":""Array"",""offset"":2069,""safe"":false},{""name"":""throwcall"",""parameters"":[],""returntype"":""Any"",""offset"":246,""safe"":false},{""name"":""tryUncatchableException"",""parameters"":[],""returntype"":""Integer"",""offset"":2220,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2298,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1kCVcCABBwOwkQEkpwRT09cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwIAEHA7FRwSSnBFDAlleGNlcHRpb246PT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAgAQcDsMExJKcEU0REU9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQAwJZXhjZXB0aW9uOkBXAgAQcDtSADsMFhJKcEU05EU9Q3ETSnBFNNpFPTk01UVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/PThxaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFPQJoIgJAVwIAEHA7FSgRSnBFDAlleGNlcHRpb246PRpxEkpwRQwJZXhjZXB0aW9uOj0HE0pwRT8USnBFaCICQFcCABBwOwkQEUpwRT0OcRJKcEU9BxNKcEU/FEpwRWgiAkBXAQAQcDsACRJKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwEAEHA7AA8SSnBFNbT+//9FPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcCABBwOw8AEkpwRTVo/v//RT04cWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT0CaCICQFcCABBwPK8AAADmAAAAOzhvaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFPW5xaBKeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRT03aBOeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRT89bnFoFJ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFPTdoFZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfSnBFP2giAkBXAgAQcDsWHRJKcEVYStgkCUrKACEoAzpxPT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAgAQcDsWHRJKcEVZStgkCUrKACEoAzpxPT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAgAQcDsWHRJKcEVYStgkCUrKABQoAzpxPT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAgAQcDsWHRJKcEVaStgkCUrKABQoAzpxPT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAgAQcDsWHRJKcEVYStgkCUrKACAoAzpxPT1xE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAgAQcDsLEhJKcEVbcT09cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwMAEHAAIYjbKErYJAlKygAhKAM6cTsNFBJKcEULSnFFPXdyE0pwRT1waEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaXJqC5cmNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT/FSmjPSmnPIgJA2yhK2CQJSsoAISgDOkBXAwAQcAAUiNsoStgkCUrKABQoAzpxOw0UEkpwRQtKcUU9d3ITSnBFPXBoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpcmoLlyY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP8VKaM9Kac8iAkDbKErYJAlKygAUKAM6QFcDABBwACCI2yhK2CQJSsoAICgDOnE7DRQSSnBFC0pxRT13chNKcEU9cGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWlyaguXJjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/xUpoz0ppzyICQNsoStgkCUrKACAoAzpAVwMAEHAMAzEyM3E7DRQSSnBFC0pxRT13chNKcEU9cGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWlyaguXJjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/xUpoz0ppzyICQFcCABBwOwoREkpwRTg9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFYEDAYKCwwNDg9gDCECRwDbLpDZ8CxPn8hiq6ypJyX5W0/dzI1/+lOGk+z0Y6lhDBR+7hqr62ftHXkdROT1/POukXGocWIMIO3PhnkQTsKRGk/imtfbIypJPluZD7HaevDHuYmUjIklY0ApsISo"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("codeAfterFinally")]
    public abstract BigInteger? CodeAfterFinally();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("throwcall")]
    public abstract object? Throwcall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("throwInCatch")]
    public abstract BigInteger? ThrowInCatch();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try01")]
    public abstract BigInteger? Try01();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try02")]
    public abstract BigInteger? Try02();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try03")]
    public abstract BigInteger? Try03();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryCatch")]
    public abstract BigInteger? TryCatch();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryecpointCast")]
    public abstract BigInteger? TryecpointCast();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryFinally")]
    public abstract BigInteger? TryFinally();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryFinallyAndRethrow")]
    public abstract BigInteger? TryFinallyAndRethrow();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryinvalidByteArray2UInt160")]
    public abstract BigInteger? TryinvalidByteArray2UInt160();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryinvalidByteArray2UInt256")]
    public abstract BigInteger? TryinvalidByteArray2UInt256();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNest")]
    public abstract BigInteger? TryNest();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Bytestring_1")]
    public abstract IList<object>? TryNULL2Bytestring_1();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Ecpoint_1")]
    public abstract IList<object>? TryNULL2Ecpoint_1();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Uint160_1")]
    public abstract IList<object>? TryNULL2Uint160_1();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNULL2Uint256_1")]
    public abstract IList<object>? TryNULL2Uint256_1();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryUncatchableException")]
    public abstract BigInteger? TryUncatchableException();

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
    public abstract BigInteger? TryWithTwoFinally();

    #endregion

    #region Constructor for internal use only

    protected Contract_TryCatch(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
