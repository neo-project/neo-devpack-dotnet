using System;
using System.Runtime.InteropServices;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method)]
    class CallingConventionAttribute : Attribute
    {
        public CallingConventionAttribute(CallingConvention callingConvention)
        {
        }
    }
}
