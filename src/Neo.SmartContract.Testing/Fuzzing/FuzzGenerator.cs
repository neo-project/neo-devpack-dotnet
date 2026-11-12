// Copyright (C) 2015-2026 The Neo Project.
//
// FuzzGenerator.cs file belongs to the neo project and is free
// software distributed under the terms of the MIT software license, see the
// accompanying file LICENSE in the main directory of the repository or
// http://www.opensource.org/licenses/mit-license.php for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Numerics;

namespace Neo.SmartContract.Testing.Fuzzing;

/// <summary>
/// Deterministic value generator backing the fuzz attributes
/// (<see cref="Attributes.FuzzTestAttribute"/> and friends).
/// </summary>
/// <remarks>
/// A dedicated generator was chosen over FsCheck or Bogus because fuzz data for contract
/// tests must be:
/// <list type="bullet">
/// <item><description>deterministic by default, so a failing row can be replayed exactly by its seed;</description></item>
/// <item><description>free of extra package dependencies, keeping the testing library self-contained;</description></item>
/// <item><description>aware of Neo contract parameter types such as <see cref="UInt160"/> and <see cref="BigInteger"/>.</description></item>
/// </list>
/// Every generator method takes the shared <see cref="Random"/> sequence, so seeding the
/// sequence once reproduces an entire test run.
/// </remarks>
public static class FuzzGenerator
{
    /// <summary>
    /// The default seed used by the fuzz attributes when no seed is specified.
    /// </summary>
    public const int DefaultSeed = 0x5EED;

    /// <summary>
    /// The default upper bound for generated amounts (10^18), matching the highest
    /// realistic token total supply.
    /// </summary>
    public static readonly BigInteger DefaultMaxAmount = BigInteger.Pow(10, 18);

    /// <summary>
    /// The default maximum length for generated strings and byte arrays.
    /// </summary>
    public const int DefaultMaxLength = 64;

    /// <summary>
    /// Produces a fuzz value for a method parameter of the given type.
    /// </summary>
    /// <param name="random">The shared deterministic random sequence.</param>
    /// <param name="type">The parameter type to generate a value for.</param>
    /// <returns>A generated value assignable to <paramref name="type"/>.</returns>
    /// <exception cref="NotSupportedException">The parameter type has no generator.</exception>
    public static object? NextValue(Random random, Type type)
    {
        if (type == typeof(int)) return NextInt32(random);
        if (type == typeof(uint)) return unchecked((uint)NextInt32(random));
        if (type == typeof(long)) return NextInt64(random);
        if (type == typeof(ulong)) return unchecked((ulong)NextInt64(random));
        if (type == typeof(short)) return (short)random.Next(short.MinValue, short.MaxValue + 1);
        if (type == typeof(ushort)) return (ushort)random.Next(ushort.MaxValue + 1);
        if (type == typeof(sbyte)) return (sbyte)random.Next(sbyte.MinValue, sbyte.MaxValue + 1);
        if (type == typeof(byte)) return (byte)random.Next(byte.MaxValue + 1);
        if (type == typeof(bool)) return random.Next(2) == 1;
        if (type == typeof(BigInteger)) return NextAmount(random);
        if (type == typeof(string)) return NextString(random);
        if (type == typeof(byte[])) return NextBytes(random);
        if (type == typeof(UInt160)) return NextUInt160(random);
        if (type == typeof(UInt256)) return NextUInt256(random);
        throw new NotSupportedException(
            $"FuzzTest does not support the parameter type '{type}'. Supported types are: " +
            "int, uint, long, ulong, short, ushort, sbyte, byte, bool, string, byte[], " +
            "System.Numerics.BigInteger, UInt160 and UInt256.");
    }

    /// <summary>
    /// Generates an int covering the full (except the overflow boundary) value range.
    /// </summary>
    public static int NextInt32(Random random) => random.Next(int.MinValue, int.MaxValue);

    /// <summary>
    /// Generates a long composed of two independent int draws.
    /// </summary>
    public static long NextInt64(Random random) => ((long)random.Next(int.MinValue, int.MaxValue) << 32) | (uint)random.Next();

    /// <summary>
    /// Generates a token amount in the inclusive range [min, max].
    /// </summary>
    /// <remarks>
    /// The sample is drawn from 128 random bits, which keeps the modulo bias far below
    /// anything observable for contract-test purposes.
    /// </remarks>
    public static BigInteger NextAmount(Random random, BigInteger? min = null, BigInteger? max = null)
    {
        BigInteger lower = min ?? BigInteger.Zero;
        BigInteger upper = max ?? DefaultMaxAmount;
        if (upper < lower) (lower, upper) = (upper, lower);
        BigInteger range = upper - lower;
        if (range.IsZero) return lower;

        var bytes = new byte[16];
        random.NextBytes(bytes);
        return lower + new BigInteger(bytes, isUnsigned: true, isBigEndian: false) % (range + 1);
    }

    /// <summary>
    /// Generates a string mixing empty, short, ASCII, hex-like and non-ASCII content,
    /// with at most <paramref name="maxLength"/> characters.
    /// </summary>
    public static string NextString(Random random, int maxLength = DefaultMaxLength)
    {
        if (maxLength < 0) maxLength = 0;
        // One in eight strings is empty to exercise boundary handling.
        if (maxLength > 0 && random.Next(8) == 0) return string.Empty;
        // One in eight strings is forced to the short boundary.
        if (maxLength > 2 && random.Next(8) == 0) maxLength = 2;

        var chars = new char[random.Next(maxLength + 1)];
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = random.Next(4) switch
            {
                0 => (char)random.Next(32, 127),            // printable ASCII
                1 => (char)random.Next(0xA1, 0x2FFF),        // non-ASCII BMP
                2 => "0123456789abcdefABCDEF"[random.Next(22)], // hex-like
                _ => (char)random.Next('a', 'z' + 1),        // letters
            };
        }
        return new string(chars);
    }

    /// <summary>
    /// Generates a byte array with at most <paramref name="maxLength"/> bytes.
    /// </summary>
    public static byte[] NextBytes(Random random, int maxLength = DefaultMaxLength)
    {
        if (maxLength < 0) maxLength = 0;
        var data = new byte[random.Next(maxLength + 1)];
        random.NextBytes(data);
        return data;
    }

    /// <summary>
    /// Generates a random script hash (account address).
    /// </summary>
    public static UInt160 NextUInt160(Random random)
    {
        var data = new byte[UInt160.Length];
        random.NextBytes(data);
        return new UInt160(data);
    }

    /// <summary>
    /// Generates a random 256-bit value.
    /// </summary>
    public static UInt256 NextUInt256(Random random)
    {
        var data = new byte[UInt256.Length];
        random.NextBytes(data);
        return new UInt256(data);
    }
}
