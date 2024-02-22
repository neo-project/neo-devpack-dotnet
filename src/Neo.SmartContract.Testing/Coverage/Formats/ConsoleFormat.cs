using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage.Formats
{
    public partial class ConsoleFormat : ICoverageFormat
    {
        /// <summary>
        /// Contract
        /// </summary>
        public CoveredContract Contract { get; }

        /// <summary>
        /// Selective methods
        /// </summary>
        public CoveredMethod[] Methods { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract">Contract</param>
        /// <param name="methods">Methods</param>
        public ConsoleFormat(CoveredContract contract, params CoveredMethod[] methods)
        {
            Contract = contract;
            Methods = methods;
        }

        public void WriteReport(Action<string, Action<Stream>> writeAttachement)
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
            var coverLines = $"{Contract.CoveredLinesPercentage:P2}";
            var coverBranch = $"{Contract.CoveredBranchPercentage:P2}";
            writer.WriteLine($"{Contract.Hash} [{coverLines} - {coverBranch}]");

            List<string[]> rows = new();
            var max = new int[] { "Method".Length, "Line  ".Length, "Branch".Length };

            foreach (var method in Methods.OrderBy(u => u.Method.Name).OrderByDescending(u => u.CoveredLinesPercentage))
            {
                coverLines = $"{method.CoveredLinesPercentage:P2}";
                coverBranch = $"{method.CoveredBranchPercentage:P2}";
                rows.Add(new string[] { method.Method.ToString(), coverLines, coverBranch });

                max[0] = Math.Max(method.Method.ToString().Length, max[0]);
                max[1] = Math.Max(coverLines.Length, max[1]);
                max[2] = Math.Max(coverLines.Length, max[2]);
            }

            writer.WriteLine($"┌-{"─".PadLeft(max[0], '─')}-┬-{"─".PadLeft(max[1], '─')}-┬-{"─".PadLeft(max[1], '─')}-┐");
            writer.WriteLine($"│ {string.Format($"{{0,-{max[0]}}}", "Method", max[0])} │ {string.Format($"{{0,{max[1]}}}", "Line  ", max[1])} │ {string.Format($"{{0,{max[2]}}}", "Branch", max[1])} │");
            writer.WriteLine($"├-{"─".PadLeft(max[0], '─')}-┼-{"─".PadLeft(max[1], '─')}-┼-{"─".PadLeft(max[1], '─')}-┤");

            foreach (var print in rows)
            {
                writer.WriteLine($"│ {string.Format($"{{0,-{max[0]}}}", print[0], max[0])} │ {string.Format($"{{0,{max[1]}}}", print[1], max[1])} │ {string.Format($"{{0,{max[1]}}}", print[2], max[2])} │");
            }

            writer.WriteLine($"└-{"─".PadLeft(max[0], '─')}-┴-{"─".PadLeft(max[1], '─')}-┴-{"─".PadLeft(max[2], '─')}-┘");
        }
    }
}
