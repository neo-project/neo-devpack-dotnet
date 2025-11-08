using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.LIR;


internal sealed class VRet : VTerminator
{
    internal VRet(VNode? value) => Value = value;

    internal VNode? Value { get; }
}
