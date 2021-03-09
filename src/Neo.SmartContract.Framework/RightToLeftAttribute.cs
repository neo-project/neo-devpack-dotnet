using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RightToLeftAttribute : Attribute
    {
        public bool RightToLeft { get; private set; }

        public RightToLeftAttribute(bool rightToLeft)
        {
            RightToLeft = rightToLeft;
        }
    }
}
