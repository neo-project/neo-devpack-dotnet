using Neo.Cryptography.ECC;
using Neo.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

#pragma warning disable CS0067

namespace Neo.SmartContract.Testing;

public abstract class Contract_Storage(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Storage"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPutByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testDeleteByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":41,""safe"":false},{""name"":""testGetByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":77,""safe"":false},{""name"":""testOver16Bytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":119,""safe"":false},{""name"":""testPutString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":207,""safe"":false},{""name"":""testDeleteString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":246,""safe"":false},{""name"":""testGetString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":280,""safe"":false},{""name"":""putLocalString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":319,""safe"":false},{""name"":""deleteLocalString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":351,""safe"":false},{""name"":""getLocalString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":378,""safe"":false},{""name"":""testPutByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":409,""safe"":false},{""name"":""testDeleteByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":450,""safe"":false},{""name"":""testGetByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":486,""safe"":false},{""name"":""testNewGetMethods"",""parameters"":[],""returntype"":""Boolean"",""offset"":532,""safe"":false},{""name"":""localGetPut"",""parameters"":[],""returntype"":""Boolean"",""offset"":1030,""safe"":false},{""name"":""testNewGetByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":1506,""safe"":false},{""name"":""putLocalByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1587,""safe"":false},{""name"":""deleteLocalByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":1621,""safe"":false},{""name"":""getLocalByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":1650,""safe"":false},{""name"":""testPutReadOnly"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1683,""safe"":false},{""name"":""testIncrease"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":1730,""safe"":false},{""name"":""testDecrease"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":1826,""safe"":false},{""name"":""localIncrease"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":1976,""safe"":false},{""name"":""localDecrease"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":2064,""safe"":false},{""name"":""localIntegerOrZero"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":2205,""safe"":false},{""name"":""testLocalPutBytes"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":2243,""safe"":false},{""name"":""testLocalDeleteBytes"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":2279,""safe"":false},{""name"":""testLocalGetBytes"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":2310,""safe"":false},{""name"":""serializeTest"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2345,""safe"":false},{""name"":""localGetPutObject"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2437,""safe"":false},{""name"":""testFind"",""parameters"":[],""returntype"":""ByteArray"",""offset"":2524,""safe"":false},{""name"":""localFind"",""parameters"":[],""returntype"":""ByteArray"",""offset"":2590,""safe"":false},{""name"":""testIndexPut"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":2671,""safe"":false},{""name"":""testIndexGet"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":2709,""safe"":false},{""name"":""localIndexPut"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":2748,""safe"":false},{""name"":""localIndexGet"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":2781,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]}],""trusts"":[],""extra"":{""Version"":""3.10.1"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Convert.FromBase64String(@"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").AsSerializable<Neo.SmartContract.NefFile>();

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAIA/9swcGgRwHFpeNsoUMFFUItBdVT1lEA=
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 7554F594 'System.Storage.Local.Delete' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("deleteLocalByteArray")]
    public abstract void DeleteLocalByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAJiYnBoEcBxaXjbKFDBRVCLQXVU9ZRA
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 6262 'bb' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 7554F594 'System.Storage.Local.Delete' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("deleteLocalString")]
    public abstract void DeleteLocalString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBDAIA/9swcGgRwHFpeNsoUMFFUItB1Y1e6HJq2zBA
    /// INITSLOT 0301 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getLocalByteArray")]
    public abstract byte[]? GetLocalByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBDAJiYnBoEcBxaXjbKFDBRVCLQdWNXuhyatswQA==
    /// INITSLOT 0301 [64 datoshi]
    /// PUSHDATA1 6262 'bb' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("getLocalString")]
    public abstract byte[]? GetLocalString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAKgv9swcGgRwHFpeNsoUDQDQA==
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 A0BF '??' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("localDecrease")]
    public abstract BigInteger? LocalDecrease(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMADARmaW5kcGgRwHFpDAEBDARrZXkxElLBRVCLQTkM4wppDAECDARrZXkyElLBRVCLQTkM4wppFFDBRUEHdlLzcmpBnAjtnEVqQfNUvx1A
    /// INITSLOT 0300 [64 datoshi]
    /// PUSHDATA1 66696E64 'find' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// PUSHDATA1 6B657931 'key1' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 02 [8 datoshi]
    /// PUSHDATA1 6B657932 'key2' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH4 [1 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SYSCALL 077652F3 'System.Storage.Local.Find' [32768 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("localFind")]
    public abstract byte[]? LocalFind();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Vw4ADAIBqtswcGgRwHEIcgB7cwwLaGVsbG8gd29ybGR0DBQAAQIDBAUGBwgJAAECAwQFBgcICdswStgkCtsoSsoAFCgDOnUMIAABAgMEBQYHCAkAAQIDBAUGBwgJAAECAwQFBgcICQAB2zBK2CQK2yhKygAgKAM6dgwhAAECAwQFBgcICQABAgMEBQYHCAkAAQIDBAUGBwgJAAEC2zBK2CQK2yhKygAhKAM6dwdpagwEYm9vbBJSwUVQi0E5DOMKaWsMA2ludBJSwUVQi0E5DOMKaWwMBnN0cmluZxJSwUVQi0E5DOMKaW0MB3VpbnQxNjASUsFFUItBOQzjCmluDAd1aW50MjU2ElLBRVCLQTkM4wppbwcMB2VjcG9pbnQSUsFFUItBOQzjCmkMBGJvb2xQwUVQi0HVjV7oqqp3CGkMA2ludFDBRVCLQdWNXujbIXcJaQwGc3RyaW5nUMFFUItB1Y1e6HcKaQwHdWludDE2MFDBRVCLQdWNXuh3C2kMB3VpbnQyNTZQwUVQi0HVjV7odwxpDAdlY3BvaW50UMFFUItB1Y1e6HcNam8IlyQFCSIGa28JlyQFCSIGbG8KlyQFCSIGbW8LlyQFCSIGbm8MlyQECUBvB28Nl0A=
    /// INITSLOT 0E00 [64 datoshi]
    /// PUSHDATA1 01AA [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSHDATA1 68656C6C6F20776F726C64 [8 datoshi]
    /// STLOC4 [2 datoshi]
    /// PUSHDATA1 0001020304050607080900010203040506070809 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0A [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC5 [2 datoshi]
    /// PUSHDATA1 0001020304050607080900010203040506070809000102030405060708090001 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0A [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC6 [2 datoshi]
    /// PUSHDATA1 000102030405060708090001020304050607080900010203040506070809000102 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0A [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 21 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC 07 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHDATA1 626F6F6C 'bool' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSHDATA1 696E74 'int' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PUSHDATA1 737472696E67 'string' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// PUSHDATA1 75696E74313630 'uint160' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC6 [2 datoshi]
    /// PUSHDATA1 75696E74323536 'uint256' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC 07 [2 datoshi]
    /// PUSHDATA1 6563706F696E74 'ecpoint' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 626F6F6C 'bool' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// NOT [4 datoshi]
    /// NOT [4 datoshi]
    /// STLOC 08 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 696E74 'int' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// STLOC 09 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 737472696E67 'string' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// STLOC 0A [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 75696E74313630 'uint160' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// STLOC 0B [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 75696E74323536 'uint256' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// STLOC 0C [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 6563706F696E74 'ecpoint' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// STLOC 0D [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC 09 [2 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC 0A [2 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// LDLOC 0B [2 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// LDLOC6 [2 datoshi]
    /// LDLOC 0C [2 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC 07 [2 datoshi]
    /// LDLOC 0D [2 datoshi]
    /// EQUAL [32 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("localGetPut")]
    public abstract bool? LocalGetPut();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCDAICu9swcGgRwHF5EcByaWp42ygSUjQOaXjbKFA0G3JqEM5A
    /// INITSLOT 0302 [64 datoshi]
    /// PUSHDATA1 02BB [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// CALL 0E [512 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// CALL 1B [512 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("localGetPutObject")]
    public abstract BigInteger? LocalGetPutObject(byte[]? key, BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAKgv9swcGgRwHFpeNsoUDQDQA==
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 A0BF '??' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("localIncrease")]
    public abstract BigInteger? LocalIncrease(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBDAVpbmRleHBoEcBxaXjbKFDBRVCLQdWNXuhyatswQA==
    /// INITSLOT 0301 [64 datoshi]
    /// PUSHDATA1 696E646578 'index' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("localIndexGet")]
    public abstract byte[]? LocalIndexGet(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDAVpbmRleHBoEcBxaXjbKHnbKFPBRVCLQTkM4wpA
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHDATA1 696E646578 'index' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// REVERSE3 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("localIndexPut")]
    public abstract void LocalIndexPut(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAKgv9swcGgRwHFpeNsoUMFFUItB1Y1e6ErYJgVFEEDbIUA=
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 A0BF '??' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIFNOT 05 [2 datoshi]
    /// DROP [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// RET [0 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("localIntegerOrZero")]
    public abstract BigInteger? LocalIntegerOrZero(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDAIA/9swcGgRwHFpedsoeNsoElLBRVCLQTkM4woIQA==
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("putLocalByteArray")]
    public abstract bool? PutLocalByteArray(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDAJiYnBoEcBxaXnbKHjbKBJSwUVQi0E5DOMKCEA=
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHDATA1 6262 'bb' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("putLocalString")]
    public abstract bool? PutLocalString(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQCDAIBqtswcEGb9mfOcWhpEsByeRHAc2preBJSNAxqeFA0HHNrEM5A
    /// INITSLOT 0402 [64 datoshi]
    /// PUSHDATA1 01AA [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// CALL 0C [512 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// CALL 1C [512 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// PICKITEM [64 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("serializeTest")]
    public abstract BigInteger? SerializeTest(byte[]? key, BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAKgr9swcEGb9mfOaFASwHFpeNsoUDQDQA==
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 A0AF '??' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDecrease")]
    public abstract BigInteger? TestDecrease(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z84AERGIThBR0FASwHBoeNsoUMFFU4tQQS9Yxe1A
    /// INITSLOT 0101 [64 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHINT8 11 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 2F58C5ED 'System.Storage.Delete' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeleteByte")]
    public abstract void TestDeleteByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAIA/9swcEGb9mfOaFASwHFpeNsoUMFFU4tQQS9Yxe1A
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 2F58C5ED 'System.Storage.Delete' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeleteByteArray")]
    public abstract void TestDeleteByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAJhYXBBm/ZnzmhQEsBxaXjbKFDBRVOLUEEvWMXtQA==
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 6161 'aa' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 2F58C5ED 'System.Storage.Delete' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeleteString")]
    public abstract void TestDeleteString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAQZv2Z85wDAEBDARrZXkxaEHmPxiEDAECDARrZXkyaEHmPxiEFAwDa2V5aEHfMLiacWlBnAjtnEVpQfNUvx1A
    /// INITSLOT 0200 [64 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// PUSHDATA1 6B657931 'key1' [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSHDATA1 02 [8 datoshi]
    /// PUSHDATA1 6B657932 'key2' [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSH4 [1 datoshi]
    /// PUSHDATA1 6B6579 'key' [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL DF30B89A 'System.Storage.Find' [32768 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// DROP [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testFind")]
    public abstract byte[]? TestFind();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBQfa0a+JwaAAREYhOEFHQUBLAcWl42yhQwUVTi1BBkl3oMXJq2zBA
    /// INITSLOT 0301 [64 datoshi]
    /// SYSCALL F6B46BE2 'System.Storage.GetReadOnlyContext' [16 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSHINT8 11 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetByte")]
    public abstract byte[]? TestGetByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBDAIA/9swcEGb9mfOQXZMv+lxaGkSwHJqeNsoUMFFU4tQQZJd6DFza9swQA==
    /// INITSLOT 0401 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// SYSCALL 764CBFE9 'System.Storage.AsReadOnly' [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetByteArray")]
    public abstract byte[]? TestGetByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBDAJhYXBB9rRr4nFoaRLAcmp42yhQwUVTi1BBkl3oMXNr2zBA
    /// INITSLOT 0401 [64 datoshi]
    /// PUSHDATA1 6161 'aa' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL F6B46BE2 'System.Storage.GetReadOnlyContext' [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetString")]
    public abstract byte[]? TestGetString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAKgr9swcEGb9mfOaFASwHFpeNsoUDQDQA==
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 A0AF '??' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIncrease")]
    public abstract BigInteger? TestIncrease(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBDAJpaXBB9rRr4nFoaRLAcmp42yhQwUVTi1BBkl3oMXNr2zBA
    /// INITSLOT 0401 [64 datoshi]
    /// PUSHDATA1 6969 'ii' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL F6B46BE2 'System.Storage.GetReadOnlyContext' [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIndexGet")]
    public abstract byte[]? TestIndexGet(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDAJpaXBBm/ZnzmhQEsBxaXjbKHnbKFPBRVOLUEHmPxiECEA=
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHDATA1 6969 'ii' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// REVERSE3 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIndexPut")]
    public abstract bool? TestIndexPut(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBABJwaBGIThBR0BHAcWl42yhQwUVQi0F1VPWUQA==
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHINT8 12 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 7554F594 'System.Storage.Local.Delete' [32768 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLocalDeleteBytes")]
    public abstract void TestLocalDeleteBytes(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBABJwaBGIThBR0BHAcWl42yhQwUVQi0HVjV7ocmrbMEA=
    /// INITSLOT 0301 [64 datoshi]
    /// PUSHINT8 12 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLocalGetBytes")]
    public abstract byte[]? TestLocalGetBytes(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICABJwaBGIThBR0BHAcWl52yh42ygSUsFFUItBOQzjCghA
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHINT8 12 [1 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testLocalPutBytes")]
    public abstract bool? TestLocalPutBytes(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUADAIA/9swcEGb9mfOcWhpEsByDAIAAdswc2prDAlieXRlQXJyYXkSUsFFU4tQQeY/GIRqDAlieXRlQXJyYXlQwUVTi1BBkl3oMdswdGxA
    /// INITSLOT 0500 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHDATA1 0001 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC3 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSHDATA1 627974654172726179 'byteArray' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHDATA1 627974654172726179 'byteArray' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC4 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNewGetByteArray")]
    public abstract byte[]? TestNewGetByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Vw8ADAIA/9swcEGb9mfOcWhpEsByCHMAe3QMC2hlbGxvIHdvcmxkdQwUAAECAwQFBgcICQABAgMEBQYHCAnbMErYJArbKErKABQoAzp2DCAAAQIDBAUGBwgJAAECAwQFBgcICQABAgMEBQYHCAkAAdswStgkCtsoSsoAICgDOncHDCEAAQIDBAUGBwgJAAECAwQFBgcICQABAgMEBQYHCAkAAQLbMErYJArbKErKACEoAzp3CGprDARib29sElLBRVOLUEHmPxiEamwMA2ludBJSwUVTi1BB5j8YhGptDAZzdHJpbmcSUsFFU4tQQeY/GIRqbgwHdWludDE2MBJSwUVTi1BB5j8YhGpvBwwHdWludDI1NhJSwUVTi1BB5j8YhGpvCAwHZWNwb2ludBJSwUVTi1BB5j8YhGoMBGJvb2xQwUVTi1BBkl3oMaqqdwlqDANpbnRQwUVTi1BBkl3oMdshdwpqDAZzdHJpbmdQwUVTi1BBkl3oMXcLagwHdWludDE2MFDBRVOLUEGSXegxdwxqDAd1aW50MjU2UMFFU4tQQZJd6DF3DWoMB2VjcG9pbnRQwUVTi1BBkl3oMXcOa28JlyQFCSIGbG8KlyQFCSIGbW8LlyQFCSIGbm8MlyQFCSIHbwdvDZckBAlAbwhvDpdA
    /// INITSLOT 0F00 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC2 [2 datoshi]
    /// PUSHT [1 datoshi]
    /// STLOC3 [2 datoshi]
    /// PUSHINT8 7B [1 datoshi]
    /// STLOC4 [2 datoshi]
    /// PUSHDATA1 68656C6C6F20776F726C64 [8 datoshi]
    /// STLOC5 [2 datoshi]
    /// PUSHDATA1 0001020304050607080900010203040506070809 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0A [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC6 [2 datoshi]
    /// PUSHDATA1 0001020304050607080900010203040506070809000102030405060708090001 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0A [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC 07 [2 datoshi]
    /// PUSHDATA1 000102030405060708090001020304050607080900010203040506070809000102 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 0A [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 21 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC 08 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSHDATA1 626F6F6C 'bool' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PUSHDATA1 696E74 'int' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// PUSHDATA1 737472696E67 'string' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC6 [2 datoshi]
    /// PUSHDATA1 75696E74313630 'uint160' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC 07 [2 datoshi]
    /// PUSHDATA1 75696E74323536 'uint256' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSHDATA1 6563706F696E74 'ecpoint' [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHDATA1 626F6F6C 'bool' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// NOT [4 datoshi]
    /// NOT [4 datoshi]
    /// STLOC 09 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHDATA1 696E74 'int' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// STLOC 0A [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHDATA1 737472696E67 'string' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// STLOC 0B [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHDATA1 75696E74313630 'uint160' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// STLOC 0C [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHDATA1 75696E74323536 'uint256' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// STLOC 0D [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHDATA1 6563706F696E74 'ecpoint' [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// STLOC 0E [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// LDLOC 09 [2 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// LDLOC4 [2 datoshi]
    /// LDLOC 0A [2 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// LDLOC5 [2 datoshi]
    /// LDLOC 0B [2 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 06 [2 datoshi]
    /// LDLOC6 [2 datoshi]
    /// LDLOC 0C [2 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 05 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// JMP 07 [2 datoshi]
    /// LDLOC 07 [2 datoshi]
    /// LDLOC 0D [2 datoshi]
    /// EQUAL [32 datoshi]
    /// JMPIF 04 [2 datoshi]
    /// PUSHF [1 datoshi]
    /// RET [0 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// LDLOC 0E [2 datoshi]
    /// EQUAL [32 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNewGetMethods")]
    public abstract bool? TestNewGetMethods();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADBg7ADIDIyMjIwIjIwIjIwIjIwIjIwIjIwLbMHBBm/ZnzgwIdGVzdF9tYXBQEsBxaWjbKAwBARJSwUVTi1BB5j8YhGkMAQFQwUVTi1BBkl3oMdswQA==
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 3B0032032323232302232302232302232302232302232302 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHDATA1 746573745F6D6170 'test_map' [8 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// SWAP [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOver16Bytes")]
    public abstract byte[]? TestOver16Bytes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQZv2Z84AERGIThBR0FASwHBoedsoeNsoElLBRVOLUEHmPxiECEA=
    /// INITSLOT 0102 [64 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSHINT8 11 [1 datoshi]
    /// PUSH1 [1 datoshi]
    /// NEWBUFFER [256 datoshi]
    /// TUCK [2 datoshi]
    /// PUSH0 [1 datoshi]
    /// ROT [2 datoshi]
    /// SETITEM [8192 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPutByte")]
    public abstract bool? TestPutByte(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDAIA/9swcEGb9mfOaFASwHFpedsoeNsoElLBRVOLUEHmPxiECEA=
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPutByteArray")]
    public abstract bool? TestPutByteArray(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCDAIA/9swcEGb9mfOQXZMv+lxaGkSwHJqedsoeNsoElLBRVOLUEHmPxiECEA=
    /// INITSLOT 0302 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// SYSCALL 764CBFE9 'System.Storage.AsReadOnly' [16 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC2 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPutReadOnly")]
    public abstract bool? TestPutReadOnly(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDAJhYXBBm/ZnzmhQEsBxaXnbKHjbKBJSwUVTi1BB5j8YhAhA
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHDATA1 6161 'aa' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SWAP [2 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSH2 [1 datoshi]
    /// ROLL [16 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPutString")]
    public abstract bool? TestPutString(byte[]? key, byte[]? value);

    #endregion
}
