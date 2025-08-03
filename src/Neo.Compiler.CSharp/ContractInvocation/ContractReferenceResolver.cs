// Copyright (C) 2015-2025 The Neo Project.
//
// ContractReferenceResolver.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Neo.SmartContract.Framework.ContractInvocation;
using Neo.SmartContract.Framework.ContractInvocation.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neo.Compiler.ContractInvocation
{
    /// <summary>
    /// Resolves contract references during compilation.
    /// This class handles the discovery and resolution of contract dependencies.
    /// </summary>
    public class ContractReferenceResolver
    {
        private readonly CompilationContext _context;
        private readonly Dictionary<string, IContractReference> _resolvedReferences;
        private readonly List<DependencyContract> _dependencyContracts;

        /// <summary>
        /// Initializes a new ContractReferenceResolver.
        /// </summary>
        /// <param name="context">The compilation context</param>
        public ContractReferenceResolver(CompilationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _resolvedReferences = new Dictionary<string, IContractReference>();
            _dependencyContracts = new List<DependencyContract>();
        }

        /// <summary>
        /// Gets all resolved contract references.
        /// </summary>
        public IReadOnlyDictionary<string, IContractReference> ResolvedReferences => _resolvedReferences;

        /// <summary>
        /// Gets all dependency contracts that need to be compiled.
        /// </summary>
        public IReadOnlyList<DependencyContract> DependencyContracts => _dependencyContracts;

        /// <summary>
        /// Discovers and resolves all contract references in the compilation.
        /// </summary>
        /// <param name="compilation">The compilation to analyze</param>
        public void ResolveReferences(Compilation compilation)
        {
            if (compilation == null)
                throw new ArgumentNullException(nameof(compilation));

            // Find all contract reference attributes
            var contractReferences = FindContractReferences(compilation);

            // Resolve each reference
            foreach (var (attribute, symbol) in contractReferences)
            {
                ResolveReference(attribute, symbol);
            }

            // Resolve dependency contracts
            ResolveDependencyContracts();
        }

        /// <summary>
        /// Resolves a contract hash for the specified reference.
        /// </summary>
        /// <param name="reference">The contract reference to resolve</param>
        /// <returns>The resolved contract hash, or null if not resolvable</returns>
        public UInt160? ResolveContractHash(IContractReference reference)
        {
            if (reference == null)
                return null;

            return reference switch
            {
                DevelopmentContractReference devRef => ResolveDevelopmentContractHash(devRef),
                DeployedContractReference deployedRef => ResolveDeployedContractHash(deployedRef),
                _ => null
            };
        }

        /// <summary>
        /// Adds a resolved contract hash for a development contract.
        /// </summary>
        /// <param name="identifier">The contract identifier</param>
        /// <param name="hash">The resolved contract hash</param>
        public void AddResolvedHash(string identifier, UInt160 hash)
        {
            if (_resolvedReferences.TryGetValue(identifier, out var reference) && 
                reference is DevelopmentContractReference devRef)
            {
                devRef.ResolveHash(hash);
            }
        }

        private List<(ContractReferenceAttribute, ISymbol)> FindContractReferences(Compilation compilation)
        {
            var references = new List<(ContractReferenceAttribute, ISymbol)>();

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
                var root = syntaxTree.GetRoot();

                // Find all field and property declarations with ContractReference attributes
                var members = root.DescendantNodes()
                    .Where(n => n.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.FieldDeclaration) ||
                               n.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PropertyDeclaration));

                foreach (var member in members)
                {
                    var symbol = semanticModel.GetDeclaredSymbol(member);
                    if (symbol == null) continue;

                    var attributes = symbol.GetAttributes();
                    foreach (var attr in attributes)
                    {
                        if (IsContractReferenceAttribute(attr))
                        {
                            var contractRefAttr = CreateContractReferenceAttribute(attr);
                            if (contractRefAttr != null)
                            {
                                references.Add((contractRefAttr, symbol));
                            }
                        }
                    }
                }
            }

            return references;
        }

        private void ResolveReference(ContractReferenceAttribute attribute, ISymbol symbol)
        {
            try
            {
                var reference = ContractInvocationFactory.CreateFromAttribute(attribute);
                _resolvedReferences[attribute.Identifier] = reference;

                // If it's a development contract, add it to dependencies
                if (reference is DevelopmentContractReference devRef && devRef.CompileAsDependency)
                {
                    _dependencyContracts.Add(new DependencyContract(devRef.Identifier, devRef.ProjectPath, symbol));
                }

                // If it's a deployed contract with manifest fetching enabled, fetch the manifest
                if (reference is DeployedContractReference deployedRef && deployedRef.FetchManifest)
                {
                    FetchContractManifest(deployedRef);
                }
            }
            catch (Exception ex)
            {
                _context.ReportError($"Failed to resolve contract reference '{attribute.Identifier}': {ex.Message}");
            }
        }

        private void ResolveDependencyContracts()
        {
            foreach (var dependency in _dependencyContracts)
            {
                try
                {
                    var projectPath = ResolveProjectPath(dependency.ProjectPath);
                    if (projectPath != null)
                    {
                        dependency.ResolvedProjectPath = projectPath;
                    }
                    else
                    {
                        _context.ReportWarning($"Could not resolve project path for contract dependency '{dependency.Identifier}': {dependency.ProjectPath}");
                    }
                }
                catch (Exception ex)
                {
                    _context.ReportError($"Failed to resolve dependency contract '{dependency.Identifier}': {ex.Message}");
                }
            }
        }

        private UInt160? ResolveDevelopmentContractHash(DevelopmentContractReference reference)
        {
            // For development contracts, the hash will be available after compilation
            return reference.ResolvedHash;
        }

        private UInt160? ResolveDeployedContractHash(DeployedContractReference reference)
        {
            // For deployed contracts, use the network context to get the current address
            return reference.NetworkContext.GetCurrentNetworkAddress();
        }

        private void FetchContractManifest(DeployedContractReference reference)
        {
            var contractHash = reference.NetworkContext.GetCurrentNetworkAddress();
            if (contractHash == null)
            {
                _context.ReportWarning($"Cannot fetch manifest for contract '{reference.Identifier}': no address configured for current network");
                return;
            }

            try
            {
                var blockchainInterface = _context.GetBlockchainInterface();
                var manifest = blockchainInterface.GetContractManifest(contractHash);
                if (manifest != null)
                {
                    reference.SetManifest(manifest);
                }
                else
                {
                    _context.ReportWarning($"Could not fetch manifest for contract '{reference.Identifier}' at address {contractHash}");
                }
            }
            catch (Exception ex)
            {
                _context.ReportWarning($"Failed to fetch manifest for contract '{reference.Identifier}': {ex.Message}");
            }
        }

        private string? ResolveProjectPath(string projectPath)
        {
            if (string.IsNullOrEmpty(projectPath))
                return null;

            // If it's already an absolute path and exists, return it
            if (Path.IsPathRooted(projectPath) && (File.Exists(projectPath) || Directory.Exists(projectPath)))
                return projectPath;

            // Try to resolve relative to the current project directory
            var currentProjectDir = _context.ProjectDirectory;
            if (!string.IsNullOrEmpty(currentProjectDir))
            {
                var resolvedPath = Path.Combine(currentProjectDir, projectPath);
                if (File.Exists(resolvedPath) || Directory.Exists(resolvedPath))
                    return Path.GetFullPath(resolvedPath);

                // Try adding .csproj extension if it's a directory
                if (Directory.Exists(resolvedPath))
                {
                    var projectName = Path.GetFileName(resolvedPath);
                    var csprojPath = Path.Combine(resolvedPath, $"{projectName}.csproj");
                    if (File.Exists(csprojPath))
                        return csprojPath;
                }
            }

            return null;
        }

        private bool IsContractReferenceAttribute(AttributeData attribute)
        {
            return attribute.AttributeClass?.Name == nameof(ContractReferenceAttribute) &&
                   attribute.AttributeClass.ContainingNamespace?.ToDisplayString() == 
                   "Neo.SmartContract.Framework.ContractInvocation.Attributes";
        }

        private ContractReferenceAttribute? CreateContractReferenceAttribute(AttributeData attribute)
        {
            try
            {
                if (attribute.ConstructorArguments.Length == 0)
                    return null;

                var identifier = attribute.ConstructorArguments[0].Value?.ToString();
                if (string.IsNullOrEmpty(identifier))
                    return null;

                var contractRefAttr = new ContractReferenceAttribute(identifier);

                // Parse named arguments
                foreach (var namedArg in attribute.NamedArguments)
                {
                    switch (namedArg.Key)
                    {
                        case nameof(ContractReferenceAttribute.ReferenceType):
                            if (namedArg.Value.Value is int refTypeValue)
                                contractRefAttr.ReferenceType = (ContractReferenceType)refTypeValue;
                            break;
                        case nameof(ContractReferenceAttribute.ProjectPath):
                            contractRefAttr.ProjectPath = namedArg.Value.Value?.ToString();
                            break;
                        case nameof(ContractReferenceAttribute.PrivnetAddress):
                            contractRefAttr.PrivnetAddress = namedArg.Value.Value?.ToString();
                            break;
                        case nameof(ContractReferenceAttribute.TestnetAddress):
                            contractRefAttr.TestnetAddress = namedArg.Value.Value?.ToString();
                            break;
                        case nameof(ContractReferenceAttribute.MainnetAddress):
                            contractRefAttr.MainnetAddress = namedArg.Value.Value?.ToString();
                            break;
                        case nameof(ContractReferenceAttribute.FetchManifest):
                            if (namedArg.Value.Value is bool fetchManifest)
                                contractRefAttr.FetchManifest = fetchManifest;
                            break;
                        case nameof(ContractReferenceAttribute.CompileAsDependency):
                            if (namedArg.Value.Value is bool compileAsDependency)
                                contractRefAttr.CompileAsDependency = compileAsDependency;
                            break;
                    }
                }

                return contractRefAttr;
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Represents a dependency contract that needs to be compiled.
    /// </summary>
    public class DependencyContract
    {
        /// <summary>
        /// Gets the contract identifier.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Gets the original project path.
        /// </summary>
        public string ProjectPath { get; }

        /// <summary>
        /// Gets the symbol that references this contract.
        /// </summary>
        public ISymbol Symbol { get; }

        /// <summary>
        /// Gets or sets the resolved project path.
        /// </summary>
        public string? ResolvedProjectPath { get; set; }

        /// <summary>
        /// Initializes a new DependencyContract.
        /// </summary>
        /// <param name="identifier">The contract identifier</param>
        /// <param name="projectPath">The project path</param>
        /// <param name="symbol">The referencing symbol</param>
        public DependencyContract(string identifier, string projectPath, ISymbol symbol)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            ProjectPath = projectPath ?? throw new ArgumentNullException(nameof(projectPath));
            Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        }
    }
}