using System;
using System.Globalization;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor)]
    public class AppcallAttribute : Attribute
    {
        public byte[] ScriptHash { get; }

        public AppcallAttribute(byte[] scriptHash)
        {
            if (scriptHash == null) throw new ArgumentNullException();
            if (scriptHash.Length != 20) throw new ArgumentException();
            this.ScriptHash = scriptHash;
        }

        public AppcallAttribute(string scriptHash)
        {
            if (scriptHash == null) throw new ArgumentNullException();

            if (scriptHash.StartsWith("0x"))
            {
                scriptHash = scriptHash.Remove(0, 2);
            }

            if (scriptHash.Length != 40) throw new ArgumentException();
            this.ScriptHash = new byte[scriptHash.Length / 2];
            for (int i = 0; i < this.ScriptHash.Length; i++)
                this.ScriptHash[i] = byte.Parse(scriptHash.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);
        }
    }
}
