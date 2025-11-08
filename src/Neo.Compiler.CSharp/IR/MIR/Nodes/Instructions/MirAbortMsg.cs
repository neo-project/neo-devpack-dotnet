using System;

namespace Neo.Compiler.MIR;

internal sealed class MirAbortMsg : MirTerminator
{
    internal MirAbortMsg(MirValue message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    internal MirValue Message { get; }
}
