using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.MSIL.Utils
{
    class CSharpBuilder
    {
        public static Microsoft.CodeAnalysis.Emit.EmitResult BuildDllBySignleFile(string filename, string frameworkfile,System.IO.Stream streamDll,System.IO.Stream streamPdb )
        {
            //set curpath
            var pepath = typeof(CSharpBuilder).Assembly.Location;
            pepath = System.IO.Path.GetDirectoryName(pepath);
            System.IO.Directory.SetCurrentDirectory(pepath);


            var srccode = System.IO.File.ReadAllText(filename);
            var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(srccode);

            var op = new CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary);
            var ref1 = MetadataReference.CreateFromFile("needlib" + System.IO.Path.DirectorySeparatorChar + "mscorlib.dll");
            var ref2 = MetadataReference.CreateFromFile("needlib" + System.IO.Path.DirectorySeparatorChar + "System.dll");
            var ref3 = MetadataReference.CreateFromFile("needlib" + System.IO.Path.DirectorySeparatorChar + "System.Numerics.dll");
            var ref4 = MetadataReference.CreateFromFile(frameworkfile);
            var comp = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create("aaa.dll", new[] { tree },
               new[] { ref1, ref2, ref3, ref4 }, op);

            var file = System.IO.Path.GetFileNameWithoutExtension(filename);
            var result = comp.Emit(streamDll, streamPdb);
            return result;
        }
    }
}
