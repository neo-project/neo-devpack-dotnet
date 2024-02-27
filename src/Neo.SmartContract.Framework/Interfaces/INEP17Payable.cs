using System.Numerics;

namespace Neo.SmartContract.Framework.Interfaces;

/// <summary>
/// Interface of method that indicate a contract receives NEP-17 Payment
/// </summary>
public interface INep17Payable
{
    /// <summary>
    /// The contract should implement the <see cref="OnNEP17Payment"/> method
    /// to receive NEP17 tokens.
    /// </summary>
    /// <param name="from">The address of the payer</param>
    /// <param name="amount">The amount of token to be transferred</param>
    /// <param name="data">Additional payment description data</param>
    public void OnNEP17Payment(UInt160 from, BigInteger amount, object? data = null);
}
