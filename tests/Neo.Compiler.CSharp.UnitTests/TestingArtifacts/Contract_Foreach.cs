using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Foreach(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Foreach"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""intForeach"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""stringForeach"",""parameters"":[],""returntype"":""String"",""offset"":84,""safe"":false},{""name"":""byteStringEmpty"",""parameters"":[],""returntype"":""Integer"",""offset"":136,""safe"":false},{""name"":""byteStringForeach"",""parameters"":[],""returntype"":""ByteArray"",""offset"":145,""safe"":false},{""name"":""structForeach"",""parameters"":[],""returntype"":""Map"",""offset"":201,""safe"":false},{""name"":""byteArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":313,""safe"":false},{""name"":""uInt160Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":351,""safe"":false},{""name"":""uInt256Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":428,""safe"":false},{""name"":""eCPointForeach"",""parameters"":[],""returntype"":""Array"",""offset"":529,""safe"":false},{""name"":""bigIntegerForeach"",""parameters"":[],""returntype"":""Array"",""offset"":590,""safe"":false},{""name"":""objectArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":645,""safe"":false},{""name"":""intForeachBreak"",""parameters"":[{""name"":""breakIndex"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":692,""safe"":false},{""name"":""testContinue"",""parameters"":[],""returntype"":""Integer"",""offset"":846,""safe"":false},{""name"":""intForloop"",""parameters"":[],""returntype"":""Integer"",""offset"":948,""safe"":false},{""name"":""testIteratorForEach"",""parameters"":[],""returntype"":""Void"",""offset"":1075,""safe"":false},{""name"":""testForEachVariable"",""parameters"":[],""returntype"":""Void"",""offset"":1181,""safe"":false},{""name"":""testDo"",""parameters"":[],""returntype"":""Void"",""offset"":1227,""safe"":false},{""name"":""testWhile"",""parameters"":[],""returntype"":""Void"",""offset"":1298,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1371,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHA7znO4OTpJcbCoGp54UQN2G/OrARpdG9hAQABDwAA/ZQFVwYAFBMSERTAcBBxaEpyynMQdCI7amzOdWltnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xbJx0bGswxWlAVwYADANoaWoMA2RlZgwDYWJjE8BwDABxaEpyynMQdCIPamzOdWlti9socWycdGxrMPFpQFcBAAwAcGjKQFcGAAwADAAMA2hpagwDZGVmDANhYmMVwHAMAHFoSnLKcxB0Ig9qbM51aW2L2yhxbJx0bGsw8WlAVwgAxUoLz0oQz0o0YXAMBXRlc3QxSmgQUdBFEUpoEVHQRcVKC89KEM9KNEJxDAV0ZXN0MkppEFHQRRJKaRFR0EVpaBLAcshzakp0ynUQdiIXbG7OdwdvBxHOSm8HEM5rU9BFbpx2bm0w6WtAVwABQFcGAAwDAQoR2zBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlAVwYADBQAAAAAAAAAAAAAAAAAAAAAAAAAAAwUAAAAAAAAAAAAAAAAAAAAAAAAAAASwHDCcWhKcspzEHQiDGpsznVpbc9snHRsazD0aUBXBgAMIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADCAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABLAcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQFcGAFjbKErYJAlKygAhKAM6WNsoStgkCUrKACEoAzoSwHDCcWhKcspzEHQiDGpsznVpbc9snHRsazD0aUBXBgADAABkp7O24A0CAMqaOwJAQg8AARAnFMBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlAVwYAAHsMBHRlc3QMAgEC2zATwHDCcWhKcspzEHQiDGpsznVpbc9snHRsazD0aUBXBgEUExIRFMBwEHE8iQAAAAAAAABoSnLKcxB0InNqbM51eEqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4AQtiYEIjtpbZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWycdGxrMI09BXI9AmlAVwYAFRQTEhEVwHAQcTtUAGhKcspzEHQiRGpsznVtEqIQlyYEIjRpbZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWycdGxrMLw9BXI9AmlAVwMAFBMSERTAcBBxEHIiaWloas6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FqSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfckVqaMq1JJVpQFcDABNBm/ZnzhMRiE4QUdBQEsDBRUHfMLiacGhxIhFpQfNUvx1yatsoQc/nR5ZpQZwI7Zwk60DFShDPSgvPSlnPDAV3b3JsZBISTTQbxUoQz0oLz0pZzwwFaGVsbG8REk00BRLAQFcAAUBXBQA0ykpwynEQciIeaGrOwUVzdGs3AAAMAjogi2yL2yhBz+dHlmqccmppMOJAVwEAEHBoNwAAQc/nR5ZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoFbUkwUBXAQAQcGgVtSZAaDcAAEHP50eWaEqcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3BFIr9AVgIMIQJHANsukNnwLE+fyGKrrKknJflbT93MjX/6U4aT7PRjqWAKAAAAAAoAAAAACgAAAAATwGFAuhdxtQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("bigIntegerForeach")]
    public abstract IList<object>? BigIntegerForeach();
    // 0000 : INITSLOT
    // 0003 : PUSHINT64
    // 000C : PUSHINT32
    // 0011 : PUSHINT32
    // 0016 : PUSHINT16
    // 0019 : PUSH4
    // 001A : PACK
    // 001B : STLOC0
    // 001C : NEWARRAY0
    // 001D : STLOC1
    // 001E : LDLOC0
    // 001F : DUP
    // 0020 : STLOC2
    // 0021 : SIZE
    // 0022 : STLOC3
    // 0023 : PUSH0
    // 0024 : STLOC4
    // 0025 : JMP
    // 0027 : LDLOC2
    // 0028 : LDLOC4
    // 0029 : PICKITEM
    // 002A : STLOC5
    // 002B : LDLOC1
    // 002C : LDLOC5
    // 002D : APPEND
    // 002E : LDLOC4
    // 002F : INC
    // 0030 : STLOC4
    // 0031 : LDLOC4
    // 0032 : LDLOC3
    // 0033 : JMPLT
    // 0035 : LDLOC1
    // 0036 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteArrayForeach")]
    public abstract IList<object>? ByteArrayForeach();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0008 : CONVERT
    // 000A : STLOC0
    // 000B : NEWARRAY0
    // 000C : STLOC1
    // 000D : LDLOC0
    // 000E : DUP
    // 000F : STLOC2
    // 0010 : SIZE
    // 0011 : STLOC3
    // 0012 : PUSH0
    // 0013 : STLOC4
    // 0014 : JMP
    // 0016 : LDLOC2
    // 0017 : LDLOC4
    // 0018 : PICKITEM
    // 0019 : STLOC5
    // 001A : LDLOC1
    // 001B : LDLOC5
    // 001C : APPEND
    // 001D : LDLOC4
    // 001E : INC
    // 001F : STLOC4
    // 0020 : LDLOC4
    // 0021 : LDLOC3
    // 0022 : JMPLT
    // 0024 : LDLOC1
    // 0025 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteStringEmpty")]
    public abstract BigInteger? ByteStringEmpty();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0005 : STLOC0
    // 0006 : LDLOC0
    // 0007 : SIZE
    // 0008 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("byteStringForeach")]
    public abstract byte[]? ByteStringForeach();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0005 : PUSHDATA1
    // 0007 : PUSHDATA1
    // 000C : PUSHDATA1
    // 0011 : PUSHDATA1
    // 0016 : PUSH5
    // 0017 : PACK
    // 0018 : STLOC0
    // 0019 : PUSHDATA1
    // 001B : STLOC1
    // 001C : LDLOC0
    // 001D : DUP
    // 001E : STLOC2
    // 001F : SIZE
    // 0020 : STLOC3
    // 0021 : PUSH0
    // 0022 : STLOC4
    // 0023 : JMP
    // 0025 : LDLOC2
    // 0026 : LDLOC4
    // 0027 : PICKITEM
    // 0028 : STLOC5
    // 0029 : LDLOC1
    // 002A : LDLOC5
    // 002B : CAT
    // 002C : CONVERT
    // 002E : STLOC1
    // 002F : LDLOC4
    // 0030 : INC
    // 0031 : STLOC4
    // 0032 : LDLOC4
    // 0033 : LDLOC3
    // 0034 : JMPLT
    // 0036 : LDLOC1
    // 0037 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("eCPointForeach")]
    public abstract IList<object>? ECPointForeach();
    // 0000 : INITSLOT
    // 0003 : LDSFLD0
    // 0004 : CONVERT
    // 0006 : DUP
    // 0007 : ISNULL
    // 0008 : JMPIF
    // 000A : DUP
    // 000B : SIZE
    // 000C : PUSHINT8
    // 000E : JMPEQ
    // 0010 : THROW
    // 0011 : LDSFLD0
    // 0012 : CONVERT
    // 0014 : DUP
    // 0015 : ISNULL
    // 0016 : JMPIF
    // 0018 : DUP
    // 0019 : SIZE
    // 001A : PUSHINT8
    // 001C : JMPEQ
    // 001E : THROW
    // 001F : PUSH2
    // 0020 : PACK
    // 0021 : STLOC0
    // 0022 : NEWARRAY0
    // 0023 : STLOC1
    // 0024 : LDLOC0
    // 0025 : DUP
    // 0026 : STLOC2
    // 0027 : SIZE
    // 0028 : STLOC3
    // 0029 : PUSH0
    // 002A : STLOC4
    // 002B : JMP
    // 002D : LDLOC2
    // 002E : LDLOC4
    // 002F : PICKITEM
    // 0030 : STLOC5
    // 0031 : LDLOC1
    // 0032 : LDLOC5
    // 0033 : APPEND
    // 0034 : LDLOC4
    // 0035 : INC
    // 0036 : STLOC4
    // 0037 : LDLOC4
    // 0038 : LDLOC3
    // 0039 : JMPLT
    // 003B : LDLOC1
    // 003C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intForeach")]
    public abstract BigInteger? IntForeach();
    // 0000 : INITSLOT
    // 0003 : PUSH4
    // 0004 : PUSH3
    // 0005 : PUSH2
    // 0006 : PUSH1
    // 0007 : PUSH4
    // 0008 : PACK
    // 0009 : STLOC0
    // 000A : PUSH0
    // 000B : STLOC1
    // 000C : LDLOC0
    // 000D : DUP
    // 000E : STLOC2
    // 000F : SIZE
    // 0010 : STLOC3
    // 0011 : PUSH0
    // 0012 : STLOC4
    // 0013 : JMP
    // 0015 : LDLOC2
    // 0016 : LDLOC4
    // 0017 : PICKITEM
    // 0018 : STLOC5
    // 0019 : LDLOC1
    // 001A : LDLOC5
    // 001B : ADD
    // 001C : DUP
    // 001D : PUSHINT32
    // 0022 : JMPGE
    // 0024 : JMP
    // 0026 : DUP
    // 0027 : PUSHINT32
    // 002C : JMPLE
    // 002E : PUSHINT64
    // 0037 : AND
    // 0038 : DUP
    // 0039 : PUSHINT32
    // 003E : JMPLE
    // 0040 : PUSHINT64
    // 0049 : SUB
    // 004A : STLOC1
    // 004B : LDLOC4
    // 004C : INC
    // 004D : STLOC4
    // 004E : LDLOC4
    // 004F : LDLOC3
    // 0050 : JMPLT
    // 0052 : LDLOC1
    // 0053 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intForeachBreak")]
    public abstract BigInteger? IntForeachBreak(BigInteger? breakIndex);
    // 0000 : INITSLOT
    // 0003 : PUSH4
    // 0004 : PUSH3
    // 0005 : PUSH2
    // 0006 : PUSH1
    // 0007 : PUSH4
    // 0008 : PACK
    // 0009 : STLOC0
    // 000A : PUSH0
    // 000B : STLOC1
    // 000C : TRY_L
    // 0015 : LDLOC0
    // 0016 : DUP
    // 0017 : STLOC2
    // 0018 : SIZE
    // 0019 : STLOC3
    // 001A : PUSH0
    // 001B : STLOC4
    // 001C : JMP
    // 001E : LDLOC2
    // 001F : LDLOC4
    // 0020 : PICKITEM
    // 0021 : STLOC5
    // 0022 : LDARG0
    // 0023 : DUP
    // 0024 : DEC
    // 0025 : DUP
    // 0026 : PUSHINT32
    // 002B : JMPGE
    // 002D : JMP
    // 002F : DUP
    // 0030 : PUSHINT32
    // 0035 : JMPLE
    // 0037 : PUSHINT64
    // 0040 : AND
    // 0041 : DUP
    // 0042 : PUSHINT32
    // 0047 : JMPLE
    // 0049 : PUSHINT64
    // 0052 : SUB
    // 0053 : STARG0
    // 0054 : PUSH0
    // 0055 : LE
    // 0056 : JMPIFNOT
    // 0058 : JMP
    // 005A : LDLOC1
    // 005B : LDLOC5
    // 005C : ADD
    // 005D : DUP
    // 005E : PUSHINT32
    // 0063 : JMPGE
    // 0065 : JMP
    // 0067 : DUP
    // 0068 : PUSHINT32
    // 006D : JMPLE
    // 006F : PUSHINT64
    // 0078 : AND
    // 0079 : DUP
    // 007A : PUSHINT32
    // 007F : JMPLE
    // 0081 : PUSHINT64
    // 008A : SUB
    // 008B : STLOC1
    // 008C : LDLOC4
    // 008D : INC
    // 008E : STLOC4
    // 008F : LDLOC4
    // 0090 : LDLOC3
    // 0091 : JMPLT
    // 0093 : ENDTRY
    // 0095 : STLOC2
    // 0096 : ENDTRY
    // 0098 : LDLOC1
    // 0099 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("intForloop")]
    public abstract BigInteger? IntForloop();
    // 0000 : INITSLOT
    // 0003 : PUSH4
    // 0004 : PUSH3
    // 0005 : PUSH2
    // 0006 : PUSH1
    // 0007 : PUSH4
    // 0008 : PACK
    // 0009 : STLOC0
    // 000A : PUSH0
    // 000B : STLOC1
    // 000C : PUSH0
    // 000D : STLOC2
    // 000E : JMP
    // 0010 : LDLOC1
    // 0011 : LDLOC0
    // 0012 : LDLOC2
    // 0013 : PICKITEM
    // 0014 : ADD
    // 0015 : DUP
    // 0016 : PUSHINT32
    // 001B : JMPGE
    // 001D : JMP
    // 001F : DUP
    // 0020 : PUSHINT32
    // 0025 : JMPLE
    // 0027 : PUSHINT64
    // 0030 : AND
    // 0031 : DUP
    // 0032 : PUSHINT32
    // 0037 : JMPLE
    // 0039 : PUSHINT64
    // 0042 : SUB
    // 0043 : STLOC1
    // 0044 : LDLOC2
    // 0045 : DUP
    // 0046 : INC
    // 0047 : DUP
    // 0048 : PUSHINT32
    // 004D : JMPGE
    // 004F : JMP
    // 0051 : DUP
    // 0052 : PUSHINT32
    // 0057 : JMPLE
    // 0059 : PUSHINT64
    // 0062 : AND
    // 0063 : DUP
    // 0064 : PUSHINT32
    // 0069 : JMPLE
    // 006B : PUSHINT64
    // 0074 : SUB
    // 0075 : STLOC2
    // 0076 : DROP
    // 0077 : LDLOC2
    // 0078 : LDLOC0
    // 0079 : SIZE
    // 007A : LT
    // 007B : JMPIF
    // 007D : LDLOC1
    // 007E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("objectArrayForeach")]
    public abstract IList<object>? ObjectArrayForeach();
    // 0000 : INITSLOT
    // 0003 : PUSHINT8
    // 0005 : PUSHDATA1
    // 000B : PUSHDATA1
    // 000F : CONVERT
    // 0011 : PUSH3
    // 0012 : PACK
    // 0013 : STLOC0
    // 0014 : NEWARRAY0
    // 0015 : STLOC1
    // 0016 : LDLOC0
    // 0017 : DUP
    // 0018 : STLOC2
    // 0019 : SIZE
    // 001A : STLOC3
    // 001B : PUSH0
    // 001C : STLOC4
    // 001D : JMP
    // 001F : LDLOC2
    // 0020 : LDLOC4
    // 0021 : PICKITEM
    // 0022 : STLOC5
    // 0023 : LDLOC1
    // 0024 : LDLOC5
    // 0025 : APPEND
    // 0026 : LDLOC4
    // 0027 : INC
    // 0028 : STLOC4
    // 0029 : LDLOC4
    // 002A : LDLOC3
    // 002B : JMPLT
    // 002D : LDLOC1
    // 002E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("stringForeach")]
    public abstract string? StringForeach();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0008 : PUSHDATA1
    // 000D : PUSHDATA1
    // 0012 : PUSH3
    // 0013 : PACK
    // 0014 : STLOC0
    // 0015 : PUSHDATA1
    // 0017 : STLOC1
    // 0018 : LDLOC0
    // 0019 : DUP
    // 001A : STLOC2
    // 001B : SIZE
    // 001C : STLOC3
    // 001D : PUSH0
    // 001E : STLOC4
    // 001F : JMP
    // 0021 : LDLOC2
    // 0022 : LDLOC4
    // 0023 : PICKITEM
    // 0024 : STLOC5
    // 0025 : LDLOC1
    // 0026 : LDLOC5
    // 0027 : CAT
    // 0028 : CONVERT
    // 002A : STLOC1
    // 002B : LDLOC4
    // 002C : INC
    // 002D : STLOC4
    // 002E : LDLOC4
    // 002F : LDLOC3
    // 0030 : JMPLT
    // 0032 : LDLOC1
    // 0033 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("structForeach")]
    public abstract IDictionary<object, object>? StructForeach();
    // 0000 : INITSLOT
    // 0003 : NEWSTRUCT0
    // 0004 : DUP
    // 0005 : PUSHNULL
    // 0006 : APPEND
    // 0007 : DUP
    // 0008 : PUSH0
    // 0009 : APPEND
    // 000A : DUP
    // 000B : CALL
    // 000D : STLOC0
    // 000E : PUSHDATA1
    // 0015 : DUP
    // 0016 : LDLOC0
    // 0017 : PUSH0
    // 0018 : ROT
    // 0019 : SETITEM
    // 001A : DROP
    // 001B : PUSH1
    // 001C : DUP
    // 001D : LDLOC0
    // 001E : PUSH1
    // 001F : ROT
    // 0020 : SETITEM
    // 0021 : DROP
    // 0022 : NEWSTRUCT0
    // 0023 : DUP
    // 0024 : PUSHNULL
    // 0025 : APPEND
    // 0026 : DUP
    // 0027 : PUSH0
    // 0028 : APPEND
    // 0029 : DUP
    // 002A : CALL
    // 002C : STLOC1
    // 002D : PUSHDATA1
    // 0034 : DUP
    // 0035 : LDLOC1
    // 0036 : PUSH0
    // 0037 : ROT
    // 0038 : SETITEM
    // 0039 : DROP
    // 003A : PUSH2
    // 003B : DUP
    // 003C : LDLOC1
    // 003D : PUSH1
    // 003E : ROT
    // 003F : SETITEM
    // 0040 : DROP
    // 0041 : LDLOC1
    // 0042 : LDLOC0
    // 0043 : PUSH2
    // 0044 : PACK
    // 0045 : STLOC2
    // 0046 : NEWMAP
    // 0047 : STLOC3
    // 0048 : LDLOC2
    // 0049 : DUP
    // 004A : STLOC4
    // 004B : SIZE
    // 004C : STLOC5
    // 004D : PUSH0
    // 004E : STLOC6
    // 004F : JMP
    // 0051 : LDLOC4
    // 0052 : LDLOC6
    // 0053 : PICKITEM
    // 0054 : STLOC
    // 0056 : LDLOC
    // 0058 : PUSH1
    // 0059 : PICKITEM
    // 005A : DUP
    // 005B : LDLOC
    // 005D : PUSH0
    // 005E : PICKITEM
    // 005F : LDLOC3
    // 0060 : REVERSE3
    // 0061 : SETITEM
    // 0062 : DROP
    // 0063 : LDLOC6
    // 0064 : INC
    // 0065 : STLOC6
    // 0066 : LDLOC6
    // 0067 : LDLOC5
    // 0068 : JMPLT
    // 006A : LDLOC3
    // 006B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testContinue")]
    public abstract BigInteger? TestContinue();
    // 0000 : INITSLOT
    // 0003 : PUSH5
    // 0004 : PUSH4
    // 0005 : PUSH3
    // 0006 : PUSH2
    // 0007 : PUSH1
    // 0008 : PUSH5
    // 0009 : PACK
    // 000A : STLOC0
    // 000B : PUSH0
    // 000C : STLOC1
    // 000D : TRY
    // 0010 : LDLOC0
    // 0011 : DUP
    // 0012 : STLOC2
    // 0013 : SIZE
    // 0014 : STLOC3
    // 0015 : PUSH0
    // 0016 : STLOC4
    // 0017 : JMP
    // 0019 : LDLOC2
    // 001A : LDLOC4
    // 001B : PICKITEM
    // 001C : STLOC5
    // 001D : LDLOC5
    // 001E : PUSH2
    // 001F : MOD
    // 0020 : PUSH0
    // 0021 : EQUAL
    // 0022 : JMPIFNOT
    // 0024 : JMP
    // 0026 : LDLOC1
    // 0027 : LDLOC5
    // 0028 : ADD
    // 0029 : DUP
    // 002A : PUSHINT32
    // 002F : JMPGE
    // 0031 : JMP
    // 0033 : DUP
    // 0034 : PUSHINT32
    // 0039 : JMPLE
    // 003B : PUSHINT64
    // 0044 : AND
    // 0045 : DUP
    // 0046 : PUSHINT32
    // 004B : JMPLE
    // 004D : PUSHINT64
    // 0056 : SUB
    // 0057 : STLOC1
    // 0058 : LDLOC4
    // 0059 : INC
    // 005A : STLOC4
    // 005B : LDLOC4
    // 005C : LDLOC3
    // 005D : JMPLT
    // 005F : ENDTRY
    // 0061 : STLOC2
    // 0062 : ENDTRY
    // 0064 : LDLOC1
    // 0065 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDo")]
    public abstract void TestDo();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : CALLT
    // 0009 : SYSCALL
    // 000E : LDLOC0
    // 000F : DUP
    // 0010 : INC
    // 0011 : DUP
    // 0012 : PUSHINT32
    // 0017 : JMPGE
    // 0019 : JMP
    // 001B : DUP
    // 001C : PUSHINT32
    // 0021 : JMPLE
    // 0023 : PUSHINT64
    // 002C : AND
    // 002D : DUP
    // 002E : PUSHINT32
    // 0033 : JMPLE
    // 0035 : PUSHINT64
    // 003E : SUB
    // 003F : STLOC0
    // 0040 : DROP
    // 0041 : LDLOC0
    // 0042 : PUSH5
    // 0043 : LT
    // 0044 : JMPIF
    // 0046 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testForEachVariable")]
    public abstract void TestForEachVariable();
    // 0000 : INITSLOT
    // 0003 : CALL
    // 0005 : DUP
    // 0006 : STLOC0
    // 0007 : SIZE
    // 0008 : STLOC1
    // 0009 : PUSH0
    // 000A : STLOC2
    // 000B : JMP
    // 000D : LDLOC0
    // 000E : LDLOC2
    // 000F : PICKITEM
    // 0010 : UNPACK
    // 0011 : DROP
    // 0012 : STLOC3
    // 0013 : STLOC4
    // 0014 : LDLOC3
    // 0015 : CALLT
    // 0018 : PUSHDATA1
    // 001C : CAT
    // 001D : LDLOC4
    // 001E : CAT
    // 001F : CONVERT
    // 0021 : SYSCALL
    // 0026 : LDLOC2
    // 0027 : INC
    // 0028 : STLOC2
    // 0029 : LDLOC2
    // 002A : LDLOC1
    // 002B : JMPLT
    // 002D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIteratorForEach")]
    public abstract void TestIteratorForEach();
    // 0000 : INITSLOT
    // 0003 : PUSH3
    // 0004 : SYSCALL
    // 0009 : PUSH3
    // 000A : PUSH1
    // 000B : NEWBUFFER
    // 000C : TUCK
    // 000D : PUSH0
    // 000E : ROT
    // 000F : SETITEM
    // 0010 : SWAP
    // 0011 : PUSH2
    // 0012 : PACK
    // 0013 : UNPACK
    // 0014 : DROP
    // 0015 : SYSCALL
    // 001A : STLOC0
    // 001B : LDLOC0
    // 001C : STLOC1
    // 001D : JMP
    // 001F : LDLOC1
    // 0020 : SYSCALL
    // 0025 : STLOC2
    // 0026 : LDLOC2
    // 0027 : CONVERT
    // 0029 : SYSCALL
    // 002E : LDLOC1
    // 002F : SYSCALL
    // 0034 : JMPIF
    // 0036 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testWhile")]
    public abstract void TestWhile();
    // 0000 : INITSLOT
    // 0003 : PUSH0
    // 0004 : STLOC0
    // 0005 : LDLOC0
    // 0006 : PUSH5
    // 0007 : LT
    // 0008 : JMPIFNOT
    // 000A : LDLOC0
    // 000B : CALLT
    // 000E : SYSCALL
    // 0013 : LDLOC0
    // 0014 : DUP
    // 0015 : INC
    // 0016 : DUP
    // 0017 : PUSHINT32
    // 001C : JMPGE
    // 001E : JMP
    // 0020 : DUP
    // 0021 : PUSHINT32
    // 0026 : JMPLE
    // 0028 : PUSHINT64
    // 0031 : AND
    // 0032 : DUP
    // 0033 : PUSHINT32
    // 0038 : JMPLE
    // 003A : PUSHINT64
    // 0043 : SUB
    // 0044 : STLOC0
    // 0045 : DROP
    // 0046 : JMP
    // 0048 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uInt160Foreach")]
    public abstract IList<object>? UInt160Foreach();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0019 : PUSHDATA1
    // 002F : PUSH2
    // 0030 : PACK
    // 0031 : STLOC0
    // 0032 : NEWARRAY0
    // 0033 : STLOC1
    // 0034 : LDLOC0
    // 0035 : DUP
    // 0036 : STLOC2
    // 0037 : SIZE
    // 0038 : STLOC3
    // 0039 : PUSH0
    // 003A : STLOC4
    // 003B : JMP
    // 003D : LDLOC2
    // 003E : LDLOC4
    // 003F : PICKITEM
    // 0040 : STLOC5
    // 0041 : LDLOC1
    // 0042 : LDLOC5
    // 0043 : APPEND
    // 0044 : LDLOC4
    // 0045 : INC
    // 0046 : STLOC4
    // 0047 : LDLOC4
    // 0048 : LDLOC3
    // 0049 : JMPLT
    // 004B : LDLOC1
    // 004C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("uInt256Foreach")]
    public abstract IList<object>? UInt256Foreach();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0025 : PUSHDATA1
    // 0047 : PUSH2
    // 0048 : PACK
    // 0049 : STLOC0
    // 004A : NEWARRAY0
    // 004B : STLOC1
    // 004C : LDLOC0
    // 004D : DUP
    // 004E : STLOC2
    // 004F : SIZE
    // 0050 : STLOC3
    // 0051 : PUSH0
    // 0052 : STLOC4
    // 0053 : JMP
    // 0055 : LDLOC2
    // 0056 : LDLOC4
    // 0057 : PICKITEM
    // 0058 : STLOC5
    // 0059 : LDLOC1
    // 005A : LDLOC5
    // 005B : APPEND
    // 005C : LDLOC4
    // 005D : INC
    // 005E : STLOC4
    // 005F : LDLOC4
    // 0060 : LDLOC3
    // 0061 : JMPLT
    // 0063 : LDLOC1
    // 0064 : RET

    #endregion

}
