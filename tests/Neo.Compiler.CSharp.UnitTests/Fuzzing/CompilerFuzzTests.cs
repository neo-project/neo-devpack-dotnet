// Copyright (C) 2015-2026 The Neo Project.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Syntax;
using System;

namespace Neo.Compiler.CSharp.UnitTests.Fuzzing;

/// <summary>
/// Deterministic compiler fuzz targets. A fixed seed makes failures reproducible in CI,
/// while the generated corpus exercises different expression and control-flow shapes.
/// </summary>
[TestClass]
public class CompilerFuzzTests
{
    [TestMethod]
    public void GeneratedExpressionsCompileWithoutRawCompilerFailures()
    {
        for (var seed = 0; seed < 32; seed++)
        {
            var random = new Random(seed);
            Neo.Compiler.CSharp.UnitTests.Syntax.Helper.TestCodeBlock(BuildMethodBody(random));
        }
    }

    private static string BuildMethodBody(Random random)
    {
        string expression = BuildExpression(random, 3);
        int initialValue = random.Next(-32, 33);
        return $"int value = {initialValue}; int generated = {expression}; "
            + "if (generated > value) { value += generated; } "
            + "else { value -= generated; } _ = value;";
    }

    private static string BuildExpression(Random random, int depth)
    {
        if (depth == 0) return random.Next(-16, 17).ToString();

        string left = BuildExpression(random, depth - 1);
        string op = random.Next(4) switch
        {
            0 => "+",
            1 => "-",
            2 => "*",
            _ => "%"
        };
        // Keep the generated corpus compilable: Roslyn rejects a constant modulo
        // by zero before the Neo compiler gets a chance to process the method.
        string right = op == "%"
            ? random.Next(1, 17).ToString()
            : BuildExpression(random, depth - 1);
        return $"({left} {op} {right})";
    }
}
