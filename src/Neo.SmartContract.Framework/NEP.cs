namespace Neo.SmartContract.Framework
{
    public static class NEP
    {
        public const string NEP11 = "NEP-11";
        public const string NEP17 = "NEP-17";

        public static string[] All => new string[] { NEP11, NEP17 };

        /// <summary>
        ///  The method that will be called when a NEP-17/11 token is transferred to the contract.
        /// </summary>
        /// <remarks>
        /// The name does not follow the C# naming convention because it is a standard method name.
        /// </remarks>
        /// <returns>A string of the nep17 standard method</returns>
        public const string NEP17Payment = "onNEP17Payment";
        public const string NEP11Payment = "onNEP11Payment";
    }
}
