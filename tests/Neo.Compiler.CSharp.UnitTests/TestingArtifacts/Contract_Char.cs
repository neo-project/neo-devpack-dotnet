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
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT8 30
    /// 07 : OpCode.PUSHINT8 3A
    /// 09 : OpCode.WITHIN
    /// 0A : OpCode.JMPIF 05
    /// 0C : OpCode.DROP
    /// 0D : OpCode.PUSHM1
    /// 0E : OpCode.RET
    /// 0F : OpCode.PUSHINT8 30
    /// 11 : OpCode.SUB
    /// 12 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharGetNumericValue")]
    public abstract BigInteger? TestCharGetNumericValue(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4SlG4SiYGU0VFQEW1QA==
    /// 00 : OpCode.INITSLOT 0003
    /// 03 : OpCode.LDARG2
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.DUP
    /// 07 : OpCode.ROT
    /// 08 : OpCode.GE
    /// 09 : OpCode.DUP
    /// 0A : OpCode.JMPIFNOT 06
    /// 0C : OpCode.REVERSE3
    /// 0D : OpCode.DROP
    /// 0E : OpCode.DROP
    /// 0F : OpCode.RET
    /// 10 : OpCode.DROP
    /// 11 : OpCode.LT
    /// 12 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsBetween")]
    public abstract bool? TestCharIsBetween(BigInteger? c, BigInteger? lower, BigInteger? upper);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQACC7UAB/AaAAu6xA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH0
    /// 06 : OpCode.PUSHINT8 20
    /// 08 : OpCode.WITHIN
    /// 09 : OpCode.SWAP
    /// 0A : OpCode.PUSHINT8 7F
    /// 0C : OpCode.PUSHINT16 A000
    /// 0F : OpCode.WITHIN
    /// 10 : OpCode.BOOLOR
    /// 11 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsControl")]
    public abstract bool? TestCharIsControl(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAAwADq7QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT8 30
    /// 06 : OpCode.PUSHINT8 3A
    /// 08 : OpCode.WITHIN
    /// 09 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsDigit")]
    public abstract bool? TestCharIsDigit(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAIA2AAAAgDcAAC7QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT32 00D80000
    /// 09 : OpCode.PUSHINT32 00DC0000
    /// 0E : OpCode.WITHIN
    /// 0F : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsHighSurrogate")]
    public abstract bool? TestCharIsHighSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAQQBbu1AAYQB7u6xA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT8 41
    /// 07 : OpCode.PUSHINT8 5B
    /// 09 : OpCode.WITHIN
    /// 0A : OpCode.SWAP
    /// 0B : OpCode.PUSHINT8 61
    /// 0D : OpCode.PUSHINT8 7B
    /// 0F : OpCode.WITHIN
    /// 10 : OpCode.BOOLOR
    /// 11 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsLetter")]
    public abstract bool? TestCharIsLetter(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAMAA6uyQPSgBBAFu7JAcAYQB7u0A=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT8 30
    /// 07 : OpCode.PUSHINT8 3A
    /// 09 : OpCode.WITHIN
    /// 0A : OpCode.JMPIF 0F
    /// 0C : OpCode.DUP
    /// 0D : OpCode.PUSHINT8 41
    /// 0F : OpCode.PUSHINT8 5B
    /// 11 : OpCode.WITHIN
    /// 12 : OpCode.JMPIF 07
    /// 14 : OpCode.PUSHINT8 61
    /// 16 : OpCode.PUSHINT8 7B
    /// 18 : OpCode.WITHIN
    /// 19 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsLetterOrDigit")]
    public abstract bool? TestCharIsLetterOrDigit(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeABhAHu7QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT8 61
    /// 06 : OpCode.PUSHINT8 7B
    /// 08 : OpCode.WITHIN
    /// 09 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsLower")]
    public abstract bool? TestCharIsLower(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAIA3AAAAgDgAAC7QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT32 00DC0000
    /// 09 : OpCode.PUSHINT32 00E00000
    /// 0E : OpCode.WITHIN
    /// 0F : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsLowSurrogate")]
    public abstract bool? TestCharIsLowSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAIQAwuyQXSgA6AEG7JA9KAFsAYbskBwB7AH+7QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT8 21
    /// 07 : OpCode.PUSHINT8 30
    /// 09 : OpCode.WITHIN
    /// 0A : OpCode.JMPIF 17
    /// 0C : OpCode.DUP
    /// 0D : OpCode.PUSHINT8 3A
    /// 0F : OpCode.PUSHINT8 41
    /// 11 : OpCode.WITHIN
    /// 12 : OpCode.JMPIF 0F
    /// 14 : OpCode.DUP
    /// 15 : OpCode.PUSHINT8 5B
    /// 17 : OpCode.PUSHINT8 61
    /// 19 : OpCode.WITHIN
    /// 1A : OpCode.JMPIF 07
    /// 1C : OpCode.PUSHINT8 7B
    /// 1E : OpCode.PUSHINT8 7F
    /// 20 : OpCode.WITHIN
    /// 21 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsPunctuation")]
    public abstract bool? TestCharIsPunctuation(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoCANgAAAIA3AAAu1ACANwAAAIA4AAAu6xA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT32 00D80000
    /// 0A : OpCode.PUSHINT32 00DC0000
    /// 0F : OpCode.WITHIN
    /// 10 : OpCode.SWAP
    /// 11 : OpCode.PUSHINT32 00DC0000
    /// 16 : OpCode.PUSHINT32 00E00000
    /// 1B : OpCode.WITHIN
    /// 1C : OpCode.BOOLOR
    /// 1D : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsSurrogate")]
    public abstract bool? TestCharIsSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAJAAsuyQfSgA8AD67JBdKAD4AQbskD0oAWwBhuyQHAHsAf7tA
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT8 24
    /// 07 : OpCode.PUSHINT8 2C
    /// 09 : OpCode.WITHIN
    /// 0A : OpCode.JMPIF 1F
    /// 0C : OpCode.DUP
    /// 0D : OpCode.PUSHINT8 3C
    /// 0F : OpCode.PUSHINT8 3E
    /// 11 : OpCode.WITHIN
    /// 12 : OpCode.JMPIF 17
    /// 14 : OpCode.DUP
    /// 15 : OpCode.PUSHINT8 3E
    /// 17 : OpCode.PUSHINT8 41
    /// 19 : OpCode.WITHIN
    /// 1A : OpCode.JMPIF 0F
    /// 1C : OpCode.DUP
    /// 1D : OpCode.PUSHINT8 5B
    /// 1F : OpCode.PUSHINT8 61
    /// 21 : OpCode.WITHIN
    /// 22 : OpCode.JMPIF 07
    /// 24 : OpCode.PUSHINT8 7B
    /// 26 : OpCode.PUSHINT8 7F
    /// 28 : OpCode.WITHIN
    /// 29 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsSymbol")]
    public abstract bool? TestCharIsSymbol(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeABBAFu7QA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.PUSHINT8 41
    /// 06 : OpCode.PUSHINT8 5B
    /// 08 : OpCode.WITHIN
    /// 09 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsUpper")]
    public abstract bool? TestCharIsUpper(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoZHrtQACCzrEA=
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSH9
    /// 06 : OpCode.PUSH14
    /// 07 : OpCode.WITHIN
    /// 08 : OpCode.SWAP
    /// 09 : OpCode.PUSHINT8 20
    /// 0B : OpCode.NUMEQUAL
    /// 0C : OpCode.BOOLOR
    /// 0D : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsWhiteSpace")]
    public abstract bool? TestCharIsWhiteSpace(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAQQBbuyYIAEGfAGGeQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT8 41
    /// 07 : OpCode.PUSHINT8 5B
    /// 09 : OpCode.WITHIN
    /// 0A : OpCode.JMPIFNOT 08
    /// 0C : OpCode.PUSHINT8 41
    /// 0E : OpCode.SUB
    /// 0F : OpCode.PUSHINT8 61
    /// 11 : OpCode.ADD
    /// 12 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharToLower")]
    public abstract BigInteger? TestCharToLower(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAYQB7uyYIAGGfAEGeQA==
    /// 00 : OpCode.INITSLOT 0001
    /// 03 : OpCode.LDARG0
    /// 04 : OpCode.DUP
    /// 05 : OpCode.PUSHINT8 61
    /// 07 : OpCode.PUSHINT8 7B
    /// 09 : OpCode.WITHIN
    /// 0A : OpCode.JMPIFNOT 08
    /// 0C : OpCode.PUSHINT8 61
    /// 0E : OpCode.SUB
    /// 0F : OpCode.PUSHINT8 41
    /// 11 : OpCode.ADD
    /// 12 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharToUpper")]
    public abstract BigInteger? TestCharToUpper(BigInteger? c);

    #endregion
}
