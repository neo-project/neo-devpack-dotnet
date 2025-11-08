using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirCall : HirInst
{
    public HirCall(string callee, IReadOnlyList<HirValue> arguments, HirType returnType, bool isStatic, HirCallSemantics semantics = HirCallSemantics.Effectful)
        : base(returnType)
    {
        Callee = callee ?? throw new ArgumentNullException(nameof(callee));
        Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        IsStatic = isStatic;
        Semantics = semantics;
        if (semantics != HirCallSemantics.Pure)
        {
            ConsumesMemoryToken = true;
            ProducesMemoryToken = true;
        }
    }

    public string Callee { get; }
    public IReadOnlyList<HirValue> Arguments { get; }
    public bool IsStatic { get; }
    public HirCallSemantics Semantics { get; }
    public override HirEffect Effect => Semantics switch
    {
        HirCallSemantics.Pure => HirEffect.None,
        HirCallSemantics.ReadOnly => HirEffect.StorageRead,
        _ => HirEffect.Memory
    };
}
