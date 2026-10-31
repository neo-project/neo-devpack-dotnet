// Copyright (C) 2015-2026 The Neo Project.
//
// CrossContractStructProbe.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Checks that a struct survives a call into another contract.
/// </summary>
/// <remarks>
/// A struct is an array on the VM, and the ABI exposes it as <c>Array</c>, so a cross-contract
/// call passes the field array as a single argument. The probe deploys both contracts and
/// verifies that the callee reads the fields in the order the caller wrote them.
/// </remarks>
public static class CrossContractStructProbe
{
    private const string Source = """
        using Neo.SmartContract.Framework;
        using Neo.SmartContract.Framework.Services;
        using Neo.SmartContract.Framework.Attributes;

        public class StructCallee : SmartContract
        {
            public struct Point
            {
                public int X;
                public int Y;
            }

            public static int Consume(Point point)
            {
                return point.X * 1000 + point.Y;
            }

            public static int Mixed(int a, string s, bool flag, byte[] data)
            {
                return a * 1000 + s.Length * 100 + (flag ? 10 : 0) + data[0];
            }

            public static int Pair(Point first, Point second)
            {
                return first.X * 1000 + second.Y;
            }
        }

        [ContractPermission("*", "*")]
        public class StructCaller : SmartContract
        {
            public struct Point
            {
                public int X;
                public int Y;
            }

            public static int Forward(UInt160 callee, int a, int b)
            {
                var point = new Point { X = a, Y = b };
                return (int)Contract.Call(callee, "consume", CallFlags.All, point);
            }

            public static int ForwardMixed(UInt160 callee, int a)
            {
                return (int)Contract.Call(callee, "mixed", CallFlags.All,
                    new object[] { a, "xy", true, new byte[] { 9 } });
            }

            public static int ForwardPair(UInt160 callee, int a, int b)
            {
                var first = new Point { X = a, Y = 0 };
                var second = new Point { X = 0, Y = b };
                return (int)Contract.Call(callee, "pair", CallFlags.All, first, second);
            }
        }
        """;

    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <returns>The number of failures; zero means the struct arrived intact.</returns>
    public static int Run()
    {
        using var harness = new CompilerHarness();
        (FuzzOutcome outcome, List<(string Name, CompilationContext Context)> contracts, FuzzFinding? finding) =
            harness.CompileAll(Source);

        if (outcome != FuzzOutcome.Clean)
        {
            Console.WriteLine($"[FAIL] compile outcome: {outcome} {finding?.Detail}");
            return 1;
        }

        var artifacts = new Dictionary<string, ArtifactSet>(StringComparer.Ordinal);
        foreach ((string name, CompilationContext context) in contracts)
        {
            Console.WriteLine($"  contract '{name}': methods={context.CreateManifest().Abi.Methods.Length}");
            artifacts[name] = new ArtifactSet(context.CreateExecutable(), context.CreateManifest());
        }

        Console.WriteLine($"  compiled contracts: [{string.Join(", ", artifacts.Keys)}]");

        if (!artifacts.TryGetValue("StructCallee", out ArtifactSet? callee) ||
            !artifacts.TryGetValue("StructCaller", out ArtifactSet? caller))
        {
            Console.WriteLine("[FAIL] both contracts are required.");
            return 1;
        }

        if (callee.Manifest.Abi.Methods.Single(m => m.Name == "consume").Parameters[0].Type
            != Neo.SmartContract.ContractParameterType.Array)
        {
            Console.WriteLine("[FAIL] the struct parameter is not exposed as Array.");
            return 1;
        }

        var bundle = new BehaviorDifferentialAnalyzer.ArtifactBundle(artifacts);
        using var analyzer = new BehaviorDifferentialAnalyzer();
        var reference = new BehaviorDifferentialAnalyzer.ContractReference("StructCallee");
        var calls = new List<(string Method, object?[] Args)>
        {
            ("forward", [reference, 7, 3]),
            ("forwardMixed", [reference, 7]),
            ("forwardPair", [reference, 7, 3]),
        };
        (List<(bool Ok, string? Error, Neo.VM.Types.StackItem? Value)> results, _, _) =
            analyzer.ExecuteBundleWithCoverage(bundle, "StructCaller", calls);
        string[] expected = ["7003", "7219", "7003"];
        int failures = 0;

        for (int i = 0; i < calls.Count; i++)
        {
            (bool ok, string? error, Neo.VM.Types.StackItem? value) = results[i];
            string actual = ok ? BehaviorDifferentialAnalyzer.Render(value!) : $"FAULT({error})";
            bool agrees = string.Equals(actual, expected[i], StringComparison.Ordinal);
            Console.WriteLine($"  {calls[i].Method,-14} => {actual,-10} expected {expected[i],-6} {(agrees ? "match" : "MISMATCH")}");
            if (!agrees)
                failures++;
        }

        Console.WriteLine();
        Console.WriteLine(failures == 0
            ? "CROSS-CONTRACT STRUCT PROBE PASSED: arguments arrive in order."
            : $"CROSS-CONTRACT STRUCT PROBE: {failures} call(s) disagree.");

        return failures;
    }
}
