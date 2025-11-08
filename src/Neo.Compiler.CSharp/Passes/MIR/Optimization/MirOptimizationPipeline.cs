using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.MIR.Optimization;

internal sealed class MirOptimizationPipeline
{
    private const int MaxIterations = 32;
    private static readonly bool s_tracePipeline =
        string.Equals(Environment.GetEnvironmentVariable("NEO_IR_TRACE"), "1", StringComparison.OrdinalIgnoreCase);
    private static readonly bool s_traceLocalLoads =
        string.Equals(Environment.GetEnvironmentVariable("NEO_IR_TRACE_LOCALS"), "1", StringComparison.OrdinalIgnoreCase);

    private static void Trace(string message)
    {
        if (s_tracePipeline)
            Console.WriteLine($"[MIR OPT] {message}");
    }

    private static readonly IMirPass[] s_passes =
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

    public void Run(MirFunction function)
    {
        if (function is null)
            throw new ArgumentNullException(nameof(function));

        var hasExceptionalControl = function.Blocks.Any(
            block => block.Instructions.Any(inst => inst is MirTry or MirCatch or MirFinally));

        if (hasExceptionalControl)
        {
            // Exceptional control flow is still under construction; skip optimisation passes that
            // are not yet aware of try/catch/finally so the verifier can run on the original graph.
            return;
        }

        var changedAny = false;
        bool changedIteration;
        var iteration = 0;
        do
        {
            iteration++;
            Trace($"Function '{function.Name}': iteration {iteration} start");
            changedIteration = false;
            foreach (var pass in s_passes)
            {
                var passName = pass.GetType().Name;
                var sw = s_tracePipeline ? System.Diagnostics.Stopwatch.StartNew() : null;
                Trace($"Function '{function.Name}': starting {passName}");
                bool changed;
                try
                {
                    changed = pass.Run(function);
                }
                catch (KeyNotFoundException ex)
                {
                    Console.WriteLine($"[MIR OPT] {function.Name}: {pass.GetType().Name} threw KeyNotFoundException\n{ex}\n{Environment.StackTrace}");
                    throw;
                }
                if (sw is not null)
                {
                    sw.Stop();
                    Trace($"Function '{function.Name}': finished {passName} changed={changed} elapsed={sw.Elapsed}");
                }
                CheckForOrphanConstants(function, pass.GetType().Name);
                MirMemoryTokenRetargeter.Retokenize(function);
                Trace($"Function '{function.Name}': retokenized after {passName}");
                MirPhiUtilities.PruneNonPredecessorInputs(function);
                var verifyErrors = new MirVerifier().Verify(function);
                if (verifyErrors.Count > 0)
                {
                    Console.WriteLine($"[MIR OPT] Verification failed after {pass.GetType().Name}.");
                    foreach (var error in verifyErrors)
                        Console.WriteLine($"[MIR OPT]   {error}");

                    var dumpPath = Path.Combine(Path.GetTempPath(), "mir_dump.txt");
                    using var writer = new StreamWriter(dumpPath, append: true);
                    writer.WriteLine($"=== MIR OPT VERIFY FAIL :: {function.Name} (after {pass.GetType().Name}) ===");
                    foreach (var error in verifyErrors)
                        writer.WriteLine($"ERROR: {error}");

                    DumpFunctionState(function, writer);
                    writer.WriteLine();

                    throw new InvalidOperationException($"MIR verification failed after optimisation pass {pass.GetType().Name}:{Environment.NewLine}{string.Join(Environment.NewLine, verifyErrors)}");
                }
                if (changed)
                {
                    changedIteration = true;
                    changedAny = true;
                }
                if (s_traceLocalLoads)
                    TraceLocalInstructions(function, passName);
            }
            Trace($"Function '{function.Name}': iteration {iteration} {(changedIteration ? "changed" : "stable")}");
            if (iteration >= MaxIterations && changedIteration)
                throw new InvalidOperationException($"MIR optimisation did not converge for '{function.Name}' after {MaxIterations} iterations. Enable NEO_IR_TRACE=1 to capture pass trace.");
        } while (changedIteration);

        if (changedAny)
        {
            MirMemoryTokenRetargeter.Retokenize(function);
            MirPhiUtilities.PruneNonPredecessorInputs(function);
            var verifier = new MirVerifier();
            IReadOnlyList<string> errors;
            try
            {
                errors = verifier.Verify(function);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"[MIR OPT] {function.Name}: MirVerifier threw KeyNotFoundException\n{ex}\n{Environment.StackTrace}");
                throw;
            }
            if (errors.Count > 0)
            {
                Console.WriteLine("[MIR OPT] Verification failed after optimisation.");
                foreach (var error in errors)
                    Console.WriteLine($"[MIR OPT]   {error}");

                var dumpPath = Path.Combine(Path.GetTempPath(), "mir_dump.txt");
                using (var writer = new StreamWriter(dumpPath, append: true))
                {
                    writer.WriteLine($"=== MIR OPT VERIFY FAIL :: {function.Name} ===");
                    foreach (var error in errors)
                        writer.WriteLine($"ERROR: {error}");
                    DumpFunctionState(function, writer);
                    writer.WriteLine();
                }

                throw new InvalidOperationException($"MIR verification failed after optimisation:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
            }
        }
    }

