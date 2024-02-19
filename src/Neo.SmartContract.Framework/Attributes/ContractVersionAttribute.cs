using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContractVersionAttribute : ManifestExtraAttribute
    {
        public ContractVersionAttribute(string value) : base(AttributeType[nameof(ContractVersionAttribute)], value)
        {
        }
    }
}
