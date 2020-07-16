using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SupportedStandardsAttribute : Attribute
    {
        public string[] Value { get; set; }

        public SupportedStandardsAttribute(params string[] supportedStandards)
        {
            Value = supportedStandards;
        }
    }
}
