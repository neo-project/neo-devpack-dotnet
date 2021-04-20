using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ContractTrustAttribute : Attribute
    {
        public ContractTrustAttribute(string contractOrGroup)
        {
        }
    }
}
