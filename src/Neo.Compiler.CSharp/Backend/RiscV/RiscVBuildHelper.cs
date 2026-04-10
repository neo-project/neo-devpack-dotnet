// Copyright (C) 2015-2026 The Neo Project.
//
// RiscVBuildHelper.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Neo.Compiler.CSharp.Backend.RiscV;

/// <summary>
/// Shared utility for building RISC-V contracts using cargo and polkatool.
/// Can be used by both tests and CLI tools.
/// </summary>
public static class RiscVBuildHelper
{
    /// <summary>
    /// Result of a build operation.
    /// </summary>
    public class BuildResult
    {
        /// <summary>
        /// True if the build succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Path to the output .polkavm file if successful.
        /// </summary>
        public string? OutputPath { get; set; }

        /// <summary>
        /// Error message if the build failed.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Full stderr output from the build commands for diagnostics.
        /// </summary>
        public string? Stderr { get; set; }
    }

    /// <summary>
    /// Builds a RISC-V crate using cargo +nightly and polkatool link.
    /// </summary>
    /// <param name="crateDir">The directory containing the Cargo.toml file.</param>
    /// <param name="outputPath">The desired output path for the .polkavm file. If null, uses contract.polkavm in the crate directory.</param>
    /// <returns>A BuildResult indicating success or failure with diagnostic information.</returns>
    public static BuildResult BuildCrate(string crateDir, string? outputPath = null)
    {
        var contractName = Path.GetFileName(crateDir);
        var result = new BuildResult();

        try
        {
            // Validate inputs
            if (!Directory.Exists(crateDir))
            {
                result.ErrorMessage = $"Crate directory does not exist: {crateDir}";
                return result;
            }

            var cargoTomlPath = Path.Combine(crateDir, "Cargo.toml");
            if (!File.Exists(cargoTomlPath))
            {
                result.ErrorMessage = $"Cargo.toml not found in crate directory: {crateDir}";
                return result;
            }

            // Determine output path
            var polkavmPath = outputPath ?? Path.Combine(crateDir, "contract.polkavm");

            // Get original target JSON from polkatool
            var origTargetJson = RunCommand("polkatool", "get-target-json-path -b 32", workingDir: null, out var polkatoolStderr)?.Trim();
            if (string.IsNullOrEmpty(origTargetJson))
            {
                result.ErrorMessage = $"[{contractName}] polkatool get-target-json-path failed.";
                result.Stderr = polkatoolStderr;
                return result;
            }

            // Fix target JSON: add "abi" field required by newer nightly rustc
            var targetJson = Path.Combine(Path.GetTempPath(), "neo-riscv32-polkavm.json");
            FixTargetJson(origTargetJson!, targetJson);

            // Build with -Zjson-target-spec for .json target files
            var buildResult = RunCommand("cargo",
                $"+nightly build --manifest-path {crateDir}/Cargo.toml --release --target {targetJson} -Zbuild-std=core,alloc -Zjson-target-spec",
                workingDir: null,
                out var cargoStderr);

            if (buildResult == null)
            {
                result.ErrorMessage = $"[{contractName}] cargo build failed.";
                result.Stderr = cargoStderr;
                return result;
            }

            // Link — the output dir uses the JSON file's stem name
            var target = Path.GetFileNameWithoutExtension(targetJson);
            var name = Path.GetFileName(crateDir);
            var elf = Path.Combine(crateDir, "target", target, "release", name);

            if (!File.Exists(elf))
            {
                result.ErrorMessage = $"[{contractName}] Expected ELF output not found: {elf}";
                return result;
            }

            RunCommand("polkatool", $"link --strip -o {polkavmPath} {elf}", workingDir: null, out var linkStderr);

            if (!File.Exists(polkavmPath))
            {
                result.ErrorMessage = $"[{contractName}] polkatool link produced no output.";
                result.Stderr = linkStderr;
                return result;
            }

            result.Success = true;
            result.OutputPath = polkavmPath;
            return result;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = $"[{contractName}] BuildCrate exception: {ex.Message}";
            return result;
        }
    }

    /// <summary>
    /// Runs a command with process execution and captures stderr for diagnostics.
    /// </summary>
    /// <param name="file">The command to run (will be resolved via PATH).</param>
    /// <param name="args">Arguments to pass to the command.</param>
    /// <param name="workingDir">Working directory for the command, or null to use current directory.</param>
    /// <param name="stderr">Output parameter receiving the stderr content.</param>
    /// <returns>The stdout content if the command succeeded (exit code 0), null otherwise.</returns>
    public static string? RunCommand(string file, string args, string? workingDir, out string? stderr)
    {
        stderr = null;
        try
        {
            // Add cargo bin to PATH if available
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
                Arguments = $"-c \"{file} {args.Replace("\"", "\\\"")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            if (workingDir != null && Directory.Exists(workingDir))
            {
                psi.WorkingDirectory = workingDir;
            }

            psi.EnvironmentVariables["PATH"] = newPath;

            using var proc = Process.Start(psi);
            if (proc == null)
            {
                stderr = "Failed to start process";
                return null;
            }

            proc.WaitForExit(300000); // 5 min timeout
            stderr = proc.StandardError.ReadToEnd();
            return proc.ExitCode == 0 ? proc.StandardOutput.ReadToEnd() : null;
        }
        catch (Exception ex)
        {
            stderr = ex.Message;
            return null;
        }
    }

    /// <summary>
    /// Finds the RISC-V target JSON file using polkatool.
    /// </summary>
    /// <returns>Path to the target JSON file, or null if not found.</returns>
    public static string? GetTargetJsonPath()
    {
        return RunCommand("polkatool", "get-target-json-path -b 32", workingDir: null, out _)?.Trim();
    }

    /// <summary>
    /// Creates a fixed target JSON file with the required "abi" field.
    /// </summary>
    /// <param name="outputPath">Where to write the fixed target JSON.</param>
    /// <returns>True if successful, false otherwise.</returns>
    public static bool CreateFixedTargetJson(string outputPath)
    {
        var origTargetJson = GetTargetJsonPath();
        if (string.IsNullOrEmpty(origTargetJson))
        {
            return false;
        }

        FixTargetJson(origTargetJson, outputPath);
        return File.Exists(outputPath);
    }

    /// <summary>
    /// Patches the polkatool-generated target JSON to add the "abi" field
    /// required by newer nightly rustc for RISC-V targets.
    /// </summary>
    private static void FixTargetJson(string sourcePath, string destPath)
    {
        using var json = JsonDocument.Parse(File.ReadAllText(sourcePath));
        var root = json.RootElement;

        // Check if "abi" field already exists
        if (root.TryGetProperty("abi", out _))
        {
            File.Copy(sourcePath, destPath, overwrite: true);
            return;
        }

        // Rebuild JSON with "abi" field inserted
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
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

    /// <summary>
    /// Finds the neo-riscv-vm root directory for resolving crate dependencies.
    /// </summary>
    /// <returns>The path to neo-riscv-vm root, or null if not found.</returns>
    public static string? FindRiscvVmRoot()
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

    /// <summary>
    /// Gets the path to the fixed target JSON file, creating it if necessary.
    /// </summary>
    /// <returns>Path to the fixed target JSON file, or null if creation failed.</returns>
    public static string? GetOrCreateFixedTargetJson()
    {
        var targetJson = Path.Combine(Path.GetTempPath(), "neo-riscv32-polkavm.json");
        if (File.Exists(targetJson))
        {
            return targetJson;
        }

        return CreateFixedTargetJson(targetJson) ? targetJson : null;
    }
}
