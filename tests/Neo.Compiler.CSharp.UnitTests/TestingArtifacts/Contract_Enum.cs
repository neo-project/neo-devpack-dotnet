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
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH1 [1 datoshi]
    /// 06 : EQUAL [32 datoshi]
    /// 07 : JMPIFNOT 0C [2 datoshi]
    /// 09 : DROP [2 datoshi]
    /// 0A : PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// 12 : RET [0 datoshi]
    /// 13 : DUP [2 datoshi]
    /// 14 : PUSH2 [1 datoshi]
    /// 15 : EQUAL [32 datoshi]
    /// 16 : JMPIFNOT 0C [2 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// 21 : RET [0 datoshi]
    /// 22 : DUP [2 datoshi]
    /// 23 : PUSH3 [1 datoshi]
    /// 24 : EQUAL [32 datoshi]
    /// 25 : JMPIFNOT 0C [2 datoshi]
    /// 27 : DROP [2 datoshi]
    /// 28 : PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 30 : RET [0 datoshi]
    /// 31 : DROP [2 datoshi]
    /// 32 : PUSHNULL [1 datoshi]
    /// 33 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetName")]
    public abstract string? TestEnumGetName(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAhUZXN0RW51bQwGVmFsdWUzDAZWYWx1ZTIMBlZhbHVlMRPAQA==
    /// 00 : PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0A : PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 12 : PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// 1A : PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// 22 : PUSH3 [1 datoshi]
    /// 23 : PACK [2048 datoshi]
    /// 24 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetNames")]
    public abstract IList<object>? TestEnumGetNames();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhUZXN0RW51bXhKEZcmDUVFDAZWYWx1ZTFAShKXJg1FRQwGVmFsdWUyQEoTlyYNRUUMBlZhbHVlM0BFRQtA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0D : LDARG0 [2 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSH1 [1 datoshi]
    /// 10 : EQUAL [32 datoshi]
    /// 11 : JMPIFNOT 0D [2 datoshi]
    /// 13 : DROP [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// 1D : RET [0 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : PUSH2 [1 datoshi]
    /// 20 : EQUAL [32 datoshi]
    /// 21 : JMPIFNOT 0D [2 datoshi]
    /// 23 : DROP [2 datoshi]
    /// 24 : DROP [2 datoshi]
    /// 25 : PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// 2D : RET [0 datoshi]
    /// 2E : DUP [2 datoshi]
    /// 2F : PUSH3 [1 datoshi]
    /// 30 : EQUAL [32 datoshi]
    /// 31 : JMPIFNOT 0D [2 datoshi]
    /// 33 : DROP [2 datoshi]
    /// 34 : DROP [2 datoshi]
    /// 35 : PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 3D : RET [0 datoshi]
    /// 3E : DROP [2 datoshi]
    /// 3F : DROP [2 datoshi]
    /// 40 : PUSHNULL [1 datoshi]
    /// 41 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetNameWithType")]
    public abstract string? TestEnumGetNameWithType(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAhUZXN0RW51bRMSERPAQA==
    /// 00 : PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0A : PUSH3 [1 datoshi]
    /// 0B : PUSH2 [1 datoshi]
    /// 0C : PUSH1 [1 datoshi]
    /// 0D : PUSH3 [1 datoshi]
    /// 0E : PACK [2048 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetValues")]
    public abstract IList<object>? TestEnumGetValues();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhUZXN0RW51bXhKEZcmBkVFCEBKEpcmBkVFCEBKE5cmBkVFCEBFRQlA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0D : LDARG0 [2 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSH1 [1 datoshi]
    /// 10 : EQUAL [32 datoshi]
    /// 11 : JMPIFNOT 06 [2 datoshi]
    /// 13 : DROP [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHT [1 datoshi]
    /// 16 : RET [0 datoshi]
    /// 17 : DUP [2 datoshi]
    /// 18 : PUSH2 [1 datoshi]
    /// 19 : EQUAL [32 datoshi]
    /// 1A : JMPIFNOT 06 [2 datoshi]
    /// 1C : DROP [2 datoshi]
    /// 1D : DROP [2 datoshi]
    /// 1E : PUSHT [1 datoshi]
    /// 1F : RET [0 datoshi]
    /// 20 : DUP [2 datoshi]
    /// 21 : PUSH3 [1 datoshi]
    /// 22 : EQUAL [32 datoshi]
    /// 23 : JMPIFNOT 06 [2 datoshi]
    /// 25 : DROP [2 datoshi]
    /// 26 : DROP [2 datoshi]
    /// 27 : PUSHT [1 datoshi]
    /// 28 : RET [0 datoshi]
    /// 29 : DROP [2 datoshi]
    /// 2A : DROP [2 datoshi]
    /// 2B : PUSHF [1 datoshi]
    /// 2C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumIsDefined")]
    public abstract bool? TestEnumIsDefined(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhUZXN0RW51bXhKDAZWYWx1ZTGXJgZFRQhASgwGVmFsdWUylyYGRUUIQEoMBlZhbHVlM5cmBkVFCEBFRQlA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0D : LDARG0 [2 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// 17 : EQUAL [32 datoshi]
    /// 18 : JMPIFNOT 06 [2 datoshi]
    /// 1A : DROP [2 datoshi]
    /// 1B : DROP [2 datoshi]
    /// 1C : PUSHT [1 datoshi]
    /// 1D : RET [0 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// 27 : EQUAL [32 datoshi]
    /// 28 : JMPIFNOT 06 [2 datoshi]
    /// 2A : DROP [2 datoshi]
    /// 2B : DROP [2 datoshi]
    /// 2C : PUSHT [1 datoshi]
    /// 2D : RET [0 datoshi]
    /// 2E : DUP [2 datoshi]
    /// 2F : PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 37 : EQUAL [32 datoshi]
    /// 38 : JMPIFNOT 06 [2 datoshi]
    /// 3A : DROP [2 datoshi]
    /// 3B : DROP [2 datoshi]
    /// 3C : PUSHT [1 datoshi]
    /// 3D : RET [0 datoshi]
    /// 3E : DROP [2 datoshi]
    /// 3F : DROP [2 datoshi]
    /// 40 : PUSHF [1 datoshi]
    /// 41 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumIsDefinedByName")]
    public abstract bool? TestEnumIsDefinedByName(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhUZXN0RW51bXhKDAZWYWx1ZTGXJgcRU0VFQEoMBlZhbHVlMpcmBxJTRUVASgwGVmFsdWUzlyYHE1NFRUBFDBJObyBzdWNoIGVudW0gdmFsdWU6
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0D : LDARG0 [2 datoshi]
    /// 0E : DUP [2 datoshi]
    /// 0F : PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// 17 : EQUAL [32 datoshi]
    /// 18 : JMPIFNOT 07 [2 datoshi]
    /// 1A : PUSH1 [1 datoshi]
    /// 1B : REVERSE3 [2 datoshi]
    /// 1C : DROP [2 datoshi]
    /// 1D : DROP [2 datoshi]
    /// 1E : RET [0 datoshi]
    /// 1F : DUP [2 datoshi]
    /// 20 : PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// 28 : EQUAL [32 datoshi]
    /// 29 : JMPIFNOT 07 [2 datoshi]
    /// 2B : PUSH2 [1 datoshi]
    /// 2C : REVERSE3 [2 datoshi]
    /// 2D : DROP [2 datoshi]
    /// 2E : DROP [2 datoshi]
    /// 2F : RET [0 datoshi]
    /// 30 : DUP [2 datoshi]
    /// 31 : PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 39 : EQUAL [32 datoshi]
    /// 3A : JMPIFNOT 07 [2 datoshi]
    /// 3C : PUSH3 [1 datoshi]
    /// 3D : REVERSE3 [2 datoshi]
    /// 3E : DROP [2 datoshi]
    /// 3F : DROP [2 datoshi]
    /// 40 : RET [0 datoshi]
    /// 41 : DROP [2 datoshi]
    /// 42 : PUSHDATA1 4E6F207375636820656E756D2076616C7565 [8 datoshi]
    /// 56 : THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testEnumParse")]
    public abstract object? TestEnumParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACDAhUZXN0RW51bXh5Ji4MABBKeMq1JiJKeFDOSgBhAHu7JAlRUItQnCLpAGGfAEGeUVCLUJwi3EXbKEp5JjQMBlZBTFVFMZcmB0VFRRFASnkmIAwGVkFMVUUylyYHRUVFEkBKeSYMDAZWQUxVRTMiCgwGVmFsdWUzlyYHRUVFE0BFRQwSTm8gc3VjaCBlbnVtIHZhbHVlOg==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0D : LDARG0 [2 datoshi]
    /// 0E : LDARG1 [2 datoshi]
    /// 0F : JMPIFNOT 2E [2 datoshi]
    /// 11 : PUSHDATA1 [8 datoshi]
    /// 13 : PUSH0 [1 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : LDARG0 [2 datoshi]
    /// 16 : SIZE [4 datoshi]
    /// 17 : LT [8 datoshi]
    /// 18 : JMPIFNOT 22 [2 datoshi]
    /// 1A : DUP [2 datoshi]
    /// 1B : LDARG0 [2 datoshi]
    /// 1C : SWAP [2 datoshi]
    /// 1D : PICKITEM [64 datoshi]
    /// 1E : DUP [2 datoshi]
    /// 1F : PUSHINT8 61 [1 datoshi]
    /// 21 : PUSHINT8 7B [1 datoshi]
    /// 23 : WITHIN [8 datoshi]
    /// 24 : JMPIF 09 [2 datoshi]
    /// 26 : ROT [2 datoshi]
    /// 27 : SWAP [2 datoshi]
    /// 28 : CAT [2048 datoshi]
    /// 29 : SWAP [2 datoshi]
    /// 2A : INC [4 datoshi]
    /// 2B : JMP E9 [2 datoshi]
    /// 2D : PUSHINT8 61 [1 datoshi]
    /// 2F : SUB [8 datoshi]
    /// 30 : PUSHINT8 41 [1 datoshi]
    /// 32 : ADD [8 datoshi]
    /// 33 : ROT [2 datoshi]
    /// 34 : SWAP [2 datoshi]
    /// 35 : CAT [2048 datoshi]
    /// 36 : SWAP [2 datoshi]
    /// 37 : INC [4 datoshi]
    /// 38 : JMP DC [2 datoshi]
    /// 3A : DROP [2 datoshi]
    /// 3B : CONVERT 28 'ByteString' [8192 datoshi]
    /// 3D : DUP [2 datoshi]
    /// 3E : LDARG1 [2 datoshi]
    /// 3F : JMPIFNOT 34 [2 datoshi]
    /// 41 : PUSHDATA1 56414C554531 'VALUE1' [8 datoshi]
    /// 49 : EQUAL [32 datoshi]
    /// 4A : JMPIFNOT 07 [2 datoshi]
    /// 4C : DROP [2 datoshi]
    /// 4D : DROP [2 datoshi]
    /// 4E : DROP [2 datoshi]
    /// 4F : PUSH1 [1 datoshi]
    /// 50 : RET [0 datoshi]
    /// 51 : DUP [2 datoshi]
    /// 52 : LDARG1 [2 datoshi]
    /// 53 : JMPIFNOT 20 [2 datoshi]
    /// 55 : PUSHDATA1 56414C554532 'VALUE2' [8 datoshi]
    /// 5D : EQUAL [32 datoshi]
    /// 5E : JMPIFNOT 07 [2 datoshi]
    /// 60 : DROP [2 datoshi]
    /// 61 : DROP [2 datoshi]
    /// 62 : DROP [2 datoshi]
    /// 63 : PUSH2 [1 datoshi]
    /// 64 : RET [0 datoshi]
    /// 65 : DUP [2 datoshi]
    /// 66 : LDARG1 [2 datoshi]
    /// 67 : JMPIFNOT 0C [2 datoshi]
    /// 69 : PUSHDATA1 56414C554533 'VALUE3' [8 datoshi]
    /// 71 : JMP 0A [2 datoshi]
    /// 73 : PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 7B : EQUAL [32 datoshi]
    /// 7C : JMPIFNOT 07 [2 datoshi]
    /// 7E : DROP [2 datoshi]
    /// 7F : DROP [2 datoshi]
    /// 80 : DROP [2 datoshi]
    /// 81 : PUSH3 [1 datoshi]
    /// 82 : RET [0 datoshi]
    /// 83 : DROP [2 datoshi]
    /// 84 : DROP [2 datoshi]
    /// 85 : PUSHDATA1 4E6F207375636820656E756D2076616C7565 [8 datoshi]
    /// 99 : THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testEnumParseIgnoreCase")]
    public abstract object? TestEnumParseIgnoreCase(string? value, bool? ignoreCase);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABC2AMCFRlc3RFbnVteFhFSgwGVmFsdWUxlyYIRUURYAhASgwGVmFsdWUylyYIRUUSYAhASgwGVmFsdWUzlyYIRUUTYAhARUUQYAlA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : STSFLD0 [2 datoshi]
    /// 05 : PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : LDSFLD0 [2 datoshi]
    /// 11 : DROP [2 datoshi]
    /// 12 : DUP [2 datoshi]
    /// 13 : PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// 1B : EQUAL [32 datoshi]
    /// 1C : JMPIFNOT 08 [2 datoshi]
    /// 1E : DROP [2 datoshi]
    /// 1F : DROP [2 datoshi]
    /// 20 : PUSH1 [1 datoshi]
    /// 21 : STSFLD0 [2 datoshi]
    /// 22 : PUSHT [1 datoshi]
    /// 23 : RET [0 datoshi]
    /// 24 : DUP [2 datoshi]
    /// 25 : PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// 2D : EQUAL [32 datoshi]
    /// 2E : JMPIFNOT 08 [2 datoshi]
    /// 30 : DROP [2 datoshi]
    /// 31 : DROP [2 datoshi]
    /// 32 : PUSH2 [1 datoshi]
    /// 33 : STSFLD0 [2 datoshi]
    /// 34 : PUSHT [1 datoshi]
    /// 35 : RET [0 datoshi]
    /// 36 : DUP [2 datoshi]
    /// 37 : PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 3F : EQUAL [32 datoshi]
    /// 40 : JMPIFNOT 08 [2 datoshi]
    /// 42 : DROP [2 datoshi]
    /// 43 : DROP [2 datoshi]
    /// 44 : PUSH3 [1 datoshi]
    /// 45 : STSFLD0 [2 datoshi]
    /// 46 : PUSHT [1 datoshi]
    /// 47 : RET [0 datoshi]
    /// 48 : DROP [2 datoshi]
    /// 49 : DROP [2 datoshi]
    /// 4A : PUSH0 [1 datoshi]
    /// 4B : STSFLD0 [2 datoshi]
    /// 4C : PUSHF [1 datoshi]
    /// 4D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumTryParse")]
    public abstract bool? TestEnumTryParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACC2EMCFRlc3RFbnVteHlZRSYwUEUMABBKeMq1JiJKeFDOSgBhAHu7JAlRUItQnCLpAGGfAEGeUVCLUJwi3EXbKEp5JjYMBlZBTFVFMZcmCEVFEWEIQEp5JiEMBlZBTFVFMpcmCEVFEmEIQEp5JgwMBlZBTFVFMyIKDAZWYWx1ZTOXJghFRRNhCEBFRRBhCUA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : STSFLD1 [2 datoshi]
    /// 05 : PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0F : LDARG0 [2 datoshi]
    /// 10 : LDARG1 [2 datoshi]
    /// 11 : LDSFLD1 [2 datoshi]
    /// 12 : DROP [2 datoshi]
    /// 13 : JMPIFNOT 30 [2 datoshi]
    /// 15 : SWAP [2 datoshi]
    /// 16 : DROP [2 datoshi]
    /// 17 : PUSHDATA1 [8 datoshi]
    /// 19 : PUSH0 [1 datoshi]
    /// 1A : DUP [2 datoshi]
    /// 1B : LDARG0 [2 datoshi]
    /// 1C : SIZE [4 datoshi]
    /// 1D : LT [8 datoshi]
    /// 1E : JMPIFNOT 22 [2 datoshi]
    /// 20 : DUP [2 datoshi]
    /// 21 : LDARG0 [2 datoshi]
    /// 22 : SWAP [2 datoshi]
    /// 23 : PICKITEM [64 datoshi]
    /// 24 : DUP [2 datoshi]
    /// 25 : PUSHINT8 61 [1 datoshi]
    /// 27 : PUSHINT8 7B [1 datoshi]
    /// 29 : WITHIN [8 datoshi]
    /// 2A : JMPIF 09 [2 datoshi]
    /// 2C : ROT [2 datoshi]
    /// 2D : SWAP [2 datoshi]
    /// 2E : CAT [2048 datoshi]
    /// 2F : SWAP [2 datoshi]
    /// 30 : INC [4 datoshi]
    /// 31 : JMP E9 [2 datoshi]
    /// 33 : PUSHINT8 61 [1 datoshi]
    /// 35 : SUB [8 datoshi]
    /// 36 : PUSHINT8 41 [1 datoshi]
    /// 38 : ADD [8 datoshi]
    /// 39 : ROT [2 datoshi]
    /// 3A : SWAP [2 datoshi]
    /// 3B : CAT [2048 datoshi]
    /// 3C : SWAP [2 datoshi]
    /// 3D : INC [4 datoshi]
    /// 3E : JMP DC [2 datoshi]
    /// 40 : DROP [2 datoshi]
    /// 41 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 43 : DUP [2 datoshi]
    /// 44 : LDARG1 [2 datoshi]
    /// 45 : JMPIFNOT 36 [2 datoshi]
    /// 47 : PUSHDATA1 56414C554531 'VALUE1' [8 datoshi]
    /// 4F : EQUAL [32 datoshi]
    /// 50 : JMPIFNOT 08 [2 datoshi]
    /// 52 : DROP [2 datoshi]
    /// 53 : DROP [2 datoshi]
    /// 54 : PUSH1 [1 datoshi]
    /// 55 : STSFLD1 [2 datoshi]
    /// 56 : PUSHT [1 datoshi]
    /// 57 : RET [0 datoshi]
    /// 58 : DUP [2 datoshi]
    /// 59 : LDARG1 [2 datoshi]
    /// 5A : JMPIFNOT 21 [2 datoshi]
    /// 5C : PUSHDATA1 56414C554532 'VALUE2' [8 datoshi]
    /// 64 : EQUAL [32 datoshi]
    /// 65 : JMPIFNOT 08 [2 datoshi]
    /// 67 : DROP [2 datoshi]
    /// 68 : DROP [2 datoshi]
    /// 69 : PUSH2 [1 datoshi]
    /// 6A : STSFLD1 [2 datoshi]
    /// 6B : PUSHT [1 datoshi]
    /// 6C : RET [0 datoshi]
    /// 6D : DUP [2 datoshi]
    /// 6E : LDARG1 [2 datoshi]
    /// 6F : JMPIFNOT 0C [2 datoshi]
    /// 71 : PUSHDATA1 56414C554533 'VALUE3' [8 datoshi]
    /// 79 : JMP 0A [2 datoshi]
    /// 7B : PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 83 : EQUAL [32 datoshi]
    /// 84 : JMPIFNOT 08 [2 datoshi]
    /// 86 : DROP [2 datoshi]
    /// 87 : DROP [2 datoshi]
    /// 88 : PUSH3 [1 datoshi]
    /// 89 : STSFLD1 [2 datoshi]
    /// 8A : PUSHT [1 datoshi]
    /// 8B : RET [0 datoshi]
    /// 8C : DROP [2 datoshi]
    /// 8D : DROP [2 datoshi]
    /// 8E : PUSH0 [1 datoshi]
    /// 8F : STSFLD1 [2 datoshi]
    /// 90 : PUSHF [1 datoshi]
    /// 91 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumTryParseIgnoreCase")]
    public abstract bool? TestEnumTryParseIgnoreCase(string? value, bool? ignoreCase);

    #endregion
}
