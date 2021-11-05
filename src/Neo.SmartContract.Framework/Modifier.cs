using System;

namespace Neo.SmartContract.Framework
{
    public abstract class Modifier : Attribute
    {
        public abstract void Validate();
    }
}
