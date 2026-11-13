// Copyright (C) 2015-2026 The Neo Project.
//
// UnitTest_DiagnosticHelpLinks.cs file belongs to the neo project and is free
// software: you can redistribute it and/or modify it under the terms of the
// MIT software license, see the accompanying file LICENSE in the main
// directory of the repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms, with or without
// modifications, are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neo.Compiler.CSharp.UnitTests;

[TestClass]
public class UnitTest_DiagnosticHelpLinks
{
    private const string SourceWithFloatingPoint = """
        using Neo.SmartContract.Framework;
        public class Contract : SmartContract
        {
            public static int Bad() => (int)default(float);
        }
        """;

    [TestMethod]
    public void CompilerDiagnosticsCarryHelpLinks()
    {
        var context = TestHelper.CompileSingleContract(SourceWithFloatingPoint);
        Assert.IsFalse(context.Success);

        var diagnostic = context.Diagnostics.FirstOrDefault(d => d.Id == DiagnosticId.FloatingPointNumber);
        Assert.IsNotNull(diagnostic, "expected a floating point diagnostic");
        Assert.IsNotNull(diagnostic.Descriptor.HelpLinkUri,
            "compiler diagnostics should carry a help link");
        StringAssert.EndsWith(diagnostic.Descriptor.HelpLinkUri, "docs/diagnostics/catalog.md#nc2004",
            $"unexpected help link '{diagnostic.Descriptor.HelpLinkUri}'");
    }

    [TestMethod]
    public void CompilerDiagnosticMessagesKeepLiteralBraces()
    {
        const string message = "Unsupported {0} token {literal}";

        var exception = new CompilationException(DiagnosticId.SyntaxNotSupported, message);

        Assert.AreEqual(message, exception.Diagnostic.GetMessage());
        StringAssert.EndsWith(exception.Diagnostic.Descriptor.HelpLinkUri, "#nc2001");
    }

    [TestMethod]
    public void CatalogCoversAllCompilerDiagnosticIds()
    {
        var catalog = File.ReadAllText(Path.Combine(FindRepositoryRoot(), "docs", "diagnostics", "catalog.md"));
        var anchors = CollectCatalogAnchors(catalog);

        foreach (var field in typeof(DiagnosticId).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var id = (string?)field.GetValue(null);
            Assert.IsNotNull(id, $"{field.Name} should have a value");
            StringAssert.Matches(id!, new System.Text.RegularExpressions.Regex(@"^NC\d{4}$"),
                $"{field.Name} should follow the NC#### pattern");
            Assert.IsTrue(anchors.Contains(id!.ToLowerInvariant()),
                $"diagnostic catalog has no entry for compiler diagnostic {id} ({field.Name})");
        }
    }

    private static HashSet<string> CollectCatalogAnchors(string catalog)
    {
        var anchors = new HashSet<string>();
        foreach (var line in catalog.Split('\n'))
        {
            var match = System.Text.RegularExpressions.Regex.Match(line, @"<a id=""(nc\d{4})""></a>");
            if (match.Success)
                anchors.Add(match.Groups[1].Value);
        }

        return anchors;
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "docs", "diagnostics", "catalog.md")))
                return directory.FullName;

            directory = directory.Parent!;
        }

        throw new DirectoryNotFoundException("Unable to locate the repository root containing docs/diagnostics/catalog.md.");
    }
}
