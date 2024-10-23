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
    /// Script: VwABeEoRlyYMRQxWYWx1ZTFAShKXJgxFDFZhbHVlMkBKE5cmDEUMVmFsdWUzQEULQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH1
    /// 06 : OpCode.EQUAL
    /// 07 : OpCode.JMPIFNOT 0C
    /// 09 : OpCode.DROP
    /// 0A : OpCode.PUSHDATA1 56616C756531
    /// 12 : OpCode.RET
    /// 13 : OpCode.DUP
    /// 14 : OpCode.PUSH2
    /// 15 : OpCode.EQUAL
    /// 16 : OpCode.JMPIFNOT 0C
    /// 18 : OpCode.DROP
    /// 19 : OpCode.PUSHDATA1 56616C756532
    /// 21 : OpCode.RET
    /// 22 : OpCode.DUP
    /// 23 : OpCode.PUSH3
    /// 24 : OpCode.EQUAL
    /// 25 : OpCode.JMPIFNOT 0C
    /// 27 : OpCode.DROP
    /// 28 : OpCode.PUSHDATA1 56616C756533
    /// 30 : OpCode.RET
    /// 31 : OpCode.DROP
    /// 32 : OpCode.PUSHNULL
    /// 33 : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumGetName")]
    public abstract string? TestEnumGetName(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DFRlc3RFbnVtDFZhbHVlMwxWYWx1ZTIMVmFsdWUxE8BA
    /// 00 : OpCode.PUSHDATA1 54657374456E756D
    /// 0A : OpCode.PUSHDATA1 56616C756533
    /// 12 : OpCode.PUSHDATA1 56616C756532
    /// 1A : OpCode.PUSHDATA1 56616C756531
    /// 22 : OpCode.PUSH3
    /// 23 : OpCode.PACK
    /// 24 : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumGetNames")]
    public abstract IList<object>? TestEnumGetNames();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDFRlc3RFbnVteEoRlyYNRUUMVmFsdWUxQEoSlyYNRUUMVmFsdWUyQEoTlyYNRUUMVmFsdWUzQEVFC0A=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHDATA1 54657374456E756D
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.PUSH1
    /// 10 : OpCode.EQUAL
    /// 11 : OpCode.JMPIFNOT 0D
    /// 13 : OpCode.DROP
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHDATA1 56616C756531
    /// 1D : OpCode.RET
    /// 1E : OpCode.DUP
    /// 1F : OpCode.PUSH2
    /// 20 : OpCode.EQUAL
    /// 21 : OpCode.JMPIFNOT 0D
    /// 23 : OpCode.DROP
    /// 24 : OpCode.DROP
    /// 25 : OpCode.PUSHDATA1 56616C756532
    /// 2D : OpCode.RET
    /// 2E : OpCode.DUP
    /// 2F : OpCode.PUSH3
    /// 30 : OpCode.EQUAL
    /// 31 : OpCode.JMPIFNOT 0D
    /// 33 : OpCode.DROP
    /// 34 : OpCode.DROP
    /// 35 : OpCode.PUSHDATA1 56616C756533
    /// 3D : OpCode.RET
    /// 3E : OpCode.DROP
    /// 3F : OpCode.DROP
    /// 40 : OpCode.PUSHNULL
    /// 41 : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumGetNameWithType")]
    public abstract string? TestEnumGetNameWithType(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DFRlc3RFbnVtExIRE8BA
    /// 00 : OpCode.PUSHDATA1 54657374456E756D
    /// 0A : OpCode.PUSH3
    /// 0B : OpCode.PUSH2
    /// 0C : OpCode.PUSH1
    /// 0D : OpCode.PUSH3
    /// 0E : OpCode.PACK
    /// 0F : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumGetValues")]
    public abstract IList<object>? TestEnumGetValues();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDFRlc3RFbnVteEoRlyYGRUUIQEoSlyYGRUUIQEoTlyYGRUUIQEVFCUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHDATA1 54657374456E756D
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.PUSH1
    /// 10 : OpCode.EQUAL
    /// 11 : OpCode.JMPIFNOT 06
    /// 13 : OpCode.DROP
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHT
    /// 16 : OpCode.RET
    /// 17 : OpCode.DUP
    /// 18 : OpCode.PUSH2
    /// 19 : OpCode.EQUAL
    /// 1A : OpCode.JMPIFNOT 06
    /// 1C : OpCode.DROP
    /// 1D : OpCode.DROP
    /// 1E : OpCode.PUSHT
    /// 1F : OpCode.RET
    /// 20 : OpCode.DUP
    /// 21 : OpCode.PUSH3
    /// 22 : OpCode.EQUAL
    /// 23 : OpCode.JMPIFNOT 06
    /// 25 : OpCode.DROP
    /// 26 : OpCode.DROP
    /// 27 : OpCode.PUSHT
    /// 28 : OpCode.RET
    /// 29 : OpCode.DROP
    /// 2A : OpCode.DROP
    /// 2B : OpCode.PUSHF
    /// 2C : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumIsDefined")]
    public abstract bool? TestEnumIsDefined(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDFRlc3RFbnVteEoMVmFsdWUxlyYGRUUIQEoMVmFsdWUylyYGRUUIQEoMVmFsdWUzlyYGRUUIQEVFCUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHDATA1 54657374456E756D
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.PUSHDATA1 56616C756531
    /// 17 : OpCode.EQUAL
    /// 18 : OpCode.JMPIFNOT 06
    /// 1A : OpCode.DROP
    /// 1B : OpCode.DROP
    /// 1C : OpCode.PUSHT
    /// 1D : OpCode.RET
    /// 1E : OpCode.DUP
    /// 1F : OpCode.PUSHDATA1 56616C756532
    /// 27 : OpCode.EQUAL
    /// 28 : OpCode.JMPIFNOT 06
    /// 2A : OpCode.DROP
    /// 2B : OpCode.DROP
    /// 2C : OpCode.PUSHT
    /// 2D : OpCode.RET
    /// 2E : OpCode.DUP
    /// 2F : OpCode.PUSHDATA1 56616C756533
    /// 37 : OpCode.EQUAL
    /// 38 : OpCode.JMPIFNOT 06
    /// 3A : OpCode.DROP
    /// 3B : OpCode.DROP
    /// 3C : OpCode.PUSHT
    /// 3D : OpCode.RET
    /// 3E : OpCode.DROP
    /// 3F : OpCode.DROP
    /// 40 : OpCode.PUSHF
    /// 41 : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumIsDefinedByName")]
    public abstract bool? TestEnumIsDefinedByName(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDFRlc3RFbnVteEoMVmFsdWUxlyYHEVNFRUBKDFZhbHVlMpcmBxJTRUVASgxWYWx1ZTOXJgcTU0VFQEUMTm8gc3VjaCBlbnVtIHZhbHVlOg==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHDATA1 54657374456E756D
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.PUSHDATA1 56616C756531
    /// 17 : OpCode.EQUAL
    /// 18 : OpCode.JMPIFNOT 07
    /// 1A : OpCode.PUSH1
    /// 1B : OpCode.REVERSE3
    /// 1C : OpCode.DROP
    /// 1D : OpCode.DROP
    /// 1E : OpCode.RET
    /// 1F : OpCode.DUP
    /// 20 : OpCode.PUSHDATA1 56616C756532
    /// 28 : OpCode.EQUAL
    /// 29 : OpCode.JMPIFNOT 07
    /// 2B : OpCode.PUSH2
    /// 2C : OpCode.REVERSE3
    /// 2D : OpCode.DROP
    /// 2E : OpCode.DROP
    /// 2F : OpCode.RET
    /// 30 : OpCode.DUP
    /// 31 : OpCode.PUSHDATA1 56616C756533
    /// 39 : OpCode.EQUAL
    /// 3A : OpCode.JMPIFNOT 07
    /// 3C : OpCode.PUSH3
    /// 3D : OpCode.REVERSE3
    /// 3E : OpCode.DROP
    /// 3F : OpCode.DROP
    /// 40 : OpCode.RET
    /// 41 : OpCode.DROP
    /// 42 : OpCode.PUSHDATA1 4E6F207375636820656E756D2076616C7565
    /// 56 : OpCode.THROW
    /// </remarks>
    [DisplayName("testEnumParse")]
    public abstract object? TestEnumParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACDFRlc3RFbnVteHkmLgwQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhKeSY0DFZBTFVFMZcmB0VFRRFASnkmIAxWQUxVRTKXJgdFRUUSQEp5JgwMVkFMVUUzIgoMVmFsdWUzlyYHRUVFE0BFRQxObyBzdWNoIGVudW0gdmFsdWU6
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.PUSHDATA1 54657374456E756D
    /// 0D : OpCode.LDARG0
    /// 0E : OpCode.LDARG1
    /// 0F : OpCode.JMPIFNOT 2E
    /// 11 : OpCode.PUSHDATA1
    /// 13 : OpCode.PUSH0
    /// 14 : OpCode.DUP
    /// 15 : OpCode.LDARG0
    /// 16 : OpCode.SIZE
    /// 17 : OpCode.LT
    /// 18 : OpCode.JMPIFNOT 22
    /// 1A : OpCode.DUP
    /// 1B : OpCode.LDARG0
    /// 1C : OpCode.SWAP
    /// 1D : OpCode.PICKITEM
    /// 1E : OpCode.DUP
    /// 1F : OpCode.PUSHINT8 61
    /// 21 : OpCode.PUSHINT8 7B
    /// 23 : OpCode.WITHIN
    /// 24 : OpCode.JMPIF 09
    /// 26 : OpCode.ROT
    /// 27 : OpCode.SWAP
    /// 28 : OpCode.CAT
    /// 29 : OpCode.SWAP
    /// 2A : OpCode.INC
    /// 2B : OpCode.JMP E9
    /// 2D : OpCode.PUSHINT8 61
    /// 2F : OpCode.SUB
    /// 30 : OpCode.PUSHINT8 41
    /// 32 : OpCode.ADD
    /// 33 : OpCode.ROT
    /// 34 : OpCode.SWAP
    /// 35 : OpCode.CAT
    /// 36 : OpCode.SWAP
    /// 37 : OpCode.INC
    /// 38 : OpCode.JMP DC
    /// 3A : OpCode.DROP
    /// 3B : OpCode.CONVERT 28
    /// 3D : OpCode.DUP
    /// 3E : OpCode.LDARG1
    /// 3F : OpCode.JMPIFNOT 34
    /// 41 : OpCode.PUSHDATA1 56414C554531
    /// 49 : OpCode.EQUAL
    /// 4A : OpCode.JMPIFNOT 07
    /// 4C : OpCode.DROP
    /// 4D : OpCode.DROP
    /// 4E : OpCode.DROP
    /// 4F : OpCode.PUSH1
    /// 50 : OpCode.RET
    /// 51 : OpCode.DUP
    /// 52 : OpCode.LDARG1
    /// 53 : OpCode.JMPIFNOT 20
    /// 55 : OpCode.PUSHDATA1 56414C554532
    /// 5D : OpCode.EQUAL
    /// 5E : OpCode.JMPIFNOT 07
    /// 60 : OpCode.DROP
    /// 61 : OpCode.DROP
    /// 62 : OpCode.DROP
    /// 63 : OpCode.PUSH2
    /// 64 : OpCode.RET
    /// 65 : OpCode.DUP
    /// 66 : OpCode.LDARG1
    /// 67 : OpCode.JMPIFNOT 0C
    /// 69 : OpCode.PUSHDATA1 56414C554533
    /// 71 : OpCode.JMP 0A
    /// 73 : OpCode.PUSHDATA1 56616C756533
    /// 7B : OpCode.EQUAL
    /// 7C : OpCode.JMPIFNOT 07
    /// 7E : OpCode.DROP
    /// 7F : OpCode.DROP
    /// 80 : OpCode.DROP
    /// 81 : OpCode.PUSH3
    /// 82 : OpCode.RET
    /// 83 : OpCode.DROP
    /// 84 : OpCode.DROP
    /// 85 : OpCode.PUSHDATA1 4E6F207375636820656E756D2076616C7565
    /// 99 : OpCode.THROW
    /// </remarks>
    [DisplayName("testEnumParseIgnoreCase")]
    public abstract object? TestEnumParseIgnoreCase(string? value, bool? ignoreCase);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABC2AMVGVzdEVudW14WEVKDFZhbHVlMZcmCEVFEWAIQEoMVmFsdWUylyYIRUUSYAhASgxWYWx1ZTOXJghFRRNgCEBFRRBgCUA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.STSFLD0
    /// 05 : OpCode.PUSHDATA1 54657374456E756D
    /// 0F : OpCode.LDARG0
    /// 10 : OpCode.LDSFLD0
    /// 11 : OpCode.DROP
    /// 12 : OpCode.DUP
    /// 13 : OpCode.PUSHDATA1 56616C756531
    /// 1B : OpCode.EQUAL
    /// 1C : OpCode.JMPIFNOT 08
    /// 1E : OpCode.DROP
    /// 1F : OpCode.DROP
    /// 20 : OpCode.PUSH1
    /// 21 : OpCode.STSFLD0
    /// 22 : OpCode.PUSHT
    /// 23 : OpCode.RET
    /// 24 : OpCode.DUP
    /// 25 : OpCode.PUSHDATA1 56616C756532
    /// 2D : OpCode.EQUAL
    /// 2E : OpCode.JMPIFNOT 08
    /// 30 : OpCode.DROP
    /// 31 : OpCode.DROP
    /// 32 : OpCode.PUSH2
    /// 33 : OpCode.STSFLD0
    /// 34 : OpCode.PUSHT
    /// 35 : OpCode.RET
    /// 36 : OpCode.DUP
    /// 37 : OpCode.PUSHDATA1 56616C756533
    /// 3F : OpCode.EQUAL
    /// 40 : OpCode.JMPIFNOT 08
    /// 42 : OpCode.DROP
    /// 43 : OpCode.DROP
    /// 44 : OpCode.PUSH3
    /// 45 : OpCode.STSFLD0
    /// 46 : OpCode.PUSHT
    /// 47 : OpCode.RET
    /// 48 : OpCode.DROP
    /// 49 : OpCode.DROP
    /// 4A : OpCode.PUSH0
    /// 4B : OpCode.STSFLD0
    /// 4C : OpCode.PUSHF
    /// 4D : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumTryParse")]
    public abstract bool? TestEnumTryParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACC2EMVGVzdEVudW14eVlFJjBQRQwQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhKeSY2DFZBTFVFMZcmCEVFEWEIQEp5JiEMVkFMVUUylyYIRUUSYQhASnkmDAxWQUxVRTMiCgxWYWx1ZTOXJghFRRNhCEBFRRBhCUA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.STSFLD1
    /// 05 : OpCode.PUSHDATA1 54657374456E756D
    /// 0F : OpCode.LDARG0
    /// 10 : OpCode.LDARG1
    /// 11 : OpCode.LDSFLD1
    /// 12 : OpCode.DROP
    /// 13 : OpCode.JMPIFNOT 30
    /// 15 : OpCode.SWAP
    /// 16 : OpCode.DROP
    /// 17 : OpCode.PUSHDATA1
    /// 19 : OpCode.PUSH0
    /// 1A : OpCode.DUP
    /// 1B : OpCode.LDARG0
    /// 1C : OpCode.SIZE
    /// 1D : OpCode.LT
    /// 1E : OpCode.JMPIFNOT 22
    /// 20 : OpCode.DUP
    /// 21 : OpCode.LDARG0
    /// 22 : OpCode.SWAP
    /// 23 : OpCode.PICKITEM
    /// 24 : OpCode.DUP
    /// 25 : OpCode.PUSHINT8 61
    /// 27 : OpCode.PUSHINT8 7B
    /// 29 : OpCode.WITHIN
    /// 2A : OpCode.JMPIF 09
    /// 2C : OpCode.ROT
    /// 2D : OpCode.SWAP
    /// 2E : OpCode.CAT
    /// 2F : OpCode.SWAP
    /// 30 : OpCode.INC
    /// 31 : OpCode.JMP E9
    /// 33 : OpCode.PUSHINT8 61
    /// 35 : OpCode.SUB
    /// 36 : OpCode.PUSHINT8 41
    /// 38 : OpCode.ADD
    /// 39 : OpCode.ROT
    /// 3A : OpCode.SWAP
    /// 3B : OpCode.CAT
    /// 3C : OpCode.SWAP
    /// 3D : OpCode.INC
    /// 3E : OpCode.JMP DC
    /// 40 : OpCode.DROP
    /// 41 : OpCode.CONVERT 28
    /// 43 : OpCode.DUP
    /// 44 : OpCode.LDARG1
    /// 45 : OpCode.JMPIFNOT 36
    /// 47 : OpCode.PUSHDATA1 56414C554531
    /// 4F : OpCode.EQUAL
    /// 50 : OpCode.JMPIFNOT 08
    /// 52 : OpCode.DROP
    /// 53 : OpCode.DROP
    /// 54 : OpCode.PUSH1
    /// 55 : OpCode.STSFLD1
    /// 56 : OpCode.PUSHT
    /// 57 : OpCode.RET
    /// 58 : OpCode.DUP
    /// 59 : OpCode.LDARG1
    /// 5A : OpCode.JMPIFNOT 21
    /// 5C : OpCode.PUSHDATA1 56414C554532
    /// 64 : OpCode.EQUAL
    /// 65 : OpCode.JMPIFNOT 08
    /// 67 : OpCode.DROP
    /// 68 : OpCode.DROP
    /// 69 : OpCode.PUSH2
    /// 6A : OpCode.STSFLD1
    /// 6B : OpCode.PUSHT
    /// 6C : OpCode.RET
    /// 6D : OpCode.DUP
    /// 6E : OpCode.LDARG1
    /// 6F : OpCode.JMPIFNOT 0C
    /// 71 : OpCode.PUSHDATA1 56414C554533
    /// 79 : OpCode.JMP 0A
    /// 7B : OpCode.PUSHDATA1 56616C756533
    /// 83 : OpCode.EQUAL
    /// 84 : OpCode.JMPIFNOT 08
    /// 86 : OpCode.DROP
    /// 87 : OpCode.DROP
    /// 88 : OpCode.PUSH3
    /// 89 : OpCode.STSFLD1
    /// 8A : OpCode.PUSHT
    /// 8B : OpCode.RET
    /// 8C : OpCode.DROP
    /// 8D : OpCode.DROP
    /// 8E : OpCode.PUSH0
    /// 8F : OpCode.STSFLD1
    /// 90 : OpCode.PUSHF
    /// 91 : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumTryParseIgnoreCase")]
    public abstract bool? TestEnumTryParseIgnoreCase(string? value, bool? ignoreCase);

    #endregion
}
