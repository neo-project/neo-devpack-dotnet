using System;
using Microsoft.CodeAnalysis;
using Neo.Compiler.HIR;
using Neo.Compiler.HIR.Import;
using Neo.Compiler.MiddleEnd.Lowering;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private static readonly HirLoweringPipeline s_hirLoweringPipeline = new();
    private static readonly bool s_traceHir = string.Equals(
        Environment.GetEnvironmentVariable("NEO_HIR_TRACE"),
        "1",
        StringComparison.OrdinalIgnoreCase);
    private static readonly bool s_dumpHir =
        string.Equals(Environment.GetEnvironmentVariable("NEO_HIR_DUMP"), "1", StringComparison.OrdinalIgnoreCase);

    private bool IsHirEnabled => _context.Options.EnableHir;

    private static void Trace(string message)
    {
        if (s_traceHir)
            Console.WriteLine($"[HIR] {message}");
    }

    private void RunHirPipeline(SemanticModel model)
    {
        if (!IsHirEnabled || _context.HirModule is null || _context.MirModule is null || _context.LirModule is null)
            return;

        if (Symbol.DeclaringSyntaxReferences.IsEmpty)
            return;

        if (!SymbolEqualityComparer.Default.Equals(Symbol.ContainingAssembly, _context.TargetContract.ContainingAssembly))
            return;

        Trace($"Import start: {Symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)}");
        var importer = new HirMethodImporter(_context, Symbol, model);
        var builder = importer.Import();
        Trace($"Import completed: {builder.Function.Name}");
        if (builder.Function.Entry.Terminator is null)
            return;
        if (s_dumpHir)
            DumpHir(builder.Function);
        Trace($"Lowering start: {builder.Function.Name}");
        s_hirLoweringPipeline.Run(builder, _context);
        Trace($"Lowering completed: {builder.Function.Name}");
    }

    private static void DumpHir(HirFunction function)
    {
        Console.WriteLine($"=== HIR {function.Name} ===");
        foreach (var block in function.Blocks)
        {
            Console.WriteLine($"BLOCK {block.Label}");
            foreach (var phi in block.Phis)
                Console.WriteLine($"  PHI {phi.GetType().Name} IsLocal={phi.IsLocalPhi} Local={phi.Local?.Name ?? "<null>"}");
            foreach (var inst in block.Instructions)
                Console.WriteLine($"  INST {inst.GetType().Name}");
            Console.WriteLine($"  TERM {block.Terminator?.GetType().Name ?? "<null>"}");
        }
    }
}
