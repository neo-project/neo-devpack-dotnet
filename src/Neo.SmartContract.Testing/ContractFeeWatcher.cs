// Copyright (C) 2015-2025 The Neo Project.
//
// ContractFeeWatcher.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Neo.SmartContract.Testing
{
    /// <summary>
    /// Watches and simulates custom contract fees during testing.
    /// </summary>
    [DebuggerDisplay("TotalFees={TotalFees}")]
    public class ContractFeeWatcher : IDisposable
    {
        private readonly TestEngine _testEngine;
        private readonly List<ContractFeeRecord> _feeRecords = new();

        /// <summary>
        /// Total custom contract fees collected (in datoshi)
        /// </summary>
        public long TotalFees { get; private set; } = 0;

        /// <summary>
        /// Fee records for each method invocation
        /// </summary>
        public IReadOnlyList<ContractFeeRecord> FeeRecords => _feeRecords;

        /// <summary>
        /// Constructor
        /// </summary>
        public ContractFeeWatcher(TestEngine engine)
        {
            _testEngine = engine;
            _testEngine._contractFeeWatchers.Add(this);
        }

        /// <summary>
        /// Record a fee charge
        /// </summary>
        internal void RecordFee(UInt160 contract, string method, long amount, string beneficiary)
        {
            var record = new ContractFeeRecord(contract, method, amount, beneficiary);
            _feeRecords.Add(record);
            TotalFees += amount;
        }

        /// <summary>
        /// Reset all fee records
        /// </summary>
        public void Reset()
        {
            _feeRecords.Clear();
            TotalFees = 0;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _testEngine._contractFeeWatchers.Remove(this);
        }
    }

    /// <summary>
    /// Record of a single contract fee charge
    /// </summary>
    public class ContractFeeRecord
    {
        public UInt160 Contract { get; }
        public string Method { get; }
        public long Amount { get; }
        public string Beneficiary { get; }
        public DateTime Timestamp { get; }

        public ContractFeeRecord(UInt160 contract, string method, long amount, string beneficiary)
        {
            Contract = contract;
            Method = method;
            Amount = amount;
            Beneficiary = beneficiary;
            Timestamp = DateTime.UtcNow;
        }
    }
}
