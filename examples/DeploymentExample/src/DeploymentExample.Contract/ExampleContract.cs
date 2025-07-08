using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace DeploymentExample.Contract
{
    [DisplayName("DeploymentExample.ExampleContract")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet")]
    [ManifestExtra("Author", "Neo Community")]
    [ManifestExtra("Email", "developer@neo.org")]
    [ManifestExtra("Description", "Example contract demonstrating deployment workflow")]
    [ManifestExtra("Version", "1.0.0")]
    [ContractPermission("*", "*")]
    public class ExampleContract : SmartContract
    {
        // Contract owner - set during deployment
        private const byte OWNER_PREFIX = 0x01;
        
        // Counter storage key
        private const byte COUNTER_PREFIX = 0x02;

        // Events
        [DisplayName("CounterIncremented")]
        public static event Action<UInt160, BigInteger> OnCounterIncremented;

        [DisplayName("OwnerChanged")]
        public static event Action<UInt160, UInt160> OnOwnerChanged;

        /// <summary>
        /// Contract deployment initialization
        /// </summary>
        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (!update)
            {
                // If data is null, use the transaction sender as owner
                UInt160 owner;
                if (data == null)
                {
                    owner = Runtime.Transaction.Sender;
                }
                else
                {
                    owner = (UInt160)data;
                }
                
                if (!owner.IsValid || owner.IsZero)
                {
                    throw new Exception("Invalid owner address");
                }
                
                Storage.Put(Storage.CurrentContext, new byte[] { OWNER_PREFIX }, owner);
                Storage.Put(Storage.CurrentContext, new byte[] { COUNTER_PREFIX }, 0);
            }
        }

        /// <summary>
        /// Get the current contract owner
        /// </summary>
        [DisplayName("getOwner")]
        [Safe]
        public static UInt160 GetOwner()
        {
            var owner = Storage.Get(Storage.CurrentContext, new byte[] { OWNER_PREFIX });
            return owner?.Length == 20 ? (UInt160)owner : UInt160.Zero;
        }

        /// <summary>
        /// Transfer ownership to a new address
        /// </summary>
        [DisplayName("setOwner")]
        public static bool SetOwner(UInt160 newOwner)
        {
            if (!newOwner.IsValid || newOwner.IsZero)
            {
                throw new Exception("Invalid new owner address");
            }

            var currentOwner = GetOwner();
            if (!Runtime.CheckWitness(currentOwner))
            {
                throw new Exception("Only owner can transfer ownership");
            }

            Storage.Put(Storage.CurrentContext, new byte[] { OWNER_PREFIX }, newOwner);
            OnOwnerChanged(currentOwner, newOwner);
            return true;
        }

        /// <summary>
        /// Increment the counter
        /// </summary>
        [DisplayName("increment")]
        public static BigInteger Increment()
        {
            var caller = Runtime.CallingScriptHash;
            if (caller == null || !Runtime.CheckWitness(caller))
            {
                throw new Exception("Unauthorized");
            }

            var currentValue = GetCounter();
            var newValue = currentValue + 1;
            
            Storage.Put(Storage.CurrentContext, new byte[] { COUNTER_PREFIX }, newValue);
            OnCounterIncremented(caller, newValue);
            
            return newValue;
        }

        /// <summary>
        /// Get the current counter value
        /// </summary>
        [DisplayName("getCounter")]
        [Safe]
        public static BigInteger GetCounter()
        {
            var value = Storage.Get(Storage.CurrentContext, new byte[] { COUNTER_PREFIX });
            return value?.Length > 0 ? (BigInteger)value : 0;
        }

        /// <summary>
        /// Multiply two numbers (example of a pure computation method)
        /// </summary>
        [DisplayName("multiply")]
        [Safe]
        public static BigInteger Multiply(BigInteger a, BigInteger b)
        {
            return a * b;
        }

        /// <summary>
        /// Get contract information
        /// </summary>
        [DisplayName("getInfo")]
        [Safe]
        public static Map<string, object> GetInfo()
        {
            var info = new Map<string, object>();
            info["name"] = "DeploymentExample";
            info["version"] = "1.0.0";
            info["owner"] = GetOwner();
            info["counter"] = GetCounter();
            return info;
        }

        /// <summary>
        /// Verify if the contract is valid (used by other contracts)
        /// </summary>
        [DisplayName("verify")]
        [Safe]
        public static bool Verify()
        {
            return Runtime.CheckWitness(GetOwner());
        }

        /// <summary>
        /// Update the contract (only owner can update)
        /// </summary>
        [DisplayName("update")]
        public static bool Update(ByteString nefFile, string manifest, object data)
        {
            if (!Runtime.CheckWitness(GetOwner()))
            {
                throw new Exception("Only owner can update contract");
            }

            ContractManagement.Update(nefFile, manifest, data);
            return true;
        }

        /// <summary>
        /// Destroy the contract (only owner can destroy)
        /// </summary>
        [DisplayName("destroy")]
        public static bool Destroy()
        {
            if (!Runtime.CheckWitness(GetOwner()))
            {
                throw new Exception("Only owner can destroy contract");
            }

            ContractManagement.Destroy();
            return true;
        }
    }
}