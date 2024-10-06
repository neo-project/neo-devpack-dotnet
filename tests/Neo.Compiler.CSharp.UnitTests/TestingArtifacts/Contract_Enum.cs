using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Enum(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Enum"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testEnumParse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""testEnumParseIgnoreCase"",""parameters"":[{""name"":""value"",""type"":""String""},{""name"":""ignoreCase"",""type"":""Boolean""}],""returntype"":""Any"",""offset"":87,""safe"":false},{""name"":""testEnumTryParse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Boolean"",""offset"":241,""safe"":false},{""name"":""testEnumTryParseIgnoreCase"",""parameters"":[{""name"":""value"",""type"":""String""},{""name"":""ignoreCase"",""type"":""Boolean""}],""returntype"":""Boolean"",""offset"":319,""safe"":false},{""name"":""testEnumGetNames"",""parameters"":[],""returntype"":""Array"",""offset"":465,""safe"":false},{""name"":""testEnumGetValues"",""parameters"":[],""returntype"":""Array"",""offset"":511,""safe"":false},{""name"":""testEnumIsDefined"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":536,""safe"":false},{""name"":""testEnumIsDefinedByName"",""parameters"":[{""name"":""name"",""type"":""String""}],""returntype"":""Boolean"",""offset"":581,""safe"":false},{""name"":""testEnumGetName"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""String"",""offset"":647,""safe"":false},{""name"":""testEnumGetNameWithType"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""String"",""offset"":699,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":765,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0AA1cAAQwIVGVzdEVudW14SgwGVmFsdWUxlyYHEVNFRUBKDAZWYWx1ZTKXJgcSU0VFQEoMBlZhbHVlM5cmBxNTRUVARQwSTm8gc3VjaCBlbnVtIHZhbHVlOlcAAgwIVGVzdEVudW14eSYuDAAQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhKeSY0DAZWQUxVRTGXJgdFRUURQEp5JiAMBlZBTFVFMpcmB0VFRRJASnkmDAwGVkFMVUUzIgoMBlZhbHVlM5cmB0VFRRNARUUMEk5vIHN1Y2ggZW51bSB2YWx1ZTpXAAELYAwIVGVzdEVudW14WEVKDAZWYWx1ZTGXJghFRRFgCEBKDAZWYWx1ZTKXJghFRRJgCEBKDAZWYWx1ZTOXJghFRRNgCEBFRRBgCUBXAAILYQwIVGVzdEVudW14eVlFJjBQRQwAEEp4yrUmIkp4UM5KAGEAe7skCVFQi1CcIukAYZ8AQZ5RUItQnCLcRdsoSnkmNgwGVkFMVUUxlyYIRUURYQhASnkmIQwGVkFMVUUylyYIRUUSYQhASnkmDAwGVkFMVUUzIgoMBlZhbHVlM5cmCEVFE2EIQEVFEGEJQAwIVGVzdEVudW0Tw0oQDAZWYWx1ZTHQShEMBlZhbHVlMtBKEgwGVmFsdWUz0EAMCFRlc3RFbnVtE8NKEBHQShES0EoSE9BAVwABDAhUZXN0RW51bXhKEZcmBkVFCEBKEpcmBkVFCEBKE5cmBkVFCEBFRQlAVwABDAhUZXN0RW51bXhKDAZWYWx1ZTGXJgZFRQhASgwGVmFsdWUylyYGRUUIQEoMBlZhbHVlM5cmBkVFCEBFRQlAVwABeEoRlyYMRQwGVmFsdWUxQEoSlyYMRQwGVmFsdWUyQEoTlyYMRQwGVmFsdWUzQEULQFcAAQwIVGVzdEVudW14ShGXJg1FRQwGVmFsdWUxQEoSlyYNRUUMBlZhbHVlMkBKE5cmDUVFDAZWYWx1ZTNARUULQFYCQCsBkiY="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoRlyYMRQxWYWx1ZTFAShKXJgxFDFZhbHVlMkBKE5cmDEUMVmFsdWUzQEULQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH1
    /// 0006 : OpCode.EQUAL
    /// 0007 : OpCode.JMPIFNOT 0C
    /// 0009 : OpCode.DROP
    /// 000A : OpCode.PUSHDATA1 56616C756531
    /// 0012 : OpCode.RET
    /// 0013 : OpCode.DUP
    /// 0014 : OpCode.PUSH2
    /// 0015 : OpCode.EQUAL
    /// 0016 : OpCode.JMPIFNOT 0C
    /// 0018 : OpCode.DROP
    /// 0019 : OpCode.PUSHDATA1 56616C756532
    /// 0021 : OpCode.RET
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSH3
    /// 0024 : OpCode.EQUAL
    /// 0025 : OpCode.JMPIFNOT 0C
    /// 0027 : OpCode.DROP
    /// 0028 : OpCode.PUSHDATA1 56616C756533
    /// 0030 : OpCode.RET
    /// 0031 : OpCode.DROP
    /// 0032 : OpCode.PUSHNULL
    /// 0033 : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumGetName")]
    public abstract string? TestEnumGetName(BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DFRlc3RFbnVtE8NKEAxWYWx1ZTHQShEMVmFsdWUy0EoSDFZhbHVlM9BA
    /// 0000 : OpCode.PUSHDATA1 54657374456E756D
    /// 000A : OpCode.PUSH3
    /// 000B : OpCode.NEWARRAY
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.PUSHDATA1 56616C756531
    /// 0016 : OpCode.SETITEM
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.PUSH1
    /// 0019 : OpCode.PUSHDATA1 56616C756532
    /// 0021 : OpCode.SETITEM
    /// 0022 : OpCode.DUP
    /// 0023 : OpCode.PUSH2
    /// 0024 : OpCode.PUSHDATA1 56616C756533
    /// 002C : OpCode.SETITEM
    /// 002D : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumGetNames")]
    public abstract IList<object>? TestEnumGetNames();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDFRlc3RFbnVteEoRlyYNRUUMVmFsdWUxQEoSlyYNRUUMVmFsdWUyQEoTlyYNRUUMVmFsdWUzQEVFC0A=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHDATA1 54657374456E756D
    /// 000D : OpCode.LDARG0
    /// 000E : OpCode.DUP
    /// 000F : OpCode.PUSH1
    /// 0010 : OpCode.EQUAL
    /// 0011 : OpCode.JMPIFNOT 0D
    /// 0013 : OpCode.DROP
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHDATA1 56616C756531
    /// 001D : OpCode.RET
    /// 001E : OpCode.DUP
    /// 001F : OpCode.PUSH2
    /// 0020 : OpCode.EQUAL
    /// 0021 : OpCode.JMPIFNOT 0D
    /// 0023 : OpCode.DROP
    /// 0024 : OpCode.DROP
    /// 0025 : OpCode.PUSHDATA1 56616C756532
    /// 002D : OpCode.RET
    /// 002E : OpCode.DUP
    /// 002F : OpCode.PUSH3
    /// 0030 : OpCode.EQUAL
    /// 0031 : OpCode.JMPIFNOT 0D
    /// 0033 : OpCode.DROP
    /// 0034 : OpCode.DROP
    /// 0035 : OpCode.PUSHDATA1 56616C756533
    /// 003D : OpCode.RET
    /// 003E : OpCode.DROP
    /// 003F : OpCode.DROP
    /// 0040 : OpCode.PUSHNULL
    /// 0041 : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumGetNameWithType")]
    public abstract string? TestEnumGetNameWithType(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DFRlc3RFbnVtE8NKEBHQShES0EoSE9BA
    /// 0000 : OpCode.PUSHDATA1 54657374456E756D
    /// 000A : OpCode.PUSH3
    /// 000B : OpCode.NEWARRAY
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.PUSH1
    /// 000F : OpCode.SETITEM
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.PUSH1
    /// 0012 : OpCode.PUSH2
    /// 0013 : OpCode.SETITEM
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.PUSH2
    /// 0016 : OpCode.PUSH3
    /// 0017 : OpCode.SETITEM
    /// 0018 : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumGetValues")]
    public abstract IList<object>? TestEnumGetValues();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDFRlc3RFbnVteEoRlyYGRUUIQEoSlyYGRUUIQEoTlyYGRUUIQEVFCUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHDATA1 54657374456E756D
    /// 000D : OpCode.LDARG0
    /// 000E : OpCode.DUP
    /// 000F : OpCode.PUSH1
    /// 0010 : OpCode.EQUAL
    /// 0011 : OpCode.JMPIFNOT 06
    /// 0013 : OpCode.DROP
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHT
    /// 0016 : OpCode.RET
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.PUSH2
    /// 0019 : OpCode.EQUAL
    /// 001A : OpCode.JMPIFNOT 06
    /// 001C : OpCode.DROP
    /// 001D : OpCode.DROP
    /// 001E : OpCode.PUSHT
    /// 001F : OpCode.RET
    /// 0020 : OpCode.DUP
    /// 0021 : OpCode.PUSH3
    /// 0022 : OpCode.EQUAL
    /// 0023 : OpCode.JMPIFNOT 06
    /// 0025 : OpCode.DROP
    /// 0026 : OpCode.DROP
    /// 0027 : OpCode.PUSHT
    /// 0028 : OpCode.RET
    /// 0029 : OpCode.DROP
    /// 002A : OpCode.DROP
    /// 002B : OpCode.PUSHF
    /// 002C : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumIsDefined")]
    public abstract bool? TestEnumIsDefined(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDFRlc3RFbnVteEoMVmFsdWUxlyYGRUUIQEoMVmFsdWUylyYGRUUIQEoMVmFsdWUzlyYGRUUIQEVFCUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHDATA1 54657374456E756D
    /// 000D : OpCode.LDARG0
    /// 000E : OpCode.DUP
    /// 000F : OpCode.PUSHDATA1 56616C756531
    /// 0017 : OpCode.EQUAL
    /// 0018 : OpCode.JMPIFNOT 06
    /// 001A : OpCode.DROP
    /// 001B : OpCode.DROP
    /// 001C : OpCode.PUSHT
    /// 001D : OpCode.RET
    /// 001E : OpCode.DUP
    /// 001F : OpCode.PUSHDATA1 56616C756532
    /// 0027 : OpCode.EQUAL
    /// 0028 : OpCode.JMPIFNOT 06
    /// 002A : OpCode.DROP
    /// 002B : OpCode.DROP
    /// 002C : OpCode.PUSHT
    /// 002D : OpCode.RET
    /// 002E : OpCode.DUP
    /// 002F : OpCode.PUSHDATA1 56616C756533
    /// 0037 : OpCode.EQUAL
    /// 0038 : OpCode.JMPIFNOT 06
    /// 003A : OpCode.DROP
    /// 003B : OpCode.DROP
    /// 003C : OpCode.PUSHT
    /// 003D : OpCode.RET
    /// 003E : OpCode.DROP
    /// 003F : OpCode.DROP
    /// 0040 : OpCode.PUSHF
    /// 0041 : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumIsDefinedByName")]
    public abstract bool? TestEnumIsDefinedByName(string? name);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDFRlc3RFbnVteEoMVmFsdWUxlyYHEVNFRUBKDFZhbHVlMpcmBxJTRUVASgxWYWx1ZTOXJgcTU0VFQEUMTm8gc3VjaCBlbnVtIHZhbHVlOg==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHDATA1 54657374456E756D
    /// 000D : OpCode.LDARG0
    /// 000E : OpCode.DUP
    /// 000F : OpCode.PUSHDATA1 56616C756531
    /// 0017 : OpCode.EQUAL
    /// 0018 : OpCode.JMPIFNOT 07
    /// 001A : OpCode.PUSH1
    /// 001B : OpCode.REVERSE3
    /// 001C : OpCode.DROP
    /// 001D : OpCode.DROP
    /// 001E : OpCode.RET
    /// 001F : OpCode.DUP
    /// 0020 : OpCode.PUSHDATA1 56616C756532
    /// 0028 : OpCode.EQUAL
    /// 0029 : OpCode.JMPIFNOT 07
    /// 002B : OpCode.PUSH2
    /// 002C : OpCode.REVERSE3
    /// 002D : OpCode.DROP
    /// 002E : OpCode.DROP
    /// 002F : OpCode.RET
    /// 0030 : OpCode.DUP
    /// 0031 : OpCode.PUSHDATA1 56616C756533
    /// 0039 : OpCode.EQUAL
    /// 003A : OpCode.JMPIFNOT 07
    /// 003C : OpCode.PUSH3
    /// 003D : OpCode.REVERSE3
    /// 003E : OpCode.DROP
    /// 003F : OpCode.DROP
    /// 0040 : OpCode.RET
    /// 0041 : OpCode.DROP
    /// 0042 : OpCode.PUSHDATA1 4E6F207375636820656E756D2076616C7565
    /// 0056 : OpCode.THROW
    /// </remarks>
    [DisplayName("testEnumParse")]
    public abstract object? TestEnumParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACDFRlc3RFbnVteHkmLgwQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhKeSY0DFZBTFVFMZcmB0VFRRFASnkmIAxWQUxVRTKXJgdFRUUSQEp5JgwMVkFMVUUzIgoMVmFsdWUzlyYHRUVFE0BFRQxObyBzdWNoIGVudW0gdmFsdWU6
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.PUSHDATA1 54657374456E756D
    /// 000D : OpCode.LDARG0
    /// 000E : OpCode.LDARG1
    /// 000F : OpCode.JMPIFNOT 2E
    /// 0011 : OpCode.PUSHDATA1
    /// 0013 : OpCode.PUSH0
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.LDARG0
    /// 0016 : OpCode.SIZE
    /// 0017 : OpCode.LT
    /// 0018 : OpCode.JMPIFNOT 22
    /// 001A : OpCode.DUP
    /// 001B : OpCode.LDARG0
    /// 001C : OpCode.SWAP
    /// 001D : OpCode.PICKITEM
    /// 001E : OpCode.DUP
    /// 001F : OpCode.PUSHINT8 61
    /// 0021 : OpCode.PUSHINT8 7B
    /// 0023 : OpCode.WITHIN
    /// 0024 : OpCode.JMPIF 09
    /// 0026 : OpCode.ROT
    /// 0027 : OpCode.SWAP
    /// 0028 : OpCode.CAT
    /// 0029 : OpCode.SWAP
    /// 002A : OpCode.INC
    /// 002B : OpCode.JMP E9
    /// 002D : OpCode.PUSHINT8 61
    /// 002F : OpCode.SUB
    /// 0030 : OpCode.PUSHINT8 41
    /// 0032 : OpCode.ADD
    /// 0033 : OpCode.ROT
    /// 0034 : OpCode.SWAP
    /// 0035 : OpCode.CAT
    /// 0036 : OpCode.SWAP
    /// 0037 : OpCode.INC
    /// 0038 : OpCode.JMP DC
    /// 003A : OpCode.DROP
    /// 003B : OpCode.CONVERT 28
    /// 003D : OpCode.DUP
    /// 003E : OpCode.LDARG1
    /// 003F : OpCode.JMPIFNOT 34
    /// 0041 : OpCode.PUSHDATA1 56414C554531
    /// 0049 : OpCode.EQUAL
    /// 004A : OpCode.JMPIFNOT 07
    /// 004C : OpCode.DROP
    /// 004D : OpCode.DROP
    /// 004E : OpCode.DROP
    /// 004F : OpCode.PUSH1
    /// 0050 : OpCode.RET
    /// 0051 : OpCode.DUP
    /// 0052 : OpCode.LDARG1
    /// 0053 : OpCode.JMPIFNOT 20
    /// 0055 : OpCode.PUSHDATA1 56414C554532
    /// 005D : OpCode.EQUAL
    /// 005E : OpCode.JMPIFNOT 07
    /// 0060 : OpCode.DROP
    /// 0061 : OpCode.DROP
    /// 0062 : OpCode.DROP
    /// 0063 : OpCode.PUSH2
    /// 0064 : OpCode.RET
    /// 0065 : OpCode.DUP
    /// 0066 : OpCode.LDARG1
    /// 0067 : OpCode.JMPIFNOT 0C
    /// 0069 : OpCode.PUSHDATA1 56414C554533
    /// 0071 : OpCode.JMP 0A
    /// 0073 : OpCode.PUSHDATA1 56616C756533
    /// 007B : OpCode.EQUAL
    /// 007C : OpCode.JMPIFNOT 07
    /// 007E : OpCode.DROP
    /// 007F : OpCode.DROP
    /// 0080 : OpCode.DROP
    /// 0081 : OpCode.PUSH3
    /// 0082 : OpCode.RET
    /// 0083 : OpCode.DROP
    /// 0084 : OpCode.DROP
    /// 0085 : OpCode.PUSHDATA1 4E6F207375636820656E756D2076616C7565
    /// 0099 : OpCode.THROW
    /// </remarks>
    [DisplayName("testEnumParseIgnoreCase")]
    public abstract object? TestEnumParseIgnoreCase(string? value, bool? ignoreCase);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABC2AMVGVzdEVudW14WEVKDFZhbHVlMZcmCEVFEWAIQEoMVmFsdWUylyYIRUUSYAhASgxWYWx1ZTOXJghFRRNgCEBFRRBgCUA=
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHNULL
    /// 0004 : OpCode.STSFLD0
    /// 0005 : OpCode.PUSHDATA1 54657374456E756D
    /// 000F : OpCode.LDARG0
    /// 0010 : OpCode.LDSFLD0
    /// 0011 : OpCode.DROP
    /// 0012 : OpCode.DUP
    /// 0013 : OpCode.PUSHDATA1 56616C756531
    /// 001B : OpCode.EQUAL
    /// 001C : OpCode.JMPIFNOT 08
    /// 001E : OpCode.DROP
    /// 001F : OpCode.DROP
    /// 0020 : OpCode.PUSH1
    /// 0021 : OpCode.STSFLD0
    /// 0022 : OpCode.PUSHT
    /// 0023 : OpCode.RET
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.PUSHDATA1 56616C756532
    /// 002D : OpCode.EQUAL
    /// 002E : OpCode.JMPIFNOT 08
    /// 0030 : OpCode.DROP
    /// 0031 : OpCode.DROP
    /// 0032 : OpCode.PUSH2
    /// 0033 : OpCode.STSFLD0
    /// 0034 : OpCode.PUSHT
    /// 0035 : OpCode.RET
    /// 0036 : OpCode.DUP
    /// 0037 : OpCode.PUSHDATA1 56616C756533
    /// 003F : OpCode.EQUAL
    /// 0040 : OpCode.JMPIFNOT 08
    /// 0042 : OpCode.DROP
    /// 0043 : OpCode.DROP
    /// 0044 : OpCode.PUSH3
    /// 0045 : OpCode.STSFLD0
    /// 0046 : OpCode.PUSHT
    /// 0047 : OpCode.RET
    /// 0048 : OpCode.DROP
    /// 0049 : OpCode.DROP
    /// 004A : OpCode.PUSH0
    /// 004B : OpCode.STSFLD0
    /// 004C : OpCode.PUSHF
    /// 004D : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumTryParse")]
    public abstract bool? TestEnumTryParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACC2EMVGVzdEVudW14eVlFJjBQRQwQSnjKtSYiSnhQzkoAYQB7uyQJUVCLUJwi6QBhnwBBnlFQi1CcItxF2yhKeSY2DFZBTFVFMZcmCEVFEWEIQEp5JiEMVkFMVUUylyYIRUUSYQhASnkmDAxWQUxVRTMiCgxWYWx1ZTOXJghFRRNhCEBFRRBhCUA=
    /// 0000 : OpCode.INITSLOT 0002
    /// 0003 : OpCode.PUSHNULL
    /// 0004 : OpCode.STSFLD1
    /// 0005 : OpCode.PUSHDATA1 54657374456E756D
    /// 000F : OpCode.LDARG0
    /// 0010 : OpCode.LDARG1
    /// 0011 : OpCode.LDSFLD1
    /// 0012 : OpCode.DROP
    /// 0013 : OpCode.JMPIFNOT 30
    /// 0015 : OpCode.SWAP
    /// 0016 : OpCode.DROP
    /// 0017 : OpCode.PUSHDATA1
    /// 0019 : OpCode.PUSH0
    /// 001A : OpCode.DUP
    /// 001B : OpCode.LDARG0
    /// 001C : OpCode.SIZE
    /// 001D : OpCode.LT
    /// 001E : OpCode.JMPIFNOT 22
    /// 0020 : OpCode.DUP
    /// 0021 : OpCode.LDARG0
    /// 0022 : OpCode.SWAP
    /// 0023 : OpCode.PICKITEM
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.PUSHINT8 61
    /// 0027 : OpCode.PUSHINT8 7B
    /// 0029 : OpCode.WITHIN
    /// 002A : OpCode.JMPIF 09
    /// 002C : OpCode.ROT
    /// 002D : OpCode.SWAP
    /// 002E : OpCode.CAT
    /// 002F : OpCode.SWAP
    /// 0030 : OpCode.INC
    /// 0031 : OpCode.JMP E9
    /// 0033 : OpCode.PUSHINT8 61
    /// 0035 : OpCode.SUB
    /// 0036 : OpCode.PUSHINT8 41
    /// 0038 : OpCode.ADD
    /// 0039 : OpCode.ROT
    /// 003A : OpCode.SWAP
    /// 003B : OpCode.CAT
    /// 003C : OpCode.SWAP
    /// 003D : OpCode.INC
    /// 003E : OpCode.JMP DC
    /// 0040 : OpCode.DROP
    /// 0041 : OpCode.CONVERT 28
    /// 0043 : OpCode.DUP
    /// 0044 : OpCode.LDARG1
    /// 0045 : OpCode.JMPIFNOT 36
    /// 0047 : OpCode.PUSHDATA1 56414C554531
    /// 004F : OpCode.EQUAL
    /// 0050 : OpCode.JMPIFNOT 08
    /// 0052 : OpCode.DROP
    /// 0053 : OpCode.DROP
    /// 0054 : OpCode.PUSH1
    /// 0055 : OpCode.STSFLD1
    /// 0056 : OpCode.PUSHT
    /// 0057 : OpCode.RET
    /// 0058 : OpCode.DUP
    /// 0059 : OpCode.LDARG1
    /// 005A : OpCode.JMPIFNOT 21
    /// 005C : OpCode.PUSHDATA1 56414C554532
    /// 0064 : OpCode.EQUAL
    /// 0065 : OpCode.JMPIFNOT 08
    /// 0067 : OpCode.DROP
    /// 0068 : OpCode.DROP
    /// 0069 : OpCode.PUSH2
    /// 006A : OpCode.STSFLD1
    /// 006B : OpCode.PUSHT
    /// 006C : OpCode.RET
    /// 006D : OpCode.DUP
    /// 006E : OpCode.LDARG1
    /// 006F : OpCode.JMPIFNOT 0C
    /// 0071 : OpCode.PUSHDATA1 56414C554533
    /// 0079 : OpCode.JMP 0A
    /// 007B : OpCode.PUSHDATA1 56616C756533
    /// 0083 : OpCode.EQUAL
    /// 0084 : OpCode.JMPIFNOT 08
    /// 0086 : OpCode.DROP
    /// 0087 : OpCode.DROP
    /// 0088 : OpCode.PUSH3
    /// 0089 : OpCode.STSFLD1
    /// 008A : OpCode.PUSHT
    /// 008B : OpCode.RET
    /// 008C : OpCode.DROP
    /// 008D : OpCode.DROP
    /// 008E : OpCode.PUSH0
    /// 008F : OpCode.STSFLD1
    /// 0090 : OpCode.PUSHF
    /// 0091 : OpCode.RET
    /// </remarks>
    [DisplayName("testEnumTryParseIgnoreCase")]
    public abstract bool? TestEnumTryParseIgnoreCase(string? value, bool? ignoreCase);

    #endregion

}
