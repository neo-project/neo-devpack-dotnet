using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Storage(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Storage"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPutByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testDeleteByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":39,""safe"":false},{""name"":""testGetByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":74,""safe"":false},{""name"":""testOver16Bytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":115,""safe"":false},{""name"":""testPutString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":207,""safe"":false},{""name"":""testDeleteString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":243,""safe"":false},{""name"":""testGetString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":275,""safe"":false},{""name"":""testPutByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":313,""safe"":false},{""name"":""testDeleteByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":351,""safe"":false},{""name"":""testGetByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":385,""safe"":false},{""name"":""testNewGetMethods"",""parameters"":[],""returntype"":""Boolean"",""offset"":430,""safe"":false},{""name"":""testNewGetByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":910,""safe"":false},{""name"":""testPutReadOnly"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":988,""safe"":false},{""name"":""serializeTest"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1033,""safe"":false},{""name"":""testFind"",""parameters"":[],""returntype"":""ByteArray"",""offset"":1125,""safe"":false},{""name"":""testIndexPut"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1199,""safe"":false},{""name"":""testIndexGet"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":1237,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwLZGVzZXJpYWxpemUBAAEPAAD9+wRXAQJBm/ZnzgAREYhOEFHQUBLAcHnbKHjbKGjBRVOLUEHmPxiECEBXAQFBm/ZnzgAREYhOEFHQUBLAcHjbKGjBRVOLUEEvWMXtQFcDAUH2tGvicGgAERGIThBR0FASwHF42yhpwUVTi1BBkl3oMXJq2zBAVwIADBg7ADIDIyMjIwIjIwIjIwIjIwIjIwIjIwLbMHAMCHRlc3RfbWFwQZv2Z84SwHFo2ygMAQHbMNsoacFFU4tQQeY/GIQMAQHbMNsoacFFU4tQQZJd6DHbMEBXAgIMAmFhcGhBm/ZnzhLAcXnbKHjbKGnBRVOLUEHmPxiECEBXAgEMAmFhcGhBm/ZnzhLAcXjbKGnBRVOLUEEvWMXtQFcEAQwCYWFwQfa0a+JxaGkSwHJ42yhqwUVTi1BBkl3oMXNr2zBAVwICDAIA/9swcGhBm/ZnzhLAcXnbKHjbKGnBRVOLUEHmPxiECEBXAgEMAgD/2zBwaEGb9mfOEsBxeNsoacFFU4tQQS9Yxe1AVwQBDAIA/9swcEGb9mfOQXZMv+lxaGkSwHJ42yhqwUVTi1BBkl3oMXNr2zBAVw8ADAIA/9swcEGb9mfOcWhpEsByCHMAe3QMC2hlbGxvIHdvcmxkdQwUAAECAwQFBgcICQABAgMEBQYHCAnbMNsoStgkCUrKABQoAzp2DCAAAQIDBAUGBwgJAAECAwQFBgcICQABAgMEBQYHCAkAAdsw2yhK2CQJSsoAICgDOncHDCEAAQIDBAUGBwgJAAECAwQFBgcICQABAgMEBQYHCAkAAQLbMNsoStgkCUrKACEoAzp3CGsMBGJvb2xqwUVTi1BB5j8YhGwMA2ludGrBRVOLUEHmPxiEbQwGc3RyaW5nasFFU4tQQeY/GIRuDAd1aW50MTYwasFFU4tQQeY/GIRvBwwHdWludDI1NmrBRVOLUEHmPxiEbwgMB2VjcG9pbnRqwUVTi1BB5j8YhAwEYm9vbGrBRVOLUEGSXegxqqp3CQwDaW50asFFU4tQQZJd6DHbIXcKDAZzdHJpbmdqwUVTi1BBkl3oMXcLDAd1aW50MTYwasFFU4tQQZJd6DF3DAwHdWludDI1NmrBRVOLUEGSXegxdw0MB2VjcG9pbnRqwUVTi1BBkl3oMXcOa28JlyQFCSIGbG8KlyQFCSIGbW8LlyQFCSIGbm8MlyQFCSIHbwdvDZckBAlAbwhvDpdAVwUADAIA/9swcEGb9mfOcWhpEsByDAIAAdswc2sMCWJ5dGVBcnJheWrBRVOLUEHmPxiEDAlieXRlQXJyYXlqwUVTi1BBkl3oMdswdGxAVwMCDAIA/9swcEGb9mfOQXZMv+lxaGkSwHJ52yh42yhqwUVTi1BB5j8YhAhAVwQCDAIBqtswcEGb9mfOcWhpEsByEBHAShB50HNreGo0C3hqNBtzaxDOQFcAA3o3AAB5eMFFU4tQQeY/GIRAVwICeXjBRVOLUEGSXegxcGhxadgmBAtAaDcBAEBXAgBBm/ZnznAMAQHbMNsoDARrZXkxaEHmPxiEDAEC2zDbKAwEa2V5MmhB5j8YhBQMA2tleWhB3zC4mnFpQZwI7ZxFaUHzVL8dQFcCAgwCaWlwaEGb9mfOEsBxedsoSnjbKGnBRVOLUEHmPxiERQhAVwQBDAJpaXBB9rRr4nFoaRLAcnjbKGrBRVOLUEGSXegxc2vbMEAKXeJG"));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQCDAIBqtswcEGb9mfOcWhpEsByEBHAShB50HNreGo0C3hqNBtzaxDOQA==
    /// 00 : INITSLOT 0402 [64 datoshi]
    /// 03 : PUSHDATA1 01AA [8 datoshi]
    /// 07 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0F : STLOC1 [2 datoshi]
    /// 10 : LDLOC0 [2 datoshi]
    /// 11 : LDLOC1 [2 datoshi]
    /// 12 : PUSH2 [1 datoshi]
    /// 13 : PACK [2048 datoshi]
    /// 14 : STLOC2 [2 datoshi]
    /// 15 : PUSH0 [1 datoshi]
    /// 16 : PUSH1 [1 datoshi]
    /// 17 : PACK [2048 datoshi]
    /// 18 : DUP [2 datoshi]
    /// 19 : PUSH0 [1 datoshi]
    /// 1A : LDARG1 [2 datoshi]
    /// 1B : SETITEM [8192 datoshi]
    /// 1C : STLOC3 [2 datoshi]
    /// 1D : LDLOC3 [2 datoshi]
    /// 1E : LDARG0 [2 datoshi]
    /// 1F : LDLOC2 [2 datoshi]
    /// 20 : CALL 0B [512 datoshi]
    /// 22 : LDARG0 [2 datoshi]
    /// 23 : LDLOC2 [2 datoshi]
    /// 24 : CALL 1B [512 datoshi]
    /// 26 : STLOC3 [2 datoshi]
    /// 27 : LDLOC3 [2 datoshi]
    /// 28 : PUSH0 [1 datoshi]
    /// 29 : PICKITEM [64 datoshi]
    /// 2A : RET [0 datoshi]
    /// </remarks>
    [DisplayName("serializeTest")]
    public abstract BigInteger? SerializeTest(byte[]? key, BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z84AERGIThBR0FASwHB42yhowUVTi1BBL1jF7UA=
    /// 00 : INITSLOT 0101 [64 datoshi]
    /// 03 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 08 : PUSHINT8 11 [1 datoshi]
    /// 0A : PUSH1 [1 datoshi]
    /// 0B : NEWBUFFER [256 datoshi]
    /// 0C : TUCK [2 datoshi]
    /// 0D : PUSH0 [1 datoshi]
    /// 0E : ROT [2 datoshi]
    /// 0F : SETITEM [8192 datoshi]
    /// 10 : SWAP [2 datoshi]
    /// 11 : PUSH2 [1 datoshi]
    /// 12 : PACK [2048 datoshi]
    /// 13 : STLOC0 [2 datoshi]
    /// 14 : LDARG0 [2 datoshi]
    /// 15 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 17 : LDLOC0 [2 datoshi]
    /// 18 : UNPACK [2048 datoshi]
    /// 19 : DROP [2 datoshi]
    /// 1A : REVERSE3 [2 datoshi]
    /// 1B : CAT [2048 datoshi]
    /// 1C : SWAP [2 datoshi]
    /// 1D : SYSCALL 2F58C5ED 'System.Storage.Delete' [32768 datoshi]
    /// 22 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeleteByte")]
    public abstract void TestDeleteByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAIA/9swcGhBm/ZnzhLAcXjbKGnBRVOLUEEvWMXtQA==
    /// 00 : INITSLOT 0201 [64 datoshi]
    /// 03 : PUSHDATA1 00FF [8 datoshi]
    /// 07 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : LDLOC0 [2 datoshi]
    /// 0B : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 10 : PUSH2 [1 datoshi]
    /// 11 : PACK [2048 datoshi]
    /// 12 : STLOC1 [2 datoshi]
    /// 13 : LDARG0 [2 datoshi]
    /// 14 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 16 : LDLOC1 [2 datoshi]
    /// 17 : UNPACK [2048 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : REVERSE3 [2 datoshi]
    /// 1A : CAT [2048 datoshi]
    /// 1B : SWAP [2 datoshi]
    /// 1C : SYSCALL 2F58C5ED 'System.Storage.Delete' [32768 datoshi]
    /// 21 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeleteByteArray")]
    public abstract void TestDeleteByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAJhYXBoQZv2Z84SwHF42yhpwUVTi1BBL1jF7UA=
    /// 00 : INITSLOT 0201 [64 datoshi]
    /// 03 : PUSHDATA1 6161 'aa' [8 datoshi]
    /// 07 : STLOC0 [2 datoshi]
    /// 08 : LDLOC0 [2 datoshi]
    /// 09 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0E : PUSH2 [1 datoshi]
    /// 0F : PACK [2048 datoshi]
    /// 10 : STLOC1 [2 datoshi]
    /// 11 : LDARG0 [2 datoshi]
    /// 12 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 14 : LDLOC1 [2 datoshi]
    /// 15 : UNPACK [2048 datoshi]
    /// 16 : DROP [2 datoshi]
    /// 17 : REVERSE3 [2 datoshi]
    /// 18 : CAT [2048 datoshi]
    /// 19 : SWAP [2 datoshi]
    /// 1A : SYSCALL 2F58C5ED 'System.Storage.Delete' [32768 datoshi]
    /// 1F : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeleteString")]
    public abstract void TestDeleteString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAQZv2Z85wDAEB2zDbKAwEa2V5MWhB5j8YhAwBAtsw2ygMBGtleTJoQeY/GIQUDANrZXloQd8wuJpxaUGcCO2cRWlB81S/HUA=
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : PUSHDATA1 01 [8 datoshi]
    /// 0C : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0E : CONVERT 28 'ByteString' [8192 datoshi]
    /// 10 : PUSHDATA1 6B657931 'key1' [8 datoshi]
    /// 16 : LDLOC0 [2 datoshi]
    /// 17 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 1C : PUSHDATA1 02 [8 datoshi]
    /// 1F : CONVERT 30 'Buffer' [8192 datoshi]
    /// 21 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 23 : PUSHDATA1 6B657932 'key2' [8 datoshi]
    /// 29 : LDLOC0 [2 datoshi]
    /// 2A : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 2F : PUSH4 [1 datoshi]
    /// 30 : PUSHDATA1 6B6579 'key' [8 datoshi]
    /// 35 : LDLOC0 [2 datoshi]
    /// 36 : SYSCALL DF30B89A 'System.Storage.Find' [32768 datoshi]
    /// 3B : STLOC1 [2 datoshi]
    /// 3C : LDLOC1 [2 datoshi]
    /// 3D : SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// 42 : DROP [2 datoshi]
    /// 43 : LDLOC1 [2 datoshi]
    /// 44 : SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// 49 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testFind")]
    public abstract byte[]? TestFind();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBQfa0a+JwaAAREYhOEFHQUBLAcXjbKGnBRVOLUEGSXegxcmrbMEA=
    /// 00 : INITSLOT 0301 [64 datoshi]
    /// 03 : SYSCALL F6B46BE2 'System.Storage.GetReadOnlyContext' [16 datoshi]
    /// 08 : STLOC0 [2 datoshi]
    /// 09 : LDLOC0 [2 datoshi]
    /// 0A : PUSHINT8 11 [1 datoshi]
    /// 0C : PUSH1 [1 datoshi]
    /// 0D : NEWBUFFER [256 datoshi]
    /// 0E : TUCK [2 datoshi]
    /// 0F : PUSH0 [1 datoshi]
    /// 10 : ROT [2 datoshi]
    /// 11 : SETITEM [8192 datoshi]
    /// 12 : SWAP [2 datoshi]
    /// 13 : PUSH2 [1 datoshi]
    /// 14 : PACK [2048 datoshi]
    /// 15 : STLOC1 [2 datoshi]
    /// 16 : LDARG0 [2 datoshi]
    /// 17 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 19 : LDLOC1 [2 datoshi]
    /// 1A : UNPACK [2048 datoshi]
    /// 1B : DROP [2 datoshi]
    /// 1C : REVERSE3 [2 datoshi]
    /// 1D : CAT [2048 datoshi]
    /// 1E : SWAP [2 datoshi]
    /// 1F : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 24 : STLOC2 [2 datoshi]
    /// 25 : LDLOC2 [2 datoshi]
    /// 26 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 28 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetByte")]
    public abstract byte[]? TestGetByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBDAIA/9swcEGb9mfOQXZMv+lxaGkSwHJ42yhqwUVTi1BBkl3oMXNr2zBA
    /// 00 : INITSLOT 0401 [64 datoshi]
    /// 03 : PUSHDATA1 00FF [8 datoshi]
    /// 07 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0F : SYSCALL 764CBFE9 'System.Storage.AsReadOnly' [16 datoshi]
    /// 14 : STLOC1 [2 datoshi]
    /// 15 : LDLOC0 [2 datoshi]
    /// 16 : LDLOC1 [2 datoshi]
    /// 17 : PUSH2 [1 datoshi]
    /// 18 : PACK [2048 datoshi]
    /// 19 : STLOC2 [2 datoshi]
    /// 1A : LDARG0 [2 datoshi]
    /// 1B : CONVERT 28 'ByteString' [8192 datoshi]
    /// 1D : LDLOC2 [2 datoshi]
    /// 1E : UNPACK [2048 datoshi]
    /// 1F : DROP [2 datoshi]
    /// 20 : REVERSE3 [2 datoshi]
    /// 21 : CAT [2048 datoshi]
    /// 22 : SWAP [2 datoshi]
    /// 23 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 28 : STLOC3 [2 datoshi]
    /// 29 : LDLOC3 [2 datoshi]
    /// 2A : CONVERT 30 'Buffer' [8192 datoshi]
    /// 2C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetByteArray")]
    public abstract byte[]? TestGetByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBDAJhYXBB9rRr4nFoaRLAcnjbKGrBRVOLUEGSXegxc2vbMEA=
    /// 00 : INITSLOT 0401 [64 datoshi]
    /// 03 : PUSHDATA1 6161 'aa' [8 datoshi]
    /// 07 : STLOC0 [2 datoshi]
    /// 08 : SYSCALL F6B46BE2 'System.Storage.GetReadOnlyContext' [16 datoshi]
    /// 0D : STLOC1 [2 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : LDLOC1 [2 datoshi]
    /// 10 : PUSH2 [1 datoshi]
    /// 11 : PACK [2048 datoshi]
    /// 12 : STLOC2 [2 datoshi]
    /// 13 : LDARG0 [2 datoshi]
    /// 14 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 16 : LDLOC2 [2 datoshi]
    /// 17 : UNPACK [2048 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : REVERSE3 [2 datoshi]
    /// 1A : CAT [2048 datoshi]
    /// 1B : SWAP [2 datoshi]
    /// 1C : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 21 : STLOC3 [2 datoshi]
    /// 22 : LDLOC3 [2 datoshi]
    /// 23 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 25 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetString")]
    public abstract byte[]? TestGetString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBDAJpaXBB9rRr4nFoaRLAcnjbKGrBRVOLUEGSXegxc2vbMEA=
    /// 00 : INITSLOT 0401 [64 datoshi]
    /// 03 : PUSHDATA1 6969 'ii' [8 datoshi]
    /// 07 : STLOC0 [2 datoshi]
    /// 08 : SYSCALL F6B46BE2 'System.Storage.GetReadOnlyContext' [16 datoshi]
    /// 0D : STLOC1 [2 datoshi]
    /// 0E : LDLOC0 [2 datoshi]
    /// 0F : LDLOC1 [2 datoshi]
    /// 10 : PUSH2 [1 datoshi]
    /// 11 : PACK [2048 datoshi]
    /// 12 : STLOC2 [2 datoshi]
    /// 13 : LDARG0 [2 datoshi]
    /// 14 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 16 : LDLOC2 [2 datoshi]
    /// 17 : UNPACK [2048 datoshi]
    /// 18 : DROP [2 datoshi]
    /// 19 : REVERSE3 [2 datoshi]
    /// 1A : CAT [2048 datoshi]
    /// 1B : SWAP [2 datoshi]
    /// 1C : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 21 : STLOC3 [2 datoshi]
    /// 22 : LDLOC3 [2 datoshi]
    /// 23 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 25 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIndexGet")]
    public abstract byte[]? TestIndexGet(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDAJpaXBoQZv2Z84SwHF52yhKeNsoacFFU4tQQeY/GIRFCEA=
    /// 00 : INITSLOT 0202 [64 datoshi]
    /// 03 : PUSHDATA1 6969 'ii' [8 datoshi]
    /// 07 : STLOC0 [2 datoshi]
    /// 08 : LDLOC0 [2 datoshi]
    /// 09 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0E : PUSH2 [1 datoshi]
    /// 0F : PACK [2048 datoshi]
    /// 10 : STLOC1 [2 datoshi]
    /// 11 : LDARG1 [2 datoshi]
    /// 12 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 14 : DUP [2 datoshi]
    /// 15 : LDARG0 [2 datoshi]
    /// 16 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 18 : LDLOC1 [2 datoshi]
    /// 19 : UNPACK [2048 datoshi]
    /// 1A : DROP [2 datoshi]
    /// 1B : REVERSE3 [2 datoshi]
    /// 1C : CAT [2048 datoshi]
    /// 1D : SWAP [2 datoshi]
    /// 1E : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 23 : DROP [2 datoshi]
    /// 24 : PUSHT [1 datoshi]
    /// 25 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIndexPut")]
    public abstract bool? TestIndexPut(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUADAIA/9swcEGb9mfOcWhpEsByDAIAAdswc2sMCWJ5dGVBcnJheWrBRVOLUEHmPxiEDAlieXRlQXJyYXlqwUVTi1BBkl3oMdswdGxA
    /// 00 : INITSLOT 0500 [64 datoshi]
    /// 03 : PUSHDATA1 00FF [8 datoshi]
    /// 07 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0F : STLOC1 [2 datoshi]
    /// 10 : LDLOC0 [2 datoshi]
    /// 11 : LDLOC1 [2 datoshi]
    /// 12 : PUSH2 [1 datoshi]
    /// 13 : PACK [2048 datoshi]
    /// 14 : STLOC2 [2 datoshi]
    /// 15 : PUSHDATA1 0001 [8 datoshi]
    /// 19 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 1B : STLOC3 [2 datoshi]
    /// 1C : LDLOC3 [2 datoshi]
    /// 1D : PUSHDATA1 627974654172726179 'byteArray' [8 datoshi]
    /// 28 : LDLOC2 [2 datoshi]
    /// 29 : UNPACK [2048 datoshi]
    /// 2A : DROP [2 datoshi]
    /// 2B : REVERSE3 [2 datoshi]
    /// 2C : CAT [2048 datoshi]
    /// 2D : SWAP [2 datoshi]
    /// 2E : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 33 : PUSHDATA1 627974654172726179 'byteArray' [8 datoshi]
    /// 3E : LDLOC2 [2 datoshi]
    /// 3F : UNPACK [2048 datoshi]
    /// 40 : DROP [2 datoshi]
    /// 41 : REVERSE3 [2 datoshi]
    /// 42 : CAT [2048 datoshi]
    /// 43 : SWAP [2 datoshi]
    /// 44 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 49 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 4B : STLOC4 [2 datoshi]
    /// 4C : LDLOC4 [2 datoshi]
    /// 4D : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNewGetByteArray")]
    public abstract byte[]? TestNewGetByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Vw8ADAIA/9swcEGb9mfOcWhpEsByCHMAe3QMC2hlbGxvIHdvcmxkdQwUAAECAwQFBgcICQABAgMEBQYHCAnbMNsoStgkCUrKABQoAzp2DCAAAQIDBAUGBwgJAAECAwQFBgcICQABAgMEBQYHCAkAAdsw2yhK2CQJSsoAICgDOncHDCEAAQIDBAUGBwgJAAECAwQFBgcICQABAgMEBQYHCAkAAQLbMNsoStgkCUrKACEoAzp3CGsMBGJvb2xqwUVTi1BB5j8YhGwMA2ludGrBRVOLUEHmPxiEbQwGc3RyaW5nasFFU4tQQeY/GIRuDAd1aW50MTYwasFFU4tQQeY/GIRvBwwHdWludDI1NmrBRVOLUEHmPxiEbwgMB2VjcG9pbnRqwUVTi1BB5j8YhAwEYm9vbGrBRVOLUEGSXegxqqp3CQwDaW50asFFU4tQQZJd6DHbIXcKDAZzdHJpbmdqwUVTi1BBkl3oMXcLDAd1aW50MTYwasFFU4tQQZJd6DF3DAwHdWludDI1NmrBRVOLUEGSXegxdw0MB2VjcG9pbnRqwUVTi1BBkl3oMXcOa28JlyQFCSIGbG8KlyQFCSIGbW8LlyQFCSIGbm8MlyQFCSIHbwdvDZckBAlAbwhvDpdA
    /// 0000 : INITSLOT 0F00 [64 datoshi]
    /// 0003 : PUSHDATA1 00FF [8 datoshi]
    /// 0007 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0009 : STLOC0 [2 datoshi]
    /// 000A : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 000F : STLOC1 [2 datoshi]
    /// 0010 : LDLOC0 [2 datoshi]
    /// 0011 : LDLOC1 [2 datoshi]
    /// 0012 : PUSH2 [1 datoshi]
    /// 0013 : PACK [2048 datoshi]
    /// 0014 : STLOC2 [2 datoshi]
    /// 0015 : PUSHT [1 datoshi]
    /// 0016 : STLOC3 [2 datoshi]
    /// 0017 : PUSHINT8 7B [1 datoshi]
    /// 0019 : STLOC4 [2 datoshi]
    /// 001A : PUSHDATA1 68656C6C6F20776F726C64 [8 datoshi]
    /// 0027 : STLOC5 [2 datoshi]
    /// 0028 : PUSHDATA1 0001020304050607080900010203040506070809 [8 datoshi]
    /// 003E : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0040 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0042 : DUP [2 datoshi]
    /// 0043 : ISNULL [2 datoshi]
    /// 0044 : JMPIF 09 [2 datoshi]
    /// 0046 : DUP [2 datoshi]
    /// 0047 : SIZE [4 datoshi]
    /// 0048 : PUSHINT8 14 [1 datoshi]
    /// 004A : JMPEQ 03 [2 datoshi]
    /// 004C : THROW [512 datoshi]
    /// 004D : STLOC6 [2 datoshi]
    /// 004E : PUSHDATA1 0001020304050607080900010203040506070809000102030405060708090001 [8 datoshi]
    /// 0070 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 0072 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 0074 : DUP [2 datoshi]
    /// 0075 : ISNULL [2 datoshi]
    /// 0076 : JMPIF 09 [2 datoshi]
    /// 0078 : DUP [2 datoshi]
    /// 0079 : SIZE [4 datoshi]
    /// 007A : PUSHINT8 20 [1 datoshi]
    /// 007C : JMPEQ 03 [2 datoshi]
    /// 007E : THROW [512 datoshi]
    /// 007F : STLOC 07 [2 datoshi]
    /// 0081 : PUSHDATA1 000102030405060708090001020304050607080900010203040506070809000102 [8 datoshi]
    /// 00A4 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 00A6 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 00A8 : DUP [2 datoshi]
    /// 00A9 : ISNULL [2 datoshi]
    /// 00AA : JMPIF 09 [2 datoshi]
    /// 00AC : DUP [2 datoshi]
    /// 00AD : SIZE [4 datoshi]
    /// 00AE : PUSHINT8 21 [1 datoshi]
    /// 00B0 : JMPEQ 03 [2 datoshi]
    /// 00B2 : THROW [512 datoshi]
    /// 00B3 : STLOC 08 [2 datoshi]
    /// 00B5 : LDLOC3 [2 datoshi]
    /// 00B6 : PUSHDATA1 626F6F6C 'bool' [8 datoshi]
    /// 00BC : LDLOC2 [2 datoshi]
    /// 00BD : UNPACK [2048 datoshi]
    /// 00BE : DROP [2 datoshi]
    /// 00BF : REVERSE3 [2 datoshi]
    /// 00C0 : CAT [2048 datoshi]
    /// 00C1 : SWAP [2 datoshi]
    /// 00C2 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 00C7 : LDLOC4 [2 datoshi]
    /// 00C8 : PUSHDATA1 696E74 'int' [8 datoshi]
    /// 00CD : LDLOC2 [2 datoshi]
    /// 00CE : UNPACK [2048 datoshi]
    /// 00CF : DROP [2 datoshi]
    /// 00D0 : REVERSE3 [2 datoshi]
    /// 00D1 : CAT [2048 datoshi]
    /// 00D2 : SWAP [2 datoshi]
    /// 00D3 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 00D8 : LDLOC5 [2 datoshi]
    /// 00D9 : PUSHDATA1 737472696E67 'string' [8 datoshi]
    /// 00E1 : LDLOC2 [2 datoshi]
    /// 00E2 : UNPACK [2048 datoshi]
    /// 00E3 : DROP [2 datoshi]
    /// 00E4 : REVERSE3 [2 datoshi]
    /// 00E5 : CAT [2048 datoshi]
    /// 00E6 : SWAP [2 datoshi]
    /// 00E7 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 00EC : LDLOC6 [2 datoshi]
    /// 00ED : PUSHDATA1 75696E74313630 'uint160' [8 datoshi]
    /// 00F6 : LDLOC2 [2 datoshi]
    /// 00F7 : UNPACK [2048 datoshi]
    /// 00F8 : DROP [2 datoshi]
    /// 00F9 : REVERSE3 [2 datoshi]
    /// 00FA : CAT [2048 datoshi]
    /// 00FB : SWAP [2 datoshi]
    /// 00FC : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 0101 : LDLOC 07 [2 datoshi]
    /// 0103 : PUSHDATA1 75696E74323536 'uint256' [8 datoshi]
    /// 010C : LDLOC2 [2 datoshi]
    /// 010D : UNPACK [2048 datoshi]
    /// 010E : DROP [2 datoshi]
    /// 010F : REVERSE3 [2 datoshi]
    /// 0110 : CAT [2048 datoshi]
    /// 0111 : SWAP [2 datoshi]
    /// 0112 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 0117 : LDLOC 08 [2 datoshi]
    /// 0119 : PUSHDATA1 6563706F696E74 'ecpoint' [8 datoshi]
    /// 0122 : LDLOC2 [2 datoshi]
    /// 0123 : UNPACK [2048 datoshi]
    /// 0124 : DROP [2 datoshi]
    /// 0125 : REVERSE3 [2 datoshi]
    /// 0126 : CAT [2048 datoshi]
    /// 0127 : SWAP [2 datoshi]
    /// 0128 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 012D : PUSHDATA1 626F6F6C 'bool' [8 datoshi]
    /// 0133 : LDLOC2 [2 datoshi]
    /// 0134 : UNPACK [2048 datoshi]
    /// 0135 : DROP [2 datoshi]
    /// 0136 : REVERSE3 [2 datoshi]
    /// 0137 : CAT [2048 datoshi]
    /// 0138 : SWAP [2 datoshi]
    /// 0139 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 013E : NOT [4 datoshi]
    /// 013F : NOT [4 datoshi]
    /// 0140 : STLOC 09 [2 datoshi]
    /// 0142 : PUSHDATA1 696E74 'int' [8 datoshi]
    /// 0147 : LDLOC2 [2 datoshi]
    /// 0148 : UNPACK [2048 datoshi]
    /// 0149 : DROP [2 datoshi]
    /// 014A : REVERSE3 [2 datoshi]
    /// 014B : CAT [2048 datoshi]
    /// 014C : SWAP [2 datoshi]
    /// 014D : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 0152 : CONVERT 21 'Integer' [8192 datoshi]
    /// 0154 : STLOC 0A [2 datoshi]
    /// 0156 : PUSHDATA1 737472696E67 'string' [8 datoshi]
    /// 015E : LDLOC2 [2 datoshi]
    /// 015F : UNPACK [2048 datoshi]
    /// 0160 : DROP [2 datoshi]
    /// 0161 : REVERSE3 [2 datoshi]
    /// 0162 : CAT [2048 datoshi]
    /// 0163 : SWAP [2 datoshi]
    /// 0164 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 0169 : STLOC 0B [2 datoshi]
    /// 016B : PUSHDATA1 75696E74313630 'uint160' [8 datoshi]
    /// 0174 : LDLOC2 [2 datoshi]
    /// 0175 : UNPACK [2048 datoshi]
    /// 0176 : DROP [2 datoshi]
    /// 0177 : REVERSE3 [2 datoshi]
    /// 0178 : CAT [2048 datoshi]
    /// 0179 : SWAP [2 datoshi]
    /// 017A : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 017F : STLOC 0C [2 datoshi]
    /// 0181 : PUSHDATA1 75696E74323536 'uint256' [8 datoshi]
    /// 018A : LDLOC2 [2 datoshi]
    /// 018B : UNPACK [2048 datoshi]
    /// 018C : DROP [2 datoshi]
    /// 018D : REVERSE3 [2 datoshi]
    /// 018E : CAT [2048 datoshi]
    /// 018F : SWAP [2 datoshi]
    /// 0190 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 0195 : STLOC 0D [2 datoshi]
    /// 0197 : PUSHDATA1 6563706F696E74 'ecpoint' [8 datoshi]
    /// 01A0 : LDLOC2 [2 datoshi]
    /// 01A1 : UNPACK [2048 datoshi]
    /// 01A2 : DROP [2 datoshi]
    /// 01A3 : REVERSE3 [2 datoshi]
    /// 01A4 : CAT [2048 datoshi]
    /// 01A5 : SWAP [2 datoshi]
    /// 01A6 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 01AB : STLOC 0E [2 datoshi]
    /// 01AD : LDLOC3 [2 datoshi]
    /// 01AE : LDLOC 09 [2 datoshi]
    /// 01B0 : EQUAL [32 datoshi]
    /// 01B1 : JMPIF 05 [2 datoshi]
    /// 01B3 : PUSHF [1 datoshi]
    /// 01B4 : JMP 06 [2 datoshi]
    /// 01B6 : LDLOC4 [2 datoshi]
    /// 01B7 : LDLOC 0A [2 datoshi]
    /// 01B9 : EQUAL [32 datoshi]
    /// 01BA : JMPIF 05 [2 datoshi]
    /// 01BC : PUSHF [1 datoshi]
    /// 01BD : JMP 06 [2 datoshi]
    /// 01BF : LDLOC5 [2 datoshi]
    /// 01C0 : LDLOC 0B [2 datoshi]
    /// 01C2 : EQUAL [32 datoshi]
    /// 01C3 : JMPIF 05 [2 datoshi]
    /// 01C5 : PUSHF [1 datoshi]
    /// 01C6 : JMP 06 [2 datoshi]
    /// 01C8 : LDLOC6 [2 datoshi]
    /// 01C9 : LDLOC 0C [2 datoshi]
    /// 01CB : EQUAL [32 datoshi]
    /// 01CC : JMPIF 05 [2 datoshi]
    /// 01CE : PUSHF [1 datoshi]
    /// 01CF : JMP 07 [2 datoshi]
    /// 01D1 : LDLOC 07 [2 datoshi]
    /// 01D3 : LDLOC 0D [2 datoshi]
    /// 01D5 : EQUAL [32 datoshi]
    /// 01D6 : JMPIF 04 [2 datoshi]
    /// 01D8 : PUSHF [1 datoshi]
    /// 01D9 : RET [0 datoshi]
    /// 01DA : LDLOC 08 [2 datoshi]
    /// 01DC : LDLOC 0E [2 datoshi]
    /// 01DE : EQUAL [32 datoshi]
    /// 01DF : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNewGetMethods")]
    public abstract bool? TestNewGetMethods();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADBg7ADIDIyMjIwIjIwIjIwIjIwIjIwIjIwLbMHAMCHRlc3RfbWFwQZv2Z84SwHFo2ygMAQHbMNsoacFFU4tQQeY/GIQMAQHbMNsoacFFU4tQQZJd6DHbMEA=
    /// 00 : INITSLOT 0200 [64 datoshi]
    /// 03 : PUSHDATA1 3B0032032323232302232302232302232302232302232302 [8 datoshi]
    /// 1D : CONVERT 30 'Buffer' [8192 datoshi]
    /// 1F : STLOC0 [2 datoshi]
    /// 20 : PUSHDATA1 746573745F6D6170 'test_map' [8 datoshi]
    /// 2A : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 2F : PUSH2 [1 datoshi]
    /// 30 : PACK [2048 datoshi]
    /// 31 : STLOC1 [2 datoshi]
    /// 32 : LDLOC0 [2 datoshi]
    /// 33 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 35 : PUSHDATA1 01 [8 datoshi]
    /// 38 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 3A : CONVERT 28 'ByteString' [8192 datoshi]
    /// 3C : LDLOC1 [2 datoshi]
    /// 3D : UNPACK [2048 datoshi]
    /// 3E : DROP [2 datoshi]
    /// 3F : REVERSE3 [2 datoshi]
    /// 40 : CAT [2048 datoshi]
    /// 41 : SWAP [2 datoshi]
    /// 42 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 47 : PUSHDATA1 01 [8 datoshi]
    /// 4A : CONVERT 30 'Buffer' [8192 datoshi]
    /// 4C : CONVERT 28 'ByteString' [8192 datoshi]
    /// 4E : LDLOC1 [2 datoshi]
    /// 4F : UNPACK [2048 datoshi]
    /// 50 : DROP [2 datoshi]
    /// 51 : REVERSE3 [2 datoshi]
    /// 52 : CAT [2048 datoshi]
    /// 53 : SWAP [2 datoshi]
    /// 54 : SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 59 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 5B : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOver16Bytes")]
    public abstract byte[]? TestOver16Bytes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQZv2Z84AERGIThBR0FASwHB52yh42yhowUVTi1BB5j8YhAhA
    /// 00 : INITSLOT 0102 [64 datoshi]
    /// 03 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 08 : PUSHINT8 11 [1 datoshi]
    /// 0A : PUSH1 [1 datoshi]
    /// 0B : NEWBUFFER [256 datoshi]
    /// 0C : TUCK [2 datoshi]
    /// 0D : PUSH0 [1 datoshi]
    /// 0E : ROT [2 datoshi]
    /// 0F : SETITEM [8192 datoshi]
    /// 10 : SWAP [2 datoshi]
    /// 11 : PUSH2 [1 datoshi]
    /// 12 : PACK [2048 datoshi]
    /// 13 : STLOC0 [2 datoshi]
    /// 14 : LDARG1 [2 datoshi]
    /// 15 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 17 : LDARG0 [2 datoshi]
    /// 18 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 1A : LDLOC0 [2 datoshi]
    /// 1B : UNPACK [2048 datoshi]
    /// 1C : DROP [2 datoshi]
    /// 1D : REVERSE3 [2 datoshi]
    /// 1E : CAT [2048 datoshi]
    /// 1F : SWAP [2 datoshi]
    /// 20 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 25 : PUSHT [1 datoshi]
    /// 26 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPutByte")]
    public abstract bool? TestPutByte(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDAIA/9swcGhBm/ZnzhLAcXnbKHjbKGnBRVOLUEHmPxiECEA=
    /// 00 : INITSLOT 0202 [64 datoshi]
    /// 03 : PUSHDATA1 00FF [8 datoshi]
    /// 07 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : LDLOC0 [2 datoshi]
    /// 0B : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 10 : PUSH2 [1 datoshi]
    /// 11 : PACK [2048 datoshi]
    /// 12 : STLOC1 [2 datoshi]
    /// 13 : LDARG1 [2 datoshi]
    /// 14 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 16 : LDARG0 [2 datoshi]
    /// 17 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 19 : LDLOC1 [2 datoshi]
    /// 1A : UNPACK [2048 datoshi]
    /// 1B : DROP [2 datoshi]
    /// 1C : REVERSE3 [2 datoshi]
    /// 1D : CAT [2048 datoshi]
    /// 1E : SWAP [2 datoshi]
    /// 1F : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 24 : PUSHT [1 datoshi]
    /// 25 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPutByteArray")]
    public abstract bool? TestPutByteArray(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCDAIA/9swcEGb9mfOQXZMv+lxaGkSwHJ52yh42yhqwUVTi1BB5j8YhAhA
    /// 00 : INITSLOT 0302 [64 datoshi]
    /// 03 : PUSHDATA1 00FF [8 datoshi]
    /// 07 : CONVERT 30 'Buffer' [8192 datoshi]
    /// 09 : STLOC0 [2 datoshi]
    /// 0A : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0F : SYSCALL 764CBFE9 'System.Storage.AsReadOnly' [16 datoshi]
    /// 14 : STLOC1 [2 datoshi]
    /// 15 : LDLOC0 [2 datoshi]
    /// 16 : LDLOC1 [2 datoshi]
    /// 17 : PUSH2 [1 datoshi]
    /// 18 : PACK [2048 datoshi]
    /// 19 : STLOC2 [2 datoshi]
    /// 1A : LDARG1 [2 datoshi]
    /// 1B : CONVERT 28 'ByteString' [8192 datoshi]
    /// 1D : LDARG0 [2 datoshi]
    /// 1E : CONVERT 28 'ByteString' [8192 datoshi]
    /// 20 : LDLOC2 [2 datoshi]
    /// 21 : UNPACK [2048 datoshi]
    /// 22 : DROP [2 datoshi]
    /// 23 : REVERSE3 [2 datoshi]
    /// 24 : CAT [2048 datoshi]
    /// 25 : SWAP [2 datoshi]
    /// 26 : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 2B : PUSHT [1 datoshi]
    /// 2C : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPutReadOnly")]
    public abstract bool? TestPutReadOnly(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDAJhYXBoQZv2Z84SwHF52yh42yhpwUVTi1BB5j8YhAhA
    /// 00 : INITSLOT 0202 [64 datoshi]
    /// 03 : PUSHDATA1 6161 'aa' [8 datoshi]
    /// 07 : STLOC0 [2 datoshi]
    /// 08 : LDLOC0 [2 datoshi]
    /// 09 : SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0E : PUSH2 [1 datoshi]
    /// 0F : PACK [2048 datoshi]
    /// 10 : STLOC1 [2 datoshi]
    /// 11 : LDARG1 [2 datoshi]
    /// 12 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 14 : LDARG0 [2 datoshi]
    /// 15 : CONVERT 28 'ByteString' [8192 datoshi]
    /// 17 : LDLOC1 [2 datoshi]
    /// 18 : UNPACK [2048 datoshi]
    /// 19 : DROP [2 datoshi]
    /// 1A : REVERSE3 [2 datoshi]
    /// 1B : CAT [2048 datoshi]
    /// 1C : SWAP [2 datoshi]
    /// 1D : SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 22 : PUSHT [1 datoshi]
    /// 23 : RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPutString")]
    public abstract bool? TestPutString(byte[]? key, byte[]? value);

    #endregion
}
