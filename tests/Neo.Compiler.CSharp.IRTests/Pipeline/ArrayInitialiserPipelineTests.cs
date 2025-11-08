using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.TestInfrastructure;
using Neo.Compiler.MIR;

namespace Neo.Compiler.CSharp.IRTests.Pipeline;

[TestClass]
public sealed class ArrayInitialiserPipelineTests
{
    [TestMethod]
    public void DefaultArrayInitializer_CompletesPipeline()
    {
        const string source = """
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class ArrayInitContract : SmartContract.Framework.SmartContract
    {
        public static bool TestDefaultArray()
        {
            var arrobj = new int[3];
            if (arrobj[0] == 0) return true;
            return false;
        }
    }
}
""";

        var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
        File.WriteAllText(tempFile, source);

        try
        {
            var contexts = CompilationTestHelper.CompileSource(tempFile, options =>
            {
                options.EnableHir = true;
                options.Optimize = CompilationOptions.OptimizationType.All;
            });

            Assert.AreEqual(1, contexts.Count, "Expected a single compilation context.");
            var context = contexts[0];
            Assert.IsNotNull(context.MirModule, "MIR module should exist when IR pipeline is enabled.");

            var methodKey = context.MirModule.Functions.Keys.Single(k => k.Contains("TestDefaultArray", StringComparison.Ordinal));
            var mirFunction = context.MirModule.Functions[methodKey];

            var verification = new MirVerifier().Verify(mirFunction);
            Assert.AreEqual(0, verification.Count, $"MIR verification failed: {string.Join(Environment.NewLine, verification)}");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
