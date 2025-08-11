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
using System.Text;
using System.Text.RegularExpressions;

namespace Neo.Compiler
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var rootCommand = new RootCommand(Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>()!.Title)
            {
                Description = "Neo Smart Contract Compiler and Tools"
            };
            
            // Add a hidden argument to support backward compatibility
            rootCommand.AddArgument(new Argument<string[]>("paths", () => null!) { IsHidden = true, Arity = ArgumentArity.ZeroOrMore });

            // Create the 'compile' subcommand with the existing compilation functionality
            var compileCommand = new Command("compile", "Compile Neo smart contracts")
            {
                new Argument<string[]>("paths", "The path of the solution file, project file, project directory or source files.") { Arity = ArgumentArity.ZeroOrMore },
                new Option<string>(["-o", "--output"], "Specifies the output directory."),
                new Option<string>("--base-name", "Specifies the base name of the output files."),
                new Option<NullableContextOptions>("--nullable", () => NullableContextOptions.Annotations, "Represents the default state of nullable analysis in this compilation."),
                new Option<bool>("--checked", "Indicates whether to check for overflow and underflow."),
                new Option<bool>("--assembly", "Indicates whether to generate assembly."),
                new Option<Options.GenerateArtifactsKind>("--generate-artifacts", "Instruct the compiler how to generate artifacts."),
                new Option<bool>("--security-analysis", "Whether to perform security analysis on the compiled contract"),
                new Option<bool>("--generate-interface", "Generate interface file for contracts with the Contract attribute"),
                new Option<CompilationOptions.OptimizationType>("--optimize", $"Optimization level. e.g. --optimize={CompilationOptions.OptimizationType.All}"),
                new Option<bool>("--no-inline", "Instruct the compiler not to insert inline code."),
                new Option<byte>("--address-version", () => ProtocolSettings.Default.AddressVersion, "Indicates the address version used by the compiler.")
            };

            var debugOption = new Option<CompilationOptions.DebugType>(["-d", "--debug"],
                new ParseArgument<CompilationOptions.DebugType>(ParseDebug), description: "Indicates the debug level.")
            {
                Arity = ArgumentArity.ZeroOrOne
            };
            compileCommand.AddOption(debugOption);
            compileCommand.Handler = CommandHandler.Create<Options, string[], InvocationContext>(HandleCompile);

            // Create the 'new' subcommand for generating contracts from templates
            var newCommand = new Command("new", "Create a new smart contract from a template")
            {
                new Argument<string>("name", "The name of the contract to create"),
                new Option<string>(["-t", "--template"], () => "basic", "The template to use (basic, nep17, nft, oracle, ownable)"),
                new Option<string>(["-o", "--output"], "The output directory for the new contract"),
                new Option<bool>(["-f", "--force"], "Overwrite existing files")
            };
            newCommand.Handler = CommandHandler.Create<string, string, string?, bool>(HandleNew);

            // Add compile options to root for backward compatibility
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
            rootCommand.AddOption(debugOption);

            rootCommand.AddCommand(compileCommand);
            rootCommand.AddCommand(newCommand);

            // Make compile the default command when no subcommand is specified
            rootCommand.Handler = CommandHandler.Create<Options, string[], InvocationContext>((options, paths, context) =>
            {
                // If arguments are provided without a subcommand, assume compile command
                if (paths != null && paths.Length > 0)
                {
                    // Use HandleCompile directly for backward compatibility
                    HandleCompile(options, paths, context);
                    return context.ExitCode;
                }
                // Otherwise show help
                return rootCommand.Invoke("--help");
            });

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

        private static void HandleNew(string name, string template, string? output, bool force)
        {
            try
            {
                // Validate contract name
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.Error.WriteLine("Contract name cannot be empty.");
                    return;
                }

                // Clean the name to be a valid C# identifier
                string cleanName = System.Text.RegularExpressions.Regex.Replace(name, @"[^a-zA-Z0-9_]", "");
                if (char.IsDigit(cleanName[0]))
                {
                    cleanName = "_" + cleanName;
                }

                // Determine output directory
                string outputDir = output ?? Path.Combine(Environment.CurrentDirectory, cleanName);

                // Check if directory exists
                if (Directory.Exists(outputDir) && !force)
                {
                    Console.Error.WriteLine($"Directory '{outputDir}' already exists. Use --force to overwrite.");
                    return;
                }

                // Validate template before creating directory
                var validTemplates = new[] { "basic", "nep17", "nft", "nep11", "oracle", "ownable" };
                if (!validTemplates.Contains(template.ToLower()))
                {
                    Console.Error.WriteLine($"Unknown template: {template}. Available templates: basic, nep17, nft, oracle, ownable");
                    return;
                }

                // Create directory
                Directory.CreateDirectory(outputDir);

                // Generate contract based on template
                switch (template.ToLower())
                {
                    case "basic":
                        GenerateBasicContract(outputDir, cleanName);
                        break;
                    case "nep17":
                        GenerateNep17Contract(outputDir, cleanName);
                        break;
                    case "nft":
                    case "nep11":
                        GenerateNftContract(outputDir, cleanName);
                        break;
                    case "oracle":
                        GenerateOracleContract(outputDir, cleanName);
                        break;
                    case "ownable":
                        GenerateOwnableContract(outputDir, cleanName);
                        break;
                }

                Console.WriteLine($"Successfully created {template} contract '{cleanName}' in {outputDir}");
                Console.WriteLine();
                Console.WriteLine("Next steps:");
                Console.WriteLine($"  cd {outputDir}");
                Console.WriteLine($"  nccs compile");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error creating contract: {ex.Message}");
            }
        }

        private static void HandleCompile(Options options, string[]? paths, InvocationContext context)
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
                        Console.WriteLine("No project files found. Use 'nccs --help' for usage information.");
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
                Console.Error.WriteLine($"Unexpected error processing solution: {ex.Message}");
                Console.Error.WriteLine(ex.StackTrace);
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
            List<Exception> exceptions = new();
            foreach (CompilationContext context in contexts)
                try
                {
                    if (ProcessOutput(options, folder, context) != 0)
                        result = 1;
                }
                catch (Exception e)
                {
                    result = 1;
                    exceptions.Add(e);
                }
            foreach (Exception e in exceptions)
                Console.Error.WriteLine(e.ToString());
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
                string path = outputFolder;
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

                try
                {
                    Directory.CreateDirectory(outputFolder);
                    path = Path.Combine(path, $"{baseName}.nef");
                    File.WriteAllBytes(path, nef.ToArray());
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
                    File.WriteAllBytes(path, manifest.ToJson().ToByteArray(false));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Can't create {path}. {ex.Message}.");
                    return 1;
                }
                Console.WriteLine($"Created {path}");

                if (options.GenerateArtifacts != Options.GenerateArtifactsKind.None)
                {
                    var artifact = manifest.GetArtifactsSource(baseName, nef, debugInfo: debugInfo);

                    if (options.GenerateArtifacts.HasFlag(Options.GenerateArtifactsKind.Source))
                    {
                        path = Path.Combine(outputFolder, $"{baseName}.artifacts.cs");
                        File.WriteAllText(path, artifact);
                        Console.WriteLine($"Created {path}");
                    }

                    if (options.GenerateArtifacts.HasFlag(Options.GenerateArtifactsKind.Library))
                    {
                        try
                        {
                            // Try to compile the artifacts into a dll

                            var coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
                            var references = new MetadataReference[]
                            {
                                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")),
                                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                                MetadataReference.CreateFromFile(typeof(DisplayNameAttribute).Assembly.Location),
                                MetadataReference.CreateFromFile(typeof(System.Numerics.BigInteger).Assembly.Location),
                                MetadataReference.CreateFromFile(typeof(NeoSystem).Assembly.Location),
                                MetadataReference.CreateFromFile(typeof(SmartContract.Testing.TestEngine).Assembly.Location)
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

                                path = Path.Combine(outputFolder, $"{baseName}.artifacts.dll");
                                File.WriteAllBytes(path, ms.ToArray());
                                Console.WriteLine($"Created {path}");
                            }
                        }
                        catch
                        {
                            Console.Error.WriteLine("Artifacts compilation error.");
                        }
                    }
                }
                if (options.Debug != CompilationOptions.DebugType.None)
                {
                    path = Path.Combine(outputFolder, $"{baseName}.nefdbgnfo");
                    using FileStream fs = new(path, FileMode.Create, FileAccess.Write);
                    using ZipArchive archive = new(fs, ZipArchiveMode.Create);
                    using Stream stream = archive.CreateEntry($"{baseName}.debug.json").Open();
                    stream.Write(debugInfo.ToByteArray(false));
                    Console.WriteLine($"Created {path}");
                }
                if (options.Assembly)
                {
                    path = Path.Combine(outputFolder, $"{baseName}.asm");
                    File.WriteAllText(path, context.CreateAssembly());
                    Console.WriteLine($"Created {path}");
                    try
                    {
                        path = Path.Combine(outputFolder, $"{baseName}.nef.txt");
                        File.WriteAllText(path, DumpNef.GenerateDumpNef(nef, debugInfo, manifest));
                        Console.WriteLine($"Created {path}");
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
                    catch (Exception e) { Console.WriteLine(e); }
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
                            File.WriteAllText(interfacePath, interfaceSource);
                            Console.WriteLine($"Created contract interface: {interfacePath}");
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"Error generating contract interface: {ex.Message}");
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

        #region Template Generation Methods

        private static void GenerateBasicContract(string outputDir, string contractName)
        {
            string contractPath = Path.Combine(outputDir, $"{contractName}.cs");
            string projectPath = Path.Combine(outputDir, $"{contractName}.csproj");

            // Generate basic contract code
            string contractCode = $@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {contractName}Contract
{{
    [DisplayName(""{contractName}"")]
    [ContractAuthor(""<Your Name>"", ""<Your Email>"")]
    [ContractDescription(""<Description of your contract>"")]
    [ContractVersion(""1.0.0"")]
    [ContractSourceCode(""https://github.com/your-repo/{contractName}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {contractName} : SmartContract
    {{
        private const byte Prefix_Owner = 0xff;

        [Safe]
        public static UInt160 GetOwner()
        {{
            return (UInt160)Storage.Get(new[] {{ Prefix_Owner }});
        }}

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public static bool Verify() => IsOwner();

        public static void _deploy(object data, bool update)
        {{
            if (update) return;
            
            var initialOwner = data is null ? Runtime.Transaction.Sender : (UInt160)data;
            Storage.Put(new[] {{ Prefix_Owner }}, initialOwner);
        }}

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Update(nefFile, manifest, data);
        }}

        public static void Destroy()
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Destroy();
        }}

        [Safe]
        public static string HelloWorld()
        {{
            return ""Hello, World!"";
        }}

        public static BigInteger Add(BigInteger a, BigInteger b)
        {{
            return a + b;
        }}

        public static void SetValue(string key, string value)
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            Storage.Put(key, value);
        }}

        [Safe]
        public static string GetValue(string key)
        {{
            return Storage.Get(key);
        }}
    }}
}}";

            // Generate project file
            string projectContent = $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.8.1-*"" />
  </ItemGroup>

