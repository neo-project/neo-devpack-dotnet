using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_BigInteger(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_BigInteger"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPow"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testSqrt"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":7,""safe"":false},{""name"":""testsbyte"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":13,""safe"":false},{""name"":""testbyte"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":48,""safe"":false},{""name"":""testshort"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":82,""safe"":false},{""name"":""testushort"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":120,""safe"":false},{""name"":""testint"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":156,""safe"":false},{""name"":""testuint"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":200,""safe"":false},{""name"":""testlong"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":240,""safe"":false},{""name"":""testulong"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":296,""safe"":false},{""name"":""testchar"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":344,""safe"":false},{""name"":""testchartostring"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""String"",""offset"":380,""safe"":false},{""name"":""testIsEven"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":400,""safe"":false},{""name"":""testIsZero"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":408,""safe"":false},{""name"":""testIsOne"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":415,""safe"":false},{""name"":""testSign"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":422,""safe"":false},{""name"":""testAdd"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":428,""safe"":false},{""name"":""testSubtract"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":435,""safe"":false},{""name"":""testNegate"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":442,""safe"":false},{""name"":""testMultiply"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":448,""safe"":false},{""name"":""testDivide"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":455,""safe"":false},{""name"":""testRemainder"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":462,""safe"":false},{""name"":""testCompare"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":469,""safe"":false},{""name"":""testGreatestCommonDivisor"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":477,""safe"":false},{""name"":""testEquals"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":493,""safe"":false},{""name"":""parseConstant"",""parameters"":[],""returntype"":""Integer"",""offset"":500,""safe"":false},{""name"":""testModPow"",""parameters"":[],""returntype"":""Integer"",""offset"":518,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0VAlcAAnh5o0BXAAF4pEBXAQE7EgB4SgCAAYAAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsRAHhKEAEAAbskAzpwaD0PcAwJZXhjZXB0aW9uOkBXAQE7FQB4SgEAgAIAgAAAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsTAHhKEAIAAAEAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsbAHhKAgAAAIADAAAAgAAAAAC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpAVwEBOxcAeEoQAwAAAAABAAAAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsnAHhKAwAAAAAAAACABAAAAAAAAACAAAAAAAAAAAC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpAVwEBOx8AeEoQBAAAAAAAAAAAAQAAAAAAAAC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpAVwEBOxMAeEoQAgAAAQC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpAVwEBeEoQAgAAAQC7JAM6cGjbKEBXAAF4EqKqQFcAAXixqkBXAAF4EbNAVwABeJlAVwACeHmeQFcAAnh5n0BXAAF4m0BXAAJ4eaBAVwACeHmhQFcAAnh5okBXAAJ4eZ+ZQFcAAnh5SlNQokqxJPpFmkBXAAJ4ebNABAAAAOTSDMjc0rdSAAAAAABAVwMAGnATcQAecmhpaqZAWB31KQ==").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: BAAAAOTSDMjc0rdSAAAAAABA
    /// 00 : PUSHINT128 000000E4D20CC8DCD2B7520000000000 [4 datoshi]
    /// 11 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("parseConstant")]
    public abstract BigInteger? ParseConstant();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : ADD [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAdd")]
    public abstract BigInteger? TestAdd(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxEAeEoQAQABuyQDOnBoPQ9wDAlleGNlcHRpb246QA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : TRY 1100 [4 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : PUSH0 [1 datoshi]
    /// 09 : PUSHINT16 0001 [1 datoshi]
    /// 0C : WITHIN [8 datoshi]
    /// 0D : JMPIF 03 [2 datoshi]
    /// 0F : THROW [512 datoshi]
    /// 10 : STLOC0 [2 datoshi]
    /// 11 : LDLOC0 [2 datoshi]
    /// 12 : ENDTRY 0F [4 datoshi]
    /// 14 : STLOC0 [2 datoshi]
    /// 15 : PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 20 : THROW [512 datoshi]
    /// 21 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testbyte")]
    public abstract BigInteger? Testbyte(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxMAeEoQAgAAAQC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpA
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : TRY 1300 [4 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : PUSH0 [1 datoshi]
    /// 09 : PUSHINT32 00000100 [1 datoshi]
    /// 0E : WITHIN [8 datoshi]
    /// 0F : JMPIF 03 [2 datoshi]
    /// 11 : THROW [512 datoshi]
    /// 12 : STLOC0 [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : ENDTRY 0F [4 datoshi]
    /// 16 : STLOC0 [2 datoshi]
    /// 17 : PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 22 : THROW [512 datoshi]
    /// 23 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testchar")]
    public abstract BigInteger? Testchar(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeEoQAgAAAQC7JAM6cGjbKEA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : DUP [2 datoshi]
    /// 05 : PUSH0 [1 datoshi]
    /// 06 : PUSHINT32 00000100 [1 datoshi]
    /// 0B : WITHIN [8 datoshi]
    /// 0C : JMPIF 03 [2 datoshi]
    /// 0E : THROW [512 datoshi]
    /// 0F : STLOC0 [2 datoshi]
    /// 10 : LDLOC0 [2 datoshi]
    /// 11 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 13 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testchartostring")]
    public abstract string? Testchartostring(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmfmUA=
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : SUB [8 datoshi]
    /// 06 : SIGN [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCompare")]
    public abstract BigInteger? TestCompare(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmhQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : DIV [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDivide")]
    public abstract BigInteger? TestDivide(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmzQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : NUMEQUAL [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEquals")]
    public abstract bool? TestEquals(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlKU1CiSrEk+kWaQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : DUP [2 datoshi]
    /// 06 : REVERSE3 [2 datoshi]
    /// 07 : SWAP [2 datoshi]
    /// 08 : MOD [8 datoshi]
    /// 09 : DUP [2 datoshi]
    /// 0A : NZ [4 datoshi]
    /// 0B : JMPIF FA [2 datoshi]
    /// 0D : DROP [2 datoshi]
    /// 0E : ABS [4 datoshi]
    /// 0F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGreatestCommonDivisor")]
    public abstract BigInteger? TestGreatestCommonDivisor(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxsAeEoCAAAAgAMAAACAAAAAALskAzpwaD0PcAwJZXhjZXB0aW9uOkA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : TRY 1B00 [4 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : PUSHINT32 00000080 [1 datoshi]
    /// 0D : PUSHINT64 0000008000000000 [1 datoshi]
    /// 16 : WITHIN [8 datoshi]
    /// 17 : JMPIF 03 [2 datoshi]
    /// 19 : THROW [512 datoshi]
    /// 1A : STLOC0 [2 datoshi]
    /// 1B : LDLOC0 [2 datoshi]
    /// 1C : ENDTRY 0F [4 datoshi]
    /// 1E : STLOC0 [2 datoshi]
    /// 1F : PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 2A : THROW [512 datoshi]
    /// 2B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testint")]
    public abstract BigInteger? Testint(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH2 [1 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : NOT [4 datoshi]
    /// 07 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIsEven")]
    public abstract bool? TestIsEven(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBGzQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : PUSH1 [1 datoshi]
    /// 05 : NUMEQUAL [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIsOne")]
    public abstract bool? TestIsOne(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeLGqQA==
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : NZ [4 datoshi]
    /// 05 : NOT [4 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIsZero")]
    public abstract bool? TestIsZero(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOycAeEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALskAzpwaD0PcAwJZXhjZXB0aW9uOkA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : TRY 2700 [4 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : PUSHINT64 0000000000000080 [1 datoshi]
    /// 11 : PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// 22 : WITHIN [8 datoshi]
    /// 23 : JMPIF 03 [2 datoshi]
    /// 25 : THROW [512 datoshi]
    /// 26 : STLOC0 [2 datoshi]
    /// 27 : LDLOC0 [2 datoshi]
    /// 28 : ENDTRY 0F [4 datoshi]
    /// 2A : STLOC0 [2 datoshi]
    /// 2B : PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 36 : THROW [512 datoshi]
    /// 37 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testlong")]
    public abstract BigInteger? Testlong(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAGnATcQAecmhpaqZA
    /// 00 : INITSLOT 0300 [64 datoshi]
    /// 03 : PUSH10 [1 datoshi]
    /// 04 : STLOC0 [2 datoshi]
    /// 05 : PUSH3 [1 datoshi]
    /// 06 : STLOC1 [2 datoshi]
    /// 07 : PUSHINT8 1E [1 datoshi]
    /// 09 : STLOC2 [2 datoshi]
    /// 0A : LDLOC0 [2 datoshi]
    /// 0B : LDLOC1 [2 datoshi]
    /// 0C : LDLOC2 [2 datoshi]
    /// 0D : MODPOW [2048 datoshi]
    /// 0E : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testModPow")]
    public abstract BigInteger? TestModPow();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmgQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : MUL [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultiply")]
    public abstract BigInteger? TestMultiply(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJtA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : NEGATE [4 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNegate")]
    public abstract BigInteger? TestNegate(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmjQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : POW [64 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPow")]
    public abstract BigInteger? TestPow(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmiQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : MOD [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRemainder")]
    public abstract BigInteger? TestRemainder(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxIAeEoAgAGAALskAzpwaD0PcAwJZXhjZXB0aW9uOkA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : TRY 1200 [4 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : PUSHINT8 80 [1 datoshi]
    /// 0A : PUSHINT16 8000 [1 datoshi]
    /// 0D : WITHIN [8 datoshi]
    /// 0E : JMPIF 03 [2 datoshi]
    /// 10 : THROW [512 datoshi]
    /// 11 : STLOC0 [2 datoshi]
    /// 12 : LDLOC0 [2 datoshi]
    /// 13 : ENDTRY 0F [4 datoshi]
    /// 15 : STLOC0 [2 datoshi]
    /// 16 : PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 21 : THROW [512 datoshi]
    /// 22 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testsbyte")]
    public abstract BigInteger? Testsbyte(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxUAeEoBAIACAIAAALskAzpwaD0PcAwJZXhjZXB0aW9uOkA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : TRY 1500 [4 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : PUSHINT16 0080 [1 datoshi]
    /// 0B : PUSHINT32 00800000 [1 datoshi]
    /// 10 : WITHIN [8 datoshi]
    /// 11 : JMPIF 03 [2 datoshi]
    /// 13 : THROW [512 datoshi]
    /// 14 : STLOC0 [2 datoshi]
    /// 15 : LDLOC0 [2 datoshi]
    /// 16 : ENDTRY 0F [4 datoshi]
    /// 18 : STLOC0 [2 datoshi]
    /// 19 : PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 24 : THROW [512 datoshi]
    /// 25 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testshort")]
    public abstract BigInteger? Testshort(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJlA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SIGN [4 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSign")]
    public abstract BigInteger? TestSign(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeKRA
    /// 00 : INITSLOT 0001 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : SQRT [64 datoshi]
    /// 05 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSqrt")]
    public abstract BigInteger? TestSqrt(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmfQA==
    /// 00 : INITSLOT 0002 [64 datoshi]
    /// 03 : LDARG0 [2 datoshi]
    /// 04 : LDARG1 [2 datoshi]
    /// 05 : SUB [8 datoshi]
    /// 06 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSubtract")]
    public abstract BigInteger? TestSubtract(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxcAeEoQAwAAAAABAAAAuyQDOnBoPQ9wDAlleGNlcHRpb246QA==
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : TRY 1700 [4 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : PUSH0 [1 datoshi]
    /// 09 : PUSHINT64 0000000001000000 [1 datoshi]
    /// 12 : WITHIN [8 datoshi]
    /// 13 : JMPIF 03 [2 datoshi]
    /// 15 : THROW [512 datoshi]
    /// 16 : STLOC0 [2 datoshi]
    /// 17 : LDLOC0 [2 datoshi]
    /// 18 : ENDTRY 0F [4 datoshi]
    /// 1A : STLOC0 [2 datoshi]
    /// 1B : PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 26 : THROW [512 datoshi]
    /// 27 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testuint")]
    public abstract BigInteger? Testuint(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOx8AeEoQBAAAAAAAAAAAAQAAAAAAAAC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpA
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : TRY 1F00 [4 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : PUSH0 [1 datoshi]
    /// 09 : PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// 1A : WITHIN [8 datoshi]
    /// 1B : JMPIF 03 [2 datoshi]
    /// 1D : THROW [512 datoshi]
    /// 1E : STLOC0 [2 datoshi]
    /// 1F : LDLOC0 [2 datoshi]
    /// 20 : ENDTRY 0F [4 datoshi]
    /// 22 : STLOC0 [2 datoshi]
    /// 23 : PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 2E : THROW [512 datoshi]
    /// 2F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testulong")]
    public abstract BigInteger? Testulong(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxMAeEoQAgAAAQC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpA
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : TRY 1300 [4 datoshi]
    /// 06 : LDARG0 [2 datoshi]
    /// 07 : DUP [2 datoshi]
    /// 08 : PUSH0 [1 datoshi]
    /// 09 : PUSHINT32 00000100 [1 datoshi]
    /// 0E : WITHIN [8 datoshi]
    /// 0F : JMPIF 03 [2 datoshi]
    /// 11 : THROW [512 datoshi]
    /// 12 : STLOC0 [2 datoshi]
    /// 13 : LDLOC0 [2 datoshi]
    /// 14 : ENDTRY 0F [4 datoshi]
    /// 16 : STLOC0 [2 datoshi]
    /// 17 : PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 22 : THROW [512 datoshi]
    /// 23 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testushort")]
    public abstract BigInteger? Testushort(BigInteger? input);

    #endregion
}
