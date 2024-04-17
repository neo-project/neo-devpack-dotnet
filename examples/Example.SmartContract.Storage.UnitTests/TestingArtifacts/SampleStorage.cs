using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleStorage : Neo.SmartContract.Testing.SmartContract
{
    #region Compiled data

    public static readonly Neo.SmartContract.Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleStorage"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPutByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testDeleteByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":42,""safe"":false},{""name"":""testGetByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":76,""safe"":false},{""name"":""testOver16Bytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":116,""safe"":false},{""name"":""testPutString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":210,""safe"":false},{""name"":""testDeleteString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":250,""safe"":false},{""name"":""testGetString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":282,""safe"":false},{""name"":""testPutByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":320,""safe"":false},{""name"":""testDeleteByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":362,""safe"":false},{""name"":""testGetByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":396,""safe"":false},{""name"":""testNewGetMethods"",""parameters"":[],""returntype"":""Boolean"",""offset"":436,""safe"":false},{""name"":""testNewGetByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":926,""safe"":false},{""name"":""testPutReadOnly"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1004,""safe"":false},{""name"":""serializeTest"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1046,""safe"":false},{""name"":""testFind"",""parameters"":[],""returntype"":""ByteArray"",""offset"":1151,""safe"":false},{""name"":""testIndexPut"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1241,""safe"":false},{""name"":""testIndexGet"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":1283,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Description"":""A sample contract to demonstrate how to use storage"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/""}}");

    public static readonly Neo.SmartContract.NefFile Nef = Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwLZGVzZXJpYWxpemUBAAEPAAD9JwVXAQIAERGIThBR0EGb9mfOEsBwedsoeNsoaMFFU4tQQeY/GIQR2yAiAkBXAQEAERGIThBR0EGb9mfOEsBweNsoaMFFU4tQQS9Yxe1AVwIBABERiE4QUdBBm/ZnzhLAcHjbKGjBRVOLUEGSXegxcWnbMCICQFcCAAwYOwAyAyMjIyMCIyMCIyMCIyMCIyMCIyMC2zBwDAh0ZXN0X21hcEGb9mfOEsBxaNsoDAEB2zDbKGnBRVOLUEHmPxiEDAEB2zDbKGnBRVOLUEGSXegx2zAiAkBXAgIMAmFhcGhBm/ZnzhLAcXnbKHjbKGnBRVOLUEHmPxiEEdsgIgJAVwIBDAJhYXBoQZv2Z84SwHF42yhpwUVTi1BBL1jF7UBXAwEMAmFhcGhBm/ZnzhLAcXjbKGnBRVOLUEGSXegxcmrbMCICQFcCAgwCAP/bMHBoQZv2Z84SwHF52yh42yhpwUVTi1BB5j8YhBHbICICQFcCAQwCAP/bMHBoQZv2Z84SwHF42yhpwUVTi1BBL1jF7UBXAwEMAgD/2zBwaEGb9mfOEsBxeNsoacFFU4tQQZJd6DFyatswIgJAVw4ADAIA/9swcGhBm/ZnzhLAcRHbIHIAe3MMC2hlbGxvIHdvcmxkdAwUAAECAwQFBgcICQABAgMEBQYHCAnbMNsoStgkCUrKABQoAzp1DCAAAQIDBAUGBwgJAAECAwQFBgcICQABAgMEBQYHCAkAAdsw2yhK2CQJSsoAICgDOnYMIQABAgMEBQYHCAkAAQIDBAUGBwgJAAECAwQFBgcICQABAtsw2yhK2CQJSsoAISgDOncHagwEYm9vbGnBRVOLUEHmPxiEawwDaW50acFFU4tQQeY/GIRsDAZzdHJpbmdpwUVTi1BB5j8YhG0MB3VpbnQxNjBpwUVTi1BB5j8YhG4MB3VpbnQyNTZpwUVTi1BB5j8YhG8HDAdlY3BvaW50acFFU4tQQeY/GIQMBGJvb2xpwUVTi1BBkl3oMaqqdwgMA2ludGnBRVOLUEGSXegx2yF3CQwGc3RyaW5nacFFU4tQQZJd6DF3CgwHdWludDE2MGnBRVOLUEGSXegxdwsMB3VpbnQyNTZpwUVTi1BBkl3oMXcMDAdlY3BvaW50acFFU4tQQZJd6DF3DWpvCJckBxDbICIGa28JlyQHENsgIgZsbwqXJAcQ2yAiBm1vC5ckBxDbICIGbm8MlyQHENsgIgdvB28NlyICQFcEAAwCAP/bMHBoQZv2Z84SwHEMAgAB2zByagwJYnl0ZUFycmF5acFFU4tQQeY/GIQMCWJ5dGVBcnJheWnBRVOLUEGSXegx2zBzayICQFcCAgwCAP/bMHBoQZv2Z84SwHF52yh42yhpwUVTi1BB5j8YhBHbICICQFcDAgwCAarbMHBoQZv2Z84SwHEQEcBKNBlKEHnQcmp4aTQTeGk0I0pyRWoQziICQFcAAUBXAAN6NwAAeXjBRVOLUEHmPxiEQFcCAnl4wUVTi1BBkl3oMXBocWkLlyYFCyIIaDcBACICQFcBAAwBAdsw2ygMBGtleTE0KgwBAtsw2ygMBGtleTI0GxQMA2tleTQjcGhBnAjtnEVoQfNUvx0iAkBXAAJ5eEGb9mfOQeY/GIRAVwACeXhB9rRr4kHfMLiaQFcCAgwCaWlwaEGb9mfOEsBxedsoSnjbKGnBRVOLUEHmPxiERRHbICICQFcCAQwCaWlwaEGb9mfOEsBxeNsoacFFU4tQQZJd6DHbMCICQJPYPlo="));

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

    protected SampleStorage(Neo.SmartContract.Testing.SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
