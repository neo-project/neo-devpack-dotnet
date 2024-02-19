using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContractDescriptionAttribute : ManifestExtraAttribute
    {
        public ContractDescriptionAttribute(string value) : base(AttributeType[nameof(ContractDescriptionAttribute)], value)
        {
        }
    }
}
