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

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Storage"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPutByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testDeleteByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":39,""safe"":false},{""name"":""testGetByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":74,""safe"":false},{""name"":""testOver16Bytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":115,""safe"":false},{""name"":""testPutString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":207,""safe"":false},{""name"":""testDeleteString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":243,""safe"":false},{""name"":""testGetString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":275,""safe"":false},{""name"":""putLocalString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":313,""safe"":false},{""name"":""deleteLocalString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":343,""safe"":false},{""name"":""getLocalString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":369,""safe"":false},{""name"":""testPutByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":399,""safe"":false},{""name"":""testDeleteByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":437,""safe"":false},{""name"":""testGetByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":471,""safe"":false},{""name"":""testNewGetMethods"",""parameters"":[],""returntype"":""Boolean"",""offset"":516,""safe"":false},{""name"":""localGetPut"",""parameters"":[],""returntype"":""Boolean"",""offset"":996,""safe"":false},{""name"":""testNewGetByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":1454,""safe"":false},{""name"":""putLocalByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1532,""safe"":false},{""name"":""deleteLocalByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":1564,""safe"":false},{""name"":""getLocalByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":1592,""safe"":false},{""name"":""testPutReadOnly"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1624,""safe"":false},{""name"":""testIncrease"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":1669,""safe"":false},{""name"":""testDecrease"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":1747,""safe"":false},{""name"":""localIncrease"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":1844,""safe"":false},{""name"":""localDecrease"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":1915,""safe"":false},{""name"":""localIntegerOrZero"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":2004,""safe"":false},{""name"":""serializeTest"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2041,""safe"":false},{""name"":""localGetPutObject"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":2129,""safe"":false},{""name"":""testFind"",""parameters"":[],""returntype"":""ByteArray"",""offset"":2212,""safe"":false},{""name"":""localFind"",""parameters"":[],""returntype"":""ByteArray"",""offset"":2286,""safe"":false},{""name"":""testIndexPut"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":2370,""safe"":false},{""name"":""testIndexGet"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":2408,""safe"":false},{""name"":""localIndexPut"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":2446,""safe"":false},{""name"":""localIndexGet"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":2480,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]}],""trusts"":[],""extra"":{""Version"":""3.9.1"",""nef"":{""optimization"":""All""}}}");

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
    /// Script: VwIBDAIA/9swcGgRwHF42yhpwUVQi0F1VPWUQA==
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwIBDAJiYnBoEcBxeNsoacFFUItBdVT1lEA=
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 6262 'bb' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwMBDAIA/9swcGgRwHF42yhpwUVQi0HVjV7ocmrbMEA=
    /// INITSLOT 0301 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwMBDAJiYnBoEcBxeNsoacFFUItB1Y1e6HJq2zBA
    /// INITSLOT 0301 [64 datoshi]
    /// PUSHDATA1 6262 'bb' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwIBDAKgv9swcGgRwHF42yhpNANA
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 A0BF '??' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("localDecrease")]
    public abstract BigInteger? LocalDecrease(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMADARmaW5kcGgRwHEMAQHbMNsoDARrZXkxacFFUItBOQzjCgwBAtsw2ygMBGtleTJpwUVQi0E5DOMKFGnBRUEHdlLzcmpBnAjtnEVqQfNUvx1A
    /// INITSLOT 0300 [64 datoshi]
    /// PUSHDATA1 66696E64 'find' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSHDATA1 6B657931 'key1' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// PUSHDATA1 02 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSHDATA1 6B657932 'key2' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// PUSH4 [1 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: Vw4ADAIBqtswcGgRwHEIcgB7cwwLaGVsbG8gd29ybGR0DBQAAQIDBAUGBwgJAAECAwQFBgcICdsw2yhK2CQJSsoAFCgDOnUMIAABAgMEBQYHCAkAAQIDBAUGBwgJAAECAwQFBgcICQAB2zDbKErYJAlKygAgKAM6dgwhAAECAwQFBgcICQABAgMEBQYHCAkAAQIDBAUGBwgJAAEC2zDbKErYJAlKygAhKAM6dwdqDARib29sacFFUItBOQzjCmsMA2ludGnBRVCLQTkM4wpsDAZzdHJpbmdpwUVQi0E5DOMKbQwHdWludDE2MGnBRVCLQTkM4wpuDAd1aW50MjU2acFFUItBOQzjCm8HDAdlY3BvaW50acFFUItBOQzjCgwEYm9vbGnBRVCLQdWNXuiqqncIDANpbnRpwUVQi0HVjV7o2yF3CQwGc3RyaW5nacFFUItB1Y1e6HcKDAd1aW50MTYwacFFUItB1Y1e6HcLDAd1aW50MjU2acFFUItB1Y1e6HcMDAdlY3BvaW50acFFUItB1Y1e6HcNam8IlyQFCSIGa28JlyQFCSIGbG8KlyQFCSIGbW8LlyQFCSIGbm8MlyQECUBvB28Nl0A=
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
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 09 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC5 [2 datoshi]
    /// PUSHDATA1 0001020304050607080900010203040506070809000102030405060708090001 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 09 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC6 [2 datoshi]
    /// PUSHDATA1 000102030405060708090001020304050607080900010203040506070809000102 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 09 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 21 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC 07 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// PUSHDATA1 626F6F6C 'bool' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSHDATA1 696E74 'int' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PUSHDATA1 737472696E67 'string' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// LDLOC5 [2 datoshi]
    /// PUSHDATA1 75696E74313630 'uint160' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// LDLOC6 [2 datoshi]
    /// PUSHDATA1 75696E74323536 'uint256' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// LDLOC 07 [2 datoshi]
    /// PUSHDATA1 6563706F696E74 'ecpoint' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// PUSHDATA1 626F6F6C 'bool' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// NOT [4 datoshi]
    /// NOT [4 datoshi]
    /// STLOC 08 [2 datoshi]
    /// PUSHDATA1 696E74 'int' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// STLOC 09 [2 datoshi]
    /// PUSHDATA1 737472696E67 'string' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// STLOC 0A [2 datoshi]
    /// PUSHDATA1 75696E74313630 'uint160' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// STLOC 0B [2 datoshi]
    /// PUSHDATA1 75696E74323536 'uint256' [8 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL D58D5EE8 'System.Storage.Local.Get' [32768 datoshi]
    /// STLOC 0C [2 datoshi]
    /// PUSHDATA1 6563706F696E74 'ecpoint' [8 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwMCDAICu9swcGgRwHF5EcByanjbKGk0DXjbKGk0GnJqEM5A
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
    /// LDLOC2 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALL 0D [512 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALL 1A [512 datoshi]
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
    /// Script: VwIBDAKgv9swcGgRwHF42yhpNANA
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 A0BF '??' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("localIncrease")]
    public abstract BigInteger? LocalIncrease(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBDAVpbmRleHBoEcBxeNsoacFFUItB1Y1e6HJq2zBA
    /// INITSLOT 0301 [64 datoshi]
    /// PUSHDATA1 696E646578 'index' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwICDAVpbmRleHBoEcBxedsoSnjbKGnBRVCLQTkM4wpFQA==
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHDATA1 696E646578 'index' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// SWAP [2 datoshi]
    /// CAT [2048 datoshi]
    /// SYSCALL 390CE30A 'System.Storage.Local.Put' [32768 datoshi]
    /// DROP [2 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("localIndexPut")]
    public abstract void LocalIndexPut(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAKgv9swcGgRwHF42yhpwUVQi0HVjV7oStgmBUUQQNshQA==
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 A0BF '??' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwICDAIA/9swcGgRwHF52yh42yhpwUVQi0E5DOMKCEA=
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwICDAJiYnBoEcBxedsoeNsoacFFUItBOQzjCghA
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHDATA1 6262 'bb' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// PUSH1 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwQCDAIBqtswcEGb9mfOcWhpEsByeRHAc2t4ajQLeGo0G3NrEM5A
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
    /// LDLOC3 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CALL 0B [512 datoshi]
    /// LDARG0 [2 datoshi]
    /// LDLOC2 [2 datoshi]
    /// CALL 1B [512 datoshi]
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
    /// Script: VwIBDAKgr9swcGhBm/ZnzhLAcXjbKGk0A0A=
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 A0AF '??' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDecrease")]
    public abstract BigInteger? TestDecrease(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z84AERGIThBR0FASwHB42yhowUVTi1BBL1jF7UA=
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
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC0 [2 datoshi]
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
    /// Script: VwIBDAIA/9swcGhBm/ZnzhLAcXjbKGnBRVOLUEEvWMXtQA==
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwIBDAJhYXBoQZv2Z84SwHF42yhpwUVTi1BBL1jF7UA=
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 6161 'aa' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwIAQZv2Z85wDAEB2zDbKAwEa2V5MWhB5j8YhAwBAtsw2ygMBGtleTJoQeY/GIQUDANrZXloQd8wuJpxaUGcCO2cRWlB81S/HUA=
    /// INITSLOT 0200 [64 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSHDATA1 6B657931 'key1' [8 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSHDATA1 02 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
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
    /// Script: VwMBQfa0a+JwaAAREYhOEFHQUBLAcXjbKGnBRVOLUEGSXegxcmrbMEA=
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
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwQBDAIA/9swcEGb9mfOQXZMv+lxaGkSwHJ42yhqwUVTi1BBkl3oMXNr2zBA
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
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC2 [2 datoshi]
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
    /// Script: VwQBDAJhYXBB9rRr4nFoaRLAcnjbKGrBRVOLUEGSXegxc2vbMEA=
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
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC2 [2 datoshi]
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
    /// Script: VwIBDAKgr9swcGhBm/ZnzhLAcXjbKGk0A0A=
    /// INITSLOT 0201 [64 datoshi]
    /// PUSHDATA1 A0AF '??' [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// CALL 03 [512 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIncrease")]
    public abstract BigInteger? TestIncrease(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBDAJpaXBB9rRr4nFoaRLAcnjbKGrBRVOLUEGSXegxc2vbMEA=
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
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC2 [2 datoshi]
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
    /// Script: VwICDAJpaXBoQZv2Z84SwHF52yhKeNsoacFFU4tQQeY/GIRFCEA=
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHDATA1 6969 'ii' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// DROP [2 datoshi]
    /// PUSHT [1 datoshi]
    /// RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIndexPut")]
    public abstract bool? TestIndexPut(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUADAIA/9swcEGb9mfOcWhpEsByDAIAAdswc2sMCWJ5dGVBcnJheWrBRVOLUEHmPxiEDAlieXRlQXJyYXlqwUVTi1BBkl3oMdswdGxA
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
    /// LDLOC3 [2 datoshi]
    /// PUSHDATA1 627974654172726179 'byteArray' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSHDATA1 627974654172726179 'byteArray' [8 datoshi]
    /// LDLOC2 [2 datoshi]
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
    /// Script: Vw8ADAIA/9swcEGb9mfOcWhpEsByCHMAe3QMC2hlbGxvIHdvcmxkdQwUAAECAwQFBgcICQABAgMEBQYHCAnbMNsoStgkCUrKABQoAzp2DCAAAQIDBAUGBwgJAAECAwQFBgcICQABAgMEBQYHCAkAAdsw2yhK2CQJSsoAICgDOncHDCEAAQIDBAUGBwgJAAECAwQFBgcICQABAgMEBQYHCAkAAQLbMNsoStgkCUrKACEoAzp3CGsMBGJvb2xqwUVTi1BB5j8YhGwMA2ludGrBRVOLUEHmPxiEbQwGc3RyaW5nasFFU4tQQeY/GIRuDAd1aW50MTYwasFFU4tQQeY/GIRvBwwHdWludDI1NmrBRVOLUEHmPxiEbwgMB2VjcG9pbnRqwUVTi1BB5j8YhAwEYm9vbGrBRVOLUEGSXegxqqp3CQwDaW50asFFU4tQQZJd6DHbIXcKDAZzdHJpbmdqwUVTi1BBkl3oMXcLDAd1aW50MTYwasFFU4tQQZJd6DF3DAwHdWludDI1NmrBRVOLUEGSXegxdw0MB2VjcG9pbnRqwUVTi1BBkl3oMXcOa28JlyQFCSIGbG8KlyQFCSIGbW8LlyQFCSIGbm8MlyQFCSIHbwdvDZckBAlAbwhvDpdA
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
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 09 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 14 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC6 [2 datoshi]
    /// PUSHDATA1 0001020304050607080900010203040506070809000102030405060708090001 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 09 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 20 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC 07 [2 datoshi]
    /// PUSHDATA1 000102030405060708090001020304050607080900010203040506070809000102 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// DUP [2 datoshi]
    /// ISNULL [2 datoshi]
    /// JMPIF 09 [2 datoshi]
    /// DUP [2 datoshi]
    /// SIZE [4 datoshi]
    /// PUSHINT8 21 [1 datoshi]
    /// JMPEQ 03 [2 datoshi]
    /// THROW [512 datoshi]
    /// STLOC 08 [2 datoshi]
    /// LDLOC3 [2 datoshi]
    /// PUSHDATA1 626F6F6C 'bool' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// LDLOC4 [2 datoshi]
    /// PUSHDATA1 696E74 'int' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// LDLOC5 [2 datoshi]
    /// PUSHDATA1 737472696E67 'string' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// LDLOC6 [2 datoshi]
    /// PUSHDATA1 75696E74313630 'uint160' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// LDLOC 07 [2 datoshi]
    /// PUSHDATA1 75696E74323536 'uint256' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// LDLOC 08 [2 datoshi]
    /// PUSHDATA1 6563706F696E74 'ecpoint' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSHDATA1 626F6F6C 'bool' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// NOT [4 datoshi]
    /// NOT [4 datoshi]
    /// STLOC 09 [2 datoshi]
    /// PUSHDATA1 696E74 'int' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// CONVERT 21 'Integer' [8192 datoshi]
    /// STLOC 0A [2 datoshi]
    /// PUSHDATA1 737472696E67 'string' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// STLOC 0B [2 datoshi]
    /// PUSHDATA1 75696E74313630 'uint160' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// STLOC 0C [2 datoshi]
    /// PUSHDATA1 75696E74323536 'uint256' [8 datoshi]
    /// LDLOC2 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// STLOC 0D [2 datoshi]
    /// PUSHDATA1 6563706F696E74 'ecpoint' [8 datoshi]
    /// LDLOC2 [2 datoshi]
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
    /// Script: VwIADBg7ADIDIyMjIwIjIwIjIwIjIwIjIwIjIwLbMHAMCHRlc3RfbWFwQZv2Z84SwHFo2ygMAQHbMNsoacFFU4tQQeY/GIQMAQHbMNsoacFFU4tQQZJd6DHbMEA=
    /// INITSLOT 0200 [64 datoshi]
    /// PUSHDATA1 3B0032032323232302232302232302232302232302232302 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// PUSHDATA1 746573745F6D6170 'test_map' [8 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
    /// UNPACK [2048 datoshi]
    /// DROP [2 datoshi]
    /// REVERSE3 [2 datoshi]
    /// CAT [2048 datoshi]
    /// SWAP [2 datoshi]
    /// SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// PUSHDATA1 01 [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwECQZv2Z84AERGIThBR0FASwHB52yh42yhowUVTi1BB5j8YhAhA
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
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC0 [2 datoshi]
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
    /// Script: VwICDAIA/9swcGhBm/ZnzhLAcXnbKHjbKGnBRVOLUEHmPxiECEA=
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHDATA1 00FF [8 datoshi]
    /// CONVERT 30 'Buffer' [8192 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
    /// Script: VwMCDAIA/9swcEGb9mfOQXZMv+lxaGkSwHJ52yh42yhqwUVTi1BB5j8YhAhA
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
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC2 [2 datoshi]
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
    /// Script: VwICDAJhYXBoQZv2Z84SwHF52yh42yhpwUVTi1BB5j8YhAhA
    /// INITSLOT 0202 [64 datoshi]
    /// PUSHDATA1 6161 'aa' [8 datoshi]
    /// STLOC0 [2 datoshi]
    /// LDLOC0 [2 datoshi]
    /// SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// PUSH2 [1 datoshi]
    /// PACK [2048 datoshi]
    /// STLOC1 [2 datoshi]
    /// LDARG1 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDARG0 [2 datoshi]
    /// CONVERT 28 'ByteString' [8192 datoshi]
    /// LDLOC1 [2 datoshi]
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
