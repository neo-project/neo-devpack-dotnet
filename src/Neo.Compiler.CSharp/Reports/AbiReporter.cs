// Copyright (C) 2015-2026 The Neo Project.
//
// AbiReporter.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler
{
    /// <summary>
    /// Generates a static ABI and bytecode summary from a compiled NEF and manifest.
    /// </summary>
    public static class AbiReporter
    {
        private const int ColumnWidth = 26;
        private const string Separator = "------------------------------------------------------------";

        /// <summary>
        /// Prints the static ABI report to the specified <paramref name="output"/> writer.
        /// </summary>
        public static void Print(NefFile nef, ContractManifest manifest, TextWriter output)
        {
            ArgumentNullException.ThrowIfNull(nef);
            ArgumentNullException.ThrowIfNull(manifest);
            ArgumentNullException.ThrowIfNull(output);

            int scriptSize = nef.Script.Length;
            int? instructionCount = TryCountInstructions(nef.Script);
            int methodCount = manifest.Abi.Methods.Length;
            int safeMethodCount = manifest.Abi.Methods.Count(m => m.Safe);
            int eventCount = manifest.Abi.Events.Length;

            output.WriteLine();
            output.WriteLine($"Contract: {manifest.Name}");
            output.WriteLine();
            output.WriteLine("Static ABI Report");
            output.WriteLine(Separator);
            output.WriteLine($"{"Script size:",-ColumnWidth}{scriptSize} bytes");
            output.WriteLine($"{"Instruction count:",-ColumnWidth}{(instructionCount.HasValue ? instructionCount.Value.ToString() : "N/A")}");
            output.WriteLine($"{"ABI methods:",-ColumnWidth}{methodCount}");
            output.WriteLine($"{"Safe methods:",-ColumnWidth}{safeMethodCount}");
            output.WriteLine($"{"Events:",-ColumnWidth}{eventCount}");
            output.WriteLine();

            output.WriteLine($"{"Method",-ColumnWidth}{"Safe",-9}{"Return",-12}{"Parameters"}");
            output.WriteLine(Separator);

            foreach (var method in manifest.Abi.Methods.OrderBy(m => m.Name))
            {
                string safe = method.Safe ? "yes" : "no";
                string returnType = method.ReturnType.ToString();
                string parameters = method.Parameters.Length == 0
                    ? "-"
                    : string.Join(", ", method.Parameters.Select(p => p.Type.ToString()));

                output.WriteLine($"{method.Name,-ColumnWidth}{safe,-9}{returnType,-12}{parameters}");
            }

            if (manifest.Abi.Events.Length > 0)
            {
                output.WriteLine();
                output.WriteLine($"{"Event",-ColumnWidth}{"Parameters"}");
                output.WriteLine(Separator);
                foreach (var ev in manifest.Abi.Events.OrderBy(e => e.Name))
                {
                    string parameters = ev.Parameters.Length == 0
                        ? "-"
                        : string.Join(", ", ev.Parameters.Select(p => p.Type.ToString()));
                    output.WriteLine($"{ev.Name,-ColumnWidth}{parameters}");
                }
            }
            output.WriteLine();
        }

        private static int? TryCountInstructions(Script script)
        {
            try
            {
                int count = 0;
                int address = 0;
                while (address < script.Length)
                {
                    var instruction = script.GetInstruction(address);
                    address += instruction.Size;
                    count++;
                }
                return count;
            }
            catch
            {
                // Script decoding can fail for malformed bytecode (e.g. truncated operands).
                // Return null so the caller can display "N/A" rather than crashing the report.
                return null;
            }
        }
    }
}
