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

        public virtual string test() { return "base"; }
        public virtual string test2() { return "base"; }
    }

    public abstract class B : A
    {
        public int mul(int a, int b)
        {
            return a * b;
        }
    }

    public class Contract_Polymorphism : B
    {
        public override string test()
        {
            return "test";
        }

        public override string test2()
        {
            return base.test2() + ".test";
        }
    }
}
