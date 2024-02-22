using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Neo.SmartContract.Testing.Coverage.Formats
{
    public partial class CoberturaFormat
    {
        internal class ContractCoverageWriter
        {
            readonly CoveredContract Contract;
            readonly NeoDebugInfo DebugInfo;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="contract">Contract</param>
            /// <param name="debugInfo">Debug info</param>
            public ContractCoverageWriter(CoveredContract contract, NeoDebugInfo debugInfo)
            {
                Contract = contract;
                DebugInfo = debugInfo;
            }

            public void WritePackage(XmlWriter writer)
            {
                var allMethods = DebugInfo.Methods.SelectMany(m => m.SequencePoints).ToArray();
                var (lineCount, hitCount) = GetLineRate(Contract, allMethods);
                var lineRate = CoverageBase.CalculateHitRate(lineCount, hitCount);
                var (branchCount, branchHit) = GetBranchRate(Contract, allMethods);
                var branchRate = CoverageBase.CalculateHitRate(branchCount, branchHit);

                writer.WriteStartElement("package");
                // TODO: complexity
                writer.WriteAttributeString("name", DebugInfo.Hash.ToString());
                writer.WriteAttributeString("scripthash", $"{DebugInfo.Hash}");
                writer.WriteAttributeString("line-rate", $"{lineRate:N4}");
                writer.WriteAttributeString("branch-rate", $"{branchRate:N4}");
                writer.WriteStartElement("classes");
                {
                    foreach (var group in DebugInfo.Methods.GroupBy(NamespaceAndFilename))
                    {
                        WriteClass(writer, group.Key.@namespace, group.Key.filename, group);
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndElement();

                (string @namespace, string filename) NamespaceAndFilename(NeoDebugInfo.Method method)
                {
                    var indexes = method.SequencePoints
                        .Select(sp => sp.Document)
                        .Distinct()
                        .ToList();
                    if (indexes.Count == 1)
                    {
                        var index = indexes[0];
                        if (index >= 0 && index < DebugInfo.Documents.Count)
                        {
                            return (method.Namespace, DebugInfo.Documents[index]);
                        }
                    }
                    return (method.Namespace, string.Empty);
                }
            }

            internal void WriteClass(XmlWriter writer, string name, string filename, IEnumerable<NeoDebugInfo.Method> methods)
            {
                var allMethods = methods.SelectMany(m => m.SequencePoints).ToArray();
                var (lineCount, hitCount) = GetLineRate(Contract, allMethods);
                var lineRate = CoverageBase.CalculateHitRate(lineCount, hitCount);
                var (branchCount, branchHit) = GetBranchRate(Contract, allMethods);
                var branchRate = CoverageBase.CalculateHitRate(branchCount, branchHit);

                writer.WriteStartElement("class");
                // TODO: complexity
                writer.WriteAttributeString("name", name);
                if (filename.Length > 0)
                { writer.WriteAttributeString("filename", filename); }
                writer.WriteAttributeString("line-rate", $"{lineRate:N4}");
                writer.WriteAttributeString("branch-rate", $"{branchRate:N4}");

                writer.WriteStartElement("methods");
                foreach (var method in methods)
                {
                    WriteMethod(writer, method);
                }
                writer.WriteEndElement();

                writer.WriteStartElement("lines");
                foreach (var method in methods)
                {
                    foreach (var sp in method.SequencePoints)
                    {
                        WriteLine(writer, method, sp);
                    }
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            internal void WriteMethod(XmlWriter writer, NeoDebugInfo.Method method)
            {
                var signature = string.Join(", ", method.Parameters.Select(p => p.Type));
                var (lineCount, hitCount) = GetLineRate(Contract, method.SequencePoints);
                var lineRate = CoverageBase.CalculateHitRate(lineCount, hitCount);
                var (branchCount, branchHit) = GetBranchRate(Contract, method.SequencePoints);
                var branchRate = CoverageBase.CalculateHitRate(branchCount, branchHit);

                writer.WriteStartElement("method");
                writer.WriteAttributeString("name", method.Name);
                writer.WriteAttributeString("signature", $"({signature})");
                writer.WriteAttributeString("line-rate", $"{lineRate:N4}");
                writer.WriteAttributeString("branch-rate", $"{branchRate:N4}");
                writer.WriteStartElement("lines");
                foreach (var sp in method.SequencePoints)
                {
                    WriteLine(writer, method, sp);
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            internal void WriteLine(XmlWriter writer, NeoDebugInfo.Method method, NeoDebugInfo.SequencePoint sp)
            {
                var hits = Contract.TryGetLine(sp.Address, out var value) ? value.Hits : 0;

                writer.WriteStartElement("line");
                writer.WriteAttributeString("number", $"{sp.Start.Line}");
                writer.WriteAttributeString("address", $"{sp.Address}");
                writer.WriteAttributeString("hits", $"{hits}");

                if (!Contract.TryGetBranch(sp.Address, out var branch))
                {
                    writer.WriteAttributeString("branch", $"{false}");
                }
                else
                {
                    int branchCount = branch.Count;
                    int branchHit = branch.Hits;
                    var branchRate = CoverageBase.CalculateHitRate(branchCount, branchHit);

                    writer.WriteAttributeString("branch", $"{true}");
                    writer.WriteAttributeString("condition-coverage", $"{branchRate * 100:N}% ({branchHit}/{branchCount})");
                    writer.WriteStartElement("conditions");

                    foreach (var (address, opCode) in GetBranchInstructions(Contract, method, sp))
                    {
                        var (condBranchCount, condContinueCount) = Contract.TryGetBranch(address, out var brach) ?
                            (brach.Count, brach.Hits) : (0, 0);
                        var coverage = condBranchCount == 0 ? 0m : 1m;
                        coverage += condContinueCount == 0 ? 0m : 1m;

                        writer.WriteStartElement("condition");
                        writer.WriteAttributeString("number", $"{address}");
                        writer.WriteAttributeString("type", $"{opCode}");
                        writer.WriteAttributeString("coverage", $"{coverage / 2m * 100m}%");
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }
    }
}
