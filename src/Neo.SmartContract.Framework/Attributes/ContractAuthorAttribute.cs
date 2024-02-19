using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContractAuthorAttribute : ManifestExtraAttribute
    {
        public ContractAuthorAttribute(string author) : base(AttributeType[nameof(ContractAuthorAttribute)], author)
        {
        }

        public ContractAuthorAttribute(string author, string email) : base(AttributeType[nameof(ContractAuthorAttribute)], author, email)
        {
        }
    }
}
