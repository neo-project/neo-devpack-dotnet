using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using Neo.SmartContract.Testing.TestingStandards;
using System.IO;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_Contract1 : TestBase<Contract1>
    {
        [TestMethod]
        public void Test_PrivateMethod()
        {
            // Optimizer will remove this method
            Assert.IsFalse(Encoding.ASCII.GetString(Contract1.Nef.Script.Span).Contains("NEO3"));

            // Compile without optimizations

            var testContractsPath = new FileInfo("../../../../Neo.Compiler.CSharp.TestContracts/Contract1.cs").FullName;
            var results = new CompilationEngine(new CompilationOptions()
            {
                Debug = true,
                CompilerVersion = "TestingEngine",
                Optimize = CompilationOptions.OptimizationType.None,
                Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable
            })
            .CompileSources(testContractsPath);

            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0].Success);

            var nef = results[0].CreateExecutable();
            Assert.IsTrue(Encoding.ASCII.GetString(nef.Script.Span).Contains("NEO3"));
        }

        [TestMethod]
        public void Test_ByteArray_New()
        {
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, Contract.UnitTest_001());
            Assert.AreEqual(1232070, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_testArgs1()
        {
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4 }, Contract.TestArgs1(4));
            Assert.AreEqual(1539180, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_testArgs2()
        {
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, Contract.TestArgs2([1, 2, 3]));
            Assert.AreEqual(1047240, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_testArgs3()
        {
            // No errors
            Assert.AreEqual(3, Contract.TestArgs3(1, 2));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);

            Assert.AreEqual(3, Contract.TestArgs3(BigInteger.One, BigInteger.Zero));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);

            Assert.AreEqual(1, Contract.TestArgs3(BigInteger.MinusOne, BigInteger.MinusOne));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);

            Assert.AreEqual(-2147483647, Contract.TestArgs3(int.MaxValue, int.MaxValue));
            Assert.AreEqual(1048560, Engine.FeeConsumed.Value);

            Assert.AreEqual(-2147483646, Contract.TestArgs3(int.MinValue, int.MaxValue));
            Assert.AreEqual(1047870, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_testArgs4()
        {
            Assert.AreEqual(5, Contract.TestArgs4(1, 2));
            Assert.AreEqual(1048470, Engine.FeeConsumed.Value);

            Assert.AreEqual(3, Contract.TestArgs4(BigInteger.One, BigInteger.Zero));
            Assert.AreEqual(1048470, Engine.FeeConsumed.Value);


            Assert.AreEqual(0, Contract.TestArgs4(BigInteger.MinusOne, BigInteger.MinusOne));
            Assert.AreEqual(1048470, Engine.FeeConsumed.Value);


            Assert.AreEqual(0, Contract.TestArgs4(int.MaxValue, int.MaxValue));
            Assert.AreEqual(1049160, Engine.FeeConsumed.Value);

            Assert.AreEqual(1, Contract.TestArgs4(int.MinValue, int.MaxValue));
            Assert.AreEqual(1048470, Engine.FeeConsumed.Value);
        }

        [TestMethod]
        public void Test_testVoid()
        {
            // No errors
            Contract.TestVoid();
            Assert.AreEqual(1232010, Engine.FeeConsumed.Value);
        }
    }
}
