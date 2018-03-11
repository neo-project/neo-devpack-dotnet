using Neo.VM;
using System;
using System.Numerics;

namespace Neo.SmartContract.Framework
{
    public static class Helper
    {
        [Nonemit]
        public extern static BigInteger AsBigInteger(this byte[] source);

        [Nonemit]
        public extern static byte[] AsByteArray(this BigInteger source);

        [Nonemit]
        public extern static byte[] AsByteArray(this string source);

        [Nonemit]
        public extern static string AsString(this byte[] source);

        [OpCode(OpCode.CAT)]
        public extern static byte[] Concat(this byte[] first, byte[] second);

        [NonemitWithConvert(ConvertMethod.HexToBytes)]
        public extern static byte[] HexToBytes(this string hex);

        [OpCode(OpCode.SUBSTR)]
        public extern static byte[] Range(this byte[] source, int index, int count);

        [OpCode(OpCode.LEFT)]
        public extern static byte[] Take(this byte[] source, int count);

        [Nonemit]
        public extern static Delegate ToDelegate(this byte[] source);

        [NonemitWithConvert(ConvertMethod.ToScriptHash)]
        public extern static byte[] ToScriptHash(this string address);

        [Syscall("Neo.Runtime.Serialize")]
        public extern static byte[] Serialize(this object objectOrArray);

        [Syscall("Neo.Runtime.Deserialize")]
        public extern static object[] Deserialize(this byte[] source);

        
    }
    public interface IDictionary
    {
    }
    public static class DictionaryHelper
    {
        [OpCode(OpCode.NEWMAP)]
        public extern static IDictionary New();

        [OpCode(OpCode.SETITEM)]
        public extern static void Put(this IDictionary dict, string key, byte[] value);

        [OpCode(OpCode.PICKITEM)]
        public extern static byte[] Get(this IDictionary dict, string key);

        [OpCode(OpCode.REMOVE)]
        public extern static byte[] Remove(this IDictionary dict, string key);

        [OpCode(OpCode.HASKEY)]
        public extern static byte[] HasKey(this IDictionary dict, string key);

        [OpCode(OpCode.KEYS)]
        public extern static System.Collections.ICollection Keys(this IDictionary dict);

        [OpCode(OpCode.VALUES)]
        public extern static System.Collections.ICollection Values(this IDictionary dict);
    }

}
