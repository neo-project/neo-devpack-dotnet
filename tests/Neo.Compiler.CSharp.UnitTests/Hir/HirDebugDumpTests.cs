using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler.CSharp.UnitTests.Hir;
using Neo.Compiler.MIR;
using Neo.Compiler.MIR.Optimization;
using Neo.Compiler.MiddleEnd.Lowering;

namespace Neo.Compiler.CSharp.UnitTests.HirDebug;

[TestClass]
public sealed class HirDebugDumpTests
{
    [TestMethod]
    [Ignore("Debug dump disabled in automated environment.")]
    public void Dump_TestDefaultArray_Optimisation()
    {
        const string source = """
using System.Numerics;
using Neo.SmartContract.Framework;

public class Contract
{
    public static bool TestDefaultArray()
    {
        var arrobj = new int[3];
        if (arrobj[0] == 0) return true;
        return false;
    }

    [TestMethod]
    public void Dump_TestDynamicArrayInit()
    {
        var contexts = Neo.Compiler.CSharp.UnitTests.TestInfrastructure.CompilationTestHelper.CompileSource(
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Neo.Compiler.CSharp.TestContracts", "Contract_Array.cs")),
            options => options.Optimize = CompilationOptions.OptimizationType.None);

        var context = contexts.Single();
        var hirFunction = context.HirModule!.Functions.First(kvp => kvp.Key.EndsWith("Contract_Array::TestDynamicArrayInit")).Value;
        Console.WriteLine("=== HIR TestDynamicArrayInit ===");
        foreach (var block in hirFunction.Blocks)
        {
            Console.WriteLine($"BLOCK {block.Label}");
            foreach (var inst in block.Instructions)
                Console.WriteLine($"  INST {inst.GetType().Name}");
            Console.WriteLine($"  TERM {block.Terminator?.GetType().Name ?? "<null>"}");
        }
    }
}
""";

        var function = HirLoweringTestBase.LowerMethod(source);
        var module = new MirModule();
        var lowerer = new HirToMirLowerer();
        var mirFunction = lowerer.Lower(function, module);

        DumpMir("initial", mirFunction);

        var passes = new IMirPass[]
        {
            new MirConstantFolderPass(),
            new MirSparseConditionalConstantPropagationPass(),
            new MirPhiSimplificationPass(),
            new MirCompareBranchLoweringPass(),
            new MirGlobalValueNumberingPass(),
            new MirScalarReplacementPass(),
            new MirContainerVersioningPass(),
            new MirGuardOptimizationPass(),
            new MirLoopOptimisationPass(),
            new MirDeadCodeEliminationPass(),
            new MirBranchSimplificationPass(),
            new MirUnreachableBlockEliminationPass(),
            new MirAbiTighteningPass(),
            new MirCostAwarenessPass()
        };

        bool changedAny;
        do
        {
            changedAny = false;
            foreach (var pass in passes)
            {
                var changed = pass.Run(mirFunction);
                if (!changed)
                    continue;

                changedAny = true;
                System.Console.WriteLine($"PASS {pass.GetType().Name} altered function");
                DumpMir(pass.GetType().Name, mirFunction);
            }
        } while (changedAny);
    }

    private static void DumpMir(string label, MirFunction function)
    {
        System.Console.WriteLine($"=== {label} ===");
        foreach (var block in function.Blocks)
        {
            System.Console.WriteLine($"BLOCK {block.Label}");
            foreach (var inst in block.Instructions)
                System.Console.WriteLine($"  INST {inst.GetType().Name}");
            var termName = block.Terminator?.GetType().Name ?? "<null>";
            System.Console.WriteLine($"  TERM {termName}");
        }
    }

    [TestMethod]
    [Ignore("Debug dump disabled in automated environment.")]
    public void Dump_ContractArray_TestDefaultArray()
    {
        var sourcePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Neo.Compiler.CSharp.TestContracts", "Contract_Array.cs"));
        var contexts = Neo.Compiler.CSharp.UnitTests.TestInfrastructure.CompilationTestHelper.CompileSource(
            sourcePath,
            options => options.Optimize = CompilationOptions.OptimizationType.None);
        var context = contexts.Single();
        Assert.IsNotNull(context.MirModule, "MIR module missing");
        var mirModule = context.MirModule!;
        foreach (var kvp in mirModule.Functions)
        {
            var name = kvp.Key;
            var function = kvp.Value;
            if (!name.Contains("TestDefaultArray", StringComparison.Ordinal))
                continue;

            System.Console.WriteLine($"Function: {name}");
            System.Console.WriteLine("HIR:");
            if (context.HirModule is not null && context.HirModule.Functions.TryGetValue(name, out var hirFunction))
            {
                foreach (var block in hirFunction.Blocks)
                {
                    System.Console.WriteLine($"  BLOCK {block.Label}");
                    foreach (var inst in block.Instructions)
                        System.Console.WriteLine($"    INST {inst.GetType().Name}");
                    System.Console.WriteLine($"    TERM {block.Terminator?.GetType().Name ?? "<null>"}");
                }
            }

            DumpMir("pre-opt", function);

            var pipeline = new MirOptimizationPipeline();
            try
            {
                pipeline.Run(function);
                DumpMir("post-opt", function);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Pipeline failed: {ex.Message}");
                DumpMir("failed", function);
            }
        }
    }
}
