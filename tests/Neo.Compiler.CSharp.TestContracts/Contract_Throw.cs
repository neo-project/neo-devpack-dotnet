using System;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Throw : SmartContract.Framework.SmartContract
    {
        public static void TestMain(string[] args)
        {
            string first = args.Length >= 1 ? args[0] : throw new ArgumentException("Please supply at least one argument.");
        }
    }

    public class Person
    {
        private string name = string.Empty;

        public string Name
        {
            get => name;
            set => name = value ??
                throw new ArgumentNullException($"{nameof(value)} cannot be null");
        }
    }
}
