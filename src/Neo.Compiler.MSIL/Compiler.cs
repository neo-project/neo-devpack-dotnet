using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.Compiler
{
    public class Compiler
    {
        public class Assembly
        {
            public byte[] Dll;
            public byte[] Pdb;
        }

        /// <summary>
        /// Build script
        /// </summary>
        /// <param name="filenames">File names</param>
        /// <param name="references">References</param>
        /// <returns></returns>
        public static Assembly BuildScript(string[] filenames, string[] references)
        {
            var coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var refs = new List<MetadataReference>(new MetadataReference[]
            {
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.Numerics.dll")),
                MetadataReference.CreateFromFile(typeof(System.ComponentModel.DisplayNameAttribute).Assembly.Location),

                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            });
            refs.AddRange(references.Select(u => MetadataReference.CreateFromFile(u)));

            var tree = filenames.Select(u => CSharpSyntaxTree.ParseText(File.ReadAllText(u))).ToArray();
            var op = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release);
            var comp = CSharpCompilation.Create("TestContract", tree, refs.ToArray(), op);

            using (var streamDll = new MemoryStream())
            using (var streamPdb = new MemoryStream())
            {
                var result = comp.Emit(streamDll, streamPdb);

                if (!result.Success)
                {
                    throw new ArgumentException(nameof(filenames));
                }

                streamDll.Position = 0;
                streamPdb.Position = 0;

                return new Assembly()
                {
                    Dll = streamDll.ToArray(),
                    Pdb = streamPdb.ToArray()
                };
            }
        }
    }
}
