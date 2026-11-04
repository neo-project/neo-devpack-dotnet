using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_UInt(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_UInt"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""isValidUInt256"",""parameters"":[{""name"":""value"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""isValidUInt160"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":17,""safe"":false},{""name"":""isValidAndNotZeroUInt256"",""parameters"":[{""name"":""value"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":34,""safe"":false},{""name"":""isValidAndNotZeroUInt160"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":65,""safe"":false},{""name"":""isZeroUInt256"",""parameters"":[{""name"":""value"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":96,""safe"":false},{""name"":""isZeroUInt160"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":103,""safe"":false},{""name"":""notZeroUInt256"",""parameters"":[{""name"":""value"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":110,""safe"":false},{""name"":""notZeroUInt160"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":116,""safe"":false},{""name"":""toAddress"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""String"",""offset"":122,""safe"":false},{""name"":""parseUInt160"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Hash160"",""offset"":162,""safe"":false},{""name"":""parseUInt256"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Hash256"",""offset"":756,""safe"":false},{""name"":""parseECPoint"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""PublicKey"",""offset"":863,""safe"":false},{""name"":""tryParseUInt160"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Boolean"",""offset"":965,""safe"":false},{""name"":""tryParseUInt256"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Boolean"",""offset"":995,""safe"":false},{""name"":""tryParseECPoint"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1025,""safe"":false},{""name"":""toUInt160"",""parameters"":[{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1055,""safe"":false},{""name"":""toUInt256"",""parameters"":[{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1093,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""base58CheckDecode"",""base58CheckEncode""]}],""trusts"":[],""extra"":{""Version"":""3.10.2"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0VuY29kZQEAAQXA7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0RlY29kZQEAAQUAAP1rBFcAAXhK2SgkBUUJQMoAILNAVwABeErZKCQFRQlAygAUs0BXAAF4NANAVwABeErZKCQGRQkiBsoAILMkBAlAeLFAVwABeDQDQFcAAXhK2SgkBkUJIgbKABSzJAQJQHixQFcAAXixqkBXAAF4sapAVwABeLFAVwABeLFAVwABeDQDQFcAAUFMSZLceDQDQFcBAhGIShB50HBoeItwaNsoNwAAQFcAAXg0A0BXAgFDcXhwaNgmGgwVVmFsdWUgY2Fubm90IGJlIG51bGwuOjtFAHg1kAAAAHBoyjWkAQAAtCYjDB5VSW50MTYwIG11c3QgYmUgMjAgYnl0ZXMgbG9uZy46aErRStgkCtsoSsoAFCgNOnBDaTIFRSL7PQQ9RHjKACKzJhx4QUxJktxQNVQBAABK2CQK2yhKygAUKAM6QAweSW52YWxpZCBVSW50MTYwIHN0cmluZyBmb3JtYXQuOkBXCAF4NZAAAABweMpon3FpEqImHwwaSW52YWxpZCBoZXggc3RyaW5nIGxlbmd0aC46aRKhcmpKyhQyAzqIcxB0Ik5obBKgnnV4bc40dXZ4bZzONG53B2tsbhSobweSShABAAG7JAM60GxKnErKFDIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3RFbGowsmtAVwEBeMoStSYEEEB4EM4AMLQmBBBAeBHOcGgAeLMmBQgiBmgAWLMmBBJAEEBXAAF4ADC4JAUJIgZ4ADm2Jgd4ADCfQHgAYbgkBQkiBngAZrYmCXgAYZ8ankB4AEG4JAUJIgZ4AEa2Jgl4AEGfGp5ADBZJbnZhbGlkIGhleCBjaGFyYWN0ZXIuOgAUQFcBAng3AQDbMHBoygAVtCYcDBdJbnZhbGlkIGFkZHJlc3MgbGVuZ3RoLjpoEM55tCYdDBhJbnZhbGlkIGFkZHJlc3MgdmVyc2lvbi46aBFoyp1KyhQyHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+MQFcAAXg0A0BXAQF4cGjYJhoMFVZhbHVlIGNhbm5vdCBiZSBudWxsLjp4NUP+//9waMo0N7QmIwweVUludDI1NiBtdXN0IGJlIDMyIGJ5dGVzIGxvbmcuOmhK0UrYJArbKErKACAoAzpAACBAVwABeDQDQFcBAXhwaNgmGgwVVmFsdWUgY2Fubm90IGJlIG51bGwuOng12P3//3BoygAhtCYjDB5FQ1BvaW50IG11c3QgYmUgMzMgYnl0ZXMgbG9uZy46aErYJArbKErKACEoAzpAVwIBQ3E7DQB4Ndv8//9FCD0NcENpMgVFIvsJPQJAVwIBQ3E7DQB4NQ////9FCD0NcENpMgVFIvsJPQJAVwIBQ3E7DQB4NVz///9FCD0NcENpMgVFIvsJPQJAVwIBQ3E7FQB4StgkCtsoSsoAFCgDOkUIPQ1wQ2kyBUUi+wk9AkBXAgFDcTsVAHhK2CQK2yhKygAgKAM6RQg9DXBDaTIFRSL7CT0CQHTHDk0=").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isValidAndNotZeroUInt160")]
    public abstract bool? IsValidAndNotZeroUInt160(UInt160? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isValidAndNotZeroUInt256")]
    public abstract bool? IsValidAndNotZeroUInt256(UInt256? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErZKCQFRQlAygAUs0A=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isValidUInt160")]
    public abstract bool? IsValidUInt160(UInt160? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeErZKCQFRQlAygAgs0A=
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISTYPE 28 'ByteString' [2 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// NUMEQUAL [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("isValidUInt256")]
    public abstract bool? IsValidUInt256(UInt256? value);

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
    [DisplayName("isZeroUInt160")]
    public abstract bool? IsZeroUInt160(UInt160? value);

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
    [DisplayName("isZeroUInt256")]
    public abstract bool? IsZeroUInt256(UInt256? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeLFA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// NZ [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("notZeroUInt160")]
    public abstract bool? NotZeroUInt160(UInt160? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeLFA
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// NZ [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("notZeroUInt256")]
    public abstract bool? NotZeroUInt256(UInt256? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("parseECPoint")]
    public abstract ECPoint? ParseECPoint(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("parseUInt160")]
    public abstract UInt160? ParseUInt160(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("parseUInt256")]
    public abstract UInt256? ParseUInt256(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwABeDQDQA==
    /// INITSLOT 0001 [64 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("toAddress")]
    public abstract string? ToAddress(UInt160? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBQ3E7FQB4StgkCtsoSsoAFCgDOkUIPQ1wQ2kyBUUi+wk9AkA=
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 1500 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0A [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// ENDTRY 0D [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHF [1 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("toUInt160")]
    public abstract bool? ToUInt160(byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBQ3E7FQB4StgkCtsoSsoAICgDOkUIPQ1wQ2kyBUUi+wk9AkA=
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 1500 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0A [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// ENDTRY 0D [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHF [1 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("toUInt256")]
    public abstract bool? ToUInt256(byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBQ3E7DQB4NVz///9FCD0NcENpMgVFIvsJPQJA
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0D00 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 5CFFFFFF [512 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// ENDTRY 0D [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHF [1 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryParseECPoint")]
    public abstract bool? TryParseECPoint(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBQ3E7DQB4Ndv8//9FCD0NcENpMgVFIvsJPQJA
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0D00 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L DBFCFFFF [512 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// ENDTRY 0D [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHF [1 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryParseUInt160")]
    public abstract bool? TryParseUInt160(string? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBQ3E7DQB4NQ////9FCD0NcENpMgVFIvsJPQJA
    /// INITSLOT 0201 [64 datoshi]
    /// DEPTH [2 datoshi]
    /// STLOC1 [2 datoshi]
    /// TRY 0D00 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 0FFFFFFF [512 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// ENDTRY 0D [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// DEPTH [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// JMPLE 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// JMP FB [2 datoshi]
    /// PUSHF [1 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryParseUInt256")]
    public abstract bool? TryParseUInt256(string? value);

    #endregion
}
