using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Neo.Compiler.HIR;
using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private void RecordHirBinary(OpCode opcode)
    {
        Debug.Assert(!IsHirEnabled, "Legacy HIR recorders should not be hit when the HIR pipeline is enabled.");
    }

    private void RecordHirLoadArgument(IParameterSymbol parameter)
    {
        Debug.Assert(!IsHirEnabled, "Legacy HIR recorders should not be hit when the HIR pipeline is enabled.");
    }

    private void RecordHirStoreArgument(IParameterSymbol parameter)
    {
        Debug.Assert(!IsHirEnabled, "Legacy HIR recorders should not be hit when the HIR pipeline is enabled.");
    }

    private void RecordHirLoadLocal(ILocalSymbol symbol)
    {
        Debug.Assert(!IsHirEnabled, "Legacy HIR recorders should not be hit when the HIR pipeline is enabled.");
    }

    private void RecordHirStoreLocal(ILocalSymbol symbol)
    {
        Debug.Assert(!IsHirEnabled, "Legacy HIR recorders should not be hit when the HIR pipeline is enabled.");
    }

    private void RecordHirLiteral(object? value)
    {
        Debug.Assert(!IsHirEnabled, "Legacy HIR recorders should not be hit when the HIR pipeline is enabled.");
    }

    private void RecordHirInvocation(IMethodSymbol method, HirValue? receiver, IReadOnlyList<HirValue> arguments)
    {
        Debug.Assert(!IsHirEnabled, "Legacy HIR recorders should not be hit when the HIR pipeline is enabled.");
    }

    private void RecordHirReturn(HirValue? value = null)
    {
        Debug.Assert(!IsHirEnabled, "Legacy HIR recorders should not be hit when the HIR pipeline is enabled.");
    }
}
