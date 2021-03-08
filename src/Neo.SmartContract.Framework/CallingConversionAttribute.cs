using System;
using System.Runtime.InteropServices;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class CallingConversionAttribute : Attribute
    {
        public CallingConvention CallingConversion { get; private set; }

        public CallingConversionAttribute(CallingConvention callingConvention)
        {
            CallingConversion = callingConvention;
        }
    }
}
