// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Neo.IO;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace Neo.Compiler
{
    class Program
    {
        static int Main(string[] args)
        {
            RootCommand rootCommand = new(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>()!.Title)
            {
                new Argument<string[]>("paths", "The path of the project file, project directory or source files."),
                new Option<string>(new[] { "-o", "--output" }, "Specifies the output directory."),
                new Option<string>("--base-name", "Specifies the base name of the output files."),
                new Option<NullableContextOptions>("--nullable", () => NullableContextOptions.Annotations, "Represents the default state of nullable analysis in this compilation."),
                new Option<bool>("--checked", "Indicates whether to check for overflow and underflow."),
                new Option<bool>(new[] { "-d", "--debug" }, "Indicates whether to generate debugging information."),
                new Option<bool>("--assembly", "Indicates whether to generate assembly."),
                new Option<bool>("--no-optimize", "Instruct the compiler not to optimize the code."),
                new Option<bool>("--no-inline", "Instruct the compiler not to insert inline code."),
                new Option<byte>("--address-version", () => ProtocolSettings.Default.AddressVersion, "Indicates the address version used by the compiler.")
            };
            rootCommand.Handler = CommandHandler.Create<RootCommand, Options, string[], InvocationContext>(Handle);
            return rootCommand.Invoke(args);
        }

        private static void Handle(RootCommand command, Options options, string[] paths, InvocationContext context)
        {
            if (paths is null || paths.Length == 0)
            {
                // catch Unhandled exception: System.Reflection.TargetInvocationException
                try
                {
                    context.ExitCode = ProcessDirectory(options, Environment.CurrentDirectory);
                    if (context.ExitCode == 2)
                    {
                        // Display help without args
                        command.Invoke("--help");
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.Error.WriteLine("Unauthorized to access the project directory, or no project is specified. Please ensure you have the proper permissions and a project is specified.");
                }

                return;
            }
            paths = paths.Select(Path.GetFullPath).ToArray();
            if (paths.Length == 1)
            {
                string path = paths[0];
                if (Directory.Exists(path))
                {
                    context.ExitCode = ProcessDirectory(options, path);
                    return;
                }
                if (File.Exists(path) && Path.GetExtension(path).ToLowerInvariant() == ".csproj")
                {
                    context.ExitCode = ProcessCsproj(options, path);
                    return;
                }
            }
            foreach (string path in paths)
            {
                if (Path.GetExtension(path).ToLowerInvariant() != ".cs")
                {
                    Console.Error.WriteLine("The files must have a .cs extension.");
                    context.ExitCode = 1;
                    return;
                }
                if (!File.Exists(path))
                {
                    Console.Error.WriteLine($"The file \"{path}\" doesn't exist.");
                    context.ExitCode = 1;
                    return;
                }
            }
            context.ExitCode = ProcessSources(options, Path.GetDirectoryName(paths[0])!, paths);
        }

        private static int ProcessDirectory(Options options, string path)
        {
            string? csproj = Directory.EnumerateFiles(path, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (csproj is null)
            {
                string obj = Path.Combine(path, "obj");
                string[] sourceFiles = Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories).Where(p => !p.StartsWith(obj)).ToArray();
                if (sourceFiles.Length == 0)
                {
                    Console.Error.WriteLine($"No .cs file is found in \"{path}\".");
                    return 2;
                }
                return ProcessSources(options, path, sourceFiles);
            }
            else
            {
                return ProcessCsproj(options, csproj);
            }
        }

        private static int ProcessCsproj(Options options, string path)
        {
            return ProcessOutputs(options, Path.GetDirectoryName(path)!, CompilationContext.CompileProject(path, options));
        }

        private static int ProcessSources(Options options, string folder, string[] sourceFiles)
        {
            return ProcessOutputs(options, folder, CompilationContext.CompileSources(sourceFiles, options));
        }

        private static int ProcessOutputs(Options options, string folder, CompilationContext context)
        {
            foreach (Diagnostic diagnostic in context.Diagnostics)
            {
                if (diagnostic.Severity == DiagnosticSeverity.Error)
                    Console.Error.WriteLine(diagnostic.ToString());
                else
                    Console.WriteLine(diagnostic.ToString());
            }
            if (context.Success)
            {
                string outputFolder = options.Output ?? Path.Combine(folder, "bin", "sc");
                string path = outputFolder;
                string baseName = options.BaseName ?? context.ContractName!;
                try
                {
                    Directory.CreateDirectory(outputFolder);
                    path = Path.Combine(path, $"{baseName}.nef");
                    File.WriteAllBytes(path, context.CreateExecutable().ToArray());
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Can't create {path}. {ex.Message}.");
                    return 1;
                }
                Console.WriteLine($"Created {path}");
                path = Path.Combine(outputFolder, $"{baseName}.manifest.json");
                try
                {
                    File.WriteAllBytes(path, context.CreateManifest().ToJson().ToByteArray(false));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Can't create {path}. {ex.Message}.");
                    return 1;
                }
                Console.WriteLine($"Created {path}");
                if (options.Debug)
                {
                    path = Path.Combine(outputFolder, $"{baseName}.nefdbgnfo");
                    using FileStream fs = new(path, FileMode.Create, FileAccess.Write);
                    using ZipArchive archive = new(fs, ZipArchiveMode.Create);
                    using Stream stream = archive.CreateEntry($"{baseName}.debug.json").Open();
                    stream.Write(context.CreateDebugInformation(folder).ToByteArray(false));
                    Console.WriteLine($"Created {path}");
                }
                if (options.Assembly)
                {
                    path = Path.Combine(outputFolder, $"{baseName}.asm");
                    File.WriteAllText(path, context.CreateAssembly());
                    Console.WriteLine($"Created {path}");
                }
                Console.WriteLine("Compilation completed successfully.");
                return 0;
            }
            else
            {
                Console.Error.WriteLine("Compilation failed.");
                return 1;
            }
        }
    }
}
