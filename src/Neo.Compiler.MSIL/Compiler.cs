extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using scfxSmartContract = scfx.Neo.SmartContract.Framework.SmartContract;

namespace Neo.Compiler
{
    public class Compiler
    {
        public class Assembly
        {
            public readonly byte[] Dll;
            public readonly byte[] Pdb;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="dll">Library</param>
            /// <param name="pdb">PDB</param>
            public Assembly(byte[] dll, byte[] pdb)
            {
                Dll = dll;
                Pdb = pdb;
            }

            /// <summary>
            /// Create Assembly
            /// </summary>
            /// <param name="comp">Compilation</param>
            /// <returns>Assembly</returns>
            internal static Assembly Create(Compilation comp)
            {
                using (var streamDll = new MemoryStream())
                using (var streamPdb = new MemoryStream())
                {
                    var result = comp.Emit(streamDll, streamPdb);

                    if (!result.Success)
                    {
                        throw new ArgumentException(string.Join(Environment.NewLine, result.Diagnostics.Select(u => u.ToString())));
                    }

                    streamDll.Position = 0;
                    streamPdb.Position = 0;

                    return new Assembly(streamDll.ToArray(), streamPdb.ToArray());
                }
            }
        }

        /// <summary>
        /// Build script
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="releaseMode">Release mode (default=true)</param>
        /// <returns>Assembly</returns>
        public static Assembly CompileVBProj(string filename, bool releaseMode = true)
        {
            ExtractFileAndReferences(filename, ".vb", out var files, out var references);
            return CompileVBFiles(files.ToArray(), references.ToArray(), releaseMode);
        }

        /// <summary>
        /// Build script
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="releaseMode">Release mode (default=true)</param>
        /// <returns>Assembly</returns>
        public static Assembly CompileCSProj(string filename, bool releaseMode = true)
        {
            ExtractFileAndReferences(filename, ".cs", out var files, out var references);
            return CompileCSFiles(files.ToArray(), references.ToArray(), releaseMode);
        }

        /// <summary>
        /// Extract information in project files
        /// </summary>
        /// <param name="filename">File name</param>
        /// <param name="extension">Extension</param>
        /// <param name="files">Files</param>
        /// <param name="references">References</param>
        private static void ExtractFileAndReferences(string filename, string extension, out List<string> files, out List<string> references)
        {
            var fileInfo = new FileInfo(filename);

            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException(filename);
            }

            // Compile csproj source

            var reader = XmlReader.Create(filename, new XmlReaderSettings() { XmlResolver = null });
            var projDefinition = XDocument.Load(reader);

            // Find ItemGroups

            var nspace = "";
            var itemGroups = projDefinition.Element("Project")?.Elements("ItemGroup").ToArray();

            if (itemGroups == null || itemGroups.Length == 0)
            {
                // Try other version

                nspace = "{http://schemas.microsoft.com/developer/msbuild/2003}";
                itemGroups = projDefinition.Element(nspace + "Project").Elements(nspace + "ItemGroup").ToArray();
            }

            // Detect references

            references = itemGroups?
                       .Elements(nspace + "PackageReference")
                       .Select(u => u.Attribute("Include").Value + ".dll")
                       .ToList();

            if (references == null) references = new List<string>();

            // Detect hints

            var refs = itemGroups?
                .Elements(nspace + "Reference")?
                .Elements(nspace + "HintPath")?
                .Select(u => u.Value)
                .ToList();

            if (refs != null && refs.Count > 0)
            {
                references.AddRange(refs);
            }

            // Detect files

            files = projDefinition
               .Elements(nspace + "Compile")?
                .Select(u => u.Attribute("Update").Value)
                .ToList();

            if (files == null) files = new List<string>();
            reader.Dispose();

            var bin = Path.Combine(fileInfo.DirectoryName, "bin");
            var obj = Path.Combine(fileInfo.DirectoryName, "obj");

            files.AddRange(Directory.GetFiles(fileInfo.Directory.FullName, "*" + extension, SearchOption.AllDirectories));
            files = files.Where(u => !u.StartsWith(bin) && !u.StartsWith(obj)).Distinct().ToList();
        }

        /// <summary>
        /// Build script
        /// </summary>
        /// <param name="filenames">File names</param>
        /// <param name="references">References</param>
        /// <param name="releaseMode">Release mode (default=true)</param>
        /// <returns>Assembly</returns>
        public static Assembly CompileVBFiles(string[] filenames, string[] references, bool releaseMode = true)
        {
            var tree = filenames.Select(u => VisualBasicSyntaxTree.ParseText(
                            File.ReadAllText(u),
                            path: u,
                            encoding: Utility.StrictUTF8)).ToArray();
            var op = new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: releaseMode ? OptimizationLevel.Release : OptimizationLevel.Debug);
            return Assembly.Create(VisualBasicCompilation.Create("SmartContract", tree, CreateReferences(references), op));
        }

        /// <summary>
        /// Build script
        /// </summary>
        /// <param name="filenames">File names</param>
        /// <param name="references">References</param>
        /// <param name="releaseMode">Release mode (default=true)</param>
        /// <returns>Assembly</returns>
        public static Assembly CompileCSFiles(string[] filenames, string[] references, bool releaseMode = true)
        {
            var tree = filenames.Select(u => CSharpSyntaxTree.ParseText(
                            File.ReadAllText(u),
                            path: u,
                            encoding: Utility.StrictUTF8)).ToArray();
            var op = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: releaseMode ? OptimizationLevel.Release : OptimizationLevel.Debug);
            return Assembly.Create(CSharpCompilation.Create("SmartContract", tree, CreateReferences(references), op));
        }

        /// <summary>
        /// Create references
        /// </summary>
        /// <param name="references">References</param>
        /// <returns>MetadataReferences</returns>
        private static MetadataReference[] CreateReferences(params string[] references)
        {
            var coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var refs = new List<MetadataReference>(new MetadataReference[]
            {
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "netstandard.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.Numerics.dll")),
                MetadataReference.CreateFromFile(typeof(System.ComponentModel.DisplayNameAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(scfxSmartContract).Assembly.Location),
            });
            refs.AddRange(references.Where(u => u != "Neo.SmartContract.Framework.dll").Select(u => MetadataReference.CreateFromFile(u)));
            return refs.ToArray();
        }
    }
}
