using System;

namespace AntShares.SmartContract.Framework
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class NonemitAttribute : Attribute
    {
    }
}
