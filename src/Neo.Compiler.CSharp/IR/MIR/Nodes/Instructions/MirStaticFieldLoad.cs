using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Neo.Compiler.HIR;

namespace Neo.Compiler.MIR;

internal sealed class MirStaticFieldLoad : MirInst
{
    internal MirStaticFieldLoad(byte slot, MirType fieldType, string fieldName)
        : base(fieldType ?? throw new ArgumentNullException(nameof(fieldType)))
    {
        Slot = slot;
        FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
    }

    internal byte Slot { get; }
    internal string FieldName { get; }
    internal override MirEffect Effect => MirEffect.Runtime;
}
