// Copyright (C) 2015-2026 The Neo Project.
//
// ArtifactGenerator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Extensions;
using Neo.Json;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Extensions;
using System;
using System.IO;

namespace Neo.SmartContract.Testing
{
    /// <summary>
    /// Standalone artifact generator that allows test projects to generate
    /// testing artifacts without depending on the compiler.
    /// </summary>
    public static class ArtifactGenerator
    {
        /// <summary>
        /// Generate testing artifact source code from .nef and .manifest.json files.
        /// </summary>
        /// <param name="nefPath">Path to the .nef file</param>
        /// <param name="manifestPath">Path to the .manifest.json file</param>
        /// <param name="debugInfoPath">Optional path to debug info file</param>
        /// <param name="contractName">Optional contract name override</param>
        /// <param name="generateProperties">Whether to generate properties</param>
        /// <param name="traceRemarks">Whether to include trace remarks</param>
        /// <returns>Generated C# source code for the testing artifact</returns>
        public static string GenerateArtifactSource(
            string nefPath,
            string manifestPath,
            string? debugInfoPath = null,
            string? contractName = null,
            bool generateProperties = true,
            bool traceRemarks = false)
        {
            if (!File.Exists(nefPath))
                throw new FileNotFoundException($"NEF file not found: {nefPath}");

            if (!File.Exists(manifestPath))
                throw new FileNotFoundException($"Manifest file not found: {manifestPath}");

            // Load NEF file
            var nefBytes = File.ReadAllBytes(nefPath);
            var nef = nefBytes.AsSerializable<NefFile>();

            // Load manifest
            var manifestJson = File.ReadAllText(manifestPath);
            var manifest = ContractManifest.Parse(manifestJson);

            // Load debug info if provided
            JToken? debugInfo = null;
            if (!string.IsNullOrEmpty(debugInfoPath) && File.Exists(debugInfoPath))
            {
                var debugInfoJson = File.ReadAllText(debugInfoPath);
                debugInfo = JToken.Parse(debugInfoJson);
            }

            // Generate artifact source
            return manifest.GetArtifactsSource(
                name: contractName,
                nef: nef,
                generateProperties: generateProperties,
                debugInfo: debugInfo,
                traceRemarks: traceRemarks);
        }

        /// <summary>
        /// Generate testing artifact and save to file.
        /// </summary>
        /// <param name="nefPath">Path to the .nef file</param>
        /// <param name="manifestPath">Path to the .manifest.json file</param>
        /// <param name="outputPath">Output path for the generated .cs file</param>
        /// <param name="debugInfoPath">Optional path to debug info file</param>
        /// <param name="contractName">Optional contract name override</param>
        /// <param name="generateProperties">Whether to generate properties</param>
        /// <param name="traceRemarks">Whether to include trace remarks</param>
        public static void GenerateArtifactFile(
            string nefPath,
            string manifestPath,
            string outputPath,
            string? debugInfoPath = null,
            string? contractName = null,
            bool generateProperties = true,
            bool traceRemarks = false)
        {
            var source = GenerateArtifactSource(
                nefPath,
                manifestPath,
                debugInfoPath,
                contractName,
                generateProperties,
                traceRemarks);

            // Ensure output directory exists
            var outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            File.WriteAllText(outputPath, source);
        }

        /// <summary>
        /// Generate testing artifacts for all contracts in a directory.
        /// Looks for .nef files and their corresponding .manifest.json files.
        /// </summary>
        /// <param name="inputDirectory">Directory containing .nef and .manifest.json files</param>
        /// <param name="outputDirectory">Output directory for generated .cs files</param>
        /// <param name="generateProperties">Whether to generate properties</param>
        /// <param name="traceRemarks">Whether to include trace remarks</param>
        /// <returns>Number of artifacts generated</returns>
        public static int GenerateArtifactsFromDirectory(
            string inputDirectory,
            string outputDirectory,
            bool generateProperties = true,
            bool traceRemarks = false)
        {
            if (!Directory.Exists(inputDirectory))
                throw new DirectoryNotFoundException($"Input directory not found: {inputDirectory}");

            var nefFiles = Directory.GetFiles(inputDirectory, "*.nef");
            var count = 0;

            foreach (var nefPath in nefFiles)
            {
                var baseName = Path.GetFileNameWithoutExtension(nefPath);
                var manifestPath = Path.Combine(inputDirectory, $"{baseName}.manifest.json");

                if (!File.Exists(manifestPath))
                {
                    Console.WriteLine($"Warning: Manifest not found for {baseName}, skipping...");
                    continue;
                }

                var debugInfoPath = Path.Combine(inputDirectory, $"{baseName}.nefdbgnfo");
                if (!File.Exists(debugInfoPath))
                {
                    debugInfoPath = null;
                }

                var outputPath = Path.Combine(outputDirectory, $"{baseName}.cs");

                try
                {
                    GenerateArtifactFile(
                        nefPath,
                        manifestPath,
                        outputPath,
                        debugInfoPath,
                        generateProperties: generateProperties,
                        traceRemarks: traceRemarks);

                    Console.WriteLine($"Generated: {outputPath}");
                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error generating artifact for {baseName}: {ex.Message}");
                }
            }

            return count;
        }
    }
}
