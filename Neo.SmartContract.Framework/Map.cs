using Neo.VM;
using System.Numerics;

namespace Neo.SmartContract.Framework
{
    public class Map
    {
        [OpCode(OpCode.NEWMAP)]
        public extern Map();
    }
    public static class MapHelper
    {
        [OpCode(OpCode.SETITEM)]
        public extern static void Put(this Map dict, string key, byte[] value);

        [OpCode(OpCode.SETITEM)]
        public extern static void Put(this Map dict, byte[] key, byte[] value);

        [OpCode(OpCode.SETITEM)]
        public extern static void Put(this Map dict, BigInteger key, byte[] value);

        [OpCode(OpCode.PICKITEM)]
        public extern static byte[] Get(this Map dict, string key);

        [OpCode(OpCode.PICKITEM)]
        public extern static byte[] Get(this Map dict, byte[] key);

        [OpCode(OpCode.PICKITEM)]
        public extern static byte[] Get(this Map dict, BigInteger key);

        [OpCode(OpCode.REMOVE)]
        public extern static void Remove(this Map dict, string key);

        [OpCode(OpCode.REMOVE)]
        public extern static void Remove(this Map dict, byte[] key);

        [OpCode(OpCode.REMOVE)]
        public extern static void Remove(this Map dict, BigInteger key);

        [OpCode(OpCode.HASKEY)]
        public extern static bool HasKey(this Map dict, string key);

        [OpCode(OpCode.HASKEY)]
        public extern static bool HasKey(this Map dict, byte[] key);

        [OpCode(OpCode.HASKEY)]
        public extern static bool HasKey(this Map dict, BigInteger key);

        [OpCode(OpCode.KEYS)]
        public extern static byte[][] Keys(this Map dict);

        [OpCode(OpCode.VALUES)]
        public extern static byte[][] Values(this Map dict);
    }

}
