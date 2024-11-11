// Copyright (C) 2015-2024 The Neo Project.
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
        private readonly LocationInformation? _location;
        private readonly int _position;

        public SequencePointInserter(IReadOnlyList<Instruction> instructions, SyntaxNodeOrToken? syntax = null, CompilerLocation? compilerOrigin = null) :
            this(instructions, new LocationInformation(syntax?.GetLocation(), compilerOrigin))
        { }

        public SequencePointInserter(IReadOnlyList<Instruction> instructions, SyntaxReference? syntax = null, CompilerLocation? compilerOrigin = null) :
           this(instructions, new LocationInformation(syntax?.SyntaxTree.GetLocation(syntax.Span), compilerOrigin))
        { }

        public SequencePointInserter(IReadOnlyList<Instruction> instructions, Location? location = null, CompilerLocation? compilerOrigin = null) :
           this(instructions, new LocationInformation(location, compilerOrigin))
        { }

        public SequencePointInserter(IReadOnlyList<Instruction> instructions, LocationInformation? location = null)
        {
            _instructions = instructions;
            _location = location;
            _position = instructions.Count;

            // No location must be removed

            if (_location?.Source?.Location.SourceTree is null)
                _location = null;
        }

        void IDisposable.Dispose()
        {
            if (_location == null) return;

            for (int x = _position; x < _instructions.Count; x++)
            {
                if (_instructions[x].Location is null)
                {
                    _instructions[x].Location = _location;
                }
            }
        }
    }
}
