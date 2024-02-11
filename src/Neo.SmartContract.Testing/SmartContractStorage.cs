using System;
using System.Numerics;

namespace Neo.SmartContract.Testing
{
    public class SmartContractStorage
    {
        private readonly SmartContract _smartContract;
        private int? _contractId;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="smartContract">Smart Contract</param>
        /// <param name="contractId">Contract id, can be null</param>
        internal SmartContractStorage(SmartContract smartContract, int? contractId = null)
        {
            _smartContract = smartContract;
            _contractId = contractId;
        }

        private int GetContractId()
        {
            // If it was not initialized checking the contract, we need to query the contract id
            _contractId ??= _smartContract.Engine.Native.ContractManagement.GetContract(_smartContract.Hash).Id;
            return _contractId.Value;
        }

        /// <summary>
        /// Read an entry from the smart contract storage
        /// </summary>
        /// <param name="key">Key</param>
        public ReadOnlyMemory<byte> Read(ReadOnlyMemory<byte> key)
        {
            var skey = new StorageKey() { Id = GetContractId(), Key = key };
            var entry = _smartContract.Engine.Storage.Snapshot.TryGet(skey);

            if (entry != null)
            {
                return entry.Value;
            }

            return null;
        }

        /// <summary>
        /// Put an entry in the smart contract storage
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Put(ReadOnlyMemory<byte> key, ReadOnlyMemory<byte> value)
        {
            var skey = new StorageKey() { Id = GetContractId(), Key = key };

            var entry = _smartContract.Engine.Storage.Snapshot.GetAndChange(skey, () => new StorageItem() { Value = value });
            entry.Value = value;
        }

        /// <summary>
        /// Put an entry in the smart contract storage
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public void Put(ReadOnlyMemory<byte> key, BigInteger value)
        {
            var skey = new StorageKey() { Id = GetContractId(), Key = key };

            var entry = _smartContract.Engine.Storage.Snapshot.GetAndChange(skey, () => new StorageItem(value));
            entry.Set(value);
        }

        /// <summary>
        /// Remove an entry from the smart contract storage
        /// </summary>
        /// <param name="key">Key</param>
        public void Remove(ReadOnlyMemory<byte> key)
        {
            var skey = new StorageKey() { Id = GetContractId(), Key = key };

            _smartContract.Engine.Storage.Snapshot.Delete(skey);
        }
    }
}
