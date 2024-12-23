using System;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public abstract class A : SmartContract.Framework.SmartContract
    {
        public A() : base() { }

        public int sum(int a, int b)
        {
            return a + b;
        }

        public int sumToBeOverriden(int a, int b)
        {
            return sum(a, b);
        }

        public virtual string test() { return "base"; }
        public virtual string test2() { return "base2"; }
        public abstract string abstractTest();
    }

    public abstract class B : A
    {
        public B() : base() { }

        public int mul(int a, int b)
        {
            return a * b;
        }
    }

    public class C : B
    {
        public override string test()
        {
            return "test";
        }

        public override string test2()
        {
            return base.test2() + ".test2";
        }

        public override string abstractTest()
        {
            return "abstractTest";
        }
    }

    public class Contract_Polymorphism : C
    {
        protected int sumToBeOverriden(sbyte a, sbyte b)
        {
            return a - b;
        }

        public new int sumToBeOverriden(int a, int b)
        {
            return base.sumToBeOverriden(a, b) + a * b + sumToBeOverriden((sbyte)a, (sbyte)b);
        }

        public override string test()
        {
            return "testFinal";
        }

        public override string test2()
        {   //     test          base2.test2    .test
            return base.test() + base.test2() + ".test";
            //     base.test() calls an overridden method
            //     base.test2()calls an overriden method, which recursively calls a virtual method
        }

        public override string abstractTest()
        {
            return base.abstractTest() + "overridenAbstract";
        }
    }
}
