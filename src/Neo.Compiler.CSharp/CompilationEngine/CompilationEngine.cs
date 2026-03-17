// Copyright (C) 2015-2026 The Neo Project.
//
// CompilationEngine.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using BigInteger = System.Numerics.BigInteger;

namespace Neo.Compiler
{
    public class CompilationEngine(CompilationOptions options)
    {
        internal Compilation? Compilation;
        internal CompilationOptions Options { get; private set; } = options;
        private static readonly MetadataReference[] CommonReferences;
        private readonly Dictionary<string, MetadataReference> MetaReferences = new();
        private string? ProjectVersion;
        private string? ProjectVersionPrefix;
        private string? ProjectVersionSuffix;
        internal readonly ConcurrentDictionary<INamedTypeSymbol, CompilationContext> Contexts = new(SymbolEqualityComparer.Default);
        private readonly Lock tempProjectLock = new();
        private string? tempProjectDirectory;
        private string? tempProjectPath;
        private string? tempProjectNuGetConfig;
        private string? tempProjectReferencesKey;
        private bool tempProjectCleanupRegistered;

        /// <summary>
        /// Gets the version that was extracted from the project
        /// </summary>
        /// <returns>The version value based on available version properties</returns>
        public string? GetProjectVersion()
        {
            // If Version is set, use it directly
            if (!string.IsNullOrEmpty(ProjectVersion))
            {
                return ProjectVersion;
            }

            // If both VersionPrefix and VersionSuffix are set, combine them
            if (!string.IsNullOrEmpty(ProjectVersionPrefix) && !string.IsNullOrEmpty(ProjectVersionSuffix))
            {
                return $"{ProjectVersionPrefix}-{ProjectVersionSuffix}";
            }

            // If only one of them is set, use that one
            if (!string.IsNullOrEmpty(ProjectVersionPrefix))
            {
                return ProjectVersionPrefix;
            }

            if (!string.IsNullOrEmpty(ProjectVersionSuffix))
            {
                return ProjectVersionSuffix;
            }

            // No version information found
            return null;
        }

        static CompilationEngine()
        {
            CommonReferences =
            [
                RuntimeAssemblyResolver.CreateFrameworkReference("System.Runtime.dll"),
                RuntimeAssemblyResolver.CreateFrameworkReference("System.Runtime.InteropServices.dll"),
                RuntimeAssemblyResolver.CreateFrameworkReference("System.ComponentModel.Primitives.dll"),
                RuntimeAssemblyResolver.CreateFrameworkReference("System.Runtime.Numerics.dll"),
                RuntimeAssemblyResolver.CreateFrameworkReference("System.Collections.dll"),
                RuntimeAssemblyResolver.CreateFrameworkReference("System.Memory.dll")
            ];
        }

        internal List<CompilationContext> CompileFromCodeBlock(string codeBlock)
        {
            var sourceCode = $"using Neo.SmartContract.Framework.Native;\n" +
                             $"using Neo.SmartContract.Framework.Services;\n" +
                             $"using System;\n" +
                             $"using System.Text;\n" +
                             $"using System.Numerics;\n" +
                             $"using Neo.SmartContract.Framework;\n\n" +
                             $"namespace Neo.Compiler.CSharp.TestContracts;\n\n" +
                             $"public class CodeBlockTest : SmartContract.Framework.SmartContract\n{{\n\n" +
                             $"    public static void CodeBlock()\n" +
                             $"    {{\n" +
                                $"        {codeBlock}\n" +
                             $"    }}\n" +
                             $"}}\n";

            // Create a secure temporary directory with unique name to avoid race conditions
            string tempDir = Path.Combine(Path.GetTempPath(), $"neo-compiler-{Guid.NewGuid():N}");
            Directory.CreateDirectory(tempDir);
            string tempFilePath = Path.Combine(tempDir, "CodeBlockTest.cs");

            try
            {
                // Write source code to the secure temp file
                File.WriteAllText(tempFilePath, sourceCode);
                return CompileSources(tempFilePath);
            }
            finally
            {
                // Ensure cleanup of temp directory and all contents
                try
                {
                    if (Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, recursive: true);
                    }
                }
                catch
                {
                    // Best effort cleanup - don't throw from finally
                }
            }
        }

