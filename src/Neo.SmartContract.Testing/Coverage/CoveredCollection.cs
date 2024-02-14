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
        /// Coverage
        /// </summary>
        public override IEnumerable<CoverageData> Coverage
        {
            get
            {
                foreach (var method in Entries)
                {
                    foreach (var entry in method.Coverage)
                    {
                        yield return entry;
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
