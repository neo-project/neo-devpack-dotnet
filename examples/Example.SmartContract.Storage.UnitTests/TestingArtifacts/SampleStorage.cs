using Neo.Cryptography.ECC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class SampleStorage(Neo.SmartContract.Testing.SmartContractInitialize initialize) : Neo.SmartContract.Testing.SmartContract(initialize), IContractInfo
{
    #region Compiled data

    public static Neo.SmartContract.Manifest.ContractManifest Manifest => Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""SampleStorage"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""testPutByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":0,""safe"":false},{""name"":""testDeleteByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":38,""safe"":false},{""name"":""testGetByte"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":72,""safe"":false},{""name"":""testOver16Bytes"",""parameters"":[],""returntype"":""ByteArray"",""offset"":110,""safe"":false},{""name"":""testPutString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":202,""safe"":false},{""name"":""testDeleteString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":238,""safe"":false},{""name"":""testGetString"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":270,""safe"":false},{""name"":""testPutByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":306,""safe"":false},{""name"":""testDeleteByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""Void"",""offset"":344,""safe"":false},{""name"":""testGetByteArray"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":378,""safe"":false},{""name"":""testNewGetMethods"",""parameters"":[],""returntype"":""Boolean"",""offset"":416,""safe"":false},{""name"":""testNewGetByteArray"",""parameters"":[],""returntype"":""ByteArray"",""offset"":891,""safe"":false},{""name"":""testPutReadOnly"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":967,""safe"":false},{""name"":""serializeTest"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":1005,""safe"":false},{""name"":""testFind"",""parameters"":[],""returntype"":""ByteArray"",""offset"":1091,""safe"":false},{""name"":""testIndexPut"",""parameters"":[{""name"":""key"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Boolean"",""offset"":1179,""safe"":false},{""name"":""testIndexGet"",""parameters"":[{""name"":""key"",""type"":""ByteArray""}],""returntype"":""ByteArray"",""offset"":1217,""safe"":false}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":{""Author"":""code-dev"",""Description"":""A sample contract to demonstrate how to use storage"",""Version"":""0.0.1"",""Sourcecode"":""https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/"",""nef"":{""optimization"":""All""}}}");

    /// <summary>
    /// Optimization: "All"
    /// </summary>
    public static Neo.SmartContract.NefFile Nef => Neo.IO.Helper.AsSerializable<Neo.SmartContract.NefFile>(Convert.FromBase64String(@"TkVGM1Rlc3RpbmdFbmdpbmUAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAALA7znO4OTpJcbCoGp54UQN2G/OrAlzZXJpYWxpemUBAAEPwO85zuDk6SXGwqBqeeFEDdhvzqwLZGVzZXJpYWxpemUBAAEPAAD94wRXAQIAERGIThBR0EGb9mfOEsBwedsoeNsoaMFFU4tQQeY/GIQIQFcBAQAREYhOEFHQQZv2Z84SwHB42yhowUVTi1BBL1jF7UBXAgEAERGIThBR0EGb9mfOEsBweNsoaMFFU4tQQZJd6DFxadswQFcCAAwYOwAyAyMjIyMCIyMCIyMCIyMCIyMCIyMC2zBwDAh0ZXN0X21hcEGb9mfOEsBxaNsoDAEB2zDbKGnBRVOLUEHmPxiEDAEB2zDbKGnBRVOLUEGSXegx2zBAVwICDAJhYXBoQZv2Z84SwHF52yh42yhpwUVTi1BB5j8YhAhAVwIBDAJhYXBoQZv2Z84SwHF42yhpwUVTi1BBL1jF7UBXAwEMAmFhcGhBm/ZnzhLAcXjbKGnBRVOLUEGSXegxcmrbMEBXAgIMAgD/2zBwaEGb9mfOEsBxedsoeNsoacFFU4tQQeY/GIQIQFcCAQwCAP/bMHBoQZv2Z84SwHF42yhpwUVTi1BBL1jF7UBXAwEMAgD/2zBwaEGb9mfOEsBxeNsoacFFU4tQQZJd6DFyatswQFcOAAwCAP/bMHBoQZv2Z84SwHEIcgB7cwwLaGVsbG8gd29ybGR0DBQAAQIDBAUGBwgJAAECAwQFBgcICdsw2yhK2CQJSsoAFCgDOnUMIAABAgMEBQYHCAkAAQIDBAUGBwgJAAECAwQFBgcICQAB2zDbKErYJAlKygAgKAM6dgwhAAECAwQFBgcICQABAgMEBQYHCAkAAQIDBAUGBwgJAAEC2zDbKErYJAlKygAhKAM6dwdqDARib29sacFFU4tQQeY/GIRrDANpbnRpwUVTi1BB5j8YhGwMBnN0cmluZ2nBRVOLUEHmPxiEbQwHdWludDE2MGnBRVOLUEHmPxiEbgwHdWludDI1NmnBRVOLUEHmPxiEbwcMB2VjcG9pbnRpwUVTi1BB5j8YhAwEYm9vbGnBRVOLUEGSXegxqqp3CAwDaW50acFFU4tQQZJd6DHbIXcJDAZzdHJpbmdpwUVTi1BBkl3oMXcKDAd1aW50MTYwacFFU4tQQZJd6DF3CwwHdWludDI1NmnBRVOLUEGSXegxdwwMB2VjcG9pbnRpwUVTi1BBkl3oMXcNam8IlyQFCSIGa28JlyQFCSIGbG8KlyQFCSIGbW8LlyQFCSIGbm8MlyQECUBvB28Nl0BXBAAMAgD/2zBwaEGb9mfOEsBxDAIAAdswcmoMCWJ5dGVBcnJheWnBRVOLUEHmPxiEDAlieXRlQXJyYXlpwUVTi1BBkl3oMdswc2tAVwICDAIA/9swcGhBm/ZnzhLAcXnbKHjbKGnBRVOLUEHmPxiECEBXAwIMAgGq2zBwaEGb9mfOEsBxeRHAcmp4aTQLeGk0G3JqEM5AVwADejcAAHl4wUVTi1BB5j8YhEBXAgJ5eMFFU4tQQZJd6DFwaHFp2CYEC0BoNwEAQFcBAAwBAdsw2ygMBGtleTE0KAwBAtsw2ygMBGtleTI0GRQMA2tleTQhcGhBnAjtnEVoQfNUvx1AVwACeXhBm/ZnzkHmPxiEQFcAAnl4Qfa0a+JB3zC4mkBXAgIMAmlpcGhBm/ZnzhLAcXnbKEp42yhpwUVTi1BB5j8YhEUIQFcCAQwCaWlwaEGb9mfOEsBxeNsoacFFU4tQQZJd6DHbMEC1CIgF"));

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
}
