// Copyright (C) 2015-2023 The Neo Project.
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
using Microsoft.CodeAnalysis.Text;
using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;
using Diagnostic = Microsoft.CodeAnalysis.Diagnostic;

namespace Neo.Compiler
{
    public class CompilationContext
    {
        private static readonly MetadataReference[] commonReferences;
        private static readonly Dictionary<string, MetadataReference> metaReferences = new();
        private readonly Compilation compilation;
        private string? displayName, className;
        private bool scTypeFound;
        private readonly List<Diagnostic> diagnostics = new();
        private readonly HashSet<string> supportedStandards = new();
        private readonly List<AbiMethod> methodsExported = new();
        private readonly HashSet<AbiEvent> eventsExported = new();
        private readonly PermissionBuilder permissions = new();
        private readonly HashSet<string> trusts = new();
        private readonly JObject manifestExtra = new();
        private readonly MethodConvertCollection methodsConverted = new();
        private readonly MethodConvertCollection methodsForward = new();
        private readonly List<MethodToken> methodTokens = new();
        private readonly Dictionary<IFieldSymbol, byte> staticFields = new(SymbolEqualityComparer.Default);
        private readonly List<byte> anonymousStaticFields = new();
        private readonly Dictionary<ITypeSymbol, byte> vtables = new(SymbolEqualityComparer.Default);
        private byte[]? script;

        public bool Success => diagnostics.All(p => p.Severity != DiagnosticSeverity.Error);
        public IReadOnlyList<Diagnostic> Diagnostics => diagnostics;
        public string? ContractName => displayName ?? Options.BaseName ?? className;
        private string? Source { get; set; }
        internal Options Options { get; private set; }
        internal IEnumerable<IFieldSymbol> StaticFieldSymbols => staticFields.OrderBy(p => p.Value).Select(p => p.Key);
        internal IEnumerable<(byte, ITypeSymbol)> VTables => vtables.OrderBy(p => p.Value).Select(p => (p.Value, p.Key));
        internal int StaticFieldCount => staticFields.Count + anonymousStaticFields.Count + vtables.Count;
        private byte[] Script => script ??= GetInstructions().Select(p => p.ToArray()).SelectMany(p => p).ToArray();

        public void AddEvent(AbiEvent abiEvent) => eventsExported.Add(abiEvent);
        static CompilationContext()
        {
            string coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            commonReferences = new[]
            {
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.InteropServices.dll")),
                MetadataReference.CreateFromFile(typeof(string).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DisplayNameAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(BigInteger).Assembly.Location)
            };
        }

        private CompilationContext(Compilation compilation, Options options)
        {
            this.compilation = compilation;
            this.Options = options;
        }

        private void RemoveEmptyInitialize()
        {
            int index = methodsExported.FindIndex(p => p.Name == "_initialize");
            if (index < 0) return;
            AbiMethod method = methodsExported[index];
            if (methodsConverted[method.Symbol].Instructions.Count <= 1)
            {
                methodsExported.RemoveAt(index);
                methodsConverted.Remove(method.Symbol);
            }
        }

        private IEnumerable<Instruction> GetInstructions()
        {
            return methodsConverted.SelectMany(p => p.Instructions).Concat(methodsForward.SelectMany(p => p.Instructions));
        }

        private int GetAbiOffset(IMethodSymbol method)
        {
            if (!methodsForward.TryGetValue(method, out MethodConvert? convert))
                convert = methodsConverted[method];
            return convert.Instructions[0].Offset;
        }

        private static bool ValidateContractTrust(string value)
        {
            if (value == "*") return true;
            if (UInt160.TryParse(value, out _)) return true;
            if (ECPoint.TryParse(value, ECCurve.Secp256r1, out _)) return true;
            return false;
        }

        private void Compile()
        {
            HashSet<INamedTypeSymbol> processed = new(SymbolEqualityComparer.Default);
            foreach (SyntaxTree tree in compilation.SyntaxTrees)
            {
                SemanticModel model = compilation.GetSemanticModel(tree);
                diagnostics.AddRange(model.GetDiagnostics());
                if (!Success) continue;
                try
                {
                    ProcessCompilationUnit(processed, model, tree.GetCompilationUnitRoot());
                }
                catch (CompilationException ex)
                {
                    diagnostics.Add(ex.Diagnostic);
                }
            }
            if (Success)
            {
                if (!scTypeFound)
                {
                    diagnostics.Add(Diagnostic.Create(DiagnosticId.NoEntryPoint, DiagnosticCategory.Default, "No SmartContract is found in the sources.", DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0));
                    return;
                }
                RemoveEmptyInitialize();
                Instruction[] instructions = GetInstructions().ToArray();
                instructions.RebuildOffsets();
                if (!Options.NoOptimize) Optimizer.CompressJumps(instructions);
                instructions.RebuildOperands();
            }
        }

