using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Neo.Compiler.CSharp.UnitTests.Peripheral
{
    [TestClass]
    public class UnitTest_Parameters
    {
        readonly string csFileDir = Utils.Extensions.TestContractRoot;

        [TestMethod]
        public void TestNoParameter()
        {
            Assert.AreEqual(Program.Main([]), 2);
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
