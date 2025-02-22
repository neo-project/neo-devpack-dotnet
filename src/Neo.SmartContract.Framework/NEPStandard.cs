// Copyright (C) 2015-2024 The Neo Project.
//
// NEPStandard.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework
{
    public enum NepStandard
    {
        // The NEP-11 standard is used for non-fungible tokens (NFTs).
        // Defined at https://github.com/neo-project/proposals/blob/master/nep-11.mediawiki
        Nep11,
        // The NEP-17 standard is used for fungible tokens.
        // Defined at https://github.com/neo-project/proposals/blob/master/nep-17.mediawiki
        Nep17,
        // Smart contract transfer callback for non-fungible tokens (NFTs).
        // This is an extension standard of NEP-11.
        // Defined at https://github.com/neo-project/proposals/pull/169/files#diff-2b5f7c12a23f7dbe4cb46bbf4be6936882f8e0f0b3a4db9d8c58eb294b02e6ed
        Nep26,
        // Smart contract transfer callback for fungible tokens.
        // This is an extension standard of NEP-17.
        // Defined at https://github.com/neo-project/proposals/pull/169/files#diff-70768f307c9aa84f8c94e790495a76d47fffeca2331444592ebba6f13b1e6460
        Nep27,
        // This NEP defines a global standard to get royalty payment information for Non-Fungible Tokens (NFTs)
        // in order to enable support for royalty payments across all NFT marketplaces in the NEO Smart Economy.
        // This NEP requires NEP-11.
        // Defined at https://github.com/neo-project/proposals/blob/master/nep-24.mediawiki
        Nep24,
        // NEP-29: Contract deployment/update callback function
        // This standard defines a callback function that contracts can implement
        // to execute code right after initial deployment or update.
        // Defined at https://github.com/neo-project/proposals/blob/master/nep-29.mediawiki
        Nep29,
        // NEP-30: Contract witness verification callback
        // This standard defines a method that contracts can implement to allow their addresses
        // to be used as transaction signers and pass witness checks in other code.
        // It enables contracts to act as transaction senders, covering fees from their GAS balance,
        // and to be verified in contexts like NEP-17 transfers.
        // The 'verify' method is automatically called during transaction verification with the Verification trigger.
        // Defined at https://github.com/neo-project/proposals/blob/master/nep-30.mediawiki
        Nep30,
    }

    public static class NepStandardExtensions
    {
        public static string ToStandard(this NepStandard standard)
        {
            return standard switch
            {
                NepStandard.Nep11 => "NEP-11",
                NepStandard.Nep17 => "NEP-17",
                NepStandard.Nep24 => "NEP-24",
                NepStandard.Nep26 => "NEP-26",
                NepStandard.Nep27 => "NEP-27",
                NepStandard.Nep29 => "NEP-29",
                NepStandard.Nep30 => "NEP-30",
                _ => standard.ToString()
            };
        }
    }
}
