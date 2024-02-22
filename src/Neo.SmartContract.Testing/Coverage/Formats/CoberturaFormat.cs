using Neo.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Neo.SmartContract.Testing.Coverage.Formats
{
    public partial class CoberturaFormat : ICoverageFormat
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

        public void WriteReport(Action<string, Action<Stream>> writeAttachement)
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

        internal void WriteReport(XmlWriter writer, IReadOnlyList<(CoveredContract Contract, NeoDebugInfo DebugInfo)> coverage)
        {
            int linesValid = 0, linesCovered = 0;
            int branchesValid = 0, branchesCovered = 0;

            foreach (var entry in coverage)
            {
                var (lineCount, hitCount) = GetLineRate(entry.Contract, entry.DebugInfo.Methods.SelectMany(m => m.SequencePoints));
                linesValid += lineCount;
                linesCovered += hitCount;

                var (branchCount, branchHit) = GetBranchRate(entry.Contract, entry.DebugInfo.Methods);
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

        private static (int branchCount, int branchHit) GetBranchRate(CoveredContract contract, IEnumerable<NeoDebugInfo.Method> methods)
        {
            int branchCount = 0, branchHit = 0;
            foreach (var method in methods)
            {
                var rate = GetBranchRate(contract, method);

                branchCount += rate.branchCount;
                branchHit += rate.branchHit;
            }
            return (branchCount, branchHit);
        }

        private static (int branchCount, int branchHit) GetBranchRate(CoveredContract contract, NeoDebugInfo.Method method)
        {
            int branchCount = 0, branchHit = 0;

            foreach (var sp in method.SequencePoints)
            {
                if (contract.TryGetBranch(sp.Address, out var branch))
                {
                    branchCount += branch.Count;
                    branchHit += branch.Hits;
                }
            }

            return (branchCount, branchHit);
        }

        private static (int lineCount, int hitCount) GetLineRate(CoveredContract contract, IEnumerable<NeoDebugInfo.SequencePoint> lines)
        {
            int lineCount = 0, hitCount = 0;

            foreach (var line in lines)
            {
                lineCount++;
                if (contract.TryGetLine(line.Address, out var hit) && hit.Hits > 0)
                {
                    hitCount++;
                }
            }

            return (lineCount, hitCount);
        }

        public static IEnumerable<(int address, OpCode opCode)> GetBranchInstructions(
            CoveredContract contract, NeoDebugInfo.Method method, NeoDebugInfo.SequencePoint sequencePoint
            )
        {
            var address = sequencePoint.Address;
            var lines = contract.Lines.Where(u => u.Offset >= address).ToArray();
            var last = GetLineLastAddress(lines, method, Array.IndexOf(method.SequencePoints.ToArray(), sequencePoint));

            foreach (var line in lines)
            {
                if (contract.TryGetBranch(address, out var branch)) // IsBranchInstruction
                {
                    //yield return (address, ins.OpCode);
                    yield return (address, OpCode.NOP);
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
                var point = method.SequencePoints[index];
                var address = point.Address;

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
