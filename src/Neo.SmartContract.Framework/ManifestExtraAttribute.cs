using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ManifestExtraAttribute : Attribute
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public ManifestExtraAttribute(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
