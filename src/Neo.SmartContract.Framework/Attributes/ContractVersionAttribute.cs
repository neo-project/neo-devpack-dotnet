using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContractVersionAttribute : ManifestExtraAttribute
    {
        /// <summary>
        /// Specifies the version of the contract in the manifest.
        /// </summary>
        /// <param name="value">Version of the contract</param>
        /// <remarks>The version is different from the Update Counter.</remarks>
        public ContractVersionAttribute(string value) : base(AttributeType[nameof(ContractVersionAttribute)], value)
        {
        }
    }
}
