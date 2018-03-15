using Neo.VM;

namespace Neo.SmartContract.Framework
{
    public class Map
    {
        [OpCode(OpCode.NEWMAP)]
        public extern Map();
    }
    public static class MapHelper
    {
        [OpCode(OpCode.NEWMAP)]
        public extern static Map New();

        [OpCode(OpCode.SETITEM)]
        public extern static void Put(this Map dict, string key, byte[] value);

        [OpCode(OpCode.PICKITEM)]
        public extern static byte[] Get(this Map dict, string key);

        [OpCode(OpCode.REMOVE)]
        public extern static byte[] Remove(this Map dict, string key);

        [OpCode(OpCode.HASKEY)]
        public extern static byte[] HasKey(this Map dict, string key);

        [OpCode(OpCode.KEYS)]
        public extern static System.Collections.ICollection Keys(this Map dict);

        [OpCode(OpCode.VALUES)]
        public extern static System.Collections.ICollection Values(this Map dict);
    }

}