</Project>";

            File.WriteAllText(contractPath, contractCode);
            File.WriteAllText(projectPath, projectContent);
        }

        private static void GenerateNep17Contract(string outputDir, string contractName)
        {
            string contractPath = Path.Combine(outputDir, $"{contractName}.cs");
            string projectPath = Path.Combine(outputDir, $"{contractName}.csproj");

            // Generate NEP-17 token contract
            string contractCode = $@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {contractName}Contract
{{
    [DisplayName(""{contractName}"")]
    [ContractAuthor(""<Your Name>"", ""<Your Email>"")]
    [ContractDescription(""NEP-17 Token Contract"")]
    [ContractVersion(""1.0.0"")]
    [ContractSourceCode(""https://github.com/your-repo/{contractName}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(NepStandard.Nep17)]
    public class {contractName} : Nep17Token
    {{
        private const byte Prefix_Owner = 0xff;

        public override string Symbol {{ [Safe] get => ""EXAMPLE""; }}
        public override byte Decimals {{ [Safe] get => 8; }}

        [Safe]
        public static UInt160 GetOwner()
        {{
            return (UInt160)Storage.Get(new[] {{ Prefix_Owner }});
        }}

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public delegate void OnSetOwnerDelegate(UInt160 previousOwner, UInt160 newOwner);

        [DisplayName(""SetOwner"")]
        public static event OnSetOwnerDelegate OnSetOwner;

        public static void SetOwner(UInt160 newOwner)
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, ""owner must be valid"");

            UInt160 previous = GetOwner();
            Storage.Put(new[] {{ Prefix_Owner }}, newOwner);
            OnSetOwner(previous, newOwner);
        }}

        public static new void Burn(UInt160 account, BigInteger amount)
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");
            Nep17Token.Burn(account, amount);
        }}

        public static new void Mint(UInt160 to, BigInteger amount)
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");
            Nep17Token.Mint(to, amount);
        }}

        [Safe]
        public static bool Verify() => IsOwner();

        public static void _deploy(object data, bool update)
        {{
            if (update) return;

            var initialOwner = data is null ? Runtime.Transaction.Sender : (UInt160)data;
            ExecutionEngine.Assert(initialOwner.IsValid && !initialOwner.IsZero, ""owner must exists"");

            Storage.Put(new[] {{ Prefix_Owner }}, initialOwner);
            OnSetOwner(null, initialOwner);
        }}

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Update(nefFile, manifest, data);
        }}
    }}
}}";

            string projectContent = $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.8.1-*"" />
  </ItemGroup>

