// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_CompilationException.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_CompilationException
    {
        [TestMethod]
        public void UnsupportedSyntax_ReportsLocationAndMessage()
        {
            var tree = CSharpSyntaxTree.ParseText(
                "class Contract { void Test() { goto label; label: return; } }",
                path: "Contract.cs");
            var gotoStatement = tree.GetRoot().DescendantNodes().OfType<GotoStatementSyntax>().First();

            var ex = CompilationException.UnsupportedSyntax(gotoStatement);

            StringAssert.Contains(ex.Message, "Unsupported GotoStatementSyntax syntax");
            StringAssert.Contains(ex.Message, "Contract.cs");
            StringAssert.Contains(ex.Message, "goto label;");
            Assert.AreEqual(ex.Message, ex.Diagnostic.GetMessage());
        }

        [TestMethod]
        public void FileOperation_PreservesInnerException()
        {
            var inner = new IOException("disk full");

            var ex = CompilationException.FileOperation("write", "/tmp/out.nef", innerException: inner);

            Assert.AreSame(inner, ex.InnerException);
            Assert.AreEqual(DiagnosticId.FileOperationFailed, ex.Diagnostic.Id);
            StringAssert.Contains(ex.Message, "disk full");
            Assert.AreEqual(ex.Message, ex.Diagnostic.GetMessage());
        }

        [TestMethod]
        public void FileOperation_AllowsCustomDiagnosticId()
        {
            var ex = CompilationException.FileOperation("write", "/tmp/out.nef", DiagnosticId.InvalidArgument);

            Assert.AreEqual(DiagnosticId.InvalidArgument, ex.Diagnostic.Id);
        }

        [TestMethod]
        public void MethodError_ToStringContainsEnhancedMessage()
        {
            var tree = CSharpSyntaxTree.ParseText(
                "class Contract { void M(int value) { } }",
                path: "Contract.cs");
            var methodDeclaration = tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().First();

            var ex = CompilationException.MethodError(methodDeclaration, "M", "signature mismatch", DiagnosticId.MethodNameConflict);

            StringAssert.Contains(ex.ToString(), "signature mismatch");
            Assert.AreEqual(ex.Message, ex.Diagnostic.GetMessage());
        }

        [TestMethod]
        public void UnexpectedError_ProvidesContextAndDiagnostic()
        {
            try
            {
                throw new InvalidOperationException("boom");
            }
            catch (Exception ex)
            {
                var compEx = CompilationException.Unexpected("emitting contract artifacts", ex);

                Assert.AreEqual(DiagnosticId.UnexpectedCompilerError, compEx.Diagnostic.Id);
                StringAssert.Contains(compEx.Message, "emitting contract artifacts");
                StringAssert.Contains(compEx.Message, nameof(InvalidOperationException));
                StringAssert.Contains(compEx.Message, "boom");
                StringAssert.Contains(compEx.Message, "compiler bug");
                Assert.AreSame(ex, compEx.InnerException);
            }
        }
    }
}
