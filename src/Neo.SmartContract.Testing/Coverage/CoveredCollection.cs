using Neo.SmartContract.Testing.Coverage.Formats;
using System;
using System.Collections.Generic;

namespace Neo.SmartContract.Testing.Coverage
{
    public class CoveredCollection : CoverageBase
    {
        /// <summary>
        /// Entries
        /// </summary>
        public CoverageBase[] Entries { get; }

        /// <summary>
        /// Coverage Lines
        /// </summary>
        public override IEnumerable<CoverageHit> Lines
        {
            get
            {
                foreach (var entry in Entries)
                {
                    foreach (var line in entry.Lines)
                    {
                        yield return line;
                    }
                }
            }
        }

        /// <summary>
        /// Coverage Branches
        /// </summary>
        public override IEnumerable<CoverageBranch> Branches
        {
            get
            {
                foreach (var entry in Entries)
                {
                    foreach (var branch in entry.Branches)
                    {
                        yield return branch;
                    }
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entries">Entries</param>
        public CoveredCollection(params CoverageBase[] entries)
        {
            Entries = entries;
        }

        /// <summary>
        /// Dump coverage
        /// </summary>
        /// <param name="format">Format</param>
        /// <returns>Coverage dump</returns>
        public override string Dump(DumpFormat format = DumpFormat.Console)
        {
            switch (format)
            {
                case DumpFormat.Console: return new ConsoleFormat(GetEntries()).Dump();
                case DumpFormat.Html: return new IntructionHtmlFormat(GetEntries()).Dump();
                default: throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get covered entries
        /// </summary>
        /// <returns>IEnumerable</returns>
        private IEnumerable<(CoveredContract, Func<CoveredMethod, bool>?)> GetEntries()
        {
            foreach (var entry in Entries)
            {
                if (entry is CoveredContract co) yield return (co, null);
                if (entry is CoveredMethod cm) yield return (cm.Contract, (CoveredMethod method) => ReferenceEquals(method, cm));
                if (entry is CoveredCollection cl)
                {
                    foreach (var subEntry in cl.GetEntries())
                    {
                        yield return subEntry;
                    }
                }
            }
        }
    }
}
