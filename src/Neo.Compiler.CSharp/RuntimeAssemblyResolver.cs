// Copyright (C) 2015-2025 The Neo Project.
//
// RuntimeAssemblyResolver helps locate framework assemblies when running
// in trimmed or single-file deployments where Assembly.Location is empty.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;

namespace Neo.Compiler
{
    internal static class RuntimeAssemblyResolver
    {
        private static readonly Lazy<string> CoreDirectory = new(ResolveCoreDirectory, LazyThreadSafetyMode.ExecutionAndPublication);
        private static readonly Lazy<IReadOnlyDictionary<string, string>> TrustedAssemblies = new(BuildTrustedAssemblyMap, LazyThreadSafetyMode.ExecutionAndPublication);
        private static readonly Lazy<string?> CompilerBaseDirectory = new(GetCompilerBaseDirectory, LazyThreadSafetyMode.ExecutionAndPublication);
        private static readonly Lazy<string?> ReferencePackDirectory = new(GetReferencePackDirectory, LazyThreadSafetyMode.ExecutionAndPublication);
        private static readonly Lazy<string> TargetFrameworkMoniker = new(GetTargetFrameworkMoniker, LazyThreadSafetyMode.ExecutionAndPublication);
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

        internal static MetadataReference CreateFrameworkReference(string fileName)
        {
            if (TryResolveFrameworkAssemblyPath(fileName, out var path) && path is not null)
            {
                return MetadataReference.CreateFromFile(path);
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
            foreach (var baseDir in EnumerateBaseDirectories())
            {
                if (string.IsNullOrEmpty(baseDir))
                {
                    continue;
                }

                var refsPath = Path.Combine(baseDir, "refs", fileName);
                if (File.Exists(refsPath))
                {
                    path = refsPath;
                    return true;
                }

                var localPath = Path.Combine(baseDir, fileName);
                if (File.Exists(localPath))
                {
                    path = localPath;
                    return true;
                }
            }

            var referencePack = ReferencePackDirectory.Value;
            if (!string.IsNullOrEmpty(referencePack))
            {
                var packPath = Path.Combine(referencePack, fileName);
                if (File.Exists(packPath))
                {
                    path = packPath;
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

        private static IEnumerable<string?> EnumerateBaseDirectories()
        {
            yield return CompilerBaseDirectory.Value;
            yield return AppContext.BaseDirectory;
        }

        private static string? GetCompilerBaseDirectory()
        {
#pragma warning disable IL3000
            var location = typeof(RuntimeAssemblyResolver).Assembly.Location;
#pragma warning restore IL3000
            if (!string.IsNullOrEmpty(location))
            {
                return Path.GetDirectoryName(location);
            }

            return null;
        }

        private static string? GetReferencePackDirectory()
        {
            var coreDir = CoreDirectory.Value;
            if (string.IsNullOrEmpty(coreDir))
            {
                return null;
            }

            var coreDirInfo = new DirectoryInfo(coreDir);
            var version = coreDirInfo.Name;
            var sharedFrameworkDir = coreDirInfo.Parent;
            if (sharedFrameworkDir is null || !string.Equals(sharedFrameworkDir.Name, "Microsoft.NETCore.App", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var dotnetRoot = sharedFrameworkDir.Parent?.Parent;
            if (dotnetRoot is null)
            {
                return null;
            }

            var tfm = TargetFrameworkMoniker.Value;
            var referencePackPath = Path.Combine(dotnetRoot.FullName, "packs", "Microsoft.NETCore.App.Ref", version, "ref", tfm);
            return Directory.Exists(referencePackPath) ? referencePackPath : null;
        }

        private static string GetTargetFrameworkMoniker()
        {
            var attribute = typeof(RuntimeAssemblyResolver).Assembly.GetCustomAttribute<TargetFrameworkAttribute>();
            if (attribute?.FrameworkName is { Length: > 0 } frameworkName)
            {
                const string prefix = ".NETCoreApp,Version=v";
                if (frameworkName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    var version = frameworkName[prefix.Length..];
                    return $"net{version}";
                }
            }

            var runtimeVersion = Environment.Version;
            return $"net{runtimeVersion.Major}.{runtimeVersion.Minor}";
        }
    }
}
