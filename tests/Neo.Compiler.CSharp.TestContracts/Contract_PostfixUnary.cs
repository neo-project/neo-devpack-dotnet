using Microsoft.ApplicationInsights.DataContracts;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Numerics;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_PostfixUnary : SmartContract.Framework.SmartContract
    {
        public class Person
        {
            public string Name { get; set; }

            public int Age;

            public int[] BWH { get; set; } = { 80, 60, 80 };

            public Person(string name) { Name = name; }
        }

        public static string? Test()
        {
            Person? p = new("John");
            if (IsValid(p))
            {
                p.Age++;
                p.BWH[1]++;

                return p!.Name;
            }

            return null;
        }

        public static bool IsValid(Person? person) => person is not null && person.Name is not null;

        public static void TestUndefinedCase()
        {
            Person p = new("Undefined");
            ExecutionEngine.Assert(p.Age == 0);
            p.Age = p.Age++;  // This is undefined; typically should be just p.Age++
            ExecutionEngine.Assert(p.Age == 0);
            p.Age = ++p.Age;
            ExecutionEngine.Assert(p.Age == 1);

            ExecutionEngine.Assert(p.BWH[0] == 80);
            p.BWH[0] = p.BWH[0]++;
            ExecutionEngine.Assert(p.BWH[0] == 80);
            p.BWH[0] = ++p.BWH[0];
            ExecutionEngine.Assert(p.BWH[0] == 81);
        }

        public static void TestInvert()
        {
            ExecutionEngine.Assert(~1 == -2);
        }
    }
}
