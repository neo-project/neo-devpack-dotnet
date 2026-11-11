// Copyright (C) 2015-2026 The Neo Project.
//
// BigIntegerRepro.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM.Types;
using System.Numerics;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Reproduces the BigInteger behaviour reported by the cross-language oracle.
/// </summary>
/// <remarks>
/// The oracle reported that a contract calling
/// <c>BigInteger.Add(BigInteger.Multiply(a, b), BigInteger.Remainder(a, b))</c> faults
/// in the Neo VM while the equivalent C# succeeds. This probe isolates the cause by
/// comparing three shapes:
/// <list type="number">
/// <item><description>a single, non-nested call (the shape the upstream test contract
/// uses, and therefore known to work),</description></item>
/// <item><description>two sequential calls,</description></item>
/// <item><description>the nested call the oracle flagged.</description></item>
/// </list>
/// If the third shape fails while the first two succeed, the defect is in the
/// compiler's argument preparation for nested static calls.
/// </remarks>
public static class BigIntegerRepro
{
    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <param name="harness">The harness used for compilation.</param>
    public static void Run(CompilerHarness harness)
    {
        const string source = """
            using System.Numerics;
            using Neo.SmartContract.Framework;

            public class BigIntContract : SmartContract
            {
                public static BigInteger Single(BigInteger a, BigInteger b)
                {
                    return BigInteger.Multiply(a, b);
                }

                public static BigInteger Sequential(BigInteger a, BigInteger b)
                {
                    BigInteger m = BigInteger.Multiply(a, b);
                    BigInteger r = BigInteger.Remainder(a, b);
                    return m;
                }

                public static BigInteger Nested(BigInteger a, BigInteger b)
                {
                    return BigInteger.Add(BigInteger.Multiply(a, b), BigInteger.Remainder(a, b));
                }

                public static BigInteger NestedOnly(BigInteger a, BigInteger b)
                {
                    return BigInteger.Add(a, b);
                }
            }
            """;

        (FuzzOutcome outcome, CompilationContext? context, FuzzFinding? finding) = harness.Compile(source);

        Console.WriteLine($"compile outcome: {outcome}");
        if (finding is not null)
        {
            Console.WriteLine($"finding: {finding.Detail}");
            return;
        }

        if (context is null)
        {
            Console.WriteLine("no compilation context");
            return;
        }

        foreach (Microsoft.CodeAnalysis.Diagnostic diagnostic in context.Diagnostics)
        {
            Console.WriteLine($"diag: {diagnostic}");
        }

        (NefFile nef, ContractManifest manifest, _) = context.CreateResults();
        var artifacts = new ArtifactSet(nef, manifest);

        var analyzer = new BehaviorDifferentialAnalyzer();

        foreach ((string method, BigInteger a, BigInteger b) in new[]
        {
            ("single", 2, 7),
            ("sequential", 2, 7),
            ("nested", 2, 7),
            ("nestedOnly", 2, 7),
        })
        {
            string abiName = method switch
            {
                "single" => "single",
                "sequential" => "sequential",
                "nested" => "nested",
                _ => "nestedOnly",
            };

            (bool ok, string? error, StackItem? value) =
                analyzer.ExecuteIsolated(artifacts, abiName, [a, b]);

            string rendered = ok ? BehaviorDifferentialAnalyzer.Render(value!) : $"FAULT({error})";
            Console.WriteLine($"  {method,-12} ({a}, {b}) => {rendered}");
        }
    }
}
