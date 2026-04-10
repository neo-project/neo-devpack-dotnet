// Copyright (C) 2015-2026 The Neo Project.
//
// RiscVEmitter.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler.Backend.RiscV;

internal class RiscVLabel : ILabel
{
    public string Name { get; }
    public RiscVLabel(string name) => Name = name;
}

internal class RiscVTryBlock : ITryBlock
{
    public string CatchLabel { get; set; } = "";
    public string FinallyLabel { get; set; } = "";
}

/// <summary>
/// ICodeEmitter implementation that generates Rust source code targeting
/// the RISC-V Context API (neo_riscv_rt::Context).
/// Each emitter method appends a Rust statement to the RustCodeBuilder.
/// </summary>
internal class RiscVEmitter : ICodeEmitter
{
    private readonly RustCodeBuilder _builder = new();

    /// <summary>
    /// The underlying builder. Callers use <see cref="RustCodeBuilder.Build"/>
    /// to obtain the final Rust source.
    /// </summary>
    public RustCodeBuilder Builder => _builder;

    #region Method lifecycle

    public void BeginMethod(string name, int paramCount, int localCount)
    {
        _builder.BeginMethod(name);
    }

    public void EndMethod()
    {
        _builder.EndMethod();
    }

    #endregion

    #region Stack: push values

    public void PushInt(BigInteger value)
    {
        _builder.Line($"ctx.push_int({value});");
    }

    public void PushBool(bool value)
    {
        _builder.Line($"ctx.push_bool({(value ? "true" : "false")});");
    }

    public void PushBytes(byte[] data)
    {
        var hex = string.Join(", ", data.Select(b => $"0x{b:x2}"));
        _builder.Line($"ctx.push_bytes(&[{hex}]);");
    }

    public void PushString(string value)
    {
        var escaped = value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t")
            .Replace("\0", "\\0");
        _builder.Line($"ctx.push_bytes(\"{escaped}\".as_bytes());");
    }

    public void PushNull()
    {
        _builder.Line("ctx.push_null();");
    }

    public void PushDefault(byte stackItemType)
    {
        // StackItemType: Boolean=1, Integer=2
        switch (stackItemType)
        {
            case 1:
                _builder.Line("ctx.push_bool(false);");
                break;
            case 2:
                _builder.Line("ctx.push_int(0);");
                break;
            default:
                _builder.Line("ctx.push_null();");
                break;
        }
    }

    #endregion

    #region Stack manipulation

    public void Drop(int count = 1)
    {
        for (int i = 0; i < count; i++)
            _builder.Line("ctx.drop();");
    }

    public void Dup() => _builder.Line("ctx.dup();");
    public void Nip() => _builder.Line("ctx.nip();");

    public void XDrop(int? count)
    {
        if (count.HasValue)
            _builder.Line($"ctx.push_int({count.Value}); ctx.xdrop();");
        else
            _builder.Line("ctx.xdrop();");
    }

    public void Over() => _builder.Line("ctx.over();");

    public void Pick(int? index)
    {
        if (index.HasValue)
            _builder.Line($"ctx.push_int({index.Value}); ctx.pick();");
        else
            _builder.Line("ctx.pick();");
    }

    public void Tuck() => _builder.Line("ctx.tuck();");
    public void Swap() => _builder.Line("ctx.swap();");
    public void Rot() => _builder.Line("ctx.rot();");

    public void Roll(int? index)
    {
        if (index.HasValue)
            _builder.Line($"ctx.push_int({index.Value}); ctx.roll();");
        else
            _builder.Line("ctx.roll();");
    }

    public void Reverse3() => _builder.Line("ctx.reverse3();");
    public void Reverse4() => _builder.Line("ctx.reverse4();");

    public void ReverseN(int count)
    {
        _builder.Line($"ctx.push_int({count}); ctx.reverse_n();");
    }

    public void Clear() => _builder.Line("ctx.clear();");
    public void Depth() => _builder.Line("ctx.depth();");

    #endregion

    #region Arithmetic

