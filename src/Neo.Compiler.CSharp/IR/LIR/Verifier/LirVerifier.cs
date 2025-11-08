using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.LIR;

/// <summary>
/// Performs structural validation on Stack-LIR to guarantee stack discipline and catch malformed control flow prior to
/// emission. Currently tracks stack height per block and reports underflows.
/// </summary>
internal sealed partial class LirVerifier
{

    internal Result Verify(LirFunction function)
    {
        if (function is null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        var errors = new List<string>();
        var labelSet = new HashSet<string>(function.Blocks.Select(b => b.Label), StringComparer.Ordinal);
        var entryBlock = function.Blocks.FirstOrDefault();

        foreach (var block in function.Blocks)
        {
            var stackHeight = ReferenceEquals(block, entryBlock) ? function.EntryParameterCount : 0;
            var stackKnown = true;

            for (var i = 0; i < block.Instructions.Count; i++)
            {
                var instruction = block.Instructions[i];
                var opcodeInfo = LirOpcodeTable.Get(instruction.Op);
                var pop = instruction.PopOverride ?? opcodeInfo.Pop;
                var push = instruction.PushOverride ?? opcodeInfo.Push;

                if (stackKnown && pop.HasValue)
                {
                    var popVal = pop.Value;
                    if (stackHeight < popVal)
                    {
                        errors.Add($"Stack underflow in {function.Name}/{block.Label} at instruction {i} ({instruction.Op}).");
                    }

                    stackHeight -= popVal;
                }
                else if (!pop.HasValue)
                {
                    stackKnown = false;
                }

                if (stackKnown && push.HasValue)
                {
                    stackHeight += push.Value;
                    if (stackHeight < 0)
                    {
                        errors.Add($"Negative stack height in {function.Name}/{block.Label} at instruction {i}.");
                    }
                }
                else if (!push.HasValue)
                {
                    stackKnown = false;
                }

                ValidateInstructionImmediate(function, block, instruction, i, labelSet, errors);
            }

            ValidateBlockTerminates(function, block, errors);
        }

        return errors.Count == 0 ? Result.Success() : Result.Failure(errors);
    }

    private static void ValidateInstructionImmediate(
        LirFunction function,
        LirBlock block,
        LirInst instruction,
        int instructionIndex,
        IReadOnlySet<string> labelSet,
        List<string> errors)
    {
        switch (instruction.Op)
        {
            case LirOpcode.PUSHINT:
                if (instruction.Immediate is null or { Length: 0 })
                    errors.Add($"PUSHINT missing payload in {function.Name}/{block.Label} at instruction {instructionIndex}.");
                else if (instruction.Immediate.Length > 32)
                    errors.Add($"PUSHINT payload exceeds 256-bit width in {function.Name}/{block.Label} at instruction {instructionIndex}.");
                break;

            case LirOpcode.PUSHDATA1:
                ValidatePushData(function, block, instruction, instructionIndex, maxLength: byte.MaxValue, errors);
                break;

            case LirOpcode.PUSHDATA2:
                ValidatePushData(function, block, instruction, instructionIndex, maxLength: ushort.MaxValue, errors);
                break;

            case LirOpcode.PUSHDATA4:
                ValidatePushData(function, block, instruction, instructionIndex, maxLength: int.MaxValue, errors);
                break;

            case LirOpcode.CONVERT:
                if (instruction.Immediate is not { Length: 1 } payload)
                {
                    errors.Add($"CONVERT requires single-byte stack item type in {function.Name}/{block.Label} at instruction {instructionIndex}.");
                }
                else if (!Enum.IsDefined(typeof(Neo.VM.Types.StackItemType), payload[0]))
                {
                    errors.Add($"CONVERT immediate '{payload[0]}' is not a valid StackItemType in {function.Name}/{block.Label} at instruction {instructionIndex}.");
                }
                break;

            case LirOpcode.JMP:
            case LirOpcode.JMPIF:
            case LirOpcode.JMPIFNOT:
                if (string.IsNullOrEmpty(instruction.TargetLabel))
                {
                    errors.Add($"{instruction.Op} missing target label in {function.Name}/{block.Label} at instruction {instructionIndex}.");
                }
                else if (!labelSet.Contains(instruction.TargetLabel))
                {
                    errors.Add($"{instruction.Op} targets unknown label '{instruction.TargetLabel}' in {function.Name}/{block.Label} at instruction {instructionIndex}.");
                }
                break;

            case LirOpcode.CALL:
                if (string.IsNullOrEmpty(instruction.TargetLabel))
                    errors.Add($"{instruction.Op} missing target label in {function.Name}/{block.Label} at instruction {instructionIndex}.");
                break;

            case LirOpcode.SYSCALL:
                if (instruction.Immediate is not { Length: 4 })
                    errors.Add($"SYSCALL requires 4-byte identifier in {function.Name}/{block.Label} at instruction {instructionIndex}.");
                break;
        }
    }

    private static void ValidatePushData(
        LirFunction function,
        LirBlock block,
        LirInst instruction,
        int instructionIndex,
        int maxLength,
        List<string> errors)
    {
        if (instruction.Immediate is not { } payload)
        {
            errors.Add($"{instruction.Op} missing payload in {function.Name}/{block.Label} at instruction {instructionIndex}.");
            return;
        }

        if (payload.Length > maxLength)
        {
            errors.Add($"{instruction.Op} payload exceeds maximum allowed length ({maxLength}) in {function.Name}/{block.Label} at instruction {instructionIndex}.");
        }
    }

    private static void ValidateBlockTerminates(LirFunction function, LirBlock block, List<string> errors)
    {
        if (block.Instructions.Count == 0)
            return;

        var terminal = block.Instructions[^1].Op;
        var isTerminator = terminal is LirOpcode.RET
            or LirOpcode.ABORT
            or LirOpcode.ABORTMSG
            or LirOpcode.JMP
            or LirOpcode.JMPIF
            or LirOpcode.JMPIFNOT
            or LirOpcode.ENDTRY_L
            or LirOpcode.ENDFINALLY;

        if (!isTerminator)
        {
            errors.Add($"Block '{block.Label}' in function '{function.Name}' does not end with a control-flow terminator (found {terminal}).");
        }
    }
}
