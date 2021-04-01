using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.Cryptography.ECC;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_ParameterType : SmartContract.Framework.SmartContract
    {
        public static int _int;
        public static uint _uint;
        public static long _long;
        public static ulong _ulong;
        public static BigInteger _bigInteger;
        public static bool _bool;
        public static byte _byte;
        public static string _string;
        public static char _char;
        public static sbyte _sbyte;
        public static short _short;
        public static ushort _ushort;
        public static object _object;
        public static byte[] _byteArray;
        public static object[] _objectArray;
        public static int[] _intArray;
        public static bool[] _boolArray;
        public static UInt160 _uint160;
        public static UInt256 _uint256;
        public static ECPoint _ecpoint;
    }
}
