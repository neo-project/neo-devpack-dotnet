using System;
using System.ComponentModel;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class OnVerifyAttribute : DisplayNameAttribute
    {
        public const string MethodName = "verify";

        public OnVerifyAttribute() : base(MethodName) { }
    }
}
