// Copyright (C) 2015-2026 The Neo Project.
//
// AbiProbe.cs file belongs to the neo project and is free
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
/// Diagnostic helper that prints the ABI produced for a small sample contract.
/// </summary>
/// <remarks>
/// Used to validate the harness itself: it shows the exact method names, parameter
/// counts, and offsets the compiler emits, so that a behavioural mismatch can be
/// attributed to the compiler rather than to a mistake in the calling convention.
/// </remarks>
public static class AbiProbe
{
    /// <summary>
    /// Compiles a sample contract and prints its ABI.
    /// </summary>
    /// <param name="harness">The harness used for compilation.</param>
    public static void Run(CompilerHarness harness)
    {
        const string source = """
            using Neo.SmartContract.Framework;
            public class ProbeContract : SmartContract
            {
                public static int Add(int a, int b) { return a + b; }
                public static int Flow0(int a, int b) { return a * b; }
                public static string Text(string s) { return s + "x"; }
            }
            """;

        (FuzzOutcome outcome, CompilationContext? context, FuzzFinding? finding) = harness.Compile(source);

        Console.WriteLine($"outcome: {outcome}");
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

        foreach (Diagnostic diagnostic in context.Diagnostics)
        {
            Console.WriteLine($"diag: {diagnostic}");
        }

        Neo.SmartContract.Manifest.ContractManifest manifest = context.CreateManifest();
        foreach (Neo.SmartContract.Manifest.ContractMethodDescriptor method in manifest.Abi.Methods)
        {
            Console.WriteLine(
                $"method: name='{method.Name}' offset={method.Offset} params={method.Parameters.Length} " +
                $"ret={method.ReturnType} safe={method.Safe}");
        }
    }
}
