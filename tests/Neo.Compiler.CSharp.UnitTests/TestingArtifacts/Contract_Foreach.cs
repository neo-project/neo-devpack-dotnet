using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Foreach(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Foreach"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""intForeach"",""parameters"":[],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""stringForeach"",""parameters"":[],""returntype"":""String"",""offset"":84,""safe"":false},{""name"":""byteStringEmpty"",""parameters"":[],""returntype"":""Integer"",""offset"":136,""safe"":false},{""name"":""byteStringForeach"",""parameters"":[],""returntype"":""ByteArray"",""offset"":145,""safe"":false},{""name"":""structForeach"",""parameters"":[],""returntype"":""Map"",""offset"":201,""safe"":false},{""name"":""byteArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":303,""safe"":false},{""name"":""uInt160Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":341,""safe"":false},{""name"":""uInt256Foreach"",""parameters"":[],""returntype"":""Array"",""offset"":418,""safe"":false},{""name"":""eCPointForeach"",""parameters"":[],""returntype"":""Array"",""offset"":519,""safe"":false},{""name"":""bigIntegerForeach"",""parameters"":[],""returntype"":""Array"",""offset"":580,""safe"":false},{""name"":""objectArrayForeach"",""parameters"":[],""returntype"":""Array"",""offset"":635,""safe"":false},{""name"":""intForeachBreak"",""parameters"":[{""name"":""breakIndex"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":682,""safe"":false},{""name"":""testContinue"",""parameters"":[],""returntype"":""Integer"",""offset"":836,""safe"":false},{""name"":""intForloop"",""parameters"":[],""returntype"":""Integer"",""offset"":938,""safe"":false},{""name"":""testIteratorForEach"",""parameters"":[],""returntype"":""Void"",""offset"":1065,""safe"":false},{""name"":""testForEachVariable"",""parameters"":[],""returntype"":""Void"",""offset"":1143,""safe"":false},{""name"":""testDo"",""parameters"":[],""returntype"":""Void"",""offset"":1189,""safe"":false},{""name"":""testWhile"",""parameters"":[],""returntype"":""Void"",""offset"":1260,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1333,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"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"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAAwAAZKeztuANAgDKmjsCQEIPAAEQJxTAcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQA==
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHINT64 000064A7B3B6E00D
    /// 0C : OpCode.PUSHINT32 00CA9A3B
    /// 11 : OpCode.PUSHINT32 40420F00
    /// 16 : OpCode.PUSHINT16 1027
    /// 19 : OpCode.PUSH4
    /// 1A : OpCode.PACK
    /// 1B : OpCode.STLOC0
    /// 1C : OpCode.NEWARRAY0
    /// 1D : OpCode.STLOC1
    /// 1E : OpCode.LDLOC0
    /// 1F : OpCode.DUP
    /// 20 : OpCode.STLOC2
    /// 21 : OpCode.SIZE
    /// 22 : OpCode.STLOC3
    /// 23 : OpCode.PUSH0
    /// 24 : OpCode.STLOC4
    /// 25 : OpCode.JMP 0C
    /// 27 : OpCode.LDLOC2
    /// 28 : OpCode.LDLOC4
    /// 29 : OpCode.PICKITEM
    /// 2A : OpCode.STLOC5
    /// 2B : OpCode.LDLOC1
    /// 2C : OpCode.LDLOC5
    /// 2D : OpCode.APPEND
    /// 2E : OpCode.LDLOC4
    /// 2F : OpCode.INC
    /// 30 : OpCode.STLOC4
    /// 31 : OpCode.LDLOC4
    /// 32 : OpCode.LDLOC3
    /// 33 : OpCode.JMPLT F4
    /// 35 : OpCode.LDLOC1
    /// 36 : OpCode.RET
    /// </remarks>
    [DisplayName("bigIntegerForeach")]
    public abstract IList<object>? BigIntegerForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAEKEdswcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQA==
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHDATA1 010A11
    /// 08 : OpCode.CONVERT 30
    /// 0A : OpCode.STLOC0
    /// 0B : OpCode.NEWARRAY0
    /// 0C : OpCode.STLOC1
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.DUP
    /// 0F : OpCode.STLOC2
    /// 10 : OpCode.SIZE
    /// 11 : OpCode.STLOC3
    /// 12 : OpCode.PUSH0
    /// 13 : OpCode.STLOC4
    /// 14 : OpCode.JMP 0C
    /// 16 : OpCode.LDLOC2
    /// 17 : OpCode.LDLOC4
    /// 18 : OpCode.PICKITEM
    /// 19 : OpCode.STLOC5
    /// 1A : OpCode.LDLOC1
    /// 1B : OpCode.LDLOC5
    /// 1C : OpCode.APPEND
    /// 1D : OpCode.LDLOC4
    /// 1E : OpCode.INC
    /// 1F : OpCode.STLOC4
    /// 20 : OpCode.LDLOC4
    /// 21 : OpCode.LDLOC3
    /// 22 : OpCode.JMPLT F4
    /// 24 : OpCode.LDLOC1
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("byteArrayForeach")]
    public abstract IList<object>? ByteArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADHBoykA=
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHDATA1
    /// 05 : OpCode.STLOC0
    /// 06 : OpCode.LDLOC0
    /// 07 : OpCode.SIZE
    /// 08 : OpCode.RET
    /// </remarks>
    [DisplayName("byteStringEmpty")]
    public abstract BigInteger? ByteStringEmpty();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAwMaGlqDGRlZgxhYmMVwHAMcWhKcspzEHQiD2psznVpbYvbKHFsnHRsazDxaUA=
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHDATA1
    /// 05 : OpCode.PUSHDATA1
    /// 07 : OpCode.PUSHDATA1 68696A
    /// 0C : OpCode.PUSHDATA1 646566
    /// 11 : OpCode.PUSHDATA1 616263
    /// 16 : OpCode.PUSH5
    /// 17 : OpCode.PACK
    /// 18 : OpCode.STLOC0
    /// 19 : OpCode.PUSHDATA1
    /// 1B : OpCode.STLOC1
    /// 1C : OpCode.LDLOC0
    /// 1D : OpCode.DUP
    /// 1E : OpCode.STLOC2
    /// 1F : OpCode.SIZE
    /// 20 : OpCode.STLOC3
    /// 21 : OpCode.PUSH0
    /// 22 : OpCode.STLOC4
    /// 23 : OpCode.JMP 0F
    /// 25 : OpCode.LDLOC2
    /// 26 : OpCode.LDLOC4
    /// 27 : OpCode.PICKITEM
    /// 28 : OpCode.STLOC5
    /// 29 : OpCode.LDLOC1
    /// 2A : OpCode.LDLOC5
    /// 2B : OpCode.CAT
    /// 2C : OpCode.CONVERT 28
    /// 2E : OpCode.STLOC1
    /// 2F : OpCode.LDLOC4
    /// 30 : OpCode.INC
    /// 31 : OpCode.STLOC4
    /// 32 : OpCode.LDLOC4
    /// 33 : OpCode.LDLOC3
    /// 34 : OpCode.JMPLT F1
    /// 36 : OpCode.LDLOC1
    /// 37 : OpCode.RET
    /// </remarks>
    [DisplayName("byteStringForeach")]
    public abstract byte[]? ByteStringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAWNsoStgkCUrKACEoAzpY2yhK2CQJSsoAISgDOhLAcMJxaEpyynMQdCIMamzOdWltz2ycdGxrMPRpQA==
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.LDSFLD0
    /// 04 : OpCode.CONVERT 28
    /// 06 : OpCode.DUP
    /// 07 : OpCode.ISNULL
    /// 08 : OpCode.JMPIF 09
    /// 0A : OpCode.DUP
    /// 0B : OpCode.SIZE
    /// 0C : OpCode.PUSHINT8 21
    /// 0E : OpCode.JMPEQ 03
    /// 10 : OpCode.THROW
    /// 11 : OpCode.LDSFLD0
    /// 12 : OpCode.CONVERT 28
    /// 14 : OpCode.DUP
    /// 15 : OpCode.ISNULL
    /// 16 : OpCode.JMPIF 09
    /// 18 : OpCode.DUP
    /// 19 : OpCode.SIZE
    /// 1A : OpCode.PUSHINT8 21
    /// 1C : OpCode.JMPEQ 03
    /// 1E : OpCode.THROW
    /// 1F : OpCode.PUSH2
    /// 20 : OpCode.PACK
    /// 21 : OpCode.STLOC0
    /// 22 : OpCode.NEWARRAY0
    /// 23 : OpCode.STLOC1
    /// 24 : OpCode.LDLOC0
    /// 25 : OpCode.DUP
    /// 26 : OpCode.STLOC2
    /// 27 : OpCode.SIZE
    /// 28 : OpCode.STLOC3
    /// 29 : OpCode.PUSH0
    /// 2A : OpCode.STLOC4
    /// 2B : OpCode.JMP 0C
    /// 2D : OpCode.LDLOC2
    /// 2E : OpCode.LDLOC4
    /// 2F : OpCode.PICKITEM
    /// 30 : OpCode.STLOC5
    /// 31 : OpCode.LDLOC1
    /// 32 : OpCode.LDLOC5
    /// 33 : OpCode.APPEND
    /// 34 : OpCode.LDLOC4
    /// 35 : OpCode.INC
    /// 36 : OpCode.STLOC4
    /// 37 : OpCode.LDLOC4
    /// 38 : OpCode.LDLOC3
    /// 39 : OpCode.JMPLT F4
    /// 3B : OpCode.LDLOC1
    /// 3C : OpCode.RET
    /// </remarks>
    [DisplayName("eCPointForeach")]
    public abstract IList<object>? ECPointForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAFBMSERTAcBBxaEpyynMQdCI7amzOdWltnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9xbJx0bGswxWlA
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSH4
    /// 04 : OpCode.PUSH3
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PUSH1
    /// 07 : OpCode.PUSH4
    /// 08 : OpCode.PACK
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC1
    /// 0C : OpCode.LDLOC0
    /// 0D : OpCode.DUP
    /// 0E : OpCode.STLOC2
    /// 0F : OpCode.SIZE
    /// 10 : OpCode.STLOC3
    /// 11 : OpCode.PUSH0
    /// 12 : OpCode.STLOC4
    /// 13 : OpCode.JMP 3B
    /// 15 : OpCode.LDLOC2
    /// 16 : OpCode.LDLOC4
    /// 17 : OpCode.PICKITEM
    /// 18 : OpCode.STLOC5
    /// 19 : OpCode.LDLOC1
    /// 1A : OpCode.LDLOC5
    /// 1B : OpCode.ADD
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSHINT32 00000080
    /// 22 : OpCode.JMPGE 04
    /// 24 : OpCode.JMP 0A
    /// 26 : OpCode.DUP
    /// 27 : OpCode.PUSHINT32 FFFFFF7F
    /// 2C : OpCode.JMPLE 1E
    /// 2E : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 37 : OpCode.AND
    /// 38 : OpCode.DUP
    /// 39 : OpCode.PUSHINT32 FFFFFF7F
    /// 3E : OpCode.JMPLE 0C
    /// 40 : OpCode.PUSHINT64 0000000001000000
    /// 49 : OpCode.SUB
    /// 4A : OpCode.STLOC1
    /// 4B : OpCode.LDLOC4
    /// 4C : OpCode.INC
    /// 4D : OpCode.STLOC4
    /// 4E : OpCode.LDLOC4
    /// 4F : OpCode.LDLOC3
    /// 50 : OpCode.JMPLT C5
    /// 52 : OpCode.LDLOC1
    /// 53 : OpCode.RET
    /// </remarks>
    [DisplayName("intForeach")]
    public abstract BigInteger? IntForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYBFBMSERTAcBBxPIkAAAAAAAAAaEpyynMQdCJzamzOdXhKnUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+AELYmBCI7aW2eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FsnHRsazCNPQVyPQJpQA==
    /// 00 : OpCode.INITSLOT 0601
    /// 03 : OpCode.PUSH4
    /// 04 : OpCode.PUSH3
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PUSH1
    /// 07 : OpCode.PUSH4
    /// 08 : OpCode.PACK
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC1
    /// 0C : OpCode.TRY_L 8900000000000000
    /// 15 : OpCode.LDLOC0
    /// 16 : OpCode.DUP
    /// 17 : OpCode.STLOC2
    /// 18 : OpCode.SIZE
    /// 19 : OpCode.STLOC3
    /// 1A : OpCode.PUSH0
    /// 1B : OpCode.STLOC4
    /// 1C : OpCode.JMP 73
    /// 1E : OpCode.LDLOC2
    /// 1F : OpCode.LDLOC4
    /// 20 : OpCode.PICKITEM
    /// 21 : OpCode.STLOC5
    /// 22 : OpCode.LDARG0
    /// 23 : OpCode.DUP
    /// 24 : OpCode.DEC
    /// 25 : OpCode.DUP
    /// 26 : OpCode.PUSHINT32 00000080
    /// 2B : OpCode.JMPGE 04
    /// 2D : OpCode.JMP 0A
    /// 2F : OpCode.DUP
    /// 30 : OpCode.PUSHINT32 FFFFFF7F
    /// 35 : OpCode.JMPLE 1E
    /// 37 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 40 : OpCode.AND
    /// 41 : OpCode.DUP
    /// 42 : OpCode.PUSHINT32 FFFFFF7F
    /// 47 : OpCode.JMPLE 0C
    /// 49 : OpCode.PUSHINT64 0000000001000000
    /// 52 : OpCode.SUB
    /// 53 : OpCode.STARG0
    /// 54 : OpCode.PUSH0
    /// 55 : OpCode.LE
    /// 56 : OpCode.JMPIFNOT 04
    /// 58 : OpCode.JMP 3B
    /// 5A : OpCode.LDLOC1
    /// 5B : OpCode.LDLOC5
    /// 5C : OpCode.ADD
    /// 5D : OpCode.DUP
    /// 5E : OpCode.PUSHINT32 00000080
    /// 63 : OpCode.JMPGE 04
    /// 65 : OpCode.JMP 0A
    /// 67 : OpCode.DUP
    /// 68 : OpCode.PUSHINT32 FFFFFF7F
    /// 6D : OpCode.JMPLE 1E
    /// 6F : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 78 : OpCode.AND
    /// 79 : OpCode.DUP
    /// 7A : OpCode.PUSHINT32 FFFFFF7F
    /// 7F : OpCode.JMPLE 0C
    /// 81 : OpCode.PUSHINT64 0000000001000000
    /// 8A : OpCode.SUB
    /// 8B : OpCode.STLOC1
    /// 8C : OpCode.LDLOC4
    /// 8D : OpCode.INC
    /// 8E : OpCode.STLOC4
    /// 8F : OpCode.LDLOC4
    /// 90 : OpCode.LDLOC3
    /// 91 : OpCode.JMPLT 8D
    /// 93 : OpCode.ENDTRY 05
    /// 95 : OpCode.STLOC2
    /// 96 : OpCode.ENDTRY 02
    /// 98 : OpCode.LDLOC1
    /// 99 : OpCode.RET
    /// </remarks>
    [DisplayName("intForeachBreak")]
    public abstract BigInteger? IntForeachBreak(BigInteger? breakIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAFBMSERTAcBBxEHIiaWloas6eSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3FqSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfckVqaMq1JJVpQA==
    /// 00 : OpCode.INITSLOT 0300
    /// 03 : OpCode.PUSH4
    /// 04 : OpCode.PUSH3
    /// 05 : OpCode.PUSH2
    /// 06 : OpCode.PUSH1
    /// 07 : OpCode.PUSH4
    /// 08 : OpCode.PACK
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC1
    /// 0C : OpCode.PUSH0
    /// 0D : OpCode.STLOC2
    /// 0E : OpCode.JMP 69
    /// 10 : OpCode.LDLOC1
    /// 11 : OpCode.LDLOC0
    /// 12 : OpCode.LDLOC2
    /// 13 : OpCode.PICKITEM
    /// 14 : OpCode.ADD
    /// 15 : OpCode.DUP
    /// 16 : OpCode.PUSHINT32 00000080
    /// 1B : OpCode.JMPGE 04
    /// 1D : OpCode.JMP 0A
    /// 1F : OpCode.DUP
    /// 20 : OpCode.PUSHINT32 FFFFFF7F
    /// 25 : OpCode.JMPLE 1E
    /// 27 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 30 : OpCode.AND
    /// 31 : OpCode.DUP
    /// 32 : OpCode.PUSHINT32 FFFFFF7F
    /// 37 : OpCode.JMPLE 0C
    /// 39 : OpCode.PUSHINT64 0000000001000000
    /// 42 : OpCode.SUB
    /// 43 : OpCode.STLOC1
    /// 44 : OpCode.LDLOC2
    /// 45 : OpCode.DUP
    /// 46 : OpCode.INC
    /// 47 : OpCode.DUP
    /// 48 : OpCode.PUSHINT32 00000080
    /// 4D : OpCode.JMPGE 04
    /// 4F : OpCode.JMP 0A
    /// 51 : OpCode.DUP
    /// 52 : OpCode.PUSHINT32 FFFFFF7F
    /// 57 : OpCode.JMPLE 1E
    /// 59 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 62 : OpCode.AND
    /// 63 : OpCode.DUP
    /// 64 : OpCode.PUSHINT32 FFFFFF7F
    /// 69 : OpCode.JMPLE 0C
    /// 6B : OpCode.PUSHINT64 0000000001000000
    /// 74 : OpCode.SUB
    /// 75 : OpCode.STLOC2
    /// 76 : OpCode.DROP
    /// 77 : OpCode.LDLOC2
    /// 78 : OpCode.LDLOC0
    /// 79 : OpCode.SIZE
    /// 7A : OpCode.LT
    /// 7B : OpCode.JMPIF 95
    /// 7D : OpCode.LDLOC1
    /// 7E : OpCode.RET
    /// </remarks>
    [DisplayName("intForloop")]
    public abstract BigInteger? IntForloop();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAAHsMdGVzdAwBAtswE8BwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlA
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHINT8 7B
    /// 05 : OpCode.PUSHDATA1 74657374
    /// 0B : OpCode.PUSHDATA1 0102
    /// 0F : OpCode.CONVERT 30
    /// 11 : OpCode.PUSH3
    /// 12 : OpCode.PACK
    /// 13 : OpCode.STLOC0
    /// 14 : OpCode.NEWARRAY0
    /// 15 : OpCode.STLOC1
    /// 16 : OpCode.LDLOC0
    /// 17 : OpCode.DUP
    /// 18 : OpCode.STLOC2
    /// 19 : OpCode.SIZE
    /// 1A : OpCode.STLOC3
    /// 1B : OpCode.PUSH0
    /// 1C : OpCode.STLOC4
    /// 1D : OpCode.JMP 0C
    /// 1F : OpCode.LDLOC2
    /// 20 : OpCode.LDLOC4
    /// 21 : OpCode.PICKITEM
    /// 22 : OpCode.STLOC5
    /// 23 : OpCode.LDLOC1
    /// 24 : OpCode.LDLOC5
    /// 25 : OpCode.APPEND
    /// 26 : OpCode.LDLOC4
    /// 27 : OpCode.INC
    /// 28 : OpCode.STLOC4
    /// 29 : OpCode.LDLOC4
    /// 2A : OpCode.LDLOC3
    /// 2B : OpCode.JMPLT F4
    /// 2D : OpCode.LDLOC1
    /// 2E : OpCode.RET
    /// </remarks>
    [DisplayName("objectArrayForeach")]
    public abstract IList<object>? ObjectArrayForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADGhpagxkZWYMYWJjE8BwDHFoSnLKcxB0Ig9qbM51aW2L2yhxbJx0bGsw8WlA
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHDATA1 68696A
    /// 08 : OpCode.PUSHDATA1 646566
    /// 0D : OpCode.PUSHDATA1 616263
    /// 12 : OpCode.PUSH3
    /// 13 : OpCode.PACK
    /// 14 : OpCode.STLOC0
    /// 15 : OpCode.PUSHDATA1
    /// 17 : OpCode.STLOC1
    /// 18 : OpCode.LDLOC0
    /// 19 : OpCode.DUP
    /// 1A : OpCode.STLOC2
    /// 1B : OpCode.SIZE
    /// 1C : OpCode.STLOC3
    /// 1D : OpCode.PUSH0
    /// 1E : OpCode.STLOC4
    /// 1F : OpCode.JMP 0F
    /// 21 : OpCode.LDLOC2
    /// 22 : OpCode.LDLOC4
    /// 23 : OpCode.PICKITEM
    /// 24 : OpCode.STLOC5
    /// 25 : OpCode.LDLOC1
    /// 26 : OpCode.LDLOC5
    /// 27 : OpCode.CAT
    /// 28 : OpCode.CONVERT 28
    /// 2A : OpCode.STLOC1
    /// 2B : OpCode.LDLOC4
    /// 2C : OpCode.INC
    /// 2D : OpCode.STLOC4
    /// 2E : OpCode.LDLOC4
    /// 2F : OpCode.LDLOC3
    /// 30 : OpCode.JMPLT F1
    /// 32 : OpCode.LDLOC1
    /// 33 : OpCode.RET
    /// </remarks>
    [DisplayName("stringForeach")]
    public abstract string? StringForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwgAxUoLz0oQz3AMdGVzdDFKaBBR0EURSmgRUdBFxUoLz0oQz3EMdGVzdDJKaRBR0EUSSmkRUdBFaWgSwHLIc2pKdMp1EHYiF2xuzncHbwcRzkpvBxDOa1PQRW6cdm5tMOlrQA==
    /// 00 : OpCode.INITSLOT 0800
    /// 03 : OpCode.NEWSTRUCT0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHNULL
    /// 06 : OpCode.APPEND
    /// 07 : OpCode.DUP
    /// 08 : OpCode.PUSH0
    /// 09 : OpCode.APPEND
    /// 0A : OpCode.STLOC0
    /// 0B : OpCode.PUSHDATA1 7465737431
    /// 12 : OpCode.DUP
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.PUSH0
    /// 15 : OpCode.ROT
    /// 16 : OpCode.SETITEM
    /// 17 : OpCode.DROP
    /// 18 : OpCode.PUSH1
    /// 19 : OpCode.DUP
    /// 1A : OpCode.LDLOC0
    /// 1B : OpCode.PUSH1
    /// 1C : OpCode.ROT
    /// 1D : OpCode.SETITEM
    /// 1E : OpCode.DROP
    /// 1F : OpCode.NEWSTRUCT0
    /// 20 : OpCode.DUP
    /// 21 : OpCode.PUSHNULL
    /// 22 : OpCode.APPEND
    /// 23 : OpCode.DUP
    /// 24 : OpCode.PUSH0
    /// 25 : OpCode.APPEND
    /// 26 : OpCode.STLOC1
    /// 27 : OpCode.PUSHDATA1 7465737432
    /// 2E : OpCode.DUP
    /// 2F : OpCode.LDLOC1
    /// 30 : OpCode.PUSH0
    /// 31 : OpCode.ROT
    /// 32 : OpCode.SETITEM
    /// 33 : OpCode.DROP
    /// 34 : OpCode.PUSH2
    /// 35 : OpCode.DUP
    /// 36 : OpCode.LDLOC1
    /// 37 : OpCode.PUSH1
    /// 38 : OpCode.ROT
    /// 39 : OpCode.SETITEM
    /// 3A : OpCode.DROP
    /// 3B : OpCode.LDLOC1
    /// 3C : OpCode.LDLOC0
    /// 3D : OpCode.PUSH2
    /// 3E : OpCode.PACK
    /// 3F : OpCode.STLOC2
    /// 40 : OpCode.NEWMAP
    /// 41 : OpCode.STLOC3
    /// 42 : OpCode.LDLOC2
    /// 43 : OpCode.DUP
    /// 44 : OpCode.STLOC4
    /// 45 : OpCode.SIZE
    /// 46 : OpCode.STLOC5
    /// 47 : OpCode.PUSH0
    /// 48 : OpCode.STLOC6
    /// 49 : OpCode.JMP 17
    /// 4B : OpCode.LDLOC4
    /// 4C : OpCode.LDLOC6
    /// 4D : OpCode.PICKITEM
    /// 4E : OpCode.STLOC 07
    /// 50 : OpCode.LDLOC 07
    /// 52 : OpCode.PUSH1
    /// 53 : OpCode.PICKITEM
    /// 54 : OpCode.DUP
    /// 55 : OpCode.LDLOC 07
    /// 57 : OpCode.PUSH0
    /// 58 : OpCode.PICKITEM
    /// 59 : OpCode.LDLOC3
    /// 5A : OpCode.REVERSE3
    /// 5B : OpCode.SETITEM
    /// 5C : OpCode.DROP
    /// 5D : OpCode.LDLOC6
    /// 5E : OpCode.INC
    /// 5F : OpCode.STLOC6
    /// 60 : OpCode.LDLOC6
    /// 61 : OpCode.LDLOC5
    /// 62 : OpCode.JMPLT E9
    /// 64 : OpCode.LDLOC3
    /// 65 : OpCode.RET
    /// </remarks>
    [DisplayName("structForeach")]
    public abstract IDictionary<object, object>? StructForeach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYAFRQTEhEVwHAQcTtUAGhKcspzEHQiRGpsznVtEqIQlyYEIjRpbZ5KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcWycdGxrMLw9BXI9AmlA
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSH5
    /// 04 : OpCode.PUSH4
    /// 05 : OpCode.PUSH3
    /// 06 : OpCode.PUSH2
    /// 07 : OpCode.PUSH1
    /// 08 : OpCode.PUSH5
    /// 09 : OpCode.PACK
    /// 0A : OpCode.STLOC0
    /// 0B : OpCode.PUSH0
    /// 0C : OpCode.STLOC1
    /// 0D : OpCode.TRY 5400
    /// 10 : OpCode.LDLOC0
    /// 11 : OpCode.DUP
    /// 12 : OpCode.STLOC2
    /// 13 : OpCode.SIZE
    /// 14 : OpCode.STLOC3
    /// 15 : OpCode.PUSH0
    /// 16 : OpCode.STLOC4
    /// 17 : OpCode.JMP 44
    /// 19 : OpCode.LDLOC2
    /// 1A : OpCode.LDLOC4
    /// 1B : OpCode.PICKITEM
    /// 1C : OpCode.STLOC5
    /// 1D : OpCode.LDLOC5
    /// 1E : OpCode.PUSH2
    /// 1F : OpCode.MOD
    /// 20 : OpCode.PUSH0
    /// 21 : OpCode.EQUAL
    /// 22 : OpCode.JMPIFNOT 04
    /// 24 : OpCode.JMP 34
    /// 26 : OpCode.LDLOC1
    /// 27 : OpCode.LDLOC5
    /// 28 : OpCode.ADD
    /// 29 : OpCode.DUP
    /// 2A : OpCode.PUSHINT32 00000080
    /// 2F : OpCode.JMPGE 04
    /// 31 : OpCode.JMP 0A
    /// 33 : OpCode.DUP
    /// 34 : OpCode.PUSHINT32 FFFFFF7F
    /// 39 : OpCode.JMPLE 1E
    /// 3B : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 44 : OpCode.AND
    /// 45 : OpCode.DUP
    /// 46 : OpCode.PUSHINT32 FFFFFF7F
    /// 4B : OpCode.JMPLE 0C
    /// 4D : OpCode.PUSHINT64 0000000001000000
    /// 56 : OpCode.SUB
    /// 57 : OpCode.STLOC1
    /// 58 : OpCode.LDLOC4
    /// 59 : OpCode.INC
    /// 5A : OpCode.STLOC4
    /// 5B : OpCode.LDLOC4
    /// 5C : OpCode.LDLOC3
    /// 5D : OpCode.JMPLT BC
    /// 5F : OpCode.ENDTRY 05
    /// 61 : OpCode.STLOC2
    /// 62 : OpCode.ENDTRY 02
    /// 64 : OpCode.LDLOC1
    /// 65 : OpCode.RET
    /// </remarks>
    [DisplayName("testContinue")]
    public abstract BigInteger? TestContinue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBoNwAAQc/nR5ZoSpxKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfcEVoFbUkwUA=
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.CALLT 0000
    /// 09 : OpCode.SYSCALL CFE74796
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.DUP
    /// 10 : OpCode.INC
    /// 11 : OpCode.DUP
    /// 12 : OpCode.PUSHINT32 00000080
    /// 17 : OpCode.JMPGE 04
    /// 19 : OpCode.JMP 0A
    /// 1B : OpCode.DUP
    /// 1C : OpCode.PUSHINT32 FFFFFF7F
    /// 21 : OpCode.JMPLE 1E
    /// 23 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 2C : OpCode.AND
    /// 2D : OpCode.DUP
    /// 2E : OpCode.PUSHINT32 FFFFFF7F
    /// 33 : OpCode.JMPLE 0C
    /// 35 : OpCode.PUSHINT64 0000000001000000
    /// 3E : OpCode.SUB
    /// 3F : OpCode.STLOC0
    /// 40 : OpCode.DROP
    /// 41 : OpCode.LDLOC0
    /// 42 : OpCode.PUSH5
    /// 43 : OpCode.LT
    /// 44 : OpCode.JMPIF C1
    /// 46 : OpCode.RET
    /// </remarks>
    [DisplayName("testDo")]
    public abstract void TestDo();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUANOZKcMpxEHIiHmhqzsFFc3RrNwAADDogi2yL2yhBz+dHlmqccmppMOJA
    /// 00 : OpCode.INITSLOT 0500
    /// 03 : OpCode.CALL E6
    /// 05 : OpCode.DUP
    /// 06 : OpCode.STLOC0
    /// 07 : OpCode.SIZE
    /// 08 : OpCode.STLOC1
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.STLOC2
    /// 0B : OpCode.JMP 1E
    /// 0D : OpCode.LDLOC0
    /// 0E : OpCode.LDLOC2
    /// 0F : OpCode.PICKITEM
    /// 10 : OpCode.UNPACK
    /// 11 : OpCode.DROP
    /// 12 : OpCode.STLOC3
    /// 13 : OpCode.STLOC4
    /// 14 : OpCode.LDLOC3
    /// 15 : OpCode.CALLT 0000
    /// 18 : OpCode.PUSHDATA1 3A20
    /// 1C : OpCode.CAT
    /// 1D : OpCode.LDLOC4
    /// 1E : OpCode.CAT
    /// 1F : OpCode.CONVERT 28
    /// 21 : OpCode.SYSCALL CFE74796
    /// 26 : OpCode.LDLOC2
    /// 27 : OpCode.INC
    /// 28 : OpCode.STLOC2
    /// 29 : OpCode.LDLOC2
    /// 2A : OpCode.LDLOC1
    /// 2B : OpCode.JMPLT E2
    /// 2D : OpCode.RET
    /// </remarks>
    [DisplayName("testForEachVariable")]
    public abstract void TestForEachVariable();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAE0Gb9mfOExGIThBR0FASwMFFQd8wuJpwaHEiEWlB81S/HXJq2yhBz+dHlmlBnAjtnCTrQA==
    /// 00 : OpCode.INITSLOT 0300
    /// 03 : OpCode.PUSH3
    /// 04 : OpCode.SYSCALL 9BF667CE
    /// 09 : OpCode.PUSH3
    /// 0A : OpCode.PUSH1
    /// 0B : OpCode.NEWBUFFER
    /// 0C : OpCode.TUCK
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.ROT
    /// 0F : OpCode.SETITEM
    /// 10 : OpCode.SWAP
    /// 11 : OpCode.PUSH2
    /// 12 : OpCode.PACK
    /// 13 : OpCode.UNPACK
    /// 14 : OpCode.DROP
    /// 15 : OpCode.SYSCALL DF30B89A
    /// 1A : OpCode.STLOC0
    /// 1B : OpCode.LDLOC0
    /// 1C : OpCode.STLOC1
    /// 1D : OpCode.JMP 11
    /// 1F : OpCode.LDLOC1
    /// 20 : OpCode.SYSCALL F354BF1D
    /// 25 : OpCode.STLOC2
    /// 26 : OpCode.LDLOC2
    /// 27 : OpCode.CONVERT 28
    /// 29 : OpCode.SYSCALL CFE74796
    /// 2E : OpCode.LDLOC1
    /// 2F : OpCode.SYSCALL 9C08ED9C
    /// 34 : OpCode.JMPIF EB
    /// 36 : OpCode.RET
    /// </remarks>
    [DisplayName("testIteratorForEach")]
    public abstract void TestIteratorForEach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAEHBoFbUmQGg3AABBz+dHlmhKnEoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9wRSK/QA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSH0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSH5
    /// 07 : OpCode.LT
    /// 08 : OpCode.JMPIFNOT 40
    /// 0A : OpCode.LDLOC0
    /// 0B : OpCode.CALLT 0000
    /// 0E : OpCode.SYSCALL CFE74796
    /// 13 : OpCode.LDLOC0
    /// 14 : OpCode.DUP
    /// 15 : OpCode.INC
    /// 16 : OpCode.DUP
    /// 17 : OpCode.PUSHINT32 00000080
    /// 1C : OpCode.JMPGE 04
    /// 1E : OpCode.JMP 0A
    /// 20 : OpCode.DUP
    /// 21 : OpCode.PUSHINT32 FFFFFF7F
    /// 26 : OpCode.JMPLE 1E
    /// 28 : OpCode.PUSHINT64 FFFFFFFF00000000
    /// 31 : OpCode.AND
    /// 32 : OpCode.DUP
    /// 33 : OpCode.PUSHINT32 FFFFFF7F
    /// 38 : OpCode.JMPLE 0C
    /// 3A : OpCode.PUSHINT64 0000000001000000
    /// 43 : OpCode.SUB
    /// 44 : OpCode.STLOC0
    /// 45 : OpCode.DROP
    /// 46 : OpCode.JMP BF
    /// 48 : OpCode.RET
    /// </remarks>
    [DisplayName("testWhile")]
    public abstract void TestWhile();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAEsBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlA
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 19 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000
    /// 2F : OpCode.PUSH2
    /// 30 : OpCode.PACK
    /// 31 : OpCode.STLOC0
    /// 32 : OpCode.NEWARRAY0
    /// 33 : OpCode.STLOC1
    /// 34 : OpCode.LDLOC0
    /// 35 : OpCode.DUP
    /// 36 : OpCode.STLOC2
    /// 37 : OpCode.SIZE
    /// 38 : OpCode.STLOC3
    /// 39 : OpCode.PUSH0
    /// 3A : OpCode.STLOC4
    /// 3B : OpCode.JMP 0C
    /// 3D : OpCode.LDLOC2
    /// 3E : OpCode.LDLOC4
    /// 3F : OpCode.PICKITEM
    /// 40 : OpCode.STLOC5
    /// 41 : OpCode.LDLOC1
    /// 42 : OpCode.LDLOC5
    /// 43 : OpCode.APPEND
    /// 44 : OpCode.LDLOC4
    /// 45 : OpCode.INC
    /// 46 : OpCode.STLOC4
    /// 47 : OpCode.LDLOC4
    /// 48 : OpCode.LDLOC3
    /// 49 : OpCode.JMPLT F4
    /// 4B : OpCode.LDLOC1
    /// 4C : OpCode.RET
    /// </remarks>
    [DisplayName("uInt160Foreach")]
    public abstract IList<object>? UInt160Foreach();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEsBwwnFoSnLKcxB0IgxqbM51aW3PbJx0bGsw9GlA
    /// 00 : OpCode.INITSLOT 0600
    /// 03 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000
    /// 25 : OpCode.PUSHDATA1 0000000000000000000000000000000000000000000000000000000000000000
    /// 47 : OpCode.PUSH2
    /// 48 : OpCode.PACK
    /// 49 : OpCode.STLOC0
    /// 4A : OpCode.NEWARRAY0
    /// 4B : OpCode.STLOC1
    /// 4C : OpCode.LDLOC0
    /// 4D : OpCode.DUP
    /// 4E : OpCode.STLOC2
    /// 4F : OpCode.SIZE
    /// 50 : OpCode.STLOC3
    /// 51 : OpCode.PUSH0
    /// 52 : OpCode.STLOC4
    /// 53 : OpCode.JMP 0C
    /// 55 : OpCode.LDLOC2
    /// 56 : OpCode.LDLOC4
    /// 57 : OpCode.PICKITEM
    /// 58 : OpCode.STLOC5
    /// 59 : OpCode.LDLOC1
    /// 5A : OpCode.LDLOC5
    /// 5B : OpCode.APPEND
    /// 5C : OpCode.LDLOC4
    /// 5D : OpCode.INC
    /// 5E : OpCode.STLOC4
    /// 5F : OpCode.LDLOC4
    /// 60 : OpCode.LDLOC3
    /// 61 : OpCode.JMPLT F4
    /// 63 : OpCode.LDLOC1
    /// 64 : OpCode.RET
    /// </remarks>
    [DisplayName("uInt256Foreach")]
    public abstract IList<object>? UInt256Foreach();

    #endregion
}
