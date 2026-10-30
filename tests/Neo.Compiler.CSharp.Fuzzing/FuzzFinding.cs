// Copyright (C) 2015-2026 The Neo Project.
//
// CompilerFuzzTarget.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Classification of a single fuzz iteration outcome.
/// </summary>
public enum FuzzOutcome
{
    /// <summary>Source compiled cleanly; no defect observed.</summary>
    Clean,

    /// <summary>Compiler rejected the source with a normal diagnostic. Expected for many inputs.</summary>
    Rejected,

    /// <summary>
    /// The compiler produced an internal-error diagnostic (NC5001 / UnexpectedCompilerError),
    /// i.e. an exception escaped a layer that was supposed to handle it. This is a defect.
    /// </summary>
    InternalError,

    /// <summary>
    /// An exception that is not a CompilationException escaped the compiler entirely
    /// (NullReferenceException, IndexOutOfRangeException, StackOverflow, ...). This is a defect.
    /// </summary>
    CompilerCrash,

    /// <summary>Compilation exceeded the time budget. Suspected infinite loop or pathological blowup.</summary>
    Hang,

    /// <summary>The differential analyzer found a semantic divergence.</summary>
    DifferentialMismatch,

    /// <summary>Two compilations of the identical source produced different artifacts. Non-determinism defect.</summary>
    NonDeterministic,

    /// <summary>
    /// A single iteration took longer than the per-iteration budget. The input itself is
    /// not necessarily wrong — it may simply contain a very long loop — but a fuzzer that
    /// stalls on one input cannot make progress, so the input is reported with a
    /// reproducer.
    /// </summary>
    SlowExecution,

    /// <summary>
    /// A call failed in both builds in a way that suggests the harness, not the contract,
    /// is at fault — a method that never succeeds, or an error that names a
    /// calling-convention problem.
    /// </summary>
    /// <remarks>
    /// This is distinct from <see cref="DifferentialMismatch"/>: the two builds agree, so
    /// there is no compiler divergence. The signal exists because agreement on failure is
    /// otherwise indistinguishable from a clean result, and that is how a broken harness
    /// hides.
    /// </remarks>
    SuspiciousFailure,
}

/// <summary>
/// A single defect discovered by the fuzzer.
/// </summary>
/// <param name="Outcome">Classification of the defect.</param>
/// <param name="Source">The C# source that triggered the defect.</param>
/// <param name="Detail">Human readable description of the defect.</param>
/// <param name="Exception">The exception that caused the defect, when applicable.</param>
/// <param name="StackTrace">Stack trace of the causing exception, when applicable.</param>
public sealed record FuzzFinding(
    FuzzOutcome Outcome,
    string Source,
    string Detail,
    string? Exception = null,
    string? StackTrace = null)
{
    /// <summary>Short, stable identifier used to group identical findings.</summary>
    public string Signature => $"{Outcome}|{Exception ?? Detail}";
}
