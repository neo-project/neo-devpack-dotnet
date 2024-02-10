using Neo.Cryptography.ECC;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.SmartContract.Testing;

public abstract class StdLib : Neo.SmartContract.Testing.SmartContract
{
    #region Safe methods
    public abstract BigInteger atoi(string value);
    public abstract BigInteger atoi(string value, BigInteger @base);
    public abstract byte[] base58CheckDecode(string s);
    public abstract string base58CheckEncode(byte[] data);
    public abstract byte[] base58Decode(string s);
    public abstract string base58Encode(byte[] data);
    public abstract byte[] base64Decode(string s);
    public abstract string base64Encode(byte[] data);
    public abstract object deserialize(byte[] data);
    public abstract string itoa(BigInteger value);
    public abstract string itoa(BigInteger value, BigInteger @base);
    public abstract object jsonDeserialize(byte[] json);
    public abstract byte[] jsonSerialize(object item);
    public abstract BigInteger memoryCompare(byte[] str1, byte[] str2);
    public abstract BigInteger memorySearch(byte[] mem, byte[] value);
    public abstract BigInteger memorySearch(byte[] mem, byte[] value, BigInteger start);
    public abstract BigInteger memorySearch(byte[] mem, byte[] value, BigInteger start, bool backward);
    public abstract byte[] serialize(object item);
    public abstract List<object> stringSplit(string str, string separator);
    public abstract List<object> stringSplit(string str, string separator, bool removeEmptyEntries);
    public abstract BigInteger strLen(string str);
    #endregion
    #region Constructor for internal use only
    protected StdLib(Neo.SmartContract.Testing.TestEngine testEngine, Neo.UInt160 hash) : base(testEngine, hash) { }
    #endregion
}
