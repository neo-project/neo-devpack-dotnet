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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using scfx::Neo.SmartContract.Framework;
using scfx::Neo.SmartContract.Framework.Attributes;
using Diagnostic = Microsoft.CodeAnalysis.Diagnostic;

namespace Neo.Compiler
{
    public class CompilationContext
    {
        private readonly CompilationEngine _engine;
        readonly INamedTypeSymbol _targetContract;
        internal Options Options => _engine.Options;
        private string? _displayName, _className;
        private readonly System.Collections.Generic.List<Diagnostic> _diagnostics = new();
        private readonly HashSet<string> _supportedStandards = new();
        private readonly System.Collections.Generic.List<AbiMethod> _methodsExported = new();
        private readonly System.Collections.Generic.List<AbiEvent> _eventsExported = new();
        private readonly PermissionBuilder _permissions = new();
        private readonly HashSet<string> _trusts = new();
        private readonly JObject _manifestExtra = new();
        // We can not reuse these converted methods as the offsets are determined while converting
        private readonly MethodConvertCollection _methodsConverted = new();
        private readonly MethodConvertCollection _methodsForward = new();
        private readonly System.Collections.Generic.List<MethodToken> _methodTokens = new();
        private readonly Dictionary<IFieldSymbol, byte> _staticFields = new(SymbolEqualityComparer.Default);
        private readonly System.Collections.Generic.List<byte> _anonymousStaticFields = new();
        private readonly Dictionary<ITypeSymbol, byte> _vtables = new(SymbolEqualityComparer.Default);
        private byte[]? _script;

        public bool Success => _diagnostics.All(p => p.Severity != DiagnosticSeverity.Error);
        public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;
        // TODO: basename should not work when multiple contracts exit in one project
        public string? ContractName => _displayName ?? Options.BaseName ?? _className;
        private string? Source { get; set; }

        internal IEnumerable<IFieldSymbol> StaticFieldSymbols => _staticFields.OrderBy(p => p.Value).Select(p => p.Key);
        internal IEnumerable<(byte, ITypeSymbol)> VTables => _vtables.OrderBy(p => p.Value).Select(p => (p.Value, p.Key));
        internal int StaticFieldCount => _staticFields.Count + _anonymousStaticFields.Count + _vtables.Count;
        private byte[] Script => _script ??= GetInstructions().Select(p => p.ToArray()).SelectMany(p => p).ToArray();


        /// <summary>
        /// Specify the contract to be compiled.
        /// </summary>
        /// <param name="engine"> CompilationEngine that contains the compilation syntax tree and compiled methods</param>
        /// <param name="targetContract">Contract to be compiled</param>
        internal CompilationContext(CompilationEngine engine, INamedTypeSymbol targetContract)
        {
            _engine = engine;
            _targetContract = targetContract;
        }

        private void RemoveEmptyInitialize()
        {
            int index = _methodsExported.FindIndex(p => p.Name == "_initialize");
            if (index < 0) return;
            AbiMethod method = _methodsExported[index];
            if (_methodsConverted[method.Symbol].Instructions.Count <= 1)
            {
                _methodsExported.RemoveAt(index);
                _methodsConverted.Remove(method.Symbol);
            }
        }

        private IEnumerable<Instruction> GetInstructions()
        {
            return _methodsConverted.SelectMany(p => p.Instructions).Concat(_methodsForward.SelectMany(p => p.Instructions));
        }

        private int GetAbiOffset(IMethodSymbol method)
        {
            if (!_methodsForward.TryGetValue(method, out MethodConvert? convert))
                convert = _methodsConverted[method];
            return convert.Instructions[0].Offset;
        }

        private static bool ValidateContractTrust(string value)
        {
            if (value == "*") return true;
            if (UInt160.TryParse(value, out _)) return true;
            if (ECPoint.TryParse(value, ECCurve.Secp256r1, out _)) return true;
            return false;
        }

