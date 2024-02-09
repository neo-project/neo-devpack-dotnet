using Akka.Util.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler
{
    public class Compiler
    {
        private static readonly List<MetadataReference> references = new();

        /// <summary>
        /// Static constructor
        /// </summary>
        static Compiler()
        {
            string coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            references.Add(MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.InteropServices.dll")));
            references.Add(MetadataReference.CreateFromFile(typeof(string).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(DisplayNameAttribute).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(BigInteger).Assembly.Location));
            string folder = Path.GetFullPath("../../../../../src/Neo.SmartContract.Framework/");
            string obj = Path.Combine(folder, "obj");
            IEnumerable<SyntaxTree> st = Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories)
                .Where(p => !p.StartsWith(obj))
                .OrderBy(p => p)
                .Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), path: p));
            CSharpCompilationOptions options = new(OutputKind.DynamicallyLinkedLibrary);
            CSharpCompilation cr = CSharpCompilation.Create(null, st, references, options);
            references.Add(cr.ToMetadataReference());
        }

        /// <summary>
        /// Compile c# source
        /// </summary>
        /// <param name="optimize">Optimize</param>
        /// <param name="debug">Debug</param>
        /// <param name="files">Files</param>
        /// <returns>Compilation Result</returns>
        public static CompilationResult Compile(bool optimize = true, bool debug = true, params string[] files)
        {
            CompilationContext context = CompilationContext.Compile(files, references, new Options
            {
                AddressVersion = ProtocolSettings.Default.AddressVersion,
                Debug = debug,
                NoOptimize = !optimize
            });
            if (context.Success)
            {
                return new CompilationResult()
                {
                    Nef = context.CreateExecutable(),
                    Manifest = context.CreateManifest(),
                    DebugInfo = context.CreateDebugInformation()
                };
            }
            else
            {
                context.Diagnostics.ForEach(Console.Error.WriteLine);

                throw new ApplicationException("Error while compiling");
            }
        }
    }
}
