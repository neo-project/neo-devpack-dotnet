using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VSyscall : VNode
{
    internal VSyscall(uint sysId, IReadOnlyList<VNode> args, LirType returnType)
        : base(returnType)
    {
        SysId = sysId;
        Arguments = args;
    }

    internal uint SysId { get; }
    internal IReadOnlyList<VNode> Arguments { get; }
}
