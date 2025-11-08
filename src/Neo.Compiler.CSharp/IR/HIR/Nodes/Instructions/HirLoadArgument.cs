using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirLoadArgument : HirInst
{
    public HirLoadArgument(HirArgument argument)
        : base(argument.Type)
    {
        Argument = argument ?? throw new ArgumentNullException(nameof(argument));
    }

    public HirArgument Argument { get; }
}
