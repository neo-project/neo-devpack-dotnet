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
    [DisplayName("testEnumGetName")]
    public abstract string? TestEnumGetName(BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH1
    // 0006 : EQUAL
    // 0007 : JMPIFNOT
    // 0009 : DROP
    // 000A : PUSHDATA1
    // 0012 : RET
    // 0013 : DUP
    // 0014 : PUSH2
    // 0015 : EQUAL
    // 0016 : JMPIFNOT
    // 0018 : DROP
    // 0019 : PUSHDATA1
    // 0021 : RET
    // 0022 : DUP
    // 0023 : PUSH3
    // 0024 : EQUAL
    // 0025 : JMPIFNOT
    // 0027 : DROP
    // 0028 : PUSHDATA1
    // 0030 : RET
    // 0031 : DROP
    // 0032 : PUSHNULL
    // 0033 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumGetNames")]
    public abstract IList<object>? TestEnumGetNames();
    // 0000 : PUSHDATA1
    // 000A : PUSH3
    // 000B : NEWARRAY
    // 000C : DUP
    // 000D : PUSH0
    // 000E : PUSHDATA1
    // 0016 : SETITEM
    // 0017 : DUP
    // 0018 : PUSH1
    // 0019 : PUSHDATA1
    // 0021 : SETITEM
    // 0022 : DUP
    // 0023 : PUSH2
    // 0024 : PUSHDATA1
    // 002C : SETITEM
    // 002D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumGetNameWithType")]
    public abstract string? TestEnumGetNameWithType(object? value = null);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 000D : LDARG0
    // 000E : DUP
    // 000F : PUSH1
    // 0010 : EQUAL
    // 0011 : JMPIFNOT
    // 0013 : DROP
    // 0014 : DROP
    // 0015 : PUSHDATA1
    // 001D : RET
    // 001E : DUP
    // 001F : PUSH2
    // 0020 : EQUAL
    // 0021 : JMPIFNOT
    // 0023 : DROP
    // 0024 : DROP
    // 0025 : PUSHDATA1
    // 002D : RET
    // 002E : DUP
    // 002F : PUSH3
    // 0030 : EQUAL
    // 0031 : JMPIFNOT
    // 0033 : DROP
    // 0034 : DROP
    // 0035 : PUSHDATA1
    // 003D : RET
    // 003E : DROP
    // 003F : DROP
    // 0040 : PUSHNULL
    // 0041 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumGetValues")]
    public abstract IList<object>? TestEnumGetValues();
    // 0000 : PUSHDATA1
    // 000A : PUSH3
    // 000B : NEWARRAY
    // 000C : DUP
    // 000D : PUSH0
    // 000E : PUSH1
    // 000F : SETITEM
    // 0010 : DUP
    // 0011 : PUSH1
    // 0012 : PUSH2
    // 0013 : SETITEM
    // 0014 : DUP
    // 0015 : PUSH2
    // 0016 : PUSH3
    // 0017 : SETITEM
    // 0018 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumIsDefined")]
    public abstract bool? TestEnumIsDefined(object? value = null);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 000D : LDARG0
    // 000E : DUP
    // 000F : PUSH1
    // 0010 : EQUAL
    // 0011 : JMPIFNOT
    // 0013 : DROP
    // 0014 : DROP
    // 0015 : PUSHT
    // 0016 : RET
    // 0017 : DUP
    // 0018 : PUSH2
    // 0019 : EQUAL
    // 001A : JMPIFNOT
    // 001C : DROP
    // 001D : DROP
    // 001E : PUSHT
    // 001F : RET
    // 0020 : DUP
    // 0021 : PUSH3
    // 0022 : EQUAL
    // 0023 : JMPIFNOT
    // 0025 : DROP
    // 0026 : DROP
    // 0027 : PUSHT
    // 0028 : RET
    // 0029 : DROP
    // 002A : DROP
    // 002B : PUSHF
    // 002C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumIsDefinedByName")]
    public abstract bool? TestEnumIsDefinedByName(string? name);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 000D : LDARG0
    // 000E : DUP
    // 000F : PUSHDATA1
    // 0017 : EQUAL
    // 0018 : JMPIFNOT
    // 001A : DROP
    // 001B : DROP
    // 001C : PUSHT
    // 001D : RET
    // 001E : DUP
    // 001F : PUSHDATA1
    // 0027 : EQUAL
    // 0028 : JMPIFNOT
    // 002A : DROP
    // 002B : DROP
    // 002C : PUSHT
    // 002D : RET
    // 002E : DUP
    // 002F : PUSHDATA1
    // 0037 : EQUAL
    // 0038 : JMPIFNOT
    // 003A : DROP
    // 003B : DROP
    // 003C : PUSHT
    // 003D : RET
    // 003E : DROP
    // 003F : DROP
    // 0040 : PUSHF
    // 0041 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumParse")]
    public abstract object? TestEnumParse(string? value);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 000D : LDARG0
    // 000E : DUP
    // 000F : PUSHDATA1
    // 0017 : EQUAL
    // 0018 : JMPIFNOT
    // 001A : PUSH1
    // 001B : REVERSE3
    // 001C : DROP
    // 001D : DROP
    // 001E : RET
    // 001F : DUP
    // 0020 : PUSHDATA1
    // 0028 : EQUAL
    // 0029 : JMPIFNOT
    // 002B : PUSH2
    // 002C : REVERSE3
    // 002D : DROP
    // 002E : DROP
    // 002F : RET
    // 0030 : DUP
    // 0031 : PUSHDATA1
    // 0039 : EQUAL
    // 003A : JMPIFNOT
    // 003C : PUSH3
    // 003D : REVERSE3
    // 003E : DROP
    // 003F : DROP
    // 0040 : RET
    // 0041 : DROP
    // 0042 : PUSHDATA1
    // 0056 : THROW

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumParseIgnoreCase")]
    public abstract object? TestEnumParseIgnoreCase(string? value, bool? ignoreCase);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 000D : LDARG0
    // 000E : LDARG1
    // 000F : JMPIFNOT
    // 0011 : PUSHDATA1
    // 0013 : PUSH0
    // 0014 : DUP
    // 0015 : LDARG0
    // 0016 : SIZE
    // 0017 : LT
    // 0018 : JMPIFNOT
    // 001A : DUP
    // 001B : LDARG0
    // 001C : SWAP
    // 001D : PICKITEM
    // 001E : DUP
    // 001F : PUSHINT8
    // 0021 : PUSHINT8
    // 0023 : WITHIN
    // 0024 : JMPIF
    // 0026 : ROT
    // 0027 : SWAP
    // 0028 : CAT
    // 0029 : SWAP
    // 002A : INC
    // 002B : JMP
    // 002D : PUSHINT8
    // 002F : SUB
    // 0030 : PUSHINT8
    // 0032 : ADD
    // 0033 : ROT
    // 0034 : SWAP
    // 0035 : CAT
    // 0036 : SWAP
    // 0037 : INC
    // 0038 : JMP
    // 003A : DROP
    // 003B : CONVERT
    // 003D : DUP
    // 003E : LDARG1
    // 003F : JMPIFNOT
    // 0041 : PUSHDATA1
    // 0049 : EQUAL
    // 004A : JMPIFNOT
    // 004C : DROP
    // 004D : DROP
    // 004E : DROP
    // 004F : PUSH1
    // 0050 : RET
    // 0051 : DUP
    // 0052 : LDARG1
    // 0053 : JMPIFNOT
    // 0055 : PUSHDATA1
    // 005D : EQUAL
    // 005E : JMPIFNOT
    // 0060 : DROP
    // 0061 : DROP
    // 0062 : DROP
    // 0063 : PUSH2
    // 0064 : RET
    // 0065 : DUP
    // 0066 : LDARG1
    // 0067 : JMPIFNOT
    // 0069 : PUSHDATA1
    // 0071 : JMP
    // 0073 : PUSHDATA1
    // 007B : EQUAL
    // 007C : JMPIFNOT
    // 007E : DROP
    // 007F : DROP
    // 0080 : DROP
    // 0081 : PUSH3
    // 0082 : RET
    // 0083 : DROP
    // 0084 : DROP
    // 0085 : PUSHDATA1
    // 0099 : THROW

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumTryParse")]
    public abstract bool? TestEnumTryParse(string? value);
    // 0000 : INITSLOT
    // 0003 : PUSHNULL
    // 0004 : STSFLD0
    // 0005 : PUSHDATA1
    // 000F : LDARG0
    // 0010 : LDSFLD0
    // 0011 : DROP
    // 0012 : DUP
    // 0013 : PUSHDATA1
    // 001B : EQUAL
    // 001C : JMPIFNOT
    // 001E : DROP
    // 001F : DROP
    // 0020 : PUSH1
    // 0021 : STSFLD0
    // 0022 : PUSHT
    // 0023 : RET
    // 0024 : DUP
    // 0025 : PUSHDATA1
    // 002D : EQUAL
    // 002E : JMPIFNOT
    // 0030 : DROP
    // 0031 : DROP
    // 0032 : PUSH2
    // 0033 : STSFLD0
    // 0034 : PUSHT
    // 0035 : RET
    // 0036 : DUP
    // 0037 : PUSHDATA1
    // 003F : EQUAL
    // 0040 : JMPIFNOT
    // 0042 : DROP
    // 0043 : DROP
    // 0044 : PUSH3
    // 0045 : STSFLD0
    // 0046 : PUSHT
    // 0047 : RET
    // 0048 : DROP
    // 0049 : DROP
    // 004A : PUSH0
    // 004B : STSFLD0
    // 004C : PUSHF
    // 004D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEnumTryParseIgnoreCase")]
    public abstract bool? TestEnumTryParseIgnoreCase(string? value, bool? ignoreCase);
    // 0000 : INITSLOT
    // 0003 : PUSHNULL
    // 0004 : STSFLD1
    // 0005 : PUSHDATA1
    // 000F : LDARG0
    // 0010 : LDARG1
    // 0011 : LDSFLD1
    // 0012 : DROP
    // 0013 : JMPIFNOT
    // 0015 : SWAP
    // 0016 : DROP
    // 0017 : PUSHDATA1
    // 0019 : PUSH0
    // 001A : DUP
    // 001B : LDARG0
    // 001C : SIZE
    // 001D : LT
    // 001E : JMPIFNOT
    // 0020 : DUP
    // 0021 : LDARG0
    // 0022 : SWAP
    // 0023 : PICKITEM
    // 0024 : DUP
    // 0025 : PUSHINT8
    // 0027 : PUSHINT8
    // 0029 : WITHIN
    // 002A : JMPIF
    // 002C : ROT
    // 002D : SWAP
    // 002E : CAT
    // 002F : SWAP
    // 0030 : INC
    // 0031 : JMP
    // 0033 : PUSHINT8
    // 0035 : SUB
    // 0036 : PUSHINT8
    // 0038 : ADD
    // 0039 : ROT
    // 003A : SWAP
    // 003B : CAT
    // 003C : SWAP
    // 003D : INC
    // 003E : JMP
    // 0040 : DROP
    // 0041 : CONVERT
    // 0043 : DUP
    // 0044 : LDARG1
    // 0045 : JMPIFNOT
    // 0047 : PUSHDATA1
    // 004F : EQUAL
    // 0050 : JMPIFNOT
    // 0052 : DROP
    // 0053 : DROP
    // 0054 : PUSH1
    // 0055 : STSFLD1
    // 0056 : PUSHT
    // 0057 : RET
    // 0058 : DUP
    // 0059 : LDARG1
    // 005A : JMPIFNOT
    // 005C : PUSHDATA1
    // 0064 : EQUAL
    // 0065 : JMPIFNOT
    // 0067 : DROP
    // 0068 : DROP
    // 0069 : PUSH2
    // 006A : STSFLD1
    // 006B : PUSHT
    // 006C : RET
    // 006D : DUP
    // 006E : LDARG1
    // 006F : JMPIFNOT
    // 0071 : PUSHDATA1
    // 0079 : JMP
    // 007B : PUSHDATA1
    // 0083 : EQUAL
    // 0084 : JMPIFNOT
    // 0086 : DROP
    // 0087 : DROP
    // 0088 : PUSH3
    // 0089 : STSFLD1
    // 008A : PUSHT
    // 008B : RET
    // 008C : DROP
    // 008D : DROP
    // 008E : PUSH0
    // 008F : STSFLD1
    // 0090 : PUSHF
    // 0091 : RET

    #endregion

}
