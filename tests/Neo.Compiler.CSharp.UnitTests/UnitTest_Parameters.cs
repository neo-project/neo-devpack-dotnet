using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using System.IO;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Parameters
    {
        [TestMethod]
        public void Test()
        {
            var currentDir = System.Environment.CurrentDirectory;
            var file = "Contract_BigInteger.cs";
            var path = $"{currentDir[..currentDir.IndexOf("Neo.Compiler.CSharp.UnitTests")]}Neo.Compiler.CSharp.TestContracts\\{file}";
            var outputDir = "output";

            File.Delete($"{currentDir}\\{outputDir}\\{file.Replace(".cs", ".nef")}");
            File.Delete($"{currentDir}\\{outputDir}\\{file.Replace(".cs", ".manifest.json")}");

            var result = Program.Main([path, "-o", outputDir]);

            Assert.AreEqual(result, 0);
            Assert.IsTrue(File.Exists($"{currentDir}\\{outputDir}\\{file.Replace(".cs", ".nef")}"));
            Assert.IsTrue(File.Exists($"{currentDir}\\{outputDir}\\{file.Replace(".cs", ".manifest.json")}"));
        }
    }
}
