using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Framework;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Neo.Compiler.MSIL.Utils
{
    internal static class NeonTestTool
    {
        public static string Bytes2HexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var d in data)
            {
                sb.Append(d.ToString("x02"));
            }
            return sb.ToString();
        }

        public static byte[] HexString2Bytes(string str)
        {
            if (str.IndexOf("0x") == 0)
                str = str.Substring(2);
            byte[] outd = new byte[str.Length / 2];
            for (var i = 0; i < str.Length / 2; i++)
            {
                outd[i] = byte.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return outd;
        }

        public static BuildScript BuildScript(string filename)
        {
            var coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var srccode = File.ReadAllText(filename);
            var tree = CSharpSyntaxTree.ParseText(srccode);
            var op = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release);
            var comp = CSharpCompilation.Create("TestContract", new[] { tree }, new[]
            {
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.Numerics.dll")),
                MetadataReference.CreateFromFile(typeof(System.ComponentModel.DisplayNameAttribute).Assembly.Location),

                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(OpCodeAttribute).Assembly.Location)
           }, op);
            using (var streamDll = new MemoryStream())
            using (var streamPdb = new MemoryStream())
            {
                var result = comp.Emit(streamDll, streamPdb);
                Assert.IsTrue(result.Success);
                streamDll.Position = 0;
                streamPdb.Position = 0;

                var bs = new BuildScript();
                bs.Build(streamDll, streamPdb);

                if (bs.Error != null) throw bs.Error;

                return bs;
            }
        }
    }
}
