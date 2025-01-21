// Copyright (C) 2015-2025 The Neo Project.
//
// Helper.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Akka.Util.Internal;
using System;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.Syntax;

internal static class Helper
{
    internal static void TestCodeBlock(string codeBlock)
    {
        var result = new CompilationEngine(new CompilationOptions()
        {
            Debug = CompilationOptions.DebugType.Extended,
            CompilerVersion = "TestingEngine",
            Optimize = CompilationOptions.OptimizationType.All,
            Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable
        }).CompileFromCodeBlock(codeBlock).First();
        if (result.Success) return;

        result.Diagnostics.ForEach(Console.WriteLine);
        const string redColor = "\u001b[31m";
        const string resetColor = "\u001b[0m";
        Console.WriteLine($"{redColor}Error compiling code block : {{\n\t{codeBlock.Replace("\n", "\n\t")}\n}}{resetColor}");
    }
}
