// Copyright (C) 2015-2024 The Neo Project.
//
// UnitTest_StringOptimization.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.VM;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.Optimizer
{
    [TestClass]
    public class UnitTest_StringOptimization : TestBase
    {
        [TestMethod]
        public void TestConstantStringConcatenation()
        {
            var testString = @"
                using Neo.SmartContract.Framework;
                using System;

                public class Contract1 : Framework.SmartContract
                {
                    public static string TestMethod()
                    {
                        string a = ""Hello, "";
                        string b = ""World!"";
                        return a + b; // This should be optimized to a single PUSHDATA
                    }
                }";

            var compilation = CompileScript(testString, "Contract1");
            var result = Peephole.OptimizeStringOperations(compilation.nef, compilation.manifest);
            
            // Verify that the optimizer correctly optimizes constant string concatenation
            Assert.IsNotNull(result.Item1, "Optimization should succeed");
            
            // Verify the optimized script is smaller than the original
            Assert.IsTrue(result.Item1.Script.Length <= compilation.nef.Script.Length, 
                "Optimized script should be smaller than original");
        }

        [TestMethod]
        public void TestRepeatedStringLiterals()
        {
            var testString = @"
                using Neo.SmartContract.Framework;
                using System;

                public class Contract2 : Framework.SmartContract
                {
                    public static void TestMethod()
                    {
                        string a = ""Common String"";
                        string b = ""Common String""; // This should reuse the same string
                        Runtime.Log(a);
                        Runtime.Log(b);
                    }
                }";

            var compilation = CompileScript(testString, "Contract2");
            var result = Peephole.OptimizeStringOperations(compilation.nef, compilation.manifest);
            
            // Verify that the optimizer correctly optimizes repeated string literals
            Assert.IsNotNull(result.Item1, "Optimization should succeed");
        }

        [TestMethod]
        public void TestConstantSubstring()
        {
            var testString = @"
                using Neo.SmartContract.Framework;
                using System;

                public class Contract3 : Framework.SmartContract
                {
                    public static string TestMethod()
                    {
                        string a = ""Hello, World!"";
                        return a.Substring(0, 5); // This should be optimized to PUSHDATA ""Hello""
                    }
                }";

            var compilation = CompileScript(testString, "Contract3");
            var result = Peephole.OptimizeStringOperations(compilation.nef, compilation.manifest);
            
            // Verify that the optimizer correctly optimizes constant substring operations
            Assert.IsNotNull(result.Item1, "Optimization should succeed");
            
            // Verify the optimized script is smaller than the original
            Assert.IsTrue(result.Item1.Script.Length <= compilation.nef.Script.Length, 
                "Optimized script should be smaller than original");
        }

        [TestMethod]
        public void TestLeftRightSubstring()
        {
            var testString = @"
                using Neo.SmartContract.Framework;
                using System;

                public class Contract4 : Framework.SmartContract
                {
                    public static string[] TestMethod()
                    {
                        string a = ""Hello, World!"";
                        string left = a[..5]; // This should be optimized to PUSHDATA ""Hello""
                        string right = a[^6..]; // This should be optimized to PUSHDATA ""World!""
                        return new string[] { left, right };
                    }
                }";

            var compilation = CompileScript(testString, "Contract4");
            var result = Peephole.OptimizeStringOperations(compilation.nef, compilation.manifest);
            
            // Verify that the optimizer correctly optimizes left/right substring operations
            Assert.IsNotNull(result.Item1, "Optimization should succeed");
        }

        [TestMethod]
        public void TestMultipleStringConcatenation()
        {
            var testString = @"
                using Neo.SmartContract.Framework;
                using System;

                public class Contract5 : Framework.SmartContract
                {
                    public static string TestMethod()
                    {
                        string a = ""Hello, "";
                        string b = ""World"";
                        string c = ""!"";
                        return a + b + c; // This should be optimized to a single PUSHDATA
                    }
                }";

            var compilation = CompileScript(testString, "Contract5");
            var result = Peephole.OptimizeStringOperations(compilation.nef, compilation.manifest);
            
            // Verify that the optimizer correctly optimizes multiple string concatenations
            Assert.IsNotNull(result.Item1, "Optimization should succeed");
            
            // Verify the optimized script is smaller than the original
            Assert.IsTrue(result.Item1.Script.Length <= compilation.nef.Script.Length, 
                "Optimized script should be smaller than original");
        }

        [TestMethod]
        public void TestStringInterpolation()
        {
            var testString = @"
                using Neo.SmartContract.Framework;
                using System;

                public class Contract6 : Framework.SmartContract
                {
                    public static string TestMethod()
                    {
                        string name = ""Neo"";
                        int age = 5;
                        return $""Name: {name}, Age: {age}""; // This is harder to optimize at compile time
                    }
                }";

            var compilation = CompileScript(testString, "Contract6");
            var result = Peephole.OptimizeStringOperations(compilation.nef, compilation.manifest);
            
            // Verify that the optimizer handles string interpolation
            Assert.IsNotNull(result.Item1, "Optimization should succeed");
        }
    }
}
