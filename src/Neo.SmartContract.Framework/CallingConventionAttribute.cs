using System;
using System.Runtime.InteropServices;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    class CallingConventionAttribute : Attribute
    {
        public CallingConventionAttribute(CallingConvention callingConvention)
        {
        }
    }
}
