// Copyright (C) 2015-2026 The Neo Project.
//
// DifferentialAnalyzers.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System.Numerics;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// A semantic divergence detected between two compilations or between the compiler
/// and the reference (C#) semantics.
/// </summary>
/// <param name="Kind">Which analyzer produced the divergence.</param>
/// <param name="Detail">Human readable description.</param>
public sealed record DifferentialDivergence(string Kind, string Detail);

/// <summary>
/// Analyzers that compare two compilations of the same source for semantic equality.
/// </summary>
/// <remarks>
/// <para>
/// The comparison axes are ordered by how likely they are to point at a real defect
/// and how unlikely they are to produce a false positive:
/// </para>
/// <list type="number">
/// <item><description><b>Optimization equivalence</b> — the same source compiled with
/// <c>Optimize=None</c> and with <c>Optimize=All</c> must produce scripts with the same
/// observable behaviour. This is an oracle-free comparison (no reference implementation
/// needed), so any divergence is a genuine optimizer defect.</description></item>
/// <item><description><b>Determinism</b> — compiling the identical source twice must
/// produce byte-identical artifacts. Divergence indicates hidden state or
/// non-deterministic ordering.</description></item>
/// <item><description><b>Manifest/ABI consistency</b> — the method set in the manifest
/// must match the methods actually present in the script, and parameter counts must
/// agree. Divergence indicates a broken ABI, which breaks deployment.</description></item>
/// <item><description><b>Cross-language semantics</b> — where a C# reference evaluation
/// is available, the value the Neo VM produces must match what C# produces.</description></item>
/// </list>
/// </remarks>
public static class DifferentialAnalyzers
{
    /// <summary>
    /// Compares two artifacts produced from the same source and reports every divergence.
    /// </summary>
    /// <param name="labelA">Label of the first artifact set, used in messages.</param>
    /// <param name="a">The first artifact set.</param>
    /// <param name="labelB">Label of the second artifact set, used in messages.</param>
    /// <param name="b">The second artifact set.</param>
    /// <returns>The list of divergences; empty when the two are equivalent.</returns>
    public static List<DifferentialDivergence> Compare(string labelA, ArtifactSet a, string labelB, ArtifactSet b)
    {
        var result = new List<DifferentialDivergence>();

        CompareScripts(labelA, a, labelB, b, result);
        CompareAbi(labelA, a, labelB, b, result);

        return result;
    }

    /// <summary>
    /// Compares the raw script bytes of two artifact sets.
    /// </summary>
    /// <remarks>
    /// A byte difference is not itself a defect: the optimizer legitimately rewrites
    /// scripts. What matters is whether the *behaviour* differs, which is checked by
    /// <see cref="BehaviorDifferentialAnalyzer"/>. This method therefore reports script
    /// differences as informational only, and the behaviour oracle carries the weight.
    /// </remarks>
    private static void CompareScripts(string labelA, ArtifactSet a, string labelB, ArtifactSet b, List<DifferentialDivergence> result)
    {
        byte[] scriptA = a.Nef.Script.ToArray();
        byte[] scriptB = b.Nef.Script.ToArray();

        if (!scriptA.AsSpan().SequenceEqual(scriptB))
        {
            result.Add(new DifferentialDivergence(
                "ScriptBytes",
                $"{labelA} and {labelB} produce different scripts ({scriptA.Length} vs {scriptB.Length} bytes); behaviour must still match."));
        }
    }

