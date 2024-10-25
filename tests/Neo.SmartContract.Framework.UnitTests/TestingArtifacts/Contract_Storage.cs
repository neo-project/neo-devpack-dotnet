using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Storage(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Storage"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPutByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testDeleteByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":39,""safe"":false},{""name"":""testGetByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":74,""safe"":false},{""name"":""testOver16Bytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":115,""safe"":false},{""name"":""testPutString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":207,""safe"":false},{""name"":""testDeleteString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":243,""safe"":false},{""name"":""testGetString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":275,""safe"":false},{""name"":""testPutByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":313,""safe"":false},{""name"":""testDeleteByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":351,""safe"":false},{""name"":""testGetByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":385,""safe"":false},{""name"":""testNewGetMethods"",""parameters"":[],""returntype"":""Boolean"",""offset"":430,""safe"":false},{""name"":""testNewGetByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":910,""safe"":false},{""name"":""testPutReadOnly"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":988,""safe"":false},{""name"":""serializeTest"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1033,""safe"":false},{""name"":""testFind"",""parameters"":[],""returntype"":""ByteArray"",""offset"":1126,""safe"":false},{""name"":""testIndexPut"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1200,""safe"":false},{""name"":""testIndexGet"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":1238,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwLZGVzZXJpYWxpemUBAAEPAAD9/ARXAQJBm/ZnzgAREYhOEFHQUBLAcHnbKHjbKGjBRVOLUEHmPxiECEBXAQFBm/ZnzgAREYhOEFHQUBLAcHjbKGjBRVOLUEEvWMXtQFcDAUH2tGvicGgAERGIThBR0FASwHF42yhpwUVTi1BBkl3oMXJq2zBAVwIADBg7ADIDIyMjIwIjIwIjIwIjIwIjIwIjIwLbMHAMCHRlc3RfbWFwQZv2Z84SwHFo2ygMAQHbMNsoacFFU4tQQeY/GIQMAQHbMNsoacFFU4tQQZJd6DHbMEBXAgIMAmFhcGhBm/ZnzhLAcXnbKHjbKGnBRVOLUEHmPxiECEBXAgEMAmFhcGhBm/ZnzhLAcXjbKGnBRVOLUEEvWMXtQFcEAQwCYWFwQfa0a+JxaGkSwHJ42yhqwUVTi1BBkl3oMXNr2zBAVwICDAIA/9swcGhBm/ZnzhLAcXnbKHjbKGnBRVOLUEHmPxiECEBXAgEMAgD/2zBwaEGb9mfOEsBxeNsoacFFU4tQQS9Yxe1AVwQBDAIA/9swcEGb9mfOQXZMv+lxaGkSwHJ42yhqwUVTi1BBkl3oMXNr2zBAVw8ADAIA/9swcEGb9mfOcWhpEsByCHMAe3QMC2hlbGxvIHdvcmxkdQwUAAECAwQFBgcICQABAgMEBQYHCAnbMNsoStgkCUrKABQoAzp2DCAAAQIDBAUGBwgJAAECAwQFBgcICQABAgMEBQYHCAkAAdsw2yhK2CQJSsoAICgDOncHDCEAAQIDBAUGBwgJAAECAwQFBgcICQABAgMEBQYHCAkAAQLbMNsoStgkCUrKACEoAzp3CGsMBGJvb2xqwUVTi1BB5j8YhGwMA2ludGrBRVOLUEHmPxiEbQwGc3RyaW5nasFFU4tQQeY/GIRuDAd1aW50MTYwasFFU4tQQeY/GIRvBwwHdWludDI1NmrBRVOLUEHmPxiEbwgMB2VjcG9pbnRqwUVTi1BB5j8YhAwEYm9vbGrBRVOLUEGSXegxqqp3CQwDaW50asFFU4tQQZJd6DHbIXcKDAZzdHJpbmdqwUVTi1BBkl3oMXcLDAd1aW50MTYwasFFU4tQQZJd6DF3DAwHdWludDI1NmrBRVOLUEGSXegxdw0MB2VjcG9pbnRqwUVTi1BBkl3oMXcOa28JlyQFCSIGbG8KlyQFCSIGbW8LlyQFCSIGbm8MlyQFCSIHbwdvDZckBAlAbwhvDpdAVwUADAIA/9swcEGb9mfOcWhpEsByDAIAAdswc2sMCWJ5dGVBcnJheWrBRVOLUEHmPxiEDAlieXRlQXJyYXlqwUVTi1BBkl3oMdswdGxAVwMCDAIA/9swcEGb9mfOQXZMv+lxaGkSwHJ52yh42yhqwUVTi1BB5j8YhAhAVwQCDAIBqtswcEGb9mfOcWhpEsByEBHAShB50HNreGo0C3hqNBtzaxDOQFcAA3o3AAB5eMFFU4tQQeY/GIRAVwICeXjBRVOLUEGSXegxcGhxaQuXJgQLQGg3AQBAVwIAQZv2Z85wDAEB2zDbKAwEa2V5MWhB5j8YhAwBAtsw2ygMBGtleTJoQeY/GIQUDANrZXloQd8wuJpxaUGcCO2cRWlB81S/HUBXAgIMAmlpcGhBm/ZnzhLAcXnbKEp42yhpwUVTi1BB5j8YhEUIQFcEAQwCaWlwQfa0a+JxaGkSwHJ42yhqwUVTi1BBkl3oMXNr2zBAmqR3OQ=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQCDAGq2zBwQZv2Z85xaGkSwHIQEcBKEHnQc2t4ajQLeGo0G3NrEM5A
    /// 00 : OpCode.INITSLOT 0402 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 01AA [8 datoshi]
    /// 07 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0F : OpCode.STLOC1 [2 datoshi]
    /// 10 : OpCode.LDLOC0 [2 datoshi]
    /// 11 : OpCode.LDLOC1 [2 datoshi]
    /// 12 : OpCode.PUSH2 [1 datoshi]
    /// 13 : OpCode.PACK [2048 datoshi]
    /// 14 : OpCode.STLOC2 [2 datoshi]
    /// 15 : OpCode.PUSH0 [1 datoshi]
    /// 16 : OpCode.PUSH1 [1 datoshi]
    /// 17 : OpCode.PACK [2048 datoshi]
    /// 18 : OpCode.DUP [2 datoshi]
    /// 19 : OpCode.PUSH0 [1 datoshi]
    /// 1A : OpCode.LDARG1 [2 datoshi]
    /// 1B : OpCode.SETITEM [8192 datoshi]
    /// 1C : OpCode.STLOC3 [2 datoshi]
    /// 1D : OpCode.LDLOC3 [2 datoshi]
    /// 1E : OpCode.LDARG0 [2 datoshi]
    /// 1F : OpCode.LDLOC2 [2 datoshi]
    /// 20 : OpCode.CALL 0B [512 datoshi]
    /// 22 : OpCode.LDARG0 [2 datoshi]
    /// 23 : OpCode.LDLOC2 [2 datoshi]
    /// 24 : OpCode.CALL 1B [512 datoshi]
    /// 26 : OpCode.STLOC3 [2 datoshi]
    /// 27 : OpCode.LDLOC3 [2 datoshi]
    /// 28 : OpCode.PUSH0 [1 datoshi]
    /// 29 : OpCode.PICKITEM [64 datoshi]
    /// 2A : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("serializeTest")]
    public abstract BigInteger? SerializeTest(byte[]? key, BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z84AERGIThBR0FASwHB42yhowUVTi1BBL1jF7UA=
    /// 00 : OpCode.INITSLOT 0101 [64 datoshi]
    /// 03 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 08 : OpCode.PUSHINT8 11 [1 datoshi]
    /// 0A : OpCode.PUSH1 [1 datoshi]
    /// 0B : OpCode.NEWBUFFER [256 datoshi]
    /// 0C : OpCode.TUCK [2 datoshi]
    /// 0D : OpCode.PUSH0 [1 datoshi]
    /// 0E : OpCode.ROT [2 datoshi]
    /// 0F : OpCode.SETITEM [8192 datoshi]
    /// 10 : OpCode.SWAP [2 datoshi]
    /// 11 : OpCode.PUSH2 [1 datoshi]
    /// 12 : OpCode.PACK [2048 datoshi]
    /// 13 : OpCode.STLOC0 [2 datoshi]
    /// 14 : OpCode.LDARG0 [2 datoshi]
    /// 15 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 17 : OpCode.LDLOC0 [2 datoshi]
    /// 18 : OpCode.UNPACK [2048 datoshi]
    /// 19 : OpCode.DROP [2 datoshi]
    /// 1A : OpCode.REVERSE3 [2 datoshi]
    /// 1B : OpCode.CAT [2048 datoshi]
    /// 1C : OpCode.SWAP [2 datoshi]
    /// 1D : OpCode.SYSCALL 2F58C5ED 'System.Storage.Delete' [32768 datoshi]
    /// 22 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeleteByte")]
    public abstract void TestDeleteByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAD/2zBwaEGb9mfOEsBxeNsoacFFU4tQQS9Yxe1A
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 00FF [8 datoshi]
    /// 07 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.LDLOC0 [2 datoshi]
    /// 0B : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 10 : OpCode.PUSH2 [1 datoshi]
    /// 11 : OpCode.PACK [2048 datoshi]
    /// 12 : OpCode.STLOC1 [2 datoshi]
    /// 13 : OpCode.LDARG0 [2 datoshi]
    /// 14 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 16 : OpCode.LDLOC1 [2 datoshi]
    /// 17 : OpCode.UNPACK [2048 datoshi]
    /// 18 : OpCode.DROP [2 datoshi]
    /// 19 : OpCode.REVERSE3 [2 datoshi]
    /// 1A : OpCode.CAT [2048 datoshi]
    /// 1B : OpCode.SWAP [2 datoshi]
    /// 1C : OpCode.SYSCALL 2F58C5ED 'System.Storage.Delete' [32768 datoshi]
    /// 21 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeleteByteArray")]
    public abstract void TestDeleteByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDGFhcGhBm/ZnzhLAcXjbKGnBRVOLUEEvWMXtQA==
    /// 00 : OpCode.INITSLOT 0201 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 6161 [8 datoshi]
    /// 07 : OpCode.STLOC0 [2 datoshi]
    /// 08 : OpCode.LDLOC0 [2 datoshi]
    /// 09 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0E : OpCode.PUSH2 [1 datoshi]
    /// 0F : OpCode.PACK [2048 datoshi]
    /// 10 : OpCode.STLOC1 [2 datoshi]
    /// 11 : OpCode.LDARG0 [2 datoshi]
    /// 12 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 14 : OpCode.LDLOC1 [2 datoshi]
    /// 15 : OpCode.UNPACK [2048 datoshi]
    /// 16 : OpCode.DROP [2 datoshi]
    /// 17 : OpCode.REVERSE3 [2 datoshi]
    /// 18 : OpCode.CAT [2048 datoshi]
    /// 19 : OpCode.SWAP [2 datoshi]
    /// 1A : OpCode.SYSCALL 2F58C5ED 'System.Storage.Delete' [32768 datoshi]
    /// 1F : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testDeleteString")]
    public abstract void TestDeleteString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAQZv2Z85wDAHbMNsoDGtleTFoQeY/GIQMAtsw2ygMa2V5MmhB5j8YhBQMa2V5aEHfMLiacWlBnAjtnEVpQfNUvx1A
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.PUSHDATA1 01 [8 datoshi]
    /// 0C : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 0E : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 10 : OpCode.PUSHDATA1 6B657931 [8 datoshi]
    /// 16 : OpCode.LDLOC0 [2 datoshi]
    /// 17 : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 1C : OpCode.PUSHDATA1 02 [8 datoshi]
    /// 1F : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 21 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 23 : OpCode.PUSHDATA1 6B657932 [8 datoshi]
    /// 29 : OpCode.LDLOC0 [2 datoshi]
    /// 2A : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 2F : OpCode.PUSH4 [1 datoshi]
    /// 30 : OpCode.PUSHDATA1 6B6579 [8 datoshi]
    /// 35 : OpCode.LDLOC0 [2 datoshi]
    /// 36 : OpCode.SYSCALL DF30B89A 'System.Storage.Find' [32768 datoshi]
    /// 3B : OpCode.STLOC1 [2 datoshi]
    /// 3C : OpCode.LDLOC1 [2 datoshi]
    /// 3D : OpCode.SYSCALL 9C08ED9C 'System.Iterator.Next' [32768 datoshi]
    /// 42 : OpCode.DROP [2 datoshi]
    /// 43 : OpCode.LDLOC1 [2 datoshi]
    /// 44 : OpCode.SYSCALL F354BF1D 'System.Iterator.Value' [16 datoshi]
    /// 49 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testFind")]
    public abstract byte[]? TestFind();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBQfa0a+JwaAAREYhOEFHQUBLAcXjbKGnBRVOLUEGSXegxcmrbMEA=
    /// 00 : OpCode.INITSLOT 0301 [64 datoshi]
    /// 03 : OpCode.SYSCALL F6B46BE2 'System.Storage.GetReadOnlyContext' [16 datoshi]
    /// 08 : OpCode.STLOC0 [2 datoshi]
    /// 09 : OpCode.LDLOC0 [2 datoshi]
    /// 0A : OpCode.PUSHINT8 11 [1 datoshi]
    /// 0C : OpCode.PUSH1 [1 datoshi]
    /// 0D : OpCode.NEWBUFFER [256 datoshi]
    /// 0E : OpCode.TUCK [2 datoshi]
    /// 0F : OpCode.PUSH0 [1 datoshi]
    /// 10 : OpCode.ROT [2 datoshi]
    /// 11 : OpCode.SETITEM [8192 datoshi]
    /// 12 : OpCode.SWAP [2 datoshi]
    /// 13 : OpCode.PUSH2 [1 datoshi]
    /// 14 : OpCode.PACK [2048 datoshi]
    /// 15 : OpCode.STLOC1 [2 datoshi]
    /// 16 : OpCode.LDARG0 [2 datoshi]
    /// 17 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 19 : OpCode.LDLOC1 [2 datoshi]
    /// 1A : OpCode.UNPACK [2048 datoshi]
    /// 1B : OpCode.DROP [2 datoshi]
    /// 1C : OpCode.REVERSE3 [2 datoshi]
    /// 1D : OpCode.CAT [2048 datoshi]
    /// 1E : OpCode.SWAP [2 datoshi]
    /// 1F : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 24 : OpCode.STLOC2 [2 datoshi]
    /// 25 : OpCode.LDLOC2 [2 datoshi]
    /// 26 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 28 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetByte")]
    public abstract byte[]? TestGetByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBDAD/2zBwQZv2Z85Bdky/6XFoaRLAcnjbKGrBRVOLUEGSXegxc2vbMEA=
    /// 00 : OpCode.INITSLOT 0401 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 00FF [8 datoshi]
    /// 07 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0F : OpCode.SYSCALL 764CBFE9 'System.Storage.AsReadOnly' [16 datoshi]
    /// 14 : OpCode.STLOC1 [2 datoshi]
    /// 15 : OpCode.LDLOC0 [2 datoshi]
    /// 16 : OpCode.LDLOC1 [2 datoshi]
    /// 17 : OpCode.PUSH2 [1 datoshi]
    /// 18 : OpCode.PACK [2048 datoshi]
    /// 19 : OpCode.STLOC2 [2 datoshi]
    /// 1A : OpCode.LDARG0 [2 datoshi]
    /// 1B : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 1D : OpCode.LDLOC2 [2 datoshi]
    /// 1E : OpCode.UNPACK [2048 datoshi]
    /// 1F : OpCode.DROP [2 datoshi]
    /// 20 : OpCode.REVERSE3 [2 datoshi]
    /// 21 : OpCode.CAT [2048 datoshi]
    /// 22 : OpCode.SWAP [2 datoshi]
    /// 23 : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 28 : OpCode.STLOC3 [2 datoshi]
    /// 29 : OpCode.LDLOC3 [2 datoshi]
    /// 2A : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 2C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetByteArray")]
    public abstract byte[]? TestGetByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBDGFhcEH2tGvicWhpEsByeNsoasFFU4tQQZJd6DFza9swQA==
    /// 00 : OpCode.INITSLOT 0401 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 6161 [8 datoshi]
    /// 07 : OpCode.STLOC0 [2 datoshi]
    /// 08 : OpCode.SYSCALL F6B46BE2 'System.Storage.GetReadOnlyContext' [16 datoshi]
    /// 0D : OpCode.STLOC1 [2 datoshi]
    /// 0E : OpCode.LDLOC0 [2 datoshi]
    /// 0F : OpCode.LDLOC1 [2 datoshi]
    /// 10 : OpCode.PUSH2 [1 datoshi]
    /// 11 : OpCode.PACK [2048 datoshi]
    /// 12 : OpCode.STLOC2 [2 datoshi]
    /// 13 : OpCode.LDARG0 [2 datoshi]
    /// 14 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 16 : OpCode.LDLOC2 [2 datoshi]
    /// 17 : OpCode.UNPACK [2048 datoshi]
    /// 18 : OpCode.DROP [2 datoshi]
    /// 19 : OpCode.REVERSE3 [2 datoshi]
    /// 1A : OpCode.CAT [2048 datoshi]
    /// 1B : OpCode.SWAP [2 datoshi]
    /// 1C : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 21 : OpCode.STLOC3 [2 datoshi]
    /// 22 : OpCode.LDLOC3 [2 datoshi]
    /// 23 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testGetString")]
    public abstract byte[]? TestGetString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBDGlpcEH2tGvicWhpEsByeNsoasFFU4tQQZJd6DFza9swQA==
    /// 00 : OpCode.INITSLOT 0401 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 6969 [8 datoshi]
    /// 07 : OpCode.STLOC0 [2 datoshi]
    /// 08 : OpCode.SYSCALL F6B46BE2 'System.Storage.GetReadOnlyContext' [16 datoshi]
    /// 0D : OpCode.STLOC1 [2 datoshi]
    /// 0E : OpCode.LDLOC0 [2 datoshi]
    /// 0F : OpCode.LDLOC1 [2 datoshi]
    /// 10 : OpCode.PUSH2 [1 datoshi]
    /// 11 : OpCode.PACK [2048 datoshi]
    /// 12 : OpCode.STLOC2 [2 datoshi]
    /// 13 : OpCode.LDARG0 [2 datoshi]
    /// 14 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 16 : OpCode.LDLOC2 [2 datoshi]
    /// 17 : OpCode.UNPACK [2048 datoshi]
    /// 18 : OpCode.DROP [2 datoshi]
    /// 19 : OpCode.REVERSE3 [2 datoshi]
    /// 1A : OpCode.CAT [2048 datoshi]
    /// 1B : OpCode.SWAP [2 datoshi]
    /// 1C : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 21 : OpCode.STLOC3 [2 datoshi]
    /// 22 : OpCode.LDLOC3 [2 datoshi]
    /// 23 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIndexGet")]
    public abstract byte[]? TestIndexGet(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDGlpcGhBm/ZnzhLAcXnbKEp42yhpwUVTi1BB5j8YhEUIQA==
    /// 00 : OpCode.INITSLOT 0202 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 6969 [8 datoshi]
    /// 07 : OpCode.STLOC0 [2 datoshi]
    /// 08 : OpCode.LDLOC0 [2 datoshi]
    /// 09 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0E : OpCode.PUSH2 [1 datoshi]
    /// 0F : OpCode.PACK [2048 datoshi]
    /// 10 : OpCode.STLOC1 [2 datoshi]
    /// 11 : OpCode.LDARG1 [2 datoshi]
    /// 12 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 14 : OpCode.DUP [2 datoshi]
    /// 15 : OpCode.LDARG0 [2 datoshi]
    /// 16 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 18 : OpCode.LDLOC1 [2 datoshi]
    /// 19 : OpCode.UNPACK [2048 datoshi]
    /// 1A : OpCode.DROP [2 datoshi]
    /// 1B : OpCode.REVERSE3 [2 datoshi]
    /// 1C : OpCode.CAT [2048 datoshi]
    /// 1D : OpCode.SWAP [2 datoshi]
    /// 1E : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 23 : OpCode.DROP [2 datoshi]
    /// 24 : OpCode.PUSHT [1 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testIndexPut")]
    public abstract bool? TestIndexPut(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUADAD/2zBwQZv2Z85xaGkSwHIMAAHbMHNrDGJ5dGVBcnJheWrBRVOLUEHmPxiEDGJ5dGVBcnJheWrBRVOLUEGSXegx2zB0bEA=
    /// 00 : OpCode.INITSLOT 0500 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 00FF [8 datoshi]
    /// 07 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0F : OpCode.STLOC1 [2 datoshi]
    /// 10 : OpCode.LDLOC0 [2 datoshi]
    /// 11 : OpCode.LDLOC1 [2 datoshi]
    /// 12 : OpCode.PUSH2 [1 datoshi]
    /// 13 : OpCode.PACK [2048 datoshi]
    /// 14 : OpCode.STLOC2 [2 datoshi]
    /// 15 : OpCode.PUSHDATA1 0001 [8 datoshi]
    /// 19 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 1B : OpCode.STLOC3 [2 datoshi]
    /// 1C : OpCode.LDLOC3 [2 datoshi]
    /// 1D : OpCode.PUSHDATA1 627974654172726179 [8 datoshi]
    /// 28 : OpCode.LDLOC2 [2 datoshi]
    /// 29 : OpCode.UNPACK [2048 datoshi]
    /// 2A : OpCode.DROP [2 datoshi]
    /// 2B : OpCode.REVERSE3 [2 datoshi]
    /// 2C : OpCode.CAT [2048 datoshi]
    /// 2D : OpCode.SWAP [2 datoshi]
    /// 2E : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 33 : OpCode.PUSHDATA1 627974654172726179 [8 datoshi]
    /// 3E : OpCode.LDLOC2 [2 datoshi]
    /// 3F : OpCode.UNPACK [2048 datoshi]
    /// 40 : OpCode.DROP [2 datoshi]
    /// 41 : OpCode.REVERSE3 [2 datoshi]
    /// 42 : OpCode.CAT [2048 datoshi]
    /// 43 : OpCode.SWAP [2 datoshi]
    /// 44 : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 49 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 4B : OpCode.STLOC4 [2 datoshi]
    /// 4C : OpCode.LDLOC4 [2 datoshi]
    /// 4D : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNewGetByteArray")]
    public abstract byte[]? TestNewGetByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Vw8ADAD/2zBwQZv2Z85xaGkSwHIIcwB7dAxoZWxsbyB3b3JsZHUMAAECAwQFBgcICQABAgMEBQYHCAnbMNsoStgkCUrKABQoAzp2DAABAgMEBQYHCAkAAQIDBAUGBwgJAAECAwQFBgcICQAB2zDbKErYJAlKygAgKAM6dwcMAAECAwQFBgcICQABAgMEBQYHCAkAAQIDBAUGBwgJAAEC2zDbKErYJAlKygAhKAM6dwhrDGJvb2xqwUVTi1BB5j8YhGwMaW50asFFU4tQQeY/GIRtDHN0cmluZ2rBRVOLUEHmPxiEbgx1aW50MTYwasFFU4tQQeY/GIRvBwx1aW50MjU2asFFU4tQQeY/GIRvCAxlY3BvaW50asFFU4tQQeY/GIQMYm9vbGrBRVOLUEGSXegxqqp3CQxpbnRqwUVTi1BBkl3oMdshdwoMc3RyaW5nasFFU4tQQZJd6DF3Cwx1aW50MTYwasFFU4tQQZJd6DF3DAx1aW50MjU2asFFU4tQQZJd6DF3DQxlY3BvaW50asFFU4tQQZJd6DF3DmtvCZckBQkiBmxvCpckBQkiBm1vC5ckBQkiBm5vDJckBQkiB28Hbw2XJAQJQG8Ibw6XQA==
    /// 0000 : OpCode.INITSLOT 0F00 [64 datoshi]
    /// 0003 : OpCode.PUSHDATA1 00FF [8 datoshi]
    /// 0007 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 0009 : OpCode.STLOC0 [2 datoshi]
    /// 000A : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 000F : OpCode.STLOC1 [2 datoshi]
    /// 0010 : OpCode.LDLOC0 [2 datoshi]
    /// 0011 : OpCode.LDLOC1 [2 datoshi]
    /// 0012 : OpCode.PUSH2 [1 datoshi]
    /// 0013 : OpCode.PACK [2048 datoshi]
    /// 0014 : OpCode.STLOC2 [2 datoshi]
    /// 0015 : OpCode.PUSHT [1 datoshi]
    /// 0016 : OpCode.STLOC3 [2 datoshi]
    /// 0017 : OpCode.PUSHINT8 7B [1 datoshi]
    /// 0019 : OpCode.STLOC4 [2 datoshi]
    /// 001A : OpCode.PUSHDATA1 68656C6C6F20776F726C64 [8 datoshi]
    /// 0027 : OpCode.STLOC5 [2 datoshi]
    /// 0028 : OpCode.PUSHDATA1 0001020304050607080900010203040506070809 [8 datoshi]
    /// 003E : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 0040 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 0042 : OpCode.DUP [2 datoshi]
    /// 0043 : OpCode.ISNULL [2 datoshi]
    /// 0044 : OpCode.JMPIF 09 [2 datoshi]
    /// 0046 : OpCode.DUP [2 datoshi]
    /// 0047 : OpCode.SIZE [4 datoshi]
    /// 0048 : OpCode.PUSHINT8 14 [1 datoshi]
    /// 004A : OpCode.JMPEQ 03 [2 datoshi]
    /// 004C : OpCode.THROW [512 datoshi]
    /// 004D : OpCode.STLOC6 [2 datoshi]
    /// 004E : OpCode.PUSHDATA1 0001020304050607080900010203040506070809000102030405060708090001 [8 datoshi]
    /// 0070 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 0072 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 0074 : OpCode.DUP [2 datoshi]
    /// 0075 : OpCode.ISNULL [2 datoshi]
    /// 0076 : OpCode.JMPIF 09 [2 datoshi]
    /// 0078 : OpCode.DUP [2 datoshi]
    /// 0079 : OpCode.SIZE [4 datoshi]
    /// 007A : OpCode.PUSHINT8 20 [1 datoshi]
    /// 007C : OpCode.JMPEQ 03 [2 datoshi]
    /// 007E : OpCode.THROW [512 datoshi]
    /// 007F : OpCode.STLOC 07 [2 datoshi]
    /// 0081 : OpCode.PUSHDATA1 000102030405060708090001020304050607080900010203040506070809000102 [8 datoshi]
    /// 00A4 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 00A6 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 00A8 : OpCode.DUP [2 datoshi]
    /// 00A9 : OpCode.ISNULL [2 datoshi]
    /// 00AA : OpCode.JMPIF 09 [2 datoshi]
    /// 00AC : OpCode.DUP [2 datoshi]
    /// 00AD : OpCode.SIZE [4 datoshi]
    /// 00AE : OpCode.PUSHINT8 21 [1 datoshi]
    /// 00B0 : OpCode.JMPEQ 03 [2 datoshi]
    /// 00B2 : OpCode.THROW [512 datoshi]
    /// 00B3 : OpCode.STLOC 08 [2 datoshi]
    /// 00B5 : OpCode.LDLOC3 [2 datoshi]
    /// 00B6 : OpCode.PUSHDATA1 626F6F6C [8 datoshi]
    /// 00BC : OpCode.LDLOC2 [2 datoshi]
    /// 00BD : OpCode.UNPACK [2048 datoshi]
    /// 00BE : OpCode.DROP [2 datoshi]
    /// 00BF : OpCode.REVERSE3 [2 datoshi]
    /// 00C0 : OpCode.CAT [2048 datoshi]
    /// 00C1 : OpCode.SWAP [2 datoshi]
    /// 00C2 : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 00C7 : OpCode.LDLOC4 [2 datoshi]
    /// 00C8 : OpCode.PUSHDATA1 696E74 [8 datoshi]
    /// 00CD : OpCode.LDLOC2 [2 datoshi]
    /// 00CE : OpCode.UNPACK [2048 datoshi]
    /// 00CF : OpCode.DROP [2 datoshi]
    /// 00D0 : OpCode.REVERSE3 [2 datoshi]
    /// 00D1 : OpCode.CAT [2048 datoshi]
    /// 00D2 : OpCode.SWAP [2 datoshi]
    /// 00D3 : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 00D8 : OpCode.LDLOC5 [2 datoshi]
    /// 00D9 : OpCode.PUSHDATA1 737472696E67 [8 datoshi]
    /// 00E1 : OpCode.LDLOC2 [2 datoshi]
    /// 00E2 : OpCode.UNPACK [2048 datoshi]
    /// 00E3 : OpCode.DROP [2 datoshi]
    /// 00E4 : OpCode.REVERSE3 [2 datoshi]
    /// 00E5 : OpCode.CAT [2048 datoshi]
    /// 00E6 : OpCode.SWAP [2 datoshi]
    /// 00E7 : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 00EC : OpCode.LDLOC6 [2 datoshi]
    /// 00ED : OpCode.PUSHDATA1 75696E74313630 [8 datoshi]
    /// 00F6 : OpCode.LDLOC2 [2 datoshi]
    /// 00F7 : OpCode.UNPACK [2048 datoshi]
    /// 00F8 : OpCode.DROP [2 datoshi]
    /// 00F9 : OpCode.REVERSE3 [2 datoshi]
    /// 00FA : OpCode.CAT [2048 datoshi]
    /// 00FB : OpCode.SWAP [2 datoshi]
    /// 00FC : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 0101 : OpCode.LDLOC 07 [2 datoshi]
    /// 0103 : OpCode.PUSHDATA1 75696E74323536 [8 datoshi]
    /// 010C : OpCode.LDLOC2 [2 datoshi]
    /// 010D : OpCode.UNPACK [2048 datoshi]
    /// 010E : OpCode.DROP [2 datoshi]
    /// 010F : OpCode.REVERSE3 [2 datoshi]
    /// 0110 : OpCode.CAT [2048 datoshi]
    /// 0111 : OpCode.SWAP [2 datoshi]
    /// 0112 : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 0117 : OpCode.LDLOC 08 [2 datoshi]
    /// 0119 : OpCode.PUSHDATA1 6563706F696E74 [8 datoshi]
    /// 0122 : OpCode.LDLOC2 [2 datoshi]
    /// 0123 : OpCode.UNPACK [2048 datoshi]
    /// 0124 : OpCode.DROP [2 datoshi]
    /// 0125 : OpCode.REVERSE3 [2 datoshi]
    /// 0126 : OpCode.CAT [2048 datoshi]
    /// 0127 : OpCode.SWAP [2 datoshi]
    /// 0128 : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 012D : OpCode.PUSHDATA1 626F6F6C [8 datoshi]
    /// 0133 : OpCode.LDLOC2 [2 datoshi]
    /// 0134 : OpCode.UNPACK [2048 datoshi]
    /// 0135 : OpCode.DROP [2 datoshi]
    /// 0136 : OpCode.REVERSE3 [2 datoshi]
    /// 0137 : OpCode.CAT [2048 datoshi]
    /// 0138 : OpCode.SWAP [2 datoshi]
    /// 0139 : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 013E : OpCode.NOT [4 datoshi]
    /// 013F : OpCode.NOT [4 datoshi]
    /// 0140 : OpCode.STLOC 09 [2 datoshi]
    /// 0142 : OpCode.PUSHDATA1 696E74 [8 datoshi]
    /// 0147 : OpCode.LDLOC2 [2 datoshi]
    /// 0148 : OpCode.UNPACK [2048 datoshi]
    /// 0149 : OpCode.DROP [2 datoshi]
    /// 014A : OpCode.REVERSE3 [2 datoshi]
    /// 014B : OpCode.CAT [2048 datoshi]
    /// 014C : OpCode.SWAP [2 datoshi]
    /// 014D : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 0152 : OpCode.CONVERT (Integer) [8192 datoshi]
    /// 0154 : OpCode.STLOC 0A [2 datoshi]
    /// 0156 : OpCode.PUSHDATA1 737472696E67 [8 datoshi]
    /// 015E : OpCode.LDLOC2 [2 datoshi]
    /// 015F : OpCode.UNPACK [2048 datoshi]
    /// 0160 : OpCode.DROP [2 datoshi]
    /// 0161 : OpCode.REVERSE3 [2 datoshi]
    /// 0162 : OpCode.CAT [2048 datoshi]
    /// 0163 : OpCode.SWAP [2 datoshi]
    /// 0164 : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 0169 : OpCode.STLOC 0B [2 datoshi]
    /// 016B : OpCode.PUSHDATA1 75696E74313630 [8 datoshi]
    /// 0174 : OpCode.LDLOC2 [2 datoshi]
    /// 0175 : OpCode.UNPACK [2048 datoshi]
    /// 0176 : OpCode.DROP [2 datoshi]
    /// 0177 : OpCode.REVERSE3 [2 datoshi]
    /// 0178 : OpCode.CAT [2048 datoshi]
    /// 0179 : OpCode.SWAP [2 datoshi]
    /// 017A : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 017F : OpCode.STLOC 0C [2 datoshi]
    /// 0181 : OpCode.PUSHDATA1 75696E74323536 [8 datoshi]
    /// 018A : OpCode.LDLOC2 [2 datoshi]
    /// 018B : OpCode.UNPACK [2048 datoshi]
    /// 018C : OpCode.DROP [2 datoshi]
    /// 018D : OpCode.REVERSE3 [2 datoshi]
    /// 018E : OpCode.CAT [2048 datoshi]
    /// 018F : OpCode.SWAP [2 datoshi]
    /// 0190 : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 0195 : OpCode.STLOC 0D [2 datoshi]
    /// 0197 : OpCode.PUSHDATA1 6563706F696E74 [8 datoshi]
    /// 01A0 : OpCode.LDLOC2 [2 datoshi]
    /// 01A1 : OpCode.UNPACK [2048 datoshi]
    /// 01A2 : OpCode.DROP [2 datoshi]
    /// 01A3 : OpCode.REVERSE3 [2 datoshi]
    /// 01A4 : OpCode.CAT [2048 datoshi]
    /// 01A5 : OpCode.SWAP [2 datoshi]
    /// 01A6 : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 01AB : OpCode.STLOC 0E [2 datoshi]
    /// 01AD : OpCode.LDLOC3 [2 datoshi]
    /// 01AE : OpCode.LDLOC 09 [2 datoshi]
    /// 01B0 : OpCode.EQUAL [32 datoshi]
    /// 01B1 : OpCode.JMPIF 05 [2 datoshi]
    /// 01B3 : OpCode.PUSHF [1 datoshi]
    /// 01B4 : OpCode.JMP 06 [2 datoshi]
    /// 01B6 : OpCode.LDLOC4 [2 datoshi]
    /// 01B7 : OpCode.LDLOC 0A [2 datoshi]
    /// 01B9 : OpCode.EQUAL [32 datoshi]
    /// 01BA : OpCode.JMPIF 05 [2 datoshi]
    /// 01BC : OpCode.PUSHF [1 datoshi]
    /// 01BD : OpCode.JMP 06 [2 datoshi]
    /// 01BF : OpCode.LDLOC5 [2 datoshi]
    /// 01C0 : OpCode.LDLOC 0B [2 datoshi]
    /// 01C2 : OpCode.EQUAL [32 datoshi]
    /// 01C3 : OpCode.JMPIF 05 [2 datoshi]
    /// 01C5 : OpCode.PUSHF [1 datoshi]
    /// 01C6 : OpCode.JMP 06 [2 datoshi]
    /// 01C8 : OpCode.LDLOC6 [2 datoshi]
    /// 01C9 : OpCode.LDLOC 0C [2 datoshi]
    /// 01CB : OpCode.EQUAL [32 datoshi]
    /// 01CC : OpCode.JMPIF 05 [2 datoshi]
    /// 01CE : OpCode.PUSHF [1 datoshi]
    /// 01CF : OpCode.JMP 07 [2 datoshi]
    /// 01D1 : OpCode.LDLOC 07 [2 datoshi]
    /// 01D3 : OpCode.LDLOC 0D [2 datoshi]
    /// 01D5 : OpCode.EQUAL [32 datoshi]
    /// 01D6 : OpCode.JMPIF 04 [2 datoshi]
    /// 01D8 : OpCode.PUSHF [1 datoshi]
    /// 01D9 : OpCode.RET [0 datoshi]
    /// 01DA : OpCode.LDLOC 08 [2 datoshi]
    /// 01DC : OpCode.LDLOC 0E [2 datoshi]
    /// 01DE : OpCode.EQUAL [32 datoshi]
    /// 01DF : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testNewGetMethods")]
    public abstract bool? TestNewGetMethods();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADDsAMgMjIyMjAiMjAiMjAiMjAiMjAiMjAtswcAx0ZXN0X21hcEGb9mfOEsBxaNsoDAHbMNsoacFFU4tQQeY/GIQMAdsw2yhpwUVTi1BBkl3oMdswQA==
    /// 00 : OpCode.INITSLOT 0200 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 3B0032032323232302232302232302232302232302232302 [8 datoshi]
    /// 1D : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 1F : OpCode.STLOC0 [2 datoshi]
    /// 20 : OpCode.PUSHDATA1 746573745F6D6170 [8 datoshi]
    /// 2A : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 2F : OpCode.PUSH2 [1 datoshi]
    /// 30 : OpCode.PACK [2048 datoshi]
    /// 31 : OpCode.STLOC1 [2 datoshi]
    /// 32 : OpCode.LDLOC0 [2 datoshi]
    /// 33 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 35 : OpCode.PUSHDATA1 01 [8 datoshi]
    /// 38 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 3A : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 3C : OpCode.LDLOC1 [2 datoshi]
    /// 3D : OpCode.UNPACK [2048 datoshi]
    /// 3E : OpCode.DROP [2 datoshi]
    /// 3F : OpCode.REVERSE3 [2 datoshi]
    /// 40 : OpCode.CAT [2048 datoshi]
    /// 41 : OpCode.SWAP [2 datoshi]
    /// 42 : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 47 : OpCode.PUSHDATA1 01 [8 datoshi]
    /// 4A : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 4C : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 4E : OpCode.LDLOC1 [2 datoshi]
    /// 4F : OpCode.UNPACK [2048 datoshi]
    /// 50 : OpCode.DROP [2 datoshi]
    /// 51 : OpCode.REVERSE3 [2 datoshi]
    /// 52 : OpCode.CAT [2048 datoshi]
    /// 53 : OpCode.SWAP [2 datoshi]
    /// 54 : OpCode.SYSCALL 925DE831 'System.Storage.Get' [32768 datoshi]
    /// 59 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 5B : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testOver16Bytes")]
    public abstract byte[]? TestOver16Bytes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQZv2Z84AERGIThBR0FASwHB52yh42yhowUVTi1BB5j8YhAhA
    /// 00 : OpCode.INITSLOT 0102 [64 datoshi]
    /// 03 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 08 : OpCode.PUSHINT8 11 [1 datoshi]
    /// 0A : OpCode.PUSH1 [1 datoshi]
    /// 0B : OpCode.NEWBUFFER [256 datoshi]
    /// 0C : OpCode.TUCK [2 datoshi]
    /// 0D : OpCode.PUSH0 [1 datoshi]
    /// 0E : OpCode.ROT [2 datoshi]
    /// 0F : OpCode.SETITEM [8192 datoshi]
    /// 10 : OpCode.SWAP [2 datoshi]
    /// 11 : OpCode.PUSH2 [1 datoshi]
    /// 12 : OpCode.PACK [2048 datoshi]
    /// 13 : OpCode.STLOC0 [2 datoshi]
    /// 14 : OpCode.LDARG1 [2 datoshi]
    /// 15 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 17 : OpCode.LDARG0 [2 datoshi]
    /// 18 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 1A : OpCode.LDLOC0 [2 datoshi]
    /// 1B : OpCode.UNPACK [2048 datoshi]
    /// 1C : OpCode.DROP [2 datoshi]
    /// 1D : OpCode.REVERSE3 [2 datoshi]
    /// 1E : OpCode.CAT [2048 datoshi]
    /// 1F : OpCode.SWAP [2 datoshi]
    /// 20 : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 25 : OpCode.PUSHT [1 datoshi]
    /// 26 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPutByte")]
    public abstract bool? TestPutByte(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDAD/2zBwaEGb9mfOEsBxedsoeNsoacFFU4tQQeY/GIQIQA==
    /// 00 : OpCode.INITSLOT 0202 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 00FF [8 datoshi]
    /// 07 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.LDLOC0 [2 datoshi]
    /// 0B : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 10 : OpCode.PUSH2 [1 datoshi]
    /// 11 : OpCode.PACK [2048 datoshi]
    /// 12 : OpCode.STLOC1 [2 datoshi]
    /// 13 : OpCode.LDARG1 [2 datoshi]
    /// 14 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 16 : OpCode.LDARG0 [2 datoshi]
    /// 17 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 19 : OpCode.LDLOC1 [2 datoshi]
    /// 1A : OpCode.UNPACK [2048 datoshi]
    /// 1B : OpCode.DROP [2 datoshi]
    /// 1C : OpCode.REVERSE3 [2 datoshi]
    /// 1D : OpCode.CAT [2048 datoshi]
    /// 1E : OpCode.SWAP [2 datoshi]
    /// 1F : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 24 : OpCode.PUSHT [1 datoshi]
    /// 25 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPutByteArray")]
    public abstract bool? TestPutByteArray(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCDAD/2zBwQZv2Z85Bdky/6XFoaRLAcnnbKHjbKGrBRVOLUEHmPxiECEA=
    /// 00 : OpCode.INITSLOT 0302 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 00FF [8 datoshi]
    /// 07 : OpCode.CONVERT (Buffer) [8192 datoshi]
    /// 09 : OpCode.STLOC0 [2 datoshi]
    /// 0A : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0F : OpCode.SYSCALL 764CBFE9 'System.Storage.AsReadOnly' [16 datoshi]
    /// 14 : OpCode.STLOC1 [2 datoshi]
    /// 15 : OpCode.LDLOC0 [2 datoshi]
    /// 16 : OpCode.LDLOC1 [2 datoshi]
    /// 17 : OpCode.PUSH2 [1 datoshi]
    /// 18 : OpCode.PACK [2048 datoshi]
    /// 19 : OpCode.STLOC2 [2 datoshi]
    /// 1A : OpCode.LDARG1 [2 datoshi]
    /// 1B : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 1D : OpCode.LDARG0 [2 datoshi]
    /// 1E : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 20 : OpCode.LDLOC2 [2 datoshi]
    /// 21 : OpCode.UNPACK [2048 datoshi]
    /// 22 : OpCode.DROP [2 datoshi]
    /// 23 : OpCode.REVERSE3 [2 datoshi]
    /// 24 : OpCode.CAT [2048 datoshi]
    /// 25 : OpCode.SWAP [2 datoshi]
    /// 26 : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 2B : OpCode.PUSHT [1 datoshi]
    /// 2C : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPutReadOnly")]
    public abstract bool? TestPutReadOnly(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDGFhcGhBm/ZnzhLAcXnbKHjbKGnBRVOLUEHmPxiECEA=
    /// 00 : OpCode.INITSLOT 0202 [64 datoshi]
    /// 03 : OpCode.PUSHDATA1 6161 [8 datoshi]
    /// 07 : OpCode.STLOC0 [2 datoshi]
    /// 08 : OpCode.LDLOC0 [2 datoshi]
    /// 09 : OpCode.SYSCALL 9BF667CE 'System.Storage.GetContext' [16 datoshi]
    /// 0E : OpCode.PUSH2 [1 datoshi]
    /// 0F : OpCode.PACK [2048 datoshi]
    /// 10 : OpCode.STLOC1 [2 datoshi]
    /// 11 : OpCode.LDARG1 [2 datoshi]
    /// 12 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 14 : OpCode.LDARG0 [2 datoshi]
    /// 15 : OpCode.CONVERT (ByteString) [8192 datoshi]
    /// 17 : OpCode.LDLOC1 [2 datoshi]
    /// 18 : OpCode.UNPACK [2048 datoshi]
    /// 19 : OpCode.DROP [2 datoshi]
    /// 1A : OpCode.REVERSE3 [2 datoshi]
    /// 1B : OpCode.CAT [2048 datoshi]
    /// 1C : OpCode.SWAP [2 datoshi]
    /// 1D : OpCode.SYSCALL E63F1884 'System.Storage.Put' [32768 datoshi]
    /// 22 : OpCode.PUSHT [1 datoshi]
    /// 23 : OpCode.RET [0 datoshi]
    /// </remarks>
    [DisplayName("testPutString")]
    public abstract bool? TestPutString(byte[]? key, byte[]? value);

    #endregion
}
