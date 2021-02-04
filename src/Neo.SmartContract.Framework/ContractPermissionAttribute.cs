using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ContractPermissionAttribute : Attribute
    {
        public string HashOrGroup { get; set; }
        public string[] Methods { get; set; }

        public ContractPermissionAttribute(string hashOrGroup, params string[] methods)
        {
            HashOrGroup = hashOrGroup;
            Methods = methods;
        }
    }
}
