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
        Nep11_Y,
        // This is the nick name of NEP-11-Y.
        Nep11Payable,
        // Smart contract transfer callback for fungible tokens.
        // This is an extension standard of NEP-17.
        // Defined at https://github.com/neo-project/proposals/pull/169/files#diff-70768f307c9aa84f8c94e790495a76d47fffeca2331444592ebba6f13b1e6460
        Nep17_Z,
        // This is the nick name of NEP-17-Z.
        Nep17Payable,
    }

    public static class NepStandardExtensions
    {
        public static string ToStandard(this NepStandard standard)
        {
            switch (standard)
            {
                case NepStandard.Nep11:
                    return "NEP-11";
                case NepStandard.Nep17:
                    return "NEP-17";
                case NepStandard.Nep11Payable:
                case NepStandard.Nep11_Y:
                    return "NEP-11-Y";
                case NepStandard.Nep17Payable:
                case NepStandard.Nep17_Z:
                    return "NEP-17-Z";
                default:
                    return standard.ToString();
            }
        }
    }
}
