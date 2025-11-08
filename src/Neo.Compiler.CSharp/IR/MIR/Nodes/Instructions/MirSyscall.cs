using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirSyscall : MirInst
{
    internal MirSyscall(string category, string name, IReadOnlyList<MirValue> arguments, MirType returnType, MirEffect effect, ulong? gasHint = null)
        : base(returnType)
    {
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        GasHint = gasHint;
        EffectOverride = effect;
    }

    internal string Category { get; }
    internal string Name { get; }
    internal IReadOnlyList<MirValue> Arguments { get; }
    internal ulong? GasHint { get; }
    internal MirEffect EffectOverride { get; }
    internal override MirEffect Effect => EffectOverride;
}
