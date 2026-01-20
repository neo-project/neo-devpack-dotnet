using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Enum"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testEnumParse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""testEnumParseIgnoreCase"",""parameters"":[{""name"":""value"",""type"":""String""},{""name"":""ignoreCase"",""type"":""Boolean""}],""returntype"":""Any"",""offset"":87,""safe"":false},{""name"":""testEnumTryParse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Boolean"",""offset"":241,""safe"":false},{""name"":""testEnumTryParseIgnoreCase"",""parameters"":[{""name"":""value"",""type"":""String""},{""name"":""ignoreCase"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":317,""safe"":false},{""name"":""testEnumGetNames"",""parameters"":[],""returntype"":""Array"",""offset"":461,""safe"":false},{""name"":""testEnumGetValues"",""parameters"":[],""returntype"":""Array"",""offset"":498,""safe"":false},{""name"":""testEnumIsDefined"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":514,""safe"":false},{""name"":""testEnumIsDefinedByName"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""Boolean"",""offset"":559,""safe"":false},{""name"":""testEnumGetName"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""String"",""offset"":625,""safe"":false},{""name"":""testEnumGetNameWithType"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""String"",""offset"":677,""safe"":false},{""name"":""testEnumHasFlag"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""flag"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":743,""safe"":false},{""name"":""testEnumToString"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""String"",""offset"":754,""safe"":false},{""name"":""testEnumToStringUnknown"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""String"",""offset"":807,""safe"":false},{""name"":""testEnumParseGeneric"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Integer"",""offset"":860,""safe"":false},{""name"":""testEnumParseGenericIgnoreCase"",""parameters"":[{""name"":""value"",""type"":""String""},{""name"":""ignoreCase"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":989,""safe"":false},{""name"":""testEnumTryParseGeneric"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1109,""safe"":false},{""name"":""testEnumTryParseGenericIgnoreCase"",""parameters"":[{""name"":""value"",""type"":""String""},{""name"":""ignoreCase"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":1169,""safe"":false},{""name"":""testEnumGetValuesGeneric"",""parameters"":[],""returntype"":""Array"",""offset"":1273,""safe"":false},{""name"":""testEnumGetNamesGeneric"",""parameters"":[],""returntype"":""Array"",""offset"":1279,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1306,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/R0FVwABDAhUZXN0RW51bXhKDAZWYWx1ZTGXJgcRU0VFQEoMBlZhbHVlMpcmBxJTRUVASgwGVmFsdWUzlyYHE1NFRUBFDBJObyBzdWNoIGVudW0gdmFsdWU6VwACDAhUZXN0RW51bXh5Ji4MABBKeMq1JiJKeFDOSgBhAHu7JAlRUItQnCLpAGGfAEGeUVCLUJwi3EXbKEp5JjQMBlZBTFVFMZcmB0VFRRFASnkmIAwGVkFMVUUylyYHRUVFEkBKeSYMDAZWQUxVRTMiCgwGVmFsdWUzlyYHRUVFE0BFRQwSTm8gc3VjaCBlbnVtIHZhbHVlOlcAAQwIVGVzdEVudW14C2BKDAZWYWx1ZTGXJghFRRFgCEBKDAZWYWx1ZTKXJghFRRJgCEBKDAZWYWx1ZTOXJghFRRNgCEBFRRBgCUBXAAIMCFRlc3RFbnVteHkLYSYwUEUMABBKeMq1JiJKeFDOSgBhAHu7JAlRUItQnCLpAGGfAEGeUVCLUJwi3EXbKEp5JjYMBlZBTFVFMZcmCEVFEWEIQEp5JiEMBlZBTFVFMpcmCEVFEmEIQEp5JgwMBlZBTFVFMyIKDAZWYWx1ZTOXJghFRRNhCEBFRRBhCUAMCFRlc3RFbnVtDAZWYWx1ZTMMBlZhbHVlMgwGVmFsdWUxE8BADAhUZXN0RW51bRMSERPAQFcAAQwIVGVzdEVudW14ShGXJgZFRQhAShKXJgZFRQhAShOXJgZFRQhARUUJQFcAAQwIVGVzdEVudW14SgwGVmFsdWUxlyYGRUUIQEoMBlZhbHVlMpcmBkVFCEBKDAZWYWx1ZTOXJgZFRQhARUUJQFcAAXhKEZcmDEUMBlZhbHVlMUBKEpcmDEUMBlZhbHVlMkBKE5cmDEUMBlZhbHVlM0BFC0BXAAEMCFRlc3RFbnVteEoRlyYNRUUMBlZhbHVlMUBKEpcmDUVFDAZWYWx1ZTJAShOXJg1FRQwGVmFsdWUzQEVFC0BXAQJ4eXBokWiXQFcAAXhKEZcmDEUMBlZhbHVlMUBKEpcmDEUMBlZhbHVlMkBKE5cmDEUMBlZhbHVlM0A3AABAVwABeEoRlyYMRQwGVmFsdWUxQEoSlyYMRQwGVmFsdWUyQEoTlyYMRQwGVmFsdWUzQDcAAEBXAQEJeDRHcGgMBlZhbHVlMZcmBBFAaAwGVmFsdWUylyYEEkBoDAZWYWx1ZTOXJgQTQAgmFwwSTm8gc3VjaCBlbnVtIHZhbHVlOmg6VwACeSYvDAAQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhAeEBXAgJ5eDTGcGhxaQwGVkFMVUUxlyYEEUBpDAZWQUxVRTKXJgQSQGkMBlZBTFVFM5cmBBNAaQwGVmFsdWUxlyYEEUBpDAZWYWx1ZTKXJgQSQGkMBlZhbHVlM5cmBBNACCYXDBJObyBzdWNoIGVudW0gdmFsdWU6aTpXAQEJeDVO////cGgMBlZhbHVlMZcmBAhAaAwGVmFsdWUylyYECEBoDAZWYWx1ZTOXJgQIQAgmBAlAaDpXAgJ5eDUS////cGhxaQwGVkFMVUUxlyYECEBpDAZWQUxVRTKXJgQIQGkMBlZBTFVFM5cmBAhAaQwGVmFsdWUxlyYECEBpDAZWYWx1ZTKXJgQIQGkMBlZhbHVlM5cmBAhACCYECUBpOhMSERPAQAwGVmFsdWUzDAZWYWx1ZTIMBlZhbHVlMRPAQFYCQK+A+Do=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoRlyYMRQwGVmFsdWUxQEoSlyYMRQwGVmFsdWUyQEoTlyYMRQwGVmFsdWUzQEULQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0C [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0C [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0C [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// RET [0 datoshi]
    /// DROP [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetName")]
    public abstract string? TestEnumGetName(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAhUZXN0RW51bQwGVmFsdWUzDAZWYWx1ZTIMBlZhbHVlMRPAQA==
    /// PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetNames")]
    public abstract IList<object>? TestEnumGetNames();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAZWYWx1ZTMMBlZhbHVlMgwGVmFsdWUxE8BA
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetNamesGeneric")]
    public abstract IList<object>? TestEnumGetNamesGeneric();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhUZXN0RW51bXhKEZcmDUVFDAZWYWx1ZTFAShKXJg1FRQwGVmFsdWUyQEoTlyYNRUUMBlZhbHVlM0BFRQtA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0D [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0D [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0D [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// RET [0 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetNameWithType")]
    public abstract string? TestEnumGetNameWithType(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAhUZXN0RW51bRMSERPAQA==
    /// PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetValues")]
    public abstract IList<object>? TestEnumGetValues();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: ExIRE8BA
    /// PUSH3 [1 datoshi]
    /// PUSH2 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PUSH3 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetValuesGeneric")]
    public abstract IList<object>? TestEnumGetValuesGeneric();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECeHlwaJFol0A=
    /// INITSLOT 0102 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// AND [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// EQUAL [32 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumHasFlag")]
    public abstract bool? TestEnumHasFlag(BigInteger? value, BigInteger? flag);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhUZXN0RW51bXhKEZcmBkVFCEBKEpcmBkVFCEBKE5cmBkVFCEBFRQlA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumIsDefined")]
    public abstract bool? TestEnumIsDefined(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhUZXN0RW51bXhKDAZWYWx1ZTGXJgZFRQhASgwGVmFsdWUylyYGRUUIQEoMBlZhbHVlM5cmBkVFCEBFRQlA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumIsDefinedByName")]
    public abstract bool? TestEnumIsDefinedByName(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhUZXN0RW51bXhKDAZWYWx1ZTGXJgcRU0VFQEoMBlZhbHVlMpcmBxJTRUVASgwGVmFsdWUzlyYHE1NFRUBFDBJObyBzdWNoIGVudW0gdmFsdWU6
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// REVERSE3 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 4E6F207375636820656E756D2076616C7565 [8 datoshi]
    /// THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testEnumParse")]
    public abstract object? TestEnumParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBCXg0R3BoDAZWYWx1ZTGXJgQRQGgMBlZhbHVlMpcmBBJAaAwGVmFsdWUzlyYEE0AIJhcMEk5vIHN1Y2ggZW51bSB2YWx1ZTpoOg==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSHF [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 47 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// JMPIFNOT 17 [2 datoshi]
    /// PUSHDATA1 4E6F207375636820656E756D2076616C7565 [8 datoshi]
    /// THROW [512 datoshi]
    /// LDLOC0 [2 datoshi]
    /// THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testEnumParseGeneric")]
    public abstract BigInteger? TestEnumParseGeneric(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeXg0xnBocWkMBlZBTFVFMZcmBBFAaQwGVkFMVUUylyYEEkBpDAZWQUxVRTOXJgQTQGkMBlZhbHVlMZcmBBFAaQwGVmFsdWUylyYEEkBpDAZWYWx1ZTOXJgQTQAgmFwwSTm8gc3VjaCBlbnVtIHZhbHVlOmk6
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL C6 [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 56414C554531 'VALUE1' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 56414C554532 'VALUE2' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 56414C554533 'VALUE3' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// JMPIFNOT 17 [2 datoshi]
    /// PUSHDATA1 4E6F207375636820656E756D2076616C7565 [8 datoshi]
    /// THROW [512 datoshi]
    /// LDLOC1 [2 datoshi]
    /// THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testEnumParseGenericIgnoreCase")]
    public abstract BigInteger? TestEnumParseGenericIgnoreCase(string? value, bool? ignoreCase);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACDAhUZXN0RW51bXh5Ji4MABBKeMq1JiJKeFDOSgBhAHu7JAlRUItQnCLpAGGfAEGeUVCLUJwi3EXbKEp5JjQMBlZBTFVFMZcmB0VFRRFASnkmIAwGVkFMVUUylyYHRUVFEkBKeSYMDAZWQUxVRTMiCgwGVmFsdWUzlyYHRUVFE0BFRQwSTm8gc3VjaCBlbnVtIHZhbHVlOg==
    /// INITSLOT 0002 [64 datoshi]
    /// PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// JMPIFNOT 2E [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 22 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 09 [2 datoshi]
    /// ROT [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP E9 [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// SUB [8 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// ADD [8 datoshi]
    /// ROT [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP DC [2 datoshi]
    /// DROP [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// JMPIFNOT 34 [2 datoshi]
    /// PUSHDATA1 56414C554531 'VALUE1' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// JMPIFNOT 20 [2 datoshi]
    /// PUSHDATA1 56414C554532 'VALUE2' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// JMPIFNOT 0C [2 datoshi]
    /// PUSHDATA1 56414C554533 'VALUE3' [8 datoshi]
    /// JMP 0A [2 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 07 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// RET [0 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 4E6F207375636820656E756D2076616C7565 [8 datoshi]
    /// THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testEnumParseIgnoreCase")]
    public abstract object? TestEnumParseIgnoreCase(string? value, bool? ignoreCase);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoRlyYMRQwGVmFsdWUxQEoSlyYMRQwGVmFsdWUyQEoTlyYMRQwGVmFsdWUzQDcAAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0C [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0C [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0C [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// RET [0 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumToString")]
    public abstract string? TestEnumToString(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoRlyYMRQwGVmFsdWUxQEoSlyYMRQwGVmFsdWUyQEoTlyYMRQwGVmFsdWUzQDcAAEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0C [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0C [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 0C [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// RET [0 datoshi]
    /// CALLT 0000 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumToStringUnknown")]
    public abstract string? TestEnumToStringUnknown(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhUZXN0RW51bXgLYEoMBlZhbHVlMZcmCEVFEWAIQEoMBlZhbHVlMpcmCEVFEmAIQEoMBlZhbHVlM5cmCEVFE2AIQEVFEGAJQA==
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumTryParse")]
    public abstract bool? TestEnumTryParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBCXg1Tv///3BoDAZWYWx1ZTGXJgQIQGgMBlZhbHVlMpcmBAhAaAwGVmFsdWUzlyYECEAIJgQJQGg6
    /// INITSLOT 0101 [64 datoshi]
    /// PUSHF [1 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 4EFFFFFF [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC0 [2 datoshi]
    /// THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testEnumTryParseGeneric")]
    public abstract bool? TestEnumTryParseGeneric(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICeXg1Ev///3BocWkMBlZBTFVFMZcmBAhAaQwGVkFMVUUylyYECEBpDAZWQUxVRTOXJgQIQGkMBlZhbHVlMZcmBAhAaQwGVmFsdWUylyYECEBpDAZWYWx1ZTOXJgQIQAgmBAlAaTo=
    /// INITSLOT 0202 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 12FFFFFF [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 56414C554531 'VALUE1' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 56414C554532 'VALUE2' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 56414C554533 'VALUE3' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHT [1 datoshi]
    /// JMPIFNOT 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testEnumTryParseGenericIgnoreCase")]
    public abstract bool? TestEnumTryParseGenericIgnoreCase(string? value, bool? ignoreCase);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACDAhUZXN0RW51bXh5C2EmMFBFDAAQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhKeSY2DAZWQUxVRTGXJghFRRFhCEBKeSYhDAZWQUxVRTKXJghFRRJhCEBKeSYMDAZWQUxVRTMiCgwGVmFsdWUzlyYIRUUTYQhARUUQYQlA
    /// INITSLOT 0002 [64 datoshi]
    /// PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// JMPIFNOT 30 [2 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 [8 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIZE [4 datoshi]
    /// LT [8 datoshi]
    /// JMPIFNOT 22 [2 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 09 [2 datoshi]
    /// ROT [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP E9 [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// SUB [8 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// ADD [8 datoshi]
    /// ROT [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// INC [4 datoshi]
    /// JMP DC [2 datoshi]
    /// DROP [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// JMPIFNOT 36 [2 datoshi]
    /// PUSHDATA1 56414C554531 'VALUE1' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// JMPIFNOT 21 [2 datoshi]
    /// PUSHDATA1 56414C554532 'VALUE2' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// DUP [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// JMPIFNOT 0C [2 datoshi]
    /// PUSHDATA1 56414C554533 'VALUE3' [8 datoshi]
    /// JMP 0A [2 datoshi]
    /// PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumTryParseIgnoreCase")]
    public abstract bool? TestEnumTryParseIgnoreCase(string? value, bool? ignoreCase);

    #endregion
}