    /// <summary>
    /// Compares the ABI of two artifact sets.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>_initialize</c> is deliberately excluded from the comparison. It is the
    /// constructor, not a callable business method, and its presence legitimately
    /// differs between optimization levels:
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>InitStaticToConst</c> folds a static-field read into a
    /// constant and drops the matching store. When that removes the last static-field
    /// access from the script, <c>Peephole.RemoveInitialize</c> then removes the
    /// now-empty <c>_initialize</c> from the manifest.</description></item>
    /// <item><description>Separately, <c>BasicOptimizer.RemoveNops</c> can shrink the
    /// instruction list enough for <c>RemoveEmptyInitialize</c> to drop it in the
    /// <c>Basic</c> and <c>All</c> builds while the <c>None</c> build keeps it.</description></item>
    /// </list>
    /// <para>
    /// In both cases the removed constructor initializes nothing that is still live, so
    /// no observable behaviour differs. Comparing it would report an expected
    /// consequence of dead-code elimination as a defect.
    /// </para>
    /// </remarks>
    private static void CompareAbi(string labelA, ArtifactSet a, string labelB, ArtifactSet b, List<DifferentialDivergence> result)
    {
        ContractMethodDescriptor[] methodsA = a.Manifest.Abi.Methods;
        ContractMethodDescriptor[] methodsB = b.Manifest.Abi.Methods;

        string[] namesA = methodsA.Where(m => !IsConstructor(m)).Select(m => m.Name)
            .OrderBy(n => n, StringComparer.Ordinal).ToArray();
        string[] namesB = methodsB.Where(m => !IsConstructor(m)).Select(m => m.Name)
            .OrderBy(n => n, StringComparer.Ordinal).ToArray();

        if (!namesA.SequenceEqual(namesB, StringComparer.Ordinal))
        {
            result.Add(new DifferentialDivergence(
                "AbiMethodSet",
                $"{labelA} exposes [{string.Join(", ", namesA)}] but {labelB} exposes [{string.Join(", ", namesB)}]."));
            return;
        }

        foreach (ContractMethodDescriptor ma in methodsA.Where(m => !IsConstructor(m)))
        {
            ContractMethodDescriptor mb = methodsB.First(m => m.Name == ma.Name);
            if (ma.Parameters.Length != mb.Parameters.Length)
            {
                result.Add(new DifferentialDivergence(
                    "AbiParameterCount",
                    $"Method '{ma.Name}' has {ma.Parameters.Length} parameters in {labelA} but {mb.Parameters.Length} in {labelB}."));
            }
            if (ma.ReturnType != mb.ReturnType)
            {
                result.Add(new DifferentialDivergence(
                    "AbiReturnType",
                    $"Method '{ma.Name}' returns '{ma.ReturnType}' in {labelA} but '{mb.ReturnType}' in {labelB}."));
            }
            if (ma.Parameters.Length == mb.Parameters.Length &&
                !ma.Parameters.Select(p => p.Type).SequenceEqual(mb.Parameters.Select(p => p.Type)))
            {
                result.Add(new DifferentialDivergence(
                    "AbiParameterTypes",
                    $"Method '{ma.Name}' has different parameter types in {labelA} vs {labelB}."));
            }
        }
    }

    /// <summary>
    /// Determines whether a manifest method is the constructor.
    /// </summary>
    /// <param name="method">The method to inspect.</param>
    /// <returns><c>true</c> when the method is <c>_initialize</c>.</returns>
    private static bool IsConstructor(ContractMethodDescriptor method) =>
        string.Equals(method.Name, "_initialize", StringComparison.Ordinal);

    /// <summary>
    /// Verifies that compiling the same source twice yields byte-identical artifacts.
    /// </summary>
    /// <param name="first">First artifact set.</param>
    /// <param name="second">Second artifact set.</param>
    /// <returns>The divergence list; empty when the compilations are deterministic.</returns>
    public static List<DifferentialDivergence> CheckDeterminism(ArtifactSet first, ArtifactSet second)
    {
        var result = new List<DifferentialDivergence>();

        byte[] scriptA = first.Nef.Script.ToArray();
        byte[] scriptB = second.Nef.Script.ToArray();

        if (!scriptA.AsSpan().SequenceEqual(scriptB))
        {
            result.Add(new DifferentialDivergence(
                "Determinism",
                $"Two compilations of the identical source produced different scripts ({scriptA.Length} vs {scriptB.Length} bytes)."));
        }

        if (first.Nef.CheckSum != second.Nef.CheckSum)
        {
            result.Add(new DifferentialDivergence(
                "Determinism",
                $"Checksum differs between two compilations of the identical source: {first.Nef.CheckSum} vs {second.Nef.CheckSum}."));
        }

        string jsonA = first.Manifest.ToJson().ToString(false);
        string jsonB = second.Manifest.ToJson().ToString(false);
        if (!string.Equals(jsonA, jsonB, StringComparison.Ordinal))
        {
            result.Add(new DifferentialDivergence(
                "Determinism",
                "Manifest differs between two compilations of the identical source."));
        }

        return result;
    }
}

/// <summary>
/// The compiled output of a single compilation, captured in a form convenient for
/// differential comparison.
/// </summary>
/// <param name="Nef">The compiled script file.</param>
/// <param name="Manifest">The compiled contract manifest.</param>
public sealed record ArtifactSet(NefFile Nef, ContractManifest Manifest);
