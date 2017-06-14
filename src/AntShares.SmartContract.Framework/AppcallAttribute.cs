using System;

namespace AntShares.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class AppcallAttribute : Attribute
    {
        public byte[] hash { get; }

        public AppcallAttribute(byte[] hash)
        {
            this.hash = new byte[20];
            for (var i = 0; i < 20; i++)
            {
                this.hash[i] = hash[i];
            }
        }
        public AppcallAttribute(string hexstring)
        {
            this.hash = new byte[20];

            for (var i = 0; i < 20; i++)
            {
                this.hash[i] = byte.Parse(hexstring.Substring(i * 2, 2));
            }
        }
    }
}
