using System;
using System.ComponentModel;
using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace DevHawk.Contracts
{
    [DisplayName("DevHawk.Registrar")]
    [ManifestExtra("Author", "Harry Pierson")]
    [ManifestExtra("Email", "harrypierson@hotmail.com")]
    [ManifestExtra("Description", "This is an example contract")]
    public class Registrar : SmartContract
    {
        const byte Prefix_DomainOwners = 0x00;
        const byte Prefix_ContractOwner = 0xFF;

        public UInt160 Query(string domain)
        {
            DomainStorage domainOwners = new(Prefix_DomainOwners);
            var currentOwner = domainOwners.Get(domain);
            if (currentOwner.IsZero)
            {
                Runtime.Log("Domain not registered");
            }

            return currentOwner;
        }

        public bool Register(string domain, UInt160 owner)
        {
            DomainStorage domainOwners = new(Prefix_DomainOwners);
            var currentOwner = domainOwners.Get(domain);
            if (!currentOwner.IsZero)
            {
                Runtime.Log("Domain already registered");
                return false;
            }
            if (!Runtime.CheckWitness(owner))
            {
                Runtime.Log("CheckWitness Failed");
                return false;
            }

            domainOwners.Put(domain, owner);
            return true;
        }

        public bool Transfer(string domain, UInt160 to)
        {
            DomainStorage domainOwners = new(Prefix_DomainOwners);
            var currentOwner = domainOwners.Get(domain);
            if (currentOwner.IsZero)
            {
                Runtime.Log("Domain not registered");
                return false;
            }
            if (!Runtime.CheckWitness(currentOwner))
            {
                Runtime.Log("CheckWitness failed for current owner");
                return false;
            }
            if (!Runtime.CheckWitness(to))
            {
                Runtime.Log("CheckWitness failed for receiver");
                return false;
            }

            domainOwners.Put(domain, to);
            return true;
        }

        public bool Delete(string domain)
        {
            DomainStorage domainOwners = new(Prefix_DomainOwners);
            var currentOwner = domainOwners.Get(domain);
            if (currentOwner.IsZero)
            {
                Runtime.Log("Domain not registered");
                return false;
            }
            if (!Runtime.CheckWitness(currentOwner))
            {
                Runtime.Log("CheckWitness failed for current owner");
                return false;
            }

            domainOwners.Delete(domain);
            return true;
        }

        [DisplayName("_deploy")]
        public void Deploy(object data, bool update)
        {
            if (update) return;

            var tx = (Transaction)Runtime.ScriptContainer;
            var key = new byte[] { Prefix_ContractOwner };
            Storage.Put(Storage.CurrentContext, key, tx.Sender);
        }

        public static void Update(ByteString nefFile, string manifest)
        {
            var key = new byte[] { Prefix_ContractOwner };
            var contractOwner = (UInt160)Storage.Get(Storage.CurrentContext, key);
            var tx = (Transaction)Runtime.ScriptContainer;

            if (!contractOwner.Equals(tx.Sender))
            {
                throw new Exception("Only the contract owner can update the contract");
            }

            ContractManagement.Update(nefFile, manifest, null);
        }
    }

    class DomainStorage
    {
        readonly StorageMap storageMap;

        public DomainStorage(byte prefix)
        {
            storageMap = new StorageMap(Storage.CurrentContext, prefix);
        }

        public UInt160 Get(string domain) => (UInt160)storageMap.Get(domain) ?? UInt160.Zero;
        public void Put(string domain, UInt160 owner) => storageMap.Put(domain, (ByteString)owner);
        public void Delete(string domain) => storageMap.Delete(domain);
    }
}
