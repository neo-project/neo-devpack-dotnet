// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

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
