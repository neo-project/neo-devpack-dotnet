namespace Neo.SmartContract.Framework.Interfaces;

/// <summary>
/// NEP-29: Contract deployment/update callback function
/// This interface standardizes the callback function that can be implemented by contracts
/// to have some code executed right after initial deployment or update.
/// </summary>
public interface INEP29
{
    /// <summary>
    /// This method will be automatically executed by ContractManagement contract when a contract is first deployed or updated.
    /// </summary>
    /// <param name="data">Contract-specific data, can be any valid NEP-14 parameter type</param>
    /// <param name="update">True when contract is updated, false on initial deployment</param>
    void _deploy(object data, bool update);
}
