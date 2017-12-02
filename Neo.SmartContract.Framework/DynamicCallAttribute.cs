using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class DynamicCallAttribute : Attribute
    {
        public DynamicCallAttribute()
        {

        }
    }
}
