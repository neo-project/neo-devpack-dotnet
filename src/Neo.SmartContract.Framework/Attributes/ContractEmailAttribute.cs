using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContractEmailAttribute : ManifestExtraAttribute
    {
        public ContractEmailAttribute(string value) : base(AttributeType[nameof(ContractEmailAttribute)], value)
        {
        }
    }
}
