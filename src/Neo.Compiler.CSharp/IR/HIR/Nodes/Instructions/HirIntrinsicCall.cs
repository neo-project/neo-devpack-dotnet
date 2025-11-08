using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirIntrinsicCall : HirInst
{
    public HirIntrinsicCall(
        string category,
        string name,
        IReadOnlyList<HirValue> arguments,
        HirIntrinsicMetadata metadata)
        : base(metadata.ReturnType)
    {
        Category = category ?? throw new ArgumentNullException(nameof(category));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        Metadata = metadata;
        if (metadata.RequiresMemoryToken)
        {
            ConsumesMemoryToken = true;
            ProducesMemoryToken = true;
        }
    }

    public string Category { get; }
    public string Name { get; }
    public IReadOnlyList<HirValue> Arguments { get; }
    public HirIntrinsicMetadata Metadata { get; }
    public override HirEffect Effect => Metadata.Effect;
}
