using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirCheckedAdd : MirInst
{
    internal MirCheckedAdd(MirValue left, MirValue right, MirType resultType, MirGuardFail failBehaviour, MirBlock? failTarget = null)
        : base(resultType)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
        Fail = failBehaviour;
        FailTarget = failTarget;
    }

    internal MirValue Left { get; }
    internal MirValue Right { get; }
    internal MirGuardFail Fail { get; }
    internal MirBlock? FailTarget { get; }
}
