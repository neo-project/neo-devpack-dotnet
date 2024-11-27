// Copyright (C) 2015-2024 The Neo Project.
//
// INEP30.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework.Interfaces
{
    /// <summary>
    /// NEP-30: Contract witness verification callback
    /// This interface standardizes a specific method that if implemented allows
    /// contracts to be used as signers for transactions and pass witness checks
    /// for their address in other code.
    /// </summary>
    public interface INEP30
    {
        /// <summary>
        /// The verification method. This must be called when verifying the contract as a transaction signer.
        /// If the contract address is included in the transaction signature, this method verifies the signature.
        /// 
        /// This method will be automatically called during transaction verification (with Verification trigger).
        /// Its return value signifies whether verification was successful (true) or not (false).
        /// 
        /// To prevent using this method in regular application calls, contract can check for trigger
        /// returned from System.Runtime.GetTrigger interop function.
        /// 
        /// Example:
        /// <code>
        ///     // Parameters are contract-specific, any number of valid NEP-14 parameters can be added if contract needs them to perform verification. 
        ///     public static bool Verify(params object [] args) => Runtime.CheckWitness(Owner);
        /// </code>
        /// 
        /// JSON representation:
        /// <code>
        /// {
        ///   "name": "verify",
        ///   "safe": true,
        ///   "parameters": [],
        ///   "returntype": "bool"
        /// }
        /// </code>
        /// </summary>
        /// <remarks>
        /// Verify method can take arbitrary parameters.
        /// Compliant contract MUST have only one method with this name, otherwise the result is undefined.
        /// </remarks>
        /// <returns>True if verification was successful, false otherwise.</returns>
        public static abstract bool Verify(params object[] args);
    }
}
