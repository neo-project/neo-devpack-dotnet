using System;

namespace Neo.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class OnDeploymentAttribute : Attribute { }
}
