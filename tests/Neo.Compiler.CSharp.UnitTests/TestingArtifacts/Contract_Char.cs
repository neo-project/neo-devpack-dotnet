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
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHINT8 30 	-> 1 datoshi
    /// 07 : OpCode.PUSHINT8 3A 	-> 1 datoshi
    /// 09 : OpCode.WITHIN 	-> 8 datoshi
    /// 0A : OpCode.JMPIF 05 	-> 2 datoshi
    /// 0C : OpCode.DROP 	-> 2 datoshi
    /// 0D : OpCode.PUSHM1 	-> 1 datoshi
    /// 0E : OpCode.RET 	-> 0 datoshi
    /// 0F : OpCode.PUSHINT8 30 	-> 1 datoshi
    /// 11 : OpCode.SUB 	-> 8 datoshi
    /// 12 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharGetNumericValue")]
    public abstract BigInteger? TestCharGetNumericValue(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4SlG4SiYGU0VFQEW1QA==
    /// 00 : OpCode.INITSLOT 0003 	-> 64 datoshi
    /// 03 : OpCode.LDARG2 	-> 2 datoshi
    /// 04 : OpCode.LDARG1 	-> 2 datoshi
    /// 05 : OpCode.LDARG0 	-> 2 datoshi
    /// 06 : OpCode.DUP 	-> 2 datoshi
    /// 07 : OpCode.ROT 	-> 2 datoshi
    /// 08 : OpCode.GE 	-> 8 datoshi
    /// 09 : OpCode.DUP 	-> 2 datoshi
    /// 0A : OpCode.JMPIFNOT 06 	-> 2 datoshi
    /// 0C : OpCode.REVERSE3 	-> 2 datoshi
    /// 0D : OpCode.DROP 	-> 2 datoshi
    /// 0E : OpCode.DROP 	-> 2 datoshi
    /// 0F : OpCode.RET 	-> 0 datoshi
    /// 10 : OpCode.DROP 	-> 2 datoshi
    /// 11 : OpCode.LT 	-> 8 datoshi
    /// 12 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharIsBetween")]
    public abstract bool? TestCharIsBetween(BigInteger? c, BigInteger? lower, BigInteger? upper);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoQACC7UAB/AaAAu6xA
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSH0 	-> 1 datoshi
    /// 06 : OpCode.PUSHINT8 20 	-> 1 datoshi
    /// 08 : OpCode.WITHIN 	-> 8 datoshi
    /// 09 : OpCode.SWAP 	-> 2 datoshi
    /// 0A : OpCode.PUSHINT8 7F 	-> 1 datoshi
    /// 0C : OpCode.PUSHINT16 A000 	-> 1 datoshi
    /// 0F : OpCode.WITHIN 	-> 8 datoshi
    /// 10 : OpCode.BOOLOR 	-> 8 datoshi
    /// 11 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharIsControl")]
    public abstract bool? TestCharIsControl(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAAwADq7QA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.PUSHINT8 30 	-> 1 datoshi
    /// 06 : OpCode.PUSHINT8 3A 	-> 1 datoshi
    /// 08 : OpCode.WITHIN 	-> 8 datoshi
    /// 09 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharIsDigit")]
    public abstract bool? TestCharIsDigit(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAIA2AAAAgDcAAC7QA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.PUSHINT32 00D80000 	-> 1 datoshi
    /// 09 : OpCode.PUSHINT32 00DC0000 	-> 1 datoshi
    /// 0E : OpCode.WITHIN 	-> 8 datoshi
    /// 0F : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharIsHighSurrogate")]
    public abstract bool? TestCharIsHighSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAQQBbu1AAYQB7u6xA
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHINT8 41 	-> 1 datoshi
    /// 07 : OpCode.PUSHINT8 5B 	-> 1 datoshi
    /// 09 : OpCode.WITHIN 	-> 8 datoshi
    /// 0A : OpCode.SWAP 	-> 2 datoshi
    /// 0B : OpCode.PUSHINT8 61 	-> 1 datoshi
    /// 0D : OpCode.PUSHINT8 7B 	-> 1 datoshi
    /// 0F : OpCode.WITHIN 	-> 8 datoshi
    /// 10 : OpCode.BOOLOR 	-> 8 datoshi
    /// 11 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharIsLetter")]
    public abstract bool? TestCharIsLetter(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAMAA6uyQPSgBBAFu7JAcAYQB7u0A=
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHINT8 30 	-> 1 datoshi
    /// 07 : OpCode.PUSHINT8 3A 	-> 1 datoshi
    /// 09 : OpCode.WITHIN 	-> 8 datoshi
    /// 0A : OpCode.JMPIF 0F 	-> 2 datoshi
    /// 0C : OpCode.DUP 	-> 2 datoshi
    /// 0D : OpCode.PUSHINT8 41 	-> 1 datoshi
    /// 0F : OpCode.PUSHINT8 5B 	-> 1 datoshi
    /// 11 : OpCode.WITHIN 	-> 8 datoshi
    /// 12 : OpCode.JMPIF 07 	-> 2 datoshi
    /// 14 : OpCode.PUSHINT8 61 	-> 1 datoshi
    /// 16 : OpCode.PUSHINT8 7B 	-> 1 datoshi
    /// 18 : OpCode.WITHIN 	-> 8 datoshi
    /// 19 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharIsLetterOrDigit")]
    public abstract bool? TestCharIsLetterOrDigit(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeABhAHu7QA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.PUSHINT8 61 	-> 1 datoshi
    /// 06 : OpCode.PUSHINT8 7B 	-> 1 datoshi
    /// 08 : OpCode.WITHIN 	-> 8 datoshi
    /// 09 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharIsLower")]
    public abstract bool? TestCharIsLower(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeAIA3AAAAgDgAAC7QA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.PUSHINT32 00DC0000 	-> 1 datoshi
    /// 09 : OpCode.PUSHINT32 00E00000 	-> 1 datoshi
    /// 0E : OpCode.WITHIN 	-> 8 datoshi
    /// 0F : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharIsLowSurrogate")]
    public abstract bool? TestCharIsLowSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAIQAwuyQXSgA6AEG7JA9KAFsAYbskBwB7AH+7QA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHINT8 21 	-> 1 datoshi
    /// 07 : OpCode.PUSHINT8 30 	-> 1 datoshi
    /// 09 : OpCode.WITHIN 	-> 8 datoshi
    /// 0A : OpCode.JMPIF 17 	-> 2 datoshi
    /// 0C : OpCode.DUP 	-> 2 datoshi
    /// 0D : OpCode.PUSHINT8 3A 	-> 1 datoshi
    /// 0F : OpCode.PUSHINT8 41 	-> 1 datoshi
    /// 11 : OpCode.WITHIN 	-> 8 datoshi
    /// 12 : OpCode.JMPIF 0F 	-> 2 datoshi
    /// 14 : OpCode.DUP 	-> 2 datoshi
    /// 15 : OpCode.PUSHINT8 5B 	-> 1 datoshi
    /// 17 : OpCode.PUSHINT8 61 	-> 1 datoshi
    /// 19 : OpCode.WITHIN 	-> 8 datoshi
    /// 1A : OpCode.JMPIF 07 	-> 2 datoshi
    /// 1C : OpCode.PUSHINT8 7B 	-> 1 datoshi
    /// 1E : OpCode.PUSHINT8 7F 	-> 1 datoshi
    /// 20 : OpCode.WITHIN 	-> 8 datoshi
    /// 21 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharIsPunctuation")]
    public abstract bool? TestCharIsPunctuation(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoCANgAAAIA3AAAu1ACANwAAAIA4AAAu6xA
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHINT32 00D80000 	-> 1 datoshi
    /// 0A : OpCode.PUSHINT32 00DC0000 	-> 1 datoshi
    /// 0F : OpCode.WITHIN 	-> 8 datoshi
    /// 10 : OpCode.SWAP 	-> 2 datoshi
    /// 11 : OpCode.PUSHINT32 00DC0000 	-> 1 datoshi
    /// 16 : OpCode.PUSHINT32 00E00000 	-> 1 datoshi
    /// 1B : OpCode.WITHIN 	-> 8 datoshi
    /// 1C : OpCode.BOOLOR 	-> 8 datoshi
    /// 1D : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharIsSurrogate")]
    public abstract bool? TestCharIsSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAJAAsuyQfSgA8AD67JBdKAD4AQbskD0oAWwBhuyQHAHsAf7tA
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHINT8 24 	-> 1 datoshi
    /// 07 : OpCode.PUSHINT8 2C 	-> 1 datoshi
    /// 09 : OpCode.WITHIN 	-> 8 datoshi
    /// 0A : OpCode.JMPIF 1F 	-> 2 datoshi
    /// 0C : OpCode.DUP 	-> 2 datoshi
    /// 0D : OpCode.PUSHINT8 3C 	-> 1 datoshi
    /// 0F : OpCode.PUSHINT8 3E 	-> 1 datoshi
    /// 11 : OpCode.WITHIN 	-> 8 datoshi
    /// 12 : OpCode.JMPIF 17 	-> 2 datoshi
    /// 14 : OpCode.DUP 	-> 2 datoshi
    /// 15 : OpCode.PUSHINT8 3E 	-> 1 datoshi
    /// 17 : OpCode.PUSHINT8 41 	-> 1 datoshi
    /// 19 : OpCode.WITHIN 	-> 8 datoshi
    /// 1A : OpCode.JMPIF 0F 	-> 2 datoshi
    /// 1C : OpCode.DUP 	-> 2 datoshi
    /// 1D : OpCode.PUSHINT8 5B 	-> 1 datoshi
    /// 1F : OpCode.PUSHINT8 61 	-> 1 datoshi
    /// 21 : OpCode.WITHIN 	-> 8 datoshi
    /// 22 : OpCode.JMPIF 07 	-> 2 datoshi
    /// 24 : OpCode.PUSHINT8 7B 	-> 1 datoshi
    /// 26 : OpCode.PUSHINT8 7F 	-> 1 datoshi
    /// 28 : OpCode.WITHIN 	-> 8 datoshi
    /// 29 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharIsSymbol")]
    public abstract bool? TestCharIsSymbol(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeABBAFu7QA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.PUSHINT8 41 	-> 1 datoshi
    /// 06 : OpCode.PUSHINT8 5B 	-> 1 datoshi
    /// 08 : OpCode.WITHIN 	-> 8 datoshi
    /// 09 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharIsUpper")]
    public abstract bool? TestCharIsUpper(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoZHrtQACCzrEA=
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSH9 	-> 1 datoshi
    /// 06 : OpCode.PUSH14 	-> 1 datoshi
    /// 07 : OpCode.WITHIN 	-> 8 datoshi
    /// 08 : OpCode.SWAP 	-> 2 datoshi
    /// 09 : OpCode.PUSHINT8 20 	-> 1 datoshi
    /// 0B : OpCode.NUMEQUAL 	-> 8 datoshi
    /// 0C : OpCode.BOOLOR 	-> 8 datoshi
    /// 0D : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharIsWhiteSpace")]
    public abstract bool? TestCharIsWhiteSpace(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAQQBbuyYIAEGfAGGeQA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHINT8 41 	-> 1 datoshi
    /// 07 : OpCode.PUSHINT8 5B 	-> 1 datoshi
    /// 09 : OpCode.WITHIN 	-> 8 datoshi
    /// 0A : OpCode.JMPIFNOT 08 	-> 2 datoshi
    /// 0C : OpCode.PUSHINT8 41 	-> 1 datoshi
    /// 0E : OpCode.SUB 	-> 8 datoshi
    /// 0F : OpCode.PUSHINT8 61 	-> 1 datoshi
    /// 11 : OpCode.ADD 	-> 8 datoshi
    /// 12 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharToLower")]
    public abstract BigInteger? TestCharToLower(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAYQB7uyYIAGGfAEGeQA==
    /// 00 : OpCode.INITSLOT 0001 	-> 64 datoshi
    /// 03 : OpCode.LDARG0 	-> 2 datoshi
    /// 04 : OpCode.DUP 	-> 2 datoshi
    /// 05 : OpCode.PUSHINT8 61 	-> 1 datoshi
    /// 07 : OpCode.PUSHINT8 7B 	-> 1 datoshi
    /// 09 : OpCode.WITHIN 	-> 8 datoshi
    /// 0A : OpCode.JMPIFNOT 08 	-> 2 datoshi
    /// 0C : OpCode.PUSHINT8 61 	-> 1 datoshi
    /// 0E : OpCode.SUB 	-> 8 datoshi
    /// 0F : OpCode.PUSHINT8 41 	-> 1 datoshi
    /// 11 : OpCode.ADD 	-> 8 datoshi
    /// 12 : OpCode.RET 	-> 0 datoshi
    /// </remarks>
    [DisplayName("testCharToUpper")]
    public abstract BigInteger? TestCharToUpper(BigInteger? c);

    #endregion
}