        internal void Compile()
        {
            HashSet<INamedTypeSymbol> processed = new(SymbolEqualityComparer.Default);
            foreach (SyntaxTree tree in _engine.Compilation!.SyntaxTrees)
            {
                SemanticModel model = _engine.Compilation!.GetSemanticModel(tree);
                _diagnostics.AddRange(model.GetDiagnostics().Where(u => u.Severity != DiagnosticSeverity.Hidden));
                if (!Success) continue;
                try
                {
                    ProcessCompilationUnit(processed, model, tree.GetCompilationUnitRoot());
                }
                catch (CompilationException ex)
                {
                    _diagnostics.Add(ex.Diagnostic);
                }
            }
            if (Success)
            {
                RemoveEmptyInitialize();
                Instruction[] instructions = GetInstructions().ToArray();
                instructions.RebuildOffsets();
                if (!Options.NoOptimize) Optimizer.CompressJumps(instructions);
                instructions.RebuildOperands();
            }
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
                Tokens = _methodTokens.ToArray(),
                Script = Script
            };

            if (nef.Compiler.Length > 64)
            {
                // Neo.Compiler.CSharp 3.6.2+470d9a8608b41de658849994a258200d8abf7caa
                nef.Compiler = nef.Compiler.Substring(0, 61) + "...";
            }

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
            foreach (MethodConvert method in _methodsConverted)
            {
                builder.Append("// ");
                builder.AppendLine(method.Symbol.ToString());
                builder.AppendLine();
                WriteMethod(builder, method);
            }
            foreach (MethodConvert method in _methodsForward)
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
                ["supportedstandards"] = _supportedStandards.OrderBy(p => p).Select(p => (JString)p!).ToArray(),
                ["abi"] = new JObject
                {
                    ["methods"] = _methodsExported.Select(p => new JObject
                    {
                        ["name"] = p.Name,
                        ["offset"] = GetAbiOffset(p.Symbol),
                        ["safe"] = p.Safe,
                        ["returntype"] = p.ReturnType,
                        ["parameters"] = p.Parameters.Select(p => p.ToJson()).ToArray()
                    }).ToArray(),
                    ["events"] = _eventsExported.Select(p => new JObject
                    {
                        ["name"] = p.Name,
                        ["parameters"] = p.Parameters.Select(p => p.ToJson()).ToArray()
                    }).ToArray()
                },
                ["permissions"] = _permissions.ToJson(),
                ["trusts"] = _trusts.Contains("*") ? "*" : _trusts.OrderBy(p => p.Length).ThenBy(p => p).Select(u => new JString(u)).ToArray(),
                ["extra"] = _manifestExtra
            };

