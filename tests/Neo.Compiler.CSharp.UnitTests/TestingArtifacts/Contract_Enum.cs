using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Enum"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testEnumParse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""testEnumParseIgnoreCase"",""parameters"":[{""name"":""value"",""type"":""String""},{""name"":""ignoreCase"",""type"":""Boolean""}],""returntype"":""Any"",""offset"":87,""safe"":false},{""name"":""testEnumTryParse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Boolean"",""offset"":241,""safe"":false},{""name"":""testEnumTryParseIgnoreCase"",""parameters"":[{""name"":""value"",""type"":""String""},{""name"":""ignoreCase"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":319,""safe"":false},{""name"":""testEnumGetNames"",""parameters"":[],""returntype"":""Array"",""offset"":465,""safe"":false},{""name"":""testEnumGetValues"",""parameters"":[],""returntype"":""Array"",""offset"":502,""safe"":false},{""name"":""testEnumIsDefined"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":518,""safe"":false},{""name"":""testEnumIsDefinedByName"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""Boolean"",""offset"":563,""safe"":false},{""name"":""testEnumGetName"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""String"",""offset"":629,""safe"":false},{""name"":""testEnumGetNameWithType"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""String"",""offset"":681,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":747,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3uAlcAAQwIVGVzdEVudW14SgwGVmFsdWUxlyYHEVNFRUBKDAZWYWx1ZTKXJgcSU0VFQEoMBlZhbHVlM5cmBxNTRUVARQwSTm8gc3VjaCBlbnVtIHZhbHVlOlcAAgwIVGVzdEVudW14eSYuDAAQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhKeSY0DAZWQUxVRTGXJgdFRUURQEp5JiAMBlZBTFVFMpcmB0VFRRJASnkmDAwGVkFMVUUzIgoMBlZhbHVlM5cmB0VFRRNARUUMEk5vIHN1Y2ggZW51bSB2YWx1ZTpXAAELYAwIVGVzdEVudW14WEVKDAZWYWx1ZTGXJghFRRFgCEBKDAZWYWx1ZTKXJghFRRJgCEBKDAZWYWx1ZTOXJghFRRNgCEBFRRBgCUBXAAILYQwIVGVzdEVudW14eVlFJjBQRQwAEEp4yrUmIkp4UM5KAGEAe7skCVFQi1CcIukAYZ8AQZ5RUItQnCLcRdsoSnkmNgwGVkFMVUUxlyYIRUURYQhASnkmIQwGVkFMVUUylyYIRUUSYQhASnkmDAwGVkFMVUUzIgoMBlZhbHVlM5cmCEVFE2EIQEVFEGEJQAwIVGVzdEVudW0MBlZhbHVlMwwGVmFsdWUyDAZWYWx1ZTETwEAMCFRlc3RFbnVtExIRE8BAVwABDAhUZXN0RW51bXhKEZcmBkVFCEBKEpcmBkVFCEBKE5cmBkVFCEBFRQlAVwABDAhUZXN0RW51bXhKDAZWYWx1ZTGXJgZFRQhASgwGVmFsdWUylyYGRUUIQEoMBlZhbHVlM5cmBkVFCEBFRQlAVwABeEoRlyYMRQwGVmFsdWUxQEoSlyYMRQwGVmFsdWUyQEoTlyYMRQwGVmFsdWUzQEULQFcAAQwIVGVzdEVudW14ShGXJg1FRQwGVmFsdWUxQEoSlyYNRUUMBlZhbHVlMkBKE5cmDUVFDAZWYWx1ZTNARUULQFYCQEjIG8o="));

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
    /// Script: VwABC2AMCFRlc3RFbnVteFhFSgwGVmFsdWUxlyYIRUURYAhASgwGVmFsdWUylyYIRUUSYAhASgwGVmFsdWUzlyYIRUUTYAhARUUQYAlA
    /// INITSLOT 0001 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// DROP [2 datoshi]
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
    /// Script: VwACC2EMCFRlc3RFbnVteHlZRSYwUEUMABBKeMq1JiJKeFDOSgBhAHu7JAlRUItQnCLpAGGfAEGeUVCLUJwi3EXbKEp5JjYMBlZBTFVFMZcmCEVFEWEIQEp5JiEMBlZBTFVFMpcmCEVFEmEIQEp5JgwMBlZBTFVFMyIKDAZWYWx1ZTOXJghFRRNhCEBFRRBhCUA=
    /// INITSLOT 0002 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// DROP [2 datoshi]
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
