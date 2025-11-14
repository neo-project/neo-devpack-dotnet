using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using System;
using System.IO;
using System.Reflection;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_TempProjectGeneration
    {
        [TestMethod]
        public void TempProjectUsesCompilerTargetFrameworkAndPreview()
        {
            string tempSource = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".cs");
            File.WriteAllText(tempSource, "public class TempContract { }");

            try
            {
                var content = InvokeBuildTempProjectContent(new CompilationSourceReferences(), new[] { tempSource });
                var expectedTfm = RuntimeAssemblyResolver.CompilerTargetFrameworkMoniker;

                StringAssert.Contains(content, $"<TargetFramework>{expectedTfm}</TargetFramework>");
                StringAssert.Contains(content, "<LangVersion>preview</LangVersion>");
            }
            finally
            {
                if (File.Exists(tempSource))
                {
                    File.Delete(tempSource);
                }
            }
        }

        private static string InvokeBuildTempProjectContent(CompilationSourceReferences references, string[] sourceFiles)
        {
            var method = typeof(CompilationEngine).GetMethod("BuildTempProjectContent", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException("Unable to locate BuildTempProjectContent method via reflection.");

            return (string)method.Invoke(null, new object[] { references, sourceFiles })!;
        }
    }
}
