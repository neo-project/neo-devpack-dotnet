using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Char(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Char"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testCharIsDigit"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testCharIsLetter"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":25,""safe"":false},{""name"":""testCharIsWhiteSpace"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":53,""safe"":false},{""name"":""testCharIsLetterOrDigit"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":82,""safe"":false},{""name"":""testCharIsLetterOrDigitResult"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":122,""safe"":false},{""name"":""testCharIsAscii"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":166,""safe"":false},{""name"":""testCharIsAsciiLetter"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":191,""safe"":false},{""name"":""testCharIsLower"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":219,""safe"":false},{""name"":""testCharToLower"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":244,""safe"":false},{""name"":""testCharIsUpper"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":288,""safe"":false},{""name"":""testCharToUpper"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":313,""safe"":false},{""name"":""testCharToUpperInvariant"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":357,""safe"":false},{""name"":""testCharToLowerInvariant"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":401,""safe"":false},{""name"":""testCharGetNumericValue"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":445,""safe"":false},{""name"":""testCharIsPunctuation"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":486,""safe"":false},{""name"":""testCharIsSymbol"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":536,""safe"":false},{""name"":""testCharIsControl"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":586,""safe"":false},{""name"":""testCharIsSurrogate"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":619,""safe"":false},{""name"":""testCharIsHighSurrogate"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":650,""safe"":false},{""name"":""testCharIsLowSurrogate"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":681,""safe"":false},{""name"":""testCharIsBetween"",""parameters"":[{""name"":""c"",""type"":""Integer""},{""name"":""lower"",""type"":""Integer""},{""name"":""upper"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":712,""safe"":false},{""name"":""testCharParse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Integer"",""offset"":770,""safe"":false},{""name"":""testCharTryParse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Array"",""offset"":821,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":860,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1fA1cAAXhKEDAKSgL//wAAMgM6RXgAMAA6u0BXAAF4ShAwCkoC//8AADIDOkV4ACCSAGEAe7tAVwABeEoQMApKAv//AAAyAzpFeEoZHrtQACCzrEBXAAF4ShAwCkoC//8AADIDOkV4SgAwADq7SiQMRUoAIJIAYQB7u0ZAVwEBeEoQMApKAv//AAAyAzpFeEoAMAA6u0okDEVKACCSAGEAe7tGcGjZIEBXAAF4ShAwCkoC//8AADIDOkV4EAGAALtAVwABeEoQMApKAv//AAAyAzpFeAAgkgBhAHu7QFcAAXhKEDAKSgL//wAAMgM6RXgAYQB7u0BXAAF4ShAwCkoC//8AADIDOkV4SgBBAFu7JgUAIJ5KEDAKSgL//wAAMgM6QFcAAXhKEDAKSgL//wAAMgM6RXgAQQBbu0BXAAF4ShAwCkoC//8AADIDOkV4SgBhAHu7JgUAIJ9KEDAKSgL//wAAMgM6QFcAAXhKEDAKSgL//wAAMgM6RXhKAGEAe7smBQAgn0oQMApKAv//AAAyAzpAVwABeEoQMApKAv//AAAyAzpFeEoAQQBbuyYFACCeShAwCkoC//8AADIDOkBXAAF4ShAwCkoC//8AADIDOkV4SgAwADq7JAZFDyIFADCfSsoUMgM6QFcAAXhKEDAKSgL//wAAMgM6RXhKAH8sGQQAAAAA7vcAjAEAALgAAAAoEVGokbFARQlAVwABeEoQMApKAv//AAAyAzpFeEoAfywZBAAAAAAQCABwAAAAQAEAAFARUaiRsUBFCUBXAAF4ShAwCkoC//8AADIDOkV4ShAAILtQAH8BoAC7rEBXAAF4ShAwCkoC//8AADIDOkV4AgDYAAACAOAAALtAVwABeEoQMApKAv//AAAyAzpFeAIA2AAAAgDcAAC7QFcAAXhKEDAKSgL//wAAMgM6RXgCANwAAAIA4AAAu0BXAAN4ShAwCkoC//8AADIDOkV5ShAwCkoC//8AADIDOkV6ShAwCkoC//8AADIDOkV4eXpTTrZTtqtAVwABeErYJgpFDAROdWxsOkrKESgQRQwKTm90T25lQ2hhcjoQzkoQMApKAv//AAAyAzpAVwIBeHEQSmBpUEVK2CQNSsoRKggQzmEIIgZFEGEJWWBwaFhQEr9AVgJA2+xaAA==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeEoAMAA6uyQGRQ8iBQAwn0rKFDIDOkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 30 [1 datoshi]
    /// PUSHINT8 3A [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// JMP 05 [2 datoshi]
    /// PUSHINT8 30 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharGetNumericValue")]
    public abstract BigInteger? TestCharGetNumericValue(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeBABgAC7QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT16 8000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsAscii")]
    public abstract bool? TestCharIsAscii(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeAAgkgBhAHu7QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsAsciiLetter")]
    public abstract bool? TestCharIsAsciiLetter(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADeEoQMApKAv//AAAyAzpFeUoQMApKAv//AAAyAzpFekoQMApKAv//AAAyAzpFeHl6U062U7arQA==
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// TUCK [2 datoshi]
    /// LE [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// LE [8 datoshi]
    /// BOOLAND [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsBetween")]
    public abstract bool? TestCharIsBetween(BigInteger? c, BigInteger? lower, BigInteger? upper);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeEoQACC7UAB/AaAAu6xA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// PUSHINT16 A000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// BOOLOR [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsControl")]
    public abstract bool? TestCharIsControl(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeAAwADq7QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 30 [1 datoshi]
    /// PUSHINT8 3A [1 datoshi]
    /// WITHIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsDigit")]
    public abstract bool? TestCharIsDigit(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeAIA2AAAAgDcAAC7QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT32 00D80000 [1 datoshi]
    /// PUSHINT32 00DC0000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsHighSurrogate")]
    public abstract bool? TestCharIsHighSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeAAgkgBhAHu7QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsLetter")]
    public abstract bool? TestCharIsLetter(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeEoAMAA6u0okDEVKACCSAGEAe7tGQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 30 [1 datoshi]
    /// PUSHINT8 3A [1 datoshi]
    /// WITHIN [8 datoshi]
    /// DUP [2 datoshi]
    /// JMPIF 0C [2 datoshi]
    /// DROP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// NIP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsLetterOrDigit")]
    public abstract bool? TestCharIsLetterOrDigit(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeEoQMApKAv//AAAyAzpFeEoAMAA6u0okDEVKACCSAGEAe7tGcGjZIEA=
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 30 [1 datoshi]
    /// PUSHINT8 3A [1 datoshi]
    /// WITHIN [8 datoshi]
    /// DUP [2 datoshi]
    /// JMPIF 0C [2 datoshi]
    /// DROP [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// OR [8 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// NIP [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ISTYPE 20 'Boolean' [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsLetterOrDigitResult")]
    public abstract bool? TestCharIsLetterOrDigitResult(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeAIA3AAAAgDgAAC7QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT32 00DC0000 [1 datoshi]
    /// PUSHINT32 00E00000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsLowSurrogate")]
    public abstract bool? TestCharIsLowSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeABhAHu7QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsLower")]
    public abstract bool? TestCharIsLower(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeEoAfywZBAAAAADu9wCMAQAAuAAAACgRUaiRsUBFCUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// JMPGT 19 [2 datoshi]
    /// PUSHINT128 00000000EEF7008C010000B800000028 [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SHL [8 datoshi]
    /// AND [8 datoshi]
    /// NZ [4 datoshi]
    /// RET [0 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsPunctuation")]
    public abstract bool? TestCharIsPunctuation(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeAIA2AAAAgDgAAC7QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT32 00D80000 [1 datoshi]
    /// PUSHINT32 00E00000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsSurrogate")]
    public abstract bool? TestCharIsSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeEoAfywZBAAAAAAQCABwAAAAQAEAAFARUaiRsUBFCUA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// JMPGT 19 [2 datoshi]
    /// PUSHINT128 00000000100800700000004001000050 [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// ROT [2 datoshi]
    /// SHL [8 datoshi]
    /// AND [8 datoshi]
    /// NZ [4 datoshi]
    /// RET [0 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsSymbol")]
    public abstract bool? TestCharIsSymbol(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeABBAFu7QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// PUSHINT8 5B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsUpper")]
    public abstract bool? TestCharIsUpper(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeEoZHrtQACCzrEA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH9 [1 datoshi]
    /// PUSH14 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// BOOLOR [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsWhiteSpace")]
    public abstract bool? TestCharIsWhiteSpace(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErYJgpFDAROdWxsOkrKESgQRQwKTm90T25lQ2hhcjoQzkoQMApKAv//AAAyAzpA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 0A [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 4E756C6C 'Null' [8 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPEQ 10 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 4E6F744F6E6543686172 'NotOneChar' [8 datoshi]
    /// THROW [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharParse")]
    public abstract BigInteger? TestCharParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeEoAQQBbuyYFACCeShAwCkoC//8AADIDOkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// PUSHINT8 5B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharToLower")]
    public abstract BigInteger? TestCharToLower(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeEoAQQBbuyYFACCeShAwCkoC//8AADIDOkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// PUSHINT8 5B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// ADD [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharToLowerInvariant")]
    public abstract BigInteger? TestCharToLowerInvariant(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeEoAYQB7uyYFACCfShAwCkoC//8AADIDOkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharToUpper")]
    public abstract BigInteger? TestCharToUpper(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQMApKAv//AAAyAzpFeEoAYQB7uyYFACCfShAwCkoC//8AADIDOkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// SUB [8 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// JMPLT 0A [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 FFFF0000 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharToUpperInvariant")]
    public abstract BigInteger? TestCharToUpperInvariant(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBeHEQSmBpUEVK2CQNSsoRKggQzmEIIgZFEGEJWWBwaFhQEr9A
    /// INITSLOT 0201 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0D [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPNE 08 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// STSFLD1 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD1 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// STSFLD0 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharTryParse")]
    public abstract IList<object>? TestCharTryParse(string? value);

    #endregion
}
