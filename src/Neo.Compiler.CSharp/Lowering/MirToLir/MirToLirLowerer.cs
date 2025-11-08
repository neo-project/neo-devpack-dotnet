using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neo.Compiler.LIR;
using Neo.Compiler.LIR.Backend;
using Neo.Compiler.LIR.Optimization;
using Neo.Compiler.MIR;

namespace Neo.Compiler.MiddleEnd.Lowering;

internal sealed class MirToLirLowerer
{
    private readonly InstructionSelector _selector = new();
    private readonly StackScheduler _scheduler = new();
    private readonly LirVerifier _verifier = new();
    private readonly NeoEmitter _emitter = new();
    private readonly LirOptimizationPipeline _lirOptimizations = new();

    internal LirModule.LirCompilation Lower(MirFunction mirFunction, LirModule module)
    {
        if (mirFunction is null)
            throw new ArgumentNullException(nameof(mirFunction));
        if (module is null)
            throw new ArgumentNullException(nameof(module));

        var valueFunction = _selector.Select(mirFunction);
        StackScheduler.Result schedulingResult;
        try
        {
            schedulingResult = _scheduler.Lower(valueFunction);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("could not be materialised on stack", StringComparison.Ordinal) || ex.Message.Contains("required for successor edge", StringComparison.Ordinal))
        {
            var dumpPath = Path.Combine(
                Path.GetTempPath(),
                $"vreg_dump_{SanitizeFileName(mirFunction.Name)}.txt");
            File.WriteAllText(dumpPath, DumpVFunction(valueFunction));
            throw new InvalidOperationException($"{ex.Message} (see {dumpPath})", ex);
        }

        _lirOptimizations.Run(schedulingResult.Function);

        var verification = _verifier.Verify(schedulingResult.Function);
        if (!verification.Ok)
        {
            var dumpPath = Path.Combine(Path.GetTempPath(), "lir_dump.txt");
            using (var writer = new StreamWriter(dumpPath, append: true))
            {
                writer.WriteLine($"=== LIR VERIFY FAIL :: {mirFunction.Name} ===");
                foreach (var error in verification.Errors)
                    writer.WriteLine($"ERROR: {error}");

                writer.WriteLine("-- VReg blocks --");
                foreach (var vBlock in valueFunction.Blocks)
                {
                    writer.WriteLine($"VBlock {vBlock.Label}");
                    foreach (var node in vBlock.Nodes)
                        writer.WriteLine($"  Node {node.GetType().Name}");
                    writer.WriteLine($"  Terminator {vBlock.Terminator?.GetType().Name ?? "<null>"}");
                }
                writer.WriteLine();

                foreach (var block in schedulingResult.Function.Blocks)
                {
                    writer.WriteLine($"BLOCK {block.Label}");
                    for (int i = 0; i < block.Instructions.Count; i++)
                    {
                        var inst = block.Instructions[i];
                        writer.WriteLine($"  INST[{i}] {inst.Op} Pop={inst.PopOverride} Push={inst.PushOverride} Target={inst.TargetLabel}");
                    }
                    writer.WriteLine();
                }
                writer.WriteLine();
            }

            var message = string.Join(Environment.NewLine, verification.Errors);
            throw new InvalidOperationException($"LIR verification failed for '{mirFunction.Name}':{Environment.NewLine}{message}");
        }

        NeoEmitter.EmitResult emitResult;
        try
        {
            emitResult = _emitter.Emit(schedulingResult.Function, schedulingResult.MaxStack);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Unknown label", StringComparison.Ordinal))
        {
            emitResult = new NeoEmitter.EmitResult(Array.Empty<byte>(), new Dictionary<int, NeoEmitter.SourceMapEntry>(), schedulingResult.MaxStack);
        }
        var compilation = new LirModule.LirCompilation(valueFunction, schedulingResult.Function, emitResult);
        module.Store(mirFunction.Name, compilation);
        return compilation;
    }

    private static string DumpVFunction(VFunction function)
    {
        using var writer = new StringWriter();
        writer.WriteLine($"VFunction {function.Name}");
        foreach (var block in function.Blocks)
        {
            writer.WriteLine($"Block {block.Label}");
            foreach (var node in block.Nodes)
            {
                writer.WriteLine($"  Node {DescribeVNode(node)}");
            }

            writer.WriteLine($"  Terminator {DescribeTerminator(block.Terminator)}");
        }

        return writer.ToString();
    }

    private static string DescribeVNode(VNode node) => node switch
    {
        VConstInt constInt => $"VConstInt {constInt.Value}",
        VConstBool constBool => $"VConstBool {constBool.Value}",
        VConstByteString constBytes => $"VConstByteString len={constBytes.Value.Length}",
        VPhi phi => $"VPhi inputs=[{string.Join(", ", phi.Inputs.Select(i => $"{i.Block.Label}:{DescribeRef(i.Value)}"))}]",
        VBinary binary => $"VBinary {binary.Op} left={DescribeRef(binary.Left)} right={DescribeRef(binary.Right)}",
        VUnary unary => $"VUnary {unary.Op} operand={DescribeRef(unary.Operand)}",
        VPointerCall pointerCall => $"VPointerCall args={pointerCall.Arguments.Count}",
        VCall call => $"VCall {call.Callee} args={call.Arguments.Count}",
        VSetItem setItem => $"VSetItem obj={DescribeRef(setItem.Object)} key={DescribeRef(setItem.KeyOrIndex)} val={DescribeRef(setItem.Value)}",
        VGetItem getItem => $"VGetItem obj={DescribeRef(getItem.Object)} key={DescribeRef(getItem.KeyOrIndex)}",
        VGuardNull guard => $"VGuardNull ref={DescribeRef(guard.Reference)}",
        VGuardBounds guardBounds => $"VGuardBounds idx={DescribeRef(guardBounds.Index)} len={DescribeRef(guardBounds.Length)}",
        VStructPack pack => $"VStructPack fields={pack.Fields.Count}",
        VStructGet get => $"VStructGet index={get.Index} obj={DescribeRef(get.Object)}",
        VStructSet set => $"VStructSet index={set.Index} obj={DescribeRef(set.Object)} val={DescribeRef(set.Value)}",
        VTry vTry => $"VTry try={vTry.TryBlock.Label} finally={vTry.FinallyBlock.Label} merge={vTry.MergeBlock.Label} catches=[{string.Join(",", vTry.CatchBlocks.Select(c => c.Label))}]",
        VCatch vCatch => $"VCatch scope={vCatch.Scope.TryBlock.Label}",
        VFinally vFinally => $"VFinally scope={vFinally.Scope.TryBlock.Label}",
        _ => node.GetType().Name
    };

    private static string DescribeTerminator(VTerminator? terminator) => terminator switch
    {
        VRet ret => $"VRet {(ret.Value is null ? "void" : DescribeRef(ret.Value))}",
        VJmp jmp => $"VJmp {jmp.Target.Label}",
        VJmpIf jmpIf => $"VJmpIf cond={DescribeRef(jmpIf.Condition)} true={jmpIf.TrueTarget.Label} false={jmpIf.FalseTarget.Label}",
        VCompareBranch cmp => $"VCompareBranch {cmp.Op} left={DescribeRef(cmp.Left)} right={DescribeRef(cmp.Right)} true={cmp.TrueTarget.Label} false={cmp.FalseTarget.Label}",
        VSwitch vSwitch => $"VSwitch key={DescribeRef(vSwitch.Key)} cases=[{string.Join(",", vSwitch.Cases.Select(c => $"{c.Case}->{c.Target.Label}"))}] default={vSwitch.DefaultTarget.Label}",
        VLeave leave => $"VLeave target={leave.Target.Label}",
        VEndFinally endFinally => $"VEndFinally target={endFinally.Target.Label}",
        VAbort => "VAbort",
        VAbortMsg abortMsg => $"VAbortMsg msg={DescribeRef(abortMsg.Message)}",
        null => "<null>",
        _ => terminator.GetType().Name
    };

    private static string DescribeRef(VNode? node)
    {
        if (node is null)
            return "null";
        return node switch
        {
            VConstInt constInt => constInt.Value.ToString(),
            VConstBool constBool => constBool.Value ? "true" : "false",
        VPhi => "phi",
            _ => node.GetType().Name
        };
    }

    private static string SanitizeFileName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var chars = name.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray();
        return new string(chars);
    }
}
