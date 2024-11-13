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
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.PUSH1 [1 datoshi]
    /// 06 : OpCode.EQUAL [32 datoshi]
    /// 07 : OpCode.JMPIFNOT 0C [2 datoshi]
    /// 09 : OpCode.DROP [2 datoshi]
    /// 0A : OpCode.PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// 12 : OpCode.RET [0 datoshi]
    /// 13 : OpCode.DUP [2 datoshi]
    /// 14 : OpCode.PUSH2 [1 datoshi]
    /// 15 : OpCode.EQUAL [32 datoshi]
    /// 16 : OpCode.JMPIFNOT 0C [2 datoshi]
    /// 18 : OpCode.DROP [2 datoshi]
    /// 19 : OpCode.PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// 21 : OpCode.RET [0 datoshi]
    /// 22 : OpCode.DUP [2 datoshi]
    /// 23 : OpCode.PUSH3 [1 datoshi]
    /// 24 : OpCode.EQUAL [32 datoshi]
    /// 25 : OpCode.JMPIFNOT 0C [2 datoshi]
    /// 27 : OpCode.DROP [2 datoshi]
    /// 28 : OpCode.PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 30 : OpCode.RET [0 datoshi]
    /// 31 : OpCode.DROP [2 datoshi]
    /// 32 : OpCode.PUSHNULL [1 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetName")]
    public abstract string? TestEnumGetName(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAhUZXN0RW51bQwGVmFsdWUzDAZWYWx1ZTIMBlZhbHVlMRPAQA==
    /// 00 : OpCode.PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0A : OpCode.PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 12 : OpCode.PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// 1A : OpCode.PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// 22 : OpCode.PUSH3 [1 datoshi]
    /// 23 : OpCode.PACK [2048 datoshi]
    /// 24 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetNames")]
    public abstract IList<object>? TestEnumGetNames();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhUZXN0RW51bXhKEZcmDUVFDAZWYWx1ZTFAShKXJg1FRQwGVmFsdWUyQEoTlyYNRUUMBlZhbHVlM0BFRQtA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0D : OpCode.LDARG0 [2 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.PUSH1 [1 datoshi]
    /// 10 : OpCode.EQUAL [32 datoshi]
    /// 11 : OpCode.JMPIFNOT 0D [2 datoshi]
    /// 13 : OpCode.DROP [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// 1D : OpCode.RET [0 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.PUSH2 [1 datoshi]
    /// 20 : OpCode.EQUAL [32 datoshi]
    /// 21 : OpCode.JMPIFNOT 0D [2 datoshi]
    /// 23 : OpCode.DROP [2 datoshi]
    /// 24 : OpCode.DROP [2 datoshi]
    /// 25 : OpCode.PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// 2D : OpCode.RET [0 datoshi]
    /// 2E : OpCode.DUP [2 datoshi]
    /// 2F : OpCode.PUSH3 [1 datoshi]
    /// 30 : OpCode.EQUAL [32 datoshi]
    /// 31 : OpCode.JMPIFNOT 0D [2 datoshi]
    /// 33 : OpCode.DROP [2 datoshi]
    /// 34 : OpCode.DROP [2 datoshi]
    /// 35 : OpCode.PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 3D : OpCode.RET [0 datoshi]
    /// 3E : OpCode.DROP [2 datoshi]
    /// 3F : OpCode.DROP [2 datoshi]
    /// 40 : OpCode.PUSHNULL [1 datoshi]
    /// 41 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetNameWithType")]
    public abstract string? TestEnumGetNameWithType(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAhUZXN0RW51bRMSERPAQA==
    /// 00 : OpCode.PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0A : OpCode.PUSH3 [1 datoshi]
    /// 0B : OpCode.PUSH2 [1 datoshi]
    /// 0C : OpCode.PUSH1 [1 datoshi]
    /// 0D : OpCode.PUSH3 [1 datoshi]
    /// 0E : OpCode.PACK [2048 datoshi]
    /// 0F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumGetValues")]
    public abstract IList<object>? TestEnumGetValues();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhUZXN0RW51bXhKEZcmBkVFCEBKEpcmBkVFCEBKE5cmBkVFCEBFRQlA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0D : OpCode.LDARG0 [2 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.PUSH1 [1 datoshi]
    /// 10 : OpCode.EQUAL [32 datoshi]
    /// 11 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 13 : OpCode.DROP [2 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.PUSHT [1 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// 17 : OpCode.DUP [2 datoshi]
    /// 18 : OpCode.PUSH2 [1 datoshi]
    /// 19 : OpCode.EQUAL [32 datoshi]
    /// 1A : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 1C : OpCode.DROP [2 datoshi]
    /// 1D : OpCode.DROP [2 datoshi]
    /// 1E : OpCode.PUSHT [1 datoshi]
    /// 1F : OpCode.RET [0 datoshi]
    /// 20 : OpCode.DUP [2 datoshi]
    /// 21 : OpCode.PUSH3 [1 datoshi]
    /// 22 : OpCode.EQUAL [32 datoshi]
    /// 23 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 25 : OpCode.DROP [2 datoshi]
    /// 26 : OpCode.DROP [2 datoshi]
    /// 27 : OpCode.PUSHT [1 datoshi]
    /// 28 : OpCode.RET [0 datoshi]
    /// 29 : OpCode.DROP [2 datoshi]
    /// 2A : OpCode.DROP [2 datoshi]
    /// 2B : OpCode.PUSHF [1 datoshi]
    /// 2C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumIsDefined")]
    public abstract bool? TestEnumIsDefined(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhUZXN0RW51bXhKDAZWYWx1ZTGXJgZFRQhASgwGVmFsdWUylyYGRUUIQEoMBlZhbHVlM5cmBkVFCEBFRQlA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0D : OpCode.LDARG0 [2 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// 17 : OpCode.EQUAL [32 datoshi]
    /// 18 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 1A : OpCode.DROP [2 datoshi]
    /// 1B : OpCode.DROP [2 datoshi]
    /// 1C : OpCode.PUSHT [1 datoshi]
    /// 1D : OpCode.RET [0 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// 27 : OpCode.EQUAL [32 datoshi]
    /// 28 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 2A : OpCode.DROP [2 datoshi]
    /// 2B : OpCode.DROP [2 datoshi]
    /// 2C : OpCode.PUSHT [1 datoshi]
    /// 2D : OpCode.RET [0 datoshi]
    /// 2E : OpCode.DUP [2 datoshi]
    /// 2F : OpCode.PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 37 : OpCode.EQUAL [32 datoshi]
    /// 38 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 3A : OpCode.DROP [2 datoshi]
    /// 3B : OpCode.DROP [2 datoshi]
    /// 3C : OpCode.PUSHT [1 datoshi]
    /// 3D : OpCode.RET [0 datoshi]
    /// 3E : OpCode.DROP [2 datoshi]
    /// 3F : OpCode.DROP [2 datoshi]
    /// 40 : OpCode.PUSHF [1 datoshi]
    /// 41 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumIsDefinedByName")]
    public abstract bool? TestEnumIsDefinedByName(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDAhUZXN0RW51bXhKDAZWYWx1ZTGXJgcRU0VFQEoMBlZhbHVlMpcmBxJTRUVASgwGVmFsdWUzlyYHE1NFRUBFDBJObyBzdWNoIGVudW0gdmFsdWU6
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0D : OpCode.LDARG0 [2 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// 17 : OpCode.EQUAL [32 datoshi]
    /// 18 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 1A : OpCode.PUSH1 [1 datoshi]
    /// 1B : OpCode.REVERSE3 [2 datoshi]
    /// 1C : OpCode.DROP [2 datoshi]
    /// 1D : OpCode.DROP [2 datoshi]
    /// 1E : OpCode.RET [0 datoshi]
    /// 1F : OpCode.DUP [2 datoshi]
    /// 20 : OpCode.PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// 28 : OpCode.EQUAL [32 datoshi]
    /// 29 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 2B : OpCode.PUSH2 [1 datoshi]
    /// 2C : OpCode.REVERSE3 [2 datoshi]
    /// 2D : OpCode.DROP [2 datoshi]
    /// 2E : OpCode.DROP [2 datoshi]
    /// 2F : OpCode.RET [0 datoshi]
    /// 30 : OpCode.DUP [2 datoshi]
    /// 31 : OpCode.PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 39 : OpCode.EQUAL [32 datoshi]
    /// 3A : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 3C : OpCode.PUSH3 [1 datoshi]
    /// 3D : OpCode.REVERSE3 [2 datoshi]
    /// 3E : OpCode.DROP [2 datoshi]
    /// 3F : OpCode.DROP [2 datoshi]
    /// 40 : OpCode.RET [0 datoshi]
    /// 41 : OpCode.DROP [2 datoshi]
    /// 42 : OpCode.PUSHDATA1 4E6F207375636820656E756D2076616C7565 [8 datoshi]
    /// 56 : OpCode.THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testEnumParse")]
    public abstract object? TestEnumParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACDAhUZXN0RW51bXh5Ji4MABBKeMq1JiJKeFDOSgBhAHu7JAlRUItQnCLpAGGfAEGeUVCLUJwi3EXbKEp5JjQMBlZBTFVFMZcmB0VFRRFASnkmIAwGVkFMVUUylyYHRUVFEkBKeSYMDAZWQUxVRTMiCgwGVmFsdWUzlyYHRUVFE0BFRQwSTm8gc3VjaCBlbnVtIHZhbHVlOg==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0D : OpCode.LDARG0 [2 datoshi]
    /// 0E : OpCode.LDARG1 [2 datoshi]
    /// 0F : OpCode.JMPIFNOT 2E [2 datoshi]
    /// 11 : OpCode.PUSHDATA1 [8 datoshi]
    /// 13 : OpCode.PUSH0 [1 datoshi]
    /// 14 : OpCode.DUP [2 datoshi]
    /// 15 : OpCode.LDARG0 [2 datoshi]
    /// 16 : OpCode.SIZE [4 datoshi]
    /// 17 : OpCode.LT [8 datoshi]
    /// 18 : OpCode.JMPIFNOT 22 [2 datoshi]
    /// 1A : OpCode.DUP [2 datoshi]
    /// 1B : OpCode.LDARG0 [2 datoshi]
    /// 1C : OpCode.SWAP [2 datoshi]
    /// 1D : OpCode.PICKITEM [64 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.PUSHINT8 61 [1 datoshi]
    /// 21 : OpCode.PUSHINT8 7B [1 datoshi]
    /// 23 : OpCode.WITHIN [8 datoshi]
    /// 24 : OpCode.JMPIF 09 [2 datoshi]
    /// 26 : OpCode.ROT [2 datoshi]
    /// 27 : OpCode.SWAP [2 datoshi]
    /// 28 : OpCode.CAT [2048 datoshi]
    /// 29 : OpCode.SWAP [2 datoshi]
    /// 2A : OpCode.INC [4 datoshi]
    /// 2B : OpCode.JMP E9 [2 datoshi]
    /// 2D : OpCode.PUSHINT8 61 [1 datoshi]
    /// 2F : OpCode.SUB [8 datoshi]
    /// 30 : OpCode.PUSHINT8 41 [1 datoshi]
    /// 32 : OpCode.ADD [8 datoshi]
    /// 33 : OpCode.ROT [2 datoshi]
    /// 34 : OpCode.SWAP [2 datoshi]
    /// 35 : OpCode.CAT [2048 datoshi]
    /// 36 : OpCode.SWAP [2 datoshi]
    /// 37 : OpCode.INC [4 datoshi]
    /// 38 : OpCode.JMP DC [2 datoshi]
    /// 3A : OpCode.DROP [2 datoshi]
    /// 3B : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 3D : OpCode.DUP [2 datoshi]
    /// 3E : OpCode.LDARG1 [2 datoshi]
    /// 3F : OpCode.JMPIFNOT 34 [2 datoshi]
    /// 41 : OpCode.PUSHDATA1 56414C554531 'VALUE1' [8 datoshi]
    /// 49 : OpCode.EQUAL [32 datoshi]
    /// 4A : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 4C : OpCode.DROP [2 datoshi]
    /// 4D : OpCode.DROP [2 datoshi]
    /// 4E : OpCode.DROP [2 datoshi]
    /// 4F : OpCode.PUSH1 [1 datoshi]
    /// 50 : OpCode.RET [0 datoshi]
    /// 51 : OpCode.DUP [2 datoshi]
    /// 52 : OpCode.LDARG1 [2 datoshi]
    /// 53 : OpCode.JMPIFNOT 20 [2 datoshi]
    /// 55 : OpCode.PUSHDATA1 56414C554532 'VALUE2' [8 datoshi]
    /// 5D : OpCode.EQUAL [32 datoshi]
    /// 5E : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 60 : OpCode.DROP [2 datoshi]
    /// 61 : OpCode.DROP [2 datoshi]
    /// 62 : OpCode.DROP [2 datoshi]
    /// 63 : OpCode.PUSH2 [1 datoshi]
    /// 64 : OpCode.RET [0 datoshi]
    /// 65 : OpCode.DUP [2 datoshi]
    /// 66 : OpCode.LDARG1 [2 datoshi]
    /// 67 : OpCode.JMPIFNOT 0C [2 datoshi]
    /// 69 : OpCode.PUSHDATA1 56414C554533 'VALUE3' [8 datoshi]
    /// 71 : OpCode.JMP 0A [2 datoshi]
    /// 73 : OpCode.PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 7B : OpCode.EQUAL [32 datoshi]
    /// 7C : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 7E : OpCode.DROP [2 datoshi]
    /// 7F : OpCode.DROP [2 datoshi]
    /// 80 : OpCode.DROP [2 datoshi]
    /// 81 : OpCode.PUSH3 [1 datoshi]
    /// 82 : OpCode.RET [0 datoshi]
    /// 83 : OpCode.DROP [2 datoshi]
    /// 84 : OpCode.DROP [2 datoshi]
    /// 85 : OpCode.PUSHDATA1 4E6F207375636820656E756D2076616C7565 [8 datoshi]
    /// 99 : OpCode.THROW [512 datoshi]
    /// </remarks>
    [DisplayName("testEnumParseIgnoreCase")]
    public abstract object? TestEnumParseIgnoreCase(string? value, bool? ignoreCase);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABC2AMCFRlc3RFbnVteFhFSgwGVmFsdWUxlyYIRUURYAhASgwGVmFsdWUylyYIRUUSYAhASgwGVmFsdWUzlyYIRUUTYAhARUUQYAlA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.STSFLD0 [2 datoshi]
    /// 05 : OpCode.PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.LDSFLD0 [2 datoshi]
    /// 11 : OpCode.DROP [2 datoshi]
    /// 12 : OpCode.DUP [2 datoshi]
    /// 13 : OpCode.PUSHDATA1 56616C756531 'Value1' [8 datoshi]
    /// 1B : OpCode.EQUAL [32 datoshi]
    /// 1C : OpCode.JMPIFNOT 08 [2 datoshi]
    /// 1E : OpCode.DROP [2 datoshi]
    /// 1F : OpCode.DROP [2 datoshi]
    /// 20 : OpCode.PUSH1 [1 datoshi]
    /// 21 : OpCode.STSFLD0 [2 datoshi]
    /// 22 : OpCode.PUSHT [1 datoshi]
    /// 23 : OpCode.RET [0 datoshi]
    /// 24 : OpCode.DUP [2 datoshi]
    /// 25 : OpCode.PUSHDATA1 56616C756532 'Value2' [8 datoshi]
    /// 2D : OpCode.EQUAL [32 datoshi]
    /// 2E : OpCode.JMPIFNOT 08 [2 datoshi]
    /// 30 : OpCode.DROP [2 datoshi]
    /// 31 : OpCode.DROP [2 datoshi]
    /// 32 : OpCode.PUSH2 [1 datoshi]
    /// 33 : OpCode.STSFLD0 [2 datoshi]
    /// 34 : OpCode.PUSHT [1 datoshi]
    /// 35 : OpCode.RET [0 datoshi]
    /// 36 : OpCode.DUP [2 datoshi]
    /// 37 : OpCode.PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 3F : OpCode.EQUAL [32 datoshi]
    /// 40 : OpCode.JMPIFNOT 08 [2 datoshi]
    /// 42 : OpCode.DROP [2 datoshi]
    /// 43 : OpCode.DROP [2 datoshi]
    /// 44 : OpCode.PUSH3 [1 datoshi]
    /// 45 : OpCode.STSFLD0 [2 datoshi]
    /// 46 : OpCode.PUSHT [1 datoshi]
    /// 47 : OpCode.RET [0 datoshi]
    /// 48 : OpCode.DROP [2 datoshi]
    /// 49 : OpCode.DROP [2 datoshi]
    /// 4A : OpCode.PUSH0 [1 datoshi]
    /// 4B : OpCode.STSFLD0 [2 datoshi]
    /// 4C : OpCode.PUSHF [1 datoshi]
    /// 4D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumTryParse")]
    public abstract bool? TestEnumTryParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACC2EMCFRlc3RFbnVteHlZRSYwUEUMABBKeMq1JiJKeFDOSgBhAHu7JAlRUItQnCLpAGGfAEGeUVCLUJwi3EXbKEp5JjYMBlZBTFVFMZcmCEVFEWEIQEp5JiEMBlZBTFVFMpcmCEVFEmEIQEp5JgwMBlZBTFVFMyIKDAZWYWx1ZTOXJghFRRNhCEBFRRBhCUA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.STSFLD1 [2 datoshi]
    /// 05 : OpCode.PUSHDATA1 54657374456E756D 'TestEnum' [8 datoshi]
    /// 0F : OpCode.LDARG0 [2 datoshi]
    /// 10 : OpCode.LDARG1 [2 datoshi]
    /// 11 : OpCode.LDSFLD1 [2 datoshi]
    /// 12 : OpCode.DROP [2 datoshi]
    /// 13 : OpCode.JMPIFNOT 30 [2 datoshi]
    /// 15 : OpCode.SWAP [2 datoshi]
    /// 16 : OpCode.DROP [2 datoshi]
    /// 17 : OpCode.PUSHDATA1 [8 datoshi]
    /// 19 : OpCode.PUSH0 [1 datoshi]
    /// 1A : OpCode.DUP [2 datoshi]
    /// 1B : OpCode.LDARG0 [2 datoshi]
    /// 1C : OpCode.SIZE [4 datoshi]
    /// 1D : OpCode.LT [8 datoshi]
    /// 1E : OpCode.JMPIFNOT 22 [2 datoshi]
    /// 20 : OpCode.DUP [2 datoshi]
    /// 21 : OpCode.LDARG0 [2 datoshi]
    /// 22 : OpCode.SWAP [2 datoshi]
    /// 23 : OpCode.PICKITEM [64 datoshi]
    /// 24 : OpCode.DUP [2 datoshi]
    /// 25 : OpCode.PUSHINT8 61 [1 datoshi]
    /// 27 : OpCode.PUSHINT8 7B [1 datoshi]
    /// 29 : OpCode.WITHIN [8 datoshi]
    /// 2A : OpCode.JMPIF 09 [2 datoshi]
    /// 2C : OpCode.ROT [2 datoshi]
    /// 2D : OpCode.SWAP [2 datoshi]
    /// 2E : OpCode.CAT [2048 datoshi]
    /// 2F : OpCode.SWAP [2 datoshi]
    /// 30 : OpCode.INC [4 datoshi]
    /// 31 : OpCode.JMP E9 [2 datoshi]
    /// 33 : OpCode.PUSHINT8 61 [1 datoshi]
    /// 35 : OpCode.SUB [8 datoshi]
    /// 36 : OpCode.PUSHINT8 41 [1 datoshi]
    /// 38 : OpCode.ADD [8 datoshi]
    /// 39 : OpCode.ROT [2 datoshi]
    /// 3A : OpCode.SWAP [2 datoshi]
    /// 3B : OpCode.CAT [2048 datoshi]
    /// 3C : OpCode.SWAP [2 datoshi]
    /// 3D : OpCode.INC [4 datoshi]
    /// 3E : OpCode.JMP DC [2 datoshi]
    /// 40 : OpCode.DROP [2 datoshi]
    /// 41 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 43 : OpCode.DUP [2 datoshi]
    /// 44 : OpCode.LDARG1 [2 datoshi]
    /// 45 : OpCode.JMPIFNOT 36 [2 datoshi]
    /// 47 : OpCode.PUSHDATA1 56414C554531 'VALUE1' [8 datoshi]
    /// 4F : OpCode.EQUAL [32 datoshi]
    /// 50 : OpCode.JMPIFNOT 08 [2 datoshi]
    /// 52 : OpCode.DROP [2 datoshi]
    /// 53 : OpCode.DROP [2 datoshi]
    /// 54 : OpCode.PUSH1 [1 datoshi]
    /// 55 : OpCode.STSFLD1 [2 datoshi]
    /// 56 : OpCode.PUSHT [1 datoshi]
    /// 57 : OpCode.RET [0 datoshi]
    /// 58 : OpCode.DUP [2 datoshi]
    /// 59 : OpCode.LDARG1 [2 datoshi]
    /// 5A : OpCode.JMPIFNOT 21 [2 datoshi]
    /// 5C : OpCode.PUSHDATA1 56414C554532 'VALUE2' [8 datoshi]
    /// 64 : OpCode.EQUAL [32 datoshi]
    /// 65 : OpCode.JMPIFNOT 08 [2 datoshi]
    /// 67 : OpCode.DROP [2 datoshi]
    /// 68 : OpCode.DROP [2 datoshi]
    /// 69 : OpCode.PUSH2 [1 datoshi]
    /// 6A : OpCode.STSFLD1 [2 datoshi]
    /// 6B : OpCode.PUSHT [1 datoshi]
    /// 6C : OpCode.RET [0 datoshi]
    /// 6D : OpCode.DUP [2 datoshi]
    /// 6E : OpCode.LDARG1 [2 datoshi]
    /// 6F : OpCode.JMPIFNOT 0C [2 datoshi]
    /// 71 : OpCode.PUSHDATA1 56414C554533 'VALUE3' [8 datoshi]
    /// 79 : OpCode.JMP 0A [2 datoshi]
    /// 7B : OpCode.PUSHDATA1 56616C756533 'Value3' [8 datoshi]
    /// 83 : OpCode.EQUAL [32 datoshi]
    /// 84 : OpCode.JMPIFNOT 08 [2 datoshi]
    /// 86 : OpCode.DROP [2 datoshi]
    /// 87 : OpCode.DROP [2 datoshi]
    /// 88 : OpCode.PUSH3 [1 datoshi]
    /// 89 : OpCode.STSFLD1 [2 datoshi]
    /// 8A : OpCode.PUSHT [1 datoshi]
    /// 8B : OpCode.RET [0 datoshi]
    /// 8C : OpCode.DROP [2 datoshi]
    /// 8D : OpCode.DROP [2 datoshi]
    /// 8E : OpCode.PUSH0 [1 datoshi]
    /// 8F : OpCode.STSFLD1 [2 datoshi]
    /// 90 : OpCode.PUSHF [1 datoshi]
    /// 91 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEnumTryParseIgnoreCase")]
    public abstract bool? TestEnumTryParseIgnoreCase(string? value, bool? ignoreCase);

    #endregion
}
