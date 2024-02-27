namespace Neo.SmartContract.Framework;

public static class Method
{
    /// <summary>
    /// Indicates that the contract is allowed to call any method of allowed contract.
    /// </summary>
    public const string WildCard = "*";

    /// <summary>
    /// The name of the method that is called when a contract receives NEP-17 tokens.
    /// </summary>
    public const string OnNEP17Payment = "onNEP17Payment";

    /// <summary>
    /// The name of the method that is called when a contract receives NEP-11 tokens.
    /// </summary>
    public const string OnNEP11Payment = "onNEP11Payment";
}
