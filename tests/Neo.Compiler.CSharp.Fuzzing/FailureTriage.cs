// Copyright (C) 2015-2026 The Neo Project.
//
// FailureTriage.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Decides whether a set of call results is suspicious even when the two builds agree.
/// </summary>
/// <remarks>
/// <para>
/// The differential oracles deliberately let a call that fails identically in both builds
/// pass: a division by zero faults the same way whichever way the source was compiled, so
/// reporting it would be noise. But that rule has a blind spot. When the harness itself is
/// wrong — a method name that does not match the ABI, a missing contract permission, a
/// malformed argument list — both builds fail identically too, and the differential
/// reports agreement. That is indistinguishable from a clean compiler.
/// </para>
/// <para>
/// This triage closes the blind spot with two independent signals:
/// </para>
/// <list type="number">
/// <item><description><b>Every input failed.</b> A method that never succeeds across the
/// whole boundary argument set is not hitting a boundary; something systematic is wrong.
/// A genuine boundary failure (overflow, division by zero) only fails on the specific
/// inputs that trigger it, while the other inputs in the set still succeed.</description></item>
/// <item><description><b>The error names a calling-convention problem.</b> Messages such as
/// "method doesn't exist" or "cannot be converted" describe the harness getting the call
/// wrong, not the contract misbehaving.</description></item>
/// </list>
/// </remarks>
public static class FailureTriage
{
    /// <summary>
    /// Error fragments that indicate the call never reached the contract under test.
    /// </summary>
    private static readonly string[] CallingConventionIndicators =
    [
        "doesn't exist",                        // the method name or parameter count is wrong
        "Cannot Call Method",                   // the caller has no permission to call the callee
        "cannot be converted",                  // an argument has the wrong type
        "Unknown method",                       // the method is absent from the ABI
        "out of stack bounds",                  // the generated code is not stack-balanced
        "Unexpected",                           // the compiler reached a state it did not expect
    ];

    /// <summary>
    /// Error fragments that describe a failure the contract code itself calls for.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The argument sets are deliberately boundary-heavy, so a whole argument set can
    /// legitimately fault: a divisor derived from an argument can be zero, an index can be
    /// negative, and an expression wrapped in <c>checked</c> is supposed to throw on
    /// overflow. None of that says anything about the harness.
    /// </para>
    /// <para>
    /// The generic VM fault message is included as well. It is what the engine reports when
    /// the contract aborts for any reason, and it cannot be told apart from a legitimate
    /// abort without inspecting the invocation stack, which the differential already covers
    /// by comparing both builds.
    /// </para>
    /// </remarks>
    private static readonly string[] ExpectedFailureIndicators =
    [
        "Overflow",
        "ArithmeticException",
        "DivideByZero",
        "divide by zero",
        "AssertionException",
        "out of range",
        "out of valid range",
        "exceeds maximum allowed size",
        "MaxStackSize exceed",
        "too large or too small for an Int32",
        "An unhandled exception",
    ];

    /// <summary>
    /// Inspects the results of one method.
    /// </summary>
    /// <param name="method">The method name, used in the message.</param>
    /// <param name="results">The per-input results, in the order the inputs were tried.</param>
    /// <returns>A description of the suspicion, or <c>null</c> when the results look normal.</returns>
    public static string? Inspect(string method, IReadOnlyList<(bool Ok, string? Error)> results)
    {
        if (results.Count == 0)
        {
            return null;
        }

        int succeeded = results.Count(r => r.Ok);

        // Signal 2: any failure whose message names a calling-convention problem. This is
        // reported even when other inputs succeeded, because it means the harness built the
        // call wrongly for at least some inputs.
        string? conventionError = results
            .Where(r => !r.Ok && r.Error is not null)
            .Select(r => r.Error!)
            .FirstOrDefault(error => CallingConventionIndicators.Any(
                indicator => error.Contains(indicator, StringComparison.OrdinalIgnoreCase)));

        if (conventionError is not null)
        {
            return $"Method '{method}' reported a calling-convention error on at least one input, " +
                   $"so the call may never have reached the contract: {Shorten(conventionError)}";
        }

        // Signal 1: every input failed. A boundary failure would only fail on the inputs
        // that trigger it, so a total failure points at the harness rather than the contract
        // — unless every failure is one the C# semantics call for, such as a checked
        // overflow that triggers on the whole argument set.
        if (succeeded == 0)
        {
            var errors = results.Where(r => !r.Ok).Select(r => r.Error ?? string.Empty).ToList();

            bool allExpected = errors.All(error => ExpectedFailureIndicators.Any(
                indicator => error.Contains(indicator, StringComparison.OrdinalIgnoreCase)));

            if (allExpected)
            {
                return null;
            }

            string sample = Shorten(errors.FirstOrDefault() ?? "<no message>");
            return $"Method '{method}' failed on all {results.Count} inputs without succeeding once, " +
                   $"which points at a systematic problem rather than a boundary condition: {sample}";
        }

        return null;
    }

    /// <summary>
    /// Trims a long error message so the finding stays readable.
    /// </summary>
    private static string Shorten(string message)
    {
        const int Limit = 240;
        return message.Length <= Limit ? message : message[..Limit] + "...";
    }
}
