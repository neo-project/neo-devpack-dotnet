// Copyright (C) 2015-2026 The Neo Project.
//
// CrossLanguageOracle.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Collections.Immutable;
using System.Reflection;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Compares the value a Neo contract returns with the value the equivalent plain C#
/// method returns.
/// </summary>
/// <remarks>
/// <para>
/// This is the strongest oracle in the system because the reference is the C# language
/// itself, not another compiler output. The same method body is compiled twice: once
/// by the Neo compiler and executed on the Neo VM, and once by Roslyn as an ordinary
/// .NET assembly and executed on the CLR. Any difference is a genuine semantic defect
/// in the Neo compiler, with no reference-implementation ambiguity to argue about.
/// </para>
/// <para>
/// The comparison is limited to methods whose bodies only use plain System types.
/// Neo-specific constructs (storage, native calls) have no CLR counterpart.
/// </para>
/// </remarks>
public sealed class CrossLanguageOracle : IDisposable
{
    private readonly ImmutableArray<MetadataReference> _references;
    private readonly Dictionary<string, Assembly> _assemblyCache = new(StringComparer.Ordinal);

    /// <summary>
    /// Creates an oracle with the default reference set.
    /// </summary>
    public CrossLanguageOracle()
    {
        _references = CreateReferences();
    }

    /// <summary>
    /// Compiles a plain C# source into an in-memory assembly.
    /// </summary>
    /// <remarks>
    /// Assemblies are cached by source text. The generator emits the same bodies
    /// repeatedly across iterations, and re-compiling and re-loading an assembly for
    /// every one of them dominates the runtime.
    /// </remarks>
    /// <param name="source">The C# source.</param>
    /// <returns>The compiled assembly, or <c>null</c> when the source does not compile.</returns>
    public Assembly? TryCompileReference(string source)
    {
        if (_assemblyCache.TryGetValue(source, out Assembly? cached))
        {
            return cached;
        }

        SyntaxTree tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest));
        CSharpCompilation compilation = CSharpCompilation.Create(
            "FuzzReferenceAssembly",
            [tree],
            _references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Debug));

        using var stream = new MemoryStream();
        EmitResult result = compilation.Emit(stream);

        if (!result.Success)
        {
            return null;
        }

        Assembly assembly = Assembly.Load(stream.ToArray());
        _assemblyCache[source] = assembly;
        return assembly;
    }

    /// <summary>
    /// Invokes a static method on a compiled reference assembly.
    /// </summary>
    /// <param name="assembly">The assembly to invoke from.</param>
    /// <param name="typeName">The type containing the method.</param>
    /// <param name="methodName">The method to invoke.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>The returned value, or a failure description.</returns>
    public (bool Ok, object? Value, string? Error) TryInvoke(Assembly assembly, string typeName, string methodName, object?[] args)
    {
        try
        {
            Type? type = assembly.GetType(typeName);
            if (type is null)
            {
                return (false, null, $"reference type '{typeName}' not found");
            }

            MethodInfo? method = type.GetMethod(
                methodName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);

            if (method is null)
            {
                return (false, null, $"reference method '{methodName}' not found");
            }

            return (true, method.Invoke(null, args), null);
        }
        catch (TargetInvocationException ex)
        {
            return (false, null, $"{ex.InnerException?.GetType().Name ?? "Exception"}: {ex.InnerException?.Message}");
        }
        catch (Exception ex)
        {
            return (false, null, $"{ex.GetType().Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Builds the metadata reference set for the plain C# compilation.
    /// </summary>
    private static ImmutableArray<MetadataReference> CreateReferences()
    {
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string tpa && !string.IsNullOrWhiteSpace(tpa))
        {
            foreach (string path in tpa.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                if (File.Exists(path))
                {
                    paths.Add(path);
                }
            }
        }

        return paths
            .OrderBy(static p => p, StringComparer.OrdinalIgnoreCase)
            .Select(static p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToImmutableArray();
    }

    /// <summary>
    /// Releases the loaded reference assemblies.
    /// </summary>
    public void Dispose()
    {
        _assemblyCache.Clear();
    }
}
