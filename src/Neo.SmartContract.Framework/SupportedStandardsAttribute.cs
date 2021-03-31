using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SupportedStandardsAttribute : Attribute
    {
        public SupportedStandardsAttribute(params string[] supportedStandards)
        {
        }
    }
}
