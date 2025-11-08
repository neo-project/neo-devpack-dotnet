using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirCall : MirInst
{
    internal MirCall(string callee, IReadOnlyList<MirValue> arguments, MirType returnType, bool isPure = false, bool isTailCall = false)
        : base(returnType)
    {
        Callee = callee ?? throw new ArgumentNullException(nameof(callee));
        Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        IsPure = isPure;
        IsTailCall = isTailCall;
    }

    internal string Callee { get; }
    internal IReadOnlyList<MirValue> Arguments { get; }
    internal bool IsPure { get; }
    internal bool IsTailCall { get; }
    internal override MirEffect Effect => IsPure ? MirEffect.None : MirEffect.Memory;
}
