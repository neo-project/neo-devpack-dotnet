using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace Neo.Compiler
{
    class SequencePointInserter : IDisposable
    {
        private readonly IReadOnlyList<Instruction> instructions;
        private readonly SyntaxNodeOrToken syntax;
        private readonly int position;

        public SequencePointInserter(IReadOnlyList<Instruction> instructions, SyntaxNodeOrToken syntax)
        {
            this.instructions = instructions;
            this.syntax = syntax;
            this.position = instructions.Count;
        }
        void IDisposable.Dispose()
        {
            if (position < instructions.Count)
                instructions[position].SourceLocation = syntax.GetLocation();
        }
    }
}
