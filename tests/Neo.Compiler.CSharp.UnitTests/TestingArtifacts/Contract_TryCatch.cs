using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_TryCatch(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_TryCatch"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""try01"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":0,""safe"":false},{""name"":""try02"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":46,""safe"":false},{""name"":""try03"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":55,""safe"":false},{""name"":""tryNest"",""parameters"":[{""name"":""throwInTry"",""type"":""Boolean""},{""name"":""throwInCatch"",""type"":""Boolean""},{""name"":""throwInFinally"",""type"":""Boolean""},{""name"":""enterOuterCatch"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":104,""safe"":false},{""name"":""throwInCatch"",""parameters"":[{""name"":""throwInTry"",""type"":""Boolean""},{""name"":""throwInCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":162,""safe"":false},{""name"":""tryFinally"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":219,""safe"":false},{""name"":""tryFinallyAndRethrow"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":257,""safe"":false},{""name"":""tryCatch"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":289,""safe"":false},{""name"":""tryWithTwoFinally"",""parameters"":[{""name"":""throwInInner"",""type"":""Boolean""},{""name"":""throwInOuter"",""type"":""Boolean""},{""name"":""enterInnerCatch"",""type"":""Boolean""},{""name"":""enterOuterCatch"",""type"":""Boolean""},{""name"":""enterInnerFinally"",""type"":""Boolean""},{""name"":""enterOuterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":323,""safe"":false},{""name"":""tryecpointCast"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":411,""safe"":false},{""name"":""tryvalidByteString2Ecpoint"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":502,""safe"":false},{""name"":""tryinvalidByteArray2UInt160"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":580,""safe"":false},{""name"":""tryvalidByteArray2UInt160"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":658,""safe"":false},{""name"":""tryinvalidByteArray2UInt256"",""parameters"":[{""name"":""useInvalidECpoint"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":723,""safe"":false},{""name"":""tryvalidByteArray2UInt256"",""parameters"":[{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":811,""safe"":false},{""name"":""tryNULL2Ecpoint_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":877,""safe"":false},{""name"":""tryNULL2Uint160_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":944,""safe"":false},{""name"":""tryNULL2Uint256_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":1011,""safe"":false},{""name"":""tryNULL2Bytestring_1"",""parameters"":[{""name"":""setToNull"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":1078,""safe"":false},{""name"":""throwCall"",""parameters"":[],""returntype"":""Any"",""offset"":92,""safe"":false},{""name"":""tryUncatchableException"",""parameters"":[{""name"":""throwException"",""type"":""Boolean""},{""name"":""enterCatch"",""type"":""Boolean""},{""name"":""enterFinally"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":1134,""safe"":false}],""events"":[]},""permissions"":[],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP2RBFcCAxBwOxYeEnB4Jg4MCWV4Y2VwdGlvbjo9E3F5JgQTcD0LeiYHaEqccEU/aEBXAAN6eXg0zEBXAgMQcDsNFRJweCYFNBhFPRNxeSYEE3A9C3omB2hKnHBFP2hADAlleGNlcHRpb246VwIEEHA7KAA7DRgScHgmBTTkRT0YcRNweSYFNNlFPQ16JgQ00WhKnHBFPz0NcXsmB2hKnHBFPQJoQFcCAxBwOxYqEXB4Jg4MCWV4Y2VwdGlvbjo9HHEScHkmDgwJZXhjZXB0aW9uOj0IeiYEE3A/FHBoQFcBAhBwOwAWEnB4Jg4MCWV4Y2VwdGlvbjo9C3kmB2hKnHBFP2hAVwECEHA7ABAScHgmCDVO////RT0LeSYHaEqccEU/aEBXAgIQcDsQABJweCYINS7///9FPQ1xeSYHaEqccEU9AmhAVwIGEHA7P0k7GSNoSpxwRXgmDgwJZXhjZXB0aW9uOj0UcXomBmgSnnA9CnwmBmgTnnA/eSYODAlleGNlcHRpb246PRRxeyYGaBSecD0KfSYGaBWecD9oQFcCAxBwO0NLEnB4JgwMBgoLDA0ODyIlDCECRwDbLpDZ8CxPn8hiq6ypJyX5W0/dzI1/+lOGk+z0Y6lK2CQJSsoAISgDOnE9E3F5JgQTcD0LeiYHaEqccEU/aEBXAgIQcDs2PhJwDCECRwDbLpDZ8CxPn8hiq6ypJyX5W0/dzI1/+lOGk+z0Y6lK2CQJSsoAISgDOnE9E3F4JgQTcD0LeSYHaEqccEU/aEBXAgMQcDs2PhJweCYMDAYKCwwNDg8iGAwUfu4aq+tn7R15HUTk9fzzrpFxqHFK2CQJSsoAFCgDOnE9E3F5JgQTcD0LeiYHaEqccEU/aEBXAgIQcDspMRJwDBR+7hqr62ftHXkdROT1/POukXGocUrYJAlKygAUKAM6cT0TcXgmBBNwPQt5JgdoSpxwRT9oQFcCAxBwO0BIEnB4JhUMBgoLDA0OD0rYJCtKygAgKCU6DCDtz4Z5EE7CkRpP4prX2yMqST5bmQ+x2nrwx7mJlIyJJXE9E3F5JgQTcD0LeiYHaEqccEU/aEBXAgIQcDsqMhJwDCDtz4Z5EE7CkRpP4prX2yMqST5bmQ+x2nrwx7mJlIyJJXE9E3F4JgQTcD0LeSYHaEqccEU/aEBXAwMQcAAhiNsoStgkCUrKACEoAzpxOwwUEnB4JgQLcT0ecnkmBBNwPRZ6JgdoSpxwRWlyatgmB2hKnHBFP2loEr9AVwMDEHAAFIjbKErYJAlKygAUKAM6cTsMFBJweCYEC3E9HnJ5JgQTcD0WeiYHaEqccEVpcmrYJgdoSpxwRT9paBK/QFcDAxBwACCI2yhK2CQJSsoAICgDOnE7DBQScHgmBAtxPR5yeSYEE3A9FnomB2hKnHBFaXJq2CYHaEqccEU/aWgSv0BXAwMQcAwDMTIzcTsMFBJweCYEC3E9HnJ5JgQTcD0WeiYHaEqccEVpcmrYJgdoSpxwRT9paBK/QFcCAxBwOwsTEnB4JgM4PRNxeSYEE3A9C3omB2hKnHBFP2hAqX4vSQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAlleGNlcHRpb246
    /// 00 : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 0B : OpCode.THROW [512 datoshi]
    /// </remarks>
    [DisplayName("throwCall")]
    public abstract object? ThrowCall();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIDEHA7FioRcHgmDgwJZXhjZXB0aW9uOj0ccRJweSYODAlleGNlcHRpb246PQh6JgQTcD8UcGhA
    /// 00 : OpCode.INITSLOT 0203 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 162A [4 datoshi]
    /// 08 : OpCode.PUSH1 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 0E [2 datoshi]
    /// 0D : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 18 : OpCode.THROW [512 datoshi]
    /// 19 : OpCode.ENDTRY 1C [4 datoshi]
    /// 1B : OpCode.STLOC1 [2 datoshi]
    /// 1C : OpCode.PUSH2 [1 datoshi]
    /// 1D : OpCode.STLOC0 [2 datoshi]
    /// 1E : OpCode.LDARG1 [2 datoshi]
    /// 1F : OpCode.JMPIFNOT 0E [2 datoshi]
    /// 21 : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 2C : OpCode.THROW [512 datoshi]
    /// 2D : OpCode.ENDTRY 08 [4 datoshi]
    /// 2F : OpCode.LDARG2 [2 datoshi]
    /// 30 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 32 : OpCode.PUSH3 [1 datoshi]
    /// 33 : OpCode.STLOC0 [2 datoshi]
    /// 34 : OpCode.ENDFINALLY [4 datoshi]
    /// 35 : OpCode.PUSH4 [1 datoshi]
    /// 36 : OpCode.STLOC0 [2 datoshi]
    /// 37 : OpCode.LDLOC0 [2 datoshi]
    /// 38 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("throwInCatch")]
    public abstract BigInteger? ThrowInCatch(bool? throwInTry, bool? throwInCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIDEHA7Fh4ScHgmDgwJZXhjZXB0aW9uOj0TcXkmBBNwPQt6JgdoSpxwRT9oQA==
    /// 00 : OpCode.INITSLOT 0203 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 161E [4 datoshi]
    /// 08 : OpCode.PUSH2 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 0E [2 datoshi]
    /// 0D : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 18 : OpCode.THROW [512 datoshi]
    /// 19 : OpCode.ENDTRY 13 [4 datoshi]
    /// 1B : OpCode.STLOC1 [2 datoshi]
    /// 1C : OpCode.LDARG1 [2 datoshi]
    /// 1D : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 1F : OpCode.PUSH3 [1 datoshi]
    /// 20 : OpCode.STLOC0 [2 datoshi]
    /// 21 : OpCode.ENDTRY 0B [4 datoshi]
    /// 23 : OpCode.LDARG2 [2 datoshi]
    /// 24 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 26 : OpCode.LDLOC0 [2 datoshi]
    /// 27 : OpCode.DUP [2 datoshi]
    /// 28 : OpCode.INC [4 datoshi]
    /// 29 : OpCode.STLOC0 [2 datoshi]
    /// 2A : OpCode.DROP [2 datoshi]
    /// 2B : OpCode.ENDFINALLY [4 datoshi]
    /// 2C : OpCode.LDLOC0 [2 datoshi]
    /// 2D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("try01")]
    public abstract BigInteger? Try01(bool? throwException, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4NMxA
    /// 00 : OpCode.INITSLOT 0003 [64 datoshi]
    /// 03 : OpCode.LDARG2 [2 datoshi]
    /// 04 : OpCode.LDARG1 [2 datoshi]
    /// 05 : OpCode.LDARG0 [2 datoshi]
    /// 06 : OpCode.CALL CC [512 datoshi]
    /// 08 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("try02")]
    public abstract BigInteger? Try02(bool? throwException, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIDEHA7DRUScHgmBTQYRT0TcXkmBBNwPQt6JgdoSpxwRT9oQA==
    /// 00 : OpCode.INITSLOT 0203 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 0D15 [4 datoshi]
    /// 08 : OpCode.PUSH2 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 0D : OpCode.CALL 18 [512 datoshi]
    /// 0F : OpCode.DROP [2 datoshi]
    /// 10 : OpCode.ENDTRY 13 [4 datoshi]
    /// 12 : OpCode.STLOC1 [2 datoshi]
    /// 13 : OpCode.LDARG1 [2 datoshi]
    /// 14 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 16 : OpCode.PUSH3 [1 datoshi]
    /// 17 : OpCode.STLOC0 [2 datoshi]
    /// 18 : OpCode.ENDTRY 0B [4 datoshi]
    /// 1A : OpCode.LDARG2 [2 datoshi]
    /// 1B : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 1D : OpCode.LDLOC0 [2 datoshi]
    /// 1E : OpCode.DUP [2 datoshi]
    /// 1F : OpCode.INC [4 datoshi]
    /// 20 : OpCode.STLOC0 [2 datoshi]
    /// 21 : OpCode.DROP [2 datoshi]
    /// 22 : OpCode.ENDFINALLY [4 datoshi]
    /// 23 : OpCode.LDLOC0 [2 datoshi]
    /// 24 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("try03")]
    public abstract BigInteger? Try03(bool? throwException, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEHA7EAAScHgmCDUu////RT0NcXkmB2hKnHBFPQJoQA==
    /// 00 : OpCode.INITSLOT 0202 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 1000 [4 datoshi]
    /// 08 : OpCode.PUSH2 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 08 [2 datoshi]
    /// 0D : OpCode.CALL_L 2EFFFFFF [512 datoshi]
    /// 12 : OpCode.DROP [2 datoshi]
    /// 13 : OpCode.ENDTRY 0D [4 datoshi]
    /// 15 : OpCode.STLOC1 [2 datoshi]
    /// 16 : OpCode.LDARG1 [2 datoshi]
    /// 17 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 19 : OpCode.LDLOC0 [2 datoshi]
    /// 1A : OpCode.DUP [2 datoshi]
    /// 1B : OpCode.INC [4 datoshi]
    /// 1C : OpCode.STLOC0 [2 datoshi]
    /// 1D : OpCode.DROP [2 datoshi]
    /// 1E : OpCode.ENDTRY 02 [4 datoshi]
    /// 20 : OpCode.LDLOC0 [2 datoshi]
    /// 21 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryCatch")]
    public abstract BigInteger? TryCatch(bool? throwException, bool? enterCatch);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIDEHA7Q0sScHgmDAwGCgsMDQ4PIiUMIQJHANsukNnwLE+fyGKrrKknJflbT93MjX/6U4aT7PRjqUrYJAlKygAhKAM6cT0TcXkmBBNwPQt6JgdoSpxwRT9oQA==
    /// 00 : OpCode.INITSLOT 0203 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 434B [4 datoshi]
    /// 08 : OpCode.PUSH2 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 0C [2 datoshi]
    /// 0D : OpCode.PUSHDATA1 0A0B0C0D0E0F [8 datoshi]
    /// 15 : OpCode.JMP 25 [2 datoshi]
    /// 17 : OpCode.PUSHDATA1 024700DB2E90D9F02C4F9FC862ABACA92725F95B4FDDCC8D7FFA538693ECF463A9 [8 datoshi]
    /// 3A : OpCode.DUP [2 datoshi]
    /// 3B : OpCode.ISNULL [2 datoshi]
    /// 3C : OpCode.JMPIF 09 [2 datoshi]
    /// 3E : OpCode.DUP [2 datoshi]
    /// 3F : OpCode.SIZE [4 datoshi]
    /// 40 : OpCode.PUSHINT8 21 [1 datoshi]
    /// 42 : OpCode.JMPEQ 03 [2 datoshi]
    /// 44 : OpCode.THROW [512 datoshi]
    /// 45 : OpCode.STLOC1 [2 datoshi]
    /// 46 : OpCode.ENDTRY 13 [4 datoshi]
    /// 48 : OpCode.STLOC1 [2 datoshi]
    /// 49 : OpCode.LDARG1 [2 datoshi]
    /// 4A : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 4C : OpCode.PUSH3 [1 datoshi]
    /// 4D : OpCode.STLOC0 [2 datoshi]
    /// 4E : OpCode.ENDTRY 0B [4 datoshi]
    /// 50 : OpCode.LDARG2 [2 datoshi]
    /// 51 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 53 : OpCode.LDLOC0 [2 datoshi]
    /// 54 : OpCode.DUP [2 datoshi]
    /// 55 : OpCode.INC [4 datoshi]
    /// 56 : OpCode.STLOC0 [2 datoshi]
    /// 57 : OpCode.DROP [2 datoshi]
    /// 58 : OpCode.ENDFINALLY [4 datoshi]
    /// 59 : OpCode.LDLOC0 [2 datoshi]
    /// 5A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryecpointCast")]
    public abstract BigInteger? TryecpointCast(bool? useInvalidECpoint, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEHA7ABYScHgmDgwJZXhjZXB0aW9uOj0LeSYHaEqccEU/aEA=
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 0016 [4 datoshi]
    /// 08 : OpCode.PUSH2 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 0E [2 datoshi]
    /// 0D : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 18 : OpCode.THROW [512 datoshi]
    /// 19 : OpCode.ENDTRY 0B [4 datoshi]
    /// 1B : OpCode.LDARG1 [2 datoshi]
    /// 1C : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 1E : OpCode.LDLOC0 [2 datoshi]
    /// 1F : OpCode.DUP [2 datoshi]
    /// 20 : OpCode.INC [4 datoshi]
    /// 21 : OpCode.STLOC0 [2 datoshi]
    /// 22 : OpCode.DROP [2 datoshi]
    /// 23 : OpCode.ENDFINALLY [4 datoshi]
    /// 24 : OpCode.LDLOC0 [2 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryFinally")]
    public abstract BigInteger? TryFinally(bool? throwException, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECEHA7ABAScHgmCDVO////RT0LeSYHaEqccEU/aEA=
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 0010 [4 datoshi]
    /// 08 : OpCode.PUSH2 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 08 [2 datoshi]
    /// 0D : OpCode.CALL_L 4EFFFFFF [512 datoshi]
    /// 12 : OpCode.DROP [2 datoshi]
    /// 13 : OpCode.ENDTRY 0B [4 datoshi]
    /// 15 : OpCode.LDARG1 [2 datoshi]
    /// 16 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 18 : OpCode.LDLOC0 [2 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.INC [4 datoshi]
    /// 1B : OpCode.STLOC0 [2 datoshi]
    /// 1C : OpCode.DROP [2 datoshi]
    /// 1D : OpCode.ENDFINALLY [4 datoshi]
    /// 1E : OpCode.LDLOC0 [2 datoshi]
    /// 1F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryFinallyAndRethrow")]
    public abstract BigInteger? TryFinallyAndRethrow(bool? throwException, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIDEHA7Nj4ScHgmDAwGCgsMDQ4PIhgMFH7uGqvrZ+0deR1E5PX8866RcahxStgkCUrKABQoAzpxPRNxeSYEE3A9C3omB2hKnHBFP2hA
    /// 00 : OpCode.INITSLOT 0203 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 363E [4 datoshi]
    /// 08 : OpCode.PUSH2 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 0C [2 datoshi]
    /// 0D : OpCode.PUSHDATA1 0A0B0C0D0E0F [8 datoshi]
    /// 15 : OpCode.JMP 18 [2 datoshi]
    /// 17 : OpCode.PUSHDATA1 7EEE1AABEB67ED1D791D44E4F5FCF3AE9171A871 [8 datoshi]
    /// 2D : OpCode.DUP [2 datoshi]
    /// 2E : OpCode.ISNULL [2 datoshi]
    /// 2F : OpCode.JMPIF 09 [2 datoshi]
    /// 31 : OpCode.DUP [2 datoshi]
    /// 32 : OpCode.SIZE [4 datoshi]
    /// 33 : OpCode.PUSHINT8 14 [1 datoshi]
    /// 35 : OpCode.JMPEQ 03 [2 datoshi]
    /// 37 : OpCode.THROW [512 datoshi]
    /// 38 : OpCode.STLOC1 [2 datoshi]
    /// 39 : OpCode.ENDTRY 13 [4 datoshi]
    /// 3B : OpCode.STLOC1 [2 datoshi]
    /// 3C : OpCode.LDARG1 [2 datoshi]
    /// 3D : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 3F : OpCode.PUSH3 [1 datoshi]
    /// 40 : OpCode.STLOC0 [2 datoshi]
    /// 41 : OpCode.ENDTRY 0B [4 datoshi]
    /// 43 : OpCode.LDARG2 [2 datoshi]
    /// 44 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 46 : OpCode.LDLOC0 [2 datoshi]
    /// 47 : OpCode.DUP [2 datoshi]
    /// 48 : OpCode.INC [4 datoshi]
    /// 49 : OpCode.STLOC0 [2 datoshi]
    /// 4A : OpCode.DROP [2 datoshi]
    /// 4B : OpCode.ENDFINALLY [4 datoshi]
    /// 4C : OpCode.LDLOC0 [2 datoshi]
    /// 4D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryinvalidByteArray2UInt160")]
    public abstract BigInteger? TryinvalidByteArray2UInt160(bool? useInvalidECpoint, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIDEHA7QEgScHgmFQwGCgsMDQ4PStgkK0rKACAoJToMIO3PhnkQTsKRGk/imtfbIypJPluZD7HaevDHuYmUjIklcT0TcXkmBBNwPQt6JgdoSpxwRT9oQA==
    /// 00 : OpCode.INITSLOT 0203 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 4048 [4 datoshi]
    /// 08 : OpCode.PUSH2 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 15 [2 datoshi]
    /// 0D : OpCode.PUSHDATA1 0A0B0C0D0E0F [8 datoshi]
    /// 15 : OpCode.DUP [2 datoshi]
    /// 16 : OpCode.ISNULL [2 datoshi]
    /// 17 : OpCode.JMPIF 2B [2 datoshi]
    /// 19 : OpCode.DUP [2 datoshi]
    /// 1A : OpCode.SIZE [4 datoshi]
    /// 1B : OpCode.PUSHINT8 20 [1 datoshi]
    /// 1D : OpCode.JMPEQ 25 [2 datoshi]
    /// 1F : OpCode.THROW [512 datoshi]
    /// 20 : OpCode.PUSHDATA1 EDCF8679104EC2911A4FE29AD7DB232A493E5B990FB1DA7AF0C7B989948C8925 [8 datoshi]
    /// 42 : OpCode.STLOC1 [2 datoshi]
    /// 43 : OpCode.ENDTRY 13 [4 datoshi]
    /// 45 : OpCode.STLOC1 [2 datoshi]
    /// 46 : OpCode.LDARG1 [2 datoshi]
    /// 47 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 49 : OpCode.PUSH3 [1 datoshi]
    /// 4A : OpCode.STLOC0 [2 datoshi]
    /// 4B : OpCode.ENDTRY 0B [4 datoshi]
    /// 4D : OpCode.LDARG2 [2 datoshi]
    /// 4E : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 50 : OpCode.LDLOC0 [2 datoshi]
    /// 51 : OpCode.DUP [2 datoshi]
    /// 52 : OpCode.INC [4 datoshi]
    /// 53 : OpCode.STLOC0 [2 datoshi]
    /// 54 : OpCode.DROP [2 datoshi]
    /// 55 : OpCode.ENDFINALLY [4 datoshi]
    /// 56 : OpCode.LDLOC0 [2 datoshi]
    /// 57 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryinvalidByteArray2UInt256")]
    public abstract BigInteger? TryinvalidByteArray2UInt256(bool? useInvalidECpoint, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIEEHA7KAA7DRgScHgmBTTkRT0YcRNweSYFNNlFPQ16JgQ00WhKnHBFPz0NcXsmB2hKnHBFPQJoQA==
    /// 00 : OpCode.INITSLOT 0204 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 2800 [4 datoshi]
    /// 08 : OpCode.TRY 0D18 [4 datoshi]
    /// 0B : OpCode.PUSH2 [1 datoshi]
    /// 0C : OpCode.STLOC0 [2 datoshi]
    /// 0D : OpCode.LDARG0 [2 datoshi]
    /// 0E : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 10 : OpCode.CALL E4 [512 datoshi]
    /// 12 : OpCode.DROP [2 datoshi]
    /// 13 : OpCode.ENDTRY 18 [4 datoshi]
    /// 15 : OpCode.STLOC1 [2 datoshi]
    /// 16 : OpCode.PUSH3 [1 datoshi]
    /// 17 : OpCode.STLOC0 [2 datoshi]
    /// 18 : OpCode.LDARG1 [2 datoshi]
    /// 19 : OpCode.JMPIFNOT 05 [2 datoshi]
    /// 1B : OpCode.CALL D9 [512 datoshi]
    /// 1D : OpCode.DROP [2 datoshi]
    /// 1E : OpCode.ENDTRY 0D [4 datoshi]
    /// 20 : OpCode.LDARG2 [2 datoshi]
    /// 21 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 23 : OpCode.CALL D1 [512 datoshi]
    /// 25 : OpCode.LDLOC0 [2 datoshi]
    /// 26 : OpCode.DUP [2 datoshi]
    /// 27 : OpCode.INC [4 datoshi]
    /// 28 : OpCode.STLOC0 [2 datoshi]
    /// 29 : OpCode.DROP [2 datoshi]
    /// 2A : OpCode.ENDFINALLY [4 datoshi]
    /// 2B : OpCode.ENDTRY 0D [4 datoshi]
    /// 2D : OpCode.STLOC1 [2 datoshi]
    /// 2E : OpCode.LDARG3 [2 datoshi]
    /// 2F : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 31 : OpCode.LDLOC0 [2 datoshi]
    /// 32 : OpCode.DUP [2 datoshi]
    /// 33 : OpCode.INC [4 datoshi]
    /// 34 : OpCode.STLOC0 [2 datoshi]
    /// 35 : OpCode.DROP [2 datoshi]
    /// 36 : OpCode.ENDTRY 02 [4 datoshi]
    /// 38 : OpCode.LDLOC0 [2 datoshi]
    /// 39 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryNest")]
    public abstract BigInteger? TryNest(bool? throwInTry, bool? throwInCatch, bool? throwInFinally, bool? enterOuterCatch);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMDEHAMAzEyM3E7DBQScHgmBAtxPR5yeSYEE3A9FnomB2hKnHBFaXJq2CYHaEqccEU/aWgSv0A=
    /// 00 : OpCode.INITSLOT 0303 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHDATA1 313233 '123' [8 datoshi]
    /// 0A : OpCode.STLOC1 [2 datoshi]
    /// 0B : OpCode.TRY 0C14 [4 datoshi]
    /// 0E : OpCode.PUSH2 [1 datoshi]
    /// 0F : OpCode.STLOC0 [2 datoshi]
    /// 10 : OpCode.LDARG0 [2 datoshi]
    /// 11 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 13 : OpCode.PUSHNULL [1 datoshi]
    /// 14 : OpCode.STLOC1 [2 datoshi]
    /// 15 : OpCode.ENDTRY 1E [4 datoshi]
    /// 17 : OpCode.STLOC2 [2 datoshi]
    /// 18 : OpCode.LDARG1 [2 datoshi]
    /// 19 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 1B : OpCode.PUSH3 [1 datoshi]
    /// 1C : OpCode.STLOC0 [2 datoshi]
    /// 1D : OpCode.ENDTRY 16 [4 datoshi]
    /// 1F : OpCode.LDARG2 [2 datoshi]
    /// 20 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 22 : OpCode.LDLOC0 [2 datoshi]
    /// 23 : OpCode.DUP [2 datoshi]
    /// 24 : OpCode.INC [4 datoshi]
    /// 25 : OpCode.STLOC0 [2 datoshi]
    /// 26 : OpCode.DROP [2 datoshi]
    /// 27 : OpCode.LDLOC1 [2 datoshi]
    /// 28 : OpCode.STLOC2 [2 datoshi]
    /// 29 : OpCode.LDLOC2 [2 datoshi]
    /// 2A : OpCode.ISNULL [2 datoshi]
    /// 2B : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 2D : OpCode.LDLOC0 [2 datoshi]
    /// 2E : OpCode.DUP [2 datoshi]
    /// 2F : OpCode.INC [4 datoshi]
    /// 30 : OpCode.STLOC0 [2 datoshi]
    /// 31 : OpCode.DROP [2 datoshi]
    /// 32 : OpCode.ENDFINALLY [4 datoshi]
    /// 33 : OpCode.LDLOC1 [2 datoshi]
    /// 34 : OpCode.LDLOC0 [2 datoshi]
    /// 35 : OpCode.PUSH2 [1 datoshi]
    /// 36 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 37 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryNULL2Bytestring_1")]
    public abstract IList<object>? TryNULL2Bytestring_1(bool? setToNull, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMDEHAAIYjbKErYJAlKygAhKAM6cTsMFBJweCYEC3E9HnJ5JgQTcD0WeiYHaEqccEVpcmrYJgdoSpxwRT9paBK/QA==
    /// 00 : OpCode.INITSLOT 0303 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHINT8 21 [1 datoshi]
    /// 07 : OpCode.NEWBUFFER [256 datoshi]
    /// 08 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 0A : OpCode.DUP [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.JMPIF 09 [2 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.SIZE [4 datoshi]
    /// 10 : OpCode.PUSHINT8 21 [1 datoshi]
    /// 12 : OpCode.JMPEQ 03 [2 datoshi]
    /// 14 : OpCode.THROW [512 datoshi]
    /// 15 : OpCode.STLOC1 [2 datoshi]
    /// 16 : OpCode.TRY 0C14 [4 datoshi]
    /// 19 : OpCode.PUSH2 [1 datoshi]
    /// 1A : OpCode.STLOC0 [2 datoshi]
    /// 1B : OpCode.LDARG0 [2 datoshi]
    /// 1C : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 1E : OpCode.PUSHNULL [1 datoshi]
    /// 1F : OpCode.STLOC1 [2 datoshi]
    /// 20 : OpCode.ENDTRY 1E [4 datoshi]
    /// 22 : OpCode.STLOC2 [2 datoshi]
    /// 23 : OpCode.LDARG1 [2 datoshi]
    /// 24 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 26 : OpCode.PUSH3 [1 datoshi]
    /// 27 : OpCode.STLOC0 [2 datoshi]
    /// 28 : OpCode.ENDTRY 16 [4 datoshi]
    /// 2A : OpCode.LDARG2 [2 datoshi]
    /// 2B : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 2D : OpCode.LDLOC0 [2 datoshi]
    /// 2E : OpCode.DUP [2 datoshi]
    /// 2F : OpCode.INC [4 datoshi]
    /// 30 : OpCode.STLOC0 [2 datoshi]
    /// 31 : OpCode.DROP [2 datoshi]
    /// 32 : OpCode.LDLOC1 [2 datoshi]
    /// 33 : OpCode.STLOC2 [2 datoshi]
    /// 34 : OpCode.LDLOC2 [2 datoshi]
    /// 35 : OpCode.ISNULL [2 datoshi]
    /// 36 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 38 : OpCode.LDLOC0 [2 datoshi]
    /// 39 : OpCode.DUP [2 datoshi]
    /// 3A : OpCode.INC [4 datoshi]
    /// 3B : OpCode.STLOC0 [2 datoshi]
    /// 3C : OpCode.DROP [2 datoshi]
    /// 3D : OpCode.ENDFINALLY [4 datoshi]
    /// 3E : OpCode.LDLOC1 [2 datoshi]
    /// 3F : OpCode.LDLOC0 [2 datoshi]
    /// 40 : OpCode.PUSH2 [1 datoshi]
    /// 41 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 42 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryNULL2Ecpoint_1")]
    public abstract IList<object>? TryNULL2Ecpoint_1(bool? setToNull, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMDEHAAFIjbKErYJAlKygAUKAM6cTsMFBJweCYEC3E9HnJ5JgQTcD0WeiYHaEqccEVpcmrYJgdoSpxwRT9paBK/QA==
    /// 00 : OpCode.INITSLOT 0303 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHINT8 14 [1 datoshi]
    /// 07 : OpCode.NEWBUFFER [256 datoshi]
    /// 08 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 0A : OpCode.DUP [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.JMPIF 09 [2 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.SIZE [4 datoshi]
    /// 10 : OpCode.PUSHINT8 14 [1 datoshi]
    /// 12 : OpCode.JMPEQ 03 [2 datoshi]
    /// 14 : OpCode.THROW [512 datoshi]
    /// 15 : OpCode.STLOC1 [2 datoshi]
    /// 16 : OpCode.TRY 0C14 [4 datoshi]
    /// 19 : OpCode.PUSH2 [1 datoshi]
    /// 1A : OpCode.STLOC0 [2 datoshi]
    /// 1B : OpCode.LDARG0 [2 datoshi]
    /// 1C : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 1E : OpCode.PUSHNULL [1 datoshi]
    /// 1F : OpCode.STLOC1 [2 datoshi]
    /// 20 : OpCode.ENDTRY 1E [4 datoshi]
    /// 22 : OpCode.STLOC2 [2 datoshi]
    /// 23 : OpCode.LDARG1 [2 datoshi]
    /// 24 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 26 : OpCode.PUSH3 [1 datoshi]
    /// 27 : OpCode.STLOC0 [2 datoshi]
    /// 28 : OpCode.ENDTRY 16 [4 datoshi]
    /// 2A : OpCode.LDARG2 [2 datoshi]
    /// 2B : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 2D : OpCode.LDLOC0 [2 datoshi]
    /// 2E : OpCode.DUP [2 datoshi]
    /// 2F : OpCode.INC [4 datoshi]
    /// 30 : OpCode.STLOC0 [2 datoshi]
    /// 31 : OpCode.DROP [2 datoshi]
    /// 32 : OpCode.LDLOC1 [2 datoshi]
    /// 33 : OpCode.STLOC2 [2 datoshi]
    /// 34 : OpCode.LDLOC2 [2 datoshi]
    /// 35 : OpCode.ISNULL [2 datoshi]
    /// 36 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 38 : OpCode.LDLOC0 [2 datoshi]
    /// 39 : OpCode.DUP [2 datoshi]
    /// 3A : OpCode.INC [4 datoshi]
    /// 3B : OpCode.STLOC0 [2 datoshi]
    /// 3C : OpCode.DROP [2 datoshi]
    /// 3D : OpCode.ENDFINALLY [4 datoshi]
    /// 3E : OpCode.LDLOC1 [2 datoshi]
    /// 3F : OpCode.LDLOC0 [2 datoshi]
    /// 40 : OpCode.PUSH2 [1 datoshi]
    /// 41 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 42 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryNULL2Uint160_1")]
    public abstract IList<object>? TryNULL2Uint160_1(bool? setToNull, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMDEHAAIIjbKErYJAlKygAgKAM6cTsMFBJweCYEC3E9HnJ5JgQTcD0WeiYHaEqccEVpcmrYJgdoSpxwRT9paBK/QA==
    /// 00 : OpCode.INITSLOT 0303 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.PUSHINT8 20 [1 datoshi]
    /// 07 : OpCode.NEWBUFFER [256 datoshi]
    /// 08 : OpCode.CONVERT 28 'ByteString' [8192 datoshi]
    /// 0A : OpCode.DUP [2 datoshi]
    /// 0B : OpCode.ISNULL [2 datoshi]
    /// 0C : OpCode.JMPIF 09 [2 datoshi]
    /// 0E : OpCode.DUP [2 datoshi]
    /// 0F : OpCode.SIZE [4 datoshi]
    /// 10 : OpCode.PUSHINT8 20 [1 datoshi]
    /// 12 : OpCode.JMPEQ 03 [2 datoshi]
    /// 14 : OpCode.THROW [512 datoshi]
    /// 15 : OpCode.STLOC1 [2 datoshi]
    /// 16 : OpCode.TRY 0C14 [4 datoshi]
    /// 19 : OpCode.PUSH2 [1 datoshi]
    /// 1A : OpCode.STLOC0 [2 datoshi]
    /// 1B : OpCode.LDARG0 [2 datoshi]
    /// 1C : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 1E : OpCode.PUSHNULL [1 datoshi]
    /// 1F : OpCode.STLOC1 [2 datoshi]
    /// 20 : OpCode.ENDTRY 1E [4 datoshi]
    /// 22 : OpCode.STLOC2 [2 datoshi]
    /// 23 : OpCode.LDARG1 [2 datoshi]
    /// 24 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 26 : OpCode.PUSH3 [1 datoshi]
    /// 27 : OpCode.STLOC0 [2 datoshi]
    /// 28 : OpCode.ENDTRY 16 [4 datoshi]
    /// 2A : OpCode.LDARG2 [2 datoshi]
    /// 2B : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 2D : OpCode.LDLOC0 [2 datoshi]
    /// 2E : OpCode.DUP [2 datoshi]
    /// 2F : OpCode.INC [4 datoshi]
    /// 30 : OpCode.STLOC0 [2 datoshi]
    /// 31 : OpCode.DROP [2 datoshi]
    /// 32 : OpCode.LDLOC1 [2 datoshi]
    /// 33 : OpCode.STLOC2 [2 datoshi]
    /// 34 : OpCode.LDLOC2 [2 datoshi]
    /// 35 : OpCode.ISNULL [2 datoshi]
    /// 36 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 38 : OpCode.LDLOC0 [2 datoshi]
    /// 39 : OpCode.DUP [2 datoshi]
    /// 3A : OpCode.INC [4 datoshi]
    /// 3B : OpCode.STLOC0 [2 datoshi]
    /// 3C : OpCode.DROP [2 datoshi]
    /// 3D : OpCode.ENDFINALLY [4 datoshi]
    /// 3E : OpCode.LDLOC1 [2 datoshi]
    /// 3F : OpCode.LDLOC0 [2 datoshi]
    /// 40 : OpCode.PUSH2 [1 datoshi]
    /// 41 : OpCode.PACKSTRUCT [2048 datoshi]
    /// 42 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryNULL2Uint256_1")]
    public abstract IList<object>? TryNULL2Uint256_1(bool? setToNull, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIDEHA7CxMScHgmAzg9E3F5JgQTcD0LeiYHaEqccEU/aEA=
    /// 00 : OpCode.INITSLOT 0203 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 0B13 [4 datoshi]
    /// 08 : OpCode.PUSH2 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.LDARG0 [2 datoshi]
    /// 0B : OpCode.JMPIFNOT 03 [2 datoshi]
    /// 0D : OpCode.ABORT [0 datoshi]
    /// 0E : OpCode.ENDTRY 13 [4 datoshi]
    /// 10 : OpCode.STLOC1 [2 datoshi]
    /// 11 : OpCode.LDARG1 [2 datoshi]
    /// 12 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 14 : OpCode.PUSH3 [1 datoshi]
    /// 15 : OpCode.STLOC0 [2 datoshi]
    /// 16 : OpCode.ENDTRY 0B [4 datoshi]
    /// 18 : OpCode.LDARG2 [2 datoshi]
    /// 19 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 1B : OpCode.LDLOC0 [2 datoshi]
    /// 1C : OpCode.DUP [2 datoshi]
    /// 1D : OpCode.INC [4 datoshi]
    /// 1E : OpCode.STLOC0 [2 datoshi]
    /// 1F : OpCode.DROP [2 datoshi]
    /// 20 : OpCode.ENDFINALLY [4 datoshi]
    /// 21 : OpCode.LDLOC0 [2 datoshi]
    /// 22 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryUncatchableException")]
    public abstract BigInteger? TryUncatchableException(bool? throwException, bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEHA7KTEScAwUfu4aq+tn7R15HUTk9fzzrpFxqHFK2CQJSsoAFCgDOnE9E3F4JgQTcD0LeSYHaEqccEU/aEA=
    /// 00 : OpCode.INITSLOT 0202 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 2931 [4 datoshi]
    /// 08 : OpCode.PUSH2 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.PUSHDATA1 7EEE1AABEB67ED1D791D44E4F5FCF3AE9171A871 [8 datoshi]
    /// 20 : OpCode.DUP [2 datoshi]
    /// 21 : OpCode.ISNULL [2 datoshi]
    /// 22 : OpCode.JMPIF 09 [2 datoshi]
    /// 24 : OpCode.DUP [2 datoshi]
    /// 25 : OpCode.SIZE [4 datoshi]
    /// 26 : OpCode.PUSHINT8 14 [1 datoshi]
    /// 28 : OpCode.JMPEQ 03 [2 datoshi]
    /// 2A : OpCode.THROW [512 datoshi]
    /// 2B : OpCode.STLOC1 [2 datoshi]
    /// 2C : OpCode.ENDTRY 13 [4 datoshi]
    /// 2E : OpCode.STLOC1 [2 datoshi]
    /// 2F : OpCode.LDARG0 [2 datoshi]
    /// 30 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 32 : OpCode.PUSH3 [1 datoshi]
    /// 33 : OpCode.STLOC0 [2 datoshi]
    /// 34 : OpCode.ENDTRY 0B [4 datoshi]
    /// 36 : OpCode.LDARG1 [2 datoshi]
    /// 37 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 39 : OpCode.LDLOC0 [2 datoshi]
    /// 3A : OpCode.DUP [2 datoshi]
    /// 3B : OpCode.INC [4 datoshi]
    /// 3C : OpCode.STLOC0 [2 datoshi]
    /// 3D : OpCode.DROP [2 datoshi]
    /// 3E : OpCode.ENDFINALLY [4 datoshi]
    /// 3F : OpCode.LDLOC0 [2 datoshi]
    /// 40 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryvalidByteArray2UInt160")]
    public abstract BigInteger? TryvalidByteArray2UInt160(bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEHA7KjIScAwg7c+GeRBOwpEaT+Ka19sjKkk+W5kPsdp68Me5iZSMiSVxPRNxeCYEE3A9C3kmB2hKnHBFP2hA
    /// 00 : OpCode.INITSLOT 0202 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 2A32 [4 datoshi]
    /// 08 : OpCode.PUSH2 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.PUSHDATA1 EDCF8679104EC2911A4FE29AD7DB232A493E5B990FB1DA7AF0C7B989948C8925 [8 datoshi]
    /// 2C : OpCode.STLOC1 [2 datoshi]
    /// 2D : OpCode.ENDTRY 13 [4 datoshi]
    /// 2F : OpCode.STLOC1 [2 datoshi]
    /// 30 : OpCode.LDARG0 [2 datoshi]
    /// 31 : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 33 : OpCode.PUSH3 [1 datoshi]
    /// 34 : OpCode.STLOC0 [2 datoshi]
    /// 35 : OpCode.ENDTRY 0B [4 datoshi]
    /// 37 : OpCode.LDARG1 [2 datoshi]
    /// 38 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 3A : OpCode.LDLOC0 [2 datoshi]
    /// 3B : OpCode.DUP [2 datoshi]
    /// 3C : OpCode.INC [4 datoshi]
    /// 3D : OpCode.STLOC0 [2 datoshi]
    /// 3E : OpCode.DROP [2 datoshi]
    /// 3F : OpCode.ENDFINALLY [4 datoshi]
    /// 40 : OpCode.LDLOC0 [2 datoshi]
    /// 41 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryvalidByteArray2UInt256")]
    public abstract BigInteger? TryvalidByteArray2UInt256(bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICEHA7Nj4ScAwhAkcA2y6Q2fAsT5/IYqusqScl+VtP3cyNf/pThpPs9GOpStgkCUrKACEoAzpxPRNxeCYEE3A9C3kmB2hKnHBFP2hA
    /// 00 : OpCode.INITSLOT 0202 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 363E [4 datoshi]
    /// 08 : OpCode.PUSH2 [1 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.PUSHDATA1 024700DB2E90D9F02C4F9FC862ABACA92725F95B4FDDCC8D7FFA538693ECF463A9 [8 datoshi]
    /// 2D : OpCode.DUP [2 datoshi]
    /// 2E : OpCode.ISNULL [2 datoshi]
    /// 2F : OpCode.JMPIF 09 [2 datoshi]
    /// 31 : OpCode.DUP [2 datoshi]
    /// 32 : OpCode.SIZE [4 datoshi]
    /// 33 : OpCode.PUSHINT8 21 [1 datoshi]
    /// 35 : OpCode.JMPEQ 03 [2 datoshi]
    /// 37 : OpCode.THROW [512 datoshi]
    /// 38 : OpCode.STLOC1 [2 datoshi]
    /// 39 : OpCode.ENDTRY 13 [4 datoshi]
    /// 3B : OpCode.STLOC1 [2 datoshi]
    /// 3C : OpCode.LDARG0 [2 datoshi]
    /// 3D : OpCode.JMPIFNOT 04 [2 datoshi]
    /// 3F : OpCode.PUSH3 [1 datoshi]
    /// 40 : OpCode.STLOC0 [2 datoshi]
    /// 41 : OpCode.ENDTRY 0B [4 datoshi]
    /// 43 : OpCode.LDARG1 [2 datoshi]
    /// 44 : OpCode.JMPIFNOT 07 [2 datoshi]
    /// 46 : OpCode.LDLOC0 [2 datoshi]
    /// 47 : OpCode.DUP [2 datoshi]
    /// 48 : OpCode.INC [4 datoshi]
    /// 49 : OpCode.STLOC0 [2 datoshi]
    /// 4A : OpCode.DROP [2 datoshi]
    /// 4B : OpCode.ENDFINALLY [4 datoshi]
    /// 4C : OpCode.LDLOC0 [2 datoshi]
    /// 4D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryvalidByteString2Ecpoint")]
    public abstract BigInteger? TryvalidByteString2Ecpoint(bool? enterCatch, bool? enterFinally);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIGEHA7P0k7GSNoSpxwRXgmDgwJZXhjZXB0aW9uOj0UcXomBmgSnnA9CnwmBmgTnnA/eSYODAlleGNlcHRpb246PRRxeyYGaBSecD0KfSYGaBWecD9oQA==
    /// 00 : OpCode.INITSLOT 0206 [64 datoshi]
    /// 03 : OpCode.PUSH0 [1 datoshi]
    /// 04 : OpCode.STLOC0 [2 datoshi]
    /// 05 : OpCode.TRY 3F49 [4 datoshi]
    /// 08 : OpCode.TRY 1923 [4 datoshi]
    /// 0B : OpCode.LDLOC0 [2 datoshi]
    /// 0C : OpCode.DUP [2 datoshi]
    /// 0D : OpCode.INC [4 datoshi]
    /// 0E : OpCode.STLOC0 [2 datoshi]
    /// 0F : OpCode.DROP [2 datoshi]
    /// 10 : OpCode.LDARG0 [2 datoshi]
    /// 11 : OpCode.JMPIFNOT 0E [2 datoshi]
    /// 13 : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 1E : OpCode.THROW [512 datoshi]
    /// 1F : OpCode.ENDTRY 14 [4 datoshi]
    /// 21 : OpCode.STLOC1 [2 datoshi]
    /// 22 : OpCode.LDARG2 [2 datoshi]
    /// 23 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 25 : OpCode.LDLOC0 [2 datoshi]
    /// 26 : OpCode.PUSH2 [1 datoshi]
    /// 27 : OpCode.ADD [8 datoshi]
    /// 28 : OpCode.STLOC0 [2 datoshi]
    /// 29 : OpCode.ENDTRY 0A [4 datoshi]
    /// 2B : OpCode.LDARG4 [2 datoshi]
    /// 2C : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 2E : OpCode.LDLOC0 [2 datoshi]
    /// 2F : OpCode.PUSH3 [1 datoshi]
    /// 30 : OpCode.ADD [8 datoshi]
    /// 31 : OpCode.STLOC0 [2 datoshi]
    /// 32 : OpCode.ENDFINALLY [4 datoshi]
    /// 33 : OpCode.LDARG1 [2 datoshi]
    /// 34 : OpCode.JMPIFNOT 0E [2 datoshi]
    /// 36 : OpCode.PUSHDATA1 657863657074696F6E 'exception' [8 datoshi]
    /// 41 : OpCode.THROW [512 datoshi]
    /// 42 : OpCode.ENDTRY 14 [4 datoshi]
    /// 44 : OpCode.STLOC1 [2 datoshi]
    /// 45 : OpCode.LDARG3 [2 datoshi]
    /// 46 : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 48 : OpCode.LDLOC0 [2 datoshi]
    /// 49 : OpCode.PUSH4 [1 datoshi]
    /// 4A : OpCode.ADD [8 datoshi]
    /// 4B : OpCode.STLOC0 [2 datoshi]
    /// 4C : OpCode.ENDTRY 0A [4 datoshi]
    /// 4E : OpCode.LDARG5 [2 datoshi]
    /// 4F : OpCode.JMPIFNOT 06 [2 datoshi]
    /// 51 : OpCode.LDLOC0 [2 datoshi]
    /// 52 : OpCode.PUSH5 [1 datoshi]
    /// 53 : OpCode.ADD [8 datoshi]
    /// 54 : OpCode.STLOC0 [2 datoshi]
    /// 55 : OpCode.ENDFINALLY [4 datoshi]
    /// 56 : OpCode.LDLOC0 [2 datoshi]
    /// 57 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryWithTwoFinally")]
    public abstract BigInteger? TryWithTwoFinally(bool? throwInInner, bool? throwInOuter, bool? enterInnerCatch, bool? enterOuterCatch, bool? enterInnerFinally, bool? enterOuterFinally);

    #endregion
}
