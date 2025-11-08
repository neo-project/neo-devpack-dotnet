using System;

namespace Neo.Compiler.HIR;

internal sealed class HirAbortMessage : HirTerminator
{
    public HirAbortMessage(HirValue message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    public HirValue Message { get; }
}
