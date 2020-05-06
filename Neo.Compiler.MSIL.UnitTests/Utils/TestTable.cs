
using Neo.VM;

namespace Neo.Compiler.MSIL.Utils
{
    internal class TestTable : IScriptTable
    {
        public byte[] GetScript(byte[] script_hash)
        {
            return null;
            //if (script_hash.Length == 1 && script_hash[0] == 99)
            //{
            //    return new byte[] { (byte)VM.OpCode.DROP, (byte)VM.OpCode.DROP, (byte)VM.OpCode.RET };
            //}
            //return new byte[] { (byte)VM.OpCode.PUSH1, (byte)VM.OpCode.RET };
        }
    }
}
