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
    [DisplayName("test_CreateRecord")]
    public abstract object? Test_CreateRecord(string? n, BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : NEWSTRUCT0
    // 0004 : DUP
    // 0005 : PUSHNULL
    // 0006 : APPEND
    // 0007 : DUP
    // 0008 : PUSH0
    // 0009 : APPEND
    // 000A : LDARG1
    // 000B : LDARG0
    // 000C : PUSH2
    // 000D : PICK
    // 000E : CALL
    // 0010 : STLOC0
    // 0011 : LDLOC0
    // 0012 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test_CreateRecord2")]
    public abstract object? Test_CreateRecord2(string? n, BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : NEWSTRUCT0
    // 0004 : DUP
    // 0005 : PUSHNULL
    // 0006 : APPEND
    // 0007 : DUP
    // 0008 : PUSH0
    // 0009 : APPEND
    // 000A : LDARG0
    // 000B : OVER
    // 000C : CALL
    // 000E : LDARG1
    // 000F : OVER
    // 0010 : PUSH1
    // 0011 : ROT
    // 0012 : SETITEM
    // 0013 : STLOC0
    // 0014 : LDLOC0
    // 0015 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test_DeconstructRecord")]
    public abstract string? Test_DeconstructRecord(string? n, BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : NEWSTRUCT0
    // 0004 : DUP
    // 0005 : PUSHNULL
    // 0006 : APPEND
    // 0007 : DUP
    // 0008 : PUSH0
    // 0009 : APPEND
    // 000A : LDARG1
    // 000B : LDARG0
    // 000C : PUSH2
    // 000D : PICK
    // 000E : CALL_L
    // 0013 : STLOC0
    // 0014 : LDLOC0
    // 0015 : DUP
    // 0016 : UNPACK
    // 0017 : DROP
    // 0018 : STLOC1
    // 0019 : STLOC2
    // 001A : DROP
    // 001B : LDLOC1
    // 001C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test_UpdateRecord")]
    public abstract object? Test_UpdateRecord(string? n, BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : NEWSTRUCT0
    // 0004 : DUP
    // 0005 : PUSHNULL
    // 0006 : APPEND
    // 0007 : DUP
    // 0008 : PUSH0
    // 0009 : APPEND
    // 000A : LDARG1
    // 000B : LDARG0
    // 000C : PUSH2
    // 000D : PICK
    // 000E : CALL
    // 0010 : STLOC0
    // 0011 : LDLOC0
    // 0012 : UNPACK
    // 0013 : PACKSTRUCT
    // 0014 : LDARG1
    // 0015 : PUSH1
    // 0016 : ADD
    // 0017 : DUP
    // 0018 : PUSHINT32
    // 001D : JMPGE
    // 001F : JMP
    // 0021 : DUP
    // 0022 : PUSHINT32
    // 0027 : JMPLE
    // 0029 : PUSHINT64
    // 0032 : AND
    // 0033 : DUP
    // 0034 : PUSHINT32
    // 0039 : JMPLE
    // 003B : PUSHINT64
    // 0044 : SUB
    // 0045 : OVER
    // 0046 : CALL
    // 0048 : STLOC1
    // 0049 : LDLOC0
    // 004A : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("test_UpdateRecord2")]
    public abstract object? Test_UpdateRecord2(string? n, BigInteger? a);
    // 0000 : INITSLOT
    // 0003 : NEWSTRUCT0
    // 0004 : DUP
    // 0005 : PUSHNULL
    // 0006 : APPEND
    // 0007 : DUP
    // 0008 : PUSH0
    // 0009 : APPEND
    // 000A : LDARG1
    // 000B : LDARG0
    // 000C : PUSH2
    // 000D : PICK
    // 000E : CALL_L
    // 0013 : STLOC0
    // 0014 : LDLOC0
    // 0015 : UNPACK
    // 0016 : PACKSTRUCT
    // 0017 : LDARG1
    // 0018 : PUSH1
    // 0019 : ADD
    // 001A : DUP
    // 001B : PUSHINT32
    // 0020 : JMPGE
    // 0022 : JMP
    // 0024 : DUP
    // 0025 : PUSHINT32
    // 002A : JMPLE
    // 002C : PUSHINT64
    // 0035 : AND
    // 0036 : DUP
    // 0037 : PUSHINT32
    // 003C : JMPLE
    // 003E : PUSHINT64
    // 0047 : SUB
    // 0048 : OVER
    // 0049 : CALL
    // 004B : PUSHDATA1
    // 004E : LDARG0
    // 004F : CAT
    // 0050 : CONVERT
    // 0052 : OVER
    // 0053 : CALL
    // 0055 : STLOC1
    // 0056 : LDLOC1
    // 0057 : RET

    #endregion

}
