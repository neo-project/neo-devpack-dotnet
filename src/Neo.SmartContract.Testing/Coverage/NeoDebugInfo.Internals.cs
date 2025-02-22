// Copyright (C) 2015-2025 The Neo Project.
//
// NeoDebugInfo.Internals.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;
using System.Diagnostics;

namespace Neo.SmartContract.Testing.Coverage
{
    public partial class NeoDebugInfo
    {
        [DebuggerDisplay("Name={Name}")]
        public readonly struct Method(string id, string @namespace, string name, (int, int) range,
            IReadOnlyList<NeoDebugInfo.Parameter> parameters, IReadOnlyList<NeoDebugInfo.SequencePoint> sequencePoints)
        {
            public readonly string Id = id;
            public readonly string Namespace = @namespace;
            public readonly string Name = name;
            public readonly (int Start, int End) Range = range;
            public readonly IReadOnlyList<Parameter> Parameters = parameters;
            public readonly IReadOnlyList<SequencePoint> SequencePoints = sequencePoints;
        }

        [DebuggerDisplay("Name={Name}, Type={Type}")]
        public readonly struct Parameter(string name, string type, int index)
        {
            public readonly string Name = name;
            public readonly string Type = type;
            public readonly int Index = index;
        }

        [DebuggerDisplay("Document={Document}, Address={Address}")]
        public readonly struct SequencePoint(int address, int document, (int, int) start, (int, int) end)
        {
            public readonly int Address = address;
            public readonly int Document = document;
            public readonly (int Line, int Column) Start = start;
            public readonly (int Line, int Column) End = end;
        }
    }
}
