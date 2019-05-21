using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Neo.SmartContract.Framework;
using System.IO;
using System.Reflection;

namespace Neo.Compiler.MSIL.Utils
{
    class CSharpBuilder
    {
        public static EmitResult BuildDllBySignleFile(string filename, Stream streamDll, Stream streamPdb)
        {
            //set curpath
            var pepath = typeof(CSharpBuilder).Assembly.Location;
            pepath = Path.GetDirectoryName(pepath);
            Directory.SetCurrentDirectory(pepath);


            var srccode = File.ReadAllText(filename);
            var tree = CSharpSyntaxTree.ParseText(srccode);

            var op = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var coreDir = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
            var comp = CSharpCompilation.Create("aaa.dll", new[] { tree }, new[]
            {
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(OpCodeAttribute).Assembly.Location)
            }, op);

            var result = comp.Emit(streamDll, streamPdb);
            return result;
        }
    }
}
