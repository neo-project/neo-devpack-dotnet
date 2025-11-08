using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirGuardNull : MirInst
{
    internal MirGuardNull(MirValue reference, MirGuardFail failBehaviour, MirBlock? failTarget = null)
        : base(MirType.TVoid)
    {
        Reference = reference ?? throw new ArgumentNullException(nameof(reference));
        Fail = failBehaviour;
        FailTarget = failTarget;
    }

    internal MirValue Reference { get; }
    internal MirGuardFail Fail { get; }
    internal MirBlock? FailTarget { get; }
}
