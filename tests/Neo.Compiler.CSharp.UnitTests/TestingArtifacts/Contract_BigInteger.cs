using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_BigInteger : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_BigInteger"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPow"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testSqrt"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":9,""safe"":false},{""name"":""testsbyte"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":17,""safe"":false},{""name"":""testbyte"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":52,""safe"":false},{""name"":""testshort"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":86,""safe"":false},{""name"":""testushort"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":124,""safe"":false},{""name"":""testint"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":160,""safe"":false},{""name"":""testuint"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":204,""safe"":false},{""name"":""testlong"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":244,""safe"":false},{""name"":""testulong"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":300,""safe"":false},{""name"":""testchar"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":348,""safe"":false},{""name"":""testchartostring"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""String"",""offset"":384,""safe"":false},{""name"":""testIsEven"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":406,""safe"":false},{""name"":""testIsZero"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":417,""safe"":false},{""name"":""testIsOne"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":426,""safe"":false},{""name"":""testSign"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":435,""safe"":false},{""name"":""testAdd"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":443,""safe"":false},{""name"":""testSubtract"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":452,""safe"":false},{""name"":""testNegate"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":461,""safe"":false},{""name"":""testMultiply"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":469,""safe"":false},{""name"":""testDivide"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":478,""safe"":false},{""name"":""testRemainder"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":487,""safe"":false},{""name"":""testCompare"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":496,""safe"":false},{""name"":""testGreatestCommonDivisor"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":506,""safe"":false},{""name"":""testEquals"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":525,""safe"":false},{""name"":""testModPow"",""parameters"":[],""returntype"":""Integer"",""offset"":534,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0nAlcAAnh5oyICQFcAAXikIgJAVwEBOxIAeEoAgAGAALskAzpwaD0PcAwJZXhjZXB0aW9uOkBXAQE7EQB4ShABAAG7JAM6cGg9D3AMCWV4Y2VwdGlvbjpAVwEBOxUAeEoBAIACAIAAALskAzpwaD0PcAwJZXhjZXB0aW9uOkBXAQE7EwB4ShACAAABALskAzpwaD0PcAwJZXhjZXB0aW9uOkBXAQE7GwB4SgIAAACAAwAAAIAAAAAAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsXAHhKEAMAAAAAAQAAALskAzpwaD0PcAwJZXhjZXB0aW9uOkBXAQE7JwB4SgMAAAAAAAAAgAQAAAAAAAAAgAAAAAAAAAAAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsfAHhKEAQAAAAAAAAAAAEAAAAAAAAAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsTAHhKEAIAAAEAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBAXhKEAIAAAEAuyQDOnBo2ygiAkBXAAF4EZEQsyICQFcAAXgQsyICQFcAAXgRsyICQFcAAXiZIgJAVwACeHmeIgJAVwACeHmfIgJAVwABeJsiAkBXAAJ4eaAiAkBXAAJ4eaEiAkBXAAJ4eaIiAkBXAAJ4eZ+ZIgJAVwACeHlKU1CiShCzJvlFmiICQFcAAnh5syICQFcDABpwE3EAHnJoaWqmIgJAu80PIA=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testAdd")]
    public abstract BigInteger? TestAdd(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testbyte")]
    public abstract BigInteger? Testbyte(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testchar")]
    public abstract BigInteger? Testchar(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testchartostring")]
    public abstract string? Testchartostring(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testCompare")]
    public abstract BigInteger? TestCompare(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDivide")]
    public abstract BigInteger? TestDivide(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testEquals")]
    public abstract bool? TestEquals(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGreatestCommonDivisor")]
    public abstract BigInteger? TestGreatestCommonDivisor(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testint")]
    public abstract BigInteger? Testint(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIsEven")]
    public abstract bool? TestIsEven(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIsOne")]
    public abstract bool? TestIsOne(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIsZero")]
    public abstract bool? TestIsZero(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testlong")]
    public abstract BigInteger? Testlong(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testModPow")]
    public abstract BigInteger? TestModPow();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testMultiply")]
    public abstract BigInteger? TestMultiply(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNegate")]
    public abstract BigInteger? TestNegate(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPow")]
    public abstract BigInteger? TestPow(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testRemainder")]
    public abstract BigInteger? TestRemainder(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testsbyte")]
    public abstract BigInteger? Testsbyte(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testshort")]
    public abstract BigInteger? Testshort(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSign")]
    public abstract BigInteger? TestSign(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSqrt")]
    public abstract BigInteger? TestSqrt(BigInteger? x);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testSubtract")]
    public abstract BigInteger? TestSubtract(BigInteger? x, BigInteger? y);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testuint")]
    public abstract BigInteger? Testuint(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testulong")]
    public abstract BigInteger? Testulong(BigInteger? input);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testushort")]
    public abstract BigInteger? Testushort(BigInteger? input);

    #endregion

    #region Constructor for internal use only

    protected Contract_BigInteger(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
