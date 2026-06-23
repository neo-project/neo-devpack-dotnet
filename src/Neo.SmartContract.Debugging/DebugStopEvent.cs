// Copyright (C) 2015-2026 The Neo Project.
//
// DebugStopEvent.cs file belongs to the neo project and is free
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
    /// Describes where a <see cref="DebugSession"/> paused execution.
    /// </summary>
    public sealed class DebugStopEvent
    {
        public DebugStopEvent(string file, int line, int column, int address)
        {
            File = file;
            Line = line;
            Column = column;
            Address = address;
        }

        /// <summary>The source file execution paused in.</summary>
        public string File { get; }

        /// <summary>The 1-based source line execution paused at.</summary>
        public int Line { get; }

        /// <summary>The 1-based source column execution paused at.</summary>
        public int Column { get; }

        /// <summary>The instruction address execution paused at.</summary>
        public int Address { get; }
    }
}
