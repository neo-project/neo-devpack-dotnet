using System;
using System.Numerics;

namespace Neo.SmartContract.Framework
{
    public static class Helper
    {
        /// <summary>
        /// Converts byte to byte[] considering the byte as a BigInteger (0x00 at the end)
        /// </summary>
        [Script]
        public extern static byte[] AsByteArray(this byte source);

        /// <summary>
        /// Converts sbyte to byte[].
        /// </summary>
        [Script]
        public extern static byte[] AsByteArray(this sbyte source);

        /// <summary>
        /// Converts sbyte[] to byte[].
        /// </summary>
        [Script]
        public extern static byte[] AsByteArray(this sbyte[] source);

        /// <summary>
        /// Converts byte[] to sbyte[].
        /// </summary>
        [Script]
        public extern static sbyte[] AsSbyteArray(this byte[] source);

        /// <summary>
        /// Converts byte[] to BigInteger. No guarantees are assumed regarding BigInteger working range.
        /// Examples: [0x0a] -> 10; [0x80] -> -128; [] -> 0; [0xff00] -> 255
        /// </summary>
        [Script]
        public extern static BigInteger AsBigInteger(this byte[] source);

        /// <summary>
        /// Converts byte[] to BigInteger and ensures output is within BigInteger range (32-bytes) in standard format; faults otherwise.
        /// Examples: -128 [0x80ff] -> -128 [0x80]; 0 [0x000000] -> 0 [0x00]; 0 [] -> 0 [0x00]; 255 [0xff00000000000000] -> 255 [0xff00]
        /// </summary>
        [OpCode(OpCode.PUSH0)]
        [OpCode(OpCode.ADD)]
        public extern static BigInteger ToBigInteger(this byte[] source);
        //{
        //    return source.AsBigInteger() + 0;
        //}

        /// <summary>
        /// Converts BigInteger to byte[]. No guarantees are assumed regarding BigInteger working range.
        /// Examples: 10 -> [0x0a]; 10 -> [0x0a00]; -128 -> [0x80]; -128 -> [0x80ff]; 0 -> []; 0 -> [0x00]; 255 -> [0xff00]
        /// </summary>
        [Script]
        public extern static byte[] AsByteArray(this BigInteger source);

        /// <summary>
        /// Converts string to byte[]. Examples: "hello" -> [0x68656c6c6f]; "" -> []; "Neo" -> [0x4e656f]
        /// </summary>
        [Script]
        public extern static byte[] AsByteArray(this string source);

        /// <summary>
        /// Converts byte[] to string. Examples: [0x68656c6c6f] -> "hello"; [] -> ""; [0x4e656f] -> "Neo"
        /// </summary>
        [Script]
        public extern static string AsString(this byte[] source);

        /// <summary>
        /// Returns true iff a <= x && x < b. Examples: x=5 a=5 b=15 is true; x=15 a=5 b=15 is false
        /// </summary>
        [OpCode(OpCode.WITHIN)]
        public extern static bool Within(this BigInteger x, BigInteger a, BigInteger b);

        /// <summary>
        /// Returns true iff a <= x && x < b. Examples: x=5 a=5 b=15 is true; x=15 a=5 b=15 is false
        /// </summary>
        [OpCode(OpCode.WITHIN)]
        public extern static bool Within(this int x, BigInteger a, BigInteger b);

        /// <summary>
        /// Faults iff b is false
        /// </summary>
        [OpCode(OpCode.THROWIFNOT)]
        public extern static void Assert(this bool b);

        /// <summary>
        /// Converts and ensures parameter source is sbyte (range 0x00 to 0xff); faults otherwise.
        /// Examples: 255 -> fault; -128 -> [0x80]; 0 -> [0x00]; 10 -> [0x0a]; 127 -> [0x7f]; 128 -> fault
        /// ScriptAttribute: DUP ARRAYSIZE PUSH1 NUMEQUAL THROWIFNOT
        /// </summary>
        [Script("76c0519cf1")]
        public extern static sbyte AsSbyte(this BigInteger source);
        //{
        //    Assert(source.AsByteArray().Length == 1);
        //    return (sbyte) source;
        //}

        /// <summary>
        /// Converts and ensures parameter source is sbyte (range 0x00 to 0xff); faults otherwise.
        /// Examples: 255 -> fault; -128 -> [0x80]; 0 -> [0x00]; 10 -> [0x0a]; 127 -> [0x7f]; 128 -> fault
        /// ScriptAttribute: DUP ARRAYSIZE PUSH1 NUMEQUAL THROWIFNOT
        /// </summary>
        [Script("76c0519cf1")]
        public extern static sbyte AsSbyte(this int source);
        //{
        //    Assert(((BigInteger)source).AsByteArray().Length == 1);
        //    return (sbyte) source;
        //}

