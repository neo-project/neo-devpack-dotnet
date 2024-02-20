using Neo.Json;
using System;
using System.Buffers.Binary;
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

            if (_contractId is not null) return _contractId.Value;

            var state = _smartContract.Engine.Native.ContractManagement.GetContract(_smartContract.Hash)
                ?? throw new Exception($"The contract 0x{_smartContract.Hash} is not deployed, so it's not possible to get the storage id.");

            _contractId ??= state.Id;
            return _contractId.Value;
        }

        /// <summary>
        /// Check if the entry exist
        /// </summary>
        /// <param name="key">Key</param>
        public bool Contains(ReadOnlyMemory<byte> key)
        {
            var skey = new StorageKey() { Id = GetContractId(), Key = key };
            var entry = _smartContract.Engine.Storage.Snapshot.TryGet(skey);
            return entry != null;
        }

        /// <summary>
        /// Read an entry from the smart contract storage
        /// </summary>
        /// <param name="key">Key</param>
        public ReadOnlyMemory<byte> Get(ReadOnlyMemory<byte> key)
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

        /// <summary>
        /// Import data from json, expected data (in base64):
        /// - "prefix"  : { "key":"value" }
        /// </summary>
        /// <param name="json">Json Object</param>
        public void Import(string json)
        {
            if (JToken.Parse(json) is not JObject jo)
            {
                throw new FormatException("The json is not a valid JObject");
            }

            Import(jo);
        }

        /// <summary>
        /// Import data from json, expected data (in base64):
        /// - "prefix"  : { "key":"value" }
        /// </summary>
        /// <param name="json">Json Object</param>
        public void Import(JObject json)
        {
            var buffer = new byte[(sizeof(int))];
            BinaryPrimitives.WriteInt32LittleEndian(buffer, GetContractId());
            var keyId = Convert.ToBase64String(buffer);

            JObject prefix;

            // Find prefix

            if (json.ContainsProperty(keyId))
            {
                if (json[keyId] is not JObject jo)
                {
                    throw new FormatException("Invalid json");
                }

                prefix = jo;
            }
            else
            {
                return;
            }

            // Read values

            foreach (var entry in prefix.Properties)
            {
                if (entry.Value is JString str)
                {
                    // "key":"value" in base64

                    Put(Convert.FromBase64String(entry.Key), Convert.FromBase64String(str.Value));
                }
            }
        }

        /// <summary>
        /// Export data to json
        /// </summary>
        public JObject Export()
        {
            var buffer = new byte[(sizeof(int))];
            BinaryPrimitives.WriteInt32LittleEndian(buffer, GetContractId());
            var keyId = Convert.ToBase64String(buffer);

            // Write prefix

            JObject ret = new();
            JObject prefix = new();
            ret[keyId] = prefix;

            foreach (var entry in _smartContract.Engine.Storage.Snapshot.Seek(Array.Empty<byte>(), Persistence.SeekDirection.Forward))
            {
                // "key":"value" in base64

                prefix[Convert.ToBase64String(entry.Key.Key.ToArray())] = Convert.ToBase64String(entry.Value.Value.ToArray());
            }

            return ret;
        }
    }
}
