using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AuthorAttribute : ManifestExtraAttribute
    {
        public AuthorAttribute(string value) : base(AttributeType[nameof(AuthorAttribute)], value)
        {
        }
    }
}
