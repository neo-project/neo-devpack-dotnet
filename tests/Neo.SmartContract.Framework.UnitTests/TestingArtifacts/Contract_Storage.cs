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
    /// 00 : OpCode.INITSLOT 0402
    /// 03 : OpCode.PUSHDATA1 01AA
    /// 07 : OpCode.CONVERT 30
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.SYSCALL 9BF667CE
    /// 0F : OpCode.STLOC1
    /// 10 : OpCode.LDLOC0
    /// 11 : OpCode.LDLOC1
    /// 12 : OpCode.PUSH2
    /// 13 : OpCode.PACK
    /// 14 : OpCode.STLOC2
    /// 15 : OpCode.PUSH0
    /// 16 : OpCode.PUSH1
    /// 17 : OpCode.PACK
    /// 18 : OpCode.DUP
    /// 19 : OpCode.PUSH0
    /// 1A : OpCode.LDARG1
    /// 1B : OpCode.SETITEM
    /// 1C : OpCode.STLOC3
    /// 1D : OpCode.LDLOC3
    /// 1E : OpCode.LDARG0
    /// 1F : OpCode.LDLOC2
    /// 20 : OpCode.CALL 0B
    /// 22 : OpCode.LDARG0
    /// 23 : OpCode.LDLOC2
    /// 24 : OpCode.CALL 1B
    /// 26 : OpCode.STLOC3
    /// 27 : OpCode.LDLOC3
    /// 28 : OpCode.PUSH0
    /// 29 : OpCode.PICKITEM
    /// 2A : OpCode.RET
    /// </remarks>
    [DisplayName("serializeTest")]
    public abstract BigInteger? SerializeTest(byte[]? key, BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwEBQZv2Z84AERGIThBR0FASwHB42yhowUVTi1BBL1jF7UA=
    /// 00 : OpCode.INITSLOT 0101
    /// 03 : OpCode.SYSCALL 9BF667CE
    /// 08 : OpCode.PUSHINT8 11
    /// 0A : OpCode.PUSH1
    /// 0B : OpCode.NEWBUFFER
    /// 0C : OpCode.TUCK
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.ROT
    /// 0F : OpCode.SETITEM
    /// 10 : OpCode.SWAP
    /// 11 : OpCode.PUSH2
    /// 12 : OpCode.PACK
    /// 13 : OpCode.STLOC0
    /// 14 : OpCode.LDARG0
    /// 15 : OpCode.CONVERT 28
    /// 17 : OpCode.LDLOC0
    /// 18 : OpCode.UNPACK
    /// 19 : OpCode.DROP
    /// 1A : OpCode.REVERSE3
    /// 1B : OpCode.CAT
    /// 1C : OpCode.SWAP
    /// 1D : OpCode.SYSCALL 2F58C5ED
    /// 22 : OpCode.RET
    /// </remarks>
    [DisplayName("testDeleteByte")]
    public abstract void TestDeleteByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDAD/2zBwaEGb9mfOEsBxeNsoacFFU4tQQS9Yxe1A
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.PUSHDATA1 00FF
    /// 07 : OpCode.CONVERT 30
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.LDLOC0
    /// 0B : OpCode.SYSCALL 9BF667CE
    /// 10 : OpCode.PUSH2
    /// 11 : OpCode.PACK
    /// 12 : OpCode.STLOC1
    /// 13 : OpCode.LDARG0
    /// 14 : OpCode.CONVERT 28
    /// 16 : OpCode.LDLOC1
    /// 17 : OpCode.UNPACK
    /// 18 : OpCode.DROP
    /// 19 : OpCode.REVERSE3
    /// 1A : OpCode.CAT
    /// 1B : OpCode.SWAP
    /// 1C : OpCode.SYSCALL 2F58C5ED
    /// 21 : OpCode.RET
    /// </remarks>
    [DisplayName("testDeleteByteArray")]
    public abstract void TestDeleteByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIBDGFhcGhBm/ZnzhLAcXjbKGnBRVOLUEEvWMXtQA==
    /// 00 : OpCode.INITSLOT 0201
    /// 03 : OpCode.PUSHDATA1 6161
    /// 07 : OpCode.STLOC0
    /// 08 : OpCode.LDLOC0
    /// 09 : OpCode.SYSCALL 9BF667CE
    /// 0E : OpCode.PUSH2
    /// 0F : OpCode.PACK
    /// 10 : OpCode.STLOC1
    /// 11 : OpCode.LDARG0
    /// 12 : OpCode.CONVERT 28
    /// 14 : OpCode.LDLOC1
    /// 15 : OpCode.UNPACK
    /// 16 : OpCode.DROP
    /// 17 : OpCode.REVERSE3
    /// 18 : OpCode.CAT
    /// 19 : OpCode.SWAP
    /// 1A : OpCode.SYSCALL 2F58C5ED
    /// 1F : OpCode.RET
    /// </remarks>
    [DisplayName("testDeleteString")]
    public abstract void TestDeleteString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIAQZv2Z85wDAHbMNsoDGtleTFoQeY/GIQMAtsw2ygMa2V5MmhB5j8YhBQMa2V5aEHfMLiacWlBnAjtnEVpQfNUvx1A
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.SYSCALL 9BF667CE
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.PUSHDATA1 01
    /// 0C : OpCode.CONVERT 30
    /// 0E : OpCode.CONVERT 28
    /// 10 : OpCode.PUSHDATA1 6B657931
    /// 16 : OpCode.LDLOC0
    /// 17 : OpCode.SYSCALL E63F1884
    /// 1C : OpCode.PUSHDATA1 02
    /// 1F : OpCode.CONVERT 30
    /// 21 : OpCode.CONVERT 28
    /// 23 : OpCode.PUSHDATA1 6B657932
    /// 29 : OpCode.LDLOC0
    /// 2A : OpCode.SYSCALL E63F1884
    /// 2F : OpCode.PUSH4
    /// 30 : OpCode.PUSHDATA1 6B6579
    /// 35 : OpCode.LDLOC0
    /// 36 : OpCode.SYSCALL DF30B89A
    /// 3B : OpCode.STLOC1
    /// 3C : OpCode.LDLOC1
    /// 3D : OpCode.SYSCALL 9C08ED9C
    /// 42 : OpCode.DROP
    /// 43 : OpCode.LDLOC1
    /// 44 : OpCode.SYSCALL F354BF1D
    /// 49 : OpCode.RET
    /// </remarks>
    [DisplayName("testFind")]
    public abstract byte[]? TestFind();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMBQfa0a+JwaAAREYhOEFHQUBLAcXjbKGnBRVOLUEGSXegxcmrbMEA=
    /// 00 : OpCode.INITSLOT 0301
    /// 03 : OpCode.SYSCALL F6B46BE2
    /// 08 : OpCode.STLOC0
    /// 09 : OpCode.LDLOC0
    /// 0A : OpCode.PUSHINT8 11
    /// 0C : OpCode.PUSH1
    /// 0D : OpCode.NEWBUFFER
    /// 0E : OpCode.TUCK
    /// 0F : OpCode.PUSH0
    /// 10 : OpCode.ROT
    /// 11 : OpCode.SETITEM
    /// 12 : OpCode.SWAP
    /// 13 : OpCode.PUSH2
    /// 14 : OpCode.PACK
    /// 15 : OpCode.STLOC1
    /// 16 : OpCode.LDARG0
    /// 17 : OpCode.CONVERT 28
    /// 19 : OpCode.LDLOC1
    /// 1A : OpCode.UNPACK
    /// 1B : OpCode.DROP
    /// 1C : OpCode.REVERSE3
    /// 1D : OpCode.CAT
    /// 1E : OpCode.SWAP
    /// 1F : OpCode.SYSCALL 925DE831
    /// 24 : OpCode.STLOC2
    /// 25 : OpCode.LDLOC2
    /// 26 : OpCode.CONVERT 30
    /// 28 : OpCode.RET
    /// </remarks>
    [DisplayName("testGetByte")]
    public abstract byte[]? TestGetByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBDAD/2zBwQZv2Z85Bdky/6XFoaRLAcnjbKGrBRVOLUEGSXegxc2vbMEA=
    /// 00 : OpCode.INITSLOT 0401
    /// 03 : OpCode.PUSHDATA1 00FF
    /// 07 : OpCode.CONVERT 30
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.SYSCALL 9BF667CE
    /// 0F : OpCode.SYSCALL 764CBFE9
    /// 14 : OpCode.STLOC1
    /// 15 : OpCode.LDLOC0
    /// 16 : OpCode.LDLOC1
    /// 17 : OpCode.PUSH2
    /// 18 : OpCode.PACK
    /// 19 : OpCode.STLOC2
    /// 1A : OpCode.LDARG0
    /// 1B : OpCode.CONVERT 28
    /// 1D : OpCode.LDLOC2
    /// 1E : OpCode.UNPACK
    /// 1F : OpCode.DROP
    /// 20 : OpCode.REVERSE3
    /// 21 : OpCode.CAT
    /// 22 : OpCode.SWAP
    /// 23 : OpCode.SYSCALL 925DE831
    /// 28 : OpCode.STLOC3
    /// 29 : OpCode.LDLOC3
    /// 2A : OpCode.CONVERT 30
    /// 2C : OpCode.RET
    /// </remarks>
    [DisplayName("testGetByteArray")]
    public abstract byte[]? TestGetByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBDGFhcEH2tGvicWhpEsByeNsoasFFU4tQQZJd6DFza9swQA==
    /// 00 : OpCode.INITSLOT 0401
    /// 03 : OpCode.PUSHDATA1 6161
    /// 07 : OpCode.STLOC0
    /// 08 : OpCode.SYSCALL F6B46BE2
    /// 0D : OpCode.STLOC1
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.LDLOC1
    /// 10 : OpCode.PUSH2
    /// 11 : OpCode.PACK
    /// 12 : OpCode.STLOC2
    /// 13 : OpCode.LDARG0
    /// 14 : OpCode.CONVERT 28
    /// 16 : OpCode.LDLOC2
    /// 17 : OpCode.UNPACK
    /// 18 : OpCode.DROP
    /// 19 : OpCode.REVERSE3
    /// 1A : OpCode.CAT
    /// 1B : OpCode.SWAP
    /// 1C : OpCode.SYSCALL 925DE831
    /// 21 : OpCode.STLOC3
    /// 22 : OpCode.LDLOC3
    /// 23 : OpCode.CONVERT 30
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("testGetString")]
    public abstract byte[]? TestGetString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwQBDGlpcEH2tGvicWhpEsByeNsoasFFU4tQQZJd6DFza9swQA==
    /// 00 : OpCode.INITSLOT 0401
    /// 03 : OpCode.PUSHDATA1 6969
    /// 07 : OpCode.STLOC0
    /// 08 : OpCode.SYSCALL F6B46BE2
    /// 0D : OpCode.STLOC1
    /// 0E : OpCode.LDLOC0
    /// 0F : OpCode.LDLOC1
    /// 10 : OpCode.PUSH2
    /// 11 : OpCode.PACK
    /// 12 : OpCode.STLOC2
    /// 13 : OpCode.LDARG0
    /// 14 : OpCode.CONVERT 28
    /// 16 : OpCode.LDLOC2
    /// 17 : OpCode.UNPACK
    /// 18 : OpCode.DROP
    /// 19 : OpCode.REVERSE3
    /// 1A : OpCode.CAT
    /// 1B : OpCode.SWAP
    /// 1C : OpCode.SYSCALL 925DE831
    /// 21 : OpCode.STLOC3
    /// 22 : OpCode.LDLOC3
    /// 23 : OpCode.CONVERT 30
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("testIndexGet")]
    public abstract byte[]? TestIndexGet(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDGlpcGhBm/ZnzhLAcXnbKEp42yhpwUVTi1BB5j8YhEUIQA==
    /// 00 : OpCode.INITSLOT 0202
    /// 03 : OpCode.PUSHDATA1 6969
    /// 07 : OpCode.STLOC0
    /// 08 : OpCode.LDLOC0
    /// 09 : OpCode.SYSCALL 9BF667CE
    /// 0E : OpCode.PUSH2
    /// 0F : OpCode.PACK
    /// 10 : OpCode.STLOC1
    /// 11 : OpCode.LDARG1
    /// 12 : OpCode.CONVERT 28
    /// 14 : OpCode.DUP
    /// 15 : OpCode.LDARG0
    /// 16 : OpCode.CONVERT 28
    /// 18 : OpCode.LDLOC1
    /// 19 : OpCode.UNPACK
    /// 1A : OpCode.DROP
    /// 1B : OpCode.REVERSE3
    /// 1C : OpCode.CAT
    /// 1D : OpCode.SWAP
    /// 1E : OpCode.SYSCALL E63F1884
    /// 23 : OpCode.DROP
    /// 24 : OpCode.PUSHT
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("testIndexPut")]
    public abstract bool? TestIndexPut(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwUADAD/2zBwQZv2Z85xaGkSwHIMAAHbMHNrDGJ5dGVBcnJheWrBRVOLUEHmPxiEDGJ5dGVBcnJheWrBRVOLUEGSXegx2zB0bEA=
    /// 00 : OpCode.INITSLOT 0500
    /// 03 : OpCode.PUSHDATA1 00FF
    /// 07 : OpCode.CONVERT 30
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.SYSCALL 9BF667CE
    /// 0F : OpCode.STLOC1
    /// 10 : OpCode.LDLOC0
    /// 11 : OpCode.LDLOC1
    /// 12 : OpCode.PUSH2
    /// 13 : OpCode.PACK
    /// 14 : OpCode.STLOC2
    /// 15 : OpCode.PUSHDATA1 0001
    /// 19 : OpCode.CONVERT 30
    /// 1B : OpCode.STLOC3
    /// 1C : OpCode.LDLOC3
    /// 1D : OpCode.PUSHDATA1 627974654172726179
    /// 28 : OpCode.LDLOC2
    /// 29 : OpCode.UNPACK
    /// 2A : OpCode.DROP
    /// 2B : OpCode.REVERSE3
    /// 2C : OpCode.CAT
    /// 2D : OpCode.SWAP
    /// 2E : OpCode.SYSCALL E63F1884
    /// 33 : OpCode.PUSHDATA1 627974654172726179
    /// 3E : OpCode.LDLOC2
    /// 3F : OpCode.UNPACK
    /// 40 : OpCode.DROP
    /// 41 : OpCode.REVERSE3
    /// 42 : OpCode.CAT
    /// 43 : OpCode.SWAP
    /// 44 : OpCode.SYSCALL 925DE831
    /// 49 : OpCode.CONVERT 30
    /// 4B : OpCode.STLOC4
    /// 4C : OpCode.LDLOC4
    /// 4D : OpCode.RET
    /// </remarks>
    [DisplayName("testNewGetByteArray")]
    public abstract byte[]? TestNewGetByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: Vw8ADAD/2zBwQZv2Z85xaGkSwHIIcwB7dAxoZWxsbyB3b3JsZHUMAAECAwQFBgcICQABAgMEBQYHCAnbMNsoStgkCUrKABQoAzp2DAABAgMEBQYHCAkAAQIDBAUGBwgJAAECAwQFBgcICQAB2zDbKErYJAlKygAgKAM6dwcMAAECAwQFBgcICQABAgMEBQYHCAkAAQIDBAUGBwgJAAEC2zDbKErYJAlKygAhKAM6dwhrDGJvb2xqwUVTi1BB5j8YhGwMaW50asFFU4tQQeY/GIRtDHN0cmluZ2rBRVOLUEHmPxiEbgx1aW50MTYwasFFU4tQQeY/GIRvBwx1aW50MjU2asFFU4tQQeY/GIRvCAxlY3BvaW50asFFU4tQQeY/GIQMYm9vbGrBRVOLUEGSXegxqqp3CQxpbnRqwUVTi1BBkl3oMdshdwoMc3RyaW5nasFFU4tQQZJd6DF3Cwx1aW50MTYwasFFU4tQQZJd6DF3DAx1aW50MjU2asFFU4tQQZJd6DF3DQxlY3BvaW50asFFU4tQQZJd6DF3DmtvCZckBQkiBmxvCpckBQkiBm1vC5ckBQkiBm5vDJckBQkiB28Hbw2XJAQJQG8Ibw6XQA==
    /// 0000 : OpCode.INITSLOT 0F00
    /// 0003 : OpCode.PUSHDATA1 00FF
    /// 0007 : OpCode.CONVERT 30
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.SYSCALL 9BF667CE
    /// 000F : OpCode.STLOC1
    /// 0010 : OpCode.LDLOC0
    /// 0011 : OpCode.LDLOC1
    /// 0012 : OpCode.PUSH2
    /// 0013 : OpCode.PACK
    /// 0014 : OpCode.STLOC2
    /// 0015 : OpCode.PUSHT
    /// 0016 : OpCode.STLOC3
    /// 0017 : OpCode.PUSHINT8 7B
    /// 0019 : OpCode.STLOC4
    /// 001A : OpCode.PUSHDATA1 68656C6C6F20776F726C64
    /// 0027 : OpCode.STLOC5
    /// 0028 : OpCode.PUSHDATA1 0001020304050607080900010203040506070809
    /// 003E : OpCode.CONVERT 30
    /// 0040 : OpCode.CONVERT 28
    /// 0042 : OpCode.DUP
    /// 0043 : OpCode.ISNULL
    /// 0044 : OpCode.JMPIF 09
    /// 0046 : OpCode.DUP
    /// 0047 : OpCode.SIZE
    /// 0048 : OpCode.PUSHINT8 14
    /// 004A : OpCode.JMPEQ 03
    /// 004C : OpCode.THROW
    /// 004D : OpCode.STLOC6
    /// 004E : OpCode.PUSHDATA1 0001020304050607080900010203040506070809000102030405060708090001
    /// 0070 : OpCode.CONVERT 30
    /// 0072 : OpCode.CONVERT 28
    /// 0074 : OpCode.DUP
    /// 0075 : OpCode.ISNULL
    /// 0076 : OpCode.JMPIF 09
    /// 0078 : OpCode.DUP
    /// 0079 : OpCode.SIZE
    /// 007A : OpCode.PUSHINT8 20
    /// 007C : OpCode.JMPEQ 03
    /// 007E : OpCode.THROW
    /// 007F : OpCode.STLOC 07
    /// 0081 : OpCode.PUSHDATA1 000102030405060708090001020304050607080900010203040506070809000102
    /// 00A4 : OpCode.CONVERT 30
    /// 00A6 : OpCode.CONVERT 28
    /// 00A8 : OpCode.DUP
    /// 00A9 : OpCode.ISNULL
    /// 00AA : OpCode.JMPIF 09
    /// 00AC : OpCode.DUP
    /// 00AD : OpCode.SIZE
    /// 00AE : OpCode.PUSHINT8 21
    /// 00B0 : OpCode.JMPEQ 03
    /// 00B2 : OpCode.THROW
    /// 00B3 : OpCode.STLOC 08
    /// 00B5 : OpCode.LDLOC3
    /// 00B6 : OpCode.PUSHDATA1 626F6F6C
    /// 00BC : OpCode.LDLOC2
    /// 00BD : OpCode.UNPACK
    /// 00BE : OpCode.DROP
    /// 00BF : OpCode.REVERSE3
    /// 00C0 : OpCode.CAT
    /// 00C1 : OpCode.SWAP
    /// 00C2 : OpCode.SYSCALL E63F1884
    /// 00C7 : OpCode.LDLOC4
    /// 00C8 : OpCode.PUSHDATA1 696E74
    /// 00CD : OpCode.LDLOC2
    /// 00CE : OpCode.UNPACK
    /// 00CF : OpCode.DROP
    /// 00D0 : OpCode.REVERSE3
    /// 00D1 : OpCode.CAT
    /// 00D2 : OpCode.SWAP
    /// 00D3 : OpCode.SYSCALL E63F1884
    /// 00D8 : OpCode.LDLOC5
    /// 00D9 : OpCode.PUSHDATA1 737472696E67
    /// 00E1 : OpCode.LDLOC2
    /// 00E2 : OpCode.UNPACK
    /// 00E3 : OpCode.DROP
    /// 00E4 : OpCode.REVERSE3
    /// 00E5 : OpCode.CAT
    /// 00E6 : OpCode.SWAP
    /// 00E7 : OpCode.SYSCALL E63F1884
    /// 00EC : OpCode.LDLOC6
    /// 00ED : OpCode.PUSHDATA1 75696E74313630
    /// 00F6 : OpCode.LDLOC2
    /// 00F7 : OpCode.UNPACK
    /// 00F8 : OpCode.DROP
    /// 00F9 : OpCode.REVERSE3
    /// 00FA : OpCode.CAT
    /// 00FB : OpCode.SWAP
    /// 00FC : OpCode.SYSCALL E63F1884
    /// 0101 : OpCode.LDLOC 07
    /// 0103 : OpCode.PUSHDATA1 75696E74323536
    /// 010C : OpCode.LDLOC2
    /// 010D : OpCode.UNPACK
    /// 010E : OpCode.DROP
    /// 010F : OpCode.REVERSE3
    /// 0110 : OpCode.CAT
    /// 0111 : OpCode.SWAP
    /// 0112 : OpCode.SYSCALL E63F1884
    /// 0117 : OpCode.LDLOC 08
    /// 0119 : OpCode.PUSHDATA1 6563706F696E74
    /// 0122 : OpCode.LDLOC2
    /// 0123 : OpCode.UNPACK
    /// 0124 : OpCode.DROP
    /// 0125 : OpCode.REVERSE3
    /// 0126 : OpCode.CAT
    /// 0127 : OpCode.SWAP
    /// 0128 : OpCode.SYSCALL E63F1884
    /// 012D : OpCode.PUSHDATA1 626F6F6C
    /// 0133 : OpCode.LDLOC2
    /// 0134 : OpCode.UNPACK
    /// 0135 : OpCode.DROP
    /// 0136 : OpCode.REVERSE3
    /// 0137 : OpCode.CAT
    /// 0138 : OpCode.SWAP
    /// 0139 : OpCode.SYSCALL 925DE831
    /// 013E : OpCode.NOT
    /// 013F : OpCode.NOT
    /// 0140 : OpCode.STLOC 09
    /// 0142 : OpCode.PUSHDATA1 696E74
    /// 0147 : OpCode.LDLOC2
    /// 0148 : OpCode.UNPACK
    /// 0149 : OpCode.DROP
    /// 014A : OpCode.REVERSE3
    /// 014B : OpCode.CAT
    /// 014C : OpCode.SWAP
    /// 014D : OpCode.SYSCALL 925DE831
    /// 0152 : OpCode.CONVERT 21
    /// 0154 : OpCode.STLOC 0A
    /// 0156 : OpCode.PUSHDATA1 737472696E67
    /// 015E : OpCode.LDLOC2
    /// 015F : OpCode.UNPACK
    /// 0160 : OpCode.DROP
    /// 0161 : OpCode.REVERSE3
    /// 0162 : OpCode.CAT
    /// 0163 : OpCode.SWAP
    /// 0164 : OpCode.SYSCALL 925DE831
    /// 0169 : OpCode.STLOC 0B
    /// 016B : OpCode.PUSHDATA1 75696E74313630
    /// 0174 : OpCode.LDLOC2
    /// 0175 : OpCode.UNPACK
    /// 0176 : OpCode.DROP
    /// 0177 : OpCode.REVERSE3
    /// 0178 : OpCode.CAT
    /// 0179 : OpCode.SWAP
    /// 017A : OpCode.SYSCALL 925DE831
    /// 017F : OpCode.STLOC 0C
    /// 0181 : OpCode.PUSHDATA1 75696E74323536
    /// 018A : OpCode.LDLOC2
    /// 018B : OpCode.UNPACK
    /// 018C : OpCode.DROP
    /// 018D : OpCode.REVERSE3
    /// 018E : OpCode.CAT
    /// 018F : OpCode.SWAP
    /// 0190 : OpCode.SYSCALL 925DE831
    /// 0195 : OpCode.STLOC 0D
    /// 0197 : OpCode.PUSHDATA1 6563706F696E74
    /// 01A0 : OpCode.LDLOC2
    /// 01A1 : OpCode.UNPACK
    /// 01A2 : OpCode.DROP
    /// 01A3 : OpCode.REVERSE3
    /// 01A4 : OpCode.CAT
    /// 01A5 : OpCode.SWAP
    /// 01A6 : OpCode.SYSCALL 925DE831
    /// 01AB : OpCode.STLOC 0E
    /// 01AD : OpCode.LDLOC3
    /// 01AE : OpCode.LDLOC 09
    /// 01B0 : OpCode.EQUAL
    /// 01B1 : OpCode.JMPIF 05
    /// 01B3 : OpCode.PUSHF
    /// 01B4 : OpCode.JMP 06
    /// 01B6 : OpCode.LDLOC4
    /// 01B7 : OpCode.LDLOC 0A
    /// 01B9 : OpCode.EQUAL
    /// 01BA : OpCode.JMPIF 05
    /// 01BC : OpCode.PUSHF
    /// 01BD : OpCode.JMP 06
    /// 01BF : OpCode.LDLOC5
    /// 01C0 : OpCode.LDLOC 0B
    /// 01C2 : OpCode.EQUAL
    /// 01C3 : OpCode.JMPIF 05
    /// 01C5 : OpCode.PUSHF
    /// 01C6 : OpCode.JMP 06
    /// 01C8 : OpCode.LDLOC6
    /// 01C9 : OpCode.LDLOC 0C
    /// 01CB : OpCode.EQUAL
    /// 01CC : OpCode.JMPIF 05
    /// 01CE : OpCode.PUSHF
    /// 01CF : OpCode.JMP 07
    /// 01D1 : OpCode.LDLOC 07
    /// 01D3 : OpCode.LDLOC 0D
    /// 01D5 : OpCode.EQUAL
    /// 01D6 : OpCode.JMPIF 04
    /// 01D8 : OpCode.PUSHF
    /// 01D9 : OpCode.RET
    /// 01DA : OpCode.LDLOC 08
    /// 01DC : OpCode.LDLOC 0E
    /// 01DE : OpCode.EQUAL
    /// 01DF : OpCode.RET
    /// </remarks>
    [DisplayName("testNewGetMethods")]
    public abstract bool? TestNewGetMethods();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwIADDsAMgMjIyMjAiMjAiMjAiMjAiMjAiMjAtswcAx0ZXN0X21hcEGb9mfOEsBxaNsoDAHbMNsoacFFU4tQQeY/GIQMAdsw2yhpwUVTi1BBkl3oMdswQA==
    /// 00 : OpCode.INITSLOT 0200
    /// 03 : OpCode.PUSHDATA1 3B0032032323232302232302232302232302232302232302
    /// 1D : OpCode.CONVERT 30
    /// 1F : OpCode.STLOC0
    /// 20 : OpCode.PUSHDATA1 746573745F6D6170
    /// 2A : OpCode.SYSCALL 9BF667CE
    /// 2F : OpCode.PUSH2
    /// 30 : OpCode.PACK
    /// 31 : OpCode.STLOC1
    /// 32 : OpCode.LDLOC0
    /// 33 : OpCode.CONVERT 28
    /// 35 : OpCode.PUSHDATA1 01
    /// 38 : OpCode.CONVERT 30
    /// 3A : OpCode.CONVERT 28
    /// 3C : OpCode.LDLOC1
    /// 3D : OpCode.UNPACK
    /// 3E : OpCode.DROP
    /// 3F : OpCode.REVERSE3
    /// 40 : OpCode.CAT
    /// 41 : OpCode.SWAP
    /// 42 : OpCode.SYSCALL E63F1884
    /// 47 : OpCode.PUSHDATA1 01
    /// 4A : OpCode.CONVERT 30
    /// 4C : OpCode.CONVERT 28
    /// 4E : OpCode.LDLOC1
    /// 4F : OpCode.UNPACK
    /// 50 : OpCode.DROP
    /// 51 : OpCode.REVERSE3
    /// 52 : OpCode.CAT
    /// 53 : OpCode.SWAP
    /// 54 : OpCode.SYSCALL 925DE831
    /// 59 : OpCode.CONVERT 30
    /// 5B : OpCode.RET
    /// </remarks>
    [DisplayName("testOver16Bytes")]
    public abstract byte[]? TestOver16Bytes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwECQZv2Z84AERGIThBR0FASwHB52yh42yhowUVTi1BB5j8YhAhA
    /// 00 : OpCode.INITSLOT 0102
    /// 03 : OpCode.SYSCALL 9BF667CE
    /// 08 : OpCode.PUSHINT8 11
    /// 0A : OpCode.PUSH1
    /// 0B : OpCode.NEWBUFFER
    /// 0C : OpCode.TUCK
    /// 0D : OpCode.PUSH0
    /// 0E : OpCode.ROT
    /// 0F : OpCode.SETITEM
    /// 10 : OpCode.SWAP
    /// 11 : OpCode.PUSH2
    /// 12 : OpCode.PACK
    /// 13 : OpCode.STLOC0
    /// 14 : OpCode.LDARG1
    /// 15 : OpCode.CONVERT 28
    /// 17 : OpCode.LDARG0
    /// 18 : OpCode.CONVERT 28
    /// 1A : OpCode.LDLOC0
    /// 1B : OpCode.UNPACK
    /// 1C : OpCode.DROP
    /// 1D : OpCode.REVERSE3
    /// 1E : OpCode.CAT
    /// 1F : OpCode.SWAP
    /// 20 : OpCode.SYSCALL E63F1884
    /// 25 : OpCode.PUSHT
    /// 26 : OpCode.RET
    /// </remarks>
    [DisplayName("testPutByte")]
    public abstract bool? TestPutByte(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDAD/2zBwaEGb9mfOEsBxedsoeNsoacFFU4tQQeY/GIQIQA==
    /// 00 : OpCode.INITSLOT 0202
    /// 03 : OpCode.PUSHDATA1 00FF
    /// 07 : OpCode.CONVERT 30
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.LDLOC0
    /// 0B : OpCode.SYSCALL 9BF667CE
    /// 10 : OpCode.PUSH2
    /// 11 : OpCode.PACK
    /// 12 : OpCode.STLOC1
    /// 13 : OpCode.LDARG1
    /// 14 : OpCode.CONVERT 28
    /// 16 : OpCode.LDARG0
    /// 17 : OpCode.CONVERT 28
    /// 19 : OpCode.LDLOC1
    /// 1A : OpCode.UNPACK
    /// 1B : OpCode.DROP
    /// 1C : OpCode.REVERSE3
    /// 1D : OpCode.CAT
    /// 1E : OpCode.SWAP
    /// 1F : OpCode.SYSCALL E63F1884
    /// 24 : OpCode.PUSHT
    /// 25 : OpCode.RET
    /// </remarks>
    [DisplayName("testPutByteArray")]
    public abstract bool? TestPutByteArray(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwMCDAD/2zBwQZv2Z85Bdky/6XFoaRLAcnnbKHjbKGrBRVOLUEHmPxiECEA=
    /// 00 : OpCode.INITSLOT 0302
    /// 03 : OpCode.PUSHDATA1 00FF
    /// 07 : OpCode.CONVERT 30
    /// 09 : OpCode.STLOC0
    /// 0A : OpCode.SYSCALL 9BF667CE
    /// 0F : OpCode.SYSCALL 764CBFE9
    /// 14 : OpCode.STLOC1
    /// 15 : OpCode.LDLOC0
    /// 16 : OpCode.LDLOC1
    /// 17 : OpCode.PUSH2
    /// 18 : OpCode.PACK
    /// 19 : OpCode.STLOC2
    /// 1A : OpCode.LDARG1
    /// 1B : OpCode.CONVERT 28
    /// 1D : OpCode.LDARG0
    /// 1E : OpCode.CONVERT 28
    /// 20 : OpCode.LDLOC2
    /// 21 : OpCode.UNPACK
    /// 22 : OpCode.DROP
    /// 23 : OpCode.REVERSE3
    /// 24 : OpCode.CAT
    /// 25 : OpCode.SWAP
    /// 26 : OpCode.SYSCALL E63F1884
    /// 2B : OpCode.PUSHT
    /// 2C : OpCode.RET
    /// </remarks>
    [DisplayName("testPutReadOnly")]
    public abstract bool? TestPutReadOnly(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// Script: VwICDGFhcGhBm/ZnzhLAcXnbKHjbKGnBRVOLUEHmPxiECEA=
    /// 00 : OpCode.INITSLOT 0202
    /// 03 : OpCode.PUSHDATA1 6161
    /// 07 : OpCode.STLOC0
    /// 08 : OpCode.LDLOC0
    /// 09 : OpCode.SYSCALL 9BF667CE
    /// 0E : OpCode.PUSH2
    /// 0F : OpCode.PACK
    /// 10 : OpCode.STLOC1
    /// 11 : OpCode.LDARG1
    /// 12 : OpCode.CONVERT 28
    /// 14 : OpCode.LDARG0
    /// 15 : OpCode.CONVERT 28
    /// 17 : OpCode.LDLOC1
    /// 18 : OpCode.UNPACK
    /// 19 : OpCode.DROP
    /// 1A : OpCode.REVERSE3
    /// 1B : OpCode.CAT
    /// 1C : OpCode.SWAP
    /// 1D : OpCode.SYSCALL E63F1884
    /// 22 : OpCode.PUSHT
    /// 23 : OpCode.RET
    /// </remarks>
    [DisplayName("testPutString")]
    public abstract bool? TestPutString(byte[]? key, byte[]? value);

    #endregion
}
