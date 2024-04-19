using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using System.IO;

namespace Neo.Compiler.CSharp.UnitTests.OldEngine
{
    [TestClass]
    public class UnitTest_Parameters
    {
        readonly string csFileDir = $"{System.Environment.CurrentDirectory[..System.Environment.CurrentDirectory.IndexOf("Neo.Compiler.CSharp.UnitTests")]}Neo.Compiler.CSharp.TestContracts";

        [TestMethod]
        public void TestNoParameter()
        {
            Assert.AreEqual(Program.Main([]), 2);
        }

        [TestMethod]
        public void Test()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            Assert.AreEqual(Program.Main([path]), 0);
            Assert.IsTrue(File.Exists(Path.Combine(csFileDir, "bin", "sc", "Contract_BigInteger.nef")));
            Assert.IsTrue(File.Exists(Path.Combine(csFileDir, "bin", "sc", "Contract_BigInteger.manifest.json")));
        }

        [TestMethod]
        public void TestOutput()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            Assert.AreEqual(Program.Main([path, "-o", "output"]), 0);
        }

        [TestMethod]
        public void TestBaseName()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            Assert.AreEqual(Program.Main([path, "--base-name", "MyContract"]), 0);
        }

        [TestMethod]
        public void TestNotCSharpFile()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.txt");
            Assert.AreEqual(Program.Main([path]), 1);
        }

        [TestMethod]
        public void TestNotExist()
        {
            var path = Path.Combine(csFileDir, "Contract_NotExist.cs");
            Assert.AreEqual(Program.Main([path]), 1);
        }

        [TestMethod]
        public void TestMultiFile()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            var path2 = Path.Combine(csFileDir, "Contract_Math.cs");
            Assert.AreEqual(Program.Main([path, path2]), 0);
        }

        [TestMethod]
        public void TestGenerateArtifacts()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            Assert.AreEqual(Program.Main([path, "--generate-artifacts", "All"]), 0);
            Assert.IsTrue(File.Exists(Path.Combine(csFileDir, "bin", "sc", "Contract_BigInteger.artifacts.cs")));
        }

        [TestMethod]
        public void TestNullAble()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            Assert.AreEqual(Program.Main([path, "--nullable", "Enable"]), 0);
        }

        [TestMethod]
        public void TestDebug()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            Assert.AreEqual(Program.Main([path, "--debug"]), 0);
        }

        [TestMethod]
        public void TestAssembly()
        {
            var path = Path.Combine(csFileDir, "Contract_BigInteger.cs");
            Assert.AreEqual(Program.Main([path, "--assembly"]), 0);
        }
    }
}
