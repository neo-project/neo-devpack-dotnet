using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.SmartContract.Testing;
using System.Collections.Generic;
using System.IO;

namespace Neo.Compiler.CSharp.UnitTests.Peripheral
{
    [TestClass]
    public class UnitTest_Debug : DebugAndTestBase<Contract_Debug>
    {
        [TestMethod]
        public void TestDebug()
        {
            // Debug

            var msgs = new List<string?>();
            Contract.OnDebug += msgs.Add;

            Assert.AreEqual(1, Contract.TestElse());
            Assert.AreEqual(1, msgs.Count);
            Assert.AreEqual("Debug compilation", msgs[0]);
            Assert.AreEqual(1, Contract.TestIf());

            // Compile without debug

            var testContractsPath = new FileInfo("../../../../Neo.Compiler.CSharp.TestContracts/Contract_Debug.cs").FullName;
            var results = new CompilationEngine(new CompilationOptions()
            {
                Debug = CompilationOptions.DebugType.None,
                CompilerVersion = "TestingEngine",
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = Microsoft.CodeAnalysis.NullableContextOptions.Enable
            })
            .CompileSources(testContractsPath);

            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0].Success);

            var nef = results[0].CreateExecutable();
            var manifest = results[0].CreateManifest();
            var noDebug = Engine.Deploy<Contract_Debug>(nef, manifest);

            msgs.Clear();
            noDebug.OnDebug += msgs.Add;

            Assert.AreEqual(2, noDebug.TestElse());
            Assert.AreEqual(0, msgs.Count);
            Assert.AreEqual(2, noDebug.TestIf());
        }
    }
}
