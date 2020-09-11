using System;
using System.Globalization;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ContractAttribute : Attribute
    {
        public byte[] Hash { get; }

        public ContractAttribute(byte[] hash)
        {
            if (hash == null) throw new ArgumentNullException();
            if (hash.Length != 20) throw new ArgumentException();
            Hash = hash;
        }

        public ContractAttribute(string hash)
        {
            if (hash == null) throw new ArgumentNullException();

            if (hash.StartsWith("0x"))
            {
                hash = hash.Remove(0, 2);
            }

            if (hash.Length != 40) throw new ArgumentException();
            Hash = new byte[hash.Length / 2];
            for (int i = 0; i < Hash.Length; i++)
                Hash[i] = byte.Parse(hash.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);
        }
    }
}
