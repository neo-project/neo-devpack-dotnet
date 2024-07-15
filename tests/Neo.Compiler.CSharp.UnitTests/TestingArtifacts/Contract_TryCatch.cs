using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_TryCatch : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_TryCatch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""try01"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""try02"",""parameters"":[],""returntype"":""Integer"",""offset"":77,""safe"":false},{""name"":""try03"",""parameters"":[],""returntype"":""Integer"",""offset"":164,""safe"":false},{""name"":""tryNest"",""parameters"":[],""returntype"":""Integer"",""offset"":253,""safe"":false},{""name"":""throwInCatch"",""parameters"":[],""returntype"":""Integer"",""offset"":337,""safe"":false},{""name"":""tryFinally"",""parameters"":[],""returntype"":""Integer"",""offset"":383,""safe"":false},{""name"":""tryFinallyAndRethrow"",""parameters"":[],""returntype"":""Integer"",""offset"":453,""safe"":false},{""name"":""tryCatch"",""parameters"":[],""returntype"":""Integer"",""offset"":526,""safe"":false},{""name"":""tryWithTwoFinally"",""parameters"":[],""returntype"":""Integer"",""offset"":601,""safe"":false},{""name"":""tryecpointCast"",""parameters"":[],""returntype"":""Integer"",""offset"":893,""safe"":false},{""name"":""tryvalidByteString2Ecpoint"",""parameters"":[],""returntype"":""Integer"",""offset"":983,""safe"":false},{""name"":""tryinvalidByteArray2UInt160"",""parameters"":[],""returntype"":""Integer"",""offset"":1073,""safe"":false},{""name"":""tryvalidByteArray2UInt160"",""parameters"":[],""returntype"":""Integer"",""offset"":1163,""safe"":false},{""name"":""tryinvalidByteArray2UInt256"",""parameters"":[],""returntype"":""Integer"",""offset"":1253,""safe"":false},{""name"":""tryvalidByteArray2UInt256"",""parameters"":[],""returntype"":""Integer"",""offset"":1343,""safe"":false},{""name"":""tryNULL2Ecpoint_1"",""parameters"":[],""returntype"":""Array"",""offset"":1422,""safe"":false},{""name"":""tryNULL2Uint160_1"",""parameters"":[],""returntype"":""Array"",""offset"":1584,""safe"":false},{""name"":""tryNULL2Uint256_1"",""parameters"":[],""returntype"":""Array"",""offset"":1746,""safe"":false},{""name"":""tryNULL2Bytestring_1"",""parameters"":[],""returntype"":""Array"",""offset"":1908,""safe"":false},{""name"":""throwcall"",""parameters"":[],""returntype"":""Any"",""offset"":241,""safe"":false},{""name"":""tryUncatchableException"",""parameters"":[],""returntype"":""Integer"",""offset"":2059,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":2135,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3BCFcCABBwOwkQEkpwRT09cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwIAEHA7ExoSSnBFDAlleGNlcHRpb246cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwIAEHA7CRASSnBFNEFxE0pwRT02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkAMCWV4Y2VwdGlvbjpXAgAQcDsVADsJEBJKcEU05XETSnBFNN403HFoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU9AmgiAkBXAgAQcDsTJBFKcEUMCWV4Y2VwdGlvbjpxEkpwRQwJZXhjZXB0aW9uOhNKcEU/VwEAEHA7AAkSSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcBABBwOwAMEkpwRTUg////aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2giAkBXAgAQcDsMABJKcEU11/7//3FoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU9AmgiAkBXAgAQcDyvAAAA5gAAADs4b2hKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT1ucWgSnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcEU9N2gTnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KcEU/PW5xaBSeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRT03aBWeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0pwRT9oIgJAVwIAEHA7Fh0SSnBFWErYJAlKygAhKAM6cT09cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwIAEHA7Fh0SSnBFWUrYJAlKygAhKAM6cT09cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwIAEHA7Fh0SSnBFWErYJAlKygAUKAM6cT09cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwIAEHA7Fh0SSnBFWkrYJAlKygAUKAM6cT09cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwIAEHA7Fh0SSnBFWErYJAlKygAgKAM6cT09cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVwIAEHA7CxISSnBFW3E9PXETSnBFPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aCICQFcDABBwACGI2yhK2CQJSsoAISgDOnE7DRQSSnBFC0pxRT13chNKcEU9cGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWlyaguXJjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/xUpoz0ppzyICQFcDABBwABSI2yhK2CQJSsoAFCgDOnE7DRQSSnBFC0pxRT13chNKcEU9cGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWlyaguXJjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/xUpoz0ppzyICQFcDABBwACCI2yhK2CQJSsoAICgDOnE7DRQSSnBFC0pxRT13chNKcEU9cGhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWlyaguXJjVoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/xUpoz0ppzyICQFcDABBwDAMxMjNxOw0UEkpwRQtKcUU9d3ITSnBFPXBoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVpcmoLlyY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP8VKaM9Kac8iAkBXAgAQcDsIDxJKcEU4cRNKcEU9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oIgJAVgQMBgoLDA0OD2AMIQJHANsukNnwLE+fyGKrrKknJflbT93MjX/6U4aT7PRjqWEMFH7uGqvrZ+0deR1E5PX8866RcahxYgwg7c+GeRBOwpEaT+Ka19sjKkk+W5kPsdp68Me5iZSMiSVjQBFmcrM="));

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
