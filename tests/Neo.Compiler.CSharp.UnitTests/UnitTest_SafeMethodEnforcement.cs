// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_SafeMethodEnforcement.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_SafeMethodEnforcement
    {
        private const string SafeMutationDiagnosticId = "NC3011";

        private const string Header = @"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

public class Contract : SmartContract
{
";

        private static string Compile(string body) => Header + body + "\n}";

        [TestMethod]
        public void SafeMethod_DirectStorageWrite_FailsCompilation()
        {
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static void Get() => Storage.Put(Storage.CurrentContext, ""k"", ""v"");"));

            Assert.IsFalse(context.Success, "A [Safe] method that writes storage must fail compilation.");
            Assert.IsTrue(
                context.Diagnostics.Any(d => d.Id == SafeMutationDiagnosticId && d.Severity == DiagnosticSeverity.Error),
                "Expected an NC3011 error for a Safe method that mutates state.");
        }

        [TestMethod]
        public void SafeMethod_TransitiveStorageWrite_FailsCompilation()
        {
            // The write is hidden behind a private helper: the check must follow the
            // intra-contract call graph, not just scan the Safe method's own body.
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static void Get() => Write();

    private static void Write() => Storage.Put(Storage.CurrentContext, ""k"", ""v"");"));

            Assert.IsFalse(context.Success, "A [Safe] method that writes storage transitively must fail compilation.");
            Assert.IsTrue(
                context.Diagnostics.Any(d => d.Id == SafeMutationDiagnosticId && d.Severity == DiagnosticSeverity.Error),
                "Expected an NC3011 error for a Safe method that reaches a write through a helper.");
        }

        [TestMethod]
        public void SafeMethod_StorageDelete_FailsCompilation()
        {
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static void Get() => Storage.Delete(Storage.CurrentContext, ""k"");"));

            Assert.IsFalse(context.Success, "A [Safe] method that deletes storage must fail compilation.");
            Assert.IsTrue(
                context.Diagnostics.Any(d => d.Id == SafeMutationDiagnosticId && d.Severity == DiagnosticSeverity.Error),
                "Expected an NC3011 error for a Safe method that deletes state.");
        }

        [TestMethod]
        public void SafeMethod_ReadOnly_Compiles()
        {
            // Reading storage and emitting an event are not state mutations.
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static ByteString Get() => Storage.Get(Storage.CurrentContext, ""k"");"));

            Assert.IsTrue(context.Success,
                string.Join('\n', context.Diagnostics.Select(d => d.ToString())));
            Assert.IsFalse(
                context.Diagnostics.Any(d => d.Id == SafeMutationDiagnosticId),
                "A read-only Safe method must not produce an NC3011 diagnostic.");
        }

        [TestMethod]
        public void NonSafeMethod_StorageWrite_Compiles()
        {
            // The same write is perfectly legal when the method is not advertised as Safe.
            var context = TestHelper.CompileSingleContract(Compile(@"
    public static void Put() => Storage.Put(Storage.CurrentContext, ""k"", ""v"");"));

            Assert.IsTrue(context.Success,
                string.Join('\n', context.Diagnostics.Select(d => d.ToString())));
            Assert.IsFalse(
                context.Diagnostics.Any(d => d.Id == SafeMutationDiagnosticId),
                "A non-Safe method writing storage must not produce an NC3011 diagnostic.");
        }
    }
}
