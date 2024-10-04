using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Record(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Record"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""test_CreateRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":0,""safe"":false},{""name"":""test_CreateRecord2"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":31,""safe"":false},{""name"":""test_UpdateRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":64,""safe"":false},{""name"":""test_UpdateRecord2"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""Any"",""offset"":147,""safe"":false},{""name"":""test_DeconstructRecord"",""parameters"":[{""name"":""n"",""type"":""String""},{""name"":""a"",""type"":""Integer""}],""returntype"":""String"",""offset"":243,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0QAVcBAsVKC89KEM95eBJNNAVwaEBXAAN4EHnQeBF60EBXAQLFSgvPShDPeEs0CnlLEVHQcGhAVwACeUp4EFHQRUBXAgLFSgvPShDPeXgSTTTFcGjBv3kRnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9LNAVxaEBXAAJ4EXnQQFcCAsVKC89KEM95eBJNNXL///9waMG/eRGeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0s0rwwBMHiL2yhLNAVxaUBXAAJ4EHnQQFcDAsVKC89KEM95eBJNNRL///9waErBRXFyRWlAmNdg3g=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0102
    /// 0003 : OpCode.NEWSTRUCT0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHNULL
    /// 0006 : OpCode.APPEND
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.PUSH0
    /// 0009 : OpCode.APPEND
    /// 000A : OpCode.LDARG1
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.PUSH2
    /// 000D : OpCode.PICK
    /// 000E : OpCode.CALL 05
    /// 0010 : OpCode.STLOC0
    /// 0011 : OpCode.LDLOC0
    /// 0012 : OpCode.RET
    /// </remarks>
    [DisplayName("test_CreateRecord")]
    public abstract object? Test_CreateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0102
    /// 0003 : OpCode.NEWSTRUCT0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHNULL
    /// 0006 : OpCode.APPEND
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.PUSH0
    /// 0009 : OpCode.APPEND
    /// 000A : OpCode.LDARG0
    /// 000B : OpCode.OVER
    /// 000C : OpCode.CALL 0A
    /// 000E : OpCode.LDARG1
    /// 000F : OpCode.OVER
    /// 0010 : OpCode.PUSH1
    /// 0011 : OpCode.ROT
    /// 0012 : OpCode.SETITEM
    /// 0013 : OpCode.STLOC0
    /// 0014 : OpCode.LDLOC0
    /// 0015 : OpCode.RET
    /// </remarks>
    [DisplayName("test_CreateRecord2")]
    public abstract object? Test_CreateRecord2(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0302
    /// 0003 : OpCode.NEWSTRUCT0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHNULL
    /// 0006 : OpCode.APPEND
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.PUSH0
    /// 0009 : OpCode.APPEND
    /// 000A : OpCode.LDARG1
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.PUSH2
    /// 000D : OpCode.PICK
    /// 000E : OpCode.CALL_L 12FFFFFF
    /// 0013 : OpCode.STLOC0
    /// 0014 : OpCode.LDLOC0
    /// 0015 : OpCode.DUP
    /// 0016 : OpCode.UNPACK
    /// 0017 : OpCode.DROP
    /// 0018 : OpCode.STLOC1
    /// 0019 : OpCode.STLOC2
    /// 001A : OpCode.DROP
    /// 001B : OpCode.LDLOC1
    /// 001C : OpCode.RET
    /// </remarks>
    [DisplayName("test_DeconstructRecord")]
    public abstract string? Test_DeconstructRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0202
    /// 0003 : OpCode.NEWSTRUCT0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHNULL
    /// 0006 : OpCode.APPEND
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.PUSH0
    /// 0009 : OpCode.APPEND
    /// 000A : OpCode.LDARG1
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.PUSH2
    /// 000D : OpCode.PICK
    /// 000E : OpCode.CALL C5
    /// 0010 : OpCode.STLOC0
    /// 0011 : OpCode.LDLOC0
    /// 0012 : OpCode.UNPACK
    /// 0013 : OpCode.PACKSTRUCT
    /// 0014 : OpCode.LDARG1
    /// 0015 : OpCode.PUSH1
    /// 0016 : OpCode.ADD
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.PUSHINT32 00000080
    /// 001D : OpCode.JMPGE 04
    /// 001F : OpCode.JMP 0A
    /// 0021 : OpCode.DUP
    /// 0022 : OpCode.PUSHINT32 FFFFFF7F
    /// 0027 : OpCode.JMPLE 1E
    /// 0029 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0032 : OpCode.AND
    /// 0033 : OpCode.DUP
    /// 0034 : OpCode.PUSHINT32 FFFFFF7F
    /// 0039 : OpCode.JMPLE 0C
    /// 003B : OpCode.PUSHINT64 0000000001000000
    /// 0044 : OpCode.SUB
    /// 0045 : OpCode.OVER
    /// 0046 : OpCode.CALL 05
    /// 0048 : OpCode.STLOC1
    /// 0049 : OpCode.LDLOC0
    /// 004A : OpCode.RET
    /// </remarks>
    [DisplayName("test_UpdateRecord")]
    public abstract object? Test_UpdateRecord(string? n, BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0202
    /// 0003 : OpCode.NEWSTRUCT0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHNULL
    /// 0006 : OpCode.APPEND
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.PUSH0
    /// 0009 : OpCode.APPEND
    /// 000A : OpCode.LDARG1
    /// 000B : OpCode.LDARG0
    /// 000C : OpCode.PUSH2
    /// 000D : OpCode.PICK
    /// 000E : OpCode.CALL_L 72FFFFFF
    /// 0013 : OpCode.STLOC0
    /// 0014 : OpCode.LDLOC0
    /// 0015 : OpCode.UNPACK
    /// 0016 : OpCode.PACKSTRUCT
    /// 0017 : OpCode.LDARG1
    /// 0018 : OpCode.PUSH1
    /// 0019 : OpCode.ADD
    /// 001A : OpCode.DUP
    /// 001B : OpCode.PUSHINT32 00000080
    /// 0020 : OpCode.JMPGE 04
    /// 0022 : OpCode.JMP 0A
    /// 0024 : OpCode.DUP
    /// 0025 : OpCode.PUSHINT32 FFFFFF7F
    /// 002A : OpCode.JMPLE 1E
    /// 002C : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0035 : OpCode.AND
    /// 0036 : OpCode.DUP
    /// 0037 : OpCode.PUSHINT32 FFFFFF7F
    /// 003C : OpCode.JMPLE 0C
    /// 003E : OpCode.PUSHINT64 0000000001000000
    /// 0047 : OpCode.SUB
    /// 0048 : OpCode.OVER
    /// 0049 : OpCode.CALL AF
    /// 004B : OpCode.PUSHDATA1 30
    /// 004E : OpCode.LDARG0
    /// 004F : OpCode.CAT
    /// 0050 : OpCode.CONVERT 28
    /// 0052 : OpCode.OVER
    /// 0053 : OpCode.CALL 05
    /// 0055 : OpCode.STLOC1
    /// 0056 : OpCode.LDLOC1
    /// 0057 : OpCode.RET
    /// </remarks>
    [DisplayName("test_UpdateRecord2")]
    public abstract object? Test_UpdateRecord2(string? n, BigInteger? a);

    #endregion

}
