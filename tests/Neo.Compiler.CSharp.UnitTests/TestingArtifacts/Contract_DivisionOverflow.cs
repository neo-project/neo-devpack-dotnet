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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_DivisionOverflow"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""divideCheckedInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""divideUncheckedInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":32,""safe"":false},{""name"":""divideAssignUncheckedInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":64,""safe"":false},{""name"":""divideCheckedInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":98,""safe"":false},{""name"":""divideUncheckedInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":134,""safe"":false},{""name"":""divideAssignUncheckedInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":170,""safe"":false},{""name"":""divideCheckedBigInteger"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":208,""safe"":false},{""name"":""divideUncheckedBigInteger"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":215,""safe"":false},{""name"":""divideCheckedUInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":222,""safe"":false},{""name"":""divideCheckedUInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":229,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOxXAAJ4eUoPKhdLAgAAAIAqD0VFDAhPdmVyZmxvdzqhQFcAAnh5Sg8qF0sCAAAAgCoPRUUMCE92ZXJmbG93OqFAVwACeHlKDyoXSwIAAACAKg9FRQwIT3ZlcmZsb3c6oYB4QFcAAnh5Sg8qG0sDAAAAAAAAAIAqD0VFDAhPdmVyZmxvdzqhQFcAAnh5Sg8qG0sDAAAAAAAAAIAqD0VFDAhPdmVyZmxvdzqhQFcAAnh5Sg8qG0sDAAAAAAAAAIAqD0VFDAhPdmVyZmxvdzqhgHhAVwACeHmhQFcAAnh5oUBXAAJ4eaFAVwACeHmhQDgtWgk=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlKDyoXSwIAAACAKg9FRQwIT3ZlcmZsb3c6oYB4QA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 17 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 0F [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
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
    /// Script: VwACeHlKDyobSwMAAAAAAAAAgCoPRUUMCE92ZXJmbG93OqGAeEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 1B [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 0F [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
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
    /// Script: VwACeHlKDyoXSwIAAACAKg9FRQwIT3ZlcmZsb3c6oUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 17 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 0F [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
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
    /// Script: VwACeHlKDyobSwMAAAAAAAAAgCoPRUUMCE92ZXJmbG93OqFA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 1B [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 0F [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
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
    /// Script: VwACeHlKDyoXSwIAAACAKg9FRQwIT3ZlcmZsb3c6oUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 17 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 0F [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
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
    /// Script: VwACeHlKDyobSwMAAAAAAAAAgCoPRUUMCE92ZXJmbG93OqFA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 1B [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 0F [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideUncheckedInt64")]
    public abstract BigInteger? DivideUncheckedInt64(BigInteger? a, BigInteger? b);

    #endregion
}
