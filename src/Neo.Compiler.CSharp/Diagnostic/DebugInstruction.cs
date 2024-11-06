using System.Runtime.CompilerServices;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    void InstructionDebug([CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
    {
        Push("DEBUG: at line " + lineNumber + " (" + caller + ")");
        AddInstruction(OpCode.DROP);
    }
}
