using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.TestEngine;
using System.IO;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Parameters
    {
        [TestMethod]
        public void TestNoParameter()
        {
            var result = Program.Main([]);
            Assert.AreEqual(result, 2);
        }

        [TestMethod]
        public void Test()
        {
            var currentDir = System.Environment.CurrentDirectory;
            var file = "Contract_BigInteger.cs";
            var path = Path.Combine($"{currentDir[..currentDir.IndexOf("Neo.Compiler.CSharp.UnitTests")]}Neo.Compiler.CSharp.TestContracts", file);
            var result = Program.Main([path]);
            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void TestOutput()
        {
            var currentDir = System.Environment.CurrentDirectory;
            var file = "Contract_BigInteger.cs";
            var path = Path.Combine($"{currentDir[..currentDir.IndexOf("Neo.Compiler.CSharp.UnitTests")]}Neo.Compiler.CSharp.TestContracts", file);
            var result = Program.Main([path, "-o", "output"]);
            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void TestBaseName()
        {
            var currentDir = System.Environment.CurrentDirectory;
            var file = "Contract_BigInteger.cs";
            var path = Path.Combine($"{currentDir[..currentDir.IndexOf("Neo.Compiler.CSharp.UnitTests")]}Neo.Compiler.CSharp.TestContracts", file);
            var result = Program.Main([path, "--base-name", "MyContract"]);
            Assert.AreEqual(result, 0);
        }

        [TestMethod]
        public void TestNotCSharpFile()
        {
            var currentDir = System.Environment.CurrentDirectory;
            var file = "Contract_BigInteger.txt";
            var path = Path.Combine($"{currentDir[..currentDir.IndexOf("Neo.Compiler.CSharp.UnitTests")]}Neo.Compiler.CSharp.TestContracts", file);
            var result = Program.Main([path]);
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void TestNotExist()
        {
            var currentDir = System.Environment.CurrentDirectory;
            var file = "Contract_NotExist.cs";
            var path = Path.Combine($"{currentDir[..currentDir.IndexOf("Neo.Compiler.CSharp.UnitTests")]}Neo.Compiler.CSharp.TestContracts", file);
            var result = Program.Main([path]);
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void TestMultiFile()
        {
            var currentDir = System.Environment.CurrentDirectory;
            var file = "Contract_BigInteger.cs";
            var file2 = "Contract_Math.cs";
            var path = Path.Combine($"{currentDir[..currentDir.IndexOf("Neo.Compiler.CSharp.UnitTests")]}Neo.Compiler.CSharp.TestContracts", file);
            var path2 = Path.Combine($"{currentDir[..currentDir.IndexOf("Neo.Compiler.CSharp.UnitTests")]}Neo.Compiler.CSharp.TestContracts", file2);
            var result = Program.Main([path, path2]);
            Assert.AreEqual(result, 0);
        }
    }
}
