using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;

namespace testcontract.builder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test ContractBuilder!");

            //set current path
            var pepath = typeof(Program).Assembly.Location;
            Console.WriteLine("pepos=" + pepath);
            pepath = System.IO.Path.GetDirectoryName(pepath);
            System.IO.Directory.SetCurrentDirectory(pepath);


            string srcproj = args[0];
            Console.WriteLine("sourceproj="+srcproj);

            string targetdir = System.IO.Path.GetFullPath(args[1]);
            if (System.IO.Directory.Exists(targetdir) == false)
                System.IO.Directory.CreateDirectory(targetdir);

            Console.WriteLine("outdir=" + targetdir);

            var srcdll = System.IO.Directory.GetFiles(System.IO.Path.GetFullPath(srcproj), "Neo.SmartContract.Framework.dll", System.IO.SearchOption.AllDirectories);
            if (srcdll.Length == 0)
            {
                Console.WriteLine("can not find framework.");
                return;
            }


            var frameworkfile = srcdll[0];
            var frameworkname = System.IO.Path.GetFileName(frameworkfile);


            var file = System.IO.Directory.GetFiles(System.IO.Path.GetFullPath(srcproj), "*.cs", System.IO.SearchOption.AllDirectories);
            var pskip = System.IO.Path.DirectorySeparatorChar + "Properties" + System.IO.Path.DirectorySeparatorChar;

            foreach (var f in file)
            {
                if (f.Contains(pskip)) continue;
                var r =BuildDllBySignleFile(f, targetdir, frameworkfile);
                if (r.Success)
                {
                    Console.WriteLine("build file succ=" + f);
                }
                else
                {
                    Console.WriteLine("build fail =" + f);
                }
            }

        }
        static Microsoft.CodeAnalysis.Emit.EmitResult BuildDllBySignleFile(string filename, string outpath, string frameworkfile)
        {
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
            var outdllname = System.IO.Path.Combine(outpath, file + ".dll");
            var outpdbname = System.IO.Path.Combine(outpath, file + ".pdb");
            var result = comp.Emit(outdllname, outpdbname);
            return result;
        }
    }
}
