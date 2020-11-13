using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ManifestNameAttribute : Attribute
    {
        public string Value { get; set; }

        public ManifestNameAttribute(string value)
        {
            Value = value;
        }
    }
}
