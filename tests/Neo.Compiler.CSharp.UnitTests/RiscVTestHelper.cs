// Copyright (C) 2015-2026 The Neo Project.
//
// RiscVTestHelper.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace Neo.Compiler.CSharp.UnitTests;

/// <summary>
/// Helper to compile and cache RISC-V contract binaries for testing.
/// </summary>
public static class RiscVTestHelper
{
    private static readonly string OutputDir = Path.Combine(Path.GetTempPath(), "neo-riscv-test-contracts");
    private static readonly ConcurrentDictionary<string, string?> _cache = new();
    private static bool _initialized;

    /// <summary>
    /// Ensures all test contracts are compiled for RISC-V.
    /// Call once in test assembly initialization.
    /// </summary>
    public static void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        // If pre-built binaries exist, skip compilation entirely
        var prebuiltDirs = new[]
        {
            Environment.GetEnvironmentVariable("NEO_RISCV_CONTRACTS_DIR"),
            "/tmp/riscv-test-output/riscv",
            Path.Combine(OutputDir, "riscv"),
        };
        foreach (var dir in prebuiltDirs)
        {
            if (dir != null && Directory.Exists(dir) &&
                Directory.GetFiles(dir, "contract.polkavm", SearchOption.AllDirectories).Length > 50)
            {
                // Pre-built binaries available — skip slow Rust compilation
                return;
            }
        }

        Directory.CreateDirectory(OutputDir);

