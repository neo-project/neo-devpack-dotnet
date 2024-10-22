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
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.PUSHNULL 	-> 1 datoshi
    /// 04 : OpCode.LDARG0 	-> 2 datoshi
    /// 05 : OpCode.NOTEQUAL 	-> 32 datoshi
    /// 06 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("equalNotNullA")]
    public abstract bool? EqualNotNullA(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuYQA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.PUSHNULL 	-> 1 datoshi
    /// 05 : OpCode.NOTEQUAL 	-> 32 datoshi
    /// 06 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("equalNotNullB")]
    public abstract bool? EqualNotNullB(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABC3iXQA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.PUSHNULL 	-> 1 datoshi
    /// 04 : OpCode.LDARG0 	-> 2 datoshi
    /// 05 : OpCode.EQUAL 	-> 32 datoshi
    /// 06 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("equalNullA")]
    public abstract bool? EqualNullA(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAuXQA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.PUSHNULL 	-> 1 datoshi
    /// 05 : OpCode.EQUAL 	-> 32 datoshi
    /// 06 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("equalNullB")]
    public abstract bool? EqualNullB(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCYECEAJQA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.JMPIFNOT 04 	-> 2 datoshi
    /// 06 : OpCode.PUSHT 	-> 1 datoshi
    /// 07 : OpCode.RET 	-> 0 datoshi
    /// 08 : OpCode.PUSHF 	-> 1 datoshi
    /// 09 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("ifNull")]
    public abstract bool? IfNull(object? obj = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBoC5dA
    /// 00 : OpCode.INITSLOT 0101 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.LDLOC0 	-> 2 datoshi
    /// 06 : OpCode.PUSHNULL 	-> 1 datoshi
    /// 07 : OpCode.EQUAL 	-> 32 datoshi
    /// 08 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("isNull")]
    public abstract bool? IsNull(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErYJAUREoxwaEA=
    /// 00 : OpCode.INITSLOT 0101 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.ISNULL 	-> 2 datoshi
    /// 06 : OpCode.JMPIF 05 	-> 2 datoshi
    /// 08 : OpCode.PUSH1 	-> 1 datoshi
    /// 09 : OpCode.PUSH2 	-> 1 datoshi
    /// 0A : OpCode.SUBSTR 	-> 2048 datoshi
    /// 0B : OpCode.STLOC0 	-> 2 datoshi
    /// 0C : OpCode.LDLOC0 	-> 2 datoshi
    /// 0D : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("nullCoalescing")]
    public abstract string? NullCoalescing(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErYJgpFDGxpbnV4cGhA
    /// 00 : OpCode.INITSLOT 0101 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.ISNULL 	-> 2 datoshi
    /// 06 : OpCode.JMPIFNOT 0A 	-> 2 datoshi
    /// 08 : OpCode.DROP 	-> 2 datoshi
    /// 09 : OpCode.PUSHDATA1 6C696E7578 	-> 8 datoshi
    /// 10 : OpCode.STLOC0 	-> 2 datoshi
    /// 11 : OpCode.LDLOC0 	-> 2 datoshi
    /// 12 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("nullCollation")]
    public abstract string? NullCollation(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z85weGhBkl3oMUrYJgpFDHvbMNsoQA==
    /// 00 : OpCode.INITSLOT 0101 	-> 64 datoshi
    /// 03 : OpCode.SYSCALL 9BF667CE 	-> 0 datoshi
    /// 08 : OpCode.STLOC0 	-> 2 datoshi
    /// 09 : OpCode.LDARG0 	-> 2 datoshi
    /// 0A : OpCode.LDLOC0 	-> 2 datoshi
    /// 0B : OpCode.SYSCALL 925DE831 	-> 0 datoshi
    /// 10 : OpCode.DUP 	-> 2 datoshi
    /// 11 : OpCode.ISNULL 	-> 2 datoshi
    /// 12 : OpCode.JMPIFNOT 0A 	-> 2 datoshi
    /// 14 : OpCode.DROP 	-> 2 datoshi
    /// 15 : OpCode.PUSHDATA1 7B 	-> 8 datoshi
    /// 18 : OpCode.CONVERT 30 	-> 8192 datoshi
    /// 1A : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 1C : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("nullCollationAndCollation")]
    public abstract object? NullCollationAndCollation(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z85wDDExMXhoQeY/GIR4aEGSXegxStgmCkUMe9sw2yhA
    /// 00 : OpCode.INITSLOT 0101 	-> 64 datoshi
    /// 03 : OpCode.SYSCALL 9BF667CE 	-> 0 datoshi
    /// 08 : OpCode.STLOC0 	-> 2 datoshi
    /// 09 : OpCode.PUSHDATA1 313131 	-> 8 datoshi
    /// 0E : OpCode.LDARG0 	-> 2 datoshi
    /// 0F : OpCode.LDLOC0 	-> 2 datoshi
    /// 10 : OpCode.SYSCALL E63F1884 	-> 0 datoshi
    /// 15 : OpCode.LDARG0 	-> 2 datoshi
    /// 16 : OpCode.LDLOC0 	-> 2 datoshi
    /// 17 : OpCode.SYSCALL 925DE831 	-> 0 datoshi
    /// 1C : OpCode.DUP 	-> 2 datoshi
    /// 1D : OpCode.ISNULL 	-> 2 datoshi
    /// 1E : OpCode.JMPIFNOT 0A 	-> 2 datoshi
    /// 20 : OpCode.DROP 	-> 2 datoshi
    /// 21 : OpCode.PUSHDATA1 7B 	-> 8 datoshi
    /// 24 : OpCode.CONVERT 30 	-> 8192 datoshi
    /// 26 : OpCode.CONVERT 28 	-> 8192 datoshi
    /// 28 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("nullCollationAndCollation2")]
    public abstract object? NullCollationAndCollation2(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKEJhA
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.ISNULL 	-> 2 datoshi
    /// 06 : OpCode.JMPIF 03 	-> 2 datoshi
    /// 08 : OpCode.SIZE 	-> 4 datoshi
    /// 09 : OpCode.PUSH0 	-> 1 datoshi
    /// 0A : OpCode.NOTEQUAL 	-> 32 datoshi
    /// 0B : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("nullProperty")]
    public abstract bool? NullProperty(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELhA
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.ISNULL 	-> 2 datoshi
    /// 06 : OpCode.JMPIF 03 	-> 2 datoshi
    /// 08 : OpCode.SIZE 	-> 4 datoshi
    /// 09 : OpCode.PUSH0 	-> 1 datoshi
    /// 0A : OpCode.GE 	-> 8 datoshi
    /// 0B : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("nullPropertyGE")]
    public abstract bool? NullPropertyGE(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELdA
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.ISNULL 	-> 2 datoshi
    /// 06 : OpCode.JMPIF 03 	-> 2 datoshi
    /// 08 : OpCode.SIZE 	-> 4 datoshi
    /// 09 : OpCode.PUSH0 	-> 1 datoshi
    /// 0A : OpCode.GT 	-> 8 datoshi
    /// 0B : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("nullPropertyGT")]
    public abstract bool? NullPropertyGT(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELZA
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.ISNULL 	-> 2 datoshi
    /// 06 : OpCode.JMPIF 03 	-> 2 datoshi
    /// 08 : OpCode.SIZE 	-> 4 datoshi
    /// 09 : OpCode.PUSH0 	-> 1 datoshi
    /// 0A : OpCode.LE 	-> 8 datoshi
    /// 0B : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("nullPropertyLE")]
    public abstract bool? NullPropertyLE(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELVA
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.ISNULL 	-> 2 datoshi
    /// 06 : OpCode.JMPIF 03 	-> 2 datoshi
    /// 08 : OpCode.SIZE 	-> 4 datoshi
    /// 09 : OpCode.PUSH0 	-> 1 datoshi
    /// 0A : OpCode.LT 	-> 8 datoshi
    /// 0B : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("nullPropertyLT")]
    public abstract bool? NullPropertyLT(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3BoStgkA0BFQA==
    /// 00 : OpCode.INITSLOT 0100 	-> 64 datoshi
    /// 03 : OpCode.PUSHNULL 	-> 1 datoshi
    /// 04 : OpCode.STLOC0 	-> 2 datoshi
    /// 05 : OpCode.LDLOC0 	-> 2 datoshi
    /// 06 : OpCode.DUP 	-> 2 datoshi
    /// 07 : OpCode.ISNULL 	-> 2 datoshi
    /// 08 : OpCode.JMPIF 03 	-> 2 datoshi
    /// 0A : OpCode.RET 	-> 0 datoshi
    /// 0B : OpCode.DROP 	-> 2 datoshi
    /// 0C : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("nullType")]
    public abstract void NullType();

    #endregion
}
