using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class Contract_Storage : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""Contract_Storage"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPutByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testDeleteByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":43,""safe"":false},{""name"":""testGetByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":78,""safe"":false},{""name"":""testOver16Bytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":121,""safe"":false},{""name"":""testPutString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":215,""safe"":false},{""name"":""testDeleteString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":255,""safe"":false},{""name"":""testGetString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":287,""safe"":false},{""name"":""testPutByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":327,""safe"":false},{""name"":""testDeleteByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":369,""safe"":false},{""name"":""testGetByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":403,""safe"":false},{""name"":""testNewGetMethods"",""parameters"":[],""returntype"":""Boolean"",""offset"":450,""safe"":false},{""name"":""testNewGetByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":945,""safe"":false},{""name"":""testPutReadOnly"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1025,""safe"":false},{""name"":""serializeTest"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1074,""safe"":false},{""name"":""testFind"",""parameters"":[],""returntype"":""ByteArray"",""offset"":1181,""safe"":false},{""name"":""testIndexPut"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1257,""safe"":false},{""name"":""testIndexGet"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":1299,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""0xacce6fd80d44e1796aa0c2c625e9e4e0ce39efc0"",""methods"":[""deserialize"",""serialize""]}],""trusts"":[],""extra"":{}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwLZGVzZXJpYWxpemUBAAEPAAD9OwVXAQJBm/ZnzgAREYhOEFHQUBLAcHnbKHjbKGjBRVOLUEHmPxiEEdsgIgJAVwEBQZv2Z84AERGIThBR0FASwHB42yhowUVTi1BBL1jF7UBXAwFB9rRr4nBoABERiE4QUdBQEsBxeNsoacFFU4tQQZJd6DFyatswIgJAVwIADBg7ADIDIyMjIwIjIwIjIwIjIwIjIwIjIwLbMHAMCHRlc3RfbWFwQZv2Z84SwHFo2ygMAQHbMNsoacFFU4tQQeY/GIQMAQHbMNsoacFFU4tQQZJd6DHbMCICQFcCAgwCYWFwaEGb9mfOEsBxedsoeNsoacFFU4tQQeY/GIQR2yAiAkBXAgEMAmFhcGhBm/ZnzhLAcXjbKGnBRVOLUEEvWMXtQFcEAQwCYWFwQfa0a+JxaGkSwHJ42yhqwUVTi1BBkl3oMXNr2zAiAkBXAgIMAgD/2zBwaEGb9mfOEsBxedsoeNsoacFFU4tQQeY/GIQR2yAiAkBXAgEMAgD/2zBwaEGb9mfOEsBxeNsoacFFU4tQQS9Yxe1AVwQBDAIA/9swcEGb9mfOQXZMv+lxaGkSwHJ42yhqwUVTi1BBkl3oMXNr2zAiAkBXDwAMAgD/2zBwQZv2Z85xaGkSwHIR2yBzAHt0DAtoZWxsbyB3b3JsZHUMFAABAgMEBQYHCAkAAQIDBAUGBwgJ2zDbKErYJAlKygAUKAM6dgwgAAECAwQFBgcICQABAgMEBQYHCAkAAQIDBAUGBwgJAAHbMNsoStgkCUrKACAoAzp3BwwhAAECAwQFBgcICQABAgMEBQYHCAkAAQIDBAUGBwgJAAEC2zDbKErYJAlKygAhKAM6dwhrDARib29sasFFU4tQQeY/GIRsDANpbnRqwUVTi1BB5j8YhG0MBnN0cmluZ2rBRVOLUEHmPxiEbgwHdWludDE2MGrBRVOLUEHmPxiEbwcMB3VpbnQyNTZqwUVTi1BB5j8YhG8IDAdlY3BvaW50asFFU4tQQeY/GIQMBGJvb2xqwUVTi1BBkl3oMaqqdwkMA2ludGrBRVOLUEGSXegx2yF3CgwGc3RyaW5nasFFU4tQQZJd6DF3CwwHdWludDE2MGrBRVOLUEGSXegxdwwMB3VpbnQyNTZqwUVTi1BBkl3oMXcNDAdlY3BvaW50asFFU4tQQZJd6DF3DmtvCZckBxDbICIGbG8KlyQHENsgIgZtbwuXJAcQ2yAiBm5vDJckBxDbICIHbwdvDZckBxDbICIHbwhvDpciAkBXBQAMAgD/2zBwQZv2Z85xaGkSwHIMAgAB2zBzawwJYnl0ZUFycmF5asFFU4tQQeY/GIQMCWJ5dGVBcnJheWrBRVOLUEGSXegx2zB0bCICQFcDAgwCAP/bMHBBm/ZnzkF2TL/pcWhpEsByedsoeNsoasFFU4tQQeY/GIQR2yAiAkBXBAIMAgGq2zBwQZv2Z85xaGkSwHIQEcBKNBlKEHnQc2t4ajQTeGo0I0pzRWsQziICQFcAAUBXAAN6NwAAeXjBRVOLUEHmPxiEQFcCAnl4wUVTi1BBkl3oMXBocWkLlyYFCyIIaDcBACICQFcCAEGb9mfOcAwBAdsw2ygMBGtleTFoQeY/GIQMAQLbMNsoDARrZXkyaEHmPxiEFAwDa2V5aEHfMLiacWlBnAjtnEVpQfNUvx0iAkBXAgIMAmlpcGhBm/ZnzhLAcXnbKEp42yhpwUVTi1BB5j8YhEUR2yAiAkBXBAEMAmlpcEH2tGvicWhpEsByeNsoasFFU4tQQZJd6DFza9swIgJAbXsL5g=="));

    #endregion

    #region Unsafe methods

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("serializeTest")]
    public abstract BigInteger? SerializeTest(byte[]? key, BigInteger? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDeleteByte")]
    public abstract void TestDeleteByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDeleteByteArray")]
    public abstract void TestDeleteByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testDeleteString")]
    public abstract void TestDeleteString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testFind")]
    public abstract byte[]? TestFind();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetByte")]
    public abstract byte[]? TestGetByte(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetByteArray")]
    public abstract byte[]? TestGetByteArray(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testGetString")]
    public abstract byte[]? TestGetString(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIndexGet")]
    public abstract byte[]? TestIndexGet(byte[]? key);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testIndexPut")]
    public abstract bool? TestIndexPut(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNewGetByteArray")]
    public abstract byte[]? TestNewGetByteArray();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testNewGetMethods")]
    public abstract bool? TestNewGetMethods();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testOver16Bytes")]
    public abstract byte[]? TestOver16Bytes();

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPutByte")]
    public abstract bool? TestPutByte(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPutByteArray")]
    public abstract bool? TestPutByteArray(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPutReadOnly")]
    public abstract bool? TestPutReadOnly(byte[]? key, byte[]? value);

    /// <summary>
    /// Unsafe method
    /// </summary>
    [DisplayName("testPutString")]
    public abstract bool? TestPutString(byte[]? key, byte[]? value);

    #endregion

    #region Constructor for internal use only

    protected Contract_Storage(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