        internal static CompilationContext Compile(IEnumerable<string> sourceFiles, IEnumerable<MetadataReference> references, Options options)
        {
            IEnumerable<SyntaxTree> syntaxTrees = sourceFiles.OrderBy(p => p).Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), options: options.GetParseOptions(), path: p));
            if (IsSingleAbstractClass(syntaxTrees)) throw new FormatException("The given class is abstract, no valid neo SmartContract found.");
            CSharpCompilationOptions compilationOptions = new(OutputKind.DynamicallyLinkedLibrary, deterministic: true, nullableContextOptions: options.Nullable);
            CSharpCompilation compilation = CSharpCompilation.Create(null, syntaxTrees, references, compilationOptions);
            CompilationContext context = new(compilation, options);
            context.Compile();
            return context;
        }

        public static CompilationContext CompileSources(string[] sourceFiles, Options options)
        {
            List<MetadataReference> references = new(commonReferences);
            references.Add(MetadataReference.CreateFromFile(typeof(scfx.Neo.SmartContract.Framework.SmartContract).Assembly.Location));
            return Compile(sourceFiles, references, options);
        }

        public static Compilation GetCompilation(string csproj, Options options)
        {
            string folder = Path.GetDirectoryName(csproj)!;
            string obj = Path.Combine(folder, "obj");
            HashSet<string> sourceFiles = Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories)
                .Where(p => !p.StartsWith(obj))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            List<MetadataReference> references = new(commonReferences);
            CSharpCompilationOptions compilationOptions = new(OutputKind.DynamicallyLinkedLibrary, deterministic: true, nullableContextOptions: options.Nullable);
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
                MetadataReference? reference = GetReference(name, (JObject)package!, assets, folder, options, compilationOptions);
                if (reference is not null) references.Add(reference);
            }
            IEnumerable<SyntaxTree> syntaxTrees = sourceFiles.OrderBy(p => p).Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), options: options.GetParseOptions(), path: p));
            return CSharpCompilation.Create(assets["project"]!["restore"]!["projectName"]!.GetString(), syntaxTrees, references, compilationOptions);
        }

        private static MetadataReference? GetReference(string name, JObject package, JObject assets, string folder, Options options, CSharpCompilationOptions compilationOptions)
        {
            string assemblyName = Path.GetDirectoryName(name)!;
            if (!metaReferences.TryGetValue(assemblyName, out var reference))
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
                            CSharpCompilation cr = CSharpCompilation.Create(assemblyName, st, commonReferences, compilationOptions);
                            reference = cr.ToMetadataReference();
                        }
                        break;
                    case "project":
                        string msbuildProject = assets["libraries"]![name]!["msbuildProject"]!.GetString();
                        msbuildProject = Path.GetFullPath(msbuildProject, folder);
                        reference = GetCompilation(msbuildProject, options).ToMetadataReference();
                        break;
                    default:
                        throw new NotSupportedException();
                }
                metaReferences.Add(assemblyName, reference);
            }
            return reference;
        }

        public static CompilationContext CompileProject(string csproj, Options options)
        {
            Compilation compilation = GetCompilation(csproj, options);
            CompilationContext context = new(compilation, options);
            context.Compile();
            return context;
        }

        public NefFile CreateExecutable()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var titleAttribute = assembly.GetCustomAttribute<AssemblyTitleAttribute>()!;
            var versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!;
            NefFile nef = new()
            {
                Compiler = $"{titleAttribute.Title} {versionAttribute.InformationalVersion}",
                Source = Source ?? string.Empty,
                Tokens = methodTokens.ToArray(),
                Script = Script
            };
            nef.CheckSum = NefFile.ComputeChecksum(nef);
            // Ensure that is serializable
            return nef.ToArray().AsSerializable<NefFile>();
        }

        public string CreateAssembly()
        {
            static void WriteMethod(StringBuilder builder, MethodConvert method)
            {
                foreach (Instruction i in method.Instructions)
                {
                    builder.Append($"{i.Offset:x8}: ");
                    i.ToString(builder);
                    builder.AppendLine();
                }
                builder.AppendLine();
                builder.AppendLine();
            }
            StringBuilder builder = new();
            foreach (MethodConvert method in methodsConverted)
            {
                builder.Append("// ");
                builder.AppendLine(method.Symbol.ToString());
                builder.AppendLine();
                WriteMethod(builder, method);
            }
            foreach (MethodConvert method in methodsForward)
            {
                builder.Append("// ");
                builder.Append(method.Symbol.ToString());
                builder.AppendLine(" (Forward)");
                builder.AppendLine();
                WriteMethod(builder, method);
            }
            return builder.ToString();
        }

        public ContractManifest CreateManifest()
        {
            JObject json = new()
            {
                ["name"] = ContractName,
                ["groups"] = new JArray(),
                ["features"] = new JObject(),
                ["supportedstandards"] = supportedStandards.OrderBy(p => p).Select(p => (JString)p!).ToArray(),
                ["abi"] = new JObject
                {
                    ["methods"] = methodsExported.Select(p => new JObject
                    {
                        ["name"] = p.Name,
                        ["offset"] = GetAbiOffset(p.Symbol),
                        ["safe"] = p.Safe,
                        ["returntype"] = p.ReturnType,
                        ["parameters"] = p.Parameters.Select(p => p.ToJson()).ToArray()
                    }).ToArray(),
                    ["events"] = eventsExported.Select(p => new JObject
                    {
                        ["name"] = p.Name,
                        ["parameters"] = p.Parameters.Select(p => p.ToJson()).ToArray()
                    }).ToArray()
                },
                ["permissions"] = permissions.ToJson(),
                ["trusts"] = trusts.Contains("*") ? "*" : trusts.OrderBy(p => p.Length).ThenBy(p => p).Select(u => new JString(u)).ToArray(),
                ["extra"] = manifestExtra
            };
            // Ensure that is serializable
            return ContractManifest.Parse(json.ToString(false));
        }

        public JObject CreateDebugInformation(string folder = "")
        {
            List<string> documents = new();
            List<JObject> methods = new();
            foreach (var m in methodsConverted.Where(p => p.SyntaxNode is not null))
            {
                List<JString> sequencePoints = new();
                foreach (var ins in m.Instructions.Where(i => i.SourceLocation is not null))
                {
                    var doc = ins.SourceLocation!.SourceTree!.FilePath;
                    if (!string.IsNullOrEmpty(folder))
                    {
                        doc = Path.GetRelativePath(folder, doc);
                    }

                    var index = documents.IndexOf(doc);
                    if (index == -1)
                    {
                        index = documents.Count;
                        documents.Add(doc);
                    }

                    FileLinePositionSpan span = ins.SourceLocation!.GetLineSpan();
                    var str = $"{ins.Offset}[{index}]{ToRangeString(span.StartLinePosition)}-{ToRangeString(span.EndLinePosition)}";
                    sequencePoints.Add(new JString(str));

                    static string ToRangeString(LinePosition pos) => $"{pos.Line + 1}:{pos.Character + 1}";
                }

                methods.Add(new JObject
                {
                    ["id"] = m.Symbol.ToString(),
                    ["name"] = $"{m.Symbol.ContainingType},{m.Symbol.Name}",
                    ["range"] = $"{m.Instructions[0].Offset}-{m.Instructions[^1].Offset}",
                    ["params"] = (m.Symbol.IsStatic ? Array.Empty<string>() : new string[] { "this,Any" })
                        .Concat(m.Symbol.Parameters.Select(p => $"{p.Name},{p.Type.GetContractParameterType()}"))
                        .Select((p, i) => ((JString)$"{p},{i}")!)
                        .ToArray(),
                    ["return"] = m.Symbol.ReturnType.GetContractParameterType().ToString(),
                    ["variables"] = m.Variables.Select(p => ((JString)$"{p.Symbol.Name},{p.Symbol.Type.GetContractParameterType()},{p.SlotIndex}")!).ToArray(),
                    ["sequence-points"] = sequencePoints.ToArray(),
                });
            }

            return new JObject
            {
                ["hash"] = Script.ToScriptHash().ToString(),
                ["documents"] = documents.Select(p => (JString)p!).ToArray(),
                ["document-root"] = string.IsNullOrEmpty(folder) ? JToken.Null : folder,
                ["static-variables"] = staticFields.OrderBy(p => p.Value).Select(p => ((JString)$"{p.Key.Name},{p.Key.Type.GetContractParameterType()},{p.Value}")!).ToArray(),
                ["methods"] = methods.ToArray(),
                ["events"] = eventsExported.Select(e => new JObject
                {
                    ["id"] = e.Name,
                    ["name"] = $"{e.Symbol.ContainingType},{e.Symbol.Name}",
                    ["params"] = e.Parameters.Select((p, i) => ((JString)$"{p.Name},{p.Type},{i}")!).ToArray()
                }).ToArray()
            };
        }

        private void ProcessCompilationUnit(HashSet<INamedTypeSymbol> processed, SemanticModel model, CompilationUnitSyntax syntax)
        {
            foreach (MemberDeclarationSyntax member in syntax.Members)
                ProcessMemberDeclaration(processed, model, member);
        }

        private void ProcessMemberDeclaration(HashSet<INamedTypeSymbol> processed, SemanticModel model, MemberDeclarationSyntax syntax)
        {
            switch (syntax)
            {
                case BaseNamespaceDeclarationSyntax @namespace:
                    foreach (MemberDeclarationSyntax member in @namespace.Members)
                        ProcessMemberDeclaration(processed, model, member);
                    break;
                case ClassDeclarationSyntax @class:
                    INamedTypeSymbol symbol = model.GetDeclaredSymbol(@class)!;
                    if (processed.Add(symbol)) ProcessClass(model, symbol);
                    break;
            }
        }

        private void ProcessClass(SemanticModel model, INamedTypeSymbol symbol)
        {
            if (symbol.IsSubclassOf(nameof(Attribute))) return;
            bool isPublic = symbol.DeclaredAccessibility == Accessibility.Public;
            bool isAbstract = symbol.IsAbstract;
            bool isContractType = symbol.IsSubclassOf(nameof(scfx.Neo.SmartContract.Framework.SmartContract));
            bool isSmartContract = isPublic && !isAbstract && isContractType;
            if (isSmartContract)
            {
                if (scTypeFound) throw new CompilationException(DiagnosticId.MultiplyContracts, $"Only one smart contract is allowed.");
                scTypeFound = true;
                foreach (var attribute in symbol.GetAttributesWithInherited())
                {
                    switch (attribute.AttributeClass!.Name)
                    {
                        case nameof(DisplayNameAttribute):
                            displayName = (string)attribute.ConstructorArguments[0].Value!;
                            break;
                        case nameof(scfx.Neo.SmartContract.Framework.Attributes.ContractSourceCodeAttribute):
                            Source = (string)attribute.ConstructorArguments[0].Value!;
                            break;
                        case nameof(scfx.Neo.SmartContract.Framework.Attributes.ManifestExtraAttribute):
                            manifestExtra[(string)attribute.ConstructorArguments[0].Value!] = (string)attribute.ConstructorArguments[1].Value!;
                            break;
                        case nameof(scfx.Neo.SmartContract.Framework.Attributes.ContractPermissionAttribute):
                            permissions.Add((string)attribute.ConstructorArguments[0].Value!, attribute.ConstructorArguments[1].Values.Select(p => (string)p.Value!).ToArray());
                            break;
                        case nameof(scfx.Neo.SmartContract.Framework.Attributes.ContractTrustAttribute):
                            string trust = (string)attribute.ConstructorArguments[0].Value!;
                            if (!ValidateContractTrust(trust))
                                throw new ArgumentException($"The value {trust} is not a valid one for ContractTrust");
                            trusts.Add(trust);
                            break;
                        case nameof(scfx.Neo.SmartContract.Framework.Attributes.SupportedStandardsAttribute):
                            supportedStandards.UnionWith(attribute.ConstructorArguments[0].Values.Select(p => (string)p.Value!));
                            break;
                    }
                }
                className = symbol.Name;
            }
            foreach (ISymbol member in symbol.GetAllMembers())
            {
                switch (member)
                {
                    case IEventSymbol @event when isSmartContract:
                        ProcessEvent(@event);
                        break;
                    case IMethodSymbol method when method.Name != "_initialize" && method.MethodKind != MethodKind.StaticConstructor:
                        ProcessMethod(model, method, isSmartContract);
                        break;
                }
            }
            if (isSmartContract)
            {
                IMethodSymbol _initialize = symbol.StaticConstructors.Length == 0
                    ? symbol.GetAllMembers().OfType<IMethodSymbol>().First(p => p.Name == "_initialize")
                    : symbol.StaticConstructors[0];
                ProcessMethod(model, _initialize, true);
            }
        }

        private void ProcessEvent(IEventSymbol symbol)
        {
            if (symbol.DeclaredAccessibility != Accessibility.Public) return;
            INamedTypeSymbol type = (INamedTypeSymbol)symbol.Type;
            if (!type.DelegateInvokeMethod!.ReturnsVoid)
                throw new CompilationException(symbol, DiagnosticId.EventReturns, $"Event return value is not supported.");
            AbiEvent ev = new(symbol);
            if (eventsExported.Any(u => u.Name == ev.Name))
                throw new CompilationException(symbol, DiagnosticId.EventNameConflict, $"Duplicate event name: {ev.Name}.");
            eventsExported.Add(ev);
        }

        private void ProcessMethod(SemanticModel model, IMethodSymbol symbol, bool export)
        {
            if (symbol.IsAbstract) return;
            if (symbol.MethodKind != MethodKind.StaticConstructor)
            {
                if (symbol.DeclaredAccessibility != Accessibility.Public)
                    export = false;
                if (symbol.MethodKind != MethodKind.Ordinary && symbol.MethodKind != MethodKind.PropertyGet && symbol.MethodKind != MethodKind.PropertySet)
                    return;
            }
            if (export)
            {
                AbiMethod method = new(symbol);
                if (methodsExported.Any(u => u.Name == method.Name && u.Parameters.Length == method.Parameters.Length))
                    throw new CompilationException(symbol, DiagnosticId.MethodNameConflict, $"Duplicate method key: {method.Name},{method.Parameters.Length}.");
                methodsExported.Add(method);
            }

            if (symbol.GetAttributesWithInherited()
                .Any(p => p.AttributeClass?.Name == nameof(MethodImplAttribute)
                    && p.ConstructorArguments[0].Value is not null
                    && (MethodImplOptions)p.ConstructorArguments[0].Value! == MethodImplOptions.AggressiveInlining))
            {
                if (export)
                    throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: Can not set contract interface {symbol.Name} as inline.");
                return;
            }

            MethodConvert convert = ConvertMethod(model, symbol);
            if (export && !symbol.IsStatic)
            {
                MethodConvert forward = new(this, symbol);
                forward.ConvertForward(model, convert);
                methodsForward.Add(forward);
            }
        }

        internal MethodConvert ConvertMethod(SemanticModel model, IMethodSymbol symbol)
        {
            if (!methodsConverted.TryGetValue(symbol, out MethodConvert? method))
            {
                method = new MethodConvert(this, symbol);
                methodsConverted.Add(method);
                if (!symbol.DeclaringSyntaxReferences.IsEmpty)
                {
                    ISourceAssemblySymbol assembly = (ISourceAssemblySymbol)symbol.ContainingAssembly;
                    model = assembly.Compilation.GetSemanticModel(symbol.DeclaringSyntaxReferences[0].SyntaxTree);
                }
                method.Convert(model);
            }
            return method;
        }

        internal ushort AddMethodToken(UInt160 hash, string method, ushort parametersCount, bool hasReturnValue, CallFlags callFlags)
        {
            int index = methodTokens.FindIndex(p => p.Hash == hash && p.Method == method && p.ParametersCount == parametersCount && p.HasReturnValue == hasReturnValue && p.CallFlags == callFlags);
            if (index >= 0) return (ushort)index;
            methodTokens.Add(new MethodToken
            {
                Hash = hash,
                Method = method,
                ParametersCount = parametersCount,
                HasReturnValue = hasReturnValue,
                CallFlags = callFlags
            });
            permissions.Add(hash.ToString(), method);
            return (ushort)(methodTokens.Count - 1);
        }

        internal byte AddStaticField(IFieldSymbol symbol)
        {
            if (!staticFields.TryGetValue(symbol, out byte index))
            {
                index = (byte)StaticFieldCount;
                staticFields.Add(symbol, index);
            }
            return index;
        }

        internal byte AddAnonymousStaticField()
        {
            byte index = (byte)StaticFieldCount;
            anonymousStaticFields.Add(index);
            return index;
        }

        internal byte AddVTable(ITypeSymbol type)
        {
            if (!vtables.TryGetValue(type, out byte index))
            {
                index = (byte)StaticFieldCount;
                vtables.Add(type, index);
            }
            return index;
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
