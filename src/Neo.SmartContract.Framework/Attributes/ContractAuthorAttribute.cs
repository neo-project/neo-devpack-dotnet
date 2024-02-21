using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContractAuthorAttribute : ManifestExtraAttribute
    {
        /// <summary>
        ///     Specifies the author of the contract in the manifest.
        /// </summary>
        /// <param name="author">The name of the contract author</param>
        public ContractAuthorAttribute(string author) : base(AttributeType[nameof(ContractAuthorAttribute)], author)
        {
        }

        /// <summary>
        /// Specifies the author and email of the contract in the manifest.
        /// </summary>
        /// <param name="author">The name of the contract author</param>
        /// <param name="email">The email of the contract author</param>
        public ContractAuthorAttribute(string author, string email) : base(AttributeType[nameof(ContractAuthorAttribute)], author, email)
        {
        }
    }
}
