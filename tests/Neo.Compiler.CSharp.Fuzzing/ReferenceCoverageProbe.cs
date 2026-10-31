// Copyright (C) 2015-2026 The Neo Project.
//
// ReferenceCoverageProbe.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Reflection;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Checks that every generated feature family actually reaches the cross-language
/// oracle.
/// </summary>
/// <remarks>
/// <para>
/// When the plain C# reference fails to compile, the cross-language oracle skips the
/// input silently. That is the correct behaviour for a fuzzer, but it also means a
/// generator bug — a reference that names a type which does not exist in the reference
/// source, for example — silently disables the oracle for that whole family.
/// </para>
/// <para>
/// This probe compiles both halves of every generated pair and reports any family
/// whose reference does not compile, so that such a regression is caught rather than
/// quietly reducing coverage.
/// </para>
/// </remarks>
public static class ReferenceCoverageProbe
{
    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <param name="seed">Seed for the generator.</param>
    /// <returns>The number of families whose reference failed to compile.</returns>
    public static int Run(int seed)
    {
        var generator = new SourceGenerator(seed);
        using var oracle = new CrossLanguageOracle();

        int failures = 0;

        foreach (FeatureFamily feature in Enum.GetValues<FeatureFamily>())
        {
            (string contract, string reference) = generator.GeneratePair(feature);

            if (string.IsNullOrWhiteSpace(reference))
            {
                // Stateful families have no CLR equivalent by design; they are covered
                // by the optimizer-equivalence oracle instead.
                Console.WriteLine($"  [SKIP] {feature}: no plain C# equivalent exists (stateful family).");
                continue;
            }

            Assembly? assembly = oracle.TryCompileReference(reference);
            if (assembly is null)
            {
                Console.WriteLine($"  [FAIL] {feature}: the plain C# reference does not compile; " +
                                  "the cross-language oracle would skip this family silently.");
                failures++;
                continue;
            }

            Type? type = assembly.GetType("FuzzReference");
            if (type is null)
            {
                Console.WriteLine($"  [FAIL] {feature}: the reference type 'FuzzReference' is missing.");
                failures++;
                continue;
            }

            int methodCount = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic)
                .Count(m => m.Name is not ("get_" or "set_"));

            Console.WriteLine($"  [ OK ] {feature}: reference compiles, {methodCount} method(s).");
        }

        Console.WriteLine();
        Console.WriteLine(failures == 0
            ? "REFERENCE COVERAGE OK: every feature family reaches the cross-language oracle."
            : $"REFERENCE COVERAGE FAILED: {failures} family/families would be skipped by the cross-language oracle.");

        return failures;
    }
}
