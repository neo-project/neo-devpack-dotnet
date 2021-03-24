using Neo.IO;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                ShowUsage();
                return;
            }
            string path = args[0];
            if (File.Exists(path))
            {
                switch (Path.GetExtension(path).ToLowerInvariant())
                {
                    case ".cs":
                        ProcessSources(Path.GetDirectoryName(path), path);
                        break;
                    case ".csproj":
                        ProcessDirectory(Path.GetDirectoryName(path));
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

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:\n\tneocs <project_file|project_directory|source_file>");
        }

        private static void ProcessDirectory(string path)
        {
            string obj = Path.Combine(path, "obj");
            string[] sourceFiles = Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories).Where(p => !p.StartsWith(obj)).ToArray();
            ProcessSources(path, sourceFiles);
        }

        private static void ProcessSources(string folder, params string[] sourceFiles)
        {
            folder = Path.Combine(folder, "bin", "sc");
            Directory.CreateDirectory(folder);
            CompilationContext context = CompilationContext.Compile(sourceFiles);
            File.WriteAllBytes($"{folder}/{context.ContractName}.nef", context.CreateExecutable().ToArray());
            File.WriteAllText($"{folder}/{context.ContractName}.manifest.json", context.CreateManifest().ToString());
        }
    }
}
