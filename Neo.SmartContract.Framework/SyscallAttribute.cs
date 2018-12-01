using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Constructor)]
    public class SyscallAttribute : Attribute
    {
        public string[] Methods { get; }

        public SyscallAttribute(params string[] methods)
        {
            this.Methods = methods;
        }
    }
}
