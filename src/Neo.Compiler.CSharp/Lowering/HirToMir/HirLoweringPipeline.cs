using System;
using System.IO;
using Neo.Compiler.HIR;
using Neo.Compiler;
using Neo.Compiler.LIR;
using Neo.Compiler.MIR;
using Neo.Compiler.MIR.Optimization;
using Neo.Compiler.HIR.Optimization;

namespace Neo.Compiler.MiddleEnd.Lowering;

internal sealed class HirLoweringPipeline
{
    private readonly HirToMirLowerer _hirToMir = new();
    private readonly MirToLirLowerer _mirToLir = new();

    internal void Run(HirBuilder builder, CompilationContext context)
    {
        if (builder is null || context is null)
            return;

        var hirFunction = builder.Function;
        var mirModule = context.MirModule;
        var lirModule = context.LirModule;

        if (mirModule is null || lirModule is null)
            return;

        var hirOptimizer = new HirOptimizationPipeline();
        hirOptimizer.Run(hirFunction);

        var mirFunction = _hirToMir.Lower(hirFunction, mirModule);

        var beforePath = Path.Combine(Path.GetTempPath(), "mir_before.txt");
        using (var writer = new StreamWriter(beforePath, append: true))
        {
            writer.WriteLine($"=== MIR BEFORE OPT :: {hirFunction.Name} ===");
            WriteMirFunction(writer, mirFunction);
            writer.WriteLine();
        }

        var mirOptimizer = new MirOptimizationPipeline();
        mirOptimizer.Run(mirFunction);

        var loadMaterializer = new MirLocalLoadMaterializationPass();
        if (loadMaterializer.Run(mirFunction))
        {
            if (string.Equals(Environment.GetEnvironmentVariable("NEO_IR_DUMP_MIR_MATERIALIZED"), "1", StringComparison.OrdinalIgnoreCase))
            {
                var matPath = Path.Combine(Path.GetTempPath(), "mir_materialized.txt");
                using var dumpWriter = new StreamWriter(matPath, append: true);
                dumpWriter.WriteLine($"=== MIR AFTER MATERIALIZER :: {hirFunction.Name} ===");
                WriteMirFunction(dumpWriter, mirFunction);
                dumpWriter.WriteLine();
            }

            MirMemoryTokenRetargeter.Retokenize(mirFunction);
            MirPhiUtilities.DeduplicateTokenInputs(mirFunction);
            MirPhiUtilities.PruneNonPredecessorInputs(mirFunction);
        }

        var mirVerification = new MirVerifier().Verify(mirFunction);
        if (mirVerification.Count > 0)
        {
            Console.WriteLine($"[MIR VERIFY] Failures for function '{hirFunction.Name}':");
            foreach (var error in mirVerification)
                Console.WriteLine($"[MIR VERIFY]   {error}");

            var dumpPath = Path.Combine(Path.GetTempPath(), "mir_dump.txt");
            using (var writer = new StreamWriter(dumpPath, append: true))
            {
                writer.WriteLine($"=== MIR VERIFY FAIL :: {hirFunction.Name} ===");
                foreach (var error in mirVerification)
                    writer.WriteLine($"ERROR: {error}");
                WriteMirFunction(writer, mirFunction);
                writer.WriteLine();
            }

            var consoleWriter = TextWriter.Synchronized(Console.Out);
            WriteMirFunction(consoleWriter, mirFunction);

            var message = string.Join(Environment.NewLine, mirVerification);
            throw new InvalidOperationException($"MIR verification failed for '{hirFunction.Name}':{Environment.NewLine}{message}");
        }



        _mirToLir.Lower(mirFunction, lirModule);
    }

