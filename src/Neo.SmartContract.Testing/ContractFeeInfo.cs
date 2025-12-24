// Copyright (C) 2015-2025 The Neo Project.
//
// ContractFeeInfo.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Json;

namespace Neo.SmartContract.Testing
{
    /// <summary>
    /// Represents custom contract fee information from the ABI.
    /// </summary>
    public class ContractFeeInfo
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

        /// <summary>
        /// Constructor
        /// </summary>
        public ContractFeeInfo(long amount, string beneficiary, string mode, string? dynamicScriptHash)
        {
            Amount = amount;
            Beneficiary = beneficiary;
            Mode = mode;
            DynamicScriptHash = dynamicScriptHash;
        }

        /// <summary>
        /// Parse fee info from ABI JSON
        /// </summary>
        public static ContractFeeInfo? FromJson(JToken? json)
        {
            if (json == null) return null;

            var mode = json["mode"]?.GetString() ?? "fixed";
            var beneficiary = json["beneficiary"]?.GetString() ?? string.Empty;
            var amount = (long)(json["amount"]?.AsNumber() ?? 0);
            var dynamicScriptHash = json["dynamicScriptHash"]?.GetString();

            return new ContractFeeInfo(amount, beneficiary, mode, dynamicScriptHash);
        }
    }
}
