// Copyright (C) 2015-2026 The Neo Project.
//
// CoberturaFormat.ContractCoverageWriter.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Neo.SmartContract.Testing.Coverage.Formats
{
    public partial class CoberturaFormat
    {
        internal class ContractCoverageWriter(CoveredContract contract, NeoDebugInfo debugInfo)
        {
            public void WritePackage(XmlWriter writer)
            {
                var points = GetPoints(debugInfo).ToArray();
                writer.WriteStartElement("package");
                writer.WriteAttributeString("name", debugInfo.Hash.ToString());
                writer.WriteAttributeString("scripthash", debugInfo.Hash.ToString());
                WriteRates(writer, points);
                writer.WriteStartElement("classes");
                foreach (var group in points.GroupBy(point => (point.Method.Namespace, point.Point.Document)))
                {
                    WriteClass(writer, group.Key.Namespace, debugInfo.Documents[group.Key.Document], group.ToArray());
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            private void WriteRates(XmlWriter writer,
                IEnumerable<(NeoDebugInfo.Method Method, NeoDebugInfo.SequencePoint Point)> points)
            {
                var (lines, hits) = GetLineRate(contract, points.Select(point => point.Point));
                var (branches, branchHits) = GetBranchRate(contract, points);
                writer.WriteAttributeString("line-rate", CoverageBase.CalculateHitRate(lines, hits).ToString("F4", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("branch-rate", CoverageBase.CalculateHitRate(branches, branchHits).ToString("F4", CultureInfo.InvariantCulture));
            }

            private void WriteClass(XmlWriter writer, string name, string filename,
                (NeoDebugInfo.Method Method, NeoDebugInfo.SequencePoint Point)[] points)
            {
                writer.WriteStartElement("class");
                writer.WriteAttributeString("name", name);
                if (filename.Length > 0) writer.WriteAttributeString("filename", filename);
                WriteRates(writer, points);
                writer.WriteStartElement("methods");
                foreach (var group in points.GroupBy(point => point.Method))
                {
                    var method = group.Key;
                    writer.WriteStartElement("method");
                    writer.WriteAttributeString("name", method.Name);
                    writer.WriteAttributeString("signature", $"({string.Join(", ", method.Parameters.Select(parameter => parameter.Type))})");
                    WriteRates(writer, group);
                    WriteLines(writer, group);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                WriteLines(writer, points);
                writer.WriteEndElement();
            }

            private void WriteLines(XmlWriter writer,
                IEnumerable<(NeoDebugInfo.Method Method, NeoDebugInfo.SequencePoint Point)> points)
            {
                writer.WriteStartElement("lines");
                foreach (var line in points.GroupBy(point => point.Point.Start.Line).OrderBy(group => group.Key))
                {
                    var hits = line.Max(point => contract.TryGetLine(point.Point.Address, out var hit) ? hit.Hits : 0);
                    var branches = line.SelectMany(point => GetBranchInstructions(contract, point.Method, point.Point))
                        .DistinctBy(branch => branch.address).ToArray();
                    writer.WriteStartElement("line");
                    writer.WriteAttributeString("number", XmlConvert.ToString(line.Key));
                    writer.WriteAttributeString("address", XmlConvert.ToString(line.Min(point => point.Point.Address)));
                    writer.WriteAttributeString("hits", XmlConvert.ToString(hits));
                    writer.WriteAttributeString("branch", XmlConvert.ToString(branches.Length > 0));
                    if (branches.Length > 0)
                    {
                        var total = branches.Sum(branch => branch.branch.Count);
                        var covered = branches.Sum(branch => branch.branch.Hits);
                        var rate = CoverageBase.CalculateHitRate(total, covered) * 100;
                        writer.WriteAttributeString("condition-coverage", string.Create(CultureInfo.InvariantCulture, $"{rate:0.##}% ({covered}/{total})"));
                        writer.WriteStartElement("conditions");
                        foreach (var (address, branch) in branches)
                        {
                            writer.WriteStartElement("condition");
                            writer.WriteAttributeString("number", XmlConvert.ToString(address));
                            writer.WriteAttributeString("type", "jump");
                            writer.WriteAttributeString("coverage", string.Create(CultureInfo.InvariantCulture,
                                $"{CoverageBase.CalculateHitRate(branch.Count, branch.Hits) * 100:0.##}%"));
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }
    }
}
