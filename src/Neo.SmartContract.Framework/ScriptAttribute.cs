using System;
using System.Text;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = true)]
    public class ScriptAttribute : Attribute
    {
        /// <summary>
        /// script in hex format
        /// </summary>
        public string Script { get; }

        public ScriptAttribute(params OpCode[] opcodes)
        {
            var sb = new StringBuilder();

            foreach (var opcode in opcodes)
            {
                sb.Append(((byte)opcode).ToString("x2"));
            }

            Script = sb.ToString();
        }
    }
}
