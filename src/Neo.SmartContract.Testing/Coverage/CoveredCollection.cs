using Neo.SmartContract.Testing.Coverage.Formats;
using System;
using System.Collections.Generic;
using System.Linq;

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
            IEnumerable<(CoveredContract, CoveredMethod[])> entries = Entries.Select(u =>
            {
                if (u is CoveredContract co) return (co, co.Methods);
                if (u is CoveredMethod cm) return (cm.Contract, new CoveredMethod[] { cm });

                throw new NotImplementedException();
            })!;

            switch (format)
            {
                case DumpFormat.Console:
                    {
                        return new ConsoleFormat(entries).Dump();
                    }
                case DumpFormat.Html:
                    {
                        return new IntructionHtmlFormat(entries).Dump();
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
    }
}
