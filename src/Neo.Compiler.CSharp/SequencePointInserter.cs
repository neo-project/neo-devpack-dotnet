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
