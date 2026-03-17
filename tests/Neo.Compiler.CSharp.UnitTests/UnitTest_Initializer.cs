// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_Initializer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System;
using System.Collections.Generic;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Initializer : DebugAndTestBase<Contract_Initializer>
    {
        [TestMethod]
        public void Initializer_Test()
        {
            Assert.AreEqual(3, Contract.Sum());
            AssertGasConsumed(1052100);
            Assert.AreEqual(12, Contract.Sum1(5, 7));
            AssertGasConsumed(1113210);
            Assert.AreEqual(12, Contract.Sum2(5, 7));
            AssertGasConsumed(1605330);
        }

        [TestMethod]
        public void AnonymousObjectCreation_LogsAnonymousMemberValues()
        {
            var logs = new Queue<string>();
            TestEngine.OnRuntimeLogDelegate handler = (sender, log) => logs.Enqueue(log);
            Contract.OnRuntimeLog += handler;

            try
            {
                Contract.AnonymousObjectCreation();
            }
            finally
            {
                Contract.OnRuntimeLog -= handler;
            }

            Assert.AreEqual(2, logs.Count);
            Assert.AreEqual("Hello", logs.Dequeue());
            Assert.AreEqual("apple", logs.Dequeue());
        }

        [TestMethod]
        public void AnonymousObjectCreation_CompilesAnonymousGetterHelpersWithValidIndexes()
        {
            var context = TestCleanup.EnsureArtifactUpToDateInternal(nameof(Contract_Initializer));
            Assert.IsNotNull(context, "Failed to compile Contract_Initializer for inspection.");

            var assembly = context!.CreateAssembly().Replace("\r\n", "\n", StringComparison.Ordinal);
            var anonymousObjectCreation = ExtractMethodBlock(
                assembly,
                "Neo.Compiler.CSharp.TestContracts.Contract_Initializer.anonymousObjectCreation()"
            );
            var messageGetter = ExtractMethodBlock(
                assembly,
                "<anonymous type: int Amount, string Message>.Message.get"
            );
            var nameGetter = ExtractMethodBlock(
                assembly,
                "<anonymous type: string name, int diam>.name.get"
            );

            StringAssert.Contains(anonymousObjectCreation, "LDLOC 0");
            StringAssert.Contains(anonymousObjectCreation, "CALL <");
            StringAssert.Contains(anonymousObjectCreation, "SYSCALL System.Runtime.Log");
            Assert.IsFalse(
                anonymousObjectCreation.Contains("STLOC 0\n000000de: SYSCALL System.Runtime.Log", StringComparison.Ordinal),
                $"anonymousObjectCreation should load/call getter before logging:\n{anonymousObjectCreation}"
            );

            StringAssert.Contains(messageGetter, "PUSH 1");
            StringAssert.Contains(messageGetter, "PICKITEM");
            Assert.IsFalse(
                messageGetter.Contains("PUSH -1", StringComparison.Ordinal),
                $"Message getter should not index with -1:\n{messageGetter}"
            );

            StringAssert.Contains(nameGetter, "PUSH 0");
            StringAssert.Contains(nameGetter, "PICKITEM");
            Assert.IsFalse(
                nameGetter.Contains("PUSH -1", StringComparison.Ordinal),
                $"name getter should not index with -1:\n{nameGetter}"
            );
        }

        private static string ExtractMethodBlock(string assembly, string methodSignature)
        {
            var marker = $"// {methodSignature}";
            var start = assembly.IndexOf(marker, StringComparison.Ordinal);
            Assert.IsTrue(start >= 0, $"Method section '{methodSignature}' was not found in generated assembly.\n{assembly}");

            var next = assembly.IndexOf("\n// ", start + marker.Length, StringComparison.Ordinal);
            if (next < 0)
                next = assembly.Length;

            return assembly[start..next];
        }
    }
}
