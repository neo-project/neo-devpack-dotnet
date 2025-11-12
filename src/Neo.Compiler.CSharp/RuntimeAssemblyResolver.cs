// Copyright (C) 2015-2025 The Neo Project.
//
// RuntimeAssemblyResolver helps locate framework assemblies when running
// in trimmed or single-file deployments where Assembly.Location is empty.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Threading;

namespace Neo.Compiler
{
    internal static class RuntimeAssemblyResolver
    {
        private static readonly Lazy<string> CoreDirectory = new(ResolveCoreDirectory, LazyThreadSafetyMode.ExecutionAndPublication);
        private static readonly Lazy<IReadOnlyDictionary<string, string>> TrustedAssemblies = new(BuildTrustedAssemblyMap, LazyThreadSafetyMode.ExecutionAndPublication);
        private static readonly ConcurrentDictionary<Assembly, MetadataReference> AssemblyMetadataCache = new();

        private static IReadOnlyDictionary<string, string> BuildTrustedAssemblyMap()
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string tpaList)
            {
                foreach (var path in tpaList.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
                {
                    var fileName = Path.GetFileName(path);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        map[fileName] = path;
                    }
                }
            }

            return map;
        }

        private static string ResolveCoreDirectory()
        {
#pragma warning disable IL3000
            var location = typeof(object).Assembly.Location;
#pragma warning restore IL3000
            if (!string.IsNullOrEmpty(location))
            {
                var dir = Path.GetDirectoryName(location);
                if (!string.IsNullOrEmpty(dir))
                {
                    return dir;
                }
            }

            var runtimeDir = RuntimeEnvironment.GetRuntimeDirectory();
            if (!string.IsNullOrEmpty(runtimeDir))
            {
                return runtimeDir;
            }

            var baseDir = AppContext.BaseDirectory;
            if (!string.IsNullOrEmpty(baseDir))
            {
                return baseDir;
            }

            throw new InvalidOperationException("Unable to resolve .NET runtime directory.");
        }

        internal static string ResolveFrameworkAssembly(string fileName)
        {
            if (TryResolveFrameworkAssemblyPath(fileName, out var path) && path is not null)
            {
                return path;
            }

            throw new FileNotFoundException($"Unable to locate framework assembly '{fileName}'.", path);
        }

        internal static MetadataReference CreateFrameworkReference(string fileName, Type? fallbackType = null)
        {
            if (TryResolveFrameworkAssemblyPath(fileName, out var path) && path is not null)
            {
                return MetadataReference.CreateFromFile(path);
            }

            if (fallbackType is not null)
            {
                return CreateMetadataReferenceFromAssembly(fallbackType.Assembly);
            }

            throw new FileNotFoundException($"Unable to locate framework assembly '{fileName}'.");
        }

        internal static string ResolveAssemblyFromType(Type type)
        {
            var assembly = type.Assembly;
#pragma warning disable IL3000
            if (!string.IsNullOrEmpty(assembly.Location) && File.Exists(assembly.Location))
            {
                return assembly.Location;
            }
#pragma warning restore IL3000

            var assemblyName = assembly.GetName().Name;
            if (string.IsNullOrEmpty(assemblyName))
            {
                throw new InvalidOperationException($"Unable to resolve assembly name for type '{type.FullName}'.");
            }

            return ResolveFrameworkAssembly($"{assemblyName}.dll");
        }

        private static bool TryResolveFrameworkAssemblyPath(string fileName, out string? path)
        {
            if (!string.IsNullOrEmpty(AppContext.BaseDirectory))
            {
                var refsPath = Path.Combine(AppContext.BaseDirectory, "refs", fileName);
                if (File.Exists(refsPath))
                {
                    path = refsPath;
                    return true;
                }

                var localPath = Path.Combine(AppContext.BaseDirectory, fileName);
                if (File.Exists(localPath))
                {
                    path = localPath;
                    return true;
                }
            }

            if (TrustedAssemblies.Value.TryGetValue(fileName, out var trustedPath) && File.Exists(trustedPath))
            {
                path = trustedPath;
                return true;
            }

            var candidate = Path.Combine(CoreDirectory.Value, fileName);
            if (File.Exists(candidate))
            {
                path = candidate;
                return true;
            }

            path = null;
            return false;
        }

        private static MetadataReference CreateMetadataReferenceFromAssembly(Assembly assembly)
        {
            return AssemblyMetadataCache.GetOrAdd(assembly, static asm =>
            {
#pragma warning disable IL3000
                if (!string.IsNullOrEmpty(asm.Location) && File.Exists(asm.Location))
                {
                    return MetadataReference.CreateFromFile(asm.Location);
                }
#pragma warning restore IL3000

                unsafe
                {
                    if (!asm.TryGetRawMetadata(out byte* blob, out int length))
                    {
                        throw new InvalidOperationException($"Unable to access metadata for assembly '{asm.FullName}'.");
                    }

                    var span = new ReadOnlySpan<byte>(blob, length);
                    var copy = span.ToArray();
                    return MetadataReference.CreateFromImage(ImmutableArray.Create(copy));
                }
            });
        }
    }
}
