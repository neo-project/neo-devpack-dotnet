using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirLoadStaticField : HirInst
{
    public HirLoadStaticField(byte slot, HirType fieldType, string fieldName)
        : base(fieldType ?? throw new ArgumentNullException(nameof(fieldType)))
    {
        Slot = slot;
        FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
        ConsumesMemoryToken = true;
        ProducesMemoryToken = true;
    }

    public byte Slot { get; }
    public string FieldName { get; }
    public override HirEffect Effect => HirEffect.Runtime;
}
