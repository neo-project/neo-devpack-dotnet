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
        public ConvertMethod Method { get; }

        public NonemitWithConvertAttribute(ConvertMethod method)
        {
            this.Method = method;
        }
    }
}