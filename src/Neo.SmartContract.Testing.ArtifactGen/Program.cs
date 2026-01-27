// Copyright (C) 2015-2026 The Neo Project.
//
// Program.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.CommandLine;

namespace Neo.SmartContract.Testing.ArtifactGen;

class Program
{
    static int Main(string[] args)
    {
        var rootCommand = new RootCommand("Generate testing artifacts from compiled NEO smart contracts");

        // Single file command
        var nefOption = new Option<FileInfo>("--nef", "Path to the .nef file") { IsRequired = true };
        var manifestOption = new Option<FileInfo>("--manifest", "Path to the .manifest.json file") { IsRequired = true };
        var outputOption = new Option<FileInfo>("--output", "Output path for the generated .cs file") { IsRequired = true };
        var debugOption = new Option<FileInfo?>("--debug", "Path to debug info file");
        var nameOption = new Option<string?>("--name", "Contract name override");

        var generateCommand = new Command("generate", "Generate artifact from a single contract")
        {
            nefOption, manifestOption, outputOption, debugOption, nameOption
        };

        generateCommand.SetHandler((nef, manifest, output, debug, name) =>
        {
            try
            {
                ArtifactGenerator.GenerateArtifactFile(
                    nef.FullName,
                    manifest.FullName,
                    output.FullName,
                    debug?.FullName,
                    name);
                Console.WriteLine($"Generated: {output.FullName}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }, nefOption, manifestOption, outputOption, debugOption, nameOption);

        // Batch command
        var inputDirOption = new Option<DirectoryInfo>("--input", "Input directory containing .nef files") { IsRequired = true };
        var outputDirOption = new Option<DirectoryInfo>("--output", "Output directory for generated .cs files") { IsRequired = true };

        var batchCommand = new Command("batch", "Generate artifacts for all contracts in a directory")
        {
            inputDirOption, outputDirOption
        };

        batchCommand.SetHandler((input, output) =>
        {
            try
            {
                var count = ArtifactGenerator.GenerateArtifactsFromDirectory(
                    input.FullName,
                    output.FullName);
                Console.WriteLine($"Generated {count} artifacts");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                Environment.Exit(1);
            }
        }, inputDirOption, outputDirOption);

        rootCommand.AddCommand(generateCommand);
        rootCommand.AddCommand(batchCommand);

        return rootCommand.Invoke(args);
    }
}