    public void Add() => _builder.Line("ctx.add();");
    public void Sub() => _builder.Line("ctx.sub();");
    public void Mul() => _builder.Line("ctx.mul();");
    public void Div() => _builder.Line("ctx.div();");
    public void Mod() => _builder.Line("ctx.modulo();");
    public void Negate() => _builder.Line("ctx.negate();");
    public void Abs() => _builder.Line("ctx.abs();");
    public void Sign() => _builder.Line("ctx.sign();");
    public void Min() => _builder.Line("ctx.min();");
    public void Max() => _builder.Line("ctx.max();");
    public void Pow() => _builder.Line("ctx.pow();");
    public void Sqrt() => _builder.Line("ctx.sqrt();");
    public void ModMul() => _builder.Line("ctx.mod_mul();");
    public void ModPow() => _builder.Line("ctx.mod_pow();");
    public void ShiftLeft() => _builder.Line("ctx.shl();");
    public void ShiftRight() => _builder.Line("ctx.shr();");

    #endregion

    #region Bitwise

    public void BitwiseAnd() => _builder.Line("ctx.bitwise_and();");
    public void BitwiseOr() => _builder.Line("ctx.bitwise_or();");
    public void BitwiseXor() => _builder.Line("ctx.bitwise_xor();");
    public void BitwiseNot() => _builder.Line("ctx.bitwise_not();");

    #endregion

    #region Comparison & Logic

    public void Equal() => _builder.Line("ctx.equal();");
    public void NotEqual() => _builder.Line("ctx.not_equal();");
    public void LessThan() => _builder.Line("ctx.less_than();");
    public void LessOrEqual() => _builder.Line("ctx.less_or_equal();");
    public void GreaterThan() => _builder.Line("ctx.greater_than();");
    public void GreaterOrEqual() => _builder.Line("ctx.greater_or_equal();");
    public void BoolAnd() => _builder.Line("ctx.bool_and();");
    public void BoolOr() => _builder.Line("ctx.bool_or();");
    public void Not() => _builder.Line("ctx.not();");
    public void NullCheck() => _builder.Line("ctx.is_null();");

    #endregion

    #region Type operations

    public void IsType(byte stackItemType)
    {
        _builder.Line($"ctx.is_type(0x{stackItemType:x2});");
    }

    public void Convert(byte stackItemType)
    {
        _builder.Line($"ctx.convert(0x{stackItemType:x2});");
    }

    #endregion

    #region Control flow

    public ILabel DefineLabel()
    {
        return new RiscVLabel(_builder.NewLabel());
    }

    public void MarkLabel(ILabel label)
    {
        var riscVLabel = (RiscVLabel)label;
        _builder.Line($"// {riscVLabel.Name}:");
    }

    public void Emit_Jump(ILabel target)
    {
        var riscVLabel = (RiscVLabel)target;
        _builder.Line($"// jump {riscVLabel.Name}");
    }

    public void Emit_JumpIf(ILabel target)
    {
        var riscVLabel = (RiscVLabel)target;
        _builder.Line($"// jump_if {riscVLabel.Name}");
    }

    public void Emit_JumpIfNot(ILabel target)
    {
        var riscVLabel = (RiscVLabel)target;
        _builder.Line($"// jump_if_not {riscVLabel.Name}");
    }

    public void Emit_JumpEq(ILabel target)
    {
        var riscVLabel = (RiscVLabel)target;
        _builder.Line($"// jump_eq {riscVLabel.Name}");
    }

    public void Emit_JumpNe(ILabel target)
    {
        var riscVLabel = (RiscVLabel)target;
        _builder.Line($"// jump_ne {riscVLabel.Name}");
    }

    public void Emit_JumpGt(ILabel target)
    {
        var riscVLabel = (RiscVLabel)target;
        _builder.Line($"// jump_gt {riscVLabel.Name}");
    }

    public void Emit_JumpGe(ILabel target)
    {
        var riscVLabel = (RiscVLabel)target;
        _builder.Line($"// jump_ge {riscVLabel.Name}");
    }

    public void Emit_JumpLt(ILabel target)
    {
        var riscVLabel = (RiscVLabel)target;
        _builder.Line($"// jump_lt {riscVLabel.Name}");
    }

    public void Emit_JumpLe(ILabel target)
    {
        var riscVLabel = (RiscVLabel)target;
        _builder.Line($"// jump_le {riscVLabel.Name}");
    }

    public void Call(ILabel target)
    {
        var riscVLabel = (RiscVLabel)target;
        _builder.Line($"// call {riscVLabel.Name}");
    }

    public void Ret() => _builder.Line("ctx.ret();");
    public void Throw() => _builder.Line("ctx.throw_ex();");
    public void Abort() => _builder.Line("ctx.abort();");
    public void AbortMsg() => _builder.Line("ctx.abort_msg();");
    public void Assert() => _builder.Line("ctx.assert_top();");
    public void AssertMsg() => _builder.Line("ctx.assert_msg();");
    public void Nop() => _builder.Line("// nop");

