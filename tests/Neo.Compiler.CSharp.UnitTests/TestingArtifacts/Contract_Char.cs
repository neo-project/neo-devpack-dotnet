using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Char(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Char"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testCharIsDigit"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testCharIsLetter"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":10,""safe"":false},{""name"":""testCharIsWhiteSpace"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":28,""safe"":false},{""name"":""testCharIsLetterOrDigit"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":43,""safe"":false},{""name"":""testCharIsLower"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":69,""safe"":false},{""name"":""testCharToLower"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":79,""safe"":false},{""name"":""testCharIsUpper"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":98,""safe"":false},{""name"":""testCharToUpper"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":108,""safe"":false},{""name"":""testCharGetNumericValue"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":127,""safe"":false},{""name"":""testCharIsPunctuation"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":146,""safe"":false},{""name"":""testCharIsSymbol"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":180,""safe"":false},{""name"":""testCharIsControl"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":222,""safe"":false},{""name"":""testCharIsSurrogate"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":240,""safe"":false},{""name"":""testCharIsHighSurrogate"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":270,""safe"":false},{""name"":""testCharIsLowSurrogate"",""parameters"":[{""name"":""c"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":286,""safe"":false},{""name"":""testCharIsBetween"",""parameters"":[{""name"":""c"",""type"":""Integer""},{""name"":""lower"",""type"":""Integer""},{""name"":""upper"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":302,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP1BAVcAAXgAMAA6u0BXAAF4SgBBAFu7UABhAHu7rEBXAAF4Shkeu1AaACG7rEBXAAF4SgAwADq7JA9KAEEAW7skBwBhAHu7QFcAAXgAYQB7u0BXAAF4SgBBAFu7JggAQZ8AYZ5AVwABeABBAFu7QFcAAXhKAGEAe7smCABhnwBBnkBXAAF4SgAwADq7JAVFD0AAMJ9AVwABeEoAIQAwuyQXSgA6AEG7JA9KAFsAYbskBwB7AH+7QFcAAXhKACQALLskH0oAPAA+uyQXSgA+AEG7JA9KAFsAYbskBwB7AH+7QFcAAXhKEAAgu1AAfwGgALusQFcAAXhKAgDYAAACANwAALtQAgDcAAACAOAAALusQFcAAXgCANgAAAIA3AAAu0BXAAF4AgDcAAACAOAAALtAVwADenl4SlG4SiYGU0VFQEW1QJtVsag="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT8 30
    /// 0007 : OpCode.PUSHINT8 3A
    /// 0009 : OpCode.WITHIN
    /// 000A : OpCode.JMPIF 05
    /// 000C : OpCode.DROP
    /// 000D : OpCode.PUSHM1
    /// 000E : OpCode.RET
    /// 000F : OpCode.PUSHINT8 30
    /// 0011 : OpCode.SUB
    /// 0012 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharGetNumericValue")]
    public abstract BigInteger? TestCharGetNumericValue(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0003
    /// 0003 : OpCode.LDARG2
    /// 0004 : OpCode.LDARG1
    /// 0005 : OpCode.LDARG0
    /// 0006 : OpCode.DUP
    /// 0007 : OpCode.ROT
    /// 0008 : OpCode.GE
    /// 0009 : OpCode.DUP
    /// 000A : OpCode.JMPIFNOT 06
    /// 000C : OpCode.REVERSE3
    /// 000D : OpCode.DROP
    /// 000E : OpCode.DROP
    /// 000F : OpCode.RET
    /// 0010 : OpCode.DROP
    /// 0011 : OpCode.LT
    /// 0012 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsBetween")]
    public abstract bool? TestCharIsBetween(BigInteger? c, BigInteger? lower, BigInteger? upper);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH0
    /// 0006 : OpCode.PUSHINT8 20
    /// 0008 : OpCode.WITHIN
    /// 0009 : OpCode.SWAP
    /// 000A : OpCode.PUSHINT8 7F
    /// 000C : OpCode.PUSHINT16 A000
    /// 000F : OpCode.WITHIN
    /// 0010 : OpCode.BOOLOR
    /// 0011 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsControl")]
    public abstract bool? TestCharIsControl(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT8 30
    /// 0006 : OpCode.PUSHINT8 3A
    /// 0008 : OpCode.WITHIN
    /// 0009 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsDigit")]
    public abstract bool? TestCharIsDigit(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT32 00D80000
    /// 0009 : OpCode.PUSHINT32 00DC0000
    /// 000E : OpCode.WITHIN
    /// 000F : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsHighSurrogate")]
    public abstract bool? TestCharIsHighSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT8 41
    /// 0007 : OpCode.PUSHINT8 5B
    /// 0009 : OpCode.WITHIN
    /// 000A : OpCode.SWAP
    /// 000B : OpCode.PUSHINT8 61
    /// 000D : OpCode.PUSHINT8 7B
    /// 000F : OpCode.WITHIN
    /// 0010 : OpCode.BOOLOR
    /// 0011 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsLetter")]
    public abstract bool? TestCharIsLetter(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT8 30
    /// 0007 : OpCode.PUSHINT8 3A
    /// 0009 : OpCode.WITHIN
    /// 000A : OpCode.JMPIF 0F
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSHINT8 41
    /// 000F : OpCode.PUSHINT8 5B
    /// 0011 : OpCode.WITHIN
    /// 0012 : OpCode.JMPIF 07
    /// 0014 : OpCode.PUSHINT8 61
    /// 0016 : OpCode.PUSHINT8 7B
    /// 0018 : OpCode.WITHIN
    /// 0019 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsLetterOrDigit")]
    public abstract bool? TestCharIsLetterOrDigit(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT8 61
    /// 0006 : OpCode.PUSHINT8 7B
    /// 0008 : OpCode.WITHIN
    /// 0009 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsLower")]
    public abstract bool? TestCharIsLower(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT32 00DC0000
    /// 0009 : OpCode.PUSHINT32 00E00000
    /// 000E : OpCode.WITHIN
    /// 000F : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsLowSurrogate")]
    public abstract bool? TestCharIsLowSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT8 21
    /// 0007 : OpCode.PUSHINT8 30
    /// 0009 : OpCode.WITHIN
    /// 000A : OpCode.JMPIF 17
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSHINT8 3A
    /// 000F : OpCode.PUSHINT8 41
    /// 0011 : OpCode.WITHIN
    /// 0012 : OpCode.JMPIF 0F
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.PUSHINT8 5B
    /// 0017 : OpCode.PUSHINT8 61
    /// 0019 : OpCode.WITHIN
    /// 001A : OpCode.JMPIF 07
    /// 001C : OpCode.PUSHINT8 7B
    /// 001E : OpCode.PUSHINT8 7F
    /// 0020 : OpCode.WITHIN
    /// 0021 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsPunctuation")]
    public abstract bool? TestCharIsPunctuation(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT32 00D80000
    /// 000A : OpCode.PUSHINT32 00DC0000
    /// 000F : OpCode.WITHIN
    /// 0010 : OpCode.SWAP
    /// 0011 : OpCode.PUSHINT32 00DC0000
    /// 0016 : OpCode.PUSHINT32 00E00000
    /// 001B : OpCode.WITHIN
    /// 001C : OpCode.BOOLOR
    /// 001D : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsSurrogate")]
    public abstract bool? TestCharIsSurrogate(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT8 24
    /// 0007 : OpCode.PUSHINT8 2C
    /// 0009 : OpCode.WITHIN
    /// 000A : OpCode.JMPIF 1F
    /// 000C : OpCode.DUP
    /// 000D : OpCode.PUSHINT8 3C
    /// 000F : OpCode.PUSHINT8 3E
    /// 0011 : OpCode.WITHIN
    /// 0012 : OpCode.JMPIF 17
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.PUSHINT8 3E
    /// 0017 : OpCode.PUSHINT8 41
    /// 0019 : OpCode.WITHIN
    /// 001A : OpCode.JMPIF 0F
    /// 001C : OpCode.DUP
    /// 001D : OpCode.PUSHINT8 5B
    /// 001F : OpCode.PUSHINT8 61
    /// 0021 : OpCode.WITHIN
    /// 0022 : OpCode.JMPIF 07
    /// 0024 : OpCode.PUSHINT8 7B
    /// 0026 : OpCode.PUSHINT8 7F
    /// 0028 : OpCode.WITHIN
    /// 0029 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsSymbol")]
    public abstract bool? TestCharIsSymbol(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.PUSHINT8 41
    /// 0006 : OpCode.PUSHINT8 5B
    /// 0008 : OpCode.WITHIN
    /// 0009 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsUpper")]
    public abstract bool? TestCharIsUpper(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSH9
    /// 0006 : OpCode.PUSH14
    /// 0007 : OpCode.WITHIN
    /// 0008 : OpCode.SWAP
    /// 0009 : OpCode.PUSH10
    /// 000A : OpCode.PUSHINT8 21
    /// 000C : OpCode.WITHIN
    /// 000D : OpCode.BOOLOR
    /// 000E : OpCode.RET
    /// </remarks>
    [DisplayName("testCharIsWhiteSpace")]
    public abstract bool? TestCharIsWhiteSpace(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT8 41
    /// 0007 : OpCode.PUSHINT8 5B
    /// 0009 : OpCode.WITHIN
    /// 000A : OpCode.JMPIFNOT 08
    /// 000C : OpCode.PUSHINT8 41
    /// 000E : OpCode.SUB
    /// 000F : OpCode.PUSHINT8 61
    /// 0011 : OpCode.ADD
    /// 0012 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharToLower")]
    public abstract BigInteger? TestCharToLower(BigInteger? c);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0001
    /// 0003 : OpCode.LDARG0
    /// 0004 : OpCode.DUP
    /// 0005 : OpCode.PUSHINT8 61
    /// 0007 : OpCode.PUSHINT8 7B
    /// 0009 : OpCode.WITHIN
    /// 000A : OpCode.JMPIFNOT 08
    /// 000C : OpCode.PUSHINT8 61
    /// 000E : OpCode.SUB
    /// 000F : OpCode.PUSHINT8 41
    /// 0011 : OpCode.ADD
    /// 0012 : OpCode.RET
    /// </remarks>
    [DisplayName("testCharToUpper")]
    public abstract BigInteger? TestCharToUpper(BigInteger? c);

    #endregion

}
