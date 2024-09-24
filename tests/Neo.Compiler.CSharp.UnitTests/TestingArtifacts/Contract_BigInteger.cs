using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_BigInteger(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_BigInteger"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPow"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""testSqrt"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":7,""safe"":false},{""name"":""testsbyte"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":13,""safe"":false},{""name"":""testbyte"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":48,""safe"":false},{""name"":""testshort"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":82,""safe"":false},{""name"":""testushort"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":120,""safe"":false},{""name"":""testint"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":156,""safe"":false},{""name"":""testuint"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":200,""safe"":false},{""name"":""testlong"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":240,""safe"":false},{""name"":""testulong"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":296,""safe"":false},{""name"":""testchar"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":344,""safe"":false},{""name"":""testchartostring"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""String"",""offset"":380,""safe"":false},{""name"":""testIsEven"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":400,""safe"":false},{""name"":""testIsZero"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":408,""safe"":false},{""name"":""testIsOne"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":415,""safe"":false},{""name"":""testSign"",""parameters"":[{""name"":""input"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":422,""safe"":false},{""name"":""testAdd"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":428,""safe"":false},{""name"":""testSubtract"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":435,""safe"":false},{""name"":""testNegate"",""parameters"":[{""name"":""x"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":442,""safe"":false},{""name"":""testMultiply"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":448,""safe"":false},{""name"":""testDivide"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":455,""safe"":false},{""name"":""testRemainder"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":462,""safe"":false},{""name"":""testCompare"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":469,""safe"":false},{""name"":""testGreatestCommonDivisor"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":477,""safe"":false},{""name"":""testEquals"",""parameters"":[{""name"":""x"",""type"":""Integer""},{""name"":""y"",""type"":""Integer""}],""returntype"":""Boolean"",""offset"":494,""safe"":false},{""name"":""parseConstant"",""parameters"":[],""returntype"":""Integer"",""offset"":501,""safe"":false},{""name"":""testModPow"",""parameters"":[],""returntype"":""Integer"",""offset"":519,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP0WAlcAAnh5o0BXAAF4pEBXAQE7EgB4SgCAAYAAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsRAHhKEAEAAbskAzpwaD0PcAwJZXhjZXB0aW9uOkBXAQE7FQB4SgEAgAIAgAAAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsTAHhKEAIAAAEAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsbAHhKAgAAAIADAAAAgAAAAAC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpAVwEBOxcAeEoQAwAAAAABAAAAuyQDOnBoPQ9wDAlleGNlcHRpb246QFcBATsnAHhKAwAAAAAAAACABAAAAAAAAACAAAAAAAAAAAC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpAVwEBOx8AeEoQBAAAAAAAAAAAAQAAAAAAAAC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpAVwEBOxMAeEoQAgAAAQC7JAM6cGg9D3AMCWV4Y2VwdGlvbjpAVwEBeEoQAgAAAQC7JAM6cGjbKEBXAAF4EqKqQFcAAXgQs0BXAAF4EbNAVwABeJlAVwACeHmeQFcAAnh5n0BXAAF4m0BXAAJ4eaBAVwACeHmhQFcAAnh5okBXAAJ4eZ+ZQFcAAnh5SlNQokoQsyb5RZpAVwACeHmzQAQAAADk0gzI3NK3UgAAAAAAQFcDABpwE3EAHnJoaWqmQMZHWLU="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("parseConstant")]
    public abstract BigInteger? ParseConstant();

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

}
