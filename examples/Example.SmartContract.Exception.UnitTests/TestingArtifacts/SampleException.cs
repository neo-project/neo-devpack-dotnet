using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleException(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleException"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""try01"",""parameters"":[],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""try02"",""parameters"":[],""returntype"":""Any"",""offset"":71,""safe"":false},{""name"":""try03"",""parameters"":[],""returntype"":""Any"",""offset"":152,""safe"":false},{""name"":""tryNest"",""parameters"":[],""returntype"":""Any"",""offset"":238,""safe"":false},{""name"":""tryFinally"",""parameters"":[],""returntype"":""Any"",""offset"":324,""safe"":false},{""name"":""tryFinallyAndRethrow"",""parameters"":[],""returntype"":""Any"",""offset"":390,""safe"":false},{""name"":""tryCatch"",""parameters"":[],""returntype"":""Any"",""offset"":462,""safe"":false},{""name"":""tryWithTwoFinally"",""parameters"":[],""returntype"":""Any"",""offset"":536,""safe"":false},{""name"":""tryecpointCast"",""parameters"":[],""returntype"":""Any"",""offset"":818,""safe"":false},{""name"":""tryvalidByteString2Ecpoint"",""parameters"":[],""returntype"":""Any"",""offset"":909,""safe"":false},{""name"":""tryinvalidByteArray2UInt160"",""parameters"":[],""returntype"":""Any"",""offset"":1027,""safe"":false},{""name"":""tryvalidByteArray2UInt160"",""parameters"":[],""returntype"":""Any"",""offset"":1118,""safe"":false},{""name"":""tryinvalidByteArray2UInt256"",""parameters"":[],""returntype"":""Any"",""offset"":1223,""safe"":false},{""name"":""tryvalidByteArray2UInt256"",""parameters"":[],""returntype"":""Any"",""offset"":1314,""safe"":false},{""name"":""tryNULL2Ecpoint_1"",""parameters"":[],""returntype"":""Array"",""offset"":1433,""safe"":false},{""name"":""tryNULL2Uint160_1"",""parameters"":[],""returntype"":""Array"",""offset"":1583,""safe"":false},{""name"":""tryNULL2Uint256_1"",""parameters"":[],""returntype"":""Array"",""offset"":1733,""safe"":false},{""name"":""tryNULL2Bytestring_1"",""parameters"":[],""returntype"":""Array"",""offset"":1883,""safe"":false},{""name"":""tryUncatchableException"",""parameters"":[],""returntype"":""Any"",""offset"":2022,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""core-dev"",""Description"":""A sample contract to demonstrate how to handle exception"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0sCFcCABBwOwcMEnA9O3ETcD02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hAVwIAEHA7ERYScAwJZXhjZXB0aW9uOnETcD02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hAVwIAEHA7Cg8ScDRART07cRNwPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aEAMCWV4Y2VwdGlvbjpXAgAQcDsZADsKEhJwNOdFPQxxE3A030U9BDTaPThxaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFPQJoQFcBABBwOwAHEnA9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oQFcBABBwOwANEnA1Uv///0U9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oQFcCABBwOw0AEnA1Cv///0U9OHFoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU9AmhAVwIAEHA8qwAAAOAAAAA7OG1oSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU9anFoEp5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcD01aBOeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3A/PWpxaBSeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3A9NWgVnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wP2hAVwIAEHA7GyAScAwGCgsMDQ4PStgkCUrKACEoAzpxPTtxE3A9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oQFcCABBwOzY7EnAMIQJHANsukNnwLE+fyGKrrKknJflbT93MjX/6U4aT7PRjqUrYJAlKygAhKAM6cT07cRNwPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aEBXAgAQcDsbIBJwDAYKCwwNDg9K2CQJSsoAFCgDOnE9O3ETcD02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hAVwIAEHA7KS4ScAwUfu4aq+tn7R15HUTk9fzzrpFxqHFK2CQJSsoAFCgDOnE9O3ETcD02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hAVwIAEHA7GyAScAwGCgsMDQ4PStgkCUrKACAoAzpxPTtxE3A9NmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9oQFcCABBwOzc8EnAMIO3PhnkQTsKRGk/imtfbIypJPluZD7HaevDHuYmUjIkl2yhK2CQJSsoAICgDOnE9O3ETcD02aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2hAVwMAEHAAIYjbKErYJAlKygAhKAM6cTsJDhJwC3E9dHITcD1vaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaXJq2CY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2loEr9AVwMAEHAAFIjbKErYJAlKygAUKAM6cTsJDhJwC3E9dHITcD1vaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaXJq2CY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2loEr9AVwMAEHAAIIjbKErYJAlKygAgKAM6cTsJDhJwC3E9dHITcD1vaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFaXJq2CY1aEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFP2loEr9AVwMAEHAMAzEyM3E7CQ4ScAtxPXRyE3A9b2hKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRWlyatgmNWhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRT9paBK/QFcCABBwOwYLEnA4cRNwPTZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEU/aEA7BE0q"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try01")]
    public abstract object? Try01();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try02")]
    public abstract object? Try02();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("try03")]
    public abstract object? Try03();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryCatch")]
    public abstract object? TryCatch();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryecpointCast")]
    public abstract object? TryecpointCast();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryFinally")]
    public abstract object? TryFinally();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryFinallyAndRethrow")]
    public abstract object? TryFinallyAndRethrow();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryinvalidByteArray2UInt160")]
    public abstract object? TryinvalidByteArray2UInt160();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryinvalidByteArray2UInt256")]
    public abstract object? TryinvalidByteArray2UInt256();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryNest")]
    public abstract object? TryNest();

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
    public abstract object? TryUncatchableException();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryvalidByteArray2UInt160")]
    public abstract object? TryvalidByteArray2UInt160();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryvalidByteArray2UInt256")]
    public abstract object? TryvalidByteArray2UInt256();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryvalidByteString2Ecpoint")]
    public abstract object? TryvalidByteString2Ecpoint();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("tryWithTwoFinally")]
    public abstract object? TryWithTwoFinally();

    #endregion
}
