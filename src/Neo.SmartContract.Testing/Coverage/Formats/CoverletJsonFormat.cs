using System;
using System.Collections.Generic;
using System.IO;

namespace Neo.SmartContract.Testing.Coverage.Formats
{
    public partial class CoverletJsonFormat : CoverageFormatBase
    {
        /// <summary>
        /// Contract
        /// </summary>
        public IReadOnlyList<(CoveredContract Contract, NeoDebugInfo DebugInfo)> Contracts { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contracts">Contracts</param>
        public CoverletJsonFormat(params (CoveredContract Contract, NeoDebugInfo DebugInfo)[] contracts)
        {
            Contracts = contracts;
        }

        /// <summary>
        /// Write module to path
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="mergeIfExists">Merge if exists</param>
        public void Write(string path, bool mergeIfExists)
        {
            var module = GetModules(Contracts);
            module?.Write(path, mergeIfExists);
        }

        public override void WriteReport(Action<string, Action<Stream>> writeAttachement)
        {
            writeAttachement("coverage.cobertura.json", stream =>
            {
                StreamWriter textWriter = new(stream);
                WriteReport(textWriter, Contracts);
                textWriter.Flush();
            });
        }

        private static void WriteReport(StreamWriter textWriter, IReadOnlyList<(CoveredContract Contract, NeoDebugInfo DebugInfo)> contracts)
        {
            Modules? modules = GetModules(contracts);
            if (modules is null) return;

            // https://github.com/coverlet-coverage/coverlet/blob/783c482bbb1d59e02e6859fd5397ebda95774f3c/src/coverlet.core/Reporters/JsonReporter.cs#L10

            textWriter.Write(modules.ToJson());
        }

        private static Modules? GetModules(IReadOnlyList<(CoveredContract Contract, NeoDebugInfo DebugInfo)> contracts)
        {
            Modules? module = null;

            foreach (var contract in contracts)
            {
                var m = GetModule(contract.Contract, contract.DebugInfo);

                if (module is null) module = m;
                else module.Merge(m);
            }

            return module;
        }

        private static Modules GetModule(CoveredContract contract, NeoDebugInfo debugInfo)
        {
            var module = new Modules();
            var docs = new Documents();

            module[contract.Name] = docs;

            foreach (var debugDoc in debugInfo.Documents)
            {
                docs[Path.Combine(debugInfo.DocumentRoot, debugDoc)] = new();
            }

            foreach (var debugMethod in debugInfo.Methods)
            {
                foreach (var debugPoint in debugMethod.SequencePoints)
                {
                    // Get method

                    var debugDoc = debugInfo.Documents[debugPoint.Document];
                    if (!docs.TryGetValue(Path.Combine(debugInfo.DocumentRoot, debugDoc), out var classes))
                    {
                        continue;
                    }

                    if (!classes.TryGetValue(debugMethod.Namespace, out var methods))
                    {
                        methods = new();
                        classes[debugMethod.Namespace] = methods;
                    }

                    if (!methods.TryGetValue(debugMethod.Name, out var method))
                    {
                        method = new();
                        methods[debugMethod.Name] = method;
                    }

                    // Branches

                    if (contract.TryGetBranch(debugPoint.Address, out var branch))
                    {
                        method.Branches.Add(new BranchInfo()
                        {
                            Line = debugPoint.Start.Line,
                            Path = branch.Count,
                            Hits = branch.Hits,
                            Offset = debugPoint.Start.Column,
                            EndOffset = debugPoint.End.Column
                        });
                    }

                    // Lines

                    for (int line = debugPoint.Start.Line; line <= debugPoint.End.Line; line++)
                    {
                        var hits = contract.TryGetLine(debugPoint.Address, out var hit) ? hit.Hits : 0;

                        if (!method.Lines.TryGetValue(line, out var hitLine))
                        {
                            method.Lines.Add(line, hits);
                        }
                        else
                        {
                            method.Lines[line] = hits + hitLine;
                        }
                    }
                }
            }

            return module;
        }
    }
}
