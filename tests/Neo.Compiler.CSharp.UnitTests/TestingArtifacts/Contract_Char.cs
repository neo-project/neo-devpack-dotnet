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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Char"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testCharIsDigit"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testCharIsLetter"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":10,""safe"":false},{""name"":""testCharIsWhiteSpace"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":28,""safe"":false},{""name"":""testCharIsLetterOrDigit"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":42,""safe"":false},{""name"":""testCharIsLower"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":68,""safe"":false},{""name"":""testCharToLower"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":78,""safe"":false},{""name"":""testCharIsUpper"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":94,""safe"":false},{""name"":""testCharToUpper"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":104,""safe"":false},{""name"":""testCharToUpperInvariant"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":120,""safe"":false},{""name"":""testCharToLowerInvariant"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":136,""safe"":false},{""name"":""testCharGetNumericValue"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":152,""safe"":false},{""name"":""testCharIsPunctuation"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":171,""safe"":false},{""name"":""testCharIsSymbol"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":205,""safe"":false},{""name"":""testCharIsControl"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":247,""safe"":false},{""name"":""testCharIsSurrogate"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":265,""safe"":false},{""name"":""testCharIsHighSurrogate"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":295,""safe"":false},{""name"":""testCharIsLowSurrogate"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":311,""safe"":false},{""name"":""testCharIsBetween"",""parameters"":[{""name"":""c"",""type"":""Integer""},{""name"":""lower"",""type"":""Integer""},{""name"":""upper"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":327,""safe"":false},{""name"":""testCharParse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Integer"",""offset"":346,""safe"":false},{""name"":""testCharTryParse"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Array"",""offset"":365,""safe"":false},{""name"":""_initialize"",""parameters"":[],""returntype"":""Void"",""offset"":402,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.9.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2VAVcAAXgAMAA6u0BXAAF4SgBBAFu7UABhAHu7rEBXAAF4Shkeu1AAILOsQFcAAXhKADAAOrskD0oAQQBbuyQHAGEAe7tAVwABeABhAHu7QFcAAXhKAEEAW7smBQAgnkBXAAF4AEEAW7tAVwABeEoAYQB7uyYFACCfQFcAAXhKAGEAe7smBQAgn0BXAAF4SgBBAFu7JgUAIJ5AVwABeEoAMAA6uyQFRQ9AADCfQFcAAXhKACEAMLskF0oAOgBBuyQPSgBbAGG7JAcAewB/u0BXAAF4SgAkACy7JB9KADwAPrskF0oAPgBBuyQPSgBbAGG7JAcAewB/u0BXAAF4ShAAILtQAH8BoAC7rEBXAAF4SgIA2AAAAgDcAAC7UAIA3AAAAgDgAAC7rEBXAAF4AgDYAAACANwAALtAVwABeAIA3AAAAgDgAAC7QFcAA3p5eEpRuEomBlNFRUBFtUBXAAF4StgmAzpKyhGzJAM6EM5AVwEBEEpheFBFStgkDkrKEbMmCBDOYAgiBkUQYAlYYXBZaBK/QFYCQGlxDLM=").AsSerializable<Neo.SmartContract.NefFile>();

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
    /// Script: VwABeErYJgM6SsoRsyQDOhDOQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharParse")]
    public abstract BigInteger? TestCharParse(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAQQBbuyYFACCeQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// PUSHINT8 5B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// ADD [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharToLower")]
    public abstract BigInteger? TestCharToLower(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAQQBbuyYFACCeQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 41 [1 datoshi]
    /// PUSHINT8 5B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// ADD [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharToLowerInvariant")]
    public abstract BigInteger? TestCharToLowerInvariant(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAYQB7uyYFACCfQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharToUpper")]
    public abstract BigInteger? TestCharToUpper(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeEoAYQB7uyYFACCfQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 61 [1 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharToUpperInvariant")]
    public abstract BigInteger? TestCharToUpperInvariant(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBEEpheFBFStgkDkrKEbMmCBDOYAgiBkUQYAlYYXBZaBK/QA==
    /// INITSLOT 0101 [64 datoshi]
    /// PUSH0 [1 datoshi]
    /// DUP [2 datoshi]
    /// STSFLD1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// DROP [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0E [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// STSFLD0 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// STSFLD0 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// LDSFLD0 [2 datoshi]
    /// STSFLD1 [2 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDSFLD1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCharTryParse")]
    public abstract IList<object>? TestCharTryParse(string? value);

    #endregion
}
