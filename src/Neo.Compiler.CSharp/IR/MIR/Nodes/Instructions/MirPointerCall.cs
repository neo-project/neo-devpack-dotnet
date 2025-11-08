using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirPointerCall : MirInst
{
    internal MirPointerCall(MirValue? pointer, IReadOnlyList<MirValue> arguments, MirType returnType, bool isPure = false, bool isTailCall = false, byte? callTableIndex = null)
        : base(returnType)
    {
        Pointer = pointer;
        Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        IsPure = isPure;
        IsTailCall = isTailCall;
        CallTableIndex = callTableIndex;

        if (Pointer is null && CallTableIndex is null)
            throw new ArgumentException("Pointer calls require either a pointer operand or a call-table index.", nameof(pointer));
    }

    internal MirValue? Pointer { get; }
    internal IReadOnlyList<MirValue> Arguments { get; }
    internal bool IsPure { get; }
    internal bool IsTailCall { get; }
    internal byte? CallTableIndex { get; }
    internal override MirEffect Effect => IsPure ? MirEffect.None : MirEffect.Memory;
}
