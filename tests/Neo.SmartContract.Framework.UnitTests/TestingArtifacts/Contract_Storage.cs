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
    [DisplayName("serializeTest")]
    public abstract BigInteger? SerializeTest(byte[]? key, BigInteger? value);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0007 : CONVERT
    // 0009 : STLOC0
    // 000A : SYSCALL
    // 000F : STLOC1
    // 0010 : LDLOC0
    // 0011 : LDLOC1
    // 0012 : PUSH2
    // 0013 : PACK
    // 0014 : STLOC2
    // 0015 : PUSH0
    // 0016 : PUSH1
    // 0017 : PACK
    // 0018 : DUP
    // 0019 : CALL
    // 001B : DUP
    // 001C : PUSH0
    // 001D : LDARG1
    // 001E : SETITEM
    // 001F : STLOC3
    // 0020 : LDLOC3
    // 0021 : LDARG0
    // 0022 : LDLOC2
    // 0023 : CALL
    // 0025 : LDARG0
    // 0026 : LDLOC2
    // 0027 : CALL
    // 0029 : STLOC3
    // 002A : LDLOC3
    // 002B : PUSH0
    // 002C : PICKITEM
    // 002D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDeleteByte")]
    public abstract void TestDeleteByte(byte[]? key);
    // 0000 : INITSLOT
    // 0003 : SYSCALL
    // 0008 : PUSHINT8
    // 000A : PUSH1
    // 000B : NEWBUFFER
    // 000C : TUCK
    // 000D : PUSH0
    // 000E : ROT
    // 000F : SETITEM
    // 0010 : SWAP
    // 0011 : PUSH2
    // 0012 : PACK
    // 0013 : STLOC0
    // 0014 : LDARG0
    // 0015 : CONVERT
    // 0017 : LDLOC0
    // 0018 : UNPACK
    // 0019 : DROP
    // 001A : REVERSE3
    // 001B : CAT
    // 001C : SWAP
    // 001D : SYSCALL
    // 0022 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDeleteByteArray")]
    public abstract void TestDeleteByteArray(byte[]? key);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0007 : CONVERT
    // 0009 : STLOC0
    // 000A : LDLOC0
    // 000B : SYSCALL
    // 0010 : PUSH2
    // 0011 : PACK
    // 0012 : STLOC1
    // 0013 : LDARG0
    // 0014 : CONVERT
    // 0016 : LDLOC1
    // 0017 : UNPACK
    // 0018 : DROP
    // 0019 : REVERSE3
    // 001A : CAT
    // 001B : SWAP
    // 001C : SYSCALL
    // 0021 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDeleteString")]
    public abstract void TestDeleteString(byte[]? key);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0007 : STLOC0
    // 0008 : LDLOC0
    // 0009 : SYSCALL
    // 000E : PUSH2
    // 000F : PACK
    // 0010 : STLOC1
    // 0011 : LDARG0
    // 0012 : CONVERT
    // 0014 : LDLOC1
    // 0015 : UNPACK
    // 0016 : DROP
    // 0017 : REVERSE3
    // 0018 : CAT
    // 0019 : SWAP
    // 001A : SYSCALL
    // 001F : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testFind")]
    public abstract byte[]? TestFind();
    // 0000 : INITSLOT
    // 0003 : SYSCALL
    // 0008 : STLOC0
    // 0009 : PUSHDATA1
    // 000C : CONVERT
    // 000E : CONVERT
    // 0010 : PUSHDATA1
    // 0016 : LDLOC0
    // 0017 : SYSCALL
    // 001C : PUSHDATA1
    // 001F : CONVERT
    // 0021 : CONVERT
    // 0023 : PUSHDATA1
    // 0029 : LDLOC0
    // 002A : SYSCALL
    // 002F : PUSH4
    // 0030 : PUSHDATA1
    // 0035 : LDLOC0
    // 0036 : SYSCALL
    // 003B : STLOC1
    // 003C : LDLOC1
    // 003D : SYSCALL
    // 0042 : DROP
    // 0043 : LDLOC1
    // 0044 : SYSCALL
    // 0049 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetByte")]
    public abstract byte[]? TestGetByte(byte[]? key);
    // 0000 : INITSLOT
    // 0003 : SYSCALL
    // 0008 : STLOC0
    // 0009 : LDLOC0
    // 000A : PUSHINT8
    // 000C : PUSH1
    // 000D : NEWBUFFER
    // 000E : TUCK
    // 000F : PUSH0
    // 0010 : ROT
    // 0011 : SETITEM
    // 0012 : SWAP
    // 0013 : PUSH2
    // 0014 : PACK
    // 0015 : STLOC1
    // 0016 : LDARG0
    // 0017 : CONVERT
    // 0019 : LDLOC1
    // 001A : UNPACK
    // 001B : DROP
    // 001C : REVERSE3
    // 001D : CAT
    // 001E : SWAP
    // 001F : SYSCALL
    // 0024 : STLOC2
    // 0025 : LDLOC2
    // 0026 : CONVERT
    // 0028 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetByteArray")]
    public abstract byte[]? TestGetByteArray(byte[]? key);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0007 : CONVERT
    // 0009 : STLOC0
    // 000A : SYSCALL
    // 000F : SYSCALL
    // 0014 : STLOC1
    // 0015 : LDLOC0
    // 0016 : LDLOC1
    // 0017 : PUSH2
    // 0018 : PACK
    // 0019 : STLOC2
    // 001A : LDARG0
    // 001B : CONVERT
    // 001D : LDLOC2
    // 001E : UNPACK
    // 001F : DROP
    // 0020 : REVERSE3
    // 0021 : CAT
    // 0022 : SWAP
    // 0023 : SYSCALL
    // 0028 : STLOC3
    // 0029 : LDLOC3
    // 002A : CONVERT
    // 002C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetString")]
    public abstract byte[]? TestGetString(byte[]? key);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0007 : STLOC0
    // 0008 : SYSCALL
    // 000D : STLOC1
    // 000E : LDLOC0
    // 000F : LDLOC1
    // 0010 : PUSH2
    // 0011 : PACK
    // 0012 : STLOC2
    // 0013 : LDARG0
    // 0014 : CONVERT
    // 0016 : LDLOC2
    // 0017 : UNPACK
    // 0018 : DROP
    // 0019 : REVERSE3
    // 001A : CAT
    // 001B : SWAP
    // 001C : SYSCALL
    // 0021 : STLOC3
    // 0022 : LDLOC3
    // 0023 : CONVERT
    // 0025 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIndexGet")]
    public abstract byte[]? TestIndexGet(byte[]? key);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0007 : STLOC0
    // 0008 : SYSCALL
    // 000D : STLOC1
    // 000E : LDLOC0
    // 000F : LDLOC1
    // 0010 : PUSH2
    // 0011 : PACK
    // 0012 : STLOC2
    // 0013 : LDARG0
    // 0014 : CONVERT
    // 0016 : LDLOC2
    // 0017 : UNPACK
    // 0018 : DROP
    // 0019 : REVERSE3
    // 001A : CAT
    // 001B : SWAP
    // 001C : SYSCALL
    // 0021 : STLOC3
    // 0022 : LDLOC3
    // 0023 : CONVERT
    // 0025 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIndexPut")]
    public abstract bool? TestIndexPut(byte[]? key, byte[]? value);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0007 : STLOC0
    // 0008 : LDLOC0
    // 0009 : SYSCALL
    // 000E : PUSH2
    // 000F : PACK
    // 0010 : STLOC1
    // 0011 : LDARG1
    // 0012 : CONVERT
    // 0014 : DUP
    // 0015 : LDARG0
    // 0016 : CONVERT
    // 0018 : LDLOC1
    // 0019 : UNPACK
    // 001A : DROP
    // 001B : REVERSE3
    // 001C : CAT
    // 001D : SWAP
    // 001E : SYSCALL
    // 0023 : DROP
    // 0024 : PUSHT
    // 0025 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNewGetByteArray")]
    public abstract byte[]? TestNewGetByteArray();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0007 : CONVERT
    // 0009 : STLOC0
    // 000A : SYSCALL
    // 000F : STLOC1
    // 0010 : LDLOC0
    // 0011 : LDLOC1
    // 0012 : PUSH2
    // 0013 : PACK
    // 0014 : STLOC2
    // 0015 : PUSHDATA1
    // 0019 : CONVERT
    // 001B : STLOC3
    // 001C : LDLOC3
    // 001D : PUSHDATA1
    // 0028 : LDLOC2
    // 0029 : UNPACK
    // 002A : DROP
    // 002B : REVERSE3
    // 002C : CAT
    // 002D : SWAP
    // 002E : SYSCALL
    // 0033 : PUSHDATA1
    // 003E : LDLOC2
    // 003F : UNPACK
    // 0040 : DROP
    // 0041 : REVERSE3
    // 0042 : CAT
    // 0043 : SWAP
    // 0044 : SYSCALL
    // 0049 : CONVERT
    // 004B : STLOC4
    // 004C : LDLOC4
    // 004D : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNewGetMethods")]
    public abstract bool? TestNewGetMethods();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0007 : CONVERT
    // 0009 : STLOC0
    // 000A : SYSCALL
    // 000F : STLOC1
    // 0010 : LDLOC0
    // 0011 : LDLOC1
    // 0012 : PUSH2
    // 0013 : PACK
    // 0014 : STLOC2
    // 0015 : PUSHT
    // 0016 : STLOC3
    // 0017 : PUSHINT8
    // 0019 : STLOC4
    // 001A : PUSHDATA1
    // 0027 : STLOC5
    // 0028 : PUSHDATA1
    // 003E : CONVERT
    // 0040 : CONVERT
    // 0042 : DUP
    // 0043 : ISNULL
    // 0044 : JMPIF
    // 0046 : DUP
    // 0047 : SIZE
    // 0048 : PUSHINT8
    // 004A : JMPEQ
    // 004C : THROW
    // 004D : STLOC6
    // 004E : PUSHDATA1
    // 0070 : CONVERT
    // 0072 : CONVERT
    // 0074 : DUP
    // 0075 : ISNULL
    // 0076 : JMPIF
    // 0078 : DUP
    // 0079 : SIZE
    // 007A : PUSHINT8
    // 007C : JMPEQ
    // 007E : THROW
    // 007F : STLOC
    // 0081 : PUSHDATA1
    // 00A4 : CONVERT
    // 00A6 : CONVERT
    // 00A8 : DUP
    // 00A9 : ISNULL
    // 00AA : JMPIF
    // 00AC : DUP
    // 00AD : SIZE
    // 00AE : PUSHINT8
    // 00B0 : JMPEQ
    // 00B2 : THROW
    // 00B3 : STLOC
    // 00B5 : LDLOC3
    // 00B6 : PUSHDATA1
    // 00BC : LDLOC2
    // 00BD : UNPACK
    // 00BE : DROP
    // 00BF : REVERSE3
    // 00C0 : CAT
    // 00C1 : SWAP
    // 00C2 : SYSCALL
    // 00C7 : LDLOC4
    // 00C8 : PUSHDATA1
    // 00CD : LDLOC2
    // 00CE : UNPACK
    // 00CF : DROP
    // 00D0 : REVERSE3
    // 00D1 : CAT
    // 00D2 : SWAP
    // 00D3 : SYSCALL
    // 00D8 : LDLOC5
    // 00D9 : PUSHDATA1
    // 00E1 : LDLOC2
    // 00E2 : UNPACK
    // 00E3 : DROP
    // 00E4 : REVERSE3
    // 00E5 : CAT
    // 00E6 : SWAP
    // 00E7 : SYSCALL
    // 00EC : LDLOC6
    // 00ED : PUSHDATA1
    // 00F6 : LDLOC2
    // 00F7 : UNPACK
    // 00F8 : DROP
    // 00F9 : REVERSE3
    // 00FA : CAT
    // 00FB : SWAP
    // 00FC : SYSCALL
    // 0101 : LDLOC
    // 0103 : PUSHDATA1
    // 010C : LDLOC2
    // 010D : UNPACK
    // 010E : DROP
    // 010F : REVERSE3
    // 0110 : CAT
    // 0111 : SWAP
    // 0112 : SYSCALL
    // 0117 : LDLOC
    // 0119 : PUSHDATA1
    // 0122 : LDLOC2
    // 0123 : UNPACK
    // 0124 : DROP
    // 0125 : REVERSE3
    // 0126 : CAT
    // 0127 : SWAP
    // 0128 : SYSCALL
    // 012D : PUSHDATA1
    // 0133 : LDLOC2
    // 0134 : UNPACK
    // 0135 : DROP
    // 0136 : REVERSE3
    // 0137 : CAT
    // 0138 : SWAP
    // 0139 : SYSCALL
    // 013E : NOT
    // 013F : NOT
    // 0140 : STLOC
    // 0142 : PUSHDATA1
    // 0147 : LDLOC2
    // 0148 : UNPACK
    // 0149 : DROP
    // 014A : REVERSE3
    // 014B : CAT
    // 014C : SWAP
    // 014D : SYSCALL
    // 0152 : CONVERT
    // 0154 : STLOC
    // 0156 : PUSHDATA1
    // 015E : LDLOC2
    // 015F : UNPACK
    // 0160 : DROP
    // 0161 : REVERSE3
    // 0162 : CAT
    // 0163 : SWAP
    // 0164 : SYSCALL
    // 0169 : STLOC
    // 016B : PUSHDATA1
    // 0174 : LDLOC2
    // 0175 : UNPACK
    // 0176 : DROP
    // 0177 : REVERSE3
    // 0178 : CAT
    // 0179 : SWAP
    // 017A : SYSCALL
    // 017F : STLOC
    // 0181 : PUSHDATA1
    // 018A : LDLOC2
    // 018B : UNPACK
    // 018C : DROP
    // 018D : REVERSE3
    // 018E : CAT
    // 018F : SWAP
    // 0190 : SYSCALL
    // 0195 : STLOC
    // 0197 : PUSHDATA1
    // 01A0 : LDLOC2
    // 01A1 : UNPACK
    // 01A2 : DROP
    // 01A3 : REVERSE3
    // 01A4 : CAT
    // 01A5 : SWAP
    // 01A6 : SYSCALL
    // 01AB : STLOC
    // 01AD : LDLOC3
    // 01AE : LDLOC
    // 01B0 : EQUAL
    // 01B1 : JMPIF
    // 01B3 : PUSHF
    // 01B4 : JMP
    // 01B6 : LDLOC4
    // 01B7 : LDLOC
    // 01B9 : EQUAL
    // 01BA : JMPIF
    // 01BC : PUSHF
    // 01BD : JMP
    // 01BF : LDLOC5
    // 01C0 : LDLOC
    // 01C2 : EQUAL
    // 01C3 : JMPIF
    // 01C5 : PUSHF
    // 01C6 : JMP
    // 01C8 : LDLOC6
    // 01C9 : LDLOC
    // 01CB : EQUAL
    // 01CC : JMPIF
    // 01CE : PUSHF
    // 01CF : JMP
    // 01D1 : LDLOC
    // 01D3 : LDLOC
    // 01D5 : EQUAL
    // 01D6 : JMPIF
    // 01D8 : PUSHF
    // 01D9 : RET
    // 01DA : LDLOC
    // 01DC : LDLOC
    // 01DE : EQUAL
    // 01DF : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testOver16Bytes")]
    public abstract byte[]? TestOver16Bytes();
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 001D : CONVERT
    // 001F : STLOC0
    // 0020 : PUSHDATA1
    // 002A : SYSCALL
    // 002F : PUSH2
    // 0030 : PACK
    // 0031 : STLOC1
    // 0032 : LDLOC0
    // 0033 : CONVERT
    // 0035 : PUSHDATA1
    // 0038 : CONVERT
    // 003A : CONVERT
    // 003C : LDLOC1
    // 003D : UNPACK
    // 003E : DROP
    // 003F : REVERSE3
    // 0040 : CAT
    // 0041 : SWAP
    // 0042 : SYSCALL
    // 0047 : PUSHDATA1
    // 004A : CONVERT
    // 004C : CONVERT
    // 004E : LDLOC1
    // 004F : UNPACK
    // 0050 : DROP
    // 0051 : REVERSE3
    // 0052 : CAT
    // 0053 : SWAP
    // 0054 : SYSCALL
    // 0059 : CONVERT
    // 005B : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPutByte")]
    public abstract bool? TestPutByte(byte[]? key, byte[]? value);
    // 0000 : INITSLOT
    // 0003 : SYSCALL
    // 0008 : PUSHINT8
    // 000A : PUSH1
    // 000B : NEWBUFFER
    // 000C : TUCK
    // 000D : PUSH0
    // 000E : ROT
    // 000F : SETITEM
    // 0010 : SWAP
    // 0011 : PUSH2
    // 0012 : PACK
    // 0013 : STLOC0
    // 0014 : LDARG1
    // 0015 : CONVERT
    // 0017 : LDARG0
    // 0018 : CONVERT
    // 001A : LDLOC0
    // 001B : UNPACK
    // 001C : DROP
    // 001D : REVERSE3
    // 001E : CAT
    // 001F : SWAP
    // 0020 : SYSCALL
    // 0025 : PUSHT
    // 0026 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPutByteArray")]
    public abstract bool? TestPutByteArray(byte[]? key, byte[]? value);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0007 : CONVERT
    // 0009 : STLOC0
    // 000A : LDLOC0
    // 000B : SYSCALL
    // 0010 : PUSH2
    // 0011 : PACK
    // 0012 : STLOC1
    // 0013 : LDARG1
    // 0014 : CONVERT
    // 0016 : LDARG0
    // 0017 : CONVERT
    // 0019 : LDLOC1
    // 001A : UNPACK
    // 001B : DROP
    // 001C : REVERSE3
    // 001D : CAT
    // 001E : SWAP
    // 001F : SYSCALL
    // 0024 : PUSHT
    // 0025 : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPutReadOnly")]
    public abstract bool? TestPutReadOnly(byte[]? key, byte[]? value);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0007 : CONVERT
    // 0009 : STLOC0
    // 000A : SYSCALL
    // 000F : SYSCALL
    // 0014 : STLOC1
    // 0015 : LDLOC0
    // 0016 : LDLOC1
    // 0017 : PUSH2
    // 0018 : PACK
    // 0019 : STLOC2
    // 001A : LDARG1
    // 001B : CONVERT
    // 001D : LDARG0
    // 001E : CONVERT
    // 0020 : LDLOC2
    // 0021 : UNPACK
    // 0022 : DROP
    // 0023 : REVERSE3
    // 0024 : CAT
    // 0025 : SWAP
    // 0026 : SYSCALL
    // 002B : PUSHT
    // 002C : RET

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPutString")]
    public abstract bool? TestPutString(byte[]? key, byte[]? value);
    // 0000 : INITSLOT
    // 0003 : PUSHDATA1
    // 0007 : STLOC0
    // 0008 : LDLOC0
    // 0009 : SYSCALL
    // 000E : PUSH2
    // 000F : PACK
    // 0010 : STLOC1
    // 0011 : LDARG1
    // 0012 : CONVERT
    // 0014 : LDARG0
    // 0015 : CONVERT
    // 0017 : LDLOC1
    // 0018 : UNPACK
    // 0019 : DROP
    // 001A : REVERSE3
    // 001B : CAT
    // 001C : SWAP
    // 001D : SYSCALL
    // 0022 : PUSHT
    // 0023 : RET

    #endregion

}
