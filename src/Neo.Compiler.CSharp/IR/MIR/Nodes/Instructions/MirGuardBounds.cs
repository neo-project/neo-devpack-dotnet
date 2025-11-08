using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirGuardBounds : MirInst
{
    internal MirGuardBounds(MirValue index, MirValue length, MirGuardFail failBehaviour, MirBlock? failTarget = null)
        : base(MirType.TVoid)
    {
        Index = index ?? throw new ArgumentNullException(nameof(index));
        Length = length ?? throw new ArgumentNullException(nameof(length));
        Fail = failBehaviour;
        FailTarget = failTarget;
    }

    internal MirValue Index { get; }
    internal MirValue Length { get; }
    internal MirGuardFail Fail { get; }
    internal MirBlock? FailTarget { get; }
}
