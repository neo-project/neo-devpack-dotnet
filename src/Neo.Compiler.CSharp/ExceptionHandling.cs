using Microsoft.CodeAnalysis;
using Neo.VM;
using System.Collections.Generic;

namespace Neo.Compiler
{
    class ExceptionHandling
    {
        public ExceptionHandlingState State;
        public HashSet<ILabelSymbol> Labels = new();
        public List<Instruction> PendingGotoStatments = new();
        public int SwitchCount = 0;
        public int ContinueTargetCount = 0;
        public int BreakTargetCount = 0;
    }
}
