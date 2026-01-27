// Copyright (C) 2015-2026 The Neo Project.
//
// ContractManagement.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0xfffdc93764dbaddd97c48f252a53ea4643faa3fd")]
    public class ContractManagement
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }

        /// <summary>
        /// Gets the minimum deployment fee in the unit of datoshi, 1 datoshi = 1e-8 GAS.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern long GetMinimumDeploymentFee();

        /// <summary>
        /// Gets the contract by the specified hash, null if not found.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// <para>
        /// The execution will fail if the 'hash' is null.
        /// </para>
        /// </summary>
        public static extern Contract GetContract(UInt160 hash);

        /// <summary>
        /// Checks if the contract with the specified hash exists.
        /// Available since HF_Echidna.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// <para>
        /// The execution will fail if the 'hash' is null.
        /// </para>
        /// </summary>
        public static extern bool IsContract(UInt160 hash);

        /// <summary>
        /// Gets the contract by the specified id, null if not found.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern Contract GetContractById(int id);

        /// <summary>
        /// Gets hashes of all non native deployed contracts.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// </summary>
        public static extern Iterator<(int, UInt160)> GetContractHashes();

        /// <summary>
        /// Checks if a method with the specified name and parameter count exists in a contract.
        /// If the 'pcount' is -1, the first method with the specified name will be checked.
        /// CallFlags requirement: CallFlags.ReadStates.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'hash' is null.
        ///  2. The 'pcount' is less than -1 or greater than ushort.MaxValue.
        /// </para>
        /// </summary>
        public static extern bool HasMethod(UInt160 hash, string method, int pcount);

        /// <summary>
        /// Deploys a new contract.
        /// CallFlags requirement: CallFlags.States | CallFlags.AllowNotify before HF_Aspidochelone, CallFlags.All after HF_Aspidochelone.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'nefFile' is null or empty, or not a valid NEF file.
        ///  2. The 'manifest' is null or empty, or not a valid manifest.
        ///  3. The contract hash has been blocked or already exists.
        /// </para>
        /// </summary>
        public static extern Contract Deploy(ByteString nefFile, string manifest);

        /// <summary>
        /// Deploys a new contract with the specified data.
        /// The 'data' will be passed to the '_deploy' method of the contract.
        /// CallFlags requirement: CallFlags.States | CallFlags.AllowNotify before HF_Aspidochelone, CallFlags.All after HF_Aspidochelone.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'nefFile' is null or empty, or not a valid NEF file.
        ///  2. The 'manifest' is null or empty, or not a valid manifest.
        ///  3. The contract hash has been blocked or already exists.
        /// </para>
        /// </summary>
        public static extern Contract Deploy(ByteString nefFile, string manifest, object data);

        /// <summary>
        /// Updates a contract.
        /// CallFlags requirement: CallFlags.States | CallFlags.AllowNotify before HF_Aspidochelone, CallFlags.All after HF_Aspidochelone.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'nefFile' and 'manifest' are both null.
        ///  2. The 'nefFile' is empty or is not a valid NEF file;
        ///  3. The 'manifest' is empty or is not a valid manifest.
        ///  4. The tartget contract does not exist.
        ///  5. The contract update times reached the maximum number of updates(ushort.MaxValue).
        /// </para>
        /// </summary>
        public static extern void Update(ByteString nefFile, string manifest);

        /// <summary>
        /// Updates a contract with the specified data.
        /// The 'data' will be passed to the '_deploy' method of the contract.
        /// CallFlags requirement: CallFlags.States | CallFlags.AllowNotify before HF_Aspidochelone, CallFlags.All after HF_Aspidochelone.
        /// <para>
        /// The execution will fail if:
        ///  1. The 'nefFile' and 'manifest' are both null.
        ///  2. The 'nefFile' is empty or is not a valid NEF file;
        ///  3. The 'manifest' is empty or is not a valid manifest.
        ///  4. The tartget contract does not exist or the old and new contract names are different.
        ///  5. The contract update times reached the maximum number of updates(ushort.MaxValue).
        /// </para>
        /// </summary>
        public static extern void Update(ByteString nefFile, string manifest, object data);

        /// <summary>
        /// Destroys a contract(the calling contract).
        /// A destroyed contract cannot be redeployed.
        /// CallFlags requirement: CallFlags.States | CallFlags.AllowNotify.
        /// </summary>
        public static extern void Destroy();
    }
}
