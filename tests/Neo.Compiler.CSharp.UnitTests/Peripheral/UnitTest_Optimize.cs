using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests.Peripheral
{
    [TestClass]
    public class UnitTest_Optimize : DebugAndTestBase<Contract_Optimize>
    {
        [TestMethod]
        public void Test_Optimize()
        {
            // Compile without optimizations

            var testContractsPath = new FileInfo("../../../../Neo.Compiler.CSharp.TestContracts/Contract_Optimize.cs").FullName;
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

            // deploy non optimized

            var nef = results[0].CreateExecutable();
            var manifest = results[0].CreateManifest();
            Assert.AreNotEqual(Contract_Optimize.Manifest.ToJson(), manifest.ToJson());

            var contract = Engine.Deploy<Contract_Optimize>(nef, manifest);

            var result = Contract.UnitTest_001();
            var result2 = contract.UnitTest_001();

            Assert.IsTrue(result?.SequenceEqual(result2!));
        }
    }
}
