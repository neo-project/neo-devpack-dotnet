using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Enum"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testEnumParse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""testEnumParseIgnoreCase"",""parameters"":[{""name"":""value"",""type"":""String""},{""name"":""ignoreCase"",""type"":""Boolean""}],""returntype"":""Any"",""offset"":87,""safe"":false},{""name"":""testEnumTryParse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Boolean"",""offset"":241,""safe"":false},{""name"":""testEnumTryParseIgnoreCase"",""parameters"":[{""name"":""value"",""type"":""String""},{""name"":""ignoreCase"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":319,""safe"":false},{""name"":""testEnumGetNames"",""parameters"":[],""returntype"":""Array"",""offset"":475,""safe"":false},{""name"":""testEnumGetValues"",""parameters"":[],""returntype"":""Array"",""offset"":521,""safe"":false},{""name"":""testEnumIsDefined"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":546,""safe"":false},{""name"":""testEnumIsDefinedByName"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""Boolean"",""offset"":591,""safe"":false},{""name"":""testEnumGetName"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""String"",""offset"":657,""safe"":false},{""name"":""testEnumGetNameWithType"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""String"",""offset"":709,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":775,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0KA1cAAQwIVGVzdEVudW14SgwGVmFsdWUxlyYHEVNFRUBKDAZWYWx1ZTKXJgcSU0VFQEoMBlZhbHVlM5cmBxNTRUVARQwSTm8gc3VjaCBlbnVtIHZhbHVlOlcAAgwIVGVzdEVudW14eSYuDAAQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhKeSY0DAZWQUxVRTGXJgdFRUURQEp5JiAMBlZBTFVFMpcmB0VFRRJASnkmDAwGVkFMVUUzIgoMBlZhbHVlM5cmB0VFRRNARUUMEk5vIHN1Y2ggZW51bSB2YWx1ZTpXAAELYAwIVGVzdEVudW14WEVKDAZWYWx1ZTGXJghFRRFgCEBKDAZWYWx1ZTKXJghFRRJgCEBKDAZWYWx1ZTOXJghFRRNgCEBFRRBgCUBXAAILYQwIVGVzdEVudW14eVl5Ji4MABBKeMq1JiJKeFDOSgBhAHu7JAlRUItQnCLpAGGfAEGeUVCLUJwi3EXbKEp5JjwMBlZBTFVFMZcmC0VFRUVFEWEIQEp5JiQMBlZBTFVFMpcmC0VFRUVFEmEIQEp5JgwMBlZBTFVFMyIKDAZWYWx1ZTOXJgtFRUVFRRNhCEBFRUVFRRBhCUAMCFRlc3RFbnVtE8NKEAwGVmFsdWUx0EoRDAZWYWx1ZTLQShIMBlZhbHVlM9BADAhUZXN0RW51bRPDShAR0EoREtBKEhPQQFcAAQwIVGVzdEVudW14ShGXJgZFRQhAShKXJgZFRQhAShOXJgZFRQhARUUJQFcAAQwIVGVzdEVudW14SgwGVmFsdWUxlyYGRUUIQEoMBlZhbHVlMpcmBkVFCEBKDAZWYWx1ZTOXJgZFRQhARUUJQFcAAXhKEZcmDEUMBlZhbHVlMUBKEpcmDEUMBlZhbHVlMkBKE5cmDEUMBlZhbHVlM0BFC0BXAAEMCFRlc3RFbnVteEoRlyYNRUUMBlZhbHVlMUBKEpcmDUVFDAZWYWx1ZTJAShOXJg1FRQwGVmFsdWUzQEVFC0BWAkCtEllC"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumGetName")]
    public abstract string? TestEnumGetName(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumGetNames")]
    public abstract IList<object>? TestEnumGetNames();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumGetNameWithType")]
    public abstract string? TestEnumGetNameWithType(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumGetValues")]
    public abstract IList<object>? TestEnumGetValues();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumIsDefined")]
    public abstract bool? TestEnumIsDefined(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumIsDefinedByName")]
    public abstract bool? TestEnumIsDefinedByName(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumParse")]
    public abstract object? TestEnumParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumParseIgnoreCase")]
    public abstract object? TestEnumParseIgnoreCase(string? value, bool? ignoreCase);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumTryParse")]
    public abstract bool? TestEnumTryParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumTryParseIgnoreCase")]
    public abstract bool? TestEnumTryParseIgnoreCase(string? value, bool? ignoreCase);

    #endregion

}
