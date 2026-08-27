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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_DivisionOverflow"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""divideCheckedInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""divideUncheckedInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":39,""safe"":false},{""name"":""divideAssignUncheckedInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":78,""safe"":false},{""name"":""divideCheckedInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":119,""safe"":false},{""name"":""divideUncheckedInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":162,""safe"":false},{""name"":""divideAssignUncheckedInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":205,""safe"":false},{""name"":""divideCheckedBigInteger"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":250,""safe"":false},{""name"":""divideUncheckedBigInteger"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":257,""safe"":false},{""name"":""divideCheckedUInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":264,""safe"":false},{""name"":""divideCheckedUInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":271,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0WAVcBAkNweHlKDyocSwIAAACAKhRDaDIFRSL7DAhPdmVyZmxvdzqhQFcBAkNweHlKDyocSwIAAACAKhRDaDIFRSL7DAhPdmVyZmxvdzqhQFcBAkNweHlKDyocSwIAAACAKhRDaDIFRSL7DAhPdmVyZmxvdzqhgHhAVwECQ3B4eUoPKiBLAwAAAAAAAACAKhRDaDIFRSL7DAhPdmVyZmxvdzqhQFcBAkNweHlKDyogSwMAAAAAAAAAgCoUQ2gyBUUi+wwIT3ZlcmZsb3c6oUBXAQJDcHh5Sg8qIEsDAAAAAAAAAIAqFENoMgVFIvsMCE92ZXJmbG93OqGAeEBXAAJ4eaFAVwACeHmhQFcAAnh5oUBXAAJ4eaFAj4zVDA==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B4eUoPKhxLAgAAAIAqFENoMgVFIvsMCE92ZXJmbG93OqGAeEA=
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 1C [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwECQ3B4eUoPKiBLAwAAAAAAAACAKhRDaDIFRSL7DAhPdmVyZmxvdzqhgHhA
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 20 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwECQ3B4eUoPKhxLAgAAAIAqFENoMgVFIvsMCE92ZXJmbG93OqFA
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 1C [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwECQ3B4eUoPKiBLAwAAAAAAAACAKhRDaDIFRSL7DAhPdmVyZmxvdzqhQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 20 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwECQ3B4eUoPKhxLAgAAAIAqFENoMgVFIvsMCE92ZXJmbG93OqFA
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 1C [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwECQ3B4eUoPKiBLAwAAAAAAAACAKhRDaDIFRSL7DAhPdmVyZmxvdzqhQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 20 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideUncheckedInt64")]
    public abstract BigInteger? DivideUncheckedInt64(BigInteger? a, BigInteger? b);

    #endregion
}
