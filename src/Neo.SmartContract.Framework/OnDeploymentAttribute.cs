using System;
using System.ComponentModel;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class OnDeploymentAttribute : DisplayNameAttribute
    {
        public const string MethodName = "_deploy";

        public OnDeploymentAttribute() : base(MethodName) { }
    }
}
