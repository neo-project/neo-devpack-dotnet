// Copyright (C) 2015-2026 The Neo Project.
//
// CryptoProbe.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Cryptography;
using Neo.Cryptography.ECC;
using Neo.Wallets;

namespace Neo.Compiler.Fuzzing;

/// <summary>
/// Checks the native crypto calls that the compiler lowers as extern methods.
/// </summary>
/// <remarks>
/// A mistake in the argument order or in the curve enum does not fail loudly: the call returns
/// <c>false</c>, which looks exactly like an invalid signature. The probe signs a message with
/// the Neo crypto primitives, then asks the contract to verify it, so a wrong lowering shows up
/// as a verification that fails on a valid signature.
/// </remarks>
public static class CryptoProbe
{
    private const string Source = """
        using System;
        using System.Numerics;
        using Neo.SmartContract.Framework;
        using Neo.SmartContract.Framework.Native;

        public class CryptoProbeContract : SmartContract
        {
            public static bool VerifyR1(ByteString message, ECPoint pubkey, ByteString signature)
            {
                return CryptoLib.VerifyWithECDsa(message, pubkey, signature, NamedCurveHash.secp256r1SHA256);
            }

            // The legacy overload that existing contracts use; it is marked obsolete in the
            // framework but still has to keep verifying signatures.
        #pragma warning disable CS0618
            public static bool VerifyR1Legacy(ByteString message, ECPoint pubkey, ByteString signature)
            {
                return CryptoLib.VerifyWithECDsa(message, pubkey, signature, NamedCurve.secp256r1);
            }
        #pragma warning restore CS0618

            public static bool VerifyK1(ByteString message, ECPoint pubkey, ByteString signature)
            {
                return CryptoLib.VerifyWithECDsa(message, pubkey, signature, NamedCurveHash.secp256k1SHA256);
            }

            public static int Keccak(ByteString data)
            {
                ByteString hash = CryptoLib.Keccak256(data);
                return hash.Length * 1000 + hash[0];
            }

            public static int Murmur(ByteString data)
            {
                ByteString hash = CryptoLib.Murmur32(data, 1234567u);
                return hash.Length;
            }
        }
        """;

    /// <summary>
    /// Runs the probe.
    /// </summary>
    /// <returns>The number of checks that failed.</returns>
    public static int Run()
    {
        int failures = 0;

        // A fixed private key keeps the probe deterministic.
        byte[] privateKey = Convert.FromHexString(
            "0102030405060708090A0B0C0D0E0F101112131415161718191A1B1C1D1E1F20");
        var key = new KeyPair(privateKey);
        byte[] message = "neo compiler fuzz"u8.ToArray();
        byte[] tampered = "neo compiler FUZZ"u8.ToArray();
        byte[] signature = Crypto.Sign(message, key.PrivateKey, ECCurve.Secp256r1);
        byte[] publicKey = key.PublicKey.EncodePoint(true);

        using var harness = new CompilerHarness();
        (FuzzOutcome outcome, CompilationContext? context, FuzzFinding? finding) = harness.Compile(Source);
        if (outcome != FuzzOutcome.Clean || context is null)
        {
            string reason = context?.Diagnostics
                .Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                .Select(d => $"{d.Id}: {d.GetMessage()}")
                .FirstOrDefault() ?? finding?.Detail ?? "no diagnostics";
            Console.WriteLine($"[FAIL] the probe contract does not compile: {reason}");
            return 1;
        }

        (Neo.SmartContract.NefFile nef, Neo.SmartContract.Manifest.ContractManifest manifest, _) = context.CreateResults();
        using var analyzer = new BehaviorDifferentialAnalyzer();

        failures += Check(analyzer, nef, manifest, "verifyR1",
            "valid secp256r1 signature", [message, publicKey, signature], "true");

        failures += Check(analyzer, nef, manifest, "verifyR1",
            "tampered message", [tampered, publicKey, signature], "false");

        failures += Check(analyzer, nef, manifest, "verifyR1Legacy",
            "valid signature via the legacy overload", [message, publicKey, signature], "true");

        failures += Check(analyzer, nef, manifest, "verifyK1",
            "r1 signature checked as secp256k1", [message, publicKey, signature], "false");

        byte[] expectedKeccak = Neo.Cryptography.Helper.Keccak256(message);
        failures += Check(analyzer, nef, manifest, "keccak",
            "keccak256", [message], (expectedKeccak.Length * 1000 + expectedKeccak[0]).ToString());

        // The native call returns the 32-bit hash as bytes; only its width is pinned here.
        failures += Check(analyzer, nef, manifest, "murmur", "murmur32 width", [message], "4");

        Console.WriteLine();
        Console.WriteLine(failures == 0
            ? "CRYPTO PROBE PASSED: signatures, curves, and hashes agree with the platform primitives."
            : $"CRYPTO PROBE: {failures} check(s) disagree.");

        return failures;
    }

    private static int Check(BehaviorDifferentialAnalyzer analyzer, Neo.SmartContract.NefFile nef,
        Neo.SmartContract.Manifest.ContractManifest manifest, string method, string label,
        object?[] args, string expected)
    {
        (bool ok, string? error, Neo.VM.Types.StackItem? value) =
            analyzer.ExecuteIsolated(new ArtifactSet(nef, manifest), method, args);

        string actual = ok ? BehaviorDifferentialAnalyzer.Render(value!) : $"FAULT({error})";
        bool agrees = string.Equals(actual, expected, StringComparison.Ordinal);

        Console.WriteLine($"  {label,-38} => {actual,-12} expected {expected,-8} {(agrees ? "match" : "MISMATCH")}");
        return agrees ? 0 : 1;
    }
}
