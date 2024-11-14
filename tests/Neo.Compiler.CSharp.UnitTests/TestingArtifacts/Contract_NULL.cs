using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_NULL(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_NULL"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""isNull"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""equalNullA"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":8,""safe"":false},{""name"":""equalNullB"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":15,""safe"":false},{""name"":""equalNotNullA"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":21,""safe"":false},{""name"":""equalNotNullB"",""parameters"":[{""name"":""value"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":28,""safe"":false},{""name"":""nullCoalescing"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""String"",""offset"":35,""safe"":false},{""name"":""nullCollation"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""String"",""offset"":49,""safe"":false},{""name"":""nullPropertyGT"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":68,""safe"":false},{""name"":""nullPropertyLT"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":80,""safe"":false},{""name"":""nullPropertyGE"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":92,""safe"":false},{""name"":""nullPropertyLE"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":104,""safe"":false},{""name"":""nullProperty"",""parameters"":[{""name"":""a"",""type"":""String""}],""returntype"":""Boolean"",""offset"":116,""safe"":false},{""name"":""ifNull"",""parameters"":[{""name"":""obj"",""type"":""Any""}],""returntype"":""Boolean"",""offset"":128,""safe"":false},{""name"":""nullCollationAndCollation"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""Any"",""offset"":138,""safe"":false},{""name"":""nullCollationAndCollation2"",""parameters"":[{""name"":""code"",""type"":""String""}],""returntype"":""Any"",""offset"":167,""safe"":false},{""name"":""nullType"",""parameters"":[],""returntype"":""Void"",""offset"":208,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAN1XAQF4cGjYQFcAAQt4l0BXAAF42EBXAAELeJhAVwABeNiqQFcBAXhK2CQFERKMcGhAVwEBeErYJgpFDAVsaW51eHBoQFcAAXhK2CQDyhC3QFcAAXhK2CQDyhC1QFcAAXhK2CQDyhC4QFcAAXhK2CQDyhC2QFcAAXhK2CQDyhCYQFcAAXgmBAhACUBXAQFBm/ZnznB4aEGSXegxStgmCkUMAXvbMNsoQFcBAUGb9mfOcAwDMTExeGhB5j8YhHhoQZJd6DFK2CYKRQwBe9sw2yhAVwEAC3BoStgkA0BFQLsXm8A="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABC3iYQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : NOTEQUAL [32 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("equalNotNullA")]
    public abstract bool? EqualNotNullA(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNiqQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : NOT [4 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("equalNotNullB")]
    public abstract bool? EqualNotNullB(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABC3iXQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : LDARG0 [2 datoshi]
    /// 05 : EQUAL [32 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("equalNullA")]
    public abstract bool? EqualNullA(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeNhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : ISNULL [2 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("equalNullB")]
    public abstract bool? EqualNullB(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCYECEAJQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : JMPIFNOT 04 [2 datoshi]
    /// 06 : PUSHT [1 datoshi]
    /// 07 : RET [0 datoshi]
    /// 08 : PUSHF [1 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("ifNull")]
    public abstract bool? IfNull(object? obj = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeHBo2EA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : ISNULL [2 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("isNull")]
    public abstract bool? IsNull(object? value = null);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErYJAUREoxwaEA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 05 [2 datoshi]
    /// 08 : PUSH1 [1 datoshi]
    /// 09 : PUSH2 [1 datoshi]
    /// 0A : SUBSTR [2048 datoshi]
    /// 0B : STLOC0 [2 datoshi]
    /// 0C : LDLOC0 [2 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("nullCoalescing")]
    public abstract string? NullCoalescing(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeErYJgpFDAVsaW51eHBoQA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIFNOT 0A [2 datoshi]
    /// 08 : DROP [2 datoshi]
    /// 09 : PUSHDATA1 6C696E7578 'linux' [8 datoshi]
    /// 10 : STLOC0 [2 datoshi]
    /// 11 : LDLOC0 [2 datoshi]
    /// 12 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("nullCollation")]
    public abstract string? NullCollation(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z85weGhBkl3oMUrYJgpFDAF72zDbKEA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDARG0 [2 datoshi]
    /// 0A : LDLOC0 [2 datoshi]
    /// 0B : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 10 : DUP [2 datoshi]
    /// 11 : ISNULL [2 datoshi]
    /// 12 : JMPIFNOT 0A [2 datoshi]
    /// 14 : DROP [2 datoshi]
    /// 15 : PUSHDATA1 7B '{' [8 datoshi]
    /// 18 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 1A : CONVERT 28 'ByteString' [8192 datoshi]
    /// 1C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("nullCollationAndCollation")]
    public abstract object? NullCollationAndCollation(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z85wDAMxMTF4aEHmPxiEeGhBkl3oMUrYJgpFDAF72zDbKEA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : PUSHDATA1 313131 '111' [8 datoshi]
    /// 0E : LDARG0 [2 datoshi]
    /// 0F : LDLOC0 [2 datoshi]
    /// 10 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 15 : LDARG0 [2 datoshi]
    /// 16 : LDLOC0 [2 datoshi]
    /// 17 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : ISNULL [2 datoshi]
    /// 1E : JMPIFNOT 0A [2 datoshi]
    /// 20 : DROP [2 datoshi]
    /// 21 : PUSHDATA1 7B '{' [8 datoshi]
    /// 24 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 26 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 28 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("nullCollationAndCollation2")]
    public abstract object? NullCollationAndCollation2(string? code);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKEJhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 03 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : NOTEQUAL [32 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("nullProperty")]
    public abstract bool? NullProperty(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELhA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 03 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : GE [8 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("nullPropertyGE")]
    public abstract bool? NullPropertyGE(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELdA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 03 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : GT [8 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("nullPropertyGT")]
    public abstract bool? NullPropertyGT(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELZA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 03 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : LE [8 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("nullPropertyLE")]
    public abstract bool? NullPropertyLE(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJAPKELVA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : ISNULL [2 datoshi]
    /// 06 : JMPIF 03 [2 datoshi]
    /// 08 : SIZE [4 datoshi]
    /// 09 : PUSH0 [1 datoshi]
    /// 0A : LT [8 datoshi]
    /// 0B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("nullPropertyLT")]
    public abstract bool? NullPropertyLT(string? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEAC3BoStgkA0BFQA==
    /// 00 : INITSLOT 0100 [64 datoshi]
    /// 03 : PUSHNULL [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : LDLOC0 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : ISNULL [2 datoshi]
    /// 08 : JMPIF 03 [2 datoshi]
    /// 0A : RET [0 datoshi]
    /// 0B : DROP [2 datoshi]
    /// 0C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("nullType")]
    public abstract void NullType();

    #endregion
}
