using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Types : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Types"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""checkBoolString"",""parameters"":[{""name"":""value"",""type"":""Boolean""}],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""checkNull"",""parameters"":[],""returntype"":""Any"",""offset"":24,""safe"":false},{""name"":""checkBoolTrue"",""parameters"":[],""returntype"":""Boolean"",""offset"":28,""safe"":false},{""name"":""checkBoolFalse"",""parameters"":[],""returntype"":""Boolean"",""offset"":34,""safe"":false},{""name"":""checkSbyte"",""parameters"":[],""returntype"":""Integer"",""offset"":40,""safe"":false},{""name"":""checkByte"",""parameters"":[],""returntype"":""Integer"",""offset"":44,""safe"":false},{""name"":""checkShort"",""parameters"":[],""returntype"":""Integer"",""offset"":48,""safe"":false},{""name"":""checkUshort"",""parameters"":[],""returntype"":""Integer"",""offset"":52,""safe"":false},{""name"":""checkInt"",""parameters"":[],""returntype"":""Integer"",""offset"":56,""safe"":false},{""name"":""checkUint"",""parameters"":[],""returntype"":""Integer"",""offset"":60,""safe"":false},{""name"":""checkLong"",""parameters"":[],""returntype"":""Integer"",""offset"":64,""safe"":false},{""name"":""checkUlong"",""parameters"":[],""returntype"":""Integer"",""offset"":68,""safe"":false},{""name"":""checkChar"",""parameters"":[],""returntype"":""Integer"",""offset"":72,""safe"":false},{""name"":""checkString"",""parameters"":[],""returntype"":""String"",""offset"":77,""safe"":false},{""name"":""checkStringIndex"",""parameters"":[{""name"":""input"",""type"":""String""},{""name"":""index"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":85,""safe"":false},{""name"":""checkArrayObj"",""parameters"":[],""returntype"":""Array"",""offset"":92,""safe"":false},{""name"":""checkBigInteger"",""parameters"":[],""returntype"":""Integer"",""offset"":102,""safe"":false},{""name"":""checkByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":106,""safe"":false},{""name"":""checkEnum"",""parameters"":[],""returntype"":""Any"",""offset"":116,""safe"":false},{""name"":""checkEnumArg"",""parameters"":[{""name"":""arg"",""type"":""Integer""}],""returntype"":""Void"",""offset"":120,""safe"":false},{""name"":""checkNameof"",""parameters"":[],""returntype"":""String"",""offset"":124,""safe"":false},{""name"":""checkDelegate"",""parameters"":[],""returntype"":""Any"",""offset"":138,""safe"":false},{""name"":""checkLambda"",""parameters"":[],""returntype"":""Any"",""offset"":146,""safe"":false},{""name"":""checkEvent"",""parameters"":[],""returntype"":""Void"",""offset"":154,""safe"":false},{""name"":""checkClass"",""parameters"":[],""returntype"":""Any"",""offset"":180,""safe"":false},{""name"":""checkStruct"",""parameters"":[],""returntype"":""Any"",""offset"":209,""safe"":false},{""name"":""checkTuple"",""parameters"":[],""returntype"":""Array"",""offset"":239,""safe"":false},{""name"":""checkTuple2"",""parameters"":[],""returntype"":""Array"",""offset"":267,""safe"":false},{""name"":""concatByteString"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":300,""safe"":false},{""name"":""toAddress"",""parameters"":[{""name"":""address"",""type"":""Hash160""},{""name"":""version"",""type"":""Integer""}],""returntype"":""String"",""offset"":319,""safe"":false},{""name"":""call"",""parameters"":[{""name"":""scriptHash"",""type"":""Hash160""},{""name"":""method"",""type"":""String""},{""name"":""flag"",""type"":""Integer""},{""name"":""args"",""type"":""Array""}],""returntype"":""Any"",""offset"":354,""safe"":false},{""name"":""create"",""parameters"":[{""name"":""nef"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Any"",""offset"":369,""safe"":false}],""events"":[{""name"":""DummyEvent"",""parameters"":[{""name"":""msg"",""type"":""String""}]}]},""permissions"":[{""contract"":""0x0102030405060708090a0102030405060708090a"",""methods"":[""testArgs1"",""testVoid""]},{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""base58CheckEncode""]},{""contract"":""0xfffdc93764dbaddd97c48f252a53ea4643faa3fd"",""methods"":[""deploy""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQKCQgHBgUEAwIBCgkIBwYFBAMCAQl0ZXN0QXJnczEBAAEPCgkIBwYFBAMCAQoJCAcGBQQDAgEIdGVzdFZvaWQAAAAPwO85zuDk6SXGwqBqeeFEDdhvzqwRYmFzZTU4Q2hlY2tFbmNvZGUBAAEP/aP6Q0bqUyolj8SX3a3bZDfJ/f8GZGVwbG95AwABDwAA/X8BVwABeCQLDAVGYWxzZSIIDARUcnVlIgJACyICQBHbICICQBDbICICQBUiAkAVIgJAFSICQBUiAkAVIgJAFSICQBUiAkAVIgJAAG4iAkAMA25lbyICQFcAAnh5zkAMA25lbxHAIgJAFSICQAwDAQID2zAiAkAVIgJAVwABQAwJY2hlY2tOdWxsIgJACgAAAAAiAkAKAAAAACICQMJKDANuZW/PDApEdW1teUV2ZW50QZUBb2FAVwEACxHASjQScAwDbmVvSmgQUdBFaCICQFcAAUBXAQDFSgvPSjQScAwDbmVvSmgQUdBFaCICQFcAAUDFSgwDbmVvz0oMDXNtYXJ0IGVjb25vbXnPIgJAVwEAxUoMA25lb89KDA1zbWFydCBlY29ub215z3BoIgJAVwACeHmL2yh4eYvbKIvbKCICQFcAAnl4NAUiAkBXAQIRiEoQedBwaHiLSnBFaNsoNwIAIgJAVwAEe3p5eEFifVtSIgJAVwACC3l42yg3AwAiAkDXzuJi"));

    #endregion

    #region Events

    public delegate void delDummyEvent(string? msg);

    [DisplayName("DummyEvent")]
    public event delDummyEvent? OnDummyEvent;

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("call")]
    public abstract object? Call(UInt160? scriptHash, string? method, BigInteger? flag, IList<object>? args);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkArrayObj")]
    public abstract IList<object>? CheckArrayObj();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkBigInteger")]
    public abstract BigInteger? CheckBigInteger();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkBoolFalse")]
    public abstract bool? CheckBoolFalse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkBoolString")]
    public abstract string? CheckBoolString(bool? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkBoolTrue")]
    public abstract bool? CheckBoolTrue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkByte")]
    public abstract BigInteger? CheckByte();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkByteArray")]
    public abstract byte[]? CheckByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkChar")]
    public abstract BigInteger? CheckChar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkClass")]
    public abstract object? CheckClass();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkDelegate")]
    public abstract object? CheckDelegate();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkEnum")]
    public abstract object? CheckEnum();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkEnumArg")]
    public abstract void CheckEnumArg(BigInteger? arg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkEvent")]
    public abstract void CheckEvent();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkInt")]
    public abstract BigInteger? CheckInt();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkLambda")]
    public abstract object? CheckLambda();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkLong")]
    public abstract BigInteger? CheckLong();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkNameof")]
    public abstract string? CheckNameof();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkNull")]
    public abstract object? CheckNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkSbyte")]
    public abstract BigInteger? CheckSbyte();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkShort")]
    public abstract BigInteger? CheckShort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkString")]
    public abstract string? CheckString();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkStringIndex")]
    public abstract BigInteger? CheckStringIndex(string? input, BigInteger? index);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkStruct")]
    public abstract object? CheckStruct();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkTuple")]
    public abstract IList<object>? CheckTuple();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkTuple2")]
    public abstract IList<object>? CheckTuple2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkUint")]
    public abstract BigInteger? CheckUint();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkUlong")]
    public abstract BigInteger? CheckUlong();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("checkUshort")]
    public abstract BigInteger? CheckUshort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("concatByteString")]
    public abstract string? ConcatByteString(byte[]? a, byte[]? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("create")]
    public abstract object? Create(byte[]? nef, string? manifest);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("toAddress")]
    public abstract string? ToAddress(UInt160? address, BigInteger? version);

    #endregion

    #region Constructor for internal use only

    protected Contract_Types(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
