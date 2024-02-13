using System.Collections.Generic;

namespace Neo.SmartContract.Testing.Coverage
{
    public class CoveredCollection : CoverageBase
    {
        /// <summary>
        /// Contract
        /// </summary>
        public CoveredContract Contract { get; }

        /// <summary>
        /// Entries
        /// </summary>
        public CoverageBase[] Entries { get; }

        /// <summary>
        /// Coverage
        /// </summary>
        public override IEnumerable<CoverageData> Coverage
        {
            get
            {
                foreach (var method in Entries)
                {
                    foreach (var entry in Contract.Coverage)
                    {
                        yield return entry;
                    }
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract">Contract</param>
        /// <param name="entries">Entries</param>
        public CoveredCollection(CoveredContract contract, params CoverageBase[] entries)
        {
            Contract = contract;
            Entries = entries;
        }
    }
}
