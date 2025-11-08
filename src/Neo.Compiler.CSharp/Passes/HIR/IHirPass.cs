namespace Neo.Compiler.HIR.Optimization;

internal interface IHirPass
{
    /// <summary>
    /// Runs the pass and returns true if the IR was modified.
    /// </summary>
    bool Run(HirFunction function);
}
