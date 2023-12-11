// Copyright (C) 2015-2023 The Neo Project.
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
        private readonly IReadOnlyList<Instruction> _instructions;
        private readonly SyntaxNodeOrToken _syntax;
        private readonly int _position;

        public SequencePointInserter(IReadOnlyList<Instruction> instructions, SyntaxNodeOrToken syntax)
        {
            _instructions = instructions;
            _syntax = syntax;
            _position = instructions.Count;
        }
        void IDisposable.Dispose()
        {
            if (_position < _instructions.Count)
                _instructions[_position].SourceLocation = _syntax.GetLocation();
        }
    }
}
