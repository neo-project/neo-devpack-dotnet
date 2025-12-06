using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NullConditional(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NullConditional"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""assignChild"",""parameters"":[{""name"":""createNode"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""assignSibling"",""parameters"":[{""name"":""createNode"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":54,""safe"":false},{""name"":""assignStatic"",""parameters"":[{""name"":""createNode"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":110,""safe"":false},{""name"":""assignGrandChild"",""parameters"":[{""name"":""createRoot"",""type"":""Boolean""},{""name"":""createChild"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":175,""safe"":false},{""name"":""assignSiblingFromOther"",""parameters"":[{""name"":""seedLeft"",""type"":""Boolean""},{""name"":""seedRight"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":246,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":327,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""Basic""}}}");

    /// <summary>
    /// Optimization: "Basic"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM05lby5Db21waWxlci5DU2hhcnAgMy44LjErNzljNTY3MDY4NGM1YjUyYTQyZWJhYjMyZjgxNWU1ODRlMzUuLi4AAAAAAP1MAVcEAXgmCAsLEsAiAwtwCwsSwHJoStgkC3NqaxBR0GoiBEULcWlyaguXJgUQIgMRIgJAEFHQQFcDAXgmCAsLEsAiAwtwCwsSwHFoStgkC3JpahFR0GkiBEULRWhK2CQEEc5xaQuXJgUQIgMRIgJAVwIBeCYICwsSwCIDC0pgRQsLEsBwWErYJAtxaGkQUdBoIgRFC0VYStgkBBDOcGgLlyYFECIDESICQGBAWEAQzkBXBAJ4JhYLCxLAeSYICwsSwCIDC0sQUdAiAwtwaErYJBgLCxLAchDOStgkC3NqaxBR0GoiBEULcWlyaguXJgUQIgMRIgJAQFcFAngmCwsLEsALEsAiAwtweSYICwsSwCIDC3ELCxLAc2lK2CQLdGtsEVHQayIERQtzaErYJAt0a2wQUdBrIgRFC3Jqc2sLlyYFECIDESICQFYBC2BATujAeA==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("assignChild")]
    public abstract BigInteger? AssignChild(bool? createNode);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("assignGrandChild")]
    public abstract BigInteger? AssignGrandChild(bool? createRoot, bool? createChild);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("assignSibling")]
    public abstract BigInteger? AssignSibling(bool? createNode);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("assignSiblingFromOther")]
    public abstract BigInteger? AssignSiblingFromOther(bool? seedLeft, bool? seedRight);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("assignStatic")]
    public abstract BigInteger? AssignStatic(bool? createNode);

    #endregion
}
