using Neo.VM;
using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = true)]
    public class ScriptAttribute : Attribute
    {
        /// <summary>
        /// script in hex format
        /// </summary>
        public string Script { get; }

        public ScriptAttribute(string script = "")
        {
            Script = script;
        }
    }
}
