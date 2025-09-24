// Copyright (C) 2015-2025 The Neo Project.
//
// ContractProjectTestBase.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.Coverage;
using Neo.SmartContract.Testing.Extensions;
using Neo.SmartContract.Testing.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Neo.SmartContract.Testing.RuntimeCompilation;

/// <summary>
/// Test base that compiles smart contract projects on demand and exposes a <see langword="dynamic"/>
/// contract instance for interaction without requiring pre-generated artifacts.
/// </summary>
public abstract class ContractProjectTestBase
{
    private readonly List<string> _contractLogs = [];
    private bool _initialized;

    protected ContractProjectTestBase(string projectPath, string? contractName = null, ArtifactBuildOptions? buildOptions = null)
    {
        if (string.IsNullOrWhiteSpace(projectPath))
            throw new ArgumentException("Project path can't be null or empty.", nameof(projectPath));

        ProjectPath = ResolveProjectPath(projectPath);
        ContractName = contractName;
        BuildOptions = buildOptions ?? ArtifactBuildOptions.Default;
    }

    /// <summary>
    /// Global coverage accumulator for the current contract.
    /// </summary>
    public static CoveredContract? Coverage { get; private set; }

    public static Signer Alice { get; set; } = TestEngine.GetNewSigner();
    public static Signer Bob { get; set; } = TestEngine.GetNewSigner();

    protected string ProjectPath { get; }
    protected string? ContractName { get; }
    protected ArtifactBuildOptions BuildOptions { get; private set; }

    protected ContractArtifacts Artifacts { get; private set; } = default!;
    public NefFile NefFile { get; private set; } = default!;
    public ContractManifest Manifest { get; private set; } = default!;
    public NeoDebugInfo? DebugInfo { get; private set; }
    public TestEngine Engine { get; private set; } = default!;
    public dynamic Contract { get; private set; } = default!;
    public SmartContractStorage Storage => ((SmartContract)Contract).Storage;
    public UInt160 ContractHash => ((SmartContract)Contract).Hash;

    /// <summary>
    /// Override to tweak build parameters before compilation.
    /// </summary>
    protected virtual ArtifactBuildOptions ConfigureBuildOptions(ArtifactBuildOptions options) => options;

    /// <summary>
    /// Override to configure the underlying test engine (signers, native contracts, etc.).
    /// </summary>
    protected virtual TestEngine CreateTestEngine()
    {
        var engine = new TestEngine(true);
        engine.SetTransactionSigners(Alice);
        return engine;
    }

    /// <summary>
    /// Called after the contract has been compiled and deployed.
    /// </summary>
    protected virtual void OnContractDeployed()
    {
    }

    /// <summary>
    /// Ensures the contract project is compiled, deployed and available for the current test.
    /// Call this from <c>[TestInitialize]</c> methods.
    /// </summary>
    protected void EnsureContractDeployed()
    {
        if (_initialized)
            return;

        BuildOptions = ConfigureBuildOptions(BuildOptions);
        var effectiveOptions = BuildOptions with { ForceRebuild = true };
        Artifacts = SmartContractProjectLoader.LoadContract(ProjectPath, ContractName, effectiveOptions);

        NefFile = Artifacts.Nef;
        Manifest = Artifacts.Manifest;
        DebugInfo = Artifacts.DebugInfo;

        Engine = CreateTestEngine();
        Contract = DeployContract(Artifacts);
        ((SmartContract)Contract).OnRuntimeLog += Contract_OnRuntimeLog;

        if (Coverage is null)
        {
            Coverage = ((SmartContract)Contract).GetCoverage();
            Assert.IsNotNull(Coverage);
        }

        _initialized = true;
        OnContractDeployed();
    }

    private dynamic DeployContract(ContractArtifacts artifacts)
    {
        var deployMethod = typeof(TestEngine)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .First(m =>
            {
                if (m.Name != nameof(TestEngine.Deploy) || !m.IsGenericMethodDefinition)
                    return false;
                var parameters = m.GetParameters();
                return parameters.Length >= 2 &&
                       parameters[0].ParameterType == typeof(NefFile) &&
                       parameters[1].ParameterType == typeof(ContractManifest);
            });

        var generic = deployMethod.MakeGenericMethod(artifacts.ProxyType);
        return generic.Invoke(Engine, new object?[] { artifacts.Nef, artifacts.Manifest, null, null })
            ?? throw new InvalidOperationException("Failed to deploy contract under test.");
    }

    private void Contract_OnRuntimeLog(UInt160 sender, string message)
    {
        _contractLogs.Add(message);
    }

    public void AssertLogs(params string[] logs)
    {
        Assert.AreEqual(logs.Length, _contractLogs.Count);
        CollectionAssert.AreEqual(_contractLogs, logs);
        _contractLogs.Clear();
    }

    public void AssertNoLogs()
    {
        Assert.AreEqual(0, _contractLogs.Count);
    }

    [TestCleanup]
    public virtual void OnCleanup()
    {
        if (!_initialized)
            return;

        var smartContract = (SmartContract)Contract;
        smartContract.OnRuntimeLog -= Contract_OnRuntimeLog;
        Coverage?.Join(smartContract.GetCoverage());
    }

    private static string ResolveProjectPath(string projectPath)
    {
        if (Path.IsPathRooted(projectPath))
            return projectPath;

        var baseDirectory = TryGetTestProjectDirectory() ?? AppContext.BaseDirectory;
        return Path.GetFullPath(Path.Combine(baseDirectory, projectPath));
    }

    private static string? TryGetTestProjectDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (string.Equals(directory.Name, "bin", StringComparison.OrdinalIgnoreCase))
            {
                return directory.Parent?.FullName;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
