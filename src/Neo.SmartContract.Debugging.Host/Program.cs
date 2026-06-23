// Copyright (C) 2015-2026 The Neo Project.
//
// Program.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Debugging.Host
{
    /// <summary>
    /// Entry point for the Neo debug adapter. Speaks the Debug Adapter Protocol over standard
    /// input/output, as an editor expects when it launches a debug adapter.
    /// </summary>
    public static class Program
    {
        public static void Main()
        {
            using var input = Console.OpenStandardInput();
            using var output = Console.OpenStandardOutput();
            var adapter = new NeoDebugAdapter(input, output);
            adapter.Run();
        }
    }
}
