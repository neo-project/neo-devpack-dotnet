using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = true)]
    public class SyscallAttribute : Attribute
    {
        public string Method { get; }

        public SyscallAttribute(string method)
        {
            this.Method = method;
        }
    }
}
