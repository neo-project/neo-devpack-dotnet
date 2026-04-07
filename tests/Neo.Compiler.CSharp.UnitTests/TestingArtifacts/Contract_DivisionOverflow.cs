using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_DivisionOverflow(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_DivisionOverflow"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""divideCheckedInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""divideUncheckedInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":20,""safe"":false},{""name"":""divideAssignUncheckedInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":40,""safe"":false},{""name"":""divideCheckedInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":108,""safe"":false},{""name"":""divideUncheckedInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":132,""safe"":false},{""name"":""divideAssignUncheckedInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":156,""safe"":false},{""name"":""divideCheckedBigInteger"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":256,""safe"":false},{""name"":""divideUncheckedBigInteger"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":263,""safe"":false},{""name"":""divideCheckedUInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":270,""safe"":false},{""name"":""divideCheckedUInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":277,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.9.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0cAVcAAnh5Sg8qC0sCAAAAgCoDOqFAVwACeHlKDyoLSwIAAACAKgM6oUBXAAJ4eUoPKgtLAgAAAIAqAzqhSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4B4QFcAAnh5Sg8qD0sDAAAAAAAAAIAqAzqhQFcAAnh5Sg8qD0sDAAAAAAAAAIAqAzqhQFcAAnh5Sg8qD0sDAAAAAAAAAIAqAzqhSgMAAAAAAAAAgC4EIg5KA/////////9/MjIE//////////8AAAAAAAAAAJFKA/////////9/MhQEAAAAAAAAAAABAAAAAAAAAJ+AeEBXAAJ4eaFAVwACeHmhQFcAAnh5oUBXAAJ4eaFAr1Q+hA==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlKDyoLSwIAAACAKgM6oUoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+AeEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 0B [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideAssignUncheckedInt32")]
    public abstract BigInteger? DivideAssignUncheckedInt32(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlKDyoPSwMAAAAAAAAAgCoDOqFKAwAAAAAAAACALgQiDkoD/////////38yMgT//////////wAAAAAAAAAAkUoD/////////38yFAQAAAAAAAAAAAEAAAAAAAAAn4B4QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 0F [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0E [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// JMPLE 32 [2 datoshi]
    /// PUSHINT128 FFFFFFFFFFFFFFFF0000000000000000 [4 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// JMPLE 14 [2 datoshi]
    /// PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// SUB [8 datoshi]
    /// STARG0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideAssignUncheckedInt64")]
    public abstract BigInteger? DivideAssignUncheckedInt64(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmhQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideCheckedBigInteger")]
    public abstract BigInteger? DivideCheckedBigInteger(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlKDyoLSwIAAACAKgM6oUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 0B [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideCheckedInt32")]
    public abstract BigInteger? DivideCheckedInt32(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlKDyoPSwMAAAAAAAAAgCoDOqFA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 0F [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideCheckedInt64")]
    public abstract BigInteger? DivideCheckedInt64(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmhQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideCheckedUInt32")]
    public abstract BigInteger? DivideCheckedUInt32(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmhQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideCheckedUInt64")]
    public abstract BigInteger? DivideCheckedUInt64(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmhQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideUncheckedBigInteger")]
    public abstract BigInteger? DivideUncheckedBigInteger(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlKDyoLSwIAAACAKgM6oUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 0B [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideUncheckedInt32")]
    public abstract BigInteger? DivideUncheckedInt32(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlKDyoPSwMAAAAAAAAAgCoDOqFA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 0F [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideUncheckedInt64")]
    public abstract BigInteger? DivideUncheckedInt64(BigInteger? a, BigInteger? b);

    #endregion
}
