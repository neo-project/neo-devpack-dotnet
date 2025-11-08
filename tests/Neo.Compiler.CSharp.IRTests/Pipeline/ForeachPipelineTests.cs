using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.TestInfrastructure;
using Neo.Compiler.MIR;
using Neo.Compiler.LIR;
using Neo.Compiler.LIR.Backend;

namespace Neo.Compiler.CSharp.IRTests.Pipeline;

/// <summary>
/// Incrementally exercises foreach lowering in the IR pipeline without relying on the full compiler UT harness.
/// </summary>
[TestClass]
public sealed class ForeachPipelineTests
{
    [TestMethod]
    public void IntForeach_Lowers_WithTokenPhi()
    {
        const string source = """
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_IntForeach : SmartContract.Framework.SmartContract
    {
        public static int IntForeach()
        {
            int[] values = new int[] { 1, 2, 3, 4 };
            var sum = 0;
            foreach (var item in values)
                sum += item;
            return sum;
        }
    }
}
""";

        CompileAndAssertTokenPhi(source, "IntForeach");
    }

    private static void CompileAndAssertTokenPhi(string source, string methodName)
    {
        var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
        File.WriteAllText(tempFile, source);

        try
        {
            var contexts = CompilationTestHelper.CompileSource(tempFile, options =>
            {
                options.EnableHir = true;
                options.Optimize = CompilationOptions.OptimizationType.None;
            });

            Assert.AreEqual(1, contexts.Count, "Expected a single compilation context.");
            var context = contexts[0];

            Assert.IsNotNull(context.MirModule, "MIR module should be materialised when IR is enabled.");
            Assert.IsNotNull(context.LirModule, "LIR module should be materialised when IR is enabled.");

            var functionKey = context.MirModule.Functions.Keys.Single(
                k => k.Contains(methodName, System.StringComparison.Ordinal));

            var mirFunction = context.MirModule.Functions[functionKey];
            var tokenPhiPresent = mirFunction.Blocks.Any(b => b.Phis.Any(phi => phi.Type is MirTokenType));
            Assert.IsTrue(tokenPhiPresent, $"Method '{methodName}' should build a token phi to merge loop memory tokens.");

            Assert.IsTrue(
                context.LirModule.TryGetCompilation(functionKey, out var compilation),
                $"LIR compilation for '{methodName}' was not produced.");

            var verification = new LirVerifier().Verify(compilation!.StackFunction);
            Assert.IsTrue(verification.Ok, $"LIR verification failed: {string.Join(System.Environment.NewLine, verification.Errors)}");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

}
