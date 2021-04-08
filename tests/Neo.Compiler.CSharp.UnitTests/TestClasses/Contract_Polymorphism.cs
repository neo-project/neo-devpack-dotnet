using System;
using System.Numerics;

namespace Neo.Compiler.CSharp.UnitTests.TestClasses
{
    public abstract class A : SmartContract.Framework.SmartContract
    {
        public int sum(int a, int b)
        {
            return a + b;
        }
    }

    public class B : A
    {
        public int mul(int a, int b)
        {
            return a * b;
        }
    }

    public class Contract_Polymorphism : B
    {

    }
}
