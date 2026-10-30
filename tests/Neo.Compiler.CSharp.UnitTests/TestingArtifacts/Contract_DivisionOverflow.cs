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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_DivisionOverflow"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""divideCheckedInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""divideUncheckedInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":67,""safe"":false},{""name"":""divideAssignUncheckedInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":134,""safe"":false},{""name"":""divideCheckedInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":203,""safe"":false},{""name"":""divideUncheckedInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":274,""safe"":false},{""name"":""divideAssignUncheckedInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":345,""safe"":false},{""name"":""divideCheckedBigInteger"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":418,""safe"":false},{""name"":""divideUncheckedBigInteger"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":455,""safe"":false},{""name"":""divideCheckedUInt32"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":492,""safe"":false},{""name"":""divideCheckedUInt64"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":529,""safe"":false},{""name"":""intDivRemCaught"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":566,""safe"":false},{""name"":""mathDivRemCaught"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":753,""safe"":false},{""name"":""bigIntegerDivRemCaught"",""parameters"":[{""name"":""left"",""type"":""Integer""},{""name"":""right"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":940,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP3/A1cBAkNweHlKECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86Sg8qHEsCAAAAgCoUQ2gyBUUi+wwIT3ZlcmZsb3c6oUBXAQJDcHh5ShAqGkNoMgVFIvsMDkRpdmlkZSBieSB6ZXJvOkoPKhxLAgAAAIAqFENoMgVFIvsMCE92ZXJmbG93OqFAVwECQ3B4eUoQKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzpKDyocSwIAAACAKhRDaDIFRSL7DAhPdmVyZmxvdzqhgHhAVwECQ3B4eUoQKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzpKDyogSwMAAAAAAAAAgCoUQ2gyBUUi+wwIT3ZlcmZsb3c6oUBXAQJDcHh5ShAqGkNoMgVFIvsMDkRpdmlkZSBieSB6ZXJvOkoPKiBLAwAAAAAAAACAKhRDaDIFRSL7DAhPdmVyZmxvdzqhQFcBAkNweHlKECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86Sg8qIEsDAAAAAAAAAIAqFENoMgVFIvsMCE92ZXJmbG93OqGAeEBXAQJDcHh5ShAqGkNoMgVFIvsMDkRpdmlkZSBieSB6ZXJvOqFAVwECQ3B4eUoQKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzqhQFcBAkNweHlKECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86oUBXAQJDcHh5ShAqGkNoMgVFIvsMDkRpdmlkZSBieSB6ZXJvOqFAVwICQ3A8qAAAAAAAAAB5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86SgIAAACAKhhLDyoUQ2gyBUUi+wwIT3ZlcmZsb3c6ShJNoVOiUBLAEE0RzhFNEM4Sv0bBRXBxaABkoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2meSsoUMisD/////wAAAACRSgL///9/MhkDAAAAAAEAAACfPQ9wQ2gyBUUi+w89BD0CQFcCAkNwPKgAAAAAAAAAeXgRTRAqGkNoMgVFIvsMDkRpdmlkZSBieSB6ZXJvOkoCAAAAgCoYSw8qFENoMgVFIvsMCE92ZXJmbG93OkoSTaFTolASwBBNEc4RTRDOEr9GwUVwcWgAZKBKyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9pnkrKFDIrA/////8AAAAAkUoC////fzIZAwAAAAABAAAAnz0PcENoMgVFIvsPPQQ9AkBXAgJDcDtCAHl4EU0QKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzpKEk2hU6JQEsAQTRHOEU0QzhK/RsFFcHFoAGSgaZ49DXBDaDIFRSL7Dz0CQJgOYkg=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICQ3A7QgB5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86ShJNoVOiUBLAEE0RzhFNEM4Sv0bBRXBxaABkoGmePQ1wQ2gyBUUi+w89AkA=
    /// INITSLOT 0202 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY 4200 [4 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// NIP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// MUL [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// ADD [8 datoshi]
    /// ENDTRY 0D [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("bigIntegerDivRemCaught")]
    public abstract BigInteger? BigIntegerDivRemCaught(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B4eUoQKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzpKDyocSwIAAACAKhRDaDIFRSL7DAhPdmVyZmxvdzqhgHhA
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
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
    /// Script: VwECQ3B4eUoQKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzpKDyogSwMAAAAAAAAAgCoUQ2gyBUUi+wwIT3ZlcmZsb3c6oYB4QA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
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
    /// Script: VwECQ3B4eUoQKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzqhQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideCheckedBigInteger")]
    public abstract BigInteger? DivideCheckedBigInteger(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B4eUoQKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzpKDyocSwIAAACAKhRDaDIFRSL7DAhPdmVyZmxvdzqhQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
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
    /// Script: VwECQ3B4eUoQKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzpKDyogSwMAAAAAAAAAgCoUQ2gyBUUi+wwIT3ZlcmZsb3c6oUA=
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
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
    /// Script: VwECQ3B4eUoQKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzqhQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideCheckedUInt32")]
    public abstract BigInteger? DivideCheckedUInt32(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B4eUoQKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzqhQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideCheckedUInt64")]
    public abstract BigInteger? DivideCheckedUInt64(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B4eUoQKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzqhQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("divideUncheckedBigInteger")]
    public abstract BigInteger? DivideUncheckedBigInteger(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQ3B4eUoQKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzpKDyocSwIAAACAKhRDaDIFRSL7DAhPdmVyZmxvdzqhQA==
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
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
    /// Script: VwECQ3B4eUoQKhpDaDIFRSL7DA5EaXZpZGUgYnkgemVybzpKDyogSwMAAAAAAAAAgCoUQ2gyBUUi+wwIT3ZlcmZsb3c6oUA=
    /// INITSLOT 0102 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
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

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICQ3A8qAAAAAAAAAB5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86SgIAAACAKhhLDyoUQ2gyBUUi+wwIT3ZlcmZsb3c6ShJNoVOiUBLAEE0RzhFNEM4Sv0bBRXBxaABkoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2meSsoUMisD/////wAAAACRSgL///9/MhkDAAAAAAEAAACfPQ9wQ2gyBUUi+w89BD0CQA==
    /// INITSLOT 0202 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY_L A800000000000000 [4 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 18 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// NIP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 2B [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 19 [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// ENDTRY 0F [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// ENDTRY 04 [4 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("intDivRemCaught")]
    public abstract BigInteger? IntDivRemCaught(BigInteger? left, BigInteger? right);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICQ3A8qAAAAAAAAAB5eBFNECoaQ2gyBUUi+wwORGl2aWRlIGJ5IHplcm86SgIAAACAKhhLDyoUQ2gyBUUi+wwIT3ZlcmZsb3c6ShJNoVOiUBLAEE0RzhFNEM4Sv0bBRXBxaABkoErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn2meSsoUMisD/////wAAAACRSgL///9/MhkDAAAAAAEAAACfPQ9wQ2gyBUUi+w89BD0CQA==
    /// INITSLOT 0202 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// TRY_L A800000000000000 [4 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPNE 1A [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 446976696465206279207A65726F [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 18 [2 datoshi]
    /// OVER [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMPNE 14 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 4F766572666C6F77 'Overflow' [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PICK [2 datoshi]
    /// DIV [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// MOD [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH1 [1 datoshi]
    /// PICK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// NIP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 64 [1 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 1E [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 2B [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 19 [2 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// SUB [8 datoshi]
    /// ENDTRY 0F [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// ENDTRY 04 [4 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("mathDivRemCaught")]
    public abstract BigInteger? MathDivRemCaught(BigInteger? left, BigInteger? right);

    #endregion
}
