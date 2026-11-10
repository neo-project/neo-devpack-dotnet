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
using System.Runtime.Loader;

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
    private const int DefaultCacheCapacity = 64;
    private const string HarnessErrorPrefix = "HarnessError: ";

    private readonly ImmutableArray<MetadataReference> _references;
    private readonly Dictionary<string, CachedAssembly> _assemblyCache = new(StringComparer.Ordinal);
    private readonly Dictionary<Assembly, byte[]> _assemblyImages = [];
    private readonly LinkedList<string> _cacheOrder = [];
    private readonly int _cacheCapacity;
    private bool _disposed;

    /// <summary>
    /// Creates an oracle with the default reference set.
    /// </summary>
    /// <param name="cacheCapacity">Maximum number of reference assemblies kept loaded.</param>
    public CrossLanguageOracle(int cacheCapacity = DefaultCacheCapacity)
    {
        if (cacheCapacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cacheCapacity));
        }

        _references = CreateReferences();
        _cacheCapacity = cacheCapacity;
    }

    /// <summary>The number of reference assemblies currently retained by the bounded cache.</summary>
    public int CachedAssemblyCount => _assemblyCache.Count;

    /// <summary>The number of cache entries evicted and marked for unloading.</summary>
    public long EvictedAssemblyCount { get; private set; }

    private sealed record CachedAssembly(
        Assembly Assembly,
        AssemblyLoadContext LoadContext,
        LinkedListNode<string> CacheNode);

    /// <summary>
    /// Compiles a plain C# source into an in-memory assembly.
    /// </summary>
    /// <remarks>
    /// Assemblies are cached by source text in a bounded LRU. Each assembly uses a
    /// collectible load context, and eviction explicitly unloads that context. An
    /// unbounded cache in the default load context grows for the entire campaign because
    /// random expression sources are almost never repeated.
    /// </remarks>
    /// <param name="source">The C# source.</param>
    /// <returns>The compiled assembly, or <c>null</c> when the source does not compile.</returns>
    public Assembly? TryCompileReference(string source)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_assemblyCache.TryGetValue(source, out CachedAssembly? cached))
        {
            _cacheOrder.Remove(cached.CacheNode);
            _cacheOrder.AddLast(cached.CacheNode);
            return cached.Assembly;
        }

        SyntaxTree tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest));
        CSharpCompilation compilation = CSharpCompilation.Create(
            "FuzzReferenceAssembly_" + Guid.NewGuid().ToString("N"),
            [tree],
            _references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Debug));

        using var stream = new MemoryStream();
        EmitResult result = compilation.Emit(stream);

        if (!result.Success)
        {
            return null;
        }

        byte[] image = stream.ToArray();
        var loadContext = new AssemblyLoadContext("FuzzReference_" + Guid.NewGuid().ToString("N"), isCollectible: true);
        Assembly assembly;
        try
        {
            stream.Position = 0;
            assembly = loadContext.LoadFromStream(stream);
        }
        catch
        {
            loadContext.Unload();
            throw;
        }

        EvictLeastRecentlyUsed();
        LinkedListNode<string> node = _cacheOrder.AddLast(source);
        _assemblyCache.Add(source, new CachedAssembly(assembly, loadContext, node));
        _assemblyImages.Add(assembly, image);
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
                return (false, null, HarnessErrorPrefix + $"reference type '{typeName}' not found");
            }

            // Each VM call uses a fresh execution engine. Mirror that isolation for
            // mutable contract fields instead of letting earlier CLR boundary calls
            // change the result of later ones.
            if (HasMutableStaticFields(type) && _assemblyImages.TryGetValue(assembly, out byte[]? image))
            {
                return TryInvokeIsolated(image, typeName, methodName, args);
            }

            return TryInvoke(type, methodName, args);
        }
        catch (TargetInvocationException ex)
        {
            return (false, null, $"{ex.InnerException?.GetType().Name ?? "Exception"}: {ex.InnerException?.Message}");
        }
        catch (Exception ex)
        {
            return (false, null, HarnessErrorPrefix + $"{ex.GetType().Name}: {ex.Message}");
        }
    }

    private static bool HasMutableStaticFields(Type type) =>
        type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Any(field => !field.IsInitOnly && !field.IsLiteral);

    private static (bool Ok, object? Value, string? Error) TryInvokeIsolated(
        byte[] image, string typeName, string methodName, object?[] args)
    {
        var loadContext = new AssemblyLoadContext("FuzzInvocation_" + Guid.NewGuid().ToString("N"), isCollectible: true);
        try
        {
            using var stream = new MemoryStream(image, writable: false);
            Assembly assembly = loadContext.LoadFromStream(stream);
            Type? type = assembly.GetType(typeName);
            return type is null
                ? (false, null, HarnessErrorPrefix + $"reference type '{typeName}' not found")
                : TryInvoke(type, methodName, args);
        }
        finally
        {
            loadContext.Unload();
        }
    }

    private static (bool Ok, object? Value, string? Error) TryInvoke(Type type, string methodName, object?[] args)
    {
        MethodInfo[] candidates = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic)
            .Where(m => m.Name == methodName && m.GetParameters().Length == args.Length)
            .ToArray();

        var matches = new List<(MethodInfo Method, object?[] Arguments, int Cost)>();
        foreach (MethodInfo candidate in candidates)
        {
            if (TryConvertArguments(candidate.GetParameters(), args, out object?[]? converted, out int cost))
            {
                matches.Add((candidate, converted, cost));
            }
        }

        if (matches.Count == 0)
        {
            return (false, null, HarnessErrorPrefix + $"reference method '{methodName}/{args.Length}' has no overload compatible with the arguments");
        }

        int bestCost = matches.Min(m => m.Cost);
        var best = matches.Where(m => m.Cost == bestCost).ToArray();
        if (best.Length != 1)
        {
            return (false, null, HarnessErrorPrefix + $"reference method '{methodName}/{args.Length}' is ambiguous for the supplied argument types");
        }

        return (true, best[0].Method.Invoke(null, best[0].Arguments), null);
    }

    /// <summary>
    /// Distinguishes a reference dispatch/conversion failure from an exception thrown
    /// by the reference method itself. Only the latter participates in the differential.
    /// </summary>
    public static bool IsHarnessError(string? error) =>
        error is not null && error.StartsWith(HarnessErrorPrefix, StringComparison.Ordinal);

    /// <summary>
    /// Selects an overload using the runtime argument shapes and converts Neo integer
    /// arguments to the CLR parameter's checked numeric type.
    /// </summary>
    private static bool TryConvertArguments(
        IReadOnlyList<ParameterInfo> parameters,
        object?[] args,
        out object?[] converted,
        out int cost)
    {
        converted = new object?[args.Length];
        cost = 0;

        for (int i = 0; i < args.Length; i++)
        {
            if (!TryConvertArgument(parameters[i].ParameterType, args[i], out converted[i], out int argumentCost))
            {
                return false;
            }
            cost += argumentCost;
        }

        return true;
    }

    private static bool TryConvertArgument(Type targetType, object? value, out object? converted, out int cost)
    {
        Type type = Nullable.GetUnderlyingType(targetType) ?? targetType;
        converted = value;
        cost = 0;

        if (value is null)
        {
            return !type.IsValueType || Nullable.GetUnderlyingType(targetType) is not null;
        }

        if (type == typeof(object) && value is System.Numerics.BigInteger objectInteger &&
            objectInteger >= int.MinValue && objectInteger <= int.MaxValue)
        {
            // The generator's ordinary integer values originate as C# int. Preserve
            // that runtime type when the ABI erases the parameter to System.Object.
            converted = (int)objectInteger;
            cost = 1;
            return true;
        }

        if (type.IsInstanceOfType(value))
        {
            return true;
        }

        if (value is System.Numerics.BigInteger integer)
        {
            try
            {
                converted = Type.GetTypeCode(type) switch
                {
                    TypeCode.Byte => (byte)integer,
                    TypeCode.SByte => (sbyte)integer,
                    TypeCode.Int16 => (short)integer,
                    TypeCode.UInt16 => (ushort)integer,
                    TypeCode.Int32 => (int)integer,
                    TypeCode.UInt32 => (uint)integer,
                    TypeCode.Int64 => (long)integer,
                    TypeCode.UInt64 => (ulong)integer,
                    TypeCode.Decimal => (decimal)integer,
                    _ => value,
                };
                cost = 1;
                return converted != value;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        return false;
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
    /// Evicts one entry when the cache reached its capacity.
    /// </summary>
    private void EvictLeastRecentlyUsed()
    {
        if (_assemblyCache.Count < _cacheCapacity)
        {
            return;
        }

        LinkedListNode<string> oldest = _cacheOrder.First!;
        CachedAssembly cached = _assemblyCache[oldest.Value];
        _cacheOrder.RemoveFirst();
        _assemblyCache.Remove(oldest.Value);
        _assemblyImages.Remove(cached.Assembly);
        cached.LoadContext.Unload();
        EvictedAssemblyCount++;
    }

    /// <summary>
    /// Reclaims collectible load contexts that have already been evicted.
    /// </summary>
    /// <remarks>
    /// Unloading an <see cref="AssemblyLoadContext"/> is cooperative and completes during
    /// a later GC. Reference compilation otherwise allocates faster than the default GC
    /// schedule reclaims evicted contexts, allowing a long campaign's working set to grow
    /// even though the live cache remains bounded.
    /// </remarks>
    public void CollectUnloadedAssemblies()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        GC.Collect(2, GCCollectionMode.Aggressive, blocking: true, compacting: true);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Aggressive, blocking: true, compacting: true);
    }

    /// <summary>
    /// Releases the loaded reference assemblies.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        foreach (CachedAssembly cached in _assemblyCache.Values)
        {
            cached.LoadContext.Unload();
            EvictedAssemblyCount++;
        }

        _assemblyCache.Clear();
        _assemblyImages.Clear();
        _cacheOrder.Clear();
        _disposed = true;
    }
}