</Project>";

            File.WriteAllText(contractPath, contractCode);
            File.WriteAllText(projectPath, projectContent);
        }

        private static void GenerateNftContract(string outputDir, string contractName)
        {
            string contractPath = Path.Combine(outputDir, $"{contractName}.cs");
            string projectPath = Path.Combine(outputDir, $"{contractName}.csproj");

            // Generate NEP-11 NFT contract
            string contractCode = $@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {contractName}Contract
{{
    [DisplayName(""{contractName}"")]
    [ContractAuthor(""<Your Name>"", ""<Your Email>"")]
    [ContractDescription(""NEP-11 NFT Contract"")]
    [ContractVersion(""1.0.0"")]
    [ContractSourceCode(""https://github.com/your-repo/{contractName}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(NepStandard.Nep11)]
    public class {contractName} : Nep11Token<{contractName}State>
    {{
        private const byte Prefix_Owner = 0xff;
        private const byte Prefix_TokenId = 0x02;

        public override string Symbol {{ [Safe] get => ""EXAMPLE""; }}

        [Safe]
        public static UInt160 GetOwner()
        {{
            return (UInt160)Storage.Get(new[] {{ Prefix_Owner }});
        }}

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public static void _deploy(object data, bool update)
        {{
            if (update) return;

            var initialOwner = data is null ? Runtime.Transaction.Sender : (UInt160)data;
            ExecutionEngine.Assert(initialOwner.IsValid && !initialOwner.IsZero, ""owner must exists"");

            Storage.Put(new[] {{ Prefix_Owner }}, initialOwner);
            Storage.Put(new[] {{ Prefix_TokenId }}, 0);
        }}

        public static void Mint(UInt160 to, string name, string description, string image)
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            var tokenId = GetNextTokenId();
            var state = new {contractName}State
            {{
                Owner = to,
                Name = name,
                Description = description,
                Image = image
            }};

            Mint(tokenId, state);
        }}

        private static ByteString GetNextTokenId()
        {{
            var currentId = (BigInteger)Storage.Get(new[] {{ Prefix_TokenId }});
            var nextId = currentId + 1;
            Storage.Put(new[] {{ Prefix_TokenId }}, nextId);
            return (ByteString)nextId.ToByteArray();
        }}

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Update(nefFile, manifest, data);
        }}

        public static void Destroy()
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Destroy();
        }}

        [Safe]
        public static bool Verify() => IsOwner();
    }}

    public class {contractName}State : Nep11TokenState
    {{
        public string Description {{ get; set; }} = string.Empty;
        public string Image {{ get; set; }} = string.Empty;
    }}
}}";

            string projectContent = $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.8.1-*"" />
  </ItemGroup>

