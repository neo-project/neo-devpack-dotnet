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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_UInt"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""isValidUInt256"",""parameters"":[{""name"":""value"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""isValidUInt160"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":17,""safe"":false},{""name"":""isValidAndNotZeroUInt256"",""parameters"":[{""name"":""value"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":34,""safe"":false},{""name"":""isValidAndNotZeroUInt160"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":65,""safe"":false},{""name"":""isZeroUInt256"",""parameters"":[{""name"":""value"",""type"":""Hash256""}],""returntype"":""Boolean"",""offset"":96,""safe"":false},{""name"":""isZeroUInt160"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""Boolean"",""offset"":103,""safe"":false},{""name"":""toAddress"",""parameters"":[{""name"":""value"",""type"":""Hash160""}],""returntype"":""String"",""offset"":110,""safe"":false},{""name"":""parseUInt160"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Hash160"",""offset"":150,""safe"":false},{""name"":""parseUInt256"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Hash256"",""offset"":1442,""safe"":false},{""name"":""parseECPoint"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""PublicKey"",""offset"":1551,""safe"":false},{""name"":""tryParseUInt160"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1655,""safe"":false},{""name"":""tryParseUInt256"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1676,""safe"":false},{""name"":""tryParseECPoint"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Boolean"",""offset"":1697,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""base58CheckDecode"",""base58CheckEncode""]}],""trusts"":[],""extra"":{""Version"":""3.9.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0VuY29kZQEAAQ/A7znO4OTpJcbCoGp54UQN2G/OrBFiYXNlNThDaGVja0RlY29kZQEAAQ8AAP22BlcAAXhK2SgkBUUJQMoAILNAVwABeErZKCQFRQlAygAUs0BXAAF4NANAVwABeErZKCQGRQkiBsoAILMkBAlAeLFAVwABeDQDQFcAAXhK2SgkBkUJIgbKABSzJAQJQHixQFcAAXixqkBXAAF4sapAVwABeDQDQFcAAUFMSZLceDQDQFcBAhGIShB50HBoeItwaNsoNwAAQFcAAXg0A0BXAQF4cGjYJhoMFVZhbHVlIGNhbm5vdCBiZSBudWxsLjo7RwB42zA1iAAAAHBoyjVRBAAAmCYjDB5VSW50MTYwIG11c3QgYmUgMjAgYnl0ZXMgbG9uZy46aErR2yhK2CQMSsoAFCgGOnA9BD1DeMoAIpcmG0FMSZLceDUJBAAA2yhK2CQJSsoAFCgDOkAMHkludmFsaWQgVUludDE2MCBzdHJpbmcgZm9ybWF0LjpAVwYBeDUuAgAAcHjKaJ9KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfEqIQmCYfDBpJbnZhbGlkIGhleCBzdHJpbmcgbGVuZ3RoLjp4ymifSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnxJKDyoLSwIAAACAKgM6oXFpiHIQcyOCAQAAeGhrEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ/ONVEBAAB0eGhrEqBKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfnkoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ+cSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn841uQAAAHVsFKhKAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfbZJKEC4EIghKAf8AMgYB/wCRSmprUdBFa0qcSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn3NFa2m1JYD+//9qQFcAAXjKErUmBBBAeBDOADCXJAUJIhN4Ec4AeJcmBQgiCHgRzgBYlyYEEkAQQFcAAXgAMLgkBQkiBngAObYmRXgAMJ9KAgAAAIAuBCIKSgL///9/Mh4D/////wAAAACRSgL///9/MgwDAAAAAAEAAACfShAuBCIISgH/ADIGAf8AkUB4AGG4JAUJIgZ4AGa2JnV4AGGfSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAnxqeSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn0oQLgQiCEoB/wAyBgH/AJFAeABBuCQFCSIGeABGtiZ1eABBn0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ8ankoCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9KEC4EIghKAf8AMgYB/wCRQAwWSW52YWxpZCBoZXggY2hhcmFjdGVyLjoAFEBXAQJ4NwEA2zBwaMoAFZgmHAwXSW52YWxpZCBhZGRyZXNzIGxlbmd0aC46aBDOeZgmHQwYSW52YWxpZCBhZGRyZXNzIHZlcnNpb24uOmgRaMqdSgIAAACALgQiCkoC////fzIeA/////8AAAAAkUoC////fzIMAwAAAAABAAAAn4xAVwABeDQDQFcBAXhwaNgmGgwVVmFsdWUgY2Fubm90IGJlIG51bGwuOnjbMDV/+///cGjKNDeYJiMMHlVJbnQyNTYgbXVzdCBiZSAzMiBieXRlcyBsb25nLjpoStHbKErYJAlKygAgKAM6QAAgQFcAAXg0A0BXAQF4cGjYJhoMFVZhbHVlIGNhbm5vdCBiZSBudWxsLjp42zA1Evv//3BoygAhmCYjDB5FQ1BvaW50IG11c3QgYmUgMzMgYnl0ZXMgbG9uZy46aNsoStgkCUrKACEoAzpAVwEBOw0AeDUf+v//RQg9BnAJPQJAVwEBOw0AeDUW////RQg9BnAJPQJAVwEBOw0AeDVu////RQg9BnAJPQJAlF+SOA==").AsSerializable<Neo.SmartContract.NefFile>();

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
    /// Script: VwEBOw0AeDVu////RQg9BnAJPQJA
    /// INITSLOT 0101 [64 datoshi]
    /// TRY 0D00 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 6EFFFFFF [512 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// ENDTRY 06 [4 datoshi]
    /// STLOC0 [2 datoshi]
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
    /// Script: VwEBOw0AeDUf+v//RQg9BnAJPQJA
    /// INITSLOT 0101 [64 datoshi]
    /// TRY 0D00 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 1FFAFFFF [512 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// ENDTRY 06 [4 datoshi]
    /// STLOC0 [2 datoshi]
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
    /// Script: VwEBOw0AeDUW////RQg9BnAJPQJA
    /// INITSLOT 0101 [64 datoshi]
    /// TRY 0D00 [4 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL_L 16FFFFFF [512 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// ENDTRY 06 [4 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// ENDTRY 02 [4 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("tryParseUInt256")]
    public abstract bool? TryParseUInt256(string? value);

    #endregion
}
