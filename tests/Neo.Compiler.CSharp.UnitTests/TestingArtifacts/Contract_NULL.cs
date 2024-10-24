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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAN9XAQF4cGgLl0BXAAELeJdAVwABeAuXQFcAAQt4mEBXAAF4C5hAVwEBeErYJAUREoxwaEBXAQF4StgmCkUMBWxpbnV4cGhAVwABeErYJAPKELdAVwABeErYJAPKELVAVwABeErYJAPKELhAVwABeErYJAPKELZAVwABeErYJAPKEJhAVwABeCYECEAJQFcBAUGb9mfOcHhoQZJd6DFK2CYKRQwBe9sw2yhAVwEBQZv2Z85wDAMxMTF4aEHmPxiEeGhBkl3oMUrYJgpFDAF72zDbKEBXAQALcGhK2CQDQEVAaY7bZQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABC3iYQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("equalNotNullA")]
    public abstract bool? EqualNotNullA(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuYQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.NOTEQUAL
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("equalNotNullB")]
    public abstract bool? EqualNotNullB(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABC3iXQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.LDARG0
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("equalNullA")]
    public abstract bool? EqualNullA(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHNULL
    /// 05 : OpCode.EQUAL
    /// 06 : OpCode.RET
    /// </remarks>
    [DisplayName("equalNullB")]
    public abstract bool? EqualNullB(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCYECEAJQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.JMPIFNOT 04
    /// 06 : OpCode.PUSHT
    /// 07 : OpCode.RET
    /// 08 : OpCode.PUSHF
    /// 09 : OpCode.RET
    /// </remarks>
    [DisplayName("ifNull")]
    public abstract bool? IfNull(object? obj = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoC5dA
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.PUSHNULL
    /// 07 : OpCode.EQUAL
    /// 08 : OpCode.RET
    /// </remarks>
    [DisplayName("isNull")]
    public abstract bool? IsNull(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErYJAUREoxwaEA=
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 05
    /// 08 : OpCode.PUSH1
    /// 09 : OpCode.PUSH2
    /// 0A : OpCode.SUBSTR
    /// 0B : OpCode.STLOC0
    /// 0C : OpCode.LDLOC0
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("nullCoalescing")]
    public abstract string? NullCoalescing(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErYJgpFDGxpbnV4cGhA
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIFNOT 0A
    /// 08 : OpCode.DROP
    /// 09 : OpCode.PUSHDATA1 6C696E7578
    /// 10 : OpCode.STLOC0
    /// 11 : OpCode.LDLOC0
    /// 12 : OpCode.RET
    /// </remarks>
    [DisplayName("nullCollation")]
    public abstract string? NullCollation(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z85weGhBkl3oMUrYJgpFDHvbMNsoQA==
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.SYSCALL 9BF667CE
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDARG0
    /// 0A : OpCode.LDLOC0
    /// 0B : OpCode.SYSCALL 925DE831
    /// 10 : OpCode.DUP
    /// 11 : OpCode.ISNULL
    /// 12 : OpCode.JMPIFNOT 0A
    /// 14 : OpCode.DROP
    /// 15 : OpCode.PUSHDATA1 7B
    /// 18 : OpCode.CONVERT 30
    /// 1A : OpCode.CONVERT 28
    /// 1C : OpCode.RET
    /// </remarks>
    [DisplayName("nullCollationAndCollation")]
    public abstract object? NullCollationAndCollation(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z85wDDExMXhoQeY/GIR4aEGSXegxStgmCkUMe9sw2yhA
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.SYSCALL 9BF667CE
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.PUSHDATA1 313131
    /// 0E : OpCode.LDARG0
    /// 0F : OpCode.LDLOC0
    /// 10 : OpCode.SYSCALL E63F1884
    /// 15 : OpCode.LDARG0
    /// 16 : OpCode.LDLOC0
    /// 17 : OpCode.SYSCALL 925DE831
    /// 1C : OpCode.DUP
    /// 1D : OpCode.ISNULL
    /// 1E : OpCode.JMPIFNOT 0A
    /// 20 : OpCode.DROP
    /// 21 : OpCode.PUSHDATA1 7B
    /// 24 : OpCode.CONVERT 30
    /// 26 : OpCode.CONVERT 28
    /// 28 : OpCode.RET
    /// </remarks>
    [DisplayName("nullCollationAndCollation2")]
    public abstract object? NullCollationAndCollation2(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKEJhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 03
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.NOTEQUAL
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("nullProperty")]
    public abstract bool? NullProperty(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELhA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 03
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.GE
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("nullPropertyGE")]
    public abstract bool? NullPropertyGE(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELdA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 03
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.GT
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("nullPropertyGT")]
    public abstract bool? NullPropertyGT(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELZA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 03
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.LE
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("nullPropertyLE")]
    public abstract bool? NullPropertyLE(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELVA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.ISNULL
    /// 06 : OpCode.JMPIF 03
    /// 08 : OpCode.SIZE
    /// 09 : OpCode.PUSH0
    /// 0A : OpCode.LT
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("nullPropertyLT")]
    public abstract bool? NullPropertyLT(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3BoStgkA0BFQA==
    /// 00 : OpCode.INITSLOT 0100
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.STLOC0
    /// 05 : OpCode.LDLOC0
    /// 06 : OpCode.DUP
    /// 07 : OpCode.ISNULL
    /// 08 : OpCode.JMPIF 03
    /// 0A : OpCode.RET
    /// 0B : OpCode.DROP
    /// 0C : OpCode.RET
    /// </remarks>
    [DisplayName("nullType")]
    public abstract void NullType();

    #endregion
}
