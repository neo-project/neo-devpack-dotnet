using System;

namespace Neo.SmartContract.Testing
{
    public class SmartContractStorage
    {
        private readonly SmartContract _smartContract;
        private int? _smartContractId;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="smartContract">Smart Contract</param>
        internal SmartContractStorage(SmartContract smartContract)
        {
            _smartContract = smartContract;
        }

        private int GetContractId()
        {
            _smartContractId ??= _smartContract.Engine.Native.ContractManagement.getContract(_smartContract.Hash).Id;
            return _smartContractId.Value;
        }

        /// <summary>
        /// Remove an entry from the smart contract storage
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
