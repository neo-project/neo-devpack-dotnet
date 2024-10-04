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
    [DisplayName("testCharGetNumericValue")]
    public abstract BigInteger? TestCharGetNumericValue(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSHINT8
    // 0007 : PUSHINT8
    // 0009 : WITHIN
    // 000A : JMPIF
    // 000C : DROP
    // 000D : PUSHM1
    // 000E : RET
    // 000F : PUSHINT8
    // 0011 : SUB
    // 0012 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharIsBetween")]
    public abstract bool? TestCharIsBetween(BigInteger? c, BigInteger? lower, BigInteger? upper);
    // 0000 : INITSLOT
    // 0003 : LDARG2
    // 0004 : LDARG1
    // 0005 : LDARG0
    // 0006 : DUP
    // 0007 : ROT
    // 0008 : GE
    // 0009 : DUP
    // 000A : JMPIFNOT
    // 000C : REVERSE3
    // 000D : DROP
    // 000E : DROP
    // 000F : RET
    // 0010 : DROP
    // 0011 : LT
    // 0012 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharIsControl")]
    public abstract bool? TestCharIsControl(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH0
    // 0006 : PUSHINT8
    // 0008 : WITHIN
    // 0009 : SWAP
    // 000A : PUSHINT8
    // 000C : PUSHINT16
    // 000F : WITHIN
    // 0010 : BOOLOR
    // 0011 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharIsDigit")]
    public abstract bool? TestCharIsDigit(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT8
    // 0006 : PUSHINT8
    // 0008 : WITHIN
    // 0009 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharIsHighSurrogate")]
    public abstract bool? TestCharIsHighSurrogate(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT32
    // 0009 : PUSHINT32
    // 000E : WITHIN
    // 000F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharIsLetter")]
    public abstract bool? TestCharIsLetter(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSHINT8
    // 0007 : PUSHINT8
    // 0009 : WITHIN
    // 000A : SWAP
    // 000B : PUSHINT8
    // 000D : PUSHINT8
    // 000F : WITHIN
    // 0010 : BOOLOR
    // 0011 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharIsLetterOrDigit")]
    public abstract bool? TestCharIsLetterOrDigit(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSHINT8
    // 0007 : PUSHINT8
    // 0009 : WITHIN
    // 000A : JMPIF
    // 000C : DUP
    // 000D : PUSHINT8
    // 000F : PUSHINT8
    // 0011 : WITHIN
    // 0012 : JMPIF
    // 0014 : PUSHINT8
    // 0016 : PUSHINT8
    // 0018 : WITHIN
    // 0019 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharIsLower")]
    public abstract bool? TestCharIsLower(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT8
    // 0006 : PUSHINT8
    // 0008 : WITHIN
    // 0009 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharIsLowSurrogate")]
    public abstract bool? TestCharIsLowSurrogate(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT32
    // 0009 : PUSHINT32
    // 000E : WITHIN
    // 000F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharIsPunctuation")]
    public abstract bool? TestCharIsPunctuation(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSHINT8
    // 0007 : PUSHINT8
    // 0009 : WITHIN
    // 000A : JMPIF
    // 000C : DUP
    // 000D : PUSHINT8
    // 000F : PUSHINT8
    // 0011 : WITHIN
    // 0012 : JMPIF
    // 0014 : DUP
    // 0015 : PUSHINT8
    // 0017 : PUSHINT8
    // 0019 : WITHIN
    // 001A : JMPIF
    // 001C : PUSHINT8
    // 001E : PUSHINT8
    // 0020 : WITHIN
    // 0021 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharIsSurrogate")]
    public abstract bool? TestCharIsSurrogate(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSHINT32
    // 000A : PUSHINT32
    // 000F : WITHIN
    // 0010 : SWAP
    // 0011 : PUSHINT32
    // 0016 : PUSHINT32
    // 001B : WITHIN
    // 001C : BOOLOR
    // 001D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharIsSymbol")]
    public abstract bool? TestCharIsSymbol(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSHINT8
    // 0007 : PUSHINT8
    // 0009 : WITHIN
    // 000A : JMPIF
    // 000C : DUP
    // 000D : PUSHINT8
    // 000F : PUSHINT8
    // 0011 : WITHIN
    // 0012 : JMPIF
    // 0014 : DUP
    // 0015 : PUSHINT8
    // 0017 : PUSHINT8
    // 0019 : WITHIN
    // 001A : JMPIF
    // 001C : DUP
    // 001D : PUSHINT8
    // 001F : PUSHINT8
    // 0021 : WITHIN
    // 0022 : JMPIF
    // 0024 : PUSHINT8
    // 0026 : PUSHINT8
    // 0028 : WITHIN
    // 0029 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharIsUpper")]
    public abstract bool? TestCharIsUpper(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : PUSHINT8
    // 0006 : PUSHINT8
    // 0008 : WITHIN
    // 0009 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharIsWhiteSpace")]
    public abstract bool? TestCharIsWhiteSpace(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSH9
    // 0006 : PUSH14
    // 0007 : WITHIN
    // 0008 : SWAP
    // 0009 : PUSH10
    // 000A : PUSHINT8
    // 000C : WITHIN
    // 000D : BOOLOR
    // 000E : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharToLower")]
    public abstract BigInteger? TestCharToLower(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSHINT8
    // 0007 : PUSHINT8
    // 0009 : WITHIN
    // 000A : JMPIFNOT
    // 000C : PUSHINT8
    // 000E : SUB
    // 000F : PUSHINT8
    // 0011 : ADD
    // 0012 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCharToUpper")]
    public abstract BigInteger? TestCharToUpper(BigInteger? c);
    // 0000 : INITSLOT
    // 0003 : LDARG0
    // 0004 : DUP
    // 0005 : PUSHINT8
    // 0007 : PUSHINT8
    // 0009 : WITHIN
    // 000A : JMPIFNOT
    // 000C : PUSHINT8
    // 000E : SUB
    // 000F : PUSHINT8
    // 0011 : ADD
    // 0012 : RET

    #endregion

}
