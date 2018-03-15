using Neo.VM;

namespace Neo.SmartContract.Framework
{
    public class MAP
    {
        [OpCode(OpCode.NEWMAP)]
        public extern MAP();
    }
    public static class MAPHelper
    {
        [OpCode(OpCode.NEWMAP)]
        public extern static MAP New();

        [OpCode(OpCode.SETITEM)]
        public extern static void Put(this MAP dict, string key, byte[] value);

        [OpCode(OpCode.PICKITEM)]
        public extern static byte[] Get(this MAP dict, string key);

        [OpCode(OpCode.REMOVE)]
        public extern static byte[] Remove(this MAP dict, string key);

        [OpCode(OpCode.HASKEY)]
        public extern static byte[] HasKey(this MAP dict, string key);

        [OpCode(OpCode.KEYS)]
        public extern static System.Collections.ICollection Keys(this MAP dict);

        [OpCode(OpCode.VALUES)]
        public extern static System.Collections.ICollection Values(this MAP dict);
    }

}
