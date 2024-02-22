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
    }
}
