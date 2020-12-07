using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ContractNameAttribute : Attribute
    {
        public string Value { get; set; }

        public ContractNameAttribute(string value)
        {
            Value = value;
        }
    }
}
