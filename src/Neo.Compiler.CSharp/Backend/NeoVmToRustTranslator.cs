// Copyright (C) 2015-2026 The Neo Project.
//
// NeoVmToRustTranslator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.VM;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler.Backend.RiscV;

/// <summary>
/// Translates compiled NeoVM method instruction lists into a complete Rust source file.
/// Methods with no control flow are emitted linearly. Methods containing jumps use
/// an offset-based state machine (match on _pc).
/// </summary>
internal class NeoVmToRustTranslator
{
    /// <summary>
    /// Produces a complete .rs file containing all exported methods and a dispatch function.
    /// </summary>
    public string Translate(string contractName, IReadOnlyList<(string Name, IReadOnlyList<Instruction> Instructions)> methods)
    {
        var builder = new RustCodeBuilder();

        foreach (var (name, instructions) in methods)
        {
            builder.BeginMethod(name);
            if (HasControlFlow(instructions))
                EmitStateMachine(builder, instructions);
            else
                EmitLinear(builder, instructions);
            builder.EndMethod();
        }

        return builder.Build(contractName);
    }

    /// <summary>
    /// Returns true if the method contains any jump, call, return, or try/catch instruction.
    /// </summary>
    private static bool HasControlFlow(IReadOnlyList<Instruction> instructions)
    {
        foreach (var instr in instructions)
        {
            if (instr.Target != null) return true;
            switch (instr.OpCode)
            {
                case OpCode.JMP:
                case OpCode.JMP_L:
                case OpCode.JMPIF:
                case OpCode.JMPIF_L:
                case OpCode.JMPIFNOT:
                case OpCode.JMPIFNOT_L:
                case OpCode.JMPEQ:
                case OpCode.JMPEQ_L:
                case OpCode.JMPNE:
                case OpCode.JMPNE_L:
                case OpCode.JMPGT:
                case OpCode.JMPGT_L:
                case OpCode.JMPGE:
                case OpCode.JMPGE_L:
                case OpCode.JMPLT:
                case OpCode.JMPLT_L:
                case OpCode.JMPLE:
                case OpCode.JMPLE_L:
                case OpCode.CALL:
                case OpCode.CALL_L:
                case OpCode.CALLA:
                case OpCode.CALLT:
                case OpCode.RET:
                case OpCode.TRY:
                case OpCode.TRY_L:
                case OpCode.ENDTRY:
                case OpCode.ENDTRY_L:
                case OpCode.ENDFINALLY:
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Emits instructions linearly (no state machine). Used for methods with no jumps.
    /// </summary>
    private static void EmitLinear(RustCodeBuilder builder, IReadOnlyList<Instruction> instructions)
    {
        foreach (var instr in instructions)
        {
            string? code = InstructionTranslator.Translate(instr);
            if (code != null)
                builder.Line(code);
        }
    }

    /// <summary>
    /// Emits an offset-based state machine for methods with control flow.
    /// Each instruction is a match arm keyed by its byte offset.
    /// </summary>
    private static void EmitStateMachine(RustCodeBuilder builder, IReadOnlyList<Instruction> instructions)
    {
        if (instructions.Count == 0) return;

        // Build a map of instruction index -> next instruction's offset for fallthrough
        var offsetToIndex = new Dictionary<int, int>();
        for (int i = 0; i < instructions.Count; i++)
            offsetToIndex[instructions[i].Offset] = i;

        builder.Line($"let mut _pc: i32 = {instructions[0].Offset};");
        builder.Line("loop {");
        builder.Line("    if ctx.is_faulted() { return; }");
        builder.Line("    if let Some(new_pc) = ctx.check_exception() { _pc = new_pc; continue; }");
        builder.Line("    match _pc {");

        for (int i = 0; i < instructions.Count; i++)
        {
            var instr = instructions[i];
            int offset = instr.Offset;
            int nextOffset = (i + 1 < instructions.Count) ? instructions[i + 1].Offset : -1;

            string body = TranslateWithControlFlow(instr, nextOffset);
            builder.Line($"        {offset} => {{ {body} }}");
        }

        builder.Line("        _ => { ctx.fault(\"invalid pc\"); return; }");
        builder.Line("    }");
        builder.Line("}");
    }

    /// <summary>
    /// Translates a single instruction within a state machine context, handling both
    /// data instructions (via InstructionTranslator) and control-flow instructions.
    /// </summary>
    private static string TranslateWithControlFlow(Instruction instr, int nextOffset)
    {
        string advance = nextOffset >= 0 ? $"_pc = {nextOffset};" : "return;";

        switch (instr.OpCode)
        {
            case OpCode.NOP:
                return advance;

            case OpCode.RET:
                return "if let Some(pc) = ctx.call_pop() { _pc = pc; } else { return; }";

            case OpCode.JMP:
            case OpCode.JMP_L:
                return $"_pc = {GetTargetOffset(instr)};";

            case OpCode.JMPIF:
            case OpCode.JMPIF_L:
                return $"if ctx.pop_bool() {{ _pc = {GetTargetOffset(instr)}; }} else {{ {advance} }}";

            case OpCode.JMPIFNOT:
            case OpCode.JMPIFNOT_L:
                return $"if !ctx.pop_bool() {{ _pc = {GetTargetOffset(instr)}; }} else {{ {advance} }}";

            case OpCode.JMPEQ:
            case OpCode.JMPEQ_L:
                return $"if ctx.pop_cmp_eq() {{ _pc = {GetTargetOffset(instr)}; }} else {{ {advance} }}";

            case OpCode.JMPNE:
            case OpCode.JMPNE_L:
                return $"if ctx.pop_cmp_ne() {{ _pc = {GetTargetOffset(instr)}; }} else {{ {advance} }}";

            case OpCode.JMPGT:
            case OpCode.JMPGT_L:
                return $"if ctx.pop_cmp_gt() {{ _pc = {GetTargetOffset(instr)}; }} else {{ {advance} }}";

            case OpCode.JMPGE:
            case OpCode.JMPGE_L:
                return $"if ctx.pop_cmp_ge() {{ _pc = {GetTargetOffset(instr)}; }} else {{ {advance} }}";

            case OpCode.JMPLT:
            case OpCode.JMPLT_L:
                return $"if ctx.pop_cmp_lt() {{ _pc = {GetTargetOffset(instr)}; }} else {{ {advance} }}";

            case OpCode.JMPLE:
            case OpCode.JMPLE_L:
                return $"if ctx.pop_cmp_le() {{ _pc = {GetTargetOffset(instr)}; }} else {{ {advance} }}";

            case OpCode.CALL:
            case OpCode.CALL_L:
                return $"ctx.call_push({nextOffset}); _pc = {GetTargetOffset(instr)};";

            case OpCode.CALLA:
                return "_pc = ctx.pop_integer() as i32;";

            case OpCode.CALLT:
                {
                    ushort token = BinaryPrimitives.ReadUInt16LittleEndian(instr.Operand!);
                    uint calltHash = 0x43540000u | token;
                    return $"ctx.syscall(0x{calltHash:x8}); {advance}";
                }

            case OpCode.TRY:
            case OpCode.TRY_L:
                {
                    int catchOffset = instr.Target?.Instruction?.Offset ?? 0;
                    int finallyOffset = instr.Target2?.Instruction?.Offset ?? 0;
                    return $"ctx.try_enter({catchOffset}, {finallyOffset}); {advance}";
                }

            case OpCode.ENDTRY:
            case OpCode.ENDTRY_L:
                {
                    int endTarget = GetTargetOffset(instr);
                    return $"_pc = ctx.end_try({endTarget});";
                }

            case OpCode.ENDFINALLY:
                return "_pc = ctx.end_finally();";

            // Abort always faults — do not advance
            case OpCode.ABORT:
                return "ctx.abort();";
            case OpCode.ABORTMSG:
                return "ctx.abort_msg();";

            // Assert: if it passes, advance; if it faults, is_faulted() catches it
            case OpCode.ASSERT:
                return $"ctx.assert_top(); {advance}";
            case OpCode.ASSERTMSG:
                return $"ctx.assert_msg(); {advance}";

            // Throw always transfers control — do not advance
            case OpCode.THROW:
                return "ctx.throw_ex();";

            default:
                {
                    // Data / arithmetic / stack / collection instructions
                    string? code = InstructionTranslator.Translate(instr);
                    if (code != null)
                        return $"{code} {advance}";
                    // Unknown opcode — fault rather than silently skip
                    return $"ctx.fault(\"unsupported opcode: {instr.OpCode}\");";
                }
        }
    }

    /// <summary>
    /// Resolves the target offset from an instruction's jump target.
    /// Falls back to computing from the operand when the target is not linked.
    /// </summary>
    private static int GetTargetOffset(Instruction instr)
    {
        if (instr.Target?.Instruction != null)
            return instr.Target.Instruction.Offset;

        // Fallback: compute from operand (signed offset relative to instruction start)
        if (instr.Operand != null && instr.Operand.Length >= 1)
        {
            int relOffset = instr.Operand.Length >= 4
                ? BinaryPrimitives.ReadInt32LittleEndian(instr.Operand)
                : (sbyte)instr.Operand[0];
            return instr.Offset + relOffset;
        }

        // Cannot resolve target — emit a sentinel that the state machine will catch
        // as an invalid pc, rather than silently jumping to offset 0.
        return -1;
    }
}
