namespace Neo.Compiler.LIR.Optimization;

internal interface ILirPass
{
    bool Run(LirFunction function);
}
