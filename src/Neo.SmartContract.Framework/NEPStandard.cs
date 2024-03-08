namespace Neo.SmartContract.Framework
{
    public enum NepStandard
    {
        // The NEP-11 standard is used for non-fungible tokens (NFTs).
        // Defined at https://github.com/neo-project/proposals/blob/master/nep-11.mediawiki
        Nep11,
        // The NEP-17 standard is used for fungible tokens.
        // Defined at https://github.com/neo-project/proposals/blob/master/nep-17.mediawiki
        Nep17
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
                default:
                    return standard.ToString();
            }
        }
    }
}
