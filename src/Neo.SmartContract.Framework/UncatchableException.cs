using System;

namespace Neo.SmartContract.Framework
{
    /// <summary>
    /// Represents an exception that cannot be caught in the contract.
    /// When the exception is thrown, the ABORT instruction of the VM will be executed, causing the ExecutionEngine to directly enter the FAULT state.
    /// </summary>
    public class UncatchableException : Exception
    {
    }
}
