using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InitialValueAttribute : Attribute
    {
        public InitialValueAttribute(string value, ContractParameterType type)
        {
        }
    }
}
