// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using BigInteger = System.Numerics.BigInteger;

namespace Neo.Compiler
{
    public class CompilationEngine
    {
        internal Compilation? Compilation;
        internal Options Options { get; private set; }
        private static readonly MetadataReference[] CommonReferences;
        private static readonly Dictionary<string, MetadataReference> MetaReferences = new();
        internal readonly Stack<INamedTypeSymbol> DerivedContracts = new();
        internal readonly Dictionary<INamedTypeSymbol, CompilationContext> Contexts = new(SymbolEqualityComparer.Default);

        static CompilationEngine()
        {
            string coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            CommonReferences = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.InteropServices.dll")),
                MetadataReference.CreateFromFile(typeof(string).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DisplayNameAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(BigInteger).Assembly.Location)
            };
        }

        public CompilationEngine(Options options)
        {
            Options = options;
        }

        public List<CompilationContext> Compile(IEnumerable<string> sourceFiles, IEnumerable<MetadataReference> references)
        {
            IEnumerable<SyntaxTree> syntaxTrees = sourceFiles.OrderBy(p => p).Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), options: Options.GetParseOptions(), path: p));
            if (IsSingleAbstractClass(syntaxTrees)) throw new FormatException("The given class is abstract, no valid neo SmartContract found.");
            CSharpCompilationOptions compilationOptions = new(OutputKind.DynamicallyLinkedLibrary, deterministic: true, nullableContextOptions: Options.Nullable);
            Compilation = CSharpCompilation.Create(null, syntaxTrees, references, compilationOptions);
            var context = new CompilationContext(this, null);
            context.Compile();
            while (DerivedContracts.Count > 0)
            {
                INamedTypeSymbol classSymbol = DerivedContracts.Pop();
                var context2 = new CompilationContext(this, classSymbol);
                context2.Compile();
            }

            return Contexts.Select(p => p.Value).ToList();
        }

        public List<CompilationContext> CompileSources(string[] sourceFiles)
        {
            List<MetadataReference> references = new(CommonReferences)
            {
                MetadataReference.CreateFromFile(typeof(scfx.Neo.SmartContract.Framework.SmartContract).Assembly.Location)
            };
            return Compile(sourceFiles, references);
        }

        public List<CompilationContext> CompileProject(string csproj)
        {
            Compilation compilation = GetCompilation(csproj);
            Compilation = compilation;
            var context = new CompilationContext(this, null);
            context.Compile();
            while (DerivedContracts.Count > 0)
            {
                INamedTypeSymbol classSymbol = DerivedContracts.Pop();
                var context2 = new CompilationContext(this, classSymbol);
                context2.Compile();
            }

            return Contexts.Select(p => p.Value).ToList();
        }

        public Compilation GetCompilation(string csproj)
        {
            string folder = Path.GetDirectoryName(csproj)!;
            string obj = Path.Combine(folder, "obj");
            HashSet<string> sourceFiles = Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories)
                .Where(p => !p.StartsWith(obj))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            List<MetadataReference> references = new(CommonReferences);
            CSharpCompilationOptions compilationOptions = new(OutputKind.DynamicallyLinkedLibrary, deterministic: true, nullableContextOptions: Options.Nullable);
            XDocument document = XDocument.Load(csproj);
            sourceFiles.UnionWith(document.Root!.Elements("ItemGroup").Elements("Compile").Attributes("Include").Select(p => Path.GetFullPath(p.Value, folder)));
            Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"restore \"{csproj}\"",
                WorkingDirectory = folder
            })!.WaitForExit();
            string assetsPath = Path.Combine(folder, "obj", "project.assets.json");
            JObject assets = (JObject)JToken.Parse(File.ReadAllBytes(assetsPath))!;
            foreach (var (name, package) in ((JObject)assets["targets"]![0]!).Properties)
            {
                MetadataReference? reference = GetReference(name, (JObject)package!, assets, folder, Options, compilationOptions);
                if (reference is not null) references.Add(reference);
            }
            IEnumerable<SyntaxTree> syntaxTrees = sourceFiles.OrderBy(p => p).Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), options: Options.GetParseOptions(), path: p));
            return CSharpCompilation.Create(assets["project"]!["restore"]!["projectName"]!.GetString(), syntaxTrees, references, compilationOptions);
        }

        private MetadataReference? GetReference(string name, JObject package, JObject assets, string folder, Options options, CSharpCompilationOptions compilationOptions)
        {
            string assemblyName = Path.GetDirectoryName(name)!;
            if (!MetaReferences.TryGetValue(assemblyName, out var reference))
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
                            IEnumerable<SyntaxTree> st = files.OrderBy(p => p).Select(p => Path.Combine(packagesPath, namePath, p)).Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), path: p));
                            CSharpCompilation cr = CSharpCompilation.Create(assemblyName, st, CommonReferences, compilationOptions);
                            reference = cr.ToMetadataReference();
                        }
                        break;
                    case "project":
                        string msbuildProject = assets["libraries"]![name]!["msbuildProject"]!.GetString();
                        msbuildProject = Path.GetFullPath(msbuildProject, folder);
                        reference = GetCompilation(msbuildProject).ToMetadataReference();
                        break;
                    default:
                        throw new NotSupportedException();
                }
                MetaReferences.Add(assemblyName, reference);
            }
            return reference;
        }

        private static bool IsSingleAbstractClass(IEnumerable<SyntaxTree> syntaxTrees)
        {
            if (syntaxTrees.Count() != 1) return false;

            var tree = syntaxTrees.First();
            var classDeclarations = tree.GetCompilationUnitRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

            return classDeclarations.Count == 1 && classDeclarations[0].Modifiers.Any(SyntaxKind.AbstractKeyword);
        }
    }
}
