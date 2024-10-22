using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Types(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Types"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""checkBoolString"",""parameters"":[{""name"":""value"",""type"":""Boolean""}],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""checkNull"",""parameters"":[],""returntype"":""Any"",""offset"":21,""safe"":false},{""name"":""checkBoolTrue"",""parameters"":[],""returntype"":""Boolean"",""offset"":23,""safe"":false},{""name"":""checkBoolFalse"",""parameters"":[],""returntype"":""Boolean"",""offset"":25,""safe"":false},{""name"":""checkSbyte"",""parameters"":[],""returntype"":""Integer"",""offset"":27,""safe"":false},{""name"":""checkByte"",""parameters"":[],""returntype"":""Integer"",""offset"":29,""safe"":false},{""name"":""checkShort"",""parameters"":[],""returntype"":""Integer"",""offset"":31,""safe"":false},{""name"":""checkUshort"",""parameters"":[],""returntype"":""Integer"",""offset"":33,""safe"":false},{""name"":""checkInt"",""parameters"":[],""returntype"":""Integer"",""offset"":35,""safe"":false},{""name"":""checkUint"",""parameters"":[],""returntype"":""Integer"",""offset"":37,""safe"":false},{""name"":""checkLong"",""parameters"":[],""returntype"":""Integer"",""offset"":39,""safe"":false},{""name"":""checkUlong"",""parameters"":[],""returntype"":""Integer"",""offset"":41,""safe"":false},{""name"":""checkChar"",""parameters"":[],""returntype"":""Integer"",""offset"":43,""safe"":false},{""name"":""checkString"",""parameters"":[],""returntype"":""String"",""offset"":46,""safe"":false},{""name"":""checkStringIndex"",""parameters"":[{""name"":""input"",""type"":""String""},{""name"":""index"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":52,""safe"":false},{""name"":""checkArrayObj"",""parameters"":[],""returntype"":""Array"",""offset"":59,""safe"":false},{""name"":""checkBigInteger"",""parameters"":[],""returntype"":""Integer"",""offset"":67,""safe"":false},{""name"":""checkByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":69,""safe"":false},{""name"":""checkEnum"",""parameters"":[],""returntype"":""Any"",""offset"":77,""safe"":false},{""name"":""checkEnumArg"",""parameters"":[{""name"":""arg"",""type"":""Integer""}],""returntype"":""Void"",""offset"":79,""safe"":false},{""name"":""checkNameof"",""parameters"":[],""returntype"":""String"",""offset"":83,""safe"":false},{""name"":""checkDelegate"",""parameters"":[],""returntype"":""Any"",""offset"":95,""safe"":false},{""name"":""checkLambda"",""parameters"":[],""returntype"":""Any"",""offset"":101,""safe"":false},{""name"":""checkEvent"",""parameters"":[],""returntype"":""Void"",""offset"":107,""safe"":false},{""name"":""checkClass"",""parameters"":[],""returntype"":""Any"",""offset"":133,""safe"":false},{""name"":""checkStruct"",""parameters"":[],""returntype"":""Any"",""offset"":164,""safe"":false},{""name"":""checkTuple"",""parameters"":[],""returntype"":""Array"",""offset"":185,""safe"":false},{""name"":""checkTuple2"",""parameters"":[],""returntype"":""Array"",""offset"":211,""safe"":false},{""name"":""concatByteString"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":242,""safe"":false},{""name"":""toAddress"",""parameters"":[{""name"":""address"",""type"":""Hash160""},{""name"":""version"",""type"":""Integer""}],""returntype"":""String"",""offset"":259,""safe"":false},{""name"":""call"",""parameters"":[{""name"":""scriptHash"",""type"":""Hash160""},{""name"":""method"",""type"":""String""},{""name"":""flag"",""type"":""Integer""},{""name"":""args"",""type"":""Array""}],""returntype"":""Any"",""offset"":288,""safe"":false},{""name"":""create"",""parameters"":[{""name"":""nef"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Any"",""offset"":301,""safe"":false}],""events"":[{""name"":""DummyEvent"",""parameters"":[{""name"":""msg"",""type"":""String""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0VuY29kZQEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZkZXBsb3kDAAEPAAD9OQFXAAF4JAoMBUZhbHNlQAwEVHJ1ZUALQAhACUAVQBVAFUAVQBVAFUAVQBVAAG5ADANuZW9AVwACeHnOQAwDbmVvEcBAFUAMAwECA9swQBVAVwABQAwJY2hlY2tOdWxsQAoAAAAAQAoAAAAAQMJKDANuZW/PDApEdW1teUV2ZW50QZUBb2FAVwEACxHASjQQcAwDbmVvSmgQUdBFaEBXAAF4EAvQQFcBAMVKC89wDANuZW9KaBBR0EVoQMVKDANuZW/PSgwNc21hcnQgZWNvbm9tec9AVwEAxUoMA25lb89KDA1zbWFydCBlY29ub215z3BoQFcAAnh5i9soeHmL2yiL2yhAVwACeXg0A0BXAQIRiEoQedBwaHiLcGjbKDcAAEBXAAR7enl4QWJ9W1JAVwACC3l42yg3AQBAOi+aRA=="));

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
    /// <remarks>
    /// Script: VwAEe3p5eEFifVtSQA==
    /// 00 : OpCode.INITSLOT 0004
    /// 03 : OpCode.LDARG3
    /// 04 : OpCode.LDARG2
    /// 05 : OpCode.LDARG1
    /// 06 : OpCode.LDARG0
    /// 07 : OpCode.SYSCALL 627D5B52
    /// 0C : OpCode.RET
    /// </remarks>
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
    /// <remarks>
    /// Script: VwACC3l42yg3AQBA
    /// 00 : OpCode.INITSLOT 0002
    /// 03 : OpCode.PUSHNULL
    /// 04 : OpCode.LDARG1
    /// 05 : OpCode.LDARG0
    /// 06 : OpCode.CONVERT 28
    /// 08 : OpCode.CALLT 0100
    /// 0B : OpCode.RET
    /// </remarks>
    [DisplayName("create")]
    public abstract object? Create(byte[]? nef, string? manifest);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEYhKEHnQcGh4i3Bo2yg3AABA
    /// 00 : OpCode.INITSLOT 0102
    /// 03 : OpCode.PUSH1
    /// 04 : OpCode.NEWBUFFER
    /// 05 : OpCode.DUP
    /// 06 : OpCode.PUSH0
    /// 07 : OpCode.LDARG1
    /// 08 : OpCode.SETITEM
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.LDLOC0
    /// 0B : OpCode.LDARG0
    /// 0C : OpCode.CAT
    /// 0D : OpCode.STLOC0
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.CONVERT 28
    /// 11 : OpCode.CALLT 0000
    /// 14 : OpCode.RET
    /// </remarks>
    [DisplayName("toAddress")]
    public abstract string? ToAddress(UInt160? address, BigInteger? version);

    #endregion
}
