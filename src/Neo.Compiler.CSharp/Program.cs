// Copyright (C) 2015-2025 The Neo Project.
//
// Program.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Neo.Extensions;
using Neo.Json;
using Neo.Optimizer;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Extensions;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.NamingConventionBinder;
using System.CommandLine.Parsing;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Neo.Compiler
{
    public class Program
    {
        public static int Main(string[] args)
        {
            RootCommand rootCommand = new(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>()!.Title);

            // Add the 'new' subcommand for creating contracts from templates
            var newCommand = new Command("new", "Create a new smart contract from a template")
            {
                new Argument<string>("name", "The name of the new contract"),
                new Option<ContractTemplate>(["-t", "--template"], () => ContractTemplate.Basic, "The template to use (Basic, NEP17, NEP11, Ownable, Oracle)"),
                new Option<string>(["-o", "--output"], () => Environment.CurrentDirectory, "The output directory for the new contract"),
                new Option<string>("--author", () => "Author", "The author of the contract"),
                new Option<string>("--email", () => $"email@example.com", "The author's email"),
                new Option<string>("--description", "A description of the contract"),
                new Option<bool>("--force", "Overwrite existing files"),
                new Option<bool>("--deploy", "Create a deployment toolkit project instead of a contract")
            };
            newCommand.Handler = CommandHandler.Create<string, ContractTemplate, string, string, string, string, bool, bool>(HandleNew);
            rootCommand.AddCommand(newCommand);

            // Add compilation arguments (make them optional for backward compatibility)
            var pathsArgument = new Argument<string[]>("paths", "The path of the solution file, project file, project directory or source files.")
            {
                Arity = ArgumentArity.ZeroOrMore
            };
            rootCommand.AddArgument(pathsArgument);

            rootCommand.AddOption(new Option<string>(["-o", "--output"], "Specifies the output directory."));
            rootCommand.AddOption(new Option<string>("--base-name", "Specifies the base name of the output files."));
            rootCommand.AddOption(new Option<NullableContextOptions>("--nullable", () => NullableContextOptions.Annotations, "Represents the default state of nullable analysis in this compilation."));
            rootCommand.AddOption(new Option<bool>("--checked", "Indicates whether to check for overflow and underflow."));
            rootCommand.AddOption(new Option<bool>("--assembly", "Indicates whether to generate assembly."));
            rootCommand.AddOption(new Option<Options.GenerateArtifactsKind>("--generate-artifacts", "Instruct the compiler how to generate artifacts."));
            rootCommand.AddOption(new Option<bool>("--security-analysis", "Whether to perform security analysis on the compiled contract"));
            rootCommand.AddOption(new Option<bool>("--generate-interface", "Generate interface file for contracts with the Contract attribute"));
            rootCommand.AddOption(new Option<CompilationOptions.OptimizationType>("--optimize", $"Optimization level. e.g. --optimize={CompilationOptions.OptimizationType.All}"));
            rootCommand.AddOption(new Option<bool>("--no-inline", "Instruct the compiler not to insert inline code."));
            rootCommand.AddOption(new Option<byte>("--address-version", () => ProtocolSettings.Default.AddressVersion, "Indicates the address version used by the compiler."));

            var debugOption = new Option<CompilationOptions.DebugType>(["-d", "--debug"],
                new ParseArgument<CompilationOptions.DebugType>(ParseDebug), description: "Indicates the debug level.")
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            rootCommand.AddOption(debugOption);

            rootCommand.Handler = CommandHandler.Create<RootCommand, Options, string[], InvocationContext>(Handle);
            return rootCommand.Invoke(args);
        }

        private static CompilationOptions.DebugType ParseDebug(ArgumentResult result)
        {
            var debugValue = result.Tokens.FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(debugValue)) return CompilationOptions.DebugType.Extended;

            if (!Enum.TryParse<CompilationOptions.DebugType>(debugValue, true, out var ret))
            {
                throw new ArgumentException($"Invalid debug type: {debugValue}");
            }

            return ret;
        }

        private static int HandleNew(string name, ContractTemplate template, string output, string author, string email, string? description, bool force, bool deploy)
        {
            try
            {
                // Validate the project name
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.Error.WriteLine("Error: Contract name cannot be empty.");
                    return 1;
                }

                if (!Regex.IsMatch(name, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
                {
                    Console.Error.WriteLine("Error: Contract name must start with a letter and contain only letters, numbers, and underscores.");
                    return 1;
                }

                // Check if the output directory already contains a project with this name
                string projectPath = Path.Combine(output, name);
                if (Directory.Exists(projectPath) && !force)
                {
                    Console.Error.WriteLine($"Error: Directory '{projectPath}' already exists. Use --force to overwrite.");
                    return 1;
                }

                if (deploy)
                {
                    return DeploymentProjectGenerator.Generate(name, output, force);
                }

                // Create the template manager and generate the contract
                var templateManager = new TemplateManager();

                // List available templates if requested
                Console.WriteLine($"Creating {template} contract: {name}");
                Console.WriteLine($"Output directory: {output}");
                Console.WriteLine($"Author: {author}");
                Console.WriteLine($"Email: {email}");
                if (!string.IsNullOrEmpty(description))
                    Console.WriteLine($"Description: {description}");
                Console.WriteLine();

                // Prepare additional replacements
                var additionalReplacements = new Dictionary<string, string>
                {
                    { "{{Author}}", author },
                    { "{{Email}}", email }
                };
                if (!string.IsNullOrEmpty(description))
                {
                    additionalReplacements["{{Description}}"] = description;
                }

                // Generate the contract from template
                templateManager.GenerateContract(template, name, output, additionalReplacements);
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error creating contract: {ex.Message}");
                return 1;
            }
        }

        private static void Handle(RootCommand command, Options options, string[]? paths, InvocationContext context)
        {
            // Check if the --generate-interface option is present in the command line args
            options.GenerateContractInterface = context.ParseResult.CommandResult.Children
                .Any(token => token.Symbol.Name == "generate-interface");

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
                if (File.Exists(path))
                {
                    string extension = Path.GetExtension(path).ToLowerInvariant();
                    if (extension == ".csproj")
                    {
                        context.ExitCode = ProcessCsproj(options, path);
                        return;
                    }
                    else if (extension == ".sln")
                    {
                        context.ExitCode = ProcessSln(options, path);
                        return;
                    }
                }
            }
            foreach (string path in paths)
            {
                string extension = Path.GetExtension(path).ToLowerInvariant();
                if (extension == ".nef")
                {
                    if (options.Optimize != CompilationOptions.OptimizationType.Experimental)
                    {
                        Console.Error.WriteLine($"Required {nameof(options.Optimize).ToLower()}={options.Optimize}, " +
                            $"but the .nef optimizer supports only {CompilationOptions.OptimizationType.Experimental} level of optimization. ");
                        Console.Error.WriteLine($"Still using {nameof(options.Optimize).ToLower()}={CompilationOptions.OptimizationType.Experimental}");
                        options.Optimize = CompilationOptions.OptimizationType.Experimental;
                    }
                    string directory = Path.GetDirectoryName(path)!;
                    string filename = Path.GetFileNameWithoutExtension(path)!;
                    Console.WriteLine($"Optimizing {filename}.nef to {filename}.optimized.nef...");
                    NefFile nef = NefFile.Parse(File.ReadAllBytes(path));
                    string manifestPath = Path.Join(directory, filename + ".manifest.json");
                    if (!File.Exists(manifestPath))
                        throw new FileNotFoundException($"{filename}.manifest.json required for optimization");
                    ContractManifest manifest = ContractManifest.Parse(File.ReadAllText(manifestPath));
                    string debugInfoPath = Path.Join(directory, filename + ".nefdbgnfo");
                    JObject? debugInfo;
                    if (File.Exists(debugInfoPath))
                        debugInfo = (JObject?)JObject.Parse(DumpNef.UnzipDebugInfo(File.ReadAllBytes(debugInfoPath)));
                    else
                        debugInfo = null;
                    (nef, manifest, debugInfo) = Neo.Optimizer.Optimizer.Optimize(nef, manifest, debugInfo, optimizationType: options.Optimize);
                    File.WriteAllBytes(Path.Combine(directory, filename + ".optimized.nef"), nef.ToArray());
                    File.WriteAllBytes(Path.Combine(directory, filename + ".optimized.manifest.json"), manifest.ToJson().ToByteArray(true));
                    if (options.Assembly)
                    {
                        string dumpnef = DumpNef.GenerateDumpNef(nef, debugInfo, manifest);
                        File.WriteAllText(Path.Combine(directory, filename + ".optimized.nef.txt"), dumpnef);
                    }
                    if (debugInfo != null)
                        File.WriteAllBytes(Path.Combine(directory, filename + ".optimized.nefdbgnfo"), DumpNef.ZipDebugInfo(debugInfo.ToByteArray(true), filename + ".optimized.debug.json"));
                    Console.WriteLine($"Optimization finished.");
                    if (options.SecurityAnalysis)
                        SecurityAnalyzer.SecurityAnalyzer.AnalyzeWithPrint(nef, manifest, debugInfo);
                    return;
                }
                else if (extension != ".cs")
                {
                    Console.Error.WriteLine("The files must have a .cs extension.");
                    context.ExitCode = 1;
                    Console.Error.WriteLine("Maybe invalid command line args. Got the following paths to compile:");
                    foreach (string p in paths)
                        Console.Error.WriteLine($"  {p}");
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
            // First, look for a solution file
            string? sln = Directory.EnumerateFiles(path, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (sln is not null)
            {
                Console.WriteLine($"Found solution file: {Path.GetFileName(sln)}");
                return ProcessSln(options, sln);
            }

            // If no solution file, look for a project file
            string? csproj = Directory.EnumerateFiles(path, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (csproj is not null)
                return ProcessCsproj(options, csproj);

            // Look for solution files in subdirectories
            Console.WriteLine($"No .sln or .csproj file found in \"{path}\". Searching in sub-directories.");
            List<string> slnFiles = Directory.EnumerateFiles(path, "*.sln", SearchOption.AllDirectories).ToList();
            if (slnFiles.Count > 0)
            {
                Console.WriteLine($"Will process {slnFiles.Count} .sln files in sub-directories.");
                return Enumerable.Max(slnFiles.Select((slnFile) =>
                    ProcessSln(options, slnFile)));
            }

            // Look for project files in subdirectories
            List<string> csprojFiles = Directory.EnumerateFiles(path, "*.csproj", SearchOption.AllDirectories).ToList();
            if (csprojFiles.Count > 0)
            {
                Console.WriteLine($"Will process {csprojFiles.Count} .csproj files in sub-directories.");
                return Enumerable.Max(csprojFiles.Select((csprojFile) =>
                    ProcessCsproj(options, csprojFile)));
            }
            string obj = Path.Combine(path, "obj");
            string[] sourceFiles = Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories).Where(p => !p.StartsWith(obj)).ToArray();
            if (sourceFiles.Length == 0)
            {
                Console.Error.WriteLine($"No .cs file is found in \"{path}\".");
                return 2;
            }
            Console.WriteLine($"Will process {sourceFiles.Length} .cs files in the requested path and its sub-directories.");
            return ProcessSources(options, path, sourceFiles);
        }

        private static int ProcessCsproj(Options options, string path)
        {
            return ProcessOutputs(options, Path.GetDirectoryName(path)!, new CompilationEngine(options).CompileProject(path));
        }

        private static int ProcessSln(Options options, string path)
        {
            try
            {
                string solutionDir = Path.GetDirectoryName(path)!;
                string solutionContent = File.ReadAllText(path);

                // Use regex to find all project references in the solution file
                var projectRegex = new Regex(@"Project\(""\{[\w-]+\}""\)\s*=\s*""[^""]*"",\s*""([^""]*\.csproj)"",\s*""\{[\w-]+\}""")
                ;
                var matches = projectRegex.Matches(solutionContent);

                if (matches.Count == 0)
                {
                    Console.Error.WriteLine("No project files found in the solution.");
                    return 1;
                }

                Console.WriteLine($"Found {matches.Count} projects in solution {Path.GetFileName(path)}");
                List<string> projectPaths = new();

                foreach (Match match in matches.Cast<Match>())
                {
                    string relativePath = match.Groups[1].Value;
                    // Replace backslashes with forward slashes for cross-platform compatibility
                    relativePath = relativePath.Replace('\\', Path.DirectorySeparatorChar);
                    string fullPath = Path.GetFullPath(Path.Combine(solutionDir, relativePath));

                    if (File.Exists(fullPath))
                    {
                        projectPaths.Add(fullPath);
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Project file not found: {fullPath}");
                    }
                }

                // Process each project file
                List<CompilationContext> allContexts = new();
                foreach (string projectPath in projectPaths)
                {
                    try
                    {
                        Console.WriteLine($"Compiling project: {Path.GetFileName(projectPath)}");
                        var contexts = new CompilationEngine(options).CompileProject(projectPath);
                        allContexts.AddRange(contexts);
                    }
                    catch (Exception ex) when (!(ex is FormatException && ex.Message.Contains("No valid neo SmartContract found")))
                    {
                        // Only log errors for projects that aren't smart contracts
                        Console.WriteLine($"Error compiling project {Path.GetFileName(projectPath)}: {ex.Message}");
                    }
                }

                if (allContexts.Count == 0)
                {
                    Console.Error.WriteLine("No valid Neo smart contracts found in any projects in the solution.");
                    return 1;
                }

                return ProcessOutputs(options, solutionDir, allContexts);
            }
            catch (Exception ex)
            {
                var compEx = CompilationException.Unexpected($"processing solution '{Path.GetFileName(path)}'", ex);
                Console.Error.WriteLine(compEx.Diagnostic);
                if (compEx.InnerException != null)
                {
                    Console.Error.WriteLine(compEx.InnerException);
                }
                return 1;
            }
        }

        private static int ProcessSources(Options options, string folder, string[] sourceFiles)
        {
            return ProcessOutputs(options, folder, new CompilationEngine(options).CompileSources(sourceFiles));
        }

        private static int ProcessOutputs(Options options, string folder, List<CompilationContext> contexts)
        {
            int result = 0;
            List<CompilationException> exceptions = new();
            foreach (CompilationContext context in contexts)
                try
                {
                    if (ProcessOutput(options, folder, context) != 0)
                        result = 1;
                }
                catch (CompilationException ce)
                {
                    result = 1;
                    exceptions.Add(ce);
                }
                catch (Exception e)
                {
                    result = 1;
                    var contractName = context.ContractName ?? "the current contract";
                    exceptions.Add(CompilationException.Unexpected($"processing contract '{contractName}' outputs", e));
                }
            foreach (CompilationException exception in exceptions)
            {
                Console.Error.WriteLine(exception.Diagnostic);

                if (exception.Diagnostic.Id == DiagnosticId.UnexpectedCompilerError && exception.InnerException != null)
                {
                    Console.Error.WriteLine(exception.InnerException);
                }
            }
            return result;
        }

        private static int ProcessOutput(Options options, string folder, CompilationContext context)
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
                string baseName = context.ContractName!;

                NefFile nef;
                ContractManifest manifest;
                JToken debugInfo;
                try
                {
                    (nef, manifest, debugInfo) = context.CreateResults(folder);
                }
                catch (CompilationException ex)
                {
                    Console.Error.WriteLine(ex.Diagnostic);
                    return -1;
                }

                if (!TryFileOperation("create directory", outputFolder, () => Directory.CreateDirectory(outputFolder)))
                {
                    return 1;
                }

                var nefPath = Path.Combine(outputFolder, $"{baseName}.nef");
                if (!TryFileOperation("write", nefPath, () => File.WriteAllBytes(nefPath, nef.ToArray())))
                {
                    return 1;
                }
                Console.WriteLine($"Created {nefPath}");

                var manifestPath = Path.Combine(outputFolder, $"{baseName}.manifest.json");
                if (!TryFileOperation("write", manifestPath, () => File.WriteAllBytes(manifestPath, manifest.ToJson().ToByteArray(false))))
                {
                    return 1;
                }
                Console.WriteLine($"Created {manifestPath}");

                if (options.GenerateArtifacts != Options.GenerateArtifactsKind.None)
                {
                    var artifact = manifest.GetArtifactsSource(baseName, nef, debugInfo: debugInfo);

                    if (options.GenerateArtifacts.HasFlag(Options.GenerateArtifactsKind.Source))
                    {
                        var artifactSourcePath = Path.Combine(outputFolder, $"{baseName}.artifacts.cs");
                        if (!TryFileOperation("write", artifactSourcePath, () => File.WriteAllText(artifactSourcePath, artifact)))
                        {
                            return 1;
                        }
                        Console.WriteLine($"Created {artifactSourcePath}");
                    }

                    if (options.GenerateArtifacts.HasFlag(Options.GenerateArtifactsKind.Library))
                    {
                        try
                        {
                            // Try to compile the artifacts into a dll

                            var references = new MetadataReference[]
                            {
                                RuntimeAssemblyResolver.CreateFrameworkReference("System.Runtime.dll"),
                                RuntimeAssemblyResolver.CreateFrameworkReference("System.Runtime.InteropServices.dll"),
                                RuntimeAssemblyResolver.CreateFrameworkReference("System.ComponentModel.Primitives.dll"),
                                RuntimeAssemblyResolver.CreateFrameworkReference("System.Runtime.Numerics.dll"),
                                RuntimeAssemblyResolver.CreateFrameworkReference("System.Collections.dll"),
                                RuntimeAssemblyResolver.CreateFrameworkReference("System.Memory.dll"),
                                MetadataReference.CreateFromFile(RuntimeAssemblyResolver.ResolveAssemblyFromType(typeof(NeoSystem))),
                                MetadataReference.CreateFromFile(RuntimeAssemblyResolver.ResolveAssemblyFromType(typeof(SmartContract.Testing.TestEngine)))
                            };

                            CSharpCompilationOptions csOptions = new(
                                    OutputKind.DynamicallyLinkedLibrary,
                                    optimizationLevel: OptimizationLevel.Debug,
                                    platform: Platform.AnyCpu,
                                    nullableContextOptions: NullableContextOptions.Enable,
                                    deterministic: true);

                            var syntaxTree = CSharpSyntaxTree.ParseText(artifact, options: CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest));
                            var compilation = CSharpCompilation.Create(baseName, new[] { syntaxTree }, references, csOptions);

                            using var ms = new MemoryStream();
                            EmitResult result = compilation.Emit(ms);

                            if (!result.Success)
                            {
                                var failures = result.Diagnostics.Where(diagnostic =>
                                    diagnostic.IsWarningAsError ||
                                    diagnostic.Severity == DiagnosticSeverity.Error);

                                foreach (var diagnostic in failures)
                                {
                                    Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                                }
                            }
                            else
                            {
                                ms.Seek(0, SeekOrigin.Begin);

                                // Write dll

                                var artifactDllPath = Path.Combine(outputFolder, $"{baseName}.artifacts.dll");
                                if (!TryFileOperation("write", artifactDllPath, () => File.WriteAllBytes(artifactDllPath, ms.ToArray())))
                                {
                                    return 1;
                                }
                                Console.WriteLine($"Created {artifactDllPath}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"Artifacts compilation error: {ex.Message}");
                        }
                    }
                }
                if (options.Debug != CompilationOptions.DebugType.None)
                {
                    var debugArchivePath = Path.Combine(outputFolder, $"{baseName}.nefdbgnfo");
                    if (!TryFileOperation("write", debugArchivePath, () =>
                    {
                        using FileStream fs = new(debugArchivePath, FileMode.Create, FileAccess.Write);
                        using ZipArchive archive = new(fs, ZipArchiveMode.Create);
                        using Stream stream = archive.CreateEntry($"{baseName}.debug.json").Open();
                        stream.Write(debugInfo.ToByteArray(false));
                    }))
                    {
                        return 1;
                    }
                    Console.WriteLine($"Created {debugArchivePath}");
                }
                if (options.Assembly)
                {
                    var asmPath = Path.Combine(outputFolder, $"{baseName}.asm");
                    if (!TryFileOperation("write", asmPath, () => File.WriteAllText(asmPath, context.CreateAssembly())))
                    {
                        return 1;
                    }
                    Console.WriteLine($"Created {asmPath}");
                    try
                    {
                        var dumpNefContents = DumpNef.GenerateDumpNef(nef, debugInfo, manifest);
                        var dumpNefPath = Path.Combine(outputFolder, $"{baseName}.nef.txt");
                        if (!TryFileOperation("write", dumpNefPath, () => File.WriteAllText(dumpNefPath, dumpNefContents)))
                        {
                            return 1;
                        }
                        Console.WriteLine($"Created {dumpNefPath}");
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Failed to dumpnef: {ex}");
                    }
                }
                Console.WriteLine("Compilation completed successfully.");

                if (options.SecurityAnalysis)
                {
                    Console.WriteLine("Performing security analysis...");
                    try
                    {
                        SecurityAnalyzer.SecurityAnalyzer.AnalyzeWithPrint(nef, manifest, debugInfo);
                    }
                    catch (Exception ex)
                    {
                        var compEx = CompilationException.Unexpected("running security analysis", ex);
                        Console.Error.WriteLine(compEx.Diagnostic);
                        Console.Error.WriteLine(ex);
                    }
                    Console.WriteLine("Finished security analysis.");
                    Console.WriteLine("There can be many false positives in the security analysis. Take it easy.");
                }

                // Generate contract interface if the option is enabled
                if (options.GenerateContractInterface)
                {
                    var contractHash = context.GetContractHash();
                    if (contractHash != null)
                    {
                        var interfacePath = Path.Combine(outputFolder, $"I{baseName}.cs");
                        try
                        {
                            var interfaceSource = ContractInterfaceGenerator.GenerateInterface(baseName, manifest, contractHash);
                            if (TryFileOperation("write", interfacePath, () => File.WriteAllText(interfacePath, interfaceSource)))
                            {
                                Console.WriteLine($"Created contract interface: {interfacePath}");
                            }
                        }
                        catch (Exception ex)
                        {
                            var compEx = CompilationException.Unexpected($"generating interface for contract '{baseName}'", ex);
                            Console.Error.WriteLine(compEx.Diagnostic);
                            Console.Error.WriteLine(ex);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Skipping interface generation for {baseName} as no contract hash was found.");
                    }
                }

                return 0;
            }
            else
            {
                Console.Error.WriteLine("Compilation failed.");
                return 1;
            }
        }

        private static bool TryFileOperation(string operation, string target, Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                var compEx = CompilationException.FileOperation(operation, target, innerException: ex);
                Console.Error.WriteLine(compEx.Diagnostic);
                return false;
            }
        }
    }
}
