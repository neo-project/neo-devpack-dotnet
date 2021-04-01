using Microsoft.CodeAnalysis;
using System;

namespace Neo.Compiler
{
    class CompilationException : Exception
    {
        public Diagnostic Diagnostic { get; }

        private CompilationException(Location? location, string id, string message)
        {
            Diagnostic = Diagnostic.Create(id, DiagnosticCategory.Default, message, DiagnosticSeverity.Error, DiagnosticSeverity.Error, true, 0, location: location);
        }

        public CompilationException(string id, string message)
            : this((Location?)null, id, message)
        {
        }

        public CompilationException(SyntaxNodeOrToken syntax, string id, string message)
            : this(syntax.GetLocation(), id, message)
        {
        }

        public CompilationException(ISymbol symbol, string id, string message)
            : this(symbol.Locations.IsEmpty ? null : symbol.Locations[0], id, message)
        {
        }
    }
}
