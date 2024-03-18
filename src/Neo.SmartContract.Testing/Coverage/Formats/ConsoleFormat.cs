using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage.Formats
{
    public partial class ConsoleFormat : CoverageFormatBase
    {
        /// <summary>
        /// Entries
        /// </summary>
        public (CoveredContract Contract, Func<CoveredMethod, bool>? Filter)[] Entries { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract">Contract</param>
        /// <param name="filter">Method Filter</param>
        public ConsoleFormat(CoveredContract contract, Func<CoveredMethod, bool>? filter = null)
        {
            Entries = new[] { (contract, filter) };
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entries">Entries</param>
        public ConsoleFormat(IEnumerable<(CoveredContract, Func<CoveredMethod, bool>?)> entries)
        {
            Entries = entries.ToArray();
        }

        public override void WriteReport(Action<string, Action<Stream>> writeAttachement)
        {
            writeAttachement("coverage.cobertura.txt", stream =>
            {
                using var writer = new StreamWriter(stream)
                {
                    NewLine = "\n"
                };
                WriteReport(writer);
                writer.Flush();
            });
        }

        private void WriteReport(StreamWriter writer)
        {
            foreach ((var contract, var methods) in Entries)
            {
                var coverLines = contract.CoveredLinesPercentage.ToString("P2", CultureInfo.InvariantCulture);
                var coverBranch = contract.CoveredBranchPercentage.ToString("P2", CultureInfo.InvariantCulture);
                writer.WriteLine($"{contract.Name} [{coverLines} - {coverBranch}]");

                List<string[]> rows = new();
                var max = new int[] { "Method".Length, "Line  ".Length, "Branch".Length };

                foreach (var method in contract.Methods
                    .Where(u => methods is null || methods.Invoke(u))
                    .OrderBy(u => u.Method.Name)
                    .OrderByDescending(u => u.CoveredLinesPercentage))
                {
                    coverLines = method.CoveredLinesPercentage.ToString("P2", CultureInfo.InvariantCulture);
                    coverBranch = method.CoveredBranchPercentage.ToString("P2", CultureInfo.InvariantCulture);
                    rows.Add(new string[] { method.Method.ToString(), coverLines, coverBranch });

                    max[0] = Math.Max(method.Method.ToString().Length, max[0]);
                    max[1] = Math.Max(coverLines.Length, max[1]);
                    max[2] = Math.Max(coverLines.Length, max[2]);
                }

                writer.WriteLine($"┌-{"─".PadLeft(max[0], '─')}-┬-{"─".PadLeft(max[1], '─')}-┬-{"─".PadLeft(max[2], '─')}-┐");
                writer.WriteLine($"│ {string.Format($"{{0,-{max[0]}}}", "Method", max[0])} │ {string.Format($"{{0,{max[1]}}}", "Line  ", max[1])} │ {string.Format($"{{0,{max[2]}}}", "Branch", max[2])} │");
                writer.WriteLine($"├-{"─".PadLeft(max[0], '─')}-┼-{"─".PadLeft(max[1], '─')}-┼-{"─".PadLeft(max[2], '─')}-┤");

                foreach (var print in rows)
                {
                    writer.WriteLine($"│ {string.Format($"{{0,-{max[0]}}}", print[0], max[0])} │ {string.Format($"{{0,{max[1]}}}", print[1], max[1])} │ {string.Format($"{{0,{max[2]}}}", print[2], max[2])} │");
                }

                writer.WriteLine($"└-{"─".PadLeft(max[0], '─')}-┴-{"─".PadLeft(max[1], '─')}-┴-{"─".PadLeft(max[2], '─')}-┘");
            }
        }
    }
}
