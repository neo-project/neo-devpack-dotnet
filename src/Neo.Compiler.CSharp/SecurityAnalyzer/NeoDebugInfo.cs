// Copyright (C) 2015-2026 The Neo Project.
//
// NeoDebugInfo.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Neo.Compiler.SecurityAnalyzer
{
    internal sealed class NeoDebugInfo
    {
        private static readonly Regex SequencePointRegex = new(@"^(\d+)\[(-?\d+)\](\d+)\:(\d+)\-(\d+)\:(\d+)$");

        internal readonly IReadOnlyList<string> Documents;
        internal readonly IReadOnlyList<Method> Methods;

        internal NeoDebugInfo(IReadOnlyList<string> documents, IReadOnlyList<Method> methods)
        {
            Documents = documents;
            Methods = methods;
        }

        internal sealed class SourceLocation
        {
            public string FileName { get; init; } = string.Empty;
            public int Line { get; init; }
            public int Column { get; init; }
            public string? CodeSnippet { get; init; }
        }

        internal readonly struct Method
        {
            public readonly (int Start, int End) Range;
            public readonly IReadOnlyList<SequencePoint> SequencePoints;

            public Method((int Start, int End) range, IReadOnlyList<SequencePoint> sequencePoints)
            {
                Range = range;
                SequencePoints = sequencePoints;
            }
        }

        internal readonly struct SequencePoint
        {
            public readonly int Address;
            public readonly int Document;
            public readonly (int Line, int Column) Start;

            public SequencePoint(int address, int document, (int Line, int Column) start)
            {
                Address = address;
                Document = document;
                Start = start;
            }
        }

        internal static NeoDebugInfo FromDebugInfoJson(JObject json)
        {
            if (json["documents"] is not JArray jDocs)
                throw new ArgumentNullException("documents must be an array");

            if (json["methods"] is not JArray jMethods)
                throw new ArgumentNullException("methods must be an array");

            var documents = jDocs.Select(doc => doc?.GetString()!)
                .Where(doc => doc is not null)
                .ToList();
            var methods = jMethods.Select(method => MethodFromJson(method as JObject)).ToList();

            return new NeoDebugInfo(documents, methods);
        }

        private static Method MethodFromJson(JObject? json)
        {
            if (json is null)
                throw new ArgumentNullException("Method can't be null");

            if (json["sequence-points"] is not JArray jSequence)
                throw new ArgumentNullException("sequence-points must be an array");

            var range = RangeFromJson(json["range"]?.GetString() ?? throw new ArgumentNullException("method.range can't be null"));
            var sequencePoints = jSequence.Select(sequence => SequencePointFromJson(sequence?.GetString())).ToList();

            return new Method(range, sequencePoints);
        }

        private static (int Start, int End) RangeFromJson(string range)
        {
            var values = range.Split('-');
            return values.Length == 2
                ? (int.Parse(values[0]), int.Parse(values[1]))
                : throw new FormatException($"Invalid range '{range}'");
        }

        private static SequencePoint SequencePointFromJson(string? sequence)
        {
            if (sequence is null)
                throw new ArgumentNullException("Sequence point can't be null");

            Match match = SequencePointRegex.Match(sequence);
            if (match.Groups.Count != 7)
                throw new FormatException($"Invalid Sequence Point '{sequence}'");

            int address = int.Parse(match.Groups[1].Value);
            int document = int.Parse(match.Groups[2].Value);
            var start = (int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));

            return new SequencePoint(address, document, start);
        }
    }
}
