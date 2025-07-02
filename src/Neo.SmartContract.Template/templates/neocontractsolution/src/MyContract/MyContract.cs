using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace MyContract
{
    [DisplayName("MyContract")]
    [ContractAuthor("Your Name", "your.email@example.com")]
    [ContractVersion("1.0.0")]
    [ContractDescription("A sample Neo N3 smart contract")]
    [ContractSourceCode("https://github.com/your-org/your-repo")]
    public class MyContract : SmartContract
    {
        // Contract owner
        private const byte Prefix_Owner = 0x01;
        
        // Storage prefixes
        private const byte Prefix_Config = 0x02;
        private const byte Prefix_Data = 0x03;
        
        // Events
        [DisplayName("Initialized")]
        public static event Action<string> OnInitialized;
        
        [DisplayName("DataStored")]
        public static event Action<UInt160, string, BigInteger> OnDataStored;
        
        /// <summary>
        /// Contract initialization method
        /// Called once after deployment to set up the contract
        /// </summary>
        /// <param name="data">Initial configuration data</param>
        /// <returns>True if initialization successful</returns>
        [DisplayName("initialize")]
        public static bool Initialize(string data)
        {
            // Check if already initialized
            if (Storage.Get(Storage.CurrentContext, new byte[] { Prefix_Config }).Length > 0)
            {
                throw new Exception("Contract already initialized");
            }
            
            // Set the contract owner to the deployer
            var owner = Runtime.Transaction.Sender;
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Owner }, owner);
            
            // Store initial configuration
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Config }, data);
            
            OnInitialized(data);
            return true;
        }
        
        /// <summary>
        /// Store data in the contract
        /// </summary>
        /// <param name="key">Data key</param>
        /// <param name="value">Data value</param>
        /// <returns>True if stored successfully</returns>
        [DisplayName("storeData")]
        public static bool StoreData(string key, BigInteger value)
        {
            // Verify caller
            if (!Runtime.CheckWitness(Runtime.Transaction.Sender))
            {
                throw new Exception("No authorization");
            }
            
            var storageKey = Helper.Concat(new byte[] { Prefix_Data }, key);
            Storage.Put(Storage.CurrentContext, storageKey, value);
            
            OnDataStored(Runtime.Transaction.Sender, key, value);
            return true;
        }
        
        /// <summary>
        /// Retrieve stored data
        /// </summary>
        /// <param name="key">Data key</param>
        /// <returns>Stored value</returns>
        [DisplayName("getData")]
        [Safe]
        public static BigInteger GetData(string key)
        {
            var storageKey = Helper.Concat(new byte[] { Prefix_Data }, key);
            var data = Storage.Get(Storage.CurrentContext, storageKey);
            return data.Length > 0 ? (BigInteger)data : 0;
        }
        
        /// <summary>
        /// Get contract owner
        /// </summary>
        /// <returns>Owner account</returns>
        [DisplayName("getOwner")]
        [Safe]
        public static UInt160 GetOwner()
        {
            var owner = Storage.Get(Storage.CurrentContext, new byte[] { Prefix_Owner });
            return owner.Length > 0 ? (UInt160)owner : UInt160.Zero;
        }
        
        /// <summary>
        /// Transfer ownership
        /// </summary>
        /// <param name="newOwner">New owner account</param>
        /// <returns>True if successful</returns>
        [DisplayName("transferOwnership")]
        public static bool TransferOwnership(UInt160 newOwner)
        {
            var currentOwner = GetOwner();
            if (!Runtime.CheckWitness(currentOwner))
            {
                throw new Exception("Only owner can transfer ownership");
            }
            
            if (!newOwner.IsValid || newOwner.IsZero)
            {
                throw new Exception("Invalid new owner");
            }
            
            Storage.Put(Storage.CurrentContext, new byte[] { Prefix_Owner }, newOwner);
            return true;
        }
        
        /// <summary>
        /// Update the contract
        /// Only the owner can update
        /// </summary>
        /// <param name="nefFile">New NEF file</param>
        /// <param name="manifest">New manifest</param>
        /// <param name="data">Update data</param>
        [DisplayName("update")]
        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            var owner = GetOwner();
            if (!Runtime.CheckWitness(owner))
            {
                throw new Exception("Only owner can update");
            }
            
            ContractManagement.Update(nefFile, manifest, data);
        }
        
        /// <summary>
        /// Destroy the contract
        /// Only the owner can destroy
        /// </summary>
        [DisplayName("destroy")]
        public static void Destroy()
        {
            var owner = GetOwner();
            if (!Runtime.CheckWitness(owner))
            {
                throw new Exception("Only owner can destroy");
            }
            
            ContractManagement.Destroy();
        }
    }
}