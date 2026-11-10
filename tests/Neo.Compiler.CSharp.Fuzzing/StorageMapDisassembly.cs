// Copyright (C) 2015-2026 The Neo Project.
//
// StorageMapDisassembly.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Prints the generated instructions for a minimal <c>StorageMap</c> construction.
/// </summary>
/// <remarks>
/// <c>StorageMap</c> declares its constructors as <c>extern</c>. The compiler decides
/// whether to emit an object-creation sequence with
/// <c>!type.DeclaringSyntaxReferences.IsEmpty &amp;&amp; !constructor.IsExtern</c>, so for an
/// extern constructor it skips creation while still emitting the constructor call. The
/// disassembly shows whether the call therefore runs against an object that was never
/// pushed.
/// </remarks>
public static class StorageMapDisassembly
{
    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <returns>Zero; this probe reports rather than asserts.</returns>
    public static int Run()
    {
        const string source = """
            using System;
            using System.Numerics;
            using Neo.SmartContract.Framework;
            using Neo.SmartContract.Framework.Services;

            public class MapProbe : SmartContract
            {
                public static int Construct(int a)
                {
                    var map = new StorageMap(Storage.CurrentContext, new byte[] { 0x02 });
                    return a;
                }

                public static int Baseline(int a)
                {
                    Storage.Put(Storage.CurrentContext, new byte[] { 0x01 }, a);
                    return a;
                }
            }
            """;

        using var harness = new CompilerHarness();
        (FuzzOutcome outcome, CompilationContext? context, FuzzFinding? finding) = harness.Compile(source);

        Console.WriteLine($"compile outcome: {outcome}");
        if (finding is not null)
        {
            Console.WriteLine($"finding: {finding.Detail}");
        }

        if (context is null)
        {
            return 1;
        }

        Console.WriteLine();
        Console.WriteLine("=== generated instructions ===");
        Console.WriteLine(context.CreateAssembly());

        return 0;
    }
}
