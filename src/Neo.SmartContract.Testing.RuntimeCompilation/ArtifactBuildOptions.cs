// Copyright (C) 2015-2025 The Neo Project.
//
// ArtifactBuildOptions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Compiler;

namespace Neo.SmartContract.Testing.RuntimeCompilation;

/// <summary>
/// Configurable knobs that influence how runtime contract artifacts are produced.
/// </summary>
public sealed record ArtifactBuildOptions
{
    /// <summary>
    /// Gets the default configuration used by the runtime compiler.
    /// </summary>
    public static ArtifactBuildOptions Default { get; } = new();

    /// <summary>
    /// Determines whether NEP-19 debug information is produced.
    /// </summary>
    public CompilationOptions.DebugType Debug { get; init; } = CompilationOptions.DebugType.Extended;

    /// <summary>
    /// Controls the optimizer level executed by Neo.Compiler.CSharp.
    /// </summary>
    public CompilationOptions.OptimizationType Optimize { get; init; } = CompilationOptions.OptimizationType.Basic;

    /// <summary>
    /// When <see langword="true"/> manifest accessors are materialised as properties on the generated proxy class.
    /// </summary>
    public bool GenerateProperties { get; init; } = true;

    /// <summary>
    /// When <see langword="true"/> the generated proxy class contains VM remark annotations sourced from the debug info.
    /// </summary>
    public bool TraceRemarks { get; init; }

    /// <summary>
    /// Bypass the cache and force the project to be recompiled even if a fingerprint match is found.
    /// </summary>
    public bool ForceRebuild { get; init; }
}
