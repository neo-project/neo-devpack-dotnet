// Copyright (C) 2015-2024 The Neo Project.
//
// Contract_PostfixUnary.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

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

            public int Age = 1;
            public int Gender { get; set; } = 0;

            public int[] BWH { get; set; } = { 80, 60, 80 };

            public static int Height = 170;
            public static int Weight { get; set; } = 50;

            public static void Invert()
            {
                ExecutionEngine.Assert(~(Height++) == -171);
                ExecutionEngine.Assert(~(--Weight) == -50);
            }

            public Person(string name) { Name = name; Age--; ++Age; --Age; }
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

            ExecutionEngine.Assert(p.Gender++ == 0);
        }

        public static void TestInvert()
        {
            Person.Invert();
            ExecutionEngine.Assert(~(Person.Height--) == -172);
            ExecutionEngine.Assert(~(++Person.Height) == -172);
        }
    }
}
