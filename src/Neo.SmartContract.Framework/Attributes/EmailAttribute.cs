using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EmailAttribute : ManifestExtraAttribute
    {
        public EmailAttribute(string value) : base(AttributeType[nameof(EmailAttribute)], value)
        {
        }
    }
}
