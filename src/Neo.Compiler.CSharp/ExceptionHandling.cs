using Neo.VM;

namespace Neo.Compiler
{
    class ExceptionHandling
    {
        public ExceptionHandlingState State;
        public int ContinueTargetCount = 0;
        public int BreakTargetCount = 0;
    }
}
