// Copyright (C) 2015-2026 The Neo Project.
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

        [TestMethod]
        public void Dsl_Should_Initialize_Without_Duplicate_Registrations()
        {
            Assert.IsTrue(Handlers.Contains("System.Math.Clamp(int, int, int)"));
            Assert.IsTrue(Handlers.Contains("string.Contains(string)"));
        }

        [TestMethod]
        public void Dsl_Should_Reject_Duplicate_Handler_Registration()
        {
            var methodConvertType = typeof(Program).Assembly.GetType("Neo.Compiler.MethodConvert", throwOnError: true)!;
            var addHandler = methodConvertType.GetMethod("AddHandler", BindingFlags.NonPublic | BindingFlags.Static)!;
            var key = "test.duplicate.handler";
            var arguments = new object?[] { key, null };

            addHandler.Invoke(null, arguments);
            var exception = Assert.ThrowsExactly<TargetInvocationException>(() => addHandler.Invoke(null, arguments));

            Assert.IsInstanceOfType<InvalidOperationException>(exception.InnerException);
            StringAssert.Contains(exception.InnerException!.Message, key);
        }

        [TestMethod]
        public void Dsl_Should_Reject_Conflicting_Handler_Alias()
        {
            var methodConvertType = typeof(Program).Assembly.GetType("Neo.Compiler.MethodConvert", throwOnError: true)!;
            var addAlias = methodConvertType.GetMethod("AddAlias", BindingFlags.NonPublic | BindingFlags.Static)!;
            var arguments = new object?[] { "string.Contains(string)", null };

            var exception = Assert.ThrowsExactly<TargetInvocationException>(() => addAlias.Invoke(null, arguments));

            Assert.IsInstanceOfType<InvalidOperationException>(exception.InnerException);
            StringAssert.Contains(exception.InnerException!.Message, "string.Contains(string)");
        }
    }
}
