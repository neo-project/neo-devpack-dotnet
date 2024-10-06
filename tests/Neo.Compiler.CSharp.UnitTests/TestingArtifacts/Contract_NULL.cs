using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NULL(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NULL"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""isNull"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""equalNullA"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":9,""safe"":false},{""name"":""equalNullB"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":16,""safe"":false},{""name"":""equalNotNullA"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":23,""safe"":false},{""name"":""equalNotNullB"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":30,""safe"":false},{""name"":""nullCoalescing"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""String"",""offset"":37,""safe"":false},{""name"":""nullCollation"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""String"",""offset"":51,""safe"":false},{""name"":""nullPropertyGT"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":70,""safe"":false},{""name"":""nullPropertyLT"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":82,""safe"":false},{""name"":""nullPropertyGE"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":94,""safe"":false},{""name"":""nullPropertyLE"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":106,""safe"":false},{""name"":""nullProperty"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":118,""safe"":false},{""name"":""ifNull"",""parameters"":[{""name"":""obj"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":130,""safe"":false},{""name"":""nullCollationAndCollation"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""Any"",""offset"":140,""safe"":false},{""name"":""nullCollationAndCollation2"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""Any"",""offset"":169,""safe"":false},{""name"":""nullType"",""parameters"":[],""returntype"":""Void"",""offset"":210,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOVXAQF4cGgLl0BXAAELeJdAVwABeAuXQFcAAQt4mEBXAAF4C5hAVwEBeErYJAUREoxwaEBXAQF4StgmCkUMBWxpbnV4cGhAVwABeErYJAPKELdAVwABeErYJAPKELVAVwABeErYJAPKELhAVwABeErYJAPKELZAVwABeErYJAPKEJhAVwABeCYECEAJQFcBAUGb9mfOcHhoQZJd6DFK2CYKRQwBe9sw2yhAVwEBQZv2Z85wDAMxMTF4aEHmPxiEeGhBkl3oMUrYJgpFDAF72zDbKEBXAQALcGhK2CQFNAVARUBXAAFARiQ5mA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABC3iYQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHNULL
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("equalNotNullA")]
    public abstract bool? EqualNotNullA(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuYQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHNULL
    /// 0005 : OpCode.NOTEQUAL
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("equalNotNullB")]
    public abstract bool? EqualNotNullB(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABC3iXQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.PUSHNULL
    /// 0004 : OpCode.LDARG0
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("equalNullA")]
    public abstract bool? EqualNullA(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHNULL
    /// 0005 : OpCode.EQUAL
    /// 0006 : OpCode.RET
    /// </remarks>
    [DisplayName("equalNullB")]
    public abstract bool? EqualNullB(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCYECEAJQA==
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.JMPIFNOT 04
    /// 0006 : OpCode.PUSHT
    /// 0007 : OpCode.RET
    /// 0008 : OpCode.PUSHF
    /// 0009 : OpCode.RET
    /// </remarks>
    [DisplayName("ifNull")]
    public abstract bool? IfNull(object? obj = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoC5dA
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.PUSHNULL
    /// 0007 : OpCode.EQUAL
    /// 0008 : OpCode.RET
    /// </remarks>
    [DisplayName("isNull")]
    public abstract bool? IsNull(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErYJAUREoxwaEA=
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 05
    /// 0008 : OpCode.PUSH1
    /// 0009 : OpCode.PUSH2
    /// 000A : OpCode.SUBSTR
    /// 000B : OpCode.STLOC0
    /// 000C : OpCode.LDLOC0
    /// 000D : OpCode.RET
    /// </remarks>
    [DisplayName("nullCoalescing")]
    public abstract string? NullCoalescing(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErYJgpFDGxpbnV4cGhA
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIFNOT 0A
    /// 0008 : OpCode.DROP
    /// 0009 : OpCode.PUSHDATA1 6C696E7578
    /// 0010 : OpCode.STLOC0
    /// 0011 : OpCode.LDLOC0
    /// 0012 : OpCode.RET
    /// </remarks>
    [DisplayName("nullCollation")]
    public abstract string? NullCollation(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z85weGhBkl3oMUrYJgpFDHvbMNsoQA==
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.SYSCALL 9BF667CE
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDARG0
    /// 000A : OpCode.LDLOC0
    /// 000B : OpCode.SYSCALL 925DE831
    /// 0010 : OpCode.DUP
    /// 0011 : OpCode.ISNULL
    /// 0012 : OpCode.JMPIFNOT 0A
    /// 0014 : OpCode.DROP
    /// 0015 : OpCode.PUSHDATA1 7B
    /// 0018 : OpCode.CONVERT 30
    /// 001A : OpCode.CONVERT 28
    /// 001C : OpCode.RET
    /// </remarks>
    [DisplayName("nullCollationAndCollation")]
    public abstract object? NullCollationAndCollation(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z85wDDExMXhoQeY/GIR4aEGSXegxStgmCkUMe9sw2yhA
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.SYSCALL 9BF667CE
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.PUSHDATA1 313131
    /// 000E : OpCode.LDARG0
    /// 000F : OpCode.LDLOC0
    /// 0010 : OpCode.SYSCALL E63F1884
    /// 0015 : OpCode.LDARG0
    /// 0016 : OpCode.LDLOC0
    /// 0017 : OpCode.SYSCALL 925DE831
    /// 001C : OpCode.DUP
    /// 001D : OpCode.ISNULL
    /// 001E : OpCode.JMPIFNOT 0A
    /// 0020 : OpCode.DROP
    /// 0021 : OpCode.PUSHDATA1 7B
    /// 0024 : OpCode.CONVERT 30
    /// 0026 : OpCode.CONVERT 28
    /// 0028 : OpCode.RET
    /// </remarks>
    [DisplayName("nullCollationAndCollation2")]
    public abstract object? NullCollationAndCollation2(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKEJhA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 03
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.NOTEQUAL
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("nullProperty")]
    public abstract bool? NullProperty(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELhA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 03
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.GE
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("nullPropertyGE")]
    public abstract bool? NullPropertyGE(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELdA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 03
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.GT
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("nullPropertyGT")]
    public abstract bool? NullPropertyGT(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELZA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 03
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.LE
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("nullPropertyLE")]
    public abstract bool? NullPropertyLE(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELVA
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.ISNULL
    /// 0006 : OpCode.JMPIF 03
    /// 0008 : OpCode.SIZE
    /// 0009 : OpCode.PUSH0
    /// 000A : OpCode.LT
    /// 000B : OpCode.RET
    /// </remarks>
    [DisplayName("nullPropertyLT")]
    public abstract bool? NullPropertyLT(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3BoStgkBTQFQEVA
    /// 0000 : OpCode.INITSLOT 0100
    /// 0003 : OpCode.PUSHNULL
    /// 0004 : OpCode.STLOC0
    /// 0005 : OpCode.LDLOC0
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.ISNULL
    /// 0008 : OpCode.JMPIF 05
    /// 000A : OpCode.CALL 05
    /// 000C : OpCode.RET
    /// 000D : OpCode.DROP
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("nullType")]
    public abstract void NullType();

    #endregion

}
