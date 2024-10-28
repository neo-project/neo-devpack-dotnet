using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Linq(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Linq"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""aggregateSum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""allGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":110,""safe"":false},{""name"":""isEmpty"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":203,""safe"":false},{""name"":""anyGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":263,""safe"":false},{""name"":""anyGreaterThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":359,""safe"":false},{""name"":""average"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":380,""safe"":false},{""name"":""averageTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":478,""safe"":false},{""name"":""count"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":606,""safe"":false},{""name"":""countGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":673,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":774,""safe"":false},{""name"":""containsText"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""String""}],""returntype"":""Boolean"",""offset"":858,""safe"":false},{""name"":""containsPerson"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":866,""safe"":false},{""name"":""containsPersonIndex"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""targetIndex"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":950,""safe"":false},{""name"":""containsPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1009,""safe"":false},{""name"":""firstGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1079,""safe"":false},{""name"":""selectTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1176,""safe"":false},{""name"":""selectPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1271,""safe"":false},{""name"":""skip"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1355,""safe"":false},{""name"":""sum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1433,""safe"":false},{""name"":""sumTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1477,""safe"":false},{""name"":""take"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1573,""safe"":false},{""name"":""toMap"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1651,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1821,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1920,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

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
    /// Script: VwABCmQAAAAQeDQDQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 64000000 [4 datoshi]
    /// 08 : OpCode.PUSH0 [1 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.CALL 03 [512 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("aggregateSum")]
    public abstract BigInteger? AggregateSum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClMAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 53000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("allGreaterThanZero")]
    public abstract bool? AllGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWAKCQAAAHg0oUA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.STSFLD0 [2 datoshi]
    /// 05 : OpCode.PUSHA 09000000 [4 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.CALL A1 [512 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreaterThan")]
    public abstract bool? AnyGreaterThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClYAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 56000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("anyGreaterThanZero")]
    public abstract bool? AnyGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYCeDRQDHNlbGVjdG9yeTVV/v//EHAQcXhKcspzEHQiFGpsznVoSpxwRWlteTaecWycdGxrMOxoEJcmFAxzb3VyY2UgaXMgZW1wdHk6aWihQA==
    /// 00 : OpCode.INITSLOT 0602 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL 50 [512 datoshi]
    /// 06 : OpCode.PUSHDATA1 73656C6563746F72 [8 datoshi]
    /// 10 : OpCode.LDARG1 [2 datoshi]
    /// 11 : OpCode.CALL_L 55FEFFFF [512 datoshi]
    /// 16 : OpCode.PUSH0 [1 datoshi]
    /// 17 : OpCode.STLOC0 [2 datoshi]
    /// 18 : OpCode.PUSH0 [1 datoshi]
    /// 19 : OpCode.STLOC1 [2 datoshi]
    /// 1A : OpCode.LDARG0 [2 datoshi]
    /// 1B : OpCode.DUP [2 datoshi]
    /// 1C : OpCode.STLOC2 [2 datoshi]
    /// 1D : OpCode.SIZE [4 datoshi]
    /// 1E : OpCode.STLOC3 [2 datoshi]
    /// 1F : OpCode.PUSH0 [1 datoshi]
    /// 20 : OpCode.STLOC4 [2 datoshi]
    /// 21 : OpCode.JMP 14 [2 datoshi]
    /// 23 : OpCode.LDLOC2 [2 datoshi]
    /// 24 : OpCode.LDLOC4 [2 datoshi]
    /// 25 : OpCode.PICKITEM [64 datoshi]
    /// 26 : OpCode.STLOC5 [2 datoshi]
    /// 27 : OpCode.LDLOC0 [2 datoshi]
    /// 28 : OpCode.DUP [2 datoshi]
    /// 29 : OpCode.INC [4 datoshi]
    /// 2A : OpCode.STLOC0 [2 datoshi]
    /// 2B : OpCode.DROP [2 datoshi]
    /// 2C : OpCode.LDLOC1 [2 datoshi]
    /// 2D : OpCode.LDLOC5 [2 datoshi]
    /// 2E : OpCode.LDARG1 [2 datoshi]
    /// 2F : OpCode.CALLA [512 datoshi]
    /// 30 : OpCode.ADD [8 datoshi]
    /// 31 : OpCode.STLOC1 [2 datoshi]
    /// 32 : OpCode.LDLOC4 [2 datoshi]
    /// 33 : OpCode.INC [4 datoshi]
    /// 34 : OpCode.STLOC4 [2 datoshi]
    /// 35 : OpCode.LDLOC4 [2 datoshi]
    /// 36 : OpCode.LDLOC3 [2 datoshi]
    /// 37 : OpCode.JMPLT EC [2 datoshi]
    /// 39 : OpCode.LDLOC0 [2 datoshi]
    /// 3A : OpCode.PUSH0 [1 datoshi]
    /// 3B : OpCode.EQUAL [32 datoshi]
    /// 3C : OpCode.JMPIFNOT 14 [2 datoshi]
    /// 3E : OpCode.PUSHDATA1 736F7572636520697320656D707479 [8 datoshi]
    /// 4F : OpCode.THROW [512 datoshi]
    /// 50 : OpCode.LDLOC1 [2 datoshi]
    /// 51 : OpCode.LDLOC0 [2 datoshi]
    /// 52 : OpCode.DIV [8 datoshi]
    /// 53 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("average")]
    public abstract BigInteger? Average(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCnYAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 76000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("averageTwice")]
    public abstract BigInteger? AverageTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWEKQAAAAHg0A0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.STSFLD1 [2 datoshi]
    /// 05 : OpCode.PUSHA 40000000 [4 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.CALL 03 [512 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("contains")]
    public abstract bool? Contains(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIh9pa850aBALEsBKNDRsNwAASxBR0GxLEVHQz2ucc2tqMOEQCxLASjQYeTcAAEsQUdB5SxFR0HFpaDVm////QA==
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.STLOC1 [2 datoshi]
    /// 08 : OpCode.SIZE [4 datoshi]
    /// 09 : OpCode.STLOC2 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.STLOC3 [2 datoshi]
    /// 0C : OpCode.JMP 1F [2 datoshi]
    /// 0E : OpCode.LDLOC1 [2 datoshi]
    /// 0F : OpCode.LDLOC3 [2 datoshi]
    /// 10 : OpCode.PICKITEM [64 datoshi]
    /// 11 : OpCode.STLOC4 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.PUSH0 [1 datoshi]
    /// 14 : OpCode.PUSHNULL [1 datoshi]
    /// 15 : OpCode.PUSH2 [1 datoshi]
    /// 16 : OpCode.PACK [2048 datoshi]
    /// 17 : OpCode.DUP [2 datoshi]
    /// 18 : OpCode.CALL 34 [512 datoshi]
    /// 1A : OpCode.LDLOC4 [2 datoshi]
    /// 1B : OpCode.CALLT 0000 [32768 datoshi]
    /// 1E : OpCode.OVER [2 datoshi]
    /// 1F : OpCode.PUSH0 [1 datoshi]
    /// 20 : OpCode.ROT [2 datoshi]
    /// 21 : OpCode.SETITEM [8192 datoshi]
    /// 22 : OpCode.LDLOC4 [2 datoshi]
    /// 23 : OpCode.OVER [2 datoshi]
    /// 24 : OpCode.PUSH1 [1 datoshi]
    /// 25 : OpCode.ROT [2 datoshi]
    /// 26 : OpCode.SETITEM [8192 datoshi]
    /// 27 : OpCode.APPEND [8192 datoshi]
    /// 28 : OpCode.LDLOC3 [2 datoshi]
    /// 29 : OpCode.INC [4 datoshi]
    /// 2A : OpCode.STLOC3 [2 datoshi]
    /// 2B : OpCode.LDLOC3 [2 datoshi]
    /// 2C : OpCode.LDLOC2 [2 datoshi]
    /// 2D : OpCode.JMPLT E1 [2 datoshi]
    /// 2F : OpCode.PUSH0 [1 datoshi]
    /// 30 : OpCode.PUSHNULL [1 datoshi]
    /// 31 : OpCode.PUSH2 [1 datoshi]
    /// 32 : OpCode.PACK [2048 datoshi]
    /// 33 : OpCode.DUP [2 datoshi]
    /// 34 : OpCode.CALL 18 [512 datoshi]
    /// 36 : OpCode.LDARG1 [2 datoshi]
    /// 37 : OpCode.CALLT 0000 [32768 datoshi]
    /// 3A : OpCode.OVER [2 datoshi]
    /// 3B : OpCode.PUSH0 [1 datoshi]
    /// 3C : OpCode.ROT [2 datoshi]
    /// 3D : OpCode.SETITEM [8192 datoshi]
    /// 3E : OpCode.LDARG1 [2 datoshi]
    /// 3F : OpCode.OVER [2 datoshi]
    /// 40 : OpCode.PUSH1 [1 datoshi]
    /// 41 : OpCode.ROT [2 datoshi]
    /// 42 : OpCode.SETITEM [8192 datoshi]
    /// 43 : OpCode.STLOC1 [2 datoshi]
    /// 44 : OpCode.LDLOC1 [2 datoshi]
    /// 45 : OpCode.LDLOC0 [2 datoshi]
    /// 46 : OpCode.CALL_L 66FFFFFF [512 datoshi]
    /// 4B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPerson")]
    public abstract bool? ContainsPerson(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIh9pa850aBALEsBKNOBsNwAASxBR0GxLEVHQz2ucc2tqMOFoec5xaWg1I////0A=
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.STLOC1 [2 datoshi]
    /// 08 : OpCode.SIZE [4 datoshi]
    /// 09 : OpCode.STLOC2 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.STLOC3 [2 datoshi]
    /// 0C : OpCode.JMP 1F [2 datoshi]
    /// 0E : OpCode.LDLOC1 [2 datoshi]
    /// 0F : OpCode.LDLOC3 [2 datoshi]
    /// 10 : OpCode.PICKITEM [64 datoshi]
    /// 11 : OpCode.STLOC4 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.PUSH0 [1 datoshi]
    /// 14 : OpCode.PUSHNULL [1 datoshi]
    /// 15 : OpCode.PUSH2 [1 datoshi]
    /// 16 : OpCode.PACK [2048 datoshi]
    /// 17 : OpCode.DUP [2 datoshi]
    /// 18 : OpCode.CALL E0 [512 datoshi]
    /// 1A : OpCode.LDLOC4 [2 datoshi]
    /// 1B : OpCode.CALLT 0000 [32768 datoshi]
    /// 1E : OpCode.OVER [2 datoshi]
    /// 1F : OpCode.PUSH0 [1 datoshi]
    /// 20 : OpCode.ROT [2 datoshi]
    /// 21 : OpCode.SETITEM [8192 datoshi]
    /// 22 : OpCode.LDLOC4 [2 datoshi]
    /// 23 : OpCode.OVER [2 datoshi]
    /// 24 : OpCode.PUSH1 [1 datoshi]
    /// 25 : OpCode.ROT [2 datoshi]
    /// 26 : OpCode.SETITEM [8192 datoshi]
    /// 27 : OpCode.APPEND [8192 datoshi]
    /// 28 : OpCode.LDLOC3 [2 datoshi]
    /// 29 : OpCode.INC [4 datoshi]
    /// 2A : OpCode.STLOC3 [2 datoshi]
    /// 2B : OpCode.LDLOC3 [2 datoshi]
    /// 2C : OpCode.LDLOC2 [2 datoshi]
    /// 2D : OpCode.JMPLT E1 [2 datoshi]
    /// 2F : OpCode.LDLOC0 [2 datoshi]
    /// 30 : OpCode.LDARG1 [2 datoshi]
    /// 31 : OpCode.PICKITEM [64 datoshi]
    /// 32 : OpCode.STLOC1 [2 datoshi]
    /// 33 : OpCode.LDLOC1 [2 datoshi]
    /// 34 : OpCode.LDLOC0 [2 datoshi]
    /// 35 : OpCode.CALL_L 23FFFFFF [512 datoshi]
    /// 3A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPersonIndex")]
    public abstract bool? ContainsPersonIndex(IList<object>? array, BigInteger? targetIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIhxpa850aBALEr9sNwAASxBR0GxLEVHQz2ucc2tqMOQQCxK/eTcAAEsQUdB5SxFR0HFpaDXd/v//QA==
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.STLOC1 [2 datoshi]
    /// 08 : OpCode.SIZE [4 datoshi]
    /// 09 : OpCode.STLOC2 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.STLOC3 [2 datoshi]
    /// 0C : OpCode.JMP 1C [2 datoshi]
    /// 0E : OpCode.LDLOC1 [2 datoshi]
    /// 0F : OpCode.LDLOC3 [2 datoshi]
    /// 10 : OpCode.PICKITEM [64 datoshi]
    /// 11 : OpCode.STLOC4 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.PUSH0 [1 datoshi]
    /// 14 : OpCode.PUSHNULL [1 datoshi]
    /// 15 : OpCode.PUSH2 [1 datoshi]
    /// 16 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 17 : OpCode.LDLOC4 [2 datoshi]
    /// 18 : OpCode.CALLT 0000 [32768 datoshi]
    /// 1B : OpCode.OVER [2 datoshi]
    /// 1C : OpCode.PUSH0 [1 datoshi]
    /// 1D : OpCode.ROT [2 datoshi]
    /// 1E : OpCode.SETITEM [8192 datoshi]
    /// 1F : OpCode.LDLOC4 [2 datoshi]
    /// 20 : OpCode.OVER [2 datoshi]
    /// 21 : OpCode.PUSH1 [1 datoshi]
    /// 22 : OpCode.ROT [2 datoshi]
    /// 23 : OpCode.SETITEM [8192 datoshi]
    /// 24 : OpCode.APPEND [8192 datoshi]
    /// 25 : OpCode.LDLOC3 [2 datoshi]
    /// 26 : OpCode.INC [4 datoshi]
    /// 27 : OpCode.STLOC3 [2 datoshi]
    /// 28 : OpCode.LDLOC3 [2 datoshi]
    /// 29 : OpCode.LDLOC2 [2 datoshi]
    /// 2A : OpCode.JMPLT E4 [2 datoshi]
    /// 2C : OpCode.PUSH0 [1 datoshi]
    /// 2D : OpCode.PUSHNULL [1 datoshi]
    /// 2E : OpCode.PUSH2 [1 datoshi]
    /// 2F : OpCode.PACKSTRUCT [2048 datoshi]
    /// 30 : OpCode.LDARG1 [2 datoshi]
    /// 31 : OpCode.CALLT 0000 [32768 datoshi]
    /// 34 : OpCode.OVER [2 datoshi]
    /// 35 : OpCode.PUSH0 [1 datoshi]
    /// 36 : OpCode.ROT [2 datoshi]
    /// 37 : OpCode.SETITEM [8192 datoshi]
    /// 38 : OpCode.LDARG1 [2 datoshi]
    /// 39 : OpCode.OVER [2 datoshi]
    /// 3A : OpCode.PUSH1 [1 datoshi]
    /// 3B : OpCode.ROT [2 datoshi]
    /// 3C : OpCode.SETITEM [8192 datoshi]
    /// 3D : OpCode.STLOC1 [2 datoshi]
    /// 3E : OpCode.LDLOC1 [2 datoshi]
    /// 3F : OpCode.LDLOC0 [2 datoshi]
    /// 40 : OpCode.CALL_L DDFEFFFF [512 datoshi]
    /// 45 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsPersonS")]
    public abstract bool? ContainsPersonS(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0r0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.CALL AF [512 datoshi]
    /// 07 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("containsText")]
    public abstract bool? ContainsText(IList<object>? array, string? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDQ1DHByZWRpY2F0ZXk1kf3//xBweEpxynIQcyITaWvOdGx5NiYHaEqccEVrnHNrajDtaEA=
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL 35 [512 datoshi]
    /// 06 : OpCode.PUSHDATA1 707265646963617465 [8 datoshi]
    /// 11 : OpCode.LDARG1 [2 datoshi]
    /// 12 : OpCode.CALL_L 91FDFFFF [512 datoshi]
    /// 17 : OpCode.PUSH0 [1 datoshi]
    /// 18 : OpCode.STLOC0 [2 datoshi]
    /// 19 : OpCode.LDARG0 [2 datoshi]
    /// 1A : OpCode.DUP [2 datoshi]
    /// 1B : OpCode.STLOC1 [2 datoshi]
    /// 1C : OpCode.SIZE [4 datoshi]
    /// 1D : OpCode.STLOC2 [2 datoshi]
    /// 1E : OpCode.PUSH0 [1 datoshi]
    /// 1F : OpCode.STLOC3 [2 datoshi]
    /// 20 : OpCode.JMP 13 [2 datoshi]
    /// 22 : OpCode.LDLOC1 [2 datoshi]
    /// 23 : OpCode.LDLOC3 [2 datoshi]
    /// 24 : OpCode.PICKITEM [64 datoshi]
    /// 25 : OpCode.STLOC4 [2 datoshi]
    /// 26 : OpCode.LDLOC4 [2 datoshi]
    /// 27 : OpCode.LDARG1 [2 datoshi]
    /// 28 : OpCode.CALLA [512 datoshi]
    /// 29 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 2B : OpCode.LDLOC0 [2 datoshi]
    /// 2C : OpCode.DUP [2 datoshi]
    /// 2D : OpCode.INC [4 datoshi]
    /// 2E : OpCode.STLOC0 [2 datoshi]
    /// 2F : OpCode.DROP [2 datoshi]
    /// 30 : OpCode.LDLOC3 [2 datoshi]
    /// 31 : OpCode.INC [4 datoshi]
    /// 32 : OpCode.STLOC3 [2 datoshi]
    /// 33 : OpCode.LDLOC3 [2 datoshi]
    /// 34 : OpCode.LDLOC2 [2 datoshi]
    /// 35 : OpCode.JMPLT ED [2 datoshi]
    /// 37 : OpCode.LDLOC0 [2 datoshi]
    /// 38 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("count")]
    public abstract BigInteger? Count(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClsAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 5B000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("countGreaterThanZero")]
    public abstract BigInteger? CountGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDwpWAAAAeDQDQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHM1 [1 datoshi]
    /// 04 : OpCode.PUSHA 56000000 [4 datoshi]
    /// 09 : OpCode.LDARG0 [2 datoshi]
    /// 0A : OpCode.CALL 03 [512 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("firstGreaterThanZero")]
    public abstract BigInteger? FirstGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQEqkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL 04 [512 datoshi]
    /// 06 : OpCode.NOT [4 datoshi]
    /// 07 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("isEmpty")]
    public abstract bool? IsEmpty(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUBwnB4SnHKchBzIiJpa850aBALEsBKNZ/+//9sNwAASxBR0GxLEVHQz2ucc2tqMN4KDAAAAGg1df///0A=
    /// 00 : OpCode.INITSLOT 0501 [64 datoshi]
    /// 03 : OpCode.NEWARRAY0 [16 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.STLOC1 [2 datoshi]
    /// 08 : OpCode.SIZE [4 datoshi]
    /// 09 : OpCode.STLOC2 [2 datoshi]
    /// 0A : OpCode.PUSH0 [1 datoshi]
    /// 0B : OpCode.STLOC3 [2 datoshi]
    /// 0C : OpCode.JMP 22 [2 datoshi]
    /// 0E : OpCode.LDLOC1 [2 datoshi]
    /// 0F : OpCode.LDLOC3 [2 datoshi]
    /// 10 : OpCode.PICKITEM [64 datoshi]
    /// 11 : OpCode.STLOC4 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.PUSH0 [1 datoshi]
    /// 14 : OpCode.PUSHNULL [1 datoshi]
    /// 15 : OpCode.PUSH2 [1 datoshi]
    /// 16 : OpCode.PACK [2048 datoshi]
    /// 17 : OpCode.DUP [2 datoshi]
    /// 18 : OpCode.CALL_L 9FFEFFFF [512 datoshi]
    /// 1D : OpCode.LDLOC4 [2 datoshi]
    /// 1E : OpCode.CALLT 0000 [32768 datoshi]
    /// 21 : OpCode.OVER [2 datoshi]
    /// 22 : OpCode.PUSH0 [1 datoshi]
    /// 23 : OpCode.ROT [2 datoshi]
    /// 24 : OpCode.SETITEM [8192 datoshi]
    /// 25 : OpCode.LDLOC4 [2 datoshi]
    /// 26 : OpCode.OVER [2 datoshi]
    /// 27 : OpCode.PUSH1 [1 datoshi]
    /// 28 : OpCode.ROT [2 datoshi]
    /// 29 : OpCode.SETITEM [8192 datoshi]
    /// 2A : OpCode.APPEND [8192 datoshi]
    /// 2B : OpCode.LDLOC3 [2 datoshi]
    /// 2C : OpCode.INC [4 datoshi]
    /// 2D : OpCode.STLOC3 [2 datoshi]
    /// 2E : OpCode.LDLOC3 [2 datoshi]
    /// 2F : OpCode.LDLOC2 [2 datoshi]
    /// 30 : OpCode.JMPLT DE [2 datoshi]
    /// 32 : OpCode.PUSHA 0C000000 [4 datoshi]
    /// 37 : OpCode.LDLOC0 [2 datoshi]
    /// 38 : OpCode.CALL_L 75FFFFFF [512 datoshi]
    /// 3D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("selectPersonS")]
    public abstract object? SelectPersonS(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClUAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 55000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("selectTwice")]
    public abstract object? SelectTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDQpwnB4SnHKchBzIhhpa850eRC3Jgl5Sp2BRSIFaGzPa5xza2ow6GhA
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL 29 [512 datoshi]
    /// 06 : OpCode.NEWARRAY0 [16 datoshi]
    /// 07 : OpCode.STLOC0 [2 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.DUP [2 datoshi]
    /// 0A : OpCode.STLOC1 [2 datoshi]
    /// 0B : OpCode.SIZE [4 datoshi]
    /// 0C : OpCode.STLOC2 [2 datoshi]
    /// 0D : OpCode.PUSH0 [1 datoshi]
    /// 0E : OpCode.STLOC3 [2 datoshi]
    /// 0F : OpCode.JMP 18 [2 datoshi]
    /// 11 : OpCode.LDLOC1 [2 datoshi]
    /// 12 : OpCode.LDLOC3 [2 datoshi]
    /// 13 : OpCode.PICKITEM [64 datoshi]
    /// 14 : OpCode.STLOC4 [2 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.PUSH0 [1 datoshi]
    /// 17 : OpCode.GT [8 datoshi]
    /// 18 : OpCode.JMPIFNOT 09 [2 datoshi]
    /// 1A : OpCode.LDARG1 [2 datoshi]
    /// 1B : OpCode.DUP [2 datoshi]
    /// 1C : OpCode.DEC [4 datoshi]
    /// 1D : OpCode.STARG1 [2 datoshi]
    /// 1E : OpCode.DROP [2 datoshi]
    /// 1F : OpCode.JMP 05 [2 datoshi]
    /// 21 : OpCode.LDLOC0 [2 datoshi]
    /// 22 : OpCode.LDLOC4 [2 datoshi]
    /// 23 : OpCode.APPEND [8192 datoshi]
    /// 24 : OpCode.LDLOC3 [2 datoshi]
    /// 25 : OpCode.INC [4 datoshi]
    /// 26 : OpCode.STLOC3 [2 datoshi]
    /// 27 : OpCode.LDLOC3 [2 datoshi]
    /// 28 : OpCode.LDLOC2 [2 datoshi]
    /// 29 : OpCode.JMPLT E8 [2 datoshi]
    /// 2B : OpCode.LDLOC0 [2 datoshi]
    /// 2C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("skip")]
    public abstract object? Skip(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDQwDHNlbGVjdG9yeTVu+v//EHB4SnHKchBzIg9pa850aGx5Np5wa5xza2ow8WhA
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL 30 [512 datoshi]
    /// 06 : OpCode.PUSHDATA1 73656C6563746F72 [8 datoshi]
    /// 10 : OpCode.LDARG1 [2 datoshi]
    /// 11 : OpCode.CALL_L 6EFAFFFF [512 datoshi]
    /// 16 : OpCode.PUSH0 [1 datoshi]
    /// 17 : OpCode.STLOC0 [2 datoshi]
    /// 18 : OpCode.LDARG0 [2 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.STLOC1 [2 datoshi]
    /// 1B : OpCode.SIZE [4 datoshi]
    /// 1C : OpCode.STLOC2 [2 datoshi]
    /// 1D : OpCode.PUSH0 [1 datoshi]
    /// 1E : OpCode.STLOC3 [2 datoshi]
    /// 1F : OpCode.JMP 0F [2 datoshi]
    /// 21 : OpCode.LDLOC1 [2 datoshi]
    /// 22 : OpCode.LDLOC3 [2 datoshi]
    /// 23 : OpCode.PICKITEM [64 datoshi]
    /// 24 : OpCode.STLOC4 [2 datoshi]
    /// 25 : OpCode.LDLOC0 [2 datoshi]
    /// 26 : OpCode.LDLOC4 [2 datoshi]
    /// 27 : OpCode.LDARG1 [2 datoshi]
    /// 28 : OpCode.CALLA [512 datoshi]
    /// 29 : OpCode.ADD [8 datoshi]
    /// 2A : OpCode.STLOC0 [2 datoshi]
    /// 2B : OpCode.LDLOC3 [2 datoshi]
    /// 2C : OpCode.INC [4 datoshi]
    /// 2D : OpCode.STLOC3 [2 datoshi]
    /// 2E : OpCode.LDLOC3 [2 datoshi]
    /// 2F : OpCode.LDLOC2 [2 datoshi]
    /// 30 : OpCode.JMPLT F1 [2 datoshi]
    /// 32 : OpCode.LDLOC0 [2 datoshi]
    /// 33 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClYAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 56000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("sumTwice")]
    public abstract BigInteger? SumTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDQpwnB4SnHKchBzIhhpa850eRC2JgQiEWhsz3lKnYFFa5xza2ow6GhA
    /// 00 : OpCode.INITSLOT 0502 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL 29 [512 datoshi]
    /// 06 : OpCode.NEWARRAY0 [16 datoshi]
    /// 07 : OpCode.STLOC0 [2 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.DUP [2 datoshi]
    /// 0A : OpCode.STLOC1 [2 datoshi]
    /// 0B : OpCode.SIZE [4 datoshi]
    /// 0C : OpCode.STLOC2 [2 datoshi]
    /// 0D : OpCode.PUSH0 [1 datoshi]
    /// 0E : OpCode.STLOC3 [2 datoshi]
    /// 0F : OpCode.JMP 18 [2 datoshi]
    /// 11 : OpCode.LDLOC1 [2 datoshi]
    /// 12 : OpCode.LDLOC3 [2 datoshi]
    /// 13 : OpCode.PICKITEM [64 datoshi]
    /// 14 : OpCode.STLOC4 [2 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.PUSH0 [1 datoshi]
    /// 17 : OpCode.LE [8 datoshi]
    /// 18 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 1A : OpCode.JMP 11 [2 datoshi]
    /// 1C : OpCode.LDLOC0 [2 datoshi]
    /// 1D : OpCode.LDLOC4 [2 datoshi]
    /// 1E : OpCode.APPEND [8192 datoshi]
    /// 1F : OpCode.LDARG1 [2 datoshi]
    /// 20 : OpCode.DUP [2 datoshi]
    /// 21 : OpCode.DEC [4 datoshi]
    /// 22 : OpCode.STARG1 [2 datoshi]
    /// 23 : OpCode.DROP [2 datoshi]
    /// 24 : OpCode.LDLOC3 [2 datoshi]
    /// 25 : OpCode.INC [4 datoshi]
    /// 26 : OpCode.STLOC3 [2 datoshi]
    /// 27 : OpCode.LDLOC3 [2 datoshi]
    /// 28 : OpCode.LDLOC2 [2 datoshi]
    /// 29 : OpCode.JMPLT E8 [2 datoshi]
    /// 2B : OpCode.LDLOC0 [2 datoshi]
    /// 2C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("take")]
    public abstract object? Take(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUDeDRPDGtleVNlbGVjdG9yeTWX+f//DGVsZW1lbnRTZWxlY3Rvcno1gPn//8hweEpxynIQcyIUaWvOdGx6NkpseTZoU9BFa5xza2ow7GhA
    /// 00 : OpCode.INITSLOT 0503 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.CALL 4F [512 datoshi]
    /// 06 : OpCode.PUSHDATA1 6B657953656C6563746F72 [8 datoshi]
    /// 13 : OpCode.LDARG1 [2 datoshi]
    /// 14 : OpCode.CALL_L 97F9FFFF [512 datoshi]
    /// 19 : OpCode.PUSHDATA1 656C656D656E7453656C6563746F72 [8 datoshi]
    /// 2A : OpCode.LDARG2 [2 datoshi]
    /// 2B : OpCode.CALL_L 80F9FFFF [512 datoshi]
    /// 30 : OpCode.NEWMAP [8 datoshi]
    /// 31 : OpCode.STLOC0 [2 datoshi]
    /// 32 : OpCode.LDARG0 [2 datoshi]
    /// 33 : OpCode.DUP [2 datoshi]
    /// 34 : OpCode.STLOC1 [2 datoshi]
    /// 35 : OpCode.SIZE [4 datoshi]
    /// 36 : OpCode.STLOC2 [2 datoshi]
    /// 37 : OpCode.PUSH0 [1 datoshi]
    /// 38 : OpCode.STLOC3 [2 datoshi]
    /// 39 : OpCode.JMP 14 [2 datoshi]
    /// 3B : OpCode.LDLOC1 [2 datoshi]
    /// 3C : OpCode.LDLOC3 [2 datoshi]
    /// 3D : OpCode.PICKITEM [64 datoshi]
    /// 3E : OpCode.STLOC4 [2 datoshi]
    /// 3F : OpCode.LDLOC4 [2 datoshi]
    /// 40 : OpCode.LDARG2 [2 datoshi]
    /// 41 : OpCode.CALLA [512 datoshi]
    /// 42 : OpCode.DUP [2 datoshi]
    /// 43 : OpCode.LDLOC4 [2 datoshi]
    /// 44 : OpCode.LDARG1 [2 datoshi]
    /// 45 : OpCode.CALLA [512 datoshi]
    /// 46 : OpCode.LDLOC0 [2 datoshi]
    /// 47 : OpCode.REVERSE3 [2 datoshi]
    /// 48 : OpCode.SETITEM [8192 datoshi]
    /// 49 : OpCode.DROP [2 datoshi]
    /// 4A : OpCode.LDLOC3 [2 datoshi]
    /// 4B : OpCode.INC [4 datoshi]
    /// 4C : OpCode.STLOC3 [2 datoshi]
    /// 4D : OpCode.LDLOC3 [2 datoshi]
    /// 4E : OpCode.LDLOC2 [2 datoshi]
    /// 4F : OpCode.JMPLT EC [2 datoshi]
    /// 51 : OpCode.LDLOC0 [2 datoshi]
    /// 52 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("toMap")]
    public abstract object? ToMap(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClkAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.PUSHA 59000000 [4 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.CALL 03 [512 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract object? WhereGreaterThanZero(IList<object>? array);

    #endregion
}
