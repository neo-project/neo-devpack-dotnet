using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ManifestExtraAttribute : Attribute
    {
        public ManifestExtraAttribute(string key, string value)
        {
        }
    }
}
