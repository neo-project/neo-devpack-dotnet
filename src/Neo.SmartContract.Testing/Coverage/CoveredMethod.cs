using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Coverage.Formats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("{Method}")]
    public class CoveredMethod : CoverageBase
    {
        /// <summary>
        /// Contract
        /// </summary>
        public CoveredContract Contract { get; }

        /// <summary>
        /// Method
        /// </summary>
        public AbiMethod Method { get; }

        /// <summary>
        /// Offset
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Method length
        /// </summary>
        public int MethodLength { get; }

        /// <summary>
        /// Coverage Lines
        /// </summary>
        public override IEnumerable<CoverageHit> Lines => Contract.GetCoverageLinesFrom(Offset, MethodLength);

        /// <summary>
        /// Coverage Branches
        /// </summary>
        public override IEnumerable<CoverageBranch> Branches => Contract.GetCoverageBranchFrom(Offset, MethodLength);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract">Contract</param>
        /// <param name="method">Method</param>
        /// <param name="methodLength">Method length</param>
        public CoveredMethod(CoveredContract contract, ContractMethodDescriptor method, int methodLength)
        {
            Contract = contract;
            Offset = method.Offset;
            MethodLength = methodLength;
            Method = new AbiMethod(method.Name, method.Parameters.Select(u => u.Name).ToArray());
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
                case DumpFormat.Console:
                    {
                        return new ConsoleFormat(Contract, m => ReferenceEquals(m, this)).Dump();
                    }
                case DumpFormat.Html:
                    {
                        return new IntructionHtmlFormat(Contract, m => ReferenceEquals(m, this)).Dump();
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public override string ToString() => Method.ToString();
    }
}
