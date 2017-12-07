using Neo.SmartContract.Framework.Services.Neo;
using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method )]
    public class EntryPointAttribute : Attribute
    {
        public TriggerType Type { get; }

        public EntryPointAttribute(TriggerType type)
        {
            this.Type = type;
        }
    }
}
