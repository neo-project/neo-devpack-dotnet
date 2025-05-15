using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Overflow(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Overflow"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""addInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""mulInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":53,""safe"":false},{""name"":""addUInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":106,""safe"":false},{""name"":""mulUInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":141,""safe"":false},{""name"":""divInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":176,""safe"":false},{""name"":""divShort"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":196,""safe"":false},{""name"":""negateIntChecked"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":216,""safe"":false},{""name"":""negateInt"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":231,""safe"":false},{""name"":""negateLongChecked"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":246,""safe"":false},{""name"":""negateLong"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":265,""safe"":false},{""name"":""negateShortChecked"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":284,""safe"":false},{""name"":""negateShort"",""parameters"":[{""name"":""a"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":290,""safe"":false},{""name"":""negateAddInt"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":296,""safe"":false},{""name"":""negateAddIntChecked"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":359,""safe"":false},{""name"":""negateAddLong"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":394,""safe"":false},{""name"":""negateAddLongChecked"",""parameters"":[{""name"":""a"",""type"":""Integer""},{""name"":""b"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":489,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.8.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0YAlcAAnh5nkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9AVwACeHmgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0BXAAJ4eZ5KEC4EIg5KA/////8AAAAAMgwD/////wAAAACRQFcAAnh5oEoQLgQiDkoD/////wAAAAAyDAP/////AAAAAJFAVwACeHlKDyoLSwIAAACAKgM6oUBXAAJ4eUoPKgtLAgAAAIAqAzqhQFcAAXhKAgAAAIAqAzqbQFcAAXhKAgAAAIAqA0CbQFcAAXhKAwAAAAAAAACAKgM6m0BXAAF4SgMAAAAAAAAAgCoDQJtAVwABeJtAVwABeJtAVwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oCAAAAgCoDQJtAVwACeHmeSgIAAACALgM6SgL///9/MgM6SgIAAACAKgM6m0BXAAJ4eZ5KAwAAAAAAAACALgQiDkoD/////////38yMgT//////////wAAAAAAAAAAkUoD/////////38yFAQAAAAAAAAAAAEAAAAAAAAAn0oDAAAAAAAAAIAqA0CbQFcAAnh5nkoDAAAAAAAAAIAuAzpKA/////////9/MgM6SgMAAAAAAAAAgCoDOptA4RUGTQ==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
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
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("addInt")]
    public abstract BigInteger? AddInt(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0E [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("addUInt")]
    public abstract BigInteger? AddUInt(BigInteger? a, BigInteger? b);

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
    [DisplayName("divInt")]
    public abstract BigInteger? DivInt(BigInteger? a, BigInteger? b);

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
    [DisplayName("divShort")]
    public abstract BigInteger? DivShort(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmgSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// MUL [8 datoshi]
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
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("mulInt")]
    public abstract BigInteger? MulInt(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmgShAuBCIOSgP/////AAAAADIMA/////8AAAAAkUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// MUL [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPGE 04 [2 datoshi]
    /// JMP 0E [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// JMPLE 0C [2 datoshi]
    /// PUSHINT64 FFFFFFFF00000000 [1 datoshi]
    /// AND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("mulUInt")]
    public abstract BigInteger? MulUInt(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oCAAAAgCoDQJtA
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
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
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// RET [0 datoshi]
    /// NEGATE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("negateAddInt")]
    public abstract BigInteger? NegateAddInt(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgIAAACALgM6SgL///9/MgM6SgIAAACAKgM6m0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFFFF7F [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// NEGATE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("negateAddIntChecked")]
    public abstract BigInteger? NegateAddIntChecked(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgMAAAAAAAAAgC4EIg5KA/////////9/MjIE//////////8AAAAAAAAAAJFKA/////////9/MhQEAAAAAAAAAAABAAAAAAAAAJ9KAwAAAAAAAACAKgNAm0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
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
    /// DUP [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// RET [0 datoshi]
    /// NEGATE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("negateAddLong")]
    public abstract BigInteger? NegateAddLong(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeSgMAAAAAAAAAgC4DOkoD/////////38yAzpKAwAAAAAAAACAKgM6m0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPGE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 FFFFFFFFFFFFFF7F [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// NEGATE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("negateAddLongChecked")]
    public abstract BigInteger? NegateAddLongChecked(BigInteger? a, BigInteger? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoCAAAAgCoDQJtA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// RET [0 datoshi]
    /// NEGATE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("negateInt")]
    public abstract BigInteger? NegateInt(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoCAAAAgCoDOptA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// NEGATE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("negateIntChecked")]
    public abstract BigInteger? NegateIntChecked(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoDAAAAAAAAAIAqA0CbQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// RET [0 datoshi]
    /// NEGATE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("negateLong")]
    public abstract BigInteger? NegateLong(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoDAAAAAAAAAIAqAzqbQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// JMPNE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// NEGATE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("negateLongChecked")]
    public abstract BigInteger? NegateLongChecked(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJtA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("negateShort")]
    public abstract BigInteger? NegateShort(BigInteger? a);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJtA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("negateShortChecked")]
    public abstract BigInteger? NegateShortChecked(BigInteger? a);

    #endregion
}