    private static void DumpFunctionState(MirFunction function, TextWriter writer)
    {
        foreach (var block in function.Blocks)
        {
            writer.WriteLine($"BLOCK {block.Label}");

            foreach (var phi in block.Phis)
            {
                writer.WriteLine($"  PHI {phi.GetType().Name}");
                foreach (var (pred, value) in phi.Inputs)
                {
                    var valueDescriptor = value is null
                        ? "<null>"
                        : $"{value.GetType().Name} #{value.GetHashCode():X}";
                    writer.WriteLine($"    IN {pred.Label} -> {valueDescriptor}");
                }
            }

            for (int i = 0; i < block.Instructions.Count; i++)
            {
                var inst = block.Instructions[i];
                writer.WriteLine($"  INST[{i}] {inst.GetType().Name} #{inst.GetHashCode():X}");
                switch (inst)
                {
                    case MirArraySet arraySet:
                        writer.WriteLine($"    ARRAY {arraySet.Array.GetType().Name} #{arraySet.Array.GetHashCode():X}");
                        writer.WriteLine($"    INDEX {arraySet.Index.GetType().Name} #{arraySet.Index.GetHashCode():X}");
                        writer.WriteLine($"    VALUE {arraySet.Value.GetType().Name} #{arraySet.Value.GetHashCode():X}");
                        break;
                    case MirConvert convert:
                        writer.WriteLine($"    VALUE {convert.Value.GetType().Name} #{convert.Value.GetHashCode():X}");
                        break;
                    case MirArrayNew arrayNew:
                        writer.WriteLine($"    LENGTH {arrayNew.Length.GetType().Name} #{arrayNew.Length.GetHashCode():X}");
                        break;
                    case MirSyscall syscall:
                        for (int argIndex = 0; argIndex < syscall.Arguments.Count; argIndex++)
                        {
                            var arg = syscall.Arguments[argIndex];
                            writer.WriteLine($"    ARG[{argIndex}] {arg.GetType().Name} #{arg.GetHashCode():X}");
                        }
                        break;
                }

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

            writer.WriteLine($"  TERM {FormatTerminator(block.Terminator)}");
        }
    }

    private static string FormatTerminator(MirTerminator? terminator)
    {
        if (terminator is null)
            return "<null>";

        return terminator switch
        {
            MirBranch branch => $"MirBranch -> {branch.Target.Label}",
            MirCondBranch cond => $"MirCondBranch -> {cond.TrueTarget.Label}/{cond.FalseTarget.Label}",
            MirCompareBranch compare => $"MirCompareBranch -> {compare.TrueTarget.Label}/{compare.FalseTarget.Label}",
            MirSwitch @switch => $"MirSwitch -> {string.Join(",", @switch.Cases.Select(c => $"{c.Case}->{c.Target.Label}"))}; default {@switch.DefaultTarget.Label}",
            MirLeave leave => $"MirLeave -> {leave.Target.Label}",
            MirEndFinally endFinally => $"MirEndFinally -> {endFinally.Target.Label}",
            _ => terminator.GetType().Name
        };
    }

    private static void CheckForOrphanConstants(MirFunction function, string passName)
    {
        var definitions = new HashSet<MirValue>();

        foreach (var block in function.Blocks)
        {
            foreach (var phi in block.Phis)
                definitions.Add(phi);
            foreach (var inst in block.Instructions)
                definitions.Add(inst);
        }

        var orphans = new List<(MirBlock Block, MirValue Value, MirInst Inst)>();
        foreach (var block in function.Blocks)
        {
            foreach (var inst in block.Instructions)
            {
                foreach (var operand in EnumerateOperands(inst))
                {
                    if (operand is MirConstInt or MirConstBool or MirConstByteString or MirConstBuffer or MirConstNull)
                    {
                        if (!definitions.Contains(operand))
                            orphans.Add((block, operand, inst));
    }
}
            }
        }

        if (orphans.Count == 0)
            return;

        var dumpPath = Path.Combine(Path.GetTempPath(), "mir_orphans.txt");
        using (var writer = new StreamWriter(dumpPath, append: true))
        {
            writer.WriteLine($"=== ORPHAN CONSTANTS after {passName} on {function.Name} ===");
            foreach (var orphan in orphans)
            {
                writer.WriteLine($"BLOCK {orphan.Block.Label} INST {orphan.Inst.GetType().Name} uses {orphan.Value.GetType().Name} #{orphan.Value.GetHashCode():X} without definition.");
            }
            writer.WriteLine();
        }
    }

    private static IEnumerable<MirValue> EnumerateOperands(MirInst inst)
    {
        switch (inst)
        {
            case MirUnary unary:
                yield return unary.Operand;
                yield break;
            case MirConvert convert:
                yield return convert.Value;
                yield break;
            case MirBinary binary:
                yield return binary.Left;
                yield return binary.Right;
                yield break;
            case MirCompare compare:
                yield return compare.Left;
                yield return compare.Right;
                yield break;
            case MirStructPack pack:
                foreach (var field in pack.Fields)
                    yield return field;
                yield break;
            case MirStructGet get:
                yield return get.Object;
                yield break;
            case MirStructSet set:
                yield return set.Object;
                yield return set.Value;
                yield break;
            case MirArrayNew arrayNew:
                yield return arrayNew.Length;
                yield break;
            case MirArrayLen arrayLen:
                yield return arrayLen.Array;
                yield break;
            case MirArrayGet arrayGet:
                yield return arrayGet.Array;
                yield return arrayGet.Index;
                yield break;
            case MirArraySet arraySet:
                yield return arraySet.Array;
                yield return arraySet.Index;
                yield return arraySet.Value;
                yield break;
            case MirSyscall syscall:
                foreach (var arg in syscall.Arguments)
                    yield return arg;
                yield break;
            case MirSlice slice:
                yield return slice.Value;
                yield return slice.Start;
                yield return slice.Length;
                yield break;
            case MirConcat concat:
                yield return concat.Left;
                yield return concat.Right;
                yield break;
        }
    }

    private static void TraceLocalInstructions(MirFunction function, string passName)
    {
        var loads = 0;
        var stores = 0;
        foreach (var block in function.Blocks)
        {
            foreach (var inst in block.Instructions)
            {
                if (inst is MirLoadLocal)
                    loads++;
                else if (inst is MirStoreLocal)
                    stores++;
            }
        }

        Console.WriteLine($"[MIR-LOCALS] After {passName}: loads={loads} stores={stores}");
    }
}
