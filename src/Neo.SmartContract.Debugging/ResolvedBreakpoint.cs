// Copyright (C) 2015-2026 The Neo Project.
//
// ResolvedBreakpoint.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Debugging
{
    /// <summary>
    /// A source breakpoint resolved to a concrete instruction address.
    /// </summary>
    public readonly struct ResolvedBreakpoint
    {
        public ResolvedBreakpoint(string document, int line, int column, int address)
        {
            Document = document;
            Line = line;
            Column = column;
            Address = address;
        }

        /// <summary>The document (source file) the breakpoint bound to.</summary>
        public string Document { get; }

        /// <summary>The 1-based source line the breakpoint actually bound to.</summary>
        public int Line { get; }

        /// <summary>The 1-based source column the breakpoint bound to.</summary>
        public int Column { get; }

        /// <summary>The instruction address (offset in the contract script) to break at.</summary>
        public int Address { get; }
    }
}
