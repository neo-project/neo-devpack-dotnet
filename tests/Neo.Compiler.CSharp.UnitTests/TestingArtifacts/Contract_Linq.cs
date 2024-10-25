using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Linq(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Linq"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""aggregateSum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""allGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":112,""safe"":false},{""name"":""isEmpty"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":207,""safe"":false},{""name"":""anyGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Boolean"",""offset"":268,""safe"":false},{""name"":""anyGreaterThan"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":365,""safe"":false},{""name"":""average"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":386,""safe"":false},{""name"":""averageTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":485,""safe"":false},{""name"":""count"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":614,""safe"":false},{""name"":""countGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":682,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":784,""safe"":false},{""name"":""containsText"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""String""}],""returntype"":""Boolean"",""offset"":868,""safe"":false},{""name"":""containsPerson"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":876,""safe"":false},{""name"":""containsPersonIndex"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""targetIndex"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":960,""safe"":false},{""name"":""containsPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""target"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":1019,""safe"":false},{""name"":""firstGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1089,""safe"":false},{""name"":""selectTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1187,""safe"":false},{""name"":""selectPersonS"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1283,""safe"":false},{""name"":""skip"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1367,""safe"":false},{""name"":""sum"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1446,""safe"":false},{""name"":""sumTwice"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Integer"",""offset"":1490,""safe"":false},{""name"":""take"",""parameters"":[{""name"":""array"",""type"":""Array""},{""name"":""count"",""type"":""Integer""}],""returntype"":""Any"",""offset"":1587,""safe"":false},{""name"":""toMap"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1666,""safe"":false},{""name"":""whereGreaterThanZero"",""parameters"":[{""name"":""array"",""type"":""Array""}],""returntype"":""Any"",""offset"":1837,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":1937,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""itoa""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

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
    /// Script: VwABCmYAAAAQeDQDQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 66000000
    /// 08 : OpCode.PUSH0
    /// 09 : OpCode.LDARG0
    /// 0A : OpCode.CALL 03
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("aggregateSum")]
    public abstract BigInteger? AggregateSum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClUAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 55000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("allGreaterThanZero")]
    public abstract bool? AllGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWAKCQAAAHg0oEA=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.STSFLD0
    /// 05 : OpCode.PUSHA 09000000
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.CALL A0
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("anyGreaterThan")]
    public abstract bool? AnyGreaterThan(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClcAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 57000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("anyGreaterThanZero")]
    public abstract bool? AnyGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwYCeDRQDHNlbGVjdG9yeTVP/v//EHAQcXhKcspzEHQiFGpsznVoSpxwRWlteTaecWycdGxrMOxoEJcmFAxzb3VyY2UgaXMgZW1wdHk6aWihQA==
    /// 00 : OpCode.INITSLOT 0602
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 50
    /// 06 : OpCode.PUSHDATA1 73656C6563746F72
    /// 10 : OpCode.LDARG1
    /// 11 : OpCode.CALL_L 4FFEFFFF
    /// 16 : OpCode.PUSH0
    /// 17 : OpCode.STLOC0
    /// 18 : OpCode.PUSH0
    /// 19 : OpCode.STLOC1
    /// 1A : OpCode.LDARG0
    /// 1B : OpCode.DUP
    /// 1C : OpCode.STLOC2
    /// 1D : OpCode.SIZE
    /// 1E : OpCode.STLOC3
    /// 1F : OpCode.PUSH0
    /// 20 : OpCode.STLOC4
    /// 21 : OpCode.JMP 14
    /// 23 : OpCode.LDLOC2
    /// 24 : OpCode.LDLOC4
    /// 25 : OpCode.PICKITEM
    /// 26 : OpCode.STLOC5
    /// 27 : OpCode.LDLOC0
    /// 28 : OpCode.DUP
    /// 29 : OpCode.INC
    /// 2A : OpCode.STLOC0
    /// 2B : OpCode.DROP
    /// 2C : OpCode.LDLOC1
    /// 2D : OpCode.LDLOC5
    /// 2E : OpCode.LDARG1
    /// 2F : OpCode.CALLA
    /// 30 : OpCode.ADD
    /// 31 : OpCode.STLOC1
    /// 32 : OpCode.LDLOC4
    /// 33 : OpCode.INC
    /// 34 : OpCode.STLOC4
    /// 35 : OpCode.LDLOC4
    /// 36 : OpCode.LDLOC3
    /// 37 : OpCode.JMPLT EC
    /// 39 : OpCode.LDLOC0
    /// 3A : OpCode.PUSH0
    /// 3B : OpCode.EQUAL
    /// 3C : OpCode.JMPIFNOT 14
    /// 3E : OpCode.PUSHDATA1 736F7572636520697320656D707479
    /// 4F : OpCode.THROW
    /// 50 : OpCode.LDLOC1
    /// 51 : OpCode.LDLOC0
    /// 52 : OpCode.DIV
    /// 53 : OpCode.RET
    /// </remarks>
    [DisplayName("average")]
    public abstract BigInteger? Average(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCncAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 77000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("averageTwice")]
    public abstract BigInteger? AverageTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeWEKQAAAAHg0A0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.STSFLD1
    /// 05 : OpCode.PUSHA 40000000
    /// 0A : OpCode.LDARG0
    /// 0B : OpCode.CALL 03
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("contains")]
    public abstract bool? Contains(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIh9pa850aBALEsBKNDRsNwAASxBR0GxLEVHQz2ucc2tqMOEQCxLASjQYeTcAAEsQUdB5SxFR0HFpaDVm////QA==
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.DUP
    /// 07 : OpCode.STLOC1
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.STLOC2
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC3
    /// 0C : OpCode.JMP 1F
    /// 0E : OpCode.LDLOC1
    /// 0F : OpCode.LDLOC3
    /// 10 : OpCode.PICKITEM
    /// 11 : OpCode.STLOC4
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.PUSH0
    /// 14 : OpCode.PUSHNULL
    /// 15 : OpCode.PUSH2
    /// 16 : OpCode.PACK
    /// 17 : OpCode.DUP
    /// 18 : OpCode.CALL 34
    /// 1A : OpCode.LDLOC4
    /// 1B : OpCode.CALLT 0000
    /// 1E : OpCode.OVER
    /// 1F : OpCode.PUSH0
    /// 20 : OpCode.ROT
    /// 21 : OpCode.SETITEM
    /// 22 : OpCode.LDLOC4
    /// 23 : OpCode.OVER
    /// 24 : OpCode.PUSH1
    /// 25 : OpCode.ROT
    /// 26 : OpCode.SETITEM
    /// 27 : OpCode.APPEND
    /// 28 : OpCode.LDLOC3
    /// 29 : OpCode.INC
    /// 2A : OpCode.STLOC3
    /// 2B : OpCode.LDLOC3
    /// 2C : OpCode.LDLOC2
    /// 2D : OpCode.JMPLT E1
    /// 2F : OpCode.PUSH0
    /// 30 : OpCode.PUSHNULL
    /// 31 : OpCode.PUSH2
    /// 32 : OpCode.PACK
    /// 33 : OpCode.DUP
    /// 34 : OpCode.CALL 18
    /// 36 : OpCode.LDARG1
    /// 37 : OpCode.CALLT 0000
    /// 3A : OpCode.OVER
    /// 3B : OpCode.PUSH0
    /// 3C : OpCode.ROT
    /// 3D : OpCode.SETITEM
    /// 3E : OpCode.LDARG1
    /// 3F : OpCode.OVER
    /// 40 : OpCode.PUSH1
    /// 41 : OpCode.ROT
    /// 42 : OpCode.SETITEM
    /// 43 : OpCode.STLOC1
    /// 44 : OpCode.LDLOC1
    /// 45 : OpCode.LDLOC0
    /// 46 : OpCode.CALL_L 66FFFFFF
    /// 4B : OpCode.RET
    /// </remarks>
    [DisplayName("containsPerson")]
    public abstract bool? ContainsPerson(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIh9pa850aBALEsBKNOBsNwAASxBR0GxLEVHQz2ucc2tqMOFoec5xaWg1I////0A=
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.DUP
    /// 07 : OpCode.STLOC1
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.STLOC2
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC3
    /// 0C : OpCode.JMP 1F
    /// 0E : OpCode.LDLOC1
    /// 0F : OpCode.LDLOC3
    /// 10 : OpCode.PICKITEM
    /// 11 : OpCode.STLOC4
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.PUSH0
    /// 14 : OpCode.PUSHNULL
    /// 15 : OpCode.PUSH2
    /// 16 : OpCode.PACK
    /// 17 : OpCode.DUP
    /// 18 : OpCode.CALL E0
    /// 1A : OpCode.LDLOC4
    /// 1B : OpCode.CALLT 0000
    /// 1E : OpCode.OVER
    /// 1F : OpCode.PUSH0
    /// 20 : OpCode.ROT
    /// 21 : OpCode.SETITEM
    /// 22 : OpCode.LDLOC4
    /// 23 : OpCode.OVER
    /// 24 : OpCode.PUSH1
    /// 25 : OpCode.ROT
    /// 26 : OpCode.SETITEM
    /// 27 : OpCode.APPEND
    /// 28 : OpCode.LDLOC3
    /// 29 : OpCode.INC
    /// 2A : OpCode.STLOC3
    /// 2B : OpCode.LDLOC3
    /// 2C : OpCode.LDLOC2
    /// 2D : OpCode.JMPLT E1
    /// 2F : OpCode.LDLOC0
    /// 30 : OpCode.LDARG1
    /// 31 : OpCode.PICKITEM
    /// 32 : OpCode.STLOC1
    /// 33 : OpCode.LDLOC1
    /// 34 : OpCode.LDLOC0
    /// 35 : OpCode.CALL_L 23FFFFFF
    /// 3A : OpCode.RET
    /// </remarks>
    [DisplayName("containsPersonIndex")]
    public abstract bool? ContainsPersonIndex(IList<object>? array, BigInteger? targetIndex);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCwnB4SnHKchBzIhxpa850aBALEr9sNwAASxBR0GxLEVHQz2ucc2tqMOQQCxK/eTcAAEsQUdB5SxFR0HFpaDXd/v//QA==
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.DUP
    /// 07 : OpCode.STLOC1
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.STLOC2
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC3
    /// 0C : OpCode.JMP 1C
    /// 0E : OpCode.LDLOC1
    /// 0F : OpCode.LDLOC3
    /// 10 : OpCode.PICKITEM
    /// 11 : OpCode.STLOC4
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.PUSH0
    /// 14 : OpCode.PUSHNULL
    /// 15 : OpCode.PUSH2
    /// 16 : OpCode.PACKSTRUCT
    /// 17 : OpCode.LDLOC4
    /// 18 : OpCode.CALLT 0000
    /// 1B : OpCode.OVER
    /// 1C : OpCode.PUSH0
    /// 1D : OpCode.ROT
    /// 1E : OpCode.SETITEM
    /// 1F : OpCode.LDLOC4
    /// 20 : OpCode.OVER
    /// 21 : OpCode.PUSH1
    /// 22 : OpCode.ROT
    /// 23 : OpCode.SETITEM
    /// 24 : OpCode.APPEND
    /// 25 : OpCode.LDLOC3
    /// 26 : OpCode.INC
    /// 27 : OpCode.STLOC3
    /// 28 : OpCode.LDLOC3
    /// 29 : OpCode.LDLOC2
    /// 2A : OpCode.JMPLT E4
    /// 2C : OpCode.PUSH0
    /// 2D : OpCode.PUSHNULL
    /// 2E : OpCode.PUSH2
    /// 2F : OpCode.PACKSTRUCT
    /// 30 : OpCode.LDARG1
    /// 31 : OpCode.CALLT 0000
    /// 34 : OpCode.OVER
    /// 35 : OpCode.PUSH0
    /// 36 : OpCode.ROT
    /// 37 : OpCode.SETITEM
    /// 38 : OpCode.LDARG1
    /// 39 : OpCode.OVER
    /// 3A : OpCode.PUSH1
    /// 3B : OpCode.ROT
    /// 3C : OpCode.SETITEM
    /// 3D : OpCode.STLOC1
    /// 3E : OpCode.LDLOC1
    /// 3F : OpCode.LDLOC0
    /// 40 : OpCode.CALL_L DDFEFFFF
    /// 45 : OpCode.RET
    /// </remarks>
    [DisplayName("containsPersonS")]
    public abstract bool? ContainsPersonS(IList<object>? array, BigInteger? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0r0A=
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.LDARG1
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.CALL AF
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("containsText")]
    public abstract bool? ContainsText(IList<object>? array, string? target);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDQ1DHByZWRpY2F0ZXk1if3//xBweEpxynIQcyITaWvOdGx5NiYHaEqccEVrnHNrajDtaEA=
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 35
    /// 06 : OpCode.PUSHDATA1 707265646963617465
    /// 11 : OpCode.LDARG1
    /// 12 : OpCode.CALL_L 89FDFFFF
    /// 17 : OpCode.PUSH0
    /// 18 : OpCode.STLOC0
    /// 19 : OpCode.LDARG0
    /// 1A : OpCode.DUP
    /// 1B : OpCode.STLOC1
    /// 1C : OpCode.SIZE
    /// 1D : OpCode.STLOC2
    /// 1E : OpCode.PUSH0
    /// 1F : OpCode.STLOC3
    /// 20 : OpCode.JMP 13
    /// 22 : OpCode.LDLOC1
    /// 23 : OpCode.LDLOC3
    /// 24 : OpCode.PICKITEM
    /// 25 : OpCode.STLOC4
    /// 26 : OpCode.LDLOC4
    /// 27 : OpCode.LDARG1
    /// 28 : OpCode.CALLA
    /// 29 : OpCode.JMPIFNOT 07
    /// 2B : OpCode.LDLOC0
    /// 2C : OpCode.DUP
    /// 2D : OpCode.INC
    /// 2E : OpCode.STLOC0
    /// 2F : OpCode.DROP
    /// 30 : OpCode.LDLOC3
    /// 31 : OpCode.INC
    /// 32 : OpCode.STLOC3
    /// 33 : OpCode.LDLOC3
    /// 34 : OpCode.LDLOC2
    /// 35 : OpCode.JMPLT ED
    /// 37 : OpCode.LDLOC0
    /// 38 : OpCode.RET
    /// </remarks>
    [DisplayName("count")]
    public abstract BigInteger? Count(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClwAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 5C000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("countGreaterThanZero")]
    public abstract BigInteger? CountGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABDwpXAAAAeDQDQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHM1
    /// 04 : OpCode.PUSHA 57000000
    /// 09 : OpCode.LDARG0
    /// 0A : OpCode.CALL 03
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("firstGreaterThanZero")]
    public abstract BigInteger? FirstGreaterThanZero(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQEqkA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 04
    /// 06 : OpCode.NOT
    /// 07 : OpCode.RET
    /// </remarks>
    [DisplayName("isEmpty")]
    public abstract bool? IsEmpty(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUBwnB4SnHKchBzIiJpa850aBALEsBKNZ3+//9sNwAASxBR0GxLEVHQz2ucc2tqMN4KDAAAAGg1dP///0A=
    /// 00 : OpCode.INITSLOT 0501
    /// 03 : OpCode.NEWARRAY0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.DUP
    /// 07 : OpCode.STLOC1
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.STLOC2
    /// 0A : OpCode.PUSH0
    /// 0B : OpCode.STLOC3
    /// 0C : OpCode.JMP 22
    /// 0E : OpCode.LDLOC1
    /// 0F : OpCode.LDLOC3
    /// 10 : OpCode.PICKITEM
    /// 11 : OpCode.STLOC4
    /// 12 : OpCode.LDLOC0
    /// 13 : OpCode.PUSH0
    /// 14 : OpCode.PUSHNULL
    /// 15 : OpCode.PUSH2
    /// 16 : OpCode.PACK
    /// 17 : OpCode.DUP
    /// 18 : OpCode.CALL_L 9DFEFFFF
    /// 1D : OpCode.LDLOC4
    /// 1E : OpCode.CALLT 0000
    /// 21 : OpCode.OVER
    /// 22 : OpCode.PUSH0
    /// 23 : OpCode.ROT
    /// 24 : OpCode.SETITEM
    /// 25 : OpCode.LDLOC4
    /// 26 : OpCode.OVER
    /// 27 : OpCode.PUSH1
    /// 28 : OpCode.ROT
    /// 29 : OpCode.SETITEM
    /// 2A : OpCode.APPEND
    /// 2B : OpCode.LDLOC3
    /// 2C : OpCode.INC
    /// 2D : OpCode.STLOC3
    /// 2E : OpCode.LDLOC3
    /// 2F : OpCode.LDLOC2
    /// 30 : OpCode.JMPLT DE
    /// 32 : OpCode.PUSHA 0C000000
    /// 37 : OpCode.LDLOC0
    /// 38 : OpCode.CALL_L 74FFFFFF
    /// 3D : OpCode.RET
    /// </remarks>
    [DisplayName("selectPersonS")]
    public abstract object? SelectPersonS(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClYAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 56000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("selectTwice")]
    public abstract object? SelectTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDQpwnB4SnHKchBzIhhpa850eRC3Jgl5Sp2BRSIFaGzPa5xza2ow6GhA
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 29
    /// 06 : OpCode.NEWARRAY0
    /// 07 : OpCode.STLOC0
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.DUP
    /// 0A : OpCode.STLOC1
    /// 0B : OpCode.SIZE
    /// 0C : OpCode.STLOC2
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.STLOC3
    /// 0F : OpCode.JMP 18
    /// 11 : OpCode.LDLOC1
    /// 12 : OpCode.LDLOC3
    /// 13 : OpCode.PICKITEM
    /// 14 : OpCode.STLOC4
    /// 15 : OpCode.LDARG1
    /// 16 : OpCode.PUSH0
    /// 17 : OpCode.GT
    /// 18 : OpCode.JMPIFNOT 09
    /// 1A : OpCode.LDARG1
    /// 1B : OpCode.DUP
    /// 1C : OpCode.DEC
    /// 1D : OpCode.STARG1
    /// 1E : OpCode.DROP
    /// 1F : OpCode.JMP 05
    /// 21 : OpCode.LDLOC0
    /// 22 : OpCode.LDLOC4
    /// 23 : OpCode.APPEND
    /// 24 : OpCode.LDLOC3
    /// 25 : OpCode.INC
    /// 26 : OpCode.STLOC3
    /// 27 : OpCode.LDLOC3
    /// 28 : OpCode.LDLOC2
    /// 29 : OpCode.JMPLT E8
    /// 2B : OpCode.LDLOC0
    /// 2C : OpCode.RET
    /// </remarks>
    [DisplayName("skip")]
    public abstract object? Skip(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDQwDHNlbGVjdG9yeTVi+v//EHB4SnHKchBzIg9pa850aGx5Np5wa5xza2ow8WhA
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 30
    /// 06 : OpCode.PUSHDATA1 73656C6563746F72
    /// 10 : OpCode.LDARG1
    /// 11 : OpCode.CALL_L 62FAFFFF
    /// 16 : OpCode.PUSH0
    /// 17 : OpCode.STLOC0
    /// 18 : OpCode.LDARG0
    /// 19 : OpCode.DUP
    /// 1A : OpCode.STLOC1
    /// 1B : OpCode.SIZE
    /// 1C : OpCode.STLOC2
    /// 1D : OpCode.PUSH0
    /// 1E : OpCode.STLOC3
    /// 1F : OpCode.JMP 0F
    /// 21 : OpCode.LDLOC1
    /// 22 : OpCode.LDLOC3
    /// 23 : OpCode.PICKITEM
    /// 24 : OpCode.STLOC4
    /// 25 : OpCode.LDLOC0
    /// 26 : OpCode.LDLOC4
    /// 27 : OpCode.LDARG1
    /// 28 : OpCode.CALLA
    /// 29 : OpCode.ADD
    /// 2A : OpCode.STLOC0
    /// 2B : OpCode.LDLOC3
    /// 2C : OpCode.INC
    /// 2D : OpCode.STLOC3
    /// 2E : OpCode.LDLOC3
    /// 2F : OpCode.LDLOC2
    /// 30 : OpCode.JMPLT F1
    /// 32 : OpCode.LDLOC0
    /// 33 : OpCode.RET
    /// </remarks>
    [DisplayName("sum")]
    public abstract BigInteger? Sum(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABClcAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 57000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("sumTwice")]
    public abstract BigInteger? SumTwice(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUCeDQpwnB4SnHKchBzIhhpa850eRC2JgQiEWhsz3lKnYFFa5xza2ow6GhA
    /// 00 : OpCode.INITSLOT 0502
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 29
    /// 06 : OpCode.NEWARRAY0
    /// 07 : OpCode.STLOC0
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.DUP
    /// 0A : OpCode.STLOC1
    /// 0B : OpCode.SIZE
    /// 0C : OpCode.STLOC2
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.STLOC3
    /// 0F : OpCode.JMP 18
    /// 11 : OpCode.LDLOC1
    /// 12 : OpCode.LDLOC3
    /// 13 : OpCode.PICKITEM
    /// 14 : OpCode.STLOC4
    /// 15 : OpCode.LDARG1
    /// 16 : OpCode.PUSH0
    /// 17 : OpCode.LE
    /// 18 : OpCode.JMPIFNOT 04
    /// 1A : OpCode.JMP 11
    /// 1C : OpCode.LDLOC0
    /// 1D : OpCode.LDLOC4
    /// 1E : OpCode.APPEND
    /// 1F : OpCode.LDARG1
    /// 20 : OpCode.DUP
    /// 21 : OpCode.DEC
    /// 22 : OpCode.STARG1
    /// 23 : OpCode.DROP
    /// 24 : OpCode.LDLOC3
    /// 25 : OpCode.INC
    /// 26 : OpCode.STLOC3
    /// 27 : OpCode.LDLOC3
    /// 28 : OpCode.LDLOC2
    /// 29 : OpCode.JMPLT E8
    /// 2B : OpCode.LDLOC0
    /// 2C : OpCode.RET
    /// </remarks>
    [DisplayName("take")]
    public abstract object? Take(IList<object>? array, BigInteger? count);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUDeDRPDGtleVNlbGVjdG9yeTWJ+f//DGVsZW1lbnRTZWxlY3Rvcno1cvn//8hweEpxynIQcyIUaWvOdGx6NkpseTZoU9BFa5xza2ow7GhA
    /// 00 : OpCode.INITSLOT 0503
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.CALL 4F
    /// 06 : OpCode.PUSHDATA1 6B657953656C6563746F72
    /// 13 : OpCode.LDARG1
    /// 14 : OpCode.CALL_L 89F9FFFF
    /// 19 : OpCode.PUSHDATA1 656C656D656E7453656C6563746F72
    /// 2A : OpCode.LDARG2
    /// 2B : OpCode.CALL_L 72F9FFFF
    /// 30 : OpCode.NEWMAP
    /// 31 : OpCode.STLOC0
    /// 32 : OpCode.LDARG0
    /// 33 : OpCode.DUP
    /// 34 : OpCode.STLOC1
    /// 35 : OpCode.SIZE
    /// 36 : OpCode.STLOC2
    /// 37 : OpCode.PUSH0
    /// 38 : OpCode.STLOC3
    /// 39 : OpCode.JMP 14
    /// 3B : OpCode.LDLOC1
    /// 3C : OpCode.LDLOC3
    /// 3D : OpCode.PICKITEM
    /// 3E : OpCode.STLOC4
    /// 3F : OpCode.LDLOC4
    /// 40 : OpCode.LDARG2
    /// 41 : OpCode.CALLA
    /// 42 : OpCode.DUP
    /// 43 : OpCode.LDLOC4
    /// 44 : OpCode.LDARG1
    /// 45 : OpCode.CALLA
    /// 46 : OpCode.LDLOC0
    /// 47 : OpCode.REVERSE3
    /// 48 : OpCode.SETITEM
    /// 49 : OpCode.DROP
    /// 4A : OpCode.LDLOC3
    /// 4B : OpCode.INC
    /// 4C : OpCode.STLOC3
    /// 4D : OpCode.LDLOC3
    /// 4E : OpCode.LDLOC2
    /// 4F : OpCode.JMPLT EC
    /// 51 : OpCode.LDLOC0
    /// 52 : OpCode.RET
    /// </remarks>
    [DisplayName("toMap")]
    public abstract object? ToMap(IList<object>? array);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABCloAAAB4NANA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHA 5A000000
    /// 08 : OpCode.LDARG0
    /// 09 : OpCode.CALL 03
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("whereGreaterThanZero")]
    public abstract object? WhereGreaterThanZero(IList<object>? array);

    #endregion
}
