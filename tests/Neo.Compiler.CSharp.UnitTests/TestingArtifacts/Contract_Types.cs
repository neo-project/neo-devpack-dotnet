using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Types(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Types"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""checkBoolString"",""parameters"":[{""name"":""value"",""type"":""Boolean""}],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""checkNull"",""parameters"":[],""returntype"":""Any"",""offset"":21,""safe"":false},{""name"":""checkBoolTrue"",""parameters"":[],""returntype"":""Boolean"",""offset"":23,""safe"":false},{""name"":""checkBoolFalse"",""parameters"":[],""returntype"":""Boolean"",""offset"":25,""safe"":false},{""name"":""checkSbyte"",""parameters"":[],""returntype"":""Integer"",""offset"":27,""safe"":false},{""name"":""checkByte"",""parameters"":[],""returntype"":""Integer"",""offset"":29,""safe"":false},{""name"":""checkShort"",""parameters"":[],""returntype"":""Integer"",""offset"":31,""safe"":false},{""name"":""checkUshort"",""parameters"":[],""returntype"":""Integer"",""offset"":33,""safe"":false},{""name"":""checkInt"",""parameters"":[],""returntype"":""Integer"",""offset"":35,""safe"":false},{""name"":""checkUint"",""parameters"":[],""returntype"":""Integer"",""offset"":37,""safe"":false},{""name"":""checkLong"",""parameters"":[],""returntype"":""Integer"",""offset"":39,""safe"":false},{""name"":""checkUlong"",""parameters"":[],""returntype"":""Integer"",""offset"":41,""safe"":false},{""name"":""checkChar"",""parameters"":[],""returntype"":""Integer"",""offset"":43,""safe"":false},{""name"":""checkString"",""parameters"":[],""returntype"":""String"",""offset"":46,""safe"":false},{""name"":""checkStringIndex"",""parameters"":[{""name"":""input"",""type"":""String""},{""name"":""index"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":52,""safe"":false},{""name"":""checkArrayObj"",""parameters"":[],""returntype"":""Array"",""offset"":59,""safe"":false},{""name"":""checkBigInteger"",""parameters"":[],""returntype"":""Integer"",""offset"":67,""safe"":false},{""name"":""checkByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":69,""safe"":false},{""name"":""checkEnum"",""parameters"":[],""returntype"":""Any"",""offset"":77,""safe"":false},{""name"":""checkEnumArg"",""parameters"":[{""name"":""arg"",""type"":""Integer""}],""returntype"":""Void"",""offset"":79,""safe"":false},{""name"":""checkNameof"",""parameters"":[],""returntype"":""String"",""offset"":83,""safe"":false},{""name"":""checkDelegate"",""parameters"":[],""returntype"":""Any"",""offset"":95,""safe"":false},{""name"":""checkLambda"",""parameters"":[],""returntype"":""Any"",""offset"":101,""safe"":false},{""name"":""checkEvent"",""parameters"":[],""returntype"":""Void"",""offset"":107,""safe"":false},{""name"":""checkClass"",""parameters"":[],""returntype"":""Any"",""offset"":132,""safe"":false},{""name"":""checkStruct"",""parameters"":[],""returntype"":""Any"",""offset"":163,""safe"":false},{""name"":""checkTuple"",""parameters"":[],""returntype"":""Array"",""offset"":183,""safe"":false},{""name"":""checkTuple2"",""parameters"":[],""returntype"":""Array"",""offset"":206,""safe"":false},{""name"":""concatByteString"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":234,""safe"":false},{""name"":""toAddress"",""parameters"":[{""name"":""address"",""type"":""Hash160""},{""name"":""version"",""type"":""Integer""}],""returntype"":""String"",""offset"":251,""safe"":false},{""name"":""call"",""parameters"":[{""name"":""scriptHash"",""type"":""Hash160""},{""name"":""method"",""type"":""String""},{""name"":""flag"",""type"":""Integer""},{""name"":""args"",""type"":""Array""}],""returntype"":""Any"",""offset"":280,""safe"":false},{""name"":""create"",""parameters"":[{""name"":""nef"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Any"",""offset"":293,""safe"":false}],""events"":[{""name"":""DummyEvent"",""parameters"":[{""name"":""msg"",""type"":""String""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0VuY29kZQEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZkZXBsb3kDAAEPAAD9MQFXAAF4JAoMBUZhbHNlQAwEVHJ1ZUALQAhACUAVQBVAFUAVQBVAFUAVQBVAAG5ADANuZW9AVwACeHnOQAwDbmVvEcBAFUAMAwECA9swQBVAVwABQAwJY2hlY2tOdWxsQAoAAAAAQAoAAAAAQAwDbmVvEcAMCkR1bW15RXZlbnRBlQFvYUBXAQALEcBKNBBwDANuZW9KaBBR0EVoQFcAAXgQC9BAVwEACxG/cAwDbmVvSmgQUdBFaEAMDXNtYXJ0IGVjb25vbXkMA25lbxK/QFcBAAwNc21hcnQgZWNvbm9teQwDbmVvEr9waEBXAAJ4eYvbKHh5i9soi9soQFcAAnl4NANAVwECEYhKEHnQcGh4i3Bo2yg3AABAVwAEe3p5eEFifVtSQFcAAgt5eNsoNwEAQMI69Ac="));

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
    /// 00 : OpCode.INITSLOT 0004 [64 datoshi]
    /// 03 : OpCode.LDARG3 [2 datoshi]
    /// 04 : OpCode.LDARG2 [2 datoshi]
    /// 05 : OpCode.LDARG1 [2 datoshi]
    /// 06 : OpCode.LDARG0 [2 datoshi]
    /// 07 : OpCode.SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// 0C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("call")]
    public abstract object? Call(UInt160? scriptHash, string? method, BigInteger? flag, IList<object>? args);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DANuZW8RwEA=
    /// 00 : OpCode.PUSHDATA1 6E656F [8 datoshi]
    /// 05 : OpCode.PUSH1 [1 datoshi]
    /// 06 : OpCode.PACK [2048 datoshi]
    /// 07 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkArrayObj")]
    public abstract IList<object>? CheckArrayObj();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// 00 : OpCode.PUSH5 [1 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkBigInteger")]
    public abstract BigInteger? CheckBigInteger();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CUA=
    /// 00 : OpCode.PUSHF [1 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkBoolFalse")]
    public abstract bool? CheckBoolFalse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCQKDAVGYWxzZUAMBFRydWVA
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.JMPIF 0A [2 datoshi]
    /// 06 : OpCode.PUSHDATA1 46616C7365 [8 datoshi]
    /// 0D : OpCode.RET [0 datoshi]
    /// 0E : OpCode.PUSHDATA1 54727565 [8 datoshi]
    /// 14 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkBoolString")]
    public abstract string? CheckBoolString(bool? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CEA=
    /// 00 : OpCode.PUSHT [1 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkBoolTrue")]
    public abstract bool? CheckBoolTrue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// 00 : OpCode.PUSH5 [1 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkByte")]
    public abstract BigInteger? CheckByte();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAMBAgPbMEA=
    /// 00 : OpCode.PUSHDATA1 010203 [8 datoshi]
    /// 05 : OpCode.CONVERT 30 'Buffer' [8192 datoshi]
    /// 07 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkByteArray")]
    public abstract byte[]? CheckByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AG5A
    /// 00 : OpCode.PUSHINT8 6E [1 datoshi]
    /// 02 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkChar")]
    public abstract BigInteger? CheckChar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxHASjQQcAwDbmVvSmgQUdBFaEA=
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.PUSH1 [1 datoshi]
    /// 05 : OpCode.PACK [2048 datoshi]
    /// 06 : OpCode.DUP [2 datoshi]
    /// 07 : OpCode.CALL 10 [512 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.PUSHDATA1 6E656F [8 datoshi]
    /// 0F : OpCode.DUP [2 datoshi]
    /// 10 : OpCode.LDLOC0 [2 datoshi]
    /// 11 : OpCode.PUSH0 [1 datoshi]
    /// 12 : OpCode.ROT [2 datoshi]
    /// 13 : OpCode.SETITEM [8192 datoshi]
    /// 14 : OpCode.DROP [2 datoshi]
    /// 15 : OpCode.LDLOC0 [2 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkClass")]
    public abstract object? CheckClass();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CgAAAABA
    /// 00 : OpCode.PUSHA 00000000 [4 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkDelegate")]
    public abstract object? CheckDelegate();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// 00 : OpCode.PUSH5 [1 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkEnum")]
    public abstract object? CheckEnum();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABQA==
    /// 00 : OpCode.INITSLOT 0001 [64 datoshi]
    /// 03 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkEnumArg")]
    public abstract void CheckEnumArg(BigInteger? arg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DANuZW8RwAwKRHVtbXlFdmVudEGVAW9hQA==
    /// 00 : OpCode.PUSHDATA1 6E656F [8 datoshi]
    /// 05 : OpCode.PUSH1 [1 datoshi]
    /// 06 : OpCode.PACK [2048 datoshi]
    /// 07 : OpCode.PUSHDATA1 44756D6D794576656E74 [8 datoshi]
    /// 13 : OpCode.SYSCALL 95016F61 'System.Runtime.Notify' [32768 datoshi]
    /// 18 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkEvent")]
    public abstract void CheckEvent();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// 00 : OpCode.PUSH5 [1 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkInt")]
    public abstract BigInteger? CheckInt();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CgAAAABA
    /// 00 : OpCode.PUSHA 00000000 [4 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkLambda")]
    public abstract object? CheckLambda();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// 00 : OpCode.PUSH5 [1 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkLong")]
    public abstract BigInteger? CheckLong();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAljaGVja051bGxA
    /// 00 : OpCode.PUSHDATA1 636865636B4E756C6C [8 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkNameof")]
    public abstract string? CheckNameof();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: C0A=
    /// 00 : OpCode.PUSHNULL [1 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkNull")]
    public abstract object? CheckNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// 00 : OpCode.PUSH5 [1 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkSbyte")]
    public abstract BigInteger? CheckSbyte();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// 00 : OpCode.PUSH5 [1 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkShort")]
    public abstract BigInteger? CheckShort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DANuZW9A
    /// 00 : OpCode.PUSHDATA1 6E656F [8 datoshi]
    /// 05 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkString")]
    public abstract string? CheckString();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHnOQA==
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.PICKITEM [64 datoshi]
    /// 06 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkStringIndex")]
    public abstract BigInteger? CheckStringIndex(string? input, BigInteger? index);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxG/cAwDbmVvSmgQUdBFaEA=
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.PUSH1 [1 datoshi]
    /// 05 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 06 : OpCode.STLOC0 [2 datoshi]
    /// 07 : OpCode.PUSHDATA1 6E656F [8 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.LDLOC0 [2 datoshi]
    /// 0E : OpCode.PUSH0 [1 datoshi]
    /// 0F : OpCode.ROT [2 datoshi]
    /// 10 : OpCode.SETITEM [8192 datoshi]
    /// 11 : OpCode.DROP [2 datoshi]
    /// 12 : OpCode.LDLOC0 [2 datoshi]
    /// 13 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkStruct")]
    public abstract object? CheckStruct();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DA1zbWFydCBlY29ub215DANuZW8Sv0A=
    /// 00 : OpCode.PUSHDATA1 736D6172742065636F6E6F6D79 [8 datoshi]
    /// 0F : OpCode.PUSHDATA1 6E656F [8 datoshi]
    /// 14 : OpCode.PUSH2 [1 datoshi]
    /// 15 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 16 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkTuple")]
    public abstract IList<object>? CheckTuple();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADA1zbWFydCBlY29ub215DANuZW8Sv3BoQA==
    /// 00 : OpCode.INITSLOT 0100 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 736D6172742065636F6E6F6D79 [8 datoshi]
    /// 12 : OpCode.PUSHDATA1 6E656F [8 datoshi]
    /// 17 : OpCode.PUSH2 [1 datoshi]
    /// 18 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 19 : OpCode.STLOC0 [2 datoshi]
    /// 1A : OpCode.LDLOC0 [2 datoshi]
    /// 1B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkTuple2")]
    public abstract IList<object>? CheckTuple2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// 00 : OpCode.PUSH5 [1 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkUint")]
    public abstract BigInteger? CheckUint();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// 00 : OpCode.PUSH5 [1 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkUlong")]
    public abstract BigInteger? CheckUlong();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// 00 : OpCode.PUSH5 [1 datoshi]
    /// 01 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkUshort")]
    public abstract BigInteger? CheckUshort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2yh4eYvbKIvbKEA=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG0 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.CAT [2048 datoshi]
    /// 06 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 08 : OpCode.LDARG0 [2 datoshi]
    /// 09 : OpCode.LDARG1 [2 datoshi]
    /// 0A : OpCode.CAT [2048 datoshi]
    /// 0B : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 0D : OpCode.CAT [2048 datoshi]
    /// 0E : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 10 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("concatByteString")]
    public abstract string? ConcatByteString(byte[]? a, byte[]? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACC3l42yg3AQBA
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.PUSHNULL [1 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 08 : OpCode.CALLT 0100 [32768 datoshi]
    /// 0B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("create")]
    public abstract object? Create(byte[]? nef, string? manifest);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0A0A=
    /// 00 : OpCode.INITSLOT 0002 [64 datoshi]
    /// 03 : OpCode.LDARG1 [2 datoshi]
    /// 04 : OpCode.LDARG0 [2 datoshi]
    /// 05 : OpCode.CALL 03 [512 datoshi]
    /// 07 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("toAddress")]
    public abstract string? ToAddress(UInt160? address, BigInteger? version);

    #endregion
}
