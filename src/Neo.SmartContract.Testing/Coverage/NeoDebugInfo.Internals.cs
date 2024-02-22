using System.Collections.Generic;
using System.Diagnostics;

namespace Neo.SmartContract.Testing.Coverage
{
    public partial class NeoDebugInfo
    {
        [DebuggerDisplay("Name={Name}")]
        public struct Method
        {
            public readonly string Id;
            public readonly string Namespace;
            public readonly string Name;
            public readonly (int Start, int End) Range;
            public readonly IReadOnlyList<Parameter> Parameters;
            public readonly IReadOnlyList<SequencePoint> SequencePoints;

            public Method(string id, string @namespace, string name, (int, int) range, IReadOnlyList<Parameter> parameters, IReadOnlyList<SequencePoint> sequencePoints)
            {
                Id = id;
                Namespace = @namespace;
                Name = name;
                Range = range;
                Parameters = parameters;
                SequencePoints = sequencePoints;
            }
        }

        [DebuggerDisplay("Name={Name}, Type={Type}")]
        public struct Parameter
        {
            public readonly string Name;
            public readonly string Type;
            public readonly int Index;

            public Parameter(string name, string type, int index)
            {
                Name = name;
                Type = type;
                Index = index;
            }
        }

        [DebuggerDisplay("Document={Document}, Address={Address}")]
        public struct SequencePoint
        {
            public readonly int Address;
            public readonly int Document;
            public readonly (int Line, int Column) Start;
            public readonly (int Line, int Column) End;

            public SequencePoint(int address, int document, (int, int) start, (int, int) end)
            {
                Address = address;
                Document = document;
                Start = start;
                End = end;
            }
        }
    }
}
