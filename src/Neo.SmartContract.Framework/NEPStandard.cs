namespace Neo.SmartContract.Framework
{
    public enum NepStandard
    {
        Nep11,
        Nep17,
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