        /// <summary>
        /// Converts and ensures parameter source is byte (range 0x00 to 0xff); faults otherwise.
        /// Examples: 255 -> fault; -128 -> [0x80]; 0 -> [0x00]; 10 -> [0x0a]; 127 -> [0x7f]; 128 -> fault
        /// ScriptAttribute: DUP ARRAYSIZE PUSH1 NUMEQUAL THROWIFNOT
        /// </summary>
        [Script("76c0519cf1")]
        public extern static byte AsByte(this BigInteger source);
        //{
        //    Assert(source.AsByteArray().Length == 1);
        //    return (byte) source;
        //}

        /// <summary>
        /// Converts and ensures parameter source is byte (range 0x00 to 0xff); faults otherwise.
        /// Examples: 255 -> fault; -128 -> [0x80]; 0 -> [0x00]; 10 -> [0x0a]; 127 -> [0x7f]; 128 -> fault
        /// ScriptAttribute: DUP ARRAYSIZE PUSH1 NUMEQUAL THROWIFNOT
        /// </summary>
        [Script("76c0519cf1")]
        public extern static byte AsByte(this int source);
        //{
        //    Assert(((BigInteger)source).AsByteArray().Length == 1);
        //    return (byte) source;
        //}

        /// <summary>
        /// Converts byte to byte[].
        /// </summary>
        [OpCode(OpCode.PUSH1)]
        [OpCode(OpCode.LEFT)]
        public extern static byte[] ToByteArray(this byte source);

        /// <summary>
        /// Converts parameter to sbyte from (big)integer range -128-255; faults if out-of-range.
        /// Examples: 256 -> fault; -1 -> -1 [0xff]; 255 -> -1 [0xff]; 0 -> 0 [0x00]; 10 -> 10 [0x0a]; 127 -> 127 [0x7f]; 128 -> -128 [0x80]
        /// </summary>
        public static sbyte ToSbyte(this BigInteger source)
        {
            if (source > 127)
                source = source - 256;
            Assert(source.Within(-128, 128));
            return (sbyte)(source + 0);
        }

        /// <summary>
        /// Converts parameter to sbyte from (big)integer range -128-255; faults if out-of-range.
        /// Examples: 256 -> fault; -1 -> -1 [0xff]; 255 -> -1 [0xff]; 0 -> 0 [0x00]; 10 -> 10 [0x0a]; 127 -> 127 [0x7f]; 128 -> -128 [0x80]
        /// </summary>
        public static sbyte ToSbyte(this int source)
        {
            if (source > 127)
                source = source - 256;
            Assert(source.Within(-128, 128));
            return (sbyte)(source + 0);
        }

        /// <summary>
        /// Converts parameter to byte from (big)integer range 0-255; faults if out-of-range.
        /// Examples: 256 -> fault; -1 -> fault; 255 -> -1 [0xff]; 0 -> 0 [0x00]; 10 -> 10 [0x0a]; 127 -> 127 [0x7f]; 128 -> -128 [0x80]
        /// </summary>
        public static byte ToByte(this BigInteger source)
        {
            Assert(source.Within(0, 256));
            if (source > 127)
                source = source - 256;
            return (byte)(source + 0);
        }

        /// <summary>
        /// Converts parameter to byte from (big)integer range 0-255; faults if out-of-range.
        /// Examples: 256 -> fault; -1 -> fault; 255 -> -1 [0xff]; 0 -> 0 [0x00]; 10 -> 10 [0x0a]; 127 -> 127 [0x7f]; 128 -> -128 [0x80]
        /// </summary>
        public static byte ToByte(this int source)
        {
            Assert(source.Within(0, 256));
            if (source > 127)
                source = source - 256;
            return (byte)(source + 0);
        }

        [OpCode(OpCode.CAT)]
        public extern static byte[] Concat(this byte[] first, byte[] second);

        [NonemitWithConvert(ConvertMethod.HexToBytes)]
        public extern static byte[] HexToBytes(this string hex);

        [OpCode(OpCode.SUBSTR)]
        public extern static byte[] Range(this byte[] source, int index, int count);

        /// <summary>
        /// Returns byte[] with first 'count' elements from 'source'. Faults if count < 0
        /// </summary>
        [OpCode(OpCode.LEFT)]
        public extern static byte[] Take(this byte[] source, int count);

        /// <summary>
        /// Returns byte[] with last 'count' elements from 'source'. Faults if count < 0
        /// </summary>
        [OpCode(OpCode.RIGHT)]
        public extern static byte[] Last(this byte[] source, int count);

        /// <summary>
        /// Returns a reversed copy of byte[] parameter 'source' (parameter is not affected because byte[] is copy-based on NeoVM).
        /// Example: [0a,0b,0c,0d,0e] -> [0e,0d,0c,0b,0a]
        /// </summary>
        public static byte[] Reverse(this byte[] source)
        {
            for (int k = 0; k < source.Length / 2; k++)
            {
                int m = source.Length - k - 1;
                byte bg = source[k]; // must store on variable before next assignment
                byte ed = source[m]; // must store on variable before next assignment
                source[k] = ed;
                source[m] = bg;
            }
            return source;
        }

        [Script]
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
