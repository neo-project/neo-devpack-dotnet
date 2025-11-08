using System;
using System.Collections.Generic;
using System.Numerics;

namespace Neo.Compiler.HIR;

internal sealed class HirStoreStaticField : HirInst
{
    public HirStoreStaticField(byte slot, HirValue value, HirType fieldType, string fieldName)
        : base(HirType.VoidType)
    {
        Slot = slot;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        FieldType = fieldType ?? throw new ArgumentNullException(nameof(fieldType));
        FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
        ConsumesMemoryToken = true;
        ProducesMemoryToken = true;
    }

    public byte Slot { get; }
    public HirValue Value { get; }
    public HirType FieldType { get; }
    public string FieldName { get; }
    public override HirEffect Effect => HirEffect.Runtime;
}
