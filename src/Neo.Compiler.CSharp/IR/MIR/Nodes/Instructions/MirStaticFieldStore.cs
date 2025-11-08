using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirStaticFieldStore : MirInst
{
    internal MirStaticFieldStore(byte slot, MirValue value, MirType fieldType, string fieldName)
        : base(MirType.TVoid)
    {
        Slot = slot;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        FieldType = fieldType ?? throw new ArgumentNullException(nameof(fieldType));
        FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
    }

    internal byte Slot { get; }
    internal MirValue Value { get; }
    internal MirType FieldType { get; }
    internal string FieldName { get; }
    internal override MirEffect Effect => MirEffect.Runtime;
}
