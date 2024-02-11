using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class StdLib : Neo.SmartContract.Testing.SmartContract
{
    #region Safe methods
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger atoi(string value);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger atoi(string value, BigInteger @base);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract byte[] base58CheckDecode(string s);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract string base58CheckEncode(byte[] data);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract byte[] base58Decode(string s);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract string base58Encode(byte[] data);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract byte[] base64Decode(string s);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract string base64Encode(byte[] data);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract object deserialize(byte[] data);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract string itoa(BigInteger value);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract string itoa(BigInteger value, BigInteger @base);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract object jsonDeserialize(byte[] json);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract byte[] jsonSerialize(object item);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger memoryCompare(byte[] str1, byte[] str2);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger memorySearch(byte[] mem, byte[] value);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger memorySearch(byte[] mem, byte[] value, BigInteger start);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger memorySearch(byte[] mem, byte[] value, BigInteger start, bool backward);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract byte[] serialize(object item);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract List<object> stringSplit(string str, string separator);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract List<object> stringSplit(string str, string separator, bool removeEmptyEntries);
    /// <summary>
    /// Safe method
    /// </summary>
    public abstract BigInteger strLen(string str);
    #endregion
    #region Constructor for internal use only
    protected StdLib(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) {}
    #endregion
}