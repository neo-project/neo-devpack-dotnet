// Copyright (C) 2015-2025 The Neo Project.
//
// CoveredMethod.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

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
            return format switch
            {
                DumpFormat.Console => new ConsoleFormat(Contract, m => ReferenceEquals(m, this)).Dump(),
                DumpFormat.Html => new IntructionHtmlFormat(Contract, m => ReferenceEquals(m, this)).Dump(),
                _ => throw new NotImplementedException(),
            };
        }

        public override string ToString() => Method.ToString();
    }
}