        public List<CompilationContext> Compile(IEnumerable<string> sourceFiles, IEnumerable<MetadataReference> references)
        {
            IEnumerable<SyntaxTree> syntaxTrees = sourceFiles.OrderBy(p => p).Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), options: Options.GetParseOptions(), path: p));
            CSharpCompilationOptions compilationOptions = new(OutputKind.DynamicallyLinkedLibrary, deterministic: true, nullableContextOptions: Options.Nullable, allowUnsafe: false);
            Compilation = CSharpCompilation.Create(null, syntaxTrees, references, compilationOptions);
            return CompileProjectContracts(Compilation);
        }

        public List<CompilationContext> CompileSources(params string[] sourceFiles)
        {
            var references = new CompilationSourceReferences();
            if (TryGetLocalFrameworkProject(out var frameworkProject))
            {
                references.Projects = [frameworkProject];
            }
            else
            {
                references.Packages = [new("Neo.SmartContract.Framework", "3.9.0")];
            }

            return CompileSources(references, sourceFiles);
        }

        public List<CompilationContext> CompileSources(CompilationSourceReferences references, params string[] sourceFiles)
        {
            if (sourceFiles is null || sourceFiles.Length == 0)
            {
                throw new ArgumentException("At least one source file must be provided.", nameof(sourceFiles));
            }

            lock (tempProjectLock)
            {
                var referencesKey = BuildReferencesKey(references);
                if (Options.SkipRestoreIfAssetsPresent)
                {
                    EnsurePersistentTempProject(referencesKey);
                }
                else
                {
                    PrepareTransientTempProject();
                }

                WriteTempProject(references, sourceFiles);

                Compilation = null;
                try
                {
                    return CompileProject(tempProjectPath!);
                }
                finally
                {
                    if (!Options.SkipRestoreIfAssetsPresent)
                    {
                        CleanupTempProjectDirectory();
                    }
                }
            }
        }

        private void PrepareTransientTempProject()
        {
            CleanupTempProjectDirectory();
            tempProjectDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempProjectDirectory);
            tempProjectPath = Path.Combine(tempProjectDirectory, "TempProject.csproj");
            tempProjectNuGetConfig = Path.Combine(tempProjectDirectory, "nuget.config");
            WriteNuGetConfig(tempProjectNuGetConfig);
            tempProjectReferencesKey = null;
        }

        private void EnsurePersistentTempProject(string referencesKey)
        {
            var directoryExists = tempProjectDirectory is not null && Directory.Exists(tempProjectDirectory);
            if (!directoryExists || tempProjectReferencesKey != referencesKey)
            {
                CleanupTempProjectDirectory();
                tempProjectDirectory = Path.Combine(Path.GetTempPath(), "Neo.Compiler", "CompileSources", Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempProjectDirectory);
                tempProjectPath = Path.Combine(tempProjectDirectory, "TempProject.csproj");
                tempProjectNuGetConfig = Path.Combine(tempProjectDirectory, "nuget.config");
                WriteNuGetConfig(tempProjectNuGetConfig);
                tempProjectReferencesKey = referencesKey;
                RegisterTempProjectCleanup();
            }
        }

        private void WriteTempProject(CompilationSourceReferences references, string[] sourceFiles)
        {
            if (tempProjectDirectory is null)
            {
                throw new InvalidOperationException("Temporary project directory must be initialized before writing the project file.");
            }

            tempProjectPath ??= Path.Combine(tempProjectDirectory, "TempProject.csproj");
            tempProjectNuGetConfig ??= Path.Combine(tempProjectDirectory, "nuget.config");

            var csproj = BuildTempProjectContent(references, sourceFiles);
            File.WriteAllText(tempProjectPath, csproj);

            if (!File.Exists(tempProjectNuGetConfig))
            {
                WriteNuGetConfig(tempProjectNuGetConfig);
            }
        }

        private static string BuildTempProjectContent(CompilationSourceReferences references, string[] sourceFiles)
        {
            var packages = references.Packages;
            var packageGroup = packages is null || packages.Length == 0
                ? string.Empty
                : $@"
    <ItemGroup>
        {string.Join(Environment.NewLine, packages.Select(u => $" <PackageReference Include =\"{u.packageName}\" Version=\"{u.packageVersion}\" />"))}
    </ItemGroup>";

            var projects = references.Projects;
            var projectsGroup = projects is null || projects.Length == 0
                ? string.Empty
                : $@"
    <ItemGroup>
        {string.Join(Environment.NewLine, projects.Select(u => $" <ProjectReference Include =\"{u}\"/>"))}
    </ItemGroup>";

            return $@"
<Project Sdk=""Microsoft.NET.Sdk"">

    <PropertyGroup>
        <TargetFramework>{GetTargetFrameworkMoniker()}</TargetFramework>
        <LangVersion>preview</LangVersion>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
    </PropertyGroup>

    <!-- Remove all Compile items from compilation -->
    <ItemGroup>
        <Compile Remove=""*.cs"" />
    </ItemGroup>

    <!-- Add specific files for compilation -->
    <ItemGroup>
        {string.Join(Environment.NewLine, sourceFiles.Select(u => $"<Compile Include=\"{Path.GetFullPath(u)}\" />"))}
    </ItemGroup>

    {packageGroup}
    {projectsGroup}

</Project>";
        }

        private void WriteNuGetConfig(string nugetConfigPath)
        {
            const string nugetConfigContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <clear />
    <add key=""NuGet.org"" value=""https://api.nuget.org/v3/index.json"" protocolVersion=""3"" />
  </packageSources>
</configuration>";

            File.WriteAllText(nugetConfigPath, nugetConfigContent);
        }

        private static string BuildReferencesKey(CompilationSourceReferences references)
        {
            var builder = new StringBuilder();

            if (references.Packages is { Length: > 0 })
            {
                foreach (var (packageName, packageVersion) in references.Packages
                             .OrderBy(p => p.packageName, StringComparer.Ordinal))
                {
                    builder.Append("pkg:")
                        .Append(packageName)
                        .Append('@')
                        .Append(packageVersion)
                        .Append(';');
                }
            }

            builder.Append('|');

            if (references.Projects is { Length: > 0 })
            {
                foreach (var project in references.Projects
                             .Select(p => Path.GetFullPath(p))
                             .OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
                {
                    builder.Append("proj:")
                        .Append(project)
                        .Append(';');
                }
            }

            return builder.ToString();
        }

        private void RegisterTempProjectCleanup()
        {
            if (tempProjectCleanupRegistered)
            {
                return;
            }

            tempProjectCleanupRegistered = true;
            AppDomain.CurrentDomain.ProcessExit += (_, _) => CleanupTempProjectDirectory();
        }

        private void CleanupTempProjectDirectory()
        {
            if (tempProjectDirectory is null)
            {
                return;
            }

            try
            {
                if (Directory.Exists(tempProjectDirectory))
                {
                    Directory.Delete(tempProjectDirectory, true);
                }
            }
            catch (IOException)
            {
                // Best-effort cleanup; ignore IO failures.
            }
            catch (UnauthorizedAccessException)
            {
                // Best-effort cleanup; ignore permission failures.
            }

            tempProjectDirectory = null;
            tempProjectPath = null;
            tempProjectNuGetConfig = null;
            tempProjectReferencesKey = null;
        }

        private static string GetTargetFrameworkMoniker() => RuntimeAssemblyResolver.CompilerTargetFrameworkMoniker;

        public List<CompilationContext> CompileProject(string csproj)
        {
            Contexts.Clear();
            Compilation ??= GetCompilation(csproj);
            return CompileProjectContracts(Compilation);
        }

        public List<CompilationContext> CompileProject(string csproj, List<INamedTypeSymbol> sortedClasses, Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> classDependencies, List<INamedTypeSymbol?> allClassSymbols, string? targetContractName = null)
        {
            if (sortedClasses == null || classDependencies == null || allClassSymbols == null)
            {
                throw new InvalidOperationException("Please call PrepareProjectContracts before calling CompileProject with sortedClasses, classDependencies and allClassSymbols parameters.");
            }
            Contexts.Clear();
            Compilation ??= GetCompilation(csproj);
            return targetContractName == null ? CompileProjectContractsWithPrepare(sortedClasses, classDependencies, allClassSymbols) : [CompileProjectContractWithPrepare(sortedClasses, classDependencies, allClassSymbols, targetContractName)];
        }

        public (List<INamedTypeSymbol> sortedClasses, Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> classDependencies, List<INamedTypeSymbol?> allClassSymbols) PrepareProjectContracts(string csproj)
        {
            Compilation ??= GetCompilation(csproj);
            var classDependencies = new Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>>(SymbolEqualityComparer.Default);
            var allSmartContracts = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            var allClassSymbols = new List<INamedTypeSymbol?>();
            foreach (var tree in Compilation.SyntaxTrees)
            {
                var semanticModel = Compilation.GetSemanticModel(tree);
                var classNodes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

                foreach (var classNode in classNodes)
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classNode);
                    allClassSymbols.Add(classSymbol);
                    if (classSymbol is { IsAbstract: false, DeclaredAccessibility: Accessibility.Public } && IsDerivedFromSmartContract(classSymbol))
                    {
                        allSmartContracts.Add(classSymbol);
                        classDependencies[classSymbol] = [];
                        foreach (var member in classSymbol.GetMembers())
                        {
                            var memberTypeSymbol = (member as IFieldSymbol)?.Type ?? (member as IPropertySymbol)?.Type;
                            if (memberTypeSymbol is INamedTypeSymbol namedTypeSymbol && allSmartContracts.Contains(namedTypeSymbol) && !namedTypeSymbol.IsAbstract)
                            {
                                classDependencies[classSymbol].Add(namedTypeSymbol);
                            }
                        }
                    }
                }
            }

            // Verify if there is any valid smart contract class
            if (classDependencies.Count == 0) throw new FormatException("No valid neo SmartContract found. Please make sure your contract is subclass of SmartContract and is not abstract.");
            // Check contract dependencies, make sure there is no cycle in the dependency graph
            var sortedClasses = TopologicalSort(classDependencies);

            return (sortedClasses, classDependencies, allClassSymbols);
        }

        private CompilationContext CompileProjectContractWithPrepare(List<INamedTypeSymbol> sortedClasses, Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> classDependencies, List<INamedTypeSymbol?> allClassSymbols, string targetContractName)
        {
            var c = sortedClasses.FirstOrDefault(p => p.Name.Equals(targetContractName, StringComparison.InvariantCulture))
                ?? throw new ArgumentException($"targetContractName '{targetContractName}' was not found");
            var dependencies = classDependencies.TryGetValue(c, out var dependency) ? dependency : [];
            var classesNotInDependencies = allClassSymbols.Except(dependencies).ToList();
            var context = new CompilationContext(this, c, classesNotInDependencies!, allowBaseName: true);
            context.Compile();
            return context;
        }

        private List<CompilationContext> CompileProjectContractsWithPrepare(List<INamedTypeSymbol> sortedClasses, Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> classDependencies, List<INamedTypeSymbol?> allClassSymbols)
        {
            Contexts.Clear();
            bool allowBaseName = sortedClasses.Count <= 1;
            Parallel.ForEach(sortedClasses, c =>
            {
                var dependencies = classDependencies.TryGetValue(c, out var dependency) ? dependency : [];
                var classesNotInDependencies = allClassSymbols.Except(dependencies).ToList();
                var context = new CompilationContext(this, c, classesNotInDependencies!, allowBaseName);
                context.Compile();
                // Process the target contract add this compilation context
                Contexts.TryAdd(c, context);
            });

            return Contexts.Select(p => p.Value).ToList();
        }

        private List<CompilationContext> CompileProjectContracts(Compilation compilation)
        {
            Contexts.Clear();
            var classDependencies = new Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>>(SymbolEqualityComparer.Default);
            var allSmartContracts = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            var allClassSymbols = new List<INamedTypeSymbol?>();
            var classSymbols = new List<INamedTypeSymbol>();
            foreach (var tree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(tree);
                var classNodes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

                foreach (var classNode in classNodes)
                {
                    var classSymbol = semanticModel.GetDeclaredSymbol(classNode);
                    allClassSymbols.Add(classSymbol);
                    if (classSymbol is null) continue;

                    classSymbols.Add(classSymbol);
                    if (classSymbol is { IsAbstract: false, DeclaredAccessibility: Accessibility.Public } && IsDerivedFromSmartContract(classSymbol))
                    {
                        allSmartContracts.Add(classSymbol);
                        classDependencies[classSymbol] = [];
                    }
                }
            }

            foreach (var classSymbol in classSymbols)
            {
                if (!allSmartContracts.Contains(classSymbol))
                    continue;

                foreach (var member in classSymbol.GetMembers())
                {
                    var memberTypeSymbol = (member as IFieldSymbol)?.Type ?? (member as IPropertySymbol)?.Type;
                    if (memberTypeSymbol is not INamedTypeSymbol namedTypeSymbol)
                        continue;
                    if (namedTypeSymbol.IsAbstract)
                        continue;
                    if (!allSmartContracts.Contains(namedTypeSymbol))
                        continue;
                    if (classDependencies[classSymbol].Any(p => SymbolEqualityComparer.Default.Equals(p, namedTypeSymbol)))
                        continue;
                    classDependencies[classSymbol].Add(namedTypeSymbol);
                }
            }

            // Verify if there is any valid smart contract class
            if (classDependencies.Count == 0) throw new FormatException("No valid neo SmartContract found. Please make sure your contract is subclass of SmartContract and is not abstract.");
            // Check contract dependencies, make sure there is no cycle in the dependency graph
            var sortedClasses = TopologicalSort(classDependencies);

            bool allowBaseName = sortedClasses.Count <= 1;
            Parallel.ForEach(sortedClasses, c =>
            {
                var dependencies = classDependencies.TryGetValue(c, out var dependency) ? dependency : [];
                var classesNotInDependencies = allClassSymbols.Except(dependencies).ToList();
                var context = new CompilationContext(this, c, classesNotInDependencies!, allowBaseName);
                context.Compile();
                // Process the target contract add this compilation context
                Contexts.TryAdd(c, context);
            });

            return Contexts.Select(p => p.Value).ToList();
        }

        /// <summary>
        /// Sort the classes based on their topological dependencies
        /// </summary>
        /// <param name="dependencies">Contract dependencies map</param>
        /// <returns>List of sorted contracts</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private static List<INamedTypeSymbol> TopologicalSort(Dictionary<INamedTypeSymbol, List<INamedTypeSymbol>> dependencies)
        {
            var sorted = new List<INamedTypeSymbol>();
            var visited = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
            var visiting = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default); // for detecting cycles

            void Visit(INamedTypeSymbol classSymbol)
            {
                if (visited.Contains(classSymbol))
                {
                    return;
                }
                if (!visiting.Add(classSymbol))
                {
                    throw new InvalidOperationException("Cyclic dependency detected");
                }

                if (dependencies.TryGetValue(classSymbol, out var dependency))
                {
                    foreach (var dep in dependency)
                    {
                        Visit(dep);
                    }
                }

                visiting.Remove(classSymbol);
                visited.Add(classSymbol);
                sorted.Add(classSymbol);
            }

            foreach (var classSymbol in dependencies.Keys)
            {
                Visit(classSymbol);
            }

            return sorted;
        }

        private const string SmartContractTypeName = "SmartContract";
        private const string SmartContractNamespace = "Neo.SmartContract.Framework";

        internal static bool IsDerivedFromSmartContract(INamedTypeSymbol classSymbol)
        {
            var baseType = classSymbol.BaseType;
            while (baseType != null)
            {
                if (baseType.Name == SmartContractTypeName &&
                    baseType.ContainingNamespace?.ToString() == SmartContractNamespace)
                {
                    return true;
                }
                baseType = baseType.BaseType;
            }
            return false;
        }

        public Compilation GetCompilation(string csproj)
        {
            // Restore project

            string folder = Path.GetDirectoryName(csproj)!;
            var assetsPath = Path.Combine(folder, "obj", "project.assets.json");
            var shouldSkipRestore = Options.SkipRestoreIfAssetsPresent && File.Exists(assetsPath);

            if (!shouldSkipRestore)
            {
                using var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"restore \"{csproj}\"",
                    WorkingDirectory = folder
                });
                process?.WaitForExit();
            }

            if (!File.Exists(assetsPath))
            {
                throw new FileNotFoundException($"Unable to locate '{assetsPath}'. Ensure the project has been restored.");
            }

            // Parse csproj

            XDocument document = XDocument.Load(csproj);

            // Extract Version information from the project file or its Directory.Build.props
            ExtractVersionInfo(document, Path.GetDirectoryName(csproj)!);

            var remove = document.Root!.Elements("ItemGroup").Elements("Compile").Attributes("Remove")
                .Select(p => p.Value.Contains('*') ? p.Value : Path.GetFullPath(p.Value)).ToArray();
            var sourceFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!remove.Contains("*.cs"))
            {
                var obj = Path.Combine(folder, "obj");
                var binSc = Path.Combine(folder, "bin");
                foreach (var entry in Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories)
                      .Where(p => !p.StartsWith(obj) && !p.StartsWith(binSc))
                      .Select(u => u))
                //.GroupBy(Path.GetFileName)
                //.Select(g => g.First()))
                {
                    if (!remove.Contains(entry)) sourceFiles.Add(entry);
                }
            }

            sourceFiles.UnionWith(document.Root!.Elements("ItemGroup").Elements("Compile").Attributes("Include").Select(p => Path.GetFullPath(p.Value, folder)));
            var assets = (JObject)JToken.Parse(File.ReadAllBytes(assetsPath))!;
            List<MetadataReference> references = new(CommonReferences);
            CSharpCompilationOptions compilationOptions = new(OutputKind.DynamicallyLinkedLibrary, deterministic: true, nullableContextOptions: Options.Nullable, allowUnsafe: false);
            foreach (var (name, package) in ((JObject)assets["targets"]![0]!).Properties)
            {
                MetadataReference? reference = GetReference(name, (JObject)package!, assets, folder, compilationOptions);
                if (reference is not null) references.Add(reference);
            }
            IEnumerable<SyntaxTree> syntaxTrees = sourceFiles.OrderBy(p => p).Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), options: Options.GetParseOptions(), path: p));
            return CSharpCompilation.Create(assets["project"]!["restore"]!["projectName"]!.GetString(), syntaxTrees, references, compilationOptions);
        }

        private MetadataReference? GetReference(string name, JObject package, JObject assets, string folder, CSharpCompilationOptions compilationOptions)
        {
            if (!MetaReferences.TryGetValue(name, out var reference))
            {
                switch (assets["libraries"]![name]!["type"]!.GetString())
                {
                    case "package":
                        string packagesPath = assets["project"]!["restore"]!["packagesPath"]!.GetString();
                        string namePath = assets["libraries"]![name]!["path"]!.GetString();
                        string[] files = ((JArray)assets["libraries"]![name]!["files"]!)
                            .Select(p => p!.GetString())
                            .Where(p => p.StartsWith("src/"))
                            .ToArray();
                        if (files.Length == 0)
                        {
                            JObject? dllFiles = (JObject?)(package["compile"] ?? package["runtime"]);
                            if (dllFiles is null) return null;
                            foreach (var (file, _) in dllFiles.Properties)
                            {
                                if (file.EndsWith("_._")) continue;
                                string path = Path.Combine(packagesPath, namePath, file);
                                if (!File.Exists(path)) continue;
                                reference = MetadataReference.CreateFromFile(path);
                                break;
                            }
                            if (reference is null) return null;
                        }
                        else
                        {
                            string assemblyName = Path.GetDirectoryName(name)!;
                            IEnumerable<SyntaxTree> st = files.OrderBy(p => p).Select(p => Path.Combine(packagesPath, namePath, p)).Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), path: p));
                            CSharpCompilation cr = CSharpCompilation.Create(assemblyName, st, CommonReferences, compilationOptions);
                            reference = cr.ToMetadataReference();
                        }
                        break;
                    case "project":
                        string msbuildProject = assets["libraries"]![name]!["msbuildProject"]!.GetString();
                        msbuildProject = Path.GetFullPath(msbuildProject, folder);
                        reference = GetCompilationPreservingVersion(msbuildProject).ToMetadataReference();
                        break;
                    default:
                        throw new NotSupportedException();
                }
                MetaReferences.Add(name, reference);
            }
            return reference;
        }

        private static bool TryGetLocalFrameworkProject(out string frameworkProject)
        {
            var searchRoots = new[]
            {
                Directory.GetCurrentDirectory(),
                AppContext.BaseDirectory,
                Path.GetDirectoryName(typeof(CompilationEngine).Assembly.Location) ?? string.Empty
            };

            foreach (var root in searchRoots.Where(r => !string.IsNullOrEmpty(r)))
            {
                var directory = new DirectoryInfo(root);
                while (directory is not null)
                {
                    var candidate = Path.Combine(directory.FullName, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");
                    if (File.Exists(candidate))
                    {
                        frameworkProject = candidate;
                        return true;
                    }

                    directory = directory.Parent;
                }
            }

            frameworkProject = string.Empty;
            return false;
        }

        private Compilation GetCompilationPreservingVersion(string csproj)
        {
            bool preserveProjectVersion = !string.IsNullOrEmpty(ProjectVersion) ||
                !string.IsNullOrEmpty(ProjectVersionPrefix) ||
                !string.IsNullOrEmpty(ProjectVersionSuffix);
            string? projectVersion = ProjectVersion;
            string? projectVersionPrefix = ProjectVersionPrefix;
            string? projectVersionSuffix = ProjectVersionSuffix;
            try
            {
                return GetCompilation(csproj);
            }
            finally
            {
                if (preserveProjectVersion)
                {
                    ProjectVersion = projectVersion;
                    ProjectVersionPrefix = projectVersionPrefix;
                    ProjectVersionSuffix = projectVersionSuffix;
                }
            }
        }

        /// <summary>
        /// Extracts the Version information from a project file or its referenced Directory.Build.props
        /// </summary>
        /// <param name="projectDocument">The loaded project document</param>
        /// <param name="projectDirectory">The directory containing the project file</param>
        private void ExtractVersionInfo(XDocument projectDocument, string projectDirectory)
        {
            // Try to get Version information directly from the project file
            // Get the XML namespace if it exists
            XNamespace ns = projectDocument.Root?.Name.Namespace ?? string.Empty;

            // Check for Version
            ProjectVersion = projectDocument.Root?
                .Elements(ns + "PropertyGroup")
                .Elements(ns + "Version")
                .FirstOrDefault()?.Value;

            // Check for VersionPrefix
            ProjectVersionPrefix = projectDocument.Root?
                .Elements(ns + "PropertyGroup")
                .Elements(ns + "VersionPrefix")
                .FirstOrDefault()?.Value;

            // Check for VersionSuffix
            ProjectVersionSuffix = projectDocument.Root?
                .Elements(ns + "PropertyGroup")
                .Elements(ns + "VersionSuffix")
                .FirstOrDefault()?.Value;

            // If not found in the project file, try to look for Directory.Build.props
            if (string.IsNullOrEmpty(ProjectVersion) &&
                string.IsNullOrEmpty(ProjectVersionPrefix) &&
                string.IsNullOrEmpty(ProjectVersionSuffix))
            {
                string? directoryBuildPropsPath = FindDirectoryBuildProps(projectDirectory);
                if (directoryBuildPropsPath != null)
                {
                    try
                    {
                        XDocument directoryBuildProps = XDocument.Load(directoryBuildPropsPath);

                        // Get the XML namespace if it exists
                        ns = directoryBuildProps.Root?.Name.Namespace ?? string.Empty;

                        // Check for Version
                        if (string.IsNullOrEmpty(ProjectVersion))
                        {
                            ProjectVersion = directoryBuildProps.Root?
                                .Elements(ns + "PropertyGroup")
                                .Elements(ns + "Version")
                                .FirstOrDefault()?.Value;
                        }

                        // Check for VersionPrefix
                        if (string.IsNullOrEmpty(ProjectVersionPrefix))
                        {
                            ProjectVersionPrefix = directoryBuildProps.Root?
                                .Elements(ns + "PropertyGroup")
                                .Elements(ns + "VersionPrefix")
                                .FirstOrDefault()?.Value;
                        }

                        // Check for VersionSuffix
                        if (string.IsNullOrEmpty(ProjectVersionSuffix))
                        {
                            ProjectVersionSuffix = directoryBuildProps.Root?
                                .Elements(ns + "PropertyGroup")
                                .Elements(ns + "VersionSuffix")
                                .FirstOrDefault()?.Value;
                        }
                    }
                    catch
                    {
                        // Ignore errors when trying to load Directory.Build.props
                    }
                }
            }
        }

        /// <summary>
        /// Recursively searches for Directory.Build.props starting from the specified directory and moving up
        /// </summary>
        /// <param name="directory">Starting directory</param>
        /// <returns>Path to Directory.Build.props file or null if not found</returns>
        private string? FindDirectoryBuildProps(string directory)
        {
            try
            {
                // Check if Directory.Build.props exists in the current directory
                string directoryBuildPropsPath = Path.Combine(directory, "Directory.Build.props");
                if (File.Exists(directoryBuildPropsPath))
                {
                    return directoryBuildPropsPath;
                }

                // Move up one directory if possible
                string? parentDirectory = Path.GetDirectoryName(directory);
                if (parentDirectory != null && parentDirectory != directory)
                {
                    return FindDirectoryBuildProps(parentDirectory);
                }

                // Not found
                return null;
            }
            catch
            {
                // Handle any exceptions that might occur during directory traversal
                return null;
            }
        }
    }
}
