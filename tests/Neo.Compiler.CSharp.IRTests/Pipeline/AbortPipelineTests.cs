using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.TestInfrastructure;
using Neo.Compiler.HIR;
using Neo.Compiler.LIR;
using Neo.Compiler.MIR;

namespace Neo.Compiler.CSharp.IRTests.Pipeline;

[TestClass]
public sealed class AbortPipelineTests
{
    [TestMethod]
    public void AbortIntrinsics_LowerToAbortAndAbortMsg()
    {
        const string source = """
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class AbortIntrinsicContract : SmartContract.Framework.SmartContract
    {
        public static void AbortSimple()
        {
            ExecutionEngine.Abort();
        }

        public static void AbortWithMessage()
        {
            ExecutionEngine.Abort("boom");
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

            Assert.IsNotNull(context.HirModule, "HIR module expected.");
            Assert.IsNotNull(context.MirModule, "MIR module expected.");
            Assert.IsNotNull(context.LirModule, "LIR module expected.");

            var abortSimpleKey = context.MirModule!.Functions.Keys.Single(k => k.Contains("AbortSimple", StringComparison.Ordinal));
            var abortMsgKey = context.MirModule.Functions.Keys.Single(k => k.Contains("AbortWithMessage", StringComparison.Ordinal));

            var abortSimpleMir = context.MirModule.Functions[abortSimpleKey];
            Assert.IsTrue(abortSimpleMir.Blocks.Any(b => b.Terminator is MirAbort), "AbortSimple should lower to MirAbort terminator.");

            var abortMsgMir = context.MirModule.Functions[abortMsgKey];
            Assert.IsTrue(abortMsgMir.Blocks.Any(b => b.Terminator is MirAbortMsg), "AbortWithMessage should lower to MirAbortMsg terminator.");

            Assert.IsTrue(context.LirModule!.TryGetCompilation(abortSimpleKey, out var abortSimpleLir));
            var simpleOpcodes = abortSimpleLir.StackFunction.Blocks.SelectMany(b => b.Instructions).Select(i => i.Op).ToArray();
            CollectionAssert.Contains(simpleOpcodes, LirOpcode.ABORT, "AbortSimple should emit ABORT opcode.");

            Assert.IsTrue(context.LirModule.TryGetCompilation(abortMsgKey, out var abortMsgLir));
            var msgOpcodes = abortMsgLir.StackFunction.Blocks.SelectMany(b => b.Instructions).Select(i => i.Op).ToArray();
            CollectionAssert.Contains(msgOpcodes, LirOpcode.ABORTMSG, "AbortWithMessage should emit ABORTMSG opcode.");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [TestMethod]
    public void AbortInFunction_MirToLir_SchedulesCallBlocks()
    {
        const string source = """
using Neo.SmartContract.Framework;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class AbortInFunctionPipelineContract : SmartContract.Framework.SmartContract
    {
        public static int TestAbortInFunction(bool flag)
        {
            int value = 0;
            try
            {
                if (flag)
                    value = 1;
                else
                    value = 2;
            }
            catch
            {
                value = -1;
            }
            finally
            {
                value++;
            }

            return value;
        }
    }
}
""";

        var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
        File.WriteAllText(tempFile, source);

        try
        {
            var options = new CompilationOptions
            {
                EnableHir = true,
                Optimize = CompilationOptions.OptimizationType.All
            };
            var engine = new CompilationEngine(options);
            var references = CompilationTestHelper.GetReferenceSet().Select(path => MetadataReference.CreateFromFile(path));
            IReadOnlyList<CompilationContext> contexts;
            try
            {
                contexts = engine.Compile(new[] { tempFile }, references);
            }
            catch (AggregateException)
            {
                foreach (var entry in engine.Contexts)
                {
                    var ctx = entry.Value;
                    Console.WriteLine($"Context: {entry.Key.Name}");
                    if (ctx.HirModule is not null)
                    {
                        Console.WriteLine("  HIR keys:");
                        foreach (var key in ctx.HirModule.Functions.Keys)
                        {
                            Console.WriteLine($"    {key}");
                            if (ctx.HirModule.Functions.TryGetValue(key, out var hir))
                                DumpHirFunction(hir);
                        }
                    }

                }

                throw;
            }

            var context = contexts.Single();
            var methodKey = context.MirModule!.Functions.Keys.Single(k => k.Contains("TestAbortInFunction", StringComparison.Ordinal));
            if (context.HirModule is not null && context.HirModule.Functions.TryGetValue(methodKey, out var hirFunction))
            {
                DumpHirFunction(hirFunction);
            }
            else if (context.HirModule is not null)
            {
                Console.WriteLine("Available HIR keys:");
                foreach (var key in context.HirModule.Functions.Keys)
                    Console.WriteLine($"  {key}");
            }
            Assert.IsTrue(context.LirModule!.TryGetCompilation(methodKey, out var compilation), "LIR compilation missing for TestAbortInFunction.");

            var instructions = compilation.StackFunction.Blocks.SelectMany(b => b.Instructions).ToArray();
            Assert.IsTrue(instructions.Any(i => i.Op == LirOpcode.TRY_L && i.TargetLabel is not null), "TRY_L instruction with catch target expected.");
            Assert.IsTrue(instructions.Any(i => i.Op == LirOpcode.ENDTRY_L), "ENDTRY_L instruction expected.");
            Assert.IsTrue(instructions.Any(i => i.Op == LirOpcode.ENDFINALLY), "ENDFINALLY instruction expected.");

            var catchBlock = compilation.StackFunction.Blocks.Single(b => b.Label.Contains("try_catch", StringComparison.Ordinal));
            Assert.IsFalse(catchBlock.Instructions.Any(i => i.Op == LirOpcode.DROP), "Catch block should not emit DROP instructions before ENDTRY.");
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    private static void DumpHirFunction(HirFunction function)
    {
        Console.WriteLine($"HIR dump for {function.Name}");
        foreach (var block in function.Blocks)
        {
            Console.WriteLine($"Block: {block.Label}");
            foreach (var phi in block.Phis)
            {
                Console.WriteLine($"  Phi ({phi.Type}):");
                foreach (var incoming in phi.Inputs)
                {
                    Console.WriteLine($"    from {incoming.Block.Label}: {incoming.Value.GetType().Name}");
                }
            }

            foreach (var inst in block.Instructions)
            {
                Console.WriteLine($"  Inst: {inst.GetType().Name}");
            }

            if (block.Terminator is not null)
                Console.WriteLine($"  Terminator: {block.Terminator.GetType().Name}");
        }
    }
}
