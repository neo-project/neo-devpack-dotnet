namespace Neo.Compiler.MIR.Optimization;

internal interface IMirPass
{
    bool Run(MirFunction function);
}
