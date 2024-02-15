using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptionAttribute : ManifestExtraAttribute
    {
        public DescriptionAttribute(string value) : base(AttributeType[nameof(DescriptionAttribute)], value)
        {
        }
    }
}