    #endregion

    #region Slots (variables)

    public void InitSlot(byte localCount, byte paramCount)
    {
        _builder.Line($"ctx.init_slot({localCount}, {paramCount});");
    }

    public void LdArg(byte index) => _builder.Line($"ctx.load_arg({index});");
    public void StArg(byte index) => _builder.Line($"ctx.store_arg({index});");
    public void LdLoc(byte index) => _builder.Line($"ctx.load_local({index});");
    public void StLoc(byte index) => _builder.Line($"ctx.store_local({index});");
    public void LdSFld(byte index) => _builder.Line($"ctx.load_static({index});");
    public void StSFld(byte index) => _builder.Line($"ctx.store_static({index});");

    #endregion

    #region Syscalls & interop

    public void Syscall(uint hash)
    {
        _builder.Line($"ctx.syscall(0x{hash:x8});");
    }

    public void CallToken(ushort token)
    {
        uint calltHash = 0x43540000u | token;
        _builder.Line($"ctx.syscall(0x{calltHash:x8});");
    }

    #endregion

    #region Collections

    public void NewArray() => _builder.Line("ctx.new_array();");

    public void NewArrayT(byte type)
    {
        _builder.Line($"ctx.new_array_t(0x{type:x2});");
    }

    public void NewStruct(int fieldCount)
    {
        _builder.Line($"ctx.push_int({fieldCount}); ctx.new_struct();");
    }

    public void NewMap() => _builder.Line("ctx.new_map();");
    public void NewBuffer() => _builder.Line("ctx.new_buffer();");
    public void Append() => _builder.Line("ctx.append();");
    public void SetItem() => _builder.Line("ctx.set_item();");
    public void GetItem() => _builder.Line("ctx.pick_item();");
    public void Remove() => _builder.Line("ctx.remove();");
    public void Size() => _builder.Line("ctx.size();");
    public void HasKey() => _builder.Line("ctx.has_key();");
    public void Keys() => _builder.Line("ctx.keys();");
    public void Values() => _builder.Line("ctx.values();");

    public void Pack(int count)
    {
        _builder.Line($"ctx.push_int({count}); ctx.pack();");
    }

    public void Unpack() => _builder.Line("ctx.unpack();");
    public void DeepCopy() => _builder.Line("// deep_copy: not yet supported in RISC-V backend");
    public void ReverseItems() => _builder.Line("ctx.reverse_items();");
    public void ClearItems() => _builder.Line("ctx.clear_items();");
    public void PopItem() => _builder.Line("ctx.pop_item();");

    #endregion

    #region String / Byte

    public void Cat() => _builder.Line("ctx.cat();");
    public void Substr() => _builder.Line("ctx.substr();");
    public void Left() => _builder.Line("ctx.left();");
    public void Right() => _builder.Line("ctx.right();");
    public void MemCpy() => _builder.Line("ctx.memcpy();");
    public void NumEqual() => _builder.Line("ctx.num_equal();");
    public void NumNotEqual() => _builder.Line("ctx.num_not_equal();");

    #endregion

    #region Exception handling

    public ITryBlock BeginTry(ILabel catchLabel, ILabel finallyLabel)
    {
        var catchName = ((RiscVLabel)catchLabel).Name;
        var finallyName = ((RiscVLabel)finallyLabel).Name;
        _builder.Line($"// try (catch: {catchName}, finally: {finallyName})");
        return new RiscVTryBlock
        {
            CatchLabel = catchName,
            FinallyLabel = finallyName
        };
    }

    public void EndTry(ILabel endLabel)
    {
        var name = ((RiscVLabel)endLabel).Name;
        _builder.Line($"// end_try -> {name}");
    }

    public void EndTryFinally()
    {
        _builder.Line("// end_try_finally");
    }

    public void EndFinally()
    {
        _builder.Line("// end_finally");
    }

    #endregion

    #region Raw opcode fallback

    public void EmitRaw(byte opcode, byte[]? operand = null)
    {
        if (operand != null && operand.Length > 0)
        {
            var hex = string.Join(", ", operand.Select(b => $"0x{b:x2}"));
            _builder.Line($"// raw opcode 0x{opcode:x2} [{hex}]");
        }
        else
        {
            _builder.Line($"// raw opcode 0x{opcode:x2}");
        }
    }

    #endregion
}
