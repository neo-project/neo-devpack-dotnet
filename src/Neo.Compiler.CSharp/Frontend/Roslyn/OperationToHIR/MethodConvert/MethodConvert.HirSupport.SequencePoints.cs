using System.Diagnostics;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private void MarkHirSequencePoint(LocationInformation? location)
    {
        Debug.Assert(!IsHirEnabled, "Legacy HIR sequence point tracking should not be hit when the HIR pipeline is enabled.");
    }

    private void PopHirSequencePoint()
    {
        Debug.Assert(!IsHirEnabled, "Legacy HIR sequence point tracking should not be hit when the HIR pipeline is enabled.");
    }
}
