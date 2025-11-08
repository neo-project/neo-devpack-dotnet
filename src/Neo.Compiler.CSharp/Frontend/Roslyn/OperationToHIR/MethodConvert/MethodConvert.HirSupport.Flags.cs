using System;
using Microsoft.CodeAnalysis;
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
        Trace($"Lowering start: {builder.Function.Name}");
        s_hirLoweringPipeline.Run(builder, _context);
        Trace($"Lowering completed: {builder.Function.Name}");
    }
}
