// Copyright (C) 2015-2026 The Neo Project.
//
// InitializeAbiProbe.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Compiler;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Investigates whether the ABI differs between optimization levels.
/// </summary>
/// <remarks>
/// <para>
/// Two independent mechanisms remove <c>_initialize</c> from the manifest:
/// </para>
/// <list type="bullet">
/// <item><description><c>CompilationContext.RemoveEmptyInitialize</c> removes it when the
/// method has at most one instruction. This runs for every build. The instruction count
/// depends on the optimization level, because <c>BasicOptimizer.RemoveNops</c> shrinks
/// the instruction list when the <c>Basic</c> flag is set.</description></item>
/// <item><description><c>Peephole.RemoveInitialize</c> removes it from the manifest when
/// the method starts with <c>INITSSLOT</c>, that instruction is not a jump target, the
/// whole script contains no static-field load or store, and the instruction after
/// <c>INITSSLOT</c> is a <c>RET</c>. This runs only in the experimental optimizer.</description></item>
/// </list>
/// <para>
/// This probe compares the ABI and the script across the three optimization levels for
/// several static-initialization shapes. The high-severity question is whether a
/// contract that <i>does</i> have static fields can lose its <c>_initialize</c>, which
/// would leave those fields uninitialized.
/// </para>
/// </remarks>
public static class InitializeAbiProbe
{
    private static readonly (string Label, string Source)[] Cases =
    [
        ("empty static ctor, no static fields",
            """
            using Neo.SmartContract.Framework;
            public class CaseContract : SmartContract
            {
                static CaseContract() { }
                public static int Compute(int a, int b) { return a + b; }
            }
            """),

        ("static field with initializer, no static ctor",
            """
            using Neo.SmartContract.Framework;
            public class CaseContract : SmartContract
            {
                private static int counter = 7;
                public static int Compute(int a, int b) { return a + b + counter; }
            }
            """),

        ("static field with initializer AND static ctor",
            """
            using Neo.SmartContract.Framework;
            public class CaseContract : SmartContract
            {
                private static int counter = 7;
                static CaseContract() { counter = counter + 1; }
                public static int Compute(int a, int b) { return a + b + counter; }
            }
            """),

        ("two static fields with initializers",
            """
            using Neo.SmartContract.Framework;
            public class CaseContract : SmartContract
            {
                private static int counter = 7;
                private static int other = 9;
                public static int Compute(int a, int b) { return a + b + counter + other; }
            }
            """),

        ("static field initialized from a constant expression",
            """
            using Neo.SmartContract.Framework;
            public class CaseContract : SmartContract
            {
                private static int counter = 3 * 4 + 1;
                public static int Compute(int a, int b) { return a + b + counter; }
            }
            """),
    ];

    /// <summary>
    /// Runs the probe.
    /// </summary>
    public static void Run()
    {
        foreach ((string label, string source) in Cases)
        {
            Console.WriteLine($"=== {label} ===");

            string? baseline = null;

            foreach (CompilationOptions.OptimizationType optimization in new[]
            {
                CompilationOptions.OptimizationType.None,
                CompilationOptions.OptimizationType.Basic,
                CompilationOptions.OptimizationType.All,
            })
            {
                CompilationOptions options = CompilerHarness.CreateDefaultOptions();
                options.Optimize = optimization;

                using var harness = new CompilerHarness(options);
                (FuzzOutcome outcome, CompilationContext? context, FuzzFinding? finding) = harness.Compile(source);

                if (outcome != FuzzOutcome.Clean || context is null)
                {
                    Console.WriteLine($"  [{optimization}] outcome={outcome} {finding?.Detail}");
                    continue;
                }

                (Neo.SmartContract.NefFile nef, Neo.SmartContract.Manifest.ContractManifest manifest, _) = context.CreateResults();

                string[] names = manifest.Abi.Methods.Select(m => m.Name).OrderBy(n => n, StringComparer.Ordinal).ToArray();
                string summary = $"[{string.Join(", ", names)}] script={nef.Script.Length}B";
                Console.WriteLine($"  [{optimization,-5}] {summary}");

                baseline ??= summary;
            }

            Console.WriteLine();
        }
    }
}
