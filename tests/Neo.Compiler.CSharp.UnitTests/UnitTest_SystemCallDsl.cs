// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_SystemCallDsl.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_SystemCallDsl
    {
        private static IDictionary Handlers => LazyHandlers.Value;
        private static readonly Lazy<IDictionary> LazyHandlers = new(LoadHandlers);

        private static IDictionary LoadHandlers()
        {
            var methodConvertType = typeof(Program).Assembly.GetType("Neo.Compiler.MethodConvert", throwOnError: true)!;
            RuntimeHelpers.RunClassConstructor(methodConvertType.TypeHandle);
            var field = methodConvertType.GetField("SystemCallHandlers", BindingFlags.NonPublic | BindingFlags.Static)!;
            return (IDictionary)field.GetValue(null)!;
        }

        [TestMethod]
        public void Dsl_Should_Register_Static_And_Instance_Properties()
        {
            Assert.IsTrue(Handlers.Contains("System.Numerics.BigInteger.Zero.get"), "Missing handler for BigInteger.Zero");
            Assert.IsTrue(Handlers.Contains("System.Numerics.BigInteger.One.get"), "Missing handler for BigInteger.One");
            Assert.IsTrue(Handlers.Contains("System.Numerics.BigInteger.IsZero.get"), "Missing handler for BigInteger.IsZero");
        }

        [TestMethod]
        public void Dsl_Should_Register_Common_Static_Methods()
        {
            Assert.IsTrue(Handlers.Contains("System.Numerics.BigInteger.Add(System.Numerics.BigInteger, System.Numerics.BigInteger)"),
                "Missing handler for BigInteger.Add");
            Assert.IsTrue(Handlers.Contains("System.Math.Clamp(int, int, int)"), "Missing handler for Math.Clamp");
            Assert.IsTrue(Handlers.Contains("string.Contains(string)"), "Missing handler for string.Contains");
        }

        [TestMethod]
        public void Dsl_Should_Register_Conversion_Operators()
        {
            Assert.IsTrue(Handlers.Contains("System.Numerics.BigInteger.explicit operator byte(System.Numerics.BigInteger)"),
                "Missing handler for explicit BigInteger->byte conversion");
            Assert.IsTrue(Handlers.Contains("System.Numerics.BigInteger.implicit operator System.Numerics.BigInteger(byte)"),
                "Missing handler for implicit byte->BigInteger conversion");
        }

        [TestMethod]
        public void Dsl_Should_Register_Indexers_And_Generic_Methods()
        {
            Assert.IsTrue(Handlers.Contains("string.this[int].get"), "Missing handler for string indexer getter");
            Assert.IsTrue(Handlers.Contains("System.Enum.GetName<>()"), "Missing handler for Enum.GetName generic overload");
        }

        [TestMethod]
        public void Dsl_Should_Register_All_Handlers()
        {
            // This sanity check makes sure future refactors do not accidentally drop registrations.
            Assert.IsTrue(Handlers.Count >= 500, $"Expected at least 500 DSL handlers, but found {Handlers.Count}.");
        }
    }
}