        // Compile all test contracts with --target riscv
        var projectPath = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(), "..", "..", "..", "..",
            "Neo.Compiler.CSharp.TestContracts",
            "Neo.Compiler.CSharp.TestContracts.csproj"));

        var options = new CompilationOptions
        {
            Target = CompilationTarget.RiscV,
            Nullable = NullableContextOptions.Annotations,
        };
        var engine = new CompilationEngine(options);
        var contexts = engine.CompileProject(projectPath);

        foreach (var ctx in contexts)
        {
            if (!ctx.Success || ctx.GeneratedRustSource == null) continue;

            var name = ctx.ContractName?.ToLowerInvariant().Replace(" ", "_");
            if (name == null) continue;

            var crateDir = Path.Combine(OutputDir, "riscv", name);
            var srcDir = Path.Combine(crateDir, "src");
            Directory.CreateDirectory(srcDir);

            // Write main.rs
            File.WriteAllText(Path.Combine(srcDir, "main.rs"), ctx.GeneratedRustSource);

            // Write Cargo.toml with absolute paths
            var cargoToml = ctx.GeneratedCargoToml ?? "";
            // Fix relative crate paths to absolute
            var riscvVmRoot = FindRiscvVmRoot();
            if (riscvVmRoot != null)
            {
                cargoToml = cargoToml.Replace(
                    "path = \"../../crates",
                    $"path = \"{riscvVmRoot}/crates");
            }
            File.WriteAllText(Path.Combine(crateDir, "Cargo.toml"), cargoToml);

            _cache[ctx.ContractName!] = crateDir;
        }
    }

    /// <summary>
    /// Get the .polkavm binary path for a contract.
    /// Checks pre-built directory first, then cache, builds if needed.
    /// Returns null if compilation fails.
    /// </summary>
    public static string? GetPolkaVmBinary(string contractName)
    {
        // 1. Check pre-built directories (from batch build or Initialize())
        var prebuiltDirs = new[]
        {
            Environment.GetEnvironmentVariable("NEO_RISCV_CONTRACTS_DIR"),
            "/tmp/riscv-test-output/riscv",
            Path.Combine(OutputDir, "riscv"),
        };
        var prebuiltName = contractName.ToLowerInvariant().Replace(" ", "_");
        foreach (var dir in prebuiltDirs)
        {
            if (dir == null) continue;
            var candidate = Path.Combine(dir, prebuiltName, "contract.polkavm");
            if (File.Exists(candidate))
                return candidate;
        }

        // 2. Check cache
        if (!_cache.TryGetValue(contractName, out var crateDir) || crateDir == null)
            return null;

        var polkavmPath = Path.Combine(crateDir, "contract.polkavm");
        if (File.Exists(polkavmPath))
            return polkavmPath;

        // 3. Build on-the-fly (slow)
        if (!BuildCrate(crateDir))
            return null;

        return File.Exists(polkavmPath) ? polkavmPath : null;
    }

    private static bool BuildCrate(string crateDir)
    {
        try
        {
            var contractName = Path.GetFileName(crateDir);

            // Get original target JSON from polkatool
            var origTargetJson = RunCommand("polkatool", "get-target-json-path -b 32", stderr: out var polkatoolStderr)?.Trim();
            if (string.IsNullOrEmpty(origTargetJson))
            {
                Console.Error.WriteLine($"[RiscV] {contractName}: polkatool get-target-json-path failed.");
                if (!string.IsNullOrEmpty(polkatoolStderr))
                    Console.Error.WriteLine($"  stderr: {polkatoolStderr}");
                return false;
            }

            // Fix target JSON: add "abi" field required by newer nightly rustc
            var targetJson = Path.Combine(Path.GetTempPath(), "neo-riscv32-polkavm.json");
            FixTargetJson(origTargetJson!, targetJson);

            // Build with -Zjson-target-spec for .json target files
            var buildResult = RunCommand("cargo",
                $"+nightly build --manifest-path {crateDir}/Cargo.toml --release --target {targetJson} -Zbuild-std=core,alloc -Zjson-target-spec",
                stderr: out var cargoStderr);
            if (buildResult == null)
            {
                Console.Error.WriteLine($"[RiscV] {contractName}: cargo build failed.");
                if (!string.IsNullOrEmpty(cargoStderr))
                    Console.Error.WriteLine($"  stderr: {cargoStderr}");
                return false;
            }

            // Link — the output dir uses the JSON file's stem name
            var target = Path.GetFileNameWithoutExtension(targetJson);
            var name = Path.GetFileName(crateDir);
            var elf = Path.Combine(crateDir, "target", target, "release", name);
            var polkavm = Path.Combine(crateDir, "contract.polkavm");
            RunCommand("polkatool", $"link --strip -o {polkavm} {elf}", stderr: out var linkStderr);

            if (!File.Exists(polkavm))
            {
                Console.Error.WriteLine($"[RiscV] {contractName}: polkatool link produced no output.");
                if (!string.IsNullOrEmpty(linkStderr))
                    Console.Error.WriteLine($"  stderr: {linkStderr}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[RiscV] BuildCrate exception: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Patches the polkatool-generated target JSON to add the "abi" field
    /// required by newer nightly rustc for RISC-V targets.
    /// </summary>
    private static void FixTargetJson(string sourcePath, string destPath)
    {
        var json = System.Text.Json.JsonDocument.Parse(File.ReadAllText(sourcePath));
        var root = json.RootElement;

        // Check if "abi" field already exists
        if (root.TryGetProperty("abi", out _))
        {
            File.Copy(sourcePath, destPath, overwrite: true);
            return;
        }

        // Rebuild JSON with "abi" field inserted
        using var stream = new MemoryStream();
        using var writer = new System.Text.Json.Utf8JsonWriter(stream, new System.Text.Json.JsonWriterOptions { Indented = true });
        writer.WriteStartObject();
        foreach (var prop in root.EnumerateObject())
        {
            prop.WriteTo(writer);
        }
        // Add abi field matching llvm-abiname
        var abiName = root.TryGetProperty("llvm-abiname", out var abiname) ? abiname.GetString() ?? "ilp32e" : "ilp32e";
        writer.WriteString("abi", abiName);
        writer.WriteEndObject();
        writer.Flush();
        File.WriteAllBytes(destPath, stream.ToArray());
    }

    private static string? RunCommand(string command, string args, out string? stderr)
    {
        stderr = null;
        try
        {
            var cargoBin = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".cargo", "bin");
            var currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
            var newPath = Directory.Exists(cargoBin)
                ? cargoBin + Path.PathSeparator + currentPath
                : currentPath;

            // Use bash so the child sees the updated PATH
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command} {args.Replace("\"", "\\\"")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            psi.EnvironmentVariables["PATH"] = newPath;
            var proc = Process.Start(psi);
            proc?.WaitForExit(300000); // 5 min timeout
            stderr = proc?.StandardError.ReadToEnd();
            return proc?.ExitCode == 0 ? proc.StandardOutput.ReadToEnd() : null;
        }
        catch (Exception ex)
        {
            stderr = ex.Message;
            return null;
        }
    }

    private static string? FindRiscvVmRoot()
    {
        // Walk up from current directory to find neo-riscv-vm
        var dir = Directory.GetCurrentDirectory();
        while (dir != null)
        {
            if (Directory.Exists(Path.Combine(dir, "crates", "neo-riscv-rt")))
                return dir;
            dir = Path.GetDirectoryName(dir);
        }

        // Check common sibling locations
        var siblingLocations = new[]
        {
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "neo-riscv-vm")),
            Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "neo-riscv-vm")),
            "/home/neo/git/neo-riscv-vm",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "git", "neo-riscv-vm"),
        };

        foreach (var candidate in siblingLocations)
        {
            if (Directory.Exists(Path.Combine(candidate, "crates", "neo-riscv-rt")))
                return candidate;
        }

        return null;
    }
}
