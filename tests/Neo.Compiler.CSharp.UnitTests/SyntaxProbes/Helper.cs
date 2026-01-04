// Copyright (C) 2015-2026 The Neo Project.
//
// Helper.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

internal static class Helper
{
    private static readonly Lock EngineLock = new();
    private static readonly Lazy<CompilationEngine> SharedEngine = new(() => new CompilationEngine(new CompilationOptions()
    {
        Debug = CompilationOptions.DebugType.Extended,
        CompilerVersion = "TestingEngine",
        Optimize = CompilationOptions.OptimizationType.All,
        Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable,
        SkipRestoreIfAssetsPresent = true
    }));
    private static readonly Lazy<CompilationSourceReferences> SharedReferences = new(() =>
    {
        var repoRoot = SyntaxProbeLoader.GetRepositoryRoot();
        var frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");
        return new CompilationSourceReferences
        {
            Projects = new[] { frameworkProject }
        };
    });

    internal static void TestCodeBlock(string codeBlock)
    {
        var source = BuildMethodBodySource(codeBlock);
        AssertCompilationResult(source, expectSuccess: true, "Expected snippet to compile successfully.");
    }

    internal static void AssertCompilationFails(string codeBlock, string message)
    {
        var source = BuildMethodBodySource(codeBlock);
        AssertCompilationResult(source, expectSuccess: false, message);
    }

    internal static void AssertClassCompilationFails(string classMembers, string message)
    {
        var source = BuildClassSource(classMembers);
        AssertCompilationResult(source, expectSuccess: false, message);
    }

    internal static void AssertClassCompilationSucceeds(string classMembers, string message)
    {
        var source = BuildClassSource(classMembers);
        AssertCompilationResult(source, expectSuccess: true, message);
    }

    internal static void AssertRawCompilationFails(string sourceCode, string message)
    {
        AssertCompilationResult(sourceCode, expectSuccess: false, message);
    }

    internal static void AssertProbe(SyntaxProbeCase probe)
    {
        var messageBuilder = new StringBuilder()
            .Append(probe.Version)
            .Append(':')
            .Append(probe.Id)
            .Append(" - ")
            .Append(probe.Title);

        if (!string.IsNullOrWhiteSpace(probe.Notes))
        {
            messageBuilder.Append(" (").Append(probe.Notes).Append(')');
        }

        var message = messageBuilder.ToString();
        var expectSuccess = probe.Status == SyntaxSupportStatus.Supported;
        var sourceCode = probe.Scope switch
        {
            SyntaxProbeScope.Method => BuildMethodBodySource(probe.Snippet),
            SyntaxProbeScope.Class => BuildClassSource(probe.Snippet),
            SyntaxProbeScope.File => BuildFileSource(probe.Snippet),
            _ => throw new ArgumentOutOfRangeException(nameof(probe.Scope), probe.Scope, "Unsupported scope for syntax probe.")
        };

        AssertCompilationResult(sourceCode, expectSuccess, message);
    }

    private static void AssertCompilationResult(string sourceCode, bool expectSuccess, string message)
    {
        CompilationContext? result = null;
        Exception? compileException = null;

        try
        {
            result = CompileSource(sourceCode);
        }
        catch (Exception ex)
        {
            compileException = ex;
        }

        if (compileException is not null)
        {
            if (expectSuccess)
            {
                const string redColor = "\u001b[31m";
                const string resetColor = "\u001b[0m";
                Console.WriteLine($"{redColor}Error compiling source :\n{sourceCode}{resetColor}");
                Console.WriteLine(compileException);
                Assert.Fail($"{message}{Environment.NewLine}Compilation failed unexpectedly: {compileException.Message}");
            }

            // Expected a failure and the compilation threw. Treat as success.
            return;
        }

        if (result is null)
        {
            Assert.Fail("Compilation result was null.");
            return;
        }

        if (result.Success == expectSuccess) return;

        var diagnostics = string.Join(Environment.NewLine, result.Diagnostics.Select(d => d.ToString()));
        const string failureColor = "\u001b[31m";
        const string resetColorFinal = "\u001b[0m";
        Console.WriteLine($"{failureColor}Error compiling source :\n{sourceCode}{resetColorFinal}");
        Console.WriteLine(diagnostics);

        if (expectSuccess)
        {
            Assert.Fail($"{message}{Environment.NewLine}Compilation failed unexpectedly.{Environment.NewLine}{diagnostics}");
        }
        else
        {
            Assert.Fail($"{message}{Environment.NewLine}Compilation succeeded but was expected to fail.");
        }
    }

    private static CompilationContext CompileSource(string sourceCode)
    {
        string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cs");
        File.WriteAllText(tempPath, sourceCode);

        try
        {
            lock (EngineLock)
            {
                return SharedEngine.Value.CompileSources(SharedReferences.Value, tempPath).First();
            }
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }

    private static string BuildMethodBodySource(string codeBlock)
    {
        return @"using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Text;
using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Linq;

namespace Neo.Compiler.CSharp.TestContracts;

public class CodeBlockTest : SmartContract.Framework.SmartContract
{
    public static void CodeBlock()
    {
" + Indent(codeBlock, 2) + @"
    }
}
";
    }

    private static string BuildClassSource(string classMembers)
    {
        return @"using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.Text;
using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Linq;

namespace Neo.Compiler.CSharp.TestContracts;

public class SyntaxProbe : SmartContract.Framework.SmartContract
{
" + Indent(classMembers, 1) + @"
}
";
    }

    private static string BuildFileSource(string source)
    {
        return source;
    }

    private static string Indent(string text, int level)
    {
        var indent = new string(' ', level * 4);
        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        return string.Join(Environment.NewLine, lines.Select(line => string.IsNullOrWhiteSpace(line) ? line : indent + line));
    }
}
