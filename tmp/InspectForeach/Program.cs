using System;
using System.IO;
using System.Linq;
using Neo.Compiler;
using Neo.Compiler.CSharp.UnitTests.TestInfrastructure;
using Neo.Compiler.MIR;
using Neo.Compiler.MIR.Optimization;

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

var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ".cs");
File.WriteAllText(tempFile, source);

try
{
    var contexts = CompilationTestHelper.CompileSource(tempFile, options =>
    {
        options.EnableHir = true;
        options.Optimize = CompilationOptions.OptimizationType.None;
    });

    Console.WriteLine($"Contexts: {contexts.Count}");
    var context = contexts.Single();
    if (context.MirModule is null)
    {
        Console.WriteLine("MIR module is null");
        return;
    }

    var functionKey = context.MirModule.Functions.Keys.Single(
        k => k.Contains("IntForeach", StringComparison.Ordinal));

    Console.WriteLine($"Function key: {functionKey}");

    var mirFunction = context.MirModule.Functions[functionKey];
    DumpMir(functionKey, mirFunction);

    var verifier = new MirVerifier();
    var errors = verifier.Verify(mirFunction);
    Console.WriteLine("Verifier errors before pipeline: " + (errors.Count == 0 ? "none" : string.Join(Environment.NewLine, errors)));

    var pipeline = new MirOptimizationPipeline();
    try
    {
        pipeline.Run(mirFunction);
        Console.WriteLine("Pipeline completed.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Pipeline threw: " + ex);
    }

    DumpMir("AfterPipeline", mirFunction);
    errors = verifier.Verify(mirFunction);
    Console.WriteLine("Verifier errors after pipeline: " + (errors.Count == 0 ? "none" : string.Join(Environment.NewLine, errors)));
}
finally
{
    if (File.Exists(tempFile))
        File.Delete(tempFile);
}

static void DumpMir(string name, MirFunction function)
{
    Console.WriteLine($"=== MIR DUMP :: {name} ===");
    foreach (var block in function.Blocks)
    {
        Console.WriteLine($"BLOCK {block.Label}");
        foreach (var phi in block.Phis)
        {
            Console.WriteLine($"  PHI {phi.GetType().Name} :: {phi.Type.GetType().Name}");
            foreach (var (pred, value) in phi.Inputs)
            {
                Console.WriteLine($"    IN {pred.Label} -> {Describe(value)}");
            }
        }

        for (int i = 0; i < block.Instructions.Count; i++)
        {
            var inst = block.Instructions[i];
            Console.WriteLine($"  INST[{i}] {inst.GetType().Name} :: {inst.Type.GetType().Name}");
            if (inst.ConsumesMemoryToken || inst.ProducesMemoryToken)
            {
                var tokenIn = inst.TokenInput is null ? "<null>" : Describe(inst.TokenInput);
                var tokenOut = inst.TokenOutput is null ? "<null>" : Describe(inst.TokenOutput);
                Console.WriteLine($"    TOKEN_IN {tokenIn}");
                Console.WriteLine($"    TOKEN_OUT {tokenOut}");
            }
        }

        Console.WriteLine($"  TERM {Describe(block.Terminator)}");
    }
}

static string Describe(MirValue? value)
{
    if (value is null)
        return "<null>";
    return value switch
    {
        MirConstInt constInt => $"ConstInt({constInt.Value})",
        MirConstBool constBool => $"ConstBool({constBool.Value})",
        MirConstNull => "ConstNull",
        MirMemoryTokenValue => "Token",
        MirPhi => "Phi",
        _ => value.GetType().Name
    };
}

static string Describe(MirTerminator? terminator)
{
    if (terminator is null)
        return "<null>";

    return terminator switch
    {
        MirBranch branch => $"Branch -> {branch.Target.Label}",
        MirCondBranch cond => $"CondBranch -> {cond.TrueTarget.Label}/{cond.FalseTarget.Label}",
        MirCompareBranch cmp => $"CompareBranch -> {cmp.TrueTarget.Label}/{cmp.FalseTarget.Label}",
        MirReturn ret => ret.Value is null ? "Return void" : $"Return {Describe(ret.Value)}",
        _ => terminator.GetType().Name
    };
}
