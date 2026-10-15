// Copyright (C) 2015-2026 The Neo Project.
//
// CoberturaFormat.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Neo.SmartContract.Testing.Coverage.Formats
{
    public partial class CoberturaFormat : CoverageFormatBase
    {
        /// <summary>
        /// Contract
        /// </summary>
        public IReadOnlyList<(CoveredContract Contract, NeoDebugInfo DebugInfo)> Contracts { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contracts">Contracts</param>
        public CoberturaFormat(params (CoveredContract Contract, NeoDebugInfo DebugInfo)[] contracts)
        {
            Contracts = contracts;
        }

        public override void WriteReport(Action<string, Action<Stream>> writeAttachement)
        {
            writeAttachement("coverage.cobertura.xml", stream =>
            {
                StreamWriter textWriter = new(stream);
                XmlTextWriter xmlWriter = new(textWriter) { Formatting = Formatting.Indented };
                WriteReport(xmlWriter, Contracts);
                xmlWriter.Flush();
                textWriter.Flush();
            });
        }

        internal static void WriteReport(XmlWriter writer, IReadOnlyList<(CoveredContract Contract, NeoDebugInfo DebugInfo)> coverage)
        {
            int linesValid = 0, linesCovered = 0;
            int branchesValid = 0, branchesCovered = 0;

            foreach (var entry in coverage)
            {
                var allPoints = GetPoints(entry.DebugInfo).ToArray();

                var (lineCount, hitCount) = GetLineRate(entry.Contract, allPoints.Select(point => point.Point));
                linesValid += lineCount;
                linesCovered += hitCount;

                var (branchCount, branchHit) = GetBranchRate(entry.Contract, allPoints);
                branchesValid += branchCount;
                branchesCovered += branchHit;
            }

            var lineRate = CoverageBase.CalculateHitRate(linesValid, linesCovered);
            var branchRate = CoverageBase.CalculateHitRate(branchesValid, branchesCovered);

            writer.WriteStartDocument();
            writer.WriteStartElement("coverage");
            writer.WriteAttributeString("line-rate", lineRate.ToString("F4", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("lines-covered", $"{linesCovered}");
            writer.WriteAttributeString("lines-valid", $"{linesValid}");
            writer.WriteAttributeString("branch-rate", branchRate.ToString("F4", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("branches-covered", $"{branchesCovered}");
            writer.WriteAttributeString("branches-valid", $"{branchesValid}");
            writer.WriteAttributeString("version", typeof(CoberturaFormat).Assembly.GetName().Version?.ToString() ?? "Unknown Version");
            writer.WriteAttributeString("timestamp", $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}");

            writer.WriteStartElement("sources");
            foreach (var contract in coverage)
            {
                writer.WriteElementString("source", contract.DebugInfo.DocumentRoot);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("packages");

            foreach (var contract in coverage)
            {
                var ccWriter = new ContractCoverageWriter(contract.Contract, contract.DebugInfo);
                ccWriter.WritePackage(writer);
            }

            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private static IEnumerable<(NeoDebugInfo.Method Method, NeoDebugInfo.SequencePoint Point)> GetPoints(NeoDebugInfo info)
            => info.Methods.SelectMany(method => method.SequencePoints
                .Where(point => point.Document >= 0 && point.Document < info.Documents.Count)
                .Select(point => (method, point)));

        private static (int branchCount, int branchHit) GetBranchRate(CoveredContract contract,
            IEnumerable<(NeoDebugInfo.Method Method, NeoDebugInfo.SequencePoint Point)> points)
        {
            var branches = points.SelectMany(point => GetBranchInstructions(contract, point.Method, point.Point))
                .DistinctBy(branch => branch.address).ToArray();
            return (branches.Sum(branch => branch.branch.Count), branches.Sum(branch => branch.branch.Hits));
        }

        private static (int lineCount, int hitCount) GetLineRate(CoveredContract contract, IEnumerable<NeoDebugInfo.SequencePoint> sequencePoints)
        {
            var lines = sequencePoints.GroupBy(point => (point.Document, point.Start.Line)).ToArray();
            return (lines.Length, lines.Count(line => line.Any(point =>
                contract.TryGetLine(point.Address, out var hit) && hit.Hits > 0)));
        }

        public static IEnumerable<(int address, CoverageBranch branch)> GetBranchInstructions(
            CoveredContract contract, NeoDebugInfo.Method method, NeoDebugInfo.SequencePoint sequencePoint
            )
        {
            var nextIndex = Array.IndexOf(method.SequencePoints.ToArray(), sequencePoint) + 1;
            foreach (var branch in contract.Branches)
            {
                if (branch.Offset < sequencePoint.Address) continue;
                if (branch.Offset > method.Range.End ||
                    nextIndex < method.SequencePoints.Count && branch.Offset >= method.SequencePoints[nextIndex].Address)
                    yield break;
                yield return (branch.Offset, branch);
            }
        }

        public static int GetLineLastAddress(CoverageHit[] lines, NeoDebugInfo.Method method, int index)
        {
            var nextIndex = index + 1;
            if (nextIndex >= method.SequencePoints.Count)
            {
                // if we're on the last SP of the method, return the method end address
                return method.Range.End;
            }
            else
            {
                var nextSPAddress = method.SequencePoints[index + 1].Address;
                var address = method.SequencePoints[index].Address;

                foreach (var line in lines)
                {
                    if (line.Offset >= nextSPAddress)
                    {
                        return address;
                    }
                    else
                    {
                        address = line.Offset;
                    }
                }

                return address;
            }
        }
    }
}
