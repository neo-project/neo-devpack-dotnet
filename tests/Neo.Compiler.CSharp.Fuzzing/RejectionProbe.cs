// Copyright (C) 2015-2026 The Neo Project.
//
// RejectionProbe.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Diagnostic helper that reports why generated sources are rejected.
/// </summary>
/// <remarks>
/// A fuzzer that spends most of its time on inputs the compiler rejects by design has
/// a low yield. This probe groups rejection reasons by diagnostic id so the generator
/// can be tuned towards constructs the compiler actually accepts.
/// </remarks>
public static class RejectionProbe
{
    /// <summary>
    /// Runs the probe over one sample per feature family.
    /// </summary>
    /// <param name="harness">The harness used for compilation.</param>
    /// <param name="generator">The generator producing the samples.</param>
    public static void Run(CompilerHarness harness, SourceGenerator generator)
    {
        foreach (FeatureFamily feature in Enum.GetValues<FeatureFamily>())
        {
            string source = generator.GenerateForFeature(feature);
            (FuzzOutcome outcome, CompilationContext? context, FuzzFinding? finding) = harness.Compile(source);

            Console.WriteLine($"--- {feature}: {outcome} ---");
            if (finding is not null)
            {
                Console.WriteLine($"    finding: {finding.Detail}");
            }

            if (context is null)
            {
                continue;
            }

            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                Console.WriteLine($"    {diagnostic.Id} [{diagnostic.Severity}] {diagnostic.GetMessage()}");
                if (diagnostic.Location.SourceSpan.Length > 0)
                {
                    Console.WriteLine($"      at {diagnostic.Location.SourceSpan}");
                }
            }
        }
    }
}
