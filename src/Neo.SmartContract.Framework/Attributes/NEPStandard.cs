namespace Neo.SmartContract.Framework.Attributes
{
    public enum NEPStandard
    {
        NEP11,
        NEP17,
    }

    public static class NEPStandardExtensions
    {
        public static string ToStandard(this NEPStandard standard)
        {
            switch (standard)
            {
                case NEPStandard.NEP11:
                    return "NEP-11";
                case NEPStandard.NEP17:
                    return "NEP-17";
                default:
                    return standard.ToString();
            }
        }
    }
}
