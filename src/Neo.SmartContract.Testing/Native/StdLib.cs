using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing.Native;

public abstract class StdLib : SmartContract
{
    #region Compiled data

    public static readonly Manifest.ContractManifest Manifest = Neo.SmartContract.Manifest.ContractManifest.Parse(@"{""name"":""StdLib"",""groups"":[],""features"":{},""supportedstandards"":[],""abi"":{""methods"":[{""name"":""atoi"",""parameters"":[{""name"":""value"",""type"":""String""}],""returntype"":""Integer"",""offset"":0,""safe"":true},{""name"":""atoi"",""parameters"":[{""name"":""value"",""type"":""String""},{""name"":""base"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":7,""safe"":true},{""name"":""base58CheckDecode"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":14,""safe"":true},{""name"":""base58CheckEncode"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":21,""safe"":true},{""name"":""base58Decode"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":28,""safe"":true},{""name"":""base58Encode"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":35,""safe"":true},{""name"":""base64Decode"",""parameters"":[{""name"":""s"",""type"":""String""}],""returntype"":""ByteArray"",""offset"":42,""safe"":true},{""name"":""base64Encode"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""String"",""offset"":49,""safe"":true},{""name"":""deserialize"",""parameters"":[{""name"":""data"",""type"":""ByteArray""}],""returntype"":""Any"",""offset"":56,""safe"":true},{""name"":""itoa"",""parameters"":[{""name"":""value"",""type"":""Integer""}],""returntype"":""String"",""offset"":63,""safe"":true},{""name"":""itoa"",""parameters"":[{""name"":""value"",""type"":""Integer""},{""name"":""base"",""type"":""Integer""}],""returntype"":""String"",""offset"":70,""safe"":true},{""name"":""jsonDeserialize"",""parameters"":[{""name"":""json"",""type"":""ByteArray""}],""returntype"":""Any"",""offset"":77,""safe"":true},{""name"":""jsonSerialize"",""parameters"":[{""name"":""item"",""type"":""Any""}],""returntype"":""ByteArray"",""offset"":84,""safe"":true},{""name"":""memoryCompare"",""parameters"":[{""name"":""str1"",""type"":""ByteArray""},{""name"":""str2"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":91,""safe"":true},{""name"":""memorySearch"",""parameters"":[{""name"":""mem"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""}],""returntype"":""Integer"",""offset"":98,""safe"":true},{""name"":""memorySearch"",""parameters"":[{""name"":""mem"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""},{""name"":""start"",""type"":""Integer""}],""returntype"":""Integer"",""offset"":105,""safe"":true},{""name"":""memorySearch"",""parameters"":[{""name"":""mem"",""type"":""ByteArray""},{""name"":""value"",""type"":""ByteArray""},{""name"":""start"",""type"":""Integer""},{""name"":""backward"",""type"":""Boolean""}],""returntype"":""Integer"",""offset"":112,""safe"":true},{""name"":""serialize"",""parameters"":[{""name"":""item"",""type"":""Any""}],""returntype"":""ByteArray"",""offset"":119,""safe"":true},{""name"":""strLen"",""parameters"":[{""name"":""str"",""type"":""String""}],""returntype"":""Integer"",""offset"":126,""safe"":true},{""name"":""stringSplit"",""parameters"":[{""name"":""str"",""type"":""String""},{""name"":""separator"",""type"":""String""}],""returntype"":""Array"",""offset"":133,""safe"":true},{""name"":""stringSplit"",""parameters"":[{""name"":""str"",""type"":""String""},{""name"":""separator"",""type"":""String""},{""name"":""removeEmptyEntries"",""type"":""Boolean""}],""returntype"":""Array"",""offset"":140,""safe"":true}],""events"":[]},""permissions"":[{""contract"":""*"",""methods"":""*""}],""trusts"":[],""extra"":null}");

    #endregion

    #region Safe methods

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("atoi")]
    public abstract BigInteger? Atoi(string? value);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("atoi")]
    public abstract BigInteger? Atoi(string? value, BigInteger? @base);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("base58CheckDecode")]
    public abstract byte[]? Base58CheckDecode(string? s);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("base58CheckEncode")]
    public abstract string? Base58CheckEncode(byte[]? data);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("base58Decode")]
    public abstract byte[]? Base58Decode(string? s);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("base58Encode")]
    public abstract string? Base58Encode(byte[]? data);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("base64Decode")]
    public abstract byte[]? Base64Decode(string? s);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("base64Encode")]
    public abstract string? Base64Encode(byte[]? data);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("deserialize")]
    public abstract object? Deserialize(byte[]? data);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("itoa")]
    public abstract string? Itoa(BigInteger? value);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("itoa")]
    public abstract string? Itoa(BigInteger? value, BigInteger? @base);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("jsonDeserialize")]
    public abstract object? JsonDeserialize(byte[]? json);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("jsonSerialize")]
    public abstract byte[]? JsonSerialize(object? item = null);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("memoryCompare")]
    public abstract BigInteger? MemoryCompare(byte[]? str1, byte[]? str2);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("memorySearch")]
    public abstract BigInteger? MemorySearch(byte[]? mem, byte[]? value);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("memorySearch")]
    public abstract BigInteger? MemorySearch(byte[]? mem, byte[]? value, BigInteger? start);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("memorySearch")]
    public abstract BigInteger? MemorySearch(byte[]? mem, byte[]? value, BigInteger? start, bool? backward);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("serialize")]
    public abstract byte[]? Serialize(object? item = null);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("stringSplit")]
    public abstract string[]? StringSplit(string? str, string? separator);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("stringSplit")]
    public abstract string[]? StringSplit(string? str, string? separator, bool? removeEmptyEntries);

    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("strLen")]
    public abstract BigInteger? StrLen(string? str);

    #endregion

    #region Constructor for internal use only

    protected StdLib(SmartContractInitialize initialize) : base(initialize) { }

    #endregion
}
