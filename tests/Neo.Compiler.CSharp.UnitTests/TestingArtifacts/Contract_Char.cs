using Neo.Cryptography.ECC;
using Neo.Extensions;
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
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1AAVcAAXgAMAA6u0BXAAF4SgBBAFu7UABhAHu7rEBXAAF4Shkeu1AAILOsQFcAAXhKADAAOrskD0oAQQBbuyQHAGEAe7tAVwABeABhAHu7QFcAAXhKAEEAW7smCABBnwBhnkBXAAF4AEEAW7tAVwABeEoAYQB7uyYIAGGfAEGeQFcAAXhKADAAOrskBUUPQAAwn0BXAAF4SgAhADC7JBdKADoAQbskD0oAWwBhuyQHAHsAf7tAVwABeEoAJAAsuyQfSgA8AD67JBdKAD4AQbskD0oAWwBhuyQHAHsAf7tAVwABeEoQACC7UAB/AaAAu6xAVwABeEoCANgAAAIA3AAAu1ACANwAAAIA4AAAu6xAVwABeAIA2AAAAgDcAAC7QFcAAXgCANwAAAIA4AAAu0BXAAN6eXhKUbhKJgZTRUVARbVA/cUnXQ==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAMAA6uyQFRQ9AADCfQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSHINT8 30 [1 datoshi]
    /// 07 : PUSHINT8 3A [1 datoshi]
    /// 09 : WITHIN [8 datoshi]
    /// 0A : JMPIF 05 [2 datoshi]
    /// 0C : DROP [2 datoshi]
    /// 0D : PUSHM1 [1 datoshi]
    /// 0E : RET [0 datoshi]
    /// 0F : PUSHINT8 30 [1 datoshi]
    /// 11 : SUB [8 datoshi]
    /// 12 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharGetNumericValue")]
    public abstract BigInteger? TestCharGetNumericValue(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4SlG4SiYGU0VFQEW1QA==
    /// 00 : INITSLOT 0003 [64 datoshi]
    /// 03 : LDARG2 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : LDARG0 [2 datoshi]
    /// 06 : DUP [2 datoshi]
    /// 07 : ROT [2 datoshi]
    /// 08 : GE [8 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : JMPIFNOT 06 [2 datoshi]
    /// 0C : REVERSE3 [2 datoshi]
    /// 0D : DROP [2 datoshi]
    /// 0E : DROP [2 datoshi]
    /// 0F : RET [0 datoshi]
    /// 10 : DROP [2 datoshi]
    /// 11 : LT [8 datoshi]
    /// 12 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsBetween")]
    public abstract bool? TestCharIsBetween(BigInteger? c, BigInteger? lower, BigInteger? upper);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQACC7UAB/AaAAu6xA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : PUSHINT8 20 [1 datoshi]
    /// 08 : WITHIN [8 datoshi]
    /// 09 : SWAP [2 datoshi]
    /// 0A : PUSHINT8 7F [1 datoshi]
    /// 0C : PUSHINT16 A000 [1 datoshi]
    /// 0F : WITHIN [8 datoshi]
    /// 10 : BOOLOR [8 datoshi]
    /// 11 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsControl")]
    public abstract bool? TestCharIsControl(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAAwADq7QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT8 30 [1 datoshi]
    /// 06 : PUSHINT8 3A [1 datoshi]
    /// 08 : WITHIN [8 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsDigit")]
    public abstract bool? TestCharIsDigit(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAIA2AAAAgDcAAC7QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT32 00D80000 [1 datoshi]
    /// 09 : PUSHINT32 00DC0000 [1 datoshi]
    /// 0E : WITHIN [8 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsHighSurrogate")]
    public abstract bool? TestCharIsHighSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAQQBbu1AAYQB7u6xA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSHINT8 41 [1 datoshi]
    /// 07 : PUSHINT8 5B [1 datoshi]
    /// 09 : WITHIN [8 datoshi]
    /// 0A : SWAP [2 datoshi]
    /// 0B : PUSHINT8 61 [1 datoshi]
    /// 0D : PUSHINT8 7B [1 datoshi]
    /// 0F : WITHIN [8 datoshi]
    /// 10 : BOOLOR [8 datoshi]
    /// 11 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsLetter")]
    public abstract bool? TestCharIsLetter(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAMAA6uyQPSgBBAFu7JAcAYQB7u0A=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSHINT8 30 [1 datoshi]
    /// 07 : PUSHINT8 3A [1 datoshi]
    /// 09 : WITHIN [8 datoshi]
    /// 0A : JMPIF 0F [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSHINT8 41 [1 datoshi]
    /// 0F : PUSHINT8 5B [1 datoshi]
    /// 11 : WITHIN [8 datoshi]
    /// 12 : JMPIF 07 [2 datoshi]
    /// 14 : PUSHINT8 61 [1 datoshi]
    /// 16 : PUSHINT8 7B [1 datoshi]
    /// 18 : WITHIN [8 datoshi]
    /// 19 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsLetterOrDigit")]
    public abstract bool? TestCharIsLetterOrDigit(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeABhAHu7QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT8 61 [1 datoshi]
    /// 06 : PUSHINT8 7B [1 datoshi]
    /// 08 : WITHIN [8 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsLower")]
    public abstract bool? TestCharIsLower(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAIA3AAAAgDgAAC7QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT32 00DC0000 [1 datoshi]
    /// 09 : PUSHINT32 00E00000 [1 datoshi]
    /// 0E : WITHIN [8 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsLowSurrogate")]
    public abstract bool? TestCharIsLowSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAIQAwuyQXSgA6AEG7JA9KAFsAYbskBwB7AH+7QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSHINT8 21 [1 datoshi]
    /// 07 : PUSHINT8 30 [1 datoshi]
    /// 09 : WITHIN [8 datoshi]
    /// 0A : JMPIF 17 [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSHINT8 3A [1 datoshi]
    /// 0F : PUSHINT8 41 [1 datoshi]
    /// 11 : WITHIN [8 datoshi]
    /// 12 : JMPIF 0F [2 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : PUSHINT8 5B [1 datoshi]
    /// 17 : PUSHINT8 61 [1 datoshi]
    /// 19 : WITHIN [8 datoshi]
    /// 1A : JMPIF 07 [2 datoshi]
    /// 1C : PUSHINT8 7B [1 datoshi]
    /// 1E : PUSHINT8 7F [1 datoshi]
    /// 20 : WITHIN [8 datoshi]
    /// 21 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsPunctuation")]
    public abstract bool? TestCharIsPunctuation(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoCANgAAAIA3AAAu1ACANwAAAIA4AAAu6xA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSHINT32 00D80000 [1 datoshi]
    /// 0A : PUSHINT32 00DC0000 [1 datoshi]
    /// 0F : WITHIN [8 datoshi]
    /// 10 : SWAP [2 datoshi]
    /// 11 : PUSHINT32 00DC0000 [1 datoshi]
    /// 16 : PUSHINT32 00E00000 [1 datoshi]
    /// 1B : WITHIN [8 datoshi]
    /// 1C : BOOLOR [8 datoshi]
    /// 1D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsSurrogate")]
    public abstract bool? TestCharIsSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAJAAsuyQfSgA8AD67JBdKAD4AQbskD0oAWwBhuyQHAHsAf7tA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSHINT8 24 [1 datoshi]
    /// 07 : PUSHINT8 2C [1 datoshi]
    /// 09 : WITHIN [8 datoshi]
    /// 0A : JMPIF 1F [2 datoshi]
    /// 0C : DUP [2 datoshi]
    /// 0D : PUSHINT8 3C [1 datoshi]
    /// 0F : PUSHINT8 3E [1 datoshi]
    /// 11 : WITHIN [8 datoshi]
    /// 12 : JMPIF 17 [2 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : PUSHINT8 3E [1 datoshi]
    /// 17 : PUSHINT8 41 [1 datoshi]
    /// 19 : WITHIN [8 datoshi]
    /// 1A : JMPIF 0F [2 datoshi]
    /// 1C : DUP [2 datoshi]
    /// 1D : PUSHINT8 5B [1 datoshi]
    /// 1F : PUSHINT8 61 [1 datoshi]
    /// 21 : WITHIN [8 datoshi]
    /// 22 : JMPIF 07 [2 datoshi]
    /// 24 : PUSHINT8 7B [1 datoshi]
    /// 26 : PUSHINT8 7F [1 datoshi]
    /// 28 : WITHIN [8 datoshi]
    /// 29 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsSymbol")]
    public abstract bool? TestCharIsSymbol(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeABBAFu7QA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSHINT8 41 [1 datoshi]
    /// 06 : PUSHINT8 5B [1 datoshi]
    /// 08 : WITHIN [8 datoshi]
    /// 09 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsUpper")]
    public abstract bool? TestCharIsUpper(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoZHrtQACCzrEA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH9 [1 datoshi]
    /// 06 : PUSH14 [1 datoshi]
    /// 07 : WITHIN [8 datoshi]
    /// 08 : SWAP [2 datoshi]
    /// 09 : PUSHINT8 20 [1 datoshi]
    /// 0B : NUMEQUAL [8 datoshi]
    /// 0C : BOOLOR [8 datoshi]
    /// 0D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharIsWhiteSpace")]
    public abstract bool? TestCharIsWhiteSpace(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAQQBbuyYIAEGfAGGeQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSHINT8 41 [1 datoshi]
    /// 07 : PUSHINT8 5B [1 datoshi]
    /// 09 : WITHIN [8 datoshi]
    /// 0A : JMPIFNOT 08 [2 datoshi]
    /// 0C : PUSHINT8 41 [1 datoshi]
    /// 0E : SUB [8 datoshi]
    /// 0F : PUSHINT8 61 [1 datoshi]
    /// 11 : ADD [8 datoshi]
    /// 12 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharToLower")]
    public abstract BigInteger? TestCharToLower(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAYQB7uyYIAGGfAEGeQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSHINT8 61 [1 datoshi]
    /// 07 : PUSHINT8 7B [1 datoshi]
    /// 09 : WITHIN [8 datoshi]
    /// 0A : JMPIFNOT 08 [2 datoshi]
    /// 0C : PUSHINT8 61 [1 datoshi]
    /// 0E : SUB [8 datoshi]
    /// 0F : PUSHINT8 41 [1 datoshi]
    /// 11 : ADD [8 datoshi]
    /// 12 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharToUpper")]
    public abstract BigInteger? TestCharToUpper(BigInteger? c);

    #endregion
}
