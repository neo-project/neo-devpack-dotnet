using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Storage(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Storage"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPutByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testDeleteByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":39,""safe"":false},{""name"":""testGetByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":74,""safe"":false},{""name"":""testOver16Bytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":115,""safe"":false},{""name"":""testPutString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":207,""safe"":false},{""name"":""testDeleteString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":243,""safe"":false},{""name"":""testGetString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":275,""safe"":false},{""name"":""testPutByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":313,""safe"":false},{""name"":""testDeleteByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":351,""safe"":false},{""name"":""testGetByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":385,""safe"":false},{""name"":""testNewGetMethods"",""parameters"":[],""returntype"":""Boolean"",""offset"":430,""safe"":false},{""name"":""testNewGetByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":910,""safe"":false},{""name"":""testPutReadOnly"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":988,""safe"":false},{""name"":""serializeTest"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1033,""safe"":false},{""name"":""testFind"",""parameters"":[],""returntype"":""ByteArray"",""offset"":1133,""safe"":false},{""name"":""testIndexPut"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1207,""safe"":false},{""name"":""testIndexGet"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":1245,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]}],""trusts"":[],""extra"":{""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwLZGVzZXJpYWxpemUBAAEPAAD9AwVXAQJBm/ZnzgAREYhOEFHQUBLAcHnbKHjbKGjBRVOLUEHmPxiECEBXAQFBm/ZnzgAREYhOEFHQUBLAcHjbKGjBRVOLUEEvWMXtQFcDAUH2tGvicGgAERGIThBR0FASwHF42yhpwUVTi1BBkl3oMXJq2zBAVwIADBg7ADIDIyMjIwIjIwIjIwIjIwIjIwIjIwLbMHAMCHRlc3RfbWFwQZv2Z84SwHFo2ygMAQHbMNsoacFFU4tQQeY/GIQMAQHbMNsoacFFU4tQQZJd6DHbMEBXAgIMAmFhcGhBm/ZnzhLAcXnbKHjbKGnBRVOLUEHmPxiECEBXAgEMAmFhcGhBm/ZnzhLAcXjbKGnBRVOLUEEvWMXtQFcEAQwCYWFwQfa0a+JxaGkSwHJ42yhqwUVTi1BBkl3oMXNr2zBAVwICDAIA/9swcGhBm/ZnzhLAcXnbKHjbKGnBRVOLUEHmPxiECEBXAgEMAgD/2zBwaEGb9mfOEsBxeNsoacFFU4tQQS9Yxe1AVwQBDAIA/9swcEGb9mfOQXZMv+lxaGkSwHJ42yhqwUVTi1BBkl3oMXNr2zBAVw8ADAIA/9swcEGb9mfOcWhpEsByCHMAe3QMC2hlbGxvIHdvcmxkdQwUAAECAwQFBgcICQABAgMEBQYHCAnbMNsoStgkCUrKABQoAzp2DCAAAQIDBAUGBwgJAAECAwQFBgcICQABAgMEBQYHCAkAAdsw2yhK2CQJSsoAICgDOncHDCEAAQIDBAUGBwgJAAECAwQFBgcICQABAgMEBQYHCAkAAQLbMNsoStgkCUrKACEoAzp3CGsMBGJvb2xqwUVTi1BB5j8YhGwMA2ludGrBRVOLUEHmPxiEbQwGc3RyaW5nasFFU4tQQeY/GIRuDAd1aW50MTYwasFFU4tQQeY/GIRvBwwHdWludDI1NmrBRVOLUEHmPxiEbwgMB2VjcG9pbnRqwUVTi1BB5j8YhAwEYm9vbGrBRVOLUEGSXegxqqp3CQwDaW50asFFU4tQQZJd6DHbIXcKDAZzdHJpbmdqwUVTi1BBkl3oMXcLDAd1aW50MTYwasFFU4tQQZJd6DF3DAwHdWludDI1NmrBRVOLUEGSXegxdw0MB2VjcG9pbnRqwUVTi1BBkl3oMXcOa28JlyQFCSIGbG8KlyQFCSIGbW8LlyQFCSIGbm8MlyQFCSIHbwdvDZckBAlAbwhvDpdAVwUADAIA/9swcEGb9mfOcWhpEsByDAIAAdswc2sMCWJ5dGVBcnJheWrBRVOLUEHmPxiEDAlieXRlQXJyYXlqwUVTi1BBkl3oMdswdGxAVwMCDAIA/9swcEGb9mfOQXZMv+lxaGkSwHJ52yh42yhqwUVTi1BB5j8YhAhAVwQCDAIBqtswcEGb9mfOcWhpEsByEBHASjQVShB50HNreGo0D3hqNB9zaxDOQFcAAUBXAAN6NwAAeXjBRVOLUEHmPxiEQFcCAnl4wUVTi1BBkl3oMXBocWkLlyYEC0BoNwEAQFcCAEGb9mfOcAwBAdsw2ygMBGtleTFoQeY/GIQMAQLbMNsoDARrZXkyaEHmPxiEFAwDa2V5aEHfMLiacWlBnAjtnEVpQfNUvx1AVwICDAJpaXBoQZv2Z84SwHF52yhKeNsoacFFU4tQQeY/GIRFCEBXBAEMAmlpcEH2tGvicWhpEsByeNsoasFFU4tQQZJd6DFza9swQOYOkac="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0402
    /// 0003 : OpCode.PUSHDATA1 01AA
    /// 0007 : OpCode.CONVERT 30
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.SYSCALL 9BF667CE
    /// 000F : OpCode.STLOC1
    /// 0010 : OpCode.LDLOC0
    /// 0011 : OpCode.LDLOC1
    /// 0012 : OpCode.PUSH2
    /// 0013 : OpCode.PACK
    /// 0014 : OpCode.STLOC2
    /// 0015 : OpCode.PUSH0
    /// 0016 : OpCode.PUSH1
    /// 0017 : OpCode.PACK
    /// 0018 : OpCode.DUP
    /// 0019 : OpCode.CALL 15
    /// 001B : OpCode.DUP
    /// 001C : OpCode.PUSH0
    /// 001D : OpCode.LDARG1
    /// 001E : OpCode.SETITEM
    /// 001F : OpCode.STLOC3
    /// 0020 : OpCode.LDLOC3
    /// 0021 : OpCode.LDARG0
    /// 0022 : OpCode.LDLOC2
    /// 0023 : OpCode.CALL 0F
    /// 0025 : OpCode.LDARG0
    /// 0026 : OpCode.LDLOC2
    /// 0027 : OpCode.CALL 1F
    /// 0029 : OpCode.STLOC3
    /// 002A : OpCode.LDLOC3
    /// 002B : OpCode.PUSH0
    /// 002C : OpCode.PICKITEM
    /// 002D : OpCode.RET
    /// </remarks>
    [DisplayName("serializeTest")]
    public abstract BigInteger? SerializeTest(byte[]? key, BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0101
    /// 0003 : OpCode.SYSCALL 9BF667CE
    /// 0008 : OpCode.PUSHINT8 11
    /// 000A : OpCode.PUSH1
    /// 000B : OpCode.NEWBUFFER
    /// 000C : OpCode.TUCK
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.ROT
    /// 000F : OpCode.SETITEM
    /// 0010 : OpCode.SWAP
    /// 0011 : OpCode.PUSH2
    /// 0012 : OpCode.PACK
    /// 0013 : OpCode.STLOC0
    /// 0014 : OpCode.LDARG0
    /// 0015 : OpCode.CONVERT 28
    /// 0017 : OpCode.LDLOC0
    /// 0018 : OpCode.UNPACK
    /// 0019 : OpCode.DROP
    /// 001A : OpCode.REVERSE3
    /// 001B : OpCode.CAT
    /// 001C : OpCode.SWAP
    /// 001D : OpCode.SYSCALL 2F58C5ED
    /// 0022 : OpCode.RET
    /// </remarks>
    [DisplayName("testDeleteByte")]
    public abstract void TestDeleteByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.PUSHDATA1 00FF
    /// 0007 : OpCode.CONVERT 30
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.LDLOC0
    /// 000B : OpCode.SYSCALL 9BF667CE
    /// 0010 : OpCode.PUSH2
    /// 0011 : OpCode.PACK
    /// 0012 : OpCode.STLOC1
    /// 0013 : OpCode.LDARG0
    /// 0014 : OpCode.CONVERT 28
    /// 0016 : OpCode.LDLOC1
    /// 0017 : OpCode.UNPACK
    /// 0018 : OpCode.DROP
    /// 0019 : OpCode.REVERSE3
    /// 001A : OpCode.CAT
    /// 001B : OpCode.SWAP
    /// 001C : OpCode.SYSCALL 2F58C5ED
    /// 0021 : OpCode.RET
    /// </remarks>
    [DisplayName("testDeleteByteArray")]
    public abstract void TestDeleteByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0201
    /// 0003 : OpCode.PUSHDATA1 6161
    /// 0007 : OpCode.STLOC0
    /// 0008 : OpCode.LDLOC0
    /// 0009 : OpCode.SYSCALL 9BF667CE
    /// 000E : OpCode.PUSH2
    /// 000F : OpCode.PACK
    /// 0010 : OpCode.STLOC1
    /// 0011 : OpCode.LDARG0
    /// 0012 : OpCode.CONVERT 28
    /// 0014 : OpCode.LDLOC1
    /// 0015 : OpCode.UNPACK
    /// 0016 : OpCode.DROP
    /// 0017 : OpCode.REVERSE3
    /// 0018 : OpCode.CAT
    /// 0019 : OpCode.SWAP
    /// 001A : OpCode.SYSCALL 2F58C5ED
    /// 001F : OpCode.RET
    /// </remarks>
    [DisplayName("testDeleteString")]
    public abstract void TestDeleteString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.SYSCALL 9BF667CE
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.PUSHDATA1 01
    /// 000C : OpCode.CONVERT 30
    /// 000E : OpCode.CONVERT 28
    /// 0010 : OpCode.PUSHDATA1 6B657931
    /// 0016 : OpCode.LDLOC0
    /// 0017 : OpCode.SYSCALL E63F1884
    /// 001C : OpCode.PUSHDATA1 02
    /// 001F : OpCode.CONVERT 30
    /// 0021 : OpCode.CONVERT 28
    /// 0023 : OpCode.PUSHDATA1 6B657932
    /// 0029 : OpCode.LDLOC0
    /// 002A : OpCode.SYSCALL E63F1884
    /// 002F : OpCode.PUSH4
    /// 0030 : OpCode.PUSHDATA1 6B6579
    /// 0035 : OpCode.LDLOC0
    /// 0036 : OpCode.SYSCALL DF30B89A
    /// 003B : OpCode.STLOC1
    /// 003C : OpCode.LDLOC1
    /// 003D : OpCode.SYSCALL 9C08ED9C
    /// 0042 : OpCode.DROP
    /// 0043 : OpCode.LDLOC1
    /// 0044 : OpCode.SYSCALL F354BF1D
    /// 0049 : OpCode.RET
    /// </remarks>
    [DisplayName("testFind")]
    public abstract byte[]? TestFind();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0301
    /// 0003 : OpCode.SYSCALL F6B46BE2
    /// 0008 : OpCode.STLOC0
    /// 0009 : OpCode.LDLOC0
    /// 000A : OpCode.PUSHINT8 11
    /// 000C : OpCode.PUSH1
    /// 000D : OpCode.NEWBUFFER
    /// 000E : OpCode.TUCK
    /// 000F : OpCode.PUSH0
    /// 0010 : OpCode.ROT
    /// 0011 : OpCode.SETITEM
    /// 0012 : OpCode.SWAP
    /// 0013 : OpCode.PUSH2
    /// 0014 : OpCode.PACK
    /// 0015 : OpCode.STLOC1
    /// 0016 : OpCode.LDARG0
    /// 0017 : OpCode.CONVERT 28
    /// 0019 : OpCode.LDLOC1
    /// 001A : OpCode.UNPACK
    /// 001B : OpCode.DROP
    /// 001C : OpCode.REVERSE3
    /// 001D : OpCode.CAT
    /// 001E : OpCode.SWAP
    /// 001F : OpCode.SYSCALL 925DE831
    /// 0024 : OpCode.STLOC2
    /// 0025 : OpCode.LDLOC2
    /// 0026 : OpCode.CONVERT 30
    /// 0028 : OpCode.RET
    /// </remarks>
    [DisplayName("testGetByte")]
    public abstract byte[]? TestGetByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0401
    /// 0003 : OpCode.PUSHDATA1 00FF
    /// 0007 : OpCode.CONVERT 30
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.SYSCALL 9BF667CE
    /// 000F : OpCode.SYSCALL 764CBFE9
    /// 0014 : OpCode.STLOC1
    /// 0015 : OpCode.LDLOC0
    /// 0016 : OpCode.LDLOC1
    /// 0017 : OpCode.PUSH2
    /// 0018 : OpCode.PACK
    /// 0019 : OpCode.STLOC2
    /// 001A : OpCode.LDARG0
    /// 001B : OpCode.CONVERT 28
    /// 001D : OpCode.LDLOC2
    /// 001E : OpCode.UNPACK
    /// 001F : OpCode.DROP
    /// 0020 : OpCode.REVERSE3
    /// 0021 : OpCode.CAT
    /// 0022 : OpCode.SWAP
    /// 0023 : OpCode.SYSCALL 925DE831
    /// 0028 : OpCode.STLOC3
    /// 0029 : OpCode.LDLOC3
    /// 002A : OpCode.CONVERT 30
    /// 002C : OpCode.RET
    /// </remarks>
    [DisplayName("testGetByteArray")]
    public abstract byte[]? TestGetByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0401
    /// 0003 : OpCode.PUSHDATA1 6161
    /// 0007 : OpCode.STLOC0
    /// 0008 : OpCode.SYSCALL F6B46BE2
    /// 000D : OpCode.STLOC1
    /// 000E : OpCode.LDLOC0
    /// 000F : OpCode.LDLOC1
    /// 0010 : OpCode.PUSH2
    /// 0011 : OpCode.PACK
    /// 0012 : OpCode.STLOC2
    /// 0013 : OpCode.LDARG0
    /// 0014 : OpCode.CONVERT 28
    /// 0016 : OpCode.LDLOC2
    /// 0017 : OpCode.UNPACK
    /// 0018 : OpCode.DROP
    /// 0019 : OpCode.REVERSE3
    /// 001A : OpCode.CAT
    /// 001B : OpCode.SWAP
    /// 001C : OpCode.SYSCALL 925DE831
    /// 0021 : OpCode.STLOC3
    /// 0022 : OpCode.LDLOC3
    /// 0023 : OpCode.CONVERT 30
    /// 0025 : OpCode.RET
    /// </remarks>
    [DisplayName("testGetString")]
    public abstract byte[]? TestGetString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0401
    /// 0003 : OpCode.PUSHDATA1 6969
    /// 0007 : OpCode.STLOC0
    /// 0008 : OpCode.SYSCALL F6B46BE2
    /// 000D : OpCode.STLOC1
    /// 000E : OpCode.LDLOC0
    /// 000F : OpCode.LDLOC1
    /// 0010 : OpCode.PUSH2
    /// 0011 : OpCode.PACK
    /// 0012 : OpCode.STLOC2
    /// 0013 : OpCode.LDARG0
    /// 0014 : OpCode.CONVERT 28
    /// 0016 : OpCode.LDLOC2
    /// 0017 : OpCode.UNPACK
    /// 0018 : OpCode.DROP
    /// 0019 : OpCode.REVERSE3
    /// 001A : OpCode.CAT
    /// 001B : OpCode.SWAP
    /// 001C : OpCode.SYSCALL 925DE831
    /// 0021 : OpCode.STLOC3
    /// 0022 : OpCode.LDLOC3
    /// 0023 : OpCode.CONVERT 30
    /// 0025 : OpCode.RET
    /// </remarks>
    [DisplayName("testIndexGet")]
    public abstract byte[]? TestIndexGet(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0202
    /// 0003 : OpCode.PUSHDATA1 6969
    /// 0007 : OpCode.STLOC0
    /// 0008 : OpCode.LDLOC0
    /// 0009 : OpCode.SYSCALL 9BF667CE
    /// 000E : OpCode.PUSH2
    /// 000F : OpCode.PACK
    /// 0010 : OpCode.STLOC1
    /// 0011 : OpCode.LDARG1
    /// 0012 : OpCode.CONVERT 28
    /// 0014 : OpCode.DUP
    /// 0015 : OpCode.LDARG0
    /// 0016 : OpCode.CONVERT 28
    /// 0018 : OpCode.LDLOC1
    /// 0019 : OpCode.UNPACK
    /// 001A : OpCode.DROP
    /// 001B : OpCode.REVERSE3
    /// 001C : OpCode.CAT
    /// 001D : OpCode.SWAP
    /// 001E : OpCode.SYSCALL E63F1884
    /// 0023 : OpCode.DROP
    /// 0024 : OpCode.PUSHT
    /// 0025 : OpCode.RET
    /// </remarks>
    [DisplayName("testIndexPut")]
    public abstract bool? TestIndexPut(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0500
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
    /// 0015 : OpCode.PUSHDATA1 0001
    /// 0019 : OpCode.CONVERT 30
    /// 001B : OpCode.STLOC3
    /// 001C : OpCode.LDLOC3
    /// 001D : OpCode.PUSHDATA1 627974654172726179
    /// 0028 : OpCode.LDLOC2
    /// 0029 : OpCode.UNPACK
    /// 002A : OpCode.DROP
    /// 002B : OpCode.REVERSE3
    /// 002C : OpCode.CAT
    /// 002D : OpCode.SWAP
    /// 002E : OpCode.SYSCALL E63F1884
    /// 0033 : OpCode.PUSHDATA1 627974654172726179
    /// 003E : OpCode.LDLOC2
    /// 003F : OpCode.UNPACK
    /// 0040 : OpCode.DROP
    /// 0041 : OpCode.REVERSE3
    /// 0042 : OpCode.CAT
    /// 0043 : OpCode.SWAP
    /// 0044 : OpCode.SYSCALL 925DE831
    /// 0049 : OpCode.CONVERT 30
    /// 004B : OpCode.STLOC4
    /// 004C : OpCode.LDLOC4
    /// 004D : OpCode.RET
    /// </remarks>
    [DisplayName("testNewGetByteArray")]
    public abstract byte[]? TestNewGetByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
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
    /// 0000 : OpCode.INITSLOT 0200
    /// 0003 : OpCode.PUSHDATA1 3B0032032323232302232302232302232302232302232302
    /// 001D : OpCode.CONVERT 30
    /// 001F : OpCode.STLOC0
    /// 0020 : OpCode.PUSHDATA1 746573745F6D6170
    /// 002A : OpCode.SYSCALL 9BF667CE
    /// 002F : OpCode.PUSH2
    /// 0030 : OpCode.PACK
    /// 0031 : OpCode.STLOC1
    /// 0032 : OpCode.LDLOC0
    /// 0033 : OpCode.CONVERT 28
    /// 0035 : OpCode.PUSHDATA1 01
    /// 0038 : OpCode.CONVERT 30
    /// 003A : OpCode.CONVERT 28
    /// 003C : OpCode.LDLOC1
    /// 003D : OpCode.UNPACK
    /// 003E : OpCode.DROP
    /// 003F : OpCode.REVERSE3
    /// 0040 : OpCode.CAT
    /// 0041 : OpCode.SWAP
    /// 0042 : OpCode.SYSCALL E63F1884
    /// 0047 : OpCode.PUSHDATA1 01
    /// 004A : OpCode.CONVERT 30
    /// 004C : OpCode.CONVERT 28
    /// 004E : OpCode.LDLOC1
    /// 004F : OpCode.UNPACK
    /// 0050 : OpCode.DROP
    /// 0051 : OpCode.REVERSE3
    /// 0052 : OpCode.CAT
    /// 0053 : OpCode.SWAP
    /// 0054 : OpCode.SYSCALL 925DE831
    /// 0059 : OpCode.CONVERT 30
    /// 005B : OpCode.RET
    /// </remarks>
    [DisplayName("testOver16Bytes")]
    public abstract byte[]? TestOver16Bytes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0102
    /// 0003 : OpCode.SYSCALL 9BF667CE
    /// 0008 : OpCode.PUSHINT8 11
    /// 000A : OpCode.PUSH1
    /// 000B : OpCode.NEWBUFFER
    /// 000C : OpCode.TUCK
    /// 000D : OpCode.PUSH0
    /// 000E : OpCode.ROT
    /// 000F : OpCode.SETITEM
    /// 0010 : OpCode.SWAP
    /// 0011 : OpCode.PUSH2
    /// 0012 : OpCode.PACK
    /// 0013 : OpCode.STLOC0
    /// 0014 : OpCode.LDARG1
    /// 0015 : OpCode.CONVERT 28
    /// 0017 : OpCode.LDARG0
    /// 0018 : OpCode.CONVERT 28
    /// 001A : OpCode.LDLOC0
    /// 001B : OpCode.UNPACK
    /// 001C : OpCode.DROP
    /// 001D : OpCode.REVERSE3
    /// 001E : OpCode.CAT
    /// 001F : OpCode.SWAP
    /// 0020 : OpCode.SYSCALL E63F1884
    /// 0025 : OpCode.PUSHT
    /// 0026 : OpCode.RET
    /// </remarks>
    [DisplayName("testPutByte")]
    public abstract bool? TestPutByte(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0202
    /// 0003 : OpCode.PUSHDATA1 00FF
    /// 0007 : OpCode.CONVERT 30
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.LDLOC0
    /// 000B : OpCode.SYSCALL 9BF667CE
    /// 0010 : OpCode.PUSH2
    /// 0011 : OpCode.PACK
    /// 0012 : OpCode.STLOC1
    /// 0013 : OpCode.LDARG1
    /// 0014 : OpCode.CONVERT 28
    /// 0016 : OpCode.LDARG0
    /// 0017 : OpCode.CONVERT 28
    /// 0019 : OpCode.LDLOC1
    /// 001A : OpCode.UNPACK
    /// 001B : OpCode.DROP
    /// 001C : OpCode.REVERSE3
    /// 001D : OpCode.CAT
    /// 001E : OpCode.SWAP
    /// 001F : OpCode.SYSCALL E63F1884
    /// 0024 : OpCode.PUSHT
    /// 0025 : OpCode.RET
    /// </remarks>
    [DisplayName("testPutByteArray")]
    public abstract bool? TestPutByteArray(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0302
    /// 0003 : OpCode.PUSHDATA1 00FF
    /// 0007 : OpCode.CONVERT 30
    /// 0009 : OpCode.STLOC0
    /// 000A : OpCode.SYSCALL 9BF667CE
    /// 000F : OpCode.SYSCALL 764CBFE9
    /// 0014 : OpCode.STLOC1
    /// 0015 : OpCode.LDLOC0
    /// 0016 : OpCode.LDLOC1
    /// 0017 : OpCode.PUSH2
    /// 0018 : OpCode.PACK
    /// 0019 : OpCode.STLOC2
    /// 001A : OpCode.LDARG1
    /// 001B : OpCode.CONVERT 28
    /// 001D : OpCode.LDARG0
    /// 001E : OpCode.CONVERT 28
    /// 0020 : OpCode.LDLOC2
    /// 0021 : OpCode.UNPACK
    /// 0022 : OpCode.DROP
    /// 0023 : OpCode.REVERSE3
    /// 0024 : OpCode.CAT
    /// 0025 : OpCode.SWAP
    /// 0026 : OpCode.SYSCALL E63F1884
    /// 002B : OpCode.PUSHT
    /// 002C : OpCode.RET
    /// </remarks>
    [DisplayName("testPutReadOnly")]
    public abstract bool? TestPutReadOnly(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    /// <remarks>
    /// 0000 : OpCode.INITSLOT 0202
    /// 0003 : OpCode.PUSHDATA1 6161
    /// 0007 : OpCode.STLOC0
    /// 0008 : OpCode.LDLOC0
    /// 0009 : OpCode.SYSCALL 9BF667CE
    /// 000E : OpCode.PUSH2
    /// 000F : OpCode.PACK
    /// 0010 : OpCode.STLOC1
    /// 0011 : OpCode.LDARG1
    /// 0012 : OpCode.CONVERT 28
    /// 0014 : OpCode.LDARG0
    /// 0015 : OpCode.CONVERT 28
    /// 0017 : OpCode.LDLOC1
    /// 0018 : OpCode.UNPACK
    /// 0019 : OpCode.DROP
    /// 001A : OpCode.REVERSE3
    /// 001B : OpCode.CAT
    /// 001C : OpCode.SWAP
    /// 001D : OpCode.SYSCALL E63F1884
    /// 0022 : OpCode.PUSHT
    /// 0023 : OpCode.RET
    /// </remarks>
    [DisplayName("testPutString")]
    public abstract bool? TestPutString(byte[]? key, byte[]? value);

    #endregion

}
