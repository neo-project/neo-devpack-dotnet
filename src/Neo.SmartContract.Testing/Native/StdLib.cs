using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class StdLib : Neo.SmartContract.Testing.SmartContract
{
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("atoi")]
    public abstract BigInteger Atoi(string value);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("atoi")]
    public abstract BigInteger Atoi(string value, BigInteger @base);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("base58CheckDecode")]
    public abstract byte[] Base58CheckDecode(string s);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("base58CheckEncode")]
    public abstract string Base58CheckEncode(byte[] data);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("base58Decode")]
    public abstract byte[] Base58Decode(string s);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("base58Encode")]
    public abstract string Base58Encode(byte[] data);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("base64Decode")]
    public abstract byte[] Base64Decode(string s);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("base64Encode")]
    public abstract string Base64Encode(byte[] data);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("deserialize")]
    public abstract object Deserialize(byte[] data);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("itoa")]
    public abstract string Itoa(BigInteger value);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("itoa")]
    public abstract string Itoa(BigInteger value, BigInteger @base);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("jsonDeserialize")]
    public abstract object JsonDeserialize(byte[] json);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("jsonSerialize")]
    public abstract byte[] JsonSerialize(object item);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("memoryCompare")]
    public abstract BigInteger MemoryCompare(byte[] str1, byte[] str2);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("memorySearch")]
    public abstract BigInteger MemorySearch(byte[] mem, byte[] value);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("memorySearch")]
    public abstract BigInteger MemorySearch(byte[] mem, byte[] value, BigInteger start);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("memorySearch")]
    public abstract BigInteger MemorySearch(byte[] mem, byte[] value, BigInteger start, bool backward);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("serialize")]
    public abstract byte[] Serialize(object item);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("stringSplit")]
    public abstract List<object> StringSplit(string str, string separator);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("stringSplit")]
    public abstract List<object> StringSplit(string str, string separator, bool removeEmptyEntries);
    /// <summary>
    /// Safe method
    /// </summary>
    [DisplayName("strLen")]
    public abstract BigInteger StrLen(string str);
    #endregion
    #region Constructor for internal use only
    protected StdLib(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {}
    #endregion
}