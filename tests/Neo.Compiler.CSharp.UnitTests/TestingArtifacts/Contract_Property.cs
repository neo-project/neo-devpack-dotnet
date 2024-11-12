using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Property(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Property"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""symbol"",""parameters"":[],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""testStaticPropertyDefaultInc"",""parameters"":[],""returntype"":""Integer"",""offset"":14,""safe"":false},{""name"":""testStaticPropertyValueInc"",""parameters"":[],""returntype"":""Integer"",""offset"":287,""safe"":false},{""name"":""testPropertyDefaultInc"",""parameters"":[],""returntype"":""Integer"",""offset"":449,""safe"":false},{""name"":""testPropertyValueInc"",""parameters"":[],""returntype"":""Integer"",""offset"":722,""safe"":false},{""name"":""incTestStaticFieldDefault"",""parameters"":[],""returntype"":""Integer"",""offset"":995,""safe"":false},{""name"":""incTestStaticFieldValue"",""parameters"":[],""returntype"":""Integer"",""offset"":1002,""safe"":false},{""name"":""incTestFieldDefault"",""parameters"":[],""returntype"":""Integer"",""offset"":1009,""safe"":false},{""name"":""incTestFieldValue"",""parameters"":[],""returntype"":""Integer"",""offset"":1016,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1022,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0SBAwLVG9rZW5TeW1ib2xADBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVgMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWBFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVkMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWFFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVhADBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVkMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWFFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVlADBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVoMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWJFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVoMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWJFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVpADBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVsMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWNFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVsMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWNFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVtAXEqcZEVcQF1KnGVFXUBeSpxmRV5AEmcHXwdAVggQZBFlEGYSZwcQYBphEGIbY0DaYtzl"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XkqcZkVeQA==
    /// 00 : OpCode.LDSFLD6 [2 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.INC [4 datoshi]
    /// 03 : OpCode.STSFLD6 [2 datoshi]
    /// 04 : OpCode.DROP [2 datoshi]
    /// 05 : OpCode.LDSFLD6 [2 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestFieldDefault")]
    public abstract BigInteger? IncTestFieldDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: EmcHXwdA
    /// 00 : OpCode.PUSH2 [1 datoshi]
    /// 01 : OpCode.STSFLD 07 [2 datoshi]
    /// 03 : OpCode.LDSFLD 07 [2 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestFieldValue")]
    public abstract BigInteger? IncTestFieldValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XEqcZEVcQA==
    /// 00 : OpCode.LDSFLD4 [2 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.INC [4 datoshi]
    /// 03 : OpCode.STSFLD4 [2 datoshi]
    /// 04 : OpCode.DROP [2 datoshi]
    /// 05 : OpCode.LDSFLD4 [2 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestStaticFieldDefault")]
    public abstract BigInteger? IncTestStaticFieldDefault();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: XUqcZUVdQA==
    /// 00 : OpCode.LDSFLD5 [2 datoshi]
    /// 01 : OpCode.DUP [2 datoshi]
    /// 02 : OpCode.INC [4 datoshi]
    /// 03 : OpCode.STSFLD5 [2 datoshi]
    /// 04 : OpCode.DROP [2 datoshi]
    /// 05 : OpCode.LDSFLD5 [2 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("incTestStaticFieldValue")]
    public abstract BigInteger? IncTestStaticFieldValue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("symbol")]
    public abstract string? Symbol();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVoMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWJFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVoMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWJFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVpA
    /// 0000 : OpCode.PUSHDATA1 787878782050726F7065727479476574 [8 datoshi]
    /// 0012 : OpCode.DROP [2 datoshi]
    /// 0013 : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479476574 [8 datoshi]
    /// 0030 : OpCode.DROP [2 datoshi]
    /// 0031 : OpCode.LDSFLD2 [2 datoshi]
    /// 0032 : OpCode.PUSHDATA1 7465737431 'test1' [8 datoshi]
    /// 0039 : OpCode.DROP [2 datoshi]
    /// 003A : OpCode.DUP [2 datoshi]
    /// 003B : OpCode.INC [4 datoshi]
    /// 003C : OpCode.PUSHDATA1 787878782050726F7065727479536574 [8 datoshi]
    /// 004E : OpCode.DROP [2 datoshi]
    /// 004F : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479536574 [8 datoshi]
    /// 006C : OpCode.DROP [2 datoshi]
    /// 006D : OpCode.STSFLD2 [2 datoshi]
    /// 006E : OpCode.DROP [2 datoshi]
    /// 006F : OpCode.PUSHDATA1 787878782050726F7065727479476574 [8 datoshi]
    /// 0081 : OpCode.DROP [2 datoshi]
    /// 0082 : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479476574 [8 datoshi]
    /// 009F : OpCode.DROP [2 datoshi]
    /// 00A0 : OpCode.LDSFLD2 [2 datoshi]
    /// 00A1 : OpCode.PUSHDATA1 7465737431 'test1' [8 datoshi]
    /// 00A8 : OpCode.DROP [2 datoshi]
    /// 00A9 : OpCode.DUP [2 datoshi]
    /// 00AA : OpCode.INC [4 datoshi]
    /// 00AB : OpCode.PUSHDATA1 787878782050726F7065727479536574 [8 datoshi]
    /// 00BD : OpCode.DROP [2 datoshi]
    /// 00BE : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479536574 [8 datoshi]
    /// 00DB : OpCode.DROP [2 datoshi]
    /// 00DC : OpCode.STSFLD2 [2 datoshi]
    /// 00DD : OpCode.DROP [2 datoshi]
    /// 00DE : OpCode.PUSHDATA1 787878782050726F7065727479476574 [8 datoshi]
    /// 00F0 : OpCode.DROP [2 datoshi]
    /// 00F1 : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479476574 [8 datoshi]
    /// 010E : OpCode.DROP [2 datoshi]
    /// 010F : OpCode.LDSFLD2 [2 datoshi]
    /// 0110 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyDefaultInc")]
    public abstract BigInteger? TestPropertyDefaultInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVsMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWNFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVsMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWNFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVtA
    /// 0000 : OpCode.PUSHDATA1 787878782050726F7065727479476574 [8 datoshi]
    /// 0012 : OpCode.DROP [2 datoshi]
    /// 0013 : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479476574 [8 datoshi]
    /// 0030 : OpCode.DROP [2 datoshi]
    /// 0031 : OpCode.LDSFLD3 [2 datoshi]
    /// 0032 : OpCode.PUSHDATA1 7465737431 'test1' [8 datoshi]
    /// 0039 : OpCode.DROP [2 datoshi]
    /// 003A : OpCode.DUP [2 datoshi]
    /// 003B : OpCode.INC [4 datoshi]
    /// 003C : OpCode.PUSHDATA1 787878782050726F7065727479536574 [8 datoshi]
    /// 004E : OpCode.DROP [2 datoshi]
    /// 004F : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479536574 [8 datoshi]
    /// 006C : OpCode.DROP [2 datoshi]
    /// 006D : OpCode.STSFLD3 [2 datoshi]
    /// 006E : OpCode.DROP [2 datoshi]
    /// 006F : OpCode.PUSHDATA1 787878782050726F7065727479476574 [8 datoshi]
    /// 0081 : OpCode.DROP [2 datoshi]
    /// 0082 : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479476574 [8 datoshi]
    /// 009F : OpCode.DROP [2 datoshi]
    /// 00A0 : OpCode.LDSFLD3 [2 datoshi]
    /// 00A1 : OpCode.PUSHDATA1 7465737431 'test1' [8 datoshi]
    /// 00A8 : OpCode.DROP [2 datoshi]
    /// 00A9 : OpCode.DUP [2 datoshi]
    /// 00AA : OpCode.INC [4 datoshi]
    /// 00AB : OpCode.PUSHDATA1 787878782050726F7065727479536574 [8 datoshi]
    /// 00BD : OpCode.DROP [2 datoshi]
    /// 00BE : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479536574 [8 datoshi]
    /// 00DB : OpCode.DROP [2 datoshi]
    /// 00DC : OpCode.STSFLD3 [2 datoshi]
    /// 00DD : OpCode.DROP [2 datoshi]
    /// 00DE : OpCode.PUSHDATA1 787878782050726F7065727479476574 [8 datoshi]
    /// 00F0 : OpCode.DROP [2 datoshi]
    /// 00F1 : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479476574 [8 datoshi]
    /// 010E : OpCode.DROP [2 datoshi]
    /// 010F : OpCode.LDSFLD3 [2 datoshi]
    /// 0110 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPropertyValueInc")]
    public abstract BigInteger? TestPropertyValueInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVgMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWBFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVkMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWFFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVhA
    /// 0000 : OpCode.PUSHDATA1 787878782050726F7065727479476574 [8 datoshi]
    /// 0012 : OpCode.DROP [2 datoshi]
    /// 0013 : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479476574 [8 datoshi]
    /// 0030 : OpCode.DROP [2 datoshi]
    /// 0031 : OpCode.LDSFLD0 [2 datoshi]
    /// 0032 : OpCode.PUSHDATA1 7465737431 'test1' [8 datoshi]
    /// 0039 : OpCode.DROP [2 datoshi]
    /// 003A : OpCode.DUP [2 datoshi]
    /// 003B : OpCode.INC [4 datoshi]
    /// 003C : OpCode.PUSHDATA1 787878782050726F7065727479536574 [8 datoshi]
    /// 004E : OpCode.DROP [2 datoshi]
    /// 004F : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479536574 [8 datoshi]
    /// 006C : OpCode.DROP [2 datoshi]
    /// 006D : OpCode.STSFLD0 [2 datoshi]
    /// 006E : OpCode.DROP [2 datoshi]
    /// 006F : OpCode.PUSHDATA1 787878782050726F7065727479476574 [8 datoshi]
    /// 0081 : OpCode.DROP [2 datoshi]
    /// 0082 : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479476574 [8 datoshi]
    /// 009F : OpCode.DROP [2 datoshi]
    /// 00A0 : OpCode.LDSFLD1 [2 datoshi]
    /// 00A1 : OpCode.PUSHDATA1 7465737431 'test1' [8 datoshi]
    /// 00A8 : OpCode.DROP [2 datoshi]
    /// 00A9 : OpCode.DUP [2 datoshi]
    /// 00AA : OpCode.INC [4 datoshi]
    /// 00AB : OpCode.PUSHDATA1 787878782050726F7065727479536574 [8 datoshi]
    /// 00BD : OpCode.DROP [2 datoshi]
    /// 00BE : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479536574 [8 datoshi]
    /// 00DB : OpCode.DROP [2 datoshi]
    /// 00DC : OpCode.STSFLD1 [2 datoshi]
    /// 00DD : OpCode.DROP [2 datoshi]
    /// 00DE : OpCode.PUSHDATA1 787878782050726F7065727479476574 [8 datoshi]
    /// 00F0 : OpCode.DROP [2 datoshi]
    /// 00F1 : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479476574 [8 datoshi]
    /// 010E : OpCode.DROP [2 datoshi]
    /// 010F : OpCode.LDSFLD0 [2 datoshi]
    /// 0110 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyDefaultInc")]
    public abstract BigInteger? TestStaticPropertyDefaultInc();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVkMBXRlc3QxRUqcDBB4eHh4IFByb3BlcnR5U2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5U2V0RWFFDBB4eHh4IFByb3BlcnR5R2V0RQwbU3RhdGljIHByb3BlcnR5IFByb3BlcnR5R2V0RVlA
    /// 00 : OpCode.PUSHDATA1 787878782050726F7065727479476574 [8 datoshi]
    /// 12 : OpCode.DROP [2 datoshi]
    /// 13 : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479476574 [8 datoshi]
    /// 30 : OpCode.DROP [2 datoshi]
    /// 31 : OpCode.LDSFLD1 [2 datoshi]
    /// 32 : OpCode.PUSHDATA1 7465737431 'test1' [8 datoshi]
    /// 39 : OpCode.DROP [2 datoshi]
    /// 3A : OpCode.DUP [2 datoshi]
    /// 3B : OpCode.INC [4 datoshi]
    /// 3C : OpCode.PUSHDATA1 787878782050726F7065727479536574 [8 datoshi]
    /// 4E : OpCode.DROP [2 datoshi]
    /// 4F : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479536574 [8 datoshi]
    /// 6C : OpCode.DROP [2 datoshi]
    /// 6D : OpCode.STSFLD1 [2 datoshi]
    /// 6E : OpCode.DROP [2 datoshi]
    /// 6F : OpCode.PUSHDATA1 787878782050726F7065727479476574 [8 datoshi]
    /// 81 : OpCode.DROP [2 datoshi]
    /// 82 : OpCode.PUSHDATA1 5374617469632070726F70657274792050726F7065727479476574 [8 datoshi]
    /// 9F : OpCode.DROP [2 datoshi]
    /// A0 : OpCode.LDSFLD1 [2 datoshi]
    /// A1 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testStaticPropertyValueInc")]
    public abstract BigInteger? TestStaticPropertyValueInc();

    #endregion
}
