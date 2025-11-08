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
            foreach (var block in mirFunction.Blocks)
            {
                writer.WriteLine($"BLOCK {block.Label}");
                foreach (var phi in block.Phis)
                    writer.WriteLine($"  PHI {phi.GetType().Name}");
                for (int i = 0; i < block.Instructions.Count; i++)
                {
                    var inst = block.Instructions[i];
                    writer.WriteLine($"  INST[{i}] {inst.GetType().Name} #{inst.GetHashCode():X}");
                    if (inst.ConsumesMemoryToken || inst.ProducesMemoryToken)
                    {
                        var tokenIn = inst.TokenInput is null
                            ? "<null>"
                            : $"{inst.TokenInput.GetType().Name} #{inst.TokenInput.GetHashCode():X}";
                        var tokenOut = inst.TokenOutput is null
                            ? "<null>"
                            : $"{inst.TokenOutput.GetType().Name} #{inst.TokenOutput.GetHashCode():X}";
                        writer.WriteLine($"    TOKEN_IN {tokenIn}");
                        writer.WriteLine($"    TOKEN_OUT {tokenOut}");
                    }
                    switch (inst)
                    {
                        case MirArrayNew arrayNew:
                            writer.WriteLine($"    LENGTH {arrayNew.Length.GetType().Name} #{arrayNew.Length.GetHashCode():X}");
                            break;
                        case MirArraySet arraySet:
                            writer.WriteLine($"    ARRAY {arraySet.Array.GetType().Name} #{arraySet.Array.GetHashCode():X}");
                            writer.WriteLine($"    INDEX {arraySet.Index.GetType().Name} #{arraySet.Index.GetHashCode():X}");
                            writer.WriteLine($"    VALUE {arraySet.Value.GetType().Name} #{arraySet.Value.GetHashCode():X}");
                            break;
                        case MirConvert convert:
                            writer.WriteLine($"    VALUE {convert.Value.GetType().Name} #{convert.Value.GetHashCode():X}");
                            break;
                        case MirSyscall syscall:
                            for (int argIndex = 0; argIndex < syscall.Arguments.Count; argIndex++)
                            {
                                var arg = syscall.Arguments[argIndex];
                                writer.WriteLine($"    ARG[{argIndex}] {arg.GetType().Name} #{arg.GetHashCode():X}");
                            }
                            break;
                    }
                }
                writer.WriteLine($"  TERM {block.Terminator?.GetType().Name ?? "<null>"}");
            }
            writer.WriteLine();
        }

        var mirOptimizer = new MirOptimizationPipeline();
        mirOptimizer.Run(mirFunction);

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

                foreach (var block in mirFunction.Blocks)
                {
                    writer.WriteLine($"BLOCK {block.Label}");
                    foreach (var phi in block.Phis)
                        writer.WriteLine($"  PHI {phi.GetType().Name}");
                    for (int i = 0; i < block.Instructions.Count; i++)
                    {
                        var inst = block.Instructions[i];
                        writer.WriteLine($"  INST[{i}] {inst.GetType().Name} #{inst.GetHashCode():X}");
                        if (inst.ConsumesMemoryToken || inst.ProducesMemoryToken)
                        {
                            var tokenIn = inst.TokenInput is null
                                ? "<null>"
                                : $"{inst.TokenInput.GetType().Name} #{inst.TokenInput.GetHashCode():X}";
                            var tokenOut = inst.TokenOutput is null
                                ? "<null>"
                                : $"{inst.TokenOutput.GetType().Name} #{inst.TokenOutput.GetHashCode():X}";
                            writer.WriteLine($"    TOKEN_IN {tokenIn}");
                            writer.WriteLine($"    TOKEN_OUT {tokenOut}");
                        }
                    }
                    writer.WriteLine($"  TERM {block.Terminator?.GetType().Name ?? "<null>"}");
                }
                writer.WriteLine();
            }

            foreach (var block in mirFunction.Blocks)
            {
                Console.WriteLine($"[MIR BLOCK] {block.Label}");
                foreach (var phi in block.Phis)
                    Console.WriteLine($"  [Phi] {phi.GetType().Name}");
                foreach (var inst in block.Instructions)
                    Console.WriteLine($"  [Inst] {inst.GetType().Name}");
                Console.WriteLine($"  [Terminator] {block.Terminator?.GetType().Name ?? "<null>"}");
            }

            var message = string.Join(Environment.NewLine, mirVerification);
            throw new InvalidOperationException($"MIR verification failed for '{hirFunction.Name}':{Environment.NewLine}{message}");
        }



        _mirToLir.Lower(mirFunction, lirModule);
    }
}
