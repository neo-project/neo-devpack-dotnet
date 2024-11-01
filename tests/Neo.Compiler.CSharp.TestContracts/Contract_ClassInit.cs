using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public struct IntInit
    {
        public int A;
        public BigInteger B;
    }

    public class ClassWithDifferentTypes
    {
        public bool b;
        public char c;
        public int i;
        public string? s;
        public IntInit ii;
        public ClassWithDifferentTypes? cl;
    }

    public class Contract_ClassInit : SmartContract.Framework.SmartContract
    {
        public static IntInit testInitInt()
        {
            return new IntInit();
        }

        public static ClassWithDifferentTypes TestInitializationExpression()
        {
            ClassWithDifferentTypes newCl = new() { s = "s", ii = new(), cl = new() };
            foreach (ClassWithDifferentTypes cl in new ClassWithDifferentTypes[] { newCl, newCl.cl })
            {
                ExecutionEngine.Assert(cl.b == default);
                ExecutionEngine.Assert(cl.c == default);
                ExecutionEngine.Assert(cl.i == default);
            }
            ExecutionEngine.Assert(newCl.s == "s");
            ExecutionEngine.Assert(newCl.cl.s == null);
            ExecutionEngine.Assert(newCl.ii.A == default);
            ExecutionEngine.Assert(newCl.ii.B == default);
            return newCl;
        }
    }
}
