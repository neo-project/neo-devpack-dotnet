// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_StaticSlotLimit.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_StaticSlotLimit
{
    [DataRow(255, true)]
    [DataRow(256, false)]
    [TestMethod]
    public void StaticFieldsRespectSlotLimit(int fieldCount, bool expectedSuccess)
    {
        var source = new StringBuilder("using Neo.SmartContract.Framework; public class Contract : SmartContract { ");
        for (int i = 0; i < fieldCount; i++)
            source.Append("public static int Field").Append(i).Append(" = ").Append(i).Append(';');
        source.Append("public static int Test() => Field0; }");

        string path = Path.Combine(Path.GetTempPath(), $"neo-static-slots-{Guid.NewGuid():N}.cs");
        File.WriteAllText(path, source.ToString());
        try
        {
            string framework = Path.Combine(SyntaxProbeLoader.GetRepositoryRoot(),
                "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");
            var references = new CompilationSourceReferences { Projects = [framework] };
            var context = new CompilationEngine(new CompilationOptions
            {
                SkipRestoreIfAssetsPresent = true
            }).CompileSources(references, path).Single();
            string diagnostics = string.Join(Environment.NewLine, context.Diagnostics);

            Assert.AreEqual(expectedSuccess, context.Success, diagnostics);
            if (!expectedSuccess)
            {
                var diagnostic = context.Diagnostics.Single(d => d.Id == DiagnosticId.StaticSlotLimit);
                Assert.AreEqual(DiagnosticSeverity.Error, diagnostic.Severity);
                Assert.IsFalse(context.Diagnostics.Any(d => d.Id == DiagnosticId.UnexpectedCompilerError), diagnostics);
            }
        }
        finally
        {
            File.Delete(path);
        }
    }
}
