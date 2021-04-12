using Microsoft.CodeAnalysis;
using Neo.IO;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
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
                new Argument<string?>("path", () => null, "The path of the project file, project directory or source file."),
                new Option<string>(new[] { "-o", "--output" }, "Specifies the output directory."),
                new Option<bool>(new[] { "-d", "--debug" }, "Indicates whether to generate debugging information."),
                new Option<bool>("--assembly", "Indicates whether to generate assembly."),
                new Option<bool>("--no-optimize", "Instruct the compiler not to optimize the code."),
                new Option<bool>("--no-inline", "Instruct the compiler not to insert inline code."),
                new Option<byte>("--address-version", () => ProtocolSettings.Default.AddressVersion, "Indicates the address version used by the compiler.")
            };
            rootCommand.Handler = CommandHandler.Create<Options, string?>(Handle);
            return rootCommand.Invoke(args);
        }

        private static int Handle(Options options, string? path)
        {
            if (path is null)
                path = Environment.CurrentDirectory;
            else
                path = Path.GetFullPath(path);
            if (File.Exists(path))
            {
                if (Path.GetExtension(path).ToLowerInvariant() != ".csproj")
                    throw new NotSupportedException();
                return ProcessCsproj(options, path);
            }
            else if (Directory.Exists(path))
            {
                string? csproj = Directory.EnumerateFiles(path, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (csproj is null)
                    throw new FileNotFoundException("No csproj file is found in the directory.");
                return ProcessCsproj(options, csproj);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        private static int ProcessCsproj(Options options, string path)
        {
            return ProcessOutputs(options, Path.GetDirectoryName(path)!, CompilationContext.Compile(path, options));
        }

        private static int ProcessOutputs(Options options, string folder, CompilationContext context)
        {
            if (context.Success)
            {
                folder = options.Output ?? Path.Combine(folder, "bin", "sc");
                Directory.CreateDirectory(folder);
                File.WriteAllBytes($"{folder}/{context.ContractName}.nef", context.CreateExecutable().ToArray());
                File.WriteAllBytes($"{folder}/{context.ContractName}.manifest.json", context.CreateManifest().ToByteArray(false));
                if (options.Debug)
                {
                    using FileStream fs = new($"{folder}/{context.ContractName}.nefdbgnfo", FileMode.Create, FileAccess.Write);
                    using ZipArchive archive = new(fs, ZipArchiveMode.Create);
                    using Stream stream = archive.CreateEntry($"{context.ContractName}.debug.json").Open();
                    stream.Write(context.CreateDebugInformation().ToByteArray(false));
                }
                if (options.Assembly)
                {
                    File.WriteAllText($"{folder}/{context.ContractName}.asm", context.CreateAssembly());
                }
                return 0;
            }
            else
            {
                foreach (Diagnostic diagnostic in context.Diagnostics)
                {
                    if (diagnostic.Severity == DiagnosticSeverity.Error)
                        Console.Error.WriteLine(diagnostic.ToString());
                    else
                        Console.WriteLine(diagnostic.ToString());
                }
                return 1;
            }
        }
    }
}
