using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VersionAttribute : ManifestExtraAttribute
    {
        public VersionAttribute(string value) : base(AttributeType[nameof(VersionAttribute)], value)
        {
        }
    }
}
