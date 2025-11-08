using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VPointerCall : VNode
{
    internal VPointerCall(VNode? pointer, IReadOnlyList<VNode> arguments, LirType returnType, bool isPure, bool isTailCall, byte? callTableIndex)
        : base(returnType)
    {
        Pointer = pointer;
        Arguments = arguments;
        IsPure = isPure;
        IsTailCall = isTailCall;
        CallTableIndex = callTableIndex;

        if (Pointer is null && CallTableIndex is null)
            throw new ArgumentException("Pointer calls require either a pointer operand or a call-table index.", nameof(pointer));
    }

    internal VNode? Pointer { get; }
    internal IReadOnlyList<VNode> Arguments { get; }
    internal bool IsPure { get; }
    internal bool IsTailCall { get; }
    internal byte? CallTableIndex { get; }
}