    private static void WriteMirFunction(TextWriter writer, MirFunction function)
    {
        foreach (var block in function.Blocks)
        {
            writer.WriteLine($"BLOCK {block.Label}");
            foreach (var phi in block.Phis)
            {
                writer.WriteLine($"  PHI {phi.GetType().Name} Type={phi.Type.GetType().Name} Inputs={phi.Inputs.Count}");
                foreach (var (pred, value) in phi.Inputs)
                    writer.WriteLine($"    IN {pred.Label}: {DescribeMirValue(value)}");
            }

            for (int i = 0; i < block.Instructions.Count; i++)
            {
                var inst = block.Instructions[i];
                writer.WriteLine($"  INST[{i}] {inst.GetType().Name} #{inst.GetHashCode():X}");

                if (inst.ConsumesMemoryToken || inst.ProducesMemoryToken)
                {
                    var tokenIn = inst.TokenInput is null ? "<null>" : DescribeMirValue(inst.TokenInput);
                    var tokenOut = inst.TokenOutput is null ? "<null>" : DescribeMirValue(inst.TokenOutput);
                    writer.WriteLine($"    TOKEN_IN {tokenIn}");
                    writer.WriteLine($"    TOKEN_OUT {tokenOut}");
                }

                switch (inst)
                {
                    case MirArrayNew arrayNew:
                        writer.WriteLine($"    LENGTH {DescribeMirValue(arrayNew.Length)}");
                        break;
                    case MirArraySet arraySet:
                        writer.WriteLine($"    ARRAY {DescribeMirValue(arraySet.Array)}");
                        writer.WriteLine($"    INDEX {DescribeMirValue(arraySet.Index)}");
                        writer.WriteLine($"    VALUE {DescribeMirValue(arraySet.Value)}");
                        break;
                    case MirConvert convert:
                        writer.WriteLine($"    VALUE {DescribeMirValue(convert.Value)}");
                        break;
                    case MirSyscall syscall:
                        for (int argIndex = 0; argIndex < syscall.Arguments.Count; argIndex++)
                            writer.WriteLine($"    ARG[{argIndex}] {DescribeMirValue(syscall.Arguments[argIndex])}");
                        break;
                    case MirLoadLocal loadLocal:
                        writer.WriteLine($"    SLOT {loadLocal.Slot}");
                        break;
                    case MirStoreLocal storeLocal:
                        writer.WriteLine($"    SLOT {storeLocal.Slot} VALUE {DescribeMirValue(storeLocal.Value)}");
                        break;
                    case MirCall call:
                        for (int argIndex = 0; argIndex < call.Arguments.Count; argIndex++)
                            writer.WriteLine($"    ARG[{argIndex}] {DescribeMirValue(call.Arguments[argIndex])}");
                        break;
                }
            }

            writer.WriteLine($"  TERM {block.Terminator?.GetType().Name ?? "<null>"}");
            switch (block.Terminator)
            {
                case MirReturn ret when ret.Value is { } value:
                    writer.WriteLine($"    VALUE {DescribeMirValue(value)}");
                    break;
                case MirCondBranch cond:
                    writer.WriteLine($"    COND {DescribeMirValue(cond.Condition)}");
                    writer.WriteLine($"    TRUE -> {cond.TrueTarget.Label}");
                    writer.WriteLine($"    FALSE -> {cond.FalseTarget.Label}");
                    break;
                case MirCompareBranch cmp:
                    writer.WriteLine($"    LEFT {DescribeMirValue(cmp.Left)} RIGHT {DescribeMirValue(cmp.Right)}");
                    writer.WriteLine($"    TRUE -> {cmp.TrueTarget.Label}");
                    writer.WriteLine($"    FALSE -> {cmp.FalseTarget.Label}");
                    break;
                case MirSwitch @switch:
                    writer.WriteLine($"    KEY {DescribeMirValue(@switch.Key)}");
                    foreach (var (value, target) in @switch.Cases)
                        writer.WriteLine($"    CASE {value} -> {target.Label}");
                    writer.WriteLine($"    DEFAULT -> {@switch.DefaultTarget.Label}");
                    break;
                case MirBranch branch:
                    writer.WriteLine($"    TARGET -> {branch.Target.Label}");
                    break;
                case MirLeave leave:
                    writer.WriteLine($"    TARGET -> {leave.Target.Label}");
                    break;
            }
        }
    }

    private static string DescribeMirValue(MirValue? value)
        => value is null ? "<null>" : $"{value.GetType().Name}#{value.GetHashCode():X}";
}
