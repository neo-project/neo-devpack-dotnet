using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContractDescriptionAttribute : ManifestExtraAttribute
    {
        /// <summary>
        /// Specifies the description of the contract in the manifest.
        /// </summary>
        /// <param name="value">Description of the contract.</param>
        public ContractDescriptionAttribute(string value) : base(AttributeType[nameof(ContractDescriptionAttribute)], value)
        {
        }
    }
}
