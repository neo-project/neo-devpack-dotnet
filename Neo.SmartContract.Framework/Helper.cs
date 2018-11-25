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
        
        // a <= x && x < b
        [OpCode(OpCode.WITHIN)]
        public extern static bool Within(this BigInteger x, int a, int b);
        
        // a <= x && x < b
        [OpCode(OpCode.WITHIN)]
        public extern static bool Within(this int x, int a, int b);
        
        // faults if b is false
        [OpCode(OpCode.THROWIFNOT)]
        public extern static void Assert(this bool b);
        
        public static sbyte AsSbyte(this BigInteger source)
        {
            Assert(source.Within(-128, 128));
            return (sbyte) source;
        }
        
        public static sbyte AsSbyte(this int source)
        {
            Assert(source.Within(-128, 128));
            return (sbyte) source;
        }
        
        public static byte AsByte(this BigInteger source)
        {
            Assert(source.Within(-128, 128));
            return (byte) source;
        }
        
        public static byte AsByte(this int source)
        {
            Assert(source.Within(-128, 128));
            return (byte) source;
        }
        
        public static sbyte ToSbyte(this BigInteger source)
        {
            if(source > 127)
                source = source - 256;
            Assert(source.Within(-128, 128));
            return (sbyte) source;
        }
        
        public static sbyte ToSbyte(this int source)
        {
            if(source > 127)
                source = source - 256;
            Assert(source.Within(-128, 128));
            return (sbyte) source;
        }
        
        public static byte ToByte(this BigInteger source)
        {
             if(source > 127)
                source = source - 256;
            Assert(source.Within(-128, 128));
            return (byte) source;
        }
        
        public static byte ToByte(this int source)
        {
            if(source > 127)
                source = source - 256;
            Assert(source.Within(-128, 128));
            return (byte) source;
        }

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

        /// <summary>
        /// ToScriptHash converts a base-58 Address to ScriptHash in little-endian byte array.
        /// Example: "AFsCjUGzicZmXQtWpwVt6hNeJTBwSipJMS".ToScriptHash() generates 0102030405060708090a0b0c0d0e0faabbccddee
        /// </summary>
        [NonemitWithConvert(ConvertMethod.ToScriptHash)]
        public extern static byte[] ToScriptHash(this string address);

        [NonemitWithConvert(ConvertMethod.ToBigInteger)]
        public extern static BigInteger ToBigInteger(this string text);

        [Syscall("Neo.Runtime.Serialize")]
        public extern static byte[] Serialize(this object source);

        [Syscall("Neo.Runtime.Deserialize")]
        public extern static object Deserialize(this byte[] source);
    }
}
