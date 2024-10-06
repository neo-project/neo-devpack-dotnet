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
    /// <remarks>
    /// Script: VwYAAwAAZKeztuANAgDKmjsCQEIPAAEQJxTAcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQA==
    /// 0000 : OpCode.INITSLOT 0600
    /// 0003 : OpCode.PUSHINT64 000064A7B3B6E00D
    /// 000C : OpCode.PUSHINT32 00CA9A3B
    /// 0011 : OpCode.PUSHINT32 40420F00
    /// 0016 : OpCode.PUSHINT16 1027
    /// 0019 : OpCode.PUSH4
    /// 001A : OpCode.PACK
    /// 001B : OpCode.STLOC0
    /// 001C : OpCode.NEWARRAY0
    /// 001D : OpCode.STLOC1
    /// 001E : OpCode.LDLOC0
    /// 001F : OpCode.DUP
    /// 0020 : OpCode.STLOC2
    /// 0021 : OpCode.SIZE
    /// 0022 : OpCode.STLOC3
    /// 0023 : OpCode.PUSH0
    /// 0024 : OpCode.STLOC4
    /// 0025 : OpCode.JMP 0C
    /// 0027 : OpCode.LDLOC2
    /// 0028 : OpCode.LDLOC4
    /// 0029 : OpCode.PICKITEM
    /// 002A : OpCode.STLOC5
    /// 002B : OpCode.LDLOC1
    /// 002C : OpCode.LDLOC5
    /// 002D : OpCode.APPEND
    /// 002E : OpCode.LDLOC4
    /// 002F : OpCode.INC
    /// 0030 : OpCode.STLOC4
    /// 0031 : OpCode.LDLOC4
    /// 0032 : OpCode.LDLOC3
    /// 0033 : OpCode.JMPLT F4
    /// 0035 : OpCode.LDLOC1
    /// 0036 : OpCode.RET
    /// </remarks>
    [DisplayName("bigIntegerForeach")]
    public abstract IList<object>? BigIntegerForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAEKEdswcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQA==
    /// 0000 : OpCode.INITSLOT 0600
    /// 0003 : OpCode.PUSHDATA1 010A11
    /// 0008 : OpCode.CONVERT 30
    /// 000A : OpCode.STLOC0
    /// 000B : OpCode.NEWARRAY0
    /// 000C : OpCode.STLOC1
    /// 000D : OpCode.LDLOC0
    /// 000E : OpCode.DUP
    /// 000F : OpCode.STLOC2
    /// 0010 : OpCode.SIZE
    /// 0011 : OpCode.STLOC3
    /// 0012 : OpCode.PUSH0
    /// 0013 : OpCode.STLOC4
    /// 0014 : OpCode.JMP 0C
    /// 0016 : OpCode.LDLOC2
    /// 0017 : OpCode.LDLOC4
    /// 0018 : OpCode.PICKITEM
    /// 0019 : OpCode.STLOC5
    /// 001A : OpCode.LDLOC1
    /// 001B : OpCode.LDLOC5
    /// 001C : OpCode.APPEND
    /// 001D : OpCode.LDLOC4
    /// 001E : OpCode.INC
    /// 001F : OpCode.STLOC4
    /// 0020 : OpCode.LDLOC4
    /// 0021 : OpCode.LDLOC3
    /// 0022 : OpCode.JMPLT F4
    /// 0024 : OpCode.LDLOC1
    /// 0025 : OpCode.RET
    /// </remarks>
    [DisplayName("byteArrayForeach")]
    public abstract IList<object>? ByteArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADHBoykA=
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSHDATA1
    /// 0005 : OpCode.STLOC0
    /// 0006 : OpCode.LDLOC0
    /// 0007 : OpCode.SIZE
    /// 0008 : OpCode.RET
    /// </remarks>
    [DisplayName("byteStringEmpty")]
    public abstract BigInteger? ByteStringEmpty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAwMaGlqDGRlZgxhYmMVwHAMcWhKcspzEHQiD2psznVpbYvbKHFsnHRsazDxaUA=
    /// 0000 : OpCode.INITSLOT 0600
    /// 0003 : OpCode.PUSHDATA1
    /// 0005 : OpCode.PUSHDATA1
    /// 0007 : OpCode.PUSHDATA1 68696A
    /// 000C : OpCode.PUSHDATA1 646566
    /// 0011 : OpCode.PUSHDATA1 616263
    /// 0016 : OpCode.PUSH5
    /// 0017 : OpCode.PACK
    /// 0018 : OpCode.STLOC0
    /// 0019 : OpCode.PUSHDATA1
    /// 001B : OpCode.STLOC1
    /// 001C : OpCode.LDLOC0
    /// 001D : OpCode.DUP
    /// 001E : OpCode.STLOC2
    /// 001F : OpCode.SIZE
    /// 0020 : OpCode.STLOC3
    /// 0021 : OpCode.PUSH0
    /// 0022 : OpCode.STLOC4
    /// 0023 : OpCode.JMP 0F
    /// 0025 : OpCode.LDLOC2
    /// 0026 : OpCode.LDLOC4
    /// 0027 : OpCode.PICKITEM
    /// 0028 : OpCode.STLOC5
    /// 0029 : OpCode.LDLOC1
    /// 002A : OpCode.LDLOC5
    /// 002B : OpCode.CAT
    /// 002C : OpCode.CONVERT 28
    /// 002E : OpCode.STLOC1
    /// 002F : OpCode.LDLOC4
    /// 0030 : OpCode.INC
    /// 0031 : OpCode.STLOC4
    /// 0032 : OpCode.LDLOC4
    /// 0033 : OpCode.LDLOC3
    /// 0034 : OpCode.JMPLT F1
    /// 0036 : OpCode.LDLOC1
    /// 0037 : OpCode.RET
    /// </remarks>
    [DisplayName("byteStringForeach")]
    public abstract byte[]? ByteStringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAWNsoStgkCUrKACEoAzpY2yhK2CQJSsoAISgDOhLAcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQA==
    /// 0000 : OpCode.INITSLOT 0600
    /// 0003 : OpCode.LDSFLD0
    /// 0004 : OpCode.CONVERT 28
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.ISNULL
    /// 0008 : OpCode.JMPIF 09
    /// 000A : OpCode.DUP
    /// 000B : OpCode.SIZE
    /// 000C : OpCode.PUSHINT8 21
    /// 000E : OpCode.JMPEQ 03
    /// 0010 : OpCode.THROW
    /// 0011 : OpCode.LDSFLD0
    /// 0012 : OpCode.CONVERT 28
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.ISNULL
    /// 0016 : OpCode.JMPIF 09
    /// 0018 : OpCode.DUP
    /// 0019 : OpCode.SIZE
    /// 001A : OpCode.PUSHINT8 21
    /// 001C : OpCode.JMPEQ 03
    /// 001E : OpCode.THROW
    /// 001F : OpCode.PUSH2
    /// 0020 : OpCode.PACK
    /// 0021 : OpCode.STLOC0
    /// 0022 : OpCode.NEWARRAY0
    /// 0023 : OpCode.STLOC1
    /// 0024 : OpCode.LDLOC0
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.STLOC2
    /// 0027 : OpCode.SIZE
    /// 0028 : OpCode.STLOC3
    /// 0029 : OpCode.PUSH0
    /// 002A : OpCode.STLOC4
    /// 002B : OpCode.JMP 0C
    /// 002D : OpCode.LDLOC2
    /// 002E : OpCode.LDLOC4
    /// 002F : OpCode.PICKITEM
    /// 0030 : OpCode.STLOC5
    /// 0031 : OpCode.LDLOC1
    /// 0032 : OpCode.LDLOC5
    /// 0033 : OpCode.APPEND
    /// 0034 : OpCode.LDLOC4
    /// 0035 : OpCode.INC
    /// 0036 : OpCode.STLOC4
    /// 0037 : OpCode.LDLOC4
    /// 0038 : OpCode.LDLOC3
    /// 0039 : OpCode.JMPLT F4
    /// 003B : OpCode.LDLOC1
    /// 003C : OpCode.RET
    /// </remarks>
    [DisplayName("eCPointForeach")]
    public abstract IList<object>? ECPointForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAFBMSERTAcBBxaEpyynMQdCI7amzOdWltnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xbJx0bGswxWlA
    /// 0000 : OpCode.INITSLOT 0600
    /// 0003 : OpCode.PUSH4
    /// 0004 : OpCode.PUSH3
    /// 0005 : OpCode.PUSH2
    /// 0006 : OpCode.PUSH1
    /// 0007 : OpCode.PUSH4
    /// 0008 : OpCode.PACK
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.STLOC1
    /// 000C : OpCode.LDLOC0
    /// 000D : OpCode.DUP
    /// 000E : OpCode.STLOC2
    /// 000F : OpCode.SIZE
    /// 0010 : OpCode.STLOC3
    /// 0011 : OpCode.PUSH0
    /// 0012 : OpCode.STLOC4
    /// 0013 : OpCode.JMP 3B
    /// 0015 : OpCode.LDLOC2
    /// 0016 : OpCode.LDLOC4
    /// 0017 : OpCode.PICKITEM
    /// 0018 : OpCode.STLOC5
    /// 0019 : OpCode.LDLOC1
    /// 001A : OpCode.LDLOC5
    /// 001B : OpCode.ADD
    /// 001C : OpCode.DUP
    /// 001D : OpCode.PUSHINT32 00000080
    /// 0022 : OpCode.JMPGE 04
    /// 0024 : OpCode.JMP 0A
    /// 0026 : OpCode.DUP
    /// 0027 : OpCode.PUSHINT32 FFFFFF7F
    /// 002C : OpCode.JMPLE 1E
    /// 002E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0037 : OpCode.AND
    /// 0038 : OpCode.DUP
    /// 0039 : OpCode.PUSHINT32 FFFFFF7F
    /// 003E : OpCode.JMPLE 0C
    /// 0040 : OpCode.PUSHINT64 0000000001000000
    /// 0049 : OpCode.SUB
    /// 004A : OpCode.STLOC1
    /// 004B : OpCode.LDLOC4
    /// 004C : OpCode.INC
    /// 004D : OpCode.STLOC4
    /// 004E : OpCode.LDLOC4
    /// 004F : OpCode.LDLOC3
    /// 0050 : OpCode.JMPLT C5
    /// 0052 : OpCode.LDLOC1
    /// 0053 : OpCode.RET
    /// </remarks>
    [DisplayName("intForeach")]
    public abstract BigInteger? IntForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBFBMSERTAcBBxPIkAAAAAAAAAaEpyynMQdCJzamzOdXhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+AELYmBCI7aW2eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FsnHRsazCNPQVyPQJpQA==
    /// 0000 : OpCode.INITSLOT 0601
    /// 0003 : OpCode.PUSH4
    /// 0004 : OpCode.PUSH3
    /// 0005 : OpCode.PUSH2
    /// 0006 : OpCode.PUSH1
    /// 0007 : OpCode.PUSH4
    /// 0008 : OpCode.PACK
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.STLOC1
    /// 000C : OpCode.TRY_L 8900000000000000
    /// 0015 : OpCode.LDLOC0
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.STLOC2
    /// 0018 : OpCode.SIZE
    /// 0019 : OpCode.STLOC3
    /// 001A : OpCode.PUSH0
    /// 001B : OpCode.STLOC4
    /// 001C : OpCode.JMP 73
    /// 001E : OpCode.LDLOC2
    /// 001F : OpCode.LDLOC4
    /// 0020 : OpCode.PICKITEM
    /// 0021 : OpCode.STLOC5
    /// 0022 : OpCode.LDARG0
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.DEC
    /// 0025 : OpCode.DUP
    /// 0026 : OpCode.PUSHINT32 00000080
    /// 002B : OpCode.JMPGE 04
    /// 002D : OpCode.JMP 0A
    /// 002F : OpCode.DUP
    /// 0030 : OpCode.PUSHINT32 FFFFFF7F
    /// 0035 : OpCode.JMPLE 1E
    /// 0037 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0040 : OpCode.AND
    /// 0041 : OpCode.DUP
    /// 0042 : OpCode.PUSHINT32 FFFFFF7F
    /// 0047 : OpCode.JMPLE 0C
    /// 0049 : OpCode.PUSHINT64 0000000001000000
    /// 0052 : OpCode.SUB
    /// 0053 : OpCode.STARG0
    /// 0054 : OpCode.PUSH0
    /// 0055 : OpCode.LE
    /// 0056 : OpCode.JMPIFNOT 04
    /// 0058 : OpCode.JMP 3B
    /// 005A : OpCode.LDLOC1
    /// 005B : OpCode.LDLOC5
    /// 005C : OpCode.ADD
    /// 005D : OpCode.DUP
    /// 005E : OpCode.PUSHINT32 00000080
    /// 0063 : OpCode.JMPGE 04
    /// 0065 : OpCode.JMP 0A
    /// 0067 : OpCode.DUP
    /// 0068 : OpCode.PUSHINT32 FFFFFF7F
    /// 006D : OpCode.JMPLE 1E
    /// 006F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0078 : OpCode.AND
    /// 0079 : OpCode.DUP
    /// 007A : OpCode.PUSHINT32 FFFFFF7F
    /// 007F : OpCode.JMPLE 0C
    /// 0081 : OpCode.PUSHINT64 0000000001000000
    /// 008A : OpCode.SUB
    /// 008B : OpCode.STLOC1
    /// 008C : OpCode.LDLOC4
    /// 008D : OpCode.INC
    /// 008E : OpCode.STLOC4
    /// 008F : OpCode.LDLOC4
    /// 0090 : OpCode.LDLOC3
    /// 0091 : OpCode.JMPLT 8D
    /// 0093 : OpCode.ENDTRY 05
    /// 0095 : OpCode.STLOC2
    /// 0096 : OpCode.ENDTRY 02
    /// 0098 : OpCode.LDLOC1
    /// 0099 : OpCode.RET
    /// </remarks>
    [DisplayName("intForeachBreak")]
    public abstract BigInteger? IntForeachBreak(BigInteger? breakIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAFBMSERTAcBBxEHIiaWloas6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FqSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfckVqaMq1JJVpQA==
    /// 0000 : OpCode.INITSLOT 0300
    /// 0003 : OpCode.PUSH4
    /// 0004 : OpCode.PUSH3
    /// 0005 : OpCode.PUSH2
    /// 0006 : OpCode.PUSH1
    /// 0007 : OpCode.PUSH4
    /// 0008 : OpCode.PACK
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.PUSH0
    /// 000B : OpCode.STLOC1
    /// 000C : OpCode.PUSH0
    /// 000D : OpCode.STLOC2
    /// 000E : OpCode.JMP 69
    /// 0010 : OpCode.LDLOC1
    /// 0011 : OpCode.LDLOC0
    /// 0012 : OpCode.LDLOC2
    /// 0013 : OpCode.PICKITEM
    /// 0014 : OpCode.ADD
    /// 0015 : OpCode.DUP
    /// 0016 : OpCode.PUSHINT32 00000080
    /// 001B : OpCode.JMPGE 04
    /// 001D : OpCode.JMP 0A
    /// 001F : OpCode.DUP
    /// 0020 : OpCode.PUSHINT32 FFFFFF7F
    /// 0025 : OpCode.JMPLE 1E
    /// 0027 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0030 : OpCode.AND
    /// 0031 : OpCode.DUP
    /// 0032 : OpCode.PUSHINT32 FFFFFF7F
    /// 0037 : OpCode.JMPLE 0C
    /// 0039 : OpCode.PUSHINT64 0000000001000000
    /// 0042 : OpCode.SUB
    /// 0043 : OpCode.STLOC1
    /// 0044 : OpCode.LDLOC2
    /// 0045 : OpCode.DUP
    /// 0046 : OpCode.INC
    /// 0047 : OpCode.DUP
    /// 0048 : OpCode.PUSHINT32 00000080
    /// 004D : OpCode.JMPGE 04
    /// 004F : OpCode.JMP 0A
    /// 0051 : OpCode.DUP
    /// 0052 : OpCode.PUSHINT32 FFFFFF7F
    /// 0057 : OpCode.JMPLE 1E
    /// 0059 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0062 : OpCode.AND
    /// 0063 : OpCode.DUP
    /// 0064 : OpCode.PUSHINT32 FFFFFF7F
    /// 0069 : OpCode.JMPLE 0C
    /// 006B : OpCode.PUSHINT64 0000000001000000
    /// 0074 : OpCode.SUB
    /// 0075 : OpCode.STLOC2
    /// 0076 : OpCode.DROP
    /// 0077 : OpCode.LDLOC2
    /// 0078 : OpCode.LDLOC0
    /// 0079 : OpCode.SIZE
    /// 007A : OpCode.LT
    /// 007B : OpCode.JMPIF 95
    /// 007D : OpCode.LDLOC1
    /// 007E : OpCode.RET
    /// </remarks>
    [DisplayName("intForloop")]
    public abstract BigInteger? IntForloop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAAHsMdGVzdAwBAtswE8BwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlA
    /// 0000 : OpCode.INITSLOT 0600
    /// 0003 : OpCode.PUSHINT8 7B
    /// 0005 : OpCode.PUSHDATA1 74657374
    /// 000B : OpCode.PUSHDATA1 0102
    /// 000F : OpCode.CONVERT 30
    /// 0011 : OpCode.PUSH3
    /// 0012 : OpCode.PACK
    /// 0013 : OpCode.STLOC0
    /// 0014 : OpCode.NEWARRAY0
    /// 0015 : OpCode.STLOC1
    /// 0016 : OpCode.LDLOC0
    /// 0017 : OpCode.DUP
    /// 0018 : OpCode.STLOC2
    /// 0019 : OpCode.SIZE
    /// 001A : OpCode.STLOC3
    /// 001B : OpCode.PUSH0
    /// 001C : OpCode.STLOC4
    /// 001D : OpCode.JMP 0C
    /// 001F : OpCode.LDLOC2
    /// 0020 : OpCode.LDLOC4
    /// 0021 : OpCode.PICKITEM
    /// 0022 : OpCode.STLOC5
    /// 0023 : OpCode.LDLOC1
    /// 0024 : OpCode.LDLOC5
    /// 0025 : OpCode.APPEND
    /// 0026 : OpCode.LDLOC4
    /// 0027 : OpCode.INC
    /// 0028 : OpCode.STLOC4
    /// 0029 : OpCode.LDLOC4
    /// 002A : OpCode.LDLOC3
    /// 002B : OpCode.JMPLT F4
    /// 002D : OpCode.LDLOC1
    /// 002E : OpCode.RET
    /// </remarks>
    [DisplayName("objectArrayForeach")]
    public abstract IList<object>? ObjectArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADGhpagxkZWYMYWJjE8BwDHFoSnLKcxB0Ig9qbM51aW2L2yhxbJx0bGsw8WlA
    /// 0000 : OpCode.INITSLOT 0600
    /// 0003 : OpCode.PUSHDATA1 68696A
    /// 0008 : OpCode.PUSHDATA1 646566
    /// 000D : OpCode.PUSHDATA1 616263
    /// 0012 : OpCode.PUSH3
    /// 0013 : OpCode.PACK
    /// 0014 : OpCode.STLOC0
    /// 0015 : OpCode.PUSHDATA1
    /// 0017 : OpCode.STLOC1
    /// 0018 : OpCode.LDLOC0
    /// 0019 : OpCode.DUP
    /// 001A : OpCode.STLOC2
    /// 001B : OpCode.SIZE
    /// 001C : OpCode.STLOC3
    /// 001D : OpCode.PUSH0
    /// 001E : OpCode.STLOC4
    /// 001F : OpCode.JMP 0F
    /// 0021 : OpCode.LDLOC2
    /// 0022 : OpCode.LDLOC4
    /// 0023 : OpCode.PICKITEM
    /// 0024 : OpCode.STLOC5
    /// 0025 : OpCode.LDLOC1
    /// 0026 : OpCode.LDLOC5
    /// 0027 : OpCode.CAT
    /// 0028 : OpCode.CONVERT 28
    /// 002A : OpCode.STLOC1
    /// 002B : OpCode.LDLOC4
    /// 002C : OpCode.INC
    /// 002D : OpCode.STLOC4
    /// 002E : OpCode.LDLOC4
    /// 002F : OpCode.LDLOC3
    /// 0030 : OpCode.JMPLT F1
    /// 0032 : OpCode.LDLOC1
    /// 0033 : OpCode.RET
    /// </remarks>
    [DisplayName("stringForeach")]
    public abstract string? StringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwgAxUoLz0oQz0o0YXAMdGVzdDFKaBBR0EURSmgRUdBFxUoLz0oQz0o0QnEMdGVzdDJKaRBR0EUSSmkRUdBFaWgSwHLIc2pKdMp1EHYiF2xuzncHbwcRzkpvBxDOa1PQRW6cdm5tMOlrQA==
    /// 0000 : OpCode.INITSLOT 0800
    /// 0003 : OpCode.NEWSTRUCT0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHNULL
    /// 0006 : OpCode.APPEND
    /// 0007 : OpCode.DUP
    /// 0008 : OpCode.PUSH0
    /// 0009 : OpCode.APPEND
    /// 000A : OpCode.DUP
    /// 000B : OpCode.CALL 61
    /// 000D : OpCode.STLOC0
    /// 000E : OpCode.PUSHDATA1 7465737431
    /// 0015 : OpCode.DUP
    /// 0016 : OpCode.LDLOC0
    /// 0017 : OpCode.PUSH0
    /// 0018 : OpCode.ROT
    /// 0019 : OpCode.SETITEM
    /// 001A : OpCode.DROP
    /// 001B : OpCode.PUSH1
    /// 001C : OpCode.DUP
    /// 001D : OpCode.LDLOC0
    /// 001E : OpCode.PUSH1
    /// 001F : OpCode.ROT
    /// 0020 : OpCode.SETITEM
    /// 0021 : OpCode.DROP
    /// 0022 : OpCode.NEWSTRUCT0
    /// 0023 : OpCode.DUP
    /// 0024 : OpCode.PUSHNULL
    /// 0025 : OpCode.APPEND
    /// 0026 : OpCode.DUP
    /// 0027 : OpCode.PUSH0
    /// 0028 : OpCode.APPEND
    /// 0029 : OpCode.DUP
    /// 002A : OpCode.CALL 42
    /// 002C : OpCode.STLOC1
    /// 002D : OpCode.PUSHDATA1 7465737432
    /// 0034 : OpCode.DUP
    /// 0035 : OpCode.LDLOC1
    /// 0036 : OpCode.PUSH0
    /// 0037 : OpCode.ROT
    /// 0038 : OpCode.SETITEM
    /// 0039 : OpCode.DROP
    /// 003A : OpCode.PUSH2
    /// 003B : OpCode.DUP
    /// 003C : OpCode.LDLOC1
    /// 003D : OpCode.PUSH1
    /// 003E : OpCode.ROT
    /// 003F : OpCode.SETITEM
    /// 0040 : OpCode.DROP
    /// 0041 : OpCode.LDLOC1
    /// 0042 : OpCode.LDLOC0
    /// 0043 : OpCode.PUSH2
    /// 0044 : OpCode.PACK
    /// 0045 : OpCode.STLOC2
    /// 0046 : OpCode.NEWMAP
    /// 0047 : OpCode.STLOC3
    /// 0048 : OpCode.LDLOC2
    /// 0049 : OpCode.DUP
    /// 004A : OpCode.STLOC4
    /// 004B : OpCode.SIZE
    /// 004C : OpCode.STLOC5
    /// 004D : OpCode.PUSH0
    /// 004E : OpCode.STLOC6
    /// 004F : OpCode.JMP 17
    /// 0051 : OpCode.LDLOC4
    /// 0052 : OpCode.LDLOC6
    /// 0053 : OpCode.PICKITEM
    /// 0054 : OpCode.STLOC 07
    /// 0056 : OpCode.LDLOC 07
    /// 0058 : OpCode.PUSH1
    /// 0059 : OpCode.PICKITEM
    /// 005A : OpCode.DUP
    /// 005B : OpCode.LDLOC 07
    /// 005D : OpCode.PUSH0
    /// 005E : OpCode.PICKITEM
    /// 005F : OpCode.LDLOC3
    /// 0060 : OpCode.REVERSE3
    /// 0061 : OpCode.SETITEM
    /// 0062 : OpCode.DROP
    /// 0063 : OpCode.LDLOC6
    /// 0064 : OpCode.INC
    /// 0065 : OpCode.STLOC6
    /// 0066 : OpCode.LDLOC6
    /// 0067 : OpCode.LDLOC5
    /// 0068 : OpCode.JMPLT E9
    /// 006A : OpCode.LDLOC3
    /// 006B : OpCode.RET
    /// </remarks>
    [DisplayName("structForeach")]
    public abstract IDictionary<object, object>? StructForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAFRQTEhEVwHAQcTtUAGhKcspzEHQiRGpsznVtEqIQlyYEIjRpbZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWycdGxrMLw9BXI9AmlA
    /// 0000 : OpCode.INITSLOT 0600
    /// 0003 : OpCode.PUSH5
    /// 0004 : OpCode.PUSH4
    /// 0005 : OpCode.PUSH3
    /// 0006 : OpCode.PUSH2
    /// 0007 : OpCode.PUSH1
    /// 0008 : OpCode.PUSH5
    /// 0009 : OpCode.PACK
    /// 000A : OpCode.STLOC0
    /// 000B : OpCode.PUSH0
    /// 000C : OpCode.STLOC1
    /// 000D : OpCode.TRY 5400
    /// 0010 : OpCode.LDLOC0
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.STLOC2
    /// 0013 : OpCode.SIZE
    /// 0014 : OpCode.STLOC3
    /// 0015 : OpCode.PUSH0
    /// 0016 : OpCode.STLOC4
    /// 0017 : OpCode.JMP 44
    /// 0019 : OpCode.LDLOC2
    /// 001A : OpCode.LDLOC4
    /// 001B : OpCode.PICKITEM
    /// 001C : OpCode.STLOC5
    /// 001D : OpCode.LDLOC5
    /// 001E : OpCode.PUSH2
    /// 001F : OpCode.MOD
    /// 0020 : OpCode.PUSH0
    /// 0021 : OpCode.EQUAL
    /// 0022 : OpCode.JMPIFNOT 04
    /// 0024 : OpCode.JMP 34
    /// 0026 : OpCode.LDLOC1
    /// 0027 : OpCode.LDLOC5
    /// 0028 : OpCode.ADD
    /// 0029 : OpCode.DUP
    /// 002A : OpCode.PUSHINT32 00000080
    /// 002F : OpCode.JMPGE 04
    /// 0031 : OpCode.JMP 0A
    /// 0033 : OpCode.DUP
    /// 0034 : OpCode.PUSHINT32 FFFFFF7F
    /// 0039 : OpCode.JMPLE 1E
    /// 003B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0044 : OpCode.AND
    /// 0045 : OpCode.DUP
    /// 0046 : OpCode.PUSHINT32 FFFFFF7F
    /// 004B : OpCode.JMPLE 0C
    /// 004D : OpCode.PUSHINT64 0000000001000000
    /// 0056 : OpCode.SUB
    /// 0057 : OpCode.STLOC1
    /// 0058 : OpCode.LDLOC4
    /// 0059 : OpCode.INC
    /// 005A : OpCode.STLOC4
    /// 005B : OpCode.LDLOC4
    /// 005C : OpCode.LDLOC3
    /// 005D : OpCode.JMPLT BC
    /// 005F : OpCode.ENDTRY 05
    /// 0061 : OpCode.STLOC2
    /// 0062 : OpCode.ENDTRY 02
    /// 0064 : OpCode.LDLOC1
    /// 0065 : OpCode.RET
    /// </remarks>
    [DisplayName("testContinue")]
    public abstract BigInteger? TestContinue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBoNwAAQc/nR5ZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoFbUkwUA=
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.CALLT 0000
    /// 0009 : OpCode.SYSCALL CFE74796
    /// 000E : OpCode.LDLOC0
    /// 000F : OpCode.DUP
    /// 0010 : OpCode.INC
    /// 0011 : OpCode.DUP
    /// 0012 : OpCode.PUSHINT32 00000080
    /// 0017 : OpCode.JMPGE 04
    /// 0019 : OpCode.JMP 0A
    /// 001B : OpCode.DUP
    /// 001C : OpCode.PUSHINT32 FFFFFF7F
    /// 0021 : OpCode.JMPLE 1E
    /// 0023 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 002C : OpCode.AND
    /// 002D : OpCode.DUP
    /// 002E : OpCode.PUSHINT32 FFFFFF7F
    /// 0033 : OpCode.JMPLE 0C
    /// 0035 : OpCode.PUSHINT64 0000000001000000
    /// 003E : OpCode.SUB
    /// 003F : OpCode.STLOC0
    /// 0040 : OpCode.DROP
    /// 0041 : OpCode.LDLOC0
    /// 0042 : OpCode.PUSH5
    /// 0043 : OpCode.LT
    /// 0044 : OpCode.JMPIF C1
    /// 0046 : OpCode.RET
    /// </remarks>
    [DisplayName("testDo")]
    public abstract void TestDo();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUANMpKcMpxEHIiHmhqzsFFc3RrNwAADDogi2yL2yhBz+dHlmqccmppMOJA
    /// 0000 : OpCode.INITSLOT 0500
    /// 0003 : OpCode.CALL CA
    /// 0005 : OpCode.DUP
    /// 0006 : OpCode.STLOC0
    /// 0007 : OpCode.SIZE
    /// 0008 : OpCode.STLOC1
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.STLOC2
    /// 000B : OpCode.JMP 1E
    /// 000D : OpCode.LDLOC0
    /// 000E : OpCode.LDLOC2
    /// 000F : OpCode.PICKITEM
    /// 0010 : OpCode.UNPACK
    /// 0011 : OpCode.DROP
    /// 0012 : OpCode.STLOC3
    /// 0013 : OpCode.STLOC4
    /// 0014 : OpCode.LDLOC3
    /// 0015 : OpCode.CALLT 0000
    /// 0018 : OpCode.PUSHDATA1 3A20
    /// 001C : OpCode.CAT
    /// 001D : OpCode.LDLOC4
    /// 001E : OpCode.CAT
    /// 001F : OpCode.CONVERT 28
    /// 0021 : OpCode.SYSCALL CFE74796
    /// 0026 : OpCode.LDLOC2
    /// 0027 : OpCode.INC
    /// 0028 : OpCode.STLOC2
    /// 0029 : OpCode.LDLOC2
    /// 002A : OpCode.LDLOC1
    /// 002B : OpCode.JMPLT E2
    /// 002D : OpCode.RET
    /// </remarks>
    [DisplayName("testForEachVariable")]
    public abstract void TestForEachVariable();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAE0Gb9mfOExGIThBR0FASwMFFQd8wuJpwaHEiEWlB81S/HXJq2yhBz+dHlmlBnAjtnCTrQA==
    /// 0000 : OpCode.INITSLOT 0300
    /// 0003 : OpCode.PUSH3
    /// 0004 : OpCode.SYSCALL 9BF667CE
    /// 0009 : OpCode.PUSH3
    /// 000A : OpCode.PUSH1
    /// 000B : OpCode.NEWBUFFER
    /// 000C : OpCode.TUCK
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.ROT
    /// 000F : OpCode.SETITEM
    /// 0010 : OpCode.SWAP
    /// 0011 : OpCode.PUSH2
    /// 0012 : OpCode.PACK
    /// 0013 : OpCode.UNPACK
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.SYSCALL DF30B89A
    /// 001A : OpCode.STLOC0
    /// 001B : OpCode.LDLOC0
    /// 001C : OpCode.STLOC1
    /// 001D : OpCode.JMP 11
    /// 001F : OpCode.LDLOC1
    /// 0020 : OpCode.SYSCALL F354BF1D
    /// 0025 : OpCode.STLOC2
    /// 0026 : OpCode.LDLOC2
    /// 0027 : OpCode.CONVERT 28
    /// 0029 : OpCode.SYSCALL CFE74796
    /// 002E : OpCode.LDLOC1
    /// 002F : OpCode.SYSCALL 9C08ED9C
    /// 0034 : OpCode.JMPIF EB
    /// 0036 : OpCode.RET
    /// </remarks>
    [DisplayName("testIteratorForEach")]
    public abstract void TestIteratorForEach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBoFbUmQGg3AABBz+dHlmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRSK/QA==
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSH0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSH5
    /// 0007 : OpCode.LT
    /// 0008 : OpCode.JMPIFNOT 40
    /// 000A : OpCode.LDLOC0
    /// 000B : OpCode.CALLT 0000
    /// 000E : OpCode.SYSCALL CFE74796
    /// 0013 : OpCode.LDLOC0
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.INC
    /// 0016 : OpCode.DUP
    /// 0017 : OpCode.PUSHINT32 00000080
    /// 001C : OpCode.JMPGE 04
    /// 001E : OpCode.JMP 0A
    /// 0020 : OpCode.DUP
    /// 0021 : OpCode.PUSHINT32 FFFFFF7F
    /// 0026 : OpCode.JMPLE 1E
    /// 0028 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 0031 : OpCode.AND
    /// 0032 : OpCode.DUP
    /// 0033 : OpCode.PUSHINT32 FFFFFF7F
    /// 0038 : OpCode.JMPLE 0C
    /// 003A : OpCode.PUSHINT64 0000000001000000
    /// 0043 : OpCode.SUB
    /// 0044 : OpCode.STLOC0
    /// 0045 : OpCode.DROP
    /// 0046 : OpCode.JMP BF
    /// 0048 : OpCode.RET
    /// </remarks>
    [DisplayName("testWhile")]
    public abstract void TestWhile();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAEsBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlA
    /// 0000 : OpCode.INITSLOT 0600
    /// 0003 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 0019 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 002F : OpCode.PUSH2
    /// 0030 : OpCode.PACK
    /// 0031 : OpCode.STLOC0
    /// 0032 : OpCode.NEWARRAY0
    /// 0033 : OpCode.STLOC1
    /// 0034 : OpCode.LDLOC0
    /// 0035 : OpCode.DUP
    /// 0036 : OpCode.STLOC2
    /// 0037 : OpCode.SIZE
    /// 0038 : OpCode.STLOC3
    /// 0039 : OpCode.PUSH0
    /// 003A : OpCode.STLOC4
    /// 003B : OpCode.JMP 0C
    /// 003D : OpCode.LDLOC2
    /// 003E : OpCode.LDLOC4
    /// 003F : OpCode.PICKITEM
    /// 0040 : OpCode.STLOC5
    /// 0041 : OpCode.LDLOC1
    /// 0042 : OpCode.LDLOC5
    /// 0043 : OpCode.APPEND
    /// 0044 : OpCode.LDLOC4
    /// 0045 : OpCode.INC
    /// 0046 : OpCode.STLOC4
    /// 0047 : OpCode.LDLOC4
    /// 0048 : OpCode.LDLOC3
    /// 0049 : OpCode.JMPLT F4
    /// 004B : OpCode.LDLOC1
    /// 004C : OpCode.RET
    /// </remarks>
    [DisplayName("uInt160Foreach")]
    public abstract IList<object>? UInt160Foreach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEsBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlA
    /// 0000 : OpCode.INITSLOT 0600
    /// 0003 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000
    /// 0025 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000
    /// 0047 : OpCode.PUSH2
    /// 0048 : OpCode.PACK
    /// 0049 : OpCode.STLOC0
    /// 004A : OpCode.NEWARRAY0
    /// 004B : OpCode.STLOC1
    /// 004C : OpCode.LDLOC0
    /// 004D : OpCode.DUP
    /// 004E : OpCode.STLOC2
    /// 004F : OpCode.SIZE
    /// 0050 : OpCode.STLOC3
    /// 0051 : OpCode.PUSH0
    /// 0052 : OpCode.STLOC4
    /// 0053 : OpCode.JMP 0C
    /// 0055 : OpCode.LDLOC2
    /// 0056 : OpCode.LDLOC4
    /// 0057 : OpCode.PICKITEM
    /// 0058 : OpCode.STLOC5
    /// 0059 : OpCode.LDLOC1
    /// 005A : OpCode.LDLOC5
    /// 005B : OpCode.APPEND
    /// 005C : OpCode.LDLOC4
    /// 005D : OpCode.INC
    /// 005E : OpCode.STLOC4
    /// 005F : OpCode.LDLOC4
    /// 0060 : OpCode.LDLOC3
    /// 0061 : OpCode.JMPLT F4
    /// 0063 : OpCode.LDLOC1
    /// 0064 : OpCode.RET
    /// </remarks>
    [DisplayName("uInt256Foreach")]
    public abstract IList<object>? UInt256Foreach();

    #endregion

}
