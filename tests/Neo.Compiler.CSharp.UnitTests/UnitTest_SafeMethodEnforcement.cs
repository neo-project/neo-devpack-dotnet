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
        private const string SafeWriteCapableCallDiagnosticId = "NC3012";

        private const string Header = @"using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

public class Contract : SmartContract
{
";

        private const string ExternalContractDeclaration = @"

[Contract(""0xe7a98ee2c70b3024d5091d72c0a52bb71df4e322"")]
public static class ExternalContract
{
#pragma warning disable CS0626
    public static extern object Write();

    [Safe]
    public static extern object Read();

    [Safe]
    public static extern object Value { get; set; }
#pragma warning restore CS0626
}";

        private static string Compile(string body, string declarations = "") => Header + body + "\n}" + declarations;

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

        // --- NC3012: [Safe] methods must not make write-capable external contract calls. ---

        [TestMethod]
        public void SafeMethod_WriteCapableContractCall_FailsCompilation()
        {
            // CallFlags.All includes WriteStates, so the [Safe] read-only promise is false.
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get(UInt160 h) => Neo.SmartContract.Framework.Services.Contract.Call(h, ""m"", CallFlags.All, new object[] { 1 });"));

            Assert.IsFalse(context.Success, "A [Safe] method calling another contract with write flags must fail compilation.");
            Assert.IsTrue(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId && d.Severity == DiagnosticSeverity.Error),
                "Expected an NC3012 error for a Safe method that calls another contract with write flags.");
        }

        [TestMethod]
        public void SafeMethod_StatesContractCall_FailsCompilation()
        {
            // CallFlags.States == ReadStates | WriteStates.
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get(UInt160 h) => Neo.SmartContract.Framework.Services.Contract.Call(h, ""m"", CallFlags.States, new object[] { 1 });"));

            Assert.IsFalse(context.Success, "CallFlags.States carries WriteStates and must fail for a [Safe] method.");
            Assert.IsTrue(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId && d.Severity == DiagnosticSeverity.Error),
                "Expected an NC3012 error for CallFlags.States.");
        }

        [TestMethod]
        public void SafeMethod_WriteStatesContractCall_FailsCompilation()
        {
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get(UInt160 h) => Neo.SmartContract.Framework.Services.Contract.Call(h, ""m"", CallFlags.WriteStates, new object[] { 1 });"));

            Assert.IsFalse(context.Success, "CallFlags.WriteStates must fail for a [Safe] method.");
            Assert.IsTrue(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId && d.Severity == DiagnosticSeverity.Error),
                "Expected an NC3012 error for CallFlags.WriteStates.");
        }

        [TestMethod]
        public void SafeMethod_TransitiveWriteCapableCall_FailsCompilation()
        {
            // The write-capable call is hidden behind a private helper; the analysis must
            // follow the intra-contract call graph just like the storage-write check does.
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get(UInt160 h) => Forward(h);

    private static object Forward(UInt160 h) => Neo.SmartContract.Framework.Services.Contract.Call(h, ""m"", CallFlags.All, new object[] { 1 });"));

            Assert.IsFalse(context.Success, "A [Safe] method reaching a write-capable call transitively must fail compilation.");
            Assert.IsTrue(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId && d.Severity == DiagnosticSeverity.Error),
                "Expected an NC3012 error for a transitively reached write-capable call.");
        }

        [TestMethod]
        public void SafeMethod_WriteCapableCallToken_FailsCompilation()
        {
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get() => ExternalContract.Write();", ExternalContractDeclaration));

            Assert.IsFalse(context.Success, "A [Safe] method using a write-capable CALLT must fail compilation.");
            Assert.IsTrue(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId && d.Severity == DiagnosticSeverity.Error),
                "Expected an NC3012 error for a write-capable CALLT.");
        }

        [TestMethod]
        public void SafeMethod_ReadOnlyContractCall_Compiles()
        {
            // CallFlags.ReadOnly (ReadStates | AllowCall) has no WriteStates bit: this is fine.
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get(UInt160 h) => Neo.SmartContract.Framework.Services.Contract.Call(h, ""m"", CallFlags.ReadOnly, new object[] { 1 });"));

            Assert.IsTrue(context.Success,
                string.Join('\n', context.Diagnostics.Select(d => d.ToString())));
            Assert.IsFalse(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId),
                "A read-only Contract.Call must not produce an NC3012 diagnostic.");
        }

        [TestMethod]
        public void SafeMethod_UnknownNonWriteConstantContractCall_Compiles()
        {
            // This exercises the non-small integer constant path while preserving the no-WriteStates bit.
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get(UInt160 h) => Neo.SmartContract.Framework.Services.Contract.Call(h, ""m"", (CallFlags)32, new object[] { 1 });"));

            Assert.IsTrue(context.Success,
                string.Join('\n', context.Diagnostics.Select(d => d.ToString())));
            Assert.IsFalse(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId),
                "A constant without WriteStates must not produce an NC3012 diagnostic.");
        }

        [TestMethod]
        public void SafeMethod_DynamicContractCallFlags_FailsCompilation()
        {
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get(UInt160 h, CallFlags flags) => Neo.SmartContract.Framework.Services.Contract.Call(h, ""m"", flags, new object[] { 1 });"));

            Assert.IsFalse(context.Success, "Dynamic call flags may include WriteStates and must fail for a [Safe] method.");
            Assert.IsTrue(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId && d.Severity == DiagnosticSeverity.Error),
                "Expected an NC3012 error for dynamic call flags.");
        }

        [TestMethod]
        public void SafeMethod_DynamicContractCallFlags_NoInline_FailsCompilation()
        {
            CompilationOptions options = TestHelper.CreateDefaultOptions();
            options.NoInline = true;
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get(UInt160 h, CallFlags flags) => Neo.SmartContract.Framework.Services.Contract.Call(h, ""m"", flags, new object[] { 1 });"), options);

            Assert.IsFalse(context.Success, "Disabling inlining must not bypass dynamic call flag enforcement.");
            Assert.IsTrue(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId && d.Severity == DiagnosticSeverity.Error),
                "Expected an NC3012 error for dynamic call flags with inlining disabled.");
        }

        [TestMethod]
        public void SafeMethod_ComputedContractCallFlags_FailsCompilation()
        {
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get(UInt160 h, bool write)
    {
        CallFlags flags = CallFlags.ReadOnly;
        if (write) flags |= CallFlags.WriteStates;
        return Neo.SmartContract.Framework.Services.Contract.Call(h, ""m"", flags, new object[] { 1 });
    }"));

            Assert.IsFalse(context.Success, "Computed call flags may include WriteStates and must fail for a [Safe] method.");
            Assert.IsTrue(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId && d.Severity == DiagnosticSeverity.Error),
                "Expected an NC3012 error for computed call flags.");
        }

        [TestMethod]
        public void SafeMethod_NoneContractCall_Compiles()
        {
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get(UInt160 h) => Neo.SmartContract.Framework.Services.Contract.Call(h, ""m"", CallFlags.None, new object[] { 1 });"));

            Assert.IsTrue(context.Success,
                string.Join('\n', context.Diagnostics.Select(d => d.ToString())));
            Assert.IsFalse(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId),
                "A CallFlags.None Contract.Call must not produce an NC3012 diagnostic.");
        }

        [TestMethod]
        public void SafeMethod_ReadOnlyContractCall_NoInline_Compiles()
        {
            CompilationOptions options = TestHelper.CreateDefaultOptions();
            options.NoInline = true;
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get(UInt160 h) => Neo.SmartContract.Framework.Services.Contract.Call(h, ""m"", CallFlags.ReadOnly, new object[] { 1 });"), options);

            Assert.IsTrue(context.Success,
                string.Join('\n', context.Diagnostics.Select(d => d.ToString())));
            Assert.IsFalse(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId),
                "A read-only Contract.Call must remain valid with inlining disabled.");
        }

        [TestMethod]
        public void SafeMethod_ReadOnlyCallToken_Compiles()
        {
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get() => ExternalContract.Read();", ExternalContractDeclaration));

            Assert.IsTrue(context.Success,
                string.Join('\n', context.Diagnostics.Select(d => d.ToString())));
            Assert.IsFalse(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId),
                "A read-only CALLT must not produce an NC3012 diagnostic.");
            Assert.AreEqual(Neo.SmartContract.CallFlags.ReadOnly, context.CreateExecutable().Tokens.Single().CallFlags,
                "A [Safe] extern method must produce a read-only CALLT token.");
        }

        [TestMethod]
        public void SafeMethod_ReadOnlyNativeCallToken_Compiles()
        {
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static string Get(ByteString data) => Neo.SmartContract.Framework.Native.StdLib.HexEncode(data);"));

            Assert.IsTrue(context.Success,
                string.Join('\n', context.Diagnostics.Select(d => d.ToString())));
            Assert.AreEqual(Neo.SmartContract.CallFlags.ReadOnly, context.CreateExecutable().Tokens.Single().CallFlags,
                "A safe native method must produce a read-only CALLT token.");
        }

        [TestMethod]
        public void SafeMethod_CompilerGeneratedReadOnlyCallToken_Compiles()
        {
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static string Get(int value) => value.ToString();"));

            Assert.IsTrue(context.Success,
                string.Join('\n', context.Diagnostics.Select(d => d.ToString())));
            Assert.AreEqual(Neo.SmartContract.CallFlags.ReadOnly, context.CreateExecutable().Tokens.Single().CallFlags,
                "Compiler-generated read-only CALLT instructions must not carry WriteStates.");
        }

        [TestMethod]
        public void ContractCallToken_SafePropertyOnlyNarrowsGetter()
        {
            var context = TestHelper.CompileSingleContract(Compile(@"
    [Safe]
    public static object Get() => ExternalContract.Value;

    public static void Set(object value) => ExternalContract.Value = value;", ExternalContractDeclaration));

            Assert.IsTrue(context.Success,
                string.Join('\n', context.Diagnostics.Select(d => d.ToString())));
            var tokens = context.CreateExecutable().Tokens;
            Assert.AreEqual(Neo.SmartContract.CallFlags.ReadOnly, tokens.Single(t => t.Method == "value").CallFlags,
                "A [Safe] external property getter must use read-only flags.");
            Assert.AreEqual(Neo.SmartContract.CallFlags.All, tokens.Single(t => t.Method == "setValue").CallFlags,
                "A setter must not inherit the getter's [Safe] flags.");
        }

        [TestMethod]
        public void NonSafeMethod_WriteCapableContractCall_Compiles()
        {
            // Write-capable external calls are perfectly legal for non-Safe methods.
            var context = TestHelper.CompileSingleContract(Compile(@"
    public static object Get(UInt160 h) => Neo.SmartContract.Framework.Services.Contract.Call(h, ""m"", CallFlags.All, new object[] { 1 });"));

            Assert.IsTrue(context.Success,
                string.Join('\n', context.Diagnostics.Select(d => d.ToString())));
            Assert.IsFalse(
                context.Diagnostics.Any(d => d.Id == SafeWriteCapableCallDiagnosticId),
                "A non-Safe method making a write-capable call must not produce an NC3012 diagnostic.");
        }
    }
}
