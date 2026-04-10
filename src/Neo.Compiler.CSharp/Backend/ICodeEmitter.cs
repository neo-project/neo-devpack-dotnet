// Copyright (C) 2015-2026 The Neo Project.
//
// ICodeEmitter.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;

namespace Neo.Compiler.Backend;

public interface ILabel { }
public interface ITryBlock { }

public interface ICodeEmitter
{
    // Method lifecycle
    void BeginMethod(string name, int paramCount, int localCount);
    void EndMethod();

    // Stack: push values
    void PushInt(BigInteger value);
    void PushBool(bool value);
    void PushBytes(byte[] data);
    void PushString(string value);
    void PushNull();
    void PushDefault(byte stackItemType);

    // Stack manipulation
    void Drop(int count = 1);
    void Dup();
    void Nip();
    void XDrop(int? count);
    void Over();
    void Pick(int? index);
    void Tuck();
    void Swap();
    void Rot();
    void Roll(int? index);
    void Reverse3();
    void Reverse4();
    void ReverseN(int count);
    void Clear();
    void Depth();

    // Arithmetic
    void Add(); void Sub(); void Mul(); void Div(); void Mod();
    void Negate(); void Abs(); void Sign();
    void Min(); void Max(); void Pow(); void Sqrt();
    void ModMul(); void ModPow();
    void ShiftLeft(); void ShiftRight();

    // Bitwise
    void BitwiseAnd(); void BitwiseOr(); void BitwiseXor(); void BitwiseNot();

    // Comparison & Logic
    void Equal(); void NotEqual();
    void LessThan(); void LessOrEqual();
    void GreaterThan(); void GreaterOrEqual();
    void BoolAnd(); void BoolOr(); void Not();
    void NullCheck();

    // Type operations
    void IsType(byte stackItemType);
    void Convert(byte stackItemType);

    // Control flow
    ILabel DefineLabel();
    void MarkLabel(ILabel label);
    void Emit_Jump(ILabel target);
    void Emit_JumpIf(ILabel target);
    void Emit_JumpIfNot(ILabel target);
    void Emit_JumpEq(ILabel target);
    void Emit_JumpNe(ILabel target);
    void Emit_JumpGt(ILabel target);
    void Emit_JumpGe(ILabel target);
    void Emit_JumpLt(ILabel target);
    void Emit_JumpLe(ILabel target);
    void Call(ILabel target);
    void Ret();
    void Throw();
    void Abort();
    void AbortMsg();
    void Assert();
    void AssertMsg();
    void Nop();

    // Slots (variables)
    void InitSlot(byte localCount, byte paramCount);
    void LdArg(byte index); void StArg(byte index);
    void LdLoc(byte index); void StLoc(byte index);
    void LdSFld(byte index); void StSFld(byte index);

    // Syscalls & interop
    void Syscall(uint hash);
    void CallToken(ushort token);

    // Collections
    void NewArray(); void NewArrayT(byte type);
    void NewStruct(int fieldCount);
    void NewMap(); void NewBuffer();
    void Append(); void SetItem(); void GetItem(); void Remove();
    void Size(); void HasKey(); void Keys(); void Values();
    void Pack(int count); void Unpack();
    void DeepCopy(); void ReverseItems(); void ClearItems(); void PopItem();

    // String / Byte
    void Cat(); void Substr(); void Left(); void Right();
    void MemCpy(); void NumEqual(); void NumNotEqual();

    // Exception handling
    ITryBlock BeginTry(ILabel catchLabel, ILabel finallyLabel);
    void EndTry(ILabel endLabel);
    void EndTryFinally();
    void EndFinally();

    // Raw opcode fallback (NeoVM-specific ops during incremental migration)
    void EmitRaw(byte opcode, byte[]? operand = null);
}
