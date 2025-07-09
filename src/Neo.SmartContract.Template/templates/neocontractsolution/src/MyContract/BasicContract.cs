using System;
using System.ComponentModel;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace MyContract
{
    [DisplayName("MyContract")]
    [ManifestExtra("Author", "MyContract Author")]
    [ManifestExtra("Email", "developer@mycontract.com")]
    [ManifestExtra("Description", "This is a basic Neo smart contract")]
    [ContractSourceCode("https://github.com/mycompany/mycontract")]
    public class BasicContract : SmartContract
    {
        #if (enableSecurityFeatures)
        // Security: Contract owner for administrative functions
        private static readonly UInt160 Owner = "NYourOwnerAddressHere".ToScriptHash();

        // Security: Multi-sig administrators
        private static readonly UInt160[] Administrators = new UInt160[]
        {
            "NAdminAddress1Here".ToScriptHash(),
            "NAdminAddress2Here".ToScriptHash()
        };

        // Access control modifier
        private static void RequireOwner()
        {
            if (!Runtime.CheckWitness(Owner))
                throw new Exception("Only owner can perform this action");
        }

        private static void RequireAdmin()
        {
            bool isAdmin = false;
            foreach (var admin in Administrators)
            {
                if (Runtime.CheckWitness(admin))
                {
                    isAdmin = true;
                    break;
                }
            }
            
            if (!isAdmin && !Runtime.CheckWitness(Owner))
                throw new Exception("Only administrators can perform this action");
        }
        #endif

        // Contract deployment
        [DisplayName("_deploy")]
        public static void Deploy(object data, bool update)
        {
            if (update) return;
            
            // Initialize contract storage
            Storage.Put(Storage.CurrentContext, "Initialized", 1);
            
            #if (enableSecurityFeatures)
            // Store owner information
            Storage.Put(Storage.CurrentContext, "Owner", Owner);
            #endif
        }

        // Contract update method
        [DisplayName("update")]
        public static void Update(ByteString nefFile, string manifest, object? data = null)
        {
            #if (enableSecurityFeatures)
            RequireOwner();
            #endif
            
            ContractManagement.Update(nefFile, manifest, data);
        }

        // Contract destruction method
        [DisplayName("destroy")]
        public static void Destroy()
        {
            #if (enableSecurityFeatures)
            RequireOwner();
            #endif
            
            ContractManagement.Destroy();
        }

        // Example method: Store data
        [DisplayName("storeData")]
        public static void StoreData(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("Key cannot be empty");
                
            Storage.Put(Storage.CurrentContext, key, value);
            OnDataStored(key, value);
        }

        // Example method: Retrieve data
        [DisplayName("getData")]
        [Safe]
        public static string GetData(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("Key cannot be empty");
                
            return Storage.Get(Storage.CurrentContext, key) ?? "";
        }

        #if (enableSecurityFeatures)
        // Security method: Transfer ownership
        [DisplayName("transferOwnership")]
        public static void TransferOwnership(UInt160 newOwner)
        {
            RequireOwner();
            
            if (!newOwner.IsValid || newOwner.IsZero)
                throw new Exception("Invalid new owner address");
                
            Storage.Put(Storage.CurrentContext, "Owner", newOwner);
            OnOwnershipTransferred(Owner, newOwner);
        }

        // Security method: Add administrator
        [DisplayName("addAdministrator")]
        public static void AddAdministrator(UInt160 admin)
        {
            RequireOwner();
            
            if (!admin.IsValid || admin.IsZero)
                throw new Exception("Invalid administrator address");
                
            // Implementation would store admin in a StorageMap
            OnAdministratorAdded(admin);
        }
        #endif

        // Events
        [DisplayName("DataStored")]
        public static event Action<string, string> OnDataStored;

        #if (enableSecurityFeatures)
        [DisplayName("OwnershipTransferred")]
        public static event Action<UInt160, UInt160> OnOwnershipTransferred;

        [DisplayName("AdministratorAdded")]
        public static event Action<UInt160> OnAdministratorAdded;
        #endif

        // Verification method for contract invocations
        public static bool Verify() => Runtime.CheckWitness(Owner);
    }
}