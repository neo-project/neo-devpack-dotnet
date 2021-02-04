using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class WildcardContractPermissionAttribute : Attribute
    {
        public string[] Methods { get; set; }

        public WildcardContractPermissionAttribute(params string[] methods)
        {
            Methods = methods;
        }
    }
}
