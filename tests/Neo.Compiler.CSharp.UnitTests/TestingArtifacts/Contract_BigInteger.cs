using Neo.Cryptography.ECC;
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
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0VAlcAAnh5o0BXAAF4pEBXAQE7EgB4SgCAAYAAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsRAHhKEAEAAbskAzpwaD0PcAwJZXhjZXB0aW9uOkBXAQE7FQB4SgEAgAIAgAAAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsTAHhKEAIAAAEAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsbAHhKAgAAAIADAAAAgAAAAAC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpAVwEBOxcAeEoQAwAAAAABAAAAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsnAHhKAwAAAAAAAACABAAAAAAAAACAAAAAAAAAAAC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpAVwEBOx8AeEoQBAAAAAAAAAAAAQAAAAAAAAC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpAVwEBOxMAeEoQAgAAAQC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpAVwEBeEoQAgAAAQC7JAM6cGjbKEBXAAF4EqKqQFcAAXixqkBXAAF4EbNAVwABeJlAVwACeHmeQFcAAnh5n0BXAAF4m0BXAAJ4eaBAVwACeHmhQFcAAnh5okBXAAJ4eZ+ZQFcAAnh5SlNQokqxJPpFmkBXAAJ4ebNABAAAAOTSDMjc0rdSAAAAAABAVwMAGnATcQAecmhpaqZAWB31KQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: BAAAAOTSDMjc0rdSAAAAAABA
    /// 00 : OpCode.PUSHINT128 000000E4D20CC8DCD2B7520000000000 [4 datoshi]
    /// 11 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("parseConstant")]
    public abstract BigInteger? ParseConstant();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.ADD [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAdd")]
    public abstract BigInteger? TestAdd(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxEAeEoQAQABuyQDOnBoPQ9wDAlleGNlcHRpb246QA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.TRY 1100 [4 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSH0 [1 datoshi]
    /// 09 : OpCode.PUSHINT16 0001 [1 datoshi]
    /// 0C : OpCode.WITHIN [8 datoshi]
    /// 0D : OpCode.JMPIF 03 [2 datoshi]
    /// 0F : OpCode.THROW [512 datoshi]
    /// 10 : OpCode.STLOC0 [2 datoshi]
    /// 11 : OpCode.LDLOC0 [2 datoshi]
    /// 12 : OpCode.ENDTRY 0F [4 datoshi]
    /// 14 : OpCode.STLOC0 [2 datoshi]
    /// 15 : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 20 : OpCode.THROW [512 datoshi]
    /// 21 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testbyte")]
    public abstract BigInteger? Testbyte(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxMAeEoQAgAAAQC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpA
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.TRY 1300 [4 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSH0 [1 datoshi]
    /// 09 : OpCode.PUSHINT32 00000100 [1 datoshi]
    /// 0E : OpCode.WITHIN [8 datoshi]
    /// 0F : OpCode.JMPIF 03 [2 datoshi]
    /// 11 : OpCode.THROW [512 datoshi]
    /// 12 : OpCode.STLOC0 [2 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.ENDTRY 0F [4 datoshi]
    /// 16 : OpCode.STLOC0 [2 datoshi]
    /// 17 : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 22 : OpCode.THROW [512 datoshi]
    /// 23 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testchar")]
    public abstract BigInteger? Testchar(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeEoQAgAAAQC7JAM6cGjbKEA=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.DUP [2 datoshi]
    /// 05 : OpCode.PUSH0 [1 datoshi]
    /// 06 : OpCode.PUSHINT32 00000100 [1 datoshi]
    /// 0B : OpCode.WITHIN [8 datoshi]
    /// 0C : OpCode.JMPIF 03 [2 datoshi]
    /// 0E : OpCode.THROW [512 datoshi]
    /// 0F : OpCode.STLOC0 [2 datoshi]
    /// 10 : OpCode.LDLOC0 [2 datoshi]
    /// 11 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 13 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testchartostring")]
    public abstract string? Testchartostring(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmfmUA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.SUB [8 datoshi]
    /// 06 : OpCode.SIGN [4 datoshi]
    /// 07 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCompare")]
    public abstract BigInteger? TestCompare(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmhQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.DIV [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDivide")]
    public abstract BigInteger? TestDivide(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmzQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.NUMEQUAL [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEquals")]
    public abstract bool? TestEquals(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlKU1CiSrEk+kWaQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.DUP [2 datoshi]
    /// 06 : OpCode.REVERSE3 [2 datoshi]
    /// 07 : OpCode.SWAP [2 datoshi]
    /// 08 : OpCode.MOD [8 datoshi]
    /// 09 : OpCode.DUP [2 datoshi]
    /// 0A : OpCode.NZ [4 datoshi]
    /// 0B : OpCode.JMPIF FA [2 datoshi]
    /// 0D : OpCode.DROP [2 datoshi]
    /// 0E : OpCode.ABS [4 datoshi]
    /// 0F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGreatestCommonDivisor")]
    public abstract BigInteger? TestGreatestCommonDivisor(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxsAeEoCAAAAgAMAAACAAAAAALskAzpwaD0PcAwJZXhjZXB0aW9uOkA=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.TRY 1B00 [4 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSHINT32 00000080 [1 datoshi]
    /// 0D : OpCode.PUSHINT64 0000008000000000 [1 datoshi]
    /// 16 : OpCode.WITHIN [8 datoshi]
    /// 17 : OpCode.JMPIF 03 [2 datoshi]
    /// 19 : OpCode.THROW [512 datoshi]
    /// 1A : OpCode.STLOC0 [2 datoshi]
    /// 1B : OpCode.LDLOC0 [2 datoshi]
    /// 1C : OpCode.ENDTRY 0F [4 datoshi]
    /// 1E : OpCode.STLOC0 [2 datoshi]
    /// 1F : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 2A : OpCode.THROW [512 datoshi]
    /// 2B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testint")]
    public abstract BigInteger? Testint(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSH2 [1 datoshi]
    /// 05 : OpCode.MOD [8 datoshi]
    /// 06 : OpCode.NOT [4 datoshi]
    /// 07 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIsEven")]
    public abstract bool? TestIsEven(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBGzQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.PUSH1 [1 datoshi]
    /// 05 : OpCode.NUMEQUAL [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIsOne")]
    public abstract bool? TestIsOne(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeLGqQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.NZ [4 datoshi]
    /// 05 : OpCode.NOT [4 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIsZero")]
    public abstract bool? TestIsZero(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOycAeEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALskAzpwaD0PcAwJZXhjZXB0aW9uOkA=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.TRY 2700 [4 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSHINT64 0000000000000080 [1 datoshi]
    /// 11 : OpCode.PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// 22 : OpCode.WITHIN [8 datoshi]
    /// 23 : OpCode.JMPIF 03 [2 datoshi]
    /// 25 : OpCode.THROW [512 datoshi]
    /// 26 : OpCode.STLOC0 [2 datoshi]
    /// 27 : OpCode.LDLOC0 [2 datoshi]
    /// 28 : OpCode.ENDTRY 0F [4 datoshi]
    /// 2A : OpCode.STLOC0 [2 datoshi]
    /// 2B : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 36 : OpCode.THROW [512 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testlong")]
    public abstract BigInteger? Testlong(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAGnATcQAecmhpaqZA
    /// 00 : OpCode.INITSLOT 0300 [64 datoshi]
    /// 03 : OpCode.PUSH10 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSH3 [1 datoshi]
    /// 06 : OpCode.STLOC1 [2 datoshi]
    /// 07 : OpCode.PUSHINT8 1E [1 datoshi]
    /// 09 : OpCode.STLOC2 [2 datoshi]
    /// 0A : OpCode.LDLOC0 [2 datoshi]
    /// 0B : OpCode.LDLOC1 [2 datoshi]
    /// 0C : OpCode.LDLOC2 [2 datoshi]
    /// 0D : OpCode.MODPOW [2048 datoshi]
    /// 0E : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testModPow")]
    public abstract BigInteger? TestModPow();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmgQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.MUL [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultiply")]
    public abstract BigInteger? TestMultiply(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJtA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.NEGATE [4 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNegate")]
    public abstract BigInteger? TestNegate(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmjQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.POW [64 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPow")]
    public abstract BigInteger? TestPow(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmiQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.MOD [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRemainder")]
    public abstract BigInteger? TestRemainder(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxIAeEoAgAGAALskAzpwaD0PcAwJZXhjZXB0aW9uOkA=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.TRY 1200 [4 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSHINT8 80 [1 datoshi]
    /// 0A : OpCode.PUSHINT16 8000 [1 datoshi]
    /// 0D : OpCode.WITHIN [8 datoshi]
    /// 0E : OpCode.JMPIF 03 [2 datoshi]
    /// 10 : OpCode.THROW [512 datoshi]
    /// 11 : OpCode.STLOC0 [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.ENDTRY 0F [4 datoshi]
    /// 15 : OpCode.STLOC0 [2 datoshi]
    /// 16 : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 21 : OpCode.THROW [512 datoshi]
    /// 22 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testsbyte")]
    public abstract BigInteger? Testsbyte(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxUAeEoBAIACAIAAALskAzpwaD0PcAwJZXhjZXB0aW9uOkA=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.TRY 1500 [4 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSHINT16 0080 [1 datoshi]
    /// 0B : OpCode.PUSHINT32 00800000 [1 datoshi]
    /// 10 : OpCode.WITHIN [8 datoshi]
    /// 11 : OpCode.JMPIF 03 [2 datoshi]
    /// 13 : OpCode.THROW [512 datoshi]
    /// 14 : OpCode.STLOC0 [2 datoshi]
    /// 15 : OpCode.LDLOC0 [2 datoshi]
    /// 16 : OpCode.ENDTRY 0F [4 datoshi]
    /// 18 : OpCode.STLOC0 [2 datoshi]
    /// 19 : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 24 : OpCode.THROW [512 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testshort")]
    public abstract BigInteger? Testshort(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJlA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.SIGN [4 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSign")]
    public abstract BigInteger? TestSign(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeKRA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.SQRT [64 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSqrt")]
    public abstract BigInteger? TestSqrt(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmfQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.SUB [8 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSubtract")]
    public abstract BigInteger? TestSubtract(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxcAeEoQAwAAAAABAAAAuyQDOnBoPQ9wDAlleGNlcHRpb246QA==
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.TRY 1700 [4 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSH0 [1 datoshi]
    /// 09 : OpCode.PUSHINT64 0000000001000000 [1 datoshi]
    /// 12 : OpCode.WITHIN [8 datoshi]
    /// 13 : OpCode.JMPIF 03 [2 datoshi]
    /// 15 : OpCode.THROW [512 datoshi]
    /// 16 : OpCode.STLOC0 [2 datoshi]
    /// 17 : OpCode.LDLOC0 [2 datoshi]
    /// 18 : OpCode.ENDTRY 0F [4 datoshi]
    /// 1A : OpCode.STLOC0 [2 datoshi]
    /// 1B : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 26 : OpCode.THROW [512 datoshi]
    /// 27 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testuint")]
    public abstract BigInteger? Testuint(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOx8AeEoQBAAAAAAAAAAAAQAAAAAAAAC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpA
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.TRY 1F00 [4 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSH0 [1 datoshi]
    /// 09 : OpCode.PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// 1A : OpCode.WITHIN [8 datoshi]
    /// 1B : OpCode.JMPIF 03 [2 datoshi]
    /// 1D : OpCode.THROW [512 datoshi]
    /// 1E : OpCode.STLOC0 [2 datoshi]
    /// 1F : OpCode.LDLOC0 [2 datoshi]
    /// 20 : OpCode.ENDTRY 0F [4 datoshi]
    /// 22 : OpCode.STLOC0 [2 datoshi]
    /// 23 : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 2E : OpCode.THROW [512 datoshi]
    /// 2F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testulong")]
    public abstract BigInteger? Testulong(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxMAeEoQAgAAAQC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpA
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.TRY 1300 [4 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.DUP [2 datoshi]
    /// 08 : OpCode.PUSH0 [1 datoshi]
    /// 09 : OpCode.PUSHINT32 00000100 [1 datoshi]
    /// 0E : OpCode.WITHIN [8 datoshi]
    /// 0F : OpCode.JMPIF 03 [2 datoshi]
    /// 11 : OpCode.THROW [512 datoshi]
    /// 12 : OpCode.STLOC0 [2 datoshi]
    /// 13 : OpCode.LDLOC0 [2 datoshi]
    /// 14 : OpCode.ENDTRY 0F [4 datoshi]
    /// 16 : OpCode.STLOC0 [2 datoshi]
    /// 17 : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 22 : OpCode.THROW [512 datoshi]
    /// 23 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testushort")]
    public abstract BigInteger? Testushort(BigInteger? input);

    #endregion
}
