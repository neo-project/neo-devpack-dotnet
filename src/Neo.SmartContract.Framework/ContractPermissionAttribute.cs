using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ContractPermissionAttribute : Attribute
    {
        public ContractPermissionAttribute(string contract, params string[] methods)
        {
        }
    }
}
