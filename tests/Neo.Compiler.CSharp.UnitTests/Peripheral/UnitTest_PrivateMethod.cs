using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.IO;
using System.Numerics;
using System.Text;

namespace Neo.Compiler.CSharp.UnitTests.Peripheral
{
    [TestClass]
    public class UnitTest_PrivateMethod : DebugAndTestBase<Contract1>
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
                Debug = CompilationOptions.DebugType.Extended,
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
    }
}
