using System;

namespace Neo.Compiler.LIR;

internal sealed class VAbortMsg : VTerminator
{
    internal VAbortMsg(VNode message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    internal VNode Message { get; }
}
