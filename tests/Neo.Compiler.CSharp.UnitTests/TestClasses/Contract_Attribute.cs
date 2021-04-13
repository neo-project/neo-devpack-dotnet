using System;
using System.ComponentModel;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public class SampleAttribute : System.Attribute
    {
        public SampleAttribute() { }
    }

    public class Contract_Attribute : SmartContract.Framework.SmartContract
    {
        [SampleAttribute]
        public static bool test() => true;
    }
}
