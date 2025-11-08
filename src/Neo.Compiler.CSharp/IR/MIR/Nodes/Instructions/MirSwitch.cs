using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirSwitch : MirTerminator
{
    internal MirSwitch(MirValue key, IReadOnlyList<(BigInteger Case, MirBlock Target)> cases, MirBlock defaultTarget)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        if (cases is null)
            throw new ArgumentNullException(nameof(cases));
        Cases = cases is List<(BigInteger Case, MirBlock Target)> list ? list.ToArray() : cases.ToArray();
        DefaultTarget = defaultTarget ?? throw new ArgumentNullException(nameof(defaultTarget));
    }

    internal MirValue Key { get; }
    internal IReadOnlyList<(BigInteger Case, MirBlock Target)> Cases { get; }
    internal MirBlock DefaultTarget { get; }
}
