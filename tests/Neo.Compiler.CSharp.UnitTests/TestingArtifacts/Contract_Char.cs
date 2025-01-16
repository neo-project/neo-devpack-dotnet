using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Char(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Char"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testCharIsDigit"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testCharIsLetter"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":10,""safe"":false},{""name"":""testCharIsWhiteSpace"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":28,""safe"":false},{""name"":""testCharIsLetterOrDigit"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":42,""safe"":false},{""name"":""testCharIsLower"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":68,""safe"":false},{""name"":""testCharToLower"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":78,""safe"":false},{""name"":""testCharIsUpper"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":97,""safe"":false},{""name"":""testCharToUpper"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":107,""safe"":false},{""name"":""testCharGetNumericValue"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":126,""safe"":false},{""name"":""testCharIsPunctuation"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":145,""safe"":false},{""name"":""testCharIsSymbol"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":179,""safe"":false},{""name"":""testCharIsControl"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":221,""safe"":false},{""name"":""testCharIsSurrogate"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":239,""safe"":false},{""name"":""testCharIsHighSurrogate"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":269,""safe"":false},{""name"":""testCharIsLowSurrogate"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":285,""safe"":false},{""name"":""testCharIsBetween"",""parameters"":[{""name"":""c"",""type"":""Integer""},{""name"":""lower"",""type"":""Integer""},{""name"":""upper"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":301,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1AAVcAAXgAMAA6u0BXAAF4SgBBAFu7UABhAHu7rEBXAAF4Shkeu1AAILOsQFcAAXhKADAAOrskD0oAQQBbuyQHAGEAe7tAVwABeABhAHu7QFcAAXhKAEEAW7smCABBnwBhnkBXAAF4AEEAW7tAVwABeEoAYQB7uyYIAGGfAEGeQFcAAXhKADAAOrskBUUPQAAwn0BXAAF4SgAhADC7JBdKADoAQbskD0oAWwBhuyQHAHsAf7tAVwABeEoAJAAsuyQfSgA8AD67JBdKAD4AQbskD0oAWwBhuyQHAHsAf7tAVwABeEoQACC7UAB/AaAAu6xAVwABeEoCANgAAAIA3AAAu1ACANwAAAIA4AAAu6xAVwABeAIA2AAAAgDcAAC7QFcAAXgCANwAAAIA4AAAu0BXAAN6eXhKUbhKJgZTRUVARbVA/cUnXQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAMAA6uyQFRQ9AADCfQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 30 [1 datoshi]
    /// PUSHINT8 3A [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHM1 [1 datoshi]
    /// RET [0 datoshi]
    /// PUSHINT8 30 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharGetNumericValue")]
    public abstract BigInteger? TestCharGetNumericValue(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4SlG4SiYGU0VFQEW1QA==
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ROT [2 datoshi]
    /// GE [8 datoshi]
    /// DUP [2 datoshi]
    /// JMPIFNOT 06 [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// DROP [2 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// DROP [2 datoshi]
    /// LT [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsBetween")]
    public abstract bool? TestCharIsBetween(BigInteger? c, BigInteger? lower, BigInteger? upper);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQACC7UAB/AaAAu6xA
    /// INITSLOT 0001 [64 datoshi]
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
    /// Script: VwABeAAwADq7QA==
    /// INITSLOT 0001 [64 datoshi]
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
    /// Script: VwABeAIA2AAAAgDcAAC7QA==
    /// INITSLOT 0001 [64 datoshi]
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
    /// Script: VwABeEoAQQBbu1AAYQB7u6xA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// PUSHINT8 5B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// BOOLOR [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsLetter")]
    public abstract bool? TestCharIsLetter(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAMAA6uyQPSgBBAFu7JAcAYQB7u0A=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 30 [1 datoshi]
    /// PUSHINT8 3A [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 0F [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// PUSHINT8 5B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 07 [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsLetterOrDigit")]
    public abstract bool? TestCharIsLetterOrDigit(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeABhAHu7QA==
    /// INITSLOT 0001 [64 datoshi]
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
    /// Script: VwABeAIA3AAAAgDgAAC7QA==
    /// INITSLOT 0001 [64 datoshi]
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
    /// Script: VwABeEoAIQAwuyQXSgA6AEG7JA9KAFsAYbskBwB7AH+7QA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 21 [1 datoshi]
    /// PUSHINT8 30 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 17 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 3A [1 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 0F [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 5B [1 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 07 [2 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// WITHIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsPunctuation")]
    public abstract bool? TestCharIsPunctuation(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoCANgAAAIA3AAAu1ACANwAAAIA4AAAu6xA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00D80000 [1 datoshi]
    /// PUSHINT32 00DC0000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSHINT32 00DC0000 [1 datoshi]
    /// PUSHINT32 00E00000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// BOOLOR [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsSurrogate")]
    public abstract bool? TestCharIsSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAJAAsuyQfSgA8AD67JBdKAD4AQbskD0oAWwBhuyQHAHsAf7tA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 24 [1 datoshi]
    /// PUSHINT8 2C [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 1F [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 3C [1 datoshi]
    /// PUSHINT8 3E [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 17 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 3E [1 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 0F [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 5B [1 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 07 [2 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// WITHIN [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsSymbol")]
    public abstract bool? TestCharIsSymbol(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeABBAFu7QA==
    /// INITSLOT 0001 [64 datoshi]
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
    /// Script: VwABeEoZHrtQACCzrEA=
    /// INITSLOT 0001 [64 datoshi]
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
    /// Script: VwABeEoAQQBbuyYIAEGfAGGeQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// PUSHINT8 5B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// SUB [8 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// ADD [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharToLower")]
    public abstract BigInteger? TestCharToLower(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAYQB7uyYIAGGfAEGeQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// SUB [8 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// ADD [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharToUpper")]
    public abstract BigInteger? TestCharToUpper(BigInteger? c);

    #endregion
}
