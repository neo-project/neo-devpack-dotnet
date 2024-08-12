using System;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_Throw : SmartContract.Framework.SmartContract
    {
        public static void TestMain(string[] args)
        {
            string first = args.Length >= 1 ? args[0] : throw new ArgumentException("Please supply at least one argument.");
        }
    }
}
