// Copyright (C) 2015-2025 The Neo Project.
//
// AbiFee.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.Compiler.ABI
{
    /// <summary>
    /// Represents fee information for a contract method in the ABI.
    /// </summary>
    class AbiFee
    {
        /// <summary>
        /// The fee amount in datoshi (1e-8 GAS). Used when Mode is "fixed".
        /// </summary>
        public long Amount { get; }

        /// <summary>
        /// The Neo address or script hash of the fee recipient.
        /// </summary>
        public string Beneficiary { get; }

        /// <summary>
        /// The fee calculation mode: "fixed" or "dynamic".
        /// </summary>
        public string Mode { get; }

        /// <summary>
        /// The script hash of the fee calculator contract. Used when Mode is "dynamic".
        /// </summary>
        public string? DynamicScriptHash { get; }

        public AbiFee(long amount, string beneficiary, string mode, string? dynamicScriptHash)
        {
            Amount = amount;
            Beneficiary = beneficiary;
            Mode = mode;
            DynamicScriptHash = dynamicScriptHash;
        }
    }
}
