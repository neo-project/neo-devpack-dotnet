using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_BigInteger(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_BigInteger"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPow"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testSqrt"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":7,""safe"":false},{""name"":""testsbyte"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":13,""safe"":false},{""name"":""testbyte"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":53,""safe"":false},{""name"":""testshort"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":96,""safe"":false},{""name"":""testushort"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":136,""safe"":false},{""name"":""testint"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":181,""safe"":false},{""name"":""testuint"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":221,""safe"":false},{""name"":""testlong"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":270,""safe"":false},{""name"":""testulong"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":310,""safe"":false},{""name"":""testchar"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":367,""safe"":false},{""name"":""testchartostring"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""String"",""offset"":412,""safe"":false},{""name"":""testIsEven"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":520,""safe"":false},{""name"":""testIsZero"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":528,""safe"":false},{""name"":""testIsOne"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":535,""safe"":false},{""name"":""testSign"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":542,""safe"":false},{""name"":""testAdd"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":548,""safe"":false},{""name"":""testSubtract"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":555,""safe"":false},{""name"":""testNegate"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":562,""safe"":false},{""name"":""testMultiply"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":568,""safe"":false},{""name"":""testDivide"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":575,""safe"":false},{""name"":""testRemainder"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":582,""safe"":false},{""name"":""testCompare"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":589,""safe"":false},{""name"":""testGreatestCommonDivisor"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":601,""safe"":false},{""name"":""testEquals"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":618,""safe"":false},{""name"":""parseConstant"",""parameters"":[],""returntype"":""Integer"",""offset"":625,""safe"":false},{""name"":""testModPow"",""parameters"":[],""returntype"":""Integer"",""offset"":643,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2SAlcAAnh5o0BXAAF4pEBXAgFDcTsOAHhKyhEyAzpwaD0WcENpMgVFIvsMCWV4Y2VwdGlvbjpAVwIBQ3E7EQB4ShABAAG7JAM6cGg9FnBDaTIFRSL7DAlleGNlcHRpb246QFcCAUNxOw4AeErKEjIDOnBoPRZwQ2kyBUUi+wwJZXhjZXB0aW9uOkBXAgFDcTsTAHhKEAIAAAEAuyQDOnBoPRZwQ2kyBUUi+wwJZXhjZXB0aW9uOkBXAgFDcTsOAHhKyhQyAzpwaD0WcENpMgVFIvsMCWV4Y2VwdGlvbjpAVwIBQ3E7FwB4ShADAAAAAAEAAAC7JAM6cGg9FnBDaTIFRSL7DAlleGNlcHRpb246QFcCAUNxOw4AeErKGDIDOnBoPRZwQ2kyBUUi+wwJZXhjZXB0aW9uOkBXAgFDcTsfAHhKEAQAAAAAAAAAAAEAAAAAAAAAuyQDOnBoPRZwQ2kyBUUi+wwJZXhjZXB0aW9uOkBXAgFDcTsTAHhKEAIAAAEAuyQDOnBoPRZwQ2kyBUUi+wwJZXhjZXB0aW9uOkBXAgF4ShACAAABALskAzpwaHFpAH8yS2kB/wcyNGkCANgAAAIA4AAAuyQgaQA/kSCoaRapAD+RGKieaRypH5GeAuCAgP+e2yhADAPvv71AaQA/kRioaRapngHAgJ7bKEBpSiQHRQwBAEDbKEBXAAF4EqKqQFcAAXixqkBXAAF4EbNAVwABeJlAVwACeHmeQFcAAnh5n0BXAAF4m0BXAAJ4eaBAVwACeHmhQFcAAnh5okBXAAJ4eUtLt1O3n0BXAAJ4eUomCFBLokok/EWaQFcAAnh5s0AEAAAA5NIMyNzSt1IAAAAAAEBXAwAacBNxAB5yaGlqpkBFjrC1").AsSerializable<Neo.SmartContract.NefFile>();

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
    /// Script: VwACeHlLS7dTt59A
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// OVER [2 datoshi]
    /// OVER [2 datoshi]
    /// GT [8 datoshi]
    /// REVERSE3 [2 datoshi]
    /// GT [8 datoshi]
    /// SUB [8 datoshi]
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
    /// Script: VwACeHlKJghQS6JKJPxFmkA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIFNOT 08 [2 datoshi]
    /// SWAP [2 datoshi]
    /// OVER [2 datoshi]
    /// MOD [8 datoshi]
    /// DUP [2 datoshi]
    /// JMPIF FC [2 datoshi]
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
    /// Script: VwIBQ3E7EQB4ShABAAG7JAM6cGg9FnBDaTIFRSL7DAlleGNlcHRpb246QA==
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
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
    /// ENDTRY 16 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwIBQ3E7EwB4ShACAAABALskAzpwaD0WcENpMgVFIvsMCWV4Y2VwdGlvbjpA
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
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
    /// ENDTRY 16 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwIBeEoQAgAAAQC7JAM6cGhxaQB/MktpAf8HMjRpAgDYAAACAOAAALskIGkAP5EgqGkWqQA/kRionmkcqR+RngLggID/ntsoQAwD77+9QGkAP5EYqGkWqZ4BwICe2yhAaUokB0UMAQBA2yhA
    /// INITSLOT 0201 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PUSHINT32 00000100 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT8 7F [1 datoshi]
    /// JMPLE 4B [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT16 FF07 [1 datoshi]
    /// JMPLE 34 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT32 00D80000 [1 datoshi]
    /// PUSHINT32 00E00000 [1 datoshi]
    /// WITHIN [8 datoshi]
    /// JMPIF 20 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH16 [1 datoshi]
    /// SHL [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH6 [1 datoshi]
    /// SHR [8 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH8 [1 datoshi]
    /// SHL [8 datoshi]
    /// ADD [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH12 [1 datoshi]
    /// SHR [8 datoshi]
    /// PUSH15 [1 datoshi]
    /// AND [8 datoshi]
    /// ADD [8 datoshi]
    /// PUSHINT32 E08080FF [1 datoshi]
    /// ADD [8 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// PUSHDATA1 EFBFBD '???' [8 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHINT8 3F [1 datoshi]
    /// AND [8 datoshi]
    /// PUSH8 [1 datoshi]
    /// SHL [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH6 [1 datoshi]
    /// SHR [8 datoshi]
    /// ADD [8 datoshi]
    /// PUSHINT16 C080 [1 datoshi]
    /// ADD [8 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// LDLOC1 [2 datoshi]
    /// DUP [2 datoshi]
    /// JMPIF 07 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHDATA1 00 [8 datoshi]
    /// RET [0 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testchartostring")]
    public abstract string? Testchartostring(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBQ3E7DgB4SsoUMgM6cGg9FnBDaTIFRSL7DAlleGNlcHRpb246QA==
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0E00 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH4 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 16 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwIBQ3E7DgB4SsoYMgM6cGg9FnBDaTIFRSL7DAlleGNlcHRpb246QA==
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0E00 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH8 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 16 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwIBQ3E7DgB4SsoRMgM6cGg9FnBDaTIFRSL7DAlleGNlcHRpb246QA==
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0E00 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH1 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 16 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwIBQ3E7DgB4SsoSMgM6cGg9FnBDaTIFRSL7DAlleGNlcHRpb246QA==
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0E00 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSH2 [1 datoshi]
    /// JMPLE 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// ENDTRY 16 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwIBQ3E7FwB4ShADAAAAAAEAAAC7JAM6cGg9FnBDaTIFRSL7DAlleGNlcHRpb246QA==
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
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
    /// ENDTRY 16 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwIBQ3E7HwB4ShAEAAAAAAAAAAABAAAAAAAAALskAzpwaD0WcENpMgVFIvsMCWV4Y2VwdGlvbjpA
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
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
    /// ENDTRY 16 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
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
    /// Script: VwIBQ3E7EwB4ShACAAABALskAzpwaD0WcENpMgVFIvsMCWV4Y2VwdGlvbjpA
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
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
    /// ENDTRY 16 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// THROW [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testushort")]
    public abstract BigInteger? Testushort(BigInteger? input);

    #endregion
}
