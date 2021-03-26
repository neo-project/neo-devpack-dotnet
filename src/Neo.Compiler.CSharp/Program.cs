using Neo.IO;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
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
                new Argument<string>("path", "The path of the project file, project directory or source file")
            };
            rootCommand.Handler = CommandHandler.Create<string>(Handle);
            return rootCommand.Invoke(args);
        }

        private static void Handle(string path)
        {
            if (File.Exists(path))
            {
                switch (Path.GetExtension(path).ToLowerInvariant())
                {
                    case ".cs":
                        ProcessSources(Path.GetDirectoryName(path)!, path);
                        break;
                    case ".csproj":
                        ProcessCsproj(path);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            else if (Directory.Exists(path))
            {
                ProcessDirectory(path);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        private static void ProcessDirectory(string path)
        {
            string? csproj = Directory.EnumerateFiles(path, "*.csproj", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (csproj is null)
            {
                string obj = Path.Combine(path, "obj");
                string[] sourceFiles = Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories).Where(p => !p.StartsWith(obj)).ToArray();
                ProcessSources(path, sourceFiles);
            }
            else
            {
                ProcessCsproj(csproj);
            }
        }

        private static void ProcessCsproj(string path)
        {
            ProcessOutputs(Path.GetDirectoryName(path)!, CompilationContext.CompileProject(path));
        }

        private static void ProcessSources(string folder, params string[] sourceFiles)
        {
            ProcessOutputs(folder, CompilationContext.CompileSources(sourceFiles));
        }

        private static void ProcessOutputs(string folder, CompilationContext context)
        {
            folder = Path.Combine(folder, "bin", "sc");
            Directory.CreateDirectory(folder);
            File.WriteAllBytes($"{folder}/{context.ContractName}.nef", context.CreateExecutable().ToArray());
            File.WriteAllText($"{folder}/{context.ContractName}.manifest.json", context.CreateManifest().ToString());
        }
    }
}
