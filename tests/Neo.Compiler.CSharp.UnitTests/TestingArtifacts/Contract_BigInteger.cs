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
    /// PUSHINT128 000000E4D20CC8DCD2B7520000000000 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("parseConstant")]
    public abstract BigInteger? ParseConstant();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmeQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// ADD [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testAdd")]
    public abstract BigInteger? TestAdd(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxEAeEoQAQABuyQDOnBoPQ9wDAlleGNlcHRpb246QA==
    /// INITSLOT 0101 [64 datoshi]
    /// TRY 1100 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT16 0001 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 0F [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testbyte")]
    public abstract BigInteger? Testbyte(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxMAeEoQAgAAAQC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpA
    /// INITSLOT 0101 [64 datoshi]
    /// TRY 1300 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT32 00000100 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 0F [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testchar")]
    public abstract BigInteger? Testchar(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBeEoQAgAAAQC7JAM6cGjbKEA=
    /// INITSLOT 0101 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT32 00000100 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testchartostring")]
    public abstract string? Testchartostring(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmfmUA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// SUB [8 datoshi]
    /// SIGN [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testCompare")]
    public abstract BigInteger? TestCompare(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmhQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DIV [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDivide")]
    public abstract BigInteger? TestDivide(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmzQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testEquals")]
    public abstract bool? TestEquals(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHlKU1CiSrEk+kWaQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// SWAP [2 datoshi]
    /// MOD [8 datoshi]
    /// DUP [2 datoshi]
    /// NZ [4 datoshi]
    /// JMPIF FA [2 datoshi]
    /// DROP [2 datoshi]
    /// ABS [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGreatestCommonDivisor")]
    public abstract BigInteger? TestGreatestCommonDivisor(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxsAeEoCAAAAgAMAAACAAAAAALskAzpwaD0PcAwJZXhjZXB0aW9uOkA=
    /// INITSLOT 0101 [64 datoshi]
    /// TRY 1B00 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT32 00000080 [1 datoshi]
    /// PUSHINT64 0000008000000000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 0F [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testint")]
    public abstract BigInteger? Testint(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBKiqkA=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// MOD [8 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIsEven")]
    public abstract bool? TestIsEven(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeBGzQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIsOne")]
    public abstract bool? TestIsOne(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeLGqQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// NZ [4 datoshi]
    /// NOT [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIsZero")]
    public abstract bool? TestIsZero(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOycAeEoDAAAAAAAAAIAEAAAAAAAAAIAAAAAAAAAAALskAzpwaD0PcAwJZXhjZXB0aW9uOkA=
    /// INITSLOT 0101 [64 datoshi]
    /// TRY 2700 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT64 0000000000000080 [1 datoshi]
    /// PUSHINT128 00000000000000800000000000000000 [4 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 0F [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testlong")]
    public abstract BigInteger? Testlong(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMAGnATcQAecmhpaqZA
    /// INITSLOT 0300 [64 datoshi]
    /// PUSH10 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSH3 [1 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSHINT8 1E [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// MODPOW [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testModPow")]
    public abstract BigInteger? TestModPow();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmgQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// MUL [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testMultiply")]
    public abstract BigInteger? TestMultiply(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJtA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// NEGATE [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNegate")]
    public abstract BigInteger? TestNegate(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmjQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// POW [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPow")]
    public abstract BigInteger? TestPow(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmiQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// MOD [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testRemainder")]
    public abstract BigInteger? TestRemainder(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxIAeEoAgAGAALskAzpwaD0PcAwJZXhjZXB0aW9uOkA=
    /// INITSLOT 0101 [64 datoshi]
    /// TRY 1200 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT8 80 [1 datoshi]
    /// PUSHINT16 8000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 0F [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testsbyte")]
    public abstract BigInteger? Testsbyte(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxUAeEoBAIACAIAAALskAzpwaD0PcAwJZXhjZXB0aW9uOkA=
    /// INITSLOT 0101 [64 datoshi]
    /// TRY 1500 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSHINT16 0080 [1 datoshi]
    /// PUSHINT32 00800000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 0F [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testshort")]
    public abstract BigInteger? Testshort(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeJlA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SIGN [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSign")]
    public abstract BigInteger? TestSign(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeKRA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// SQRT [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSqrt")]
    public abstract BigInteger? TestSqrt(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmfQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// SUB [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testSubtract")]
    public abstract BigInteger? TestSubtract(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxcAeEoQAwAAAAABAAAAuyQDOnBoPQ9wDAlleGNlcHRpb246QA==
    /// INITSLOT 0101 [64 datoshi]
    /// TRY 1700 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT64 0000000001000000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 0F [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testuint")]
    public abstract BigInteger? Testuint(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOx8AeEoQBAAAAAAAAAAAAQAAAAAAAAC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpA
    /// INITSLOT 0101 [64 datoshi]
    /// TRY 1F00 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT128 00000000000000000100000000000000 [4 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 0F [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testulong")]
    public abstract BigInteger? Testulong(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBOxMAeEoQAgAAAQC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpA
    /// INITSLOT 0101 [64 datoshi]
    /// TRY 1300 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT32 00000100 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 0F [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testushort")]
    public abstract BigInteger? Testushort(BigInteger? input);

    #endregion
}
