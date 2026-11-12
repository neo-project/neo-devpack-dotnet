// Copyright (C) 2015-2026 The Neo Project.
//
// FuzzTestAttribute.cs file belongs to the neo project and is free
// software distributed under the terms of the MIT software license, see the
// accompanying file LICENSE in the main directory of the repository or
// http://www.opensource.org/licenses/mit-license.php for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing.Fuzzing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Neo.SmartContract.Testing.Attributes;

/// <summary>
/// Runs a test method once per generated iteration, fuzzing every parameter from its type
/// (integers, <see cref="string"/>, <see cref="byte"/>[], <see cref="BigInteger"/>,
/// <see cref="UInt160"/> and <see cref="UInt256"/>).
/// </summary>
/// <remarks>
/// The generated sequence is fully determined by <see cref="Seed"/>: a failing iteration
/// can be replayed exactly by re-running the test with the same seed. Use
/// <c>[FuzzTest(iterations: 100, seed: 42)]</c> to customize the run.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class FuzzTestAttribute : TestMethodAttribute, ITestDataSource
{
    /// <summary>
    /// The number of generated invocations of the test method.
    /// </summary>
    public int Iterations { get; }

    /// <summary>
    /// The seed reproducing the whole generated sequence.
    /// </summary>
    public int Seed { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="iterations">The number of generated invocations (default 20).</param>
    /// <param name="seed">The seed of the generated sequence (default <see cref="FuzzGenerator.DefaultSeed"/>).</param>
    public FuzzTestAttribute(
        int iterations = 20,
        int seed = FuzzGenerator.DefaultSeed,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = -1)
        : base(callerFilePath, callerLineNumber)
    {
        if (iterations <= 0)
            throw new ArgumentOutOfRangeException(nameof(iterations), "At least one iteration is required.");
        Iterations = iterations;
        Seed = seed;
    }

    /// <inheritdoc />
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
    {
        var random = new Random(Seed);
        var parameters = methodInfo.GetParameters();
        for (int i = 0; i < Iterations; i++)
            yield return parameters.Select(parameter => FuzzGenerator.NextValue(random, parameter.ParameterType)).ToArray();
    }

    /// <inheritdoc />
    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data) =>
        $"{methodInfo.Name} (fuzz: {string.Join(", ", data ?? [])})";
}

/// <summary>
/// Base class of the single-value fuzz attributes, generating <see cref="Count"/> rows of
/// one fuzz value each.
/// </summary>
public abstract class FuzzValueAttributeBase : TestMethodAttribute, ITestDataSource
{
    /// <summary>
    /// The number of generated rows.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// The seed reproducing the whole generated sequence.
    /// </summary>
    public int Seed { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="count">The number of generated rows.</param>
    /// <param name="seed">The seed of the generated sequence.</param>
    protected FuzzValueAttributeBase(
        int count,
        int seed,
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = -1)
        : base(callerFilePath, callerLineNumber)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count), "At least one row is required.");
        Count = count;
        Seed = seed;
    }

    /// <summary>
    /// Produces the fuzz value of one row.
    /// </summary>
    protected abstract object? NextValue(Random random);

    /// <inheritdoc />
    public IEnumerable<object?[]> GetData(MethodInfo methodInfo)
    {
        var random = new Random(Seed);
        for (int i = 0; i < Count; i++)
            yield return [NextValue(random)];
    }

    /// <inheritdoc />
    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data) =>
        $"{methodInfo.Name} ({data?.FirstOrDefault()})";
}

/// <summary>
/// Generates random script hashes, for example to fuzz recipient accounts in
/// transfer, mint or burn tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class FuzzUInt160Attribute(
    int count = 10,
    int seed = FuzzGenerator.DefaultSeed,
    [CallerFilePath] string callerFilePath = "",
    [CallerLineNumber] int callerLineNumber = -1)
    : FuzzValueAttributeBase(count, seed, callerFilePath, callerLineNumber)
{
    /// <inheritdoc />
    protected override object? NextValue(Random random) => FuzzGenerator.NextUInt160(random);
}

/// <summary>
/// Generates random token amounts in the inclusive range [Min, Max].
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class FuzzAmountAttribute(
    int count = 10,
    int seed = FuzzGenerator.DefaultSeed,
    [CallerFilePath] string callerFilePath = "",
    [CallerLineNumber] int callerLineNumber = -1)
    : FuzzValueAttributeBase(count, seed, callerFilePath, callerLineNumber)
{
    /// <summary>
    /// The inclusive lower bound as a decimal string. Strings keep arbitrarily large
    /// amounts usable as C# attribute arguments.
    /// </summary>
    public string Min { get; set; } = "0";

    /// <summary>
    /// The inclusive upper bound as a decimal string.
    /// </summary>
    public string Max { get; set; } = "1000000000000000000";

    /// <inheritdoc />
    protected override object? NextValue(Random random) => FuzzGenerator.NextAmount(
        random,
        BigInteger.Parse(Min, CultureInfo.InvariantCulture),
        BigInteger.Parse(Max, CultureInfo.InvariantCulture));
}

/// <summary>
/// Generates random strings with at most <see cref="MaxLength"/> characters, mixing
/// empty, short, ASCII, hex-like and non-ASCII content.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class FuzzStringAttribute(
    int count = 10,
    int seed = FuzzGenerator.DefaultSeed,
    [CallerFilePath] string callerFilePath = "",
    [CallerLineNumber] int callerLineNumber = -1)
    : FuzzValueAttributeBase(count, seed, callerFilePath, callerLineNumber)
{
    /// <summary>
    /// The inclusive maximum length of the generated strings.
    /// </summary>
    public int MaxLength { get; set; } = FuzzGenerator.DefaultMaxLength;

    /// <inheritdoc />
    protected override object? NextValue(Random random) => FuzzGenerator.NextString(random, MaxLength);
}

/// <summary>
/// Generates random ints in the inclusive range [Min, Max].
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class FuzzIntAttribute(
    int count = 10,
    int seed = FuzzGenerator.DefaultSeed,
    [CallerFilePath] string callerFilePath = "",
    [CallerLineNumber] int callerLineNumber = -1)
    : FuzzValueAttributeBase(count, seed, callerFilePath, callerLineNumber)
{
    /// <summary>
    /// The inclusive lower bound of the generated values.
    /// </summary>
    public int Min { get; set; } = int.MinValue;

    /// <summary>
    /// The inclusive upper bound of the generated values.
    /// </summary>
    public int Max { get; set; } = int.MaxValue;

    /// <inheritdoc />
    protected override object? NextValue(Random random)
    {
        int lower = Math.Min(Min, Max);
        int upper = Math.Max(Min, Max);
        return (int)random.NextInt64(lower, (long)upper + 1);
    }
}
