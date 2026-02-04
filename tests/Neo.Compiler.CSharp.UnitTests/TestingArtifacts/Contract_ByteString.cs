using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_ByteString(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_ByteString"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""literal00ToFF"",""parameters"":[],""returntype"":""ByteArray"",""offset"":0,""safe"":false},{""name"":""literalWithOtherChar"",""parameters"":[],""returntype"":""ByteArray"",""offset"":260,""safe"":false},{""name"":""characterCount"",""parameters"":[{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":274,""safe"":false},{""name"":""startsWith"",""parameters"":[{""name"":""value"",""type"":""ByteArray""},{""name"":""toFind"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":289,""safe"":false},{""name"":""endsWith"",""parameters"":[{""name"":""value"",""type"":""ByteArray""},{""name"":""toFind"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":308,""safe"":false},{""name"":""contains"",""parameters"":[{""name"":""value"",""type"":""ByteArray""},{""name"":""toFind"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":395,""safe"":false},{""name"":""indexOf"",""parameters"":[{""name"":""value"",""type"":""ByteArray""},{""name"":""toFind"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":414,""safe"":false},{""name"":""lastIndexOf"",""parameters"":[{""name"":""value"",""type"":""ByteArray""},{""name"":""toFind"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":431,""safe"":false},{""name"":""split"",""parameters"":[{""name"":""value"",""type"":""ByteArray""},{""name"":""separator"",""type"":""ByteArray""},{""name"":""removeEmptyEntries"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":451,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""memorySearch"",""stringSplit"",""strLen""]}],""trusts"":[],""extra"":{""Version"":""3.9.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAXA7znO4OTpJcbCoGp54UQN2G/OrAZzdHJMZW4BAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwMbWVtb3J5U2VhcmNoAgABD8DvOc7g5OklxsKgannhRA3Yb86sDG1lbW9yeVNlYXJjaAMAAQ/A7znO4OTpJcbCoGp54UQN2G/OrAxtZW1vcnlTZWFyY2gEAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwLc3RyaW5nU3BsaXQDAAEPAAD91gENAAEAAQIDBAUGBwgJCgsMDQ4PEBESExQVFhcYGRobHB0eHyAhIiMkJSYnKCkqKywtLi8wMTIzNDU2Nzg5Ojs8PT4/QEFCQ0RFRkdISUpLTE1OT1BRUlNUVVZXWFlaW1xdXl9gYWJjZGVmZ2hpamtsbW5vcHFyc3R1dnd4eXp7fH1+f4CBgoOEhYaHiImKi4yNjo+QkZKTlJWWl5iZmpucnZ6foKGio6SlpqeoqaqrrK2ur7CxsrO0tba3uLm6u7y9vr/AwcLDxMXGx8jJysvMzc7P0NHS09TV1tfY2drb3N3e3+Dh4uPk5ebn6Onq6+zt7u/w8fLz9PX29/j5+vv8/f7/QAwL5L2g5aW9AMKAw79AVwABeDQDQFcAAXg3AABAVwACeXg0A0BXAAJ5eDcBALGqQFcAAnl4NANAVwECecoQlyYECEB4ynnKn0oCAAAAgC4EIgpKAv///38yHgP/////AAAAAJFKAv///38yDAMAAAAAAQAAAJ9waBC1JgQJQGh5eDcCAGizQFcAAnl4NANAVwACeXg3AQAPmEBXAAJ5eDQDQFcAAnl4NwEAQFcAAnl4NANAVwACCHjKeXg3AwBAVwADenl4NANAVwADenl4NwQAQIQB9WI=").AsSerializable<Neo.SmartContract.NefFile>();

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
    [DisplayName("characterCount")]
    public abstract BigInteger? CharacterCount(byte[]? value);

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
    [DisplayName("contains")]
    public abstract bool? Contains(byte[]? value, byte[]? toFind);

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
    [DisplayName("endsWith")]
    public abstract bool? EndsWith(byte[]? value, byte[]? toFind);

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
    [DisplayName("indexOf")]
    public abstract BigInteger? IndexOf(byte[]? value, byte[]? toFind);

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
    [DisplayName("lastIndexOf")]
    public abstract BigInteger? LastIndexOf(byte[]? value, byte[]? toFind);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DQABAAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpKissLS4vMDEyMzQ1Njc4OTo7PD0+P0BBQkNERUZHSElKS0xNTk9QUVJTVFVWV1hZWltcXV5fYGFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6e3x9fn+AgYKDhIWGh4iJiouMjY6PkJGSk5SVlpeYmZqbnJ2en6ChoqOkpaanqKmqq6ytrq+wsbKztLW2t7i5uru8vb6/wMHCw8TFxsfIycrLzM3Oz9DR0tPU1dbX2Nna29zd3t/g4eLj5OXm5+jp6uvs7e7v8PHy8/T19vf4+fr7/P3+/0A=
    /// PUSHDATA2 000102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F202122232425262728292A2B2C2D2E2F303132333435363738393A3B3C3D3E3F404142434445464748494A4B4C4D4E4F505152535455565758595A5B5C5D5E5F606162636465666768696A6B6C6D6E6F707172737475767778797A7B7C7D7E7F808182838485868788898A8B8C8D8E8F909192939495969798999A9B9C9D9E9FA0A1A2A3A4A5A6A7A8A9AAABACADAEAFB0B1B2B3B4B5B6B7B8B9BABBBCBDBEBFC0C1C2C3C4C5C6C7C8C9CACBCCCDCECFD0D1D2D3D4D5D6D7D8D9DADBDCDDDEDFE0E1E2E3E4E5E6E7E8E9EAEBECEDEEEFF0F1F2F3F4F5F6F7F8F9FAFBFCFDFEFF [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("literal00ToFF")]
    public abstract byte[]? Literal00ToFF();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: DAvkvaDlpb0AwoDDv0A=
    /// PUSHDATA1 E4BDA0E5A5BD00C280C3BF [8 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("literalWithOtherChar")]
    public abstract byte[]? LiteralWithOtherChar();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwADenl4NANA
    /// INITSLOT 0003 [64 datoshi]
    /// LDARG2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("split")]
    public abstract IList<object>? Split(byte[]? value, byte[]? separator, bool? removeEmptyEntries);

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
    [DisplayName("startsWith")]
    public abstract bool? StartsWith(byte[]? value, byte[]? toFind);

    #endregion
}
