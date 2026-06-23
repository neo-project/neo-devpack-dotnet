// Copyright (C) 2015-2026 The Neo Project.
//
// BreakpointResolver.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Testing.Coverage;
using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.SmartContract.Debugging
{
    /// <summary>
    /// Resolves a source location (file and line) to the instruction address a debugger should
    /// break at, using a contract's <see cref="NeoDebugInfo"/> source map. This is the inverse of
    /// <see cref="NeoDebugInfo.GetSourceLocation(int)"/> and is the primitive a Debug Adapter uses
    /// to bind source breakpoints.
    /// </summary>
    public static class BreakpointResolver
    {
        /// <summary>
        /// Resolves a requested breakpoint at <paramref name="sourcePath"/>:<paramref name="line"/>
        /// to the first instruction of the nearest executable line at or after the requested line.
        /// </summary>
        /// <param name="debugInfo">The contract's debug information (source map).</param>
        /// <param name="sourcePath">The source file the breakpoint was set in. Matched against the
        /// debug info documents by full path first, then by file name.</param>
        /// <param name="line">The requested 1-based source line.</param>
        /// <returns>
        /// The resolved breakpoint — including the line/column it actually bound to, which may be
        /// after the requested line when that line has no executable code — or <see langword="null"/>
        /// when the file is unknown or no executable code exists at or after the requested line.
        /// </returns>
        public static ResolvedBreakpoint? ResolveBreakpoint(this NeoDebugInfo debugInfo, string sourcePath, int line)
        {
            if (debugInfo is null) throw new ArgumentNullException(nameof(debugInfo));
            if (sourcePath is null) throw new ArgumentNullException(nameof(sourcePath));

            HashSet<int> documents = MatchDocuments(debugInfo.Documents, sourcePath);
            if (documents.Count == 0)
                return null;

            // Bind to the nearest executable line at or after the requested line; among the
            // sequence points on that line, choose the one at the lowest instruction address (the
            // first instruction of the line).
            bool found = false;
            NeoDebugInfo.SequencePoint best = default;
            foreach (NeoDebugInfo.Method method in debugInfo.Methods)
            {
                foreach (NeoDebugInfo.SequencePoint sp in method.SequencePoints)
                {
                    if (sp.Document < 0 || !documents.Contains(sp.Document))
                        continue;
                    if (sp.Start.Line < line)
                        continue;

                    if (!found ||
                        sp.Start.Line < best.Start.Line ||
                        (sp.Start.Line == best.Start.Line && sp.Address < best.Address))
                    {
                        best = sp;
                        found = true;
                    }
                }
            }

            if (!found)
                return null;

            return new ResolvedBreakpoint(debugInfo.Documents[best.Document], best.Start.Line, best.Start.Column, best.Address);
        }

        private static HashSet<int> MatchDocuments(IReadOnlyList<string> documents, string sourcePath)
        {
            HashSet<int> result = new();

            // Prefer an exact (separator- and case-insensitive) full-path match.
            string normalized = NormalizePath(sourcePath);
            for (int i = 0; i < documents.Count; i++)
            {
                if (NormalizePath(documents[i]) == normalized)
                    result.Add(i);
            }
            if (result.Count > 0)
                return result;

            // Fall back to a file-name match so a breakpoint still binds when the debugger and the
            // debug info disagree on the absolute path (relocated sources, different roots).
            string fileName = Path.GetFileName(sourcePath);
            for (int i = 0; i < documents.Count; i++)
            {
                if (string.Equals(Path.GetFileName(documents[i]), fileName, StringComparison.OrdinalIgnoreCase))
                    result.Add(i);
            }

            return result;
        }

        private static string NormalizePath(string path)
            => path.Replace('\\', '/').ToLowerInvariant();
    }
}
