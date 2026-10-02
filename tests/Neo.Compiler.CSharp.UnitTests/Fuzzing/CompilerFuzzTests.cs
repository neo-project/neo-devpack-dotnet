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

    [TestMethod]
    public void GeneratedControlFlowAndDispatchCompileWithoutRawCompilerFailures()
    {
        for (var seed = 0; seed < 32; seed++)
        {
            var random = new Random(seed);
            Neo.Compiler.CSharp.UnitTests.Syntax.Helper.TestCodeBlock(BuildControlFlowBody(random));
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

    private static string BuildControlFlowBody(Random random)
    {
        int initialValue = random.Next(-8, 9);
        int loopLimit = random.Next(2, 8);
        int step = random.Next(1, 4);
        int target = random.Next(4, 12);
        int lowerBound = random.Next(-8, -1);
        int firstItem = random.Next(-4, 5);
        int secondItem = random.Next(-4, 5);

        return $$"""
            int value = {{initialValue}};
            int total = 0;
            for (int index = 0; index < {{loopLimit}}; index++)
            {
                if ((index & 1) == 0) total += index;
                else total -= index;
            }
            foreach (int item in new[] { {{firstItem}}, {{secondItem}} })
            {
                total += item;
            }
            switch (total % 3)
            {
                case 0:
                    value += total;
                    break;
                case 1:
                    value -= total;
                    break;
                default:
                    value += total * 2;
                    break;
            }
            while (value < {{target}}) value += {{step}};
            do value--; while (value > {{lowerBound}});
            _ = value;
            """;
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
