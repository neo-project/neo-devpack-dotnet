using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ContractAttribute : Attribute
    {
        public ContractAttribute(string hash)
        {
        }
    }
}