            // Ensure that is serializable
            return ContractManifest.Parse(json.ToString(false)).CheckStandards();
        }

        public JObject CreateDebugInformation(string folder = "")
        {
            System.Collections.Generic.List<string> documents = new();
            System.Collections.Generic.List<JObject> methods = new();
            foreach (var m in _methodsConverted.Where(p => p.SyntaxNode is not null))
            {
                System.Collections.Generic.List<JString> sequencePoints = new();
                foreach (var ins in m.Instructions.Where(i => i.SourceLocation?.SourceTree is not null))
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
                ["static-variables"] = _staticFields.OrderBy(p => p.Value).Select(p => ((JString)$"{p.Key.Name},{p.Key.Type.GetContractParameterType()},{p.Value}")!).ToArray(),
                ["methods"] = methods.ToArray(),
                ["events"] = _eventsExported.Select(e => new JObject
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
                // Considering that the complication will process all classes for every smart contract
                // it is possible to process multiple smart contract classes in the same project
                // As a result, we must stop the process if the current contract class is not the target contract
                // For example, if the target contract is "Contract1" and the project contains "Contract1" and "Contract2"
                // the process must skip when the "Contract2" class is processed
                if (_targetContract.Name != symbol.Name)
                {
                    return;
                }

                foreach (var attribute in symbol.GetAttributesWithInherited())
                {
                    if (attribute.AttributeClass!.IsSubclassOf(nameof(ManifestExtraAttribute)))
                    {
                        _manifestExtra[ManifestExtraAttribute.AttributeType[attribute.AttributeClass!.Name]] = (string)attribute.ConstructorArguments[0].Value!;
                        continue;
                    }

                    switch (attribute.AttributeClass!.Name)
                    {
                        case nameof(DisplayNameAttribute):
                            _displayName = (string)attribute.ConstructorArguments[0].Value!;
                            break;
                        case nameof(ContractSourceCodeAttribute):
                            Source = (string)attribute.ConstructorArguments[0].Value!;
                            break;
                        case nameof(ManifestExtraAttribute):
                            _manifestExtra[(string)attribute.ConstructorArguments[0].Value!] = (string)attribute.ConstructorArguments[1].Value!;
                            break;
                        case nameof(ContractPermissionAttribute):
                            _permissions.Add((string)attribute.ConstructorArguments[0].Value!, attribute.ConstructorArguments[1].Values.Select(p => (string)p.Value!).ToArray());
                            break;
                        case nameof(ContractTrustAttribute):
                            string trust = (string)attribute.ConstructorArguments[0].Value!;
                            if (!ValidateContractTrust(trust))
                                throw new ArgumentException($"The value {trust} is not a valid one for ContractTrust");
                            _trusts.Add(trust);
                            break;
                        case nameof(SupportedStandardsAttribute):
                            _supportedStandards.UnionWith(
                                attribute.ConstructorArguments[0].Values
                                    .Select(p => p.Value)
                                    .Select(p =>
                                        p is int ip && Enum.IsDefined(typeof(NepStandard), ip)
                                            ? ((NepStandard)ip).ToStandard()
                                            : p as string
                                    )
                                    .Where(v => v != null)! // Ensure null values are not added
                            );
                            break;
                    }
                }
                _className = symbol.Name;
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
                IMethodSymbol initialize = symbol.StaticConstructors.Length == 0
                    ? symbol.GetAllMembers().OfType<IMethodSymbol>().First(p => p.Name == "_initialize")
                    : symbol.StaticConstructors[0];
                ProcessMethod(model, initialize, true);
            }
        }

        private void ProcessEvent(IEventSymbol symbol)
        {
            if (symbol.DeclaredAccessibility != Accessibility.Public) return;
            INamedTypeSymbol type = (INamedTypeSymbol)symbol.Type;
            if (!type.DelegateInvokeMethod!.ReturnsVoid)
                throw new CompilationException(symbol, DiagnosticId.EventReturns, $"Event return value is not supported.");
            AddEvent(new AbiEvent(symbol), true);
        }

        internal void AddEvent(AbiEvent ev, bool throwErrorIfExists)
        {
            if (_eventsExported.Any(u => u.Name == ev.Name))
            {
                if (!throwErrorIfExists) return;
                throw new CompilationException(ev.Symbol, DiagnosticId.EventNameConflict, $"Duplicate event name: {ev.Name}.");
            }
            _eventsExported.Add(ev);
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
                if (_methodsExported.Any(u => u.Name == method.Name && u.Parameters.Length == method.Parameters.Length))
                    throw new CompilationException(symbol, DiagnosticId.MethodNameConflict, $"Duplicate method key: {method.Name},{method.Parameters.Length}.");
                _methodsExported.Add(method);
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
                _methodsForward.Add(forward);
            }
        }

        internal MethodConvert ConvertMethod(SemanticModel model, IMethodSymbol symbol)
        {
            if (!_methodsConverted.TryGetValue(symbol, out MethodConvert? method))
            {
                method = new MethodConvert(this, symbol);
                _methodsConverted.Add(method);
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
            int index = _methodTokens.FindIndex(p => p.Hash == hash && p.Method == method && p.ParametersCount == parametersCount && p.HasReturnValue == hasReturnValue && p.CallFlags == callFlags);
            if (index >= 0) return (ushort)index;
            _methodTokens.Add(new MethodToken
            {
                Hash = hash,
                Method = method,
                ParametersCount = parametersCount,
                HasReturnValue = hasReturnValue,
                CallFlags = callFlags
            });
            _permissions.Add(hash.ToString(), method);
            return (ushort)(_methodTokens.Count - 1);
        }

        internal byte AddStaticField(IFieldSymbol symbol)
        {
            if (!_staticFields.TryGetValue(symbol, out byte index))
            {
                index = (byte)StaticFieldCount;
                _staticFields.Add(symbol, index);
            }
            return index;
        }

        internal byte AddAnonymousStaticField()
        {
            byte index = (byte)StaticFieldCount;
            _anonymousStaticFields.Add(index);
            return index;
        }

        internal byte AddVTable(ITypeSymbol type)
        {
            if (!_vtables.TryGetValue(type, out byte index))
            {
                index = (byte)StaticFieldCount;
                _vtables.Add(type, index);
            }
            return index;
        }
    }
}
