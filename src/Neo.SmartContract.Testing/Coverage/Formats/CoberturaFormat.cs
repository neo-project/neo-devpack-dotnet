using System;
using System.Collections.Generic;
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
                var allPoints = entry.DebugInfo.Methods.SelectMany(m => m.SequencePoints).ToArray();

                var (lineCount, hitCount) = GetLineRate(entry.Contract, allPoints);
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
            writer.WriteAttributeString("line-rate", $"{lineRate:N4}");
            writer.WriteAttributeString("lines-covered", $"{linesCovered}");
            writer.WriteAttributeString("lines-valid", $"{linesValid}");
            writer.WriteAttributeString("branch-rate", $"{branchRate:N4}");
            writer.WriteAttributeString("branches-covered", $"{branchesCovered}");
            writer.WriteAttributeString("branches-valid", $"{branchesValid}");
            writer.WriteAttributeString("version", typeof(CoberturaFormat).Assembly.GetVersion());
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

        private static (int branchCount, int branchHit) GetBranchRate(CoveredContract contract, IEnumerable<NeoDebugInfo.SequencePoint> sequencePoints)
        {
            int branchCount = 0, branchHit = 0;

            foreach (var sp in sequencePoints)
            {
                if (contract.TryGetBranch(sp.Address, out var branch))
                {
                    branchCount += branch.Count;
                    branchHit += branch.Hits;
                }
            }

            return (branchCount, branchHit);
        }

        private static (int lineCount, int hitCount) GetLineRate(CoveredContract contract, IEnumerable<NeoDebugInfo.SequencePoint> sequencePoints)
        {
            int lineCount = 0, hitCount = 0;

            foreach (var sp in sequencePoints)
            {
                lineCount++;
                if (contract.TryGetLine(sp.Address, out var hit) && hit.Hits > 0)
                {
                    hitCount++;
                }
            }

            return (lineCount, hitCount);
        }

        public static IEnumerable<(int address, CoverageBranch branch)> GetBranchInstructions(
            CoveredContract contract, NeoDebugInfo.Method method, NeoDebugInfo.SequencePoint sequencePoint
            )
        {
            var address = sequencePoint.Address;
            var lines = contract.Lines.Where(u => u.Offset >= address).ToArray();
            var last = GetLineLastAddress(lines, method, Array.IndexOf(method.SequencePoints.ToArray(), sequencePoint));

            foreach (var line in lines)
            {
                if (line.Offset > last) break;

                if (contract.TryGetBranch(address, out var branch)) // IsBranchInstruction
                {
                    yield return (address, branch);
                }
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
                    if (line.Offset > nextSPAddress)
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
