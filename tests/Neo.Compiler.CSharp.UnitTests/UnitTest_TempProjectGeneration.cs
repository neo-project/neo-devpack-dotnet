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

        [TestMethod]
        public void BuildReferencesKeyIsStableAcrossOrdering()
        {
            var first = new CompilationSourceReferences
            {
                Packages =
                [
                    ("Neo.SmartContract.Framework", "3.9.0"),
                    ("Neo.Json", "3.9.0")
                ],
                Projects =
                [
                    Path.Combine(Path.GetTempPath(), "b", "ProjectB.csproj"),
                    Path.Combine(Path.GetTempPath(), "a", "ProjectA.csproj")
                ]
            };

            var second = new CompilationSourceReferences
            {
                Packages =
                [
                    ("Neo.Json", "3.9.0"),
                    ("Neo.SmartContract.Framework", "3.9.0")
                ],
                Projects =
                [
                    Path.Combine(Path.GetTempPath(), "a", "ProjectA.csproj"),
                    Path.Combine(Path.GetTempPath(), "b", "ProjectB.csproj")
                ]
            };

            var firstKey = InvokeBuildReferencesKey(first);
            var secondKey = InvokeBuildReferencesKey(second);

            Assert.AreEqual(firstKey, secondKey);
        }

        private static string InvokeBuildTempProjectContent(CompilationSourceReferences references, string[] sourceFiles)
        {
            var workspaceType = typeof(CompilationEngine).Assembly.GetType("Neo.Compiler.TemporaryProjectWorkspace")
                ?? throw new InvalidOperationException("Unable to locate TemporaryProjectWorkspace type via reflection.");

            var method = workspaceType.GetMethod("BuildTempProjectContent", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException("Unable to locate BuildTempProjectContent method via reflection.");

            return (string)method.Invoke(null, new object[] { references, sourceFiles })!;
        }

        private static string InvokeBuildReferencesKey(CompilationSourceReferences references)
        {
            var workspaceType = typeof(CompilationEngine).Assembly.GetType("Neo.Compiler.TemporaryProjectWorkspace")
                ?? throw new InvalidOperationException("Unable to locate TemporaryProjectWorkspace type via reflection.");

            var method = workspaceType.GetMethod("BuildReferencesKey", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException("Unable to locate BuildReferencesKey method via reflection.");

            return (string)method.Invoke(null, new object[] { references })!;
        }
    }
}
