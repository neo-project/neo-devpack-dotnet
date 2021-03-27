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
                new Argument<string>("path", "The path of the project file, project directory or source file."),
                new Option<string>(new[] { "-o", "--output" }, "Specifies the output directory."),
                new Option<bool>(new[] { "-d", "--debug" }, "Indicates whether to generate debugging information."),
                new Option<bool>("--no-optimize", "Instruct the compiler not to optimize the code."),
                new Option<byte>("--address-version", () => ProtocolSettings.Default.AddressVersion, "Indicates the address version used by the compiler.")
            };
            rootCommand.Handler = CommandHandler.Create<Options, string>(Handle);
            return rootCommand.Invoke(args);
        }

        private static void Handle(Options options, string path)
        {
            if (File.Exists(path))
            {
                switch (Path.GetExtension(path).ToLowerInvariant())
                {
                    case ".cs":
                        ProcessSources(options, Path.GetDirectoryName(path)!, path);
                        break;
                    case ".csproj":
                        ProcessCsproj(options, path);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            else if (Directory.Exists(path))
            {
                ProcessDirectory(options, path);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        private static void ProcessDirectory(Options options, string path)
        {
            string? csproj = Directory.EnumerateFiles(path, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (csproj is null)
            {
                string obj = Path.Combine(path, "obj");
                string[] sourceFiles = Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories).Where(p => !p.StartsWith(obj)).ToArray();
                ProcessSources(options, path, sourceFiles);
            }
            else
            {
                ProcessCsproj(options, csproj);
            }
        }

        private static void ProcessCsproj(Options options, string path)
        {
            ProcessOutputs(options, Path.GetDirectoryName(path)!, CompilationContext.CompileProject(path, options));
        }

        private static void ProcessSources(Options options, string folder, params string[] sourceFiles)
        {
            ProcessOutputs(options, folder, CompilationContext.CompileSources(sourceFiles, options));
        }

        private static void ProcessOutputs(Options options, string folder, CompilationContext context)
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
        }
    }
}
