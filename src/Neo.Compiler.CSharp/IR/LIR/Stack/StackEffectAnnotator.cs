using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.LIR.Backend;

/// <summary>
/// Post-processing pass that annotates Stack-LIR instructions with precise stack overrides when the effect can be
/// derived statically (e.g., PACK with constant arity). This improves verifier accuracy without requiring MIR data.
/// </summary>
internal static partial class StackEffectAnnotator
{
    internal static void Annotate(LirFunction function)
    {
        if (function is null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        foreach (var block in function.Blocks)
        {
            var stack = new List<ValueInfo>();
            var stackKnown = true;

            foreach (var inst in block.Instructions)
            {
                var info = LirOpcodeTable.Get(inst.Op);

                var pop = inst.PopOverride ?? info.Pop;
                var push = inst.PushOverride ?? info.Push;

                if (stackKnown)
                {
                    switch (inst.Op)
                    {
                        case LirOpcode.PACK when TryPeekInt(stack, out var packCount) && packCount >= 0 && stack.Count >= packCount + 1:
                            pop = packCount + 1;
                            push = 1;
                            inst.PopOverride = pop;
                            inst.PushOverride = push;
                            break;

                        case LirOpcode.PACKSTRUCT when TryPeekInt(stack, out var packStructCount) && packStructCount >= 0 && stack.Count >= packStructCount + 1:
                            pop = packStructCount + 1;
                            push = 1;
                            inst.PopOverride = pop;
                            inst.PushOverride = push;
                            break;
                    }
                }

                if (stackKnown)
                {
                    if (pop.HasValue)
                    {
                        if (stack.Count < pop.Value)
                        {
                            stackKnown = false;
                        }
                        else
                        {
                            stack.RemoveRange(stack.Count - pop.Value, pop.Value);
                        }
                    }
                    else
                    {
                        stackKnown = false;
                    }
                }

                if (stackKnown)
                {
                    if (push.HasValue)
                    {
                        for (var i = 0; i < push.Value; i++)
                        {
                            stack.Add(new ValueInfo());
                        }

                        if (push.Value > 0)
                        {
                            var topIndex = stack.Count - 1;
                            stack[topIndex].IntConst = TryDecodeInt(inst);
                        }
                    }
                    else
                    {
                        stackKnown = false;
                    }
                }

                if (!stackKnown)
                {
                    // Reset state to avoid cascading invalid assumptions.
                    stack.Clear();
                }
            }
        }
    }

    private static bool TryPeekInt(List<ValueInfo> stack, out int value)
    {
        if (stack.Count > 0 && stack[^1].IntConst is int count)
        {
            value = count;
            return true;
        }

        value = default;
        return false;
    }

    private static int? TryDecodeInt(LirInst inst)
    {
        return inst.Op switch
        {
            LirOpcode.PUSH0 => 0,
            LirOpcode.PUSHM1 => -1,
            LirOpcode.PUSHINT when inst.Immediate is { Length: > 0 } => DecodeImmediateInt(inst.Immediate),
            _ => null
        };
    }

    private static int? DecodeImmediateInt(byte[] bytes)
    {
        var value = new BigInteger(bytes);
        return value >= int.MinValue && value <= int.MaxValue ? (int)value : null;
    }
}
