using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class Contract_MultipleA : SmartContract.Framework.SmartContract
    {
        public static bool test() => true;
    }

    public class Contract_MultipleB : SmartContract.Framework.SmartContract
    {
        public static bool test() => false;
    }
}
