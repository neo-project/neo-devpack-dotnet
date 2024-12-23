using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Types(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Types"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""checkBoolString"",""parameters"":[{""name"":""value"",""type"":""Boolean""}],""returntype"":""String"",""offset"":0,""safe"":false},{""name"":""checkNull"",""parameters"":[],""returntype"":""Any"",""offset"":21,""safe"":false},{""name"":""checkBoolTrue"",""parameters"":[],""returntype"":""Boolean"",""offset"":23,""safe"":false},{""name"":""checkBoolFalse"",""parameters"":[],""returntype"":""Boolean"",""offset"":25,""safe"":false},{""name"":""checkSbyte"",""parameters"":[],""returntype"":""Integer"",""offset"":27,""safe"":false},{""name"":""checkByte"",""parameters"":[],""returntype"":""Integer"",""offset"":29,""safe"":false},{""name"":""checkShort"",""parameters"":[],""returntype"":""Integer"",""offset"":31,""safe"":false},{""name"":""checkUshort"",""parameters"":[],""returntype"":""Integer"",""offset"":33,""safe"":false},{""name"":""checkInt"",""parameters"":[],""returntype"":""Integer"",""offset"":35,""safe"":false},{""name"":""checkUint"",""parameters"":[],""returntype"":""Integer"",""offset"":37,""safe"":false},{""name"":""checkLong"",""parameters"":[],""returntype"":""Integer"",""offset"":39,""safe"":false},{""name"":""checkUlong"",""parameters"":[],""returntype"":""Integer"",""offset"":41,""safe"":false},{""name"":""checkChar"",""parameters"":[],""returntype"":""Integer"",""offset"":43,""safe"":false},{""name"":""checkString"",""parameters"":[],""returntype"":""String"",""offset"":46,""safe"":false},{""name"":""checkStringIndex"",""parameters"":[{""name"":""input"",""type"":""String""},{""name"":""index"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":52,""safe"":false},{""name"":""checkArrayObj"",""parameters"":[],""returntype"":""Array"",""offset"":59,""safe"":false},{""name"":""checkBigInteger"",""parameters"":[],""returntype"":""Integer"",""offset"":67,""safe"":false},{""name"":""checkByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":69,""safe"":false},{""name"":""checkEnum"",""parameters"":[],""returntype"":""Any"",""offset"":77,""safe"":false},{""name"":""checkEnumArg"",""parameters"":[{""name"":""arg"",""type"":""Integer""}],""returntype"":""Void"",""offset"":79,""safe"":false},{""name"":""checkNameof"",""parameters"":[],""returntype"":""String"",""offset"":83,""safe"":false},{""name"":""checkDelegate"",""parameters"":[],""returntype"":""Any"",""offset"":95,""safe"":false},{""name"":""checkLambda"",""parameters"":[],""returntype"":""Any"",""offset"":101,""safe"":false},{""name"":""checkEvent"",""parameters"":[],""returntype"":""Void"",""offset"":107,""safe"":false},{""name"":""checkClass"",""parameters"":[],""returntype"":""Any"",""offset"":132,""safe"":false},{""name"":""checkStruct"",""parameters"":[],""returntype"":""Any"",""offset"":152,""safe"":false},{""name"":""checkTuple"",""parameters"":[],""returntype"":""Array"",""offset"":172,""safe"":false},{""name"":""checkTuple2"",""parameters"":[],""returntype"":""Array"",""offset"":195,""safe"":false},{""name"":""concatByteString"",""parameters"":[{""name"":""a"",""type"":""ByteArray""},{""name"":""b"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":223,""safe"":false},{""name"":""toAddress"",""parameters"":[{""name"":""address"",""type"":""Hash160""},{""name"":""version"",""type"":""Integer""}],""returntype"":""String"",""offset"":240,""safe"":false},{""name"":""call"",""parameters"":[{""name"":""scriptHash"",""type"":""Hash160""},{""name"":""method"",""type"":""String""},{""name"":""flag"",""type"":""Integer""},{""name"":""args"",""type"":""Array""}],""returntype"":""Any"",""offset"":269,""safe"":false},{""name"":""create"",""parameters"":[{""name"":""nef"",""type"":""ByteArray""},{""name"":""manifest"",""type"":""String""}],""returntype"":""Any"",""offset"":282,""safe"":false}],""events"":[{""name"":""DummyEvent"",""parameters"":[{""name"":""msg"",""type"":""String""}]}]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0VuY29kZQEAAQ/9o/pDRupTKiWPxJfdrdtkN8n9/wZkZXBsb3kDAAEPAAD9JgFXAAF4JAoMBUZhbHNlQAwEVHJ1ZUALQAhACUAVQBVAFUAVQBVAFUAVQBVAAG5ADANuZW9AVwACeHnOQAwDbmVvEcBAFUAMAwECA9swQBVAVwABQAwJY2hlY2tOdWxsQAoAAAAAQAoAAAAAQAwDbmVvEcAMCkR1bW15RXZlbnRBlQFvYUBXAQALEcBwDANuZW9KaBBR0EVoQFcBAAsRv3AMA25lb0poEFHQRWhADA1zbWFydCBlY29ub215DANuZW8Sv0BXAQAMDXNtYXJ0IGVjb25vbXkMA25lbxK/cGhAVwACeHmL2yh4eYvbKIvbKEBXAAJ5eDQDQFcBAhGIShB50HBoeItwaNsoNwAAQFcABHt6eXhBYn1bUkBXAAILeXjbKDcBAEBjNc15"));

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
    /// INITSLOT 0004 [64 datoshi]
    /// LDARG3 [2 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SYSCALL 627D5B52 'System.Contract.Call' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("call")]
    public abstract object? Call(UInt160? scriptHash, string? method, BigInteger? flag, IList<object>? args);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DANuZW8RwEA=
    /// PUSHDATA1 6E656F 'neo' [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkArrayObj")]
    public abstract IList<object>? CheckArrayObj();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// PUSH5 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkBigInteger")]
    public abstract BigInteger? CheckBigInteger();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CUA=
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkBoolFalse")]
    public abstract bool? CheckBoolFalse();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeCQKDAVGYWxzZUAMBFRydWVA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// JMPIF 0A [2 datoshi]
    /// PUSHDATA1 46616C7365 'False' [8 datoshi]
    /// RET [0 datoshi]
    /// PUSHDATA1 54727565 'True' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkBoolString")]
    public abstract string? CheckBoolString(bool? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CEA=
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkBoolTrue")]
    public abstract bool? CheckBoolTrue();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// PUSH5 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkByte")]
    public abstract BigInteger? CheckByte();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAMBAgPbMEA=
    /// PUSHDATA1 010203 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkByteArray")]
    public abstract byte[]? CheckByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: AG5A
    /// PUSHINT8 6E [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkChar")]
    public abstract BigInteger? CheckChar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxHAcAwDbmVvSmgQUdBFaEA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 6E656F 'neo' [8 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkClass")]
    public abstract object? CheckClass();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CgAAAABA
    /// PUSHA 00000000 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkDelegate")]
    public abstract object? CheckDelegate();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// PUSH5 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkEnum")]
    public abstract object? CheckEnum();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABQA==
    /// INITSLOT 0001 [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkEnumArg")]
    public abstract void CheckEnumArg(BigInteger? arg);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DANuZW8RwAwKRHVtbXlFdmVudEGVAW9hQA==
    /// PUSHDATA1 6E656F 'neo' [8 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// PUSHDATA1 44756D6D794576656E74 'DummyEvent' [8 datoshi]
    /// SYSCALL 95016F61 'System.Runtime.Notify' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkEvent")]
    public abstract void CheckEvent();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// PUSH5 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkInt")]
    public abstract BigInteger? CheckInt();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: CgAAAABA
    /// PUSHA 00000000 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkLambda")]
    public abstract object? CheckLambda();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// PUSH5 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkLong")]
    public abstract BigInteger? CheckLong();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAljaGVja051bGxA
    /// PUSHDATA1 636865636B4E756C6C 'checkNull' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkNameof")]
    public abstract string? CheckNameof();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: C0A=
    /// PUSHNULL [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkNull")]
    public abstract object? CheckNull();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// PUSH5 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkSbyte")]
    public abstract BigInteger? CheckSbyte();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// PUSH5 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkShort")]
    public abstract BigInteger? CheckShort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DANuZW9A
    /// PUSHDATA1 6E656F 'neo' [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkString")]
    public abstract string? CheckString();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHnOQA==
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkStringIndex")]
    public abstract BigInteger? CheckStringIndex(string? input, BigInteger? index);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEACxG/cAwDbmVvSmgQUdBFaEA=
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 6E656F 'neo' [8 datoshi]
    /// DUP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkStruct")]
    public abstract object? CheckStruct();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DA1zbWFydCBlY29ub215DANuZW8Sv0A=
    /// PUSHDATA1 736D6172742065636F6E6F6D79 [8 datoshi]
    /// PUSHDATA1 6E656F 'neo' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkTuple")]
    public abstract IList<object>? CheckTuple();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEADA1zbWFydCBlY29ub215DANuZW8Sv3BoQA==
    /// INITSLOT 0100 [64 datoshi]
    /// PUSHDATA1 736D6172742065636F6E6F6D79 [8 datoshi]
    /// PUSHDATA1 6E656F 'neo' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACKSTRUCT [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkTuple2")]
    public abstract IList<object>? CheckTuple2();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// PUSH5 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkUint")]
    public abstract BigInteger? CheckUint();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// PUSH5 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkUlong")]
    public abstract BigInteger? CheckUlong();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: FUA=
    /// PUSH5 [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("checkUshort")]
    public abstract BigInteger? CheckUshort();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeHmL2yh4eYvbKIvbKEA=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CAT [2048 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("concatByteString")]
    public abstract string? ConcatByteString(byte[]? a, byte[]? b);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACC3l42yg3AQBA
    /// INITSLOT 0002 [64 datoshi]
    /// PUSHNULL [1 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// CALLT 0100 [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("create")]
    public abstract object? Create(byte[]? nef, string? manifest);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwACeXg0A0A=
    /// INITSLOT 0002 [64 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("toAddress")]
    public abstract string? ToAddress(UInt160? address, BigInteger? version);

    #endregion
}