</Project>";

            File.WriteAllText(contractPath, contractCode);
            File.WriteAllText(projectPath, projectContent);
        }

        private static void GenerateOracleContract(string outputDir, string contractName)
        {
            string contractPath = Path.Combine(outputDir, $"{contractName}.cs");
            string projectPath = Path.Combine(outputDir, $"{contractName}.csproj");

            // Generate Oracle contract
            string contractCode = $@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {contractName}Contract
{{
    [DisplayName(""{contractName}"")]
    [ContractAuthor(""<Your Name>"", ""<Your Email>"")]
    [ContractDescription(""Oracle Request Contract"")]
    [ContractVersion(""1.0.0"")]
    [ContractSourceCode(""https://github.com/your-repo/{contractName}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {contractName} : SmartContract
    {{
        private const byte Prefix_Owner = 0xff;
        private const byte Prefix_RequestId = 0x10;

        [Safe]
        public static UInt160 GetOwner()
        {{
            return (UInt160)Storage.Get(new[] {{ Prefix_Owner }});
        }}

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        public static void _deploy(object data, bool update)
        {{
            if (update) return;

            var initialOwner = data is null ? Runtime.Transaction.Sender : (UInt160)data;
            Storage.Put(new[] {{ Prefix_Owner }}, initialOwner);
        }}

        public static void RequestData(string url, string filter, string callback, object userData, long gasForResponse)
        {{
            if (gasForResponse < 10000000)
                throw new InvalidOperationException(""Insufficient gas for response"");

            Oracle.Request(url, filter, callback, userData, gasForResponse);
        }}

        public static void OnOracleResponse(string url, object userData, int code, byte[] result)
        {{
            if (Runtime.CallingScriptHash != Oracle.Hash)
                throw new InvalidOperationException(""Unauthorized"");

            if (code != 0)
            {{
                Runtime.Log(""Oracle request failed"");
                return;
            }}

            // Process the oracle response
            var responseData = StdLib.JsonDeserialize(result);
            ProcessOracleData(userData, responseData);
        }}

        private static void ProcessOracleData(object userData, object responseData)
        {{
            // Store or process the oracle response data
            var key = new byte[] {{ Prefix_RequestId }}.Concat((ByteString)userData);
            Storage.Put(key, StdLib.JsonSerialize(responseData));
            
            Runtime.Log(""Oracle data processed successfully"");
        }}

        [Safe]
        public static ByteString GetOracleData(ByteString requestId)
        {{
            var key = new byte[] {{ Prefix_RequestId }}.Concat(requestId);
            return Storage.Get(key);
        }}

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Update(nefFile, manifest, data);
        }}

        [Safe]
        public static bool Verify() => IsOwner();
    }}
}}";

            string projectContent = $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.8.1-*"" />
  </ItemGroup>

</Project>";

            File.WriteAllText(contractPath, contractCode);
            File.WriteAllText(projectPath, projectContent);
        }

        private static void GenerateOwnableContract(string outputDir, string contractName)
        {
            string contractPath = Path.Combine(outputDir, $"{contractName}.cs");
            string projectPath = Path.Combine(outputDir, $"{contractName}.csproj");

            // Generate Ownable contract
            string contractCode = $@"using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace {contractName}Contract
{{
    [DisplayName(""{contractName}"")]
    [ContractAuthor(""<Your Name>"", ""<Your Email>"")]
    [ContractDescription(""Ownable Contract with Access Control"")]
    [ContractVersion(""1.0.0"")]
    [ContractSourceCode(""https://github.com/your-repo/{contractName}"")]
    [ContractPermission(Permission.Any, Method.Any)]
    public class {contractName} : SmartContract
    {{
        private const byte Prefix_Owner = 0xff;
        private const byte Prefix_Admin = 0xfe;
        private const byte Prefix_Data = 0x01;

        public delegate void OnSetOwnerDelegate(UInt160 previousOwner, UInt160 newOwner);
        public delegate void OnSetAdminDelegate(UInt160 admin, bool isAdmin);

        [DisplayName(""SetOwner"")]
        public static event OnSetOwnerDelegate OnSetOwner;

        [DisplayName(""SetAdmin"")]
        public static event OnSetAdminDelegate OnSetAdmin;

        [Safe]
        public static UInt160 GetOwner()
        {{
            return (UInt160)Storage.Get(new[] {{ Prefix_Owner }});
        }}

        private static bool IsOwner() =>
            Runtime.CheckWitness(GetOwner());

        [Safe]
        public static bool IsAdmin(UInt160 account)
        {{
            if (account == GetOwner()) return true;
            return Storage.Get(new[] {{ Prefix_Admin }}.Concat(account)).Equals(1);
        }}

        private static bool HasPermission()
        {{
            var caller = Runtime.Transaction.Sender;
            return IsOwner() || IsAdmin(caller);
        }}

        public static void SetOwner(UInt160 newOwner)
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, ""owner must be valid"");

            UInt160 previous = GetOwner();
            Storage.Put(new[] {{ Prefix_Owner }}, newOwner);
            OnSetOwner(previous, newOwner);
        }}

        public static void SetAdmin(UInt160 admin, bool isAdmin)
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No Authorization!"");

            ExecutionEngine.Assert(admin.IsValid && !admin.IsZero, ""admin must be valid"");

            if (isAdmin)
                Storage.Put(new[] {{ Prefix_Admin }}.Concat(admin), 1);
            else
                Storage.Delete(new[] {{ Prefix_Admin }}.Concat(admin));

            OnSetAdmin(admin, isAdmin);
        }}

        public static void _deploy(object data, bool update)
        {{
            if (update) return;

            var initialOwner = data is null ? Runtime.Transaction.Sender : (UInt160)data;
            ExecutionEngine.Assert(initialOwner.IsValid && !initialOwner.IsZero, ""owner must exists"");

            Storage.Put(new[] {{ Prefix_Owner }}, initialOwner);
            OnSetOwner(null, initialOwner);
        }}

        public static void SetData(string key, ByteString value)
        {{
            if (!HasPermission())
                throw new InvalidOperationException(""No Authorization!"");

            var storageKey = new byte[] {{ Prefix_Data }}.Concat(key);
            Storage.Put(storageKey, value);
        }}

        [Safe]
        public static ByteString GetData(string key)
        {{
            var storageKey = new byte[] {{ Prefix_Data }}.Concat(key);
            return Storage.Get(storageKey);
        }}

        public static void DeleteData(string key)
        {{
            if (!HasPermission())
                throw new InvalidOperationException(""No Authorization!"");

            var storageKey = new byte[] {{ Prefix_Data }}.Concat(key);
            Storage.Delete(storageKey);
        }}

        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Update(nefFile, manifest, data);
        }}

        public static void Destroy()
        {{
            if (!IsOwner())
                throw new InvalidOperationException(""No authorization."");
            ContractManagement.Destroy();
        }}

        [Safe]
        public static bool Verify() => IsOwner();
    }}
}}";

            string projectContent = $@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.8.1-*"" />
  </ItemGroup>

</Project>";

            File.WriteAllText(contractPath, contractCode);
            File.WriteAllText(projectPath, projectContent);
        }

        #endregion
    }
}
