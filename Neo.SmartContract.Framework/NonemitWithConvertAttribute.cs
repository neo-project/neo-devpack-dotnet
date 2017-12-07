using System;
using System.Globalization;

namespace Neo.SmartContract.Framework
{
    public enum ConvertMethod
    {
        HexString2Bytes,
        AddressString2ScriptHashBytes,
    }
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class NonemitWithConvertAttribute : Attribute
    {
        public ConvertMethod method { get; }

        public NonemitWithConvertAttribute(ConvertMethod method)
        {
            this.method = method;
        }
    }
}