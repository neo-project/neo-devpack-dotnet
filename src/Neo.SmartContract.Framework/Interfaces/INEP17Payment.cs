using System.Numerics;

namespace Neo.SmartContract.Framework.Interfaces;

/// <summary>
/// Interface of method that indicate a contract receive NEP-17 Payment
/// </summary>
public interface INep17Payment
{
    /// <summary>
    /// The Token contract should implement the <see cref="OnNEP17Payment"/> method
    /// to receive assets and modify the Manifest file to trust the received asset contract.
    /// </summary>
    /// <param name="from">The address of the payer</param>
    /// <param name="amount">The amount of token to be transferred</param>
    /// <param name="data">Additional payment description data</param>
    /// <remarks>
    /// This interface method is defined as non-static,
    /// but if you need it to be static, you can directly
    /// remove the interface and define it as a static method.
    /// Both static and non-static methods of smart contract interface works,
    /// they differs on how you process static field.
    /// </remarks>
    public void OnNEP17Payment(UInt160 from, BigInteger amount, object? data = null);
}
