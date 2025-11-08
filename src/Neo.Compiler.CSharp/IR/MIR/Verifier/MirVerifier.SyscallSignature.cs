using System.Collections.Generic;

namespace Neo.Compiler.MIR;

internal sealed partial class MirVerifier
{
    private sealed record SyscallSignature(
        MirEffect Effect,
        MirType ReturnType,
        IReadOnlyList<MirType> Parameters,
        bool Deterministic = true);
}

